// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangFontLink2.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangFontLink2 interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 0108

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangFontLink2 interface.
    /// </summary>
    [ComImport]
    [ComConversionLoss]
    [InterfaceType(1)]
    [Guid("DCCFC162-2B38-11D2-B7EC-00C04F8F5D9A")]
    public interface IMLangFontLink2 : IMLangCodePages
    {
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
        void GetCharCodePages([In] ushort chSrc, out uint pdwCodePages);

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
        void GetStrCodePages([In] ref ushort pszSrc, [In] int cchSrc, [In] uint dwPriorityCodePages, out uint pdwCodePages, out int pcchCodePages);

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
        void CodePageToCodePages([In] uint uCodePage, out uint pdwCodePages);

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
        void CodePagesToCodePage([In] uint dwCodePages, [In] uint uDefaultCodePage, out uint puCodePage);

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
        void GetFontCodePages([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hFont, out uint pdwCodePages);

        /// <summary>
        /// The release font.
        /// </summary>
        /// <param name="hFont">
        /// The h font.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ReleaseFont([In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hFont);

        /// <summary>
        /// The reset font mapping.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ResetFontMapping();

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
        void MapFont([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] uint dwCodePages, [In] ushort chSrc, [Out] [ComAliasName("MultiLanguage.wireHFONT")] IntPtr pFont);

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
        void GetFontUnicodeRanges([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] [Out] ref uint puiRanges, out tagUNICODERANGE pUranges);

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
        void GetScriptFontInfo([In] byte sid, [In] uint dwFlags, [In] [Out] ref uint puiFonts, out tagSCRIPFONTINFO pScriptFont);

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
        void CodePageToScriptID([In] uint uiCodePage, out byte pSid);
    }
}

#pragma warning restore 0108