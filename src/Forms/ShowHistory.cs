// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowHistory.cs" company="">
//   
// </copyright>
// <summary>
//   The show history.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The show history.
    /// </summary>
    public sealed partial class ShowHistory : Form
    {
        /// <summary>
        /// The _selected index.
        /// </summary>
        private int _selectedIndex = -1;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _undo index.
        /// </summary>
        private int _undoIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowHistory"/> class.
        /// </summary>
        public ShowHistory()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.ShowHistory.Title;
            this.label1.Text = Configuration.Settings.Language.ShowHistory.SelectRollbackPoint;
            this.listViewHistory.Columns[0].Text = Configuration.Settings.Language.ShowHistory.Time;
            this.listViewHistory.Columns[1].Text = Configuration.Settings.Language.ShowHistory.Description;
            this.buttonCompare.Text = Configuration.Settings.Language.ShowHistory.CompareWithCurrent;
            this.buttonCompareHistory.Text = Configuration.Settings.Language.ShowHistory.CompareHistoryItems;
            this.buttonRollback.Text = Configuration.Settings.Language.ShowHistory.Rollback;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonRollback);
        }

        /// <summary>
        /// Gets the selected index.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this._selectedIndex;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="undoIndex">
        /// The undo index.
        /// </param>
        public void Initialize(Subtitle subtitle, int undoIndex)
        {
            this._subtitle = subtitle;
            this._undoIndex = undoIndex;
            int i = 0;
            foreach (HistoryItem item in subtitle.HistoryItems)
            {
                this.AddHistoryItemToListView(item, i);
                i++;
            }

            this.ListViewHistorySelectedIndexChanged(null, null);
            if (this.listViewHistory.Items.Count > 0 && this._undoIndex >= 0 && this._undoIndex < this.listViewHistory.Items.Count)
            {
                this.listViewHistory.Items[this._undoIndex].Selected = true;
            }
        }

        /// <summary>
        /// The add history item to list view.
        /// </summary>
        /// <param name="hi">
        /// The hi.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        private void AddHistoryItemToListView(HistoryItem hi, int index)
        {
            var item = new ListViewItem(string.Empty) { Tag = hi, Text = hi.ToHHMMSS() };

            if (index > this._undoIndex)
            {
                item.UseItemStyleForSubItems = true;
                item.Font = new Font(item.Font.FontFamily, item.Font.SizeInPoints, FontStyle.Italic);
                item.ForeColor = Color.Gray;
            }

            var subItem = new ListViewItem.ListViewSubItem(item, hi.Description);
            item.SubItems.Add(subItem);
            this.listViewHistory.Items.Add(item);
        }

        /// <summary>
        /// The form show history_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormShowHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button ok click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (this.listViewHistory.SelectedItems.Count > 0)
            {
                this._selectedIndex = this.listViewHistory.SelectedItems[0].Index;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button compare click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonCompareClick(object sender, EventArgs e)
        {
            if (this.listViewHistory.SelectedItems.Count == 1)
            {
                HistoryItem h2 = this._subtitle.HistoryItems[this.listViewHistory.SelectedItems[0].Index];
                string descr2 = h2.ToHHMMSS() + " - " + h2.Description;
                using (var compareForm = new Compare())
                {
                    compareForm.Initialize(this._subtitle, Configuration.Settings.Language.General.CurrentSubtitle, h2.Subtitle, descr2);
                    compareForm.ShowDialog(this);
                }
            }
        }

        /// <summary>
        /// The list view history selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewHistorySelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRollback.Enabled = this.listViewHistory.SelectedItems.Count == 1;
            this.buttonCompare.Enabled = this.listViewHistory.SelectedItems.Count == 1;
            this.buttonCompareHistory.Enabled = this.listViewHistory.SelectedItems.Count == 2;
            this.buttonRollback.Enabled = this.listViewHistory.SelectedItems.Count == 1 && this.listViewHistory.SelectedItems[0].Index <= this._undoIndex;
        }

        /// <summary>
        /// The button compare history click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonCompareHistoryClick(object sender, EventArgs e)
        {
            if (this.listViewHistory.SelectedItems.Count == 2)
            {
                HistoryItem h1 = this._subtitle.HistoryItems[this.listViewHistory.SelectedItems[0].Index];
                HistoryItem h2 = this._subtitle.HistoryItems[this.listViewHistory.SelectedItems[1].Index];
                string descr1 = h1.ToHHMMSS() + " - " + h1.Description;
                string descr2 = h2.ToHHMMSS() + " - " + h2.Description;
                using (var compareForm = new Compare())
                {
                    compareForm.Initialize(h1.Subtitle, descr1, h2.Subtitle, descr2);
                    compareForm.ShowDialog(this);
                }
            }
        }
    }
}