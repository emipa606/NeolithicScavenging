using System.Collections.Generic;
using Verse;

namespace Scavenging;

public class ScavengableRecord
{
    public IntRange amountToSpawn;

    public List<string> cannotSpawnIn;

    public float chance;
    public string thingDef;
}