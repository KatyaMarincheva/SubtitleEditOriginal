// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectDataSegment.cs" company="">
//   
// </copyright>
// <summary>
//   The object data segment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// The object data segment.
    /// </summary>
    public class ObjectDataSegment
    {
        /// <summary>
        /// The pixel decoding 2 bit.
        /// </summary>
        public const int PixelDecoding2Bit = 0x10;

        /// <summary>
        /// The pixel decoding 4 bit.
        /// </summary>
        public const int PixelDecoding4Bit = 0x11;

        /// <summary>
        /// The pixel decoding 8 bit.
        /// </summary>
        public const int PixelDecoding8Bit = 0x12;

        /// <summary>
        /// The map table 2 to 4 bit.
        /// </summary>
        public const int MapTable2To4Bit = 0x20;

        /// <summary>
        /// The map table 2 to 8 bit.
        /// </summary>
        public const int MapTable2To8Bit = 0x21;

        /// <summary>
        /// The map table 4 to 8 bit.
        /// </summary>
        public const int MapTable4To8Bit = 0x22;

        /// <summary>
        /// The end of object line code.
        /// </summary>
        public const int EndOfObjectLineCode = 0xf0;

        /// <summary>
        /// The _fast image.
        /// </summary>
        private FastBitmap _fastImage;

        /// <summary>
        /// The first data types.
        /// </summary>
        public List<string> FirstDataTypes = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataSegment"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public ObjectDataSegment(byte[] buffer, int index)
        {
            this.ObjectId = Helper.GetEndianWord(buffer, index);
            this.ObjectVersionNumber = buffer[index + 2] >> 4;
            this.ObjectCodingMethod = (buffer[index + 2] & Helper.B00001100) >> 2;
            this.NonModifyingColorFlag = (buffer[index + 2] & Helper.B00000010) > 0;
            this.TopFieldDataBlockLength = Helper.GetEndianWord(buffer, index + 3);
            this.BottomFieldDataBlockLength = Helper.GetEndianWord(buffer, index + 5);
            this.BufferIndex = index;
        }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object version number.
        /// </summary>
        public int ObjectVersionNumber { get; set; }

        /// <summary>
        /// 0x00 coding of pixels, 0x01 coded as a string of characters
        /// </summary>
        public int ObjectCodingMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether non modifying color flag.
        /// </summary>
        public bool NonModifyingColorFlag { get; set; }

        /// <summary>
        /// Gets or sets the top field data block length.
        /// </summary>
        public int TopFieldDataBlockLength { get; set; }

        /// <summary>
        /// Gets or sets the bottom field data block length.
        /// </summary>
        public int BottomFieldDataBlockLength { get; set; }

        /// <summary>
        /// Gets or sets the number of codes.
        /// </summary>
        public int NumberOfCodes { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Gets the buffer index.
        /// </summary>
        public int BufferIndex { get; private set; }

        /// <summary>
        /// The decode image.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="cds">
        /// The cds.
        /// </param>
        public void DecodeImage(byte[] buffer, int index, ClutDefinitionSegment cds)
        {
            if (this.ObjectCodingMethod == 0)
            {
                var twoToFourBitColorLookup = new List<int> { 0, 1, 2, 3 };
                var twoToEightBitColorLookup = new List<int> { 0, 1, 2, 3 };
                var fourToEightBitColorLookup = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

                int pixelCode = 0;
                int runLength = 0;
                int dataType = buffer[index + 7];
                int length = this.TopFieldDataBlockLength;

                index += 8;
                int start = index;
                int x = 0;
                int y = 0;

                // Pre-decoding to determine image size
                int width = 0;
                while (index < start + this.TopFieldDataBlockLength)
                {
                    index = CalculateSize(buffer, index, ref dataType, start, ref x, ref y, length, ref runLength, ref width);
                }

                if (width > 2000)
                {
                    width = 2000;
                }

                if (y > 500)
                {
                    y = 500;
                }

                this.Image = new Bitmap(width, y + 1);
                this._fastImage = new FastBitmap(this.Image);
                this._fastImage.LockImage();

                x = 0;
                y = 0;
                index = start;
                while (index < start + this.TopFieldDataBlockLength)
                {
                    index = this.ProcessDataType(buffer, index, cds, ref dataType, start, twoToFourBitColorLookup, fourToEightBitColorLookup, twoToEightBitColorLookup, ref x, ref y, length, ref pixelCode, ref runLength);
                }

                x = 0;
                y = 1;
                if (this.BottomFieldDataBlockLength == 0)
                {
                    index = start;
                }
                else
                {
                    length = this.BottomFieldDataBlockLength;
                    index = start + this.TopFieldDataBlockLength;
                    start = index;
                }

                dataType = buffer[index - 1];
                while (index < start + this.BottomFieldDataBlockLength - 1)
                {
                    index = this.ProcessDataType(buffer, index, cds, ref dataType, start, twoToFourBitColorLookup, fourToEightBitColorLookup, twoToEightBitColorLookup, ref x, ref y, length, ref pixelCode, ref runLength);
                }

                this._fastImage.UnlockImage();
            }
            else if (this.ObjectCodingMethod == 1)
            {
                this.Image = new Bitmap(1, 1);
                this.NumberOfCodes = buffer[index + 3];
            }
        }

        /// <summary>
        /// The process data type.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="cds">
        /// The cds.
        /// </param>
        /// <param name="dataType">
        /// The data type.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="twoToFourBitColorLookup">
        /// The two to four bit color lookup.
        /// </param>
        /// <param name="fourToEightBitColorLookup">
        /// The four to eight bit color lookup.
        /// </param>
        /// <param name="twoToEightBitColorLookup">
        /// The two to eight bit color lookup.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="pixelCode">
        /// The pixel code.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int ProcessDataType(byte[] buffer, int index, ClutDefinitionSegment cds, ref int dataType, int start, List<int> twoToFourBitColorLookup, List<int> fourToEightBitColorLookup, List<int> twoToEightBitColorLookup, ref int x, ref int y, int length, ref int pixelCode, ref int runLength)
        {
            if (dataType == PixelDecoding2Bit)
            {
                int bitIndex = 0;
                while (index < start + length - 1 && TwoBitPixelDecoding(buffer, ref index, ref bitIndex, out pixelCode, out runLength))
                {
                    this.DrawPixels(cds, twoToFourBitColorLookup[pixelCode], runLength, ref x, ref y);
                }
            }
            else if (dataType == PixelDecoding4Bit)
            {
                bool startHalf = false;
                while (index < start + length - 1 && FourBitPixelDecoding(buffer, ref index, ref startHalf, out pixelCode, out runLength))
                {
                    this.DrawPixels(cds, fourToEightBitColorLookup[pixelCode], runLength, ref x, ref y);
                }
            }
            else if (dataType == PixelDecoding8Bit)
            {
                while (index < start + length - 1 && EightBitPixelDecoding(buffer, ref index, out pixelCode, out runLength))
                {
                    this.DrawPixels(cds, pixelCode, runLength, ref x, ref y);
                }
            }
            else if (dataType == MapTable2To4Bit)
            {
                // 4 entry numbers of 4-bits each; entry number 0 first, entry number 3 last
                twoToFourBitColorLookup[0] = buffer[index] >> 4;
                twoToFourBitColorLookup[1] = buffer[index] & Helper.B00001111;
                index++;
                twoToFourBitColorLookup[2] = buffer[index] >> 4;
                twoToFourBitColorLookup[3] = buffer[index] & Helper.B00001111;
                index++;
            }
            else if (dataType == MapTable2To8Bit)
            {
                // 4 entry numbers of 8-bits each; entry number 0 first, entry number 3 last
                twoToEightBitColorLookup[0] = buffer[index];
                index++;
                twoToEightBitColorLookup[1] = buffer[index];
                index++;
                twoToEightBitColorLookup[2] = buffer[index];
                index++;
                twoToEightBitColorLookup[3] = buffer[index];
                index++;
            }
            else if (dataType == MapTable4To8Bit)
            {
                // 16 entry numbers of 8-bits each
                for (int k = 0; k < 16; k++)
                {
                    fourToEightBitColorLookup[k] = buffer[index];
                    index++;
                }
            }
            else if (dataType == EndOfObjectLineCode)
            {
                x = 0;
                y += 2; // interlaced - skip one line
            }

            if (index < start + length)
            {
                dataType = buffer[index];
                index++;
            }

            return index;
        }

        /// <summary>
        /// The calculate size.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="dataType">
        /// The data type.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalculateSize(byte[] buffer, int index, ref int dataType, int start, ref int x, ref int y, int length, ref int runLength, ref int width)
        {
            int pixelCode;
            if (dataType == PixelDecoding2Bit)
            {
                int bitIndex = 0;
                while (index < start + length - 1 && TwoBitPixelDecoding(buffer, ref index, ref bitIndex, out pixelCode, out runLength))
                {
                    x += runLength;
                }
            }
            else if (dataType == PixelDecoding4Bit)
            {
                bool startHalf = false;
                while (index < start + length - 1 && FourBitPixelDecoding(buffer, ref index, ref startHalf, out pixelCode, out runLength))
                {
                    x += runLength;
                }
            }
            else if (dataType == PixelDecoding8Bit)
            {
                while (index < start + length - 1 && EightBitPixelDecoding(buffer, ref index, out pixelCode, out runLength))
                {
                    x += runLength;
                }
            }
            else if (dataType == MapTable2To4Bit)
            {
                index += 2;
            }
            else if (dataType == MapTable2To8Bit)
            {
                index += 4;
            }
            else if (dataType == MapTable4To8Bit)
            {
                index += 16;
            }
            else if (dataType == EndOfObjectLineCode)
            {
                x = 0;
                y += 2; // interlaced - skip one line
            }

            if (index < start + length)
            {
                dataType = buffer[index];
                index++;
            }

            if (x > width)
            {
                width = x;
            }

            return index;
        }

        /// <summary>
        /// The draw pixels.
        /// </summary>
        /// <param name="cds">
        /// The cds.
        /// </param>
        /// <param name="pixelCode">
        /// The pixel code.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        private void DrawPixels(ClutDefinitionSegment cds, int pixelCode, int runLength, ref int x, ref int y)
        {
            var c = Color.Red;
            if (cds != null)
            {
                foreach (var item in cds.Entries)
                {
                    if (item.ClutEntryId == pixelCode)
                    {
                        c = item.GetColor();
                        break;
                    }
                }
            }

            for (int k = 0; k < runLength; k++)
            {
                if (y < this._fastImage.Height && x < this._fastImage.Width)
                {
                    this._fastImage.SetPixel(x, y, c);
                }

                x++;
            }
        }

        /// <summary>
        /// The next 8 bits.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int Next8Bits(byte[] buffer, ref int index)
        {
            int result = buffer[index];
            index++;
            return result;
        }

        /// <summary>
        /// The eight bit pixel decoding.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="pixelCode">
        /// The pixel code.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool EightBitPixelDecoding(byte[] buffer, ref int index, out int pixelCode, out int runLength)
        {
            pixelCode = 0;
            runLength = 1;
            int firstByte = Next8Bits(buffer, ref index);
            if (firstByte != 0)
            {
                runLength = 1;
                pixelCode = firstByte;
            }
            else
            {
                int nextByte = Next8Bits(buffer, ref index);
                if (nextByte >> 7 == 0)
                {
                    if (nextByte != 0)
                    {
                        runLength = nextByte & Helper.B01111111; // 1-127
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    runLength = nextByte & Helper.B01111111; // 3-127
                    pixelCode = Next8Bits(buffer, ref index);
                }
            }

            return true;
        }

        /// <summary>
        /// The next 4 bits.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="startHalf">
        /// The start half.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int Next4Bits(byte[] buffer, ref int index, ref bool startHalf)
        {
            int result;
            if (startHalf)
            {
                startHalf = false;
                result = buffer[index] & Helper.B00001111;
                index++;
            }
            else
            {
                startHalf = true;
                result = buffer[index] >> 4;
            }

            return result;
        }

        /// <summary>
        /// The four bit pixel decoding.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="startHalf">
        /// The start half.
        /// </param>
        /// <param name="pixelCode">
        /// The pixel code.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool FourBitPixelDecoding(byte[] buffer, ref int index, ref bool startHalf, out int pixelCode, out int runLength)
        {
            pixelCode = 0;
            runLength = 1;
            int first = Next4Bits(buffer, ref index, ref startHalf);
            if (first != 0)
            {
                pixelCode = first; // Next4Bits(buffer, ref index, ref startHalf);
                runLength = 1;
            }
            else
            {
                int next1 = Next4Bits(buffer, ref index, ref startHalf);
                if ((next1 & Helper.B00001000) == 0)
                {
                    if (next1 != 0)
                    {
                        runLength = (next1 & Helper.B00000111) + 2; // 3-9
                    }
                    else
                    {
                        if (startHalf)
                        {
                            startHalf = false;
                            index++;
                        }

                        return false;
                    }
                }
                else if (next1 == Helper.B00001100)
                {
                    runLength = 1;
                    pixelCode = 0;
                }
                else if (next1 == Helper.B00001101)
                {
                    runLength = 2;
                    pixelCode = 0;
                }
                else
                {
                    int next2 = Next4Bits(buffer, ref index, ref startHalf);
                    if ((next1 & Helper.B00000100) == 0)
                    {
                        runLength = (next1 & Helper.B00000011) + 4; // 4-7
                        pixelCode = next2;
                    }
                    else
                    {
                        int next3 = Next4Bits(buffer, ref index, ref startHalf);
                        if ((next1 & Helper.B00000001) == 0)
                        {
                            runLength = next2 + 9; // 9-24
                            pixelCode = next3;
                        }
                        else if (next1 == Helper.B00001111)
                        {
                            runLength = ((next2 << 4) + next3) + 25; // 25-280
                            pixelCode = Next4Bits(buffer, ref index, ref startHalf);
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The next 2 bits.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="bitIndex">
        /// The bit index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int Next2Bits(byte[] buffer, ref int index, ref int bitIndex)
        {
            int result;
            if (bitIndex == 0)
            {
                bitIndex++;
                result = (buffer[index] & Helper.B11000000) >> 6;
            }
            else if (bitIndex == 1)
            {
                bitIndex++;
                result = (buffer[index] & Helper.B00110000) >> 4;
            }
            else if (bitIndex == 2)
            {
                bitIndex++;
                result = (buffer[index] & Helper.B00001100) >> 2;
            }
            else
            {
                // 3 - last bit pair
                bitIndex = 0;
                result = buffer[index] & Helper.B00000011;
                index++;
            }

            return result;
        }

        /// <summary>
        /// The two bit pixel decoding.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="bitIndex">
        /// The bit index.
        /// </param>
        /// <param name="pixelCode">
        /// The pixel code.
        /// </param>
        /// <param name="runLength">
        /// The run length.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TwoBitPixelDecoding(byte[] buffer, ref int index, ref int bitIndex, out int pixelCode, out int runLength)
        {
            runLength = 1;
            pixelCode = 0;
            int first = Next2Bits(buffer, ref index, ref bitIndex);
            if (first != 0)
            {
                runLength = 1;
                pixelCode = first;
            }
            else
            {
                int next = Next2Bits(buffer, ref index, ref bitIndex);
                if (next == 1)
                {
                    runLength = 1;
                    pixelCode = 0;
                }
                else if (next > 1)
                {
                    runLength = ((next & Helper.B00000001) << 2) + Next2Bits(buffer, ref index, ref bitIndex) + 3; // 3-10
                    pixelCode = Next2Bits(buffer, ref index, ref bitIndex);
                }
                else
                {
                    int next2 = Next2Bits(buffer, ref index, ref bitIndex);
                    if (next2 == Helper.B00000001)
                    {
                        runLength = 2;
                        pixelCode = 0;
                    }
                    else if (next2 == Helper.B00000010)
                    {
                        runLength = (Next2Bits(buffer, ref index, ref bitIndex) << 2) + // 12-27
                                    Next2Bits(buffer, ref index, ref bitIndex) + 12;
                        pixelCode = Next2Bits(buffer, ref index, ref bitIndex);
                    }
                    else if (next2 == Helper.B00000011)
                    {
                        runLength = (Next2Bits(buffer, ref index, ref bitIndex) << 6) + // 29 - 284
                                    (Next2Bits(buffer, ref index, ref bitIndex) << 4) + (Next2Bits(buffer, ref index, ref bitIndex) << 2) + Next2Bits(buffer, ref index, ref bitIndex) + 29;

                        pixelCode = Next2Bits(buffer, ref index, ref bitIndex);
                    }
                    else
                    {
                        if (bitIndex != 0)
                        {
                            index++;
                        }

                        return false; // end of 2-bit/pixel code string
                    }
                }
            }

            return true;
        }
    }
}