// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagSCRIPFONTINFO.cs" company="">
//   
// </copyright>
// <summary>
//   The tag scripfontinfo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag scripfontinfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct tagSCRIPFONTINFO
    {
        /// <summary>
        /// The scripts.
        /// </summary>
        public long scripts;

        /// <summary>
        /// The wsz font.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public ushort[] wszFont;
    }
}