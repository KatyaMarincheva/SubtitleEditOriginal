// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="Core.cs">
//   
// </copyright>
// <summary>
//   The core.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.BluRaySup
{
    /// <summary>
    /// The core.
    /// </summary>
    public static class Core
    {
        /** Frames per seconds for 24p (23.976) */

        /// <summary>
        /// The fps 24 p.
        /// </summary>
        public const double Fps24P = 24000.0 / 1001;

        /** Frames per seconds for wrong 24P (23.975) */

        /// <summary>
        /// The fps 23975.
        /// </summary>
        public const double Fps23975 = 23.975;

        /** Frames per seconds for 24Hz (24.0) */

        /// <summary>
        /// The fps 24 hz.
        /// </summary>
        public const double Fps24Hz = 24.0;

        /** Frames per seconds for PAL progressive (25.0) */

        /// <summary>
        /// The fps pal.
        /// </summary>
        public const double FpsPal = 25.0;

        /** Frames per seconds for NTSC progressive (29.97) */

        /// <summary>
        /// The fps ntsc.
        /// </summary>
        public const double FpsNtsc = 30000.0 / 1001;

        /** Frames per seconds for PAL interlaced (50.0) */

        /// <summary>
        /// The fps pal i.
        /// </summary>
        public const double FpsPalI = 50.0;

        /** Frames per seconds for NTSC interlaced (59.94) */

        /// <summary>
        /// The fps ntsc i.
        /// </summary>
        public const double FpsNtscI = 60000.0 / 1001;

        /// <summary>
        /// The fps trg.
        /// </summary>
        public const double FpsTrg = FpsPal;

        /** Use BT.601 color model instead of BT.709 */

        /// <summary>
        /// The use bt 601.
        /// </summary>
        private const bool UseBt601 = false;

        /** Flag that defines whether to swap Cr/Cb components when loading a SUP */

        /// <summary>
        /// The swap cr cb.
        /// </summary>
        private const bool SwapCrCb = false;

        /** Alpha threshold for cropping */

        /// <summary>
        /// The alpha crop.
        /// </summary>
        private const int AlphaCrop = 14;

        /** Two equal captions are merged of they are closer than 200ms (0.2*90000 = 18000) */

        /// <summary>
        /// The merge pt sdiff.
        /// </summary>
        private const int MergePtSdiff = 18000;

        /// <summary>
        /// Gets or sets the crop ofs y.
        /// </summary>
        public static int CropOfsY { get; set; }

        /**
         * Get maximum time difference for merging captions.
         * @return Maximum time difference for merging captions
         */

        /// <summary>
        /// The get merge pt sdiff.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetMergePtSdiff()
        {
            return MergePtSdiff;
        }

        /**
        * Get: use of BT.601 color model instead of BT.709.
        * @return True if BT.601 is used
        */

        /// <summary>
        /// The uses bt 601.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool UsesBt601()
        {
            return UseBt601;
        }

        /**
         * Get flag that defines whether to swap Cr/Cb components when loading a SUP.
         * @return True: swap cr/cb
         */

        /// <summary>
        /// The get swap cr cb.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetSwapCrCb()
        {
            return SwapCrCb;
        }

        /**
         * Get alpha threshold for cropping.
         * @return Alpha threshold for cropping
         */

        /// <summary>
        /// The get alpha crop.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetAlphaCrop()
        {
            return AlphaCrop;
        }
    }
}