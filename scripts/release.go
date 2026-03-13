package main

import (
	"encoding/json/jsontext"
	"encoding/json/v2"
	"fmt"
	"os"
	"reflect"
	"regexp"
	"strings"
)

type Release struct {
	Version string `json:"version"`
	Steps   struct {
		CheckVersion    int `json:"checkVersion"`
		RunTests1       int `json:"runTests1"`
		UpdateChangelog int `json:"updateChangelog"`
		SyncUnity       int `json:"syncUnity"`
		RunTests2       int `json:"runTests2"`
		BumpVersion     int `json:"bumpVersion"`
	} `json:"steps"`
}

var semverRe = regexp.MustCompile(`^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?(\+[0-9A-Za-z.-]+)?$`)

const releaseFile = "release.json"

func main() {
	defer func() {
		if r := recover(); r != nil {
			fmt.Fprintln(os.Stderr, r)
			os.Exit(1)
		}
	}()

	if len(os.Args) > 2 {
		panic("too many arguments")
	}

	version := ""
	if len(os.Args) > 1 {
		version = strings.TrimPrefix(strings.TrimPrefix(os.Args[1], "v"), "V")
		if !semverRe.MatchString(version) {
			panic("invalid semver format: " + version)
		}
	}

	if _, err := os.Stat(releaseFile); err != nil {
		if !os.IsNotExist(err) {
			panic(err)
		}
		if version == "" {
			panic("usage: release.sh <version>")
		}
		rel := Release{Version: version}
		data, err := json.Marshal(rel, jsontext.WithIndent("\t"))
		if err != nil {
			panic(err)
		}
		data = append(data, '\n')
		if err := os.WriteFile(releaseFile, data, 0644); err != nil {
			panic(err)
		}
	}

	data, err := os.ReadFile(releaseFile)
	if err != nil {
		panic(err)
	}
	var rel Release
	if err := json.Unmarshal(data, &rel); err != nil {
		panic("failed to parse release.json: " + err.Error())
	}

	if !semverRe.MatchString(rel.Version) {
		panic("invalid semver format in release.json: " + rel.Version)
	}

	if version != "" && version != rel.Version {
		if allDone(&rel) {
			panic("release already completed, please delete release.json first")
		} else {
			panic("version mismatch: release.json has " + rel.Version + ", got " + version)
		}
	}

	validateSteps(&rel)

	if allDone(&rel) {
		fmt.Println("DONE")
	} else {
		fmt.Println(nextStep(&rel))
	}
}

// stepValues returns step values in declaration order.
func stepValues(rel *Release) []int {
	v := reflect.ValueOf(rel.Steps)
	vals := make([]int, v.NumField())
	for i := range vals {
		vals[i] = int(v.Field(i).Int())
	}
	return vals
}

// stepNames returns step JSON field names in declaration order.
func stepNames() []string {
	t := reflect.TypeOf(Release{}.Steps)
	names := make([]string, t.NumField())
	for i := range names {
		names[i] = t.Field(i).Tag.Get("json")
	}
	return names
}

func allDone(rel *Release) bool {
	for _, v := range stepValues(rel) {
		if v != 1 {
			return false
		}
	}
	return true
}

func validateSteps(rel *Release) {
	values := stepValues(rel)
	names := stepNames()
	seenZero := false
	for i, v := range values {
		if v != 0 && v != 1 {
			panic(fmt.Sprintf("invalid step value for %s: %d (must be 0 or 1)", names[i], v))
		}
		if seenZero && v == 1 {
			panic(fmt.Sprintf("invalid step order: %s is done but a previous step is not", names[i]))
		}
		if v == 0 {
			seenZero = true
		}
	}
}

func nextStep(rel *Release) string {
	values := stepValues(rel)
	names := stepNames()
	for i, v := range values {
		if v == 0 {
			return names[i]
		}
	}
	return ""
}
