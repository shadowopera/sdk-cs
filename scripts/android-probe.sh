#!/usr/bin/env bash

# Builds the ArchmageDev platform probe for Android, installs it on the connected device or emulator,
# and runs each probe case, printing its [Probe] log lines.
#
# Usage: scripts/android-probe.sh [--no-build] [--format apk|aab|aab-split] [<case>...]
#   --no-build  reuse the existing build in unity/ArchmageDev/Builds/PlatformProbe
#   --format    apk (default); aab; aab-split (Split Application Binary: StreamingAssets go into a
#               Play Asset Delivery install-time pack)
#   <case>      cases to run (default: all); see Assets/PlatformProbe/PlatformProbe.cs
#
# The Unity Editor must be closed. Set UNITY_EDITOR to override the Unity Hub lookup, and ANDROID_HOME
# to the SDK that provides adb (default: ~/Library/Android/sdk).

[[ "$TRACE" ]] && set -x
set -o pipefail
cd "$(dirname "$0")/.."
source scripts/platform-probe-lib.sh

ALL_CASES=(taskrun sync-resources async-resources async-streaming async-addressables
    streaming-read streaming-override streaming-override-missing)
PACKAGE=com.UnityTechnologies.com.unity.template.urpblank
TIMEOUT=60

ANDROID_HOME="${ANDROID_HOME:-$HOME/Library/Android/sdk}"
ADB="$ANDROID_HOME/platform-tools/adb"

build=true
format=apk
cases=()
while [[ $# -gt 0 ]]; do
    case "$1" in
        --no-build) build=false; shift ;;
        --format) format="$2"; shift 2 ;;
        *) cases+=("$1"); shift ;;
    esac
done
[[ ${#cases[@]} -eq 0 ]] && cases=("${ALL_CASES[@]}")

case "$format" in
    apk) method=BuildApk; artifact="$BUILD_DIR/probe.apk" ;;
    aab) method=BuildAab; artifact="$BUILD_DIR/probe.aab" ;;
    aab-split) method=BuildAabSplit; artifact="$BUILD_DIR/probe-split.aab" ;;
    *) echo "ERROR: Unknown format: $format" >&2; exit 1 ;;
esac

if [[ "$("$ADB" get-state 2>/dev/null)" != device ]]; then
    echo "ERROR: No Android device or emulator is connected." >&2
    exit 1
fi

if $build; then
    buildProbe Android "$method" || exit 1
fi

# Install
"$ADB" uninstall "$PACKAGE" > /dev/null 2>&1
if [[ "$format" == apk ]]; then
    "$ADB" install "$artifact" > /dev/null || exit 1
else
    player_dir="$(dirname "$UNITY_EDITOR")/../../../PlaybackEngines/AndroidPlayer"
    java="$player_dir/OpenJDK/bin/java"
    bundletool="$(ls "$player_dir"/Tools/bundletool-all-*.jar | head -1)"
    # bundletool signs with the debug keystore; create it if this machine has none.
    if [[ ! -f "$HOME/.android/debug.keystore" ]]; then
        mkdir -p "$HOME/.android"
        "$player_dir/OpenJDK/bin/keytool" -genkeypair -keystore "$HOME/.android/debug.keystore" \
            -storepass android -keypass android -alias androiddebugkey -keyalg RSA -validity 10000 \
            -dname "CN=Android Debug,O=Android,C=US" > /dev/null 2>&1 || exit 1
    fi
    apks="${artifact%.aab}.apks"
    rm -f "$apks"
    PATH="$ANDROID_HOME/platform-tools:$PATH" "$java" -jar "$bundletool" build-apks --bundle="$artifact" \
        --output="$apks" --connected-device --adb="$ADB" > /dev/null || exit 1
    "$java" -jar "$bundletool" install-apks --apks="$apks" --adb="$ADB" > /dev/null || exit 1
fi

# The first immersive (full-screen) launch shows a confirmation that takes focus and stalls the player.
"$ADB" shell settings put secure immersive_mode_confirmations confirmed
activity="$("$ADB" shell cmd package resolve-activity --brief "$PACKAGE" | tail -1 | tr -d '\r')"

for c in "${cases[@]}"; do
    echo
    echo "=== $c ($format)"
    echo
    "$ADB" shell am force-stop "$PACKAGE"
    "$ADB" logcat -c
    "$ADB" shell am start -n "$activity" -e case "$c" > /dev/null
    status=2
    for ((i = 0; i < TIMEOUT; i++)); do
        sleep 1
        if "$ADB" logcat -d -s Unity:V | grep -q '\[Probe\].* DONE'; then
            status=0
            break
        fi
    done
    "$ADB" logcat -d -s Unity:V | grep -o '\[Probe\].*'
    [[ $status -ne 0 ]] && echo "[harness] timeout after ${TIMEOUT}s"
    echo "(exit $status)"
done
"$ADB" shell am force-stop "$PACKAGE"
