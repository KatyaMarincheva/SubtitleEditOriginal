// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FcpProperties.cs" company="">
//   
// </copyright>
// <summary>
//   The fcp properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The fcp properties.
    /// </summary>
    public partial class FcpProperties : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FcpProperties"/> class.
        /// </summary>
        public FcpProperties()
        {
            this.InitializeComponent();
            this.textBoxFontName.Text = Configuration.Settings.SubtitleSettings.FcpFontName;
            try
            {
                this.numericUpDownFontSize.Value = Configuration.Settings.SubtitleSettings.FcpFontSize;
            }
            catch
            {
                this.numericUpDownFontSize.Value = 18;
            }
        }

        /// <summary>
        /// Gets or sets the fcp font size.
        /// </summary>
        public int FcpFontSize { get; set; }

        /// <summary>
        /// Gets or sets the fcp font name.
        /// </summary>
        public string FcpFontName { get; set; }

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
            this.FcpFontName = this.textBoxFontName.Text;
            this.FcpFontSize = (int)this.numericUpDownFontSize.Value;
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

        /// <summary>
        /// The fcp properties_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FcpProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}