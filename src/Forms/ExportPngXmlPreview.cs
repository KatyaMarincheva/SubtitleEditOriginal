// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportPngXmlPreview.cs" company="">
//   
// </copyright>
// <summary>
//   The export png xml preview.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The export png xml preview.
    /// </summary>
    public sealed partial class ExportPngXmlPreview : Form
    {
        /// <summary>
        /// The _bmp.
        /// </summary>
        private readonly Bitmap _bmp;

        /// <summary>
        /// The _zoom factor.
        /// </summary>
        private double _zoomFactor = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPngXmlPreview"/> class.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        public ExportPngXmlPreview(Bitmap bmp)
        {
            this.InitializeComponent();

            this.DoubleBuffered = true;

            this._bmp = bmp;
            this.pictureBox1.Image = bmp;
            this.pictureBox1.Width = bmp.Width;
            this.pictureBox1.Height = bmp.Height;
            this.MaximumSize = new Size(bmp.Width + (this.Width - this.ClientSize.Width), bmp.Height + (this.Height - this.ClientSize.Height));
            if (Screen.PrimaryScreen.Bounds.Width > bmp.Width && Screen.PrimaryScreen.Bounds.Height > bmp.Height)
            {
                this.ClientSize = new Size(bmp.Width, bmp.Height);
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }

            this.pictureBox2.Width = 1;
            this.pictureBox2.Height = 1;
            this.pictureBox2.Top = bmp.Height - 2;

            this.Text = string.Format("{0} {1}x{2}", Configuration.Settings.Language.General.Preview, bmp.Width, bmp.Height);

            this.MouseWheel += this.MouseWheelHandler;
        }

        /// <summary>
        /// The mouse wheel handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            this.Zoom(e.Delta / 50.0);
        }

        /// <summary>
        /// The zoom.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        private void Zoom(double delta)
        {
            double newZoomFactor = this._zoomFactor += delta;
            if (newZoomFactor < 25)
            {
                this._zoomFactor = 25;
            }
            else if (newZoomFactor > 500)
            {
                this._zoomFactor = 500;
            }
            else
            {
                this._zoomFactor = newZoomFactor;
            }

            if (this._zoomFactor > 99 && this._zoomFactor < 101)
            {
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                this._zoomFactor = 100.0;
            }
            else
            {
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }

            this.pictureBox1.Width = (int)(this._bmp.Width * this._zoomFactor / 100.0);
            this.pictureBox1.Height = (int)(this._bmp.Height * this._zoomFactor / 100.0);

            this.Text = string.Format("{0}  {1}x{2}  {3}%", Configuration.Settings.Language.General.Preview, this._bmp.Width, this._bmp.Height, (int)this._zoomFactor);

            this.Invalidate();
        }

        /// <summary>
        /// The export png xml preview_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXmlPreview_Shown(object sender, EventArgs e)
        {
            this.panel1.ScrollControlIntoView(this.pictureBox2);
            this.pictureBox2.Visible = false;
        }

        /// <summary>
        /// The export png xml preview_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXmlPreview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (e.KeyCode == Keys.Add)
            {
                this.Zoom(10);
            }
            else if (e.KeyCode == Keys.Subtract)
            {
                this.Zoom(-10);
            }
            else if (e.KeyCode == Keys.Home)
            {
                this._zoomFactor = 100;
                this.Zoom(0);
            }
        }
    }
}