// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagMIMECSETINFO.cs" company="">
//   
// </copyright>
// <summary>
//   The tag mimecsetinfo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag mimecsetinfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct tagMIMECSETINFO
    {
        /// <summary>
        /// The ui code page.
        /// </summary>
        public uint uiCodePage;

        /// <summary>
        /// The ui internet encoding.
        /// </summary>
        public uint uiInternetEncoding;

        /// <summary>
        /// The wsz charset.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public ushort[] wszCharset;
    }
}