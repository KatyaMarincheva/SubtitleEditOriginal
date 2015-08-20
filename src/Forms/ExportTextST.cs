// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ExportTextST.cs">
//   
// </copyright>
// <summary>
//   The export text st.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The export text st.
    /// </summary>
    public partial class ExportTextST : Form
    {
        /// <summary>
        /// The _current palette.
        /// </summary>
        private TextST.Palette _currentPalette;

        /// <summary>
        /// The _current region style.
        /// </summary>
        private TextST.RegionStyle _currentRegionStyle;

        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _root.
        /// </summary>
        private TreeNode _root;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _text st.
        /// </summary>
        private TextST _textST;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTextST"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public ExportTextST(Subtitle subtitle)
        {
            this.InitializeComponent();

            this._subtitle = subtitle;

            this.subtitleListView1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.subtitleListView1.Fill(this._subtitle);

            this.groupBoxPropertiesPalette.Left = this.groupBoxPropertiesRoot.Left;
            this.groupBoxPropertiesPalette.Top = this.groupBoxPropertiesRoot.Top;
            this.groupBoxPropertiesPalette.Size = this.groupBoxPropertiesRoot.Size;
            this.groupBoxPropertiesPalette.Anchor = this.groupBoxPropertiesRoot.Anchor;

            this.groupBoxPropertiesRegionStyle.Left = this.groupBoxPropertiesRoot.Left;
            this.groupBoxPropertiesRegionStyle.Top = this.groupBoxPropertiesRoot.Top;
            this.groupBoxPropertiesRegionStyle.Size = this.groupBoxPropertiesRoot.Size;
            this.groupBoxPropertiesRegionStyle.Anchor = this.groupBoxPropertiesRoot.Anchor;

            this.groupBoxPropertiesUserStyle.Left = this.groupBoxPropertiesRoot.Left;
            this.groupBoxPropertiesUserStyle.Top = this.groupBoxPropertiesRoot.Top;
            this.groupBoxPropertiesUserStyle.Size = this.groupBoxPropertiesRoot.Size;
            this.groupBoxPropertiesUserStyle.Anchor = this.groupBoxPropertiesRoot.Anchor;
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
        /// The button import click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonImportClick(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this._textST = new TextST();
                this._subtitle = new Subtitle();
                this._fileName = this.openFileDialog1.FileName;
                this._textST.LoadSubtitle(this._subtitle, null, this._fileName);
                this.groupBoxTextST.Text = "TextST structure: " + Path.GetFileName(this._fileName);

                this.subtitleListView1.Fill(this._subtitle);

                this.treeView1.Nodes.Clear();
                this._root = new TreeNode("TextST");
                this.treeView1.Nodes.Add(this._root);
                if (this._textST.StyleSegment != null)
                {
                    var regionsNode = new TreeNode(string.Format("Regions ({0})", this._textST.StyleSegment.Regions.Count));
                    this._root.Nodes.Add(regionsNode);
                    foreach (TextST.Region region in this._textST.StyleSegment.Regions)
                    {
                        var regionNode = new TreeNode("Region") { Tag = region };
                        regionsNode.Nodes.Add(regionNode);

                        var regionStyleNode = new TreeNode("Region style") { Tag = region.RegionStyle };
                        regionNode.Nodes.Add(regionStyleNode);

                        var userStylesNode = new TreeNode(string.Format("User styles ({0})", region.UserStyles.Count)) { Tag = region.UserStyles };
                        regionNode.Nodes.Add(userStylesNode);
                        foreach (var userStyle in region.UserStyles)
                        {
                            var userStyleNode = new TreeNode("User style") { Tag = userStyle };
                            userStylesNode.Nodes.Add(userStyleNode);
                        }
                    }

                    var palettesNode = new TreeNode(string.Format("Palettes ({0})", this._textST.StyleSegment.Palettes.Count)) { Tag = this._textST.StyleSegment.Palettes };
                    this._root.Nodes.Add(palettesNode);
                    foreach (TextST.Palette palette in this._textST.StyleSegment.Palettes)
                    {
                        var paletteNode = new TreeNode("Palette") { Tag = palette };
                        palettesNode.Nodes.Add(paletteNode);
                    }
                }

                if (this._textST.PresentationSegments != null)
                {
                    var presentationSegmentsNode = new TreeNode(string.Format("Presentation segments ({0})", this._textST.PresentationSegments.Count));
                    this._root.Nodes.Add(presentationSegmentsNode);
                    int count = 0;
                    foreach (TextST.DialogPresentationSegment segment in this._textST.PresentationSegments)
                    {
                        count++;
                        var presentationSegmentNode = new TreeNode(string.Format("Segment {0}: {1} -- > {2}", count, new TimeCode(segment.StartPtsMilliseconds), new TimeCode(segment.EndPtsMilliseconds))) { Tag = segment };
                        presentationSegmentsNode.Nodes.Add(presentationSegmentNode);
                    }
                }

                this.treeView1.ExpandAll();
                this.treeView1.SelectedNode = this._root;
            }
        }

        /// <summary>
        /// The tree view 1_ after select.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.groupBoxPropertiesRoot.Visible = false;
            this.groupBoxPropertiesPalette.Visible = false;
            this.groupBoxPropertiesRegionStyle.Visible = false;
            this.groupBoxPropertiesUserStyle.Visible = false;
            if (e.Node != null && this._textST != null)
            {
                if (e.Node == this._root)
                {
                    this.groupBoxPropertiesRoot.Visible = true;
                    this.textBoxRoot.Text = "File name: " + this._fileName + Environment.NewLine + "Number of style regions: " + this._textST.StyleSegment.Regions.Count + Environment.NewLine + "Number of style palettes: " + this._textST.StyleSegment.Palettes.Count + Environment.NewLine + "Number of subtitles: " + this._textST.StyleSegment.NumberOfDialogPresentationSegments + Environment.NewLine;
                }
                else if (e.Node.Tag is TextST.Palette)
                {
                    this.groupBoxPropertiesPalette.Visible = true;
                    this._currentPalette = e.Node.Tag as TextST.Palette;
                    this.numericUpDownPaletteEntry.Value = this._currentPalette.PaletteEntryId;
                    this.numericUpDownPaletteY.Value = this._currentPalette.Y;
                    this.numericUpDownPaletteCr.Value = this._currentPalette.Cr;
                    this.numericUpDownPaletteCb.Value = this._currentPalette.Cb;
                    this.numericUpDownPaletteOpacity.Value = this._currentPalette.T;
                    this.panelPaletteColor.BackColor = Color.FromArgb(255, this._currentPalette.Color);
                }
                else if (e.Node.Tag is TextST.RegionStyle)
                {
                    this.groupBoxPropertiesRegionStyle.Visible = true;
                    this._currentRegionStyle = e.Node.Tag as TextST.RegionStyle;
                    this.numericUpDownRegionStyleId.Value = this._currentRegionStyle.RegionStyleId;
                    this.numericUpDownRegionStyleHPos.Value = this._currentRegionStyle.RegionHorizontalPosition;
                    this.numericUpDownRegionStyleVPos.Value = this._currentRegionStyle.RegionVerticalPosition;
                    this.numericUpDownRegionStyleWidth.Value = this._currentRegionStyle.RegionWidth;
                    this.numericUpDownRegionStyleHeight.Value = this._currentRegionStyle.RegionHeight;
                    this.numericUpDownRegionStylePaletteEntryId.Value = this._currentRegionStyle.RegionBgPaletteEntryIdRef;

                    this.numericUpDownRegionStyleFontSize.Value = this._currentRegionStyle.FontSize;
                }
                else if (e.Node.Tag is TextST.UserStyle)
                {
                    this.groupBoxPropertiesUserStyle.Visible = true;
                }
                else if (e.Node.Tag is TextST.DialogPresentationSegment)
                {
                }
            }
        }

        /// <summary>
        /// The button color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonColor_Click(object sender, EventArgs e)
        {
            using (var colorChooser = new ColorChooser { Color = this.panelPaletteColor.BackColor, ShowAlpha = true })
            {
                if (colorChooser.ShowDialog() == DialogResult.OK)
                {
                    this.panelPaletteColor.BackColor = colorChooser.Color;
                }
            }
        }
    }
}