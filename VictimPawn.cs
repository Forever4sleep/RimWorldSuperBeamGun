using System;
using Verse;

namespace EmptysMod_RimWorld
{
    public class VictimPawn
    {
        public Pawn victim;
        public int ticksHitAt, ticksLastHitAt;
        public int heatLevel;
        public VictimPawn(Pawn victim, int hitAt)
        {
            this.victim = victim;
            this.ticksLastHitAt = hitAt;
            this.ticksHitAt = hitAt;
            this.heatLevel = 5;
        }
    }
}
