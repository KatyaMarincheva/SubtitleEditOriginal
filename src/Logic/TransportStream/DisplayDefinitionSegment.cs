// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayDefinitionSegment.cs" company="">
//   
// </copyright>
// <summary>
//   The display definition segment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    /// <summary>
    /// The display definition segment.
    /// </summary>
    public class DisplayDefinitionSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayDefinitionSegment"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public DisplayDefinitionSegment(byte[] buffer, int index)
        {
            this.DisplayDefinitionVersionNumber = buffer[index] >> 4;
            this.DisplayWindowFlag = (buffer[index] & Helper.B00001000) > 0;
            this.DisplayWith = Helper.GetEndianWord(buffer, index + 1);
            this.DisplayHeight = Helper.GetEndianWord(buffer, index + 3);
            if (this.DisplayWindowFlag)
            {
                this.DisplayWindowHorizontalPositionMinimum = Helper.GetEndianWord(buffer, index + 5);
                this.DisplayWindowHorizontalPositionMaximum = Helper.GetEndianWord(buffer, index + 7);
                this.DisplayWindowVerticalPositionMinimum = Helper.GetEndianWord(buffer, index + 9);
                this.DisplayWindowVerticalPositionMaximum = Helper.GetEndianWord(buffer, index + 11);
            }
        }

        /// <summary>
        /// Gets or sets the display definition version number.
        /// </summary>
        public int DisplayDefinitionVersionNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether display window flag.
        /// </summary>
        public bool DisplayWindowFlag { get; set; }

        /// <summary>
        /// Gets or sets the display with.
        /// </summary>
        public int DisplayWith { get; set; }

        /// <summary>
        /// Gets or sets the display height.
        /// </summary>
        public int DisplayHeight { get; set; }

        /// <summary>
        /// Gets or sets the display window horizontal position minimum.
        /// </summary>
        public int? DisplayWindowHorizontalPositionMinimum { get; set; }

        /// <summary>
        /// Gets or sets the display window horizontal position maximum.
        /// </summary>
        public int? DisplayWindowHorizontalPositionMaximum { get; set; }

        /// <summary>
        /// Gets or sets the display window vertical position minimum.
        /// </summary>
        public int? DisplayWindowVerticalPositionMinimum { get; set; }

        /// <summary>
        /// Gets or sets the display window vertical position maximum.
        /// </summary>
        public int? DisplayWindowVerticalPositionMaximum { get; set; }
    }
}