// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementId.cs" company="">
//   
// </copyright>
// <summary>
//   The element id.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Ebml
{
    /// <summary>
    /// The element id.
    /// </summary>
    internal enum ElementId
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0, 

        /// <summary>
        /// The ebml.
        /// </summary>
        Ebml = 0x1A45DFA3, 

        /// <summary>
        /// The segment.
        /// </summary>
        Segment = 0x18538067, 

        /// <summary>
        /// The info.
        /// </summary>
        Info = 0x1549A966, 

        /// <summary>
        /// The timecode scale.
        /// </summary>
        TimecodeScale = 0x2AD7B1, 

        /// <summary>
        /// The duration.
        /// </summary>
        Duration = 0x4489, 

        /// <summary>
        /// The tracks.
        /// </summary>
        Tracks = 0x1654AE6B, 

        /// <summary>
        /// The track entry.
        /// </summary>
        TrackEntry = 0xAE, 

        /// <summary>
        /// The track number.
        /// </summary>
        TrackNumber = 0xD7, 

        /// <summary>
        /// The track type.
        /// </summary>
        TrackType = 0x83, 

        /// <summary>
        /// The default duration.
        /// </summary>
        DefaultDuration = 0x23E383, 

        /// <summary>
        /// The name.
        /// </summary>
        Name = 0x536E, 

        /// <summary>
        /// The language.
        /// </summary>
        Language = 0x22B59C, 

        /// <summary>
        /// The codec id.
        /// </summary>
        CodecId = 0x86, 

        /// <summary>
        /// The codec private.
        /// </summary>
        CodecPrivate = 0x63A2, 

        /// <summary>
        /// The video.
        /// </summary>
        Video = 0xE0, 

        /// <summary>
        /// The pixel width.
        /// </summary>
        PixelWidth = 0xB0, 

        /// <summary>
        /// The pixel height.
        /// </summary>
        PixelHeight = 0xBA, 

        /// <summary>
        /// The audio.
        /// </summary>
        Audio = 0xE1, 

        /// <summary>
        /// The content encodings.
        /// </summary>
        ContentEncodings = 0x6D80, 

        /// <summary>
        /// The content encoding.
        /// </summary>
        ContentEncoding = 0x6240, 

        /// <summary>
        /// The content encoding order.
        /// </summary>
        ContentEncodingOrder = 0x5031, 

        /// <summary>
        /// The content encoding scope.
        /// </summary>
        ContentEncodingScope = 0x5032, 

        /// <summary>
        /// The content encoding type.
        /// </summary>
        ContentEncodingType = 0x5033, 

        /// <summary>
        /// The content compression.
        /// </summary>
        ContentCompression = 0x5034, 

        /// <summary>
        /// The content comp algo.
        /// </summary>
        ContentCompAlgo = 0x4254, 

        /// <summary>
        /// The content comp settings.
        /// </summary>
        ContentCompSettings = 0x4255, 

        /// <summary>
        /// The cluster.
        /// </summary>
        Cluster = 0x1F43B675, 

        /// <summary>
        /// The timecode.
        /// </summary>
        Timecode = 0xE7, 

        /// <summary>
        /// The simple block.
        /// </summary>
        SimpleBlock = 0xA3, 

        /// <summary>
        /// The block group.
        /// </summary>
        BlockGroup = 0xA0, 

        /// <summary>
        /// The block.
        /// </summary>
        Block = 0xA1, 

        /// <summary>
        /// The block duration.
        /// </summary>
        BlockDuration = 0x9B
    }
}