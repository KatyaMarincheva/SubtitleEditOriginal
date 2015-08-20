// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubStationAlphaStyles.cs" company="">
//   
// </copyright>
// <summary>
//   The sub station alpha styles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms.Styles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The sub station alpha styles.
    /// </summary>
    public partial class SubStationAlphaStyles : StylesForm
    {
        /// <summary>
        /// The _format.
        /// </summary>
        private readonly SubtitleFormat _format;

        /// <summary>
        /// The _is sub station alpha.
        /// </summary>
        private readonly bool _isSubStationAlpha;

        /// <summary>
        /// The _do update.
        /// </summary>
        private bool _doUpdate;

        /// <summary>
        /// The _header.
        /// </summary>
        private string _header;

        /// <summary>
        /// The _old ssa name.
        /// </summary>
        private string _oldSsaName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubStationAlphaStyles"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        public SubStationAlphaStyles(Subtitle subtitle, SubtitleFormat format)
            : base(subtitle)
        {
            this.InitializeComponent();

            this.labelStatus.Text = string.Empty;
            this._header = subtitle.Header;
            this._format = format;
            this._isSubStationAlpha = this._format.Name == SubStationAlpha.NameOfFormat;
            if (this._header == null || !this._header.Contains("style:", StringComparison.OrdinalIgnoreCase))
            {
                this.ResetHeader();
            }

            this.comboBoxFontName.Items.Clear();
            foreach (var x in FontFamily.Families)
            {
                this.comboBoxFontName.Items.Add(x.Name);
            }

            var l = Configuration.Settings.Language.SubStationAlphaStyles;
            this.Text = l.Title;
            this.groupBoxStyles.Text = l.Styles;
            this.listViewStyles.Columns[0].Text = l.Name;
            this.listViewStyles.Columns[1].Text = l.FontName;
            this.listViewStyles.Columns[2].Text = l.FontSize;
            this.listViewStyles.Columns[3].Text = l.UseCount;
            this.listViewStyles.Columns[4].Text = l.Primary;
            this.listViewStyles.Columns[5].Text = l.Outline;
            this.groupBoxProperties.Text = l.Properties;
            this.labelStyleName.Text = l.Name;
            this.groupBoxFont.Text = l.Font;
            this.labelFontName.Text = l.FontName;
            this.labelFontSize.Text = l.FontSize;
            this.checkBoxFontItalic.Text = Configuration.Settings.Language.General.Italic;
            this.checkBoxFontBold.Text = Configuration.Settings.Language.General.Bold;
            this.checkBoxFontUnderline.Text = Configuration.Settings.Language.General.Underline;
            this.groupBoxAlignment.Text = l.Alignment;
            this.radioButtonTopLeft.Text = l.TopLeft;
            this.radioButtonTopCenter.Text = l.TopCenter;
            this.radioButtonTopRight.Text = l.TopRight;
            this.radioButtonMiddleLeft.Text = l.MiddleLeft;
            this.radioButtonMiddleCenter.Text = l.MiddleCenter;
            this.radioButtonMiddleRight.Text = l.MiddleRight;
            this.radioButtonBottomLeft.Text = l.BottomLeft;
            this.radioButtonBottomCenter.Text = l.BottomCenter;
            this.radioButtonBottomRight.Text = l.BottomRight;
            this.groupBoxColors.Text = l.Colors;
            this.buttonPrimaryColor.Text = l.Primary;
            this.buttonSecondaryColor.Text = l.Secondary;
            this.buttonOutlineColor.Text = l.Outline;
            this.buttonBackColor.Text = l.Shadow;
            this.groupBoxMargins.Text = l.Margins;
            this.labelMarginLeft.Text = l.MarginLeft;
            this.labelMarginRight.Text = l.MarginRight;
            this.labelMarginRight.Text = l.MarginRight;
            this.labelMarginVertical.Text = l.MarginVertical;
            this.groupBoxBorder.Text = l.Border;
            this.radioButtonOutline.Text = l.Outline;
            this.labelShadow.Text = l.PlusShadow;
            this.radioButtonOpaqueBox.Text = l.OpaqueBox;
            this.buttonImport.Text = l.Import;
            this.buttonExport.Text = l.Export;
            this.buttonCopy.Text = l.Copy;
            this.buttonAdd.Text = l.New;
            this.buttonRemove.Text = l.Remove;
            this.buttonRemoveAll.Text = l.RemoveAll;
            this.groupBoxPreview.Text = Configuration.Settings.Language.General.Preview;

            if (this._isSubStationAlpha)
            {
                this.Text = l.TitleSubstationAlpha;
                this.buttonOutlineColor.Text = l.Tertiary;
                this.buttonBackColor.Text = l.Back;
                this.listViewStyles.Columns[5].Text = l.Back;
                this.checkBoxFontUnderline.Visible = false;
            }

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            this.InitializeListView();
            Utilities.FixLargeFonts(this, this.buttonCancel);

            this.comboBoxFontName.Left = this.labelFontName.Left + this.labelFontName.Width + 10;
            this.numericUpDownFontSize.Left = this.labelFontSize.Left + this.labelFontSize.Width + 10;
            if (this.comboBoxFontName.Left > this.numericUpDownFontSize.Left)
            {
                this.numericUpDownFontSize.Left = this.comboBoxFontName.Left;
            }
            else
            {
                this.comboBoxFontName.Left = this.numericUpDownFontSize.Left;
            }

            this.numericUpDownOutline.Left = this.radioButtonOutline.Left + this.radioButtonOutline.Width + 5;
            this.labelShadow.Left = this.numericUpDownOutline.Left + this.numericUpDownOutline.Width + 5;
            this.numericUpDownShadowWidth.Left = this.labelShadow.Left + this.labelShadow.Width + 5;
            this.listViewStyles.Columns[5].Width = -2;
            this.checkBoxFontItalic.Left = this.checkBoxFontBold.Left + this.checkBoxFontBold.Width + 12;
            this.checkBoxFontUnderline.Left = this.checkBoxFontItalic.Left + this.checkBoxFontItalic.Width + 12;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public override string Header
        {
            get
            {
                return this._header;
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
                        Color c = Color.WhiteSmoke;
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
                    font = new Font(this.comboBoxFontName.Text, (float)this.numericUpDownFontSize.Value);
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

                var measuredWidth = TextDraw.MeasureTextWidth(font, sb.ToString(), this.checkBoxFontBold.Checked) + 1;
                var measuredHeight = TextDraw.MeasureTextHeight(font, sb.ToString(), this.checkBoxFontBold.Checked) + 1;

                float left = 5;
                if (this.radioButtonTopLeft.Checked || this.radioButtonMiddleLeft.Checked || this.radioButtonBottomLeft.Checked)
                {
                    left = (float)this.numericUpDownMarginLeft.Value;
                }
                else if (this.radioButtonTopRight.Checked || this.radioButtonMiddleRight.Checked || this.radioButtonBottomRight.Checked)
                {
                    left = bmp.Width - (measuredWidth + ((float)this.numericUpDownMarginRight.Value));
                }
                else
                {
                    left = (float)(bmp.Width - measuredWidth * 0.8 + 15) / 2;
                }

                float top = 2;
                if (this.radioButtonTopLeft.Checked || this.radioButtonTopCenter.Checked || this.radioButtonTopRight.Checked)
                {
                    top = (float)this.numericUpDownMarginVertical.Value;
                }
                else if (this.radioButtonMiddleLeft.Checked || this.radioButtonMiddleCenter.Checked || this.radioButtonMiddleRight.Checked)
                {
                    top = (bmp.Height - measuredHeight) / 2;
                }
                else
                {
                    top = bmp.Height - measuredHeight - ((int)this.numericUpDownMarginVertical.Value);
                }

                top -= (int)this.numericUpDownShadowWidth.Value;
                if (this.radioButtonTopCenter.Checked || this.radioButtonMiddleCenter.Checked || this.radioButtonBottomCenter.Checked)
                {
                    left -= (int)(this.numericUpDownShadowWidth.Value / 2);
                }

                const int leftMargin = 0;
                int pathPointsStart = -1;

                if (this.radioButtonOpaqueBox.Checked)
                {
                    if (this._isSubStationAlpha)
                    {
                        g.FillRectangle(new SolidBrush(this.panelBackColor.BackColor), left, top, measuredWidth + 3, measuredHeight + 3);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(this.panelOutlineColor.BackColor), left, top, measuredWidth + 3, measuredHeight + 3);
                    }
                }

                TextDraw.DrawText(font, sf, path, sb, this.checkBoxFontItalic.Checked, this.checkBoxFontBold.Checked, this.checkBoxFontUnderline.Checked, left, top, ref newLine, leftMargin, ref pathPointsStart);

                int outline = (int)this.numericUpDownOutline.Value;

                // draw shadow
                if (this.numericUpDownShadowWidth.Value > 0 && this.radioButtonOutline.Checked)
                {
                    var shadowPath = (GraphicsPath)path.Clone();
                    for (int i = 0; i < (int)this.numericUpDownShadowWidth.Value; i++)
                    {
                        var translateMatrix = new Matrix();
                        translateMatrix.Translate(1, 1);
                        shadowPath.Transform(translateMatrix);

                        using (var p1 = new Pen(Color.FromArgb(250, this.panelBackColor.BackColor), outline)) g.DrawPath(p1, shadowPath);
                    }
                }

                if (outline > 0 && this.radioButtonOutline.Checked)
                {
                    if (this._isSubStationAlpha)
                    {
                        g.DrawPath(new Pen(this.panelBackColor.BackColor, outline), path);
                    }
                    else
                    {
                        g.DrawPath(new Pen(this.panelOutlineColor.BackColor, outline), path);
                    }
                }

                g.FillPath(new SolidBrush(this.panelPrimaryColor.BackColor), path);
            }

            this.pictureBoxPreview.Image = bmp;
        }

        /// <summary>
        /// The initialize list view.
        /// </summary>
        private void InitializeListView()
        {
            var styles = AdvancedSubStationAlpha.GetStylesFromHeader(this._header);
            this.listViewStyles.Items.Clear();
            foreach (string style in styles)
            {
                SsaStyle ssaStyle = this.GetSsaStyle(style);
                AddStyle(this.listViewStyles, ssaStyle, this.Subtitle, this._isSubStationAlpha);
            }

            if (this.listViewStyles.Items.Count > 0)
            {
                this.listViewStyles.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// The add style.
        /// </summary>
        /// <param name="lv">
        /// The lv.
        /// </param>
        /// <param name="ssaStyle">
        /// The ssa style.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="isSubstationAlpha">
        /// The is substation alpha.
        /// </param>
        public static void AddStyle(ListView lv, SsaStyle ssaStyle, Subtitle subtitle, bool isSubstationAlpha)
        {
            var item = new ListViewItem(ssaStyle.Name.Trim());
            item.UseItemStyleForSubItems = false;

            var subItem = new ListViewItem.ListViewSubItem(item, ssaStyle.FontName);
            item.SubItems.Add(subItem);

            subItem = new ListViewItem.ListViewSubItem(item, ssaStyle.FontSize.ToString());
            item.SubItems.Add(subItem);

            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (string.IsNullOrEmpty(p.Extra) && ssaStyle.Name.TrimStart('*') == "Default")
                {
                    count++;
                }
                else if (p.Extra != null && ssaStyle.Name.TrimStart('*').Equals(p.Extra.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            subItem = new ListViewItem.ListViewSubItem(item, count.ToString());
            item.SubItems.Add(subItem);

            subItem = new ListViewItem.ListViewSubItem(item, string.Empty);
            subItem.BackColor = ssaStyle.Primary;
            item.SubItems.Add(subItem);

            subItem = new ListViewItem.ListViewSubItem(item, string.Empty);
            if (isSubstationAlpha)
            {
                subItem.BackColor = ssaStyle.Background;
            }
            else
            {
                subItem.BackColor = ssaStyle.Outline;
            }

            subItem.Text = Configuration.Settings.Language.General.Text;
            subItem.ForeColor = ssaStyle.Primary;
            try
            {
                if (ssaStyle.Bold || ssaStyle.Italic)
                {
                    subItem.Font = new Font(ssaStyle.FontName, subItem.Font.Size, FontStyle.Bold | FontStyle.Italic);
                }
                else if (ssaStyle.Bold)
                {
                    subItem.Font = new Font(ssaStyle.FontName, subItem.Font.Size, FontStyle.Bold);
                }
                else if (ssaStyle.Italic)
                {
                    subItem.Font = new Font(ssaStyle.FontName, subItem.Font.Size, FontStyle.Italic);
                }
                else if (ssaStyle.Italic)
                {
                    subItem.Font = new Font(ssaStyle.FontName, subItem.Font.Size, FontStyle.Regular);
                }
            }
            catch
            {
            }

            item.SubItems.Add(subItem);

            lv.Items.Add(item);
        }

        /// <summary>
        /// The set ssa style.
        /// </summary>
        /// <param name="styleName">
        /// The style name.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        private void SetSsaStyle(string styleName, string propertyName, string propertyValue)
        {
            int propertyIndex = -1;
            int nameIndex = -1;
            var sb = new StringBuilder();
            foreach (var line in this._header.Split(Utilities.NewLineChars, StringSplitOptions.None))
            {
                string s = line.Trim().ToLower();
                if (s.StartsWith("format:", StringComparison.Ordinal))
                {
                    if (line.Length > 10)
                    {
                        var format = line.ToLower().Substring(8).Split(',');
                        for (int i = 0; i < format.Length; i++)
                        {
                            string f = format[i].Trim().ToLower();
                            if (f == "name")
                            {
                                nameIndex = i;
                            }

                            if (f == propertyName)
                            {
                                propertyIndex = i;
                            }
                        }
                    }

                    sb.AppendLine(line);
                }
                else if (s.Replace(" ", string.Empty).StartsWith("style:", StringComparison.Ordinal))
                {
                    if (line.Length > 10)
                    {
                        bool correctLine = false;
                        var format = line.Substring(6).Split(',');
                        for (int i = 0; i < format.Length; i++)
                        {
                            string f = format[i].Trim();
                            if (i == nameIndex)
                            {
                                correctLine = f.Equals(styleName, StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        if (correctLine)
                        {
                            sb.Append(line.Substring(0, 6) + " ");
                            format = line.Substring(6).Split(',');
                            for (int i = 0; i < format.Length; i++)
                            {
                                string f = format[i].Trim();
                                if (i == propertyIndex)
                                {
                                    sb.Append(propertyValue);
                                }
                                else
                                {
                                    sb.Append(f);
                                }

                                if (i < format.Length - 1)
                                {
                                    sb.Append(',');
                                }
                            }

                            sb.AppendLine();
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            this._header = sb.ToString().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
        }

        /// <summary>
        /// The get ssa style.
        /// </summary>
        /// <param name="styleName">
        /// The style name.
        /// </param>
        /// <returns>
        /// The <see cref="SsaStyle"/>.
        /// </returns>
        private SsaStyle GetSsaStyle(string styleName)
        {
            return AdvancedSubStationAlpha.GetSsaStyle(styleName, this._header);
        }

        /// <summary>
        /// The reset header.
        /// </summary>
        private void ResetHeader()
        {
            SubtitleFormat format;
            if (this._isSubStationAlpha)
            {
                format = new SubStationAlpha();
            }
            else
            {
                format = new AdvancedSubStationAlpha();
            }

            var sub = new Subtitle();
            string text = format.ToText(sub, string.Empty);
            var lines = new List<string>();
            foreach (string line in text.SplitToLines())
            {
                lines.Add(line);
            }

            format.LoadSubtitle(sub, lines, string.Empty);
            this._header = sub.Header;
        }

        /// <summary>
        /// The sub station alpha styles_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubStationAlphaStyles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
        /// The button next finish_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonNextFinish_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
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
            if (this.listViewStyles.SelectedItems.Count == 1)
            {
                string styleName = this.listViewStyles.SelectedItems[0].Text;
                this._oldSsaName = styleName;
                SsaStyle style = this.GetSsaStyle(styleName);
                this.SetControlsFromStyle(style);
                this._doUpdate = true;
                this.groupBoxProperties.Enabled = true;
                this.GeneratePreview();

                // if (listViewStyles.SelectedItems[0].Index == 0)
                // {
                // textBoxStyleName.Enabled = false;
                // }
                // else
                // {
                // textBoxStyleName.Enabled = true;
                // }
                this.buttonRemove.Enabled = this.listViewStyles.Items.Count > 1;
            }
            else
            {
                this.groupBoxProperties.Enabled = false;
                this._doUpdate = false;
            }
        }

        /// <summary>
        /// The set controls from style.
        /// </summary>
        /// <param name="style">
        /// The style.
        /// </param>
        private void SetControlsFromStyle(SsaStyle style)
        {
            this.textBoxStyleName.Text = style.Name;
            this.textBoxStyleName.BackColor = this.listViewStyles.BackColor;
            this.comboBoxFontName.Text = style.FontName;
            this.checkBoxFontItalic.Checked = style.Italic;
            this.checkBoxFontBold.Checked = style.Bold;
            this.checkBoxFontUnderline.Checked = style.Underline;

            if (style.FontSize > 0 && style.FontSize <= this.numericUpDownFontSize.Maximum)
            {
                this.numericUpDownFontSize.Value = style.FontSize;
            }
            else
            {
                this.numericUpDownFontSize.Value = 20;
            }

            this.panelPrimaryColor.BackColor = style.Primary;
            this.panelSecondaryColor.BackColor = style.Secondary;
            if (this._isSubStationAlpha)
            {
                this.panelOutlineColor.BackColor = style.Tertiary;
            }
            else
            {
                this.panelOutlineColor.BackColor = style.Outline;
            }

            this.panelBackColor.BackColor = style.Background;
            this.numericUpDownOutline.Value = style.OutlineWidth;
            this.numericUpDownShadowWidth.Value = style.ShadowWidth;

            if (this._isSubStationAlpha)
            {
                switch (style.Alignment)
                {
                    case "1":
                        this.radioButtonBottomLeft.Checked = true;
                        break;
                    case "3":
                        this.radioButtonBottomRight.Checked = true;
                        break;
                    case "9":
                        this.radioButtonMiddleLeft.Checked = true;
                        break;
                    case "10":
                        this.radioButtonMiddleCenter.Checked = true;
                        break;
                    case "11":
                        this.radioButtonMiddleRight.Checked = true;
                        break;
                    case "5":
                        this.radioButtonTopLeft.Checked = true;
                        break;
                    case "6":
                        this.radioButtonTopCenter.Checked = true;
                        break;
                    case "7":
                        this.radioButtonTopRight.Checked = true;
                        break;
                    default:
                        this.radioButtonBottomCenter.Checked = true;
                        break;
                }
            }
            else
            {
                switch (style.Alignment)
                {
                    case "1":
                        this.radioButtonBottomLeft.Checked = true;
                        break;
                    case "3":
                        this.radioButtonBottomRight.Checked = true;
                        break;
                    case "4":
                        this.radioButtonMiddleLeft.Checked = true;
                        break;
                    case "5":
                        this.radioButtonMiddleCenter.Checked = true;
                        break;
                    case "6":
                        this.radioButtonMiddleRight.Checked = true;
                        break;
                    case "7":
                        this.radioButtonTopLeft.Checked = true;
                        break;
                    case "8":
                        this.radioButtonTopCenter.Checked = true;
                        break;
                    case "9":
                        this.radioButtonTopRight.Checked = true;
                        break;
                    default:
                        this.radioButtonBottomCenter.Checked = true;
                        break;
                }
            }

            if (style.MarginLeft >= 0 && style.MarginLeft <= this.numericUpDownMarginLeft.Maximum)
            {
                this.numericUpDownMarginLeft.Value = style.MarginLeft;
            }
            else
            {
                this.numericUpDownMarginLeft.Value = 10;
            }

            if (style.MarginRight >= 0 && style.MarginRight <= this.numericUpDownMarginRight.Maximum)
            {
                this.numericUpDownMarginRight.Value = style.MarginRight;
            }
            else
            {
                this.numericUpDownMarginRight.Value = 10;
            }

            if (style.MarginVertical >= 0 && style.MarginVertical <= this.numericUpDownMarginVertical.Maximum)
            {
                this.numericUpDownMarginVertical.Value = style.MarginVertical;
            }
            else
            {
                this.numericUpDownMarginVertical.Value = 10;
            }

            if (style.BorderStyle == "3")
            {
                this.radioButtonOpaqueBox.Checked = true;
            }
            else
            {
                this.radioButtonOutline.Checked = true;
            }
        }

        /// <summary>
        /// The button primary color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonPrimaryColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelPrimaryColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.listViewStyles.SelectedItems[0].SubItems[4].BackColor = this.colorDialogSSAStyle.Color;
                this.listViewStyles.SelectedItems[0].SubItems[5].ForeColor = this.colorDialogSSAStyle.Color;
                this.panelPrimaryColor.BackColor = this.colorDialogSSAStyle.Color;
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "primarycolour", this.GetSsaColorString(this.colorDialogSSAStyle.Color));
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The button secondary color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSecondaryColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelSecondaryColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelSecondaryColor.BackColor = this.colorDialogSSAStyle.Color;
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "secondarycolour", this.GetSsaColorString(this.colorDialogSSAStyle.Color));
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The button outline color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOutlineColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelOutlineColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                if (!this._isSubStationAlpha)
                {
                    this.listViewStyles.SelectedItems[0].SubItems[4].BackColor = this.colorDialogSSAStyle.Color;
                }

                this.panelOutlineColor.BackColor = this.colorDialogSSAStyle.Color;
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "tertiarycolour", this.GetSsaColorString(this.colorDialogSSAStyle.Color));
                }
                else
                {
                    this.SetSsaStyle(name, "outlinecolour", this.GetSsaColorString(this.colorDialogSSAStyle.Color));
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The button shadow color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonShadowColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelBackColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                if (this._isSubStationAlpha)
                {
                    this.listViewStyles.SelectedItems[0].SubItems[4].BackColor = this.colorDialogSSAStyle.Color;
                }

                this.panelBackColor.BackColor = this.colorDialogSSAStyle.Color;
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "backcolour", this.GetSsaColorString(this.colorDialogSSAStyle.Color));
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The get ssa color string.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetSsaColorString(Color c)
        {
            if (this._isSubStationAlpha)
            {
                return Color.FromArgb(0, c.B, c.G, c.R).ToArgb().ToString();
            }

            return string.Format("&H00{0:X2}{1:X2}{2:X2}", c.B, c.G, c.R);
        }

        /// <summary>
        /// The button copy_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1)
            {
                string styleName = this.listViewStyles.SelectedItems[0].Text;
                SsaStyle oldStyle = this.GetSsaStyle(styleName);
                var style = new SsaStyle(oldStyle); // Copy contructor
                style.Name = string.Format(Configuration.Settings.Language.SubStationAlphaStyles.CopyOfY, styleName);

                if (this.GetSsaStyle(style.Name).LoadedFromHeader)
                {
                    int count = 2;
                    bool doRepeat = true;
                    while (doRepeat)
                    {
                        style.Name = string.Format(Configuration.Settings.Language.SubStationAlphaStyles.CopyXOfY, count, styleName);
                        doRepeat = this.GetSsaStyle(style.Name).LoadedFromHeader;
                        count++;
                    }
                }

                this._doUpdate = false;
                AddStyle(this.listViewStyles, style, this.Subtitle, this._isSubStationAlpha);
                this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Selected = true;
                this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].EnsureVisible();
                this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Focused = true;
                this.textBoxStyleName.Text = style.Name;
                this.textBoxStyleName.Focus();
                this.AddStyleToHeader(style, oldStyle);
                this._doUpdate = true;
                this.listViewStyles_SelectedIndexChanged(null, null);
            }
        }

        /// <summary>
        /// The add style to header.
        /// </summary>
        /// <param name="newStyle">
        /// The new style.
        /// </param>
        /// <param name="oldStyle">
        /// The old style.
        /// </param>
        private void AddStyleToHeader(SsaStyle newStyle, SsaStyle oldStyle)
        {
            if (this.listViewStyles.SelectedItems.Count == 1)
            {
                string newLine = oldStyle.RawLine;
                newLine = newLine.Replace(oldStyle.Name + ",", newStyle.Name + ",");

                int indexOfEvents = this._header.IndexOf("[Events]", StringComparison.Ordinal);
                if (indexOfEvents > 0)
                {
                    int i = indexOfEvents - 1;
                    while (i > 0 && Environment.NewLine.Contains(this._header[i]))
                    {
                        i--;
                    }

                    this._header = this._header.Insert(i + 1, Environment.NewLine + newLine);
                }
                else
                {
                    this._header += Environment.NewLine + newLine;
                }
            }
        }

        /// <summary>
        /// The remove style from header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        private void RemoveStyleFromHeader(string name)
        {
            var sb = new StringBuilder();
            foreach (var line in this._header.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (!line.ToLower().Replace(" ", string.Empty).StartsWith("style:" + name.ToLower() + ","))
                {
                    sb.AppendLine(line);
                }
            }

            this._header = sb.ToString();
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
            SsaStyle style = this.GetSsaStyle(Configuration.Settings.Language.SubStationAlphaStyles.New);
            if (this.GetSsaStyle(style.Name).LoadedFromHeader)
            {
                int count = 2;
                bool doRepeat = true;
                while (doRepeat)
                {
                    style = this.GetSsaStyle(Configuration.Settings.Language.SubStationAlphaStyles.New + count);
                    doRepeat = this.GetSsaStyle(style.Name).LoadedFromHeader;
                    count++;
                }
            }

            this._doUpdate = false;
            AddStyle(this.listViewStyles, style, this.Subtitle, this._isSubStationAlpha);
            SsaStyle oldStyle = this.GetSsaStyle(this.listViewStyles.Items[0].Text);
            this.AddStyleToHeader(style, oldStyle);
            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Selected = true;
            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].EnsureVisible();
            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Focused = true;
            this.textBoxStyleName.Text = style.Name;
            this.textBoxStyleName.Focus();
            this._doUpdate = true;
            this.SetControlsFromStyle(style);
            this.listViewStyles_SelectedIndexChanged(null, null);
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
                    if (!this.GetSsaStyle(this.textBoxStyleName.Text).LoadedFromHeader)
                    {
                        this.textBoxStyleName.BackColor = this.listViewStyles.BackColor;
                        this.listViewStyles.SelectedItems[0].Text = this.textBoxStyleName.Text;
                        this.SetSsaStyle(this._oldSsaName, "name", this.textBoxStyleName.Text);
                        this._oldSsaName = this.textBoxStyleName.Text;
                    }
                    else
                    {
                        this.textBoxStyleName.BackColor = Color.LightPink;
                    }
                }
            }
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
                this.listViewStyles.Items.RemoveAt(this.listViewStyles.SelectedItems[0].Index);
                this.RemoveStyleFromHeader(name);

                if (this.listViewStyles.Items.Count == 0)
                {
                    this.buttonRemoveAll_Click(null, null);
                }
                else
                {
                    if (index >= this.listViewStyles.Items.Count)
                    {
                        index--;
                    }

                    this.listViewStyles.Items[index].Selected = true;
                }
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
            this.listViewStyles.Items.Clear();
            var sub = new Subtitle();
            if (this._isSubStationAlpha)
            {
                var ssa = new SubStationAlpha();
                string text = ssa.ToText(sub, string.Empty);
                string[] lineArray = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var lines = new List<string>();
                foreach (string line in lineArray)
                {
                    lines.Add(line);
                }

                ssa.LoadSubtitle(sub, lines, string.Empty);
                this._header = this._header.Remove(this._header.IndexOf("[V4 Styles]", StringComparison.Ordinal)) + sub.Header.Substring(sub.Header.IndexOf("[V4 Styles]", StringComparison.Ordinal));
            }
            else
            {
                var ass = new AdvancedSubStationAlpha();
                string text = ass.ToText(sub, string.Empty);
                string[] lineArray = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var lines = new List<string>();
                foreach (string line in lineArray)
                {
                    lines.Add(line);
                }

                ass.LoadSubtitle(sub, lines, string.Empty);
                this._header = this._header.Remove(this._header.IndexOf("[V4+ Styles]", StringComparison.Ordinal)) + sub.Header.Substring(sub.Header.IndexOf("[V4+ Styles]", StringComparison.Ordinal));
            }

            this.InitializeListView();
        }

        /// <summary>
        /// The combo box font name_ selected value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFontName_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                this.listViewStyles.SelectedItems[0].SubItems[1].Text = this.comboBoxFontName.SelectedItem.ToString();
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "fontname", this.comboBoxFontName.SelectedItem.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The combo box font name_ key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFontName_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                var item = this.comboBoxFontName.SelectedItem;
                if (item != null)
                {
                    this.SetSsaStyle(name, "fontname", item.ToString());
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The numeric up down font size_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownFontSize_ValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                this.listViewStyles.SelectedItems[0].SubItems[2].Text = this.numericUpDownFontSize.Value.ToString();
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "fontsize", this.numericUpDownFontSize.Value.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The check box font bold_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxFontBold_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this.checkBoxFontBold.Checked)
                {
                    this.SetSsaStyle(name, "bold", "1");
                }
                else
                {
                    this.SetSsaStyle(name, "bold", "0");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The check box font italic_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxFontItalic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this.checkBoxFontItalic.Checked)
                {
                    this.SetSsaStyle(name, "italic", "1");
                }
                else
                {
                    this.SetSsaStyle(name, "italic", "0");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The check box underline_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxUnderline_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this.checkBoxFontUnderline.Checked)
                {
                    this.SetSsaStyle(name, "underline", "1");
                }
                else
                {
                    this.SetSsaStyle(name, "underline", "0");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button bottom left_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonBottomLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "alignment", "1");
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button bottom center_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonBottomCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "alignment", "2");
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button bottom right_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonBottomRight_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "alignment", "3");
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button middle left_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonMiddleLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "alignment", "9");
                }
                else
                {
                    this.SetSsaStyle(name, "alignment", "4");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button middle center_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonMiddleCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "alignment", "10");
                }
                else
                {
                    this.SetSsaStyle(name, "alignment", "5");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button middle right_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonMiddleRight_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "alignment", "11");
                }
                else
                {
                    this.SetSsaStyle(name, "alignment", "6");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button top left_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonTopLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "alignment", "5");
                }
                else
                {
                    this.SetSsaStyle(name, "alignment", "7");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button top center_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonTopCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "alignment", "6");
                }
                else
                {
                    this.SetSsaStyle(name, "alignment", "8");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button top right_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonTopRight_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && (sender as RadioButton).Checked)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                if (this._isSubStationAlpha)
                {
                    this.SetSsaStyle(name, "alignment", "7");
                }
                else
                {
                    this.SetSsaStyle(name, "alignment", "9");
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The numeric up down margin left_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMarginLeft_ValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "marginl", this.numericUpDownMarginLeft.Value.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The numeric up down margin right_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMarginRight_ValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "marginr", this.numericUpDownMarginRight.Value.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The numeric up down margin vertical_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMarginVertical_ValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "marginv", this.numericUpDownMarginVertical.Value.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The sub station alpha styles_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubStationAlphaStyles_ResizeEnd(object sender, EventArgs e)
        {
            this.listViewStyles.Columns[5].Width = -2;
            this.GeneratePreview();
        }

        /// <summary>
        /// The numeric up down outline_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownOutline_ValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "outline", this.numericUpDownOutline.Value.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The numeric up down shadow width_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownShadowWidth_ValueChanged(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate)
            {
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "shadow", this.numericUpDownShadowWidth.Value.ToString());
                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The radio button outline_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonOutline_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null && this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && rb.Checked)
            {
                this.numericUpDownShadowWidth.Value = 2;
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "outline", this.numericUpDownOutline.Value.ToString());
                this.SetSsaStyle(name, "borderstyle", "1");
                this.GeneratePreview();

                this.numericUpDownOutline.Enabled = rb.Checked;
                this.labelShadow.Enabled = rb.Checked;
                this.numericUpDownShadowWidth.Enabled = rb.Checked;
            }
        }

        /// <summary>
        /// The radio button opaque box_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonOpaqueBox_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null && this.listViewStyles.SelectedItems.Count == 1 && this._doUpdate && rb.Checked)
            {
                this.numericUpDownShadowWidth.Value = 0;
                string name = this.listViewStyles.SelectedItems[0].Text;
                this.SetSsaStyle(name, "outline", this.numericUpDownOutline.Value.ToString());
                this.SetSsaStyle(name, "borderstyle", "3");
                this.GeneratePreview();

                this.numericUpDownOutline.Enabled = !rb.Checked;
                this.labelShadow.Enabled = !rb.Checked;
                this.numericUpDownShadowWidth.Enabled = !rb.Checked;
            }
        }

        /// <summary>
        /// The list view styles_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewStyles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                this.buttonCopy_Click(null, null);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Delete)
            {
                this.buttonRemove_Click(null, null);
            }
        }

        /// <summary>
        /// The context menu strip styles_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void contextMenuStripStyles_Opening(object sender, CancelEventArgs e)
        {
            this.copyToolStripMenuItem.Visible = this.listViewStyles.SelectedItems.Count == 1;
            this.newToolStripMenuItem.Visible = this.listViewStyles.SelectedItems.Count == 1;
            this.removeToolStripMenuItem.Visible = this.listViewStyles.SelectedItems.Count == 1;
        }

        /// <summary>
        /// The copy tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.buttonCopy_Click(null, null);
        }

        /// <summary>
        /// The new tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.buttonAdd_Click(null, null);
        }

        /// <summary>
        /// The remove tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.buttonRemove_Click(null, null);
        }

        /// <summary>
        /// The remove all tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.buttonRemoveAll_Click(null, null);
        }

        /// <summary>
        /// The sub station alpha styles_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubStationAlphaStyles_SizeChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
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
            this.openFileDialogImport.Title = Configuration.Settings.Language.SubStationAlphaStyles.ImportStyleFromFile;
            this.openFileDialogImport.InitialDirectory = Configuration.DataDirectory;
            if (this._isSubStationAlpha)
            {
                this.openFileDialogImport.Filter = SubStationAlpha.NameOfFormat + "|*.ssa|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
                this.saveFileDialogStyle.FileName = "my_styles.ssa";
            }
            else
            {
                this.openFileDialogImport.Filter = AdvancedSubStationAlpha.NameOfFormat + "|*.ass|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
                this.saveFileDialogStyle.FileName = "my_styles.ass";
            }

            if (this.openFileDialogImport.ShowDialog(this) == DialogResult.OK)
            {
                Encoding encoding = null;
                var s = new Subtitle();
                var format = s.LoadSubtitle(this.openFileDialogImport.FileName, out encoding, null);
                if (format != null && format.HasStyleSupport)
                {
                    var styles = AdvancedSubStationAlpha.GetStylesFromHeader(s.Header);
                    if (styles.Count > 0)
                    {
                        var cs = new ChooseStyle(s, format.GetType() == typeof(SubStationAlpha));
                        if (cs.ShowDialog(this) == DialogResult.OK && cs.SelectedStyleName != null)
                        {
                            SsaStyle style = AdvancedSubStationAlpha.GetSsaStyle(cs.SelectedStyleName, s.Header);
                            if (this.GetSsaStyle(style.Name).LoadedFromHeader)
                            {
                                int count = 2;
                                bool doRepeat = this.GetSsaStyle(style.Name + count).LoadedFromHeader;
                                while (doRepeat)
                                {
                                    doRepeat = this.GetSsaStyle(style.Name + count).LoadedFromHeader;
                                    count++;
                                }

                                style.RawLine = style.RawLine.Replace(" " + style.Name + ",", " " + style.Name + count + ",");
                                style.Name = style.Name + count;
                            }

                            this._doUpdate = false;
                            AddStyle(this.listViewStyles, style, this.Subtitle, this._isSubStationAlpha);
                            SsaStyle oldStyle = this.GetSsaStyle(this.listViewStyles.Items[0].Text);
                            this._header = this._header.Trim();
                            if (this._header.EndsWith("[Events]"))
                            {
                                this._header = this._header.Substring(0, this._header.Length - "[Events]".Length).Trim();
                                this._header += Environment.NewLine + style.RawLine;
                                this._header += Environment.NewLine + Environment.NewLine + "[Events]" + Environment.NewLine;
                            }
                            else
                            {
                                this._header = this._header.Trim() + Environment.NewLine + style.RawLine + Environment.NewLine;
                            }

                            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Selected = true;
                            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].EnsureVisible();
                            this.listViewStyles.Items[this.listViewStyles.Items.Count - 1].Focused = true;
                            this.textBoxStyleName.Text = style.Name;
                            this.textBoxStyleName.Focus();
                            this._doUpdate = true;
                            this.SetControlsFromStyle(style);
                            this.listViewStyles_SelectedIndexChanged(null, null);

                            this.labelStatus.Text = string.Format(Configuration.Settings.Language.SubStationAlphaStyles.StyleXImportedFromFileY, style.Name, this.openFileDialogImport.FileName);
                            this.timerClearStatus.Start();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The make only one style.
        /// </summary>
        internal void MakeOnlyOneStyle()
        {
            this.groupBoxPreview.Top = this.groupBoxStyles.Top;
            this.groupBoxPreview.Height = this.groupBoxProperties.Height;
            this.groupBoxStyles.SendToBack();
        }

        /// <summary>
        /// The button export_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (this.listViewStyles.SelectedItems.Count != 1)
            {
                return;
            }

            string styleName = this.listViewStyles.SelectedItems[0].Text;

            this.saveFileDialogStyle.Title = Configuration.Settings.Language.SubStationAlphaStyles.ExportStyleToFile;
            this.saveFileDialogStyle.InitialDirectory = Configuration.DataDirectory;
            if (this._isSubStationAlpha)
            {
                this.saveFileDialogStyle.Filter = SubStationAlpha.NameOfFormat + "|*.ssa|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
                this.saveFileDialogStyle.FileName = "my_styles.ssa";
            }
            else
            {
                this.saveFileDialogStyle.Filter = AdvancedSubStationAlpha.NameOfFormat + "|*.ass|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
                this.saveFileDialogStyle.FileName = "my_styles.ass";
            }

            if (this.saveFileDialogStyle.ShowDialog(this) == DialogResult.OK)
            {
                if (File.Exists(this.saveFileDialogStyle.FileName))
                {
                    Encoding encoding = null;
                    var s = new Subtitle();
                    var format = s.LoadSubtitle(this.saveFileDialogStyle.FileName, out encoding, null);
                    if (format == null)
                    {
                        MessageBox.Show("Not subtitle format: " + this._format.Name);
                        return;
                    }
                    else if (format.Name != this._format.Name)
                    {
                        MessageBox.Show(string.Format("Cannot save {1} style in {0} file!", format.Name, this._format.Name));
                        return;
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        bool stylesOn = false;
                        bool done = false;
                        string styleFormat = "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding";
                        foreach (string line in File.ReadAllLines(this.saveFileDialogStyle.FileName))
                        {
                            if (line.StartsWith("format:", StringComparison.OrdinalIgnoreCase))
                            {
                                styleFormat = line;
                            }
                            else if (line.StartsWith("style:", StringComparison.OrdinalIgnoreCase))
                            {
                                stylesOn = true;
                            }
                            else if (stylesOn && !done)
                            {
                                done = true;
                                SsaStyle style = this.GetSsaStyle(styleName);
                                if (this._isSubStationAlpha)
                                {
                                    sb.AppendLine(style.ToRawSsa(styleFormat));
                                }
                                else
                                {
                                    sb.AppendLine(style.ToRawAss(styleFormat));
                                }
                            }

                            sb.AppendLine(line);
                            if (stylesOn && line.Replace(" ", string.Empty).TrimStart().StartsWith("style:" + styleName.Replace(" ", string.Empty).Trim() + ",", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show(string.Format(Configuration.Settings.Language.SubStationAlphaStyles.StyleAlreadyExits, styleName));
                                return;
                            }
                        }

                        File.WriteAllText(this.saveFileDialogStyle.FileName, sb.ToString(), Encoding.UTF8);
                    }
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (var line in this._header.Replace(Environment.NewLine, "\n").Split('\n'))
                    {
                        if (line.StartsWith("style:", StringComparison.OrdinalIgnoreCase))
                        {
                            if (line.ToLower().Replace(" ", string.Empty).StartsWith("style:" + styleName.ToLower().Trim()))
                            {
                                sb.AppendLine(line);
                            }
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }

                    string content = sb.ToString();
                    if (content.TrimEnd().EndsWith("[Events]"))
                    {
                        content = content.Trim() + Environment.NewLine + "Format: Layer, Start, End, Style, Actor, MarginL, MarginR, MarginV, Effect, Text" + Environment.NewLine + "Dialogue: 0,0:00:31.91,0:00:33.91,Default,,0,0,0,,My Styles :)";
                    }

                    File.WriteAllText(this.saveFileDialogStyle.FileName, content, Encoding.UTF8);
                }
            }

            this.labelStatus.Text = string.Format(Configuration.Settings.Language.SubStationAlphaStyles.StyleXExportedToFileY, styleName, this.saveFileDialogStyle.FileName);
            this.timerClearStatus.Start();
        }

        /// <summary>
        /// The timer clear status_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timerClearStatus_Tick(object sender, EventArgs e)
        {
            this.timerClearStatus.Stop();
            this.labelStatus.Text = string.Empty;
        }
    }
}