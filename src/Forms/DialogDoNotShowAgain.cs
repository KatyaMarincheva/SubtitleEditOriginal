// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogDoNotShowAgain.cs" company="">
//   
// </copyright>
// <summary>
//   The dialog do not show again.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The dialog do not show again.
    /// </summary>
    public partial class DialogDoNotShowAgain : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogDoNotShowAgain"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public DialogDoNotShowAgain(string title, string text)
        {
            this.InitializeComponent();

            Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);
            int titleBarHeight = screenRectangle.Top - this.Top;

            this.checkBoxDoNotDisplayAgain.Text = Configuration.Settings.Language.Main.DoNotDisplayMessageAgain;

            this.Text = title;
            this.labelText.Text = text;
            Utilities.FixLargeFonts(this, this.buttonOK);

            int width = Math.Max(this.checkBoxDoNotDisplayAgain.Width, this.labelText.Width);
            this.Width = width + this.buttonOK.Width + 75;
            this.Height = this.labelText.Top + this.labelText.Height + this.buttonOK.Height + titleBarHeight + 40;
        }

        /// <summary>
        /// Gets or sets a value indicating whether do no display again.
        /// </summary>
        public bool DoNoDisplayAgain { get; set; }

        /// <summary>
        /// The spell check completed_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SpellCheckCompleted_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.Cancel;
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
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The dialog do not show again_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DialogDoNotShowAgain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DoNoDisplayAgain = this.checkBoxDoNotDisplayAgain.Checked;
        }
    }
}