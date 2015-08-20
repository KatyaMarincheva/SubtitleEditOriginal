// --------------------------------------------------------------------------------------------------------------------
// <copyright file="_FILETIME.cs" company="">
//   
// </copyright>
// <summary>
//   The _ filetime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The _ filetime.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _FILETIME
    {
        /// <summary>
        /// The dw low date time.
        /// </summary>
        public uint dwLowDateTime;

        /// <summary>
        /// The dw high date time.
        /// </summary>
        public uint dwHighDateTime;
    }
}