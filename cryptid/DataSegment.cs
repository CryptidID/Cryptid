#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cryptid.Exceptions;
using Cryptid.Utils;

#endregion

namespace Cryptid {
    /// <summary>
    ///     Segments data into segments of a given size
    /// </summary>
    public class DataSegment {
        /// <summary>
        ///     The minimum length of a Factom header
        /// </summary>
        public const int ConstantHeaderLength = 33;

        /// <summary>
        ///     The default _maxSegmentLength
        /// </summary>
        public const int DefaultMaxSegmentLength = 10240 - ConstantHeaderLength;

        /// <summary>
        ///     The record identier for this record
        /// </summary>
        public static readonly byte[] DataSegmentPrefix = {0x0, 0x11, 0x22, 0x33, 0x44, 0x0};

        /// <summary>
        ///     The maximum amount of real data that can be stored in this segment
        /// </summary>
        private readonly int _maxDataLength;

        /// <summary>
        ///     The max length of a data segment
        /// </summary>
        private readonly int _maxSegmentLength;

        /// <summary>
        ///     Create a new DataSegment
        /// </summary>
        /// <param name="data">The data to store in this segment</param>
        /// <param name="currSegment">The index of the current segment</param>
        /// <param name="maxSegments">The number of segments in this segments sequence</param>
        /// <param name="maxSegmentLength">The maximum data length for each segment</param>
        public DataSegment(byte[] data, ushort currSegment, ushort maxSegments,
            int maxSegmentLength = DefaultMaxSegmentLength) {
            _maxSegmentLength = maxSegmentLength;
            _maxDataLength = GetMaxDataLength(maxSegmentLength);

            if (data.Length > _maxDataLength)
                throw new DataSegmentOverflowException("Attempted to pack " + data.Length +
                                                       " bytes in a segment that can only hold " +
                                                       _maxDataLength);

            CurrentSegment = currSegment;
            MaxSegments = maxSegments;
            Data = data;
        }

        /// <summary>
        ///     The data in this segment
        /// </summary>
        public byte[] Data { get; private set; }


        /// <summary>
        ///     The number of the current segment in the sequence
        /// </summary>
        public ushort CurrentSegment { get; private set; }

        /// <summary>
        ///     The amount of segments in the sequence
        /// </summary>
        public ushort MaxSegments { get; private set; }


        public static int GetMaxDataLength(int maxSegmentLength) {
            return maxSegmentLength - (DataSegmentPrefix.Length - sizeof (ushort)*2);
        }

        /// <summary>
        ///     Pack the record to storable data
        /// </summary>
        /// <returns>The packed DataSegment bytes</returns>
        public byte[] Pack() {
            var data = DataSegmentPrefix;
            data = data.Concat(BitConverter.GetBytes(CurrentSegment)).ToArray();
            data = data.Concat(BitConverter.GetBytes(MaxSegments)).ToArray();
            data = data.Concat(Data).ToArray();
            return data;
        }

        /// <summary>
        ///     Unpack storable data to a DataSegment
        /// </summary>
        /// <param name="packed">The packed DataSegment bytes</param>
        /// <returns>The record object</returns>
        public static DataSegment Unpack(byte[] packed) {
            var currSegment =
                BitConverter.ToUInt16(
                    Arrays.CopyOfRange(packed, DataSegmentPrefix.Length, DataSegmentPrefix.Length + sizeof (ushort)), 0);
            var maxSegments =
                BitConverter.ToUInt16(
                    Arrays.CopyOfRange(packed, DataSegmentPrefix.Length + sizeof (ushort),
                        DataSegmentPrefix.Length + sizeof (ushort)*2), 0);
            var data = Arrays.CopyOfRange(packed, DataSegmentPrefix.Length + sizeof (ushort)*2, packed.Length);

            return new DataSegment(data, currSegment, maxSegments);
        }

        /// <summary>
        ///     Packs variable length data into a list of segments
        /// </summary>
        /// <param name="data">The data to segmentize</param>
        /// <param name="maxSegmentLength">The maximum length of each segment</param>
        /// <param name="firstSegmentLength">The maximum length of the first segment (defaults to DefaultMaxSegmentLength)</param>
        /// <returns>A list of DataSegments for the data</returns>
        public static List<DataSegment> Segmentize(byte[] data, int maxSegmentLength = DefaultMaxSegmentLength,
            int firstSegmentLength = DefaultMaxSegmentLength) {
            var segments = new List<DataSegment>();

            IEnumerable<byte[]> slices;
            if (data.Length >= firstSegmentLength) {
                slices =
                    Arrays.CopyOfRange(data, firstSegmentLength, data.Length).Slices(GetMaxDataLength(maxSegmentLength));
                segments.Add(new DataSegment(Arrays.CopyOfRange(data, 0, firstSegmentLength), 0,
                    (ushort) (slices.Count() + 1)));
            }
            else {
                segments.Add(new DataSegment(data, 0, 1));
                return segments;
            }

            ushort i = 1;
            foreach (var segmentData in slices) {
                segments.Add(new DataSegment(segmentData, i++, (ushort) (slices.Count() + 1)));
            }

            return segments;
        }

        /// <summary>
        ///     Unpacks a list of segments into a byte array which contain the entirety of the original data
        /// </summary>
        /// <param name="segments">A list containing all the data segments for the sequence</param>
        /// <returns>The desegmented data</returns>
        public static byte[] Desegmentize(List<DataSegment> segments) {
            segments = segments.OrderBy(x => x.CurrentSegment).ToList();
            using (var ms = new MemoryStream()) {
                foreach (var segment in segments) {
                    ms.Write(segment.Data, 0, segment.Data.Length);
                }
                return ms.ToArray();
            }
        }
    }
}