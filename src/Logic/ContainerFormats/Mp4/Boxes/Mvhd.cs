// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mvhd.cs" company="">
//   
// </copyright>
// <summary>
//   The mvhd.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    /// <summary>
    /// The mvhd.
    /// </summary>
    public class Mvhd : Box
    {
        /// <summary>
        /// The creation time.
        /// </summary>
        public readonly uint CreationTime;

        /// <summary>
        /// The duration.
        /// </summary>
        public readonly uint Duration;

        /// <summary>
        /// The modification time.
        /// </summary>
        public readonly uint ModificationTime;

        /// <summary>
        /// The time scale.
        /// </summary>
        public readonly uint TimeScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mvhd"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        public Mvhd(FileStream fs)
        {
            this.Buffer = new byte[20];
            int bytesRead = fs.Read(this.Buffer, 0, this.Buffer.Length);
            if (bytesRead < this.Buffer.Length)
            {
                return;
            }

            this.CreationTime = this.GetUInt(4);
            this.ModificationTime = this.GetUInt(8);
            this.TimeScale = this.GetUInt(12);
            this.Duration = this.GetUInt(16);
        }
    }
}