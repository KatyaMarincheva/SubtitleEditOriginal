// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="PaletteInfo.cs">
//   
// </copyright>
// <summary>
//   contains offset and size of one update of a palette
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.BluRaySup
{
    using System;

    /// <summary>
    ///     contains offset and size of one update of a palette
    /// </summary>
    public class PaletteInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteInfo"/> class.
        /// </summary>
        public PaletteInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteInfo"/> class.
        /// </summary>
        /// <param name="paletteInfo">
        /// The palette info.
        /// </param>
        public PaletteInfo(PaletteInfo paletteInfo)
        {
            this.PaletteSize = paletteInfo.PaletteSize;
            this.PaletteBuffer = new byte[paletteInfo.PaletteBuffer.Length];
            Buffer.BlockCopy(paletteInfo.PaletteBuffer, 0, this.PaletteBuffer, 0, this.PaletteBuffer.Length);
        }

        /// <summary>
        ///     number of palette entries
        /// </summary>
        public int PaletteSize { get; set; }

        /// <summary>
        ///     raw palette data
        /// </summary>
        public byte[] PaletteBuffer { get; set; }
    }
}