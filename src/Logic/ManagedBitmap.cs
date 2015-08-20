// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedBitmap.cs" company="">
//   
// </copyright>
// <summary>
//   The managed bitmap.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// The managed bitmap.
    /// </summary>
    public class ManagedBitmap
    {
        /// <summary>
        /// The _colors.
        /// </summary>
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedBitmap"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public ManagedBitmap(string fileName)
        {
            try
            {
                byte[] buffer = new byte[1024];
                MemoryStream fd = new MemoryStream();
                Stream fs = File.OpenRead(fileName);
                using (Stream csStream = new GZipStream(fs, CompressionMode.Decompress))
                {
                    int nRead;
                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fd.Write(buffer, 0, nRead);
                    }

                    csStream.Flush();
                    buffer = fd.ToArray();
                }

                this.Width = buffer[4] << 8 | buffer[5];
                this.Height = buffer[6] << 8 | buffer[7];
                this._colors = new Color[this.Width * this.Height];
                int start = 8;
                for (int i = 0; i < this._colors.Length; i++)
                {
                    this._colors[i] = Color.FromArgb(buffer[start], buffer[start + 1], buffer[start + 2], buffer[start + 3]);
                    start += 4;
                }
            }
            catch
            {
                this.LoadedOk = false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedBitmap"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public ManagedBitmap(Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, buffer.Length);
            this.Width = buffer[4] << 8 | buffer[5];
            this.Height = buffer[6] << 8 | buffer[7];
            this._colors = new Color[this.Width * this.Height];
            buffer = new byte[this.Width * this.Height * 4];
            stream.Read(buffer, 0, buffer.Length);
            int start = 0;
            for (int i = 0; i < this._colors.Length; i++)
            {
                this._colors[i] = Color.FromArgb(buffer[start], buffer[start + 1], buffer[start + 2], buffer[start + 3]);
                start += 4;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedBitmap"/> class.
        /// </summary>
        /// <param name="oldBitmap">
        /// The old bitmap.
        /// </param>
        public ManagedBitmap(Bitmap oldBitmap)
        {
            NikseBitmap nbmp = new NikseBitmap(oldBitmap);
            this.Width = nbmp.Width;
            this.Height = nbmp.Height;
            this._colors = new Color[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.SetPixel(x, y, nbmp.GetPixel(x, y));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedBitmap"/> class.
        /// </summary>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        public ManagedBitmap(NikseBitmap nbmp)
        {
            this.Width = nbmp.Width;
            this.Height = nbmp.Height;
            this._colors = new Color[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.SetPixel(x, y, nbmp.GetPixel(x, y));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedBitmap"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public ManagedBitmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._colors = new Color[this.Width * this.Height];
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets a value indicating whether loaded ok.
        /// </summary>
        public bool LoadedOk { get; private set; }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void Save(string fileName)
        {
            using (MemoryStream outFile = new MemoryStream())
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("MBMP");
                outFile.Write(buffer, 0, buffer.Length);
                WriteInt16(outFile, (short)this.Width);
                WriteInt16(outFile, (short)this.Height);
                foreach (Color c in this._colors)
                {
                    WriteColor(outFile, c);
                }

                buffer = outFile.ToArray();
                using (GZipStream gz = new GZipStream(new FileStream(fileName, FileMode.Create), CompressionMode.Compress, false))
                {
                    gz.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// The append to stream.
        /// </summary>
        /// <param name="targetStream">
        /// The target stream.
        /// </param>
        public void AppendToStream(Stream targetStream)
        {
            using (MemoryStream outFile = new MemoryStream())
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("MBMP");
                outFile.Write(buffer, 0, buffer.Length);
                WriteInt16(outFile, (short)this.Width);
                WriteInt16(outFile, (short)this.Height);
                foreach (Color c in this._colors)
                {
                    WriteColor(outFile, c);
                }

                buffer = outFile.ToArray();
                targetStream.Write(buffer, 0, buffer.Length);
            }
        }

        // private static int ReadInt16(Stream stream)
        // {
        // byte b0 = (byte)stream.ReadByte();
        // byte b1 = (byte)stream.ReadByte();
        // return b0 << 8 | b1;
        // }

        /// <summary>
        /// The write int 16.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        private static void WriteInt16(Stream stream, short val)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)((val & 0xFF00) >> 8);
            buffer[1] = (byte)(val & 0x00FF);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// The write color.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        private static void WriteColor(Stream stream, Color c)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)c.A;
            buffer[1] = (byte)c.R;
            buffer[2] = (byte)c.G;
            buffer[3] = (byte)c.B;
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// The get pixel.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetPixel(int x, int y)
        {
            return this._colors[this.Width * y + x];
        }

        /// <summary>
        /// The set pixel.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        public void SetPixel(int x, int y, Color c)
        {
            this._colors[this.Width * y + x] = c;
        }

        /// <summary>
        /// Copies a rectangle from the bitmap to a new bitmap
        /// </summary>
        /// <param name="section">
        /// Source rectangle
        /// </param>
        /// <returns>
        /// Rectangle from current image as new bitmap
        /// </returns>
        public ManagedBitmap GetRectangle(Rectangle section)
        {
            ManagedBitmap newRectangle = new ManagedBitmap(section.Width, section.Height);

            int recty = 0;
            for (int y = section.Top; y < section.Top + section.Height; y++)
            {
                int rectx = 0;
                for (int x = section.Left; x < section.Left + section.Width; x++)
                {
                    newRectangle.SetPixel(rectx, recty, this.GetPixel(x, y));
                    rectx++;
                }

                recty++;
            }

            return newRectangle;
        }

        /// <summary>
        /// The to old bitmap.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap ToOldBitmap()
        {
            NikseBitmap nbmp = new NikseBitmap(this.Width, this.Height);
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    nbmp.SetPixel(x, y, this.GetPixel(x, y));
                }
            }

            return nbmp.GetBitmap();
        }
    }
}