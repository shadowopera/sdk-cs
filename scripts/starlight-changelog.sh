#!/usr/bin/env bash
# Usage: scripts/starlight-changelog.sh <input> <output>

INPUT="$1"
OUTPUT="$2"

{
    echo "---"
    echo "title: 'Changelog'"
    echo "sidebar:"
    echo "  order: 99"
    echo "---"
    echo ""
    tail -n +3 "$INPUT"
} > "$OUTPUT"
