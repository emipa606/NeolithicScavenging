using Mlie;
using UnityEngine;
using Verse;

namespace Scavenging;

internal class ScavengeMod : Mod
{
    public static ScavengeSettings settings;
    public static string currentVersion;

    public ScavengeMod(ModContentPack pack) : base(pack)
    {
        settings = GetSettings<ScavengeSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(pack.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoSettingsWindowContents(inRect);
    }

    public override void WriteSettings()
    {
        base.WriteSettings();
        SettingsApplier.ApplySettings();
    }

    public override string SettingsCategory()
    {
        return "Neolithic Scavenging";
    }
}