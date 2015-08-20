// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XSub.cs" company="">
//   
// </copyright>
// <summary>
//   The x sub.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// The x sub.
    /// </summary>
    public class XSub
    {
        /// <summary>
        /// The color buffer.
        /// </summary>
        private byte[] colorBuffer;

        /// <summary>
        /// The rle buffer.
        /// </summary>
        private byte[] rleBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XSub"/> class.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="colors">
        /// The colors.
        /// </param>
        /// <param name="rle">
        /// The rle.
        /// </param>
        public XSub(string timeCode, int width, int height, byte[] colors, byte[] rle)
        {
            this.Start = DecodeTimeCode(timeCode.Substring(0, 13));
            this.End = DecodeTimeCode(timeCode.Substring(13, 12));
            this.Width = width;
            this.Height = height;
            this.colorBuffer = colors;
            this.rleBuffer = rle;
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public TimeCode Start { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public TimeCode End { get; set; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timeCode)
        {
            var parts = timeCode.Split(new[] { ':', ';', '.', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
        }

        /// <summary>
        /// The generate bitmap.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="buf">
        /// The buf.
        /// </param>
        /// <param name="fourColors">
        /// The four colors.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GenerateBitmap(FastBitmap bmp, byte[] buf, List<Color> fourColors)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            int nibbleOffset = 0;
            var nibble_end = buf.Length * 2;
            var x = 0;
            var y = 0;
            for (;;)
            {
                if (nibbleOffset >= nibble_end)
                {
                    return -1;
                }

                var v = GetNibble(buf, nibbleOffset++);
                if (v < 0x4)
                {
                    v = (v << 4) | GetNibble(buf, nibbleOffset++);
                    if (v < 0x10)
                    {
                        v = (v << 4) | GetNibble(buf, nibbleOffset++);
                        if (v < 0x040)
                        {
                            v = (v << 4) | GetNibble(buf, nibbleOffset++);
                            if (v < 4)
                            {
                                v |= (w - x) << 2;
                            }
                        }
                    }
                }

                var len = v >> 2;
                if (len > (w - x))
                {
                    len = w - x;
                }

                var color = v & 0x03;
                if (color > 0)
                {
                    Color c = fourColors[color];
                    bmp.SetPixel(x, y, c, len);
                }

                x += len;
                if (x >= w)
                {
                    y++;
                    if (y >= h)
                    {
                        break;
                    }

                    x = 0;
                    nibbleOffset += nibbleOffset & 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// The get nibble.
        /// </summary>
        /// <param name="buf">
        /// The buf.
        /// </param>
        /// <param name="nibble_offset">
        /// The nibble_offset.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetNibble(byte[] buf, int nibble_offset)
        {
            return (buf[nibble_offset >> 1] >> ((1 - (nibble_offset & 1)) << 2)) & 0xf;
        }

        /// <summary>
        /// The get image.
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
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetImage(Color background, Color pattern, Color emphasis1, Color emphasis2)
        {
            var fourColors = new List<Color> { background, pattern, emphasis1, emphasis2 };
            var bmp = new Bitmap(this.Width, this.Height);
            if (fourColors[0] != Color.Transparent)
            {
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.FillRectangle(new SolidBrush(fourColors[0]), new Rectangle(0, 0, bmp.Width, bmp.Height));
                }
            }

            var fastBmp = new FastBitmap(bmp);
            fastBmp.LockImage();
            GenerateBitmap(fastBmp, this.rleBuffer, fourColors);
            fastBmp.UnlockImage();
            return bmp;
        }

        /// <summary>
        /// The get color.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        private Color GetColor(int start)
        {
            return Color.FromArgb(this.colorBuffer[start], this.colorBuffer[start + 1], this.colorBuffer[start + 2]);
        }

        /// <summary>
        /// The get image.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetImage()
        {
            return this.GetImage(Color.Transparent, this.GetColor(3), this.GetColor(6), this.GetColor(9));
        }
    }
}