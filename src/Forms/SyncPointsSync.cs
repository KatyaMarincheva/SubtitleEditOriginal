// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncPointsSync.cs" company="">
//   
// </copyright>
// <summary>
//   The sync points sync.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The sync points sync.
    /// </summary>
    public sealed partial class SyncPointsSync : PositionAndSizeForm
    {
        /// <summary>
        /// The _main general go to next subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToNextSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle);

        /// <summary>
        /// The _main general go to prev subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToPrevSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle);

        /// <summary>
        /// The _audio track number.
        /// </summary>
        private int _audioTrackNumber;

        /// <summary>
        /// The _original subtitle.
        /// </summary>
        private Subtitle _originalSubtitle;

        /// <summary>
        /// The _other subtitle.
        /// </summary>
        private Subtitle _otherSubtitle;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _subtitle file name.
        /// </summary>
        private string _subtitleFileName;

        /// <summary>
        /// The _synchronization points.
        /// </summary>
        private SortedDictionary<int, TimeSpan> _synchronizationPoints = new SortedDictionary<int, TimeSpan>();

        /// <summary>
        /// The _video file name.
        /// </summary>
        private string _videoFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPointsSync"/> class.
        /// </summary>
        public SyncPointsSync()
        {
            this.InitializeComponent();

            this.buttonSetSyncPoint.Text = Configuration.Settings.Language.PointSync.SetSyncPoint;
            this.buttonRemoveSyncPoint.Text = Configuration.Settings.Language.PointSync.RemoveSyncPoint;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonApplySync.Text = Configuration.Settings.Language.PointSync.ApplySync;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.labelNoOfSyncPoints.Text = string.Format(Configuration.Settings.Language.PointSync.SyncPointsX, 0);
            this.labelSyncInfo.Text = Configuration.Settings.Language.PointSync.Info;
            this.buttonFindText.Text = Configuration.Settings.Language.VisualSync.FindText;
            this.buttonFindTextOther.Text = Configuration.Settings.Language.VisualSync.FindText;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.subtitleListView2.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.SubtitleListview1.InitializeTimestampColumnWidths(this);
            this.subtitleListView2.InitializeTimestampColumnWidths(this);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            Utilities.InitializeSubtitleFont(this.subtitleListView2);
            this.SubtitleListview1.AutoSizeAllColumns(this);
            this.subtitleListView2.AutoSizeAllColumns(this);
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the video file name.
        /// </summary>
        public string VideoFileName
        {
            get
            {
                return this._videoFileName;
            }
        }

        /// <summary>
        /// Gets the fixed subtitle.
        /// </summary>
        public Subtitle FixedSubtitle
        {
            get
            {
                return this._subtitle;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="subtitleFileName">
        /// The subtitle file name.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="audioTrackNumber">
        /// The audio track number.
        /// </param>
        public void Initialize(Subtitle subtitle, string subtitleFileName, string videoFileName, int audioTrackNumber)
        {
            this.Text = Configuration.Settings.Language.PointSync.Title;
            this.labelSubtitleFileName.Text = subtitleFileName;
            this._subtitle = new Subtitle(subtitle);
            this._originalSubtitle = subtitle;
            this._subtitleFileName = subtitleFileName;
            this._videoFileName = videoFileName;
            this._audioTrackNumber = audioTrackNumber;
            this.SubtitleListview1.Fill(subtitle);
            if (this.SubtitleListview1.Items.Count > 0)
            {
                this.SubtitleListview1.Items[0].Selected = true;
            }

            this.SubtitleListview1.Anchor = AnchorStyles.Left;
            this.buttonSetSyncPoint.Anchor = AnchorStyles.Left;
            this.buttonRemoveSyncPoint.Anchor = AnchorStyles.Left;
            this.labelNoOfSyncPoints.Anchor = AnchorStyles.Left;
            this.listBoxSyncPoints.Anchor = AnchorStyles.Left;
            this.groupBoxImportResult.Anchor = AnchorStyles.Left;
            this.labelOtherSubtitleFileName.Visible = false;
            this.subtitleListView2.Visible = false;
            this.buttonFindTextOther.Visible = false;
            this.groupBoxImportResult.Width = this.listBoxSyncPoints.Left + this.listBoxSyncPoints.Width + 20;
            this.Width = this.groupBoxImportResult.Left + this.groupBoxImportResult.Width + 15;
            this.SubtitleListview1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Right;
            this.buttonSetSyncPoint.Anchor = AnchorStyles.Right;
            this.buttonRemoveSyncPoint.Anchor = AnchorStyles.Right;
            this.labelNoOfSyncPoints.Anchor = AnchorStyles.Right;
            this.listBoxSyncPoints.Anchor = AnchorStyles.Right;
            this.groupBoxImportResult.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Right;
            this.buttonFindText.Left = this.SubtitleListview1.Left + this.SubtitleListview1.Width - this.buttonFindText.Width;
            this.Width = 800;
            this.groupBoxImportResult.Width = this.Width - this.groupBoxImportResult.Left * 3;
            this.MinimumSize = new Size(this.Width - 50, this.MinimumSize.Height);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="subtitleFileName">
        /// The subtitle file name.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="audioTrackNumber">
        /// The audio track number.
        /// </param>
        /// <param name="otherSubtitleFileName">
        /// The other subtitle file name.
        /// </param>
        /// <param name="otherSubtitle">
        /// The other subtitle.
        /// </param>
        public void Initialize(Subtitle subtitle, string subtitleFileName, string videoFileName, int audioTrackNumber, string otherSubtitleFileName, Subtitle otherSubtitle)
        {
            this.Text = Configuration.Settings.Language.PointSync.TitleViaOtherSubtitle;
            this.labelSubtitleFileName.Text = subtitleFileName;
            this._subtitle = new Subtitle(subtitle);
            this._otherSubtitle = otherSubtitle;
            this._originalSubtitle = subtitle;
            this._subtitleFileName = subtitleFileName;
            this._videoFileName = videoFileName;
            this._audioTrackNumber = audioTrackNumber;
            this.SubtitleListview1.Fill(subtitle);
            if (this.SubtitleListview1.Items.Count > 0)
            {
                this.SubtitleListview1.Items[0].Selected = true;
            }

            this.labelOtherSubtitleFileName.Text = otherSubtitleFileName;
            this.subtitleListView2.Fill(otherSubtitle);

            this.SubtitleListview1.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            this.subtitleListView2.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            this.buttonSetSyncPoint.Anchor = AnchorStyles.Left;
            this.buttonRemoveSyncPoint.Anchor = AnchorStyles.Left;
            this.labelNoOfSyncPoints.Anchor = AnchorStyles.Left;
            this.listBoxSyncPoints.Anchor = AnchorStyles.Left;
            this.labelOtherSubtitleFileName.Visible = true;
            this.subtitleListView2.Visible = true;
            this.buttonFindTextOther.Visible = true;
            this.Width = this.subtitleListView2.Width * 2 + 250;
            this.MinimumSize = new Size(this.Width - 50, this.MinimumSize.Height);
        }

        /// <summary>
        /// The refresh synchronization points ui.
        /// </summary>
        private void RefreshSynchronizationPointsUI()
        {
            this.buttonApplySync.Enabled = this._synchronizationPoints.Count > 0;
            this.labelNoOfSyncPoints.Text = string.Format(Configuration.Settings.Language.PointSync.SyncPointsX, this._synchronizationPoints.Count);

            this.listBoxSyncPoints.Items.Clear();

            for (int i = 0; i < this.SubtitleListview1.Items.Count; i++)
            {
                if (this._synchronizationPoints.ContainsKey(i))
                {
                    var p = new Paragraph();
                    p.StartTime.TotalMilliseconds = this._synchronizationPoints[i].TotalMilliseconds;
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + this._subtitle.Paragraphs[i].Duration.TotalMilliseconds;
                    this.SubtitleListview1.SetStartTime(i, p);

                    var item = new ListBoxSyncPoint { Index = i, Text = this._subtitle.Paragraphs[i].Number + " - " + p.StartTime };
                    this.listBoxSyncPoints.Items.Add(item);
                    this.SubtitleListview1.SetBackgroundColor(i, Color.Green);
                    this.SubtitleListview1.SetNumber(i, "* * * *");
                }
                else
                {
                    this.SubtitleListview1.SetBackgroundColor(i, this.SubtitleListview1.BackColor);
                    this.SubtitleListview1.SetNumber(i, (i + 1).ToString(CultureInfo.InvariantCulture));
                    this.SubtitleListview1.SetStartTime(i, this._subtitle.Paragraphs[i]);
                }
            }
        }

        /// <summary>
        /// The button set sync point_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSetSyncPoint_Click(object sender, EventArgs e)
        {
            if (this.subtitleListView2.Visible)
            {
                this.SetSyncPointViaOthersubtitle();
            }
            else
            {
                if (this.SubtitleListview1.SelectedItems.Count == 1 && this._subtitle != null)
                {
                    using (var getTime = new SetSyncPoint())
                    {
                        int index = this.SubtitleListview1.SelectedItems[0].Index;
                        getTime.Initialize(this._subtitle, this._subtitleFileName, index, this._videoFileName, this._audioTrackNumber);
                        if (getTime.ShowDialog(this) == DialogResult.OK)
                        {
                            if (this._synchronizationPoints.ContainsKey(index))
                            {
                                this._synchronizationPoints[index] = getTime.SynchronizationPoint;
                            }
                            else
                            {
                                this._synchronizationPoints.Add(index, getTime.SynchronizationPoint);
                            }

                            this.RefreshSynchronizationPointsUI();
                            this._videoFileName = getTime.VideoFileName;
                        }

                        this.Activate();
                        this._videoFileName = getTime.VideoFileName;
                    }
                }
            }
        }

        /// <summary>
        /// The set sync point via othersubtitle.
        /// </summary>
        private void SetSyncPointViaOthersubtitle()
        {
            if (this._otherSubtitle != null && this.subtitleListView2.SelectedItems.Count == 1)
            {
                int index = this.SubtitleListview1.SelectedItems[0].Index;
                int indexOther = this.subtitleListView2.SelectedItems[0].Index;

                if (this._synchronizationPoints.ContainsKey(index))
                {
                    this._synchronizationPoints[index] = TimeSpan.FromMilliseconds(this._otherSubtitle.Paragraphs[indexOther].StartTime.TotalMilliseconds);
                }
                else
                {
                    this._synchronizationPoints.Add(index, TimeSpan.FromMilliseconds(this._otherSubtitle.Paragraphs[indexOther].StartTime.TotalMilliseconds));
                }

                this.RefreshSynchronizationPointsUI();
            }
        }

        /// <summary>
        /// The button remove sync point_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemoveSyncPoint_Click(object sender, EventArgs e)
        {
            if (this.SubtitleListview1.SelectedItems.Count == 1 && this._subtitle != null)
            {
                int index = this.SubtitleListview1.SelectedItems[0].Index;
                if (this._synchronizationPoints.ContainsKey(index))
                {
                    this._synchronizationPoints.Remove(index);
                }

                this.RefreshSynchronizationPointsUI();
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
            if (this.buttonApplySync.Enabled)
            {
                this.buttonSync_Click(null, null);
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
        /// The sync points sync_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SyncPointsSync_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#sync");
                e.SuppressKeyPress = true;
            }
            else if (this._mainGeneralGoToNextSubtitle == e.KeyData || (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt))
            {
                int selectedIndex = 0;
                if (this.SubtitleListview1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.SubtitleListview1.SelectedItems[0].Index;
                    selectedIndex++;
                }

                this.SubtitleListview1.SelectIndexAndEnsureVisible(selectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (this._mainGeneralGoToPrevSubtitle == e.KeyData || (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt))
            {
                int selectedIndex = 0;
                if (this.SubtitleListview1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.SubtitleListview1.SelectedItems[0].Index;
                    selectedIndex--;
                }

                this.SubtitleListview1.SelectIndexAndEnsureVisible(selectedIndex);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The sync.
        /// </summary>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <param name="minIndex">
        /// The min index.
        /// </param>
        /// <param name="maxIndex">
        /// The max index.
        /// </param>
        /// <param name="startPos">
        /// The start pos.
        /// </param>
        /// <param name="endPos">
        /// The end pos.
        /// </param>
        private void Sync(int startIndex, int endIndex, int minIndex, int maxIndex, double startPos, double endPos)
        {
            if (endPos > startPos)
            {
                double subStart = this._originalSubtitle.Paragraphs[startIndex].StartTime.TotalMilliseconds / TimeCode.BaseUnit;
                double subEnd = this._originalSubtitle.Paragraphs[endIndex].StartTime.TotalMilliseconds / TimeCode.BaseUnit;

                double subDiff = subEnd - subStart;
                double realDiff = endPos - startPos;

                // speed factor
                double factor = realDiff / subDiff;

                // adjust to starting position
                double adjust = startPos - subStart * factor;

                for (int i = minIndex; i < this._subtitle.Paragraphs.Count; i++)
                {
                    if (i <= maxIndex)
                    {
                        Paragraph p = this._subtitle.Paragraphs[i];
                        p.StartTime.TotalMilliseconds = this._originalSubtitle.Paragraphs[i].StartTime.TotalMilliseconds;
                        p.EndTime.TotalMilliseconds = this._originalSubtitle.Paragraphs[i].EndTime.TotalMilliseconds;
                        p.Adjust(factor, adjust);
                    }
                }
            }
        }

        /// <summary>
        /// The button sync_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSync_Click(object sender, EventArgs e)
        {
            if (this._synchronizationPoints.Count == 1)
            {
                foreach (KeyValuePair<int, TimeSpan> kvp in this._synchronizationPoints)
                {
                    this.AdjustViaShowEarlierLater(kvp.Key, kvp.Value.TotalMilliseconds);
                }

                this._synchronizationPoints = new SortedDictionary<int, TimeSpan>();
                this.SubtitleListview1.Fill(this._subtitle);
                this.RefreshSynchronizationPointsUI();
                return;
            }

            int endIndex = -1;
            int minIndex = 0;
            var syncIndices = new List<int>();
            foreach (var kvp in this._synchronizationPoints)
            {
                syncIndices.Add(kvp.Key);
            }

            for (int i = 0; i < syncIndices.Count; i++)
            {
                if (i == 0)
                {
                    endIndex = syncIndices[i];
                }
                else
                {
                    var startIndex = endIndex;
                    endIndex = syncIndices[i];

                    int maxIndex;
                    if (i == syncIndices.Count - 1)
                    {
                        maxIndex = this._subtitle.Paragraphs.Count;
                    }
                    else
                    {
                        maxIndex = syncIndices[i]; // maxIndex = syncIndices[i + 1];
                    }

                    this.Sync(startIndex, endIndex, minIndex, maxIndex, this._synchronizationPoints[startIndex].TotalMilliseconds / TimeCode.BaseUnit, this._synchronizationPoints[endIndex].TotalMilliseconds / TimeCode.BaseUnit);

                    minIndex = endIndex;
                }
            }

            this.SubtitleListview1.Fill(this._subtitle);
            this.RefreshSynchronizationPointsUI();
        }

        /// <summary>
        /// The adjust via show earlier later.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="newTotalMilliseconds">
        /// The new total milliseconds.
        /// </param>
        private void AdjustViaShowEarlierLater(int index, double newTotalMilliseconds)
        {
            var oldTotalMilliseconds = this._subtitle.Paragraphs[index].StartTime.TotalMilliseconds;
            var diff = newTotalMilliseconds - oldTotalMilliseconds;
            this._subtitle.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(diff));
        }

        /// <summary>
        /// The list box sync points_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxSyncPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBoxSyncPoints.SelectedIndex >= 0)
            {
                var item = (ListBoxSyncPoint)this.listBoxSyncPoints.Items[this.listBoxSyncPoints.SelectedIndex];
                this.SubtitleListview1.SelectIndexAndEnsureVisible(item.Index);
            }
        }

        /// <summary>
        /// The subtitle listview 1_ mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListview1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.SubtitleListview1.SelectedItems.Count == 1)
            {
                int index = this.SubtitleListview1.SelectedItems[0].Index;
                if (this._synchronizationPoints.ContainsKey(index))
                {
                    this.buttonRemoveSyncPoint_Click(null, null);
                }
                else
                {
                    this.buttonSetSyncPoint_Click(null, null);
                }
            }
        }

        /// <summary>
        /// The sync points sync resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SyncPointsSyncResize(object sender, EventArgs e)
        {
            if (this.subtitleListView2.Visible)
            {
                int widthInMiddle = this.listBoxSyncPoints.Width;
                this.SubtitleListview1.Width = (this.groupBoxImportResult.Width - widthInMiddle) / 2 - 12;
                this.subtitleListView2.Width = this.SubtitleListview1.Width;
                this.subtitleListView2.Left = this.SubtitleListview1.Left + this.SubtitleListview1.Width + widthInMiddle + 10;
                this.listBoxSyncPoints.Left = this.SubtitleListview1.Left + this.SubtitleListview1.Width + 5;
                this.buttonSetSyncPoint.Left = this.listBoxSyncPoints.Left;
                this.buttonRemoveSyncPoint.Left = this.listBoxSyncPoints.Left;
                this.labelNoOfSyncPoints.Left = this.listBoxSyncPoints.Left;
                this.labelOtherSubtitleFileName.Left = this.subtitleListView2.Left;
                this.buttonFindText.Left = this.SubtitleListview1.Left + this.SubtitleListview1.Width - this.buttonFindText.Width;
            }
        }

        /// <summary>
        /// The sync points sync shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SyncPointsSyncShown(object sender, EventArgs e)
        {
            this.SyncPointsSyncResize(null, null);
        }

        /// <summary>
        /// The button find text click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFindTextClick(object sender, EventArgs e)
        {
            using (var findSubtitle = new FindSubtitleLine())
            {
                findSubtitle.Initialize(this._subtitle.Paragraphs, string.Empty);
                findSubtitle.ShowDialog();
                if (findSubtitle.SelectedIndex >= 0)
                {
                    this.SubtitleListview1.SelectIndexAndEnsureVisible(findSubtitle.SelectedIndex);
                }
            }
        }

        /// <summary>
        /// The button find text other click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFindTextOtherClick(object sender, EventArgs e)
        {
            using (var findSubtitle = new FindSubtitleLine())
            {
                findSubtitle.Initialize(this._otherSubtitle.Paragraphs, string.Empty);
                findSubtitle.ShowDialog();
                if (findSubtitle.SelectedIndex >= 0)
                {
                    this.subtitleListView2.SelectIndexAndEnsureVisible(findSubtitle.SelectedIndex);
                }
            }
        }

        /// <summary>
        /// The list box sync point.
        /// </summary>
        public class ListBoxSyncPoint
        {
            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Text;
            }
        }
    }
}