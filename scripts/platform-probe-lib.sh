# Shared by scripts/webgl-probe.sh and scripts/android-probe.sh; source it from the repository root.

PROJECT_DIR="$(pwd)/unity/ArchmageDev"
BUILD_DIR="$PROJECT_DIR/Builds/PlatformProbe"
# Files the player build modifies (URP shader prefiltering, per-target settings, Addressables
# content state); they are restored after the build.
BUILD_SIDE_EFFECTS=(Assets/Settings ProjectSettings/ProjectSettings.asset ProjectSettings/UnityConnectSettings.asset)
BUILD_GENERATED=(Assets/PlatformProbe/PlatformProbe.unity Assets/PlatformProbe/PlatformProbe.unity.meta)

if [[ -z "${UNITY_EDITOR:-}" ]]; then
    UNITY_EDITOR="/Applications/Unity/Hub/Editor/$(sed -n 's/^m_EditorVersion: //p' \
        "$PROJECT_DIR/ProjectSettings/ProjectVersion.txt")/Unity.app/Contents/MacOS/Unity"
fi

# buildProbe <build target> <PlatformProbeBuild method>
function buildProbe() {
    local target="$1" method="$2"
    local log_file="unity/ArchmageDev/Logs/platform-probe-$method.log"

    local lockfile="$PROJECT_DIR/Temp/UnityLockfile"
    if [[ -f "$lockfile" ]] && lsof "$lockfile" > /dev/null 2>&1; then
        echo "ERROR: ArchmageDev is open in the Unity Editor. Close it and retry." >&2
        return 1
    fi
    if [[ -n "$(git -C "$PROJECT_DIR" status --porcelain -- "${BUILD_SIDE_EFFECTS[@]}")" ]]; then
        echo "ERROR: Commit or discard the changes in ${BUILD_SIDE_EFFECTS[*]}; the build overwrites them." >&2
        return 1
    fi
    scripts/rsync-unity.sh > /dev/null || return 1
    mkdir -p "$(dirname "$log_file")"
    echo "Building $method (log: $log_file)..." >&2

    local failed=false
    "$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$PROJECT_DIR" -buildTarget "$target" \
        -executeMethod "ArchmageDev.Editor.PlatformProbeBuild.$method" -logFile "$(pwd)/$log_file" || failed=true

    git -C "$PROJECT_DIR" checkout -- "${BUILD_SIDE_EFFECTS[@]}"
    local f
    for f in "${BUILD_GENERATED[@]}"; do
        rm -f "$PROJECT_DIR/$f"
    done
    rm -rf "$PROJECT_DIR/Assets/AddressableAssetsData/$target" "$PROJECT_DIR/Assets/AddressableAssetsData/$target.meta"

    if $failed; then
        echo "ERROR: $method failed. See $log_file." >&2
        return 1
    fi
}
