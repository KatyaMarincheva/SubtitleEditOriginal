// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdaptationField.cs" company="">
//   
// </copyright>
// <summary>
//   The adaptation field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System;

    /// <summary>
    /// The adaptation field.
    /// </summary>
    public class AdaptationField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptationField"/> class.
        /// </summary>
        /// <param name="packetBuffer">
        /// The packet buffer.
        /// </param>
        public AdaptationField(byte[] packetBuffer)
        {
            this.Length = packetBuffer[4];
            this.DiscontinuityIndicator = 1 == packetBuffer[5] >> 7;
            this.RandomAccessIndicator = (packetBuffer[5] & 64) > 0; // and with 01000000 to get second byte
            this.ElementaryStreamPriorityIndicator = (packetBuffer[5] & 32) > 0; // and with 00100000 to get third byte
            this.PcrFlag = (packetBuffer[5] & 16) > 0; // and with 00010000 to get fourth byte
            this.OpcrFlag = (packetBuffer[5] & 8) > 0; // and with 00001000 to get fifth byte
            this.SplicingPointFlag = (packetBuffer[5] & 4) > 0; // and with 00000100 to get sixth byte
            this.TransportPrivateDataFlag = (packetBuffer[5] & 4) > 0; // and with 00000100 to get seventh byte
            this.AdaptationFieldExtensionFlag = (packetBuffer[5] & 2) > 0; // and with 00000010 to get 8th byte

            int index = 6;
            if (this.PcrFlag)
            {
                this.ProgramClockReferenceBase = (packetBuffer[index] * 256 + packetBuffer[index + 1]) << 1;
                this.ProgramClockReferenceBase += packetBuffer[index + 2] >> 7;
                this.ProgramClockReferenceExtension = (packetBuffer[index + 2] & Helper.B00000001) * 256 + packetBuffer[index + 3];
                index += 4;
            }

            if (this.OpcrFlag)
            {
                this.OriginalProgramClockReferenceBase = (packetBuffer[index] * 256 + packetBuffer[index + 1]) << 1;
                this.OriginalProgramClockReferenceBase += packetBuffer[index + 2] >> 7;
                this.OriginalProgramClockReferenceExtension = (packetBuffer[index + 2] & Helper.B00000001) * 256 + packetBuffer[index + 3];
                index += 4;
            }

            if (this.SplicingPointFlag)
            {
                this.SpliceCountdown = packetBuffer[index];
                index++;
            }

            if (this.TransportPrivateDataFlag)
            {
                this.TransportPrivateDataLength = packetBuffer[index];
                index++;
                this.TransportPrivateData = new byte[this.TransportPrivateDataLength];

                if (index + this.TransportPrivateDataLength <= packetBuffer.Length)
                {
                    Buffer.BlockCopy(packetBuffer, index, this.TransportPrivateData, 0, this.TransportPrivateDataLength);
                    index += this.TransportPrivateDataLength;
                }
            }

            if (this.AdaptationFieldExtensionFlag && index < packetBuffer.Length)
            {
                this.AdaptationFieldExtensionLength = packetBuffer[index];
            }
        }

        /// <summary>
        /// Number of bytes in the adaptation field immediately following the Length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discontinuity indicator.
        /// </summary>
        public bool DiscontinuityIndicator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether random access indicator.
        /// </summary>
        public bool RandomAccessIndicator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether elementary stream priority indicator.
        /// </summary>
        public bool ElementaryStreamPriorityIndicator { get; set; }

        /// <summary>
        /// '1' indicates that the adaptation field contains a PCR field coded in two parts
        /// </summary>
        public bool PcrFlag { get; set; }

        /// <summary>
        /// '1' indicates that the adaptation field contains an OPCR field coded in two parts
        /// </summary>
        public bool OpcrFlag { get; set; }

        /// <summary>
        /// '1' indicates that a splice countdown field shall be present in the associated adaptation field
        /// </summary>
        public bool SplicingPointFlag { get; set; }

        /// <summary>
        /// 1' indicates that the adaptation field contains one or more private data bytes
        /// </summary>
        public bool TransportPrivateDataFlag { get; set; }

        /// <summary>
        /// '1' indicates the presence of an adaptation field extension
        /// </summary>
        public bool AdaptationFieldExtensionFlag { get; set; }

        /// <summary>
        /// Gets or sets the program clock reference base.
        /// </summary>
        public int ProgramClockReferenceBase { get; set; }

        /// <summary>
        /// Gets or sets the program clock reference extension.
        /// </summary>
        public int ProgramClockReferenceExtension { get; set; }

        /// <summary>
        /// Gets or sets the original program clock reference base.
        /// </summary>
        public int OriginalProgramClockReferenceBase { get; set; }

        /// <summary>
        /// Gets or sets the original program clock reference extension.
        /// </summary>
        public int OriginalProgramClockReferenceExtension { get; set; }

        /// <summary>
        /// Gets or sets the splice countdown.
        /// </summary>
        public int SpliceCountdown { get; set; }

        /// <summary>
        /// Gets or sets the transport private data length.
        /// </summary>
        public int TransportPrivateDataLength { get; set; }

        /// <summary>
        /// Gets or sets the transport private data.
        /// </summary>
        public byte[] TransportPrivateData { get; set; }

        /// <summary>
        /// Gets or sets the adaptation field extension length.
        /// </summary>
        public int AdaptationFieldExtensionLength { get; set; }
    }
}