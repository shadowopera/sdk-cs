#!/usr/bin/env bash

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

function printImportantMessage() {
    $colorful && tput setaf 3 || true
    local timestamp=$([ -n "${WTS:-}" ] && [ "${WTS}" != "0" ] && date +'[%Y-%m-%d %H:%M:%S] ')
    >&2 echo "${timestamp}$*"
    $colorful && tput sgr0 || true
}

# Move to project root
cd ..

# 1) Parse arguments
AUTO_YES=false
while [[ $# -gt 0 ]]; do
    case "$1" in
        --yes)
            AUTO_YES=true
            shift
            ;;
        *)
            break
            ;;
    esac
done

# 2) Get and clean version number
RAW_VERSION=${1:-}
if [[ -z "$RAW_VERSION" ]]; then
    printError "Missing version argument."
    echo "Usage: ./scripts/bump-version.sh [--yes] <version>"
    exit 1
fi

NEW_VERSION=$(echo "$RAW_VERSION" | sed 's/^[vV]//')

# 3) Validate semver with perl
if ! echo "$NEW_VERSION" | perl -ne 'exit 1 unless /^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?(\+[0-9A-Za-z.-]+)?$/'; then
    printError "Invalid semver format: $NEW_VERSION"
    exit 1
fi

# 4) Confirm with gum (skip if --yes)
if ! $AUTO_YES; then
    if ! gum confirm "Bump version to $NEW_VERSION?"; then
        printMessage "Aborted."
        exit 0
    fi
fi

# 5) Update files
CSPROJ="src/Archmage/Archmage.csproj"
PKG_JSON="unity/dev.shadop.archmage/package.json"

printImportantMessage "Updating $CSPROJ..."
sed -i '' "s|<Version>.*</Version>|<Version>$NEW_VERSION</Version>|" "$CSPROJ"

# Verify csproj update
if ! grep -q "<Version>$NEW_VERSION</Version>" "$CSPROJ"; then
    printError "Failed to update version in $CSPROJ"
    exit 1
fi

printImportantMessage "Updating $PKG_JSON..."
sed -i '' "s/\"version\": \".*\"/\"version\": \"$NEW_VERSION\"/" "$PKG_JSON"

# Verify package.json update
if ! grep -q "\"version\": \"$NEW_VERSION\"" "$PKG_JSON"; then
    printError "Failed to update version in $PKG_JSON"
    exit 1
fi

# 6) Commit changes (skip if nothing to commit)
git add "$CSPROJ" "$PKG_JSON"
if ! git diff --cached --quiet; then
    printImportantMessage "Committing changes..."
    git commit -m "chore: bump version to $NEW_VERSION"
    echo
    git show --stat HEAD
    echo
else
    printMessage "Version already up to date, skipping commit."
fi

# 7) Create git tag (skip if already exists)
if git tag -l "v$NEW_VERSION" | grep -q "v$NEW_VERSION"; then
    printMessage "Tag v$NEW_VERSION already exists, skipping."
else
    if ! $AUTO_YES; then
        if ! gum confirm "Do you want to create git tag v$NEW_VERSION?"; then
            printMessage "Skipping git tag."
            printImportantMessage "Done."
            exit 0
        fi
    fi
    git tag -a "v$NEW_VERSION" -m "v$NEW_VERSION"
    printImportantMessage "Tag v$NEW_VERSION created."
fi

printImportantMessage "Done."
