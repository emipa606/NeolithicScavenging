using UnityEngine;
using Verse;

namespace Scavenging;

internal static class SettingsHelper
{
    extension(Listing_Standard ls)
    {
        public void SliderLabeled(string label, ref int val, string format, float min = 0f,
            float max = 100f, string tooltip = null)
        {
            float num = val;
            ls.SliderLabeled(label, ref num, format, min, max, tooltip);
            val = (int)num;
        }

        public void SliderLabeled(string label, ref float val, string format,
            float min = 0f, float max = 1f, string tooltip = null)
        {
            var rect = ls.GetRect(Text.LineHeight);
            var rect2 = rect.LeftPart(0.7f).Rounded();
            var rect3 = rect.RightPart(0.3f).Rounded().LeftPart(0.67f).Rounded();
            var rect4 = rect.RightPart(0.1f).Rounded();
            var anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);
            var num = Widgets.HorizontalSlider(rect3, val, min, max, true);
            val = num;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect4, string.Format(format, val));
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }

            Text.Anchor = anchor;
            ls.Gap(ls.verticalSpacing);
        }

        public void IntRange(string label, ref IntRange range, int min = 0, int max = 1,
            string tooltip = null, ToStringStyle valueStyle = ToStringStyle.Integer)
        {
            var rect = ls.GetRect(Text.LineHeight);
            var rect2 = rect.LeftPart(0.7f).Rounded();
            var rect3 = rect.RightPart(0.3f).Rounded().LeftPart(0.9f).Rounded();
            rect3.yMin -= 5f;
            var anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);
            Text.Anchor = TextAnchor.MiddleRight;
            var hashCode = ls.CurHeight.GetHashCode();
            Widgets.IntRange(rect3, hashCode, ref range, min, max);
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }

            Text.Anchor = anchor;
            ls.Gap(ls.verticalSpacing);
        }
    }

    public class LabeledRadioValue<T>(string label, T val)
    {
        public string Label { get; } = label;

        public T Value { get; } = val;
    }
}