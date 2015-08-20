// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagUNICODERANGE.cs" company="">
//   
// </copyright>
// <summary>
//   The tag unicoderange.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The tag unicoderange.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct tagUNICODERANGE
    {
        /// <summary>
        /// The wc from.
        /// </summary>
        public ushort wcFrom;

        /// <summary>
        /// The wc to.
        /// </summary>
        public ushort wcTo;
    }
}