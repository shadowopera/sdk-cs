#!/usr/bin/env bash

# Packs godot/ArchmageDev/addons/archmage/ into builds/archmage-sdk-cs-godot-<version>.zip for upload to the Godot Asset Store.
# The zip contains addons/archmage/ with the .uid files. The version is read from Archmage.Godot.csproj.
#
# Usage: scripts/pack-godot-asset.sh [--no-sync]
#   --no-sync  skip scripts/rsync-engines.sh

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

# Move to project root
cd ..

PROJECT_DIR="$(pwd)/godot/ArchmageDev"
OUT_DIR="$(pwd)/builds"

# 1) Parse arguments
sync=true
while [[ $# -gt 0 ]]; do
    case "$1" in
        --no-sync)
            sync=false
            shift
            ;;
        *)
            printError "Unknown argument: $1"
            echo "Usage: ./scripts/pack-godot-asset.sh [--no-sync]"
            exit 1
            ;;
    esac
done

# 2) Sync the SDK sources to the asset
if $sync; then
    scripts/rsync-engines.sh > /dev/null || exit 1
fi

# 3) Check that every .cs file has its .uid file; Godot generates them on import
missing=$(cd "$PROJECT_DIR" && find addons/archmage -name "*.cs" | while IFS= read -r f; do
    [[ -f "$f.uid" ]] || echo "$f"
done)
if [[ -n "$missing" ]]; then
    printError "Missing .uid files. Run scripts/godot-test.sh, which imports the project, and commit the .uid files:"
    >&2 echo "$missing"
    exit 1
fi

# 4) Pack
version=$(perl -ne 'print $1 if /<Version>(.+?)<\/Version>/' src/Archmage/Sdk/Godot/Archmage.Godot.csproj)
if [[ -z "$version" ]]; then
    printError "Version not found in Archmage.Godot.csproj"
    exit 1
fi

mkdir -p "$OUT_DIR"
zip_file="$OUT_DIR/archmage-sdk-cs-godot-$version.zip"
rm -f "$zip_file"
(cd "$PROJECT_DIR" && zip -qrX "$zip_file" addons/archmage -x "*.DS_Store") || exit 1

printMessage "Created $zip_file"
