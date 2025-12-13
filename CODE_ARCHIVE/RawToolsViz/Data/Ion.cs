using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawToolsViz.Data
{
    public class Ion
    {
        public double Mass;
        public double Intensity;

        public Ion(double mass, double intensity)
        {
            Mass = mass;
            Intensity = intensity;
        }
    }
}
