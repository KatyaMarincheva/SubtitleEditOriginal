// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlignmentPicker.cs" company="">
//   
// </copyright>
// <summary>
//   The alignment picker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The alignment picker.
    /// </summary>
    public partial class AlignmentPicker : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlignmentPicker"/> class.
        /// </summary>
        public AlignmentPicker()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.SubStationAlphaStyles.Alignment;

            this.button1.Text = Configuration.Settings.Language.SubStationAlphaStyles.TopLeft;
            this.button2.Text = Configuration.Settings.Language.SubStationAlphaStyles.TopCenter;
            this.button3.Text = Configuration.Settings.Language.SubStationAlphaStyles.TopRight;

            this.button4.Text = Configuration.Settings.Language.SubStationAlphaStyles.MiddleLeft;
            this.button5.Text = Configuration.Settings.Language.SubStationAlphaStyles.MiddleCenter;
            this.button6.Text = Configuration.Settings.Language.SubStationAlphaStyles.MiddleRight;

            this.button7.Text = Configuration.Settings.Language.SubStationAlphaStyles.BottomLeft;
            this.button8.Text = Configuration.Settings.Language.SubStationAlphaStyles.BottomCenter;
            this.button9.Text = Configuration.Settings.Language.SubStationAlphaStyles.BottomRight;
        }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        public ContentAlignment Alignment { get; set; }

        /// <summary>
        /// The done.
        /// </summary>
        public void Done()
        {
            this.DialogResult = DialogResult.OK;
        }

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
            this.Alignment = ContentAlignment.TopLeft;
            this.Done();
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.TopCenter;
            this.Done();
        }

        /// <summary>
        /// The button 3_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.TopRight;
            this.Done();
        }

        /// <summary>
        /// The button 4_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.MiddleLeft;
            this.Done();
        }

        /// <summary>
        /// The button 5_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button5_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.MiddleCenter;
            this.Done();
        }

        /// <summary>
        /// The button 6_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button6_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.MiddleRight;
            this.Done();
        }

        /// <summary>
        /// The button 7_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button7_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.BottomLeft;
            this.Done();
        }

        /// <summary>
        /// The button 8_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button8_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.BottomCenter;
            this.Done();
        }

        /// <summary>
        /// The button 9_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button9_Click(object sender, EventArgs e)
        {
            this.Alignment = ContentAlignment.BottomRight;
            this.Done();
        }

        /// <summary>
        /// The alignment picker_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AlignmentPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The alignment picker_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AlignmentPicker_Shown(object sender, EventArgs e)
        {
            this.button8.Focus();
        }
    }
}