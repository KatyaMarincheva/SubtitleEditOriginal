// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ColorChangedEventArgs.cs">
//   
// </copyright>
// <summary>
//   The color changed event args.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.ColorChooser
{
    using System;

    /// <summary>
    /// The color changed event args.
    /// </summary>
    public class ColorChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorChangedEventArgs"/> class.
        /// </summary>
        /// <param name="argb">
        /// The argb.
        /// </param>
        /// <param name="hsv">
        /// The hsv.
        /// </param>
        public ColorChangedEventArgs(ColorHandler.ARGB argb, ColorHandler.HSV hsv)
        {
            this.ARGB = argb;
            this.HSV = hsv;
        }

        /// <summary>
        /// Gets the argb.
        /// </summary>
        public ColorHandler.ARGB ARGB { get; private set; }

        /// <summary>
        /// Gets the hsv.
        /// </summary>
        public ColorHandler.HSV HSV { get; private set; }
    }
}