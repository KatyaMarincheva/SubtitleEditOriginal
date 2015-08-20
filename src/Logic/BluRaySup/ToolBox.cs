// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ToolBox.cs">
//   
// </copyright>
// <summary>
//   The tool box.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.BluRaySup
{
    using System.Text;

    /// <summary>
    /// The tool box.
    /// </summary>
    public static class ToolBox
    {
        /// <summary>
        /// Convert bytes to a C-style hex string with leading zeroes
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToHex(byte[] buffer, int index, int digits)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = index; i < index + digits; i++)
            {
                string s = string.Format("{0:X}", buffer[i]);
                if (s.Length < 2)
                {
                    sb.Append('0');
                }

                sb.Append(s);
            }

            return "0x" + sb;
        }

        /// <summary>
        /// Convert a long integer to a C-style hex string with leading zeroes
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToHex(int number, int digits)
        {
            string s = string.Format("{0:X}", number);
            if (s.Length < digits)
            {
                s = s.PadLeft(digits, '0');
            }

            return "0x" + s;
        }

        /**
         * Convert time in milliseconds to array containing hours, minutes, seconds and milliseconds
         * @param ms Time in milliseconds
         * @return Array containing hours, minutes, seconds and milliseconds (in this order)
         */

        /// <summary>
        /// The milliseconds to time.
        /// </summary>
        /// <param name="ms">
        /// The ms.
        /// </param>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public static int[] MillisecondsToTime(long ms)
        {
            int[] time = new int[4];

            // time[0] = hours
            time[0] = (int)(ms / (60 * 60 * 1000));
            ms -= time[0] * 60 * 60 * 1000;

            // time[1] = minutes
            time[1] = (int)(ms / (60 * 1000));
            ms -= time[1] * 60 * 1000;

            // time[2] = seconds
            time[2] = (int)(ms / 1000);
            ms -= time[2] * 1000;
            time[3] = (int)ms;
            return time;
        }

        /// <summary>
        /// Convert time in 90kHz ticks to string hh:mm:ss.ms
        /// </summary>
        /// <param name="pts">
        /// Time in 90kHz resolution
        /// </param>
        /// <returns>
        /// String in format hh:mm:ss:ms
        /// </returns>
        public static string PtsToTimeString(long pts)
        {
            int[] time = MillisecondsToTime((pts + 45) / 90);
            return string.Format(@"{0:D2}:{1:D2}:{2:D2}.{3:D3}", time[0], time[1], time[2], time[3]);
        }

        /// <summary>
        /// Write (big endian) double word to buffer[index] (index points at most significant byte)
        /// </summary>
        /// <param name="buffer">
        /// Byte array
        /// </param>
        /// <param name="index">
        /// Index to write to
        /// </param>
        /// <param name="val">
        /// Integer value of double word to write
        /// </param>
        public static void SetDWord(byte[] buffer, int index, int val)
        {
            buffer[index] = (byte)(val >> 24);
            buffer[index + 1] = (byte)(val >> 16);
            buffer[index + 2] = (byte)(val >> 8);
            buffer[index + 3] = (byte)val;
        }

        /// <summary>
        /// Write (big endian) word to buffer[index] (index points at most significant byte)
        /// </summary>
        /// <param name="buffer">
        /// Byte array
        /// </param>
        /// <param name="index">
        /// index Index to write to
        /// </param>
        /// <param name="val">
        /// val Integer value of word to write
        /// </param>
        public static void SetWord(byte[] buffer, int index, int val)
        {
            buffer[index] = (byte)(val >> 8);
            buffer[index + 1] = (byte)val;
        }

        /// <summary>
        /// Write byte to buffer[index]
        /// </summary>
        /// <param name="buffer">
        /// Byte array
        /// </param>
        /// <param name="index">
        /// Index to write to
        /// </param>
        /// <param name="val">
        /// Integer value of byte to write
        /// </param>
        public static void SetByte(byte[] buffer, int index, int val)
        {
            buffer[index] = (byte)val;
        }
    }
}