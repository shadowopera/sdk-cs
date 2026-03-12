#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SRC_DIR="$ROOT_DIR/src/Archmage"
PKG_DIR="$ROOT_DIR/unity/dev.shadop.archmage"
DST_DIR="$PKG_DIR/Runtime"

added=0
updated=0
removed=0

mkdir -p "$DST_DIR"

# Sync README.md and LICENSE
for name in "README.md" "LICENSE"; do
    src_file="$ROOT_DIR/$name"
    dst_file="$PKG_DIR/$name"

    if [[ -f "$src_file" ]]; then
        if [[ ! -f "$dst_file" ]]; then
            cp "$src_file" "$dst_file"
            echo "  + $name"
            added=$((added + 1))
        elif ! cmp -s "$src_file" "$dst_file"; then
            cp "$src_file" "$dst_file"
            echo "  ~ $name"
            updated=$((updated + 1))
        fi
    fi
done

# Sync .cs files from src to unity/dev.shadop.archmage/Runtime
for src_file in "$SRC_DIR"/*.cs; do
    filename="$(basename "$src_file")"
    dst_file="$DST_DIR/$filename"

    if [[ ! -f "$dst_file" ]]; then
        cp "$src_file" "$dst_file"
        echo "  + $filename"
        added=$((added + 1))
    elif ! cmp -s "$src_file" "$dst_file"; then
        cp "$src_file" "$dst_file"
        echo "  ~ $filename"
        updated=$((updated + 1))
    fi
done

# Remove .cs files in unity/dev.shadop.archmage/Runtime that no longer exist in src
for dst_file in "$DST_DIR"/*.cs; do
    [[ -f "$dst_file" ]] || continue
    filename="$(basename "$dst_file")"
    if [[ ! -f "$SRC_DIR/$filename" ]]; then
        rm "$dst_file"
        echo "  - $filename"
        removed=$((removed + 1))
        # Note: .meta files are left intact for manual cleanup
    fi
done

echo ""
echo "Sync complete: $added added, $updated updated, $removed removed"

echo ""
"$SCRIPT_DIR/check-unity-meta.sh"
