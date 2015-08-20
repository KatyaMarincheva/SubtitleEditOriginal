// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStream.cs" company="">
//   
// </copyright>
// <summary>
//   The Stream interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 0108

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The Stream interface.
    /// </summary>
    [ComImport]
    [Guid("0000000C-0000-0000-C000-000000000046")]
    [InterfaceType(1)]
    public interface IStream : ISequentialStream
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

        /// <summary>
        /// The remote seek.
        /// </summary>
        /// <param name="dlibMove">
        /// The dlib move.
        /// </param>
        /// <param name="dwOrigin">
        /// The dw origin.
        /// </param>
        /// <param name="plibNewPosition">
        /// The plib new position.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, IntPtr plibNewPosition);

        /// <summary>
        /// The set size.
        /// </summary>
        /// <param name="libNewSize">
        /// The lib new size.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetSize([In] _ULARGE_INTEGER libNewSize);

        /// <summary>
        /// The remote copy to.
        /// </summary>
        /// <param name="pstm">
        /// The pstm.
        /// </param>
        /// <param name="cb">
        /// The cb.
        /// </param>
        /// <param name="pcbRead">
        /// The pcb read.
        /// </param>
        /// <param name="pcbWritten">
        /// The pcb written.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteCopyTo([In] [MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);

        /// <summary>
        /// The commit.
        /// </summary>
        /// <param name="grfCommitFlags">
        /// The grf commit flags.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit([In] uint grfCommitFlags);

        /// <summary>
        /// The revert.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Revert();

        /// <summary>
        /// The lock region.
        /// </summary>
        /// <param name="libOffset">
        /// The lib offset.
        /// </param>
        /// <param name="cb">
        /// The cb.
        /// </param>
        /// <param name="dwLockType">
        /// The dw lock type.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);

        /// <summary>
        /// The unlock region.
        /// </summary>
        /// <param name="libOffset">
        /// The lib offset.
        /// </param>
        /// <param name="cb">
        /// The cb.
        /// </param>
        /// <param name="dwLockType">
        /// The dw lock type.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);

        /// <summary>
        /// The stat.
        /// </summary>
        /// <param name="pstatstg">
        /// The pstatstg.
        /// </param>
        /// <param name="grfStatFlag">
        /// The grf stat flag.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="ppstm">
        /// The ppstm.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
    }
}

#pragma warning restore 0108