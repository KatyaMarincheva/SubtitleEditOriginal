// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubOcrCharacterInspect.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub ocr character inspect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Ocr.Binary;

    /// <summary>
    /// The vob sub ocr character inspect.
    /// </summary>
    public sealed partial class VobSubOcrCharacterInspect : Form
    {
        /// <summary>
        /// The _bin ocr db.
        /// </summary>
        private BinaryOcrDb _binOcrDb = null;

        /// <summary>
        /// The _directory path.
        /// </summary>
        private string _directoryPath;

        /// <summary>
        /// The _image sources.
        /// </summary>
        private List<Bitmap> _imageSources;

        /// <summary>
        /// The _matches.
        /// </summary>
        private List<VobSubOcr.CompareMatch> _matches;

        /// <summary>
        /// The _selected compare binary ocr bitmap.
        /// </summary>
        private BinaryOcrBitmap _selectedCompareBinaryOcrBitmap = null;

        /// <summary>
        /// The _selected compare node.
        /// </summary>
        private XmlNode _selectedCompareNode = null;

        /// <summary>
        /// The _selected match.
        /// </summary>
        private VobSubOcr.CompareMatch _selectedMatch = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcrCharacterInspect"/> class.
        /// </summary>
        public VobSubOcrCharacterInspect()
        {
            this.InitializeComponent();

            this.labelCount.Text = string.Empty;
            this.labelExpandCount.Text = string.Empty;
            this.Text = Configuration.Settings.Language.VobSubOcrCharacterInspect.Title;
            this.groupBoxInspectItems.Text = Configuration.Settings.Language.VobSubOcrCharacterInspect.InspectItems;
            this.labelImageInfo.Text = string.Empty;
            this.groupBoxCurrentCompareImage.Text = Configuration.Settings.Language.VobSubEditCharacters.CurrentCompareImage;
            this.labelTextAssociatedWithImage.Text = Configuration.Settings.Language.VobSubEditCharacters.TextAssociatedWithImage;
            this.checkBoxItalic.Text = Configuration.Settings.Language.VobSubEditCharacters.IsItalic;
            this.buttonUpdate.Text = Configuration.Settings.Language.VobSubEditCharacters.Update;
            this.buttonDelete.Text = Configuration.Settings.Language.VobSubEditCharacters.Delete;
            this.buttonAddBetterMatch.Text = Configuration.Settings.Language.VobSubOcrCharacterInspect.AddBetterMatch;
            this.labelDoubleSize.Text = Configuration.Settings.Language.VobSubEditCharacters.ImageDoubleSize;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the image compare document.
        /// </summary>
        public XmlDocument ImageCompareDocument { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="databaseFolderName">
        /// The database folder name.
        /// </param>
        /// <param name="matches">
        /// The matches.
        /// </param>
        /// <param name="imageSources">
        /// The image sources.
        /// </param>
        /// <param name="binOcrDb">
        /// The bin ocr db.
        /// </param>
        internal void Initialize(string databaseFolderName, List<VobSubOcr.CompareMatch> matches, List<Bitmap> imageSources, BinaryOcrDb binOcrDb)
        {
            this._binOcrDb = binOcrDb;
            this._matches = matches;
            this._imageSources = imageSources;

            if (this._binOcrDb == null)
            {
                this.ImageCompareDocument = new XmlDocument();
                this._directoryPath = Configuration.VobSubCompareFolder + databaseFolderName + Path.DirectorySeparatorChar;
                if (!File.Exists(this._directoryPath + "Images.xml"))
                {
                    this.ImageCompareDocument.LoadXml("<OcrBitmaps></OcrBitmaps>");
                }
                else
                {
                    this.ImageCompareDocument.Load(this._directoryPath + "Images.xml");
                }
            }

            for (int i = 0; i < this._matches.Count; i++)
            {
                this.listBoxInspectItems.Items.Add(this._matches[i]);
            }

            if (this.listBoxInspectItems.Items.Count > 0)
            {
                this.listBoxInspectItems.SelectedIndex = 0;
            }

            this.ShowCount();
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
            this.labelImageInfo.Text = string.Empty;
            this.labelExpandCount.Text = string.Empty;

            if (this.listBoxInspectItems.SelectedIndex < 0)
            {
                return;
            }

            this._selectedCompareNode = null;
            this._selectedCompareBinaryOcrBitmap = null;

            this.pictureBoxInspectItem.Image = this._imageSources[this.listBoxInspectItems.SelectedIndex];
            this.pictureBoxCompareBitmap.Image = null;
            this.pictureBoxCompareBitmapDouble.Image = null;

            int index = this.listBoxInspectItems.SelectedIndex;
            var match = this._matches[index];
            this._selectedMatch = match;
            if (!string.IsNullOrEmpty(match.Name))
            {
                if (this._binOcrDb != null)
                {
                    bool bobFound = false;
                    foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImages)
                    {
                        if (match.Name == bob.Key)
                        {
                            this.textBoxText.Text = bob.Text;
                            this.checkBoxItalic.Checked = bob.Italic;
                            this._selectedCompareBinaryOcrBitmap = bob;

                            var bitmap = bob.ToOldBitmap();
                            this.pictureBoxCompareBitmap.Image = bitmap;
                            this.pictureBoxCompareBitmapDouble.Width = bitmap.Width * 2;
                            this.pictureBoxCompareBitmapDouble.Height = bitmap.Height * 2;
                            this.pictureBoxCompareBitmapDouble.Image = bitmap;

                            var matchBob = new BinaryOcrBitmap(new NikseBitmap(this._imageSources[this.listBoxInspectItems.SelectedIndex]));
                            if (matchBob.Hash == bob.Hash && matchBob.Width == bob.Width && matchBob.Height == bob.Height && matchBob.NumberOfColoredPixels == bob.NumberOfColoredPixels)
                            {
                                this.buttonAddBetterMatch.Enabled = false; // exact match
                            }
                            else
                            {
                                this.buttonAddBetterMatch.Enabled = true;
                            }

                            bobFound = true;
                            break;
                        }
                    }

                    if (!bobFound)
                    {
                        foreach (BinaryOcrBitmap bob in this._binOcrDb.CompareImagesExpanded)
                        {
                            if (match.Name == bob.Key)
                            {
                                this.textBoxText.Text = bob.Text;
                                this.checkBoxItalic.Checked = bob.Italic;
                                this._selectedCompareBinaryOcrBitmap = bob;

                                var bitmap = bob.ToOldBitmap();
                                this.pictureBoxCompareBitmap.Image = bitmap;
                                this.pictureBoxCompareBitmapDouble.Width = bitmap.Width * 2;
                                this.pictureBoxCompareBitmapDouble.Height = bitmap.Height * 2;
                                this.pictureBoxCompareBitmapDouble.Image = bitmap;
                                this.buttonAddBetterMatch.Enabled = false; // exact match
                                this.labelExpandCount.Text = string.Format("Expand count: {0}", bob.ExpandCount);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (XmlNode node in this.ImageCompareDocument.DocumentElement.ChildNodes)
                    {
                        if (node.Attributes["Text"] != null && node.InnerText == match.Name)
                        {
                            string text = node.Attributes["Text"].InnerText;
                            string imageFileName = node.InnerText;
                            imageFileName = Path.Combine(this._directoryPath, imageFileName);
                            this.textBoxText.Text = text;
                            this.checkBoxItalic.Checked = node.Attributes["Italic"] != null;

                            string databaseName = Path.Combine(this._directoryPath, "Images.db");
                            using (var f = new FileStream(databaseName, FileMode.Open))
                            {
                                try
                                {
                                    string name = node.InnerText;
                                    int pos = Convert.ToInt32(name);
                                    f.Position = pos;
                                    ManagedBitmap mbmp = new ManagedBitmap(f);
                                    var bitmap = mbmp.ToOldBitmap();
                                    this.pictureBoxCompareBitmap.Image = bitmap;
                                    this.pictureBoxCompareBitmapDouble.Width = bitmap.Width * 2;
                                    this.pictureBoxCompareBitmapDouble.Height = bitmap.Height * 2;
                                    this.pictureBoxCompareBitmapDouble.Image = bitmap;
                                    this.labelImageInfo.Text = string.Format(Configuration.Settings.Language.VobSubEditCharacters.Image + " - {0}x{1}", bitmap.Width, bitmap.Height);
                                }
                                catch (Exception exception)
                                {
                                    this.labelImageInfo.Text = Configuration.Settings.Language.VobSubEditCharacters.Image;
                                    MessageBox.Show(exception.Message);
                                }
                            }

                            this._selectedCompareNode = node;
                            break;
                        }
                    }
                }
            }

            if (this._selectedCompareNode == null && this._selectedCompareBinaryOcrBitmap == null)
            {
                this.buttonUpdate.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.buttonAddBetterMatch.Enabled = false;
                this.textBoxText.Enabled = false;
                this.textBoxText.Text = string.Empty;
                this.checkBoxItalic.Enabled = false;
            }
            else
            {
                this.buttonUpdate.Enabled = true;
                this.buttonDelete.Enabled = true;
                if (this._selectedCompareNode != null)
                {
                    this.buttonAddBetterMatch.Enabled = true;
                }

                this.textBoxText.Enabled = true;
                this.checkBoxItalic.Enabled = true;
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
            if (this._selectedCompareNode == null && this._selectedCompareBinaryOcrBitmap == null)
            {
                return;
            }

            string newText = this.textBoxText.Text;

            if (this._selectedCompareBinaryOcrBitmap != null)
            {
                foreach (var match in this._matches)
                {
                    if (match.Name == this._selectedCompareBinaryOcrBitmap.Key)
                    {
                        this._selectedCompareBinaryOcrBitmap.Text = newText;
                        this._selectedCompareBinaryOcrBitmap.Italic = this.checkBoxItalic.Checked;
                        match.Text = newText;
                        match.Italic = this.checkBoxItalic.Checked;
                        match.Name = this._selectedCompareBinaryOcrBitmap.Key;
                        break;
                    }
                }

                this._selectedCompareBinaryOcrBitmap.Text = newText;
                this._selectedCompareBinaryOcrBitmap.Italic = this.checkBoxItalic.Checked;
                this.listBoxInspectItems.SelectedIndexChanged -= this.listBoxInspectItems_SelectedIndexChanged;
                this.listBoxInspectItems.Items[this.listBoxInspectItems.SelectedIndex] = newText;
                this.listBoxInspectItems.SelectedIndexChanged += this.listBoxInspectItems_SelectedIndexChanged;
            }
            else
            {
                XmlNode node = this._selectedCompareNode;
                this.listBoxInspectItems.SelectedIndexChanged -= this.listBoxInspectItems_SelectedIndexChanged;
                this.listBoxInspectItems.Items[this.listBoxInspectItems.SelectedIndex] = newText;
                this.listBoxInspectItems.SelectedIndexChanged += this.listBoxInspectItems_SelectedIndexChanged;
                node.Attributes["Text"].InnerText = newText;
                this.SetItalic(node);
            }

            this.listBoxInspectItems_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The set italic.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        private void SetItalic(XmlNode node)
        {
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
            if (this._selectedCompareNode == null && this._selectedCompareBinaryOcrBitmap == null)
            {
                return;
            }

            this.listBoxInspectItems.Items[this.listBoxInspectItems.SelectedIndex] = Configuration.Settings.Language.VobSubOcr.NoMatch;
            if (this._selectedCompareBinaryOcrBitmap != null)
            {
                if (this._selectedCompareBinaryOcrBitmap.ExpandCount > 0)
                {
                    this._binOcrDb.CompareImagesExpanded.Remove(this._selectedCompareBinaryOcrBitmap);
                }
                else
                {
                    this._binOcrDb.CompareImages.Remove(this._selectedCompareBinaryOcrBitmap);
                }

                this._selectedCompareBinaryOcrBitmap = null;
            }
            else
            {
                this.ImageCompareDocument.DocumentElement.RemoveChild(this._selectedCompareNode);
                this._selectedCompareNode = null;
            }

            this.listBoxInspectItems_SelectedIndexChanged(null, null);
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
            if (this.listBoxInspectItems.SelectedIndex < 0)
            {
                return;
            }

            if (this.listBoxInspectItems.Items[this.listBoxInspectItems.SelectedIndex].ToString() == this.textBoxText.Text)
            {
                this.textBoxText.SelectAll();
                this.textBoxText.Focus();
                return;
            }

            if (this._selectedCompareNode != null)
            {
                XmlNode newNode = this.ImageCompareDocument.CreateElement("Item");
                XmlAttribute text = newNode.OwnerDocument.CreateAttribute("Text");
                text.InnerText = this.textBoxText.Text;
                newNode.Attributes.Append(text);

                string databaseName = Path.Combine(this._directoryPath, "Images.db");
                FileStream f;
                long pos = 0;
                if (!File.Exists(databaseName))
                {
                    using (f = new FileStream(databaseName, FileMode.Create))
                    {
                        pos = f.Position;
                        new ManagedBitmap(this.pictureBoxInspectItem.Image as Bitmap).AppendToStream(f);
                    }
                }
                else
                {
                    using (f = new FileStream(databaseName, FileMode.Append))
                    {
                        pos = f.Position;
                        new ManagedBitmap(this.pictureBoxInspectItem.Image as Bitmap).AppendToStream(f);
                    }
                }

                string name = pos.ToString(CultureInfo.InvariantCulture);
                newNode.InnerText = name;

                this.SetItalic(newNode);
                this.ImageCompareDocument.DocumentElement.AppendChild(newNode);

                int index = this.listBoxInspectItems.SelectedIndex;
                this._matches[index].Name = name;
                this._matches[index].ExpandCount = 0;
                this._matches[index].Italic = this.checkBoxItalic.Checked;
                this._matches[index].Text = this.textBoxText.Text;
                this.listBoxInspectItems.Items.Clear();
                for (int i = 0; i < this._matches.Count; i++)
                {
                    this.listBoxInspectItems.Items.Add(this._matches[i].Text);
                }

                this.listBoxInspectItems.SelectedIndex = index;
                this.ShowCount();
                this.listBoxInspectItems_SelectedIndexChanged(null, null);
            }
            else if (this._selectedCompareBinaryOcrBitmap != null)
            {
                var nbmp = new NikseBitmap(this.pictureBoxInspectItem.Image as Bitmap);
                int x = 0;
                int y = 0;
                if (this._selectedMatch != null && this._selectedMatch.ImageSplitterItem != null)
                {
                    x = this._selectedMatch.X;
                    y = this._selectedMatch.Y;
                }

                var bob = new BinaryOcrBitmap(nbmp, this.checkBoxItalic.Checked, 0, this.textBoxText.Text, x, y);
                this._binOcrDb.Add(bob);

                int index = this.listBoxInspectItems.SelectedIndex;
                this._matches[index].Name = bob.Key;
                this._matches[index].ExpandCount = 0;
                this._matches[index].Italic = this.checkBoxItalic.Checked;
                this._matches[index].Text = this.textBoxText.Text;
                this.listBoxInspectItems.Items.Clear();
                for (int i = 0; i < this._matches.Count; i++)
                {
                    this.listBoxInspectItems.Items.Add(this._matches[i].Text);
                }

                this.listBoxInspectItems.SelectedIndex = index;
                this.listBoxInspectItems_SelectedIndexChanged(null, null);
                this.ShowCount();
            }
        }

        /// <summary>
        /// The show count.
        /// </summary>
        private void ShowCount()
        {
            if (this.listBoxInspectItems.Items.Count > 1)
            {
                this.labelCount.Text = this.listBoxInspectItems.Items.Count.ToString();
            }
            else
            {
                this.labelCount.Text = string.Empty;
            }
        }
    }
}