namespace Nikse.SubtitleEdit.Controls
{
    using System.Windows.Forms;

    public class RichTextBoxViewOnly : RichTextBox
    {
        public RichTextBoxViewOnly()
        {
            ReadOnly = true;
            BorderStyle = BorderStyle.None;
            TabStop = false;
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.UserMouse, true);
            MouseEnter += delegate { Cursor = Cursors.Default; };
            ScrollBars = RichTextBoxScrollBars.None;
            Margin = new Padding(0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x204) return; // WM_RBUTTONDOWN
            if (m.Msg == 0x205) return; // WM_RBUTTONUP
            base.WndProc(ref m);
        }
    }
}