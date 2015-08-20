// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChooseStyle.cs" company="">
//   
// </copyright>
// <summary>
//   The choose style.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Forms.Styles;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The choose style.
    /// </summary>
    public partial class ChooseStyle : Form
    {
        /// <summary>
        /// The _is sub station alpha.
        /// </summary>
        private bool _isSubStationAlpha;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseStyle"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="isSubStationAlpha">
        /// The is sub station alpha.
        /// </param>
        public ChooseStyle(Subtitle subtitle, bool isSubStationAlpha)
        {
            this.SelectedStyleName = null;
            this.InitializeComponent();
            this._subtitle = subtitle;
            this._isSubStationAlpha = isSubStationAlpha;

            var l = Configuration.Settings.Language.SubStationAlphaStyles;
            this.Text = l.ChooseStyle;
            this.listViewStyles.Columns[0].Text = l.Name;
            this.listViewStyles.Columns[1].Text = l.FontName;
            this.listViewStyles.Columns[2].Text = l.FontSize;
            this.listViewStyles.Columns[3].Text = l.UseCount;
            this.listViewStyles.Columns[4].Text = l.Primary;
            this.listViewStyles.Columns[5].Text = l.Outline;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            this.InitializeListView();
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets or sets the selected style name.
        /// </summary>
        public string SelectedStyleName { get; set; }

        /// <summary>
        /// The choose style_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChooseStyle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
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
            if (this.listViewStyles.SelectedItems.Count > 0)
            {
                this.SelectedStyleName = this.listViewStyles.SelectedItems[0].Text;
            }

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
        /// The initialize list view.
        /// </summary>
        private void InitializeListView()
        {
            var styles = AdvancedSubStationAlpha.GetStylesFromHeader(this._subtitle.Header);
            this.listViewStyles.Items.Clear();
            foreach (string style in styles)
            {
                SsaStyle ssaStyle = AdvancedSubStationAlpha.GetSsaStyle(style, this._subtitle.Header);
                SubStationAlphaStyles.AddStyle(this.listViewStyles, ssaStyle, this._subtitle, this._isSubStationAlpha);
            }

            if (this.listViewStyles.Items.Count > 0)
            {
                this.listViewStyles.Items[0].Selected = true;
            }
        }
    }
}