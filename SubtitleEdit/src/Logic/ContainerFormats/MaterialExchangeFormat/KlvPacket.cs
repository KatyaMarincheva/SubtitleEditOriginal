namespace Nikse.SubtitleEdit.Logic.ContainerFormats.MaterialExchangeFormat
{
    using System;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    ///     Key-Length-Value MXF package - http://en.wikipedia.org/wiki/KLV +
    ///     http://en.wikipedia.org/wiki/Material_Exchange_Format
    /// </summary>
    public class KlvPacket
    {
        private const int KeySize = 16;

        public static byte[] PartitionPack = { 0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0D, 0x01, 0x02, 0x01, 0x01, 0xff, 0xff, 0x00 }; // 0xff can have different values

        public static byte[] Preface = { 0x06, 0x0E, 0x2B, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x2F, 0x00 };

        public static byte[] EssenceElement = { 0x06, 0x0E, 0x2B, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0D, 0x01, 0x03, 0x01, 0xff, 0xff, 0xff, 0xff };

        public static byte[] OperationalPattern = { 0x06, 0x0E, 0x2B, 0x34, 0x04, 0x01, 0x01, 0x01, 0x0D, 0x01, 0x02, 0x01, 0x00, 0xff, 0xff, 0x00 };

        public static byte[] PartitionMetadata = { 0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x04, 0x04, 0x00 };

        public static byte[] StructuralMetadata = { 0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x00, 0xff, 0xff, 0x00 };

        public static byte[] DataDefinitionVideo = { 0x06, 0x0E, 0x2B, 0x34, 0x04, 0x01, 0x01, 0x01, 0x01, 0x03, 0x02, 0x02, 0x01, 0xff, 0xff, 0x00 };

        public static byte[] DataDefinitionAudio = { 0x06, 0x0E, 0x2B, 0x34, 0x04, 0x01, 0x01, 0x01, 0x01, 0x03, 0x02, 0x02, 0x02, 0xff, 0xff, 0x00 };

        public static byte[] Primer = { 0x06, 0xe, 0x2b, 0x34, 0x02, 0x05, 0x1, 0xff, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x05, 0x01, 0xff };

        public long DataPosition;

        public long DataSize;

        public byte[] Key;

        public PartitionStatus PartionStatus = PartitionStatus.Unknown;

        public long TotalSize;

        public KlvPacket(Stream stream)
        {
            // read 16-bytes key
            this.Key = new byte[KeySize];
            int read = stream.Read(this.Key, 0, this.Key.Length);
            if (read < this.Key.Length)
            {
                throw new Exception("MXF KLV packet - stream does not have 16 bytes available for key");
            }

            int lengthSize;
            this.DataSize = this.GetBasicEncodingRuleLength(stream, out lengthSize);
            this.DataPosition = stream.Position;
            this.TotalSize = KeySize + lengthSize + this.DataSize;
            if (this.Key[14] >= 1 && this.Key[14] <= 4)
            {
                this.PartionStatus = (PartitionStatus)this.Key[14];
            }
        }

        public KeyIdentifier IdentifierType
        {
            get
            {
                if (this.IsKey(PartitionPack))
                {
                    return KeyIdentifier.PartitionPack;
                }

                if (this.IsKey(Preface))
                {
                    return KeyIdentifier.Preface;
                }

                if (this.IsKey(EssenceElement))
                {
                    return KeyIdentifier.EssenceElement;
                }

                if (this.IsKey(OperationalPattern))
                {
                    return KeyIdentifier.OperationalPattern;
                }

                if (this.IsKey(PartitionMetadata))
                {
                    return KeyIdentifier.PartitionMetadata;
                }

                if (this.IsKey(StructuralMetadata))
                {
                    return KeyIdentifier.StructuralMetadata;
                }

                if (this.IsKey(DataDefinitionVideo))
                {
                    return KeyIdentifier.DataDefinitionVideo;
                }

                if (this.IsKey(DataDefinitionAudio))
                {
                    return KeyIdentifier.DataDefinitionAudio;
                }

                if (this.IsKey(Primer))
                {
                    return KeyIdentifier.DataDefinitionAudio;
                }

                return KeyIdentifier.Unknown;
            }
        }

        public string DisplayKey
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < KeySize; i++)
                {
                    sb.Append(this.Key[i].ToString("X2") + "-");
                }

                return sb.ToString().TrimEnd('-');
            }
        }

        /// <summary>
        ///     Read length - never be more than 9 bytes in size (which means max 8 bytes of payload length)
        ///     There are four kinds of encoding for the Length field: 1-byte, 2-byte, 4-byte
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytesInLength"></param>
        /// <returns></returns>
        private long GetBasicEncodingRuleLength(Stream stream, out int bytesInLength)
        {
            int first = stream.ReadByte();
            if (first > 127)
            {
                // first bit set
                bytesInLength = first & Helper.B01111111;
                if (bytesInLength > 8)
                {
                    throw new Exception("MXF KLV packet - lenght bytes > 8");
                }

                this.DataSize = 0;
                for (int i = 0; i < bytesInLength; i++)
                {
                    this.DataSize = this.DataSize * 256 + stream.ReadByte();
                }

                bytesInLength++;
                return this.DataSize;
            }

            bytesInLength = 1;
            return first;
        }

        private bool IsKey(byte[] key)
        {
            if (KeySize != key.Length)
            {
                return false;
            }

            for (int i = 0; i < KeySize; i++)
            {
                if (this.Key[i] != key[i] && key[i] != 0xff)
                {
                    return false;
                }
            }

            return true;
        }
    }
}