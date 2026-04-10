#nullable enable

using UnityEditor;
using System;
using Shadop.Archmage.Sdk.Editor;
using Conf;

[CustomPropertyDrawer(typeof(HeroCfgId))]
public class HeroCfgIdDrawer : IntCfgIdDrawer<HeroCfgId, long>
{
    public static void Initialize(HeroTable tbl, Func<long, string> format) =>
        Initialize("HeroTable", tbl.Keys, id => id.Value, format);
}
