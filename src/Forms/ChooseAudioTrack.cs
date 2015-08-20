// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChooseAudioTrack.cs" company="">
//   
// </copyright>
// <summary>
//   The choose audio track.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The choose audio track.
    /// </summary>
    public partial class ChooseAudioTrack : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseAudioTrack"/> class.
        /// </summary>
        /// <param name="tracks">
        /// The tracks.
        /// </param>
        /// <param name="defaultTrack">
        /// The default track.
        /// </param>
        public ChooseAudioTrack(List<string> tracks, int defaultTrack)
        {
            this.InitializeComponent();
            foreach (string track in tracks)
            {
                this.listBoxTracks.Items.Add(track);
                if (this.listBoxTracks.Items.Count == defaultTrack)
                {
                    this.listBoxTracks.SelectedIndex = this.listBoxTracks.Items.Count - 1;
                }
            }

            if (this.listBoxTracks.SelectedIndex == -1 && this.listBoxTracks.Items.Count > 0)
            {
                this.listBoxTracks.SelectedIndex = 0;
            }

            this.Text = Configuration.Settings.Language.ChooseAudioTrack.Title;
            this.labelDescr.Text = Configuration.Settings.Language.ChooseAudioTrack.Title;
        }

        /// <summary>
        /// Gets or sets the selected track.
        /// </summary>
        public int SelectedTrack { get; set; }

        /// <summary>
        /// The choose audio track_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChooseAudioTrack_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
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
            this.SelectedTrack = this.listBoxTracks.SelectedIndex;
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