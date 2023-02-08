using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EmptysMod_RimWorld
{
    public class BeamDamage : DamageWorker_AddInjury
    {
        public static List<VictimPawn> targets = new List<VictimPawn>();

        public static int lastDamage, lastPenetration = 1;
        public void AddPawnToVictimList(Pawn victim)
        {
            targets.Add(new VictimPawn(victim, Find.TickManager.TicksGame));
        }
        public void UpdatePawnHitTime(VictimPawn victim)
        {
            victim.ticksHitAt = Find.TickManager.TicksGame;
        }
        public void UpdatePawnLastHitAt(VictimPawn pawn)
        {
            pawn.ticksLastHitAt = Find.TickManager.TicksGame;
        }
        public int GetTicksSinceLastHit(VictimPawn pawn) => Find.TickManager.TicksGame - pawn.ticksLastHitAt;

        public void SetDamageDependingOnHeatLevel(ref DamageInfo dinfo, VictimPawn pawn)
        {
            int level = pawn.heatLevel;

            float damage = 1;

            if (damage < 100)
                damage += (2 * level + 5) / 5;
            else damage = 30;

            lastDamage = (int)damage;

            dinfo.SetAmount(damage);
        }
        public void SetArmorPenetrationDependingOnHeatLevel(ref DamageInfo dinfo, VictimPawn pawn)
        {
            int level = pawn.heatLevel;
            float penetration = lastPenetration;
            if (level == 10) penetration = 5;
            if (level == 50) penetration = 20;
            if (level == 100) penetration = 60;
            if (level > 50 && level < 100) if (level < 60) penetration = level - 30; else penetration = level - 40;

            lastPenetration = level;
            dinfo = new DamageInfo(dinfo.Def, dinfo.Amount, penetration, dinfo.Angle, dinfo.Instigator, dinfo.HitPart, dinfo.Weapon,
                dinfo.Category, dinfo.IntendedTarget, dinfo.InstigatorGuilty, dinfo.SpawnFilth);

        }
        public void SetPenetration(ref DamageInfo dinfo)
        {
            dinfo = new DamageInfo(dinfo.Def, dinfo.Amount, lastPenetration, dinfo.Angle, dinfo.Instigator, dinfo.HitPart, dinfo.Weapon,
               dinfo.Category, dinfo.IntendedTarget, dinfo.InstigatorGuilty, dinfo.SpawnFilth);
        }
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            Pawn victim = thing as Pawn;
            dinfo.SetAmount(lastDamage);
            SetPenetration(ref dinfo);
            if (victim != null)
            {
                VictimPawn victimInList = targets.FirstOrDefault(x => x.victim == victim);
                if (victimInList != null)
                {
                    UpdatePawnHitTime(victimInList);

                    float seconds = GetTicksSinceLastHit(victimInList).TicksToSeconds();
                    if (seconds >= 1.5)
                    {
                        if (victimInList.heatLevel < 100) victimInList.heatLevel += 10;
                        SetDamageDependingOnHeatLevel(ref dinfo, victimInList);
                        SetArmorPenetrationDependingOnHeatLevel(ref dinfo, victimInList);
                        UpdatePawnLastHitAt(victimInList);
                    }
                }
                else AddPawnToVictimList(victim);
            }
            return base.Apply(dinfo, thing);
        }
    }
}
