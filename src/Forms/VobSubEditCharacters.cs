// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubEditCharacters.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub edit characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Ocr.Binary;

    /// <summary>
    /// The vob sub edit characters.
    /// </summary>
    public sealed partial class VobSubEditCharacters : Form
    {
        /// <summary>
        /// The _bin ocr db.
        /// </summary>
        private BinaryOcrDb _binOcrDb = null;

        /// <summary>
        /// The _compare doc.
        /// </summary>
        private XmlDocument _compareDoc = new XmlDocument();

        /// <summary>
        /// The _directory path.
        /// </summary>
        private string _directoryPath;

        /// <summary>
        /// The _italics.
        /// </summary>
        private List<bool> _italics = new List<bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubEditCharacters"/> class.
        /// </summary>
        /// <param name="databaseFolderName">
        /// The database folder name.
        /// </param>
        /// <param name="additions">
        /// The additions.
        /// </param>
        /// <param name="binOcrDb">
        /// The bin ocr db.
        /// </param>
        internal VobSubEditCharacters(string databaseFolderName, List<VobSubOcr.ImageCompareAddition> additions, BinaryOcrDb binOcrDb)
        {
            this.InitializeComponent();
            this.labelExpandCount.Text = string.Empty;
            this._binOcrDb = binOcrDb;
            this.labelCount.Text = string.Empty;
            if (additions != null)
            {
                this.Additions = new List<VobSubOcr.ImageCompareAddition>();
                foreach (var a in additions)
                {
                    this.Additions.Add(a);
                }

                const int makeHigher = 40;
                this.labelImageCompareFiles.Top = this.labelImageCompareFiles.Top - makeHigher;
                this.listBoxFileNames.Top = this.listBoxFileNames.Top - makeHigher;
                this.listBoxFileNames.Height = this.listBoxFileNames.Height + makeHigher;
                this.groupBoxCurrentCompareImage.Top = this.groupBoxCurrentCompareImage.Top - makeHigher;
                this.groupBoxCurrentCompareImage.Height = this.groupBoxCurrentCompareImage.Height + makeHigher;
            }

            this.labelImageInfo.Text = string.Empty;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            this._directoryPath = Configuration.VobSubCompareFolder + databaseFolderName + Path.DirectorySeparatorChar;
            if (!File.Exists(this._directoryPath + "Images.xml"))
            {
                this._compareDoc.LoadXml("<OcrBitmaps></OcrBitmaps>");
            }
            else
            {
                this._compareDoc.Load(this._directoryPath + "Images.xml");
            }

            this.Refill(this.Additions);

            this.Text = Configuration.Settings.Language.VobSubEditCharacters.Title;
            this.labelChooseCharacters.Text = Configuration.Settings.Language.VobSubEditCharacters.ChooseCharacter;
            this.labelImageCompareFiles.Text = Configuration.Settings.Language.VobSubEditCharacters.ImageCompareFiles;
            this.groupBoxCurrentCompareImage.Text = Configuration.Settings.Language.VobSubEditCharacters.CurrentCompareImage;
            this.labelTextAssociatedWithImage.Text = Configuration.Settings.Language.VobSubEditCharacters.TextAssociatedWithImage;
            this.checkBoxItalic.Text = Configuration.Settings.Language.VobSubEditCharacters.IsItalic;
            this.buttonUpdate.Text = Configuration.Settings.Language.VobSubEditCharacters.Update;
            this.buttonDelete.Text = Configuration.Settings.Language.VobSubEditCharacters.Delete;
            this.labelDoubleSize.Text = Configuration.Settings.Language.VobSubEditCharacters.ImageDoubleSize;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the additions.
        /// </summary>
        internal List<VobSubOcr.ImageCompareAddition> Additions { get; private set; }

        /// <summary>
        /// Gets the image compare document.
        /// </summary>
        public XmlDocument ImageCompareDocument
        {
            get
            {
                return this._compareDoc;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether changes made.
        /// </summary>
        public bool ChangesMade { get; set; }

        /// <summary>
        /// The refill.
        /// </summary>
        /// <param name="additions">
        /// The additions.
        /// </param>
        private void Refill(List<VobSubOcr.ImageCompareAddition> additions)
        {
            if (additions != null && additions.Count > 0)
            {
                this.labelChooseCharacters.Visible = false;
                this.comboBoxTexts.Visible = false;
                this.FillLastAdditions(additions);
            }
            else
            {
                this.FillComboWithUniqueAndSortedTexts();
            }
        }

        /// <summary>
        /// The fill last additions.
        /// </summary>
        /// <param name="additions">
        /// The additions.
        /// </param>
        private void FillLastAdditions(List<VobSubOcr.ImageCompareAddition> additions)
        {
            this._italics = new List<bool>();
            this.listBoxFileNames.Items.Clear();

            if (this._binOcrDb != null)
            {
                foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImages)
                {
                    string name = bob.Key;
                    foreach (VobSubOcr.ImageCompareAddition a in additions)
                    {
                        if (name == a.Name && bob.Text != null)
                        {
                            this.listBoxFileNames.Items.Add(bob);
                            this._italics.Add(bob.Italic);
                        }
                    }
                }

                foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImagesExpanded)
                {
                    string name = bob.Key;
                    foreach (VobSubOcr.ImageCompareAddition a in additions)
                    {
                        if (name == a.Name && bob.Text != null)
                        {
                            this.listBoxFileNames.Items.Add(bob);
                            this._italics.Add(bob.Italic);
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode node in this._compareDoc.DocumentElement.ChildNodes)
                {
                    if (node.Attributes["Text"] != null)
                    {
                        string text = node.Attributes["Text"].InnerText;
                        string name = node.InnerText;
                        foreach (VobSubOcr.ImageCompareAddition a in additions)
                        {
                            if (name == a.Name)
                            {
                                this.listBoxFileNames.Items.Add("[" + text + "] " + node.InnerText);
                                this._italics.Add(node.Attributes["Italic"] != null);
                            }
                        }
                    }
                }
            }

            if (this.listBoxFileNames.Items.Count > 0)
            {
                this.listBoxFileNames.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The fill combo with unique and sorted texts.
        /// </summary>
        private void FillComboWithUniqueAndSortedTexts()
        {
            int count = 0;
            List<string> texts = new List<string>();

            if (this._binOcrDb != null)
            {
                foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImages)
                {
                    string text = bob.Text;
                    if (!texts.Contains(text) && text != null)
                    {
                        texts.Add(text);
                    }

                    count++;
                }

                foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImagesExpanded)
                {
                    string text = bob.Text;
                    if (!texts.Contains(text) && text != null)
                    {
                        texts.Add(text);
                    }

                    count++;
                }
            }
            else
            {
                foreach (XmlNode node in this._compareDoc.DocumentElement.SelectNodes("Item"))
                {
                    if (node.Attributes.Count >= 1)
                    {
                        string text = node.Attributes["Text"].InnerText;
                        if (!texts.Contains(text))
                        {
                            texts.Add(text);
                        }

                        count++;
                    }
                }
            }

            texts.Sort();
            this.labelCount.Text = string.Format("{0:#,##0}", count);

            this.comboBoxTexts.Items.Clear();
            foreach (string text in texts)
            {
                this.comboBoxTexts.Items.Add(text);
            }

            if (this.comboBoxTexts.Items.Count > 0)
            {
                this.comboBoxTexts.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The combo box texts selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxTextsSelectedIndexChanged(object sender, EventArgs e)
        {
            this._italics = new List<bool>();
            string target = this.comboBoxTexts.SelectedItem.ToString();
            this.textBoxText.Text = target;
            this.listBoxFileNames.Items.Clear();

            if (this._binOcrDb != null)
            {
                foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImages)
                {
                    string text = bob.Text;
                    if (text == target)
                    {
                        this.listBoxFileNames.Items.Add(bob);
                        this._italics.Add(bob.Italic);
                    }
                }

                foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImagesExpanded)
                {
                    string text = bob.Text;
                    if (text == target)
                    {
                        this.listBoxFileNames.Items.Add(bob);
                        this._italics.Add(bob.Italic);
                    }
                }
            }
            else
            {
                foreach (XmlNode node in this._compareDoc.DocumentElement.ChildNodes)
                {
                    if (node.Attributes["Text"] != null)
                    {
                        string text = node.Attributes["Text"].InnerText;
                        if (text == target)
                        {
                            this.listBoxFileNames.Items.Add(node.InnerText);
                            this._italics.Add(node.Attributes["Italic"] != null);
                        }
                    }
                }
            }

            if (this.listBoxFileNames.Items.Count > 0)
            {
                this.listBoxFileNames.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The get selected file name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetSelectedFileName()
        {
            string fileName = this.listBoxFileNames.SelectedItem.ToString();
            if (fileName.StartsWith('['))
            {
                fileName = fileName.Substring(fileName.IndexOf(']') + 1);
            }

            return fileName.Trim();
        }

        /// <summary>
        /// The get selected bin ocr bitmap.
        /// </summary>
        /// <returns>
        /// The <see cref="BinaryOcrBitmap"/>.
        /// </returns>
        private BinaryOcrBitmap GetSelectedBinOcrBitmap()
        {
            int idx = this.listBoxFileNames.SelectedIndex;
            if (idx < 0 || this._binOcrDb == null)
            {
                return null;
            }

            return this.listBoxFileNames.Items[idx] as BinaryOcrBitmap;
        }

        /// <summary>
        /// The get file name.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFileName(int index)
        {
            string fileName = this.listBoxFileNames.Items[index].ToString();
            if (fileName.StartsWith('['))
            {
                fileName = fileName.Substring(fileName.IndexOf(']') + 1);
            }

            return fileName.Trim();
        }

        /// <summary>
        /// The list box file names selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxFileNamesSelectedIndexChanged(object sender, EventArgs e)
        {
            this.checkBoxItalic.Checked = this._italics[this.listBoxFileNames.SelectedIndex];
            string name = this.listBoxFileNames.Items[this.listBoxFileNames.SelectedIndex].ToString();
            string databaseName = this._directoryPath + "Images.db";
            string posAsString = this.GetSelectedFileName();
            Bitmap bmp = null;
            this.labelExpandCount.Text = string.Empty;
            if (this._binOcrDb != null)
            {
                var bob = this.GetSelectedBinOcrBitmap();
                if (bob != null)
                {
                    bmp = bob.ToOldBitmap();
                    if (bob.ExpandCount > 0)
                    {
                        this.labelExpandCount.Text = string.Format("Expand count: {0}", bob.ExpandCount);
                    }
                }
            }
            else if (File.Exists(databaseName))
            {
                using (var f = new FileStream(databaseName, FileMode.Open))
                {
                    if (name.Contains(']'))
                    {
                        name = name.Substring(name.IndexOf(']') + 1).Trim();
                    }

                    f.Position = Convert.ToInt64(name);
                    bmp = new ManagedBitmap(f).ToOldBitmap();
                }
            }

            if (bmp == null)
            {
                bmp = new Bitmap(1, 1);
                this.labelImageInfo.Text = Configuration.Settings.Language.VobSubEditCharacters.ImageFileNotFound;
            }

            this.pictureBox1.Image = bmp;
            this.pictureBox2.Width = bmp.Width * 2;
            this.pictureBox2.Height = bmp.Height * 2;
            this.pictureBox2.Image = bmp;

            if (this.Additions != null && this.Additions.Count > 0)
            {
                if (this._binOcrDb != null)
                {
                    var bob = this.GetSelectedBinOcrBitmap();
                    foreach (var a in this.Additions)
                    {
                        if (bob != null && bob.Text != null && bob.Key == a.Name)
                        {
                            this.textBoxText.Text = a.Text;
                            this.checkBoxItalic.Checked = a.Italic;
                            break;
                        }
                    }
                }
                else
                {
                    string target = this.GetSelectedFileName();
                    foreach (var a in this.Additions)
                    {
                        if (target.StartsWith(a.Name))
                        {
                            this.textBoxText.Text = a.Text;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The vob sub edit characters_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubEditCharacters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button update click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonUpdateClick(object sender, EventArgs e)
        {
            if (this.listBoxFileNames.Items.Count == 0)
            {
                return;
            }

            string target = this.GetSelectedFileName();
            string newText = this.textBoxText.Text;
            int oldTextItem = this.comboBoxTexts.SelectedIndex;
            int oldListBoxFileNamesIndex = this.listBoxFileNames.SelectedIndex;

            if (this._binOcrDb != null)
            {
                var bob = this.GetSelectedBinOcrBitmap();
                if (bob == null)
                {
                    return;
                }

                string oldText = bob.Text;
                bob.Text = newText;
                bob.Italic = this.checkBoxItalic.Checked;

                if (this.Additions != null && this.Additions.Count > 0)
                {
                    for (int i = this.Additions.Count - 1; i >= 0; i--)
                    {
                        if (this.Additions[i].Name == bob.Key)
                        {
                            this.Additions[i].Italic = bob.Italic;
                            this.Additions[i].Text = bob.Text;
                            break;
                        }
                    }
                }

                this.Refill(this.Additions);

                if (oldText == newText)
                {
                    if (oldTextItem >= 0 && oldTextItem < this.comboBoxTexts.Items.Count)
                    {
                        this.comboBoxTexts.SelectedIndex = oldTextItem;
                    }

                    if (oldListBoxFileNamesIndex >= 0 && oldListBoxFileNamesIndex < this.listBoxFileNames.Items.Count)
                    {
                        this.listBoxFileNames.SelectedIndex = oldListBoxFileNamesIndex;
                    }
                }
                else
                {
                    int i = 0;
                    foreach (var item in this.comboBoxTexts.Items)
                    {
                        if (item.ToString() == newText)
                        {
                            this.comboBoxTexts.SelectedIndexChanged -= this.ComboBoxTextsSelectedIndexChanged;
                            this.comboBoxTexts.SelectedIndex = i;
                            this.ComboBoxTextsSelectedIndexChanged(sender, e);
                            this.comboBoxTexts.SelectedIndexChanged += this.ComboBoxTextsSelectedIndexChanged;
                            int k = 0;
                            foreach (var inner in this.listBoxFileNames.Items)
                            {
                                if ((inner as BinaryOcrBitmap) == bob)
                                {
                                    this.listBoxFileNames.SelectedIndex = k;
                                    break;
                                }

                                k++;
                            }

                            break;
                        }

                        i++;
                    }
                }

                return;
            }

            XmlNode node = this._compareDoc.DocumentElement.SelectSingleNode("Item[.='" + target + "']");
            if (node != null)
            {
                node.Attributes["Text"].InnerText = newText;

                if (this.Additions != null && this.Additions.Count > 0)
                {
                    foreach (var a in this.Additions)
                    {
                        if (target.StartsWith(a.Name))
                        {
                            a.Text = newText;
                            a.Italic = this.checkBoxItalic.Checked;
                            break;
                        }
                    }
                }

                if (this.checkBoxItalic.Checked)
                {
                    if (node.Attributes["Italic"] == null)
                    {
                        XmlAttribute italic = node.OwnerDocument.CreateAttribute("Italic");
                        italic.InnerText = "true";
                        node.Attributes.Append(italic);
                    }
                }
                else
                {
                    if (node.Attributes["Italic"] != null)
                    {
                        node.Attributes.RemoveNamedItem("Italic");
                    }
                }

                this.Refill(this.Additions);
                if (this.Additions == null || this.Additions.Count == 0)
                {
                    for (int i = 0; i < this.comboBoxTexts.Items.Count; i++)
                    {
                        if (this.comboBoxTexts.Items[i].ToString() == newText)
                        {
                            this.comboBoxTexts.SelectedIndex = i;
                            for (int j = 0; j < this.listBoxFileNames.Items.Count; j++)
                            {
                                if (this.GetFileName(j).StartsWith(target))
                                {
                                    this.listBoxFileNames.SelectedIndex = j;
                                }
                            }

                            return;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.listBoxFileNames.Items.Count; i++)
                    {
                        if (this.listBoxFileNames.Items[i].ToString().Contains(target))
                        {
                            this.listBoxFileNames.SelectedIndex = i;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The button delete click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonDeleteClick(object sender, EventArgs e)
        {
            if (this.listBoxFileNames.Items.Count == 0)
            {
                return;
            }

            int oldComboBoxIndex = this.comboBoxTexts.SelectedIndex;
            string target = this.GetSelectedFileName();

            if (this._binOcrDb != null)
            {
                BinaryOcrBitmap bob = this.GetSelectedBinOcrBitmap();
                if (bob != null)
                {
                    if (bob.ExpandCount > 0)
                    {
                        this._binOcrDb.CompareImagesExpanded.Remove(bob);
                    }
                    else
                    {
                        this._binOcrDb.CompareImages.Remove(bob);
                    }

                    if (this.Additions != null && this.Additions.Count > 0)
                    {
                        for (int i = this.Additions.Count - 1; i >= 0; i--)
                        {
                            if (this.Additions[i].Name == bob.Key)
                            {
                                this.Additions.RemoveAt(i);
                                this.Refill(this.Additions);
                                break;
                            }
                        }
                    }

                    this.Refill(this.Additions);
                }

                if (oldComboBoxIndex >= 0 && oldComboBoxIndex < this.comboBoxTexts.Items.Count)
                {
                    this.comboBoxTexts.SelectedIndex = oldComboBoxIndex;
                }

                return;
            }

            XmlNode node = this._compareDoc.DocumentElement.SelectSingleNode("Item[.='" + target + "']");
            if (node != null)
            {
                this._compareDoc.DocumentElement.RemoveChild(node);
                if (this.Additions != null && this.Additions.Count > 0)
                {
                    for (int i = this.Additions.Count - 1; i >= 0; i--)
                    {
                        if (this.Additions[i].Name == target)
                        {
                            this.Additions.RemoveAt(i);
                            this.Refill(this.Additions);
                            break;
                        }
                    }
                }

                this.Refill(this.Additions);
                if (this.Additions == null || this.Additions.Count == 0)
                {
                    if (oldComboBoxIndex < this.comboBoxTexts.Items.Count)
                    {
                        this.comboBoxTexts.SelectedIndex = oldComboBoxIndex;
                    }
                }
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        internal void Initialize(string name, string text)
        {
            if (name != null && text != null)
            {
                for (int i = 0; i < this.comboBoxTexts.Items.Count; i++)
                {
                    if (this.comboBoxTexts.Items[i].ToString() == text)
                    {
                        this.comboBoxTexts.SelectedIndex = i;
                        if (this._binOcrDb != null)
                        {
                            for (int j = 0; j < this.listBoxFileNames.Items.Count; j++)
                            {
                                if ((this.listBoxFileNames.Items[j] as BinaryOcrBitmap).Key == name)
                                {
                                    this.listBoxFileNames.SelectedIndex = j;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < this.listBoxFileNames.Items.Count; j++)
                            {
                                if (this.GetFileName(j).StartsWith(name))
                                {
                                    this.listBoxFileNames.SelectedIndex = j;
                                }
                            }
                        }

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// The vob sub edit characters_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubEditCharacters_Shown(object sender, EventArgs e)
        {
            this.textBoxText.Focus();
        }

        /// <summary>
        /// The text box text_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonUpdateClick(null, null);
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The save image as tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Title = Configuration.Settings.Language.VobSubOcr.SaveSubtitleImageAs;
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.FileName = "Image";
            this.saveFileDialog1.Filter = "PNG image|*.png|BMP image|*.bmp|GIF image|*.gif|TIFF image|*.tiff";
            this.saveFileDialog1.FilterIndex = 0;

            DialogResult result = this.saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Bitmap bmp = this.pictureBox1.Image as Bitmap;
                if (bmp == null)
                {
                    MessageBox.Show("No image!");
                    return;
                }

                try
                {
                    if (this.saveFileDialog1.FilterIndex == 0)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 1)
                    {
                        bmp.Save(this.saveFileDialog1.FileName);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 2)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
    }
}