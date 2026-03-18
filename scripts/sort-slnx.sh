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

# List of .slnx files to sort
FILES=(
    "../unity/ArchmageDev/ArchmageDev.slnx"
)

for file in "${FILES[@]}"; do
    if [[ ! -f "$file" ]]; then
        printError "File not found: $file"
        continue
    fi

    if [[ ! "$file" =~ \.slnx$ ]]; then
        printError "File is not a .slnx file: $file"
        continue
    fi

    printMessage "Sorting projects in $file..."

    # Get script directory and run Go program from there
    if ! go run "__impl/sort-slnx/main.go" "$file"; then
        printError "Failed to sort $file"
        exit 1
    fi

    printImportantMessage "✓ $file"
done
