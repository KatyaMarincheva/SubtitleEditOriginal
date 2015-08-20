// --------------------------------------------------------------------------------------------------------------------
// <copyright file="About.cs" company="">
//   
// </copyright>
// <summary>
//   The about.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The about.
    /// </summary>
    partial class About : PositionAndSizeForm
    {
        /// <summary>
        /// The _language.
        /// </summary>
        private readonly LanguageStructure.About _language = Configuration.Settings.Language.About;

        /// <summary>
        /// The _language general.
        /// </summary>
        private readonly LanguageStructure.General _languageGeneral = Configuration.Settings.Language.General;

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            this.Text = this._language.Title + " - " + (IntPtr.Size * 8) + "-bit";
            this.okButton.Text = this._languageGeneral.Ok;
            string[] versionInfo = Utilities.AssemblyVersion.Split('.');
            string revisionNumber = "0";
            if (versionInfo.Length >= 4)
            {
                revisionNumber = versionInfo[3];
            }

            if (revisionNumber == "0")
            {
                this.labelProduct.Text = string.Format("{0} {1}.{2}.{3}, ", this._languageGeneral.Title, versionInfo[0], versionInfo[1], versionInfo[2]);
                revisionNumber = Utilities.AssemblyDescription.Substring(0, 7);
            }
            else
            {
                this.labelProduct.Text = string.Format("{0} {1}.{2}.{3}, build", this._languageGeneral.Title, versionInfo[0], versionInfo[1], versionInfo[2]);
            }

            this.linkLabelGitBuildHash.Left = this.labelProduct.Left + this.labelProduct.Width - 5;
            this.linkLabelGitBuildHash.Text = revisionNumber;
            this.tooltip.SetToolTip(this.linkLabelGitBuildHash, GetGitHubHashLink());

            string aboutText = this._language.AboutText1.TrimEnd() + Environment.NewLine + Environment.NewLine + this._languageGeneral.TranslatedBy.Trim();
            while (aboutText.Contains("\n ") || aboutText.Contains("\n\t"))
            {
                aboutText = aboutText.Replace("\n ", "\n");
                aboutText = aboutText.Replace("\n\t", "\n");
            }

            this.richTextBoxAbout1.Text = aboutText;

            double height = TextDraw.MeasureTextHeight(this.richTextBoxAbout1.Font, this.richTextBoxAbout1.Text, false) * 1.4 + 80;
            this.richTextBoxAbout1.Height = (int)height;
            this.Height = this.richTextBoxAbout1.Top + this.richTextBoxAbout1.Height + 90;
        }

        /// <summary>
        /// The ok button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OkButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The about_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void About_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp(null);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The rich text box about 1 link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RichTextBoxAbout1LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch
            {
                MessageBox.Show("Unable to start link: " + e.LinkText, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The button donate_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDonate_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.nikse.dk/Donate");
        }

        /// <summary>
        /// The link label git build hash_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabelGitBuildHash_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(GetGitHubHashLink());
        }

        /// <summary>
        /// The get git hub hash link.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetGitHubHashLink()
        {
            try
            {
                return "https://github.com/SubtitleEdit/subtitleedit/commit/" + Utilities.AssemblyDescription.Substring(0, 7);
            }
            catch
            {
                return "https://github.com/SubtitleEdit/subtitleedit";
            }
        }
    }
}