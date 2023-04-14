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

    [DefOf]
    public static class Faction_Forgotty
    {
        public static FactionDef Forgotty_ForgottyGroup;
    }

	[StaticConstructorOnStartup]
	public static class Graphic_Forgotty
	{
		public static readonly Texture2D Star = ContentFinder<Texture2D>.Get("Forgotty/UI/System/Star");
		public static readonly Texture2D Star_None = ContentFinder<Texture2D>.Get("Forgotty/UI/System/Star_None");
		public static readonly Texture2D Format = ContentFinder<Texture2D>.Get("Forgotty/UI/Skill/Format");
	}

}
