// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Beamer.cs" company="">
//   
// </copyright>
// <summary>
//   The beamer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The beamer.
    /// </summary>
    public sealed partial class Beamer : Form
    {
        /// <summary>
        /// The _border color.
        /// </summary>
        private Color _borderColor = Color.Black;

        /// <summary>
        /// The _border width.
        /// </summary>
        private float _borderWidth = 2.0f;

        /// <summary>
        /// The _fullscreen.
        /// </summary>
        private bool _fullscreen;

        /// <summary>
        /// The _index.
        /// </summary>
        private int _index;

        /// <summary>
        /// The _is loading.
        /// </summary>
        private bool _isLoading = true;

        /// <summary>
        /// The _main.
        /// </summary>
        private Main _main;

        // Keys _mainGeneralGoToNextSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle);
        /// <summary>
        /// The _main general go to prev subtitle.
        /// </summary>
        private Keys _mainGeneralGoToPrevSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle);

        /// <summary>
        /// The _margin bottom.
        /// </summary>
        private int _marginBottom = 25;

        /// <summary>
        /// The _margin left.
        /// </summary>
        private int _marginLeft;

        /// <summary>
        /// The _milliseconds factor.
        /// </summary>
        private double _millisecondsFactor = 1.0;

        /// <summary>
        /// The _no timer action.
        /// </summary>
        private bool _noTimerAction;

        /// <summary>
        /// The _show index.
        /// </summary>
        private int _showIndex = -2;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _subtitle color.
        /// </summary>
        private Color _subtitleColor = Color.White;

        /// <summary>
        /// The _subtitle font name.
        /// </summary>
        private string _subtitleFontName = "Verdana";

        /// <summary>
        /// The _subtitle font size.
        /// </summary>
        private float _subtitleFontSize = 75.0f;

        /// <summary>
        /// The _video start tick.
        /// </summary>
        private long _videoStartTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="Beamer"/> class.
        /// </summary>
        /// <param name="main">
        /// The main.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public Beamer(Main main, Subtitle subtitle, int index)
        {
            this.InitializeComponent();
            this._main = main;
            this._subtitle = subtitle;
            this._index = index;

            LanguageStructure.Beamer language = Configuration.Settings.Language.Beamer;
            this.Text = language.Title;
            this.groupBoxImageSettings.Text = Configuration.Settings.Language.ExportPngXml.ImageSettings;
            this.labelSubtitleFont.Text = Configuration.Settings.Language.ExportPngXml.FontFamily;
            this.labelSubtitleFontSize.Text = Configuration.Settings.Language.ExportPngXml.FontSize;
            this.buttonColor.Text = Configuration.Settings.Language.ExportPngXml.FontColor;
            this.buttonBorderColor.Text = Configuration.Settings.Language.ExportPngXml.BorderColor;
            this.labelBorderWidth.Text = Configuration.Settings.Language.ExportPngXml.BorderWidth;

            this._subtitleFontName = Configuration.Settings.SubtitleBeaming.FontName;
            this._subtitleFontSize = Configuration.Settings.SubtitleBeaming.FontSize;
            if (this._subtitleFontSize > 100 || this._subtitleFontSize < 10)
            {
                this._subtitleFontSize = 60;
            }

            this._subtitleColor = Configuration.Settings.SubtitleBeaming.FontColor;
            this._borderColor = Configuration.Settings.SubtitleBeaming.BorderColor;
            this._borderWidth = Configuration.Settings.SubtitleBeaming.BorderWidth;

            this.panelColor.BackColor = this._subtitleColor;
            this.panelBorderColor.BackColor = this._borderColor;

            if (Configuration.Settings.SubtitleBeaming.BorderWidth > 0 && Configuration.Settings.SubtitleBeaming.BorderWidth < 5)
            {
                this.comboBoxBorderWidth.SelectedIndex = (int)this._borderWidth;
            }
            else
            {
                this.comboBoxBorderWidth.SelectedIndex = 2;
            }

            this.comboBoxHAlign.SelectedIndex = 1;

            foreach (var x in FontFamily.Families)
            {
                this.comboBoxSubtitleFont.Items.Add(x.Name);
                if (x.Name.Equals(this._subtitleFontName, StringComparison.OrdinalIgnoreCase))
                {
                    this.comboBoxSubtitleFont.SelectedIndex = this.comboBoxSubtitleFont.Items.Count - 1;
                }
            }

            if (this._subtitleFontSize > 10 && this._subtitleFontSize < 100)
            {
                this.comboBoxSubtitleFontSize.SelectedIndex = (int)(this._subtitleFontSize - 10);
            }
            else
            {
                this.comboBoxSubtitleFontSize.SelectedIndex = 40;
            }

            this._isLoading = false;
            this.ShowCurrent();
        }

        /// <summary>
        /// The button color click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonColorClick(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.panelColor.BackColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.panelColor.BackColor = this.colorDialog1.Color;
                this.ShowCurrent();
            }
        }

        /// <summary>
        /// The button border color click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonBorderColorClick(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.panelBorderColor.BackColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.panelBorderColor.BackColor = this.colorDialog1.Color;
                this.ShowCurrent();
            }
        }

        /// <summary>
        /// The combo box subtitle font selected value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxSubtitleFontSelectedValueChanged(object sender, EventArgs e)
        {
            this.ShowCurrent();
        }

        /// <summary>
        /// The combo box subtitle font size selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxSubtitleFontSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowCurrent();
        }

        /// <summary>
        /// The combo box border width selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxBorderWidthSelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowCurrent();
        }

        /// <summary>
        /// The show current.
        /// </summary>
        private void ShowCurrent()
        {
            this.SetupImageParameters();
            if (this._fullscreen)
            {
                if (this._index > 0 && this._index < this._subtitle.Paragraphs.Count)
                {
                    string text = this._subtitle.Paragraphs[this._index].Text;
                    var bmp = this.GenerateImageFromTextWithStyle(text);
                    this.pictureBox1.Image = bmp;
                    this.pictureBox1.Height = bmp.Height;
                    this.pictureBox1.Width = bmp.Width;
                    this.pictureBox1.Left = (this.Width - bmp.Width) / 2 + this._marginLeft;
                    this.pictureBox1.Top = this.Height - (this.pictureBox1.Height + this._marginBottom);
                    this._showIndex = this._index;
                    this._main.FocusParagraph(this._index);
                }
                else
                {
                    this.pictureBox1.Image = null;
                }
            }
            else
            {
                string text = "Testing 123" + Environment.NewLine + "Subtitle Edit";
                if (this._index >= 0 && this._index < this._subtitle.Paragraphs.Count && this._subtitle.Paragraphs[this._index].Text.Length > 1)
                {
                    text = this._subtitle.Paragraphs[this._index].Text;
                    this._main.FocusParagraph(this._index);
                }

                var bmp = this.GenerateImageFromTextWithStyle(text);
                this.pictureBox1.Top = this.groupBoxImageSettings.Top + this.groupBoxImageSettings.Height + 5;
                this.pictureBox1.Left = 5;
                if (this.comboBoxHAlign.SelectedIndex == 1)
                {
                    // center
                    this.pictureBox1.Left = (this.groupBoxImageSettings.Width - bmp.Width) / 2;
                }

                this.pictureBox1.Image = bmp;
                this.pictureBox1.Height = bmp.Height;
                this.pictureBox1.Width = bmp.Width;
                this._showIndex = -2;
            }
        }

        /// <summary>
        /// The setup image parameters.
        /// </summary>
        private void SetupImageParameters()
        {
            if (this._isLoading)
            {
                return;
            }

            this._subtitleColor = this.panelColor.BackColor;
            this._borderColor = this.panelBorderColor.BackColor;
            this._subtitleFontName = this.comboBoxSubtitleFont.SelectedItem.ToString();
            this._subtitleFontSize = float.Parse(this.comboBoxSubtitleFontSize.SelectedItem.ToString());
            this._borderWidth = float.Parse(this.comboBoxBorderWidth.SelectedItem.ToString());

            Configuration.Settings.SubtitleBeaming.FontName = this._subtitleFontName;
            Configuration.Settings.SubtitleBeaming.FontSize = (int)this._subtitleFontSize;
            Configuration.Settings.SubtitleBeaming.FontColor = this._subtitleColor;
            Configuration.Settings.SubtitleBeaming.BorderColor = this._borderColor;
            Configuration.Settings.SubtitleBeaming.BorderWidth = (int)this._borderWidth;
        }

        /// <summary>
        /// The generate image from text with style.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private Bitmap GenerateImageFromTextWithStyle(string text)
        {
            const bool subtitleFontBold = false;
            bool subtitleAlignLeft = this.comboBoxHAlign.SelectedIndex == 0;

            // remove styles for display text (except italic)
            text = RemoveSubStationAlphaFormatting(text);
            text = text.Replace("<b>", string.Empty);
            text = text.Replace("</b>", string.Empty);
            text = text.Replace("<B>", string.Empty);
            text = text.Replace("</B>", string.Empty);
            text = text.Replace("<u>", string.Empty);
            text = text.Replace("</u>", string.Empty);
            text = text.Replace("<U>", string.Empty);
            text = text.Replace("</U>", string.Empty);

            Font font;
            try
            {
                font = new Font(this._subtitleFontName, this._subtitleFontSize, FontStyle.Regular);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                font = new Font(FontFamily.Families[0].Name, this._subtitleFontSize);
            }

            var bmp = new Bitmap(400, 200);
            var g = Graphics.FromImage(bmp);

            SizeF textSize = g.MeasureString("Hj!", font);
            var lineHeight = textSize.Height * 0.64f;

            textSize = g.MeasureString(HtmlUtil.RemoveHtmlTags(text), font);
            g.Dispose();
            bmp.Dispose();
            int sizeX = (int)(textSize.Width * 0.8) + 40;
            int sizeY = (int)(textSize.Height * 0.8) + 30;
            if (sizeX < 1)
            {
                sizeX = 1;
            }

            if (sizeY < 1)
            {
                sizeY = 1;
            }

            bmp = new Bitmap(sizeX, sizeY);
            g = Graphics.FromImage(bmp);

            var lefts = new List<float>();
            foreach (var line in HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagItalic, HtmlUtil.TagFont).SplitToLines())
            {
                if (subtitleAlignLeft)
                {
                    lefts.Add(5);
                }
                else
                {
                    lefts.Add((float)(bmp.Width - g.MeasureString(line, font).Width * 0.8 + 15) / 2);
                }
            }

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near; // draw the text to a path
            var path = new GraphicsPath();

            // display italic
            var sb = new StringBuilder();
            int i = 0;
            bool isItalic = false;
            float left = 5;
            if (lefts.Count > 0)
            {
                left = lefts[0];
            }

            float top = 5;
            bool newLine = false;
            int lineNumber = 0;
            float leftMargin = left;
            bool italicFromStart = false;
            int newLinePathPoint = -1;
            Color c = this._subtitleColor;
            var colorStack = new Stack<Color>();
            var lastText = new StringBuilder();
            while (i < text.Length)
            {
                if (text.Substring(i).StartsWith("<font ", StringComparison.OrdinalIgnoreCase))
                {
                    float addLeft = 0;
                    int oldPathPointIndex = path.PointCount;
                    if (oldPathPointIndex < 0)
                    {
                        oldPathPointIndex = 0;
                    }

                    if (sb.Length > 0)
                    {
                        TextDraw.DrawText(font, sf, path, sb, isItalic, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    }

                    if (path.PointCount > 0)
                    {
                        PointF[] list = (PointF[])path.PathPoints.Clone(); // avoid using very slow path.PathPoints indexer!!!
                        for (int k = oldPathPointIndex; k < list.Length; k++)
                        {
                            if (list[k].X > addLeft)
                            {
                                addLeft = list[k].X;
                            }
                        }
                    }

                    if (addLeft == 0)
                    {
                        addLeft = left + 2;
                    }

                    left = addLeft;

                    if (this._borderWidth > 0)
                    {
                        g.DrawPath(new Pen(this._borderColor, this._borderWidth), path);
                    }

                    g.FillPath(new SolidBrush(c), path);
                    path.Reset();
                    path = new GraphicsPath();
                    sb = new StringBuilder();

                    int endIndex = text.Substring(i).IndexOf('>');
                    if (endIndex < 0)
                    {
                        i += 9999;
                    }
                    else
                    {
                        string fontContent = text.Substring(i, endIndex);
                        if (fontContent.Contains(" color="))
                        {
                            var arr = fontContent.Substring(fontContent.IndexOf(" color=", StringComparison.Ordinal) + 7).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length > 0)
                            {
                                string fontColor = arr[0].Trim('\'').Trim('"').Trim('\'');
                                try
                                {
                                    colorStack.Push(c); // save old color
                                    if (fontColor.StartsWith("rgb("))
                                    {
                                        arr = fontColor.Remove(0, 4).TrimEnd(')').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        c = Color.FromArgb(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
                                    }
                                    else
                                    {
                                        c = ColorTranslator.FromHtml(fontColor);
                                    }
                                }
                                catch
                                {
                                    c = this._subtitleColor;
                                }
                            }
                        }

                        i += endIndex;
                    }
                }
                else if (text.Substring(i).StartsWith("</font>", StringComparison.OrdinalIgnoreCase))
                {
                    if (text.Substring(i).ToLower().Replace("</font>", string.Empty).Length > 0)
                    {
                        if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                        {
                            string t = sb.ToString();
                            sb.Clear();
                            sb.Append(' ');
                            sb.Append(t);
                        }

                        float addLeft = 0;
                        int oldPathPointIndex = path.PointCount - 1;
                        if (oldPathPointIndex < 0)
                        {
                            oldPathPointIndex = 0;
                        }

                        if (sb.Length > 0)
                        {
                            TextDraw.DrawText(font, sf, path, sb, isItalic, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                        }

                        if (path.PointCount > 0)
                        {
                            PointF[] list = (PointF[])path.PathPoints.Clone(); // avoid using very slow path.PathPoints indexer!!!
                            for (int k = oldPathPointIndex; k < list.Length; k++)
                            {
                                if (list[k].X > addLeft)
                                {
                                    addLeft = list[k].X;
                                }
                            }
                        }

                        if (addLeft == 0)
                        {
                            addLeft = left + 2;
                        }

                        left = addLeft;

                        if (this._borderWidth > 0)
                        {
                            g.DrawPath(new Pen(this._borderColor, this._borderWidth), path);
                        }

                        g.FillPath(new SolidBrush(c), path);
                        path.Reset();

                        // path = new GraphicsPath();
                        sb = new StringBuilder();
                        if (colorStack.Count > 0)
                        {
                            c = colorStack.Pop();
                        }
                    }

                    i += 6;
                }
                else if (text.Substring(i).StartsWith("<i>", StringComparison.OrdinalIgnoreCase))
                {
                    italicFromStart = i == 0;
                    if (sb.Length > 0)
                    {
                        TextDraw.DrawText(font, sf, path, sb, isItalic, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    }

                    isItalic = true;
                    i += 2;
                }
                else if (text.Substring(i).StartsWith("</i>", StringComparison.OrdinalIgnoreCase) && isItalic)
                {
                    if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                    {
                        string t = sb.ToString();
                        sb.Clear();
                        sb.Append(' ');
                        sb.Append(t);
                    }

                    TextDraw.DrawText(font, sf, path, sb, isItalic, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    isItalic = false;
                    i += 3;
                }
                else if (text.Substring(i).StartsWith(Environment.NewLine))
                {
                    TextDraw.DrawText(font, sf, path, sb, isItalic, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);

                    top += lineHeight;
                    newLine = true;
                    i += Environment.NewLine.Length - 1;
                    lineNumber++;
                    if (lineNumber < lefts.Count)
                    {
                        leftMargin = lefts[lineNumber];
                        left = leftMargin;
                    }

                    if (isItalic)
                    {
                        italicFromStart = true;
                    }
                }
                else
                {
                    sb.Append(text[i]);
                }

                i++;
            }

            if (sb.Length > 0)
            {
                TextDraw.DrawText(font, sf, path, sb, isItalic, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
            }

            sf.Dispose();

            if (this._borderWidth > 0)
            {
                g.DrawPath(new Pen(this._borderColor, this._borderWidth), path);
            }

            g.FillPath(new SolidBrush(c), path);
            g.Dispose();
            var nbmp = new NikseBitmap(bmp);
            nbmp.CropTransparentSidesAndBottom(2, true);
            return nbmp.GetBitmap();
        }

        /// <summary>
        /// The remove sub station alpha formatting.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveSubStationAlphaFormatting(string s)
        {
            int indexOfBegin = s.IndexOf('{');
            while (indexOfBegin >= 0)
            {
                int indexOfEnd = s.IndexOf('}', indexOfBegin + 1);
                if (indexOfEnd < indexOfBegin)
                {
                    break;
                }

                s = s.Remove(indexOfBegin, indexOfEnd - indexOfBegin + 1);
                indexOfBegin = s.IndexOf('{', indexOfBegin);
            }

            return s;
        }

        /// <summary>
        /// The timer 1 tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            if (this._noTimerAction)
            {
                return;
            }

            double positionInMilliseconds = (DateTime.Now.Ticks - this._videoStartTick) / 10000.0D; // 10,000 ticks = 1 millisecond
            positionInMilliseconds *= this._millisecondsFactor;
            int index = 0;
            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                if (p.StartTime.TotalMilliseconds <= positionInMilliseconds && p.EndTime.TotalMilliseconds > positionInMilliseconds)
                {
                    break;
                }

                index++;
            }

            if (index == this._subtitle.Paragraphs.Count)
            {
                index = -1;
            }

            if (index == -1)
            {
                this.pictureBox1.Image = null;
            }
            else if (index != this._showIndex)
            {
                this._index = index;
                this.ShowCurrent();
            }
        }

        /// <summary>
        /// The button start click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (this._index >= this._subtitle.Paragraphs.Count - 1)
            {
                this._index = -1;
                this._videoStartTick = DateTime.Now.Ticks;
            }
            else if (this._index >= 0)
            {
                this._videoStartTick = DateTime.Now.Ticks - ((long)this._subtitle.Paragraphs[this._index].StartTime.TotalMilliseconds * 10000); // 10,000 ticks = 1 millisecond
            }

            this.groupBoxImageSettings.Hide();
            this.buttonStart.Hide();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            this._fullscreen = true;
            this.pictureBox1.Image = null;
            Cursor.Hide();
            this._marginBottom = this.Height - 200;
            this.timer1.Start();
        }

        /// <summary>
        /// The beamer key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BeamerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Home)
            {
                this._index = 0;
                this.ShowCurrent();
                e.Handled = true;
                return;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.End)
            {
                this._index = this._subtitle.Paragraphs.Count - 1;
                this.ShowCurrent();
                e.Handled = true;
                return;
            }

            if (!this._fullscreen)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                else if (e.KeyCode == Keys.Space || (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt) || this._mainGeneralGoToPrevSubtitle == e.KeyData)
                {
                    if (this._index < this._subtitle.Paragraphs.Count - 1)
                    {
                        this._index++;
                    }

                    this.ShowCurrent();
                    e.Handled = true;
                }
                else if (this._mainGeneralGoToPrevSubtitle == e.KeyData || (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt))
                {
                    if (this._index > 0)
                    {
                        this._index--;
                    }

                    this.ShowCurrent();
                    e.Handled = true;
                }
                else if (e.Modifiers == Keys.None && e.KeyCode == Keys.PageDown)
                {
                    if (this._index < this._subtitle.Paragraphs.Count - 21)
                    {
                        this._index += 20;
                    }
                    else
                    {
                        this._index = this._subtitle.Paragraphs.Count - 1;
                    }

                    this.ShowCurrent();
                    e.Handled = true;
                }
                else if (e.Modifiers == Keys.None && e.KeyCode == Keys.PageUp)
                {
                    if (this._index > 20)
                    {
                        this._index -= 20;
                    }
                    else
                    {
                        this._index = 0;
                    }

                    this.ShowCurrent();
                    e.Handled = true;
                }

                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.groupBoxImageSettings.Show();
                this.buttonStart.Show();
                this.timer1.Stop();
                Cursor.Show();
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.BackColor = DefaultBackColor;
                this.WindowState = FormWindowState.Normal;
                this._showIndex = -2;
                this._fullscreen = false;
                this.ShowCurrent();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Pause)
            {
                this.timer1.Stop();
                this.timer1.Enabled = false;
            }
            else if (e.KeyCode == Keys.Space || (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt))
            {
                bool timer1Enabled = this.timer1.Enabled;
                this.timer1.Enabled = false;
                System.Threading.Thread.Sleep(100);
                if (this._index < this._subtitle.Paragraphs.Count - 1)
                {
                    this._index++;
                }

                this._videoStartTick = DateTime.Now.Ticks - ((long)this._subtitle.Paragraphs[this._index].StartTime.TotalMilliseconds * 10000); // 10,000 ticks = 1 millisecond

                this.ShowCurrent();
                this._noTimerAction = false;
                if (timer1Enabled || this._fullscreen)
                {
                    this.timer1.Start();
                }

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt)
            {
                bool timer1Enabled = this.timer1.Enabled;
                this.timer1.Enabled = false;
                System.Threading.Thread.Sleep(100);
                if (this._index > 0)
                {
                    this._index--;
                }

                this._videoStartTick = DateTime.Now.Ticks - ((long)this._subtitle.Paragraphs[this._index].StartTime.TotalMilliseconds * 10000); // 10,000 ticks = 1 millisecond
                this.ShowCurrent();
                if (timer1Enabled)
                {
                    this.timer1.Start();
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.PageDown)
            {
                if (this._index < this._subtitle.Paragraphs.Count - 21)
                {
                    this._index += 20;
                }
                else
                {
                    this._index = this._subtitle.Paragraphs.Count - 1;
                }

                this.ShowCurrent();
                e.Handled = true;
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.PageUp)
            {
                if (this._index > 20)
                {
                    this._index -= 20;
                }
                else
                {
                    this._index = 0;
                }

                this.ShowCurrent();
                e.Handled = true;
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Add)
            {
                if (this.comboBoxSubtitleFontSize.SelectedIndex < this.comboBoxSubtitleFontSize.Items.Count - 1)
                {
                    this.comboBoxSubtitleFontSize.SelectedIndex = this.comboBoxSubtitleFontSize.SelectedIndex + 1;
                    this.ShowCurrent();
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Subtract)
            {
                if (this.comboBoxSubtitleFontSize.SelectedIndex > 0)
                {
                    this.comboBoxSubtitleFontSize.SelectedIndex = this.comboBoxSubtitleFontSize.SelectedIndex - 1;
                    this.ShowCurrent();
                }
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Add)
            {
                this._millisecondsFactor += 0.001;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Subtract)
            {
                this._millisecondsFactor -= 0.001;
            }
            else if (e.KeyCode == Keys.Up)
            {
                this._marginBottom++;
                this.ShowCurrent();
            }
            else if (e.KeyCode == Keys.Down)
            {
                this._marginBottom--;
                this.ShowCurrent();
            }
            else if (e.KeyCode == Keys.Left)
            {
                this._marginLeft--;
                this.ShowCurrent();
            }
            else if (e.KeyCode == Keys.Right)
            {
                this._marginLeft++;
                this.ShowCurrent();
            }
            else if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                this.timer1.Stop();
                int temp = this._index;
                this._index = -1;
                try
                {
                    this.ShowCurrent();
                }
                finally
                {
                    this._index = temp;
                }
            }
        }

        /// <summary>
        /// The beamer form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BeamerFormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Show();
        }

        /// <summary>
        /// The combo box h align selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxHAlignSelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowCurrent();
        }
    }
}