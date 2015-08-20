// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusLog.cs" company="">
//   
// </copyright>
// <summary>
//   The status log.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The status log.
    /// </summary>
    public partial class StatusLog : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusLog"/> class.
        /// </summary>
        /// <param name="logText">
        /// The log text.
        /// </param>
        public StatusLog(string logText)
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.Main.StatusLog;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.textBoxStatusLog.Text = logText;
        }

        /// <summary>
        /// The status log_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void StatusLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}