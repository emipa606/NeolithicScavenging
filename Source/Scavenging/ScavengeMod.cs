using Mlie;
using UnityEngine;
using Verse;

namespace Scavenging;

internal class ScavengeMod : Mod
{
    public static ScavengeSettings Settings;
    public static string CurrentVersion;

    public ScavengeMod(ModContentPack pack) : base(pack)
    {
        Settings = GetSettings<ScavengeSettings>();
        CurrentVersion =
            VersionFromManifest.GetVersionFromModMetaData(pack.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoSettingsWindowContents(inRect);
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