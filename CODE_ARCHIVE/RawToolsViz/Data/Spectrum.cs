using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermoFisher.CommonCore.Data.Business;

namespace RawToolsViz.Data
{
    public class Spectrum
    {
        public List<Ion> Ions { get; set; }
        public double SummedIntensity, MaximumMass, MinimumMass, MaximumIntensity;

        public Spectrum()
        {
            Ions = new List<Ion>();
        }

        public Spectrum(double[] Masses, double[] Intensities)
        {
            Ions = new List<Ion>();

            for (int i = 0; i < Masses.Length; i++)
            {
                Ions.Add(new Ion(Masses[i], Intensities[i]));
            }

            MaximumMass = (from x in Ions select x.Mass).Max();
            MinimumMass = (from x in Ions select x.Mass).Min();
            SummedIntensity = (from x in Ions select x.Intensity).Sum();
            MaximumIntensity = (from x in Ions select x.Intensity).Max();
        }

        public Spectrum(CentroidStream centroidStream)
        {
            Ions = new List<Ion>();

            for (int i = 0; i < centroidStream.Length; i++)
            {
                Ions.Add(new Ion(centroidStream.Masses[i], centroidStream.Intensities[i]));
            }

            MaximumMass = (from x in Ions select x.Mass).Max();
            MinimumMass = (from x in Ions select x.Mass).Min();
            SummedIntensity = (from x in Ions select x.Intensity).Sum();
            MaximumIntensity = (from x in Ions select x.Intensity).Max();
        }

        public Spectrum(SegmentedScan segmentedScan)
        {
            Ions = new List<Ion>();

            for (int i = 0; i < segmentedScan.PositionCount; i++)
            {
                Ions.Add(new Ion(segmentedScan.Positions[i], segmentedScan.Intensities[i]));
            }

            MaximumMass = (from x in Ions select x.Mass).Max();
            MinimumMass = (from x in Ions select x.Mass).Min();
            SummedIntensity = (from x in Ions select x.Intensity).Sum();
            MaximumIntensity = (from x in Ions select x.Intensity).Max();
        }
    }
}
