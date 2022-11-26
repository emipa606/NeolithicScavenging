using System.Collections.Generic;
using Verse;

namespace Scavenging;

public class TerrainSettings : IExposable
{
    public float failChance;
    private List<float> floatValues1;
    private List<IntRange> intRangeValue;
    public Dictionary<string, float> scavengingChance;
    public Dictionary<string, IntRange> spawnAmount;

    private List<string> stringKeys1;
    private List<string> stringKeys2;
    public int workAmount;

    public void ExposeData()
    {
        Scribe_Values.Look(ref workAmount, "workAmount");
        Scribe_Values.Look(ref failChance, "failChance");
        Scribe_Collections.Look(ref scavengingChance, "scavengingChance", LookMode.Value, LookMode.Value,
            ref stringKeys1, ref floatValues1);
        Scribe_Collections.Look(ref spawnAmount, "spawnAmount", LookMode.Value, LookMode.Value, ref stringKeys2,
            ref intRangeValue);
    }
}