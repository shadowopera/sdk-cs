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

# Move to project root
cd ..

# Clean previous generated API docs
printMessage "Cleaning docs/src/content/docs/sdk-cs/ ..."
rm -rf docs/src/content/docs/sdk-cs/

# Build the library
printMessage "Building Archmage ..."
if ! dotnet build src/Archmage/Archmage.csproj; then
    printError "dotnet build failed"
    exit 1
fi

# Generate markdown docs from the built DLL
printMessage "Generating API docs with xmldoc2md ..."
if ! xmldoc2md src/Archmage/bin/Debug/netstandard2.1/Archmage.dll -o docs/src/content/docs/sdk-cs/; then
    printError "xmldoc2md failed"
    exit 1
fi

# Remove the generated index.md (conflicts with Starlight's own index)
rm -f docs/src/content/docs/sdk-cs/index.md

# Copy manually maintained docs into the generated directory
printMessage "Copying manual docs from sdk-cs.manual/ ..."
cp docs/src/content/docs/sdk-cs.manual/*.md docs/src/content/docs/sdk-cs/

# Post-process the generated docs
printMessage "Post-processing API docs ..."
cd docs
if ! node fix-api-docs.mjs; then
    printError "fix-api-docs.mjs failed"
    exit 1
fi

# Sync generated docs to the main docs site
printMessage "Syncing sdk-cs docs to ../docs/src/content/docs/sdk-cs/ ..."
if ! rsync -av --delete src/content/docs/sdk-cs/ ../../docs/src/content/docs/sdk-cs/; then
    printError "rsync failed"
    exit 1
fi

echo
printMessage "Done."
