// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The video info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    /// <summary>
    /// The video info.
    /// </summary>
    public class VideoInfo
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the total milliseconds.
        /// </summary>
        public double TotalMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the total seconds.
        /// </summary>
        public double TotalSeconds { get; set; }

        /// <summary>
        /// Gets or sets the frames per second.
        /// </summary>
        public double FramesPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the total frames.
        /// </summary>
        public double TotalFrames { get; set; }

        /// <summary>
        /// Gets or sets the video codec.
        /// </summary>
        public string VideoCodec { get; set; }

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether success.
        /// </summary>
        public bool Success { get; set; }
    }
}