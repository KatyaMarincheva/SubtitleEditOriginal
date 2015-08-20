// --------------------------------------------------------------------------------------------------------------------
// <copyright file="_LARGE_INTEGER.cs" company="">
//   
// </copyright>
// <summary>
//   The _ larg e_ integer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The _ larg e_ integer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct _LARGE_INTEGER
    {
        /// <summary>
        /// The quad part.
        /// </summary>
        public long QuadPart;
    }
}