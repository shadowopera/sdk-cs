#!/usr/bin/env bash

# Builds the ArchmageDev WebGL probe and runs each probe case in headless Chrome.
#
# Usage: scripts/webgl-probe.sh [--no-build] [<case>...]
#   --no-build  reuse the existing build in unity/ArchmageDev/Builds/WebGLProbe
#   <case>      cases to run (default: all); see Assets/WebGLProbe/WebGLProbe.cs
#
# The Unity Editor must be closed. Set UNITY_EDITOR to override the Unity Hub lookup,
# and CHROME to override the Chrome executable.

[[ "$TRACE" ]] && set -x
set -o pipefail
cd "$(dirname "$0")/.."

PROJECT_DIR="$(pwd)/unity/ArchmageDev"
BUILD_DIR="$PROJECT_DIR/Builds/WebGLProbe"
LOG_FILE="unity/ArchmageDev/Logs/webgl-probe-build.log"
# Files the player build modifies (URP shader prefiltering, per-target settings, Addressables
# content state); they are restored after the build.
BUILD_SIDE_EFFECTS=(Assets/Settings ProjectSettings/ProjectSettings.asset)
ALL_CASES=(taskrun taskrun-wait sync-resources async-resources async-streaming async-addressables)

build=true
cases=()
for arg in "$@"; do
    case "$arg" in
        --no-build) build=false ;;
        *) cases+=("$arg") ;;
    esac
done
[[ ${#cases[@]} -eq 0 ]] && cases=("${ALL_CASES[@]}")

if $build; then
    if [[ -z "${UNITY_EDITOR:-}" ]]; then
        version="$(sed -n 's/^m_EditorVersion: //p' "$PROJECT_DIR/ProjectSettings/ProjectVersion.txt")"
        UNITY_EDITOR="/Applications/Unity/Hub/Editor/$version/Unity.app/Contents/MacOS/Unity"
    fi
    lockfile="$PROJECT_DIR/Temp/UnityLockfile"
    if [[ -f "$lockfile" ]] && lsof "$lockfile" > /dev/null 2>&1; then
        echo "ERROR: ArchmageDev is open in the Unity Editor. Close it and retry." >&2
        exit 1
    fi
    if [[ -n "$(git -C "$PROJECT_DIR" status --porcelain -- "${BUILD_SIDE_EFFECTS[@]}")" ]]; then
        echo "ERROR: Commit or discard the changes in ${BUILD_SIDE_EFFECTS[*]}; the build overwrites them." >&2
        exit 1
    fi
    scripts/rsync-unity.sh > /dev/null || exit 1
    mkdir -p "$(dirname "$LOG_FILE")"
    echo "Building WebGL probe (log: $LOG_FILE)..." >&2
    if ! "$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$PROJECT_DIR" -buildTarget WebGL \
        -executeMethod ArchmageDev.Editor.WebGLProbeBuild.Build -logFile "$(pwd)/$LOG_FILE"; then
        build_failed=true
    fi
    git -C "$PROJECT_DIR" checkout -- "${BUILD_SIDE_EFFECTS[@]}"
    rm -rf "$PROJECT_DIR/Assets/AddressableAssetsData/WebGL" "$PROJECT_DIR/Assets/AddressableAssetsData/WebGL.meta" \
        "$PROJECT_DIR/Assets/WebGLProbe/WebGLProbe.unity" "$PROJECT_DIR/Assets/WebGLProbe/WebGLProbe.unity.meta"
    if ${build_failed:-false}; then
        echo "ERROR: WebGL build failed. See $LOG_FILE." >&2
        exit 1
    fi
fi

for c in "${cases[@]}"; do
    echo
    echo "=== $c"
    echo
    node scripts/webgl-probe.mjs "$BUILD_DIR" "$c" 60
    echo "(exit $?)"
done
