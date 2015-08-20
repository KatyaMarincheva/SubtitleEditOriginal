// --------------------------------------------------------------------------------------------------------------------
// <copyright file="_RemotableHandle.cs" company="">
//   
// </copyright>
// <summary>
//   The _ remotable handle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The _ remotable handle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _RemotableHandle
    {
        /// <summary>
        /// The f context.
        /// </summary>
        public int fContext;

        /// <summary>
        /// The u.
        /// </summary>
        public __MIDL_IWinTypes_0009 u;
    }
}