// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangFontLink.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangFontLink interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 0108

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangFontLink interface.
    /// </summary>
    [ComImport]
    [InterfaceType(1)]
    [ComConversionLoss]
    [Guid("359F3441-BD4A-11D0-B188-00AA0038C969")]
    public interface IMLangFontLink : IMLangCodePages
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
        void MapFont([In] [ComAliasName("MultiLanguage.wireHDC")] ref _RemotableHandle hDC, [In] uint dwCodePages, [In] [ComAliasName("MultiLanguage.wireHFONT")] ref _RemotableHandle hSrcFont, [Out] [ComAliasName("MultiLanguage.wireHFONT")] IntPtr phDestFont);

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
    }
}