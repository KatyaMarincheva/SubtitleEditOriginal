// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helper.cs" company="">
//   
// </copyright>
// <summary>
//   The helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.Text;

    /// <summary>
    /// The helper.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// The int to hex.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string IntToHex(ulong value, int digits)
        {
            return value.ToString("X").PadLeft(digits, '0');
        }

        /// <summary>
        /// The int to hex.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string IntToHex(int value, int digits)
        {
            return value.ToString("X").PadLeft(digits, '0');
        }

        /// <summary>
        /// The int to bin.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string IntToBin(long value, int digits)
        {
            return Convert.ToString(value, 2).PadLeft(digits, '0');
        }

        /// <summary>
        /// The get endian.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint GetEndian(byte[] buffer, int index, int count)
        {
            uint result = 0;
            for (int i = 0; i < count; i++)
            {
                result = (result << 8) + buffer[index + i];
            }

            return result;
        }

        /// <summary>
        /// Get two bytes word stored in endian order
        /// </summary>
        /// <param name="buffer">
        /// Byte array
        /// </param>
        /// <param name="index">
        /// Index in byte array
        /// </param>
        /// <returns>
        /// Word as int
        /// </returns>
        public static int GetEndianWord(byte[] buffer, int index)
        {
            if (index + 1 < buffer.Length)
            {
                return (buffer[index] << 8) + buffer[index + 1];
            }

            return 0;
        }

        /// <summary>
        /// The get little endian 32.
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
        public static int GetLittleEndian32(byte[] buffer, int index)
        {
            return buffer[index + 3] << 24 | (int)buffer[index + 2] << 16 | (int)buffer[index + 1] << 8 | (int)buffer[index + 0];
        }

        /// <summary>
        /// The get binary string.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetBinaryString(byte[] buffer, int index, int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(Convert.ToString(buffer[index + i], 2).PadLeft(8, '0'));
            }

            return sb.ToString();
        }

        /// <summary>
        /// The get u int 32 from binary string.
        /// </summary>
        /// <param name="binaryValue">
        /// The binary value.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public static ulong GetUInt32FromBinaryString(string binaryValue)
        {
            return Convert.ToUInt32(binaryValue, 2);
        }

        #region Binary constants

        /// <summary>
        /// The b 00000000.
        /// </summary>
        public const int B00000000 = 0;

        /// <summary>
        /// The b 00000001.
        /// </summary>
        public const int B00000001 = 1;

        /// <summary>
        /// The b 00000010.
        /// </summary>
        public const int B00000010 = 2;

        /// <summary>
        /// The b 00000011.
        /// </summary>
        public const int B00000011 = 3;

        /// <summary>
        /// The b 00000100.
        /// </summary>
        public const int B00000100 = 4;

        /// <summary>
        /// The b 00000101.
        /// </summary>
        public const int B00000101 = 5;

        /// <summary>
        /// The b 00000110.
        /// </summary>
        public const int B00000110 = 6;

        /// <summary>
        /// The b 00000111.
        /// </summary>
        public const int B00000111 = 7;

        /// <summary>
        /// The b 00001000.
        /// </summary>
        public const int B00001000 = 8;

        /// <summary>
        /// The b 00001001.
        /// </summary>
        public const int B00001001 = 9;

        /// <summary>
        /// The b 00001010.
        /// </summary>
        public const int B00001010 = 10;

        /// <summary>
        /// The b 00001011.
        /// </summary>
        public const int B00001011 = 11;

        /// <summary>
        /// The b 00001100.
        /// </summary>
        public const int B00001100 = 12;

        /// <summary>
        /// The b 00001101.
        /// </summary>
        public const int B00001101 = 13;

        /// <summary>
        /// The b 00001110.
        /// </summary>
        public const int B00001110 = 14;

        /// <summary>
        /// The b 00001111.
        /// </summary>
        public const int B00001111 = 15;

        /// <summary>
        /// The b 00010000.
        /// </summary>
        public const int B00010000 = 16;

        /// <summary>
        /// The b 00010001.
        /// </summary>
        public const int B00010001 = 17;

        /// <summary>
        /// The b 00010010.
        /// </summary>
        public const int B00010010 = 18;

        /// <summary>
        /// The b 00010011.
        /// </summary>
        public const int B00010011 = 19;

        /// <summary>
        /// The b 00010100.
        /// </summary>
        public const int B00010100 = 20;

        /// <summary>
        /// The b 00010101.
        /// </summary>
        public const int B00010101 = 21;

        /// <summary>
        /// The b 00010110.
        /// </summary>
        public const int B00010110 = 22;

        /// <summary>
        /// The b 00010111.
        /// </summary>
        public const int B00010111 = 23;

        /// <summary>
        /// The b 00011000.
        /// </summary>
        public const int B00011000 = 24;

        /// <summary>
        /// The b 00011001.
        /// </summary>
        public const int B00011001 = 25;

        /// <summary>
        /// The b 00011010.
        /// </summary>
        public const int B00011010 = 26;

        /// <summary>
        /// The b 00011011.
        /// </summary>
        public const int B00011011 = 27;

        /// <summary>
        /// The b 00011100.
        /// </summary>
        public const int B00011100 = 28;

        /// <summary>
        /// The b 00011101.
        /// </summary>
        public const int B00011101 = 29;

        /// <summary>
        /// The b 00011110.
        /// </summary>
        public const int B00011110 = 30;

        /// <summary>
        /// The b 00011111.
        /// </summary>
        public const int B00011111 = 31;

        /// <summary>
        /// The b 00100000.
        /// </summary>
        public const int B00100000 = 32;

        /// <summary>
        /// The b 00100001.
        /// </summary>
        public const int B00100001 = 33;

        /// <summary>
        /// The b 00100010.
        /// </summary>
        public const int B00100010 = 34;

        /// <summary>
        /// The b 00100011.
        /// </summary>
        public const int B00100011 = 35;

        /// <summary>
        /// The b 00100100.
        /// </summary>
        public const int B00100100 = 36;

        /// <summary>
        /// The b 00100101.
        /// </summary>
        public const int B00100101 = 37;

        /// <summary>
        /// The b 00100110.
        /// </summary>
        public const int B00100110 = 38;

        /// <summary>
        /// The b 00100111.
        /// </summary>
        public const int B00100111 = 39;

        /// <summary>
        /// The b 00101000.
        /// </summary>
        public const int B00101000 = 40;

        /// <summary>
        /// The b 00101001.
        /// </summary>
        public const int B00101001 = 41;

        /// <summary>
        /// The b 00101010.
        /// </summary>
        public const int B00101010 = 42;

        /// <summary>
        /// The b 00101011.
        /// </summary>
        public const int B00101011 = 43;

        /// <summary>
        /// The b 00101100.
        /// </summary>
        public const int B00101100 = 44;

        /// <summary>
        /// The b 00101101.
        /// </summary>
        public const int B00101101 = 45;

        /// <summary>
        /// The b 00101110.
        /// </summary>
        public const int B00101110 = 46;

        /// <summary>
        /// The b 00101111.
        /// </summary>
        public const int B00101111 = 47;

        /// <summary>
        /// The b 00110000.
        /// </summary>
        public const int B00110000 = 48;

        /// <summary>
        /// The b 00110001.
        /// </summary>
        public const int B00110001 = 49;

        /// <summary>
        /// The b 00110010.
        /// </summary>
        public const int B00110010 = 50;

        /// <summary>
        /// The b 00110011.
        /// </summary>
        public const int B00110011 = 51;

        /// <summary>
        /// The b 00110100.
        /// </summary>
        public const int B00110100 = 52;

        /// <summary>
        /// The b 00110101.
        /// </summary>
        public const int B00110101 = 53;

        /// <summary>
        /// The b 00110110.
        /// </summary>
        public const int B00110110 = 54;

        /// <summary>
        /// The b 00110111.
        /// </summary>
        public const int B00110111 = 55;

        /// <summary>
        /// The b 00111000.
        /// </summary>
        public const int B00111000 = 56;

        /// <summary>
        /// The b 00111001.
        /// </summary>
        public const int B00111001 = 57;

        /// <summary>
        /// The b 00111010.
        /// </summary>
        public const int B00111010 = 58;

        /// <summary>
        /// The b 00111011.
        /// </summary>
        public const int B00111011 = 59;

        /// <summary>
        /// The b 00111100.
        /// </summary>
        public const int B00111100 = 60;

        /// <summary>
        /// The b 00111101.
        /// </summary>
        public const int B00111101 = 61;

        /// <summary>
        /// The b 00111110.
        /// </summary>
        public const int B00111110 = 62;

        /// <summary>
        /// The b 00111111.
        /// </summary>
        public const int B00111111 = 63;

        /// <summary>
        /// The b 01000000.
        /// </summary>
        public const int B01000000 = 64;

        /// <summary>
        /// The b 01000001.
        /// </summary>
        public const int B01000001 = 65;

        /// <summary>
        /// The b 01000010.
        /// </summary>
        public const int B01000010 = 66;

        /// <summary>
        /// The b 01000011.
        /// </summary>
        public const int B01000011 = 67;

        /// <summary>
        /// The b 01000100.
        /// </summary>
        public const int B01000100 = 68;

        /// <summary>
        /// The b 01000101.
        /// </summary>
        public const int B01000101 = 69;

        /// <summary>
        /// The b 01000110.
        /// </summary>
        public const int B01000110 = 70;

        /// <summary>
        /// The b 01000111.
        /// </summary>
        public const int B01000111 = 71;

        /// <summary>
        /// The b 01001000.
        /// </summary>
        public const int B01001000 = 72;

        /// <summary>
        /// The b 01001001.
        /// </summary>
        public const int B01001001 = 73;

        /// <summary>
        /// The b 01001010.
        /// </summary>
        public const int B01001010 = 74;

        /// <summary>
        /// The b 01001011.
        /// </summary>
        public const int B01001011 = 75;

        /// <summary>
        /// The b 01001100.
        /// </summary>
        public const int B01001100 = 76;

        /// <summary>
        /// The b 01001101.
        /// </summary>
        public const int B01001101 = 77;

        /// <summary>
        /// The b 01001110.
        /// </summary>
        public const int B01001110 = 78;

        /// <summary>
        /// The b 01001111.
        /// </summary>
        public const int B01001111 = 79;

        /// <summary>
        /// The b 01010000.
        /// </summary>
        public const int B01010000 = 80;

        /// <summary>
        /// The b 01010001.
        /// </summary>
        public const int B01010001 = 81;

        /// <summary>
        /// The b 01010010.
        /// </summary>
        public const int B01010010 = 82;

        /// <summary>
        /// The b 01010011.
        /// </summary>
        public const int B01010011 = 83;

        /// <summary>
        /// The b 01010100.
        /// </summary>
        public const int B01010100 = 84;

        /// <summary>
        /// The b 01010101.
        /// </summary>
        public const int B01010101 = 85;

        /// <summary>
        /// The b 01010110.
        /// </summary>
        public const int B01010110 = 86;

        /// <summary>
        /// The b 01010111.
        /// </summary>
        public const int B01010111 = 87;

        /// <summary>
        /// The b 01011000.
        /// </summary>
        public const int B01011000 = 88;

        /// <summary>
        /// The b 01011001.
        /// </summary>
        public const int B01011001 = 89;

        /// <summary>
        /// The b 01011010.
        /// </summary>
        public const int B01011010 = 90;

        /// <summary>
        /// The b 01011011.
        /// </summary>
        public const int B01011011 = 91;

        /// <summary>
        /// The b 01011100.
        /// </summary>
        public const int B01011100 = 92;

        /// <summary>
        /// The b 01011101.
        /// </summary>
        public const int B01011101 = 93;

        /// <summary>
        /// The b 01011110.
        /// </summary>
        public const int B01011110 = 94;

        /// <summary>
        /// The b 01011111.
        /// </summary>
        public const int B01011111 = 95;

        /// <summary>
        /// The b 01100000.
        /// </summary>
        public const int B01100000 = 96;

        /// <summary>
        /// The b 01100001.
        /// </summary>
        public const int B01100001 = 97;

        /// <summary>
        /// The b 01100010.
        /// </summary>
        public const int B01100010 = 98;

        /// <summary>
        /// The b 01100011.
        /// </summary>
        public const int B01100011 = 99;

        /// <summary>
        /// The b 01100100.
        /// </summary>
        public const int B01100100 = 100;

        /// <summary>
        /// The b 01100101.
        /// </summary>
        public const int B01100101 = 101;

        /// <summary>
        /// The b 01100110.
        /// </summary>
        public const int B01100110 = 102;

        /// <summary>
        /// The b 01100111.
        /// </summary>
        public const int B01100111 = 103;

        /// <summary>
        /// The b 01101000.
        /// </summary>
        public const int B01101000 = 104;

        /// <summary>
        /// The b 01101001.
        /// </summary>
        public const int B01101001 = 105;

        /// <summary>
        /// The b 01101010.
        /// </summary>
        public const int B01101010 = 106;

        /// <summary>
        /// The b 01101011.
        /// </summary>
        public const int B01101011 = 107;

        /// <summary>
        /// The b 01101100.
        /// </summary>
        public const int B01101100 = 108;

        /// <summary>
        /// The b 01101101.
        /// </summary>
        public const int B01101101 = 109;

        /// <summary>
        /// The b 01101110.
        /// </summary>
        public const int B01101110 = 110;

        /// <summary>
        /// The b 01101111.
        /// </summary>
        public const int B01101111 = 111;

        /// <summary>
        /// The b 01110000.
        /// </summary>
        public const int B01110000 = 112;

        /// <summary>
        /// The b 01110001.
        /// </summary>
        public const int B01110001 = 113;

        /// <summary>
        /// The b 01110010.
        /// </summary>
        public const int B01110010 = 114;

        /// <summary>
        /// The b 01110011.
        /// </summary>
        public const int B01110011 = 115;

        /// <summary>
        /// The b 01110100.
        /// </summary>
        public const int B01110100 = 116;

        /// <summary>
        /// The b 01110101.
        /// </summary>
        public const int B01110101 = 117;

        /// <summary>
        /// The b 01110110.
        /// </summary>
        public const int B01110110 = 118;

        /// <summary>
        /// The b 01110111.
        /// </summary>
        public const int B01110111 = 119;

        /// <summary>
        /// The b 01111000.
        /// </summary>
        public const int B01111000 = 120;

        /// <summary>
        /// The b 01111001.
        /// </summary>
        public const int B01111001 = 121;

        /// <summary>
        /// The b 01111010.
        /// </summary>
        public const int B01111010 = 122;

        /// <summary>
        /// The b 01111011.
        /// </summary>
        public const int B01111011 = 123;

        /// <summary>
        /// The b 01111100.
        /// </summary>
        public const int B01111100 = 124;

        /// <summary>
        /// The b 01111101.
        /// </summary>
        public const int B01111101 = 125;

        /// <summary>
        /// The b 01111110.
        /// </summary>
        public const int B01111110 = 126;

        /// <summary>
        /// The b 01111111.
        /// </summary>
        public const int B01111111 = 127;

        /// <summary>
        /// The b 10000000.
        /// </summary>
        public const int B10000000 = 128;

        /// <summary>
        /// The b 10000001.
        /// </summary>
        public const int B10000001 = 129;

        /// <summary>
        /// The b 10000010.
        /// </summary>
        public const int B10000010 = 130;

        /// <summary>
        /// The b 10000011.
        /// </summary>
        public const int B10000011 = 131;

        /// <summary>
        /// The b 10000100.
        /// </summary>
        public const int B10000100 = 132;

        /// <summary>
        /// The b 10000101.
        /// </summary>
        public const int B10000101 = 133;

        /// <summary>
        /// The b 10000110.
        /// </summary>
        public const int B10000110 = 134;

        /// <summary>
        /// The b 10000111.
        /// </summary>
        public const int B10000111 = 135;

        /// <summary>
        /// The b 10001000.
        /// </summary>
        public const int B10001000 = 136;

        /// <summary>
        /// The b 10001001.
        /// </summary>
        public const int B10001001 = 137;

        /// <summary>
        /// The b 10001010.
        /// </summary>
        public const int B10001010 = 138;

        /// <summary>
        /// The b 10001011.
        /// </summary>
        public const int B10001011 = 139;

        /// <summary>
        /// The b 10001100.
        /// </summary>
        public const int B10001100 = 140;

        /// <summary>
        /// The b 10001101.
        /// </summary>
        public const int B10001101 = 141;

        /// <summary>
        /// The b 10001110.
        /// </summary>
        public const int B10001110 = 142;

        /// <summary>
        /// The b 10001111.
        /// </summary>
        public const int B10001111 = 143;

        /// <summary>
        /// The b 10010000.
        /// </summary>
        public const int B10010000 = 144;

        /// <summary>
        /// The b 10010001.
        /// </summary>
        public const int B10010001 = 145;

        /// <summary>
        /// The b 10010010.
        /// </summary>
        public const int B10010010 = 146;

        /// <summary>
        /// The b 10010011.
        /// </summary>
        public const int B10010011 = 147;

        /// <summary>
        /// The b 10010100.
        /// </summary>
        public const int B10010100 = 148;

        /// <summary>
        /// The b 10010101.
        /// </summary>
        public const int B10010101 = 149;

        /// <summary>
        /// The b 10010110.
        /// </summary>
        public const int B10010110 = 150;

        /// <summary>
        /// The b 10010111.
        /// </summary>
        public const int B10010111 = 151;

        /// <summary>
        /// The b 10011000.
        /// </summary>
        public const int B10011000 = 152;

        /// <summary>
        /// The b 10011001.
        /// </summary>
        public const int B10011001 = 153;

        /// <summary>
        /// The b 10011010.
        /// </summary>
        public const int B10011010 = 154;

        /// <summary>
        /// The b 10011011.
        /// </summary>
        public const int B10011011 = 155;

        /// <summary>
        /// The b 10011100.
        /// </summary>
        public const int B10011100 = 156;

        /// <summary>
        /// The b 10011101.
        /// </summary>
        public const int B10011101 = 157;

        /// <summary>
        /// The b 10011110.
        /// </summary>
        public const int B10011110 = 158;

        /// <summary>
        /// The b 10011111.
        /// </summary>
        public const int B10011111 = 159;

        /// <summary>
        /// The b 10100000.
        /// </summary>
        public const int B10100000 = 160;

        /// <summary>
        /// The b 10100001.
        /// </summary>
        public const int B10100001 = 161;

        /// <summary>
        /// The b 10100010.
        /// </summary>
        public const int B10100010 = 162;

        /// <summary>
        /// The b 10100011.
        /// </summary>
        public const int B10100011 = 163;

        /// <summary>
        /// The b 10100100.
        /// </summary>
        public const int B10100100 = 164;

        /// <summary>
        /// The b 10100101.
        /// </summary>
        public const int B10100101 = 165;

        /// <summary>
        /// The b 10100110.
        /// </summary>
        public const int B10100110 = 166;

        /// <summary>
        /// The b 10100111.
        /// </summary>
        public const int B10100111 = 167;

        /// <summary>
        /// The b 10101000.
        /// </summary>
        public const int B10101000 = 168;

        /// <summary>
        /// The b 10101001.
        /// </summary>
        public const int B10101001 = 169;

        /// <summary>
        /// The b 10101010.
        /// </summary>
        public const int B10101010 = 170;

        /// <summary>
        /// The b 10101011.
        /// </summary>
        public const int B10101011 = 171;

        /// <summary>
        /// The b 10101100.
        /// </summary>
        public const int B10101100 = 172;

        /// <summary>
        /// The b 10101101.
        /// </summary>
        public const int B10101101 = 173;

        /// <summary>
        /// The b 10101110.
        /// </summary>
        public const int B10101110 = 174;

        /// <summary>
        /// The b 10101111.
        /// </summary>
        public const int B10101111 = 175;

        /// <summary>
        /// The b 10110000.
        /// </summary>
        public const int B10110000 = 176;

        /// <summary>
        /// The b 10110001.
        /// </summary>
        public const int B10110001 = 177;

        /// <summary>
        /// The b 10110010.
        /// </summary>
        public const int B10110010 = 178;

        /// <summary>
        /// The b 10110011.
        /// </summary>
        public const int B10110011 = 179;

        /// <summary>
        /// The b 10110100.
        /// </summary>
        public const int B10110100 = 180;

        /// <summary>
        /// The b 10110101.
        /// </summary>
        public const int B10110101 = 181;

        /// <summary>
        /// The b 10110110.
        /// </summary>
        public const int B10110110 = 182;

        /// <summary>
        /// The b 10110111.
        /// </summary>
        public const int B10110111 = 183;

        /// <summary>
        /// The b 10111000.
        /// </summary>
        public const int B10111000 = 184;

        /// <summary>
        /// The b 10111001.
        /// </summary>
        public const int B10111001 = 185;

        /// <summary>
        /// The b 10111010.
        /// </summary>
        public const int B10111010 = 186;

        /// <summary>
        /// The b 10111011.
        /// </summary>
        public const int B10111011 = 187;

        /// <summary>
        /// The b 10111100.
        /// </summary>
        public const int B10111100 = 188;

        /// <summary>
        /// The b 10111101.
        /// </summary>
        public const int B10111101 = 189;

        /// <summary>
        /// The b 10111110.
        /// </summary>
        public const int B10111110 = 190;

        /// <summary>
        /// The b 10111111.
        /// </summary>
        public const int B10111111 = 191;

        /// <summary>
        /// The b 11000000.
        /// </summary>
        public const int B11000000 = 192;

        /// <summary>
        /// The b 11000001.
        /// </summary>
        public const int B11000001 = 193;

        /// <summary>
        /// The b 11000010.
        /// </summary>
        public const int B11000010 = 194;

        /// <summary>
        /// The b 11000011.
        /// </summary>
        public const int B11000011 = 195;

        /// <summary>
        /// The b 11000100.
        /// </summary>
        public const int B11000100 = 196;

        /// <summary>
        /// The b 11000101.
        /// </summary>
        public const int B11000101 = 197;

        /// <summary>
        /// The b 11000110.
        /// </summary>
        public const int B11000110 = 198;

        /// <summary>
        /// The b 11000111.
        /// </summary>
        public const int B11000111 = 199;

        /// <summary>
        /// The b 11001000.
        /// </summary>
        public const int B11001000 = 200;

        /// <summary>
        /// The b 11001001.
        /// </summary>
        public const int B11001001 = 201;

        /// <summary>
        /// The b 11001010.
        /// </summary>
        public const int B11001010 = 202;

        /// <summary>
        /// The b 11001011.
        /// </summary>
        public const int B11001011 = 203;

        /// <summary>
        /// The b 11001100.
        /// </summary>
        public const int B11001100 = 204;

        /// <summary>
        /// The b 11001101.
        /// </summary>
        public const int B11001101 = 205;

        /// <summary>
        /// The b 11001110.
        /// </summary>
        public const int B11001110 = 206;

        /// <summary>
        /// The b 11001111.
        /// </summary>
        public const int B11001111 = 207;

        /// <summary>
        /// The b 11010000.
        /// </summary>
        public const int B11010000 = 208;

        /// <summary>
        /// The b 11010001.
        /// </summary>
        public const int B11010001 = 209;

        /// <summary>
        /// The b 11010010.
        /// </summary>
        public const int B11010010 = 210;

        /// <summary>
        /// The b 11010011.
        /// </summary>
        public const int B11010011 = 211;

        /// <summary>
        /// The b 11010100.
        /// </summary>
        public const int B11010100 = 212;

        /// <summary>
        /// The b 11010101.
        /// </summary>
        public const int B11010101 = 213;

        /// <summary>
        /// The b 11010110.
        /// </summary>
        public const int B11010110 = 214;

        /// <summary>
        /// The b 11010111.
        /// </summary>
        public const int B11010111 = 215;

        /// <summary>
        /// The b 11011000.
        /// </summary>
        public const int B11011000 = 216;

        /// <summary>
        /// The b 11011001.
        /// </summary>
        public const int B11011001 = 217;

        /// <summary>
        /// The b 11011010.
        /// </summary>
        public const int B11011010 = 218;

        /// <summary>
        /// The b 11011011.
        /// </summary>
        public const int B11011011 = 219;

        /// <summary>
        /// The b 11011100.
        /// </summary>
        public const int B11011100 = 220;

        /// <summary>
        /// The b 11011101.
        /// </summary>
        public const int B11011101 = 221;

        /// <summary>
        /// The b 11011110.
        /// </summary>
        public const int B11011110 = 222;

        /// <summary>
        /// The b 11011111.
        /// </summary>
        public const int B11011111 = 223;

        /// <summary>
        /// The b 11100000.
        /// </summary>
        public const int B11100000 = 224;

        /// <summary>
        /// The b 11100001.
        /// </summary>
        public const int B11100001 = 225;

        /// <summary>
        /// The b 11100010.
        /// </summary>
        public const int B11100010 = 226;

        /// <summary>
        /// The b 11100011.
        /// </summary>
        public const int B11100011 = 227;

        /// <summary>
        /// The b 11100100.
        /// </summary>
        public const int B11100100 = 228;

        /// <summary>
        /// The b 11100101.
        /// </summary>
        public const int B11100101 = 229;

        /// <summary>
        /// The b 11100110.
        /// </summary>
        public const int B11100110 = 230;

        /// <summary>
        /// The b 11100111.
        /// </summary>
        public const int B11100111 = 231;

        /// <summary>
        /// The b 11101000.
        /// </summary>
        public const int B11101000 = 232;

        /// <summary>
        /// The b 11101001.
        /// </summary>
        public const int B11101001 = 233;

        /// <summary>
        /// The b 11101010.
        /// </summary>
        public const int B11101010 = 234;

        /// <summary>
        /// The b 11101011.
        /// </summary>
        public const int B11101011 = 235;

        /// <summary>
        /// The b 11101100.
        /// </summary>
        public const int B11101100 = 236;

        /// <summary>
        /// The b 11101101.
        /// </summary>
        public const int B11101101 = 237;

        /// <summary>
        /// The b 11101110.
        /// </summary>
        public const int B11101110 = 238;

        /// <summary>
        /// The b 11101111.
        /// </summary>
        public const int B11101111 = 239;

        /// <summary>
        /// The b 11110000.
        /// </summary>
        public const int B11110000 = 240;

        /// <summary>
        /// The b 11110001.
        /// </summary>
        public const int B11110001 = 241;

        /// <summary>
        /// The b 11110010.
        /// </summary>
        public const int B11110010 = 242;

        /// <summary>
        /// The b 11110011.
        /// </summary>
        public const int B11110011 = 243;

        /// <summary>
        /// The b 11110100.
        /// </summary>
        public const int B11110100 = 244;

        /// <summary>
        /// The b 11110101.
        /// </summary>
        public const int B11110101 = 245;

        /// <summary>
        /// The b 11110110.
        /// </summary>
        public const int B11110110 = 246;

        /// <summary>
        /// The b 11110111.
        /// </summary>
        public const int B11110111 = 247;

        /// <summary>
        /// The b 11111000.
        /// </summary>
        public const int B11111000 = 248;

        /// <summary>
        /// The b 11111001.
        /// </summary>
        public const int B11111001 = 249;

        /// <summary>
        /// The b 11111010.
        /// </summary>
        public const int B11111010 = 250;

        /// <summary>
        /// The b 11111011.
        /// </summary>
        public const int B11111011 = 251;

        /// <summary>
        /// The b 11111100.
        /// </summary>
        public const int B11111100 = 252;

        /// <summary>
        /// The b 11111101.
        /// </summary>
        public const int B11111101 = 253;

        /// <summary>
        /// The b 11111110.
        /// </summary>
        public const int B11111110 = 254;

        /// <summary>
        /// The b 11111111.
        /// </summary>
        public const int B11111111 = 255;

        #endregion Binary constants
    }
}