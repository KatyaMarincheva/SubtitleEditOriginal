// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CMLangStringClass.cs" company="">
//   
// </copyright>
// <summary>
//   The cm lang string class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The cm lang string class.
    /// </summary>
    [ComImport]
    [Guid("C04D65CF-B70D-11D0-B188-00AA0038C969")]
    [ComConversionLoss]
    [ClassInterface((short)0)]
    [TypeLibType(2)]
    public class CMLangStringClass : ICMLangString, IMLangStringWStr, IMLangStringAStr
    {
        /// <summary>
        /// The get length.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int GetLength();

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
        public virtual extern void GetMLStr([In] int lSrcPos, [In] int lSrcLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] uint dwClsContext, [In] ref Guid piid, [MarshalAs(UnmanagedType.IUnknown)] out object ppDestMLStr, out int plDestPos, out int plDestLen);

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
        public virtual extern void SetMLStr([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen);

        /// <summary>
        /// The sync.
        /// </summary>
        /// <param name="fNoAccess">
        /// The f no access.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Sync([In] int fNoAccess);

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
        public virtual extern void GetAStr([In] int lSrcPos, [In] int lSrcLen, [In] uint uCodePageIn, out uint puCodePageOut, out sbyte pszDest, [In] int cchDest, out int pcchActual, out int plActualLen);

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
        public virtual extern void GetStrBufA([In] int lSrcPos, [In] int lSrcMaxLen, out uint puDestCodePage, [MarshalAs(UnmanagedType.Interface)] out IMLangStringBufA ppDestBuf, out int plDestLen);

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
        public virtual extern void LockAStr([In] int lSrcPos, [In] int lSrcLen, [In] int lFlags, [In] uint uCodePageIn, [In] int cchRequest, out uint puCodePageOut, [Out] IntPtr ppszDest, out int pcchDest, out int plDestLen);

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
        public virtual extern void SetAStr([In] int lDestPos, [In] int lDestLen, [In] uint uCodePage, [In] ref sbyte pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

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
        public virtual extern void SetStrBufA([In] int lDestPos, [In] int lDestLen, [In] uint uCodePage, [In] [MarshalAs(UnmanagedType.Interface)] IMLangStringBufA pSrcBuf, out int pcchActual, out int plActualLen);

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
        public virtual extern void UnlockAStr([In] ref sbyte pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

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
        public virtual extern void GetLocale([In] int lSrcPos, [In] int lSrcMaxLen, out uint plocale, out int plLocalePos, out int plLocaleLen);

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
        public virtual extern void GetStrBufW([In] int lSrcPos, [In] int lSrcMaxLen, [MarshalAs(UnmanagedType.Interface)] out IMLangStringBufW ppDestBuf, out int plDestLen);

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
        public virtual extern void GetWStr([In] int lSrcPos, [In] int lSrcLen, out ushort pszDest, [In] int cchDest, out int pcchActual, out int plActualLen);

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
        public virtual extern void LockWStr([In] int lSrcPos, [In] int lSrcLen, [In] int lFlags, [In] int cchRequest, [Out] IntPtr ppszDest, out int pcchDest, out int plDestLen);

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
        public virtual extern void SetLocale([In] int lDestPos, [In] int lDestLen, [In] uint locale);

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
        public virtual extern void SetStrBufW([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.Interface)] IMLangStringBufW pSrcBuf, out int pcchActual, out int plActualLen);

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
        public virtual extern void SetWStr([In] int lDestPos, [In] int lDestLen, [In] ref ushort pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

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
        public virtual extern void UnlockWStr([In] ref ushort pszSrc, [In] int cchSrc, out int pcchActual, out int plActualLen);

        /// <summary>
        /// The im lang string a str get length.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int IMLangStringAStrGetLength();

        /// <summary>
        /// The im lang string a str get locale.
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
        public virtual extern void IMLangStringAStrGetLocale([In] int lSrcPos, [In] int lSrcMaxLen, out uint plocale, out int plLocalePos, out int plLocaleLen);

        /// <summary>
        /// The im lang string a str get ml str.
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
        public virtual extern void IMLangStringAStrGetMLStr([In] int lSrcPos, [In] int lSrcLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] uint dwClsContext, [In] ref Guid piid, [MarshalAs(UnmanagedType.IUnknown)] out object ppDestMLStr, out int plDestPos, out int plDestLen);

        /// <summary>
        /// The im lang string a str set locale.
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
        public virtual extern void IMLangStringAStrSetLocale([In] int lDestPos, [In] int lDestLen, [In] uint locale);

        /// <summary>
        /// The im lang string a str set ml str.
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
        public virtual extern void IMLangStringAStrSetMLStr([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen);

        /// <summary>
        /// The im lang string a str sync.
        /// </summary>
        /// <param name="fNoAccess">
        /// The f no access.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangStringAStrSync([In] int fNoAccess);

        /// <summary>
        /// The im lang string w str get length.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int IMLangStringWStrGetLength();

        /// <summary>
        /// The im lang string w str get ml str.
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
        public virtual extern void IMLangStringWStrGetMLStr([In] int lSrcPos, [In] int lSrcLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] uint dwClsContext, [In] ref Guid piid, [MarshalAs(UnmanagedType.IUnknown)] out object ppDestMLStr, out int plDestPos, out int plDestLen);

        /// <summary>
        /// The im lang string w str set ml str.
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
        public virtual extern void IMLangStringWStrSetMLStr([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen);

        /// <summary>
        /// The im lang string w str sync.
        /// </summary>
        /// <param name="fNoAccess">
        /// The f no access.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangStringWStrSync([In] int fNoAccess);
    }
}