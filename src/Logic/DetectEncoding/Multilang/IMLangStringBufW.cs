// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangStringBufW.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangStringBufW interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangStringBufW interface.
    /// </summary>
    [ComImport]
    [InterfaceType(1)]
    [Guid("D24ACD21-BA72-11D0-B188-00AA0038C969")]
    [ComConversionLoss]
    public interface IMLangStringBufW
    {
        /// <summary>
        /// The get status.
        /// </summary>
        /// <param name="plFlags">
        /// The pl flags.
        /// </param>
        /// <param name="pcchBuf">
        /// The pcch buf.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetStatus(out int plFlags, out int pcchBuf);

        /// <summary>
        /// The lock buf.
        /// </summary>
        /// <param name="cchOffset">
        /// The cch offset.
        /// </param>
        /// <param name="cchMaxLock">
        /// The cch max lock.
        /// </param>
        /// <param name="ppszBuf">
        /// The ppsz buf.
        /// </param>
        /// <param name="pcchBuf">
        /// The pcch buf.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LockBuf([In] int cchOffset, [In] int cchMaxLock, [Out] IntPtr ppszBuf, out int pcchBuf);

        /// <summary>
        /// The unlock buf.
        /// </summary>
        /// <param name="pszBuf">
        /// The psz buf.
        /// </param>
        /// <param name="cchOffset">
        /// The cch offset.
        /// </param>
        /// <param name="cchWrite">
        /// The cch write.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void UnlockBuf([In] ref ushort pszBuf, [In] int cchOffset, [In] int cchWrite);

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="cchOffset">
        /// The cch offset.
        /// </param>
        /// <param name="cchMaxInsert">
        /// The cch max insert.
        /// </param>
        /// <param name="pcchActual">
        /// The pcch actual.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Insert([In] int cchOffset, [In] int cchMaxInsert, out int pcchActual);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="cchOffset">
        /// The cch offset.
        /// </param>
        /// <param name="cchDelete">
        /// The cch delete.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Delete([In] int cchOffset, [In] int cchDelete);
    }
}