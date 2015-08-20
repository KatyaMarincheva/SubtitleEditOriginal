// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubNOcrEdit.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub n ocr edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Ocr;

    /// <summary>
    /// The vob sub n ocr edit.
    /// </summary>
    public partial class VobSubNOcrEdit : Form
    {
        /// <summary>
        /// The _bitmap.
        /// </summary>
        private Bitmap _bitmap;

        /// <summary>
        /// The _draw line on.
        /// </summary>
        private bool _drawLineOn;

        /// <summary>
        /// The _end.
        /// </summary>
        private Point _end;

        /// <summary>
        /// The _history.
        /// </summary>
        private List<NOcrChar> _history = new List<NOcrChar>();

        /// <summary>
        /// The _history index.
        /// </summary>
        private int _historyIndex = -1;

        /// <summary>
        /// The _mx.
        /// </summary>
        private int _mx;

        /// <summary>
        /// The _my.
        /// </summary>
        private int _my;

        /// <summary>
        /// The _nocr char.
        /// </summary>
        private NOcrChar _nocrChar;

        /// <summary>
        /// The _nocr chars.
        /// </summary>
        private List<NOcrChar> _nocrChars;

        /// <summary>
        /// The _start.
        /// </summary>
        private Point _start;

        /// <summary>
        /// The _start done.
        /// </summary>
        private bool _startDone;

        /// <summary>
        /// The _zoom factor.
        /// </summary>
        private double _zoomFactor = 5.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubNOcrEdit"/> class.
        /// </summary>
        /// <param name="nocrChars">
        /// The nocr chars.
        /// </param>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        public VobSubNOcrEdit(List<NOcrChar> nocrChars, Bitmap bitmap)
        {
            this.InitializeComponent();

            this._nocrChars = nocrChars;
            this._bitmap = bitmap;

            this.FillComboBox();

            if (bitmap != null)
            {
                this.pictureBoxCharacter.Image = bitmap;
                this.SizePictureBox();
            }

            this.labelInfo.Text = string.Format("{0} elements in database", nocrChars.Count);
            this.labelNOcrCharInfo.Text = string.Empty;
        }

        /// <summary>
        /// The fill combo box.
        /// </summary>
        private void FillComboBox()
        {
            List<string> list = new List<string>();
            foreach (NOcrChar c in this._nocrChars)
            {
                if (!list.Contains(c.Text))
                {
                    list.Add(c.Text);
                }
            }

            list.Sort();
            this.comboBoxTexts.Items.Clear();
            foreach (string s in list)
            {
                this.comboBoxTexts.Items.Add(s);
            }
        }

        /// <summary>
        /// The vob sub n ocr edit_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubNOcrEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this._drawLineOn = false;
                this._startDone = false;
                this.pictureBoxCharacter.Invalidate();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
            {
                e.SuppressKeyPress = true;
                this.Undo();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y)
            {
                e.SuppressKeyPress = true;
                this.Redo();
            }
        }

        /// <summary>
        /// The size picture box.
        /// </summary>
        private void SizePictureBox()
        {
            if (this.pictureBoxCharacter.Image != null)
            {
                Bitmap bmp = this.pictureBoxCharacter.Image as Bitmap;
                this.pictureBoxCharacter.SizeMode = PictureBoxSizeMode.StretchImage;
                this.pictureBoxCharacter.Width = (int)Math.Round(bmp.Width * this._zoomFactor);
                this.pictureBoxCharacter.Height = (int)Math.Round(bmp.Height * this._zoomFactor);
                this.pictureBoxCharacter.Invalidate();
            }
        }

        /// <summary>
        /// The button zoom in_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            if (this._zoomFactor < 20)
            {
                this._zoomFactor++;
                this.SizePictureBox();
            }
        }

        /// <summary>
        /// The button zoom out_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            if (this._zoomFactor > 1)
            {
                this._zoomFactor--;
                this.SizePictureBox();
            }
        }

        /// <summary>
        /// The show ocr points.
        /// </summary>
        private void ShowOcrPoints()
        {
            this.listBoxLinesForeground.Items.Clear();
            foreach (NOcrPoint op in this._nocrChar.LinesForeground)
            {
                this.listBoxLinesForeground.Items.Add(op);
            }

            this.listBoxlinesBackground.Items.Clear();
            foreach (NOcrPoint op in this._nocrChar.LinesBackground)
            {
                this.listBoxlinesBackground.Items.Add(op);
            }

            this.pictureBoxCharacter.Invalidate();
        }

        /// <summary>
        /// The is match.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsMatch()
        {
            NikseBitmap nbmp = new NikseBitmap(this.pictureBoxCharacter.Image as Bitmap);
            foreach (NOcrPoint op in this._nocrChar.LinesForeground)
            {
                foreach (Point point in op.ScaledGetPoints(this._nocrChar, nbmp.Width, nbmp.Height))
                {
                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                    {
                        Color c = nbmp.GetPixel(point.X, point.Y);
                        if (c.A > 150 && c.R + c.G + c.B > VobSubOcr.NocrMinColor)
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            foreach (NOcrPoint op in this._nocrChar.LinesBackground)
            {
                foreach (Point point in op.ScaledGetPoints(this._nocrChar, nbmp.Width, nbmp.Height))
                {
                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                    {
                        Color c = nbmp.GetPixel(point.X, point.Y);
                        if (c.A > 150 && c.R + c.G + c.B > VobSubOcr.NocrMinColor)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The list box file names_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNOcrCharInfo.Text = string.Empty;
            if (this.listBoxFileNames.SelectedIndex < 0)
            {
                return;
            }

            this._nocrChar = this.listBoxFileNames.Items[this.listBoxFileNames.SelectedIndex] as NOcrChar;
            if (this._nocrChar == null)
            {
                this.pictureBoxCharacter.Invalidate();
                this.groupBoxCurrentCompareImage.Enabled = false;
                this.listBoxLinesForeground.Items.Clear();
                this.listBoxlinesBackground.Items.Clear();
            }
            else
            {
                this.textBoxText.Text = this._nocrChar.Text;
                this.checkBoxItalic.Checked = this._nocrChar.Italic;
                this.pictureBoxCharacter.Invalidate();
                this.groupBoxCurrentCompareImage.Enabled = true;
                this.labelNOcrCharInfo.Text = string.Format("Size: {0}x{1}, margin top: {2} ", this._nocrChar.Width, this._nocrChar.Height, this._nocrChar.MarginTop);

                if (this.pictureBoxCharacter.Image != null)
                {
                    if (this.IsMatch())
                    {
                        this.groupBoxCurrentCompareImage.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        this.groupBoxCurrentCompareImage.BackColor = DefaultBackColor;
                    }
                }

                this._drawLineOn = false;
                this._history = new List<NOcrChar>();
                this._historyIndex = -1;

                if (this._bitmap == null)
                {
                    var bitmap = new Bitmap(this._nocrChar.Width, this._nocrChar.Height);
                    var nbmp = new NikseBitmap(bitmap);
                    nbmp.Fill(Color.White);
                    this.pictureBoxCharacter.Image = nbmp.GetBitmap();
                    this.SizePictureBox();
                    this.ShowOcrPoints();
                    bitmap.Dispose();
                }
            }
        }

        /// <summary>
        /// The combo box texts_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxTexts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxTexts.SelectedIndex < 0)
            {
                return;
            }

            this.listBoxFileNames.Items.Clear();
            string text = this.comboBoxTexts.Items[this.comboBoxTexts.SelectedIndex].ToString();
            foreach (NOcrChar c in this._nocrChars)
            {
                if (c.Text == text)
                {
                    this.listBoxFileNames.Items.Add(c);
                }
            }

            if (this.listBoxFileNames.Items.Count > 0)
            {
                this.listBoxFileNames.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The picture box character_ paint.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pictureBoxCharacter_Paint(object sender, PaintEventArgs e)
        {
            if (this._nocrChar == null)
            {
                return;
            }

            NOcrPoint selectedPoint = null;
            if (this.listBoxLinesForeground.Focused && this.listBoxLinesForeground.SelectedIndex >= 0)
            {
                selectedPoint = (NOcrPoint)this.listBoxLinesForeground.Items[this.listBoxLinesForeground.SelectedIndex];
            }
            else if (this.listBoxlinesBackground.Focused && this.listBoxlinesBackground.SelectedIndex >= 0)
            {
                selectedPoint = (NOcrPoint)this.listBoxlinesBackground.Items[this.listBoxlinesBackground.SelectedIndex];
            }

            var foreground = new Pen(new SolidBrush(Color.Green));
            var background = new Pen(new SolidBrush(Color.Red));
            var selPenF = new Pen(new SolidBrush(Color.Green), 3);
            var selPenB = new Pen(new SolidBrush(Color.Red), 3);
            if (this.pictureBoxCharacter.Image != null)
            {
                foreach (NOcrPoint op in this._nocrChar.LinesForeground)
                {
                    Point start = op.GetScaledStart(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height);
                    Point end = op.GetScaledEnd(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height);
                    if (start.X == end.X && start.Y == end.Y)
                    {
                        end.X++;
                    }

                    e.Graphics.DrawLine(foreground, start, end);
                    if (op == selectedPoint)
                    {
                        e.Graphics.DrawLine(selPenF, op.GetScaledStart(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height), op.GetScaledEnd(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height));
                    }
                }

                foreach (NOcrPoint op in this._nocrChar.LinesBackground)
                {
                    Point start = op.GetScaledStart(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height);
                    Point end = op.GetScaledEnd(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height);
                    e.Graphics.DrawLine(background, start, end);
                    if (op == selectedPoint)
                    {
                        e.Graphics.DrawLine(selPenB, op.GetScaledStart(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height), op.GetScaledEnd(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height));
                    }
                }
            }

            if (this._drawLineOn)
            {
                if (this._startDone)
                {
                    var p = foreground;
                    if (this.radioButtonCold.Checked)
                    {
                        p = background;
                    }

                    e.Graphics.DrawLine(p, new Point((int)Math.Round(this._start.X * this._zoomFactor), (int)Math.Round(this._start.Y * this._zoomFactor)), new Point(this._mx, this._my));
                }
            }

            foreground.Dispose();
            background.Dispose();
            selPenF.Dispose();
            selPenB.Dispose();
        }

        /// <summary>
        /// The button delete_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (this.listBoxFileNames.Items.Count == 0 || this._nocrChar == null)
            {
                return;
            }

            this._nocrChars.Remove(this._nocrChar);
            this.FillComboBox();
            if (this.comboBoxTexts.Items.Count > 0)
            {
                this.comboBoxTexts.SelectedIndex = 0;
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
        /// The add history item.
        /// </summary>
        /// <param name="nocrChar">
        /// The nocr char.
        /// </param>
        private void AddHistoryItem(NOcrChar nocrChar)
        {
            if (this._historyIndex > 0 && this._historyIndex < this._history.Count - 1)
            {
                while (this._history.Count > this._historyIndex + 1)
                {
                    this._history.RemoveAt(this._history.Count - 1);
                }

                this._historyIndex = this._history.Count - 1;
            }

            this._history.Add(new NOcrChar(nocrChar));
            this._historyIndex++;
        }

        /// <summary>
        /// The redo.
        /// </summary>
        private void Redo()
        {
            if (this._history.Count > 0 && this._historyIndex < this._history.Count - 1)
            {
                this._historyIndex++;
                this._nocrChar = new NOcrChar(this._history[this._historyIndex]);
                this.ShowOcrPoints();
            }
        }

        /// <summary>
        /// The undo.
        /// </summary>
        private void Undo()
        {
            this._drawLineOn = false;
            this._startDone = false;
            if (this._history.Count > 0 && this._historyIndex > 0)
            {
                this._historyIndex--;
                this._nocrChar = new NOcrChar(this._history[this._historyIndex]);
            }
            else if (this._historyIndex == 0)
            {
                var c = new NOcrChar(this._nocrChar);
                c.LinesForeground.Clear();
                c.LinesBackground.Clear();
                this._nocrChar = c;
                this._historyIndex--;
            }

            this.ShowOcrPoints();
        }

        /// <summary>
        /// The picture box character_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pictureBoxCharacter_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._drawLineOn)
            {
                if (this._startDone)
                {
                    this._end = new Point((int)Math.Round(e.Location.X / this._zoomFactor), (int)Math.Round(e.Location.Y / this._zoomFactor));
                    this._nocrChar.Width = this.pictureBoxCharacter.Image.Width;
                    this._nocrChar.Height = this.pictureBoxCharacter.Image.Height;
                    if (this.radioButtonHot.Checked)
                    {
                        this._nocrChar.LinesForeground.Add(new NOcrPoint(this._start, this._end));
                    }
                    else
                    {
                        this._nocrChar.LinesBackground.Add(new NOcrPoint(this._start, this._end));
                    }

                    this._drawLineOn = false;
                    this.pictureBoxCharacter.Invalidate();
                    this.ShowOcrPoints();
                    this.AddHistoryItem(this._nocrChar);

                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        this._start = new Point((int)Math.Round(e.Location.X / this._zoomFactor), (int)Math.Round(e.Location.Y / this._zoomFactor));
                        this._startDone = true;
                        this._drawLineOn = true;
                        this.pictureBoxCharacter.Invalidate();
                    }
                }
                else
                {
                    this._start = new Point((int)Math.Round(e.Location.X / this._zoomFactor), (int)Math.Round(e.Location.Y / this._zoomFactor));
                    this._startDone = true;
                    this.pictureBoxCharacter.Invalidate();
                }
            }
            else
            {
                this._startDone = false;
                this._drawLineOn = true;
                this._start = new Point((int)Math.Round(e.Location.X / this._zoomFactor), (int)Math.Round(e.Location.Y / this._zoomFactor));
                this._startDone = true;
                this.pictureBoxCharacter.Invalidate();
            }
        }

        /// <summary>
        /// The picture box character_ mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pictureBoxCharacter_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._drawLineOn)
            {
                this._mx = e.X;
                this._my = e.Y;
                this.pictureBoxCharacter.Invalidate();
            }
        }

        /// <summary>
        /// The list box lines foreground_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxLinesForeground_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pictureBoxCharacter.Invalidate();
        }

        /// <summary>
        /// The list boxlines background_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxlinesBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pictureBoxCharacter.Invalidate();
        }

        /// <summary>
        /// The check box italic_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxItalic_CheckedChanged(object sender, EventArgs e)
        {
            if (this._nocrChar != null)
            {
                this._nocrChar.Italic = this.checkBoxItalic.Checked;
            }
        }

        /// <summary>
        /// The text box text_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxText_TextChanged(object sender, EventArgs e)
        {
            if (this._nocrChar != null)
            {
                this._nocrChar.Text = this.textBoxText.Text;
            }
        }

        /// <summary>
        /// The remove foreground tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeForegroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBoxLinesForeground.SelectedItems.Count == 1)
            {
                var op = this.listBoxLinesForeground.Items[this.listBoxLinesForeground.SelectedIndex] as NOcrPoint;
                this._nocrChar.LinesForeground.Remove(op);
            }

            this.ShowOcrPoints();
        }

        /// <summary>
        /// The remove back tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBoxlinesBackground.SelectedItems.Count == 1)
            {
                var op = this.listBoxlinesBackground.Items[this.listBoxlinesBackground.SelectedIndex] as NOcrPoint;
                this._nocrChar.LinesBackground.Remove(op);
            }

            this.ShowOcrPoints();
        }

        /// <summary>
        /// The button import_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            int importedCount = 0;
            int notImportedCount = 0;
            this.openFileDialog1.Filter = "nOCR files|*.nocr";
            this.openFileDialog1.InitialDirectory = Configuration.DataDirectory;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Title = "Import existing nOCR database into current";
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                NOcrDb newDb = new NOcrDb(this.openFileDialog1.FileName);
                foreach (NOcrChar newChar in newDb.OcrCharacters)
                {
                    bool found = false;
                    foreach (NOcrChar oldChar in this._nocrChars)
                    {
                        if (oldChar.Text == newChar.Text && oldChar.Width == newChar.Width && oldChar.Height == newChar.Height && oldChar.MarginTop == newChar.MarginTop && oldChar.ExpandCount == newChar.ExpandCount && oldChar.LinesForeground.Count == newChar.LinesForeground.Count && oldChar.LinesBackground.Count == newChar.LinesBackground.Count)
                        {
                            found = true;
                            for (int i = 0; i < oldChar.LinesForeground.Count; i++)
                            {
                                if (oldChar.LinesForeground[i].Start.X != newChar.LinesForeground[i].Start.X || oldChar.LinesForeground[i].Start.Y != newChar.LinesForeground[i].Start.Y || oldChar.LinesForeground[i].End.X != newChar.LinesForeground[i].End.X || oldChar.LinesForeground[i].End.Y != newChar.LinesForeground[i].End.Y)
                                {
                                    found = false;
                                }
                            }

                            for (int i = 0; i < oldChar.LinesBackground.Count; i++)
                            {
                                if (oldChar.LinesBackground[i].Start.X != newChar.LinesBackground[i].Start.X || oldChar.LinesBackground[i].Start.Y != newChar.LinesBackground[i].Start.Y || oldChar.LinesBackground[i].End.X != newChar.LinesBackground[i].End.X || oldChar.LinesBackground[i].End.Y != newChar.LinesBackground[i].End.Y)
                                {
                                    found = false;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        this._nocrChars.Add(newChar);
                        importedCount++;
                    }
                    else
                    {
                        notImportedCount++;
                    }
                }

                MessageBox.Show(string.Format("Number of characters imported: {0}\r\nNumber of characters not imported (already present): {1}", importedCount, notImportedCount));
            }
        }
    }
}