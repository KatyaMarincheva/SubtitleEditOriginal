// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionAndSizeForm.cs" company="">
//   
// </copyright>
// <summary>
//   The position and size form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The position and size form.
    /// </summary>
    public /* abstract */ class PositionAndSizeForm : Form
    {
        /// <summary>
        /// The _positions and sizes.
        /// </summary>
        private static readonly Dictionary<string, Rectangle> _positionsAndSizes = new Dictionary<string, Rectangle>();

        /// <summary>
        /// Gets a value indicating whether is position and size saved.
        /// </summary>
        public bool IsPositionAndSizeSaved
        {
            get
            {
                return _positionsAndSizes.ContainsKey(this.Name);
            }
        }

        /// <summary>
        /// The set position and size.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        public static void SetPositionAndSize(string name, Rectangle bounds)
        {
            if (_positionsAndSizes.ContainsKey(name))
            {
                _positionsAndSizes[name] = bounds;
            }
            else
            {
                _positionsAndSizes.Add(name, bounds);
            }
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            Rectangle ps;
            if (_positionsAndSizes.TryGetValue(this.Name, out ps))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Bounds = ps;
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// The on form closed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SetPositionAndSize(this.Name, this.Bounds);
            base.OnFormClosed(e);
        }
    }
}