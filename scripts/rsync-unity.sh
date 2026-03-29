#!/usr/bin/env bash

set -euo pipefail
shopt -s nullglob

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SRC_DIR="$ROOT_DIR/src/Archmage"
PKG_DIR="$ROOT_DIR/unity/dev.shadop.archmage"
DST_DIR="$PKG_DIR/Runtime"

mkdir -p "$DST_DIR"

for name in "README.md" "CHANGELOG.md" "LICENSE"; do
    src_file="$ROOT_DIR/$name"
    dst_file="$PKG_DIR/$name"

    if [[ -f "$src_file" ]]; then
        if [[ "$name" == "README.md" ]]; then
            # Skip first two lines
            tmp_file="$(mktemp)"
            tail -n +3 "$src_file" > "$tmp_file"
            if [[ ! -f "$dst_file" ]]; then
                cp "$tmp_file" "$dst_file"
                echo "  + $name"
            elif ! cmp -s "$tmp_file" "$dst_file"; then
                cp "$tmp_file" "$dst_file"
                echo "  ~ $name"
            fi
            rm "$tmp_file"
        else
            if [[ ! -f "$dst_file" ]]; then
                cp "$src_file" "$dst_file"
                echo "  + $name"
            elif ! cmp -s "$src_file" "$dst_file"; then
                cp "$src_file" "$dst_file"
                echo "  ~ $name"
            fi
        fi
    fi
done

# Sync .cs files from src/Archmage to Runtime
CONF_DIR="$ROOT_DIR/unity/ArchmageDev/Assets/Scripts/Conf"
mkdir -p "$CONF_DIR"

rsync -a --delete --exclude="obj/" --exclude="bin/" --include="*/" --include="*.cs" --exclude="*" "$SRC_DIR/" "$DST_DIR/"

# Sync testdata JSON files to Unity config directories (independent of above counters)
TESTDATA_DIR="$ROOT_DIR/tests/testdata"
UNITY_CONFIG_DIRS=(
    "$ROOT_DIR/unity/ArchmageDev/Assets/Configs"
    "$ROOT_DIR/unity/ArchmageDev/Assets/Resources/StaticConfigs"
    "$ROOT_DIR/unity/ArchmageDev/Assets/StreamingAssets/StreamingConfigs"
)

echo ""
echo "Syncing testdata JSON to Unity config directories..."
for config_dir in "${UNITY_CONFIG_DIRS[@]}"; do
    mkdir -p "$config_dir"
    rsync -a --delete --include="*/" --include="*.json" --exclude="*" "$TESTDATA_DIR/" "$config_dir/"
    echo "  synced -> $config_dir"
done

echo ""
"$SCRIPT_DIR/reconcile-unity-meta.sh"
