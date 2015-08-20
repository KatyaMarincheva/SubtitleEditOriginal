// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParagraphEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The paragraph event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The paragraph event args.
    /// </summary>
    public class ParagraphEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphEventArgs"/> class.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        public ParagraphEventArgs(Paragraph p)
        {
            this.Paragraph = p;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphEventArgs"/> class.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        public ParagraphEventArgs(double seconds, Paragraph p)
        {
            this.Seconds = seconds;
            this.Paragraph = p;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphEventArgs"/> class.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b)
        {
            this.Seconds = seconds;
            this.Paragraph = p;
            this.BeforeParagraph = b;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphEventArgs"/> class.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <param name="mouseDownParagraphType">
        /// The mouse down paragraph type.
        /// </param>
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b, MouseDownParagraphType mouseDownParagraphType)
        {
            this.Seconds = seconds;
            this.Paragraph = p;
            this.BeforeParagraph = b;
            this.MouseDownParagraphType = mouseDownParagraphType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphEventArgs"/> class.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <param name="mouseDownParagraphType">
        /// The mouse down paragraph type.
        /// </param>
        /// <param name="movePreviousOrNext">
        /// The move previous or next.
        /// </param>
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b, MouseDownParagraphType mouseDownParagraphType, bool movePreviousOrNext)
        {
            this.Seconds = seconds;
            this.Paragraph = p;
            this.BeforeParagraph = b;
            this.MouseDownParagraphType = mouseDownParagraphType;
            this.MovePreviousOrNext = movePreviousOrNext;
        }

        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <summary>
        /// Gets the seconds.
        /// </summary>
        public double Seconds { get; private set; }

        /// <summary>
        /// Gets or sets the before paragraph.
        /// </summary>
        public Paragraph BeforeParagraph { get; set; }

        /// <summary>
        /// Gets or sets the mouse down paragraph type.
        /// </summary>
        public MouseDownParagraphType MouseDownParagraphType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether move previous or next.
        /// </summary>
        public bool MovePreviousOrNext { get; set; }
    }
}