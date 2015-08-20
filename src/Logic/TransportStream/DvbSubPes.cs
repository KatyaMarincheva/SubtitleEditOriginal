// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DvbSubPes.cs" company="">
//   
// </copyright>
// <summary>
//   The dvb sub pes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// The dvb sub pes.
    /// </summary>
    public class DvbSubPes
    {
        /// <summary>
        /// The header length.
        /// </summary>
        public const int HeaderLength = 6;

        /// <summary>
        /// The mpeg 2 header length.
        /// </summary>
        public const int Mpeg2HeaderLength = 14;

        /// <summary>
        /// The _data buffer.
        /// </summary>
        private readonly byte[] _dataBuffer;

        /// <summary>
        /// The additional copy info flag.
        /// </summary>
        public readonly int AdditionalCopyInfoFlag;

        /// <summary>
        /// The copyright.
        /// </summary>
        public readonly int Copyright;

        /// <summary>
        /// The crc flag.
        /// </summary>
        public readonly int CrcFlag;

        /// <summary>
        /// The data alignment indicator.
        /// </summary>
        public readonly int DataAlignmentIndicator;

        /// <summary>
        /// The decode timestamp.
        /// </summary>
        public readonly ulong? DecodeTimestamp;

        /// <summary>
        /// The dsm trick mode flag.
        /// </summary>
        public readonly int DsmTrickModeFlag;

        /// <summary>
        /// The elementary stream clock reference flag.
        /// </summary>
        public readonly int ElementaryStreamClockReferenceFlag;

        /// <summary>
        /// The es rate flag.
        /// </summary>
        public readonly int EsRateFlag;

        /// <summary>
        /// The extension flag.
        /// </summary>
        public readonly int ExtensionFlag;

        /// <summary>
        /// The header data length.
        /// </summary>
        public readonly int HeaderDataLength;

        /// <summary>
        /// The length.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The original or copy.
        /// </summary>
        public readonly int OriginalOrCopy;

        /// <summary>
        /// The presentation timestamp.
        /// </summary>
        public readonly ulong? PresentationTimestamp;

        /// <summary>
        /// The presentation timestamp decode timestamp flags.
        /// </summary>
        public readonly int PresentationTimestampDecodeTimestampFlags;

        /// <summary>
        /// The priority.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// The scrambling control.
        /// </summary>
        public readonly int ScramblingControl;

        /// <summary>
        /// The start code.
        /// </summary>
        public readonly uint StartCode;

        /// <summary>
        /// The stream id.
        /// </summary>
        public readonly int StreamId;

        /// <summary>
        /// The sub picture stream id.
        /// </summary>
        public readonly int? SubPictureStreamId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DvbSubPes"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public DvbSubPes(byte[] buffer, int index)
        {
            if (buffer.Length < 9)
            {
                return;
            }

            this.StartCode = Helper.GetEndian(buffer, index + 0, 3);
            this.StreamId = buffer[index + 3];
            this.Length = Helper.GetEndianWord(buffer, index + 4);

            this.ScramblingControl = (buffer[index + 6] >> 4) & Helper.B00000011;
            this.Priority = buffer[index + 6] & Helper.B00001000;
            this.DataAlignmentIndicator = buffer[index + 6] & Helper.B00000100;
            this.Copyright = buffer[index + 6] & Helper.B00000010;
            this.OriginalOrCopy = buffer[index + 6] & Helper.B00000001;
            this.PresentationTimestampDecodeTimestampFlags = buffer[index + 7] >> 6;
            this.ElementaryStreamClockReferenceFlag = buffer[index + 7] & Helper.B00100000;
            this.EsRateFlag = buffer[index + 7] & Helper.B00010000;
            this.DsmTrickModeFlag = buffer[index + 7] & Helper.B00001000;
            this.AdditionalCopyInfoFlag = buffer[index + 7] & Helper.B00000100;
            this.CrcFlag = buffer[index + 7] & Helper.B00001000;
            this.ExtensionFlag = buffer[index + 7] & Helper.B00000010;

            this.HeaderDataLength = buffer[index + 8];

            if (this.StreamId == 0xBD)
            {
                // 10111101 binary = 189 decimal = 0xBD hex -> private_stream_1
                int id = buffer[index + 9 + this.HeaderDataLength];
                if (id >= 0x20 && id <= 0x40)
                {
                    // x3f 0r x40 ?
                    this.SubPictureStreamId = id;
                }
            }

            int tempIndex = index + 9;
            if (this.PresentationTimestampDecodeTimestampFlags == Helper.B00000010 || this.PresentationTimestampDecodeTimestampFlags == Helper.B00000011)
            {
                this.PresentationTimestamp = (ulong)buffer[tempIndex + 4] >> 1;
                this.PresentationTimestamp += (ulong)buffer[tempIndex + 3] << 7;
                this.PresentationTimestamp += (ulong)(buffer[tempIndex + 2] & Helper.B11111110) << 14;
                this.PresentationTimestamp += (ulong)buffer[tempIndex + 1] << 22;
                this.PresentationTimestamp += (ulong)(buffer[tempIndex + 0] & Helper.B00001110) << 29;
            }

            if (this.PresentationTimestampDecodeTimestampFlags == Helper.B00000011)
            {
                this.DecodeTimestamp = (ulong)buffer[tempIndex + 4] >> 1;
                this.DecodeTimestamp += (ulong)buffer[tempIndex + 3] << 7;
                this.DecodeTimestamp += (ulong)(buffer[tempIndex + 2] & Helper.B11111110) << 14;
                this.DecodeTimestamp += (ulong)buffer[tempIndex + 1] << 22;
                this.DecodeTimestamp += (ulong)(buffer[tempIndex + 0] & Helper.B00001110) << 29;
            }

            int dataIndex = index + this.HeaderDataLength + 24 - Mpeg2HeaderLength;
            int dataSize = this.Length - (4 + this.HeaderDataLength);

            if (dataSize < 0 || (dataSize + dataIndex > buffer.Length))
            {
                // to fix bad subs...
                dataSize = buffer.Length - dataIndex;
                if (dataSize < 0)
                {
                    return;
                }
            }

            this._dataBuffer = new byte[dataSize + 1];
            Buffer.BlockCopy(buffer, dataIndex - 1, this._dataBuffer, 0, this._dataBuffer.Length); // why subtract one from dataIndex???
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DvbSubPes"/> class.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public DvbSubPes(int index, byte[] buffer)
        {
            int start = index;
            this.Length = index + 1;

            if (index + 9 >= buffer.Length)
            {
                return;
            }

            if (buffer[0 + index] != 0x20)
            {
                return;
            }

            if (buffer[1 + index] != 0)
            {
                return;
            }

            this.SubtitleSegments = new List<SubtitleSegment>();
            this.ClutDefinitions = new List<ClutDefinitionSegment>();
            this.RegionCompositions = new List<RegionCompositionSegment>();
            this.PageCompositions = new List<PageCompositionSegment>();
            this.ObjectDataList = new List<ObjectDataSegment>();

            // Find length of segments
            index = start + 2;
            var ss = new SubtitleSegment(buffer, index);
            while (ss.SyncByte == Helper.B00001111)
            {
                this.SubtitleSegments.Add(ss);
                index += 6 + ss.SegmentLength;
                if (index + 6 < buffer.Length)
                {
                    ss = new SubtitleSegment(buffer, index);
                }
                else
                {
                    ss.SyncByte = Helper.B11111111;
                }
            }

            this.Length = index;
            int size = index - start;
            this._dataBuffer = new byte[size];
            Buffer.BlockCopy(buffer, start, this._dataBuffer, 0, this._dataBuffer.Length);

            // Parse segments
            index = 2;
            ss = new SubtitleSegment(this._dataBuffer, index);
            while (ss.SyncByte == Helper.B00001111)
            {
                this.SubtitleSegments.Add(ss);
                if (ss.ClutDefinition != null)
                {
                    this.ClutDefinitions.Add(ss.ClutDefinition);
                }
                else if (ss.RegionComposition != null)
                {
                    this.RegionCompositions.Add(ss.RegionComposition);
                }
                else if (ss.PageComposition != null)
                {
                    this.PageCompositions.Add(ss.PageComposition);
                }
                else if (ss.ObjectData != null)
                {
                    this.ObjectDataList.Add(ss.ObjectData);
                }

                index += 6 + ss.SegmentLength;
                if (index + 6 < this._dataBuffer.Length)
                {
                    ss = new SubtitleSegment(this._dataBuffer, index);
                }
                else
                {
                    ss.SyncByte = Helper.B11111111;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether is dvb subpicture.
        /// </summary>
        public bool IsDvbSubpicture
        {
            get
            {
                return this.SubPictureStreamId.HasValue && this.SubPictureStreamId.Value == 32;
            }
        }

        /// <summary>
        /// Gets the data identifier.
        /// </summary>
        public int DataIdentifier
        {
            get
            {
                if (this._dataBuffer == null || this._dataBuffer.Length < 2)
                {
                    return 0;
                }

                return this._dataBuffer[0];
            }
        }

        /// <summary>
        /// Gets the subtitle stream id.
        /// </summary>
        public int SubtitleStreamId
        {
            get
            {
                if (this._dataBuffer == null || this._dataBuffer.Length < 2)
                {
                    return 0;
                }

                return this._dataBuffer[1];
            }
        }

        /// <summary>
        /// Gets or sets the subtitle segments.
        /// </summary>
        public List<SubtitleSegment> SubtitleSegments { get; set; }

        /// <summary>
        /// Gets or sets the clut definitions.
        /// </summary>
        public List<ClutDefinitionSegment> ClutDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the region compositions.
        /// </summary>
        public List<RegionCompositionSegment> RegionCompositions { get; set; }

        /// <summary>
        /// Gets or sets the page compositions.
        /// </summary>
        public List<PageCompositionSegment> PageCompositions { get; set; }

        /// <summary>
        /// Gets or sets the object data list.
        /// </summary>
        public List<ObjectDataSegment> ObjectDataList { get; set; }

        /// <summary>
        /// The parse segments.
        /// </summary>
        public void ParseSegments()
        {
            if (this.SubtitleSegments != null)
            {
                return;
            }

            this.SubtitleSegments = new List<SubtitleSegment>();
            this.ClutDefinitions = new List<ClutDefinitionSegment>();
            this.RegionCompositions = new List<RegionCompositionSegment>();
            this.PageCompositions = new List<PageCompositionSegment>();
            this.ObjectDataList = new List<ObjectDataSegment>();

            int index = 2;
            var ss = new SubtitleSegment(this._dataBuffer, index);
            while (ss.SyncByte == Helper.B00001111)
            {
                this.SubtitleSegments.Add(ss);
                if (ss.ClutDefinition != null)
                {
                    this.ClutDefinitions.Add(ss.ClutDefinition);
                }
                else if (ss.RegionComposition != null)
                {
                    this.RegionCompositions.Add(ss.RegionComposition);
                }
                else if (ss.PageComposition != null)
                {
                    this.PageCompositions.Add(ss.PageComposition);
                }
                else if (ss.ObjectData != null)
                {
                    this.ObjectDataList.Add(ss.ObjectData);
                }

                index += 6 + ss.SegmentLength;
                if (index + 6 < this._dataBuffer.Length)
                {
                    ss = new SubtitleSegment(this._dataBuffer, index);
                }
                else
                {
                    ss.SyncByte = Helper.B11111111;
                }
            }
        }

        /// <summary>
        /// The get clut definition segment.
        /// </summary>
        /// <param name="ods">
        /// The ods.
        /// </param>
        /// <returns>
        /// The <see cref="ClutDefinitionSegment"/>.
        /// </returns>
        private ClutDefinitionSegment GetClutDefinitionSegment(ObjectDataSegment ods)
        {
            foreach (RegionCompositionSegment rcs in this.RegionCompositions)
            {
                foreach (RegionCompositionSegmentObject o in rcs.Objects)
                {
                    if (o.ObjectId == ods.ObjectId)
                    {
                        foreach (ClutDefinitionSegment cds in this.ClutDefinitions)
                        {
                            if (cds.ClutId == rcs.RegionClutId)
                            {
                                return cds;
                            }
                        }
                    }
                }
            }

            if (this.ClutDefinitions.Count > 0)
            {
                return this.ClutDefinitions[0];
            }

            return null; // TODO: Return default clut
        }

        /// <summary>
        /// The get image position.
        /// </summary>
        /// <param name="ods">
        /// The ods.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        public Point GetImagePosition(ObjectDataSegment ods)
        {
            if (this.SubtitleSegments == null)
            {
                this.ParseSegments();
            }

            var p = new Point(0, 0);

            foreach (RegionCompositionSegment rcs in this.RegionCompositions)
            {
                foreach (RegionCompositionSegmentObject o in rcs.Objects)
                {
                    if (o.ObjectId == ods.ObjectId)
                    {
                        foreach (PageCompositionSegment cds in this.PageCompositions)
                        {
                            foreach (var r in cds.Regions)
                            {
                                if (r.RegionId == rcs.RegionId)
                                {
                                    p.X = r.RegionHorizontalAddress + o.ObjectHorizontalPosition;
                                    p.Y = r.RegionVerticalAddress + o.ObjectVerticalPosition;
                                    return p;
                                }
                            }
                        }

                        p.X = o.ObjectHorizontalPosition;
                        p.Y = o.ObjectVerticalPosition;
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// The get image.
        /// </summary>
        /// <param name="ods">
        /// The ods.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetImage(ObjectDataSegment ods)
        {
            if (this.SubtitleSegments == null)
            {
                this.ParseSegments();
            }

            if (ods.Image != null)
            {
                return ods.Image;
            }

            ClutDefinitionSegment cds = this.GetClutDefinitionSegment(ods);
            ods.DecodeImage(this._dataBuffer, ods.BufferIndex, cds);
            return ods.Image;
        }

        /// <summary>
        /// The get image full.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetImageFull()
        {
            if (this.SubtitleSegments == null)
            {
                this.ParseSegments();
            }

            int width = 720;
            int height = 576;

            var segments = this.SubtitleSegments;
            foreach (SubtitleSegment ss in segments)
            {
                if (ss.DisplayDefinition != null)
                {
                    width = ss.DisplayDefinition.DisplayWith;
                    height = ss.DisplayDefinition.DisplayHeight;
                }
            }

            var bmp = new Bitmap(width, height);
            foreach (var ods in this.ObjectDataList)
            {
                var odsImage = this.GetImage(ods);
                if (odsImage != null)
                {
                    var odsPoint = this.GetImagePosition(ods);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.DrawImageUnscaled(odsImage, odsPoint);
                    }
                }
            }

            return bmp;
        }

        /// <summary>
        /// The get stream id description.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetStreamIdDescription(int streamId)
        {
            if (0xC0 <= streamId && streamId < 0xE0)
            {
                return "ISO/IEC 13818-3 or ISO/IEC 11172-3 or ISO/IEC 13818-7 or ISO/IEC 14496-3 audio stream number " + (streamId & 0x1F).ToString("X4");
            }

            if (0xE0 <= streamId && streamId < 0xF0)
            {
                return "ITU-T Rec. H.262 | ISO/IEC 13818-2 or ISO/IEC 11172-2 or ISO/IEC 14496-2 video stream number " + (streamId & 0x0F).ToString("X4");
            }

            switch (streamId)
            {
                case 0xBC:
                    return "program_stream_map";
                case 0xBD:
                    return "private_stream_1";
                case 0xBE:
                    return "padding_stream";
                case 0xBF:
                    return "private_stream_2";
                case 0xF0:
                    return "ECM_stream";
                case 0xF1:
                    return "EMM_stream";
                case 0xF2:
                    return "DSMCC_stream";
                case 0xF3:
                    return "ISO/IEC_13522_stream";
                case 0xF4:
                    return "ITU-T Rec. H.222.1 type A";
                case 0xF5:
                    return "ITU-T Rec. H.222.1 type B";
                case 0xF6:
                    return "ITU-T Rec. H.222.1 type C";
                case 0xF7:
                    return "ITU-T Rec. H.222.1 type D";
                case 0xF8:
                    return "ITU-T Rec. H.222.1 type E";
                case 0xF9:
                    return "ancillary_stream";
                case 0xFA:
                    return "ISO/IEC14496-1_SL-packetized_stream";
                case 0xFB:
                    return "ISO/IEC14496-1_FlexMux_stream";
                case 0xFC:
                    return "metadata stream";
                case 0xFD:
                    return "extended_stream_id";
                case 0xFE:
                    return "reserved data stream";
                case 0xFF:
                    return "program_stream_directory";
                default:
                    return "?";
            }
        }

        /// <summary>
        /// The presentation timestamp to milliseconds.
        /// </summary>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public ulong PresentationTimestampToMilliseconds()
        {
            if (this.PresentationTimestamp.HasValue)
            {
                return (ulong)Math.Round((this.PresentationTimestamp.Value + 45.0) / 90.0);
            }

            return 0;
        }

        /// <summary>
        /// The write to stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void WriteToStream(Stream stream)
        {
            stream.Write(this._dataBuffer, 0, this._dataBuffer.Length);
        }
    }
}