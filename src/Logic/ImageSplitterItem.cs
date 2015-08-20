// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSplitterItem.cs" company="">
//   
// </copyright>
// <summary>
//   The image splitter item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    /// <summary>
    /// The image splitter item.
    /// </summary>
    public class ImageSplitterItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSplitterItem"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        public ImageSplitterItem(int x, int y, NikseBitmap bitmap)
        {
            this.X = x;
            this.Y = y;
            this.NikseBitmap = bitmap;
            this.SpecialCharacter = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSplitterItem"/> class.
        /// </summary>
        /// <param name="specialCharacter">
        /// The special character.
        /// </param>
        public ImageSplitterItem(string specialCharacter)
        {
            this.X = 0;
            this.Y = 0;
            this.SpecialCharacter = specialCharacter;
            this.NikseBitmap = null;
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the parent y.
        /// </summary>
        public int ParentY { get; set; }

        /// <summary>
        /// Gets or sets the nikse bitmap.
        /// </summary>
        public NikseBitmap NikseBitmap { get; set; }

        /// <summary>
        /// Gets or sets the special character.
        /// </summary>
        public string SpecialCharacter { get; set; }
    }
}