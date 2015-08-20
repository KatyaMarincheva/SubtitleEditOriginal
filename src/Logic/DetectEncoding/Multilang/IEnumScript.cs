// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumScript.cs" company="">
//   
// </copyright>
// <summary>
//   The EnumScript interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The EnumScript interface.
    /// </summary>
    [ComImport]
    [Guid("AE5F1430-388B-11D2-8380-00C04F8F5DA1")]
    [InterfaceType(1)]
    public interface IEnumScript
    {
        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="ppEnum">
        /// The pp enum.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumScript ppEnum);

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
        void Next([In] uint celt, out tagSCRIPTINFO rgelt, out uint pceltFetched);

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