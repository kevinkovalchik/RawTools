using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace RawToolsViz.Resources
{
    static class Colors
    {
        public static void SetDefaultColorsToColorBrewer8ClassSet2(this PlotModel model)
        {
            model.DefaultColors = ColorBrewer8ClassSet2(200);
        }

        public static List<OxyColor> ColorBrewer8ClassSet2(byte alpha)
        {
            return new List<OxyColor>
            {
                OxyColor.FromAColor(alpha, OxyColor.Parse("#66c2a5").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#fc8d62").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#8da0cb").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#e78ac3").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#a6d854").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#ffd92f").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#e5c494").ChangeSaturation(1.3)),
                OxyColor.FromAColor(alpha, OxyColor.Parse("#b3b3b3").ChangeSaturation(1.3))
            };
        }
    }
}
