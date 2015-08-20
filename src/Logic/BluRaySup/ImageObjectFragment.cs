// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ImageObjectFragment.cs">
//   
// </copyright>
// <summary>
//   contains offset and size of one fragment containing (parts of the) RLE buffer
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.BluRaySup
{
    /// <summary>
    ///     contains offset and size of one fragment containing (parts of the) RLE buffer
    /// </summary>
    public class ImageObjectFragment
    {
        /// <summary>
        ///     size of this part of the RLE buffer
        /// </summary>
        public int ImagePacketSize { get; set; }

        /// <summary>
        ///     Buffer for raw image data fragment
        /// </summary>
        public byte[] ImageBuffer { get; set; }
    }
}