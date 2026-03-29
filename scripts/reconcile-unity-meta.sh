#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
UNITY_RUNTIME_DIR="$ROOT_DIR/unity/dev.shadop.archmage/Runtime"

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
while IFS= read -r -d '' cs_file; do
    meta_file="${cs_file}.meta"
    if [[ ! -f "$meta_file" ]]; then
        printError "Missing .meta file for ${cs_file#$UNITY_RUNTIME_DIR/}"
        errs=$((errs + 1))
    fi
done < <(find "$UNITY_RUNTIME_DIR" -name "*.cs" -print0)

# 2. Check if every .meta file has a corresponding source file or directory
while IFS= read -r -d '' meta_file; do
    src_file="${meta_file%.meta}"
    if [[ ! -f "$src_file" && ! -d "$src_file" ]]; then
        rm "$meta_file"
        echo "Deleted orphaned .meta file: ${meta_file#$UNITY_RUNTIME_DIR/}"
        deleted=$((deleted + 1))
    fi
done < <(find "$UNITY_RUNTIME_DIR" -name "*.meta" -print0)

echo ""
echo "Check complete: $errs errs found, $deleted orphaned .meta files deleted."

if [[ $errs -gt 0 ]]; then
    exit 1
fi
