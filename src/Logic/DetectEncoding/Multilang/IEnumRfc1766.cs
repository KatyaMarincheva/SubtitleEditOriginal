// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumRfc1766.cs" company="">
//   
// </copyright>
// <summary>
//   The EnumRfc1766 interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The EnumRfc1766 interface.
    /// </summary>
    [ComImport]
    [Guid("3DC39D1D-C030-11D0-B81B-00C04FC9B31F")]
    [InterfaceType(1)]
    public interface IEnumRfc1766
    {
        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="ppEnum">
        /// The pp enum.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumRfc1766 ppEnum);

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
        void Next([In] uint celt, out tagRFC1766INFO rgelt, out uint pceltFetched);

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