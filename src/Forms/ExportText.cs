// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportText.cs" company="">
//   
// </copyright>
// <summary>
//   The export text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The export text.
    /// </summary>
    public sealed partial class ExportText : Form
    {
        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportText"/> class.
        /// </summary>
        public ExportText()
        {
            this.InitializeComponent();
            var l = Configuration.Settings.Language.ExportText;
            this.Text = l.Title;
            this.labelPreview.Text = l.Preview;
            this.groupBoxImportOptions.Text = l.ExportOptions;
            this.groupBoxFormatText.Text = l.FormatText;
            this.radioButtonFormatNone.Text = l.None;
            this.radioButtonFormatMergeAll.Text = l.MergeAllLines;
            this.radioButtonFormatUnbreak.Text = l.UnbreakLines;
            this.checkBoxRemoveStyling.Text = l.RemoveStyling;
            this.checkBoxShowLineNumbers.Text = l.ShowLineNumbers;
            this.checkBoxAddNewlineAfterLineNumber.Text = l.AddNewLineAfterLineNumber;
            this.checkBoxShowTimeCodes.Text = l.ShowTimeCode;
            this.checkBoxAddAfterText.Text = l.AddNewLineAfterTexts;
            this.checkBoxAddNewlineAfterTimeCodes.Text = l.AddNewLineAfterTimeCode;
            this.checkBoxAddNewLine2.Text = l.AddNewLineBetweenSubtitles;
            this.groupBoxTimeCodeFormat.Text = l.TimeCodeFormat;
            this.radioButtonTimeCodeSrt.Text = l.Srt;
            this.radioButtonTimeCodeMs.Text = l.Milliseconds;
            this.radioButtonTimeCodeHHMMSSFF.Text = l.HHMMSSFF;
            this.labelTimeCodeSeparator.Text = l.TimeCodeSeparator;
            this.labelEncoding.Text = Configuration.Settings.Language.Main.Controls.FileEncoding;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonOK.Text = Configuration.Settings.Language.Main.Menu.File.SaveAs;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(Subtitle subtitle, string fileName)
        {
            this._subtitle = subtitle;
            this._fileName = fileName;
            this.textBoxText.ReadOnly = true;
            this.comboBoxTimeCodeSeparator.SelectedIndex = 0;
            this.GeneratePreview();

            this.comboBoxEncoding.Items.Clear();
            int encodingSelectedIndex = 0;
            this.comboBoxEncoding.Items.Add(Encoding.UTF8.EncodingName);
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.Name != Encoding.UTF8.BodyName && ei.CodePage >= 949 && !ei.DisplayName.Contains("EBCDIC") && ei.CodePage != 1047)
                {
                    this.comboBoxEncoding.Items.Add(ei.CodePage + ": " + ei.DisplayName);
                    if (ei.Name == Configuration.Settings.General.DefaultEncoding)
                    {
                        encodingSelectedIndex = this.comboBoxEncoding.Items.Count - 1;
                    }
                }
            }

            this.comboBoxEncoding.SelectedIndex = encodingSelectedIndex;
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            this.groupBoxTimeCodeFormat.Enabled = this.checkBoxShowTimeCodes.Checked;
            this.checkBoxAddAfterText.Enabled = !this.radioButtonFormatMergeAll.Checked;
            this.checkBoxAddNewLine2.Enabled = !this.radioButtonFormatMergeAll.Checked;
            this.checkBoxAddNewlineAfterLineNumber.Enabled = this.checkBoxShowLineNumbers.Checked;
            this.checkBoxAddNewlineAfterTimeCodes.Enabled = this.checkBoxShowTimeCodes.Checked;

            string text = GeneratePlainText(this._subtitle, this.checkBoxShowLineNumbers.Checked, this.checkBoxAddNewlineAfterLineNumber.Checked, this.checkBoxShowTimeCodes.Checked, this.radioButtonTimeCodeSrt.Checked, this.radioButtonTimeCodeHHMMSSFF.Checked, this.checkBoxAddNewlineAfterTimeCodes.Checked, this.comboBoxTimeCodeSeparator.Text, this.checkBoxRemoveStyling.Checked, this.radioButtonFormatUnbreak.Checked, this.checkBoxAddAfterText.Checked, this.checkBoxAddNewLine2.Checked, this.radioButtonFormatMergeAll.Checked);
            this.textBoxText.Text = text;
        }

        /// <summary>
        /// The generate plain text.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="showLineNumbers">
        /// The show line numbers.
        /// </param>
        /// <param name="addNewlineAfterLineNumber">
        /// The add newline after line number.
        /// </param>
        /// <param name="showTimecodes">
        /// The show timecodes.
        /// </param>
        /// <param name="timeCodeSrt">
        /// The time code srt.
        /// </param>
        /// <param name="timeCodeHHMMSSFF">
        /// The time code hhmmssff.
        /// </param>
        /// <param name="addNewlineAfterTimeCodes">
        /// The add newline after time codes.
        /// </param>
        /// <param name="timeCodeSeparator">
        /// The time code separator.
        /// </param>
        /// <param name="removeStyling">
        /// The remove styling.
        /// </param>
        /// <param name="formatUnbreak">
        /// The format unbreak.
        /// </param>
        /// <param name="addAfterText">
        /// The add after text.
        /// </param>
        /// <param name="checkBoxAddNewLine2">
        /// The check box add new line 2.
        /// </param>
        /// <param name="formatMergeAll">
        /// The format merge all.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GeneratePlainText(Subtitle subtitle, bool showLineNumbers, bool addNewlineAfterLineNumber, bool showTimecodes, bool timeCodeSrt, bool timeCodeHHMMSSFF, bool addNewlineAfterTimeCodes, string timeCodeSeparator, bool removeStyling, bool formatUnbreak, bool addAfterText, bool checkBoxAddNewLine2, bool formatMergeAll)
        {
            var sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (showLineNumbers)
                {
                    sb.Append(p.Number);
                    if (addNewlineAfterLineNumber)
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }

                if (showTimecodes)
                {
                    if (timeCodeSrt)
                    {
                        sb.Append(p.StartTime + timeCodeSeparator + p.EndTime);
                    }
                    else if (timeCodeHHMMSSFF)
                    {
                        sb.Append(p.StartTime.ToHHMMSSFF() + timeCodeSeparator + p.EndTime.ToHHMMSSFF());
                    }
                    else
                    {
                        sb.Append(p.StartTime.TotalMilliseconds + timeCodeSeparator + p.EndTime.TotalMilliseconds);
                    }

                    if (addNewlineAfterTimeCodes)
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }

                string s = p.Text;
                if (removeStyling)
                {
                    s = HtmlUtil.RemoveHtmlTags(s, true);
                }

                if (formatUnbreak)
                {
                    sb.Append(s.Replace(Environment.NewLine, " ").Replace("  ", " "));
                }
                else
                {
                    sb.Append(s);
                }

                if (addAfterText)
                {
                    sb.AppendLine();
                }

                if (checkBoxAddNewLine2)
                {
                    sb.AppendLine();
                }

                if (!addAfterText && !checkBoxAddNewLine2)
                {
                    sb.Append(' ');
                }
            }

            string text = sb.ToString().Trim();
            if (formatMergeAll)
            {
                text = text.Replace(Environment.NewLine, " ");
                text = text.Replace("  ", " ").Replace("  ", " ");
            }

            return text;
        }

        /// <summary>
        /// The get current encoding.
        /// </summary>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        private Encoding GetCurrentEncoding()
        {
            if (this.comboBoxEncoding.Text == Encoding.UTF8.BodyName || this.comboBoxEncoding.Text == Encoding.UTF8.EncodingName || this.comboBoxEncoding.Text == "utf-8")
            {
                return Encoding.UTF8;
            }

            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.CodePage + ": " + ei.DisplayName == this.comboBoxEncoding.Text)
                {
                    return ei.GetEncoding();
                }
            }

            return Encoding.UTF8;
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
            this.GeneratePreview();
            this.saveFileDialog1.Title = Configuration.Settings.Language.Main.ExportPlainTextAs;
            this.saveFileDialog1.Filter = Configuration.Settings.Language.Main.TextFiles + "|*.txt";
            if (!string.IsNullOrEmpty(this._fileName))
            {
                this.saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(this._fileName);
            }

            if (this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                File.WriteAllText(this.saveFileDialog1.FileName, this.textBoxText.Text, this.GetCurrentEncoding());
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The radio button format none_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonFormatNone_CheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
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
        /// The export text_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}