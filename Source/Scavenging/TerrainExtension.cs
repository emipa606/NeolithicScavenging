using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Scavenging;

public class TerrainExtension : DefModExtension
{
    public float failChance;
    public List<ScavengableRecord> scavengables;
    public int workAmount;

    public List<ScavengableRecord> ScavengablesFor(BiomeDef biomeDef)
    {
        return scavengables.Where(x =>
            DefDatabase<ThingDef>.GetNamedSilentFail(x.thingDef) != null &&
            (x.cannotSpawnIn is null || !x.cannotSpawnIn.Contains(biomeDef.defName))).ToList();
    }
}