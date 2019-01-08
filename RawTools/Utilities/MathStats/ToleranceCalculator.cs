// Copyright 2018 Kevin Kovalchik & Christopher Hughes
// 
// Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// Kevin Kovalchik and Christopher Hughes do not claim copyright of
// any third-party libraries ditributed with RawTools. All third party
// licenses are provided in accompanying files as outline in the NOTICE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawTools.Utilities.MathStats
{
    public static class ToleranceCalculator
    {
        public static bool withinTolerance(this IReadOnlyList<double> isotopes, double ion, double ppmTolerance)
        {
            double[] ppm = new double[isotopes.Count()];

            for (int i = 0; i < ppm.Length; i++)
            {
                ppm[i] = Math.Abs(ion - isotopes[i]) / isotopes[i] * 1e6;
            }

            return ppm.Min() < ppmTolerance;
        }

        public static int indexOfClosest(this IList<double> values, double target)
        {
            int index = 0;
            double diff = 1e6;
            for (int i = 0; i < values.Count(); i++)
            {
                if (Math.Abs(target - values[i]) < diff)
                {
                    diff = Math.Abs(target - values[i]);
                    index = i;
                }
            }
            return index;
        }
    }
}
