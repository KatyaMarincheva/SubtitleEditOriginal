// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubtitleSegment.cs" company="">
//   
// </copyright>
// <summary>
//   The subtitle segment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    /// <summary>
    /// The subtitle segment.
    /// </summary>
    public class SubtitleSegment
    {
        /// <summary>
        /// The page composition segment.
        /// </summary>
        public const int PageCompositionSegment = 0x10;

        /// <summary>
        /// The region composition segment.
        /// </summary>
        public const int RegionCompositionSegment = 0x11;

        /// <summary>
        /// The clut definition segment.
        /// </summary>
        public const int ClutDefinitionSegment = 0x12;

        /// <summary>
        /// The object data segment.
        /// </summary>
        public const int ObjectDataSegment = 0x13;

        /// <summary>
        /// The display definition segment.
        /// </summary>
        public const int DisplayDefinitionSegment = 0x14;

        /// <summary>
        /// The end of display set segment.
        /// </summary>
        public const int EndOfDisplaySetSegment = 0x80;

        /// <summary>
        /// The clut definition.
        /// </summary>
        public ClutDefinitionSegment ClutDefinition;

        /// <summary>
        /// The display definition.
        /// </summary>
        public DisplayDefinitionSegment DisplayDefinition;

        /// <summary>
        /// The object data.
        /// </summary>
        public ObjectDataSegment ObjectData;

        /// <summary>
        /// The page composition.
        /// </summary>
        public PageCompositionSegment PageComposition;

        /// <summary>
        /// The region composition.
        /// </summary>
        public RegionCompositionSegment RegionComposition;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtitleSegment"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public SubtitleSegment(byte[] buffer, int index)
        {
            if (buffer == null || buffer.Length < 7)
            {
                return;
            }

            this.SyncByte = buffer[index];
            this.SegmentType = buffer[index + 1];
            this.PageId = Helper.GetEndianWord(buffer, index + 2);
            this.SegmentLength = Helper.GetEndianWord(buffer, index + 4);

            if (buffer.Length - 6 < this.SegmentLength)
            {
                return;
            }

            if (index + 6 + this.SegmentLength > buffer.Length)
            {
                return;
            }

            this.IsValid = true;

            switch (this.SegmentType)
            {
                case PageCompositionSegment:
                    this.PageComposition = new PageCompositionSegment(buffer, index + 6, this.SegmentLength - 2);
                    break;
                case RegionCompositionSegment:
                    this.RegionComposition = new RegionCompositionSegment(buffer, index + 6, this.SegmentLength - 10);
                    break;
                case ClutDefinitionSegment:
                    this.ClutDefinition = new ClutDefinitionSegment(buffer, index + 6, this.SegmentLength);
                    break;
                case ObjectDataSegment:
                    this.ObjectData = new ObjectDataSegment(buffer, index + 6);
                    break;
                case DisplayDefinitionSegment:
                    this.DisplayDefinition = new DisplayDefinitionSegment(buffer, index + 6);
                    break;
                case EndOfDisplaySetSegment:
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the sync byte.
        /// </summary>
        public int SyncByte { get; set; }

        /// <summary>
        /// Gets or sets the segment type.
        /// </summary>
        public int SegmentType { get; set; }

        /// <summary>
        /// Gets or sets the page id.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Gets or sets the segment length.
        /// </summary>
        public int SegmentLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets the segment type description.
        /// </summary>
        public string SegmentTypeDescription
        {
            get
            {
                switch (this.SegmentType)
                {
                    case PageCompositionSegment:
                        return "Page composition segment";
                    case RegionCompositionSegment:
                        return "Region composition segment";
                    case ClutDefinitionSegment:
                        return "CLUT definition segment";
                    case ObjectDataSegment:
                        return "Object data segment";
                    case DisplayDefinitionSegment:
                        return "Display definition segment";
                    case EndOfDisplaySetSegment:
                        return "End of display set segment";
                    default:
                        return "Unknown";
                }
            }
        }
    }
}