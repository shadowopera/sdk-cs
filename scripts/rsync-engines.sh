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
GODOT_ASSET_DIR="$ROOT_DIR/godot/ArchmageDev/addons/archmage"

mkdir -p "$DST_DIR1"
mkdir -p "$DST_DIR2"
mkdir -p "$GODOT_ASSET_DIR"

# Copy README.md, CHANGELOG.md and LICENSE to the Unity package, and README.md and LICENSE to the Godot asset
echo "Copying README.md, CHANGELOG.md and LICENSE to the Unity package and the Godot asset..."
for target in "$PKG_ROOT:README.md" "$PKG_ROOT:CHANGELOG.md" "$PKG_ROOT:LICENSE" \
    "$GODOT_ASSET_DIR:README.md" "$GODOT_ASSET_DIR:LICENSE"; do
    name="${target##*:}"
    src_file="$ROOT_DIR/$name"
    dst_file="${target%:*}/$name"

    if [[ -f "$src_file" ]]; then
        if [[ "$name" == "README.md" ]]; then
            # Skip the first two lines (the image) and everything from "## Development" on
            tmp_file="$(mktemp)"
            tail -n +3 "$src_file" | perl -0777 -pe 's/\n---\s+## Development.*//s' > "$tmp_file"
            if [[ ! -f "$dst_file" ]]; then
                cp "$tmp_file" "$dst_file"
                echo "  + $dst_file"
            elif ! cmp -s "$tmp_file" "$dst_file"; then
                cp "$tmp_file" "$dst_file"
                echo "  ~ $dst_file"
            fi
            rm "$tmp_file"
        else
            if [[ ! -f "$dst_file" ]]; then
                cp "$src_file" "$dst_file"
                echo "  + $dst_file"
            elif ! cmp -s "$src_file" "$dst_file"; then
                cp "$src_file" "$dst_file"
                echo "  ~ $dst_file"
            fi
        fi
    fi
done

# Sync .cs files from src/Archmage to Runtime
CONF_DIR="$ROOT_DIR/unity/ArchmageDev/Assets/Scripts/Conf"
mkdir -p "$CONF_DIR"

echo ""
echo "Syncing .cs files to the Unity package..."
rsync -a --delete --exclude="obj/" --exclude="bin/" --exclude="Godot/" --include="*/" --include="*.cs" --exclude="*" "$SRC_DIR1/" "$DST_DIR1/"
rsync -a --delete --exclude="obj/" --exclude="bin/" --include="*/" --include="*.cs" --exclude="*" "$SRC_DIR2/" "$DST_DIR2/"

# Sync the .cs files of the core SDK and the Godot adapters to the Godot asset. The exclude rules keep the .uid files
# that Godot generates on import, so remove the .uid files whose .cs file is gone.
echo ""
echo "Syncing .cs files to the Godot asset..."
rsync -a --delete --exclude="obj/" --exclude="bin/" --exclude="Unity/" --include="*/" --include="*.cs" --exclude="*" \
    "$SRC_DIR1/" "$GODOT_ASSET_DIR/"
while IFS= read -r uid_file; do
    if [[ ! -f "${uid_file%.uid}" ]]; then
        rm "$uid_file"
        echo "  - $uid_file"
    fi
done < <(find "$GODOT_ASSET_DIR" -name "*.cs.uid")

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
