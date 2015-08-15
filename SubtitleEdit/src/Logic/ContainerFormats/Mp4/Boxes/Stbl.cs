namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;
    using Nikse.SubtitleEdit.Logic.VobSub;

    public class Stbl : Box
    {
        private readonly Mdia _mdia;

        public List<double> EndTimeCodes = new List<double>();

        public List<double> StartTimeCodes = new List<double>();

        public ulong StszSampleCount;

        public List<SubPicture> SubPictures = new List<SubPicture>();

        public List<string> Texts = new List<string>();

        public Stbl(FileStream fs, ulong maximumLength, uint timeScale, string handlerType, Mdia mdia)
        {
            this._mdia = mdia;
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "stco")
                {
                    // 32-bit - chunk offset
                    this.Buffer = new byte[this.Size - 4];
                    fs.Read(this.Buffer, 0, this.Buffer.Length);
                    int version = this.Buffer[0];
                    uint totalEntries = this.GetUInt(4);

                    uint lastOffset = 0;
                    for (int i = 0; i < totalEntries; i++)
                    {
                        uint offset = this.GetUInt(8 + i * 4);
                        if (lastOffset + 5 < offset)
                        {
                            this.ReadText(fs, offset, handlerType);
                        }

                        lastOffset = offset;
                    }
                }
                else if (this.Name == "co64")
                {
                    // 64-bit
                    this.Buffer = new byte[this.Size - 4];
                    fs.Read(this.Buffer, 0, this.Buffer.Length);
                    int version = this.Buffer[0];
                    uint totalEntries = this.GetUInt(4);

                    ulong lastOffset = 0;
                    for (int i = 0; i < totalEntries; i++)
                    {
                        ulong offset = this.GetUInt64(8 + i * 8);
                        if (lastOffset + 8 < offset)
                        {
                            this.ReadText(fs, offset, handlerType);
                        }

                        lastOffset = offset;
                    }
                }
                else if (this.Name == "stsz")
                {
                    // sample sizes
                    this.Buffer = new byte[this.Size - 4];
                    fs.Read(this.Buffer, 0, this.Buffer.Length);
                    int version = this.Buffer[0];
                    uint uniformSizeOfEachSample = this.GetUInt(4);
                    uint numberOfSampleSizes = this.GetUInt(8);
                    this.StszSampleCount = numberOfSampleSizes;
                    for (int i = 0; i < numberOfSampleSizes; i++)
                    {
                        if (12 + i * 4 + 4 < this.Buffer.Length)
                        {
                            uint sampleSize = this.GetUInt(12 + i * 4);
                        }
                    }
                }
                else if (this.Name == "stts")
                {
                    // sample table time to sample map
                    // https://developer.apple.com/library/mac/#documentation/QuickTime/QTFF/QTFFChap2/qtff2.html#//apple_ref/doc/uid/TP40000939-CH204-SW1
                    this.Buffer = new byte[this.Size - 4];
                    fs.Read(this.Buffer, 0, this.Buffer.Length);
                    int version = this.Buffer[0];
                    uint numberOfSampleTimes = this.GetUInt(4);
                    double totalTime = 0;
                    if (this._mdia.IsClosedCaption)
                    {
                        for (int i = 0; i < numberOfSampleTimes; i++)
                        {
                            uint sampleCount = this.GetUInt(8 + i * 8);
                            uint sampleDelta = this.GetUInt(12 + i * 8);
                            for (int j = 0; j < sampleCount; j++)
                            {
                                totalTime += sampleDelta / (double)timeScale;
                                if (this.StartTimeCodes.Count > 0)
                                {
                                    this.EndTimeCodes[this.EndTimeCodes.Count - 1] = totalTime - 0.001;
                                }

                                this.StartTimeCodes.Add(totalTime);
                                this.EndTimeCodes.Add(totalTime + 2.5);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numberOfSampleTimes; i++)
                        {
                            uint sampleCount = this.GetUInt(8 + i * 8);
                            uint sampleDelta = this.GetUInt(12 + i * 8);
                            totalTime += sampleDelta / (double)timeScale;
                            if (this.StartTimeCodes.Count <= this.EndTimeCodes.Count)
                            {
                                this.StartTimeCodes.Add(totalTime);
                            }
                            else
                            {
                                this.EndTimeCodes.Add(totalTime);
                            }
                        }
                    }
                }
                else if (this.Name == "stsc")
                {
                    // sample table sample to chunk map
                    this.Buffer = new byte[this.Size - 4];
                    fs.Read(this.Buffer, 0, this.Buffer.Length);
                    int version = this.Buffer[0];
                    uint numberOfSampleTimes = this.GetUInt(4);
                    for (int i = 0; i < numberOfSampleTimes; i++)
                    {
                        if (16 + i * 12 + 4 < this.Buffer.Length)
                        {
                            uint firstChunk = this.GetUInt(8 + i * 12);
                            uint samplesPerChunk = this.GetUInt(12 + i * 12);
                            uint sampleDescriptionIndex = this.GetUInt(16 + i * 12);
                        }
                    }
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }

        private void ReadText(FileStream fs, ulong offset, string handlerType)
        {
            fs.Seek((long)offset, SeekOrigin.Begin);
            byte[] data = new byte[4];
            fs.Read(data, 0, 2);
            uint textSize = (uint)GetWord(data, 0);

            if (handlerType == "subp")
            {
                // VobSub created with Mp4Box
                if (textSize > 100)
                {
                    fs.Seek((long)offset, SeekOrigin.Begin);
                    data = new byte[textSize + 2];
                    fs.Read(data, 0, data.Length);
                    this.SubPictures.Add(new SubPicture(data)); // TODO: Where is palette?
                }
            }
            else
            {
                if (textSize == 0)
                {
                    fs.Read(data, 2, 2);
                    textSize = GetUInt(data, 0); // don't get it exactly - seems like mp4box sometimes uses 2 bytes length field (first text record only)... handbrake uses 4 bytes
                }

                if (textSize > 0 && textSize < 500)
                {
                    data = new byte[textSize];
                    fs.Read(data, 0, data.Length);
                    string text = GetString(data, 0, (int)textSize).TrimEnd();

                    if (this._mdia.IsClosedCaption)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int j = 8; j < data.Length - 3; j++)
                        {
                            string h = data[j].ToString("X2").ToLower();
                            if (h.Length < 2)
                            {
                                h = "0" + h;
                            }

                            sb.Append(h);
                            if (j % 2 == 1)
                            {
                                sb.Append(' ');
                            }
                        }

                        string hex = sb.ToString();
                        int errorCount = 0;
                        text = ScenaristClosedCaptions.GetSccText(hex, ref errorCount);
                        if (text.StartsWith('n') && text.Length > 1)
                        {
                            text = "<i>" + text.Substring(1) + "</i>";
                        }

                        if (text.StartsWith("-n"))
                        {
                            text = text.Remove(0, 2);
                        }

                        if (text.StartsWith("-N"))
                        {
                            text = text.Remove(0, 2);
                        }

                        if (text.StartsWith('-') && !text.Contains(Environment.NewLine + "-"))
                        {
                            text = text.Remove(0, 1);
                        }
                    }

                    this.Texts.Add(text.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine));
                }
                else
                {
                    this.Texts.Add(string.Empty);
                }
            }
        }
    }
}