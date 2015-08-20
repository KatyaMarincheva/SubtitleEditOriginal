// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tkhd.cs" company="">
//   
// </copyright>
// <summary>
//   The tkhd.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    /// <summary>
    /// The tkhd.
    /// </summary>
    public class Tkhd : Box
    {
        /// <summary>
        /// The duration.
        /// </summary>
        public readonly ulong Duration;

        /// <summary>
        /// The height.
        /// </summary>
        public readonly uint Height;

        /// <summary>
        /// The track id.
        /// </summary>
        public readonly uint TrackId;

        /// <summary>
        /// The width.
        /// </summary>
        public readonly uint Width;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tkhd"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        public Tkhd(FileStream fs)
        {
            this.Buffer = new byte[84];
            int bytesRead = fs.Read(this.Buffer, 0, this.Buffer.Length);
            if (bytesRead < this.Buffer.Length)
            {
                return;
            }

            int version = this.Buffer[0];
            int addToIndex64Bit = 0;
            if (version == 1)
            {
                addToIndex64Bit = 8;
            }

            this.TrackId = this.GetUInt(12 + addToIndex64Bit);
            if (version == 1)
            {
                this.Duration = this.GetUInt64(20 + addToIndex64Bit);
                addToIndex64Bit += 4;
            }
            else
            {
                this.Duration = this.GetUInt(20 + addToIndex64Bit);
            }

            this.Width = (uint)this.GetWord(76 + addToIndex64Bit); // skip decimals
            this.Height = (uint)this.GetWord(80 + addToIndex64Bit); // skip decimals

            // System.Windows.Forms.MessageBox.Show("Width: " + GetWord(76 + addToIndex64Bit).ToString() + "." + GetWord(78 + addToIndex64Bit).ToString());
            // System.Windows.Forms.MessageBox.Show("Height: " + GetWord(80 + addToIndex64Bit).ToString() + "." + GetWord(82 + addToIndex64Bit).ToString());
        }
    }
}