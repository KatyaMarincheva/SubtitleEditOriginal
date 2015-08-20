// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CMLangConvertCharset.cs" company="">
//   
// </copyright>
// <summary>
//   The CMLangConvertCharset interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The CMLangConvertCharset interface.
    /// </summary>
    [ComImport]
    [Guid("D66D6F98-CDAA-11D0-B822-00C04FC9B31F")]
    [CoClass(typeof(CMLangConvertCharsetClass))]
    public interface ICMLangConvertCharset : IMLangConvertCharset
    {
    }
}