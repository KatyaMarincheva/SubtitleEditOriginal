// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubOcrNewFolder.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub ocr new folder.
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
    /// The vob sub ocr new folder.
    /// </summary>
    public sealed partial class VobSubOcrNewFolder : Form
    {
        /// <summary>
        /// The _vob sub.
        /// </summary>
        private bool _vobSub = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcrNewFolder"/> class.
        /// </summary>
        /// <param name="vobsub">
        /// The vobsub.
        /// </param>
        public VobSubOcrNewFolder(bool vobsub)
        {
            this.InitializeComponent();
            this.FolderName = null;
            this._vobSub = vobsub;

            this.Text = Configuration.Settings.Language.VobSubOcrNewFolder.Title;
            this.label1.Text = Configuration.Settings.Language.VobSubOcrNewFolder.Message;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// The form vob sub ocr new folder_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormVobSubOcrNewFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
            string folderName = this.textBoxFolder.Text.Trim();
            if (folderName.Length == 0)
            {
                return;
            }

            if (folderName.Contains('?') || folderName.Contains('/') || folderName.Contains("\\"))
            {
                MessageBox.Show("Please correct invalid characters");
                this.textBoxFolder.Focus();
                this.textBoxFolder.SelectAll();
                return;
            }

            if (!this._vobSub)
            {
                this.FolderName = folderName;
                this.DialogResult = DialogResult.OK;
                return;
            }

            if (folderName.Length >= 0 && this._vobSub)
            {
                try
                {
                    string fullName = Configuration.VobSubCompareFolder + folderName;
                    Directory.CreateDirectory(fullName);
                    this.FolderName = folderName;
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// The text box folder key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxFolderKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonOkClick(null, null);
            }
        }
    }
}