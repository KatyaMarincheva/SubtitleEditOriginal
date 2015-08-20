// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMLangString.cs" company="">
//   
// </copyright>
// <summary>
//   The MLangString interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The MLangString interface.
    /// </summary>
    [ComImport]
    [Guid("C04D65CE-B70D-11D0-B188-00AA0038C969")]
    [InterfaceType(1)]
    public interface IMLangString
    {
        /// <summary>
        /// The sync.
        /// </summary>
        /// <param name="fNoAccess">
        /// The f no access.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Sync([In] int fNoAccess);

        /// <summary>
        /// The get length.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetLength();

        /// <summary>
        /// The set ml str.
        /// </summary>
        /// <param name="lDestPos">
        /// The l dest pos.
        /// </param>
        /// <param name="lDestLen">
        /// The l dest len.
        /// </param>
        /// <param name="pSrcMLStr">
        /// The p src ml str.
        /// </param>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetMLStr([In] int lDestPos, [In] int lDestLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pSrcMLStr, [In] int lSrcPos, [In] int lSrcLen);

        /// <summary>
        /// The get ml str.
        /// </summary>
        /// <param name="lSrcPos">
        /// The l src pos.
        /// </param>
        /// <param name="lSrcLen">
        /// The l src len.
        /// </param>
        /// <param name="pUnkOuter">
        /// The p unk outer.
        /// </param>
        /// <param name="dwClsContext">
        /// The dw cls context.
        /// </param>
        /// <param name="piid">
        /// The piid.
        /// </param>
        /// <param name="ppDestMLStr">
        /// The pp dest ml str.
        /// </param>
        /// <param name="plDestPos">
        /// The pl dest pos.
        /// </param>
        /// <param name="plDestLen">
        /// The pl dest len.
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetMLStr([In] int lSrcPos, [In] int lSrcLen, [In] [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] uint dwClsContext, [In] ref Guid piid, [MarshalAs(UnmanagedType.IUnknown)] out object ppDestMLStr, out int plDestPos, out int plDestLen);
    }
}