// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SETextBox.cs" company="">
//   
// </copyright>
// <summary>
//   TextBox where double click selects current word
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// TextBox where double click selects current word
    /// </summary>
    public class SETextBox : TextBox
    {
        /// <summary>
        /// The break chars.
        /// </summary>
        private const string BreakChars = " \".!?,)([]<>:;♪{}-/#*|¿¡\r\n\t";

        /// <summary>
        /// The _drag from this.
        /// </summary>
        private bool _dragFromThis = false;

        /// <summary>
        /// The _drag remove old.
        /// </summary>
        private bool _dragRemoveOld = false;

        /// <summary>
        /// The _drag start from.
        /// </summary>
        private int _dragStartFrom = 0;

        /// <summary>
        /// The _drag start ticks.
        /// </summary>
        private long _dragStartTicks = 0;

        /// <summary>
        /// The _drag text.
        /// </summary>
        private string _dragText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SETextBox"/> class.
        /// </summary>
        public SETextBox()
        {
            this.AllowDrop = true;
            this.DragEnter += this.SETextBox_DragEnter;

            // DragOver += SETextBox_DragOver; could draw some gfx where drop position is...
            this.DragDrop += this.SETextBox_DragDrop;
            this.MouseDown += this.SETextBox_MouseDown;
            this.MouseUp += this.SETextBox_MouseUp;
            this.KeyDown += this.SETextBox_KeyDown;
        }

        /// <summary>
        /// The se text box_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SETextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Back)
            {
                int index = this.SelectionStart;
                if (this.SelectionLength == 0)
                {
                    string s = this.Text;
                    int deleteFrom = index - 1;

                    if (deleteFrom > 0 && deleteFrom < s.Length)
                    {
                        if (s[deleteFrom] == ' ')
                        {
                            deleteFrom--;
                        }

                        while (deleteFrom > 0 && !BreakChars.Contains(s[deleteFrom]))
                        {
                            deleteFrom--;
                        }

                        if (deleteFrom == index - 1)
                        {
                            var breakCharsNoSpace = BreakChars.Substring(1);
                            while (deleteFrom > 0 && breakCharsNoSpace.Contains(s[deleteFrom - 1]))
                            {
                                deleteFrom--;
                            }
                        }

                        if (s[deleteFrom] == ' ')
                        {
                            deleteFrom++;
                        }

                        this.Text = s.Remove(deleteFrom, index - deleteFrom);
                        this.SelectionStart = deleteFrom;
                    }
                }

                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The se text box_ mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SETextBox_MouseUp(object sender, MouseEventArgs e)
        {
            this._dragRemoveOld = false;
            this._dragFromThis = false;
        }

        /// <summary>
        /// The se text box_ mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SETextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left && !string.IsNullOrEmpty(this._dragText))
            {
                Point pt = new Point(e.X, e.Y);
                int index = this.GetCharIndexFromPosition(pt);
                if (index >= this._dragStartFrom && index <= this._dragStartFrom + this._dragText.Length)
                {
                    // re-make selection
                    this.SelectionStart = this._dragStartFrom;
                    this.SelectionLength = this._dragText.Length;

                    DataObject dataObject = new DataObject();
                    dataObject.SetText(this._dragText, TextDataFormat.UnicodeText);
                    dataObject.SetText(this._dragText, TextDataFormat.Text);

                    this._dragFromThis = true;
                    if (ModifierKeys == Keys.Control)
                    {
                        this._dragRemoveOld = false;
                        this.DoDragDrop(dataObject, DragDropEffects.Copy);
                    }
                    else if (ModifierKeys == Keys.None)
                    {
                        this._dragRemoveOld = true;
                        this.DoDragDrop(dataObject, DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// The se text box_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SETextBox_DragDrop(object sender, DragEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            pt = this.PointToClient(pt);
            int index = this.GetCharIndexFromPosition(pt);

            string newText = string.Empty;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                newText = (string)e.Data.GetData(DataFormats.UnicodeText);
            }
            else
            {
                newText = (string)e.Data.GetData(DataFormats.Text);
            }

            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = newText;
            }
            else
            {
                bool justAppend = index == this.Text.Length - 1 && index > 0;
                const string expectedChars = @";:]<.!?";
                if (this._dragFromThis)
                {
                    this._dragFromThis = false;
                    long milliseconds = (DateTime.Now.Ticks - this._dragStartTicks) / 10000;
                    if (milliseconds < 400)
                    {
                        this.SelectionLength = 0;
                        if (index == this.Text.Length - 1 && index > 0)
                        {
                            index++;
                        }

                        this.SelectionStart = index;
                        return; // too fast - nobody can drag'n'drop this fast
                    }

                    if (index >= this._dragStartFrom && index <= this._dragStartFrom + this._dragText.Length)
                    {
                        return; // don't drop same text at same position
                    }

                    if (this._dragRemoveOld)
                    {
                        this._dragRemoveOld = false;
                        this.Text = this.Text.Remove(this._dragStartFrom, this._dragText.Length);

                        // fix spaces
                        if (this._dragStartFrom == 0 && this.Text.Length > 0 && this.Text[0] == ' ')
                        {
                            this.Text = this.Text.Remove(0, 1);
                            index--;
                        }
                        else if (this._dragStartFrom > 1 && this.Text.Length > this._dragStartFrom + 1 && this.Text[this._dragStartFrom] == ' ' && this.Text[this._dragStartFrom - 1] == ' ')
                        {
                            this.Text = this.Text.Remove(this._dragStartFrom, 1);
                            if (this._dragStartFrom < index)
                            {
                                index--;
                            }
                        }
                        else if (this._dragStartFrom > 0 && this.Text.Length > this._dragStartFrom + 1 && this.Text[this._dragStartFrom] == ' ' && expectedChars.Contains(this.Text[this._dragStartFrom + 1]))
                        {
                            this.Text = this.Text.Remove(this._dragStartFrom, 1);
                            if (this._dragStartFrom < index)
                            {
                                index--;
                            }
                        }

                        // fix index
                        if (index > this._dragStartFrom)
                        {
                            index -= this._dragText.Length;
                        }

                        if (index < 0)
                        {
                            index = 0;
                        }
                    }
                }

                if (justAppend)
                {
                    index = this.Text.Length;
                    this.Text += newText;
                }
                else
                {
                    this.Text = this.Text.Insert(index, newText);
                }

                // fix start spaces
                int endIndex = index + newText.Length;
                if (index > 0 && !newText.StartsWith(' ') && this.Text[index - 1] != ' ')
                {
                    this.Text = this.Text.Insert(index, " ");
                    endIndex++;
                }
                else if (index > 0 && newText.StartsWith(' ') && this.Text[index - 1] == ' ')
                {
                    this.Text = this.Text.Remove(index, 1);
                    endIndex--;
                }

                // fix end spaces
                if (endIndex < this.Text.Length && !newText.EndsWith(' ') && this.Text[endIndex] != ' ')
                {
                    bool lastWord = expectedChars.Contains(this.Text[endIndex]);
                    if (!lastWord)
                    {
                        this.Text = this.Text.Insert(endIndex, " ");
                    }
                }
                else if (endIndex < this.Text.Length && newText.EndsWith(' ') && this.Text[endIndex] == ' ')
                {
                    this.Text = this.Text.Remove(endIndex, 1);
                }

                this.SelectionStart = index + 1;
                this.SelectCurrentWord();
            }

            this._dragRemoveOld = false;
            this._dragFromThis = false;
        }

        /// <summary>
        /// The se text box_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SETextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                if (ModifierKeys == Keys.Control)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            const int WM_DBLCLICK = 0xA3;
            const int WM_LBUTTONDBLCLK = 0x203;
            const int WM_LBUTTONDOWN = 0x0201;

            if (m.Msg == WM_DBLCLICK || m.Msg == WM_LBUTTONDBLCLK)
            {
                this.SelectCurrentWord();
                return;
            }

            if (m.Msg == WM_LBUTTONDOWN)
            {
                this._dragText = this.SelectedText;
                this._dragStartFrom = this.SelectionStart;
                this._dragStartTicks = DateTime.Now.Ticks;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// The select current word.
        /// </summary>
        private void SelectCurrentWord()
        {
            var i = this.SelectionStart;
            while (i > 0 && !BreakChars.Contains(this.Text[i - 1]))
            {
                i--;
            }

            this.SelectionStart = i;

            var selectionLength = 0;
            for (; i < this.Text.Length; i++)
            {
                if (BreakChars.Contains(this.Text[i]))
                {
                    break;
                }

                selectionLength++;
            }

            this.SelectionLength = selectionLength;
        }
    }
}