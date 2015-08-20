// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangLineBreakConsole.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangLineBreakConsole interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangLineBreakConsole interface.
    /// </summary>
    [ComImport]
    [InterfaceType(1)]
    [Guid("F5BE2EE1-BFD7-11D0-B188-00AA0038C969")]
    public interface IMLangLineBreakConsole
    {
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
        void BreakLineML([In] [MarshalAs(UnmanagedType.Interface)] ICMLangString pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen, [In] int cMinColumns, [In] int cMaxColumns, out int plLineLen, out int plSkipLen);

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
        void BreakLineW([In] uint locale, [In] ref ushort pszSrc, [In] int cchSrc, [In] int cMaxColumns, out int pcchLine, out int pcchSkip);

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
        void BreakLineA([In] uint locale, [In] uint uCodePage, [In] ref sbyte pszSrc, [In] int cchSrc, [In] int cMaxColumns, out int pcchLine, out int pcchSkip);
    }
}