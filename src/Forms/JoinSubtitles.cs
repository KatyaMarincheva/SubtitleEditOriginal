// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinSubtitles.cs" company="">
//   
// </copyright>
// <summary>
//   The join subtitles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The join subtitles.
    /// </summary>
    public sealed partial class JoinSubtitles : PositionAndSizeForm
    {
        /// <summary>
        /// The _file names to join.
        /// </summary>
        private List<string> _fileNamesToJoin = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinSubtitles"/> class.
        /// </summary>
        public JoinSubtitles()
        {
            this.InitializeComponent();
            this.JoinedSubtitle = new Subtitle();
            this.labelTotalLines.Text = string.Empty;

            this.listViewParts.Columns[0].Text = Configuration.Settings.Language.JoinSubtitles.NumberOfLines;
            this.listViewParts.Columns[1].Text = Configuration.Settings.Language.JoinSubtitles.StartTime;
            this.listViewParts.Columns[2].Text = Configuration.Settings.Language.JoinSubtitles.EndTime;
            this.listViewParts.Columns[3].Text = Configuration.Settings.Language.JoinSubtitles.FileName;

            this.buttonAddVobFile.Text = Configuration.Settings.Language.DvdSubRip.Add;
            this.ButtonRemoveVob.Text = Configuration.Settings.Language.DvdSubRip.Remove;
            this.buttonClear.Text = Configuration.Settings.Language.DvdSubRip.Clear;

            this.Text = Configuration.Settings.Language.JoinSubtitles.Title;
            this.labelNote.Text = Configuration.Settings.Language.JoinSubtitles.Note;
            this.groupBoxPreview.Text = Configuration.Settings.Language.JoinSubtitles.Information;
            this.buttonJoin.Text = Configuration.Settings.Language.JoinSubtitles.Join;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets or sets the joined subtitle.
        /// </summary>
        public Subtitle JoinedSubtitle { get; set; }

        /// <summary>
        /// Gets the joined format.
        /// </summary>
        public SubtitleFormat JoinedFormat { get; private set; }

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
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The join subtitles_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void JoinSubtitles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The list view parts_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewParts_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The list view parts_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewParts_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileName in files)
            {
                bool alreadyInList = false;
                foreach (string existingFileName in this._fileNamesToJoin)
                {
                    if (existingFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        alreadyInList = true;
                    }
                }

                if (!alreadyInList)
                {
                    this._fileNamesToJoin.Add(fileName);
                }
            }

            this.SortAndLoad();
        }

        /// <summary>
        /// The sort and load.
        /// </summary>
        private void SortAndLoad()
        {
            this.JoinedFormat = new SubRip(); // default subtitle format
            string header = null;
            SubtitleFormat lastFormat = null;
            var subtitles = new List<Subtitle>();
            for (int k = 0; k < this._fileNamesToJoin.Count; k++)
            {
                string fileName = this._fileNamesToJoin[k];
                try
                {
                    var sub = new Subtitle();
                    Encoding encoding;
                    var format = sub.LoadSubtitle(fileName, out encoding, null);
                    if (format == null)
                    {
                        for (int j = k; j < this._fileNamesToJoin.Count; j++)
                        {
                            this._fileNamesToJoin.RemoveAt(j);
                        }

                        MessageBox.Show("Unkown subtitle format: " + fileName);
                        return;
                    }

                    if (sub.Header != null)
                    {
                        header = sub.Header;
                    }

                    if (lastFormat == null || lastFormat.FriendlyName == format.FriendlyName)
                    {
                        lastFormat = format;
                    }
                    else
                    {
                        lastFormat = new SubRip(); // default subtitle format
                    }

                    subtitles.Add(sub);
                }
                catch (Exception exception)
                {
                    for (int j = k; j < this._fileNamesToJoin.Count; j++)
                    {
                        this._fileNamesToJoin.RemoveAt(j);
                    }

                    MessageBox.Show(exception.Message);
                    return;
                }
            }

            this.JoinedFormat = lastFormat;

            for (int outer = 0; outer < subtitles.Count; outer++)
            {
                for (int inner = 1; inner < subtitles.Count; inner++)
                {
                    var a = subtitles[inner - 1];
                    var b = subtitles[inner];
                    if (a.Paragraphs.Count > 0 && b.Paragraphs.Count > 0 && a.Paragraphs[0].StartTime.TotalMilliseconds > b.Paragraphs[0].StartTime.TotalMilliseconds)
                    {
                        string t1 = this._fileNamesToJoin[inner - 1];
                        this._fileNamesToJoin[inner - 1] = this._fileNamesToJoin[inner];
                        this._fileNamesToJoin[inner] = t1;

                        var t2 = subtitles[inner - 1];
                        subtitles[inner - 1] = subtitles[inner];
                        subtitles[inner] = t2;
                    }
                }
            }

            this.listViewParts.BeginUpdate();
            this.listViewParts.Items.Clear();
            int i = 0;
            foreach (string fileName in this._fileNamesToJoin)
            {
                Subtitle sub = subtitles[i];
                var lvi = new ListViewItem(string.Format("{0:#,###,###}", sub.Paragraphs.Count));
                if (sub.Paragraphs.Count > 0)
                {
                    lvi.SubItems.Add(sub.Paragraphs[0].StartTime.ToString());
                    lvi.SubItems.Add(sub.Paragraphs[sub.Paragraphs.Count - 1].StartTime.ToString());
                }
                else
                {
                    lvi.SubItems.Add("-");
                    lvi.SubItems.Add("-");
                }

                lvi.SubItems.Add(fileName);
                this.listViewParts.Items.Add(lvi);
                i++;
            }

            this.listViewParts.EndUpdate();

            this.JoinedSubtitle = new Subtitle();
            if (this.JoinedFormat.FriendlyName != SubRip.NameOfFormat)
            {
                this.JoinedSubtitle.Header = header;
            }

            foreach (Subtitle sub in subtitles)
            {
                foreach (Paragraph p in sub.Paragraphs)
                {
                    this.JoinedSubtitle.Paragraphs.Add(p);
                }
            }

            this.JoinedSubtitle.Renumber();
            this.labelTotalLines.Text = string.Format(Configuration.Settings.Language.JoinSubtitles.TotalNumberOfLinesX, this.JoinedSubtitle.Paragraphs.Count);
        }

        /// <summary>
        /// The join subtitles_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void JoinSubtitles_Resize(object sender, EventArgs e)
        {
            this.columnHeaderFileName.Width = -2;
        }

        /// <summary>
        /// The button add vob file_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddVobFile_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenSubtitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetOpenDialogFilter();
            this.openFileDialog1.Multiselect = true;
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string fileName in this.openFileDialog1.FileNames)
                {
                    bool alreadyInList = false;
                    foreach (string existingFileName in this._fileNamesToJoin)
                    {
                        if (existingFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        {
                            alreadyInList = true;
                        }
                    }

                    if (!alreadyInList)
                    {
                        this._fileNamesToJoin.Add(fileName);
                    }
                }

                this.SortAndLoad();
            }
        }

        /// <summary>
        /// The button remove vob_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonRemoveVob_Click(object sender, EventArgs e)
        {
            var indices = new List<int>();
            foreach (int index in this.listViewParts.SelectedIndices)
            {
                indices.Add(index);
            }

            indices.Reverse();
            foreach (int index in indices)
            {
                this._fileNamesToJoin.RemoveAt(index);
            }

            if (this._fileNamesToJoin.Count == 0)
            {
                this.buttonClear_Click(null, null);
            }
            else
            {
                this.SortAndLoad();
            }
        }

        /// <summary>
        /// The button clear_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            this._fileNamesToJoin.Clear();
            this.listViewParts.Items.Clear();
            this.JoinedSubtitle = new Subtitle();
        }

        /// <summary>
        /// The join subtitles_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void JoinSubtitles_Shown(object sender, EventArgs e)
        {
            this.columnHeaderFileName.Width = -2;
        }
    }
}