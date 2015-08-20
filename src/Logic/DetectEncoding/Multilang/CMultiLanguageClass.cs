// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CMultiLanguageClass.cs" company="">
//   
// </copyright>
// <summary>
//   The c multi language class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The c multi language class.
    /// </summary>
    [ComImport]
    [TypeLibType(2)]
    [ClassInterface((short)0)]
    [Guid("275C23E2-3747-11D0-9FEA-00AA003F8646")]
    public class CMultiLanguageClass : ICMultiLanguage, IMLangFontLink, IMLangLineBreakConsole, IMLangFontLink2, IMultiLanguage3
    {
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
        public virtual extern void ConvertString([In] [Out] ref uint pdwMode, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] ref byte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref byte pDstStr, [In] [Out] ref uint pcDstSize);

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
        public virtual extern void ConvertStringFromUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The convert string reset.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ConvertStringReset();

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
        public virtual extern void ConvertStringToUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize);

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
        public virtual extern void CreateConvertCharset([In] uint uiSrcCodePage, [In] uint uiDstCodePage, [In] uint dwProperty, [MarshalAs(UnmanagedType.Interface)] out ICMLangConvertCharset ppMLangConvertCharset);

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
        public virtual extern void EnumCodePages([In] uint grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumCodePage ppEnumCodePage);

        /// <summary>
        /// The enum rfc 1766.
        /// </summary>
        /// <param name="ppEnumRfc1766">
        /// The pp enum rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void EnumRfc1766([MarshalAs(UnmanagedType.Interface)] out IEnumRfc1766 ppEnumRfc1766);

        /// <summary>
        /// The get charset info.
        /// </summary>
        /// <param name="Charset">
        /// The charset.
        /// </param>
        /// <param name="pCharsetInfo">
        /// The p charset info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetCharsetInfo([In] [MarshalAs(UnmanagedType.BStr)] string Charset, out tagMIMECSETINFO pCharsetInfo);

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
        public virtual extern void GetCodePageInfo([In] uint uiCodePage, out tagMIMECPINFO pCodePageInfo);

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
        public virtual extern void GetFamilyCodePage([In] uint uiCodePage, out uint puiFamilyCodePage);

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
        public virtual extern void GetLcidFromRfc1766(out uint plocale, [In] [MarshalAs(UnmanagedType.BStr)] string bstrRfc1766);

        /// <summary>
        /// The get number of code page info.
        /// </summary>
        /// <param name="pcCodePage">
        /// The pc code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetNumberOfCodePageInfo(out uint pcCodePage);

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
        public virtual extern void GetRfc1766FromLcid([In] uint locale, [MarshalAs(UnmanagedType.BStr)] out string pbstrRfc1766);

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
        public virtual extern void GetRfc1766Info([In] uint locale, out tagRFC1766INFO pRfc1766Info);

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
        public virtual extern void IsConvertible([In] uint dwSrcEncoding, [In] uint dwDstEncoding);

        /// <summary>
        /// The code pages to code page.
        /// </summary>
        /// <param name="dwCodePages">
        /// The dw code pages.
        /// </param>
        /// <param name="uDefaultCodePage">
        /// The u default code page.
        /// </param>
        /// <param name="puCodePage">
        /// The pu code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void CodePagesToCodePage([In] uint dwCodePages, [In] uint uDefaultCodePage, out uint puCodePage);

        /// <summary>
        /// The code page to code pages.
        /// </summary>
        /// <param name="uCodePage">
        /// The u code page.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void CodePageToCodePages([In] uint uCodePage, out uint pdwCodePages);

        /// <summary>
        /// The get char code pages.
        /// </summary>
        /// <param name="chSrc">
        /// The ch src.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetCharCodePages([In] ushort chSrc, out uint pdwCodePages);

        /// <summary>
        /// The get font code pages.
        /// </summary>
        /// <param name="hDC">
        /// The h dc.
        /// </param>
        /// <param name="hFont">
        /// The h font.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetFontCodePages([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hFont, out uint pdwCodePages);

        /// <summary>
        /// The get str code pages.
        /// </summary>
        /// <param name="pszSrc">
        /// The psz src.
        /// </param>
        /// <param name="cchSrc">
        /// The cch src.
        /// </param>
        /// <param name="dwPriorityCodePages">
        /// The dw priority code pages.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        /// <param name="pcchCodePages">
        /// The pcch code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetStrCodePages([In] ref ushort pszSrc, [In] int cchSrc, [In] uint dwPriorityCodePages, out uint pdwCodePages, out int pcchCodePages);

        /// <summary>
        /// The map font.
        /// </summary>
        /// <param name="hDC">
        /// The h dc.
        /// </param>
        /// <param name="dwCodePages">
        /// The dw code pages.
        /// </param>
        /// <param name="hSrcFont">
        /// The h src font.
        /// </param>
        /// <param name="phDestFont">
        /// The ph dest font.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void MapFont([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] uint dwCodePages, [In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hSrcFont, [Out] [ComAliasName("MultiLanguage.wireHFONT")] IntPtr phDestFont);

        /// <summary>
        /// The release font.
        /// </summary>
        /// <param name="hFont">
        /// The h font.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ReleaseFont([In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hFont);

        /// <summary>
        /// The reset font mapping.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ResetFontMapping();

        /// <summary>
        /// The code page to script id.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="pSid">
        /// The p sid.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void CodePageToScriptID([In] uint uiCodePage, out byte pSid);

        /// <summary>
        /// The get font unicode ranges.
        /// </summary>
        /// <param name="hDC">
        /// The h dc.
        /// </param>
        /// <param name="puiRanges">
        /// The pui ranges.
        /// </param>
        /// <param name="pUranges">
        /// The p uranges.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetFontUnicodeRanges([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] [Out] ref uint puiRanges, out tagUNICODERANGE pUranges);

        /// <summary>
        /// The get script font info.
        /// </summary>
        /// <param name="sid">
        /// The sid.
        /// </param>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="puiFonts">
        /// The pui fonts.
        /// </param>
        /// <param name="pScriptFont">
        /// The p script font.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetScriptFontInfo([In] byte sid, [In] uint dwFlags, [In] [Out] ref uint puiFonts, out tagSCRIPFONTINFO pScriptFont);

        /// <summary>
        /// The map font.
        /// </summary>
        /// <param name="hDC">
        /// The h dc.
        /// </param>
        /// <param name="dwCodePages">
        /// The dw code pages.
        /// </param>
        /// <param name="chSrc">
        /// The ch src.
        /// </param>
        /// <param name="pFont">
        /// The p font.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void MapFont([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] uint dwCodePages, [In] ushort chSrc, [Out] [ComAliasName("MultiLanguage.wireHFONT")] IntPtr pFont);

        /// <summary>
        /// The break line a.
        /// </summary>
        /// <param name="locale">
        /// The locale.
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
        /// <param name="cMaxColumns">
        /// The c max columns.
        /// </param>
        /// <param name="pcchLine">
        /// The pcch line.
        /// </param>
        /// <param name="pcchSkip">
        /// The pcch skip.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void BreakLineA([In] uint locale, [In] uint uCodePage, [In] ref sbyte pszSrc, [In] int cchSrc, [In] int cMaxColumns, out int pcchLine, out int pcchSkip);

        /// <summary>
        /// The break line ml.
        /// </summary>
        /// <param name="pSrcMLStr">
        /// The p src ml str.
        /// </param>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        /// <param name="cMinColumns">
        /// The c min columns.
        /// </param>
        /// <param name="cMaxColumns">
        /// The c max columns.
        /// </param>
        /// <param name="plLineLen">
        /// The pl line len.
        /// </param>
        /// <param name="plSkipLen">
        /// The pl skip len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void BreakLineML([In] [MarshalAs(UnmanagedType.Interface)] ICMLangString pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen, [In] int cMinColumns, [In] int cMaxColumns, out int plLineLen, out int plSkipLen);

        /// <summary>
        /// The break line w.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="pszSrc">
        /// The psz src.
        /// </param>
        /// <param name="cchSrc">
        /// The cch src.
        /// </param>
        /// <param name="cMaxColumns">
        /// The c max columns.
        /// </param>
        /// <param name="pcchLine">
        /// The pcch line.
        /// </param>
        /// <param name="pcchSkip">
        /// The pcch skip.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void BreakLineW([In] uint locale, [In] ref ushort pszSrc, [In] int cchSrc, [In] int cMaxColumns, out int pcchLine, out int pcchSkip);

        /// <summary>
        /// The convert string from unicode ex.
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
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="lpFallBack">
        /// The lp fall back.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ConvertStringFromUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

        /// <summary>
        /// The convert string in i stream.
        /// </summary>
        /// <param name="pdwMode">
        /// The pdw mode.
        /// </param>
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="lpFallBack">
        /// The lp fall back.
        /// </param>
        /// <param name="dwSrcEncoding">
        /// The dw src encoding.
        /// </param>
        /// <param name="dwDstEncoding">
        /// The dw dst encoding.
        /// </param>
        /// <param name="pstmIn">
        /// The pstm in.
        /// </param>
        /// <param name="pstmOut">
        /// The pstm out.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ConvertStringInIStream([In] [Out] ref uint pdwMode, [In] uint dwFlag, [In] ref ushort lpFallBack, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmIn, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmOut);

        /// <summary>
        /// The convert string to unicode ex.
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
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="lpFallBack">
        /// The lp fall back.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ConvertStringToUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

        /// <summary>
        /// The detect codepage in i stream.
        /// </summary>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="dwPrefWinCodePage">
        /// The dw pref win code page.
        /// </param>
        /// <param name="pstmIn">
        /// The pstm in.
        /// </param>
        /// <param name="lpEncoding">
        /// The lp encoding.
        /// </param>
        /// <param name="pnScores">
        /// The pn scores.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void DetectCodepageInIStream([In] MLDETECTCP flags, [In] uint dwPrefWinCodePage, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmIn, [In] [Out] ref DetectEncodingInfo lpEncoding, [In] [Out] ref int pnScores);

        /// <summary>
        /// The detect input codepage.
        /// </summary>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="dwPrefWinCodePage">
        /// The dw pref win code page.
        /// </param>
        /// <param name="pSrcStr">
        /// The p src str.
        /// </param>
        /// <param name="pcSrcSize">
        /// The pc src size.
        /// </param>
        /// <param name="lpEncoding">
        /// The lp encoding.
        /// </param>
        /// <param name="pnScores">
        /// The pn scores.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void DetectInputCodepage([In] MLDETECTCP flags, [In] uint dwPrefWinCodePage, [In] ref byte pSrcStr, [In] [Out] ref int pcSrcSize, [In] [Out] ref DetectEncodingInfo lpEncoding, [In] [Out] ref int pnScores);

        /// <summary>
        /// The detect outbound code page.
        /// </summary>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="lpWideCharStr">
        /// The lp wide char str.
        /// </param>
        /// <param name="cchWideChar">
        /// The cch wide char.
        /// </param>
        /// <param name="puiPreferredCodePages">
        /// The pui preferred code pages.
        /// </param>
        /// <param name="nPreferredCodePages">
        /// The n preferred code pages.
        /// </param>
        /// <param name="puiDetectedCodePages">
        /// The pui detected code pages.
        /// </param>
        /// <param name="pnDetectedCodePages">
        /// The pn detected code pages.
        /// </param>
        /// <param name="lpSpecialChar">
        /// The lp special char.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void DetectOutboundCodePage([In] MLCPF dwFlags, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, [In] uint cchWideChar, [In] IntPtr puiPreferredCodePages, [In] uint nPreferredCodePages, [In] IntPtr puiDetectedCodePages, [In] [Out] ref uint pnDetectedCodePages, [In] ref ushort lpSpecialChar);

        /// <summary>
        /// The detect outbound code page in i stream.
        /// </summary>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="pStrIn">
        /// The p str in.
        /// </param>
        /// <param name="puiPreferredCodePages">
        /// The pui preferred code pages.
        /// </param>
        /// <param name="nPreferredCodePages">
        /// The n preferred code pages.
        /// </param>
        /// <param name="puiDetectedCodePages">
        /// The pui detected code pages.
        /// </param>
        /// <param name="pnDetectedCodePages">
        /// The pn detected code pages.
        /// </param>
        /// <param name="lpSpecialChar">
        /// The lp special char.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void DetectOutboundCodePageInIStream([In] uint dwFlags, [In] [MarshalAs(UnmanagedType.Interface)] IStream pStrIn, [In] ref uint puiPreferredCodePages, [In] uint nPreferredCodePages, [In] ref uint puiDetectedCodePages, [In] [Out] ref uint pnDetectedCodePages, [In] ref ushort lpSpecialChar);

        /// <summary>
        /// The enum code pages.
        /// </summary>
        /// <param name="grfFlags">
        /// The grf flags.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumCodePage">
        /// The pp enum code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void EnumCodePages([In] uint grfFlags, [In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumCodePage ppEnumCodePage);

        /// <summary>
        /// The enum rfc 1766.
        /// </summary>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumRfc1766">
        /// The pp enum rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void EnumRfc1766([In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumRfc1766 ppEnumRfc1766);

        /// <summary>
        /// The enum scripts.
        /// </summary>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumScript">
        /// The pp enum script.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void EnumScripts([In] uint dwFlags, [In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumScript ppEnumScript);

        /// <summary>
        /// The get code page description.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="lcid">
        /// The lcid.
        /// </param>
        /// <param name="lpWideCharStr">
        /// The lp wide char str.
        /// </param>
        /// <param name="cchWideChar">
        /// The cch wide char.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetCodePageDescription([In] uint uiCodePage, [In] uint lcid, [In] [Out] [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, [In] int cchWideChar);

        /// <summary>
        /// The get code page info.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="pCodePageInfo">
        /// The p code page info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetCodePageInfo([In] uint uiCodePage, [In] ushort langId, out tagMIMECPINFO pCodePageInfo);

        /// <summary>
        /// The get number of scripts.
        /// </summary>
        /// <param name="pnScripts">
        /// The pn scripts.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetNumberOfScripts(out uint pnScripts);

        /// <summary>
        /// The get rfc 1766 info.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="pRfc1766Info">
        /// The p rfc 1766 info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetRfc1766Info([In] uint locale, [In] ushort langId, out tagRFC1766INFO pRfc1766Info);

        /// <summary>
        /// The is code page installable.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IsCodePageInstallable([In] uint uiCodePage);

        /// <summary>
        /// The set mime db source.
        /// </summary>
        /// <param name="dwSource">
        /// The dw source.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetMimeDBSource([In] tagMIMECONTF dwSource);

        /// <summary>
        /// The validate code page.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ValidateCodePage([In] uint uiCodePage, [In] [ComAliasName("MultiLanguage.wireHWND")] ref _RemotableHandle hwnd);

        /// <summary>
        /// The validate code page ex.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="dwfIODControl">
        /// The dwf iod control.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ValidateCodePageEx([In] uint uiCodePage, [In] [ComAliasName("MultiLanguage.wireHWND")] ref _RemotableHandle hwnd, [In] uint dwfIODControl);

        /// <summary>
        /// The im lang font link code pages to code page.
        /// </summary>
        /// <param name="dwCodePages">
        /// The dw code pages.
        /// </param>
        /// <param name="uDefaultCodePage">
        /// The u default code page.
        /// </param>
        /// <param name="puCodePage">
        /// The pu code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLinkCodePagesToCodePage([In] uint dwCodePages, [In] uint uDefaultCodePage, out uint puCodePage);

        /// <summary>
        /// The im lang font link code page to code pages.
        /// </summary>
        /// <param name="uCodePage">
        /// The u code page.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLinkCodePageToCodePages([In] uint uCodePage, out uint pdwCodePages);

        /// <summary>
        /// The im lang font link get char code pages.
        /// </summary>
        /// <param name="chSrc">
        /// The ch src.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLinkGetCharCodePages([In] ushort chSrc, out uint pdwCodePages);

        /// <summary>
        /// The im lang font link get str code pages.
        /// </summary>
        /// <param name="pszSrc">
        /// The psz src.
        /// </param>
        /// <param name="cchSrc">
        /// The cch src.
        /// </param>
        /// <param name="dwPriorityCodePages">
        /// The dw priority code pages.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        /// <param name="pcchCodePages">
        /// The pcch code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLinkGetStrCodePages([In] ref ushort pszSrc, [In] int cchSrc, [In] uint dwPriorityCodePages, out uint pdwCodePages, out int pcchCodePages);

        /// <summary>
        /// The im lang font link 2 code pages to code page.
        /// </summary>
        /// <param name="dwCodePages">
        /// The dw code pages.
        /// </param>
        /// <param name="uDefaultCodePage">
        /// The u default code page.
        /// </param>
        /// <param name="puCodePage">
        /// The pu code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2CodePagesToCodePage([In] uint dwCodePages, [In] uint uDefaultCodePage, out uint puCodePage);

        /// <summary>
        /// The im lang font link 2 code page to code pages.
        /// </summary>
        /// <param name="uCodePage">
        /// The u code page.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2CodePageToCodePages([In] uint uCodePage, out uint pdwCodePages);

        /// <summary>
        /// The im lang font link 2 get char code pages.
        /// </summary>
        /// <param name="chSrc">
        /// The ch src.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2GetCharCodePages([In] ushort chSrc, out uint pdwCodePages);

        /// <summary>
        /// The im lang font link 2 get font code pages.
        /// </summary>
        /// <param name="hDC">
        /// The h dc.
        /// </param>
        /// <param name="hFont">
        /// The h font.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2GetFontCodePages([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hFont, out uint pdwCodePages);

        /// <summary>
        /// The im lang font link 2 get str code pages.
        /// </summary>
        /// <param name="pszSrc">
        /// The psz src.
        /// </param>
        /// <param name="cchSrc">
        /// The cch src.
        /// </param>
        /// <param name="dwPriorityCodePages">
        /// The dw priority code pages.
        /// </param>
        /// <param name="pdwCodePages">
        /// The pdw code pages.
        /// </param>
        /// <param name="pcchCodePages">
        /// The pcch code pages.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2GetStrCodePages([In] ref ushort pszSrc, [In] int cchSrc, [In] uint dwPriorityCodePages, out uint pdwCodePages, out int pcchCodePages);

        /// <summary>
        /// The im lang font link 2 release font.
        /// </summary>
        /// <param name="hFont">
        /// The h font.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2ReleaseFont([In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hFont);

        /// <summary>
        /// The im lang font link 2 reset font mapping.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMLangFontLink2ResetFontMapping();

        /// <summary>
        /// The i multi language 2 convert string.
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
        public virtual extern void IMultiLanguage2ConvertString([In] [Out] ref uint pdwMode, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] ref byte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref byte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The i multi language 2 convert string from unicode.
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
        public virtual extern void IMultiLanguage2ConvertStringFromUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The i multi language 2 convert string reset.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2ConvertStringReset();

        /// <summary>
        /// The i multi language 2 convert string to unicode.
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
        public virtual extern void IMultiLanguage2ConvertStringToUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The i multi language 2 create convert charset.
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
        public virtual extern void IMultiLanguage2CreateConvertCharset([In] uint uiSrcCodePage, [In] uint uiDstCodePage, [In] uint dwProperty, [MarshalAs(UnmanagedType.Interface)] out ICMLangConvertCharset ppMLangConvertCharset);

        /// <summary>
        /// The i multi language 2 get charset info.
        /// </summary>
        /// <param name="charset">
        /// The charset.
        /// </param>
        /// <param name="pCharsetInfo">
        /// The p charset info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2GetCharsetInfo([In] [MarshalAs(UnmanagedType.BStr)] string charset, out tagMIMECSETINFO pCharsetInfo);

        /// <summary>
        /// The i multi language 2 get family code page.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="puiFamilyCodePage">
        /// The pui family code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2GetFamilyCodePage([In] uint uiCodePage, out uint puiFamilyCodePage);

        /// <summary>
        /// The i multi language 2 get lcid from rfc 1766.
        /// </summary>
        /// <param name="plocale">
        /// The plocale.
        /// </param>
        /// <param name="bstrRfc1766">
        /// The bstr rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2GetLcidFromRfc1766(out uint plocale, [In] [MarshalAs(UnmanagedType.BStr)] string bstrRfc1766);

        /// <summary>
        /// The i multi language 2 get number of code page info.
        /// </summary>
        /// <param name="pcCodePage">
        /// The pc code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2GetNumberOfCodePageInfo(out uint pcCodePage);

        /// <summary>
        /// The i multi language 2 get rfc 1766 from lcid.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="pbstrRfc1766">
        /// The pbstr rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2GetRfc1766FromLcid([In] uint locale, [MarshalAs(UnmanagedType.BStr)] out string pbstrRfc1766);

        /// <summary>
        /// The i multi language 2 is convertible.
        /// </summary>
        /// <param name="dwSrcEncoding">
        /// The dw src encoding.
        /// </param>
        /// <param name="dwDstEncoding">
        /// The dw dst encoding.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage2IsConvertible([In] uint dwSrcEncoding, [In] uint dwDstEncoding);

        /// <summary>
        /// The i multi language 3 convert string.
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
        public virtual extern void IMultiLanguage3ConvertString([In] [Out] ref uint pdwMode, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] ref byte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref byte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The i multi language 3 convert string from unicode.
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
        public virtual extern void IMultiLanguage3ConvertStringFromUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The i multi language 3 convert string from unicode ex.
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
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="lpFallBack">
        /// The lp fall back.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3ConvertStringFromUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

        /// <summary>
        /// The i multi language 3 convert string in i stream.
        /// </summary>
        /// <param name="pdwMode">
        /// The pdw mode.
        /// </param>
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="lpFallBack">
        /// The lp fall back.
        /// </param>
        /// <param name="dwSrcEncoding">
        /// The dw src encoding.
        /// </param>
        /// <param name="dwDstEncoding">
        /// The dw dst encoding.
        /// </param>
        /// <param name="pstmIn">
        /// The pstm in.
        /// </param>
        /// <param name="pstmOut">
        /// The pstm out.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3ConvertStringInIStream([In] [Out] ref uint pdwMode, [In] uint dwFlag, [In] ref ushort lpFallBack, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmIn, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmOut);

        /// <summary>
        /// The i multi language 3 convert string reset.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3ConvertStringReset();

        /// <summary>
        /// The i multi language 3 convert string to unicode.
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
        public virtual extern void IMultiLanguage3ConvertStringToUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The i multi language 3 convert string to unicode ex.
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
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="lpFallBack">
        /// The lp fall back.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3ConvertStringToUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

        /// <summary>
        /// The i multi language 3 create convert charset.
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
        public virtual extern void IMultiLanguage3CreateConvertCharset([In] uint uiSrcCodePage, [In] uint uiDstCodePage, [In] uint dwProperty, [MarshalAs(UnmanagedType.Interface)] out ICMLangConvertCharset ppMLangConvertCharset);

        /// <summary>
        /// The i multi language 3 detect codepage in i stream.
        /// </summary>
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="dwPrefWinCodePage">
        /// The dw pref win code page.
        /// </param>
        /// <param name="pstmIn">
        /// The pstm in.
        /// </param>
        /// <param name="lpEncoding">
        /// The lp encoding.
        /// </param>
        /// <param name="pnScores">
        /// The pn scores.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3DetectCodepageInIStream([In] uint dwFlag, [In] uint dwPrefWinCodePage, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmIn, [In] [Out] ref DetectEncodingInfo lpEncoding, [In] [Out] ref int pnScores);

        /// <summary>
        /// The i multi language 3 detect input codepage.
        /// </summary>
        /// <param name="dwFlag">
        /// The dw flag.
        /// </param>
        /// <param name="dwPrefWinCodePage">
        /// The dw pref win code page.
        /// </param>
        /// <param name="pSrcStr">
        /// The p src str.
        /// </param>
        /// <param name="pcSrcSize">
        /// The pc src size.
        /// </param>
        /// <param name="lpEncoding">
        /// The lp encoding.
        /// </param>
        /// <param name="pnScores">
        /// The pn scores.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3DetectInputCodepage([In] uint dwFlag, [In] uint dwPrefWinCodePage, [In] ref sbyte pSrcStr, [In] [Out] ref int pcSrcSize, [In] [Out] ref DetectEncodingInfo lpEncoding, [In] [Out] ref int pnScores);

        /// <summary>
        /// The i multi language 3 enum code pages.
        /// </summary>
        /// <param name="grfFlags">
        /// The grf flags.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumCodePage">
        /// The pp enum code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3EnumCodePages([In] uint grfFlags, [In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumCodePage ppEnumCodePage);

        /// <summary>
        /// The i multi language 3 enum rfc 1766.
        /// </summary>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumRfc1766">
        /// The pp enum rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3EnumRfc1766([In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumRfc1766 ppEnumRfc1766);

        /// <summary>
        /// The i multi language 3 enum scripts.
        /// </summary>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumScript">
        /// The pp enum script.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3EnumScripts([In] uint dwFlags, [In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumScript ppEnumScript);

        /// <summary>
        /// The i multi language 3 get charset info.
        /// </summary>
        /// <param name="charset">
        /// The charset.
        /// </param>
        /// <param name="pCharsetInfo">
        /// The p charset info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetCharsetInfo([In] [MarshalAs(UnmanagedType.BStr)] string charset, out tagMIMECSETINFO pCharsetInfo);

        /// <summary>
        /// The i multi language 3 get code page description.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="lcid">
        /// The lcid.
        /// </param>
        /// <param name="lpWideCharStr">
        /// The lp wide char str.
        /// </param>
        /// <param name="cchWideChar">
        /// The cch wide char.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetCodePageDescription([In] uint uiCodePage, [In] uint lcid, [In] [Out] [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, [In] int cchWideChar);

        /// <summary>
        /// The i multi language 3 get code page info.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="pCodePageInfo">
        /// The p code page info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetCodePageInfo([In] uint uiCodePage, [In] ushort langId, out tagMIMECPINFO pCodePageInfo);

        /// <summary>
        /// The i multi language 3 get family code page.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="puiFamilyCodePage">
        /// The pui family code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetFamilyCodePage([In] uint uiCodePage, out uint puiFamilyCodePage);

        /// <summary>
        /// The i multi language 3 get lcid from rfc 1766.
        /// </summary>
        /// <param name="plocale">
        /// The plocale.
        /// </param>
        /// <param name="bstrRfc1766">
        /// The bstr rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetLcidFromRfc1766(out uint plocale, [In] [MarshalAs(UnmanagedType.BStr)] string bstrRfc1766);

        /// <summary>
        /// The i multi language 3 get number of code page info.
        /// </summary>
        /// <param name="pcCodePage">
        /// The pc code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetNumberOfCodePageInfo(out uint pcCodePage);

        /// <summary>
        /// The i multi language 3 get number of scripts.
        /// </summary>
        /// <param name="pnScripts">
        /// The pn scripts.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetNumberOfScripts(out uint pnScripts);

        /// <summary>
        /// The i multi language 3 get rfc 1766 from lcid.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="pbstrRfc1766">
        /// The pbstr rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetRfc1766FromLcid([In] uint locale, [MarshalAs(UnmanagedType.BStr)] out string pbstrRfc1766);

        /// <summary>
        /// The i multi language 3 get rfc 1766 info.
        /// </summary>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="pRfc1766Info">
        /// The p rfc 1766 info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3GetRfc1766Info([In] uint locale, [In] ushort langId, out tagRFC1766INFO pRfc1766Info);

        /// <summary>
        /// The i multi language 3 is code page installable.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3IsCodePageInstallable([In] uint uiCodePage);

        /// <summary>
        /// The i multi language 3 is convertible.
        /// </summary>
        /// <param name="dwSrcEncoding">
        /// The dw src encoding.
        /// </param>
        /// <param name="dwDstEncoding">
        /// The dw dst encoding.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3IsConvertible([In] uint dwSrcEncoding, [In] uint dwDstEncoding);

        /// <summary>
        /// The i multi language 3 set mime db source.
        /// </summary>
        /// <param name="dwSource">
        /// The dw source.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3SetMimeDBSource([In] tagMIMECONTF dwSource);

        /// <summary>
        /// The i multi language 3 validate code page.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3ValidateCodePage([In] uint uiCodePage, [In] [ComAliasName("MultiLanguage.wireHWND")] ref _RemotableHandle hwnd);

        /// <summary>
        /// The i multi language 3 validate code page ex.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="dwfIODControl">
        /// The dwf iod control.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void IMultiLanguage3ValidateCodePageEx([In] uint uiCodePage, [In] [ComAliasName("MultiLanguage.wireHWND")] ref _RemotableHandle hwnd, [In] uint dwfIODControl);
    }
}