// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdxParagraph.cs" company="">
//   
// </copyright>
// <summary>
//   The idx paragraph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;

    /// <summary>
    /// The idx paragraph.
    /// </summary>
    public class IdxParagraph
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdxParagraph"/> class.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="filePosition">
        /// The file position.
        /// </param>
        public IdxParagraph(TimeSpan startTime, long filePosition)
        {
            this.StartTime = startTime;
            this.FilePosition = filePosition;
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public TimeSpan StartTime { get; private set; }

        /// <summary>
        /// Gets the file position.
        /// </summary>
        public long FilePosition { get; private set; }
    }
}