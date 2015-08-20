// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Split.cs" company="">
//   
// </copyright>
// <summary>
//   The split.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The split.
    /// </summary>
    public sealed partial class Split : Form
    {
        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _loading.
        /// </summary>
        private bool _loading = true;

        /// <summary>
        /// The _parts.
        /// </summary>
        private List<Subtitle> _parts;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _total number of characters.
        /// </summary>
        private int _totalNumberOfCharacters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Split"/> class.
        /// </summary>
        public Split()
        {
            this.InitializeComponent();

            var l = Configuration.Settings.Language.Split;
            this.Text = l.Title;
            this.groupBoxSplitOptions.Text = l.SplitOptions;
            this.RadioButtonLines.Text = l.Lines;
            this.radioButtonCharacters.Text = l.Characters;
            this.labelNumberOfParts.Text = l.NumberOfEqualParts;
            this.groupBoxSubtitleInfo.Text = l.SubtitleInfo;
            this.groupBoxOutput.Text = l.Output;
            this.labelFileName.Text = l.FileName;
            this.labelChooseOutputFolder.Text = l.OutputFolder;
            this.labelOutputFormat.Text = Configuration.Settings.Language.Main.Controls.SubtitleFormat;
            this.labelEncoding.Text = Configuration.Settings.Language.Main.Controls.FileEncoding;
            this.groupBoxPreview.Text = Configuration.Settings.Language.General.Preview;
            this.buttonOpenOutputFolder.Text = Configuration.Settings.Language.Main.Menu.File.Open;

            this.listViewParts.Columns[0].Text = l.Lines;
            this.listViewParts.Columns[1].Text = l.Characters;
            this.listViewParts.Columns[2].Text = l.FileName;

            this.buttonSplit.Text = l.DoSplit;
            this.buttonBasic.Text = l.Basic;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            this.comboBoxSubtitleFormats.Left = this.labelOutputFormat.Left + this.labelOutputFormat.Width + 3;
            this.comboBoxEncoding.Left = this.labelEncoding.Left + this.labelEncoding.Width + 3;

            Utilities.FixLargeFonts(this, this.buttonSplit);
        }

        /// <summary>
        /// Gets a value indicating whether show basic.
        /// </summary>
        public bool ShowBasic { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        public void Initialize(Subtitle subtitle, string fileName, SubtitleFormat format)
        {
            this.ShowBasic = false;
            this._subtitle = subtitle;
            if (string.IsNullOrEmpty(fileName))
            {
                this.textBoxFileName.Text = Configuration.Settings.Language.SplitSubtitle.Untitled;
            }
            else
            {
                this.textBoxFileName.Text = fileName;
            }

            this._fileName = fileName;
            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                this._totalNumberOfCharacters += p.Text.Length;
            }

            this.labelLines.Text = string.Format(Configuration.Settings.Language.Split.NumberOfLinesX, this._subtitle.Paragraphs.Count);
            this.labelCharacters.Text = string.Format(Configuration.Settings.Language.Split.NumberOfCharactersX, this._totalNumberOfCharacters);

            try
            {
                this.numericUpDownParts.Value = Configuration.Settings.Tools.SplitNumberOfParts;
            }
            catch
            {
            }

            if (Configuration.Settings.Tools.SplitVia.Trim().Equals("lines", StringComparison.OrdinalIgnoreCase))
            {
                this.RadioButtonLines.Checked = true;
            }
            else
            {
                this.radioButtonCharacters.Checked = true;
            }

            foreach (SubtitleFormat f in SubtitleFormat.AllSubtitleFormats)
            {
                if (!f.IsVobSubIndexFile)
                {
                    this.comboBoxSubtitleFormats.Items.Add(f.FriendlyName);
                }

                if (f.FriendlyName == format.FriendlyName)
                {
                    this.comboBoxSubtitleFormats.SelectedIndex = this.comboBoxSubtitleFormats.Items.Count - 1;
                }
            }

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

            if (this.numericUpDownParts.Maximum > this._subtitle.Paragraphs.Count)
            {
                this.numericUpDownParts.Maximum = this._subtitle.Paragraphs.Count / 2;
            }

            if (!string.IsNullOrEmpty(this._fileName))
            {
                this.textBoxOutputFolder.Text = Path.GetDirectoryName(this._fileName);
            }
            else if (string.IsNullOrEmpty(Configuration.Settings.Tools.SplitOutputFolder) || !Directory.Exists(Configuration.Settings.Tools.SplitOutputFolder))
            {
                this.textBoxOutputFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                this.textBoxOutputFolder.Text = Configuration.Settings.Tools.SplitOutputFolder;
            }
        }

        /// <summary>
        /// The calculate parts.
        /// </summary>
        private void CalculateParts()
        {
            if (this._loading)
            {
                return;
            }

            this._loading = true;
            this._parts = new List<Subtitle>();
            if (string.IsNullOrEmpty(this.textBoxOutputFolder.Text) || !Directory.Exists(this.textBoxOutputFolder.Text))
            {
                this.textBoxOutputFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var format = Utilities.GetSubtitleFormatByFriendlyName(this.comboBoxSubtitleFormats.SelectedItem.ToString());
            string fileNameNoExt = Path.GetFileNameWithoutExtension(this.textBoxFileName.Text);
            if (string.IsNullOrWhiteSpace(fileNameNoExt))
            {
                fileNameNoExt = Configuration.Settings.Language.SplitSubtitle.Untitled;
            }

            this.listViewParts.Items.Clear();
            int startNumber = 0;
            if (this.RadioButtonLines.Checked)
            {
                int partSize = (int)(this._subtitle.Paragraphs.Count / this.numericUpDownParts.Value);
                for (int i = 0; i < this.numericUpDownParts.Value; i++)
                {
                    int noOfLines = partSize;
                    if (i == this.numericUpDownParts.Value - 1)
                    {
                        noOfLines = (int)(this._subtitle.Paragraphs.Count - ((this.numericUpDownParts.Value - 1) * partSize));
                    }

                    var temp = new Subtitle();
                    temp.Header = this._subtitle.Header;
                    int size = 0;
                    for (int number = 0; number < noOfLines; number++)
                    {
                        Paragraph p = this._subtitle.Paragraphs[startNumber + number];
                        temp.Paragraphs.Add(new Paragraph(p));
                        size += p.Text.Length;
                    }

                    startNumber += noOfLines;
                    this._parts.Add(temp);

                    var lvi = new ListViewItem(string.Format("{0:#,###,###}", noOfLines));
                    lvi.SubItems.Add(string.Format("{0:#,###,###}", size));
                    lvi.SubItems.Add(fileNameNoExt + ".Part" + (i + 1) + format.Extension);
                    this.listViewParts.Items.Add(lvi);
                }
            }
            else if (this.radioButtonCharacters.Checked)
            {
                int partSize = (int)(this._totalNumberOfCharacters / this.numericUpDownParts.Value);
                int nextLimit = partSize;
                int currentSize = 0;
                Subtitle temp = new Subtitle();
                temp.Header = this._subtitle.Header;
                for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
                {
                    Paragraph p = this._subtitle.Paragraphs[i];
                    int size = p.Text.Length;
                    if (currentSize + size > nextLimit + 4 && this._parts.Count < this.numericUpDownParts.Value - 1)
                    {
                        this._parts.Add(temp);
                        var lvi = new ListViewItem(string.Format("{0:#,###,###}", temp.Paragraphs.Count));
                        lvi.SubItems.Add(string.Format("{0:#,###,###}", currentSize));
                        lvi.SubItems.Add(fileNameNoExt + ".Part" + this._parts.Count + format.Extension);
                        this.listViewParts.Items.Add(lvi);
                        currentSize = size;
                        temp = new Subtitle();
                        temp.Header = this._subtitle.Header;
                        temp.Paragraphs.Add(new Paragraph(p));
                    }
                    else
                    {
                        currentSize += size;
                        temp.Paragraphs.Add(new Paragraph(p));
                    }
                }

                this._parts.Add(temp);
                var lvi2 = new ListViewItem(string.Format("{0:#,###,###}", temp.Paragraphs.Count));
                lvi2.SubItems.Add(string.Format("{0:#,###,###}", currentSize));
                lvi2.SubItems.Add(fileNameNoExt + ".Part" + this.numericUpDownParts.Value + ".srt");
                this.listViewParts.Items.Add(lvi2);
            }

            this._loading = false;
        }

        /// <summary>
        /// The button basic_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonBasic_Click(object sender, EventArgs e)
        {
            this.ShowBasic = true;
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The button split_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSplit_Click(object sender, EventArgs e)
        {
            bool overwrite = false;
            var format = Utilities.GetSubtitleFormatByFriendlyName(this.comboBoxSubtitleFormats.SelectedItem.ToString());
            string fileNameNoExt = Path.GetFileNameWithoutExtension(this.textBoxFileName.Text);
            if (string.IsNullOrWhiteSpace(fileNameNoExt))
            {
                fileNameNoExt = Configuration.Settings.Language.SplitSubtitle.Untitled;
            }

            int number = 1;
            try
            {
                foreach (Subtitle sub in this._parts)
                {
                    string fileName = Path.Combine(this.textBoxOutputFolder.Text, fileNameNoExt + ".Part" + number + format.Extension);
                    string allText = sub.ToText(format);
                    if (File.Exists(fileName) && !overwrite)
                    {
                        if (MessageBox.Show(Configuration.Settings.Language.SplitSubtitle.OverwriteExistingFiles, string.Empty, MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }

                        overwrite = true;
                    }

                    File.WriteAllText(fileName, allText, this.GetCurrentEncoding());
                    number++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            Configuration.Settings.Tools.SplitNumberOfParts = (int)this.numericUpDownParts.Value;
            Configuration.Settings.Tools.SplitOutputFolder = this.textBoxOutputFolder.Text;
            if (this.RadioButtonLines.Checked)
            {
                Configuration.Settings.Tools.SplitVia = "Lines";
            }
            else
            {
                Configuration.Settings.Tools.SplitVia = "Characters";
            }

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
        /// The numeric up down parts_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownParts_ValueChanged(object sender, EventArgs e)
        {
            this.CalculateParts();
        }

        /// <summary>
        /// The radio button characters_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonCharacters_CheckedChanged(object sender, EventArgs e)
        {
            this.CalculateParts();
        }

        /// <summary>
        /// The radio button lines_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonLines_CheckedChanged(object sender, EventArgs e)
        {
            this.CalculateParts();
        }

        /// <summary>
        /// The text box output folder_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxOutputFolder_TextChanged(object sender, EventArgs e)
        {
            this.CalculateParts();
        }

        /// <summary>
        /// The split_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Split_ResizeEnd(object sender, EventArgs e)
        {
            this.columnHeaderFileName.Width = -2;
        }

        /// <summary>
        /// The button choose folder_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChooseFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxOutputFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }
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
        /// The split_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Split_Shown(object sender, EventArgs e)
        {
            this._loading = false;
            this.CalculateParts();
        }

        /// <summary>
        /// The split_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Split_Resize(object sender, EventArgs e)
        {
            this.columnHeaderFileName.Width = -2;
        }

        /// <summary>
        /// The split_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Split_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The text box file name_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxFileName_TextChanged(object sender, EventArgs e)
        {
            this.CalculateParts();
        }

        /// <summary>
        /// The combo box subtitle formats_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxSubtitleFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CalculateParts();
        }

        /// <summary>
        /// The button open output folder_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOpenOutputFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.textBoxOutputFolder.Text))
            {
                System.Diagnostics.Process.Start(this.textBoxOutputFolder.Text);
            }
            else
            {
                MessageBox.Show(string.Format(Configuration.Settings.Language.SplitSubtitle.FolderNotFoundX, this.textBoxOutputFolder.Text));
            }
        }
    }
}