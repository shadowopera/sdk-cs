#!/usr/bin/env bash

[[ "$TRACE" ]] && set -x
pushd `dirname "$0"` > /dev/null
trap __EXIT EXIT

colorful=false
if [[ -t 1 ]]; then
    colorful=true
fi

function __EXIT() {
    popd > /dev/null
}

function printMessage() {
    >&2 echo "[$(date +'%Y-%m-%d %H:%M:%S')] $*"
}

function printError() {
    $colorful && tput setaf 1
    local timestamp=$([ -n "$WTS" ] && [ "$WTS" != "0" ] && date +'[%Y-%m-%d %H:%M:%S] ')
    >&2 echo "${timestamp}ERROR: $*"
    $colorful && tput setaf 7
}

function printImportantMessage() {
    $colorful && tput setaf 3
    local timestamp=$([ -n "$WTS" ] && [ "$WTS" != "0" ] && date +'[%Y-%m-%d %H:%M:%S] ')
    >&2 echo "${timestamp}$*"
    $colorful && tput setaf 7
}

# Move to project root
cd ..

# 1) Get and clean version number
RAW_VERSION=$1
if [[ -z "$RAW_VERSION" ]]; then
    printError "Missing version argument."
    echo "Usage: ./scripts/bump-version.sh <version>"
    exit 1
fi

NEW_VERSION=$(echo "$RAW_VERSION" | sed 's/^[vV]//')

# 2) Validate semver with perl
if ! echo "$NEW_VERSION" | perl -ne 'exit 1 unless /^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?(\+[0-9A-Za-z.-]+)?$/'; then
    printError "Invalid semver format: $NEW_VERSION"
    exit 1
fi

# 3) Confirm with gum
if ! gum confirm "Bump version to $NEW_VERSION?"; then
    printMessage "Aborted."
    exit 0
fi

# 4) Update files
CSPROJ="src/Archmage/Archmage.csproj"
PKG_JSON="unity/dev.shadop.archmage/package.json"

printImportantMessage "Updating $CSPROJ..."
sed -i '' "s|<Version>.*</Version>|<Version>$NEW_VERSION</Version>|" "$CSPROJ"

printImportantMessage "Updating $PKG_JSON..."
sed -i '' "s/\"version\": \".*\"/\"version\": \"$NEW_VERSION\"/" "$PKG_JSON"

# 5) Commit changes
printImportantMessage "Committing changes..."
git add "$CSPROJ" "$PKG_JSON"
git commit -m "chore: bump version to $NEW_VERSION"

# 6) Show the commit
echo
git show --stat HEAD
echo

# 7) Ask for git tag
if ! gum confirm "Do you want to create git tag v$NEW_VERSION?"; then
    printMessage "Skipping git tag."
    printImportantMessage "Done."
    exit 0
fi

git tag "v$NEW_VERSION"
printImportantMessage "Tag v$NEW_VERSION created."

printImportantMessage "Done."
