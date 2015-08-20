// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageCompositionSegmentRegion.cs" company="">
//   
// </copyright>
// <summary>
//   The page composition segment region.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    /// <summary>
    /// The page composition segment region.
    /// </summary>
    public class PageCompositionSegmentRegion
    {
        /// <summary>
        /// Gets or sets the region id.
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the region horizontal address.
        /// </summary>
        public int RegionHorizontalAddress { get; set; }

        /// <summary>
        /// Gets or sets the region vertical address.
        /// </summary>
        public int RegionVerticalAddress { get; set; }
    }
}