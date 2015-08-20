// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Paragraph.cs" company="">
//   
// </copyright>
// <summary>
//   The paragraph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic.Interfaces;

    /// <summary>
    /// The paragraph.
    /// </summary>
    public class Paragraph : IParagraph
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        public Paragraph()
        {
            this.StartTime = TimeCode.FromSeconds(0);
            this.EndTime = TimeCode.FromSeconds(0);
            this.Text = string.Empty;
            this.ID = this.GenerateId();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="endTime">
        /// The end time.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public Paragraph(TimeCode startTime, TimeCode endTime, string text)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Text = text;
            this.ID = this.GenerateId();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        public Paragraph(Paragraph paragraph)
        {
            this.Number = paragraph.Number;
            this.Text = paragraph.Text;
            this.StartTime = new TimeCode(paragraph.StartTime.TotalMilliseconds);
            this.EndTime = new TimeCode(paragraph.EndTime.TotalMilliseconds);
            this.StartFrame = paragraph.StartFrame;
            this.EndFrame = paragraph.EndFrame;
            this.Forced = paragraph.Forced;
            this.Extra = paragraph.Extra;
            this.IsComment = paragraph.IsComment;
            this.Actor = paragraph.Actor;
            this.Effect = paragraph.Effect;
            this.Layer = paragraph.Layer;
            this.ID = this.GenerateId(); // Do not reuse unique ID
            this.Language = paragraph.Language;
            this.Style = paragraph.Style;
            this.NewSection = paragraph.NewSection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        /// <param name="startFrame">
        /// The start frame.
        /// </param>
        /// <param name="endFrame">
        /// The end frame.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public Paragraph(int startFrame, int endFrame, string text)
        {
            this.StartTime = new TimeCode(0, 0, 0, 0);
            this.EndTime = new TimeCode(0, 0, 0, 0);
            this.StartFrame = startFrame;
            this.EndFrame = endFrame;
            this.Text = text;
            this.ID = this.GenerateId();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="startTotalMilliseconds">
        /// The start total milliseconds.
        /// </param>
        /// <param name="endTotalMilliseconds">
        /// The end total milliseconds.
        /// </param>
        public Paragraph(string text, double startTotalMilliseconds, double endTotalMilliseconds)
        {
            this.StartTime = new TimeCode(startTotalMilliseconds);
            this.EndTime = new TimeCode(endTotalMilliseconds);
            this.Text = text;
            this.ID = this.GenerateId();
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public TimeCode StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public TimeCode EndTime { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public TimeCode Duration
        {
            get
            {
                return new TimeCode(this.EndTime.TotalMilliseconds - this.StartTime.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Gets or sets the start frame.
        /// </summary>
        public int StartFrame { get; set; }

        /// <summary>
        /// Gets or sets the end frame.
        /// </summary>
        public int EndFrame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether forced.
        /// </summary>
        public bool Forced { get; set; }

        /// <summary>
        /// Gets or sets the extra.
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is comment.
        /// </summary>
        public bool IsComment { get; set; }

        /// <summary>
        /// Gets or sets the actor.
        /// </summary>
        public string Actor { get; set; }

        /// <summary>
        /// Gets or sets the effect.
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        public int Layer { get; set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether new section.
        /// </summary>
        public bool NewSection { get; set; }

        /// <summary>
        /// Gets the number of lines.
        /// </summary>
        public int NumberOfLines
        {
            get
            {
                return Utilities.GetNumberOfLines(this.Text);
            }
        }

        /// <summary>
        /// Gets the words per minute.
        /// </summary>
        public double WordsPerMinute
        {
            get
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    return 0;
                }

                int wordCount = HtmlUtil.RemoveHtmlTags(this.Text, true).Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '(', ')', '[', ']', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                return (60.0 / this.Duration.TotalSeconds) * wordCount;
            }
        }

        /// <summary>
        /// The generate id.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// The adjust.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <param name="adjust">
        /// The adjust.
        /// </param>
        public void Adjust(double factor, double adjust)
        {
            if (this.StartTime.IsMaxTime)
            {
                return;
            }

            double seconds = (this.StartTime.TimeSpan.TotalSeconds * factor) + adjust;
            this.StartTime.TimeSpan = TimeSpan.FromSeconds(seconds);

            seconds = (this.EndTime.TimeSpan.TotalSeconds * factor) + adjust;
            this.EndTime.TimeSpan = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// The calculate frame numbers from time codes.
        /// </summary>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        public void CalculateFrameNumbersFromTimeCodes(double frameRate)
        {
            this.StartFrame = (int)Math.Round(this.StartTime.TotalMilliseconds / TimeCode.BaseUnit * frameRate);
            this.EndFrame = (int)Math.Round(this.EndTime.TotalMilliseconds / TimeCode.BaseUnit * frameRate);
        }

        /// <summary>
        /// The calculate time codes from frame numbers.
        /// </summary>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        public void CalculateTimeCodesFromFrameNumbers(double frameRate)
        {
            this.StartTime.TotalMilliseconds = this.StartFrame * (TimeCode.BaseUnit / frameRate);
            this.EndTime.TotalMilliseconds = this.EndFrame * (TimeCode.BaseUnit / frameRate);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.StartTime + " --> " + this.EndTime + " " + this.Text;
        }
    }
}