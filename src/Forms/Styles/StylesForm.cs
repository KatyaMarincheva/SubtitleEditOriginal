// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StylesForm.cs" company="">
//   
// </copyright>
// <summary>
//   The styles form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms.Styles
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The styles form.
    /// </summary>
    public /* abstract */ class StylesForm : Form
    {
        /// <summary>
        /// The _preview timer.
        /// </summary>
        private readonly Timer _previewTimer = new Timer();

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private readonly Subtitle _subtitle;

        /// <summary>
        /// Prevents a default instance of the <see cref="StylesForm"/> class from being created.
        /// </summary>
        private StylesForm()
        {
            // Only used by the Visual Studio designer.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StylesForm"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        protected StylesForm(Subtitle subtitle)
        {
            this._subtitle = subtitle;

            this._previewTimer.Interval = 200;
            this._previewTimer.Tick += this.PreviewTimerTick;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual string Header
        {
            get
            {
                throw new NotImplementedException("This property getter has to be overridden.");
            }
        }

        /// <summary>
        /// Gets the subtitle.
        /// </summary>
        protected Subtitle Subtitle
        {
            get
            {
                return this._subtitle;
            }
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        protected void GeneratePreview()
        {
            if (this._previewTimer.Enabled)
            {
                this._previewTimer.Stop();
                this._previewTimer.Start();
            }
            else
            {
                this._previewTimer.Start();
            }
        }

        /// <summary>
        /// The generate preview real.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected virtual void GeneratePreviewReal()
        {
            throw new NotImplementedException("This method has to be overridden.");
        }

        /// <summary>
        /// The preview timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PreviewTimerTick(object sender, EventArgs e)
        {
            this._previewTimer.Stop();
            this.GeneratePreviewReal();
        }
    }
}