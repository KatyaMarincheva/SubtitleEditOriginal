// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PacketizedElementaryStream.cs" company="">
//   
// </copyright>
// <summary>
//   http://www.mpucoder.com/DVD/pes-hdr.html
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.IO;

    /// <summary>
    /// http://www.mpucoder.com/DVD/pes-hdr.html
    /// </summary>
    public class PacketizedElementaryStream
    {
        /// <summary>
        /// The header length.
        /// </summary>
        public const int HeaderLength = 6;

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
        /// Initializes a new instance of the <see cref="PacketizedElementaryStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public PacketizedElementaryStream(byte[] buffer, int index)
        {
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

                // string bString = Helper.GetBinaryString(buffer, tempIndex, 5);
                // bString = bString.Substring(4, 3) + bString.Substring(8, 15) + bString.Substring(24, 15);
                // PresentationTimestamp = Convert.ToUInt64(bString, 2);
                tempIndex += 5;
            }

            if (this.PresentationTimestampDecodeTimestampFlags == Helper.B00000011)
            {
                // string bString = Helper.GetBinaryString(buffer, tempIndex, 5);
                // bString = bString.Substring(4, 3) + bString.Substring(8, 15) + bString.Substring(24, 15);
                // DecodeTimestamp = Convert.ToUInt64(bString, 2);
                this.DecodeTimestamp = (ulong)buffer[tempIndex + 4] >> 1;
                this.DecodeTimestamp += (ulong)buffer[tempIndex + 3] << 7;
                this.DecodeTimestamp += (ulong)(buffer[tempIndex + 2] & Helper.B11111110) << 14;
                this.DecodeTimestamp += (ulong)buffer[tempIndex + 1] << 22;
                this.DecodeTimestamp += (ulong)(buffer[tempIndex + 0] & Helper.B00001110) << 29;
            }

            int dataIndex = index + this.HeaderDataLength + 24 - Mpeg2Header.Length;
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

            this._dataBuffer = new byte[dataSize];
            Buffer.BlockCopy(buffer, dataIndex, this._dataBuffer, 0, dataSize);
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