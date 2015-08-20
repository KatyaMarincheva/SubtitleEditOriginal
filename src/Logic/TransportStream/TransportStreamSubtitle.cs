// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportStreamSubtitle.cs" company="">
//   
// </copyright>
// <summary>
//   The transport stream subtitle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System.Drawing;

    using Nikse.SubtitleEdit.Logic.BluRaySup;

    /// <summary>
    /// The transport stream subtitle.
    /// </summary>
    public class TransportStreamSubtitle
    {
        /// <summary>
        /// The _bd sup.
        /// </summary>
        private BluRaySupParser.PcsData _bdSup;

        /// <summary>
        /// The _end milliseconds.
        /// </summary>
        private ulong _endMilliseconds;

        /// <summary>
        /// The _start milliseconds.
        /// </summary>
        private ulong _startMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportStreamSubtitle"/> class.
        /// </summary>
        /// <param name="bdSup">
        /// The bd sup.
        /// </param>
        /// <param name="startMilliseconds">
        /// The start milliseconds.
        /// </param>
        /// <param name="endMilliseconds">
        /// The end milliseconds.
        /// </param>
        public TransportStreamSubtitle(BluRaySupParser.PcsData bdSup, ulong startMilliseconds, ulong endMilliseconds)
        {
            this._bdSup = bdSup;
            this.StartMilliseconds = startMilliseconds;
            this.EndMilliseconds = endMilliseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportStreamSubtitle"/> class.
        /// </summary>
        /// <param name="bdSup">
        /// The bd sup.
        /// </param>
        /// <param name="startMilliseconds">
        /// The start milliseconds.
        /// </param>
        /// <param name="endMilliseconds">
        /// The end milliseconds.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        public TransportStreamSubtitle(BluRaySupParser.PcsData bdSup, ulong startMilliseconds, ulong endMilliseconds, ulong offset)
        {
            this._bdSup = bdSup;
            this.StartMilliseconds = startMilliseconds;
            this.EndMilliseconds = endMilliseconds;
            this.OffsetMilliseconds = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportStreamSubtitle"/> class.
        /// </summary>
        public TransportStreamSubtitle()
        {
        }

        /// <summary>
        /// Gets or sets the start milliseconds.
        /// </summary>
        public ulong StartMilliseconds
        {
            get
            {
                if (this._startMilliseconds < this.OffsetMilliseconds)
                {
                    return 0;
                }

                return this._startMilliseconds - this.OffsetMilliseconds;
            }

            set
            {
                this._startMilliseconds = value + this.OffsetMilliseconds;
            }
        }

        /// <summary>
        /// Gets or sets the end milliseconds.
        /// </summary>
        public ulong EndMilliseconds
        {
            get
            {
                if (this._endMilliseconds < this.OffsetMilliseconds)
                {
                    return 0;
                }

                return this._endMilliseconds - this.OffsetMilliseconds;
            }

            set
            {
                this._endMilliseconds = value + this.OffsetMilliseconds;
            }
        }

        /// <summary>
        /// Gets or sets the offset milliseconds.
        /// </summary>
        public ulong OffsetMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the pes.
        /// </summary>
        public DvbSubPes Pes { get; set; }

        /// <summary>
        /// Gets or sets the active image index.
        /// </summary>
        public int? ActiveImageIndex { get; set; }

        /// <summary>
        /// Gets a value indicating whether is blu ray sup.
        /// </summary>
        public bool IsBluRaySup
        {
            get
            {
                return this._bdSup != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is dvb sub.
        /// </summary>
        public bool IsDvbSub
        {
            get
            {
                return this.Pes != null;
            }
        }

        /// <summary>
        /// Gets the number of images.
        /// </summary>
        public int NumberOfImages
        {
            get
            {
                if (this.Pes != null)
                {
                    return this.Pes.ObjectDataList.Count;
                }
                else
                {
                    return this._bdSup.BitmapObjects.Count;
                }
            }
        }

        /// <summary>
        /// Gets full image if 'ActiveImageIndex' not set, otherwise only gets image by index
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetActiveImage()
        {
            if (this._bdSup != null)
            {
                return this._bdSup.GetBitmap();
            }

            if (this.ActiveImageIndex.HasValue && this.ActiveImageIndex >= 0 && this.ActiveImageIndex < this.Pes.ObjectDataList.Count)
            {
                return (Bitmap)this.Pes.GetImage(this.Pes.ObjectDataList[this.ActiveImageIndex.Value]).Clone();
            }

            return this.Pes.GetImageFull();
        }
    }
}