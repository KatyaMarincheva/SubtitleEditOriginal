// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RichTextBoxViewOnly.cs" company="">
//   
// </copyright>
// <summary>
//   The rich text box view only.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System.Windows.Forms;

    /// <summary>
    /// The rich text box view only.
    /// </summary>
    public class RichTextBoxViewOnly : RichTextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxViewOnly"/> class.
        /// </summary>
        public RichTextBoxViewOnly()
        {
            this.ReadOnly = true;
            this.BorderStyle = BorderStyle.None;
            this.TabStop = false;
            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.UserMouse, true);
            this.MouseEnter += delegate { Cursor = Cursors.Default; };
            this.ScrollBars = RichTextBoxScrollBars.None;
            this.Margin = new Padding(0);
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x204)
            {
                return; // WM_RBUTTONDOWN
            }

            if (m.Msg == 0x205)
            {
                return; // WM_RBUTTONUP
            }

            base.WndProc(ref m);
        }
    }
}