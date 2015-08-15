namespace Nikse.SubtitleEdit.Logic.ContainerFormats.MaterialExchangeFormat
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class MxfParser
    {
        private readonly List<string> _subtitleList = new List<string>();

        private long _startPosition;

        public MxfParser(string fileName)
        {
            this.FileName = fileName;
            using (FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.ParseMxf(fs);
            }
        }

        public MxfParser(Stream stream)
        {
            this.FileName = null;
            this.ParseMxf(stream);
        }

        public string FileName { get; private set; }

        public bool IsValid { get; private set; }

        public List<string> GetSubtitles()
        {
            return this._subtitleList;
        }

        private void ParseMxf(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            this.ReadHeaderPartitionPack(stream);
            if (this.IsValid)
            {
                long length = stream.Length;
                long next = this._startPosition;
                while (next + 20 < length)
                {
                    stream.Seek(next, SeekOrigin.Begin);
                    KlvPacket klv = new KlvPacket(stream);

                    // Console.WriteLine();
                    // Console.WriteLine("Key: " + klv.DisplayKey);
                    // Console.WriteLine("Type: " + klv.IdentifyerType);
                    // Console.WriteLine("Total size: " + klv.TotalSize);
                    // Console.WriteLine("Data position: " + klv.DataPosition);
                    // if (klv.IdentifyerType == KeyIdentifier.PartitionPack)
                    // Console.WriteLine("Partition status: " + klv.PartionStatus);
                    next += klv.TotalSize;

                    if (klv.IdentifierType == KeyIdentifier.EssenceElement && klv.DataSize < 500000)
                    {
                        stream.Seek(klv.DataPosition, SeekOrigin.Begin);
                        byte[] buffer = new byte[klv.DataSize];
                        stream.Read(buffer, 0, buffer.Length);
                        string s = Encoding.UTF8.GetString(buffer);
                        if (this.IsSubtitle(s))
                        {
                            this._subtitleList.Add(s);
                        }
                    }
                }
            }
        }

        private bool IsSubtitle(string s)
        {
            if (s.Contains("\0"))
            {
                return false;
            }

            if (!s.Contains("xml") && !s.Contains(" --> ") && !s.Contains("00:00"))
            {
                return false;
            }

            List<string> list = new List<string>();
            foreach (string line in s.Replace(Environment.NewLine, "\r").Replace("\n", "\r").Split('\r'))
            {
                list.Add(line);
            }

            Subtitle subtitle = new Subtitle();
            return subtitle.ReloadLoadSubtitle(list, null) != null;
        }

        private void ReadHeaderPartitionPack(Stream stream)
        {
            this.IsValid = false;
            byte[] buffer = new byte[65536];
            int count = stream.Read(buffer, 0, buffer.Length);
            if (count < 100)
            {
                return;
            }

            this._startPosition = 0;

            for (int i = 0; i < count - 11; i++)
            {
                // Header Partition PackId = 06 0E 2B 34 02 05 01 01 0D 01 02
                if (buffer[i + 00] == 0x06 && // OID
                    buffer[i + 01] == 0x0E && // payload is 14 bytes
                    buffer[i + 02] == 0x2B && // 0x2B+34 lookup bytes
                    buffer[i + 03] == 0x34 && buffer[i + 04] == 0x02 && buffer[i + 05] == 0x05 && buffer[i + 06] == 0x01 && buffer[i + 07] == 0x01 && buffer[i + 08] == 0x0D && buffer[i + 09] == 0x01 && buffer[i + 10] == 0x02)
                {
                    this._startPosition = i;
                    this.IsValid = true;
                    break;
                }
            }
        }
    }
}