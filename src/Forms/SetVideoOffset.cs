// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetVideoOffset.cs" company="">
//   
// </copyright>
// <summary>
//   The set video offset.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The set video offset.
    /// </summary>
    public partial class SetVideoOffset : PositionAndSizeForm
    {
        /// <summary>
        /// The _video offset.
        /// </summary>
        private TimeCode _videoOffset = new TimeCode(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="SetVideoOffset"/> class.
        /// </summary>
        public SetVideoOffset()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.SetVideoOffset.Title;
            this.labelDescription.Text = Configuration.Settings.Language.SetVideoOffset.Description;
            this.checkBoxFromCurrentPosition.Text = Configuration.Settings.Language.SetVideoOffset.RelativeToCurrentVideoPosition;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets or sets a value indicating whether from current video position.
        /// </summary>
        public bool FromCurrentVideoPosition { get; set; }

        /// <summary>
        /// Gets or sets the video offset.
        /// </summary>
        public TimeCode VideoOffset
        {
            get
            {
                return this._videoOffset;
            }

            set
            {
                this._videoOffset.TotalMilliseconds = value.TotalMilliseconds;
                this.timeUpDownVideoPosition.SetTotalMilliseconds(value.TotalMilliseconds);
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
            this.VideoOffset = this.timeUpDownVideoPosition.TimeCode;
            this.FromCurrentVideoPosition = this.checkBoxFromCurrentPosition.Checked;
            this.DialogResult = DialogResult.OK;
        }

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
    }
}