#!/usr/bin/env bash

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

# This script syncs directories from the Go SDK.
# It ensures that configuration data and test cases are consistent across SDKs.

set -e

# Source path relative to this script.
# sdk-go is assumed to be a sibling of sdk-cs.
SRC="../../sdk-go/internal"

if [ ! -d "$SRC" ]; then
    echo "Error: Source directory not found at $SRC"
    echo "Ensure that sdk-go is a sibling directory to sdk-cs."
    exit 1
fi

# Sync testdata
printImportantMessage "Syncing testdata..."
rsync -av --delete "$SRC/testdata/" "../tests/testdata/"

# Sync override
echo
printImportantMessage "Syncing override..."
rsync -av --delete "$SRC/override/" "../tests/override/"

echo
printImportantMessage "Sync complete."
