#!/usr/bin/env bash
# Usage: scripts/starlight-changelog.sh <input> <output>

INPUT="$1"
OUTPUT="$2"

{
    echo "---"
    echo "title: 'C# SDK Changelog'"
    echo "sidebar:"
    echo "  label: Changelog"
    echo "  order: 99"
    echo "---"
    echo ""
    tail -n +3 "$INPUT"
} > "$OUTPUT"
