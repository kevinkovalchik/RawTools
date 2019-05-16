using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace RawToolsViz
{
    class RawFileDataPoint : ScatterPoint
    {
        public string RawFile { get; set; }

        public RawFileDataPoint(double x, double y, string rawFile) : base(x, y)
        {
            RawFile = Path.GetFileName(rawFile);
        }
    }
}
