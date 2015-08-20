// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedTextStyles.cs" company="">
//   
// </copyright>
// <summary>
//   The timed text styles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms.Styles
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The timed text styles.
    /// </summary>
    public sealed partial class TimedTextStyles : StylesForm
    {
        /// <summary>
        /// The _nsmgr.
        /// </summary>
        private readonly XmlNamespaceManager _nsmgr;

        /// <summary>
        /// The _xml.
        /// </summary>
        private readonly XmlDocument _xml;

        /// <summary>
        /// The _xml head.
        /// </summary>
        private readonly XmlNode _xmlHead;

        /// <summary>
        /// The _do update.
        /// </summary>
        private bool _doUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTextStyles"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public TimedTextStyles(Subtitle subtitle)
            : base(subtitle)
        {
            this.InitializeComponent();

            this._xml = new XmlDocument();
            try
            {
                this._xml.LoadXml(subtitle.Header);
                var xnsmgr = new XmlNamespaceManager(this._xml.NameTable);
                xnsmgr.AddNamespace("ttml", "http://www.w3.org/ns/ttml");
                if (this._xml.DocumentElement.SelectSingleNode("ttml:head", xnsmgr) == null)
                {
                    this._xml.LoadXml(new TimedText10().ToText(new Subtitle(), "tt")); // load default xml
                }
            }
            catch
            {
                this._xml.LoadXml(new TimedText10().ToText(new Subtitle(), "tt")); // load default xml
            }

            this._nsmgr = new XmlNamespaceManager(this._xml.NameTable);
            this._nsmgr.AddNamespace("ttml", "http://www.w3.org/ns/ttml");
            this._xmlHead = this._xml.DocumentElement.SelectSingleNode("ttml:head", this._nsmgr);

            foreach (FontFamily ff in FontFamily.Families)
            {
                this.comboBoxFontName.Items.Add(char.ToLower(ff.Name[0]) + ff.Name.Substring(1));
            }

            this.InitializeListView();
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public override string Header
        {
            get
            {
                return this._xml.OuterXml;
            }
        }

        /// <summary>
        /// The generate preview real.
        /// </summary>
        protected override void GeneratePreviewReal()
        {
            if (this.listViewStyles.SelectedItems.Count != 1)
            {
                return;
            }

            if (this.pictureBoxPreview.Image != null)
            {
                this.pictureBoxPreview.Image.Dispose();
            }

            var bmp = new Bitmap(this.pictureBoxPreview.Width, this.pictureBoxPreview.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Draw background
                const int rectangleSize = 9;
                for (int y = 0; y < bmp.Height; y += rectangleSize)
                {
                    for (int x = 0; x < bmp.Width; x += rectangleSize)
                    {
                        Color c = Color.DarkGray;
                        if (y % (rectangleSize * 2) == 0)
                        {
                            if (x % (rectangleSize * 2) == 0)
                            {
                                c = Color.LightGray;
                            }
                        }
                        else
                        {
                            if (x % (rectangleSize * 2) != 0)
                            {
                                c = Color.LightGray;
                            }
                        }

                        g.FillRectangle(new SolidBrush(c), x, y, rectangleSize, rectangleSize);
                    }
                }

                // Draw text
                Font font;
                try
                {
                    var fontSize = 20.0f;
                    int fontSizeInt;
                    if (int.TryParse(this.textBoxFontSize.Text.Replace("px", string.Empty), out fontSizeInt))
                    {
                        fontSize = fontSizeInt;
                    }
                    else if (this.textBoxFontSize.Text.EndsWith('%'))
                    {
                        if (int.TryParse(this.textBoxFontSize.Text.TrimEnd('%'), out fontSizeInt))
                        {
                            fontSize *= fontSizeInt / 100.0f;
                        }
                    }

                    font = new Font(this.comboBoxFontName.Text, fontSize);
                }
                catch
                {
                    font = new Font(this.Font, FontStyle.Regular);
                }

                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
                var path = new GraphicsPath();

                bool newLine = false;
                var sb = new StringBuilder();
                sb.Append("This is a test!");

                var measuredWidth = TextDraw.MeasureTextWidth(font, sb.ToString(), this.comboBoxFontWeight.Text == "bold") + 1;
                var measuredHeight = TextDraw.MeasureTextHeight(font, sb.ToString(), this.comboBoxFontWeight.Text == "bold") + 1;

                const float left = 5f;

                // if (radioButtonTopLeft.Checked || radioButtonMiddleLeft.Checked || radioButtonBottomLeft.Checked)
                // left = (float)numericUpDownMarginLeft.Value;
                // else if (radioButtonTopRight.Checked || radioButtonMiddleRight.Checked || radioButtonBottomRight.Checked)
                // left = bmp.Width - (measuredWidth + ((float)numericUpDownMarginRight.Value));
                // else
                // left = ((float)(bmp.Width - measuredWidth * 0.8 + 15) / 2);
                const float top = 2f;

                // if (radioButtonTopLeft.Checked || radioButtonTopCenter.Checked || radioButtonTopRight.Checked)
                // top = (float)numericUpDownMarginVertical.Value;
                // else if (radioButtonMiddleLeft.Checked || radioButtonMiddleCenter.Checked || radioButtonMiddleRight.Checked)
                // top = (bmp.Height - measuredHeight) / 2;
                // else
                // top = bmp.Height - measuredHeight - ((int)numericUpDownMarginVertical.Value);
                const int leftMargin = 0;
                int pathPointsStart = -1;

                // if (radioButtonOpaqueBox.Checked)
                // {
                // if (_isSubStationAlpha)
                // g.FillRectangle(new SolidBrush(panelBackColor.BackColor), left, top, measuredWidth + 3, measuredHeight + 3);
                // else
                // g.FillRectangle(new SolidBrush(panelOutlineColor.BackColor), left, top, measuredWidth + 3, measuredHeight + 3);
                // }
                TextDraw.DrawText(font, sf, path, sb, this.comboBoxFontStyle.Text == "italic", this.comboBoxFontWeight.Text == "bold", false, left, top, ref newLine, leftMargin, ref pathPointsStart);

                // int outline = 0; // (int)numericUpDownOutline.Value;
                // if (outline > 0)
                // {
                // Color outlineColor = Color.White;
                // g.DrawPath(new Pen(outlineColor, outline), path);
                // }
                g.FillPath(new SolidBrush(this.panelFontColor.BackColor), path);
            }

            this.pictureBoxPreview.Image = bmp;
        }

        /// <summary>
        /// The initialize list view.
        /// </summary>
        private void InitializeListView()
        {
            XmlNode head = this._xml.DocumentElement.SelectSingleNode("ttml:head", this._nsmgr);
            foreach (XmlNode node in head.SelectNodes("//ttml:style", this._nsmgr))
            {
                string name = "default";
                if (node.Attributes["xml:id"] != null)
                {
                    name = node.Attributes["xml:id"].Value;
                }
                else if (node.Attributes["id"] != null)
                {
                    name = node.Attributes["id"].Value;
                }

                string fontFamily = "Arial";
                if (node.Attributes["tts:fontFamily"] != null)
                {
                    fontFamily = node.Attributes["tts:fontFamily"].Value;
                }

                string fontWeight = "normal";
                if (node.Attributes["tts:fontWeight"] != null)
                {
                    fontWeight = node.Attributes["tts:fontWeight"].Value;
                }

                string fontStyle = "normal";
                if (node.Attributes["tts:fontStyle"] != null)
                {
                    fontStyle = node.Attributes["tts:fontStyle"].Value;
                }

                string fontColor = "white";
                if (node.Attributes["tts:color"] != null)
                {
                    fontColor = node.Attributes["tts:color"].Value;
                }

                string fontSize = "100%";
                if (node.Attributes["tts:fontSize"] != null)
                {
                    fontSize = node.Attributes["tts:fontSize"].Value;
                }

                this.AddStyle(name, fontFamily, fontColor, fontSize);
            }

            if (this.listViewStyles.Items.Count > 0)
            {
                this.listViewStyles.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// The add style.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="fontSize">
        /// The font size.
        /// </param>
        private void AddStyle(string name, string fontFamily, string color, string fontSize)
        {
            var item = new ListViewItem(name.Trim());
            item.UseItemStyleForSubItems = false;

            var subItem = new ListViewItem.ListViewSubItem(item, fontFamily);
            item.SubItems.Add(subItem);

            subItem = new ListViewItem.ListViewSubItem(item, fontSize);
            item.SubItems.Add(subItem);

            subItem = new ListViewItem.ListViewSubItem(item, string.Empty);
            subItem.Text = color;
            Color c = Color.White;
            try
            {
                if (color.StartsWith("rgb(", StringComparison.Ordinal))
                {
                    string[] arr = color.Remove(0, 4).TrimEnd(')').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    c = Color.FromArgb(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
                }
                else
                {
                    c = ColorTranslator.FromHtml(color);
                }
            }
            catch
            {
            }

            subItem.BackColor = c;
            item.SubItems.Add(subItem);

            int count = 0;
            foreach (var p in this.Subtitle.Paragraphs)
            {
                if (string.IsNullOrEmpty(p.Extra) && name.Trim() == "Default")
                {
                    count++;
                }
                else if (p.Extra != null && name.Trim().Equals(p.Extra.TrimStart(), StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            subItem = new ListViewItem.ListViewSubItem(item, count.ToString());
            item.SubItems.Add(subItem);

            this.listViewStyles.Items.Add(item);
        }

        /// <summary>
        /// The timed text styles_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimedTextStyles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The list view styles_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._doUpdate = false;
            if (this.listViewStyles.SelectedItems.Count == 1)
            {
                string styleName = this.listViewStyles.SelectedItems[0].Text;
                this.LoadStyle(styleName);
                this.GeneratePreview();
            }

            this._doUpdate = true;
        }

        /// <summary>
        /// The load style.
        /// </summary>
        /// <param name="styleName">
        /// The style name.
        /// </param>
        private void LoadStyle(string styleName)
        {
            XmlNode head = this._xml.DocumentElement.SelectSingleNode("ttml:head", this._nsmgr);
            foreach (XmlNode node in head.SelectNodes("//ttml:style", this._nsmgr))
            {
                string name = "default";
                if (node.Attributes["xml:id"] != null)
                {
                    name = node.Attributes["xml:id"].Value;
                }
                else if (node.Attributes["id"] != null)
                {
                    name = node.Attributes["id"].Value;
                }

                if (name == styleName)
                {
                    string fontFamily = "Arial";
                    if (node.Attributes["tts:fontFamily"] != null)
                    {
                        fontFamily = node.Attributes["tts:fontFamily"].Value;
                    }

                    string fontWeight = "normal";
                    if (node.Attributes["tts:fontWeight"] != null)
                    {
                        fontWeight = node.Attributes["tts:fontWeight"].Value;
                    }

                    string fontStyle = "normal";
                    if (node.Attributes["tts:fontStyle"] != null)
                    {
                        fontStyle = node.Attributes["tts:fontStyle"].Value;
                    }

                    string fontColor = "white";
                    if (node.Attributes["tts:color"] != null)
                    {
                        fontColor = node.Attributes["tts:color"].Value;
                    }

                    string fontSize = "100%";
                    if (node.Attributes["tts:fontSize"] != null)
                    {
                        fontSize = node.Attributes["tts:fontSize"].InnerText;
                    }

                    this.textBoxStyleName.Text = name;
                    this.comboBoxFontName.Text = fontFamily;

                    this.textBoxFontSize.Text = fontSize;

                    // normal | italic | oblique
                    this.comboBoxFontStyle.SelectedIndex = 0;
                    if (fontStyle.Equals("italic", StringComparison.OrdinalIgnoreCase))
                    {
                        this.comboBoxFontStyle.SelectedIndex = 1;
                    }

                    if (fontStyle.Equals("oblique", StringComparison.OrdinalIgnoreCase))
                    {
                        this.comboBoxFontStyle.SelectedIndex = 2;
                    }

                    // normal | bold
                    this.comboBoxFontWeight.SelectedIndex = 0;
                    if (fontStyle.Equals("bold", StringComparison.OrdinalIgnoreCase))
                    {
                        this.comboBoxFontStyle.SelectedIndex = 1;
                    }

                    Color color = Color.White;
                    try
                    {
                        if (fontColor.StartsWith("rgb(", StringComparison.Ordinal))
                        {
                            string[] arr = fontColor.Remove(0, 4).TrimEnd(')').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            color = Color.FromArgb(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
                        }
                        else
                        {
                            color = ColorTranslator.FromHtml(fontColor);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to read color: " + fontColor + " - " + exception.Message);
                    }

                    this.panelFontColor.BackColor = color;
                }
            }
        }

        /// <summary>
        /// The button font color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonFontColor_Click(object sender, EventArgs e)
        {
            this.colorDialogStyle.Color = this.panelFontColor.BackColor;
            if (this.colorDialogStyle.ShowDialog() == DialogResult.OK)
            {
                this.listViewStyles.SelectedItems[0].SubItems[3].BackColor = this.colorDialogStyle.Color;
                this.listViewStyles.SelectedItems[0].SubItems[3].Text = Utilities.ColorToHex(this.colorDialogStyle.Color);
                this.panelFontColor.BackColor = this.colorDialogStyle.Color;
                this.UpdateHeaderXml(this.listViewStyles.SelectedItems[0].Text, "tts:color", Utilities.ColorToHex(this.colorDialogStyle.Color));
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The button remove all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewStyles.Items)
            {
                this.UpdateHeaderXmlRemoveStyle(item.Text);
            }

            this.listViewStyles.Items.Clear();
        }

        /// <summary>
        /// The button remove_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1)
            {
                int index = this.listViewStyles.SelectedItems[0].Index;
                string name = this.listViewStyles.SelectedItems[0].Text;

                this.UpdateHeaderXmlRemoveStyle(this.listViewStyles.SelectedItems[0].Text);
                this.listViewStyles.Items.RemoveAt(this.listViewStyles.SelectedItems[0].Index);

                if (index >= this.listViewStyles.Items.Count)
                {
                    index--;
                }

                this.listViewStyles.Items[index].Selected = true;
            }
        }

        /// <summary>
        /// The button add_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string name = "new";
            int count = 2;
            while (this.StyleExists(name))
            {
                name = "new" + count;
                count++;
            }

            this.AddStyle(name, "Arial", "white", "100%");
            this.AddStyleToXml(name, "Arial", "normal", "normal", "white", "100%");
            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Selected = true;
        }

        /// <summary>
        /// The add style to xml.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        /// <param name="fontWeight">
        /// The font weight.
        /// </param>
        /// <param name="fontStyle">
        /// The font style.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="fontSize">
        /// The font size.
        /// </param>
        private void AddStyleToXml(string name, string fontFamily, string fontWeight, string fontStyle, string color, string fontSize)
        {
            TimedText10.AddStyleToXml(this._xml, this._xmlHead, this._nsmgr, name, fontFamily, fontWeight, fontStyle, color, fontSize);
        }

        /// <summary>
        /// The style exists.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool StyleExists(string name)
        {
            foreach (ListViewItem item in this.listViewStyles.Items)
            {
                if (item.Text == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The text box style name_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxStyleName_TextChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1)
            {
                if (this._doUpdate)
                {
                    if (!this.StyleExists(this.textBoxStyleName.Text))
                    {
                        this.UpdateHeaderXml(this.listViewStyles.SelectedItems[0].Text, "xml:id", this.textBoxStyleName.Text);
                        this.textBoxStyleName.BackColor = this.listViewStyles.BackColor;
                        this.listViewStyles.SelectedItems[0].Text = this.textBoxStyleName.Text;
                    }
                    else
                    {
                        this.textBoxStyleName.BackColor = Color.LightPink;
                    }
                }
            }
        }

        /// <summary>
        /// The update header xml.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void UpdateHeaderXml(string id, string tag, string value)
        {
            foreach (XmlNode innerNode in this._xmlHead)
            {
                if (innerNode.Name == "styling")
                {
                    foreach (XmlNode innerInnerNode in innerNode)
                    {
                        if (innerInnerNode.Name == "style")
                        {
                            XmlAttribute idAttr = innerInnerNode.Attributes["xml:id"];
                            if (idAttr != null && idAttr.InnerText == id)
                            {
                                XmlAttribute attr = innerInnerNode.Attributes[tag];
                                if (attr == null)
                                {
                                    attr = this._xml.CreateAttribute("tts:fontSize", "http://www.w3.org/ns/10/ttml#style");
                                    innerInnerNode.Attributes.Append(attr);
                                }

                                attr.InnerText = value;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The update header xml remove style.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        private void UpdateHeaderXmlRemoveStyle(string id)
        {
            foreach (XmlNode innerNode in this._xmlHead)
            {
                if (innerNode.Name == "styling")
                {
                    foreach (XmlNode innerInnerNode in innerNode)
                    {
                        if (innerInnerNode.Name == "style")
                        {
                            XmlAttribute idAttr = innerInnerNode.Attributes["xml:id"];
                            if (idAttr != null && idAttr.InnerText == id)
                            {
                                innerNode.RemoveChild(innerInnerNode);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The combo box font name_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFontName_TextChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                this.listViewStyles.SelectedItems[0].SubItems[1].Text = this.comboBoxFontName.Text;
                this.UpdateHeaderXml(this.listViewStyles.SelectedItems[0].Text, "tts:fontFamily", this.comboBoxFontName.Text);
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The text box font size_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxFontSize_TextChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                this.listViewStyles.SelectedItems[0].SubItems[2].Text = this.textBoxFontSize.Text;
                this.UpdateHeaderXml(this.listViewStyles.SelectedItems[0].Text, "tts:fontSize", this.textBoxFontSize.Text);
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The combo box font style_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFontStyle_TextChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                this.UpdateHeaderXml(this.listViewStyles.SelectedItems[0].Text, "tts:fontStyle", this.comboBoxFontStyle.Text);
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The combo box font weight_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFontWeight_TextChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                this.UpdateHeaderXml(this.listViewStyles.SelectedItems[0].Text, "tts:fontWeight", this.comboBoxFontWeight.Text);
                this.GeneratePreview();
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
        /// The timed text styles_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimedTextStyles_ResizeEnd(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The timed text styles_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimedTextStyles_SizeChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }
    }
}