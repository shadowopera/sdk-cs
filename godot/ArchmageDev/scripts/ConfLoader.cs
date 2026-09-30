using System;
using System.Threading.Tasks;
using Conf;
using Conf.Enums;
using Godot;
using Shadop.Archmage.Sdk;

public partial class ConfLoader : Node
{
    // The Archmage editor plugin shows each of these properties in the Inspector as a dropdown of the IDs in its
    // table. See addons/archmage/ArchmageEditorPlugin.cs for details. Use long[] or string[] for a list, because
    // Godot drops the hint string of a Godot.Collections.Array<T>.
    [ExportCategory("Easy Config ID Selection")]
    [Export(PropertyHint.None, "HeroCfgId")]
    public long Hero { get; set; } = 2;

    [Export(PropertyHint.None, "RaceCfgId")]
    public string Race { get; set; } = "Elf";

    [Export(PropertyHint.TypeString, "2/0:HeroCfgId")]
    public long[] Heroes { get; set; } = Array.Empty<long>();

    [Export(PropertyHint.TypeString, "4/0:RaceCfgId")]
    public string[] Races { get; set; } = new[] { "Human" };

    public override async void _Ready()
    {
        // 1. Set the root directory for configuration loading.
        string cfgRoot = "res://configs";
        string atlasFile = "res://configs/atlas.json";

        // 2. Initialize the Atlas instance.
        var atlas = new ConfigAtlas();

        // 3. Configure loading options.
        var options = new AtlasOptions()
            .WithLogger(new GodotAtlasLogger())
            .WithJsonSettings(GodotJsonSettingsFactory.Create())
            .WithFS(new GodotFileAccessFS())
            .WithVariant("balance", "hard");

        try
        {
            // workerThreadLoading runs the load on the thread pool, so the main thread keeps rendering frames.
            // The code after await runs on the main thread again.
            GD.Print("[ConfLoader] Starting async config loading...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, atlas, options, workerThreadLoading: true);
            GD.Print("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new GodotFileAccessFS(), cfgRoot);
            ShowAtlasBasicFeatures(atlas);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    static async Task InitI18nAsync(IFS fs, string cfgRoot)
    {
        var en = "en";
        var fr = "fr";
        var i18n = new I18n(en);
        await i18n.MergeL10nFileAsync($"{cfgRoot}/l10n.json", en, fs);
        await i18n.MergeL10nFileAsync($"{cfgRoot}/l10n.fr.json", fr, fs);
        L10n.GetI18n = () => i18n;
        L10n.GetPreferredLanguage = () => "fr";
    }

    static void ShowAtlasBasicFeatures(ConfigAtlas atlas)
    {
        // IMPORTANT: Set the global ConfigAtlas.
        ConfigAtlas.Instance = atlas;

        // 1. Look up a config entry by ID from a dictionary-based table.
        var cfgId = new HeroCfgId(2);
        atlas.HeroTable.TryLookup(cfgId, out var hero);
        GD.Print($"[ConfLoader] HeroTable[2]: Level={hero!.Level}");

        // 2. Do the same, but in a more convenient way.
        GD.Print($"[ConfLoader] HeroTable[2]: Level={cfgId.Cfg.Level} (shortcut)");

        // 3. Access a cross-table reference via XRef.Ref.
        GD.Print($"[ConfLoader] HeroTable[2].Weapon.CfgId: {hero.Weapon.CfgId}");
        GD.Print($"[ConfLoader] HeroTable[2].Weapon.Ref.Name: {hero.Weapon.Ref!.Name}");
        GD.Print($"[ConfLoader] HeroTable[2].Race.Ref.Birthplace: {hero.Race.Ref!.Birthplace.Text}");

        // 4. Query localized text via L10n.
        GD.Print($"[ConfLoader] HeroTable[1].Name (en, not translated): {atlas.HeroTable[1].Name.Text}");
        GD.Print($"[ConfLoader] GameCfg.Title (fr, translated): {atlas.GameCfg.Title.Text}");

        // 5. Read enums, including bitflags and localized enum items.
        GD.Print($"[ConfLoader] HeroTable[2].Class: {hero.Class}");
        GD.Print($"[ConfLoader] HeroTable[2].Elements (bitflags): {hero.Elements}");
        GD.Print($"[ConfLoader] HeroClass.Warrior (fr, translated): {new L10n(HeroClass.Warrior.GetL10nKey()).Text}");

        // 6. Show Godot built-in vectors.
        GD.Print($"[ConfLoader] HeroTable[1].SpawnPos (Vector3I): {atlas.HeroTable[1].SpawnPos}");
        GD.Print($"[ConfLoader] HeroTable[1].Facing (Vector2): {atlas.HeroTable[1].Facing}");
        GD.Print($"[ConfLoader] GameCfg.GridSize (Vector2I): {atlas.GameCfg.GridSize}");
        GD.Print($"[ConfLoader] GameCfg.CameraOffset (Vector3): {atlas.GameCfg.CameraOffset}");
        GD.Print($"[ConfLoader] GameCfg.Tint (Vector4): {atlas.GameCfg.Tint}");

        // 7. Convert Rgba to Godot Color.
        GD.Print($"[ConfLoader] GameCfg.BgColor (Rgba): {atlas.GameCfg.BgColor.ToColor()}");

        // 8. Read a variant item. WithVariant above selects the "hard" variant of balance.
        GD.Print($"[ConfLoader] BalanceCfg.HpScale (hard): {atlas.BalanceCfg.HpScale}");
    }
}
