// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMultiLanguage.cs" company="">
//   
// </copyright>
// <summary>
//   The MultiLanguage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MultiLanguage interface.
    /// </summary>
    [ComImport]
    [Guid("275C23E1-3747-11D0-9FEA-00AA003F8646")]
    [InterfaceType(1)]
    public interface IMultiLanguage
    {
        /// <summary>
        /// The get number of code page info.
        /// </summary>
        /// <param name="pcCodePage">
        /// The pc code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetNumberOfCodePageInfo(out uint pcCodePage);

        /// <summary>
        /// The get code page info.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="pCodePageInfo">
        /// The p code page info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCodePageInfo([In] uint uiCodePage, out tagMIMECPINFO pCodePageInfo);

        /// <summary>
        /// The get family code page.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="puiFamilyCodePage">
        /// The pui family code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFamilyCodePage([In] uint uiCodePage, out uint puiFamilyCodePage);

        /// <summary>
        /// The enum code pages.
        /// </summary>
        /// <param name="grfFlags">
        /// The grf flags.
        /// </param>
        /// <param name="ppEnumCodePage">
        /// The pp enum code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumCodePages([In] uint grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumCodePage ppEnumCodePage);

        /// <summary>
        /// The get charset info.
        /// </summary>
        /// <param name="charset">
        /// The charset.
        /// </param>
        /// <param name="pCharsetInfo">
        /// The p charset info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCharsetInfo([In] [MarshalAs(UnmanagedType.BStr)] string charset, out tagMIMECSETINFO pCharsetInfo);

        /// <summary>
        /// The is convertible.
        /// </summary>
        /// <param name="dwSrcEncoding">
        /// The dw src encoding.
        /// </param>
        /// <param name="dwDstEncoding">
        /// The dw dst encoding.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void IsConvertible([In] uint dwSrcEncoding, [In] uint dwDstEncoding);

        /// <summary>
        /// The convert string.
        /// </summary>
        /// <param name="pdwMode">
        /// The pdw mode.
        /// </param>
        /// <param name="dwSrcEncoding">
        /// The dw src encoding.
        /// </param>
        /// <param name="dwDstEncoding">
        /// The dw dst encoding.
        /// </param>
        /// <param name="pSrcStr">
        /// The p src str.
        /// </param>
        /// <param name="pcSrcSize">
        /// The pc src size.
        /// </param>
        /// <param name="pDstStr">
        /// The p dst str.
        /// </param>
        /// <param name="pcDstSize">
        /// The pc dst size.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ConvertString([In] [Out] ref uint pdwMode, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] ref byte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref byte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The convert string to unicode.
        /// </summary>
        /// <param name="pdwMode">
        /// The pdw mode.
        /// </param>
        /// <param name="dwEncoding">
        /// The dw encoding.
        /// </param>
        /// <param name="pSrcStr">
        /// The p src str.
        /// </param>
        /// <param name="pcSrcSize">
        /// The pc src size.
        /// </param>
        /// <param name="pDstStr">
        /// The p dst str.
        /// </param>
        /// <param name="pcDstSize">
        /// The pc dst size.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ConvertStringToUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The convert string from unicode.
        /// </summary>
        /// <param name="pdwMode">
        /// The pdw mode.
        /// </param>
        /// <param name="dwEncoding">
        /// The dw encoding.
        /// </param>
        /// <param name="pSrcStr">
        /// The p src str.
        /// </param>
        /// <param name="pcSrcSize">
        /// The pc src size.
        /// </param>
        /// <param name="pDstStr">
        /// The p dst str.
        /// </param>
        /// <param name="pcDstSize">
        /// The pc dst size.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ConvertStringFromUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The convert string reset.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ConvertStringReset();

        /// <summary>
        /// The get rfc 1766 from lcid.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="pbstrRfc1766">
        /// The pbstr rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRfc1766FromLcid([In] uint locale, [MarshalAs(UnmanagedType.BStr)] out string pbstrRfc1766);

        /// <summary>
        /// The get lcid from rfc 1766.
        /// </summary>
        /// <param name="plocale">
        /// The plocale.
        /// </param>
        /// <param name="bstrRfc1766">
        /// The bstr rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetLcidFromRfc1766(out uint plocale, [In] [MarshalAs(UnmanagedType.BStr)] string bstrRfc1766);

        /// <summary>
        /// The enum rfc 1766.
        /// </summary>
        /// <param name="ppEnumRfc1766">
        /// The pp enum rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumRfc1766([MarshalAs(UnmanagedType.Interface)] out IEnumRfc1766 ppEnumRfc1766);

        /// <summary>
        /// The get rfc 1766 info.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="pRfc1766Info">
        /// The p rfc 1766 info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRfc1766Info([In] uint locale, out tagRFC1766INFO pRfc1766Info);

        /// <summary>
        /// The create convert charset.
        /// </summary>
        /// <param name="uiSrcCodePage">
        /// The ui src code page.
        /// </param>
        /// <param name="uiDstCodePage">
        /// The ui dst code page.
        /// </param>
        /// <param name="dwProperty">
        /// The dw property.
        /// </param>
        /// <param name="ppMLangConvertCharset">
        /// The pp m lang convert charset.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateConvertCharset([In] uint uiSrcCodePage, [In] uint uiDstCodePage, [In] uint dwProperty, [MarshalAs(UnmanagedType.Interface)] out ICMLangConvertCharset ppMLangConvertCharset);
    }
}