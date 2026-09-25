#!/usr/bin/env bash

# Runs the ArchmageDev PlayMode tests in batch mode.
#
# Usage: scripts/unity-test.sh [--no-sync] [--packed] [--filter <expr>]
#   --no-sync        skip scripts/rsync-unity.sh
#   --packed         build Addressables content and load it from bundles ("Use Existing Build");
#                    the developer's play mode script is restored afterwards
#   --filter <expr>  passed to Unity's -testFilter (e.g. ConfLoaderTests.Resources)
#
# Logs, results and a readable summary go to unity/ArchmageDev/Logs/playmode-<mode>.*
# (mode: assetdb or packed).
#
# Set UNITY_EDITOR to the Unity executable to override the Unity Hub lookup.

[[ "$TRACE" ]] && set -x
pushd "$(dirname "$0")" > /dev/null
packed_entered=false
trap __EXIT EXIT

colorful=false
if [[ -t 1 ]] && [[ -n "${TERM:-}" ]]; then
    colorful=true
fi

function __EXIT() {
    local status=$?
    if $packed_entered; then
        echo
        restorePlayModeScript || status=1
    fi
    popd > /dev/null
    exit "$status"
}

function printMessage() {
    local timestamp=$([ -n "${WTS:-}" ] && [ "${WTS}" != "0" ] && date +'[%Y-%m-%d %H:%M:%S] ')
    >&2 echo "${timestamp}$*"
}

function printError() {
    $colorful && tput setaf 1 || true
    local timestamp=$([ -n "${WTS:-}" ] && [ "${WTS}" != "0" ] && date +'[%Y-%m-%d %H:%M:%S] ')
    >&2 echo "${timestamp}ERROR: $*"
    $colorful && tput sgr0 || true
}

function printImportantMessage() {
    $colorful && tput setaf 3 || true
    local timestamp=$([ -n "${WTS:-}" ] && [ "${WTS}" != "0" ] && date +'[%Y-%m-%d %H:%M:%S] ')
    >&2 echo "${timestamp}$*"
    $colorful && tput sgr0 || true
}

# Move to project root
cd ..

PROJECT_DIR="$(pwd)/unity/ArchmageDev"
LOG_DIR="unity/ArchmageDev/Logs"
ENTER_LOG_FILE="$LOG_DIR/packed-enter.log"
EXIT_LOG_FILE="$LOG_DIR/packed-exit.log"
# Written by PackedPlayMode.Enter and removed by PackedPlayMode.Exit; it holds the developer's
# play mode script while a packed run is in progress.
SAVED_INDEX_FILE="$PROJECT_DIR/Library/ArchmagePlayModeIndex.txt"

# 1) Parse arguments
sync=true
packed=false
filter=""
while [[ $# -gt 0 ]]; do
    case "$1" in
        --no-sync)
            sync=false
            shift
            ;;
        --packed)
            packed=true
            shift
            ;;
        --filter)
            filter="$2"
            shift 2
            ;;
        *)
            printError "Unknown argument: $1"
            echo "Usage: ./scripts/unity-test.sh [--no-sync] [--packed] [--filter <expr>]"
            exit 1
            ;;
    esac
done

mode=assetdb
$packed && mode=packed
RESULTS_FILE="$LOG_DIR/playmode-$mode-results.xml"
LOG_FILE="$LOG_DIR/playmode-$mode.log"
SUMMARY_FILE="$LOG_DIR/playmode-$mode-summary.txt"

# 2) Locate the Unity Editor
if [[ -z "${UNITY_EDITOR:-}" ]]; then
    version="$(sed -n 's/^m_EditorVersion: //p' "$PROJECT_DIR/ProjectSettings/ProjectVersion.txt")"
    for candidate in \
        "/Applications/Unity/Hub/Editor/$version/Unity.app/Contents/MacOS/Unity" \
        "$HOME/Unity/Hub/Editor/$version/Editor/Unity"; do
        if [[ -x "$candidate" ]]; then
            UNITY_EDITOR="$candidate"
            break
        fi
    done
    if [[ -z "${UNITY_EDITOR:-}" ]]; then
        printError "Unity $version not found in Unity Hub. Set UNITY_EDITOR to override."
        exit 1
    fi
fi

lockfile="$PROJECT_DIR/Temp/UnityLockfile"
if [[ -f "$lockfile" ]] && lsof "$lockfile" > /dev/null 2>&1; then
    printError "ArchmageDev is open in the Unity Editor. Close it and retry."
    exit 1
fi

# 3) Sync sources and test data
if $sync; then
    if ! scripts/rsync-unity.sh; then
        printError "rsync-unity.sh failed."
        exit 1
    fi
    echo
fi

mkdir -p "$LOG_DIR"
rm -f "$RESULTS_FILE" "$SUMMARY_FILE"

# 4) Restore the play mode script left by an interrupted packed run, then build Addressables
#    content and switch to "Use Existing Build"
# runEditorMethod <method> <log file>
function runEditorMethod() {
    "$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$PROJECT_DIR" \
        -executeMethod "ArchmageDev.Editor.PackedPlayMode.$1" -logFile "$(pwd)/$2"
}

function restorePlayModeScript() {
    printImportantMessage "Restoring the Addressables play mode script..."
    if ! runEditorMethod Exit "$EXIT_LOG_FILE"; then
        printError "Failed to restore the Addressables play mode script. See $EXIT_LOG_FILE."
        return 1
    fi
}

if [[ -f "$SAVED_INDEX_FILE" ]]; then
    printImportantMessage "Found the play mode script saved by an interrupted packed run."
    restorePlayModeScript || exit 1
fi

if $packed; then
    printImportantMessage "Addressables: building content (log: $ENTER_LOG_FILE)..."
    # Set before running: a failed Enter may have switched the script already.
    packed_entered=true
    if ! runEditorMethod Enter "$ENTER_LOG_FILE"; then
        printError "Failed to build Addressables content. See $ENTER_LOG_FILE."
        exit 1
    fi
fi

# 5) Run tests

args=(-batchmode -nographics -projectPath "$PROJECT_DIR"
    -runTests -testPlatform PlayMode
    -testResults "$(pwd)/$RESULTS_FILE" -logFile "$(pwd)/$LOG_FILE")
if [[ -n "$filter" ]]; then
    args+=(-testFilter "$filter")
fi

printImportantMessage "Running PlayMode tests (log: $LOG_FILE)..."
status=0
"$UNITY_EDITOR" "${args[@]}" || status=$?

if [[ ! -f "$RESULTS_FILE" ]]; then
    printError "Unity exited with $status and produced no test results. See $LOG_FILE."
    exit 1
fi

# 6) Report results
echo
scripts/unity-test-summary.sh report "$mode" "$RESULTS_FILE" "$LOG_FILE" | tee "$SUMMARY_FILE"
echo
printMessage "Summary: $SUMMARY_FILE"

exit "$status"
