// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NikseBitmap.cs" company="">
//   
// </copyright>
// <summary>
//   The run length two parts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    /// The run length two parts.
    /// </summary>
    public class RunLengthTwoParts
    {
        /// <summary>
        /// Gets or sets the buffer 1.
        /// </summary>
        public byte[] Buffer1 { get; set; }

        /// <summary>
        /// Gets or sets the buffer 2.
        /// </summary>
        public byte[] Buffer2 { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get
            {
                return this.Buffer1.Length + this.Buffer2.Length;
            }
        }
    }

    /// <summary>
    /// The nikse bitmap.
    /// </summary>
    public class NikseBitmap
    {
        /// <summary>
        /// The _bitmap data.
        /// </summary>
        private byte[] _bitmapData;

        /// <summary>
        /// The _pixel address.
        /// </summary>
        private int _pixelAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="NikseBitmap"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public NikseBitmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._bitmapData = new byte[this.Width * this.Height * 4];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NikseBitmap"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="bitmapData">
        /// The bitmap data.
        /// </param>
        public NikseBitmap(int width, int height, byte[] bitmapData)
        {
            this.Width = width;
            this.Height = height;
            this._bitmapData = bitmapData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NikseBitmap"/> class.
        /// </summary>
        /// <param name="inputBitmap">
        /// The input bitmap.
        /// </param>
        public NikseBitmap(Bitmap inputBitmap)
        {
            if (inputBitmap == null)
            {
                return;
            }

            this.Width = inputBitmap.Width;
            this.Height = inputBitmap.Height;

            if (inputBitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                var newBitmap = new Bitmap(inputBitmap.Width, inputBitmap.Height, PixelFormat.Format32bppArgb);
                using (var gr = Graphics.FromImage(newBitmap))
                {
                    gr.DrawImage(inputBitmap, 0, 0);
                }

                inputBitmap.Dispose();
                inputBitmap = newBitmap;
            }

            var bitmapData = inputBitmap.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            this._bitmapData = new byte[bitmapData.Stride * this.Height];
            Marshal.Copy(bitmapData.Scan0, this._bitmapData, 0, this._bitmapData.Length);
            inputBitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NikseBitmap"/> class.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        public NikseBitmap(NikseBitmap input)
        {
            this.Width = input.Width;
            this.Height = input.Height;
            this._bitmapData = new byte[input._bitmapData.Length];
            Buffer.BlockCopy(input._bitmapData, 0, this._bitmapData, 0, this._bitmapData.Length);
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The replace not dark with white.
        /// </summary>
        public void ReplaceNotDarkWithWhite()
        {
            var buffer = new byte[3];
            buffer[0] = 255;
            buffer[1] = 255;
            buffer[2] = 255;
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 3] > 200 && // Alpha
                    this._bitmapData[i + 2] + this._bitmapData[i + 1] + this._bitmapData[i] > 200)
                {
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 3);
                }
            }
        }

        /// <summary>
        /// The replace yellow with white.
        /// </summary>
        public void ReplaceYellowWithWhite()
        {
            var buffer = new byte[3];
            buffer[0] = 255;
            buffer[1] = 255;
            buffer[2] = 255;
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 3] > 200 && // Alpha
                    this._bitmapData[i + 2] > 199 && // Red
                    this._bitmapData[i + 1] > 190 && // Green
                    this._bitmapData[i] < 40)
                {
                    // Blue
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 3);
                }
            }
        }

        /// <summary>
        /// The replace non white with transparent.
        /// </summary>
        public void ReplaceNonWhiteWithTransparent()
        {
            var buffer = new byte[4];
            buffer[0] = 0; // B
            buffer[1] = 0; // G
            buffer[2] = 0; // R
            buffer[3] = 0; // A
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 2] + this._bitmapData[i + 1] + this._bitmapData[i] < 300)
                {
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
                }
            }
        }

        /// <summary>
        /// The replace transparent with.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public void ReplaceTransparentWith(Color c)
        {
            var buffer = new byte[4];
            buffer[0] = c.B;
            buffer[1] = c.G;
            buffer[2] = c.R;
            buffer[3] = c.A;
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 3] < 10)
                {
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
                }
            }
        }

        /// <summary>
        /// The make one color.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public void MakeOneColor(Color c)
        {
            var buffer = new byte[4];
            buffer[0] = c.B;
            buffer[1] = c.G;
            buffer[2] = c.R;
            buffer[3] = c.A;

            var bufferTransparent = new byte[4];
            bufferTransparent[0] = 0;
            bufferTransparent[1] = 0;
            bufferTransparent[2] = 0;
            bufferTransparent[3] = 0;
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i] > 20)
                {
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
                }
                else
                {
                    Buffer.BlockCopy(bufferTransparent, 0, this._bitmapData, i, 4);
                }
            }
        }

        /// <summary>
        /// The make one color remover others.
        /// </summary>
        /// <param name="c1">
        /// The c 1.
        /// </param>
        /// <param name="c2">
        /// The c 2.
        /// </param>
        /// <param name="maxDif">
        /// The max dif.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int MakeOneColorRemoverOthers(Color c1, Color c2, int maxDif)
        {
            var buffer1 = new byte[4];
            buffer1[0] = c1.B;
            buffer1[1] = c1.G;
            buffer1[2] = c1.R;
            buffer1[3] = c1.A;

            var buffer2 = new byte[4];
            buffer2[0] = c2.B;
            buffer2[1] = c2.G;
            buffer2[2] = c2.R;
            buffer2[3] = c2.A;

            var bufferTransparent = new byte[4];
            bufferTransparent[0] = 0;
            bufferTransparent[1] = 0;
            bufferTransparent[2] = 0;
            bufferTransparent[3] = 0;
            int count = 0;
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 3] > 20)
                {
                    if ((Math.Abs(buffer1[0] - this._bitmapData[i]) < maxDif && Math.Abs(buffer1[1] - this._bitmapData[i + 1]) < maxDif && Math.Abs(buffer1[2] - this._bitmapData[i + 2]) < maxDif) || (Math.Abs(buffer2[0] - this._bitmapData[i]) < maxDif && Math.Abs(buffer2[1] - this._bitmapData[i + 1]) < maxDif && Math.Abs(buffer2[2] - this._bitmapData[i + 2]) < maxDif))
                    {
                        count++;
                    }
                    else
                    {
                        Buffer.BlockCopy(bufferTransparent, 0, this._bitmapData, i, 4);
                    }
                }
                else
                {
                    Buffer.BlockCopy(bufferTransparent, 0, this._bitmapData, i, 4);
                }
            }

            return count;
        }

        /// <summary>
        /// The get outline color.
        /// </summary>
        /// <param name="borderColor">
        /// The border color.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        private static Color GetOutlineColor(Color borderColor)
        {
            if (borderColor.R + borderColor.G + borderColor.B < 30)
            {
                return Color.FromArgb(200, 75, 75, 75);
            }

            return Color.FromArgb(150, borderColor.R, borderColor.G, borderColor.B);
        }

        /// <summary>
        /// Convert a x-color image to four colors, for e.g. DVD sub pictures.
        /// </summary>
        /// <param name="background">
        /// Background color
        /// </param>
        /// <param name="pattern">
        /// Pattern color, normally white or yellow
        /// </param>
        /// <param name="emphasis1">
        /// Emphasis 1, normally black or near black (border)
        /// </param>
        /// <param name="useInnerAntialize">
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color ConverToFourColors(Color background, Color pattern, Color emphasis1, bool useInnerAntialize)
        {
            var backgroundBuffer = new byte[4];
            backgroundBuffer[0] = background.B;
            backgroundBuffer[1] = background.G;
            backgroundBuffer[2] = background.R;
            backgroundBuffer[3] = background.A;

            var patternBuffer = new byte[4];
            patternBuffer[0] = pattern.B;
            patternBuffer[1] = pattern.G;
            patternBuffer[2] = pattern.R;
            patternBuffer[3] = pattern.A;

            var emphasis1Buffer = new byte[4];
            emphasis1Buffer[0] = emphasis1.B;
            emphasis1Buffer[1] = emphasis1.G;
            emphasis1Buffer[2] = emphasis1.R;
            emphasis1Buffer[3] = emphasis1.A;

            var emphasis2Buffer = new byte[4];
            var emphasis2 = GetOutlineColor(emphasis1);
            if (!useInnerAntialize)
            {
                emphasis2Buffer[0] = emphasis2.B;
                emphasis2Buffer[1] = emphasis2.G;
                emphasis2Buffer[2] = emphasis2.R;
                emphasis2Buffer[3] = emphasis2.A;
            }

            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                int smallestDiff = 10000;
                byte[] buffer = backgroundBuffer;
                if (backgroundBuffer[3] == 0 && this._bitmapData[i + 3] < 10)
                {
                    // transparent
                }
                else
                {
                    int patternDiff = Math.Abs(patternBuffer[0] - this._bitmapData[i]) + Math.Abs(patternBuffer[1] - this._bitmapData[i + 1]) + Math.Abs(patternBuffer[2] - this._bitmapData[i + 2]) + Math.Abs(patternBuffer[3] - this._bitmapData[i + 3]);
                    if (patternDiff < smallestDiff)
                    {
                        smallestDiff = patternDiff;
                        buffer = patternBuffer;
                    }

                    int emphasis1Diff = Math.Abs(emphasis1Buffer[0] - this._bitmapData[i]) + Math.Abs(emphasis1Buffer[1] - this._bitmapData[i + 1]) + Math.Abs(emphasis1Buffer[2] - this._bitmapData[i + 2]) + Math.Abs(emphasis1Buffer[3] - this._bitmapData[i + 3]);
                    if (useInnerAntialize)
                    {
                        if (emphasis1Diff - 20 < smallestDiff)
                        {
                            buffer = emphasis1Buffer;
                        }
                    }
                    else
                    {
                        if (emphasis1Diff < smallestDiff)
                        {
                            smallestDiff = emphasis1Diff;
                            buffer = emphasis1Buffer;
                        }

                        int emphasis2Diff = Math.Abs(emphasis2Buffer[0] - this._bitmapData[i]) + Math.Abs(emphasis2Buffer[1] - this._bitmapData[i + 1]) + Math.Abs(emphasis2Buffer[2] - this._bitmapData[i + 2]) + Math.Abs(emphasis2Buffer[3] - this._bitmapData[i + 3]);
                        if (emphasis2Diff < smallestDiff)
                        {
                            buffer = emphasis2Buffer;
                        }
                        else if (this._bitmapData[i + 3] >= 10 && this._bitmapData[i + 3] < 90)
                        {
                            // anti-alias
                            buffer = emphasis2Buffer;
                        }
                    }
                }

                Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
            }

            if (useInnerAntialize)
            {
                return this.VobSubAntialize(pattern, emphasis1);
            }

            return emphasis2;
        }

        /// <summary>
        /// The vob sub antialize.
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="emphasis1">
        /// The emphasis 1.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        private Color VobSubAntialize(Color pattern, Color emphasis1)
        {
            int r = (int)Math.Round((pattern.R * 2.0 + emphasis1.R) / 3.0);
            int g = (int)Math.Round((pattern.G * 2.0 + emphasis1.G) / 3.0);
            int b = (int)Math.Round((pattern.B * 2.0 + emphasis1.B) / 3.0);
            Color antializeColor = Color.FromArgb(r, g, b);

            for (int y = 1; y < this.Height - 1; y++)
            {
                for (int x = 1; x < this.Width - 1; x++)
                {
                    if (this.GetPixel(x, y) == pattern)
                    {
                        if (this.GetPixel(x - 1, y) == emphasis1 && this.GetPixel(x, y - 1) == emphasis1)
                        {
                            this.SetPixel(x, y, antializeColor);
                        }
                        else if (this.GetPixel(x - 1, y) == emphasis1 && this.GetPixel(x, y + 1) == emphasis1)
                        {
                            this.SetPixel(x, y, antializeColor);
                        }
                        else if (this.GetPixel(x + 1, y) == emphasis1 && this.GetPixel(x, y + 1) == emphasis1)
                        {
                            this.SetPixel(x, y, antializeColor);
                        }
                        else if (this.GetPixel(x + 1, y) == emphasis1 && this.GetPixel(x, y - 1) == emphasis1)
                        {
                            this.SetPixel(x, y, antializeColor);
                        }
                    }
                }
            }

            return antializeColor;
        }

        /// <summary>
        /// The run length encode for dvd.
        /// </summary>
        /// <param name="background">
        /// The background.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="emphasis1">
        /// The emphasis 1.
        /// </param>
        /// <param name="emphasis2">
        /// The emphasis 2.
        /// </param>
        /// <returns>
        /// The <see cref="RunLengthTwoParts"/>.
        /// </returns>
        public RunLengthTwoParts RunLengthEncodeForDvd(Color background, Color pattern, Color emphasis1, Color emphasis2)
        {
            var backgroundBuffer = new byte[4];
            backgroundBuffer[0] = background.B;
            backgroundBuffer[1] = background.G;
            backgroundBuffer[2] = background.R;
            backgroundBuffer[3] = background.A;

            var patternBuffer = new byte[4];
            patternBuffer[0] = pattern.B;
            patternBuffer[1] = pattern.G;
            patternBuffer[2] = pattern.R;
            patternBuffer[3] = pattern.A;

            var emphasis1Buffer = new byte[4];
            emphasis1Buffer[0] = emphasis1.B;
            emphasis1Buffer[1] = emphasis1.G;
            emphasis1Buffer[2] = emphasis1.R;
            emphasis1Buffer[3] = emphasis1.A;

            var emphasis2Buffer = new byte[4];
            emphasis2Buffer[0] = emphasis2.B;
            emphasis2Buffer[1] = emphasis2.G;
            emphasis2Buffer[2] = emphasis2.R;
            emphasis2Buffer[3] = emphasis2.A;

            var bufferEqual = new byte[this.Width * this.Height];
            var bufferUnEqual = new byte[this.Width * this.Height];
            int indexBufferEqual = 0;
            int indexBufferUnEqual = 0;

            this._pixelAddress = -4;
            for (int y = 0; y < this.Height; y++)
            {
                int index;
                byte[] buffer;
                if (y % 2 == 0)
                {
                    index = indexBufferEqual;
                    buffer = bufferEqual;
                }
                else
                {
                    index = indexBufferUnEqual;
                    buffer = bufferUnEqual;
                }

                var indexHalfNibble = false;
                var lastColor = -1;
                var count = 0;

                for (int x = 0; x < this.Width; x++)
                {
                    int color = this.GetDvdColor(patternBuffer, emphasis1Buffer, emphasis2Buffer);

                    if (lastColor == -1)
                    {
                        lastColor = color;
                        count = 1;
                    }
                    else if (lastColor == color && count < 64)
                    {
                        // only allow up to 63 run-length (for SubtitleCreator compatibility)
                        count++;
                    }
                    else
                    {
                        WriteRle(ref indexHalfNibble, lastColor, count, ref index, buffer);
                        lastColor = color;
                        count = 1;
                    }
                }

                if (count > 0)
                {
                    WriteRle(ref indexHalfNibble, lastColor, count, ref index, buffer);
                }

                if (indexHalfNibble)
                {
                    index++;
                }

                if (y % 2 == 0)
                {
                    indexBufferEqual = index;
                    bufferEqual = buffer;
                }
                else
                {
                    indexBufferUnEqual = index;
                    bufferUnEqual = buffer;
                }
            }

            var twoParts = new RunLengthTwoParts { Buffer1 = new byte[indexBufferEqual] };
            Buffer.BlockCopy(bufferEqual, 0, twoParts.Buffer1, 0, indexBufferEqual);
            twoParts.Buffer2 = new byte[indexBufferUnEqual + 2];
            Buffer.BlockCopy(bufferUnEqual, 0, twoParts.Buffer2, 0, indexBufferUnEqual);
            return twoParts;
        }

        /// <summary>
        /// The write rle.
        /// </summary>
        /// <param name="indexHalfNibble">
        /// The index half nibble.
        /// </param>
        /// <param name="lastColor">
        /// The last color.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        private static void WriteRle(ref bool indexHalfNibble, int lastColor, int count, ref int index, byte[] buffer)
        {
            if (count <= Helper.B00000011)
            {
                // 1-3 repetitions
                WriteOneNibble(buffer, count, lastColor, ref index, ref indexHalfNibble);
            }
            else if (count <= Helper.B00001111)
            {
                // 4-15 repetitions
                WriteTwoNibbles(buffer, count, lastColor, ref index, indexHalfNibble);
            }
            else if (count <= Helper.B00111111)
            {
                // 4-15 repetitions
                WriteThreeNibbles(buffer, count, lastColor, ref index, ref indexHalfNibble); // 16-63 repetitions
            }
            else
            {
                // 64-255 repetitions
                int factor = count / 255;
                for (int i = 0; i < factor; i++)
                {
                    WriteFourNibbles(buffer, 0xff, lastColor, ref index, indexHalfNibble);
                }

                int rest = count % 255;
                if (rest > 0)
                {
                    WriteFourNibbles(buffer, rest, lastColor, ref index, indexHalfNibble);
                }
            }
        }

        /// <summary>
        /// The write four nibbles.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="indexHalfNibble">
        /// The index half nibble.
        /// </param>
        private static void WriteFourNibbles(byte[] buffer, int count, int color, ref int index, bool indexHalfNibble)
        {
            int n = (count << 2) + color;
            if (indexHalfNibble)
            {
                index++;
                var firstNibble = (byte)(n >> 4);
                buffer[index] = firstNibble;
                index++;
                var secondNibble = (byte)((n & Helper.B00001111) << 4);
                buffer[index] = secondNibble;
            }
            else
            {
                var firstNibble = (byte)(n >> 8);
                buffer[index] = firstNibble;
                index++;
                var secondNibble = (byte)(n & Helper.B11111111);
                buffer[index] = secondNibble;
                index++;
            }
        }

        /// <summary>
        /// The write three nibbles.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="indexHalfNibble">
        /// The index half nibble.
        /// </param>
        private static void WriteThreeNibbles(byte[] buffer, int count, int color, ref int index, ref bool indexHalfNibble)
        {
            // Value     Bits   n=length, c=color
            // 16-63     12     0 0 0 0 n n n n n n c c           (one and a half byte)
            var n = (ushort)((count << 2) + color);
            if (indexHalfNibble)
            {
                index++; // there should already zeroes in last nibble
                buffer[index] = (byte)n;
                index++;
            }
            else
            {
                buffer[index] = (byte)(n >> 4);
                index++;
                buffer[index] = (byte)((n & Helper.B00011111) << 4);
            }

            indexHalfNibble = !indexHalfNibble;
        }

        /// <summary>
        /// The write two nibbles.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="indexHalfNibble">
        /// The index half nibble.
        /// </param>
        private static void WriteTwoNibbles(byte[] buffer, int count, int color, ref int index, bool indexHalfNibble)
        {
            // Value      Bits   n=length, c=color
            // 4-15       8      0 0 n n n n c c                   (one byte)
            var n = (byte)((count << 2) + color);
            if (indexHalfNibble)
            {
                var firstNibble = (byte)(n >> 4);
                buffer[index] = (byte)(buffer[index] | firstNibble);
                var secondNibble = (byte)((n & Helper.B00001111) << 4);
                index++;
                buffer[index] = secondNibble;
            }
            else
            {
                buffer[index] = n;
                index++;
            }
        }

        /// <summary>
        /// The write one nibble.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="indexHalfNibble">
        /// The index half nibble.
        /// </param>
        private static void WriteOneNibble(byte[] buffer, int count, int color, ref int index, ref bool indexHalfNibble)
        {
            var n = (byte)((count << 2) + color);
            if (indexHalfNibble)
            {
                buffer[index] = (byte)(buffer[index] | n);
                index++;
            }
            else
            {
                buffer[index] = (byte)(n << 4);
            }

            indexHalfNibble = !indexHalfNibble;
        }

        /// <summary>
        /// The get dvd color.
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="emphasis1">
        /// The emphasis 1.
        /// </param>
        /// <param name="emphasis2">
        /// The emphasis 2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetDvdColor(byte[] pattern, byte[] emphasis1, byte[] emphasis2)
        {
            this._pixelAddress += 4;
            int a = this._bitmapData[this._pixelAddress + 3];
            int r = this._bitmapData[this._pixelAddress + 2];
            int g = this._bitmapData[this._pixelAddress + 1];
            int b = this._bitmapData[this._pixelAddress];

            if (pattern[0] == b && pattern[1] == g && pattern[2] == r && pattern[3] == a)
            {
                return 1;
            }

            if (emphasis1[0] == b && emphasis1[1] == g && emphasis1[2] == r && emphasis1[3] == a)
            {
                return 2;
            }

            if (emphasis2[0] == b && emphasis2[1] == g && emphasis2[2] == r && emphasis2[3] == a)
            {
                return 3;
            }

            return 0;
        }

        /// <summary>
        /// The crop transparent sides and bottom.
        /// </summary>
        /// <param name="maximumCropping">
        /// The maximum cropping.
        /// </param>
        /// <param name="bottom">
        /// The bottom.
        /// </param>
        public void CropTransparentSidesAndBottom(int maximumCropping, bool bottom)
        {
            int leftStart = 0;
            bool done = false;
            int x = 0;
            int y;
            while (!done && x < this.Width)
            {
                y = 0;
                while (!done && y < this.Height)
                {
                    int alpha = this.GetAlpha(x, y);
                    if (alpha != 0)
                    {
                        done = true;
                        leftStart = x;
                        leftStart -= maximumCropping;
                        if (leftStart < 0)
                        {
                            leftStart = 0;
                        }
                    }

                    y++;
                }

                x++;
            }

            int rightEnd = this.Width - 1;
            done = false;
            x = this.Width - 1;
            while (!done && x >= 0)
            {
                y = 0;
                while (!done && y < this.Height)
                {
                    int alpha = this.GetAlpha(x, y);
                    if (alpha != 0)
                    {
                        done = true;
                        rightEnd = x;
                        rightEnd += maximumCropping;
                        if (rightEnd >= this.Width)
                        {
                            rightEnd = this.Width - 1;
                        }
                    }

                    y++;
                }

                x--;
            }

            // crop bottom
            done = false;
            int newHeight = this.Height;
            if (bottom)
            {
                y = this.Height - 1;
                while (!done && y > 0)
                {
                    x = 0;
                    while (!done && x < this.Width)
                    {
                        int alpha = this.GetAlpha(x, y);
                        if (alpha != 0)
                        {
                            done = true;
                            newHeight = y + maximumCropping + 1;
                            if (newHeight > this.Height)
                            {
                                newHeight = this.Height;
                            }
                        }

                        x++;
                    }

                    y--;
                }
            }

            if (leftStart < 2 && rightEnd >= this.Width - 3)
            {
                return;
            }

            int newWidth = rightEnd - leftStart + 1;
            if (newWidth <= 0)
            {
                return;
            }

            var newBitmapData = new byte[newWidth * newHeight * 4];
            int index = 0;
            for (y = 0; y < newHeight; y++)
            {
                int pixelAddress = (leftStart * 4) + (y * 4 * this.Width);
                Buffer.BlockCopy(this._bitmapData, pixelAddress, newBitmapData, index, 4 * newWidth);
                index += 4 * newWidth;
            }

            this.Width = newWidth;
            this.Height = newHeight;
            this._bitmapData = newBitmapData;
        }

        /// <summary>
        /// The crop sides and bottom.
        /// </summary>
        /// <param name="maximumCropping">
        /// The maximum cropping.
        /// </param>
        /// <param name="transparentColor">
        /// The transparent color.
        /// </param>
        /// <param name="bottom">
        /// The bottom.
        /// </param>
        public void CropSidesAndBottom(int maximumCropping, Color transparentColor, bool bottom)
        {
            int leftStart = 0;
            bool done = false;
            int x = 0;
            int y;
            while (!done && x < this.Width)
            {
                y = 0;
                while (!done && y < this.Height)
                {
                    Color c = this.GetPixel(x, y);
                    if (c != transparentColor)
                    {
                        done = true;
                        leftStart = x;
                        leftStart -= maximumCropping;
                        if (leftStart < 0)
                        {
                            leftStart = 0;
                        }
                    }

                    y++;
                }

                x++;
            }

            int rightEnd = this.Width - 1;
            done = false;
            x = this.Width - 1;
            while (!done && x >= 0)
            {
                y = 0;
                while (!done && y < this.Height)
                {
                    Color c = this.GetPixel(x, y);
                    if (c != transparentColor)
                    {
                        done = true;
                        rightEnd = x;
                        rightEnd += maximumCropping;
                        if (rightEnd >= this.Width)
                        {
                            rightEnd = this.Width - 1;
                        }
                    }

                    y++;
                }

                x--;
            }

            // crop bottom
            done = false;
            int newHeight = this.Height;
            if (bottom)
            {
                y = this.Height - 1;
                while (!done && y > 0)
                {
                    x = 0;
                    while (!done && x < this.Width)
                    {
                        Color c = this.GetPixel(x, y);
                        if (c != transparentColor)
                        {
                            done = true;
                            newHeight = y + maximumCropping;
                            if (newHeight > this.Height)
                            {
                                newHeight = this.Height;
                            }
                        }

                        x++;
                    }

                    y--;
                }
            }

            if (leftStart < 2 && rightEnd >= this.Width - 3)
            {
                return;
            }

            int newWidth = rightEnd - leftStart + 1;
            if (newWidth <= 0)
            {
                return;
            }

            var newBitmapData = new byte[newWidth * newHeight * 4];
            int index = 0;
            for (y = 0; y < newHeight; y++)
            {
                int pixelAddress = (leftStart * 4) + (y * 4 * this.Width);
                Buffer.BlockCopy(this._bitmapData, pixelAddress, newBitmapData, index, 4 * newWidth);
                index += 4 * newWidth;
            }

            this.Width = newWidth;
            this.Height = newHeight;
            this._bitmapData = newBitmapData;
        }

        /// <summary>
        /// The crop top.
        /// </summary>
        /// <param name="maximumCropping">
        /// The maximum cropping.
        /// </param>
        /// <param name="transparentColor">
        /// The transparent color.
        /// </param>
        public void CropTop(int maximumCropping, Color transparentColor)
        {
            bool done = false;
            int newTop = 0;
            int y = 0;
            while (!done && y < this.Height)
            {
                var x = 0;
                while (!done && x < this.Width)
                {
                    Color c = this.GetPixel(x, y);
                    if (c != transparentColor && !(c.A == 0 && transparentColor.A == 0))
                    {
                        done = true;
                        newTop = y - maximumCropping;
                        if (newTop < 0)
                        {
                            newTop = 0;
                        }
                    }

                    x++;
                }

                y++;
            }

            if (newTop == 0)
            {
                return;
            }

            int newHeight = this.Height - newTop;
            var newBitmapData = new byte[this.Width * newHeight * 4];
            int index = 0;
            for (y = newTop; y < this.Height; y++)
            {
                int pixelAddress = y * 4 * this.Width;
                Buffer.BlockCopy(this._bitmapData, pixelAddress, newBitmapData, index, 4 * this.Width);
                index += 4 * this.Width;
            }

            this.Height = newHeight;
            this._bitmapData = newBitmapData;
        }

        /// <summary>
        /// The crop top transparent.
        /// </summary>
        /// <param name="maximumCropping">
        /// The maximum cropping.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CropTopTransparent(int maximumCropping)
        {
            bool done = false;
            int newTop = 0;
            int y = 0;
            while (!done && y < this.Height)
            {
                var x = 0;
                while (!done && x < this.Width)
                {
                    int alpha = this.GetAlpha(x, y);
                    if (alpha > 10)
                    {
                        done = true;
                        newTop = y - maximumCropping;
                        if (newTop < 0)
                        {
                            newTop = 0;
                        }
                    }

                    x++;
                }

                y++;
            }

            if (newTop == 0)
            {
                return 0;
            }

            int newHeight = this.Height - newTop;
            var newBitmapData = new byte[this.Width * newHeight * 4];
            int index = 0;
            for (y = newTop; y < this.Height; y++)
            {
                int pixelAddress = y * 4 * this.Width;
                Buffer.BlockCopy(this._bitmapData, pixelAddress, newBitmapData, index, 4 * this.Width);
                index += 4 * this.Width;
            }

            this.Height = newHeight;
            this._bitmapData = newBitmapData;
            return newTop;
        }

        /// <summary>
        /// The fill.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        public void Fill(Color color)
        {
            var buffer = new byte[4];
            buffer[0] = color.B;
            buffer[1] = color.G;
            buffer[2] = color.R;
            buffer[3] = color.A;
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
            }
        }

        /// <summary>
        /// The get alpha.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetAlpha(int x, int y)
        {
            return this._bitmapData[(x * 4) + (y * 4 * this.Width) + 3];
        }

        /// <summary>
        /// The get pixel.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetPixel(int x, int y)
        {
            this._pixelAddress = (x * 4) + (y * 4 * this.Width);
            return Color.FromArgb(this._bitmapData[this._pixelAddress + 3], this._bitmapData[this._pixelAddress + 2], this._bitmapData[this._pixelAddress + 1], this._bitmapData[this._pixelAddress]);
        }

        /// <summary>
        /// The get pixel colors.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] GetPixelColors(int x, int y)
        {
            this._pixelAddress = (x * 4) + (y * 4 * this.Width);
            return new[] { this._bitmapData[this._pixelAddress + 3], this._bitmapData[this._pixelAddress + 2], this._bitmapData[this._pixelAddress + 1], this._bitmapData[this._pixelAddress] };
        }

        /// <summary>
        /// The get pixel next.
        /// </summary>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetPixelNext()
        {
            this._pixelAddress += 4;
            return Color.FromArgb(this._bitmapData[this._pixelAddress + 3], this._bitmapData[this._pixelAddress + 2], this._bitmapData[this._pixelAddress + 1], this._bitmapData[this._pixelAddress]);
        }

        /// <summary>
        /// The set pixel.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public void SetPixel(int x, int y, Color color)
        {
            this._pixelAddress = (x * 4) + (y * 4 * this.Width);
            this._bitmapData[this._pixelAddress] = color.B;
            this._bitmapData[this._pixelAddress + 1] = color.G;
            this._bitmapData[this._pixelAddress + 2] = color.R;
            this._bitmapData[this._pixelAddress + 3] = color.A;
        }

        /// <summary>
        /// The set pixel next.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        public void SetPixelNext(Color color)
        {
            this._pixelAddress += 4;
            this._bitmapData[this._pixelAddress] = color.B;
            this._bitmapData[this._pixelAddress + 1] = color.G;
            this._bitmapData[this._pixelAddress + 2] = color.R;
            this._bitmapData[this._pixelAddress + 3] = color.A;
        }

        /// <summary>
        /// The get bitmap.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetBitmap()
        {
            var bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            IntPtr destination = bitmapdata.Scan0;
            Marshal.Copy(this._bitmapData, 0, destination, this._bitmapData.Length);
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        /// <summary>
        /// The find best match.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="palette">
        /// The palette.
        /// </param>
        /// <param name="maxDiff">
        /// The max diff.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int FindBestMatch(Color color, List<Color> palette, out int maxDiff)
        {
            int smallestDiff = 1000;
            int smallestDiffIndex = -1;
            int i = 0;
            foreach (var pc in palette)
            {
                int diff = Math.Abs(pc.A - color.A) + Math.Abs(pc.R - color.R) + Math.Abs(pc.G - color.G) + Math.Abs(pc.B - color.B);
                if (diff < smallestDiff)
                {
                    smallestDiff = diff;
                    smallestDiffIndex = i;
                    if (smallestDiff < 4)
                    {
                        maxDiff = smallestDiff;
                        return smallestDiffIndex;
                    }
                }

                i++;
            }

            maxDiff = smallestDiff;
            return smallestDiffIndex;
        }

        /// <summary>
        /// The conver to 8 bits per pixel.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap ConverTo8BitsPerPixel()
        {
            var newBitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);
            var palette = new List<Color> { Color.Transparent };
            ColorPalette bPalette = newBitmap.Palette;
            var entries = bPalette.Entries;
            for (int i = 0; i < newBitmap.Palette.Entries.Length; i++)
            {
                entries[i] = Color.Transparent;
            }

            BitmapData data = newBitmap.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    Color c = this.GetPixel(x, y);
                    if (c.A < 5)
                    {
                        bytes[y * data.Stride + x] = 0;
                    }
                    else
                    {
                        int maxDiff;
                        int index = FindBestMatch(c, palette, out maxDiff);

                        if (index == -1 && palette.Count < 255)
                        {
                            index = palette.Count;
                            entries[index] = c;
                            palette.Add(c);
                            bytes[y * data.Stride + x] = (byte)index;
                        }
                        else if (palette.Count < 200 && maxDiff > 5)
                        {
                            index = palette.Count;
                            entries[index] = c;
                            palette.Add(c);
                            bytes[y * data.Stride + x] = (byte)index;
                        }
                        else if (palette.Count < 255 && maxDiff > 15)
                        {
                            index = palette.Count;
                            entries[index] = c;
                            palette.Add(c);
                            bytes[y * data.Stride + x] = (byte)index;
                        }
                        else if (index >= 0)
                        {
                            bytes[y * data.Stride + x] = (byte)index;
                        }
                    }
                }
            }

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            newBitmap.UnlockBits(data);
            newBitmap.Palette = bPalette;
            return newBitmap;
        }

        /// <summary>
        /// The copy rectangle.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <returns>
        /// The <see cref="NikseBitmap"/>.
        /// </returns>
        public NikseBitmap CopyRectangle(Rectangle section)
        {
            if (section.Bottom > this.Height)
            {
                section = new Rectangle(section.Left, section.Top, section.Width, this.Height - section.Top);
            }

            if (section.Width + section.Left > this.Width)
            {
                section = new Rectangle(section.Left, section.Top, this.Width - section.Left, section.Height);
            }

            var newBitmapData = new byte[section.Width * section.Height * 4];
            int index = 0;
            for (int y = section.Top; y < section.Bottom; y++)
            {
                int pixelAddress = (section.Left * 4) + (y * 4 * this.Width);
                Buffer.BlockCopy(this._bitmapData, pixelAddress, newBitmapData, index, 4 * section.Width);
                index += 4 * section.Width;
            }

            return new NikseBitmap(section.Width, section.Height, newBitmapData);
        }

        /// <summary>
        /// Returns brightest color (not white though)
        /// </summary>
        /// <returns>Brightest color, if not found or if brightes color is white, then Color.Transparent is returned</returns>
        public Color GetBrightestColor()
        {
            int max = this.Width * this.Height - 4;
            Color brightest = Color.Black;
            for (int i = 0; i < max; i++)
            {
                Color c = this.GetPixelNext();
                if (c.A > 220 && c.R + c.G + c.B > 200 && c.R + c.G + c.B > brightest.R + brightest.G + brightest.B)
                {
                    brightest = c;
                }
            }

            if (IsColorClose(Color.White, brightest, 40))
            {
                return Color.Transparent;
            }

            if (IsColorClose(Color.Black, brightest, 10))
            {
                return Color.Transparent;
            }

            return brightest;
        }

        /// <summary>
        /// The is color close.
        /// </summary>
        /// <param name="color1">
        /// The color 1.
        /// </param>
        /// <param name="color2">
        /// The color 2.
        /// </param>
        /// <param name="maxDiff">
        /// The max diff.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsColorClose(Color color1, Color color2, int maxDiff)
        {
            if (Math.Abs(color1.R - color2.R) < maxDiff && Math.Abs(color1.G - color2.G) < maxDiff && Math.Abs(color1.B - color2.B) < maxDiff)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The gray scale.
        /// </summary>
        public void GrayScale()
        {
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                int medium = Convert.ToInt32((this._bitmapData[i + 2] + this._bitmapData[i + 1] + this._bitmapData[i]) * 1.5 / 3.0 + 2);
                if (medium > byte.MaxValue)
                {
                    medium = byte.MaxValue;
                }

                this._bitmapData[i + 2] = this._bitmapData[i + 1] = this._bitmapData[i] = (byte)medium;
            }
        }

        /// <summary>
        /// Make pixels with some transparency completely transparent
        /// </summary>
        /// <param name="minAlpha">
        /// Min alpha value, 0=transparent, 255=fully visible
        /// </param>
        internal void MakeBackgroundTransparent(int minAlpha)
        {
            var buffer = new byte[4];
            buffer[0] = 0; // B
            buffer[1] = 0; // G
            buffer[2] = 0; // R
            buffer[3] = 0; // A
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 3] < minAlpha)
                {
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
                }
            }
        }

        /// <summary>
        /// The make two color.
        /// </summary>
        /// <param name="minRgb">
        /// The min rgb.
        /// </param>
        internal void MakeTwoColor(int minRgb)
        {
            var buffer = new byte[4];
            buffer[0] = 0; // B
            buffer[1] = 0; // G
            buffer[2] = 0; // R
            buffer[3] = 0; // A
            var bufferWhite = new byte[4];
            bufferWhite[0] = 255; // B
            bufferWhite[1] = 255; // G
            bufferWhite[2] = 255; // R
            bufferWhite[3] = 255; // A
            for (int i = 0; i < this._bitmapData.Length; i += 4)
            {
                if (this._bitmapData[i + 3] < 1 || (this._bitmapData[i + 0] + this._bitmapData[i + 1] + this._bitmapData[i + 2] < minRgb))
                {
                    Buffer.BlockCopy(buffer, 0, this._bitmapData, i, 4);
                }
                else
                {
                    Buffer.BlockCopy(bufferWhite, 0, this._bitmapData, i, 4);
                }
            }
        }

        /// <summary>
        /// The make vertical line part transparent.
        /// </summary>
        /// <param name="xStart">
        /// The x start.
        /// </param>
        /// <param name="xEnd">
        /// The x end.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        internal void MakeVerticalLinePartTransparent(int xStart, int xEnd, int y)
        {
            if (xEnd > this.Width - 1)
            {
                xEnd = this.Width - 1;
            }

            if (xStart < 0)
            {
                xStart = 0;
            }

            int i = (xStart * 4) + (y * 4 * this.Width);
            int end = (xEnd * 4) + (y * 4 * this.Width) + 4;
            while (i < end)
            {
                this._bitmapData[i] = 0;
                i++;
            }
        }

        /// <summary>
        /// The add transparent line right.
        /// </summary>
        internal void AddTransparentLineRight()
        {
            int newWidth = this.Width + 1;

            var newBitmapData = new byte[newWidth * this.Height * 4];
            int index = 0;
            for (int y = 0; y < this.Height; y++)
            {
                int pixelAddress = (0 * 4) + (y * 4 * this.Width);
                Buffer.BlockCopy(this._bitmapData, pixelAddress, newBitmapData, index, 4 * this.Width);
                index += 4 * newWidth;
            }

            this.Width = newWidth;
            this._bitmapData = newBitmapData;
            for (int y = 0; y < this.Height; y++)
            {
                this.SetPixel(this.Width - 1, y, Color.Transparent);
            }
        }

        /// <summary>
        /// The save as targa.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void SaveAsTarga(string fileName)
        {
            // TGA header (18-byte fixed header)
            byte[] header = { 0, // ID length (1 bytes)
                              0, // no color map (1 bytes)
                              2, // uncompressed, true color (1 bytes)
                              0, 0, // Color map First Entry Index
                              0, 0, // Color map Length
                              0, // Color map Entry Size
                              0, 0, 0, 0, // x and y origin
                              (byte)(this.Width & 0x00FF), (byte)((this.Width & 0xFF00) >> 8), (byte)(this.Height & 0x00FF), (byte)((this.Height & 0xFF00) >> 8), 32, // pixel depth - 32=32 bit bitmap
                              0 // Image Descriptor
                            };

            var pixels = new byte[this._bitmapData.Length];
            int offsetDest = 0;
            for (int y = this.Height - 1; y >= 0; y--)
            {
                // takes lines from bottom lines to top (mirrowed horizontally)
                for (int x = 0; x < this.Width; x++)
                {
                    var c = this.GetPixel(x, y);
                    pixels[offsetDest] = c.B;
                    pixels[offsetDest + 1] = c.G;
                    pixels[offsetDest + 2] = c.R;
                    pixels[offsetDest + 3] = c.A;
                    offsetDest += 4;
                }
            }

            using (var fileStream = File.Create(fileName))
            {
                fileStream.Write(header, 0, header.Length);
                fileStream.Write(pixels, 0, pixels.Length);
            }
        }
    }
}