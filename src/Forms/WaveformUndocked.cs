// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaveformUndocked.cs" company="">
//   
// </copyright>
// <summary>
//   The waveform undocked.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The waveform undocked.
    /// </summary>
    public partial class WaveformUndocked : PositionAndSizeForm
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
        /// Initializes a new instance of the <see cref="WaveformUndocked"/> class.
        /// </summary>
        /// <param name="mainForm">
        /// The main form.
        /// </param>
        public WaveformUndocked(Main mainForm)
        {
            this.InitializeComponent();
            this._mainForm = mainForm;
            this.Icon = (Icon)mainForm.Icon.Clone();
            this._redockKeys = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveformUndocked"/> class.
        /// </summary>
        public WaveformUndocked()
        {
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
        /// The waveform undocked_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformUndocked_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.panelContainer.Controls.Count > 0)
            {
                var controlWaveform = this.panelContainer.Controls[0];
                var controlButtons = this.panelContainer.Controls[1];
                var controlTrackBar = this.panelContainer.Controls[2];
                this.panelContainer.Controls.Clear();
                this._mainForm.ReDockWaveform(controlWaveform, controlButtons, controlTrackBar);
                this._mainForm.SetWaveformToggleOff();
            }
        }

        /// <summary>
        /// The waveform undocked_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformUndocked_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._redockKeys == e.KeyData)
            {
                this._mainForm.RedockVideoControlsToolStripMenuItemClick(null, null);
            }
            else
            {
                this._mainForm.MainKeyDown(sender, e);
            }
        }
    }
}