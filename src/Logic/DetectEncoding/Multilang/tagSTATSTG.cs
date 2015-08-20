// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagSTATSTG.cs" company="">
//   
// </copyright>
// <summary>
//   The tag statstg.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag statstg.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct tagSTATSTG
    {
        /// <summary>
        /// The pwcs name.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwcsName;

        /// <summary>
        /// The type.
        /// </summary>
        public uint type;

        /// <summary>
        /// The cb size.
        /// </summary>
        public _ULARGE_INTEGER cbSize;

        /// <summary>
        /// The mtime.
        /// </summary>
        public _FILETIME mtime;

        /// <summary>
        /// The ctime.
        /// </summary>
        public _FILETIME ctime;

        /// <summary>
        /// The atime.
        /// </summary>
        public _FILETIME atime;

        /// <summary>
        /// The grf mode.
        /// </summary>
        public uint grfMode;

        /// <summary>
        /// The grf locks supported.
        /// </summary>
        public uint grfLocksSupported;

        /// <summary>
        /// The clsid.
        /// </summary>
        public Guid clsid;

        /// <summary>
        /// The grf state bits.
        /// </summary>
        public uint grfStateBits;

        /// <summary>
        /// The reserved.
        /// </summary>
        public uint reserved;
    }
}