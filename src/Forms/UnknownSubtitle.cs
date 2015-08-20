// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The unknown subtitle.
    /// </summary>
    public sealed partial class UnknownSubtitle : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownSubtitle"/> class.
        /// </summary>
        public UnknownSubtitle()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.UnknownSubtitle.Title;
            this.labelTitle.Text = Configuration.Settings.Language.UnknownSubtitle.Title;
            this.richTextBoxMessage.Text = Configuration.Settings.Language.UnknownSubtitle.Message;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void Initialize(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// The form unknown subtitle_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormUnknownSubtitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}