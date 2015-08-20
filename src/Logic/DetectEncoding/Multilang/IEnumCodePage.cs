// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumCodePage.cs" company="">
//   
// </copyright>
// <summary>
//   The EnumCodePage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The EnumCodePage interface.
    /// </summary>
    [ComImport]
    [Guid("275C23E3-3747-11D0-9FEA-00AA003F8646")]
    [InterfaceType(1)]
    public interface IEnumCodePage
    {
        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="ppEnum">
        /// The pp enum.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumCodePage ppEnum);

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="celt">
        /// The celt.
        /// </param>
        /// <param name="rgelt">
        /// The rgelt.
        /// </param>
        /// <param name="pceltFetched">
        /// The pcelt fetched.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Next([In] uint celt, out tagMIMECPINFO rgelt, out uint pceltFetched);

        /// <summary>
        /// The reset.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Reset();

        /// <summary>
        /// The skip.
        /// </summary>
        /// <param name="celt">
        /// The celt.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Skip([In] uint celt);
    }
}