// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubNOcrTrain.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub n ocr train.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Ocr;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The vob sub n ocr train.
    /// </summary>
    public partial class VobSubNOcrTrain : Form
    {
        /// <summary>
        /// The _border width.
        /// </summary>
        private const float _borderWidth = 2.0f;

        /// <summary>
        /// The _border color.
        /// </summary>
        private Color _borderColor = Color.Black;

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
        private float _subtitleFontSize = 25.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubNOcrTrain"/> class.
        /// </summary>
        public VobSubNOcrTrain()
        {
            this.InitializeComponent();

            this.labelInfo.Text = string.Empty;
            foreach (var x in FontFamily.Families)
            {
                if (x.IsStyleAvailable(FontStyle.Regular) && x.IsStyleAvailable(FontStyle.Bold))
                {
                    ListViewItem item = new ListViewItem(x.Name);
                    item.Font = new Font(x.Name, 14);
                    this.listViewFonts.Items.Add(item);
                }
            }

            this.comboBoxSubtitleFontSize.SelectedIndex = 10;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="_nOcrDb">
        /// The _n ocr db.
        /// </param>
        internal void Initialize(NOcrDb _nOcrDb)
        {
            if (_nOcrDb != null)
            {
            }
        }

        /// <summary>
        /// The button input choose_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonInputChoose_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxInputFile.Text = this.openFileDialog1.FileName;
            }
        }

        /// <summary>
        /// The button n ocr db choose_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonNOcrDbChoose_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxNOcrDb.Text = this.openFileDialog1.FileName;
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
        /// The button train_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonTrain_Click(object sender, EventArgs e)
        {
            if (!File.Exists(this.textBoxInputFile.Text))
            {
                return;
            }

            int numberOfCharactersLeaned = 0;
            int numberOfCharactersSkipped = 0;
            var nOcrD = new NOcrDb(this.textBoxNOcrDb.Text);
            var lines = new List<string>();
            foreach (string line in File.ReadAllLines(this.textBoxInputFile.Text))
            {
                lines.Add(line);
            }

            var format = new SubRip();
            var sub = new Subtitle();
            format.LoadSubtitle(sub, lines, this.textBoxInputFile.Text);

            var charactersLearned = new List<string>();
            foreach (ListViewItem item in this.listViewFonts.Items)
            {
                if (item.Checked)
                {
                    this._subtitleFontName = item.Text;
                    this._subtitleFontSize = Convert.ToInt32(this.comboBoxSubtitleFontSize.Items[this.comboBoxSubtitleFontSize.SelectedIndex].ToString());
                    charactersLearned = new List<string>();

                    foreach (Paragraph p in sub.Paragraphs)
                    {
                        foreach (char ch in p.Text)
                        {
                            if (!char.IsWhiteSpace(ch))
                            {
                                var s = ch.ToString();
                                if (!charactersLearned.Contains(s))
                                {
                                    this.TrainLetter(ref numberOfCharactersLeaned, ref numberOfCharactersSkipped, nOcrD, charactersLearned, s, false);
                                    if (this.checkBoxBold.Checked)
                                    {
                                        this.TrainLetter(ref numberOfCharactersLeaned, ref numberOfCharactersSkipped, nOcrD, charactersLearned, s, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            nOcrD.Save();
        }

        /// <summary>
        /// The train letter.
        /// </summary>
        /// <param name="numberOfCharactersLeaned">
        /// The number of characters leaned.
        /// </param>
        /// <param name="numberOfCharactersSkipped">
        /// The number of characters skipped.
        /// </param>
        /// <param name="nOcrD">
        /// The n ocr d.
        /// </param>
        /// <param name="charactersLearned">
        /// The characters learned.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="bold">
        /// The bold.
        /// </param>
        private void TrainLetter(ref int numberOfCharactersLeaned, ref int numberOfCharactersSkipped, NOcrDb nOcrD, List<string> charactersLearned, string s, bool bold)
        {
            Bitmap bmp = this.GenerateImageFromTextWithStyle(s, bold);
            var nbmp = new NikseBitmap(bmp);
            nbmp.MakeTwoColor(280);
            var list = NikseBitmapImageSplitter.SplitBitmapToLettersNew(nbmp, 10, false, false, 25);
            if (list.Count == 1)
            {
                NOcrChar match = nOcrD.GetMatch(list[0].NikseBitmap);
                if (match == null)
                {
                    this.pictureBox1.Image = list[0].NikseBitmap.GetBitmap();
                    this.Refresh();
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(100);

                    NOcrChar nOcrChar = new NOcrChar(s);
                    nOcrChar.Width = list[0].NikseBitmap.Width;
                    nOcrChar.Height = list[0].NikseBitmap.Height;
                    VobSubOcrNOcrCharacter.GenerateLineSegments((int)this.numericUpDownSegmentsPerCharacter.Value, this.checkBoxVeryAccurate.Checked, nOcrChar, list[0].NikseBitmap);
                    nOcrD.Add(nOcrChar);

                    charactersLearned.Add(s);
                    numberOfCharactersLeaned++;
                    this.labelInfo.Text = string.Format("Now training font '{1}', total characters leaned is {0}, {2} skipped", numberOfCharactersLeaned, this._subtitleFontName, numberOfCharactersSkipped);
                    bmp.Dispose();
                }
                else
                {
                    numberOfCharactersSkipped++;
                }
            }
        }

        /// <summary>
        /// The generate image from text with style.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="bold">
        /// The bold.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private Bitmap GenerateImageFromTextWithStyle(string text, bool bold)
        {
            bool subtitleFontBold = bold;

            text = HtmlUtil.RemoveHtmlTags(text);

            Font font;
            try
            {
                var fontStyle = FontStyle.Regular;
                if (subtitleFontBold)
                {
                    fontStyle = FontStyle.Bold;
                }

                font = new Font(this._subtitleFontName, this._subtitleFontSize, fontStyle);
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
            const float left = 5f;
            float top = 5;
            bool newLine = false;
            const float leftMargin = left;
            int newLinePathPoint = -1;
            Color c = this._subtitleColor;
            while (i < text.Length)
            {
                if (text.Substring(i).StartsWith(Environment.NewLine))
                {
                    TextDraw.DrawText(font, sf, path, sb, false, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);

                    top += lineHeight;
                    newLine = true;
                    i += Environment.NewLine.Length - 1;
                }
                else
                {
                    sb.Append(text[i]);
                }

                i++;
            }

            if (sb.Length > 0)
            {
                TextDraw.DrawText(font, sf, path, sb, false, subtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
            }

            sf.Dispose();

            if (_borderWidth > 0)
            {
                g.DrawPath(new Pen(this._borderColor, _borderWidth), path);
            }

            g.FillPath(new SolidBrush(c), path);
            g.Dispose();
            var nbmp = new NikseBitmap(bmp);
            nbmp.CropTransparentSidesAndBottom(2, true);
            return nbmp.GetBitmap();
        }
    }
}