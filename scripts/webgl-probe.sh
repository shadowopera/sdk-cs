#!/usr/bin/env bash

# Builds the ArchmageDev platform probe for WebGL and runs each probe case in headless Chrome.
#
# Usage: scripts/webgl-probe.sh [--no-build] [<case>...]
#   --no-build  reuse the existing build in unity/ArchmageDev/Builds/PlatformProbe/WebGL
#   <case>      cases to run (default: all); see Assets/PlatformProbe/PlatformProbe.cs
#
# The Unity Editor must be closed. Set UNITY_EDITOR to override the Unity Hub lookup,
# and CHROME to override the Chrome executable.

[[ "$TRACE" ]] && set -x
set -o pipefail
cd "$(dirname "$0")/.."
source scripts/platform-probe-lib.sh

ALL_CASES=(taskrun taskrun-wait sync-resources async-resources async-streaming async-addressables
    $ADDR_READ_CASES)

build=true
cases=()
for arg in "$@"; do
    case "$arg" in
        --no-build) build=false ;;
        *) cases+=("$arg") ;;
    esac
done
[[ ${#cases[@]} -eq 0 ]] && cases=("${ALL_CASES[@]}")

if $build; then
    buildProbe WebGL BuildWebGL || exit 1
fi

for c in "${cases[@]}"; do
    echo
    echo "=== $c"
    echo
    node scripts/webgl-probe.mjs "$BUILD_DIR/WebGL" "$c" 60
    echo "(exit $?)"
done
