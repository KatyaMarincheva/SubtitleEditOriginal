// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagRFC1766INFO.cs" company="">
//   
// </copyright>
// <summary>
//   The tag rf c 1766 info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag rf c 1766 info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct tagRFC1766INFO
    {
        /// <summary>
        /// The lcid.
        /// </summary>
        public uint lcid;

        /// <summary>
        /// The wsz rfc 1766.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public ushort[] wszRfc1766;

        /// <summary>
        /// The wsz locale name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public ushort[] wszLocaleName;
    }
}