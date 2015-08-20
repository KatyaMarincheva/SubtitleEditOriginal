// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebVttNewVoice.cs" company="">
//   
// </copyright>
// <summary>
//   The web vtt new voice.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The web vtt new voice.
    /// </summary>
    public partial class WebVttNewVoice : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebVttNewVoice"/> class.
        /// </summary>
        public WebVttNewVoice()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.WebVttNewVoice.Title;
            this.labelVoiceName.Text = Configuration.Settings.Language.WebVttNewVoice.VoiceName;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets or sets the voice name.
        /// </summary>
        public string VoiceName { get; set; }

        /// <summary>
        /// The web vtt new voice_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebVttNewVoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                this.VoiceName = this.textBox1.Text;
                this.DialogResult = DialogResult.OK;
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
            this.VoiceName = this.textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}