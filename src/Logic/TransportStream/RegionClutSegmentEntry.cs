// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionClutSegmentEntry.cs" company="">
//   
// </copyright>
// <summary>
//   The region clut segment entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System.Drawing;

    /// <summary>
    /// The region clut segment entry.
    /// </summary>
    public class RegionClutSegmentEntry
    {
        /// <summary>
        /// Gets or sets the clut entry id.
        /// </summary>
        public int ClutEntryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether clut entry 2 bit clut entry flag.
        /// </summary>
        public bool ClutEntry2BitClutEntryFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether clut entry 4 bit clut entry flag.
        /// </summary>
        public bool ClutEntry4BitClutEntryFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether clut entry 8 bit clut entry flag.
        /// </summary>
        public bool ClutEntry8BitClutEntryFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether full range flag.
        /// </summary>
        public bool FullRangeFlag { get; set; }

        /// <summary>
        /// Gets or sets the clut entry y.
        /// </summary>
        public int ClutEntryY { get; set; }

        /// <summary>
        /// Gets or sets the clut entry cr.
        /// </summary>
        public int ClutEntryCr { get; set; }

        /// <summary>
        /// Gets or sets the clut entry cb.
        /// </summary>
        public int ClutEntryCb { get; set; }

        /// <summary>
        /// Gets or sets the clut entry t.
        /// </summary>
        public int ClutEntryT { get; set; }

        /// <summary>
        /// The bound byte range.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int BoundByteRange(int i)
        {
            if (i < byte.MinValue)
            {
                return byte.MinValue;
            }

            if (i > byte.MaxValue)
            {
                return byte.MaxValue;
            }

            return i;
        }

        /// <summary>
        /// The get color.
        /// </summary>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetColor()
        {
            double y, cr, cb;
            if (this.FullRangeFlag)
            {
                y = this.ClutEntryY;
                cr = this.ClutEntryCr;
                cb = this.ClutEntryCb;
            }
            else
            {
                y = this.ClutEntryY * 255 / 63.0;
                cr = this.ClutEntryCr * 255 / 15.0;
                cb = this.ClutEntryCb * 255 / 15.0;
            }

            // Calculate rgb - based on Project X
            int r = (int)(y + (1.402f * (cr - 128)));
            int g = (int)(y - (0.34414 * (cb - 128)) - (0.71414 * (cr - 128)));
            int b = (int)(y + (1.722 * (cb - 128)));

            int t = byte.MaxValue - BoundByteRange(this.ClutEntryT);
            r = BoundByteRange(r);
            g = BoundByteRange(g);
            b = BoundByteRange(b);

            if (y < 0.1)
            {
                // full transparency
                t = 0;
            }

            return Color.FromArgb(t, r, g, b);
        }
    }
}