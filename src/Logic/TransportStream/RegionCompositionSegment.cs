// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionCompositionSegment.cs" company="">
//   
// </copyright>
// <summary>
//   The region composition segment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System.Collections.Generic;

    /// <summary>
    /// The region composition segment.
    /// </summary>
    public class RegionCompositionSegment
    {
        /// <summary>
        /// The objects.
        /// </summary>
        public List<RegionCompositionSegmentObject> Objects = new List<RegionCompositionSegmentObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionCompositionSegment"/> class.
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
        public RegionCompositionSegment(byte[] buffer, int index, int regionLength)
        {
            this.RegionId = buffer[index];
            this.RegionVersionNumber = buffer[index + 1] >> 4;
            this.RegionFillFlag = (buffer[index + 1] & Helper.B00001000) > 0;
            this.RegionWidth = Helper.GetEndianWord(buffer, index + 2);
            this.RegionHeight = Helper.GetEndianWord(buffer, index + 4);
            this.RegionLevelOfCompatibility = buffer[index + 6] >> 5;
            this.RegionDepth = (buffer[index + 6] & Helper.B00011100) >> 2;
            this.RegionClutId = buffer[index + 7];
            this.Region8BitPixelCode = buffer[index + 8];
            this.Region4BitPixelCode = buffer[index + 9] >> 4;
            this.Region2BitPixelCode = (buffer[index + 9] & Helper.B00001100) >> 2;
            int i = 0;
            while (i < regionLength && i + index < buffer.Length)
            {
                var rcso = new RegionCompositionSegmentObject();
                rcso.ObjectId = Helper.GetEndianWord(buffer, i + index + 10);
                i += 2;
                rcso.ObjectType = buffer[i + index + 10] >> 6;
                rcso.ObjectProviderFlag = (buffer[index + i + 10] & Helper.B00110000) >> 4;
                rcso.ObjectHorizontalPosition = ((buffer[index + i + 10] & Helper.B00001111) << 8) + buffer[index + i + 11];
                i += 2;
                rcso.ObjectVerticalPosition = ((buffer[index + i + 10] & Helper.B00001111) << 8) + buffer[index + i + 11];
                i += 2;
                if (rcso.ObjectType == 1 || rcso.ObjectType == 2)
                {
                    i += 2;
                }

                this.Objects.Add(rcso);
            }
        }

        /// <summary>
        /// Gets or sets the region id.
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the region version number.
        /// </summary>
        public int RegionVersionNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether region fill flag.
        /// </summary>
        public bool RegionFillFlag { get; set; }

        /// <summary>
        /// Gets or sets the region width.
        /// </summary>
        public int RegionWidth { get; set; }

        /// <summary>
        /// Gets or sets the region height.
        /// </summary>
        public int RegionHeight { get; set; }

        /// <summary>
        /// Gets or sets the region level of compatibility.
        /// </summary>
        public int RegionLevelOfCompatibility { get; set; }

        /// <summary>
        /// Gets or sets the region depth.
        /// </summary>
        public int RegionDepth { get; set; }

        /// <summary>
        /// Gets or sets the region clut id.
        /// </summary>
        public int RegionClutId { get; set; }

        /// <summary>
        /// Gets or sets the region 8 bit pixel code.
        /// </summary>
        public int Region8BitPixelCode { get; set; }

        /// <summary>
        /// Gets or sets the region 4 bit pixel code.
        /// </summary>
        public int Region4BitPixelCode { get; set; }

        /// <summary>
        /// Gets or sets the region 2 bit pixel code.
        /// </summary>
        public int Region2BitPixelCode { get; set; }
    }
}