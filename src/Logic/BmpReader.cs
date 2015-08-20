// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BmpReader.cs" company="">
//   
// </copyright>
// <summary>
//   The bmp reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;

    /// <summary>
    /// The bmp reader.
    /// </summary>
    public class BmpReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BmpReader"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public BmpReader(string fileName)
        {
            byte[] buffer = System.IO.File.ReadAllBytes(fileName);
            this.HeaderId = System.Text.Encoding.UTF8.GetString(buffer, 0, 2);
            this.HeaderFileSize = BitConverter.ToUInt32(buffer, 2);
            this.OffsetToPixelArray = BitConverter.ToUInt32(buffer, 0xa);
        }

        /// <summary>
        /// Gets the header id.
        /// </summary>
        public string HeaderId { get; private set; }

        /// <summary>
        /// Gets the header file size.
        /// </summary>
        public uint HeaderFileSize { get; private set; }

        /// <summary>
        /// Gets the offset to pixel array.
        /// </summary>
        public uint OffsetToPixelArray { get; private set; }
    }
}