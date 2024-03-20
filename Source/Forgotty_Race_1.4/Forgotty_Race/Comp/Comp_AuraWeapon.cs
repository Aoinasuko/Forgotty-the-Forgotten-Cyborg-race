using System.Collections.Generic;
using System.Linq;
using BEPRace_Core;
using RimWorld;
using Verse;

namespace Forgotty_Race
{
	public class CompProperties_AuraWeapon : CompProperties
	{
		// 与えるHediff
		public HediffDef hediff;

		public CompProperties_AuraWeapon()
		{
			compClass = typeof(Comp_AuraWeapon);
		}
	}

	public class Comp_AuraWeapon : ThingComp
	{
		public CompProperties_AuraWeapon Props => (CompProperties_AuraWeapon)props;

		// 武器として使用した時にエフェクトと周囲の味方にバフ
		public override void Notify_UsedWeapon(Pawn pawn)
		{
			List<Pawn> press = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(pawn.Position) <= 5.9f && !x.HostileTo(pawn) && x != pawn).ToList();
			if (!press.NullOrEmpty())
            {
				for (int j = press.Count() - 1; j >= 0; j--)
				{
					// 死亡していない場合、hediffを付与
					if (!press.ElementAt(j).Dead)
					{
						Effecter_BEPCore.BEP_UseSkill_E.Spawn(press.ElementAt(j).Position, press.ElementAt(j).Map);
						press.ElementAt(j).health.AddHediff(Props.hediff);
					}
				}
			}
			return;
		}
	}
}
