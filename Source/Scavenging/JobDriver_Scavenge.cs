using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Scavenging;

public class JobDriver_Scavenge : JobDriver
{
    public const int InitialStandingPeriod = 360;
    private int curWorkTick;

    private int nextTurnTick;
    public Thing ScavengeSpot => TargetA.Thing;
    public IntVec3 Cell => TargetB.Cell;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(TargetA, job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
        var doWork = new Toil
        {
            tickAction = () =>
            {
                curWorkTick++;
                if (curWorkTick >= nextTurnTick)
                {
                    pawn.Rotation = Rot4.Random;
                    nextTurnTick = curWorkTick + Rand.RangeInclusive(60, 120);
                }

                if (curWorkTick < InitialStandingPeriod)
                {
                    return;
                }

                var cell = ScavengeUtils.GetScavengableCells(pawn.Position, Map).RandomElement();
                job.SetTarget(TargetIndex.B, cell);
                ReadyForNextToil();
            },
            handlingFacing = true,
            defaultCompleteMode = ToilCompleteMode.Never
        };
        doWork.WithProgressBar(TargetIndex.A, () => curWorkTick / (float)InitialStandingPeriod);
        doWork.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return doWork;
        yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
        var doScavenging = new Toil
        {
            initAction = () => curWorkTick = 0,
            tickAction = () =>
            {
                curWorkTick++;
                if (curWorkTick < GetScavengingPeriod(Cell))
                {
                    return;
                }

                var extension = Cell.GetTerrain(Map).GetModExtension<TerrainExtension>();
                if (!Rand.Chance(extension.failChance) && extension.ScavengablesFor(Map.Biome)
                        .TryRandomElementByWeight(x => x.chance, out var scavengableRecord))
                {
                    SC_DefOf.Harvest_Standard_Finish.PlayOneShot(pawn);
                    var thingDef = DefDatabase<ThingDef>.GetNamed(scavengableRecord.thingDef);
                    var thing = ThingMaker.MakeThing(thingDef);
                    thing.stackCount = scavengableRecord.amountToSpawn.RandomInRange;
                    GenSpawn.Spawn(thing, pawn.Position, Map);
                }

                ReadyForNextToil();
            }
        };
        doScavenging.WithProgressBar(TargetIndex.B, () => curWorkTick / (float)GetScavengingPeriod(Cell));
        doScavenging.PlaySustainerOrSound(() => SC_DefOf.Harvest_Standard);

        doScavenging.defaultCompleteMode = ToilCompleteMode.Never;
        doScavenging.handlingFacing = true;
        yield return doScavenging;
    }

    private int GetScavengingPeriod(IntVec3 cell)
    {
        var extension = cell.GetTerrain(Map).GetModExtension<TerrainExtension>();
        var plantWorkbonus = pawn.GetStatValue(StatDefOf.PlantWorkSpeed);
        var huntingWorkBonus = pawn.GetStatValue(StatDefOf.HuntingStealth);
        var total = 1 + plantWorkbonus + huntingWorkBonus;
        return (int)(extension.workAmount / total);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref curWorkTick, "curWorkTick");
        Scribe_Values.Look(ref nextTurnTick, "nextTurnTick");
    }
}