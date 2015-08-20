// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangStringAStr.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangStringAStr interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 0108

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangStringAStr interface.
    /// </summary>
    [ComImport]
    [Guid("C04D65D2-B70D-11D0-B188-00AA0038C969")]
    [ComConversionLoss]
    [InterfaceType(1)]
    public interface IMLangStringAStr : IMLangString
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
        /// The set a str.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="uCodePage">
        /// The u code page.
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
        void SetAStr([In] int lDestPos, [In] int lDestLen, [In] uint uCodePage, [In] ref sbyte pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The set str buf a.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="uCodePage">
        /// The u code page.
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
        void SetStrBufA([In] int lDestPos, [In] int lDestLen, [In] uint uCodePage, [In] [MarshalAs(UnmanagedType.Interface)] IMLangStringBufA pSrcBuf, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The get a str.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        /// <param name="uCodePageIn">
        /// The u code page in.
        /// </param>
        /// <param name="puCodePageOut">
        /// The pu code page out.
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
        void GetAStr([In] int lSrcPos, [In] int lSrcLen, [In] uint uCodePageIn, out uint puCodePageOut, out sbyte pszDest, [In] int cchDest, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The get str buf a.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcMaxLen">
        /// The l src max len.
        /// </param>
        /// <param name="puDestCodePage">
        /// The pu dest code page.
        /// </param>
        /// <param name="ppDestBuf">
        /// The pp dest buf.
        /// </param>
        /// <param name="plDestLen">
        /// The pl dest len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetStrBufA([In] int lSrcPos, [In] int lSrcMaxLen, out uint puDestCodePage, [MarshalAs(UnmanagedType.Interface)] out IMLangStringBufA ppDestBuf, out int plDestLen);

        /// <summary>
        /// The lock a str.
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
        /// <param name="uCodePageIn">
        /// The u code page in.
        /// </param>
        /// <param name="cchRequest">
        /// The cch request.
        /// </param>
        /// <param name="puCodePageOut">
        /// The pu code page out.
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
        void LockAStr([In] int lSrcPos, [In] int lSrcLen, [In] int lFlags, [In] uint uCodePageIn, [In] int cchRequest, out uint puCodePageOut, [Out] IntPtr ppszDest, out int pcchDest, out int plDestLen);

        /// <summary>
        /// The unlock a str.
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
        void UnlockAStr([In] ref sbyte pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

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