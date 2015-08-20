// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagDetectEncodingInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The detect encoding info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The detect encoding info.
    /// </summary>
    /// Thanks to jannewe for finding the fix!
    /// http://www.codeproject.com/KB/recipes/DetectEncoding.aspx?msg=3247475#xx3247475xx
    [StructLayout(LayoutKind.Sequential)]
    public struct DetectEncodingInfo
    {
        /// <summary>
        /// The n lang id.
        /// </summary>
        public uint nLangID;

        /// <summary>
        /// The n code page.
        /// </summary>
        public uint nCodePage;

        /// <summary>
        /// The n doc percent.
        /// </summary>
        public int nDocPercent;

        /// <summary>
        /// The n confidence.
        /// </summary>
        public int nConfidence;
    }
}