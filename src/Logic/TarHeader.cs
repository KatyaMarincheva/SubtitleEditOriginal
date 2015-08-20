// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TarHeader.cs" company="">
//   
// </copyright>
// <summary>
//   The tar header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The tar header.
    /// </summary>
    public class TarHeader
    {
        /// <summary>
        /// The header size.
        /// </summary>
        public const int HeaderSize = 512;

        /// <summary>
        /// The _stream.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="TarHeader"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public TarHeader(Stream stream)
        {
            this._stream = stream;
            var buffer = new byte[HeaderSize];
            stream.Read(buffer, 0, HeaderSize);
            this.FilePosition = stream.Position;

            this.FileName = Encoding.ASCII.GetString(buffer, 0, 100).Replace("\0", string.Empty);

            string sizeInBytes = Encoding.ASCII.GetString(buffer, 124, 11);
            if (!string.IsNullOrEmpty(this.FileName) && Utilities.IsInteger(sizeInBytes))
            {
                this.FileSizeInBytes = Convert.ToInt64(sizeInBytes.Trim(), 8);
            }
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long FileSizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the file position.
        /// </summary>
        public long FilePosition { get; set; }

        /// <summary>
        /// The write data.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void WriteData(string fileName)
        {
            var buffer = new byte[this.FileSizeInBytes];
            this._stream.Position = this.FilePosition;
            this._stream.Read(buffer, 0, buffer.Length);
            File.WriteAllBytes(fileName, buffer);
        }
    }
}