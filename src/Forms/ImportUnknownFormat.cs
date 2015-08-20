// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportUnknownFormat.cs" company="">
//   
// </copyright>
// <summary>
//   The import unknown format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The import unknown format.
    /// </summary>
    public partial class ImportUnknownFormat : Form
    {
        /// <summary>
        /// The _refresh timer.
        /// </summary>
        private readonly Timer _refreshTimer = new Timer();

        /// <summary>
        /// The imported subitle.
        /// </summary>
        public Subtitle ImportedSubitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportUnknownFormat"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public ImportUnknownFormat(string fileName)
        {
            this.InitializeComponent();
            this._refreshTimer.Interval = 400;
            this._refreshTimer.Tick += this.RefreshTimerTick;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.AutoSizeAllColumns(this);

            if (!string.IsNullOrEmpty(fileName))
            {
                this.LoadTextFile(fileName);
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            if (this._refreshTimer.Enabled)
            {
                this._refreshTimer.Stop();
                this._refreshTimer.Start();
            }
            else
            {
                this._refreshTimer.Start();
            }
        }

        /// <summary>
        /// The generate preview real.
        /// </summary>
        private void GeneratePreviewReal()
        {
            var uknownFormatImporter = new UknownFormatImporter();
            uknownFormatImporter.UseFrames = this.radioButtonTimeCodeFrames.Checked;
            this.ImportedSubitle = uknownFormatImporter.AutoGuessImport(this.textBoxText.Lines);
            this.groupBoxImportResult.Text = string.Format(Configuration.Settings.Language.ImportText.PreviewLinesModifiedX, this.ImportedSubitle.Paragraphs.Count);
            this.SubtitleListview1.Fill(this.ImportedSubitle);
            if (this.ImportedSubitle.Paragraphs.Count > 0)
            {
                this.SubtitleListview1.SelectIndexAndEnsureVisible(0);
            }
        }

        /// <summary>
        /// The refresh timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RefreshTimerTick(object sender, EventArgs e)
        {
            this._refreshTimer.Stop();
            this.GeneratePreviewReal();
        }

        /// <summary>
        /// The load text file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void LoadTextFile(string fileName)
        {
            try
            {
                this.SubtitleListview1.Items.Clear();
                Encoding encoding = Utilities.GetEncodingFromFile(fileName);
                this.textBoxText.Text = File.ReadAllText(fileName, encoding);

                // check for RTF file
                if (fileName.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase) && !this.textBoxText.Text.TrimStart().StartsWith("{\\rtf"))
                {
                    using (var rtb = new RichTextBox())
                    {
                        rtb.Rtf = this.textBoxText.Text;
                        this.textBoxText.Text = rtb.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// The button open text_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOpenText_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = this.buttonOpenText.Text;
            this.openFileDialog1.Filter = Configuration.Settings.Language.ImportText.TextFiles + "|*.txt|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.LoadTextFile(this.openFileDialog1.FileName);
            }

            this.GeneratePreview();
        }

        /// <summary>
        /// The button refresh_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            this.GeneratePreviewReal();
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
            this.DialogResult = DialogResult.OK;
        }
    }
}