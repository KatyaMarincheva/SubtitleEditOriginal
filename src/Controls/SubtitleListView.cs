// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubtitleListView.cs" company="">
//   
// </copyright>
// <summary>
//   The subtitle list view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The subtitle list view.
    /// </summary>
    public sealed class SubtitleListView : ListView
    {
        /// <summary>
        /// The column index number.
        /// </summary>
        public const int ColumnIndexNumber = 0;

        /// <summary>
        /// The column index start.
        /// </summary>
        public const int ColumnIndexStart = 1;

        /// <summary>
        /// The column index end.
        /// </summary>
        public const int ColumnIndexEnd = 2;

        /// <summary>
        /// The column index duration.
        /// </summary>
        public const int ColumnIndexDuration = 3;

        /// <summary>
        /// The column index text.
        /// </summary>
        public const int ColumnIndexText = 4;

        /// <summary>
        /// The column index text alternate.
        /// </summary>
        public const int ColumnIndexTextAlternate = 5;

        /// <summary>
        /// The _first visible index.
        /// </summary>
        private int _firstVisibleIndex = -1;

        /// <summary>
        /// The _line separator string.
        /// </summary>
        private string _lineSeparatorString = " || ";

        /// <summary>
        /// The _save column width changes.
        /// </summary>
        private bool _saveColumnWidthChanges;

        /// <summary>
        /// The _settings.
        /// </summary>
        private Settings _settings;

        /// <summary>
        /// The _subtitle font.
        /// </summary>
        private Font _subtitleFont = new Font("Tahoma", 8.25F);

        /// <summary>
        /// The _subtitle font bold.
        /// </summary>
        private bool _subtitleFontBold;

        /// <summary>
        /// The _subtitle font name.
        /// </summary>
        private string _subtitleFontName = "Tahoma";

        /// <summary>
        /// The _subtitle font size.
        /// </summary>
        private int _subtitleFontSize = 8;

        /// <summary>
        /// The column index extra.
        /// </summary>
        public int ColumnIndexExtra = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtitleListView"/> class.
        /// </summary>
        public SubtitleListView()
        {
            this.UseSyntaxColoring = true;
            this.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.Columns.AddRange(new[] { new ColumnHeader { Text = "#", Width = 55 }, new ColumnHeader { Width = 80 }, new ColumnHeader { Width = 80 }, new ColumnHeader { Width = 55 }, new ColumnHeader { Width = -2 } // -2 = as rest of space (300)
                                        });
            this.SubtitleListViewResize(this, null);

            this.FullRowSelect = true;
            this.View = View.Details;
            this.Resize += this.SubtitleListViewResize;
            this.GridLines = true;
            this.ColumnWidthChanged += this.SubtitleListViewColumnWidthChanged;
            this.OwnerDraw = true;
            this.DrawItem += this.SubtitleListView_DrawItem;
            this.DrawSubItem += this.SubtitleListView_DrawSubItem;
            this.DrawColumnHeader += this.SubtitleListView_DrawColumnHeader;
        }

        /// <summary>
        /// Gets or sets the subtitle font name.
        /// </summary>
        public string SubtitleFontName
        {
            get
            {
                return this._subtitleFontName;
            }

            set
            {
                this._subtitleFontName = value;
                if (this.SubtitleFontBold)
                {
                    this._subtitleFont = new Font(this._subtitleFontName, this.SubtitleFontSize, FontStyle.Bold);
                }
                else
                {
                    this._subtitleFont = new Font(this._subtitleFontName, this.SubtitleFontSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether subtitle font bold.
        /// </summary>
        public bool SubtitleFontBold
        {
            get
            {
                return this._subtitleFontBold;
            }

            set
            {
                this._subtitleFontBold = value;
                if (this.SubtitleFontBold)
                {
                    this._subtitleFont = new Font(this._subtitleFontName, this.SubtitleFontSize, FontStyle.Bold);
                }
                else
                {
                    this._subtitleFont = new Font(this._subtitleFontName, this.SubtitleFontSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets the subtitle font size.
        /// </summary>
        public int SubtitleFontSize
        {
            get
            {
                return this._subtitleFontSize;
            }

            set
            {
                this._subtitleFontSize = value;
                if (this.SubtitleFontBold)
                {
                    this._subtitleFont = new Font(this._subtitleFontName, this.SubtitleFontSize, FontStyle.Bold);
                }
                else
                {
                    this._subtitleFont = new Font(this._subtitleFontName, this.SubtitleFontSize);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether is alternate text column visible.
        /// </summary>
        public bool IsAlternateTextColumnVisible { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is extra column visible.
        /// </summary>
        public bool IsExtraColumnVisible { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether display extra from extra.
        /// </summary>
        public bool DisplayExtraFromExtra { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use syntax coloring.
        /// </summary>
        public bool UseSyntaxColoring { get; set; }

        /// <summary>
        /// Gets or sets the first visible index.
        /// </summary>
        public int FirstVisibleIndex
        {
            get
            {
                return this._firstVisibleIndex;
            }

            set
            {
                this._firstVisibleIndex = value;
            }
        }

        /// <summary>
        /// The initialize language.
        /// </summary>
        /// <param name="general">
        /// The general.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public void InitializeLanguage(LanguageStructure.General general, Settings settings)
        {
            this.Columns[ColumnIndexNumber].Text = general.NumberSymbol;
            this.Columns[ColumnIndexStart].Text = general.StartTime;
            this.Columns[ColumnIndexEnd].Text = general.EndTime;
            this.Columns[ColumnIndexDuration].Text = general.Duration;
            this.Columns[ColumnIndexText].Text = general.Text;
            if (settings.General.ListViewLineSeparatorString != null)
            {
                this._lineSeparatorString = settings.General.ListViewLineSeparatorString;
            }

            if (!string.IsNullOrEmpty(settings.General.SubtitleFontName))
            {
                this._subtitleFontName = settings.General.SubtitleFontName;
            }

            this.SubtitleFontBold = settings.General.SubtitleFontBold;
            if (settings.General.SubtitleFontSize > 6 && settings.General.SubtitleFontSize < 72)
            {
                this.SubtitleFontSize = settings.General.SubtitleFontSize;
            }

            this.ForeColor = settings.General.SubtitleFontColor;
            this.BackColor = settings.General.SubtitleBackgroundColor;
            this._settings = settings;
        }

        /// <summary>
        /// The initialize timestamp column widths.
        /// </summary>
        /// <param name="parentForm">
        /// The parent form.
        /// </param>
        public void InitializeTimestampColumnWidths(Form parentForm)
        {
            if (this._settings != null && this._settings.General.ListViewColumnsRememberSize && this._settings.General.ListViewNumberWidth > 1 && this._settings.General.ListViewStartWidth > 1 && this._settings.General.ListViewEndWidth > 1 && this._settings.General.ListViewDurationWidth > 1)
            {
                this.Columns[ColumnIndexNumber].Width = this._settings.General.ListViewNumberWidth;
                this.Columns[ColumnIndexStart].Width = this._settings.General.ListViewStartWidth;
                this.Columns[ColumnIndexEnd].Width = this._settings.General.ListViewEndWidth;
                this.Columns[ColumnIndexDuration].Width = this._settings.General.ListViewDurationWidth;
                this.Columns[ColumnIndexText].Width = this._settings.General.ListViewTextWidth;
                this._saveColumnWidthChanges = true;
            }
            else
            {
                using (var graphics = parentForm.CreateGraphics())
                {
                    var timestampSizeF = graphics.MeasureString("00:00:33,527", this.Font);
                    var timestampWidth = (int)(timestampSizeF.Width + 0.5) + 11;
                    this.Columns[ColumnIndexStart].Width = timestampWidth;
                    this.Columns[ColumnIndexEnd].Width = timestampWidth;
                    this.Columns[ColumnIndexDuration].Width = (int)(timestampWidth * 0.8);
                }
            }

            this.SubtitleListViewResize(this, null);
        }

        /// <summary>
        /// The subtitle list view_ draw column header.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        /// <summary>
        /// The is vertical scrollbar visible.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsVerticalScrollbarVisible()
        {
            if (this.Items.Count < 2)
            {
                return false;
            }

            int singleRowHeight = this.GetItemRect(0).Height;
            int maxVisibleItems = (this.Height - this.TopItem.Bounds.Top) / singleRowHeight;

            return this.Items.Count > maxVisibleItems;
        }

        /// <summary>
        /// The subtitle list view_ draw sub item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Color backgroundColor = this.Items[e.ItemIndex].SubItems[e.ColumnIndex].BackColor;
            if (this.Focused && backgroundColor == this.BackColor)
            {
                e.DrawDefault = true;
                return;
            }

            using (var sf = new StringFormat())
            {
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                if (e.Item.Selected)
                {
                    if (this.RightToLeftLayout)
                    {
                        int w = this.Columns.Count;
                        for (int i = 0; i < this.Columns.Count; i++)
                        {
                            w += this.Columns[i].Width;
                        }

                        int extra = 0;
                        int extra2 = 0;
                        if (!this.IsVerticalScrollbarVisible())
                        {
                            // no vertical scrollbar
                            extra = 14;
                            extra2 = 11;
                        }
                        else
                        {
                            // no vertical scrollbar
                            extra = -3;
                            extra2 = -5;
                        }

                        var rect = new Rectangle(w - (e.Bounds.Left + e.Bounds.Width + 2) + extra, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
                        if (Configuration.Settings != null)
                        {
                            if (backgroundColor == this.BackColor)
                            {
                                backgroundColor = Configuration.Settings.Tools.ListViewUnfocusedSelectedColor;
                            }
                            else
                            {
                                int r = backgroundColor.R - 39;
                                int g = backgroundColor.G - 39;
                                int b = backgroundColor.B - 39;
                                if (r < 0)
                                {
                                    r = 0;
                                }

                                if (g < 0)
                                {
                                    g = 0;
                                }

                                if (b < 0)
                                {
                                    b = 0;
                                }

                                backgroundColor = Color.FromArgb(backgroundColor.A, r, g, b);
                            }

                            var sb = new SolidBrush(backgroundColor);
                            e.Graphics.FillRectangle(sb, rect);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(Brushes.LightBlue, rect);
                        }

                        var rtlBounds = new Rectangle(w - (e.Bounds.Left + e.Bounds.Width) + extra2, e.Bounds.Top + 2, e.Bounds.Width, e.Bounds.Height);
                        sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                        e.Graphics.DrawString(e.SubItem.Text, this.Font, new SolidBrush(e.Item.ForeColor), rtlBounds, sf);
                    }
                    else
                    {
                        Rectangle rect = e.Bounds;
                        if (Configuration.Settings != null)
                        {
                            if (backgroundColor == this.BackColor)
                            {
                                backgroundColor = Configuration.Settings.Tools.ListViewUnfocusedSelectedColor;
                            }
                            else
                            {
                                int r = backgroundColor.R - 39;
                                int g = backgroundColor.G - 39;
                                int b = backgroundColor.B - 39;
                                if (r < 0)
                                {
                                    r = 0;
                                }

                                if (g < 0)
                                {
                                    g = 0;
                                }

                                if (b < 0)
                                {
                                    b = 0;
                                }

                                backgroundColor = Color.FromArgb(backgroundColor.A, r, g, b);
                            }

                            var sb = new SolidBrush(backgroundColor);
                            e.Graphics.FillRectangle(sb, rect);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(Brushes.LightBlue, rect);
                        }

                        TextRenderer.DrawText(e.Graphics, e.Item.SubItems[e.ColumnIndex].Text, this._subtitleFont, new Point(e.Bounds.Left + 3, e.Bounds.Top + 2), e.Item.ForeColor, TextFormatFlags.NoPrefix);
                    }
                }
                else
                {
                    e.DrawDefault = true;
                }
            }
        }

        /// <summary>
        /// The subtitle list view_ draw item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (!this.Focused && (e.State & ListViewItemStates.Selected) != 0)
            {
                // Rectangle r = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                // e.Graphics.FillRectangle(Brushes.LightGoldenrodYellow, r);
                if (e.Item.Focused)
                {
                    e.DrawFocusRectangle();
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        /// <summary>
        /// The subtitle list view column width changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListViewColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this._settings != null && this._saveColumnWidthChanges)
            {
                switch (e.ColumnIndex)
                {
                    case ColumnIndexNumber:
                        Configuration.Settings.General.ListViewNumberWidth = this.Columns[ColumnIndexNumber].Width;
                        break;
                    case ColumnIndexStart:
                        Configuration.Settings.General.ListViewStartWidth = this.Columns[ColumnIndexStart].Width;
                        break;
                    case ColumnIndexEnd:
                        Configuration.Settings.General.ListViewEndWidth = this.Columns[ColumnIndexEnd].Width;
                        break;
                    case ColumnIndexDuration:
                        Configuration.Settings.General.ListViewDurationWidth = this.Columns[ColumnIndexDuration].Width;
                        break;
                    case ColumnIndexText:
                        Configuration.Settings.General.ListViewTextWidth = this.Columns[ColumnIndexText].Width;
                        break;
                }
            }
        }

        /// <summary>
        /// The auto size all columns.
        /// </summary>
        /// <param name="parentForm">
        /// The parent form.
        /// </param>
        public void AutoSizeAllColumns(Form parentForm)
        {
            if (this._settings != null && this._settings.General.ListViewColumnsRememberSize && this._settings.General.ListViewNumberWidth > 1)
            {
                this.Columns[ColumnIndexNumber].Width = this._settings.General.ListViewNumberWidth;
            }
            else
            {
                this.Columns[ColumnIndexNumber].Width = 55;
            }

            this.InitializeTimestampColumnWidths(parentForm);

            int length = this.Columns[ColumnIndexNumber].Width + this.Columns[ColumnIndexStart].Width + this.Columns[ColumnIndexEnd].Width + this.Columns[ColumnIndexDuration].Width;
            int lengthAvailable = this.Width - length;

            int numberOfRestColumns = 1;
            if (this.IsAlternateTextColumnVisible)
            {
                numberOfRestColumns++;
            }

            if (this.IsExtraColumnVisible)
            {
                numberOfRestColumns++;
            }

            if (this.IsAlternateTextColumnVisible && !this.IsExtraColumnVisible)
            {
                if (this._settings != null && this._settings.General.ListViewColumnsRememberSize && this._settings.General.ListViewNumberWidth > 1 && this._settings.General.ListViewStartWidth > 1 && this._settings.General.ListViewEndWidth > 1 && this._settings.General.ListViewDurationWidth > 1)
                {
                    int restWidth = lengthAvailable - 15 - this.Columns[ColumnIndexText].Width;
                    if (restWidth > 0)
                    {
                        this.Columns[ColumnIndexTextAlternate].Width = restWidth;
                    }
                }
                else
                {
                    int restWidth = (lengthAvailable / 2) - 15;
                    this.Columns[ColumnIndexText].Width = restWidth;
                    this.Columns[ColumnIndexTextAlternate].Width = restWidth;
                }
            }
            else if (!this.IsAlternateTextColumnVisible && !this.IsExtraColumnVisible)
            {
                int restWidth = lengthAvailable - 23;
                this.Columns[ColumnIndexText].Width = restWidth;
            }
            else if (!this.IsAlternateTextColumnVisible && this.IsExtraColumnVisible)
            {
                int restWidth = lengthAvailable - 15;
                this.Columns[ColumnIndexText].Width = (int)(restWidth * 0.6);
                this.Columns[this.ColumnIndexExtra].Width = (int)(restWidth * 0.4);
            }
            else
            {
                int restWidth = lengthAvailable - 15;
                this.Columns[ColumnIndexText].Width = (int)(restWidth * 0.4);
                this.Columns[ColumnIndexTextAlternate].Width = (int)(restWidth * 0.4);
                this.Columns[this.ColumnIndexExtra].Width = (int)(restWidth * 0.2);
            }
        }

        /// <summary>
        /// The show alternate text column.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public void ShowAlternateTextColumn(string text)
        {
            if (!this.IsAlternateTextColumnVisible)
            {
                this.ColumnIndexExtra = ColumnIndexTextAlternate + 1;
                if (this.IsExtraColumnVisible)
                {
                    this.Columns.Insert(ColumnIndexTextAlternate, new ColumnHeader { Text = text, Width = -2 });
                }
                else
                {
                    this.Columns.Add(new ColumnHeader { Text = text, Width = -2 });
                }

                int length = this.Columns[ColumnIndexNumber].Width + this.Columns[ColumnIndexStart].Width + this.Columns[ColumnIndexEnd].Width + this.Columns[ColumnIndexDuration].Width;
                int lengthAvailable = this.Width - length;
                this.Columns[ColumnIndexText].Width = (lengthAvailable / 2) - 15;
                this.Columns[ColumnIndexTextAlternate].Width = -2;

                this.IsAlternateTextColumnVisible = true;
            }
        }

        /// <summary>
        /// The hide alternate text column.
        /// </summary>
        public void HideAlternateTextColumn()
        {
            if (this.IsAlternateTextColumnVisible)
            {
                this.IsAlternateTextColumnVisible = false;
                this.Columns.RemoveAt(ColumnIndexTextAlternate);
                this.ColumnIndexExtra = ColumnIndexTextAlternate;
                this.SubtitleListViewResize(null, null);
            }
        }

        /// <summary>
        /// The subtitle list view resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListViewResize(object sender, EventArgs e)
        {
            int width = 0;
            for (int i = 0; i < this.Columns.Count - 1; i++)
            {
                width += this.Columns[i].Width;
            }

            this.Columns[this.Columns.Count - 1].Width = this.Width - (width + 25);
        }

        /// <summary>
        /// The get first visible item.
        /// </summary>
        /// <returns>
        /// The <see cref="ListViewItem"/>.
        /// </returns>
        private ListViewItem GetFirstVisibleItem()
        {
            foreach (ListViewItem item in this.Items)
            {
                if (this.ClientRectangle.Contains(new Rectangle(item.Bounds.Left, item.Bounds.Top, item.Bounds.Height, 10)))
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// The save first visible index.
        /// </summary>
        public void SaveFirstVisibleIndex()
        {
            ListViewItem first = this.GetFirstVisibleItem();
            if (this.Items.Count > 0 && first != null)
            {
                this.FirstVisibleIndex = first.Index;
            }
            else
            {
                this.FirstVisibleIndex = -1;
            }
        }

        /// <summary>
        /// The restore first visible index.
        /// </summary>
        private void RestoreFirstVisibleIndex()
        {
            if (this.FirstVisibleIndex >= 0 && this.FirstVisibleIndex < this.Items.Count)
            {
                if (this.FirstVisibleIndex + 1 < this.Items.Count)
                {
                    this.FirstVisibleIndex++;
                }

                this.Items[this.Items.Count - 1].EnsureVisible();
                this.Items[this.FirstVisibleIndex].EnsureVisible();
            }
        }

        /// <summary>
        /// The fill.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="subtitleAlternate">
        /// The subtitle alternate.
        /// </param>
        internal void Fill(Subtitle subtitle, Subtitle subtitleAlternate = null)
        {
            if (subtitleAlternate == null)
            {
                this.Fill(subtitle.Paragraphs);
            }
            else
            {
                this.Fill(subtitle.Paragraphs, subtitleAlternate.Paragraphs);
            }
        }

        /// <summary>
        /// The fill.
        /// </summary>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        internal void Fill(List<Paragraph> paragraphs)
        {
            this.SaveFirstVisibleIndex();
            this.BeginUpdate();
            this.Items.Clear();
            var x = this.ListViewItemSorter;
            this.ListViewItemSorter = null;
            int i = 0;
            foreach (Paragraph paragraph in paragraphs)
            {
                this.Add(paragraph);
                if (this.DisplayExtraFromExtra && this.IsExtraColumnVisible && this.Items[i].SubItems.Count > this.ColumnIndexExtra)
                {
                    this.Items[i].SubItems[this.ColumnIndexExtra].Text = paragraph.Extra;
                }

                this.SyntaxColorLine(paragraphs, i, paragraph);
                i++;
            }

            this.ListViewItemSorter = x;
            this.EndUpdate();

            if (this.FirstVisibleIndex == 0)
            {
                this.FirstVisibleIndex = -1;
            }
        }

        /// <summary>
        /// The fill.
        /// </summary>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        /// <param name="paragraphsAlternate">
        /// The paragraphs alternate.
        /// </param>
        internal void Fill(List<Paragraph> paragraphs, List<Paragraph> paragraphsAlternate)
        {
            this.SaveFirstVisibleIndex();
            this.BeginUpdate();
            this.Items.Clear();
            var x = this.ListViewItemSorter;
            this.ListViewItemSorter = null;
            int i = 0;
            foreach (Paragraph paragraph in paragraphs)
            {
                this.Add(paragraph);
                Paragraph alternate = Utilities.GetOriginalParagraph(i, paragraph, paragraphsAlternate);
                if (alternate != null)
                {
                    this.SetAlternateText(i, alternate.Text);
                }

                if (this.DisplayExtraFromExtra && this.IsExtraColumnVisible)
                {
                    this.SetExtraText(i, paragraph.Extra, this.ForeColor);
                }

                this.SyntaxColorLine(paragraphs, i, paragraph);
                i++;
            }

            this.ListViewItemSorter = x;
            this.EndUpdate();

            if (this.FirstVisibleIndex == 0)
            {
                this.FirstVisibleIndex = -1;
            }
        }

        /// <summary>
        /// The syntax color line.
        /// </summary>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        public void SyntaxColorLine(List<Paragraph> paragraphs, int i, Paragraph paragraph)
        {
            if (this.UseSyntaxColoring && this._settings != null && this.Items.Count > 0 && i < this.Items.Count)
            {
                var item = this.Items[i];
                if (item.UseItemStyleForSubItems)
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[ColumnIndexDuration].BackColor = this.BackColor;
                }

                bool durationChanged = false;
                if (this._settings.Tools.ListViewSyntaxColorDurationSmall)
                {
                    double charactersPerSecond = Utilities.GetCharactersPerSecond(paragraph);
                    if (charactersPerSecond > Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds)
                    {
                        item.SubItems[ColumnIndexDuration].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                        durationChanged = true;
                    }
                    else if (paragraph.Duration.TotalMilliseconds < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds)
                    {
                        item.SubItems[ColumnIndexDuration].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                        durationChanged = true;
                    }
                }

                if (this._settings.Tools.ListViewSyntaxColorDurationBig)
                {
                    // double charactersPerSecond = Utilities.GetCharactersPerSecond(paragraph);
                    if (paragraph.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                    {
                        item.SubItems[ColumnIndexDuration].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                        durationChanged = true;
                    }
                }

                if (!durationChanged && item.SubItems[ColumnIndexDuration].BackColor != this.BackColor)
                {
                    item.SubItems[ColumnIndexDuration].BackColor = this.BackColor;
                }

                if (this._settings.Tools.ListViewSyntaxColorOverlap && i > 0 && i < paragraphs.Count)
                {
                    Paragraph prev = paragraphs[i - 1];
                    if (paragraph.StartTime.TotalMilliseconds < prev.EndTime.TotalMilliseconds)
                    {
                        this.Items[i - 1].SubItems[ColumnIndexEnd].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                        item.SubItems[ColumnIndexStart].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                    }
                    else
                    {
                        if (this.Items[i - 1].SubItems[ColumnIndexEnd].BackColor != this.BackColor)
                        {
                            this.Items[i - 1].SubItems[ColumnIndexEnd].BackColor = this.BackColor;
                        }

                        if (item.SubItems[ColumnIndexStart].BackColor != this.BackColor)
                        {
                            item.SubItems[ColumnIndexStart].BackColor = this.BackColor;
                        }
                    }
                }

                if (this._settings.Tools.ListViewSyntaxColorLongLines)
                {
                    int noOfLines = paragraph.Text.Split(Environment.NewLine[0]).Length;
                    string s = HtmlUtil.RemoveHtmlTags(paragraph.Text, true);
                    foreach (string line in s.SplitToLines())
                    {
                        if (line.Length > Configuration.Settings.General.SubtitleLineMaximumLength)
                        {
                            item.SubItems[ColumnIndexText].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                            return;
                        }
                    }

                    s = s.Replace(Environment.NewLine, string.Empty); // we don't count new line in total length... correct?
                    if (s.Length <= Configuration.Settings.General.SubtitleLineMaximumLength * noOfLines)
                    {
                        if (noOfLines > Configuration.Settings.Tools.ListViewSyntaxMoreThanXLinesX && this._settings.Tools.ListViewSyntaxMoreThanXLines)
                        {
                            item.SubItems[ColumnIndexText].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                        }
                        else if (item.SubItems[ColumnIndexText].BackColor != this.BackColor)
                        {
                            item.SubItems[ColumnIndexText].BackColor = this.BackColor;
                        }
                    }
                    else
                    {
                        item.SubItems[ColumnIndexText].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                    }
                }

                if (this._settings.Tools.ListViewSyntaxMoreThanXLines && item.SubItems[ColumnIndexText].BackColor != Configuration.Settings.Tools.ListViewSyntaxErrorColor)
                {
                    int newLines = paragraph.Text.SplitToLines().Length;
                    if (newLines > Configuration.Settings.Tools.ListViewSyntaxMoreThanXLinesX)
                    {
                        item.SubItems[ColumnIndexText].BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                    }
                }
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        private void Add(Paragraph paragraph)
        {
            var item = new ListViewItem(paragraph.Number.ToString(CultureInfo.InvariantCulture)) { Tag = paragraph };
            ListViewItem.ListViewSubItem subItem;

            if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
            {
                if (paragraph.StartTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.StartTime.ToHHMMSSFF());
                }

                item.SubItems.Add(subItem);

                if (paragraph.EndTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.EndTime.ToHHMMSSFF());
                }

                item.SubItems.Add(subItem);

                subItem = new ListViewItem.ListViewSubItem(item, string.Format("{0},{1:00}", paragraph.Duration.Seconds, Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFramesMaxFrameRate(paragraph.Duration.Milliseconds)));
                item.SubItems.Add(subItem);
            }
            else
            {
                if (paragraph.StartTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.StartTime.ToString());
                }

                item.SubItems.Add(subItem);

                if (paragraph.EndTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.EndTime.ToString());
                }

                item.SubItems.Add(subItem);

                subItem = new ListViewItem.ListViewSubItem(item, string.Format("{0},{1:000}", paragraph.Duration.Seconds, paragraph.Duration.Milliseconds));
                item.SubItems.Add(subItem);
            }

            subItem = new ListViewItem.ListViewSubItem(item, paragraph.Text.Replace(Environment.NewLine, this._lineSeparatorString));
            if (this.SubtitleFontBold)
            {
                subItem.Font = new Font(this._subtitleFontName, this.SubtitleFontSize, FontStyle.Bold);
            }
            else
            {
                subItem.Font = new Font(this._subtitleFontName, this.SubtitleFontSize);
            }

            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(subItem);

            this.Items.Add(item);
        }

        /// <summary>
        /// The select none.
        /// </summary>
        public void SelectNone()
        {
            foreach (ListViewItem item in this.SelectedItems)
            {
                item.Selected = false;
            }
        }

        /// <summary>
        /// The select index and ensure visible.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="focus">
        /// The focus.
        /// </param>
        public void SelectIndexAndEnsureVisible(int index, bool focus)
        {
            if (index < 0 || index >= this.Items.Count || this.Items.Count == 0)
            {
                return;
            }

            if (this.TopItem == null)
            {
                return;
            }

            int bottomIndex = this.TopItem.Index + ((this.Height - 25) / 16);
            int itemsBeforeAfterCount = ((bottomIndex - this.TopItem.Index) / 2) - 1;
            if (itemsBeforeAfterCount < 0)
            {
                itemsBeforeAfterCount = 1;
            }

            int beforeIndex = index - itemsBeforeAfterCount;
            if (beforeIndex < 0)
            {
                beforeIndex = 0;
            }

            int afterIndex = index + itemsBeforeAfterCount;
            if (afterIndex >= this.Items.Count)
            {
                afterIndex = this.Items.Count - 1;
            }

            this.SelectNone();
            if (this.TopItem.Index <= beforeIndex && bottomIndex > afterIndex)
            {
                this.Items[index].Selected = true;
                this.Items[index].EnsureVisible();
                if (focus)
                {
                    this.Items[index].Focused = true;
                }

                return;
            }

            this.Items[beforeIndex].EnsureVisible();
            this.EnsureVisible(beforeIndex);
            this.Items[afterIndex].EnsureVisible();
            this.EnsureVisible(afterIndex);
            this.Items[index].Selected = true;
            this.Items[index].EnsureVisible();
            if (focus)
            {
                this.Items[index].Focused = true;
            }
        }

        /// <summary>
        /// The select index and ensure visible.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void SelectIndexAndEnsureVisible(int index)
        {
            this.SelectIndexAndEnsureVisible(index, false);
        }

        /// <summary>
        /// The select index and ensure visible.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        public void SelectIndexAndEnsureVisible(Paragraph p)
        {
            this.SelectNone();
            if (p == null)
            {
                return;
            }

            foreach (ListViewItem item in this.Items)
            {
                if (item.Text == p.Number.ToString(CultureInfo.InvariantCulture) && item.SubItems[ColumnIndexStart].Text == p.StartTime.ToString() && item.SubItems[ColumnIndexEnd].Text == p.EndTime.ToString() && item.SubItems[ColumnIndexText].Text == p.Text)
                {
                    this.RestoreFirstVisibleIndex();
                    item.Selected = true;
                    item.EnsureVisible();
                    return;
                }
            }
        }

        /// <summary>
        /// The get selected paragraph.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        public Paragraph GetSelectedParagraph(Subtitle subtitle)
        {
            if (subtitle != null && this.SelectedItems.Count > 0)
            {
                return subtitle.GetParagraphOrDefault(this.SelectedItems[0].Index);
            }

            return null;
        }

        /// <summary>
        /// The get text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetText(int index)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                return this.Items[index].SubItems[ColumnIndexText].Text.Replace(this._lineSeparatorString, Environment.NewLine);
            }

            return null;
        }

        /// <summary>
        /// The get text alternate.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTextAlternate(int index)
        {
            if (index >= 0 && index < this.Items.Count && this.IsAlternateTextColumnVisible)
            {
                return this.Items[index].SubItems[ColumnIndexTextAlternate].Text.Replace(this._lineSeparatorString, Environment.NewLine);
            }

            return null;
        }

        /// <summary>
        /// The set text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void SetText(int index, string text)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                this.Items[index].SubItems[ColumnIndexText].Text = text.Replace(Environment.NewLine, this._lineSeparatorString);
            }
        }

        /// <summary>
        /// The set time and text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        public void SetTimeAndText(int index, Paragraph paragraph)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];

                if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    if (paragraph.StartTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexStart].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexStart].Text = paragraph.StartTime.ToHHMMSSFF();
                    }

                    if (paragraph.EndTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexEnd].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToHHMMSSFF();
                    }

                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:00}", paragraph.Duration.Seconds, Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFramesMaxFrameRate(paragraph.Duration.Milliseconds));
                }
                else
                {
                    if (paragraph.StartTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexStart].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexStart].Text = paragraph.StartTime.ToString();
                    }

                    if (paragraph.EndTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexEnd].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToString();
                    }

                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:000}", paragraph.Duration.Seconds, paragraph.Duration.Milliseconds);
                }

                this.Items[index].SubItems[ColumnIndexText].Text = paragraph.Text.Replace(Environment.NewLine, this._lineSeparatorString);
            }
        }

        /// <summary>
        /// The show extra column.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void ShowExtraColumn(string title)
        {
            if (!this.IsExtraColumnVisible)
            {
                if (this.IsAlternateTextColumnVisible)
                {
                    this.ColumnIndexExtra = ColumnIndexTextAlternate + 1;
                }
                else
                {
                    this.ColumnIndexExtra = ColumnIndexTextAlternate;
                }

                this.Columns.Add(new ColumnHeader { Text = title, Width = 80 });

                int length = this.Columns[ColumnIndexNumber].Width + this.Columns[ColumnIndexStart].Width + this.Columns[ColumnIndexEnd].Width + this.Columns[ColumnIndexDuration].Width;
                int lengthAvailable = this.Width - length;

                if (this.IsAlternateTextColumnVisible)
                {
                    int part = lengthAvailable / 5;
                    this.Columns[ColumnIndexText].Width = part * 2;
                    this.Columns[ColumnIndexTextAlternate].Width = part * 2;
                    this.Columns[ColumnIndexTextAlternate].Width = part;
                }
                else
                {
                    int part = lengthAvailable / 6;
                    this.Columns[ColumnIndexText].Width = part * 4;
                    this.Columns[ColumnIndexTextAlternate].Width = part * 2;
                }

                this.IsExtraColumnVisible = true;
            }
        }

        /// <summary>
        /// The hide extra column.
        /// </summary>
        public void HideExtraColumn()
        {
            if (this.IsExtraColumnVisible)
            {
                this.IsExtraColumnVisible = false;

                if (this.IsAlternateTextColumnVisible)
                {
                    this.ColumnIndexExtra = ColumnIndexTextAlternate + 1;
                }
                else
                {
                    this.ColumnIndexExtra = ColumnIndexTextAlternate;
                }

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].SubItems.Count == this.ColumnIndexExtra + 1)
                    {
                        this.Items[i].SubItems[this.ColumnIndexExtra].Text = string.Empty;
                        this.Items[i].SubItems[this.ColumnIndexExtra].BackColor = this.BackColor;
                        this.Items[i].SubItems[this.ColumnIndexExtra].ForeColor = this.ForeColor;
                    }
                }

                this.Columns.RemoveAt(this.ColumnIndexExtra);
                this.SubtitleListViewResize(null, null);
            }
        }

        /// <summary>
        /// The set extra text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public void SetExtraText(int index, string text, Color color)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                if (this.IsAlternateTextColumnVisible)
                {
                    this.ColumnIndexExtra = ColumnIndexTextAlternate + 1;
                }
                else
                {
                    this.ColumnIndexExtra = ColumnIndexTextAlternate;
                }

                if (!this.IsExtraColumnVisible)
                {
                    this.ShowExtraColumn(string.Empty);
                }

                if (this.Items[index].SubItems.Count <= this.ColumnIndexExtra)
                {
                    this.Items[index].SubItems.Add(new ListViewItem.ListViewSubItem());
                }

                if (this.Items[index].SubItems.Count <= this.ColumnIndexExtra)
                {
                    this.Items[index].SubItems.Add(new ListViewItem.ListViewSubItem());
                }

                this.Items[index].SubItems[this.ColumnIndexExtra].Text = text;

                this.Items[index].UseItemStyleForSubItems = false;
                this.Items[index].SubItems[this.ColumnIndexExtra].BackColor = Color.AntiqueWhite;
                this.Items[index].SubItems[this.ColumnIndexExtra].ForeColor = color;
            }
        }

        /// <summary>
        /// The set alternate text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void SetAlternateText(int index, string text)
        {
            if (index >= 0 && index < this.Items.Count && this.Columns.Count >= ColumnIndexTextAlternate + 1)
            {
                if (this.Items[index].SubItems.Count <= ColumnIndexTextAlternate)
                {
                    var subItem = new ListViewItem.ListViewSubItem(this.Items[index], text.Replace(Environment.NewLine, this._lineSeparatorString));
                    this.Items[index].SubItems.Add(subItem);
                }
                else
                {
                    this.Items[index].SubItems[ColumnIndexTextAlternate].Text = text.Replace(Environment.NewLine, this._lineSeparatorString);
                }
            }
        }

        /// <summary>
        /// The set duration.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        public void SetDuration(int index, Paragraph paragraph)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:00}", paragraph.Duration.Seconds, Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFramesMaxFrameRate(paragraph.Duration.Milliseconds));
                    item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToHHMMSSFF();
                }
                else
                {
                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:000}", paragraph.Duration.Seconds, paragraph.Duration.Milliseconds);
                    item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToString();
                }
            }
        }

        /// <summary>
        /// The set number.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="number">
        /// The number.
        /// </param>
        public void SetNumber(int index, string number)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                item.SubItems[ColumnIndexNumber].Text = number;
            }
        }

        /// <summary>
        /// The update frames.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public void UpdateFrames(Subtitle subtitle)
        {
            if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
            {
                this.BeginUpdate();
                for (int i = 0; i < subtitle.Paragraphs.Count; i++)
                {
                    if (i >= 0 && i < this.Items.Count)
                    {
                        ListViewItem item = this.Items[i];
                        Paragraph p = subtitle.Paragraphs[i];
                        item.SubItems[ColumnIndexStart].Text = p.StartTime.ToHHMMSSFF();
                        item.SubItems[ColumnIndexEnd].Text = p.EndTime.ToHHMMSSFF();
                        item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:00}", p.Duration.Seconds, Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFramesMaxFrameRate(p.Duration.Milliseconds));
                    }
                }

                this.EndUpdate();
            }
        }

        /// <summary>
        /// The set start time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        public void SetStartTime(int index, Paragraph paragraph)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    if (paragraph.StartTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexStart].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexStart].Text = paragraph.StartTime.ToHHMMSSFF();
                    }

                    if (paragraph.EndTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexEnd].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToHHMMSSFF();
                    }

                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:00}", paragraph.Duration.Seconds, Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFramesMaxFrameRate(paragraph.Duration.Milliseconds));
                    item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToHHMMSSFF();
                }
                else
                {
                    if (paragraph.StartTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexStart].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexStart].Text = paragraph.StartTime.ToString();
                    }

                    if (paragraph.EndTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexEnd].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToString();
                    }

                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:000}", paragraph.Duration.Seconds, paragraph.Duration.Milliseconds);
                    item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToString();
                }
            }
        }

        /// <summary>
        /// The set start time and duration.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        public void SetStartTimeAndDuration(int index, Paragraph paragraph)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    if (paragraph.StartTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexStart].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexStart].Text = paragraph.StartTime.ToHHMMSSFF();
                    }

                    if (paragraph.EndTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexEnd].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToHHMMSSFF();
                    }

                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:00}", paragraph.Duration.Seconds, Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFramesMaxFrameRate(paragraph.Duration.Milliseconds));
                }
                else
                {
                    if (paragraph.StartTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexStart].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexStart].Text = paragraph.StartTime.ToString();
                    }

                    if (paragraph.EndTime.IsMaxTime)
                    {
                        item.SubItems[ColumnIndexEnd].Text = "-";
                    }
                    else
                    {
                        item.SubItems[ColumnIndexEnd].Text = paragraph.EndTime.ToString();
                    }

                    item.SubItems[ColumnIndexDuration].Text = string.Format("{0},{1:000}", paragraph.Duration.Seconds, paragraph.Duration.Milliseconds);
                }
            }
        }

        /// <summary>
        /// The set background color.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        public void SetBackgroundColor(int index, Color color, int columnNumber)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                if (item.UseItemStyleForSubItems)
                {
                    item.UseItemStyleForSubItems = false;
                }

                if (columnNumber >= 0 && columnNumber < item.SubItems.Count)
                {
                    item.SubItems[columnNumber].BackColor = color;
                }
            }
        }

        /// <summary>
        /// The set background color.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public void SetBackgroundColor(int index, Color color)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                item.BackColor = color;
                this.Items[index].SubItems[ColumnIndexStart].BackColor = color;
                this.Items[index].SubItems[ColumnIndexEnd].BackColor = color;
                this.Items[index].SubItems[ColumnIndexDuration].BackColor = color;
                this.Items[index].SubItems[ColumnIndexText].BackColor = color;
            }
        }

        /// <summary>
        /// The get background color.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetBackgroundColor(int index)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                return item.BackColor;
            }

            return DefaultBackColor;
        }

        /// <summary>
        /// Removes all text and set background color
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <param name="color">
        /// </param>
        public void ColorOut(int index, Color color)
        {
            if (index >= 0 && index < this.Items.Count)
            {
                ListViewItem item = this.Items[index];
                item.Text = string.Empty;
                item.SubItems[ColumnIndexStart].Text = string.Empty;
                item.SubItems[ColumnIndexEnd].Text = string.Empty;
                item.SubItems[ColumnIndexDuration].Text = string.Empty;
                item.SubItems[ColumnIndexText].Text = string.Empty;

                this.SetBackgroundColor(index, color);
            }
        }

        /// <summary>
        /// The hide non vob sub columns.
        /// </summary>
        public void HideNonVobSubColumns()
        {
            this.Columns[ColumnIndexNumber].Width = 0;
            this.Columns[ColumnIndexEnd].Width = 0;
            this.Columns[ColumnIndexDuration].Width = 0;
            this.Columns[ColumnIndexText].Width = 0;
        }

        /// <summary>
        /// The show all columns.
        /// </summary>
        public void ShowAllColumns()
        {
            if (this._settings != null && this._settings.General.ListViewColumnsRememberSize && this._settings.General.ListViewNumberWidth > 1 && this._settings.General.ListViewStartWidth > 1 && this._settings.General.ListViewEndWidth > 1 && this._settings.General.ListViewDurationWidth > 1)
            {
                this.Columns[ColumnIndexNumber].Width = this._settings.General.ListViewNumberWidth;
                this.Columns[ColumnIndexStart].Width = this._settings.General.ListViewStartWidth;
                this.Columns[ColumnIndexEnd].Width = this._settings.General.ListViewEndWidth;
                this.Columns[ColumnIndexDuration].Width = this._settings.General.ListViewDurationWidth;
                this.Columns[ColumnIndexText].Width = this._settings.General.ListViewTextWidth;

                if (this.IsAlternateTextColumnVisible)
                {
                    this.Columns[ColumnIndexTextAlternate].Width = -2;
                }
                else
                {
                    this.Columns[ColumnIndexText].Width = -2;
                }

                return;
            }

            this.Columns[ColumnIndexNumber].Width = 45;
            this.Columns[ColumnIndexEnd].Width = 80;
            this.Columns[ColumnIndexDuration].Width = 55;
            if (this.IsAlternateTextColumnVisible)
            {
                this.Columns[ColumnIndexText].Width = 250;
                this.Columns[ColumnIndexTextAlternate].Width = -2;
            }
            else
            {
                this.Columns[ColumnIndexText].Width = -2;
            }
        }
    }
}