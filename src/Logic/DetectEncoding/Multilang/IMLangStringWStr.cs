// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangStringWStr.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangStringWStr interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 0108

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangStringWStr interface.
    /// </summary>
    [ComImport]
    [InterfaceType(1)]
    [ComConversionLoss]
    [Guid("C04D65D0-B70D-11D0-B188-00AA0038C969")]
    public interface IMLangStringWStr : IMLangString
    {
        /// <summary>
        /// The sync.
        /// </summary>
        /// <param name="fNoAccess">
        /// The f no access.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Sync([In] int fNoAccess);

        /// <summary>
        /// The get length.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetLength();

        /// <summary>
        /// The set ml str.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="pSrcMLStr">
        /// The p src ml str.
        /// </param>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetMLStr([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen);

        /// <summary>
        /// The get ml str.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        /// <param name="pUnkOuter">
        /// The p unk outer.
        /// </param>
        /// <param name="dwClsContext">
        /// The dw cls context.
        /// </param>
        /// <param name="piid">
        /// The piid.
        /// </param>
        /// <param name="ppDestMLStr">
        /// The pp dest ml str.
        /// </param>
        /// <param name="plDestPos">
        /// The pl dest pos.
        /// </param>
        /// <param name="plDestLen">
        /// The pl dest len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetMLStr([In] int lSrcPos, [In] int lSrcLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] uint dwClsContext, [In] ref Guid piid, [MarshalAs(UnmanagedType.IUnknown)] out object ppDestMLStr, out int plDestPos, out int plDestLen);

        /// <summary>
        /// The set w str.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="pszSrc">
        /// The psz src.
        /// </param>
        /// <param name="cchSrc">
        /// The cch src.
        /// </param>
        /// <param name="pcchActual">
        /// The pcch actual.
        /// </param>
        /// <param name="plActualLen">
        /// The pl actual len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetWStr([In] int lDestPos, [In] int lDestLen, [In] ref ushort pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The set str buf w.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="pSrcBuf">
        /// The p src buf.
        /// </param>
        /// <param name="pcchActual">
        /// The pcch actual.
        /// </param>
        /// <param name="plActualLen">
        /// The pl actual len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetStrBufW([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.Interface)] IMLangStringBufW pSrcBuf, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The get w str.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        /// <param name="pszDest">
        /// The psz dest.
        /// </param>
        /// <param name="cchDest">
        /// The cch dest.
        /// </param>
        /// <param name="pcchActual">
        /// The pcch actual.
        /// </param>
        /// <param name="plActualLen">
        /// The pl actual len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetWStr([In] int lSrcPos, [In] int lSrcLen, out ushort pszDest, [In] int cchDest, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The get str buf w.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcMaxLen">
        /// The l src max len.
        /// </param>
        /// <param name="ppDestBuf">
        /// The pp dest buf.
        /// </param>
        /// <param name="plDestLen">
        /// The pl dest len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetStrBufW([In] int lSrcPos, [In] int lSrcMaxLen, [MarshalAs(UnmanagedType.Interface)] out IMLangStringBufW ppDestBuf, out int plDestLen);

        /// <summary>
        /// The lock w str.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        /// <param name="lFlags">
        /// The l flags.
        /// </param>
        /// <param name="cchRequest">
        /// The cch request.
        /// </param>
        /// <param name="ppszDest">
        /// The ppsz dest.
        /// </param>
        /// <param name="pcchDest">
        /// The pcch dest.
        /// </param>
        /// <param name="plDestLen">
        /// The pl dest len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LockWStr([In] int lSrcPos, [In] int lSrcLen, [In] int lFlags, [In] int cchRequest, [Out] IntPtr ppszDest, out int pcchDest, out int plDestLen);

        /// <summary>
        /// The unlock w str.
        /// </summary>
        /// <param name="pszSrc">
        /// The psz src.
        /// </param>
        /// <param name="cchSrc">
        /// The cch src.
        /// </param>
        /// <param name="pcchActual">
        /// The pcch actual.
        /// </param>
        /// <param name="plActualLen">
        /// The pl actual len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void UnlockWStr([In] ref ushort pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The set locale.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="locale">
        /// The locale.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetLocale([In] int lDestPos, [In] int lDestLen, [In] uint locale);

        /// <summary>
        /// The get locale.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcMaxLen">
        /// The l src max len.
        /// </param>
        /// <param name="plocale">
        /// The plocale.
        /// </param>
        /// <param name="plLocalePos">
        /// The pl locale pos.
        /// </param>
        /// <param name="plLocaleLen">
        /// The pl locale len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetLocale([In] int lSrcPos, [In] int lSrcMaxLen, out uint plocale, out int plLocalePos, out int plLocaleLen);
    }
}

#pragma warning restore 0108