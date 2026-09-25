#!/usr/bin/env bash

# Runs the .NET tests, then the ArchmageDev PlayMode tests twice: against the
# Asset Database, then against built Addressables bundles (--packed).
# Arguments are passed through to scripts/unity-test.sh.

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

printImportantMessage "Running .NET tests..."
echo
if ! dotnet test tests/Archmage.Tests.csproj; then
    printError ".NET tests failed."
    exit 1
fi

echo
printImportantMessage "Running Unity PlayMode tests..."
assetdb_ok=true
scripts/unity-test.sh "$@" || assetdb_ok=false

echo
printImportantMessage "Running Unity PlayMode tests with packed Addressables..."
packed_ok=true
scripts/unity-test.sh --no-sync --packed "$@" || packed_ok=false

LOG_DIR="unity/ArchmageDev/Logs"
if [[ -f "$LOG_DIR/playmode-assetdb-results.xml" && -f "$LOG_DIR/playmode-packed-results.xml" ]]; then
    echo
    scripts/unity-test-summary.sh compare "$LOG_DIR/playmode-assetdb-results.xml" "$LOG_DIR/playmode-packed-results.xml"
fi

if ! $assetdb_ok; then
    echo
    printError "Unity PlayMode tests failed."
fi
if ! $packed_ok; then
    echo
    printError "Unity PlayMode tests with packed Addressables failed."
fi
if ! $assetdb_ok || ! $packed_ok; then
    exit 1
fi

echo
printImportantMessage "All tests passed."
