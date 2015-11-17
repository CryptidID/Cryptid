using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cryptid.Utils;

namespace cryptid {
    class DataSegment {
        /// <summary>
        /// The max length of a data segment
        /// </summary>
        private const int MaxSegmentLength = 10240;
        
        /// <summary>
        /// The maximum amount of real data that can be stored in this segment
        /// </summary>
        private static readonly int MaxDataLength = MaxSegmentLength - DataSegmentPrefix.Length - (sizeof(ushort) * 2);

        /// <summary>
        /// The record identier for this record
        /// </summary>
        private static readonly byte[] DataSegmentPrefix = { 0x0, 0x11, 0x22, 0x33, 0x44, 0x0 };

        /// <summary>
        /// The data in this segment
        /// </summary>
        private byte[] Data { get; set; }


        /// <summary>
        /// The number of the current segment in the sequence
        /// </summary>
        private ushort CurrentSegment { get; set; }

        /// <summary>
        /// The amount of segments in the sequence
        /// </summary>
        private ushort MaxSegments { get; set; }

        /// <summary>
        /// Create a new DataSegment
        /// </summary>
        /// <param name="currSegment">The index of the current segment</param>
        /// <param name="maxSegments">The number of segments in this segments sequence</param>
        public DataSegment(byte[] data, ushort currSegment, ushort maxSegments) {
            if(data.Length > MaxDataLength) throw new Exception("Attempted to pack " + data.Length + " bytes in a segment that can only hold " + MaxDataLength);

            CurrentSegment = currSegment;
            MaxSegments = maxSegments;
            Data = data;
        }

        /// <summary>
        /// Pack the record to storable data
        /// </summary>
        /// <returns>The packed DataSegment bytes</returns>
        public byte[] Pack() {
            byte[] data = DataSegmentPrefix;
            data = data.Concat(BitConverter.GetBytes(CurrentSegment)).ToArray();
            data = data.Concat(BitConverter.GetBytes(MaxSegments)).ToArray();
            data = data.Concat(Data).ToArray();
            return data;
        }

        /// <summary>
        /// Unpack storable data to a DataSegment
        /// </summary>
        /// <param name="packed">The packed DataSegment bytes</param>
        /// <returns>The record object</returns>
        public static DataSegment Unpack(byte[] packed) {
            ushort currSegment = BitConverter.ToUInt16(Arrays.CopyOfRange(packed, DataSegmentPrefix.Length, DataSegmentPrefix.Length + sizeof(ushort)), 0);
            ushort maxSegments = BitConverter.ToUInt16(Arrays.CopyOfRange(packed, DataSegmentPrefix.Length + sizeof(ushort), DataSegmentPrefix.Length + (sizeof(ushort) * 2)), 0);
            byte[] data = Arrays.CopyOfRange(packed, DataSegmentPrefix.Length + (sizeof (ushort)*2), packed.Length);

            return new DataSegment(data, currSegment, maxSegments);
        }

        /// <summary>
        /// Packs variable length data into a list of segments
        /// </summary>
        /// <param name="data">The data to segmentize</param>
        /// <returns>A list of DataSegments for the data</returns>
        public static List<DataSegment> Segmentize(byte[] data) {
            List<DataSegment> segments = new List<DataSegment>();
            var slices = data.Slices(MaxDataLength);

            ushort i = 0;
            foreach(byte[] segmentData in slices) {
                segments.Add(new DataSegment(segmentData, i++, (ushort) segmentData.Length));
            }

            return segments;
        }

        /// <summary>
        /// Unpacks a list of segments into a byte array which contain the entirety of the original data
        /// </summary>
        /// <param name="segments">A list containing all the data segments for the sequence</param>
        /// <returns>The desegmented data</returns>
        public static byte[] Desegmentize(List<DataSegment> segments) {
            segments = segments.OrderBy(x => x.CurrentSegment).ToList();
            using (MemoryStream ms = new MemoryStream()) {
                foreach (DataSegment segment in segments) {
                    ms.Write(segment.Data, 0, segment.Data.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
