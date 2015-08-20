// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CMultiLanguage.cs" company="">
//   
// </copyright>
// <summary>
//   The CMultiLanguage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The CMultiLanguage interface.
    /// </summary>
    [ComImport]
    [Guid("275C23E1-3747-11D0-9FEA-00AA003F8646")]
    [CoClass(typeof(CMultiLanguageClass))]
    public interface ICMultiLanguage : IMultiLanguage
    {
    }
}