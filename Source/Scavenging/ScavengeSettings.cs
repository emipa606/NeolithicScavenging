using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Scavenging;

internal class ScavengeSettings : ModSettings
{
    private static Vector2 scrollPosition = Vector2.zero;
    private List<TerrainSettings> biomeSettingsValues;
    public List<string> disabledTerrains = [];

    private List<string> stringKeys;
    public Dictionary<string, TerrainSettings> terrainSettings;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref disabledTerrains, "disabledTerrains", LookMode.Value);
        Scribe_Collections.Look(ref terrainSettings, "biomeSettings", LookMode.Value, LookMode.Deep, ref stringKeys,
            ref biomeSettingsValues);

        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        if (disabledTerrains is null)
        {
            disabledTerrains = [];
        }

        if (terrainSettings is null)
        {
            terrainSettings = new Dictionary<string, TerrainSettings>();
        }
    }

    public void DoSettingsWindowContents(Rect inRect)
    {
        var terrainDefs = DefDatabase<TerrainDef>.AllDefs.Where(x => x.HasModExtension<TerrainExtension>()).ToList();
        var scrollHeight = GetScrollHeight(terrainDefs);
        var rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
        var rect2 = new Rect(0f, 0f, rect.width - 60f, scrollHeight);
        Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect2);
        if (ScavengeMod.currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("SC.ModVersion".Translate(ScavengeMod.currentVersion));
            GUI.contentColor = Color.white;
        }

        foreach (var terrain in terrainDefs)
        {
            listingStandard.GapLine();
            if (terrainSettings is null)
            {
                terrainSettings = new Dictionary<string, TerrainSettings>();
            }

            if (!terrainSettings.ContainsKey(terrain.defName))
            {
                terrainSettings[terrain.defName] = new TerrainSettings();
            }

            var biomeSettings = terrainSettings[terrain.defName];
            if (disabledTerrains is null)
            {
                disabledTerrains = [];
            }

            var disabled = disabledTerrains.Contains(terrain.defName);
            listingStandard.CheckboxLabeled("SC.Disable".Translate() + " " + terrain.label, ref disabled);
            switch (disabled)
            {
                case false when disabledTerrains.Contains(terrain.defName):
                    disabledTerrains.Remove(terrain.defName);
                    break;
                case true when !disabledTerrains.Contains(terrain.defName):
                    disabledTerrains.Add(terrain.defName);
                    break;
            }

            if (disabled)
            {
                continue;
            }

            var extension = terrain.GetModExtension<TerrainExtension>();
            biomeSettings.workAmount = extension.workAmount;
            listingStandard.SliderLabeled("SC.BaseWorkAmount".Translate(), ref biomeSettings.workAmount,
                biomeSettings.workAmount.ToString(), 1, 10000);
            biomeSettings.failChance = extension.failChance;
            listingStandard.SliderLabeled("SC.ScavengingFailChance".Translate(), ref biomeSettings.failChance,
                biomeSettings.failChance.ToStringPercent());
            foreach (var scavengable in extension.scavengables)
            {
                var thingDef = DefDatabase<ThingDef>.GetNamedSilentFail(scavengable.thingDef);
                if (thingDef == null)
                {
                    continue;
                }

                if (biomeSettings.scavengingChance is null)
                {
                    biomeSettings.scavengingChance = new Dictionary<string, float>();
                }

                var scavengeValue = biomeSettings.scavengingChance[thingDef.defName] = scavengable.chance;
                listingStandard.SliderLabeled("SC.ScavengingChanceFor".Translate(thingDef.label),
                    ref scavengeValue, scavengeValue.ToStringPercent());
                biomeSettings.scavengingChance[thingDef.defName] = scavengeValue;

                if (biomeSettings.spawnAmount is null)
                {
                    biomeSettings.spawnAmount = new Dictionary<string, IntRange>();
                }

                var spawnAmount = biomeSettings.spawnAmount[thingDef.defName] = scavengable.amountToSpawn;
                listingStandard.IntRange("SC.SpawnAmount".Translate(thingDef.label), ref spawnAmount, 1, 999);
                biomeSettings.spawnAmount[thingDef.defName] = spawnAmount;
            }
        }

        listingStandard.End();
        Widgets.EndScrollView();
        SettingsApplier.ApplySettings();
        Write();
    }

    private float GetScrollHeight(List<TerrainDef> terrainDefs)
    {
        float num = 30;
        foreach (var terrain in terrainDefs)
        {
            var extension = terrain.GetModExtension<TerrainExtension>();
            if (extension == null)
            {
                continue;
            }

            num += 24 + 12;
            if (disabledTerrains.Contains(terrain.defName))
            {
                continue;
            }

            num += 48;
            foreach (var unused in extension.scavengables)
            {
                num += 24 + 24;
            }
        }

        return num + 10;
    }
}