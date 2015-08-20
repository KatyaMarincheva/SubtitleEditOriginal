// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="FastBitmap.cs">
//   
// </copyright>
// <summary>
//   The fast bitmap.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// The fast bitmap.
    /// </summary>
    public unsafe class FastBitmap
    {
        /// <summary>
        /// The _working bitmap.
        /// </summary>
        private readonly Bitmap _workingBitmap;

        /// <summary>
        /// The _bitmap data.
        /// </summary>
        private BitmapData _bitmapData;

        /// <summary>
        /// The _p base.
        /// </summary>
        private byte* _pBase = null;

        /// <summary>
        /// The _pixel data.
        /// </summary>
        private PixelData* _pixelData = null;

        /// <summary>
        /// The _width.
        /// </summary>
        private int _width;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class.
        /// </summary>
        /// <param name="inputBitmap">
        /// The input bitmap.
        /// </param>
        public FastBitmap(Bitmap inputBitmap)
        {
            this._workingBitmap = inputBitmap;

            if (this._workingBitmap.PixelFormat != PixelFormat.Format32bppArgb && Environment.OSVersion.Version.Major < 6 && Configuration.Settings.General.SubtitleFontName == Utilities.WinXP2KUnicodeFontName)
            { // WinXp Fix
                // 6 == Vista/Win2008Server/Win7
                var newBitmap = new Bitmap(this._workingBitmap.Width, this._workingBitmap.Height, PixelFormat.Format32bppArgb);
                for (int y = 0; y < this._workingBitmap.Height; y++)
                {
                    for (int x = 0; x < this._workingBitmap.Width; x++)
                    {
                        newBitmap.SetPixel(x, y, this._workingBitmap.GetPixel(x, y));
                    }
                }

                this._workingBitmap = newBitmap;
            }

            this.Width = inputBitmap.Width;
            this.Height = inputBitmap.Height;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The lock image.
        /// </summary>
        public void LockImage()
        {
            var bounds = new Rectangle(Point.Empty, this._workingBitmap.Size);

            this._width = bounds.Width * sizeof(PixelData);
            if (this._width % 4 != 0)
            {
                this._width = 4 * (this._width / 4 + 1);
            }

            // Lock Image
            this._bitmapData = this._workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            this._pBase = (Byte*)this._bitmapData.Scan0.ToPointer();
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
            this._pixelData = (PixelData*)(this._pBase + y * this._width + x * sizeof(PixelData));
            return Color.FromArgb(this._pixelData->Alpha, this._pixelData->Red, this._pixelData->Green, this._pixelData->Blue);
        }

        /// <summary>
        /// The get pixel next.
        /// </summary>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetPixelNext()
        {
            this._pixelData++;
            return Color.FromArgb(this._pixelData->Alpha, this._pixelData->Red, this._pixelData->Green, this._pixelData->Blue);
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
        /// <param name="color">
        /// The color.
        /// </param>
        public void SetPixel(int x, int y, Color color)
        {
            var data = (PixelData*)(this._pBase + y * this._width + x * sizeof(PixelData));
            data->Alpha = color.A;
            data->Red = color.R;
            data->Green = color.G;
            data->Blue = color.B;
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
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        public void SetPixel(int x, int y, Color color, int length)
        {
            var data = (PixelData*)(this._pBase + y * this._width + x * sizeof(PixelData));
            for (int i = 0; i < length; i++)
            {
                data->Alpha = color.A;
                data->Red = color.R;
                data->Green = color.G;
                data->Blue = color.B;
                data++;
            }
        }

        /// <summary>
        /// The get bitmap.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetBitmap()
        {
            return this._workingBitmap;
        }

        /// <summary>
        /// The unlock image.
        /// </summary>
        public void UnlockImage()
        {
            this._workingBitmap.UnlockBits(this._bitmapData);
            this._bitmapData = null;
            this._pBase = null;
        }

        /// <summary>
        /// The pixel data.
        /// </summary>
        private struct PixelData
        {
            /// <summary>
            /// The alpha.
            /// </summary>
            public byte Alpha;

            /// <summary>
            /// The blue.
            /// </summary>
            public byte Blue;

            /// <summary>
            /// The green.
            /// </summary>
            public byte Green;

            /// <summary>
            /// The red.
            /// </summary>
            public byte Red;

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return "(" + this.Alpha + ", " + this.Red + ", " + this.Green + ", " + this.Blue + ")";
            }
        }
    }
}