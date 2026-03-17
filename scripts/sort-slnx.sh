#!/usr/bin/env bash

[[ "$TRACE" ]] && set -x

colorful=false
if [[ -t 1 ]] && [[ -n "${TERM:-}" ]]; then
    colorful=true
fi

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

# Validate input
if [[ $# -eq 0 ]]; then
    printError "Usage: $0 <file.slnx>"
    exit 1
fi

file="$1"

if [[ ! -f "$file" ]]; then
    printError "File not found: $file"
    exit 1
fi

if [[ ! "$file" =~ \.slnx$ ]]; then
    printError "File is not a .slnx file: $file"
    exit 1
fi

printMessage "Sorting projects in $file..."

# Get script directory and run Go program from there
dir="$(cd "$(dirname "$0")" && pwd)"
if ! go run "$dir/__impl/sort-slnx.go" "$file"; then
    printError "Failed to sort $file"
    exit 1
fi

printImportantMessage "✓ $file sorted successfully"
