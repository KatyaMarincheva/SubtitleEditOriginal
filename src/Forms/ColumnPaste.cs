// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnPaste.cs" company="">
//   
// </copyright>
// <summary>
//   The column paste.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The column paste.
    /// </summary>
    public partial class ColumnPaste : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPaste"/> class.
        /// </summary>
        /// <param name="isOriginalAvailable">
        /// The is original available.
        /// </param>
        /// <param name="onlyText">
        /// The only text.
        /// </param>
        public ColumnPaste(bool isOriginalAvailable, bool onlyText)
        {
            this.InitializeComponent();
            Utilities.FixLargeFonts(this, this.buttonOK);

            this.radioButtonAll.Enabled = !onlyText;
            this.radioButtonTimeCodes.Enabled = !onlyText;
            this.radioButtonOriginalText.Visible = isOriginalAvailable;

            this.Text = Configuration.Settings.Language.ColumnPaste.Title;
            this.groupBoxChooseColumn.Text = Configuration.Settings.Language.ColumnPaste.ChooseColumn;
            this.groupBoxOverwriteOrInsert.Text = Configuration.Settings.Language.ColumnPaste.OverwriteShiftCellsDown;
            this.radioButtonOverwrite.Text = Configuration.Settings.Language.ColumnPaste.Overwrite;
            this.radioButtonShiftCellsDown.Text = Configuration.Settings.Language.ColumnPaste.ShiftCellsDown;
            this.radioButtonAll.Text = Configuration.Settings.Language.General.All;
            this.radioButtonTimeCodes.Text = Configuration.Settings.Language.ColumnPaste.TimeCodesOnly;
            this.radioButtonTextOnly.Text = Configuration.Settings.Language.ColumnPaste.TextOnly;
            this.radioButtonOriginalText.Text = Configuration.Settings.Language.ColumnPaste.OriginalTextOnly;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether paste all.
        /// </summary>
        public bool PasteAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether paste time codes only.
        /// </summary>
        public bool PasteTimeCodesOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether paste text only.
        /// </summary>
        public bool PasteTextOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether paste original text only.
        /// </summary>
        public bool PasteOriginalTextOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether paste overwrite.
        /// </summary>
        public bool PasteOverwrite { get; set; }

        /// <summary>
        /// The paste special_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PasteSpecial_KeyDown(object sender, KeyEventArgs e)
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
            this.PasteAll = this.radioButtonAll.Checked;
            this.PasteTimeCodesOnly = this.radioButtonTimeCodes.Checked;
            this.PasteTextOnly = this.radioButtonTextOnly.Checked;
            this.PasteOriginalTextOnly = this.radioButtonOriginalText.Checked;

            this.PasteOverwrite = this.radioButtonOverwrite.Checked;

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
    }
}