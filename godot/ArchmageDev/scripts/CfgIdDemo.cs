using Godot;

// Shows config ID properties in the Inspector. The Archmage editor plugin shows each one as a dropdown of the IDs in
// its table. Use long[] or string[] for a list, because Godot drops the hint string of a Godot.Collections.Array<T>.
public partial class CfgIdDemo : Node
{
    [Export(PropertyHint.None, "HeroCfgId")] public long Hero { get; set; }
    [Export(PropertyHint.None, "RaceCfgId")] public string Race { get; set; } = string.Empty;
    [Export(PropertyHint.TypeString, "2/0:HeroCfgId")] public long[] Heroes { get; set; } = System.Array.Empty<long>();
    [Export(PropertyHint.TypeString, "4/0:RaceCfgId")] public string[] Races { get; set; } = System.Array.Empty<string>();
}
