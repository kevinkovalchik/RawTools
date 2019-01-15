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

namespace RawTools.Constants
{
    static class Masses
    {
        /// <summary>
        /// Mass difference between isotopes Carbon-13 and Carbon-12
        /// </summary>
        public const double C13MinusC12 = 1.00335483;

        /// <summary>
        /// Mass of a proton
        /// </summary>
        public const double Proton = 1.00727649;
    }

    static class MultiThreading
    {
        public static int ChunkSize(int TotalSize)
        {
            int processorCount = Environment.ProcessorCount;

            if (processorCount > 8) processorCount = 8;

            return TotalSize / Environment.ProcessorCount / 20 + 1;
        }

        public static ParallelOptions Options()
        {
            ParallelOptions options = new ParallelOptions();

            if (Environment.ProcessorCount > 12)
            {
                options.MaxDegreeOfParallelism = 12;
            }
            else
            {
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            }

            return options;
        }

        public static ParallelOptions Options(int MaxThreads)
        {
            ParallelOptions options = new ParallelOptions();

            if (MaxThreads > Environment.ProcessorCount)
            {
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            }
            else
            {
                options.MaxDegreeOfParallelism = MaxThreads;
            }

            return options;
        }
    }
}
