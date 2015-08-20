// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubPicture.cs" company="">
//   
// </copyright>
// <summary>
//   Subtitle Picture - see http://www.mpucoder.com/DVD/spu.html for more info
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Subtitle Picture - see http://www.mpucoder.com/DVD/spu.html for more info
    /// </summary>
    public class SubPicture
    {
        /// <summary>
        /// The _data.
        /// </summary>
        private readonly byte[] _data;

        /// <summary>
        /// The sub picture date size.
        /// </summary>
        public readonly int SubPictureDateSize;

        /// <summary>
        /// The _pixel data address offset.
        /// </summary>
        private int _pixelDataAddressOffset;

        /// <summary>
        /// The _start display control sequence table address.
        /// </summary>
        private int _startDisplayControlSequenceTableAddress;

        /// <summary>
        /// The delay.
        /// </summary>
        public TimeSpan Delay;

        /// <summary>
        /// The image display area.
        /// </summary>
        public Rectangle ImageDisplayArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubPicture"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public SubPicture(byte[] data)
        {
            this._data = data;
            this.SubPictureDateSize = Helper.GetEndianWord(this._data, 0);
            this._startDisplayControlSequenceTableAddress = Helper.GetEndianWord(this._data, 2);
            this.ParseDisplayControlCommands(false, null, null, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubPicture"/> class. 
        /// For SP packet with DVD subpictures
        /// </summary>
        /// <param name="data">
        /// Byte data buffer
        /// </param>
        /// <param name="startDisplayControlSequenceTableAddress">
        /// Address of first control sequence in data
        /// </param>
        /// <param name="pixelDataAddressOffset">
        /// Bitmap pixel data address offset
        /// </param>
        public SubPicture(byte[] data, int startDisplayControlSequenceTableAddress, int pixelDataAddressOffset)
        {
            this._data = data;
            this.SubPictureDateSize = this._data.Length;
            this._startDisplayControlSequenceTableAddress = startDisplayControlSequenceTableAddress;
            this._pixelDataAddressOffset = pixelDataAddressOffset;
            this.ParseDisplayControlCommands(false, null, null, false);
        }

        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        public int BufferSize
        {
            get
            {
                return this._data.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether forced.
        /// </summary>
        public bool Forced { get; private set; }

        /// <summary>
        /// Generates the current subtitle image
        /// </summary>
        /// <param name="colorLookupTable">
        /// The Color LookUp Table (CLUT), if null then only the four colors are used (should contain 16 elements if not null)
        /// </param>
        /// <param name="background">
        /// Background color
        /// </param>
        /// <param name="pattern">
        /// Color
        /// </param>
        /// <param name="emphasis1">
        /// Color
        /// </param>
        /// <param name="emphasis2">
        /// Color
        /// </param>
        /// <param name="useCustomColors">
        /// Use custom colors instead of lookup table
        /// </param>
        /// <returns>
        /// Subtitle image
        /// </returns>
        public Bitmap GetBitmap(List<Color> colorLookupTable, Color background, Color pattern, Color emphasis1, Color emphasis2, bool useCustomColors)
        {
            var fourColors = new List<Color> { background, pattern, emphasis1, emphasis2 };
            return this.ParseDisplayControlCommands(true, colorLookupTable, fourColors, useCustomColors);
        }

        /// <summary>
        /// The parse display control commands.
        /// </summary>
        /// <param name="createBitmap">
        /// The create bitmap.
        /// </param>
        /// <param name="colorLookUpTable">
        /// The color look up table.
        /// </param>
        /// <param name="fourColors">
        /// The four colors.
        /// </param>
        /// <param name="useCustomColors">
        /// The use custom colors.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private Bitmap ParseDisplayControlCommands(bool createBitmap, List<Color> colorLookUpTable, List<Color> fourColors, bool useCustomColors)
        {
            this.ImageDisplayArea = new Rectangle();
            Bitmap bmp = null;
            var displayControlSequenceTableAddresses = new List<int>();
            int imageTopFieldDataAddress = 0;
            int imageBottomFieldDataAddress = 0;
            bool bitmapGenerated = false;
            double largestDelay = -999999;
            int displayControlSequenceTableAddress = this._startDisplayControlSequenceTableAddress - this._pixelDataAddressOffset;
            int lastDisplayControlSequenceTableAddress = 0;
            displayControlSequenceTableAddresses.Add(displayControlSequenceTableAddress);
            int commandIndex = 0;
            while (displayControlSequenceTableAddress > lastDisplayControlSequenceTableAddress && displayControlSequenceTableAddress + 1 < this._data.Length && commandIndex < this._data.Length)
            {
                int delayBeforeExecute = Helper.GetEndianWord(this._data, displayControlSequenceTableAddress + this._pixelDataAddressOffset);
                commandIndex = displayControlSequenceTableAddress + 4 + this._pixelDataAddressOffset;
                if (commandIndex >= this._data.Length)
                {
                    break; // invalid index
                }

                int command = this._data[commandIndex];
                int numberOfCommands = 0;
                while (command != (int)DisplayControlCommand.End && numberOfCommands < 1000 && commandIndex < this._data.Length)
                {
                    numberOfCommands++;
                    switch (command)
                    {
                        case (int)DisplayControlCommand.ForcedStartDisplay: // 0
                            this.Forced = true;
                            commandIndex++;
                            break;
                        case (int)DisplayControlCommand.StartDisplay: // 1
                            commandIndex++;
                            break;
                        case (int)DisplayControlCommand.StopDisplay: // 2
                            this.Delay = TimeSpan.FromMilliseconds(((delayBeforeExecute << 10) + 1023) / 90.0);
                            if (createBitmap && this.Delay.TotalMilliseconds > largestDelay)
                            {
                                // in case of more than one images, just use the one with the largest display time
                                largestDelay = this.Delay.TotalMilliseconds;
                                if (bmp != null)
                                {
                                    bmp.Dispose();
                                }

                                bmp = this.GenerateBitmap(this.ImageDisplayArea, imageTopFieldDataAddress, imageBottomFieldDataAddress, fourColors);
                                bitmapGenerated = true;
                            }

                            commandIndex++;
                            break;
                        case (int)DisplayControlCommand.SetColor: // 3
                            if (colorLookUpTable != null && fourColors.Count == 4)
                            {
                                byte[] imageColor = { this._data[commandIndex + 1], this._data[commandIndex + 2] };
                                if (!useCustomColors)
                                {
                                    SetColor(fourColors, 3, imageColor[0] >> 4, colorLookUpTable);
                                    SetColor(fourColors, 2, imageColor[0] & Helper.B00001111, colorLookUpTable);
                                    SetColor(fourColors, 1, imageColor[1] >> 4, colorLookUpTable);
                                    SetColor(fourColors, 0, imageColor[1] & Helper.B00001111, colorLookUpTable);
                                }
                            }

                            commandIndex += 3;
                            break;
                        case (int)DisplayControlCommand.SetContrast: // 4
                            if (colorLookUpTable != null && fourColors.Count == 4)
                            {
                                var imageContrast = new[] { this._data[commandIndex + 1], this._data[commandIndex + 2] };
                                if (imageContrast[0] + imageContrast[1] > 0)
                                {
                                    SetTransparency(fourColors, 3, (imageContrast[0] & 0xF0) >> 4);
                                    SetTransparency(fourColors, 2, imageContrast[0] & Helper.B00001111);
                                    SetTransparency(fourColors, 1, (imageContrast[1] & 0xF0) >> 4);
                                    SetTransparency(fourColors, 0, imageContrast[1] & Helper.B00001111);
                                }
                            }

                            commandIndex += 3;
                            break;
                        case (int)DisplayControlCommand.SetDisplayArea: // 5
                            if (this._data.Length > commandIndex + 6)
                            {
                                int startingX = (this._data[commandIndex + 1] << 8 | this._data[commandIndex + 2]) >> 4;
                                int endingX = (this._data[commandIndex + 2] & Helper.B00001111) << 8 | this._data[commandIndex + 3];
                                int startingY = (this._data[commandIndex + 4] << 8 | this._data[commandIndex + 5]) >> 4;
                                int endingY = (this._data[commandIndex + 5] & Helper.B00001111) << 8 | this._data[commandIndex + 6];
                                this.ImageDisplayArea = new Rectangle(startingX, startingY, endingX - startingX, endingY - startingY);
                            }

                            commandIndex += 7;
                            break;
                        case (int)DisplayControlCommand.SetPixelDataAddress: // 6
                            imageTopFieldDataAddress = Helper.GetEndianWord(this._data, commandIndex + 1) + this._pixelDataAddressOffset;
                            imageBottomFieldDataAddress = Helper.GetEndianWord(this._data, commandIndex + 3) + this._pixelDataAddressOffset;
                            commandIndex += 5;
                            break;
                        case (int)DisplayControlCommand.ChangeColorAndContrast: // 7
                            commandIndex++;

                            // int parameterAreaSize = (int)Helper.GetEndian(_data, commandIndex, 2);
                            if (commandIndex + 1 < this._data.Length)
                            {
                                int parameterAreaSize = this._data[commandIndex + 1]; // this should be enough??? (no larger than 255 bytes)
                                if (colorLookUpTable != null)
                                {
                                    // TODO: Set fourColors
                                }

                                commandIndex += parameterAreaSize;
                            }
                            else
                            {
                                commandIndex++;
                            }

                            break;
                        default:
                            commandIndex++;
                            break;
                    }

                    if (commandIndex >= this._data.Length)
                    {
                        // in case of bad files...
                        break;
                    }

                    command = this._data[commandIndex];
                }

                lastDisplayControlSequenceTableAddress = displayControlSequenceTableAddress;
                if (this._pixelDataAddressOffset == -4)
                {
                    displayControlSequenceTableAddress = Helper.GetEndianWord(this._data, commandIndex + 3);
                }
                else
                {
                    displayControlSequenceTableAddress = Helper.GetEndianWord(this._data, displayControlSequenceTableAddress + 2);
                }
            }

            if (createBitmap && !bitmapGenerated)
            {
                // StopDisplay not needed (delay will be zero - should be just before start of next subtitle)
                bmp = this.GenerateBitmap(this.ImageDisplayArea, imageTopFieldDataAddress, imageBottomFieldDataAddress, fourColors);
            }

            return bmp;
        }

        /// <summary>
        /// The set color.
        /// </summary>
        /// <param name="fourColors">
        /// The four colors.
        /// </param>
        /// <param name="fourColorIndex">
        /// The four color index.
        /// </param>
        /// <param name="clutIndex">
        /// The clut index.
        /// </param>
        /// <param name="colorLookUpTable">
        /// The color look up table.
        /// </param>
        private static void SetColor(List<Color> fourColors, int fourColorIndex, int clutIndex, List<Color> colorLookUpTable)
        {
            if (clutIndex >= 0 && clutIndex < colorLookUpTable.Count && fourColorIndex >= 0)
            {
                fourColors[fourColorIndex] = colorLookUpTable[clutIndex];
            }
        }

        /// <summary>
        /// The set transparency.
        /// </summary>
        /// <param name="fourColors">
        /// The four colors.
        /// </param>
        /// <param name="fourColorIndex">
        /// The four color index.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        private static void SetTransparency(List<Color> fourColors, int fourColorIndex, int alpha)
        {
            // alpha: 0x0 = transparent, 0xF = opaque (in C# 0 is fully transparent, and 255 is fully opaque so we have to multiply by 17)
            if (fourColorIndex >= 0)
            {
                fourColors[fourColorIndex] = Color.FromArgb(alpha * 17, fourColors[fourColorIndex].R, fourColors[fourColorIndex].G, fourColors[fourColorIndex].B);
            }
        }

        /// <summary>
        /// The generate bitmap.
        /// </summary>
        /// <param name="imageDisplayArea">
        /// The image display area.
        /// </param>
        /// <param name="imageTopFieldDataAddress">
        /// The image top field data address.
        /// </param>
        /// <param name="imageBottomFieldDataAddress">
        /// The image bottom field data address.
        /// </param>
        /// <param name="fourColors">
        /// The four colors.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private Bitmap GenerateBitmap(Rectangle imageDisplayArea, int imageTopFieldDataAddress, int imageBottomFieldDataAddress, List<Color> fourColors)
        {
            if (imageDisplayArea.Width <= 0 || imageDisplayArea.Height <= 0)
            {
                return new Bitmap(1, 1);
            }

            var bmp = new Bitmap(imageDisplayArea.Width + 1, imageDisplayArea.Height + 1);
            if (fourColors[0] != Color.Transparent)
            {
                var gr = Graphics.FromImage(bmp);
                gr.FillRectangle(new SolidBrush(fourColors[0]), new Rectangle(0, 0, bmp.Width, bmp.Height));
                gr.Dispose();
            }

            var fastBmp = new FastBitmap(bmp);
            fastBmp.LockImage();
            GenerateBitmap(this._data, fastBmp, 0, imageTopFieldDataAddress, fourColors, 2);
            GenerateBitmap(this._data, fastBmp, 1, imageBottomFieldDataAddress, fourColors, 2);
            Bitmap cropped = CropBitmapAndUnlok(fastBmp, fourColors[0]);
            bmp.Dispose();
            return cropped;
        }

        /// <summary>
        /// The crop bitmap and unlok.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="backgroundColor">
        /// The background color.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private static Bitmap CropBitmapAndUnlok(FastBitmap bmp, Color backgroundColor)
        {
            int y = 0;
            int x;
            Color c = backgroundColor;
            int backgroundArgb = backgroundColor.ToArgb();

            // Crop top
            while (y < bmp.Height && IsBackgroundColor(c, backgroundArgb))
            {
                c = bmp.GetPixel(0, y);
                if (IsBackgroundColor(c, backgroundArgb))
                {
                    for (x = 1; x < bmp.Width; x++)
                    {
                        c = bmp.GetPixelNext();
                        if (c.A != 0 && c.ToArgb() != backgroundArgb)
                        {
                            break;
                        }
                    }
                }

                if (IsBackgroundColor(c, backgroundArgb))
                {
                    y++;
                }
            }

            int minY = y;
            if (minY > 3)
            {
                minY -= 3;
            }
            else
            {
                minY = 0;
            }

            // Crop left
            x = 0;
            c = backgroundColor;
            while (x < bmp.Width && IsBackgroundColor(c, backgroundArgb))
            {
                for (y = minY; y < bmp.Height; y++)
                {
                    c = bmp.GetPixel(x, y);
                    if (!IsBackgroundColor(c, backgroundArgb))
                    {
                        break;
                    }
                }

                if (IsBackgroundColor(c, backgroundArgb))
                {
                    x++;
                }
            }

            int minX = x;
            if (minX > 3)
            {
                minX -= 3;
            }
            else
            {
                minX -= 0;
            }

            // Crop bottom
            y = bmp.Height - 1;
            c = backgroundColor;
            while (y > minY && IsBackgroundColor(c, backgroundArgb))
            {
                c = bmp.GetPixel(0, y);
                if (IsBackgroundColor(c, backgroundArgb))
                {
                    for (x = 1; x < bmp.Width; x++)
                    {
                        c = bmp.GetPixelNext();
                        if (!IsBackgroundColor(c, backgroundArgb))
                        {
                            break;
                        }
                    }
                }

                if (IsBackgroundColor(c, backgroundArgb))
                {
                    y--;
                }
            }

            int maxY = y + 7;
            if (maxY >= bmp.Height)
            {
                maxY = bmp.Height - 1;
            }

            // Crop right
            x = bmp.Width - 1;
            c = backgroundColor;
            while (x > minX && IsBackgroundColor(c, backgroundArgb))
            {
                for (y = minY; y < bmp.Height; y++)
                {
                    c = bmp.GetPixel(x, y);
                    if (!IsBackgroundColor(c, backgroundArgb))
                    {
                        break;
                    }
                }

                if (IsBackgroundColor(c, backgroundArgb))
                {
                    x--;
                }
            }

            int maxX = x + 7;
            if (maxX >= bmp.Width)
            {
                maxX = bmp.Width - 1;
            }

            bmp.UnlockImage();
            Bitmap bmpImage = bmp.GetBitmap();
            if (bmpImage.Width > 1 && bmpImage.Height > 1 && maxX - minX > 0 && maxY - minY > 0)
            {
                Bitmap bmpCrop = bmpImage.Clone(new Rectangle(minX, minY, maxX - minX, maxY - minY), bmpImage.PixelFormat);
                return bmpCrop;
            }

            return (Bitmap)bmpImage.Clone();
        }

        /// <summary>
        /// The is background color.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="backgroundArgb">
        /// The background argb.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsBackgroundColor(Color c, int backgroundArgb)
        {
            return c.A == 0 || c.ToArgb() == backgroundArgb;
        }

        /// <summary>
        /// The generate bitmap.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="startY">
        /// The start y.
        /// </param>
        /// <param name="dataAddress">
        /// The data address.
        /// </param>
        /// <param name="fourColors">
        /// The four colors.
        /// </param>
        /// <param name="addY">
        /// The add y.
        /// </param>
        public static void GenerateBitmap(byte[] data, FastBitmap bmp, int startY, int dataAddress, List<Color> fourColors, int addY)
        {
            int index = 0;
            bool onlyHalf = false;
            int y = startY;
            int x = 0;
            int colorZeroValue = fourColors[0].ToArgb();
            while (y < bmp.Height && dataAddress + index + 2 < data.Length)
            {
                int runLength;
                int color;
                bool restOfLine;
                index += DecodeRle(dataAddress + index, data, out color, out runLength, ref onlyHalf, out restOfLine);
                if (restOfLine)
                {
                    runLength = bmp.Width - x;
                }

                Color c = fourColors[color]; // set color via the four colors
                for (int i = 0; i < runLength; i++, x++)
                {
                    if (x >= bmp.Width - 1)
                    {
                        if (y < bmp.Height && x < bmp.Width && c != fourColors[0])
                        {
                            bmp.SetPixel(x, y, c);
                        }

                        if (onlyHalf)
                        {
                            onlyHalf = false;
                            index++;
                        }

                        x = 0;
                        y += addY;
                        break;
                    }

                    if (y < bmp.Height && c.ToArgb() != colorZeroValue)
                    {
                        bmp.SetPixel(x, y, c);
                    }
                }
            }
        }

        /// <summary>
        /// The decode rle.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <param name="onlyHalf">
        /// The only half.
        /// </param>
        /// <param name="restOfLine">
        /// The rest of line.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int DecodeRle(int index, byte[] data, out int color, out int runLength, ref bool onlyHalf, out bool restOfLine)
        {
            // Value      Bits   n=length, c=color
            // 1-3        4      nncc               (half a byte)
            // 4-15       8      00nnnncc           (one byte)
            // 16-63     12      0000nnnnnncc       (one and a half byte)
            // 64-255    16      000000nnnnnnnncc   (two bytes)
            // When reaching EndOfLine, index is byte aligned (skip 4 bits if necessary)
            restOfLine = false;
            byte b1 = data[index];
            byte b2 = data[index + 1];

            if (onlyHalf)
            {
                byte b3 = data[index + 2];
                b1 = (byte)(((b1 & Helper.B00001111) << 4) | ((b2 & Helper.B11110000) >> 4));
                b2 = (byte)(((b2 & Helper.B00001111) << 4) | ((b3 & Helper.B11110000) >> 4));
            }

            if (b1 >> 2 == 0)
            {
                runLength = (b1 << 6) | (b2 >> 2);
                color = b2 & Helper.B00000011;
                if (runLength == 0)
                {
                    // rest of line + skip 4 bits if Only half
                    restOfLine = true;
                    if (onlyHalf)
                    {
                        onlyHalf = false;
                        return 3;
                    }
                }

                return 2;
            }

            if (b1 >> 4 == 0)
            {
                runLength = (b1 << 2) | (b2 >> 6);
                color = (b2 & Helper.B00110000) >> 4;
                if (onlyHalf)
                {
                    onlyHalf = false;
                    return 2;
                }

                onlyHalf = true;
                return 1;
            }

            if (b1 >> 6 == 0)
            {
                runLength = b1 >> 2;
                color = b1 & Helper.B00000011;
                return 1;
            }

            runLength = b1 >> 6;
            color = (b1 & Helper.B00110000) >> 4;

            if (onlyHalf)
            {
                onlyHalf = false;
                return 1;
            }

            onlyHalf = true;
            return 0;
        }

        /// <summary>
        /// The display control command.
        /// </summary>
        private enum DisplayControlCommand
        {
            /// <summary>
            /// The forced start display.
            /// </summary>
            ForcedStartDisplay = 0, 

            /// <summary>
            /// The start display.
            /// </summary>
            StartDisplay = 1, 

            /// <summary>
            /// The stop display.
            /// </summary>
            StopDisplay = 2, 

            /// <summary>
            /// The set color.
            /// </summary>
            SetColor = 3, 

            /// <summary>
            /// The set contrast.
            /// </summary>
            SetContrast = 4, 

            /// <summary>
            /// The set display area.
            /// </summary>
            SetDisplayArea = 5, 

            /// <summary>
            /// The set pixel data address.
            /// </summary>
            SetPixelDataAddress = 6, 

            /// <summary>
            /// The change color and contrast.
            /// </summary>
            ChangeColorAndContrast = 7, 

            /// <summary>
            /// The end.
            /// </summary>
            End = 0xFF, 
        }
    }
}