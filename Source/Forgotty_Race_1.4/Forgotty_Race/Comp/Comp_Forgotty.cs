using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Forgotty_Race
{
	public class CompProperties_Forgotty : CompProperties
	{
		public CompProperties_Forgotty()
		{
			compClass = typeof(Comp_Forgotty);
		}
	}

	public class Comp_Forgotty : ThingComp
	{

		// Tier
		public int Tier = 1;
		// 経験値
		public float Xp = 0;
		// タイプ
		public int Type = 1;
		// コア
		public int Core = 0;
		// 初期設定
		public bool First = false;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref Tier, "Tier");
			Scribe_Values.Look(ref Xp, "Xp");
			Scribe_Values.Look(ref Type, "Type");
			Scribe_Values.Look(ref Core, "Core");
			Scribe_Values.Look(ref First, "First");
		}

		// 必要XP
		public static float NeedXP(int level)
		{
			switch (level)
			{
				case 1:
					return 10000f;
				case 2:
					return 20000f;
				case 3:
					return 50000f;
				case 4:
					return 100000f;
				case 5:
					return 999999f;
				case 6:
					return 999999f;
			}
			return 0f;
		}

		// 忘れ去られる(レベルアップ）
		public void Forgot(Pawn pawn, bool saveflag)
		{
			if (!saveflag)
            {
				List<Pawn> pawns = new List<Pawn>();
				foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
                {
					if (relation.otherPawn.needs.mood != null)
                    {
						if (!pawns.Contains(relation.otherPawn))
                        {
							pawns.Add(relation.otherPawn);
						}
					}
                }
				// 忘れ去られた心情デバフ
				foreach (Pawn pawn_other in pawns)
                {
					if (pawn_other.relations.OpinionOf(pawn) > 0)
                    {
						if (!pawn_other.story.traits.HasTrait(TraitDefOf.Psychopath))
                        {
							pawn_other.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("Forgotty_Forgot"), pawn);
						}
                    }
                }
				pawn.relations.ClearAllRelations();
				pawn.needs.mood.thoughts.memories = new MemoryThoughtHandler(pawn);
			}
			if (Tier < 5)
            {
				Tier++;
			}
			FixSkill(pawn, Tier, Type);
			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_Tier")))
            {
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Forgotty_Tier")));
			}
			pawn.health.AddHediff(HediffDef.Named("Forgotty_Tier")).Severity = Tier;
			pawn.health.AddHediff(HediffDefOf.Anesthetic);
			Core = 0;
			Xp = 0;
		}

		// 経験値固定化
		public static void FixSkill(Pawn pawn, int level, int type)
		{
			// 作業タイプ
			if (type == 0)
            {
				int inputlevel = 0;
				if (level == 1)
                {
					inputlevel = 4;
				}
				if (level == 2)
				{
					inputlevel = 6;
				}
				if (level == 3)
				{
					inputlevel = 8;
				}
				if (level == 4)
				{
					inputlevel = 10;
				}
				if (level == 5)
				{
					inputlevel = 15;
				}
				if (level == 6)
				{
					inputlevel = 20;
				}
				foreach (SkillDef allDef in DefDatabase<SkillDef>.AllDefs.Where(x => x.defName != "Social"))
				{
					pawn.skills.GetSkill(allDef).passion = Passion.None;
					pawn.skills.GetSkill(allDef).Level = 0;
					pawn.skills.GetSkill(allDef).xpSinceLastLevel = 0f;
					if (allDef.defName != "Shooting" && allDef.defName != "Melee")
                    {
						pawn.skills.GetSkill(allDef).passion = Passion.None;
						pawn.skills.GetSkill(allDef).Level = inputlevel;
						pawn.skills.GetSkill(allDef).xpSinceLastLevel = 500f;
					}
				}
				String lastname = ((NameTriple)pawn.Name).Last;
				pawn.Name = new NameTriple("W-" + level, "", lastname);
			}
			// 戦闘タイプ
			if (type == 1)
			{
				int inputlevel = 0;
				if (level == 1)
				{
					inputlevel = 8;
				}
				if (level == 2)
				{
					inputlevel = 10;
				}
				if (level == 3)
				{
					inputlevel = 12;
				}
				if (level == 4)
				{
					inputlevel = 14;
				}
				if (level == 5)
				{
					inputlevel = 20;
				}
				if (level == 6)
				{
					inputlevel = 20;
				}
				foreach (SkillDef allDef in DefDatabase<SkillDef>.AllDefs.Where(x => x.defName != "Social"))
				{
					pawn.skills.GetSkill(allDef).passion = Passion.None;
					pawn.skills.GetSkill(allDef).Level = 0;
					pawn.skills.GetSkill(allDef).xpSinceLastLevel = 0f;
					if (allDef.defName == "Shooting" || allDef.defName == "Melee")
					{
						pawn.skills.GetSkill(allDef).passion = Passion.None;
						pawn.skills.GetSkill(allDef).Level = inputlevel;
						pawn.skills.GetSkill(allDef).xpSinceLastLevel = 500f;
					}
				}
				String lastname = ((NameTriple)pawn.Name).Last;
				pawn.Name = new NameTriple("C-" + level, "", lastname);
			}
			// 研究タイプ
			if (type == 2)
			{
				int inputlevel = 0;
				if (level == 1)
				{
					inputlevel = 10;
				}
				if (level == 2)
				{
					inputlevel = 12;
				}
				if (level == 3)
				{
					inputlevel = 14;
				}
				if (level == 4)
				{
					inputlevel = 16;
				}
				if (level == 5)
				{
					inputlevel = 20;
				}
				if (level == 6)
				{
					inputlevel = 20;
				}
				foreach (SkillDef allDef in DefDatabase<SkillDef>.AllDefs.Where(x => x.defName != "Social"))
				{
					pawn.skills.GetSkill(allDef).passion = Passion.None;
					pawn.skills.GetSkill(allDef).Level = 0;
					pawn.skills.GetSkill(allDef).xpSinceLastLevel = 0f;
					if (allDef.defName == "Intellectual")
					{
						pawn.skills.GetSkill(allDef).passion = Passion.None;
						pawn.skills.GetSkill(allDef).Level = inputlevel;
						pawn.skills.GetSkill(allDef).xpSinceLastLevel = 500f;
					}
				}
				String lastname = ((NameTriple)pawn.Name).Last;
				pawn.Name = new NameTriple("R-" + level, "", lastname);
			}

			// 社交スキルを10にする
			foreach (SkillDef allDef in DefDatabase<SkillDef>.AllDefs.Where(x => x.defName == "Social"))
			{
				pawn.skills.GetSkill(allDef).passion = Passion.None;
				pawn.skills.GetSkill(allDef).Level = 10;
				pawn.skills.GetSkill(allDef).xpSinceLastLevel = 500f;
			}
		}

		public override void CompTick()
		{
			Pawn comppawn = (Pawn)this.parent;
			// もし子供から生まれたなどの理由で未設定の場合、ランダム設定を行う
			if (!First)
            {
				bool setflag = false;
				if (comppawn.story.Adulthood?.defName == "Forgotty_Adult_N_A")
                {
					setflag = true;
					Type = 0;
					FixSkill(comppawn, Tier, Type);
					comppawn.health.AddHediff(HediffDef.Named("Forgotty_WorkType"));
				}
				if (comppawn.story.Adulthood?.defName == "Forgotty_Adult_N_B")
				{
					setflag = true;
					Type = 1;
					FixSkill(comppawn, Tier, Type);
					comppawn.health.AddHediff(HediffDef.Named("Forgotty_CombatType"));
				}
				if (comppawn.story.Adulthood?.defName == "Forgotty_Adult_N_C")
				{
					setflag = true;
					Type = 2;
					FixSkill(comppawn, Tier, Type);
					comppawn.health.AddHediff(HediffDef.Named("Forgotty_ResearchType"));
				}
				if (setflag == false)
                {
					Type = Rand.Range(0, 3);
					FixSkill(comppawn, Tier, Type);
				}
				comppawn.health.AddHediff(HediffDef.Named("Forgotty_Tier")).Severity = Tier;
				First = true;
			}
			if (comppawn.IsHashIntervalTick(180))
            {
				if (Type == 0)
                {
					if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_WorkType")))
                    {
						comppawn.health.AddHediff(HediffDef.Named("Forgotty_WorkType"));
					}
                }
				if (Type == 1)
				{
					if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_CombatType")))
					{
						comppawn.health.AddHediff(HediffDef.Named("Forgotty_CombatType"));
					}
				}
				if (Type == 2)
				{
					if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_ResearchType")))
					{
						comppawn.health.AddHediff(HediffDef.Named("Forgotty_ResearchType"));
					}
				}
				if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_Tier")))
                {
					comppawn.health.AddHediff(HediffDef.Named("Forgotty_Tier")).Severity = Tier;
				}
			}
		}

		public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			base.PostPreApplyDamage(dinfo, out absorbed);
			if (dinfo.Def.defName == "EMP")
            {
				Pawn comppawn = (Pawn)this.parent;
				comppawn?.stances?.stunner?.StunFor(1800, dinfo.Instigator);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Pawn comppawn = (Pawn)this.parent;
			String disablestr = "";
			if (Find.Selector.SelectedPawns.Contains(comppawn))
			{
				if (comppawn.Faction?.IsPlayer ?? false)
                {
					if (Tier < 5)
					{
						Gizmo Gizmo = new Command_Action
						{
							defaultLabel = "Forgotty.UI.label_Format".Translate(),
							defaultDesc = "Forgotty.UI.desc_Format".Translate(),
							icon = Graphic_Forgotty.Format,
							disabled = CheckDisabled(out disablestr),
							disabledReason = disablestr,
							action = delegate
							{
								if (comppawn.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_BackupMemory")))
								{
									Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("Forgotty.UI.warn_FormatSave".Translate(), delegate
									{
										Forgot(comppawn, true);
										comppawn.health.RemoveHediff(comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Forgotty_BackupMemory")));
									}, destructive: false));

								}
								else
								{
									Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("Forgotty.UI.warn_Format".Translate(), delegate
									{
										Forgot(comppawn, false);
									}, destructive: true));
								}
							}
						};
						yield return Gizmo;
					}
				}
			}
		}
		
		public bool CheckDisabled(out String disablestr)
        {
			if (Tier >= 3)
            {
				if (!ResearchProjectDef.Named("Forgotty_Overhaul").IsFinished)
                {
					disablestr = "Forgotty.UI.NeedResearch".Translate();
					return true;
				}
			}
			if (Tier >= 4)
			{
				if (!ResearchProjectDef.Named("Forgotty_FinalOverhaul").IsFinished)
				{
					disablestr = "Forgotty.UI.NeedResearch".Translate();
					return true;
				}
			}
			if (Xp < NeedXP(Tier))
			{
				disablestr = "Forgotty.UI.NeedXP".Translate(NeedXP(Tier) - Xp);
				return true;
			}
			if (Core < Tier)
            {
				disablestr = "Forgotty.UI.NeedCore".Translate(Tier - Core);
				return true;
			}
			disablestr = "";
			return false;
		}

	}
}
