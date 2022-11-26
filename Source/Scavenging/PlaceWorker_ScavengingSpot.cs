using System.Linq;
using UnityEngine;
using Verse;

namespace Scavenging;

public class PlaceWorker_ScavengingSpot : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        var scavengingSpots = map.listerThings.ThingsOfDef(SC_DefOf.SC_ScavengingSpot);
        return scavengingSpots.Any(x => x.Position.DistanceTo(loc) <= 40)
            ? new AcceptanceReport("SC.CannotPlaceSpotsNearToEachOther".Translate())
            : base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore, thing);
    }

    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
    {
        var currentMap = Find.CurrentMap;
        var cells = ScavengeUtils.GetScavengableCells(center, currentMap);
        if (cells.Any())
        {
            GenDraw.DrawFieldEdges(cells.ToList(), Color.green);
        }
    }
}