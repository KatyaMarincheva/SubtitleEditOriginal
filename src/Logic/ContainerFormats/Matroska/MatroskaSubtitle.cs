// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatroskaSubtitle.cs" company="">
//   
// </copyright>
// <summary>
//   The matroska subtitle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska
{
    using System;
    using System.Text;

    /// <summary>
    /// The matroska subtitle.
    /// </summary>
    internal class MatroskaSubtitle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatroskaSubtitle"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        public MatroskaSubtitle(byte[] data, long start, long duration)
        {
            this.Data = data;
            this.Start = start;
            this.Duration = duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatroskaSubtitle"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        public MatroskaSubtitle(byte[] data, long start)
            : this(data, start, 0)
        {
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets the end.
        /// </summary>
        public long End
        {
            get
            {
                return this.Start + this.Duration;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get
            {
                if (this.Data != null)
                {
                    return Encoding.UTF8.GetString(this.Data).Replace("\\N", Environment.NewLine);
                }

                return string.Empty;
            }
        }
    }
}