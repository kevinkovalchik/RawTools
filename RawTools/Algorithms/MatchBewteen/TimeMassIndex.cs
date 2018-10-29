using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawTools.Algorithms.MatchBewteen
{
    class TimeMass
    {
        double Time;
        double Mass;

        public bool WithinTolerance(double matchMass, double matchTime, int ppmMassTolerance, double percentTimeTolerance)
        {
            bool massWithin = Math.Abs(matchMass - Mass) / Mass * 1e6 < ppmMassTolerance;
            bool timeWithin = Math.Abs(matchTime - Time) / Time * 100 < percentTimeTolerance;
            return massWithin & timeWithin;
        }
    }

    class TimeMassIndex
    {

    }
}
