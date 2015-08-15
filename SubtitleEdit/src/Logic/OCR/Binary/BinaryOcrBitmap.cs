namespace Nikse.SubtitleEdit.Logic.Ocr.Binary
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Logic.VobSub;

    public class BinaryOcrBitmap
    {
        private byte[] _colors;

        public BinaryOcrBitmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._colors = new byte[this.Width * this.Height];
            this.Hash = MurMurHash3.Hash(this._colors);
            this.CalcuateNumberOfColoredPixels();
        }

        public BinaryOcrBitmap(Stream stream)
        {
            try
            {
                byte[] buffer = new byte[16];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read < buffer.Length)
                {
                    this.LoadedOk = false;
                    return;
                }

                this.Width = buffer[0] << 8 | buffer[1];
                this.Height = buffer[2] << 8 | buffer[3];
                this.X = buffer[4] << 8 | buffer[5];
                this.Y = buffer[6] << 8 | buffer[7];
                this.NumberOfColoredPixels = buffer[8] << 8 | buffer[9];
                this.Italic = (buffer[10] & Helper.B10000000) > 0;
                this.ExpandCount = buffer[10] & Helper.B01111111;
                this.Hash = (uint)(buffer[11] << 24 | buffer[12] << 16 | buffer[13] << 8 | buffer[14]);
                int textLen = buffer[15];
                if (textLen > 0)
                {
                    buffer = new byte[textLen];
                    stream.Read(buffer, 0, buffer.Length);
                    this.Text = Encoding.UTF8.GetString(buffer);
                }

                this._colors = new byte[this.Width * this.Height];
                stream.Read(this._colors, 0, this._colors.Length);
                this.LoadedOk = true;
            }
            catch
            {
                this.LoadedOk = false;
            }
        }

        public BinaryOcrBitmap(NikseBitmap nbmp)
        {
            this.InitializeViaNikseBmp(nbmp);
        }

        public BinaryOcrBitmap(NikseBitmap nbmp, bool italic, int expandCount, string text, int x, int y)
        {
            this.InitializeViaNikseBmp(nbmp);
            this.Italic = italic;
            this.ExpandCount = expandCount;
            this.Text = text;
            this.X = x;
            this.Y = y;
        }

        // File format:
        // -------------
        // 2bytes=width
        // 2bytes=height
        // 2bytes=x
        // 2bytes=y
        // 2bytes=numberOfColoredPixels
        // 1byte=flags (1 bit = italic, next 7 bits = ExpandCount)
        // 4bytes=hash
        // 1bytes=text len
        // text len bytes=text (UTF-8)
        // w*h bytes / 8=pixels as bits(byte aligned)
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int NumberOfColoredPixels { get; private set; }

        public uint Hash { get; private set; }

        public bool Italic { get; set; }

        public int ExpandCount { get; set; }

        public bool LoadedOk { get; private set; }

        public string Text { get; set; }

        public List<BinaryOcrBitmap> ExpandedList { get; set; }

        public string Key
        {
            get
            {
                return this.Text + "|#|" + this.Hash + "_" + this.Width + "x" + this.Height + "_" + this.NumberOfColoredPixels;
            }
        }

        public override string ToString()
        {
            if (this.Italic)
            {
                return this.Text + " (" + this.Width + "x" + this.Height + ", italic)";
            }

            return this.Text + " (" + this.Width + "x" + this.Height + ")";
        }

        public void Save(Stream stream)
        {
            WriteInt16(stream, (ushort)this.Width);
            WriteInt16(stream, (ushort)this.Height);

            WriteInt16(stream, (ushort)this.X);
            WriteInt16(stream, (ushort)this.Y);

            WriteInt16(stream, (ushort)this.NumberOfColoredPixels);

            byte flags = (byte)(this.ExpandCount & Helper.B01111111);
            if (this.Italic)
            {
                flags = (byte)(flags + Helper.B10000000);
            }

            stream.WriteByte(flags);

            WriteInt32(stream, this.Hash);

            if (this.Text == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                byte[] textBuffer = Encoding.UTF8.GetBytes(this.Text);
                stream.WriteByte((byte)textBuffer.Length);
                stream.Write(textBuffer, 0, textBuffer.Length);
            }

            stream.Write(this._colors, 0, this._colors.Length);
        }

        public int GetPixel(int x, int y)
        {
            return this._colors[this.Width * y + x];
        }

        public void SetPixel(int x, int y, int c)
        {
            this._colors[this.Width * y + x] = (byte)c;
        }

        public void SetPixel(int x, int y, Color c)
        {
            if (c.A < 100)
            {
                this._colors[this.Width * y + x] = 0;
            }
            else
            {
                this._colors[this.Width * y + x] = 1;
            }
        }

        public void SetPixelViaAlpha(int x, int y, int alpha)
        {
            if (alpha < 100)
            {
                this._colors[this.Width * y + x] = 0;
            }
            else
            {
                this._colors[this.Width * y + x] = 1;
            }
        }

        /// <summary>
        ///     Copies a rectangle from the bitmap to a new bitmap
        /// </summary>
        /// <param name="section">Source rectangle</param>
        /// <returns>Rectangle from current image as new bitmap</returns>
        public ManagedBitmap GetRectangle(Rectangle section)
        {
            ManagedBitmap newRectangle = new ManagedBitmap(section.Width, section.Height);

            int recty = 0;
            for (int y = section.Top; y < section.Top + section.Height; y++)
            {
                int rectx = 0;
                for (int x = section.Left; x < section.Left + section.Width; x++)
                {
                    Color c = Color.Transparent;
                    if (this.GetPixel(x, y) > 0)
                    {
                        c = Color.White;
                    }

                    newRectangle.SetPixel(rectx, recty, c);
                    rectx++;
                }

                recty++;
            }

            return newRectangle;
        }

        public Bitmap ToOldBitmap()
        {
            if (this.ExpandedList != null && this.ExpandedList.Count > 0)
            {
                int minX = this.X;
                int minY = this.Y;
                int maxX = this.X + this.Width;
                int maxY = this.Y + this.Height;
                List<BinaryOcrBitmap> list = new List<BinaryOcrBitmap>();
                list.Add(this);
                foreach (BinaryOcrBitmap bob in this.ExpandedList)
                {
                    if (bob.X < minX)
                    {
                        minX = bob.X;
                    }

                    if (bob.Y < minY)
                    {
                        minY = bob.Y;
                    }

                    if (bob.X + bob.Width > maxX)
                    {
                        maxX = bob.X + bob.Width;
                    }

                    if (bob.Y + bob.Height > maxY)
                    {
                        maxY = bob.Y + bob.Height;
                    }

                    list.Add(bob);
                }

                BinaryOcrBitmap nbmp = new BinaryOcrBitmap(maxX - minX, maxY - minY);
                foreach (BinaryOcrBitmap bob in list)
                {
                    for (int y = 0; y < bob.Height; y++)
                    {
                        for (int x = 0; x < bob.Width; x++)
                        {
                            int c = bob.GetPixel(x, y);
                            if (c > 0)
                            {
                                nbmp.SetPixel(bob.X - minX + x, bob.Y - minY + y, 1);
                            }
                        }
                    }
                }

                return nbmp.ToOldBitmap(); // Resursive
            }
            else
            {
                NikseBitmap nbmp = new NikseBitmap(this.Width, this.Height);
                for (int y = 0; y < this.Height; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        Color c = Color.Transparent;
                        if (this.GetPixel(x, y) > 0)
                        {
                            c = Color.White;
                        }

                        nbmp.SetPixel(x, y, c);
                    }
                }

                return nbmp.GetBitmap();
            }
        }

        private static void WriteInt16(Stream stream, ushort val)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)((val & 0xFF00) >> 8);
            buffer[1] = (byte)(val & 0x00FF);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static void WriteInt32(Stream stream, uint val)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)((val & 0xFF000000) >> 24);
            buffer[1] = (byte)((val & 0xFF0000) >> 16);
            buffer[2] = (byte)((val & 0xFF00) >> 8);
            buffer[3] = (byte)(val & 0xFF);
            stream.Write(buffer, 0, buffer.Length);
        }

        private void InitializeViaNikseBmp(NikseBitmap nbmp)
        {
            this.Width = nbmp.Width;
            this.Height = nbmp.Height;
            this._colors = new byte[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.SetPixelViaAlpha(x, y, nbmp.GetAlpha(x, y));
                }
            }

            this.Hash = MurMurHash3.Hash(this._colors);
            this.CalcuateNumberOfColoredPixels();
        }

        private void CalcuateNumberOfColoredPixels()
        {
            this.NumberOfColoredPixels = 0;
            for (int i = 0; i < this._colors.Length; i++)
            {
                if (this._colors[i] > 0)
                {
                    this.NumberOfColoredPixels++;
                }
            }
        }
    }
}