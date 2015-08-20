// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageCompositionSegemnt.cs" company="">
//   
// </copyright>
// <summary>
//   The page composition segment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System.Collections.Generic;

    /// <summary>
    /// The page composition segment.
    /// </summary>
    public class PageCompositionSegment
    {
        /// <summary>
        /// The regions.
        /// </summary>
        public List<PageCompositionSegmentRegion> Regions = new List<PageCompositionSegmentRegion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PageCompositionSegment"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="regionLength">
        /// The region length.
        /// </param>
        public PageCompositionSegment(byte[] buffer, int index, int regionLength)
        {
            this.PageTimeOut = buffer[index];
            this.PageVersionNumber = buffer[index + 1] >> 4;
            this.PageState = (buffer[index + 1] & Helper.B00001100) >> 2;
            this.Regions = new List<PageCompositionSegmentRegion>();
            int i = 0;
            while (i < regionLength && i + index < buffer.Length)
            {
                var rcsr = new PageCompositionSegmentRegion();
                rcsr.RegionId = buffer[i + index + 2];
                i += 2;
                rcsr.RegionHorizontalAddress = Helper.GetEndianWord(buffer, i + index + 2);
                i += 2;
                rcsr.RegionVerticalAddress = Helper.GetEndianWord(buffer, i + index + 2);
                i += 2;
                this.Regions.Add(rcsr);
            }
        }

        /// <summary>
        /// Gets or sets the page time out.
        /// </summary>
        public int PageTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the page version number.
        /// </summary>
        public int PageVersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the page state.
        /// </summary>
        public int PageState { get; set; }
    }
}