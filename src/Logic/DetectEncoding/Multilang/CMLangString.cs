// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CMLangString.cs" company="">
//   
// </copyright>
// <summary>
//   The CMLangString interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The CMLangString interface.
    /// </summary>
    [ComImport]
    [CoClass(typeof(CMLangStringClass))]
    [Guid("C04D65CE-B70D-11D0-B188-00AA0038C969")]
    public interface ICMLangString : IMLangString
    {
    }
}