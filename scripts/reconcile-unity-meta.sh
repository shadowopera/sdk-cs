#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
UNITY_RUNTIME_DIR="$ROOT_DIR/unity/dev.shadop.archmage/Runtime"
OPENUPM_PACKAGE_JSON="$ROOT_DIR/unity/dev.shadop.archmage/package.json"
VERSION="$(grep '"version"' "$OPENUPM_PACKAGE_JSON" | sed 's/.*"version": *"\([^"]*\)".*/\1/')"
SAMPLES_SRC_DIR="$ROOT_DIR/unity/dev.shadop.archmage/Samples~/Integration"
SAMPLES_ARCHMAGE_DIR="$ROOT_DIR/unity/ArchmageDev/Assets/Samples/Archmage"
SAMPLES_ARCHMAGE_DST_DIR="$SAMPLES_ARCHMAGE_DIR/$VERSION/Integration"

colorful=false
if [[ -t 1 ]]; then
    colorful=true
fi

function printError() {
    $colorful && tput setaf 1
    >&2 echo "ERROR: $*"
    $colorful && tput setaf 7
}

errs=0
deleted=0

echo "Checking .cs and .meta files in unity/dev.shadop.archmage/Runtime..."

# 1. Check if every .cs file has a corresponding .meta file
for cs_file in "$UNITY_RUNTIME_DIR"/*.cs; do
    [[ -f "$cs_file" ]] || continue
    meta_file="${cs_file}.meta"
    if [[ ! -f "$meta_file" ]]; then
        printError "Missing .meta file for $(basename "$cs_file")"
        errs=$((errs + 1))
    fi
done

# 2. Check if every .meta file has a corresponding source file
for meta_file in "$UNITY_RUNTIME_DIR"/*.meta; do
    [[ -f "$meta_file" ]] || continue
    src_file="${meta_file%.meta}"
    if [[ ! -f "$src_file" && ! -d "$src_file" ]]; then
        rm "$meta_file"
        echo "Deleted orphaned .meta file: $(basename "$meta_file")"
        deleted=$((deleted + 1))
    fi
done

# 3. ArchmageDev/Assets/Samples/Archmage/ must has exactly one subdirectory named $VERSION
echo ""
echo "Checking unity/ArchmageDev/Assets/Samples/Archmage/ has exactly one subdirectory named $VERSION..."

mapfile -t version_dirs < <(find "$SAMPLES_ARCHMAGE_DIR" -mindepth 1 -maxdepth 1 -type d)
if [[ ${#version_dirs[@]} -ne 1 ]]; then
    printError "Expected exactly 1 subdirectory in Samples/Archmage/, found ${#version_dirs[@]}: ${version_dirs[*]}"
    errs=$((errs + 1))
elif [[ "$(basename "${version_dirs[0]}")" != "$VERSION" ]]; then
    printError "Subdirectory name '$(basename "${version_dirs[0]}")' does not match version '$VERSION'"
    errs=$((errs + 1))
fi

# 4. Samples~/Integration *.cs files must match ArchmageDev installed samples
echo ""
echo "Checking Samples~/Integration *.cs files match unity/ArchmageDev/Assets/Samples/Archmage/$VERSION/Integration..."

if [[ ! -d "$SAMPLES_ARCHMAGE_DST_DIR" ]]; then
    printError "Dev samples directory not found: $SAMPLES_ARCHMAGE_DST_DIR"
    errs=$((errs + 1))
else
    for cs_file in "$SAMPLES_SRC_DIR"/*.cs; do
        [[ -f "$cs_file" ]] || continue
        name="$(basename "$cs_file")"
        dev_file="$SAMPLES_ARCHMAGE_DST_DIR/$name"
        if [[ ! -f "$dev_file" ]]; then
            printError "Missing in dev samples: $name"
            errs=$((errs + 1))
        elif ! diff -q "$cs_file" "$dev_file" > /dev/null 2>&1; then
            printError "File differs from dev samples: $name"
            errs=$((errs + 1))
        fi
    done

    for cs_file in "$SAMPLES_ARCHMAGE_DST_DIR"/*.cs; do
        [[ -f "$cs_file" ]] || continue
        name="$(basename "$cs_file")"
        src_file="$SAMPLES_SRC_DIR/$name"
        if [[ ! -f "$src_file" ]]; then
            printError "Extra file in dev samples (not in Samples~): $name"
            errs=$((errs + 1))
        fi
    done
fi

echo ""
echo "Check complete: $errs errs found, $deleted orphaned .meta files deleted."

if [[ $errs -gt 0 ]]; then
    exit 1
fi
