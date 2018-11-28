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

            return TotalSize / Environment.ProcessorCount / 4 + 1;
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
