// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CMLangConvertCharsetClass.cs" company="">
//   
// </copyright>
// <summary>
//   The cm lang convert charset class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The cm lang convert charset class.
    /// </summary>
    [ComImport]
    [ClassInterface((short)0)]
    [TypeLibType(2)]
    [Guid("D66D6F99-CDAA-11D0-B822-00C04FC9B31F")]
    public class CMLangConvertCharsetClass : ICMLangConvertCharset
    {
        /// <summary>
        /// The do conversion.
        /// </summary>
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
        public virtual extern void DoConversion([In] ref byte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref byte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The do conversion from unicode.
        /// </summary>
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
        public virtual extern void DoConversionFromUnicode([In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The do conversion to unicode.
        /// </summary>
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
        public virtual extern void DoConversionToUnicode([In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize);

        /// <summary>
        /// The get destination code page.
        /// </summary>
        /// <param name="puiDstCodePage">
        /// The pui dst code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetDestinationCodePage(out uint puiDstCodePage);

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="pdwProperty">
        /// The pdw property.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetProperty(out uint pdwProperty);

        /// <summary>
        /// The get source code page.
        /// </summary>
        /// <param name="puiSrcCodePage">
        /// The pui src code page.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetSourceCodePage(out uint puiSrcCodePage);

        /// <summary>
        /// The initialize.
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
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Initialize([In] uint uiSrcCodePage, [In] uint uiDstCodePage, [In] uint dwProperty);
    }
}