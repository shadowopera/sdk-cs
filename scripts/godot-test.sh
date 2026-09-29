#!/usr/bin/env bash

# Runs the Godot ArchmageDev tests on the Godot main thread.
#
# Usage: scripts/godot-test.sh [--no-sync] [--filter <regex>]
#   --no-sync         skip scripts/rsync-engines.sh
#   --filter <regex>  run only the tests whose Class.Method matches (e.g. OverrideTests)
#
# The log and a summary go to godot/ArchmageDev/logs/godot-path.log and godot-path-summary.txt.
# The exit code is the exit code of Godot: 0 when all tests pass.
#
# Set GODOT to the Godot .NET executable to override /Applications/Godot_mono.app.

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

PROJECT_DIR="$(pwd)/godot/ArchmageDev"
LOG_DIR="$PROJECT_DIR/logs"

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
            echo "Usage: ./scripts/godot-test.sh [--no-sync] [--filter <regex>]"
            exit 1
            ;;
    esac
done

mode=path
LOG_FILE="$LOG_DIR/godot-$mode.log"
SUMMARY_FILE="$LOG_DIR/godot-$mode-summary.txt"

# 2) Locate Godot
GODOT="${GODOT:-/Applications/Godot_mono.app/Contents/MacOS/godot}"
if [[ ! -x "$GODOT" ]]; then
    printError "Godot not found at $GODOT. Set GODOT to the Godot .NET executable."
    exit 1
fi

# 3) Sync test data
if $sync; then
    if ! scripts/rsync-engines.sh; then
        printError "rsync-engines.sh failed."
        exit 1
    fi
    echo
fi

# 4) Build, then import. Importing before the build reports the C# scripts as not compiling.
printImportantMessage "Building ArchmageDev..."
if ! dotnet build "$PROJECT_DIR/ArchmageDev.csproj"; then
    printError "dotnet build failed."
    exit 1
fi

printImportantMessage "Importing resources..."
if ! "$GODOT" --headless --path "$PROJECT_DIR" --import > /dev/null 2>&1; then
    printError "Godot failed to import the project. Run: $GODOT --headless --path $PROJECT_DIR --import"
    exit 1
fi

# 5) Run tests. The runner scene is passed as the scene to run, so the main scene (the demo) does not start.
mkdir -p "$LOG_DIR"
rm -f "$SUMMARY_FILE"

user_args=(--summary "$SUMMARY_FILE")
if [[ -n "$filter" ]]; then
    user_args+=(--filter "$filter")
fi

printImportantMessage "Running tests (log: $LOG_FILE)..."
"$GODOT" --headless --path "$PROJECT_DIR" res://tests/test_runner.tscn -- "${user_args[@]}" 2>&1 | tee "$LOG_FILE"
status=${PIPESTATUS[0]}

if [[ ! -f "$SUMMARY_FILE" ]]; then
    printError "Godot exited with $status and wrote no summary. See $LOG_FILE."
    exit 1
fi

printMessage "Summary: $SUMMARY_FILE"
exit "$status"
