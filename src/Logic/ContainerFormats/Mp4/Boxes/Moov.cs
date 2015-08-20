// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Moov.cs" company="">
//   
// </copyright>
// <summary>
//   The moov.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The moov.
    /// </summary>
    public class Moov : Box
    {
        /// <summary>
        /// The mvhd.
        /// </summary>
        public Mvhd Mvhd;

        /// <summary>
        /// The tracks.
        /// </summary>
        public List<Trak> Tracks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Moov"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="maximumLength">
        /// The maximum length.
        /// </param>
        public Moov(FileStream fs, ulong maximumLength)
        {
            this.Tracks = new List<Trak>();
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "trak")
                {
                    this.Tracks.Add(new Trak(fs, this.Position));
                }
                else if (this.Name == "mvhd")
                {
                    this.Mvhd = new Mvhd(fs);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }
    }
}