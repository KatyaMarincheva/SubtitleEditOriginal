// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChooseResolution.cs" company="">
//   
// </copyright>
// <summary>
//   The choose resolution.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The choose resolution.
    /// </summary>
    public partial class ChooseResolution : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseResolution"/> class.
        /// </summary>
        public ChooseResolution()
        {
            this.InitializeComponent();
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.Text = Configuration.Settings.Language.ExportPngXml.VideoResolution;
            this.labelVideoResolution.Text = Configuration.Settings.Language.ExportPngXml.VideoResolution;
        }

        /// <summary>
        /// Gets or sets the video width.
        /// </summary>
        public int VideoWidth { get; set; }

        /// <summary>
        /// Gets or sets the video height.
        /// </summary>
        public int VideoHeight { get; set; }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenVideoFileTitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(false);
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoInfo info = Utilities.GetVideoInfo(this.openFileDialog1.FileName);
                if (info != null && info.Success)
                {
                    this.numericUpDownVideoWidth.Value = info.Width;
                    this.numericUpDownVideoHeight.Value = info.Height;
                }
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
            this.DialogResult = DialogResult.OK;
            this.VideoWidth = (int)this.numericUpDownVideoWidth.Value;
            this.VideoHeight = (int)this.numericUpDownVideoHeight.Value;
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

        /// <summary>
        /// The choose resolution_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChooseResolution_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}