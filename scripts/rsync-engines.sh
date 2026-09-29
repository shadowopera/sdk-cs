#!/usr/bin/env bash

set -euo pipefail
shopt -s nullglob

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
PKG_ROOT="$ROOT_DIR/unity/dev.shadop.archmage"
SRC_DIR1="$ROOT_DIR/src/Archmage/Sdk"
DST_DIR1="$PKG_ROOT/Runtime"
SRC_DIR2="$ROOT_DIR/src/Archmage/Editor"
DST_DIR2="$PKG_ROOT/Editor"

mkdir -p "$DST_DIR1"
mkdir -p "$DST_DIR2"

for name in "README.md" "CHANGELOG.md" "LICENSE"; do
    src_file="$ROOT_DIR/$name"
    dst_file="$PKG_ROOT/$name"

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

rsync -a --delete --exclude="obj/" --exclude="bin/" --exclude="Godot/" --include="*/" --include="*.cs" --exclude="*" "$SRC_DIR1/" "$DST_DIR1/"
rsync -a --delete --exclude="obj/" --exclude="bin/" --include="*/" --include="*.cs" --exclude="*" "$SRC_DIR2/" "$DST_DIR2/"

# Sync testdata JSON files to Unity and Godot config directories (independent of above counters)
TESTDATA_DIR="$ROOT_DIR/tests/testdata"
CONFIG_DIRS=(
    "$ROOT_DIR/unity/ArchmageDev/Assets/Configs"
    "$ROOT_DIR/unity/ArchmageDev/Assets/Resources/StaticConfigs"
    "$ROOT_DIR/unity/ArchmageDev/Assets/StreamingAssets/StreamingConfigs"
    "$ROOT_DIR/godot/ArchmageDev/configs"
)

echo ""
echo "Syncing testdata JSON to Unity and Godot config directories..."
for config_dir in "${CONFIG_DIRS[@]}"; do
    mkdir -p "$config_dir"
    rsync -a --delete --include="*/" --include="*.json" --exclude="*" "$TESTDATA_DIR/" "$config_dir/"
    echo "- $config_dir"
done

# Sync override JSON files to Unity and Godot override directories
OVERRIDE_DIR="$ROOT_DIR/tests/override"
OVERRIDE_DIRS=(
    "$ROOT_DIR/unity/ArchmageDev/Assets/ConfigOverrides"
    "$ROOT_DIR/unity/ArchmageDev/Assets/Resources/StaticConfigOverrides"
    "$ROOT_DIR/unity/ArchmageDev/Assets/StreamingAssets/StreamingConfigOverrides"
    "$ROOT_DIR/godot/ArchmageDev/config_overrides"
)

echo ""
echo "Syncing override JSON to Unity and Godot override directories..."
for override_dir in "${OVERRIDE_DIRS[@]}"; do
    mkdir -p "$override_dir"
    rsync -a --delete --include="*/" --include="*.json" --exclude="*" "$OVERRIDE_DIR/" "$override_dir/"
    echo "- $override_dir"
done

echo ""
"$SCRIPT_DIR/reconcile-unity-meta.sh"
