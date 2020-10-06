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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using RawTools.Data.Containers;
using RawTools.Data.Extraction;
using System.Xml.Serialization;
using System.IO;

namespace RawTools.Data.Collections
{
    class CentroidStreamCollection : Dictionary<int, CentroidStreamData> { }

    class SegmentScanCollection : Dictionary<int, SegmentedScanData> { }

    class TrailerExtraCollection : Dictionary<int, TrailerExtraData> { }

    class PrecursorScanCollection : Dictionary<int, PrecursorScanData>
    {
        public PrecursorScanCollection()
        { }

        public PrecursorScanCollection(Dictionary<int, PrecursorScanData> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public PrecursorScanCollection(ConcurrentDictionary<int, PrecursorScanData> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }
    }

    class PrecursorMassCollection : Dictionary<int, PrecursorMassData> { }

    class RetentionTimeCollection : Dictionary<int, double> { }

    class ScanDependentsCollections : Dictionary<int, IScanDependents>
    {
        public ScanDependentsCollections()
        { }

        public ScanDependentsCollections(Dictionary<int, IScanDependents> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public ScanDependentsCollections(ConcurrentDictionary<int, IScanDependents> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }
    }

    class PrecursorPeakCollection : Dictionary<int, PrecursorPeakData>
    {
        public Containers.PeakShape PeakShapeMedians;
    }

    class QuantDataCollection : Dictionary<int, QuantData>
    {
        public string LabelingReagents;
        public string TmtImpurityTable;

        public void CorrectTmtPurity()
        {
            List<string> Table = new List<string>();
            using (StreamReader table = new StreamReader(this.TmtImpurityTable))
            {
                while (!table.EndOfStream)
                {
                    Table.Add(table.ReadLine());
                }
            }
            List<string> Header = Table[0].Split(',').ToList();
            int n_reporters = Table.Count - 1;
            List<List<float>> CorrectionMatrix = new List<List<float>>();
            for (int i = 0; i < n_reporters; i++)
            {
                CorrectionMatrix[i] = new List<float>();
                for (int j = 3; j < 8; j++)
                {
                    CorrectionMatrix[i].Add(Table[])
                }
            }
        }
    }

    class ScanMetaDataCollectionDDA
    {
        public Dictionary<int, double> DutyCycle, FillTime, SummedIntensity, Ms1IsolationInterference, FractionConsumingTop80PercentTotalIntensity;
        public Dictionary<int, Distribution> IntensityDistribution;
        public Dictionary<int, int> MS2ScansPerCycle;

        public ScanMetaDataCollectionDDA()
        {
            DutyCycle = FillTime = Ms1IsolationInterference = SummedIntensity =
                FractionConsumingTop80PercentTotalIntensity = new Dictionary<int, double>();
            IntensityDistribution = new Dictionary<int, Distribution>();
            MS2ScansPerCycle = new Dictionary<int, int>();
        }
    }

    class ScanMetaDataCollectionDIA
    {
        public Dictionary<int, double> DutyCycle, SummedIntensity, FillTime, FractionConsumingTop80PercentTotalIntensity;
        public Dictionary<int, Distribution> IntensityDistribution;

        public ScanMetaDataCollectionDIA()
        {
            SummedIntensity = DutyCycle = FillTime = FractionConsumingTop80PercentTotalIntensity = new Dictionary<int, double>();
            IntensityDistribution = new Dictionary<int, Distribution>();
        }
    }

    class ScanEventReactionCollection: Dictionary<int, IReaction> { }

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();

            }

        }
        #endregion
    }

    class PsmDataCollection: Dictionary<int, PsmData>
    {
    }

    class MultiRunFeatureCollection: Dictionary<int, MultiRunFeature>
    { }

    class MultiRunFeature: Dictionary<string, PsmData>
    {
        public bool IdIn1;
        public bool IdIn2;
        public bool FoundIn1;
        public bool FoundIn2;
        public bool ConfirmSeqMatch;
        public Dictionary<(int scan1, int scan2), double> AllScores;
        public Dictionary<(int scan1, int scan2), double> LowScores;

        public MultiRunFeature()
        {
            IdIn1 = false;
            IdIn2 = false;
            FoundIn1 = false;
            FoundIn2 = false;
            ConfirmSeqMatch = false;
            AllScores = new Dictionary<(int scan1, int scan2), double>();
            LowScores = new Dictionary<(int scan1, int scan2), double>();
        }
    }
    
    class Ms1FeatureCollection: Dictionary<int, Ms1Feature>
    { }
    
    class FeatureMatchDataCollection : Dictionary<int, MultiRunFeatureMatchData>
    {
        Dictionary<int, string> RunNameKeys;

        public FeatureMatchDataCollection()
        {
            RunNameKeys = new Dictionary<int, string>();
        }

        public void AddFeature(Ms1Feature feature)
        {
            
        }
    }

    class GroupedFeatureCollection: Dictionary<Guid, GroupedMs1Feature>
    { }
}