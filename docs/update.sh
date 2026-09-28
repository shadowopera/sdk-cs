#!/usr/bin/env bash

[[ "$TRACE" ]] && set -x
pushd "$(dirname "$0")" > /dev/null
trap __EXIT EXIT

colorful=false
if [[ -t 1 ]] && [[ -n "${TERM:-}" ]]; then
    colorful=true
fi

function __EXIT() {
    [[ -n "${api_tmp:-}" ]] && rm -rf "$api_tmp"
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

# Sync image and prepare guide docs
printMessage "Syncing assets and preparing guide docs ..."
mkdir -p docs/src/assets/archmage/
if ! rsync -av images/archmage.jpg docs/src/assets/archmage/archmage.jpg; then
    printError "rsync images/archmage.jpg failed"
    exit 1
fi

rm -rf docs/src/content/docs/overview-cs/
mkdir -p docs/src/content/docs/overview-cs/

# Process README.md for Starlight
{
    echo "---"
    echo "title: 'C# SDK Overview'"
    echo "sidebar:"
    echo "  label: Overview"
    echo "  order: 1"
    echo "---"
    echo ""
    perl -0777 -pe 's/\n---\s+## Development.*//s' README.md | \
        perl -0777 -pe 's/^# Archmage\n\n//m' | \
        perl -pe 's|\./images/archmage\.jpg|../../../assets/archmage/archmage.jpg|g' | \
        perl -0777 -pe 's/^> \[!NOTE\]\n((?:> [^\n]*\n?)+)/my $b=$1; $b=~s{^> }{}gm; ":::note\n${b}:::\n"/gme'
} > docs/src/content/docs/overview-cs/sdk-cs.mdx

# Process CHANGELOG.md for Starlight
if ! bash scripts/starlight-changelog.sh CHANGELOG.md docs/src/content/docs/overview-cs/CHANGELOG.md; then
    printError "starlight-changelog.sh failed"
    exit 1
fi

# Locate the Unity Editor of ArchmageDev, whose UnityEngine assemblies the Unity API docs build references
unity_project_dir=unity/ArchmageDev
if [[ -z "${UNITY_EDITOR:-}" ]]; then
    unity_version="$(sed -n 's/^m_EditorVersion: //p' "$unity_project_dir/ProjectSettings/ProjectVersion.txt")"
    for candidate in \
        "/Applications/Unity/Hub/Editor/$unity_version/Unity.app/Contents/MacOS/Unity" \
        "$HOME/Unity/Hub/Editor/$unity_version/Editor/Unity"; do
        if [[ -x "$candidate" ]]; then
            UNITY_EDITOR="$candidate"
            break
        fi
    done
    if [[ -z "${UNITY_EDITOR:-}" ]]; then
        printError "Unity $unity_version not found in Unity Hub. Set UNITY_EDITOR to override."
        exit 1
    fi
fi
unity_engine_dir=""
for candidate in \
    "$(dirname "$UNITY_EDITOR")/../Resources/Scripting/Managed/UnityEngine" \
    "$(dirname "$UNITY_EDITOR")/Data/Managed/UnityEngine"; do
    if [[ -f "$candidate/UnityEngine.CoreModule.dll" ]]; then
        unity_engine_dir="$candidate"
        break
    fi
done
if [[ -z "$unity_engine_dir" ]]; then
    printError "UnityEngine assemblies not found next to $UNITY_EDITOR"
    exit 1
fi
script_assemblies_dir="$PWD/$unity_project_dir/Library/ScriptAssemblies"
if [[ ! -f "$script_assemblies_dir/Unity.Addressables.dll" ]]; then
    printError "$script_assemblies_dir/Unity.Addressables.dll not found. Open $unity_project_dir in the Unity Editor once."
    exit 1
fi

godot_sharp_version="$(sed -n 's/.*<PackageReference Include="GodotSharp" Version="\([^"]*\)".*/\1/p' \
    src/Archmage/Sdk/Godot/Archmage.Godot.csproj)"
if [[ -z "$godot_sharp_version" ]]; then
    printError "GodotSharp version not found in Archmage.Godot.csproj"
    exit 1
fi

# Clean previous generated API docs
printMessage "Cleaning generated API docs ..."
rm -rf docs/src/content/docs/sdk-cs/ docs/src/content/docs/sdk-cs-unity/ docs/src/content/docs/sdk-cs-godot/

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

# Generate the Unity and Godot pages. Each docs project compiles the core sources too, so that doc comments can link
# to core types; only the pages that the core docs lack are kept.
api_tmp="$(mktemp -d)"

function generatePlatformDocs() {
    local name="$1" csproj="$2" out_dir="$3"
    shift 3
    printMessage "Building the $name API docs assembly ..."
    if ! dotnet build "$csproj" -o "$api_tmp/$name-bin" "$@"; then
        printError "dotnet build $csproj failed"
        exit 1
    fi
    printMessage "Generating $name API docs with xmldoc2md ..."
    if ! xmldoc2md "$api_tmp/$name-bin/$(basename "$csproj" .csproj).dll" -o "$api_tmp/$name-md"; then
        printError "xmldoc2md failed for $name"
        exit 1
    fi
    mkdir -p "$out_dir"
    local page
    for page in "$api_tmp/$name-md"/*.md; do
        [[ "$(basename "$page")" == index.md ]] && continue
        [[ -e "docs/src/content/docs/sdk-cs/$(basename "$page")" ]] && continue
        cp "$page" "$out_dir/"
    done
}

generatePlatformDocs unity docs/utils/api-unity/ArchmageUnityDocs.csproj docs/src/content/docs/sdk-cs-unity \
    -p:UnityEngineDir="$unity_engine_dir" -p:ScriptAssembliesDir="$script_assemblies_dir"
generatePlatformDocs godot docs/utils/api-godot/ArchmageGodotDocs.csproj docs/src/content/docs/sdk-cs-godot \
    -p:GodotSharpVersion="$godot_sharp_version"

# Drop the JSON converter and type converter pages, which are noise for readers
find docs/src/content/docs/sdk-cs docs/src/content/docs/sdk-cs-unity docs/src/content/docs/sdk-cs-godot \
    \( -name '*jsonconverter*.md' -o -name '*typeconverter*.md' \) -delete

# Post-process the generated docs
printMessage "Post-processing API docs ..."
cd docs
if ! node utils/fix-api-docs.mjs; then
    printError "fix-api-docs.mjs failed"
    exit 1
fi
cd ..

# Sync generated docs to the main docs site
printMessage "Syncing assets ..."
if ! rsync -av --delete docs/src/assets/archmage/ ../docs/archmage/src/assets/archmage/; then
    printError "rsync assets failed"
    exit 1
fi

printMessage "Syncing overview-cs ..."
if ! rsync -av --delete docs/src/content/docs/overview-cs/ ../docs/archmage/src/content/docs/overview-cs/; then
    printError "rsync overview-cs failed"
    exit 1
fi

printMessage "Syncing gen-cs ..."
if ! rsync -av --delete docs/src/content/docs/gen-cs/ ../docs/archmage/src/content/docs/gen-cs/; then
    printError "rsync gen-cs failed"
    exit 1
fi

printMessage "Syncing sdk-cs ..."
if ! rsync -av --delete docs/src/content/docs/sdk-cs/ ../docs/archmage/src/content/docs/sdk-cs/; then
    printError "rsync sdk-cs failed"
    exit 1
fi

printMessage "Syncing sdk-cs-unity ..."
if ! rsync -av --delete docs/src/content/docs/sdk-cs-unity/ ../docs/archmage/src/content/docs/sdk-cs-unity/; then
    printError "rsync sdk-cs-unity failed"
    exit 1
fi

printMessage "Syncing sdk-cs-unity-editor ..."
if ! rsync -av --delete docs/src/content/docs/sdk-cs-unity-editor/ ../docs/archmage/src/content/docs/sdk-cs-unity-editor/; then
    printError "rsync sdk-cs-unity-editor failed"
    exit 1
fi

printMessage "Syncing sdk-cs-godot ..."
if ! rsync -av --delete docs/src/content/docs/sdk-cs-godot/ ../docs/archmage/src/content/docs/sdk-cs-godot/; then
    printError "rsync sdk-cs-godot failed"
    exit 1
fi

# Stage all changes
printMessage "Staging changes in docs site ..."
cd ../docs
git add -A

echo
printMessage "Done."
