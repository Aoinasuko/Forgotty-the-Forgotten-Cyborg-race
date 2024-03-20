using AlienRace;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Forgotty_Race
{

    [StaticConstructorOnStartup]
    static class Forgotty_Harmony
    {
        static Forgotty_Harmony()
        {
            var harmony = new Harmony("BEP.Forgotty");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    /// <summary>
    /// フォアゴッティは経験値を得られない
    /// <summary>
    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    [HarmonyPatch(new Type[]
    {
        typeof(float),
        typeof(bool),
    })]
    internal static class Learn_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref SkillRecord __instance, float xp, bool direct = false)
        {
            if (__instance.Pawn.def.defName == "Forgotty_Pawn")
            {
                if (xp > 0.0f)
                {
                    Comp_Forgotty comp = __instance.Pawn.TryGetComp<Comp_Forgotty>();
                    if (comp.Tier >= 5)
                    {
                        return false;
                    }
                    if (Comp_Forgotty.NeedXP(comp.Tier) > comp.Xp)
                    {
                        comp.Xp += xp;
                        if (Comp_Forgotty.NeedXP(comp.Tier) <= comp.Xp)
                        {
                            comp.Xp = Comp_Forgotty.NeedXP(comp.Tier);
                            Find.LetterStack.ReceiveLetter("Forgotty.UI.CanLevelUpLabel".Translate(comp.parent), "Forgotty.UI.CanLevelUp".Translate(comp.parent), LetterDefOf.NeutralEvent, comp.parent, null, null);
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }

	/// <summary>
    /// メカニター扱いにする
    /// <summary>
    [HarmonyPatch(typeof(MechanitorUtility), "IsMechanitor")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    internal static class IsMechanitor_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref bool __result, Pawn pawn)
        {
            if (pawn.def.defName == "Forgotty_Pawn")
            {
                if (!ModsConfig.BiotechActive)
				{
					return;
				}
				if (pawn.Faction.IsPlayerSafe())
				{
					__result = true;
					return;
				}
            }
            return;
        }
    }
}
