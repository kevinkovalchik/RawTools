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
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using RawTools.Data.Containers;
using RawTools.Data.Extraction;
using System.Xml.Serialization;

namespace RawTools.Data.Collections
{
    //class MasterDataCollection:

    class RawDataCollection
    {
        public string rawFileName, instrument;
        public ScanIndex scanIndex;
        public Dictionary<int, CentroidStreamData> centroidStreams;
        public Dictionary<int, SegmentedScanData> segmentedScans;
        public Dictionary<int, TrailerExtraData> trailerExtras;
        public Dictionary<int, PrecursorScanData> precursorScans;
        public Dictionary<int, PrecursorMassData> precursorMasses;
        public Dictionary<int, double> retentionTimes;
        public QuantDataCollection quantData;
        public ScanMetaDataCollection metaData;
        public PrecursorPeakDataCollection peakData;
        public HashSet<Operations> Performed = new HashSet<Operations>();
        public Containers.MethodData methodData;
        public bool isExactive, isBoxCar;

        public RawDataCollection(IRawDataPlus rawFile)
        {
            rawFileName = rawFile.FileName;
            instrument = rawFile.GetInstrumentData().Name;
            isExactive = instrument.ToLower().Contains("exactive");
            isBoxCar = rawFile.GetScanEventForScanNumber(1).MassRangeCount > 1;
            centroidStreams = new Dictionary<int, CentroidStreamData>();
            segmentedScans = new Dictionary<int, SegmentedScanData>();
            trailerExtras = new Dictionary<int, TrailerExtraData>();
            precursorScans = new Dictionary<int, PrecursorScanData>();
            precursorMasses = new Dictionary<int, PrecursorMassData>();
            retentionTimes = new Dictionary<int, double>();
            methodData = new Containers.MethodData();
            quantData = new QuantDataCollection();
            metaData = new ScanMetaDataCollection();
            peakData = new PrecursorPeakDataCollection();

            this.ExtractScanIndex(rawFile);
            this.ExtractMethodData(rawFile);
        }
    }

    class QuantDataCollection : Dictionary<int, QuantData>
    {
        public string LabelingReagents;
    }

    class ScanMetaDataCollection : Dictionary<int, ScanMetaData>
    { }

    class PrecursorPeakDataCollection : Dictionary<int, PrecursorPeakData>
    {
        public Containers.PeakShape PeakShapeMedians;
    }

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

}