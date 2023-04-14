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
    /// 脱走しない
    /// </summary>
    [HarmonyPatch(typeof(PrisonBreakUtility), "InitiatePrisonBreakMtbDays")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
        typeof(StringBuilder),
        typeof(bool)
    })]
    static class FixPrisonBreak
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn, StringBuilder sb, bool ignoreAsleep)
        {
            if (pawn.def.defName == "Forgotty_Pawn")
            {
                __result = -1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlaveRebellionUtility), "InitiateSlaveRebellionMtbDays")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    static class FixSlaveRebellion
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "Forgotty_Pawn")
            {
                __result = -1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlaveRebellionUtility), "InitiateSlaveRebellionMtbDaysHelper")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    static class FixSlaveRebellion_Fix2
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "Forgotty_Pawn")
            {
                __result = -1f;
                return false;
            }
            return true;
        }
    }

    // フォアゴッティの派閥追加時、服装規定を無効化する
    [HarmonyPatch(typeof(Precept_Role), "GenerateNewApparelRequirements")]
    [HarmonyPatch(new Type[]
    {
        typeof(FactionDef),
    })]
    internal static class ApparelRequirement_Patch
    {

        [HarmonyPrefix]
        static bool Prefix(ref Precept_Role __instance, ref List<PreceptApparelRequirement> __result, FactionDef generatingFor)
        {
            if (generatingFor != null)
            {
                if (generatingFor.defName == "Forgotty_ForgottyGroup")
                {
                    List<PreceptApparelRequirement> ApparelRequirement = new List<PreceptApparelRequirement>();
                    // 役割のdefName取得
                    String role = __instance.def.defName;
                    // 役割によって装備を指定
                    if (!ApparelRequirement.NullOrEmpty())
                    {
                        __result = ApparelRequirement;
                    }
                    else
                    {
                        __result = null;
                    }
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 奴隷時に作業速度ボーナス
    /// <summary>
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(StatPart_Slave))]
    internal static class TransformValue_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StatPart_Slave.TransformValue),
            new Type[] { typeof(StatRequest), typeof(float) },
            new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        static bool Prefix(StatRequest req, ref float val)
        {
            if (req.HasThing)
            {
                if (req.Thing as Pawn != null)
                {
                    Pawn pawn = (Pawn)req.Thing;
                    if (pawn.def.defName == "Forgotty_Pawn")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 奴隷時のボーナス説明
    /// <summary>
    [HarmonyPatch(typeof(StatPart_Slave), "ExplanationPart")]
    [HarmonyPatch(new Type[]
    {
        typeof(StatRequest),
    })]
    internal static class ExplanationPart_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_Slave __instance, ref String __result, StatRequest req)
        {
            if (req.HasThing)
            {
                if (req.Thing as Pawn != null)
                {
                    Pawn pawn = (Pawn)req.Thing;
                    if (pawn.def.defName == "Forgotty_Pawn")
                    {
                        return false;
                    }
                }
            }
            return true;
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

}
