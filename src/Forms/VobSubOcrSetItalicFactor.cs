// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubOcrSetItalicFactor.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub ocr set italic factor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The vob sub ocr set italic factor.
    /// </summary>
    public sealed partial class VobSubOcrSetItalicFactor : Form
    {
        /// <summary>
        /// The _bmp.
        /// </summary>
        private Bitmap _bmp;

        /// <summary>
        /// The _factor.
        /// </summary>
        private double _factor;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcrSetItalicFactor"/> class.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="factor">
        /// The factor.
        /// </param>
        public VobSubOcrSetItalicFactor(Bitmap bmp, double factor)
        {
            this.InitializeComponent();

            this._bmp = bmp;
            this._factor = factor;
            this.numericUpDown1.Value = (decimal)factor;

            this.Text = Configuration.Settings.Language.VobSubOcrSetItalicFactor.Title;
            this.labelDescription.Text = Configuration.Settings.Language.VobSubOcrSetItalicFactor.Description;
            this.saveImageAsToolStripMenuItem.Text = Configuration.Settings.Language.VobSubOcr.SaveSubtitleImageAs;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
        }

        /// <summary>
        /// The numeric up down 1_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.pictureBoxSubtitleImage.Image = VobSubOcr.UnItalic(this._bmp, (double)this.numericUpDown1.Value);
        }

        /// <summary>
        /// The get un italic factor.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        internal double GetUnItalicFactor()
        {
            return this._factor;
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
            this._factor = (double)this.numericUpDown1.Value;
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
        /// The vob sub ocr set italic factor_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcrSetItalicFactor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The save image as tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Title = Configuration.Settings.Language.VobSubOcr.SaveSubtitleImageAs;
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.FileName = "ImageUnItalic";
            this.saveFileDialog1.Filter = "PNG image|*.png|BMP image|*.bmp|GIF image|*.gif|TIFF image|*.tiff";
            this.saveFileDialog1.FilterIndex = 0;

            DialogResult result = this.saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var bmp = (Bitmap)this.pictureBoxSubtitleImage.Image;
                if (bmp == null)
                {
                    MessageBox.Show("No image!");
                    return;
                }

                try
                {
                    if (this.saveFileDialog1.FilterIndex == 0)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 1)
                    {
                        bmp.Save(this.saveFileDialog1.FileName);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 2)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
    }
}