// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoControlsUndocked.cs" company="">
//   
// </copyright>
// <summary>
//   The video controls undocked.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The video controls undocked.
    /// </summary>
    public partial class VideoControlsUndocked : PositionAndSizeForm
    {
        /// <summary>
        /// The _main form.
        /// </summary>
        private Main _mainForm = null;

        /// <summary>
        /// The _redock keys.
        /// </summary>
        private Keys _redockKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoControlsUndocked"/> class.
        /// </summary>
        /// <param name="mainForm">
        /// The main form.
        /// </param>
        public VideoControlsUndocked(Main mainForm)
        {
            this.InitializeComponent();
            this._mainForm = mainForm;
            this.Icon = (Icon)mainForm.Icon.Clone();
            this._redockKeys = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoControlsUndocked"/> class.
        /// </summary>
        public VideoControlsUndocked()
        {
            // TODO: Complete member initialization
        }

        /// <summary>
        /// Gets the panel container.
        /// </summary>
        public Panel PanelContainer
        {
            get
            {
                return this.panelContainer;
            }
        }

        /// <summary>
        /// The video controls undocked_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoControlsUndocked_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.panelContainer.Controls.Count > 0)
            {
                this.ReDock();
            }
        }

        /// <summary>
        /// The re dock.
        /// </summary>
        private void ReDock()
        {
            var control = this.panelContainer.Controls[0];
            var controlCheckBox = this.panelContainer.Controls[1];
            this.panelContainer.Controls.Clear();
            this._mainForm.ReDockVideoButtons(control, controlCheckBox);
        }

        /// <summary>
        /// The video controls undocked_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoControlsUndocked_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._redockKeys == e.KeyData)
            {
                this._mainForm.RedockVideoControlsToolStripMenuItemClick(null, null);
            }
        }
    }
}