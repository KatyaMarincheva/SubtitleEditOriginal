// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubNOcrCharacterInspect.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub n ocr character inspect.
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
    /// The vob sub n ocr character inspect.
    /// </summary>
    public partial class VobSubNOcrCharacterInspect : Form
    {
        /// <summary>
        /// The _bitmap.
        /// </summary>
        private Bitmap _bitmap;

        /// <summary>
        /// The _bitmap 2.
        /// </summary>
        private Bitmap _bitmap2;

        /// <summary>
        /// The _image list.
        /// </summary>
        private List<ImageSplitterItem> _imageList;

        /// <summary>
        /// The _match list.
        /// </summary>
        private List<VobSubOcr.CompareMatch> _matchList;

        /// <summary>
        /// The _nocr char.
        /// </summary>
        private NOcrChar _nocrChar;

        /// <summary>
        /// The _nocr chars.
        /// </summary>
        private List<NOcrChar> _nocrChars;

        /// <summary>
        /// The _vob sub ocr.
        /// </summary>
        private VobSubOcr _vobSubOcr;

        /// <summary>
        /// The _zoom factor.
        /// </summary>
        private double _zoomFactor = 3.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubNOcrCharacterInspect"/> class.
        /// </summary>
        public VobSubNOcrCharacterInspect()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The vob sub n ocr character inspect_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubNOcrCharacterInspect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="pixelsIsSpace">
        /// The pixels is space.
        /// </param>
        /// <param name="rightToLeft">
        /// The right to left.
        /// </param>
        /// <param name="nOcrDb">
        /// The n ocr db.
        /// </param>
        /// <param name="vobSubOcr">
        /// The vob sub ocr.
        /// </param>
        internal void Initialize(Bitmap bitmap, int pixelsIsSpace, bool rightToLeft, NOcrDb nOcrDb, VobSubOcr vobSubOcr)
        {
            this._bitmap = bitmap;
            var nbmp = new NikseBitmap(bitmap);
            nbmp.ReplaceNonWhiteWithTransparent();
            bitmap = nbmp.GetBitmap();
            this._bitmap2 = bitmap;
            this._nocrChars = nOcrDb.OcrCharacters;
            this._matchList = new List<VobSubOcr.CompareMatch>();
            this._vobSubOcr = vobSubOcr;

            const int minLineHeight = 6;
            this._imageList = NikseBitmapImageSplitter.SplitBitmapToLettersNew(nbmp, pixelsIsSpace, rightToLeft, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight);

            // _imageList = NikseBitmapImageSplitter.SplitBitmapToLetters(nbmp, pixelsIsSpace, rightToLeft, Configuration.Settings.VobSubOcr.TopToBottom);
            int index = 0;
            while (index < this._imageList.Count)
            {
                ImageSplitterItem item = this._imageList[index];
                if (item.NikseBitmap == null)
                {
                    this.listBoxInspectItems.Items.Add(item.SpecialCharacter);
                    this._matchList.Add(null);
                }
                else
                {
                    nbmp = item.NikseBitmap;
                    nbmp.ReplaceNonWhiteWithTransparent();
                    item.Y += nbmp.CropTopTransparent(0);
                    nbmp.CropTransparentSidesAndBottom(0, true);
                    nbmp.ReplaceTransparentWith(Color.Black);

                    // get nocr matches
                    var match = vobSubOcr.GetNOcrCompareMatchNew(item, nbmp, nOcrDb, false, false);
                    if (match == null)
                    {
                        this.listBoxInspectItems.Items.Add("?");
                        this._matchList.Add(null);
                    }
                    else
                    {
                        this.listBoxInspectItems.Items.Add(match.Text);
                        this._matchList.Add(match);
                    }
                }

                index++;
            }
        }

        /// <summary>
        /// The list box inspect items_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxInspectItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBoxInspectItems.SelectedIndex < 0)
            {
                return;
            }

            var img = this._imageList[this.listBoxInspectItems.SelectedIndex];
            if (img.NikseBitmap != null)
            {
                this.pictureBoxInspectItem.Width = img.NikseBitmap.Width;
                this.pictureBoxInspectItem.Height = img.NikseBitmap.Height;
                var old = img.NikseBitmap.GetBitmap();
                this.pictureBoxInspectItem.Image = old;
                this.pictureBoxCharacter.Image = old;
                this.SizePictureBox();
            }
            else
            {
                this.pictureBoxInspectItem.Image = null;
                this.pictureBoxCharacter.Image = null;
            }

            var match = this._matchList[this.listBoxInspectItems.SelectedIndex];
            if (match == null)
            { // spaces+new lines
                this._nocrChar = null;
                this.pictureBoxCharacter.Invalidate();

                this.buttonUpdate.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.buttonEditDB.Enabled = false;
                this.buttonAddBetterMatch.Enabled = false;
                this.textBoxText.Text = string.Empty;
                this.textBoxText.Enabled = false;
                this.checkBoxItalic.Checked = false;
                this.checkBoxItalic.Enabled = false;
            }
            else if (match.NOcrCharacter == null)
            { // no match found
                this.buttonUpdate.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.textBoxText.Text = string.Empty;
                this.checkBoxItalic.Checked = match.Italic;
                this._nocrChar = null;
                this.pictureBoxCharacter.Invalidate();

                this.buttonEditDB.Enabled = true;
                this.buttonAddBetterMatch.Enabled = true;
                this.textBoxText.Enabled = false;
                this.checkBoxItalic.Enabled = false;
            }
            else
            {
                this.buttonUpdate.Enabled = true;
                this.buttonDelete.Enabled = true;
                this.textBoxText.Text = match.Text;
                this.checkBoxItalic.Checked = match.Italic;
                this._nocrChar = match.NOcrCharacter;
                this.pictureBoxCharacter.Invalidate();

                this.buttonEditDB.Enabled = true;
                this.buttonAddBetterMatch.Enabled = true;
                this.textBoxText.Enabled = true;
                this.checkBoxItalic.Enabled = true;
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

            var foreground = new Pen(new SolidBrush(Color.Green));
            var background = new Pen(new SolidBrush(Color.Red));
            if (this.pictureBoxCharacter.Image != null)
            {
                foreach (NOcrPoint op in this._nocrChar.LinesForeground)
                {
                    e.Graphics.DrawLine(foreground, op.GetScaledStart(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height), op.GetScaledEnd(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height));
                }

                foreach (NOcrPoint op in this._nocrChar.LinesBackground)
                {
                    e.Graphics.DrawLine(background, op.GetScaledStart(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height), op.GetScaledEnd(this._nocrChar, this.pictureBoxCharacter.Width, this.pictureBoxCharacter.Height));
                }
            }
        }

        /// <summary>
        /// The size picture box.
        /// </summary>
        private void SizePictureBox()
        {
            if (this.pictureBoxCharacter.Image != null)
            {
                var bmp = this.pictureBoxCharacter.Image as Bitmap;
                if (bmp != null)
                {
                    this.pictureBoxCharacter.SizeMode = PictureBoxSizeMode.StretchImage;
                    this.pictureBoxCharacter.Width = (int)Math.Round(bmp.Width * this._zoomFactor);
                    this.pictureBoxCharacter.Height = (int)Math.Round(bmp.Height * this._zoomFactor);
                    this.pictureBoxCharacter.Invalidate();
                }
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
        /// The button update_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (this._nocrChar != null)
            {
                this._nocrChar.Text = this.textBoxText.Text;
                this._nocrChar.Italic = this.checkBoxItalic.Checked;
                this._vobSubOcr.SaveNOcrWithCurrentLanguage();
                MessageBox.Show("nOCR saved!");
            }
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
            if (this._nocrChar != null)
            {
                this._nocrChars.Remove(this._nocrChar);
                this._vobSubOcr.SaveNOcrWithCurrentLanguage();
                MessageBox.Show("nOCR saved!");
            }
        }

        /// <summary>
        /// The button add better match_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddBetterMatch_Click(object sender, EventArgs e)
        {
            var expandSelectionList = new List<ImageSplitterItem>();
            if (this.listBoxInspectItems.SelectedIndex < 0)
            {
                return;
            }

            int index = this.listBoxInspectItems.SelectedIndex;
            var img = this._imageList[index];
            if (img.NikseBitmap == null)
            {
                return;
            }

            using (var vobSubOcrNOcrCharacter = new VobSubOcrNOcrCharacter())
            {
                vobSubOcrNOcrCharacter.Initialize(this._bitmap, img, new Point(0, 0), false, expandSelectionList.Count > 1);
                DialogResult result = vobSubOcrNOcrCharacter.ShowDialog(this);
                bool expandSelection = false;
                bool shrinkSelection = false;
                if (result == DialogResult.OK && vobSubOcrNOcrCharacter.ExpandSelection)
                {
                    expandSelection = true;
                    expandSelectionList.Add(img);
                }

                while (result == DialogResult.OK && (vobSubOcrNOcrCharacter.ShrinkSelection || vobSubOcrNOcrCharacter.ExpandSelection))
                {
                    if (expandSelection || shrinkSelection)
                    {
                        expandSelection = false;
                        if (shrinkSelection && index > 0)
                        {
                            shrinkSelection = false;
                        }
                        else if (index + 1 < this._imageList.Count && this._imageList[index + 1].NikseBitmap != null)
                        {
                            // only allow expand to EndOfLine or space
                            index++;
                            expandSelectionList.Add(this._imageList[index]);
                        }

                        img = VobSubOcr.GetExpandedSelection(new NikseBitmap(this._bitmap), expandSelectionList, false); // true
                    }

                    vobSubOcrNOcrCharacter.Initialize(this._bitmap2, img, new Point(0, 0), false, expandSelectionList.Count > 1);
                    result = vobSubOcrNOcrCharacter.ShowDialog(this);

                    if (result == DialogResult.OK && vobSubOcrNOcrCharacter.ShrinkSelection)
                    {
                        shrinkSelection = true;
                        index--;
                        if (expandSelectionList.Count > 0)
                        {
                            expandSelectionList.RemoveAt(expandSelectionList.Count - 1);
                        }
                    }
                    else if (result == DialogResult.OK && vobSubOcrNOcrCharacter.ExpandSelection)
                    {
                        expandSelection = true;
                        index++;
                        expandSelectionList.Add(this._imageList[index]);
                    }
                }

                if (result == DialogResult.OK)
                {
                    if (expandSelectionList.Count > 1)
                    {
                        vobSubOcrNOcrCharacter.NOcrChar.ExpandCount = expandSelectionList.Count;
                    }

                    this._nocrChars.Add(vobSubOcrNOcrCharacter.NOcrChar);
                    this._vobSubOcr.SaveNOcrWithCurrentLanguage();
                    this.DialogResult = DialogResult.OK;
                }
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
        /// The button edit d b_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonEditDB_Click(object sender, EventArgs e)
        {
            var form = new VobSubNOcrEdit(this._nocrChars, this.pictureBoxInspectItem.Image as Bitmap);
            form.ShowDialog(this);
        }
    }
}