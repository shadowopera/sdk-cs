package main

import (
	"bytes"
	"encoding/xml"
	"flag"
	"fmt"
	"log"
	"os"
	"sort"
)

type Solution struct {
	Name     xml.Name  `xml:"Solution"`
	Projects []Project `xml:"Project"`
}

type Project struct {
	Path string `xml:"Path,attr"`
}

func main() {
	flag.Parse()
	args := flag.Args()

	if len(args) != 1 {
		fmt.Fprintf(os.Stderr, "Usage: sort-slnx <file.slnx>\n")
		os.Exit(1)
	}

	filePath := args[0]

	// Read file
	data, err := os.ReadFile(filePath)
	if err != nil {
		log.Fatalf("Error reading file: %v", err)
	}

	// Parse XML
	var solution Solution
	if err := xml.Unmarshal(data, &solution); err != nil {
		log.Fatalf("Error parsing XML: %v", err)
	}

	// Sort projects by Path attribute
	sort.Slice(solution.Projects, func(i, j int) bool {
		return solution.Projects[i].Path < solution.Projects[j].Path
	})

	// Marshal back to XML with proper indentation
	var buf bytes.Buffer
	encoder := xml.NewEncoder(&buf)
	encoder.Indent("", "  ")
	if err := encoder.Encode(solution); err != nil {
		log.Fatalf("Error marshaling XML: %v", err)
	}

	// Write back to file with CRLF line endings
	output := buf.Bytes()
	// Normalize line endings to LF first
	output = bytes.ReplaceAll(output, []byte("\r\n"), []byte("\n"))
	// Ensure final newline
	if !bytes.HasSuffix(output, []byte("\n")) {
		output = append(output, '\n')
	}
	// Convert LF to CRLF
	output = bytes.ReplaceAll(output, []byte("\n"), []byte("\r\n"))

	if err := os.WriteFile(filePath, output, 0644); err != nil {
		log.Fatalf("Error writing file: %v", err)
	}

	fmt.Printf("Sorted %d projects in %s\n", len(solution.Projects), filePath)
}
