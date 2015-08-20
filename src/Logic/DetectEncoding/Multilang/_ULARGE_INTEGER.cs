// --------------------------------------------------------------------------------------------------------------------
// <copyright file="_ULARGE_INTEGER.cs" company="">
//   
// </copyright>
// <summary>
//   The _ ularg e_ integer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The _ ularg e_ integer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct _ULARGE_INTEGER
    {
        /// <summary>
        /// The quad part.
        /// </summary>
        public ulong QuadPart;
    }
}