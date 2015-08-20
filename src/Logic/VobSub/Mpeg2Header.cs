// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mpeg2Header.cs" company="">
//   
// </copyright>
// <summary>
//   http://www.mpucoder.com/DVD/packhdr.html
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;

    /// <summary>
    /// http://www.mpucoder.com/DVD/packhdr.html
    /// </summary>
    public class Mpeg2Header
    {
        /// <summary>
        /// The length.
        /// </summary>
        public const int Length = 14;

        /// <summary>
        /// The pack indentifier.
        /// </summary>
        public readonly byte PackIndentifier;

        /// <summary>
        /// The pack stuffing length.
        /// </summary>
        public readonly int PackStuffingLength;

        // public readonly UInt64 SystemClockReferenceQuotient;
        // public readonly UInt64 SystemClockReferenceRemainder;
        /// <summary>
        /// The program mux rate.
        /// </summary>
        public readonly ulong ProgramMuxRate;

        /// <summary>
        /// The start code.
        /// </summary>
        public readonly uint StartCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mpeg2Header"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public Mpeg2Header(byte[] buffer)
        {
            this.StartCode = Helper.GetEndian(buffer, 0, 3);
            this.PackIndentifier = buffer[3];

            // string b4To9AsBinary = Helper.GetBinaryString(buffer, 4, 6);
            // b4To9AsBinary = b4To9AsBinary.Substring(2,3) + b4To9AsBinary.Substring(6,15) + b4To9AsBinary.Substring(22,15);
            // SystemClockReferenceQuotient = Helper.GetUInt32FromBinaryString(b4To9AsBinary);

            // SystemClockReferenceRemainder = (ulong)(((buffer[8] & Helper.B00000011) << 8) + buffer[9])
            this.ProgramMuxRate = Helper.GetEndian(buffer, 10, 3) >> 2;

            this.PackStuffingLength = buffer[13] & Helper.B00000111;
        }
    }
}