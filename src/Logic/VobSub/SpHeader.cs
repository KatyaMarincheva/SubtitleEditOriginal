// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpHeader.cs" company="">
//   
// </copyright>
// <summary>
//   The sp header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;

    /// <summary>
    /// The sp header.
    /// </summary>
    public class SpHeader
    {
        /// <summary>
        /// The sp header length.
        /// </summary>
        public const int SpHeaderLength = 14;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpHeader"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public SpHeader(byte[] buffer)
        {
            this.Identifier = System.Text.Encoding.ASCII.GetString(buffer, 0, 2);
            int startMilliseconds = (int)Helper.GetLittleEndian32(buffer, 2) / 90;
            this.StartTime = TimeSpan.FromMilliseconds(startMilliseconds);
            this.NextBlockPosition = Helper.GetEndianWord(buffer, 10) - 4;
            this.ControlSequencePosition = Helper.GetEndianWord(buffer, 12) - 4;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public TimeSpan StartTime { get; private set; }

        /// <summary>
        /// Gets the next block position.
        /// </summary>
        public int NextBlockPosition { get; private set; }

        /// <summary>
        /// Gets the control sequence position.
        /// </summary>
        public int ControlSequencePosition { get; private set; }

        /// <summary>
        /// Gets the picture.
        /// </summary>
        public SubPicture Picture { get; private set; }

        /// <summary>
        /// The add picture.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="SubPicture"/>.
        /// </returns>
        public SubPicture AddPicture(byte[] buffer)
        {
            this.Picture = new SubPicture(buffer, this.ControlSequencePosition, -4);
            return this.Picture;
        }
    }
}