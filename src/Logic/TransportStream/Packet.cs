// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Packet.cs" company="">
//   
// </copyright>
// <summary>
//   MPEG transport stream packet
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System;

    /// <summary>
    /// MPEG transport stream packet
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// ID byte of TS Packet
        /// </summary>
        public const byte SynchronizationByte = 0x47; // 74 decimal, or 01000111 binary

        /// <summary>
        /// Null packets can ensure that the stream maintains a constant bitrate. Null packets is to be ignored
        /// </summary>
        public const int NullPacketId = 0x1FFF;

        /// <summary>
        /// Program Association Table: lists all programs available in the transport stream.
        /// </summary>
        public const int ProgramAssociationTablePacketId = 0;

        /// <summary>
        ///
        /// </summary>
        public const int ConditionalAccessTablePacketId = 1;

        /// <summary>
        ///
        /// </summary>
        public const int TransportStreamDescriptionTablePacketId = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="packetBuffer">
        /// The packet buffer.
        /// </param>
        public Packet(byte[] packetBuffer)
        {
            this.TransportErrorIndicator = 1 == packetBuffer[1] >> 7; // Set by demodulator if can't correct errors in the stream, to tell the demultiplexer that the packet has an uncorrectable error
            this.PayloadUnitStartIndicator = 1 == ((packetBuffer[1] & 64) >> 6); // and with 01000000 to get second byte - 1 means start of PES data or PSI otherwise zero
            this.TransportPriority = 1 == ((packetBuffer[1] & 32) >> 5); // and with 00100000 to get third byte - 1 means higher priority than other packets with the same PID
            this.PacketId = (packetBuffer[1] & 31) * 256 + packetBuffer[2]; // and with 00011111 to get last 5 bytes
            this.ScramblingControl = packetBuffer[3] >> 6; // '00' = Not scrambled.   The following per DVB spec:[12]   '01' = Reserved for future use,   '10' = Scrambled with even key,   '11' = Scrambled with odd key
            this.AdaptationFieldControl = (packetBuffer[3] & 48) >> 4; // and with 00110000, 01 = no adaptation fields (payload only), 10 = adaptation field only, 11 = adaptation field and payload
            this.ContinuityCounter = packetBuffer[3] & 15;
            this.AdaptionFieldLength = this.AdaptationFieldControl > 1 ? (0xFF & packetBuffer[4]) + 1 : 0;

            if (this.AdaptationFieldControl == Helper.B00000010 || this.AdaptationFieldControl == Helper.B00000011)
            {
                this.AdaptationField = new AdaptationField(packetBuffer);
            }

            if (this.AdaptationFieldControl == Helper.B00000001 || this.AdaptationFieldControl == Helper.B00000011)
            {
                // Payload exists -  binary '01' || '11'
                int payloadStart = 4;
                if (this.AdaptationField != null)
                {
                    payloadStart += 1 + this.AdaptationField.Length;
                }

                if (this.PacketId == ProgramAssociationTablePacketId)
                {
                    // PAT = Program Association Table: lists all programs available in the transport stream.
                    this.ProgramAssociationTable = new ProgramAssociationTable(packetBuffer, payloadStart + 1); // TODO: What index?
                }

                // Save payload
                this.Payload = new byte[packetBuffer.Length - payloadStart];
                Buffer.BlockCopy(packetBuffer, payloadStart, this.Payload, 0, this.Payload.Length);
            }
        }

        /// <summary>
        /// Set by demodulator if can't correct errors in the stream, to tell the demultiplexer that the packet has an uncorrectable error
        /// </summary>
        public bool TransportErrorIndicator { get; set; }

        /// <summary>
        /// Start of PES data or PSI
        /// </summary>
        public bool PayloadUnitStartIndicator { get; set; }

        /// <summary>
        /// Higher priority than other packets with the same PID
        /// </summary>
        public bool TransportPriority { get; set; }

        /// <summary>
        /// Program Identifier
        /// </summary>
        public int PacketId { get; set; }

        /// <summary>
        /// 1 = Reserved for future use, 10 = Scrambled with even key,  11 = Scrambled with odd key
        /// </summary>
        public int ScramblingControl { get; set; }

        /// <summary>
        /// 1 = no adaptation fields (payload only), 10 = adaptation field only, 11 = adaptation field and payload
        /// </summary>
        public int AdaptationFieldControl { get; set; }

        /// <summary>
        /// Incremented only when a payload is present (AdaptationFieldExist = 10 or 11). Starts at 0.
        /// </summary>
        public int ContinuityCounter { get; set; }

        /// <summary>
        /// Gets or sets the adaption field length.
        /// </summary>
        public int AdaptionFieldLength { get; set; }

        /// <summary>
        /// Gets the adaptation field.
        /// </summary>
        public AdaptationField AdaptationField { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is null packet.
        /// </summary>
        public bool IsNullPacket
        {
            get
            {
                return this.PacketId == NullPacketId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is program association table.
        /// </summary>
        public bool IsProgramAssociationTable
        {
            get
            {
                return this.PacketId == ProgramAssociationTablePacketId;
            }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public byte[] Payload { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is private stream 1.
        /// </summary>
        public bool IsPrivateStream1
        {
            get
            {
                if (this.Payload == null || this.Payload.Length < 4)
                {
                    return false;
                }

                return this.Payload[0] == 0 && this.Payload[1] == 0 && this.Payload[2] == 1 && this.Payload[3] == 0xbd; // 0xbd == 189 - MPEG-2 Private stream 1 (non MPEG audio, subpictures)
            }
        }

        /// <summary>
        /// Gets a value indicating whether is video stream.
        /// </summary>
        public bool IsVideoStream
        {
            get
            {
                if (this.Payload == null || this.Payload.Length < 4)
                {
                    return false;
                }

                return this.Payload[0] == 0 && this.Payload[1] == 0 && this.Payload[2] == 1 && this.Payload[3] >= 0xE0 && this.Payload[3] < 0xF0;
            }
        }

        /// <summary>
        /// Gets the program association table.
        /// </summary>
        public ProgramAssociationTable ProgramAssociationTable { get; private set; }
    }
}