using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawTools.WorkFlows;
using RawTools.Data.Collections;
using RawTools.Data.Containers;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.BackgroundSubtraction;
using ThermoFisher.CommonCore.Data.Business;

namespace RawTools.Algorithms.MatchBewteen
{
    static class SearchAverageScans
    {
        public static void RunSearch(WorkflowParameters parameters, List<(int scans1, int scans2)> scans, IRawDataPlus rawFile1, IRawDataPlus rawFile2,
            string outputFile, List<Ms1FeatureCollection> features, List<CentroidStreamCollection> centroids, List<SegmentScanCollection> segmentScans,
            List<RetentionTimeCollection> retentionTimes, List<PrecursorMassCollection> precursorMasses,
            List<TrailerExtraCollection> trailerExtras, MethodDataContainer methodData, List<string> rawFileNames)
        {
            
        }

        public static SimpleCentroid ScanAverager(SimpleCentroid scan1, SimpleCentroid scan2, double resolution)
        {
            double[] combinedMasses = new double[scan1.Masses.Count() + scan2.Masses.Count()];
            double[] combinedIntensities = new double[scan1.Intensities.Count() + scan2.Intensities.Count()];

            int i1 = 0;
            int i2 = 0;


            // combine the masses and intensities into a single list
            for (int i = 0; i < combinedMasses.Length; i++)
            {
                if (scan1.Masses[i1] < scan2.Masses[i2])
                {
                    combinedMasses[i] = scan1.Masses[i1];
                    combinedIntensities[i] = scan1.Intensities[i1];
                    i1++;
                }
                else if (scan1.Masses[i1] > scan2.Masses[i2])
                {
                    combinedMasses[i] = scan1.Masses[i2];
                    combinedIntensities[i] = scan1.Intensities[i2];
                    i2++;
                }
                else if (scan1.Masses[i1] == scan2.Masses[i2])
                {
                    combinedMasses[i] = scan1.Masses[i1];
                    combinedIntensities[i] = scan1.Intensities[i1];
                    i1++;
                    combinedMasses[i] = scan1.Masses[i2];
                    combinedIntensities[i] = scan1.Intensities[i2];
                    i2++;
                }
            }
        }
    }
}
