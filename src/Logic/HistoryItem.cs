// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryItem.cs" company="">
//   
// </copyright>
// <summary>
//   The history item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The history item.
    /// </summary>
    public class HistoryItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryItem"/> class.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="fileModified">
        /// The file modified.
        /// </param>
        /// <param name="subtitleFormatFriendlyName">
        /// The subtitle format friendly name.
        /// </param>
        /// <param name="originalSubtitle">
        /// The original subtitle.
        /// </param>
        /// <param name="originalSubtitleFileName">
        /// The original subtitle file name.
        /// </param>
        /// <param name="lineIndex">
        /// The line index.
        /// </param>
        /// <param name="linePosition">
        /// The line position.
        /// </param>
        /// <param name="linePositionAlternate">
        /// The line position alternate.
        /// </param>
        public HistoryItem(int index, Subtitle subtitle, string description, string fileName, DateTime fileModified, string subtitleFormatFriendlyName, Subtitle originalSubtitle, string originalSubtitleFileName, int lineIndex, int linePosition, int linePositionAlternate)
        {
            this.Index = index;
            this.Timestamp = DateTime.Now;
            this.Subtitle = new Subtitle(subtitle);
            this.Description = description;
            this.FileName = fileName;
            this.FileModified = fileModified;
            this.SubtitleFormatFriendlyName = subtitleFormatFriendlyName;
            this.OriginalSubtitle = new Subtitle(originalSubtitle);
            this.OriginalSubtitleFileName = originalSubtitleFileName;
            this.LineIndex = lineIndex;
            this.LinePosition = linePosition;
            this.LinePositionAlternate = linePositionAlternate;
            this.RedoLineIndex = -1;
            this.RedoLinePosition = -1;
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file modified.
        /// </summary>
        public DateTime FileModified { get; set; }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        public Subtitle Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the subtitle format friendly name.
        /// </summary>
        public string SubtitleFormatFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the original subtitle.
        /// </summary>
        public Subtitle OriginalSubtitle { get; set; }

        /// <summary>
        /// Gets or sets the original subtitle file name.
        /// </summary>
        public string OriginalSubtitleFileName { get; set; }

        /// <summary>
        /// Gets or sets the redo paragraphs.
        /// </summary>
        public List<Paragraph> RedoParagraphs { get; set; }

        /// <summary>
        /// Gets or sets the redo paragraphs alternate.
        /// </summary>
        public List<Paragraph> RedoParagraphsAlternate { get; set; }

        /// <summary>
        /// Gets or sets the redo line index.
        /// </summary>
        public int RedoLineIndex { get; set; }

        /// <summary>
        /// Gets or sets the redo line position.
        /// </summary>
        public int RedoLinePosition { get; set; }

        /// <summary>
        /// Gets or sets the redo line position alternate.
        /// </summary>
        public int RedoLinePositionAlternate { get; set; }

        /// <summary>
        /// Gets or sets the redo file name.
        /// </summary>
        public string RedoFileName { get; set; }

        /// <summary>
        /// Gets or sets the redo file modified.
        /// </summary>
        public DateTime RedoFileModified { get; set; }

        /// <summary>
        /// Gets or sets the redo original file name.
        /// </summary>
        public string RedoOriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the line index.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        /// Gets or sets the line position.
        /// </summary>
        public int LinePosition { get; set; }

        /// <summary>
        /// Gets or sets the line position alternate.
        /// </summary>
        public int LinePositionAlternate { get; set; }

        /// <summary>
        /// The to hhmmss.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToHHMMSS()
        {
            return string.Format("{0:00}:{1:00}:{2:00}", this.Timestamp.Hour, this.Timestamp.Minute, this.Timestamp.Second);
        }
    }
}