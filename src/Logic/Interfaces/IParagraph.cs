// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParagraph.cs" company="">
//   
// </copyright>
// <summary>
//   The Paragraph interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Interfaces
{
    /// <summary>
    /// The Paragraph interface.
    /// </summary>
    public interface IParagraph
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        int Number { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        TimeCode StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        TimeCode EndTime { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        TimeCode Duration { get; }

        /// <summary>
        /// Gets or sets the start frame.
        /// </summary>
        int StartFrame { get; set; }

        /// <summary>
        /// Gets or sets the end frame.
        /// </summary>
        int EndFrame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether forced.
        /// </summary>
        bool Forced { get; set; }

        /// <summary>
        /// Gets or sets the extra.
        /// </summary>
        string Extra { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is comment.
        /// </summary>
        bool IsComment { get; set; }

        /// <summary>
        /// Gets or sets the actor.
        /// </summary>
        string Actor { get; set; }

        /// <summary>
        /// Gets or sets the effect.
        /// </summary>
        string Effect { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        int Layer { get; set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        string Style { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether new section.
        /// </summary>
        bool NewSection { get; set; }

        /// <summary>
        /// Gets the number of lines.
        /// </summary>
        int NumberOfLines { get; }

        /// <summary>
        /// Gets the words per minute.
        /// </summary>
        double WordsPerMinute { get; }

        /// <summary>
        /// The generate id.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GenerateId();

        /// <summary>
        /// The adjust.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <param name="adjust">
        /// The adjust.
        /// </param>
        void Adjust(double factor, double adjust);

        /// <summary>
        /// The calculate frame numbers from time codes.
        /// </summary>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        void CalculateFrameNumbersFromTimeCodes(double frameRate);

        /// <summary>
        /// The calculate time codes from frame numbers.
        /// </summary>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        void CalculateTimeCodesFromFrameNumbers(double frameRate);
    }
}