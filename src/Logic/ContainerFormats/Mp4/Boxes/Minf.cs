// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Minf.cs" company="">
//   
// </copyright>
// <summary>
//   The minf.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    /// <summary>
    /// The minf.
    /// </summary>
    public class Minf : Box
    {
        /// <summary>
        /// The stbl.
        /// </summary>
        public Stbl Stbl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Minf"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="maximumLength">
        /// The maximum length.
        /// </param>
        /// <param name="timeScale">
        /// The time scale.
        /// </param>
        /// <param name="handlerType">
        /// The handler type.
        /// </param>
        /// <param name="mdia">
        /// The mdia.
        /// </param>
        public Minf(FileStream fs, ulong maximumLength, uint timeScale, string handlerType, Mdia mdia)
        {
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "stbl")
                {
                    this.Stbl = new Stbl(fs, this.Position, timeScale, handlerType, mdia);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }
    }
}