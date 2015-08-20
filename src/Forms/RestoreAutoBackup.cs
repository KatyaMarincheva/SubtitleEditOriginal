// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreAutoBackup.cs" company="">
//   
// </copyright>
// <summary>
//   The restore auto backup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The restore auto backup.
    /// </summary>
    public partial class RestoreAutoBackup : PositionAndSizeForm
    {
        /// <summary>
        /// The _files.
        /// </summary>
        private string[] _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreAutoBackup"/> class.
        /// </summary>
        public RestoreAutoBackup()
        {
            this.InitializeComponent();
            this.labelStatus.Text = string.Empty;

            var l = Configuration.Settings.Language.RestoreAutoBackup;
            this.Text = l.Title;
            this.linkLabelOpenContainingFolder.Text = Configuration.Settings.Language.Main.Menu.File.OpenContainingFolder;
            this.listViewBackups.Columns[0].Text = l.DateAndTime;
            this.listViewBackups.Columns[1].Text = l.FileName;
            this.listViewBackups.Columns[2].Text = l.Extension;
            this.listViewBackups.Columns[3].Text = Configuration.Settings.Language.General.Size;
            this.labelInfo.Text = l.Information;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets or sets the auto backup file name.
        /// </summary>
        public string AutoBackupFileName { get; set; }

        /// <summary>
        /// The restore auto backup_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RestoreAutoBackup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The restore auto backup_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RestoreAutoBackup_Shown(object sender, EventArgs e)
        {
            // 2011-12-13_20-19-18_title
            var fileNamePattern = new Regex(@"^\d\d\d\d-\d\d-\d\d_\d\d-\d\d-\d\d", RegexOptions.Compiled);
            this.listViewBackups.Columns[2].Width = -2;
            if (Directory.Exists(Configuration.AutoBackupFolder))
            {
                this._files = Directory.GetFiles(Configuration.AutoBackupFolder, "*.*");
                foreach (string fileName in this._files)
                {
                    if (fileNamePattern.IsMatch(Path.GetFileName(fileName)))
                    {
                        this.AddBackupToListView(fileName);
                    }
                }

                this.listViewBackups.Sorting = SortOrder.Descending;
                this.listViewBackups.Sort();
                if (this._files.Length > 0)
                {
                    return;
                }
            }

            this.linkLabelOpenContainingFolder.Visible = false;
            this.labelStatus.Left = this.linkLabelOpenContainingFolder.Left;
            this.labelStatus.Text = Configuration.Settings.Language.RestoreAutoBackup.NoBackedUpFilesFound;
        }

        /// <summary>
        /// The add backup to list view.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void AddBackupToListView(string fileName)
        {
            string displayDate = Path.GetFileName(fileName).Substring(0, 19).Replace('_', ' ');
            displayDate = displayDate.Remove(13, 1).Insert(13, ":");
            displayDate = displayDate.Remove(16, 1).Insert(16, ":");

            string displayName = Path.GetFileName(fileName).Remove(0, 20);

            if (displayName == "srt")
            {
                displayName = "Untitled.srt";
            }

            var item = new ListViewItem(displayDate);
            item.UseItemStyleForSubItems = false;
            item.Tag = fileName;

            var subItem = new ListViewItem.ListViewSubItem(item, Path.GetFileNameWithoutExtension(displayName));
            item.SubItems.Add(subItem);

            subItem = new ListViewItem.ListViewSubItem(item, Path.GetExtension(fileName));
            item.SubItems.Add(subItem);

            try
            {
                FileInfo fi = new FileInfo(fileName);
                subItem = new ListViewItem.ListViewSubItem(item, fi.Length + " bytes");
                item.SubItems.Add(subItem);
            }
            catch
            {
            }

            this.listViewBackups.Items.Add(item);
        }

        /// <summary>
        /// The set auto backup file name.
        /// </summary>
        private void SetAutoBackupFileName()
        {
            this.AutoBackupFileName = this.listViewBackups.SelectedItems[0].Tag.ToString();
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
            if (this.listViewBackups.SelectedItems.Count == 1)
            {
                this.SetAutoBackupFileName();
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The list view backups_ mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewBackups_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listViewBackups.SelectedItems.Count == 1)
            {
                this.SetAutoBackupFileName();
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The restore auto backup_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RestoreAutoBackup_ResizeEnd(object sender, EventArgs e)
        {
            this.listViewBackups.Columns[2].Width = -2;
        }

        /// <summary>
        /// The restore auto backup_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RestoreAutoBackup_SizeChanged(object sender, EventArgs e)
        {
            this.listViewBackups.Columns[2].Width = -2;
        }

        /// <summary>
        /// The link label open containing folder_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabelOpenContainingFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string folderName = Configuration.AutoBackupFolder;
            if (Utilities.IsRunningOnMono())
            {
                System.Diagnostics.Process.Start(folderName);
            }
            else
            {
                if (this.listViewBackups.SelectedItems.Count == 1)
                {
                    string argument = @"/select, " + this.listViewBackups.SelectedItems[0].Tag;
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
                else
                {
                    System.Diagnostics.Process.Start(folderName);
                }
            }
        }
    }
}