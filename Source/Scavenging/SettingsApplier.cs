using Verse;

namespace Scavenging;

[StaticConstructorOnStartup]
internal static class SettingsApplier
{
    static SettingsApplier()
    {
        ApplySettings();
    }

    public static void ApplySettings()
    {
        if (ScavengeMod.settings.terrainSettings == null)
        {
            return;
        }

        foreach (var terrainSettings in ScavengeMod.settings.terrainSettings)
        {
            var terrain = DefDatabase<TerrainDef>.GetNamedSilentFail(terrainSettings.Key);

            var extension = terrain?.GetModExtension<TerrainExtension>();
            if (extension == null)
            {
                continue;
            }

            extension.workAmount = terrainSettings.Value.workAmount;
            extension.failChance = terrainSettings.Value.failChance;

            if (terrainSettings.Value.scavengingChance != null)
            {
                foreach (var sc in terrainSettings.Value.scavengingChance)
                {
                    var entry = extension.scavengables.FirstOrDefault(x => x.thingDef == sc.Key);
                    if (entry != null)
                    {
                        entry.chance = sc.Value;
                    }
                }
            }

            if (terrainSettings.Value.spawnAmount == null)
            {
                continue;
            }

            foreach (var sc in terrainSettings.Value.spawnAmount)
            {
                var entry = extension.scavengables.FirstOrDefault(x => x.thingDef == sc.Key);
                if (entry != null)
                {
                    entry.amountToSpawn = sc.Value;
                }
            }
        }
    }
}