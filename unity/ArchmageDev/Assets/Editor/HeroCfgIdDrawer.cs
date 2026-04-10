#nullable enable

using UnityEditor;
using System;
using Shadop.Archmage.Editor;
using Conf;

[CustomPropertyDrawer(typeof(HeroCfgId))]
public class HeroCfgIdDrawer : CfgIdDrawer<long>
{
    static long[]? _idValues;
    static string[]? _displayNames;

    protected override long[] GetIdValues() => _idValues ?? Array.Empty<long>();
    protected override string[] GetDisplayNames() => _displayNames ?? Array.Empty<string>();

    public static void Initialize(HeroTable tbl, Func<long, string> format)
    {
        int i = 1;
        _idValues = tbl.Count > 0 ? new long[tbl.Count + 1] : Array.Empty<long>();
        foreach (var id in tbl.Keys) _idValues[i++] = id.Value;
        Array.Sort(_idValues);
        _displayNames = BuildDisplayNames(_idValues, format, 1);
        if (_displayNames.Length > 0)
            _displayNames[0] = "0 (Default)";
    }
}
