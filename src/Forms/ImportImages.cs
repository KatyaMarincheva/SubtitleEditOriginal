// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportImages.cs" company="">
//   
// </copyright>
// <summary>
//   The import images.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The import images.
    /// </summary>
    public sealed partial class ImportImages : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportImages"/> class.
        /// </summary>
        public ImportImages()
        {
            this.InitializeComponent();
            this.Subtitle = new Subtitle();
            this.Text = Configuration.Settings.Language.ImportImages.Title;
            this.groupBoxInput.Text = Configuration.Settings.Language.ImportImages.Input;
            this.labelChooseInputFiles.Text = Configuration.Settings.Language.ImportImages.InputDescription;
            this.columnHeaderFName.Text = Configuration.Settings.Language.JoinSubtitles.FileName;
            this.columnHeaderSize.Text = Configuration.Settings.Language.General.Size;
            this.columnHeaderStartTime.Text = Configuration.Settings.Language.General.StartTime;
            this.columnHeaderEndTime.Text = Configuration.Settings.Language.General.EndTime;
            this.columnHeaderDuration.Text = Configuration.Settings.Language.General.Duration;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
        }

        /// <summary>
        /// Gets the subtitle.
        /// </summary>
        public Subtitle Subtitle { get; private set; }

        /// <summary>
        /// The button input browse_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonInputBrowse_Click(object sender, EventArgs e)
        {
            this.buttonInputBrowse.Enabled = false;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Configuration.Settings.Language.ImportImages.ImageFiles + "|*.png;*.jpg;*.bmp;*.gif;*.tif;*.tiff";
            this.openFileDialog1.Multiselect = true;
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string fileName in this.openFileDialog1.FileNames)
                {
                    this.AddInputFile(fileName);
                }
            }

            this.buttonInputBrowse.Enabled = true;
        }

        /// <summary>
        /// The add input file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void AddInputFile(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);
                var item = new ListViewItem(fileName);
                item.SubItems.Add(Utilities.FormatBytesToDisplayFileSize(fi.Length));
                string ext = Path.GetExtension(fileName).ToLowerInvariant();
                if (ext == ".png" || ext == ".jpg" || ext == ".bmp" || ext == ".gif" || ext == ".tif" || ext == ".tiff")
                {
                    SetTimeCodes(fileName, item);
                    this.listViewInputFiles.Items.Add(item);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The set time codes.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        private static void SetTimeCodes(string fileName, ListViewItem item)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            var p = new Paragraph();
            SetEndTimeAndStartTime(name, p);
            item.SubItems.Add(p.StartTime.ToString());
            item.SubItems.Add(p.EndTime.ToString());
            item.SubItems.Add(p.Duration.ToShortString());
        }

        /// <summary>
        /// The set end time and start time.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        private static void SetEndTimeAndStartTime(string name, Paragraph p)
        {
            if (name.Contains("-to-"))
            {
                var arr = name.Replace("-to-", "_").Split('_');
                if (arr.Length == 3)
                {
                    int startTime, endTime;
                    if (int.TryParse(arr[1], out startTime) && int.TryParse(arr[2], out endTime))
                    {
                        p.StartTime.TotalMilliseconds = startTime;
                        p.EndTime.TotalMilliseconds = endTime;
                    }
                }
            }
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
            foreach (ListViewItem item in this.listViewInputFiles.Items)
            {
                var p = new Paragraph();
                p.Text = item.Text;
                string name = Path.GetFileNameWithoutExtension(p.Text);
                SetEndTimeAndStartTime(name, p);
                this.Subtitle.Paragraphs.Add(p);
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
        /// The list view input files_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The list view input files_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileName in fileNames)
            {
                this.AddInputFile(fileName);
            }
        }

        /// <summary>
        /// The import images_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ImportImages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyData == (Keys.Control | Keys.O))
            {
                this.buttonInputBrowse_Click(null, EventArgs.Empty);
            }
        }
    }
}