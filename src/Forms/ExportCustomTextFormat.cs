// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportCustomTextFormat.cs" company="">
//   
// </copyright>
// <summary>
//   The export custom text format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The export custom text format.
    /// </summary>
    public partial class ExportCustomTextFormat : Form
    {
        /// <summary>
        /// The english do no modify.
        /// </summary>
        public const string EnglishDoNoModify = "[Do not modify]";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCustomTextFormat"/> class.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        public ExportCustomTextFormat(string format)
        {
            this.InitializeComponent();
            var l = Configuration.Settings.Language.ExportCustomTextFormat;
            this.comboBoxNewLine.Items.Clear();
            this.comboBoxNewLine.Items.Add(l.DoNotModify);
            this.comboBoxNewLine.Items.Add("||");
            this.comboBoxNewLine.Items.Add(" ");

            this.comboBoxTimeCode.Text = "hh:mm:ss,zzz";
            if (!string.IsNullOrEmpty(format))
            {
                var arr = format.Split('Æ');
                if (arr.Length == 6)
                {
                    this.textBoxName.Text = arr[0];
                    this.textBoxHeader.Text = arr[1];
                    this.textBoxParagraph.Text = arr[2];
                    this.comboBoxTimeCode.Text = arr[3];
                    this.comboBoxNewLine.Text = arr[4].Replace(EnglishDoNoModify, l.DoNotModify);
                    this.textBoxFooter.Text = arr[5];
                }
            }

            this.GeneratePreview();

            this.Text = l.Title;
            this.labelName.Text = Configuration.Settings.Language.General.Name;
            this.groupBoxTemplate.Text = l.Template;
            this.labelTimeCode.Text = l.TimeCode;
            this.labelNewLine.Text = l.NewLine;
            this.labelHeader.Text = l.Header;
            this.labelTextLine.Text = l.TextLine;
            this.labelFooter.Text = l.Footer;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.groupBoxPreview.Text = Configuration.Settings.Language.General.Preview;
        }

        /// <summary>
        /// Gets or sets the format ok.
        /// </summary>
        public string FormatOk { get; set; }

        /// <summary>
        /// The export custom text format key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportCustomTextFormatKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The text box paragraph text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxParagraphTextChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            var subtitle = new Subtitle();
            var p1 = new Paragraph("Line 1a." + Environment.NewLine + "Line 1b.", 1000, 3500);
            string start1 = GetTimeCode(p1.StartTime, this.comboBoxTimeCode.Text);
            string end1 = GetTimeCode(p1.EndTime, this.comboBoxTimeCode.Text);
            var p2 = new Paragraph("Line 2a." + Environment.NewLine + "Line 2b.", 1000, 3500);
            string start2 = GetTimeCode(p2.StartTime, this.comboBoxTimeCode.Text);
            string end2 = GetTimeCode(p2.EndTime, this.comboBoxTimeCode.Text);
            subtitle.Paragraphs.Add(p1);
            subtitle.Paragraphs.Add(p2);
            try
            {
                string newLine = this.comboBoxNewLine.Text.Replace(Configuration.Settings.Language.ExportCustomTextFormat.DoNotModify, EnglishDoNoModify);
                string template = GetParagraphTemplate(this.textBoxParagraph.Text);
                this.textBoxPreview.Text = GetHeaderOrFooter("Demo", subtitle, this.textBoxHeader.Text) + GetParagraph(template, start1, end1, GetText(p1.Text, newLine), GetText("Linje 1a." + Environment.NewLine + "Line 1b.", newLine), 0, p1.Duration, this.comboBoxTimeCode.Text) + GetParagraph(template, start2, end2, GetText(p2.Text, newLine), GetText("Linje 2a." + Environment.NewLine + "Line 2b.", newLine), 1, p2.Duration, this.comboBoxTimeCode.Text) + GetHeaderOrFooter("Demo", subtitle, this.textBoxFooter.Text);
            }
            catch (Exception ex)
            {
                this.textBoxPreview.Text = ex.Message;
            }
        }

        /// <summary>
        /// The get paragraph template.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetParagraphTemplate(string template)
        {
            template = template.Replace("{start}", "{0}");
            template = template.Replace("{end}", "{1}");
            template = template.Replace("{text}", "{2}");
            template = template.Replace("{translation}", "{3}");
            template = template.Replace("{number}", "{4}");
            template = template.Replace("{number:", "{4:");
            template = template.Replace("{number-1}", "{5}");
            template = template.Replace("{number-1:", "{5:");
            template = template.Replace("{duration}", "{6}");
            template = template.Replace("{tab}", "\t");
            return template;
        }

        /// <summary>
        /// The get text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="newLine">
        /// The new line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetText(string text, string newLine)
        {
            string template = newLine;
            if (string.IsNullOrEmpty(newLine) || template == "[Do not modify]" || template == Configuration.Settings.Language.ExportCustomTextFormat.DoNotModify)
            {
                return text;
            }

            if (template == "[Only newline (hex char 0xd)]")
            {
                return text.Replace(Environment.NewLine, "\n");
            }

            return text.Replace(Environment.NewLine, template);
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTimeCode(TimeCode timeCode, string template)
        {
            if (template.Trim() == "ss")
            {
                template = template.Replace("ss", string.Format("{0:00}", timeCode.TotalSeconds));
            }

            if (template.Trim() == "s")
            {
                template = template.Replace("s", string.Format("{0}", timeCode.TotalSeconds));
            }

            if (template.Trim() == "zzz")
            {
                template = template.Replace("zzz", string.Format("{0:000}", timeCode.TotalMilliseconds));
            }

            if (template.Trim() == "z")
            {
                template = template.Replace("z", string.Format("{0}", timeCode.TotalMilliseconds));
            }

            if (template.Trim() == "ff")
            {
                template = template.Replace("ff", string.Format("{0}", SubtitleFormat.MillisecondsToFrames(timeCode.TotalMilliseconds)));
            }

            if (template.StartsWith("ssssssss"))
            {
                template = template.Replace("ssssssss", string.Format("{0:00000000}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("sssssss"))
            {
                template = template.Replace("sssssss", string.Format("{0:0000000}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("ssssss"))
            {
                template = template.Replace("ssssss", string.Format("{0:000000}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("sssss"))
            {
                template = template.Replace("sssss", string.Format("{0:00000}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("ssss"))
            {
                template = template.Replace("ssss", string.Format("{0:0000}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("sss"))
            {
                template = template.Replace("sss", string.Format("{0:000}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("ss"))
            {
                template = template.Replace("ss", string.Format("{0:00}", timeCode.TotalSeconds));
            }

            if (template.StartsWith("zzzzzzzz"))
            {
                template = template.Replace("zzzzzzzz", string.Format("{0:00000000}", timeCode.TotalMilliseconds));
            }

            if (template.StartsWith("zzzzzzz"))
            {
                template = template.Replace("zzzzzzz", string.Format("{0:0000000}", timeCode.TotalMilliseconds));
            }

            if (template.StartsWith("zzzzzz"))
            {
                template = template.Replace("zzzzzz", string.Format("{0:000000}", timeCode.TotalMilliseconds));
            }

            if (template.StartsWith("zzzzz"))
            {
                template = template.Replace("zzzzz", string.Format("{0:00000}", timeCode.TotalMilliseconds));
            }

            if (template.StartsWith("zzzz"))
            {
                template = template.Replace("zzzz", string.Format("{0:0000}", timeCode.TotalMilliseconds));
            }

            if (template.StartsWith("zzz"))
            {
                template = template.Replace("zzz", string.Format("{0:000}", timeCode.TotalMilliseconds));
            }

            template = template.Replace("hh", string.Format("{0:00}", timeCode.Hours));
            template = template.Replace("h", string.Format("{0}", timeCode.Hours));
            template = template.Replace("mm", string.Format("{0:00}", timeCode.Minutes));
            template = template.Replace("m", string.Format("{0}", timeCode.Minutes));
            template = template.Replace("ss", string.Format("{0:00}", timeCode.Seconds));
            template = template.Replace("s", string.Format("{0}", timeCode.Seconds));
            template = template.Replace("zzz", string.Format("{0:000}", timeCode.Milliseconds));
            template = template.Replace("zz", string.Format("{0:00}", Math.Round(timeCode.Milliseconds / 10.0)));
            template = template.Replace("z", string.Format("{0:0}", Math.Round(timeCode.Milliseconds / 100.0)));
            template = template.Replace("ff", string.Format("{0:00}", SubtitleFormat.MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds)));
            template = template.Replace("f", string.Format("{0}", SubtitleFormat.MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds)));
            return template;
        }

        /// <summary>
        /// The insert tag.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void InsertTag(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            if (item != null)
            {
                string s = item.Text;
                this.textBoxParagraph.Text = this.textBoxParagraph.Text.Insert(this.textBoxParagraph.SelectionStart, s);
            }
        }

        /// <summary>
        /// The combo box time code_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxTimeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The combo box new line_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxNewLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The combo box time code_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxTimeCode_TextChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The combo box new line_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxNewLine_TextChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The button o k_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.FormatOk = this.textBoxName.Text + "Æ" + this.textBoxHeader.Text + "Æ" + this.textBoxParagraph.Text + "Æ" + this.comboBoxTimeCode.Text + "Æ" + this.comboBoxNewLine.Text.Replace(Configuration.Settings.Language.ExportCustomTextFormat.DoNotModify, EnglishDoNoModify) + "Æ" + this.textBoxFooter.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The insert tag header.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void InsertTagHeader(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            if (item != null)
            {
                string s = item.Text;
                this.textBoxHeader.Text = this.textBoxHeader.Text.Insert(this.textBoxHeader.SelectionStart, s);
            }
        }

        /// <summary>
        /// The insert tag footer.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void InsertTagFooter(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            if (item != null)
            {
                string s = item.Text;
                this.textBoxFooter.Text = this.textBoxFooter.Text.Insert(this.textBoxFooter.SelectionStart, s);
            }
        }

        /// <summary>
        /// The get header or footer.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetHeaderOrFooter(string title, Subtitle subtitle, string template)
        {
            template = template.Replace("{title}", title);
            template = template.Replace("{#lines}", subtitle.Paragraphs.Count.ToString(CultureInfo.InvariantCulture));
            template = template.Replace("{tab}", "\t");
            return template;
        }

        /// <summary>
        /// The get paragraph.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <param name="timeCodeTemplate">
        /// The time code template.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string GetParagraph(string template, string start, string end, string text, string translation, int number, TimeCode duration, string timeCodeTemplate)
        {
            string d = duration.ToString();
            if (timeCodeTemplate == "ff" || timeCodeTemplate == "f")
            {
                d = SubtitleFormat.MillisecondsToFrames(duration.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
            }

            if (timeCodeTemplate == "zzz" || timeCodeTemplate == "zz" || timeCodeTemplate == "z")
            {
                d = duration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            }

            if (timeCodeTemplate == "sss" || timeCodeTemplate == "ss" || timeCodeTemplate == "s")
            {
                d = duration.Seconds.ToString(CultureInfo.InvariantCulture);
            }
            else if (timeCodeTemplate.EndsWith("ss.ff", StringComparison.Ordinal))
            {
                d = string.Format("{0:00}.{1:00}", duration.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(duration.Milliseconds));
            }
            else if (timeCodeTemplate.EndsWith("ss:ff", StringComparison.Ordinal))
            {
                d = string.Format("{0:00}:{1:00}", duration.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(duration.Milliseconds));
            }
            else if (timeCodeTemplate.EndsWith("ss,ff", StringComparison.Ordinal))
            {
                d = string.Format("{0:00},{1:00}", duration.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(duration.Milliseconds));
            }
            else if (timeCodeTemplate.EndsWith("ss;ff", StringComparison.Ordinal))
            {
                d = string.Format("{0:00};{1:00}", duration.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(duration.Milliseconds));
            }
            else if (timeCodeTemplate.EndsWith("ss;ff", StringComparison.Ordinal))
            {
                d = string.Format("{0:00};{1:00}", duration.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(duration.Milliseconds));
            }
            else if (timeCodeTemplate.EndsWith("ss.zzz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00}.{1:000}", duration.Seconds, duration.Milliseconds);
            }
            else if (timeCodeTemplate.EndsWith("ss:zzz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00}:{1:000}", duration.Seconds, duration.Milliseconds);
            }
            else if (timeCodeTemplate.EndsWith("ss,zzz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00},{1:000}", duration.Seconds, duration.Milliseconds);
            }
            else if (timeCodeTemplate.EndsWith("ss;zzz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00};{1:000}", duration.Seconds, duration.Milliseconds);
            }
            else if (timeCodeTemplate.EndsWith("ss;zzz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00};{1:000}", duration.Seconds, duration.Milliseconds);
            }
            else if (timeCodeTemplate.EndsWith("ss.zz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00}.{1:00}", duration.Seconds, Math.Round(duration.Milliseconds / 10.0));
            }
            else if (timeCodeTemplate.EndsWith("ss:zz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00}:{1:00}", duration.Seconds, Math.Round(duration.Milliseconds / 10.0));
            }
            else if (timeCodeTemplate.EndsWith("ss,zz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00},{1:00}", duration.Seconds, Math.Round(duration.Milliseconds / 10.0));
            }
            else if (timeCodeTemplate.EndsWith("ss;zz", StringComparison.Ordinal))
            {
                d = string.Format("{0:00};{1:00}", duration.Seconds, Math.Round(duration.Milliseconds / 10.0));
            }

            string s = template;
            s = s.Replace("{{", "@@@@_@@@{");
            s = s.Replace("}}", "}@@@_@@@@");
            s = string.Format(s, start, end, text, translation, number + 1, number, d);
            s = s.Replace("@@@@_@@@", "{");
            s = s.Replace("@@@_@@@@", "}");
            return s;
        }
    }
}