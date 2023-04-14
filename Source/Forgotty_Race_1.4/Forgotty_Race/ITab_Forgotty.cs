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
    public class ITab_Forgotty : ITab
    {
        private Texture2D deleteX;

        private static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        public override bool IsVisible
        {
            get
            {
                return this.SelPawn.Faction != null && this.SelPawn.Faction.IsPlayer;
            }
        }

        private Comp_Forgotty comp
        {
            get
            {
                return this.SelPawn.GetComp<Comp_Forgotty>();
            }
        }

        public void LoadGraphics()
        {
            this.deleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        }

        public ITab_Forgotty()
        {
            this.size = new Vector2(450f, 170f);
            this.labelKey = "Forgotty.UI.ITab";
            this.tutorTag = "Forgotty";
            LongEventHandler.ExecuteWhenFinished(LoadGraphics);
        }

        protected override void FillTab()
        {
            Text.Font = GameFont.Small;
            Rect rect_label = new Rect(45f, 30f, 100f, 20f);
            Rect rect_star = new Rect(45f, 50f, 40f, 40f);
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            GUI.Label(rect_label, "Forgotty.UI.Tier".Translate());
            for (int i = 0; i < 5; i++)
            {
                if (comp.Tier <= i)
                {
                    Widgets.DrawTextureFitted(rect_star, Graphic_Forgotty.Star_None, 1.5f);
                }
                else
                {
                    Widgets.DrawTextureFitted(rect_star, Graphic_Forgotty.Star, 1.5f);
                }
                rect_star.x += 60;
            }
            Rect rect_label2 = new Rect(45f, 100f, 300f, 20f);
            GUI.Label(rect_label2, "Forgotty.UI.XP".Translate() + ":" + comp.Xp + "/" + Comp_Forgotty.NeedXP(comp.Tier));
            rect_label2.y += 30;
            GUI.Label(rect_label2, "Forgotty.UI.Core".Translate() + ":" + comp.Core + "/" + comp.Tier);
        }
    }
}
