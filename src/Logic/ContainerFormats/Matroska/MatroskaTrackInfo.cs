// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatroskaTrackInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The matroska track info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska
{
    /// <summary>
    /// The matroska track info.
    /// </summary>
    internal class MatroskaTrackInfo
    {
        /// <summary>
        /// Gets or sets the track number.
        /// </summary>
        public int TrackNumber { get; set; }

        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is video.
        /// </summary>
        public bool IsVideo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is audio.
        /// </summary>
        public bool IsAudio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is subtitle.
        /// </summary>
        public bool IsSubtitle { get; set; }

        /// <summary>
        /// Gets or sets the codec id.
        /// </summary>
        public string CodecId { get; set; }

        /// <summary>
        /// Gets or sets the codec private.
        /// </summary>
        public string CodecPrivate { get; set; }

        /// <summary>
        /// Gets or sets the default duration.
        /// </summary>
        public int DefaultDuration { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content compression algorithm.
        /// </summary>
        public int ContentCompressionAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the content encoding type.
        /// </summary>
        public int ContentEncodingType { get; set; }
    }
}