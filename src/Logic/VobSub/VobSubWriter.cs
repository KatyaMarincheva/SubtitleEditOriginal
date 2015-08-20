// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubWriter.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The vob sub writer.
    /// </summary>
    public class VobSubWriter : IDisposable
    {
        /// <summary>
        /// The _bottom margin.
        /// </summary>
        private readonly int _bottomMargin = 15;

        /// <summary>
        /// The _idx.
        /// </summary>
        private readonly StringBuilder _idx = new StringBuilder();

        /// <summary>
        /// The _language name.
        /// </summary>
        private readonly string _languageName = "English";

        /// <summary>
        /// The _language name short.
        /// </summary>
        private readonly string _languageNameShort = "en";

        /// <summary>
        /// The _language stream id.
        /// </summary>
        private readonly int _languageStreamId;

        /// <summary>
        /// The _left right margin.
        /// </summary>
        private readonly int _leftRightMargin = 15;

        /// <summary>
        /// The _screen height.
        /// </summary>
        private readonly int _screenHeight = 480;

        /// <summary>
        /// The _screen width.
        /// </summary>
        private readonly int _screenWidth = 720;

        /// <summary>
        /// The _sub file name.
        /// </summary>
        private readonly string _subFileName;

        /// <summary>
        /// The _use inner antialiasing.
        /// </summary>
        private readonly bool _useInnerAntialiasing = true;

        /// <summary>
        /// The _background.
        /// </summary>
        private Color _background = Color.Transparent;

        /// <summary>
        /// The _emphasis 1.
        /// </summary>
        private Color _emphasis1 = Color.Black;

        /// <summary>
        /// The _emphasis 2.
        /// </summary>
        private Color _emphasis2 = Color.FromArgb(240, Color.Black);

        /// <summary>
        /// The _pattern.
        /// </summary>
        private Color _pattern = Color.White;

        /// <summary>
        /// The _sub file.
        /// </summary>
        private FileStream _subFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubWriter"/> class.
        /// </summary>
        /// <param name="subFileName">
        /// The sub file name.
        /// </param>
        /// <param name="screenWidth">
        /// The screen width.
        /// </param>
        /// <param name="screenHeight">
        /// The screen height.
        /// </param>
        /// <param name="bottomMargin">
        /// The bottom margin.
        /// </param>
        /// <param name="leftRightMargin">
        /// The left right margin.
        /// </param>
        /// <param name="languageStreamId">
        /// The language stream id.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="emphasis1">
        /// The emphasis 1.
        /// </param>
        /// <param name="useInnerAntialiasing">
        /// The use inner antialiasing.
        /// </param>
        /// <param name="languageName">
        /// The language name.
        /// </param>
        /// <param name="languageNameShort">
        /// The language name short.
        /// </param>
        public VobSubWriter(string subFileName, int screenWidth, int screenHeight, int bottomMargin, int leftRightMargin, int languageStreamId, Color pattern, Color emphasis1, bool useInnerAntialiasing, string languageName, string languageNameShort)
        {
            this._subFileName = subFileName;
            this._screenWidth = screenWidth;
            this._screenHeight = screenHeight;
            this._bottomMargin = bottomMargin;
            this._leftRightMargin = leftRightMargin;
            this._languageStreamId = languageStreamId;
            this._pattern = pattern;
            this._emphasis1 = emphasis1;
            this._useInnerAntialiasing = useInnerAntialiasing;
            this._languageName = languageName;
            this._languageNameShort = languageNameShort;
            this._idx = this.CreateIdxHeader();
            this._subFile = new FileStream(subFileName, FileMode.Create);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The write endian word.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public static void WriteEndianWord(int i, Stream stream)
        {
            stream.WriteByte((byte)(i / 256));
            stream.WriteByte((byte)(i % 256));
        }

        /// <summary>
        /// The get sub image buffer.
        /// </summary>
        /// <param name="twoPartBuffer">
        /// The two part buffer.
        /// </param>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="alignment">
        /// The alignment.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private byte[] GetSubImageBuffer(RunLengthTwoParts twoPartBuffer, NikseBitmap nbmp, Paragraph p, ContentAlignment alignment)
        {
            var ms = new MemoryStream();

            // sup picture datasize
            WriteEndianWord(twoPartBuffer.Length + 34, ms);

            // first display control sequence table address
            int startDisplayControlSequenceTableAddress = twoPartBuffer.Length + 4;
            WriteEndianWord(startDisplayControlSequenceTableAddress, ms);

            // Write image
            const int imageTopFieldDataAddress = 4;
            ms.Write(twoPartBuffer.Buffer1, 0, twoPartBuffer.Buffer1.Length);
            int imageBottomFieldDataAddress = 4 + twoPartBuffer.Buffer1.Length;
            ms.Write(twoPartBuffer.Buffer2, 0, twoPartBuffer.Buffer2.Length);

            // Write zero delay
            ms.WriteByte(0);
            ms.WriteByte(0);

            // next display control sequence table address (use current is last)
            WriteEndianWord(startDisplayControlSequenceTableAddress + 24, ms); // start of display control sequence table address

            // Control command start
            if (p.Forced)
            {
                ms.WriteByte(0); // ForcedStartDisplay==0
            }
            else
            {
                ms.WriteByte(1); // StartDisplay==1
            }

            // Control command 3 = SetColor
            WriteColors(ms); // 3 bytes

            // Control command 4 = SetContrast
            this.WriteContrast(ms); // 3 bytes

            // Control command 5 = SetDisplayArea
            this.WriteDisplayArea(ms, nbmp, alignment); // 7 bytes

            // Control command 6 = SetPixelDataAddress
            WritePixelDataAddress(ms, imageTopFieldDataAddress, imageBottomFieldDataAddress); // 5 bytes

            // Control command exit
            ms.WriteByte(255); // 1 byte

            // Control Sequence Table
            // Write delay - subtitle duration
            WriteEndianWord(Convert.ToInt32(p.Duration.TotalMilliseconds * 90.0 - 1023) >> 10, ms);

            // next display control sequence table address (use current is last)
            WriteEndianWord(startDisplayControlSequenceTableAddress + 24, ms); // start of display control sequence table address

            // Control command 2 = StopDisplay
            ms.WriteByte(2);

            // extra byte - for compatability with gpac/MP4BOX
            ms.WriteByte(255); // 1 byte

            return ms.ToArray();
        }

        /// <summary>
        /// The write paragraph.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="alignment">
        /// The alignment.
        /// </param>
        public void WriteParagraph(Paragraph p, Bitmap bmp, ContentAlignment alignment)
        {
            // inspired by code from SubtitleCreator
            // timestamp: 00:00:33:900, filepos: 000000000
            this._idx.AppendLine(string.Format("timestamp: {0:00}:{1:00}:{2:00}:{3:000}, filepos: {4}", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, p.StartTime.Milliseconds, this._subFile.Position.ToString("X").PadLeft(9, '0').ToLower()));

            var nbmp = new NikseBitmap(bmp);
            this._emphasis2 = nbmp.ConverToFourColors(this._background, this._pattern, this._emphasis1, this._useInnerAntialiasing);
            var twoPartBuffer = nbmp.RunLengthEncodeForDvd(this._background, this._pattern, this._emphasis1, this._emphasis2);
            var imageBuffer = this.GetSubImageBuffer(twoPartBuffer, nbmp, p, alignment);

            int bufferIndex = 0;
            byte vobSubId = (byte)this._languageStreamId;
            var mwsub = new MemWriter(200000);
            byte[] subHeader = new byte[30];
            byte[] ts = new byte[4];

            // Lended from "Son2VobSub" by Alain Vielle and Petr Vyskocil
            // And also from Sup2VobSub by Emmel
            subHeader[0] = 0x00; // MPEG 2 PACK HEADER
            subHeader[1] = 0x00;
            subHeader[2] = 0x01;
            subHeader[3] = 0xba;
            subHeader[4] = 0x44;
            subHeader[5] = 0x02;
            subHeader[6] = 0xc4;
            subHeader[7] = 0x82;
            subHeader[8] = 0x04;
            subHeader[9] = 0xa9;
            subHeader[10] = 0x01;
            subHeader[11] = 0x89;
            subHeader[12] = 0xc3;
            subHeader[13] = 0xf8;

            subHeader[14] = 0x00; // PES
            subHeader[15] = 0x00;
            subHeader[16] = 0x01;
            subHeader[17] = 0xbd;

            int packetSize = imageBuffer.Length;
            long toWrite = packetSize; // Image buffer + control sequence length
            bool header0 = true;

            while (toWrite > 0)
            {
                long headerSize;
                if (header0)
                {
                    header0 = false;

                    // This is only for first packet
                    subHeader[20] = 0x81; // mark as original
                    subHeader[21] = 0x80; // first packet: PTS
                    subHeader[22] = 0x05; // PES header data length

                    // PTS (90kHz):
                    // --------------
                    subHeader[23] = (byte)((ts[3] & 0xc0) >> 5 | 0x21);
                    subHeader[24] = (byte)((ts[3] & 0x3f) << 2 | (ts[2] & 0xc0) >> 6);
                    subHeader[25] = (byte)((ts[2] & 0x3f) << 2 | (ts[1] & 0x80) >> 6 | 0x01);
                    subHeader[26] = (byte)((ts[1] & 0x7f) << 1 | (ts[0] & 0x80) >> 7);
                    subHeader[27] = (byte)((ts[0] & 0x7f) << 1 | 0x01);

                    const string pre = "0010"; // 0011 or 0010 ? (KMPlayer will not understand 0011!!!)
                    long newPts = (long)(p.StartTime.TotalSeconds * 90000.0 + 0.5);
                    string bString = Convert.ToString(newPts, 2).PadLeft(33, '0');
                    string fiveBytesString = pre + bString.Substring(0, 3) + "1" + bString.Substring(3, 15) + "1" + bString.Substring(18, 15) + "1";
                    for (int i = 0; i < 5; i++)
                    {
                        subHeader[23 + i] = Convert.ToByte(fiveBytesString.Substring(i * 8, 8), 2);
                    }

                    subHeader[28] = vobSubId;
                    headerSize = 29;
                }
                else
                {
                    subHeader[20] = 0x81; // mark as original
                    subHeader[21] = 0x00; // no PTS
                    subHeader[22] = 0x00; // header data length
                    subHeader[23] = vobSubId;
                    headerSize = 24;
                }

                if ((toWrite + headerSize) <= 0x800)
                {
                    // write whole image in one 0x800 part
                    long j = (headerSize - 20) + toWrite;
                    subHeader[18] = (byte)(j / 0x100);
                    subHeader[19] = (byte)(j % 0x100);

                    // First Write header
                    for (int x = 0; x < headerSize; x++)
                    {
                        mwsub.WriteByte(subHeader[x]);
                    }

                    // Write Image Data
                    for (int x = 0; x < toWrite; x++)
                    {
                        mwsub.WriteByte(imageBuffer[bufferIndex++]);
                    }

                    // Pad remaining space
                    long paddingSize = 0x800 - headerSize - toWrite;
                    for (int x = 0; x < paddingSize; x++)
                    {
                        mwsub.WriteByte(0xff);
                    }

                    toWrite = 0;
                }
                else
                {
                    // write multiple parts
                    long blockSize = 0x800 - headerSize;
                    long j = (headerSize - 20) + blockSize;
                    subHeader[18] = (byte)(j / 0x100);
                    subHeader[19] = (byte)(j % 0x100);

                    // First Write header
                    for (int x = 0; x < headerSize; x++)
                    {
                        mwsub.WriteByte(subHeader[x]);
                    }

                    // Write Image Data
                    for (int x = 0; x < blockSize; x++)
                    {
                        mwsub.WriteByte(imageBuffer[bufferIndex++]);
                    }

                    toWrite -= blockSize;
                }
            }

            // Write whole memory stream to file
            long endPosition = mwsub.GetPosition();
            mwsub.GotoBegin();
            this._subFile.Write(mwsub.GetBuf(), 0, (int)endPosition);
        }

        /// <summary>
        /// The write pixel data address.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="imageTopFieldDataAddress">
        /// The image top field data address.
        /// </param>
        /// <param name="imageBottomFieldDataAddress">
        /// The image bottom field data address.
        /// </param>
        private static void WritePixelDataAddress(Stream stream, int imageTopFieldDataAddress, int imageBottomFieldDataAddress)
        {
            stream.WriteByte(6);
            WriteEndianWord(imageTopFieldDataAddress, stream);
            WriteEndianWord(imageBottomFieldDataAddress, stream);
        }

        /// <summary>
        /// The write display area.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <param name="alignment">
        /// The alignment.
        /// </param>
        private void WriteDisplayArea(Stream stream, NikseBitmap nbmp, ContentAlignment alignment)
        {
            stream.WriteByte(5);

            // Write 6 bytes of area - starting X, ending X, starting Y, ending Y, each 12 bits
            ushort startX = (ushort)((this._screenWidth - nbmp.Width) / 2);
            ushort startY = (ushort)(this._screenHeight - nbmp.Height - this._bottomMargin);

            if (alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.TopRight)
            {
                startY = (ushort)this._bottomMargin;
            }

            if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.MiddleCenter || alignment == ContentAlignment.MiddleRight)
            {
                startY = (ushort)((this._screenHeight / 2) - (nbmp.Height / 2));
            }

            if (alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.BottomLeft)
            {
                startX = (ushort)this._leftRightMargin;
            }

            if (alignment == ContentAlignment.TopRight || alignment == ContentAlignment.MiddleRight || alignment == ContentAlignment.BottomRight)
            {
                startX = (ushort)(this._screenWidth - nbmp.Width - this._leftRightMargin);
            }

            ushort endX = (ushort)(startX + nbmp.Width - 1);
            ushort endY = (ushort)(startY + nbmp.Height - 1);

            WriteEndianWord((ushort)(startX << 4 | endX >> 8), stream); // 16 - 12 start x + 4 end x
            WriteEndianWord((ushort)(endX << 8 | startY >> 4), stream); // 16 - 8 endx + 8 starty
            WriteEndianWord((ushort)(startY << 12 | endY), stream); // 16 - 4 start y + 12 end y
        }

        /// <summary>
        /// Directly provides the four contrast (alpha blend) values to associate with the four pixel values. One nibble per pixel value for a total of 2 bytes. 0x0 = transparent, 0xF = opaque
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        private void WriteContrast(Stream stream)
        {
            stream.WriteByte(4);
            stream.WriteByte((byte)((this._emphasis2.A << 4) | this._emphasis1.A)); // emphasis2 + emphasis1
            stream.WriteByte((byte)((this._pattern.A << 4) | this._background.A)); // pattern + background
        }

        /// <summary>
        /// provides four indices into the CLUT for the current PGC to associate with the four pixel values. One nibble per pixel value for a total of 2 bytes.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        private static void WriteColors(Stream stream)
        {
            // Index to palette
            const byte emphasis2 = 3;
            const byte emphasis1 = 2;
            const byte pattern = 1;
            const byte background = 0;

            stream.WriteByte(3);
            stream.WriteByte((emphasis2 << 4) | emphasis1); // emphasis2 + emphasis1
            stream.WriteByte((pattern << 4) | background); // pattern + background
        }

        /// <summary>
        /// The write idx file.
        /// </summary>
        public void WriteIdxFile()
        {
            string idxFileName = this._subFileName.Substring(0, this._subFileName.Length - 3) + "idx";
            File.WriteAllText(idxFileName, this._idx.ToString().Trim());
        }

        /// <summary>
        /// The create idx header.
        /// </summary>
        /// <returns>
        /// The <see cref="StringBuilder"/>.
        /// </returns>
        private StringBuilder CreateIdxHeader()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"# VobSub index file, v7 (do not modify this line!)
#
# To repair desynchronization, you can insert gaps this way:
# (it usually happens after vob id changes)
#
#    delay: [sign]hh:mm:ss:ms
#
# Where:
#    [sign]: +, - (optional)
#    hh: hours (0 <= hh)
#    mm/ss: minutes/seconds (0 <= mm/ss <= 59)
#    ms: milliseconds (0 <= ms <= 999)
#
#    Note: You can't position a sub before the previous with a negative value.
#
# You can also modify timestamps or delete a few subs you don't like.
# Just make sure they stay in increasing order.

# Settings

# Original frame size
size: " + this._screenWidth + "x" + this._screenHeight + @"

# Origin, relative to the upper-left corner, can be overloaded by aligment
org: 0, 0

# Image scaling (hor,ver), origin is at the upper-left corner or at the alignment coord (x, y)
scale: 100%, 100%

# Alpha blending
alpha: 100%

# Smoothing for very blocky images (use OLD for no filtering)
smooth: OFF

# In milliseconds
fadein/out: 50, 50

# Force subtitle placement relative to (org.x, org.y)
align: OFF at LEFT TOP

# For correcting non-progressive desync. (in milliseconds or hh:mm:ss:ms)
# Note: Not effective in DirectVobSub, use 'delay: ... ' instead.
time offset: 0

# ON: displays only forced subtitles, OFF: shows everything
forced subs: OFF

# The original palette of the DVD
palette: 000000, " + ToHexColor(this._pattern) + ", " + ToHexColor(this._emphasis1) + ", " + ToHexColor(this._emphasis2) + @", 828282, 828282, 828282, ffffff, 828282, bababa, 828282, 828282, 828282, 828282, 828282, 828282

# Custom colors (transp idxs and the four colors)
custom colors: OFF, tridx: 0000, colors: 000000, 000000, 000000, 000000

# Language index in use
langidx: 0

# " + this._languageName + @"
id: " + this._languageNameShort + @", index: 0
# Decomment next line to activate alternative name in DirectVobSub / Windows Media Player 6.x
# alt: " + this._languageName + @"
# Vob/Cell ID: 1, 1 (PTS: 0)");
            return sb;
        }

        /// <summary>
        /// The to hex color.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ToHexColor(Color c)
        {
            return (c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")).ToLower();
        }

        /// <summary>
        /// The release managed resources.
        /// </summary>
        private void ReleaseManagedResources()
        {
            if (this._subFile != null)
            {
                this._subFile.Dispose();
                this._subFile = null;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ReleaseManagedResources();
            }
        }

        /// <summary>
        /// The mem writer.
        /// </summary>
        private class MemWriter
        {
            /// <summary>
            /// The _buf.
            /// </summary>
            private readonly byte[] _buf;

            /// <summary>
            /// The _pos.
            /// </summary>
            private long _pos;

            /// <summary>
            /// Initializes a new instance of the <see cref="MemWriter"/> class.
            /// </summary>
            /// <param name="size">
            /// The size.
            /// </param>
            public MemWriter(long size)
            {
                this._buf = new byte[size];
                this._pos = 0;
            }

            /// <summary>
            /// The get buf.
            /// </summary>
            /// <returns>
            /// The <see cref="byte[]"/>.
            /// </returns>
            public byte[] GetBuf()
            {
                return this._buf;
            }

            /// <summary>
            /// The get position.
            /// </summary>
            /// <returns>
            /// The <see cref="long"/>.
            /// </returns>
            public long GetPosition()
            {
                return this._pos;
            }

            /// <summary>
            /// The goto begin.
            /// </summary>
            public void GotoBegin()
            {
                this._pos = 0;
            }

            /// <summary>
            /// The write byte.
            /// </summary>
            /// <param name="val">
            /// The val.
            /// </param>
            public void WriteByte(byte val)
            {
                this._buf[this._pos++] = val;
            }
        }
    }
}