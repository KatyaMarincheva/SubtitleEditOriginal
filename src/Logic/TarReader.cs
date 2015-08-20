// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TarReader.cs" company="">
//   
// </copyright>
// <summary>
//   The tar reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The tar reader.
    /// </summary>
    public class TarReader : IDisposable
    {
        /// <summary>
        /// The _stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="TarReader"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public TarReader(string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            this.OpenTarFile(fs);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TarReader"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public TarReader(Stream stream)
        {
            this.OpenTarFile(stream);
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        public List<TarHeader> Files { get; private set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this._stream != null)
            {
                this._stream.Dispose();
                this._stream = null;
            }
        }

        /// <summary>
        /// The open tar file.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        private void OpenTarFile(Stream stream)
        {
            this._stream = stream;
            this.Files = new List<TarHeader>();
            long length = stream.Length;
            long pos = 0;
            stream.Position = 0;
            while (pos + 512 < length)
            {
                stream.Seek(pos, SeekOrigin.Begin);
                var th = new TarHeader(stream);
                if (th.FileSizeInBytes > 0)
                {
                    this.Files.Add(th);
                }

                pos += TarHeader.HeaderSize + th.FileSizeInBytes;
                if (pos % TarHeader.HeaderSize > 0)
                {
                    pos += 512 - (pos % TarHeader.HeaderSize);
                }
            }
        }

        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            this._stream.Close();
        }
    }
}