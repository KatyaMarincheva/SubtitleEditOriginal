// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagSCRIPTINFO.cs" company="">
//   
// </copyright>
// <summary>
//   The tag scriptinfo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag scriptinfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct tagSCRIPTINFO
    {
        /// <summary>
        /// The script id.
        /// </summary>
        public byte ScriptId;

        /// <summary>
        /// The ui code page.
        /// </summary>
        public uint uiCodePage;

        /// <summary>
        /// The wsz description.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public ushort[] wszDescription;

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
    }
}