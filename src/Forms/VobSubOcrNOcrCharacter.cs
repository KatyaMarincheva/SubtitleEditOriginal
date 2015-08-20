// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubOcrNOcrCharacter.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub ocr n ocr character.
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
    /// The vob sub ocr n ocr character.
    /// </summary>
    public partial class VobSubOcrNOcrCharacter : Form
    {
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
        /// The _image height.
        /// </summary>
        private int _imageHeight;

        /// <summary>
        /// The _image width.
        /// </summary>
        private int _imageWidth;

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
        private NOcrChar _nocrChar = null;

        /// <summary>
        /// The _start.
        /// </summary>
        private Point _start;

        /// <summary>
        /// The _start done.
        /// </summary>
        private bool _startDone;

        /// <summary>
        /// The _warning no not foreground lines shown.
        /// </summary>
        private bool _warningNoNotForegroundLinesShown = false;

        /// <summary>
        /// The _zoom factor.
        /// </summary>
        private double _zoomFactor = 3.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcrNOcrCharacter"/> class.
        /// </summary>
        public VobSubOcrNOcrCharacter()
        {
            this.InitializeComponent();
            Utilities.FixLargeFonts(this, this.buttonCancel);
            this.labelItalicOn.Visible = false;
        }

        /// <summary>
        /// Gets the n ocr char.
        /// </summary>
        public NOcrChar NOcrChar
        {
            get
            {
                return this._nocrChar;
            }
        }

        /// <summary>
        /// Gets the form position.
        /// </summary>
        public Point FormPosition
        {
            get
            {
                return new Point(this.Left, this.Top);
            }
        }

        /// <summary>
        /// Gets a value indicating whether expand selection.
        /// </summary>
        public bool ExpandSelection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether shrink selection.
        /// </summary>
        public bool ShrinkSelection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is italic.
        /// </summary>
        public bool IsItalic
        {
            get
            {
                return this.checkBoxItalic.Checked;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="vobSubImage">
        /// The vob sub image.
        /// </param>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="italicChecked">
        /// The italic checked.
        /// </param>
        /// <param name="showShrink">
        /// The show shrink.
        /// </param>
        internal void Initialize(Bitmap vobSubImage, ImageSplitterItem character, Point position, bool italicChecked, bool showShrink)
        {
            this.listBoxLinesForeground.Items.Clear();
            this.listBoxlinesBackground.Items.Clear();
            NikseBitmap nbmp = new NikseBitmap(vobSubImage);
            nbmp.ReplaceTransparentWith(Color.Black);
            vobSubImage = nbmp.GetBitmap();

            this.radioButtonHot.Checked = true;
            this.ShrinkSelection = false;
            this.ExpandSelection = false;

            this.textBoxCharacters.Text = string.Empty;
            this._nocrChar = new NOcrChar();
            this._nocrChar.MarginTop = character.Y - character.ParentY;
            this._imageWidth = character.NikseBitmap.Width;
            this._imageHeight = character.NikseBitmap.Height;
            this._drawLineOn = false;
            this._warningNoNotForegroundLinesShown = false;

            this.buttonShrinkSelection.Visible = showShrink;

            if (position.X != -1 && position.Y != -1)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Left = position.X;
                this.Top = position.Y;
            }

            this.pictureBoxSubtitleImage.Image = vobSubImage;
            this.pictureBoxCharacter.Image = character.NikseBitmap.GetBitmap();

            Bitmap org = (Bitmap)vobSubImage.Clone();
            Bitmap bm = new Bitmap(org.Width, org.Height);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(org, 0, 0, org.Width, org.Height);
            g.DrawRectangle(Pens.Red, character.X, character.Y, character.NikseBitmap.Width, character.NikseBitmap.Height - 1);
            g.Dispose();
            this.pictureBoxSubtitleImage.Image = bm;

            this.pictureBoxCharacter.Top = this.labelCharacters.Top + 16;
            this.SizePictureBox();
            this.checkBoxItalic.Checked = italicChecked;

            this._history = new List<NOcrChar>();
            this._historyIndex = -1;

            this._nocrChar.Width = this._imageWidth;
            this._nocrChar.Height = this._imageHeight;
            GenerateLineSegments(150, false, this._nocrChar, new NikseBitmap(this.pictureBoxCharacter.Image as Bitmap));
            this.ShowOcrPoints();
            this.pictureBoxCharacter.Invalidate();
        }

        /// <summary>
        /// The button expand selection_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonExpandSelection_Click(object sender, EventArgs e)
        {
            this.ExpandSelection = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button shrink selection_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonShrinkSelection_Click(object sender, EventArgs e)
        {
            this.ShrinkSelection = true;
            this.DialogResult = DialogResult.OK;
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
                        if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
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
                foreach (Point point in op.GetPoints())
                {
                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                    {
                        Color c = nbmp.GetPixel(point.X, point.Y);
                        if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
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
            if (this.listBoxLinesForeground.Items.Count == 0)
            {
                MessageBox.Show("No foreground lines!");
                return;
            }

            if (this.listBoxlinesBackground.Items.Count == 0 && !this._warningNoNotForegroundLinesShown)
            {
                MessageBox.Show("No not-foreground lines!");
                this._warningNoNotForegroundLinesShown = true;
                return;
            }

            if (this.textBoxCharacters.Text.Length == 0)
            {
                MessageBox.Show("Text is empty!");
                return;
            }

            if (!this.IsMatch())
            {
                MessageBox.Show("Lines does not match image!");
                return;
            }

            this._nocrChar.Text = this.textBoxCharacters.Text;
            this._nocrChar.Italic = this.checkBoxItalic.Checked;
            this.DialogResult = DialogResult.OK;
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
        /// The size picture box.
        /// </summary>
        private void SizePictureBox()
        {
            this.pictureBoxCharacter.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxCharacter.Width = (int)Math.Round(this._imageWidth * this._zoomFactor);
            this.pictureBoxCharacter.Height = (int)Math.Round(this._imageHeight * this._zoomFactor);
            this.pictureBoxCharacter.Invalidate();
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
        /// The vob sub ocr n ocr character_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcrNOcrCharacter_KeyDown(object sender, KeyEventArgs e)
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
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.A)
            {
                this._nocrChar.Width = this._imageWidth;
                this._nocrChar.Height = this._imageHeight;
                GenerateLineSegments(150, false, this._nocrChar, new NikseBitmap(this.pictureBoxCharacter.Image as Bitmap));
                this.ShowOcrPoints();
                this.pictureBoxCharacter.Invalidate();
                this.Redo();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y)
            {
                e.SuppressKeyPress = true;
                this.Redo();
            }

            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Left && this.buttonShrinkSelection.Visible)
            {
                this.buttonShrinkSelection_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Right && this.buttonExpandSelection.Visible)
            {
                this.buttonExpandSelection_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Subtract && this.buttonShrinkSelection.Visible)
            {
                this.buttonShrinkSelection_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Add && this.buttonExpandSelection.Visible)
            {
                this.buttonExpandSelection_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The is match point fore ground.
        /// </summary>
        /// <param name="op">
        /// The op.
        /// </param>
        /// <param name="loose">
        /// The loose.
        /// </param>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <param name="nOcrChar">
        /// The n ocr char.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsMatchPointForeGround(NOcrPoint op, bool loose, NikseBitmap nbmp, NOcrChar nOcrChar)
        {
            if (Math.Abs(op.Start.X - op.End.X) < 2 && Math.Abs(op.End.Y - op.Start.Y) < 2)
            {
                return false;
            }

            foreach (Point point in op.ScaledGetPoints(nOcrChar, nbmp.Width, nbmp.Height))
            {
                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                {
                    Color c = nbmp.GetPixel(point.X, point.Y);
                    if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                    {
                    }
                    else
                    {
                        return false;
                    }

                    if (loose)
                    {
                        if (nbmp.Width > 10 && point.X + 1 < nbmp.Width)
                        {
                            c = nbmp.GetPixel(point.X + 1, point.Y);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (nbmp.Width > 10 && point.X >= 1)
                        {
                            c = nbmp.GetPixel(point.X - 1, point.Y);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (nbmp.Height > 10 && point.Y + 1 < nbmp.Height)
                        {
                            c = nbmp.GetPixel(point.X, point.Y + 1);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (nbmp.Height > 10 && point.Y >= 1)
                        {
                            c = nbmp.GetPixel(point.X, point.Y - 1);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The is match point back ground.
        /// </summary>
        /// <param name="op">
        /// The op.
        /// </param>
        /// <param name="loose">
        /// The loose.
        /// </param>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <param name="nOcrChar">
        /// The n ocr char.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsMatchPointBackGround(NOcrPoint op, bool loose, NikseBitmap nbmp, NOcrChar nOcrChar)
        {
            foreach (Point point in op.ScaledGetPoints(nOcrChar, nbmp.Width, nbmp.Height))
            {
                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                {
                    Color c = nbmp.GetPixel(point.X, point.Y);
                    if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                    {
                        return false;
                    }

                    if (nbmp.Width > 10 && point.X + 1 < nbmp.Width)
                    {
                        c = nbmp.GetPixel(point.X + 1, point.Y);
                        if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                        {
                            return false;
                        }
                    }

                    if (loose)
                    {
                        if (nbmp.Width > 10 && point.X >= 1)
                        {
                            c = nbmp.GetPixel(point.X - 1, point.Y);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                                return false;
                            }
                        }

                        if (nbmp.Height > 10 && point.Y + 1 < nbmp.Height)
                        {
                            c = nbmp.GetPixel(point.X, point.Y + 1);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                                return false;
                            }
                        }

                        if (nbmp.Height > 10 && point.Y >= 1)
                        {
                            c = nbmp.GetPixel(point.X, point.Y - 1);
                            if (c.A > 150 && c.R + 100 + c.G + c.B > VobSubOcr.NocrMinColor)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The generate line segments.
        /// </summary>
        /// <param name="numberOfLines">
        /// The number of lines.
        /// </param>
        /// <param name="veryPrecise">
        /// The very precise.
        /// </param>
        /// <param name="nOcrChar">
        /// The n ocr char.
        /// </param>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        public static void GenerateLineSegments(int numberOfLines, bool veryPrecise, NOcrChar nOcrChar, NikseBitmap nbmp)
        {
            const int giveUpCount = 10000;
            var r = new Random();
            int count = 0;
            int hits = 0;
            bool tempVeryPrecise = veryPrecise;
            while (hits < numberOfLines && count < giveUpCount)
            {
                var start = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                var end = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));

                if (hits < 5 && count < 100)
                {
                    // a few large lines
                    for (int k = 0; k < 500; k++)
                    {
                        if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) > nOcrChar.Height / 2)
                        {
                            break;
                        }
                        else
                        {
                            end = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                        }
                    }
                }
                else
                {
                    // and a lot of small lines
                    for (int k = 0; k < 500; k++)
                    {
                        if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) < 5)
                        {
                            break;
                        }
                        else
                        {
                            end = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                        }
                    }
                }

                var op = new NOcrPoint(start, end);
                bool ok = true;
                foreach (NOcrPoint existingOp in nOcrChar.LinesForeground)
                {
                    if (existingOp.Start.X == op.Start.X && existingOp.Start.Y == op.Start.Y && existingOp.End.X == op.End.X && existingOp.End.Y == op.End.Y)
                    {
                        ok = false;
                    }
                }

                if (ok && IsMatchPointForeGround(op, !tempVeryPrecise, nbmp, nOcrChar))
                {
                    nOcrChar.LinesForeground.Add(op);

                    // AddHistoryItem(nOcrChar);
                    hits++;
                }

                count++;
                if (count > giveUpCount - 100 && !tempVeryPrecise)
                {
                    tempVeryPrecise = true;
                }
            }

            count = 0;
            hits = 0;
            tempVeryPrecise = veryPrecise;
            while (hits < numberOfLines && count < giveUpCount)
            {
                var start = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                var end = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));

                if (hits < 5 && count < 100)
                {
                    // a few large lines
                    for (int k = 0; k < 500; k++)
                    {
                        if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) > nOcrChar.Height / 2)
                        {
                            break;
                        }
                        else
                        {
                            end = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                        }
                    }
                }
                else
                {
                    // and a lot of small lines
                    for (int k = 0; k < 500; k++)
                    {
                        if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) < 5)
                        {
                            break;
                        }
                        else
                        {
                            end = new Point(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                        }
                    }
                }

                var op = new NOcrPoint(start, end);
                bool ok = true;
                foreach (NOcrPoint existingOp in nOcrChar.LinesBackground)
                {
                    if (existingOp.Start.X == op.Start.X && existingOp.Start.Y == op.Start.Y && existingOp.End.X == op.End.X && existingOp.End.Y == op.End.Y)
                    {
                        ok = false;
                    }
                }

                if (ok && IsMatchPointBackGround(op, !tempVeryPrecise, nbmp, nOcrChar))
                {
                    nOcrChar.LinesBackground.Add(op);

                    // AddHistoryItem(nOcrChar);
                    hits++;
                }

                count++;

                if (count > giveUpCount - 100 && !tempVeryPrecise)
                {
                    tempVeryPrecise = true;
                }
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
            if (this.listBoxlinesBackground.SelectedItems.Count == 1)
            {
                var op = this.listBoxlinesBackground.Items[this.listBoxlinesBackground.SelectedIndex] as NOcrPoint;
                this._nocrChar.LinesBackground.Remove(op);
            }

            this.ShowOcrPoints();
        }

        /// <summary>
        /// The remove foreground tool strip menu item_ click_1.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeForegroundToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (this.listBoxLinesForeground.SelectedItems.Count == 1)
            {
                var op = this.listBoxLinesForeground.Items[this.listBoxLinesForeground.SelectedIndex] as NOcrPoint;
                this._nocrChar.LinesForeground.Remove(op);
            }

            this.ShowOcrPoints();
        }

        /// <summary>
        /// The text box characters_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxCharacters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.buttonOK_Click(null, null);
            }
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
            this.textBoxCharacters.Focus();

            if (this.checkBoxItalic.Checked)
            {
                this.labelCharactersAsText.Font = new Font(this.labelCharactersAsText.Font.FontFamily, this.labelCharactersAsText.Font.Size, FontStyle.Italic);
                this.textBoxCharacters.Font = new Font(this.textBoxCharacters.Font.FontFamily, this.textBoxCharacters.Font.Size, FontStyle.Italic);
                this.labelItalicOn.Visible = true;
            }
            else
            {
                this.labelCharactersAsText.Font = new Font(this.labelCharactersAsText.Font.FontFamily, this.labelCharactersAsText.Font.Size);
                this.textBoxCharacters.Font = new Font(this.textBoxCharacters.Font.FontFamily, this.textBoxCharacters.Font.Size);
                this.labelItalicOn.Visible = false;
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
        /// The insert letter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void InsertLetter(object sender, EventArgs e)
        {
            this.textBoxCharacters.SelectedText = (sender as ToolStripMenuItem).Text;
        }
    }
}