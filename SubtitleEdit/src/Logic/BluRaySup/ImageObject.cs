namespace Nikse.SubtitleEdit.Logic.BluRaySup
{
    using System.Collections.Generic;

    public class ImageObject
    {
        /// <summary>
        ///     list of ODS packets containing image info
        /// </summary>
        public List<ImageObjectFragment> Fragments;

        /// <summary>
        ///     palette identifier
        /// </summary>
        public int PaletteId { get; set; }

        /// <summary>
        ///     overall size of RLE buffer (might be spread over several packages)
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        ///     with of subtitle image
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     height of subtitle image
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     upper left corner of subtitle x
        /// </summary>
        public int XOffset { get; set; }

        /// <summary>
        ///     upper left corner of subtitle y
        /// </summary>
        public int YOffset { get; set; }
    }
}