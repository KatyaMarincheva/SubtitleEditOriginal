// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMultiLanguage3.cs" company="">
//   
// </copyright>
// <summary>
//   The MultiLanguage3 interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 0108

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MultiLanguage3 interface.
    /// </summary>
    [ComImport]
    [InterfaceType(1)]
    [Guid("4E5868AB-B157-4623-9ACC-6A1D9CAEBE04")]
    public interface IMultiLanguage3 : IMultiLanguage2
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
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="pCodePageInfo">
        /// The p code page info.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCodePageInfo([In] uint uiCodePage, [In] ushort langId, out tagMIMECPINFO pCodePageInfo);

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
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumCodePage">
        /// The pp enum code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumCodePages([In] uint grfFlags, [In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumCodePage ppEnumCodePage);

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
        void GetCharsetInfo([In] [MarshalAs(UnmanagedType.BStr)] string Charset, out tagMIMECSETINFO pCharsetInfo);

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
        /// <param name="langId">
        /// The lang id.
        /// </param>
        /// <param name="ppEnumRfc1766">
        /// The pp enum rfc 1766.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumRfc1766([In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumRfc1766 ppEnumRfc1766);

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
        void GetRfc1766Info([In] uint locale, [In] ushort langId, out tagRFC1766INFO pRfc1766Info);

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
        void ConvertStringInIStream([In] [Out] ref uint pdwMode, [In] uint dwFlag, [In] ref ushort lpFallBack, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmIn, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmOut);

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
        void ConvertStringToUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

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
        void ConvertStringFromUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

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
        void DetectCodepageInIStream([In] MLDETECTCP flags, [In] uint dwPrefWinCodePage, [In] [MarshalAs(UnmanagedType.Interface)] IStream pstmIn, [In] [Out] ref DetectEncodingInfo lpEncoding, [In] [Out] ref int pnScores);

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
        void DetectInputCodepage([In] MLDETECTCP flags, [In] uint dwPrefWinCodePage, [In] ref byte pSrcStr, [In] [Out] ref int pcSrcSize, [In] [Out] ref DetectEncodingInfo lpEncoding, [In] [Out] ref int pnScores);

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
        void ValidateCodePage([In] uint uiCodePage, [In] [ComAliasName("MultiLanguage.wireHWND")] ref _RemotableHandle hwnd);

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
        void GetCodePageDescription([In] uint uiCodePage, [In] uint lcid, [In] [Out] [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, [In] int cchWideChar);

        /// <summary>
        /// The is code page installable.
        /// </summary>
        /// <param name="uiCodePage">
        /// The ui code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void IsCodePageInstallable([In] uint uiCodePage);

        /// <summary>
        /// The set mime db source.
        /// </summary>
        /// <param name="dwSource">
        /// The dw source.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetMimeDBSource([In] tagMIMECONTF dwSource);

        /// <summary>
        /// The get number of scripts.
        /// </summary>
        /// <param name="pnScripts">
        /// The pn scripts.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetNumberOfScripts(out uint pnScripts);

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
        void EnumScripts([In] uint dwFlags, [In] ushort langId, [MarshalAs(UnmanagedType.Interface)] out IEnumScript ppEnumScript);

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
        void ValidateCodePageEx([In] uint uiCodePage, [In] [ComAliasName("MultiLanguage.wireHWND")] ref _RemotableHandle hwnd, [In] uint dwfIODControl);

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
        void DetectOutboundCodePage([In] MLCPF dwFlags, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, [In] uint cchWideChar, [In] IntPtr puiPreferredCodePages, [In] uint nPreferredCodePages, [In] IntPtr puiDetectedCodePages, [In] [Out] ref uint pnDetectedCodePages, [In] ref ushort lpSpecialChar);

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
        void DetectOutboundCodePageInIStream([In] uint dwFlags, [In] [MarshalAs(UnmanagedType.Interface)] IStream pStrIn, [In] ref uint puiPreferredCodePages, [In] uint nPreferredCodePages, [In] ref uint puiDetectedCodePages, [In] [Out] ref uint pnDetectedCodePages, [In] ref ushort lpSpecialChar);
    }
}

#pragma warning restore 0108