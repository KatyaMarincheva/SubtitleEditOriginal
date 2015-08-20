// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionCompositionSegmentObject.cs" company="">
//   
// </copyright>
// <summary>
//   The region composition segment object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    /// <summary>
    /// The region composition segment object.
    /// </summary>
    public class RegionCompositionSegmentObject
    {
        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public int ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the object provider flag.
        /// </summary>
        public int ObjectProviderFlag { get; set; }

        /// <summary>
        /// Gets or sets the object horizontal position.
        /// </summary>
        public int ObjectHorizontalPosition { get; set; }

        /// <summary>
        /// Gets or sets the object vertical position.
        /// </summary>
        public int ObjectVerticalPosition { get; set; }

        /// <summary>
        /// Gets or sets the object foreground pixel code.
        /// </summary>
        public int? ObjectForegroundPixelCode { get; set; }

        /// <summary>
        /// Gets or sets the object background pixel code.
        /// </summary>
        public int? ObjectBackgroundPixelCode { get; set; }
    }
}