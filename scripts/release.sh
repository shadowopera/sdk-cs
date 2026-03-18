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

function ensureCleanWorktree() {
    if [[ -n "$(git status --porcelain)" ]]; then
        printError "Working tree is not clean. Please commit or stash your changes."
        exit 1
    fi
}

function markStepDone() {
    GOEXPERIMENT=jsonv2 go run scripts/__impl/release/main.go mark "$1" || exit 1
}

VERSION_ARG="${1:-}"

while true; do
    # Run release.go (pass version arg only on first iteration)
    RESULT=$(GOEXPERIMENT=jsonv2 go run scripts/__impl/release/main.go $VERSION_ARG 2>&1) || {
        printError "$RESULT"
        exit 1
    }
    # Clear version arg after first iteration
    VERSION_ARG=""

    if [[ "$RESULT" == "DONE" ]]; then
        printImportantMessage "Release complete!"
        printMessage "Delete release.json when ready for the next release."
        printImportantMessage "Run 'git push --follow-tags' to publish."
        exit 0
    fi

    NEXT_STEP="$RESULT"
    VERSION=$(GOEXPERIMENT=jsonv2 go run scripts/__impl/release/main.go version 2>&1) || {
        printError "Failed to get version: $VERSION"
        exit 1
    }

    printImportantMessage "Step: $NEXT_STEP (v$VERSION)"

    case "$NEXT_STEP" in
        checkVersion)
            if git tag -l "v$VERSION" | grep -q "v$VERSION"; then
                printError "Git tag v$VERSION already exists."
                exit 1
            fi
            printMessage "Version v$VERSION is available."
            ensureCleanWorktree
            markStepDone "checkVersion"
            ;;

        runTests1)
            printMessage "Running tests..."
            if ! dotnet test; then
                printError "Tests failed."
                exit 1
            fi
            ensureCleanWorktree
            markStepDone "runTests1"
            ;;

        updateChangelog)
            printMessage "Please update CHANGELOG.md manually according to Keep a Changelog 1.1.0 format."
            printMessage "New version number: $VERSION"
            if ! gum confirm --affirmative=OK --negative=Cancel \
                "After updating CHANGELOG.md, click OK to sync and commit changes."; then
                printMessage "Aborted."
                exit 1
            fi

            printMessage "Syncing CHANGELOG.md to Unity package..."
            if ! rsync -av CHANGELOG.md unity/dev.shadop.archmage/CHANGELOG.md; then
                printError "Failed to sync CHANGELOG.md to Unity package."
                exit 1
            fi

            printMessage "Creating Starlight version of CHANGELOG.md..."
            if ! {
                echo "---"
                echo "title: 'Changelog'"
                echo "sidebar:"
                echo "  order: 99"
                echo "---"
                echo ""
                cat CHANGELOG.md
            } > docs/src/content/docs/guides-cs/CHANGELOG.md; then
                printError "Failed to create Starlight CHANGELOG.md."
                exit 1
            fi

            printMessage "Staging CHANGELOG.md files..."
            if ! git add CHANGELOG.md unity/dev.shadop.archmage/CHANGELOG.md docs/src/content/docs/guides-cs/CHANGELOG.md; then
                printError "Failed to stage CHANGELOG.md files."
                exit 1
            fi

            printMessage "Committing changes..."
            if ! git commit -m "docs: update changelog"; then
                printError "Failed to commit CHANGELOG.md changes."
                exit 1
            fi

            ensureCleanWorktree
            markStepDone "updateChangelog"
            ;;

        syncUnity)
            printMessage "Syncing to Unity..."
            if ! bash scripts/rsync-unity.sh; then
                printError "rsync-unity.sh failed."
                exit 1
            fi
            git add unity/dev.shadop.archmage/
            if ! git diff --cached --quiet; then
                git commit -m "chore: sync changes to the unity directory"
            fi
            ensureCleanWorktree
            markStepDone "syncUnity"
            ;;

        bumpVersion)
            printMessage "Running defensive tests..."
            if ! dotnet test; then
                printError "Tests failed."
                exit 1
            fi

            ensureCleanWorktree
            printMessage "Bumping version..."
            if ! bash scripts/bump-version.sh --yes "$VERSION"; then
                printError "bump-version.sh failed."
                exit 1
            fi

            if ! bash scripts/reconcile-unity-meta.sh; then
                printError "reconcile-unity-meta.sh failed."
                exit 1
            fi
            if ! bash scripts/sort-slnx.sh; then
                printError "sort-slnx.sh failed."
                exit 1
            fi

            # Create git tag (skip if already exists)
            if git tag -l "v$VERSION" | grep -q "v$VERSION"; then
                printMessage "Tag v$VERSION already exists, skipping."
            else
                if ! gum confirm "Do you want to create git tag v$VERSION?"; then
                    printMessage "Skipping git tag."
                else
                    git tag -a "v$VERSION" -m "v$VERSION"
                    printImportantMessage "Tag v$VERSION created."
                fi
            fi
            if ! git tag -l "v$VERSION" | grep -q "v$VERSION"; then
                printError "Git tag v$VERSION was not created. Please run: git tag v$VERSION"
                exit 1
            fi

            ensureCleanWorktree
            markStepDone "bumpVersion"
            ;;

        *)
            printError "Unknown step: $NEXT_STEP"
            exit 1
            ;;
    esac
done
