#!/usr/bin/env bash

# Runs the ArchmageDev PlayMode tests in batch mode.
#
# Usage: scripts/unity-test.sh [--no-sync] [--filter <expr>]
#   --no-sync        skip scripts/rsync-unity.sh
#   --filter <expr>  passed to Unity's -testFilter (e.g. ConfLoaderTests.Resources)
#
# Set UNITY_EDITOR to the Unity executable to override the Unity Hub lookup.

[[ "$TRACE" ]] && set -x
pushd "$(dirname "$0")" > /dev/null
trap __EXIT EXIT

colorful=false
if [[ -t 1 ]] && [[ -n "${TERM:-}" ]]; then
    colorful=true
fi

function __EXIT() {
    popd > /dev/null
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
RESULTS_FILE="unity/ArchmageDev/Logs/playmode-results.xml"
LOG_FILE="unity/ArchmageDev/Logs/playmode-tests.log"

# 1) Parse arguments
sync=true
filter=""
while [[ $# -gt 0 ]]; do
    case "$1" in
        --no-sync)
            sync=false
            shift
            ;;
        --filter)
            filter="$2"
            shift 2
            ;;
        *)
            printError "Unknown argument: $1"
            echo "Usage: ./scripts/unity-test.sh [--no-sync] [--filter <expr>]"
            exit 1
            ;;
    esac
done

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

# 4) Run tests
mkdir -p "$(dirname "$RESULTS_FILE")"
rm -f "$RESULTS_FILE"

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

# 5) Report results
function xpath() {
    xmllint --xpath "$1" "$RESULTS_FILE" 2> /dev/null || true
}

echo
printImportantMessage "total=$(xpath 'string(/test-run/@total)')" \
    "passed=$(xpath 'string(/test-run/@passed)')" \
    "failed=$(xpath 'string(/test-run/@failed)')" \
    "skipped=$(xpath 'string(/test-run/@skipped)')"

failed="$(xpath '//test-case[@result="Failed"]/@fullname' | grep -o 'fullname="[^"]*"' | sed 's/^fullname="//; s/"$//')"
if [[ -n "$failed" ]]; then
    echo
    printError "Failed tests:"
    while IFS= read -r name; do
        echo "  - $name"
        xpath "string(//test-case[@fullname=\"$name\"]/failure/message)" | sed 's/^/      /'
    done <<< "$failed"
    echo
    printMessage "Details: $RESULTS_FILE"
fi

exit "$status"
