using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Forgotty_Race
{
	public class CompTargetable_Forgotty : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetBuildings = false,
				validator = ((TargetInfo x) => x.Thing is Pawn && x.Thing.def.defName == "Forgotty_Pawn")
			};
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

	// 死んだフォアゴッティにのみ使用可能
	public class CompTargetable_DeadForgotty : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = false,
				canTargetBuildings = false,
				canTargetItems = true,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = ((TargetInfo x) => x.Thing is Corpse && ((Corpse)x.Thing).InnerPawn.def.defName == "Forgotty_Pawn" && BaseTargetValidator(x.Thing))
			};
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

	// フォアゴッティのみ使用可能
	public class CompUsable_Forgotty : CompUsable
	{
		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			if (p.def.defName != "Forgotty_Pawn")
            {
				failReason = "Forgotty.UI.NotForgotty".Translate();
				return false;
			}
			if (p.TryGetComp<Comp_Forgotty>().Core >= p.TryGetComp<Comp_Forgotty>().Tier)
            {
				failReason = "Forgotty.UI.CoreFull".Translate();
				return false;
			}
			return base.CanBeUsedBy(p, out failReason);
		}
	}

	// フォアゴッティのみ使用可能
	public class CompUsable_ForgottyMemory : CompUsable
	{
		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			if (p.def.defName != "Forgotty_Pawn")
			{
				failReason = "Forgotty.UI.NotForgotty".Translate();
				return false;
			}
			if (p.health.hediffSet.HasHediff(HediffDef.Named("Forgotty_BackupMemory")))
			{
				failReason = "Forgotty.UI.BackupInstalled".Translate();
				return false;
			}
			return base.CanBeUsedBy(p, out failReason);
		}
	}

	// コア追加
	public class CompUseEffect_AddCore : CompUseEffect
	{
		public override void DoEffect(Pawn usedBy)
		{
			base.DoEffect(usedBy);
			usedBy.TryGetComp<Comp_Forgotty>().Core++;
			Messages.Message("Forgotty.UI.CoreCharge".Translate(usedBy), usedBy, MessageTypeDefOf.PositiveEvent);
		}
	}

	// コア追加
	public class CompUseEffect_BackupMemory : CompUseEffect
	{
		public override void DoEffect(Pawn usedBy)
		{
			base.DoEffect(usedBy);
			usedBy.health.AddHediff(HediffDef.Named("Forgotty_BackupMemory"));
			Messages.Message("Forgotty.UI.BackupInstall".Translate(usedBy), usedBy, MessageTypeDefOf.PositiveEvent);
		}
	}

	// フォアゴッティ召喚
	public class CompUseEffect_SummonForgotty : CompUseEffect
	{
		public override void DoEffect(Pawn usedBy)
		{
			Dialog_SummonForgotty dialog = new Dialog_SummonForgotty();
			dialog.SetData(usedBy);
			Find.WindowStack.Add(dialog);
		}
	}

	public class Dialog_SummonForgotty : Window
	{
		public override Vector2 InitialSize => new Vector2(800f, 500f);

		Pawn User;

		public Dialog_SummonForgotty()
		{
			forcePause = true;
			doCloseX = false;
			absorbInputAroundWindow = true;
			closeOnAccept = false;
			closeOnClickedOutside = false;
		}

		public void SetData(Pawn pawn)
		{
			User = pawn;
		}

		public PawnKindDef find_living(ThingDef def)
		{

			IEnumerable<PawnKindDef> pawnkinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(x => x.race == def);

			if (!pawnkinds.EnumerableNullOrEmpty())
			{
				PawnKindDef pawnkind = pawnkinds.RandomElement();
				return pawnkind;
			}
			return null;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listingStandard = new Listing_Standard();
			Text.Font = GameFont.Small;
			String text = ("Forgotty.UI.SelectForgotty").Translate();
			Widgets.Label(new Rect(0f, 0f, inRect.width, inRect.height), text);
			if (Widgets.ButtonText(new Rect(5f, inRect.height - 115f, inRect.width - 10f, 35f), "Forgotty.UI.Worker".Translate()))
			{
				MakeForgotty(0);
				Find.WindowStack.TryRemove(this);
				return;
			}
			if (Widgets.ButtonText(new Rect(5f, inRect.height - 75f, inRect.width - 10f, 35f), "Forgotty.UI.Combat".Translate()))
			{
				MakeForgotty(1);
				Find.WindowStack.TryRemove(this);
				return;
			}
			if (Widgets.ButtonText(new Rect(5f, inRect.height - 35f, inRect.width - 10f, 35f), "Forgotty.UI.Research".Translate()))
			{
				MakeForgotty(2);
				Find.WindowStack.TryRemove(this);
				return;
			}
		}

		// フォアゴッティ生産
		public void MakeForgotty(int Type)
        {
			PawnKindDef forgotty = PawnKindDef.Named("Forgotty_Visitor");
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(forgotty, Faction.OfPlayer, PawnGenerationContext.NonPlayer, default, true, default, default, false, default, 0f, default, default, default, default, false, default, default, default, default, default, default, default, default, default, default, default, default, default, 19f, 19f, default, default));
			// Typeによってストーリー変化
			// Worker
			if (Type == 0)
            {
				pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Forgotty_Adult_N_A");
				pawn.TryGetComp<Comp_Forgotty>().Type = 0;
			}
			if (Type == 1)
			{
				pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Forgotty_Adult_N_B");
				pawn.TryGetComp<Comp_Forgotty>().Type = 1;
			}
			if (Type == 2)
			{
				pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Forgotty_Adult_N_C");
				pawn.TryGetComp<Comp_Forgotty>().Type = 2;
			}
			Comp_Forgotty.FixSkill(pawn, 1, Type);
			pawn.apparel.DestroyAll();
			GenSpawn.Spawn(pawn, User.Position, User.Map, Rot4.Random);
			Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter("Forgotty.UI.Summonlabel".Translate(pawn), "Forgotty.UI.Summon".Translate(pawn), LetterDefOf.PositiveEvent, pawn, null, null, null));
			return;
		}

	}

}
