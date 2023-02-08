using System.Linq;
using Verse;

namespace EmptysMod_RimWorld
{
    public class LaserHediffComp : HediffComp
    {       
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!CheckIfPawnBeingLasered())
            {
                this.Pawn.health.RemoveHediff(parent);
                RemovePawnFromTargets();
            }
        }
        public override void Notify_PawnKilled()
        {
            BeamDamage.targets.FirstOrDefault(x => x.victim == Pawn).heatLevel = 5;
            base.Notify_PawnKilled();
        }
        public bool CheckIfPawnBeingLasered() =>
            (Find.TickManager.TicksGame - BeamDamage.targets.FirstOrDefault(x => x.victim == Pawn).ticksHitAt)
            .TicksToSeconds() <= 3;

        public void RemovePawnFromTargets()
        {
            try
            {
                var pawnInList = BeamDamage.targets.FirstOrDefault(x => x.victim == this.Pawn);
                BeamDamage.targets.Remove(pawnInList);
            }
            catch { }
        }
    }
}
