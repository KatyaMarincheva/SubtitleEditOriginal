// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISequentialStream.cs" company="">
//   
// </copyright>
// <summary>
//   The SequentialStream interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The SequentialStream interface.
    /// </summary>
    [ComImport]
    [Guid("0C733A30-2A1C-11CE-ADE5-00AA0044773D")]
    [InterfaceType(1)]
    public interface ISequentialStream
    {
        /// <summary>
        /// The remote read.
        /// </summary>
        /// <param name="pv">
        /// The pv.
        /// </param>
        /// <param name="cb">
        /// The cb.
        /// </param>
        /// <param name="pcbRead">
        /// The pcb read.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteRead(IntPtr pv, uint cb, ref uint pcbRead);

        /// <summary>
        /// The remote write.
        /// </summary>
        /// <param name="pv">
        /// The pv.
        /// </param>
        /// <param name="cb">
        /// The cb.
        /// </param>
        /// <param name="pcbWritten">
        /// The pcb written.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteWrite([In] ref byte pv, [In] uint cb, ref uint pcbWritten);
    }
}