// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubMergedPack.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub merged pack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;

    /// <summary>
    /// The vob sub merged pack.
    /// </summary>
    public class VobSubMergedPack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubMergedPack"/> class.
        /// </summary>
        /// <param name="subPictureData">
        /// The sub picture data.
        /// </param>
        /// <param name="presentationTimestamp">
        /// The presentation timestamp.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <param name="idxLine">
        /// The idx line.
        /// </param>
        public VobSubMergedPack(byte[] subPictureData, TimeSpan presentationTimestamp, int streamId, IdxParagraph idxLine)
        {
            this.SubPicture = new SubPicture(subPictureData);
            this.StartTime = presentationTimestamp;
            this.StreamId = streamId;
            this.IdxLine = idxLine;
        }

        /// <summary>
        /// Gets the sub picture.
        /// </summary>
        public SubPicture SubPicture { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public TimeSpan StartTime { get; private set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Gets the stream id.
        /// </summary>
        public int StreamId { get; private set; }

        /// <summary>
        /// Gets the idx line.
        /// </summary>
        public IdxParagraph IdxLine { get; private set; }
    }
}