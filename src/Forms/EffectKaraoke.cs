// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EffectKaraoke.cs" company="">
//   
// </copyright>
// <summary>
//   The effect karaoke.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The effect karaoke.
    /// </summary>
    public sealed partial class EffectKaraoke : Form
    {
        /// <summary>
        /// The _animation.
        /// </summary>
        private List<Paragraph> _animation;

        /// <summary>
        /// The _color list.
        /// </summary>
        private List<ColorEntry> _colorList;

        /// <summary>
        /// The _font list.
        /// </summary>
        private List<FontEntry> _fontList;

        /// <summary>
        /// The _paragraph.
        /// </summary>
        private Paragraph _paragraph;

        /// <summary>
        /// The _timer count.
        /// </summary>
        private int _timerCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectKaraoke"/> class.
        /// </summary>
        public EffectKaraoke()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.EffectKaraoke.Title;
            this.labelChooseColor.Text = Configuration.Settings.Language.EffectKaraoke.ChooseColor;
            this.labelTM.Text = Configuration.Settings.Language.EffectKaraoke.TotalMilliseconds;
            this.labelEndDelay.Text = Configuration.Settings.Language.EffectKaraoke.EndDelayInMilliseconds;
            this.buttonPreview.Text = Configuration.Settings.Language.General.Preview;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The form effectkaraoke_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormEffectkaraoke_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        internal void Initialize(Paragraph paragraph)
        {
            this._paragraph = paragraph;
            this._animation = new List<Paragraph>();

            this.colorDialog1.Color = Color.Red;
            this.labelColor.Text = Utilities.ColorToHex(this.colorDialog1.Color);
            this.panelColor.BackColor = this.colorDialog1.Color;

            this.AddToPreview(this.richTextBoxPreview, paragraph.Text);
            this.RefreshPreview();
            this.labelTotalMilliseconds.Text = string.Format("{0:#,##0.000}", paragraph.Duration.TotalMilliseconds / 1000);
            this.numericUpDownDelay.Maximum = (int)((paragraph.Duration.TotalMilliseconds - 500) / 1000);
            this.numericUpDownDelay.Minimum = 0;

            this.numericUpDownDelay.Left = this.labelEndDelay.Left + this.labelEndDelay.Width + 5;
        }

        /// <summary>
        /// The add to preview.
        /// </summary>
        /// <param name="rtb">
        /// The rtb.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void AddToPreview(RichTextBox rtb, string text)
        {
            this.richTextBoxPreview.ForeColor = Color.White;
            this._colorList = new List<ColorEntry>();
            this._fontList = new List<FontEntry>();

            int bold = 0;
            int underline = 0;
            int italic = 0;
            var fontColors = new Stack<string>();
            string currentColor = string.Empty;

            var sb = new StringBuilder();
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '<')
                {
                    this.AddTextToRichTextBox(rtb, bold > 0, italic > 0, underline > 0, currentColor, sb.ToString());
                    sb.Clear();
                    string tag = GetTag(text.Substring(i).ToLower());
                    if (i + 1 < text.Length && text[i + 1] == '/')
                    {
                        if (tag == "</i>" && italic > 0)
                        {
                            italic--;
                        }
                        else if (tag == "</b>" && bold > 0)
                        {
                            bold--;
                        }
                        else if (tag == "<u>" && underline > 0)
                        {
                            underline--;
                        }
                        else if (tag == "</font>")
                        {
                            currentColor = fontColors.Count > 0 ? fontColors.Pop() : string.Empty;
                        }
                    }
                    else
                    {
                        if (tag == "<i>")
                        {
                            italic++;
                        }
                        else if (tag == "<b>")
                        {
                            bold++;
                        }
                        else if (tag == "<u>")
                        {
                            underline++;
                        }
                        else if (tag.StartsWith("<font "))
                        {
                            const string colorTag = " color=";
                            if (tag.Contains(colorTag))
                            {
                                string tempColor = string.Empty;
                                var start = tag.IndexOf(colorTag, StringComparison.Ordinal);
                                int j = start + colorTag.Length;
                                if (@"""'".Contains(tag[j]))
                                {
                                    j++;
                                }

                                while (j < tag.Length && (@"#" + Utilities.LowercaseLettersWithNumbers).Contains(tag[j]))
                                {
                                    tempColor += tag[j];
                                    j++;
                                }

                                if (!string.IsNullOrEmpty(currentColor))
                                {
                                    fontColors.Push(currentColor);
                                }

                                currentColor = tempColor;
                            }
                        }
                    }

                    i += tag.Length;
                }
                else
                {
                    sb.Append(text[i]);
                    i++;
                }
            }

            if (sb.Length > 0)
            {
                this.AddTextToRichTextBox(rtb, bold > 0, italic > 0, underline > 0, currentColor, sb.ToString());
            }

            foreach (var fontEntry in this._fontList)
            {
                rtb.SelectionStart = fontEntry.Start;
                rtb.SelectionLength = fontEntry.Length;
                rtb.SelectionFont = fontEntry.Font;
                rtb.DeselectAll();
            }

            foreach (var colorEntry in this._colorList)
            {
                rtb.SelectionStart = colorEntry.Start;
                rtb.SelectionLength = colorEntry.Length;
                rtb.SelectionColor = colorEntry.Color;
                rtb.DeselectAll();
            }
        }

        /// <summary>
        /// The add text to rich text box.
        /// </summary>
        /// <param name="rtb">
        /// The rtb.
        /// </param>
        /// <param name="bold">
        /// The bold.
        /// </param>
        /// <param name="italic">
        /// The italic.
        /// </param>
        /// <param name="underline">
        /// The underline.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void AddTextToRichTextBox(RichTextBox rtb, bool bold, bool italic, bool underline, string color, string text)
        {
            if (text.Length > 0)
            {
                int length = rtb.Text.Length;
                this.richTextBoxPreview.Text += text;

                this._colorList.Add(new ColorEntry { Start = length, Length = text.Length, Color = string.IsNullOrWhiteSpace(color) ? Color.White : ColorTranslator.FromHtml(color) });

                var fontStyle = new FontStyle();
                if (underline)
                {
                    fontStyle = fontStyle | FontStyle.Underline;
                }

                if (italic)
                {
                    fontStyle = fontStyle | FontStyle.Italic;
                }

                if (bold)
                {
                    fontStyle = fontStyle | FontStyle.Bold;
                }

                this._fontList.Add(new FontEntry { Start = length, Length = text.Length, Font = new Font(rtb.Font.FontFamily, rtb.Font.Size, fontStyle) });
            }
        }

        /// <summary>
        /// The get tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetTag(string text)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                sb.Append(text[i]);
                if (text[i] == '>')
                {
                    return sb.ToString();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The clear preview.
        /// </summary>
        private void ClearPreview()
        {
            this.richTextBoxPreview.Text = string.Empty;
        }

        /// <summary>
        /// The refresh preview.
        /// </summary>
        private void RefreshPreview()
        {
            this.richTextBoxPreview.SelectAll();
            this.richTextBoxPreview.SelectionAlignment = HorizontalAlignment.Center;
            this.richTextBoxPreview.Refresh();
        }

        /// <summary>
        /// The button choose color click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonChooseColorClick(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.labelColor.Text = Utilities.ColorToHex(this.colorDialog1.Color);
                this.panelColor.BackColor = this.colorDialog1.Color;
            }
        }

        /// <summary>
        /// The button preview click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonPreviewClick(object sender, EventArgs e)
        {
            this.MakeAnimation();
            this.PlayAnimation();
        }

        /// <summary>
        /// The play animation.
        /// </summary>
        private void PlayAnimation()
        {
            this._colorList = new List<ColorEntry>();
            this._fontList = new List<FontEntry>();
            this._timerCount = (int)this._paragraph.StartTime.TotalMilliseconds;
            this.timer1.Start();
        }

        /// <summary>
        /// The calculate step length.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double CalculateStepLength(string text, double duration)
        {
            text = HtmlUtil.RemoveHtmlTags(text);
            return duration / text.Length;
        }

        /// <summary>
        /// The make animation.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Paragraph> MakeAnimation(Paragraph paragraph)
        {
            this._paragraph = paragraph;
            this.MakeAnimation();
            return this._animation;
        }

        /// <summary>
        /// The make animation.
        /// </summary>
        private void MakeAnimation()
        {
            this._animation = new List<Paragraph>();
            double duration = this._paragraph.Duration.TotalMilliseconds - ((double)this.numericUpDownDelay.Value * TimeCode.BaseUnit);
            double stepsLength = CalculateStepLength(this._paragraph.Text, duration);

            double startMilliseconds;
            double endMilliseconds;
            TimeCode start;
            TimeCode end;
            int index = 0;
            string text = string.Empty;
            bool tagOn = false;
            string tag = string.Empty;
            int i = 0;
            string startFontTag = string.Format("<font color=\"{0}\">", Utilities.ColorToHex(this.panelColor.BackColor));
            const string endFontTag = "</font>";
            string afterCurrentTag = string.Empty;
            string beforeEndTag = string.Empty;
            string alignment = string.Empty;
            while (i < this._paragraph.Text.Length)
            {
                var c = this._paragraph.Text[i];
                if (i == 0 && this._paragraph.Text.StartsWith("{\\", StringComparison.Ordinal) && this._paragraph.Text.IndexOf('}') > 2)
                {
                    int idx = this._paragraph.Text.IndexOf('}');
                    alignment = this._paragraph.Text.Substring(0, idx + 1);
                    i = idx;
                }
                else if (tagOn)
                {
                    tag += c;
                    if (this._paragraph.Text[i] == '>')
                    {
                        tagOn = false;
                        if (tag.StartsWith("<font ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            afterCurrentTag = tag;
                            tag = string.Empty;
                        }
                        else if (tag == "<i>")
                        {
                            afterCurrentTag = "<i>";
                            beforeEndTag = "</i>";
                        }
                        else if (tag == "<b>")
                        {
                            afterCurrentTag = "<b>";
                            beforeEndTag = "</b>";
                        }
                        else if (tag == "<u>")
                        {
                            afterCurrentTag = "<u>";
                            beforeEndTag = "</u>";
                        }
                    }
                }
                else if (this._paragraph.Text[i] == '<')
                {
                    afterCurrentTag = string.Empty;
                    tagOn = true;
                    tag += c;
                    beforeEndTag = string.Empty;
                }
                else
                {
                    text += tag + c;
                    tag = string.Empty;
                    string afterText = string.Empty;
                    if (i + 1 < this._paragraph.Text.Length)
                    {
                        afterText = this._paragraph.Text.Substring(i + 1);
                    }

                    if (string.Compare(afterText, endFontTag, StringComparison.InvariantCultureIgnoreCase) == 0 && afterCurrentTag.StartsWith("<font", StringComparison.InvariantCultureIgnoreCase))
                    {
                        afterText = string.Empty;
                        afterCurrentTag = string.Empty;
                    }

                    string tempText = alignment + startFontTag + text + beforeEndTag + endFontTag + afterCurrentTag + afterText;
                    tempText = tempText.Replace("<i></i>", string.Empty);
                    tempText = tempText.Replace("<u></u>", string.Empty);
                    tempText = tempText.Replace("<b></b>", string.Empty);

                    startMilliseconds = index * stepsLength;
                    startMilliseconds += this._paragraph.StartTime.TotalMilliseconds;
                    endMilliseconds = ((index + 1) * stepsLength) - 1;
                    endMilliseconds += this._paragraph.StartTime.TotalMilliseconds;
                    start = new TimeCode(startMilliseconds);
                    end = new TimeCode(endMilliseconds);
                    this._animation.Add(new Paragraph(start, end, tempText));
                    index++;
                }

                i++;
            }

            if (this.numericUpDownDelay.Value > 0)
            {
                startMilliseconds = index * stepsLength;
                startMilliseconds += this._paragraph.StartTime.TotalMilliseconds;
                endMilliseconds = this._paragraph.EndTime.TotalMilliseconds;
                start = new TimeCode(startMilliseconds);
                end = new TimeCode(endMilliseconds);
                this._animation.Add(new Paragraph(start, end, startFontTag + this._paragraph.Text + endFontTag));
            }
            else if (this._animation.Count > 0)
            {
                this._animation[this._animation.Count - 1].EndTime.TotalMilliseconds = this._paragraph.EndTime.TotalMilliseconds;
            }
        }

        /// <summary>
        /// The timer 1 tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            this._timerCount += this.timer1.Interval;

            string s = GetText(this._timerCount, this._animation);
            this.ClearPreview();
            this.AddToPreview(this.richTextBoxPreview, s);
            this.RefreshPreview();

            if (this._timerCount > this._paragraph.EndTime.TotalMilliseconds)
            {
                this.timer1.Stop();
                System.Threading.Thread.Sleep(200);
                this.ClearPreview();
                this.AddToPreview(this.richTextBoxPreview, this._paragraph.Text);
            }
        }

        /// <summary>
        /// The get text.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="animation">
        /// The animation.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetText(int milliseconds, IEnumerable<Paragraph> animation)
        {
            foreach (Paragraph p in animation)
            {
                if (p.StartTime.TotalMilliseconds <= milliseconds && p.EndTime.TotalMilliseconds >= milliseconds)
                {
                    return p.Text;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// The button ok click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOkClick(object sender, EventArgs e)
        {
            this.MakeAnimation();
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The color entry.
        /// </summary>
        internal class ColorEntry
        {
            /// <summary>
            /// Gets or sets the start.
            /// </summary>
            public int Start { get; set; }

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// Gets or sets the color.
            /// </summary>
            public Color Color { get; set; }
        }

        /// <summary>
        /// The font entry.
        /// </summary>
        internal class FontEntry
        {
            /// <summary>
            /// Gets or sets the start.
            /// </summary>
            public int Start { get; set; }

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// Gets or sets the font.
            /// </summary>
            public Font Font { get; set; }
        }
    }
}