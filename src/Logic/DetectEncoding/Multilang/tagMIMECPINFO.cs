// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagMIMECPINFO.cs" company="">
//   
// </copyright>
// <summary>
//   The tag mimecpinfo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag mimecpinfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct tagMIMECPINFO
    {
        /// <summary>
        /// The dw flags.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// The ui code page.
        /// </summary>
        public uint uiCodePage;

        /// <summary>
        /// The ui family code page.
        /// </summary>
        public uint uiFamilyCodePage;

        /// <summary>
        /// The wsz description.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
        public ushort[] wszDescription;

        /// <summary>
        /// The wsz web charset.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public ushort[] wszWebCharset;

        /// <summary>
        /// The wsz header charset.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public ushort[] wszHeaderCharset;

        /// <summary>
        /// The wsz body charset.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public ushort[] wszBodyCharset;

        /// <summary>
        /// The wsz fixed width font.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public ushort[] wszFixedWidthFont;

        /// <summary>
        /// The wsz proportional font.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public ushort[] wszProportionalFont;

        /// <summary>
        /// The b gdi charset.
        /// </summary>
        public byte bGDICharset;
    }
}