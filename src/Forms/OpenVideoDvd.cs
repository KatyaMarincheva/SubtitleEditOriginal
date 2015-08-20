// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenVideoDvd.cs" company="">
//   
// </copyright>
// <summary>
//   The open video dvd.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The open video dvd.
    /// </summary>
    public partial class OpenVideoDvd : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenVideoDvd"/> class.
        /// </summary>
        public OpenVideoDvd()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.OpenVideoDvd.Title;
            this.groupBoxOpenDvdFrom.Text = Configuration.Settings.Language.OpenVideoDvd.OpenDvdFrom;
            this.radioButtonDisc.Text = Configuration.Settings.Language.OpenVideoDvd.Disc;
            this.radioButtonFolder.Text = Configuration.Settings.Language.OpenVideoDvd.Folder;
            this.labelChooseDrive.Text = Configuration.Settings.Language.OpenVideoDvd.ChooseDrive;
            this.labelChooseFolder.Text = Configuration.Settings.Language.OpenVideoDvd.ChooseFolder;
            this.PanelDrive.Enabled = false;
            Utilities.FixLargeFonts(this, this.buttonOK);
            this.radioButtonDisc_CheckedChanged(null, null);

            this.PanelFolder.Left = this.PanelDrive.Left;
            this.PanelFolder.Top = this.PanelDrive.Top;
        }

        /// <summary>
        /// Gets or sets the dvd path.
        /// </summary>
        public string DvdPath { get; set; }

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
            if (this.radioButtonDisc.Checked)
            {
                string s = this.comboBoxDrive.Items[this.comboBoxDrive.SelectedIndex].ToString();
                if (s.Contains(' '))
                {
                    s = s.Substring(0, s.IndexOf(' '));
                }

                this.DvdPath = s;
            }
            else
            {
                this.DvdPath = this.textBoxFolder.Text;
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The radio button disc_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonDisc_CheckedChanged(object sender, EventArgs e)
        {
            this.PanelDrive.Visible = this.radioButtonDisc.Checked;
            this.PanelFolder.Visible = this.radioButtonFolder.Checked;
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
            this.folderBrowserDialog1.ShowNewFolderButton = true;
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// The open video dvd_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OpenVideoDvd_Shown(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.CDRom || drive.DriveType == DriveType.Removable)
                {
                    if (drive.IsReady)
                    {
                        try
                        {
                            this.comboBoxDrive.Items.Add(drive + "  " + drive.VolumeLabel);
                        }
                        catch
                        {
                            this.comboBoxDrive.Items.Add(drive.ToString());
                        }
                    }
                    else
                    {
                        this.comboBoxDrive.Items.Add(drive.ToString());
                    }
                }
            }

            if (this.comboBoxDrive.Items.Count > 0)
            {
                this.comboBoxDrive.SelectedIndex = 0;
            }
            else
            {
                this.radioButtonFolder.Checked = true;
            }

            this.PanelDrive.Enabled = true;
        }

        /// <summary>
        /// The open video dvd_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OpenVideoDvd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}