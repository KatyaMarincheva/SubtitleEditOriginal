// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trak.cs" company="">
//   
// </copyright>
// <summary>
//   The trak.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    /// <summary>
    /// The trak.
    /// </summary>
    public class Trak : Box
    {
        /// <summary>
        /// The mdia.
        /// </summary>
        public Mdia Mdia;

        /// <summary>
        /// The tkhd.
        /// </summary>
        public Tkhd Tkhd;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trak"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="maximumLength">
        /// The maximum length.
        /// </param>
        public Trak(FileStream fs, ulong maximumLength)
        {
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "mdia")
                {
                    this.Mdia = new Mdia(fs, this.Position);
                }
                else if (this.Name == "tkhd")
                {
                    this.Tkhd = new Tkhd(fs);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }
    }
}