// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="">
//   
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Dictionaries;
    using Nikse.SubtitleEdit.Logic.VideoPlayers;

    /// <summary>
    /// The settings.
    /// </summary>
    public sealed partial class Settings : PositionAndSizeForm
    {
        /// <summary>
        /// The _old vlc location.
        /// </summary>
        private readonly string _oldVlcLocation;

        /// <summary>
        /// The _old vlc location relative.
        /// </summary>
        private readonly string _oldVlcLocationRelative;

        /// <summary>
        /// The _list box search string.
        /// </summary>
        private string _listBoxSearchString = string.Empty;

        /// <summary>
        /// The _list box search string last used.
        /// </summary>
        private DateTime _listBoxSearchStringLastUsed = DateTime.Now;

        /// <summary>
        /// The _ocr fix replace list.
        /// </summary>
        private OcrFixReplaceList _ocrFixReplaceList;

        /// <summary>
        /// The _ssa font color.
        /// </summary>
        private int _ssaFontColor;

        /// <summary>
        /// The _ssa font name.
        /// </summary>
        private string _ssaFontName;

        /// <summary>
        /// The _ssa font size.
        /// </summary>
        private double _ssaFontSize;

        /// <summary>
        /// The _user word list.
        /// </summary>
        private List<string> _userWordList = new List<string>();

        /// <summary>
        /// The _word list names etc.
        /// </summary>
        private List<string> _wordListNamesEtc = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();

            this.labelStatus.Text = string.Empty;

            GeneralSettings gs = Configuration.Settings.General;

            this.checkBoxToolbarNew.Checked = gs.ShowToolbarNew;
            this.checkBoxToolbarOpen.Checked = gs.ShowToolbarOpen;
            this.checkBoxToolbarSave.Checked = gs.ShowToolbarSave;
            this.checkBoxToolbarSaveAs.Checked = gs.ShowToolbarSaveAs;
            this.checkBoxToolbarFind.Checked = gs.ShowToolbarFind;
            this.checkBoxReplace.Checked = gs.ShowToolbarReplace;
            this.checkBoxTBFixCommonErrors.Checked = gs.ShowToolbarFixCommonErrors;
            this.checkBoxVisualSync.Checked = gs.ShowToolbarVisualSync;
            this.checkBoxSettings.Checked = gs.ShowToolbarSettings;
            this.checkBoxSpellCheck.Checked = gs.ShowToolbarSpellCheck;
            this.checkBoxHelp.Checked = gs.ShowToolbarHelp;

            this.comboBoxFrameRate.Items.Add(23.976.ToString());
            this.comboBoxFrameRate.Items.Add(24.0.ToString());
            this.comboBoxFrameRate.Items.Add(25.0.ToString());
            this.comboBoxFrameRate.Items.Add(29.97.ToString());

            this.checkBoxShowFrameRate.Checked = gs.ShowFrameRate;
            this.comboBoxFrameRate.Text = gs.DefaultFrameRate.ToString();

            this.comboBoxEncoding.Items.Clear();
            int encodingSelectedIndex = 0;
            this.comboBoxEncoding.Items.Add(Encoding.UTF8.EncodingName);
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.Name != Encoding.UTF8.BodyName && ei.CodePage >= 949 && !ei.DisplayName.Contains("EBCDIC") && ei.CodePage != 1047)
                {
                    this.comboBoxEncoding.Items.Add(ei.CodePage + ": " + ei.DisplayName);
                    if (ei.Name == gs.DefaultEncoding || ei.CodePage + ": " + ei.DisplayName == gs.DefaultEncoding)
                    {
                        encodingSelectedIndex = this.comboBoxEncoding.Items.Count - 1;
                    }
                }
            }

            this.comboBoxEncoding.SelectedIndex = encodingSelectedIndex;

            this.checkBoxAutoDetectAnsiEncoding.Checked = gs.AutoGuessAnsiEncoding;
            this.comboBoxSubtitleFontSize.Text = gs.SubtitleFontSize.ToString(CultureInfo.InvariantCulture);
            this.checkBoxSubtitleFontBold.Checked = gs.SubtitleFontBold;
            this.checkBoxSubtitleCenter.Checked = gs.CenterSubtitleInTextBox;
            this.panelSubtitleFontColor.BackColor = gs.SubtitleFontColor;
            this.panelSubtitleBackgroundColor.BackColor = gs.SubtitleBackgroundColor;
            this.checkBoxRememberRecentFiles.Checked = gs.ShowRecentFiles;
            this.checkBoxRememberRecentFiles_CheckedChanged(null, null);
            this.checkBoxRememberSelectedLine.Checked = gs.RememberSelectedLine;
            this.checkBoxReopenLastOpened.Checked = gs.StartLoadLastFile;
            this.checkBoxStartInSourceView.Checked = gs.StartInSourceView;
            this.checkBoxRemoveBlankLinesWhenOpening.Checked = gs.RemoveBlankLinesWhenOpening;
            this.checkBoxRememberWindowPosition.Checked = gs.StartRememberPositionAndSize;
            this.numericUpDownSubtitleLineMaximumLength.Value = gs.SubtitleLineMaximumLength;
            this.numericUpDownMaxCharsSec.Value = (decimal)gs.SubtitleMaximumCharactersPerSeconds;
            this.checkBoxAutoWrapWhileTyping.Checked = gs.AutoWrapLineWhileTyping;
            this.textBoxShowLineBreaksAs.Text = gs.ListViewLineSeparatorString;

            this.numericUpDownDurationMin.Value = gs.SubtitleMinimumDisplayMilliseconds;
            this.numericUpDownDurationMax.Value = gs.SubtitleMaximumDisplayMilliseconds;

            if (gs.MinimumMillisecondsBetweenLines >= this.numericUpDownMinGapMs.Minimum && gs.MinimumMillisecondsBetweenLines <= this.numericUpDownMinGapMs.Maximum)
            {
                this.numericUpDownMinGapMs.Value = gs.MinimumMillisecondsBetweenLines;
            }

            if (gs.VideoPlayer.Trim().Equals("VLC", StringComparison.OrdinalIgnoreCase) && LibVlcDynamic.IsInstalled)
            {
                this.radioButtonVideoPlayerVLC.Checked = true;
            }
            else if (gs.VideoPlayer.Trim().Equals("MPlayer", StringComparison.OrdinalIgnoreCase) && Utilities.IsMPlayerAvailable)
            {
                this.radioButtonVideoPlayerMPlayer.Checked = true;
            }
            else if (gs.VideoPlayer.Trim().Equals("MPC-HC", StringComparison.OrdinalIgnoreCase) && Utilities.IsMpcHcInstalled)
            {
                this.radioButtonVideoPlayerMpcHc.Checked = true;
            }
            else if (Utilities.IsQuartsDllInstalled)
            {
                this.radioButtonVideoPlayerDirectShow.Checked = true;
            }
            else if (Utilities.IsMPlayerAvailable)
            {
                this.radioButtonVideoPlayerMPlayer.Checked = true;
            }
            else if (LibVlcDynamic.IsInstalled)
            {
                this.radioButtonVideoPlayerVLC.Checked = true;
            }

            if (!LibVlcDynamic.IsInstalled)
            {
                this.radioButtonVideoPlayerVLC.Enabled = false;
            }

            if (!Utilities.IsMPlayerAvailable)
            {
                this.radioButtonVideoPlayerMPlayer.Enabled = false;
            }

            if (!Utilities.IsQuartsDllInstalled)
            {
                this.radioButtonVideoPlayerDirectShow.Enabled = false;
            }

            if (Logic.VideoPlayers.MpcHC.MpcHc.GetMpcHcFileName() == null)
            {
                this.radioButtonVideoPlayerMpcHc.Enabled = false;
            }

            this.textBoxVlcPath.Text = gs.VlcLocation;
            this.textBoxVlcPath.Left = this.labelVideoPlayerVLC.Left + this.labelVideoPlayerVLC.Width + 5;
            this.textBoxVlcPath.Width = this.buttonVlcPathBrowse.Left - this.textBoxVlcPath.Left - 5;

            this.labelVlcPath.Text = Configuration.Settings.Language.Settings.VlcBrowseToLabel;

            this.checkBoxVideoPlayerShowStopButton.Checked = gs.VideoPlayerShowStopButton;
            this.checkBoxVideoPlayerShowMuteButton.Checked = gs.VideoPlayerShowMuteButton;
            this.checkBoxVideoPlayerShowFullscreenButton.Checked = gs.VideoPlayerShowFullscreenButton;

            int videoPlayerPreviewFontSizeIndex = gs.VideoPlayerPreviewFontSize - int.Parse(this.comboBoxlVideoPlayerPreviewFontSize.Items[0].ToString());
            if (videoPlayerPreviewFontSizeIndex >= 0 && videoPlayerPreviewFontSizeIndex < this.comboBoxlVideoPlayerPreviewFontSize.Items.Count)
            {
                this.comboBoxlVideoPlayerPreviewFontSize.SelectedIndex = videoPlayerPreviewFontSizeIndex;
            }
            else
            {
                this.comboBoxlVideoPlayerPreviewFontSize.SelectedIndex = 3;
            }

            this.checkBoxVideoPlayerPreviewFontBold.Checked = gs.VideoPlayerPreviewFontBold;

            this.comboBoxCustomSearch1.Text = Configuration.Settings.VideoControls.CustomSearchText1;
            this.comboBoxCustomSearch2.Text = Configuration.Settings.VideoControls.CustomSearchText2;
            this.comboBoxCustomSearch3.Text = Configuration.Settings.VideoControls.CustomSearchText3;
            this.comboBoxCustomSearch4.Text = Configuration.Settings.VideoControls.CustomSearchText4;
            this.comboBoxCustomSearch5.Text = Configuration.Settings.VideoControls.CustomSearchText5;
            this.comboBoxCustomSearch6.Text = Configuration.Settings.VideoControls.CustomSearchText6;
            this.textBoxCustomSearchUrl1.Text = Configuration.Settings.VideoControls.CustomSearchUrl1;
            this.textBoxCustomSearchUrl2.Text = Configuration.Settings.VideoControls.CustomSearchUrl2;
            this.textBoxCustomSearchUrl3.Text = Configuration.Settings.VideoControls.CustomSearchUrl3;
            this.textBoxCustomSearchUrl4.Text = Configuration.Settings.VideoControls.CustomSearchUrl4;
            this.textBoxCustomSearchUrl5.Text = Configuration.Settings.VideoControls.CustomSearchUrl5;
            this.textBoxCustomSearchUrl6.Text = Configuration.Settings.VideoControls.CustomSearchUrl6;

            foreach (var x in FontFamily.Families)
            {
                if (x.IsStyleAvailable(FontStyle.Regular) && x.IsStyleAvailable(FontStyle.Bold))
                {
                    this.comboBoxSubtitleFont.Items.Add(x.Name);
                    if (x.Name.Equals(gs.SubtitleFontName, StringComparison.OrdinalIgnoreCase))
                    {
                        this.comboBoxSubtitleFont.SelectedIndex = this.comboBoxSubtitleFont.Items.Count - 1;
                    }
                }
            }

            WordListSettings wordListSettings = Configuration.Settings.WordLists;
            this.checkBoxNamesEtcOnline.Checked = wordListSettings.UseOnlineNamesEtc;
            this.textBoxNamesEtcOnline.Text = wordListSettings.NamesEtcUrl;

            SubtitleSettings ssa = Configuration.Settings.SubtitleSettings;
            this._ssaFontName = ssa.SsaFontName;
            this._ssaFontSize = ssa.SsaFontSize;
            this._ssaFontColor = ssa.SsaFontColorArgb;
            this.fontDialogSSAStyle.Font = new Font(ssa.SsaFontName, (float)ssa.SsaFontSize);
            this.fontDialogSSAStyle.Color = Color.FromArgb(this._ssaFontColor);
            if (ssa.SsaOutline >= this.numericUpDownSsaOutline.Minimum && ssa.SsaOutline <= this.numericUpDownSsaOutline.Maximum)
            {
                this.numericUpDownSsaOutline.Value = ssa.SsaOutline;
            }

            if (ssa.SsaShadow >= this.numericUpDownSsaShadow.Minimum && ssa.SsaShadow <= this.numericUpDownSsaShadow.Maximum)
            {
                this.numericUpDownSsaShadow.Value = ssa.SsaShadow;
            }

            this.checkBoxSsaOpaqueBox.Checked = ssa.SsaOpaqueBox;
            this.UpdateSsaExample();

            var proxy = Configuration.Settings.Proxy;
            this.textBoxProxyAddress.Text = proxy.ProxyAddress;
            this.textBoxProxyUserName.Text = proxy.UserName;
            if (proxy.Password == null)
            {
                this.textBoxProxyPassword.Text = string.Empty;
            }
            else
            {
                this.textBoxProxyPassword.Text = proxy.DecodePassword();
            }

            this.textBoxProxyDomain.Text = proxy.Domain;

            this.checkBoxSyntaxColorDurationTooSmall.Checked = Configuration.Settings.Tools.ListViewSyntaxColorDurationSmall;
            this.checkBoxSyntaxColorDurationTooLarge.Checked = Configuration.Settings.Tools.ListViewSyntaxColorDurationBig;
            this.checkBoxSyntaxColorTextTooLong.Checked = Configuration.Settings.Tools.ListViewSyntaxColorLongLines;
            this.checkBoxSyntaxColorTextMoreThanTwoLines.Checked = Configuration.Settings.Tools.ListViewSyntaxMoreThanXLines;
            this.numericUpDownSyntaxColorTextMoreThanXLines.Value = Configuration.Settings.Tools.ListViewSyntaxMoreThanXLinesX;
            this.checkBoxSyntaxOverlap.Checked = Configuration.Settings.Tools.ListViewSyntaxColorOverlap;
            this.panelListViewSyntaxColorError.BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;

            // Language
            var language = Configuration.Settings.Language.Settings;
            this.Text = language.Title;
            this.tabPageGenerel.Text = language.General;
            this.tabPageVideoPlayer.Text = language.VideoPlayer;
            this.tabPageWaveform.Text = language.WaveformAndSpectrogram;
            this.tabPageWordLists.Text = language.WordLists;
            this.tabPageTools.Text = language.Tools;
            this.tabPageSsaStyle.Text = language.SsaStyle;
            this.tabPageProxy.Text = language.Proxy;
            this.tabPageToolBar.Text = language.Toolbar;
            this.tabPageShortcuts.Text = language.Shortcuts;
            this.tabPageSyntaxColoring.Text = language.SyntaxColoring;
            this.groupBoxShowToolBarButtons.Text = language.ShowToolBarButtons;
            this.labelTBNew.Text = language.New;
            this.labelTBOpen.Text = language.Open;
            this.labelTBSave.Text = language.Save;
            this.labelTBSaveAs.Text = language.SaveAs;
            this.labelTBFind.Text = language.Find;
            this.labelTBReplace.Text = language.Replace;
            this.labelTBFixCommonErrors.Text = language.FixCommonerrors;
            this.labelTBVisualSync.Text = language.VisualSync;
            this.labelTBSpellCheck.Text = language.SpellCheck;
            this.labelTBSettings.Text = language.SettingsName;
            this.labelTBHelp.Text = language.Help;
            this.checkBoxToolbarNew.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxToolbarOpen.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxToolbarSave.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxToolbarSaveAs.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxToolbarFind.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxReplace.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxTBFixCommonErrors.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxVisualSync.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxSpellCheck.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxSettings.Text = Configuration.Settings.Language.General.Visible;
            this.checkBoxHelp.Text = Configuration.Settings.Language.General.Visible;

            this.groupBoxMiscellaneous.Text = language.General;
            this.checkBoxShowFrameRate.Text = language.ShowFrameRate;
            this.labelDefaultFrameRate.Text = language.DefaultFrameRate;
            this.labelDefaultFileEncoding.Text = language.DefaultFileEncoding;
            this.labelAutoDetectAnsiEncoding.Text = language.AutoDetectAnsiEncoding;
            this.labelSubMaxLen.Text = language.SubtitleLineMaximumLength;
            this.labelMaxCharsPerSecond.Text = language.MaximumCharactersPerSecond;
            this.checkBoxAutoWrapWhileTyping.Text = language.AutoWrapWhileTyping;

            this.labelMinDuration.Text = language.DurationMinimumMilliseconds;
            this.labelMaxDuration.Text = language.DurationMaximumMilliseconds;
            this.labelMinGapMs.Text = language.MinimumGapMilliseconds;
            if (this.labelSubMaxLen.Left + this.labelSubMaxLen.Width > this.numericUpDownSubtitleLineMaximumLength.Left)
            {
                this.numericUpDownSubtitleLineMaximumLength.Left = this.labelSubMaxLen.Left + this.labelSubMaxLen.Width + 3;
            }

            if (this.labelMaxCharsPerSecond.Left + this.labelMaxCharsPerSecond.Width > this.numericUpDownMaxCharsSec.Left)
            {
                this.numericUpDownMaxCharsSec.Left = this.labelMaxCharsPerSecond.Left + this.labelMaxCharsPerSecond.Width + 3;
            }

            if (this.labelMinDuration.Left + this.labelMinDuration.Width > this.numericUpDownDurationMin.Left)
            {
                this.numericUpDownDurationMin.Left = this.labelMinDuration.Left + this.labelMinDuration.Width + 3;
            }

            if (this.labelMaxDuration.Left + this.labelMaxDuration.Width > this.numericUpDownDurationMax.Left)
            {
                this.numericUpDownDurationMax.Left = this.labelMaxDuration.Left + this.labelMaxDuration.Width + 3;
            }

            if (this.labelMinGapMs.Left + this.labelMinGapMs.Width > this.numericUpDownMinGapMs.Left)
            {
                this.numericUpDownMinGapMs.Left = this.labelMinGapMs.Left + this.labelMinGapMs.Width + 3;
            }

            if (this.labelMergeShortLines.Left + this.labelMergeShortLines.Width > this.comboBoxMergeShortLineLength.Left)
            {
                this.comboBoxMergeShortLineLength.Left = this.labelMergeShortLines.Left + this.labelMergeShortLines.Width + 3;
            }

            this.labelSubtitleFont.Text = language.SubtitleFont;
            this.labelSubtitleFontSize.Text = language.SubtitleFontSize;
            this.checkBoxSubtitleFontBold.Text = language.SubtitleBold;
            this.checkBoxSubtitleCenter.Text = language.SubtitleCenter;
            this.checkBoxSubtitleCenter.Left = this.checkBoxSubtitleFontBold.Left + this.checkBoxSubtitleFontBold.Width + 4;
            this.labelSubtitleFontColor.Text = language.SubtitleFontColor;
            this.labelSubtitleFontBackgroundColor.Text = language.SubtitleBackgroundColor;
            this.labelSpellChecker.Text = language.SpellChecker;
            this.comboBoxSpellChecker.Left = this.labelSpellChecker.Left + this.labelSpellChecker.Width + 4;
            this.checkBoxRememberRecentFiles.Text = language.RememberRecentFiles;
            this.checkBoxReopenLastOpened.Text = language.StartWithLastFileLoaded;
            this.checkBoxRememberSelectedLine.Text = language.RememberSelectedLine;
            this.checkBoxStartInSourceView.Text = language.StartInSourceView;
            this.checkBoxRemoveBlankLinesWhenOpening.Text = language.RemoveBlankLinesWhenOpening;
            this.checkBoxRememberWindowPosition.Text = language.RememberPositionAndSize;

            this.labelShowLineBreaksAs.Text = language.ShowLineBreaksAs;
            this.textBoxShowLineBreaksAs.Left = this.labelShowLineBreaksAs.Left + this.labelShowLineBreaksAs.Width;
            this.labelListViewDoubleClickEvent.Text = language.MainListViewDoubleClickAction;
            this.labelAutoBackup.Text = language.AutoBackup;
            this.comboBoxAutoBackup.Left = this.labelAutoBackup.Left + this.labelAutoBackup.Width + 3;
            this.checkBoxCheckForUpdates.Text = language.CheckForUpdates;
            this.checkBoxAllowEditOfOriginalSubtitle.Text = language.AllowEditOfOriginalSubtitle;
            this.checkBoxPromptDeleteLines.Text = language.PromptDeleteLines;

            this.comboBoxTimeCodeMode.Items.Clear();
            this.comboBoxTimeCodeMode.Items.Add(language.TimeCodeModeHHMMSSMS);
            this.comboBoxTimeCodeMode.Items.Add(language.TimeCodeModeHHMMSSFF);
            if (gs.UseTimeFormatHHMMSSFF)
            {
                this.comboBoxTimeCodeMode.SelectedIndex = 1;
            }
            else
            {
                this.comboBoxTimeCodeMode.SelectedIndex = 0;
            }

            this.labelTimeCodeMode.Text = language.TimeCodeMode;
            this.comboBoxTimeCodeMode.Left = this.labelTimeCodeMode.Left + this.labelTimeCodeMode.Width + 4;

            this.comboBoxAutoBackup.Items[0] = Configuration.Settings.Language.General.None;
            this.comboBoxAutoBackup.Items[1] = language.AutoBackupEveryMinute;
            this.comboBoxAutoBackup.Items[2] = language.AutoBackupEveryFiveMinutes;
            this.comboBoxAutoBackup.Items[3] = language.AutoBackupEveryFifteenMinutes;

            this.groupBoxVideoEngine.Text = language.VideoEngine;
            this.radioButtonVideoPlayerDirectShow.Text = language.DirectShow;

            this.labelDirectShowDescription.Text = language.DirectShowDescription;

            this.radioButtonVideoPlayerMpcHc.Text = language.MpcHc;
            this.labelMpcHcDescription.Text = language.MpcHcDescription;

            this.radioButtonVideoPlayerMPlayer.Text = language.MPlayer;
            this.labelVideoPlayerMPlayer.Text = language.MPlayerDescription;
            this.radioButtonVideoPlayerVLC.Text = language.VlcMediaPlayer;
            this.labelVideoPlayerVLC.Text = language.VlcMediaPlayerDescription;
            gs.VlcLocation = this.textBoxVlcPath.Text;

            this.checkBoxVideoPlayerShowStopButton.Text = language.ShowStopButton;
            this.checkBoxVideoPlayerShowMuteButton.Text = language.ShowMuteButton;
            this.checkBoxVideoPlayerShowFullscreenButton.Text = language.ShowFullscreenButton;

            this.labelVideoPlayerPreviewFontSize.Text = language.PreviewFontSize;
            this.comboBoxlVideoPlayerPreviewFontSize.Left = this.labelVideoPlayerPreviewFontSize.Left + this.labelVideoPlayerPreviewFontSize.Width;
            this.checkBoxVideoPlayerPreviewFontBold.Text = language.SubtitleBold;
            this.checkBoxVideoPlayerPreviewFontBold.Left = this.comboBoxlVideoPlayerPreviewFontSize.Left;

            this.groupBoxMainWindowVideoControls.Text = language.MainWindowVideoControls;
            this.labelCustomSearch.Text = language.CustomSearchTextAndUrl;

            this.groupBoxWaveformAppearence.Text = language.WaveformAppearance;
            this.checkBoxWaveformShowGrid.Text = language.WaveformShowGridLines;
            this.checkBoxReverseMouseWheelScrollDirection.Text = language.ReverseMouseWheelScrollDirection;
            this.checkBoxAllowOverlap.Text = language.WaveformAllowOverlap;
            this.checkBoxWaveformHoverFocus.Text = language.WaveformFocusMouseEnter;
            this.checkBoxListViewMouseEnterFocus.Text = language.WaveformListViewFocusMouseEnter;
            this.labelWaveformBorderHitMs1.Text = language.WaveformBorderHitMs1;
            this.labelWaveformBorderHitMs2.Text = language.WaveformBorderHitMs2;
            this.numericUpDownWaveformBorderHitMs.Left = this.labelWaveformBorderHitMs1.Left + this.labelWaveformBorderHitMs1.Width;
            this.labelWaveformBorderHitMs2.Left = this.numericUpDownWaveformBorderHitMs.Left + this.numericUpDownWaveformBorderHitMs.Width + 2;

            this.buttonWaveformGridColor.Text = language.WaveformGridColor;
            this.buttonWaveformColor.Text = language.WaveformColor;
            this.buttonWaveformSelectedColor.Text = language.WaveformSelectedColor;
            this.buttonWaveformTextColor.Text = language.WaveformTextColor;
            this.buttonWaveformBackgroundColor.Text = language.WaveformBackgroundColor;
            this.groupBoxSpectrogram.Text = language.Spectrogram;
            this.checkBoxGenerateSpectrogram.Text = language.GenerateSpectrogram;
            this.labelSpectrogramAppearance.Text = language.SpectrogramAppearance;
            this.comboBoxSpectrogramAppearance.Items.Clear();
            this.comboBoxSpectrogramAppearance.Items.Add(language.SpectrogramOneColorGradient);
            this.comboBoxSpectrogramAppearance.Items.Add(language.SpectrogramClassic);
            this.labelWaveformTextSize.Text = language.WaveformTextFontSize;
            this.comboBoxWaveformTextSize.Left = this.labelWaveformTextSize.Left + this.labelWaveformTextSize.Width + 5;
            this.checkBoxWaveformTextBold.Text = language.SubtitleBold;
            this.checkBoxWaveformTextBold.Left = this.comboBoxWaveformTextSize.Left + this.comboBoxWaveformTextSize.Width + 5;

            this.buttonWaveformsFolderEmpty.Text = language.WaveformAndSpectrogramsFolderEmpty;
            this.InitializeWaveformsAndSpectrogramsFolderEmpty(language);

            this.checkBoxUseFFmpeg.Text = language.WaveformUseFFmpeg;
            this.labelFFmpegPath.Text = language.WaveformFFmpegPath;

            this.groupBoxSsaStyle.Text = language.SubStationAlphaStyle;
            this.buttonSSAChooseFont.Text = language.ChooseFont;
            this.buttonSSAChooseColor.Text = language.ChooseColor;

            this.labelSsaOutline.Text = language.SsaOutline;
            this.labelSsaShadow.Text = language.SsaShadow;
            this.checkBoxSsaOpaqueBox.Text = language.SsaOpaqueBox;
            this.groupBoxPreview.Text = Configuration.Settings.Language.General.Preview;

            this.numericUpDownSsaOutline.Left = this.labelSsaOutline.Left + this.labelSsaOutline.Width + 4;
            this.numericUpDownSsaShadow.Left = this.labelSsaShadow.Left + this.labelSsaShadow.Width + 4;
            if (Math.Abs(this.numericUpDownSsaOutline.Left - this.numericUpDownSsaShadow.Left) < 9)
            {
                if (this.numericUpDownSsaOutline.Left > this.numericUpDownSsaShadow.Left)
                {
                    this.numericUpDownSsaShadow.Left = this.numericUpDownSsaOutline.Left;
                }
                else
                {
                    this.numericUpDownSsaOutline.Left = this.numericUpDownSsaShadow.Left;
                }
            }

            this.groupBoxWordLists.Text = language.WordLists;
            this.labelWordListLanguage.Text = language.Language;
            this.comboBoxWordListLanguage.Left = this.labelWordListLanguage.Left + this.labelWordListLanguage.Width + 4;
            this.groupBoxNamesIgonoreLists.Text = language.NamesIgnoreLists;
            this.groupBoxUserWordList.Text = language.UserWordList;
            this.groupBoxOcrFixList.Text = language.OcrFixList;
            this.buttonRemoveNameEtc.Text = language.Remove;
            this.buttonRemoveUserWord.Text = language.Remove;
            this.buttonRemoveOcrFix.Text = language.Remove;
            this.buttonAddNamesEtc.Text = language.AddNameEtc;
            this.buttonAddUserWord.Text = language.AddWord;
            this.buttonAddOcrFix.Text = language.AddPair;
            this.groupBoxWordListLocation.Text = language.Location;
            this.checkBoxNamesEtcOnline.Text = language.UseOnlineNamesEtc;
            this.linkLabelOpenDictionaryFolder.Text = Configuration.Settings.Language.GetDictionaries.OpenDictionariesFolder;

            this.groupBoxProxySettings.Text = language.ProxyServerSettings;
            this.labelProxyAddress.Text = language.ProxyAddress;
            this.groupBoxProxyAuthentication.Text = language.ProxyAuthentication;
            this.labelProxyUserName.Text = language.ProxyUserName;
            this.labelProxyPassword.Text = language.ProxyPassword;
            this.labelProxyDomain.Text = language.ProxyDomain;

            this.groupBoxToolsVisualSync.Text = language.VisualSync;
            this.labelVerifyButton.Text = language.PlayXSecondsAndBack;
            this.labelToolsStartScene.Text = language.StartSceneIndex;
            this.labelToolsEndScene.Text = language.EndSceneIndex;
            this.comboBoxToolsStartSceneIndex.Items.Clear();
            this.comboBoxToolsStartSceneIndex.Items.Add(string.Format(language.FirstPlusX, 0));
            this.comboBoxToolsStartSceneIndex.Items.Add(string.Format(language.FirstPlusX, 1));
            this.comboBoxToolsStartSceneIndex.Items.Add(string.Format(language.FirstPlusX, 2));
            this.comboBoxToolsStartSceneIndex.Items.Add(string.Format(language.FirstPlusX, 3));
            this.comboBoxToolsEndSceneIndex.Items.Clear();
            this.comboBoxToolsEndSceneIndex.Items.Add(string.Format(language.LastMinusX, 0));
            this.comboBoxToolsEndSceneIndex.Items.Add(string.Format(language.LastMinusX, 1));
            this.comboBoxToolsEndSceneIndex.Items.Add(string.Format(language.LastMinusX, 2));
            this.comboBoxToolsEndSceneIndex.Items.Add(string.Format(language.LastMinusX, 3));
            int visAdjustTextMax = Math.Max(this.labelVerifyButton.Width, this.labelToolsStartScene.Width);
            visAdjustTextMax = Math.Max(visAdjustTextMax, this.labelToolsEndScene.Width);
            this.comboBoxToolsVerifySeconds.Left = this.groupBoxToolsVisualSync.Left + visAdjustTextMax + 12;
            this.comboBoxToolsStartSceneIndex.Left = this.comboBoxToolsVerifySeconds.Left;
            this.comboBoxToolsEndSceneIndex.Left = this.comboBoxToolsVerifySeconds.Left;

            this.groupBoxFixCommonErrors.Text = language.FixCommonerrors;
            this.labelMergeShortLines.Text = language.MergeLinesShorterThan;
            this.labelToolsMusicSymbol.Text = language.MusicSymbol;
            this.labelToolsMusicSymbolsToReplace.Text = language.MusicSymbolsToReplace;
            this.checkBoxFixCommonOcrErrorsUsingHardcodedRules.Text = language.FixCommonOcrErrorsUseHardcodedRules;
            this.checkBoxFixShortDisplayTimesAllowMoveStartTime.Text = language.FixCommonerrorsFixShortDisplayTimesAllowMoveStartTime;
            this.groupBoxSpellCheck.Text = language.SpellCheck;
            this.checkBoxSpellCheckAutoChangeNames.Text = Configuration.Settings.Language.SpellCheck.AutoFixNames;
            this.checkBoxSpellCheckOneLetterWords.Text = Configuration.Settings.Language.SpellCheck.CheckOneLetterWords;
            this.checkBoxTreatINQuoteAsING.Text = Configuration.Settings.Language.SpellCheck.TreatINQuoteAsING;

            this.groupBoxToolsMisc.Text = language.Miscellaneous;
            this.checkBoxUseDoNotBreakAfterList.Text = language.UseDoNotBreakAfterList;
            this.buttonEditDoNotBreakAfterList.Text = Configuration.Settings.Language.VobSubOcr.Edit;

            this.comboBoxListViewDoubleClickEvent.Items.Clear();
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewNothing);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewVideoGoToPositionAndPause);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewVideoGoToPositionAndPlay);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewVideoGoToPositionMinusHalfSecAndPause);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewVideoGoToPositionMinus1SecAndPause);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewVideoGoToPositionMinus1SecAndPlay);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewEditTextAndPause);
            this.comboBoxListViewDoubleClickEvent.Items.Add(language.MainListViewEditText);

            if (gs.ListViewDoubleClickAction >= 0 && gs.ListViewDoubleClickAction < this.comboBoxListViewDoubleClickEvent.Items.Count)
            {
                this.comboBoxListViewDoubleClickEvent.SelectedIndex = gs.ListViewDoubleClickAction;
            }

            if (gs.AutoBackupSeconds == 60)
            {
                this.comboBoxAutoBackup.SelectedIndex = 1;
            }
            else if (gs.AutoBackupSeconds == 60 * 5)
            {
                this.comboBoxAutoBackup.SelectedIndex = 2;
            }
            else if (gs.AutoBackupSeconds == 60 * 15)
            {
                this.comboBoxAutoBackup.SelectedIndex = 3;
            }
            else
            {
                this.comboBoxAutoBackup.SelectedIndex = 0;
            }

            this.checkBoxCheckForUpdates.Checked = gs.CheckForUpdates;

            if (gs.SpellChecker.Contains("word", StringComparison.OrdinalIgnoreCase))
            {
                this.comboBoxSpellChecker.SelectedIndex = 1;
            }
            else
            {
                this.comboBoxSpellChecker.SelectedIndex = 0;
            }

            if (Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac())
            {
                this.comboBoxSpellChecker.SelectedIndex = 0;
                this.comboBoxSpellChecker.Enabled = false;
            }

            this.checkBoxAllowEditOfOriginalSubtitle.Checked = gs.AllowEditOfOriginalSubtitle;
            this.checkBoxPromptDeleteLines.Checked = gs.PromptDeleteLines;

            ToolsSettings toolsSettings = Configuration.Settings.Tools;
            if (toolsSettings.VerifyPlaySeconds - 2 >= 0 && toolsSettings.VerifyPlaySeconds - 2 < this.comboBoxToolsVerifySeconds.Items.Count)
            {
                this.comboBoxToolsVerifySeconds.SelectedIndex = toolsSettings.VerifyPlaySeconds - 2;
            }
            else
            {
                this.comboBoxToolsVerifySeconds.SelectedIndex = 0;
            }

            if (toolsSettings.StartSceneIndex >= 0 && toolsSettings.StartSceneIndex < this.comboBoxToolsStartSceneIndex.Items.Count)
            {
                this.comboBoxToolsStartSceneIndex.SelectedIndex = toolsSettings.StartSceneIndex;
            }
            else
            {
                this.comboBoxToolsStartSceneIndex.SelectedIndex = 0;
            }

            if (toolsSettings.EndSceneIndex >= 0 && toolsSettings.EndSceneIndex < this.comboBoxToolsEndSceneIndex.Items.Count)
            {
                this.comboBoxToolsEndSceneIndex.SelectedIndex = toolsSettings.EndSceneIndex;
            }
            else
            {
                this.comboBoxToolsEndSceneIndex.SelectedIndex = 0;
            }

            this.comboBoxMergeShortLineLength.Items.Clear();
            for (int i = 10; i < 100; i++)
            {
                this.comboBoxMergeShortLineLength.Items.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            if (toolsSettings.MergeLinesShorterThan >= 10 && toolsSettings.MergeLinesShorterThan - 10 < this.comboBoxMergeShortLineLength.Items.Count)
            {
                this.comboBoxMergeShortLineLength.SelectedIndex = toolsSettings.MergeLinesShorterThan - 10;
            }
            else
            {
                this.comboBoxMergeShortLineLength.SelectedIndex = 0;
            }

            // Music notes / music symbols
            if (!Utilities.IsRunningOnMono() && Environment.OSVersion.Version.Major < 6)
            {
                // 6 == Vista/Win2008Server/Win7
                float fontSize = this.comboBoxToolsMusicSymbol.Font.Size;
                const string unicodeFontName = Utilities.WinXP2KUnicodeFontName;
                this.listBoxNamesEtc.Font = new Font(unicodeFontName, fontSize);
                this.listBoxUserWordLists.Font = new Font(unicodeFontName, fontSize);
                this.listBoxOcrFixList.Font = new Font(unicodeFontName, fontSize);
                this.comboBoxToolsMusicSymbol.Font = new Font(unicodeFontName, fontSize);
                this.textBoxMusicSymbolsToReplace.Font = new Font(unicodeFontName, fontSize);
                this.textBoxNameEtc.Font = new Font(unicodeFontName, fontSize);
                this.textBoxUserWord.Font = new Font(unicodeFontName, fontSize);
                this.textBoxOcrFixKey.Font = new Font(unicodeFontName, fontSize);
                this.textBoxOcrFixValue.Font = new Font(unicodeFontName, fontSize);
            }

            this.comboBoxToolsMusicSymbol.Items.Clear();
            this.comboBoxToolsMusicSymbol.Items.Add("♪");
            this.comboBoxToolsMusicSymbol.Items.Add("♫");
            this.comboBoxToolsMusicSymbol.Items.Add("♪♪");
            this.comboBoxToolsMusicSymbol.Items.Add("*");
            this.comboBoxToolsMusicSymbol.Items.Add("#");
            if (toolsSettings.MusicSymbol == "♪")
            {
                this.comboBoxToolsMusicSymbol.SelectedIndex = 0;
            }
            else if (toolsSettings.MusicSymbol == "♫")
            {
                this.comboBoxToolsMusicSymbol.SelectedIndex = 1;
            }
            else if (toolsSettings.MusicSymbol == "♪♪")
            {
                this.comboBoxToolsMusicSymbol.SelectedIndex = 2;
            }
            else if (toolsSettings.MusicSymbol == "*")
            {
                this.comboBoxToolsMusicSymbol.SelectedIndex = 3;
            }
            else if (toolsSettings.MusicSymbol == "#")
            {
                this.comboBoxToolsMusicSymbol.SelectedIndex = 4;
            }
            else
            {
                this.comboBoxToolsMusicSymbol.Items.Add(toolsSettings.MusicSymbol);
                this.comboBoxToolsMusicSymbol.SelectedIndex = 5;
            }

            this.textBoxMusicSymbolsToReplace.Text = toolsSettings.MusicSymbolToReplace;
            this.checkBoxFixCommonOcrErrorsUsingHardcodedRules.Checked = toolsSettings.OcrFixUseHardcodedRules;
            this.checkBoxFixShortDisplayTimesAllowMoveStartTime.Checked = toolsSettings.FixShortDisplayTimesAllowMoveStartTime;
            this.checkBoxSpellCheckAutoChangeNames.Checked = toolsSettings.SpellCheckAutoChangeNames;
            this.checkBoxSpellCheckOneLetterWords.Checked = toolsSettings.SpellCheckOneLetterWords;
            this.checkBoxTreatINQuoteAsING.Checked = toolsSettings.SpellCheckEnglishAllowInQuoteAsIng;
            this.checkBoxUseDoNotBreakAfterList.Checked = toolsSettings.UseNoLineBreakAfter;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            this.ListWordListLanguages();

            this.checkBoxWaveformShowGrid.Checked = Configuration.Settings.VideoControls.WaveformDrawGrid;
            this.panelWaveformGridColor.BackColor = Configuration.Settings.VideoControls.WaveformGridColor;
            this.panelWaveformSelectedColor.BackColor = Configuration.Settings.VideoControls.WaveformSelectedColor;
            this.panelWaveformColor.BackColor = Configuration.Settings.VideoControls.WaveformColor;
            this.panelWaveformBackgroundColor.BackColor = Configuration.Settings.VideoControls.WaveformBackgroundColor;
            this.panelWaveformTextColor.BackColor = Configuration.Settings.VideoControls.WaveformTextColor;
            this.checkBoxGenerateSpectrogram.Checked = Configuration.Settings.VideoControls.GenerateSpectrogram;
            if (Configuration.Settings.VideoControls.SpectrogramAppearance == "OneColorGradient")
            {
                this.comboBoxSpectrogramAppearance.SelectedIndex = 0;
            }
            else
            {
                this.comboBoxSpectrogramAppearance.SelectedIndex = 1;
            }

            this.comboBoxWaveformTextSize.Text = Configuration.Settings.VideoControls.WaveformTextSize.ToString(CultureInfo.InvariantCulture);
            this.checkBoxWaveformTextBold.Checked = Configuration.Settings.VideoControls.WaveformTextBold;
            this.checkBoxReverseMouseWheelScrollDirection.Checked = Configuration.Settings.VideoControls.WaveformMouseWheelScrollUpIsForward;
            this.checkBoxAllowOverlap.Checked = Configuration.Settings.VideoControls.WaveformAllowOverlap;
            this.checkBoxWaveformHoverFocus.Checked = Configuration.Settings.VideoControls.WaveformFocusOnMouseEnter;
            this.checkBoxListViewMouseEnterFocus.Checked = Configuration.Settings.VideoControls.WaveformListViewFocusOnMouseEnter;
            this.checkBoxListViewMouseEnterFocus.Enabled = Configuration.Settings.VideoControls.WaveformFocusOnMouseEnter;
            if (Configuration.Settings.VideoControls.WaveformBorderHitMs >= this.numericUpDownWaveformBorderHitMs.Minimum && Configuration.Settings.VideoControls.WaveformBorderHitMs <= this.numericUpDownWaveformBorderHitMs.Maximum)
            {
                this.numericUpDownWaveformBorderHitMs.Value = Configuration.Settings.VideoControls.WaveformBorderHitMs;
            }

            this.checkBoxUseFFmpeg.Checked = gs.UseFFmpegForWaveExtraction;
            this.textBoxFFmpegPath.Text = gs.FFmpegLocation;
            var generalNode = new TreeNode(Configuration.Settings.Language.General.GeneralText);
            generalNode.Nodes.Add(language.MergeSelectedLines + GetShortcutText(Configuration.Settings.Shortcuts.GeneralMergeSelectedLines));
            generalNode.Nodes.Add(language.MergeSelectedLinesOnlyFirstText + GetShortcutText(Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesOnlyFirstText));
            generalNode.Nodes.Add(language.MergeOriginalAndTranslation + GetShortcutText(Configuration.Settings.Shortcuts.GeneralMergeOriginalAndTranslation));
            generalNode.Nodes.Add(language.ToggleTranslationMode + GetShortcutText(Configuration.Settings.Shortcuts.GeneralToggleTranslationMode));
            generalNode.Nodes.Add(language.SwitchOriginalAndTranslation + GetShortcutText(Configuration.Settings.Shortcuts.GeneralSwitchOriginalAndTranslation));
            generalNode.Nodes.Add(language.WaveformPlayFirstSelectedSubtitle + GetShortcutText(Configuration.Settings.Shortcuts.GeneralPlayFirstSelected));
            generalNode.Nodes.Add(language.GoToFirstSelectedLine + GetShortcutText(Configuration.Settings.Shortcuts.GeneralGoToFirstSelectedLine));
            generalNode.Nodes.Add(language.GoToNextEmptyLine + GetShortcutText(Configuration.Settings.Shortcuts.GeneralGoToNextEmptyLine));
            generalNode.Nodes.Add(language.GoToNext + GetShortcutText(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle));
            generalNode.Nodes.Add(language.GoToPrevious + GetShortcutText(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle));
            generalNode.Nodes.Add(language.GoToCurrentSubtitleStart + GetShortcutText(Configuration.Settings.Shortcuts.GeneralGoToStartOfCurrentSubtitle));
            generalNode.Nodes.Add(language.GoToCurrentSubtitleEnd + GetShortcutText(Configuration.Settings.Shortcuts.GeneralGoToEndOfCurrentSubtitle));
            this.treeViewShortcuts.Nodes.Add(generalNode);

            var fileNode = new TreeNode(Configuration.Settings.Language.Main.Menu.File.Title);
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.New + GetShortcutText(Configuration.Settings.Shortcuts.MainFileNew));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.Open + GetShortcutText(Configuration.Settings.Shortcuts.MainFileOpen));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.OpenKeepVideo + GetShortcutText(Configuration.Settings.Shortcuts.MainFileOpenKeepVideo));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.Save + GetShortcutText(Configuration.Settings.Shortcuts.MainFileSave));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.SaveAs + GetShortcutText(Configuration.Settings.Shortcuts.MainFileSaveAs));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.SaveOriginal + GetShortcutText(Configuration.Settings.Shortcuts.MainFileSaveOriginal));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.SaveOriginalSubtitleAs + GetShortcutText(Configuration.Settings.Shortcuts.MainFileSaveOriginalAs));
            fileNode.Nodes.Add(language.MainFileSaveAll + GetShortcutText(Configuration.Settings.Shortcuts.MainFileSaveAll));
            fileNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.File.Export + " -> " + Configuration.Settings.Language.Main.Menu.File.ExportEbu + GetShortcutText(Configuration.Settings.Shortcuts.MainFileExportEbu));
            this.treeViewShortcuts.Nodes.Add(fileNode);

            var editNode = new TreeNode(Configuration.Settings.Language.Main.Menu.Edit.Title);
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.Undo + GetShortcutText(Configuration.Settings.Shortcuts.MainEditUndo));
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.Redo + GetShortcutText(Configuration.Settings.Shortcuts.MainEditRedo));
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.Find + GetShortcutText(Configuration.Settings.Shortcuts.MainEditFind));
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.FindNext + GetShortcutText(Configuration.Settings.Shortcuts.MainEditFindNext));
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.Replace + GetShortcutText(Configuration.Settings.Shortcuts.MainEditReplace));
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.MultipleReplace + GetShortcutText(Configuration.Settings.Shortcuts.MainEditMultipleReplace));
            editNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Edit.GoToSubtitleNumber + GetShortcutText(Configuration.Settings.Shortcuts.MainEditGoToLineNumber));
            editNode.Nodes.Add(Configuration.Settings.Language.VobSubOcr.RightToLeft + GetShortcutText(Configuration.Settings.Shortcuts.MainEditRightToLeft));
            editNode.Nodes.Add(language.ReverseStartAndEndingForRTL + GetShortcutText(Configuration.Settings.Shortcuts.MainEditReverseStartAndEndingForRTL));
            editNode.Nodes.Add(language.ToggleTranslationAndOriginalInPreviews + GetShortcutText(Configuration.Settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews));
            this.treeViewShortcuts.Nodes.Add(editNode);

            var toolsNode = new TreeNode(Configuration.Settings.Language.Main.Menu.Tools.Title);
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.FixCommonErrors + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsFixCommonErrors));
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.StartNumberingFrom + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsRenumber));
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.RemoveTextForHearingImpaired + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsRemoveTextForHI));
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.ChangeCasing + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsChangeCasing));
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.SplitLongLines + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsSplitLongLines));
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.MergeShortLines + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsMergeShortLines));
            toolsNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.ContextMenu.AutoDurationCurrentLine + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsAutoDuration));
            toolsNode.Nodes.Add(language.ShowBeamer + GetShortcutText(Configuration.Settings.Shortcuts.MainToolsBeamer));
            this.treeViewShortcuts.Nodes.Add(toolsNode);

            var videoNode = new TreeNode(Configuration.Settings.Language.Main.Menu.Video.Title);
            videoNode.Nodes.Add(language.TogglePlayPause + GetShortcutText(Configuration.Settings.Shortcuts.MainVideoPlayPauseToggle));
            videoNode.Nodes.Add(language.Pause + GetShortcutText(Configuration.Settings.Shortcuts.MainVideoPause));
            videoNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Video.ShowHideVideo + GetShortcutText(Configuration.Settings.Shortcuts.MainVideoShowHideVideo));
            videoNode.Nodes.Add(language.ToggleDockUndockOfVideoControls + GetShortcutText(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls));
            videoNode.Nodes.Add(language.GoBack1Frame + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo1FrameLeft));
            videoNode.Nodes.Add(language.GoForward1Frame + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo1FrameRight));
            videoNode.Nodes.Add(language.GoBack100Milliseconds + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo100MsLeft));
            videoNode.Nodes.Add(language.GoForward100Milliseconds + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo100MsRight));
            videoNode.Nodes.Add(language.GoBack500Milliseconds + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo500MsLeft));
            videoNode.Nodes.Add(language.GoForward500Milliseconds + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo500MsRight));
            videoNode.Nodes.Add(language.GoBack1Second + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo1000MsLeft));
            videoNode.Nodes.Add(language.GoForward1Second + GetShortcutText(Configuration.Settings.Shortcuts.MainVideo1000MsRight));
            videoNode.Nodes.Add(language.Fullscreen + GetShortcutText(Configuration.Settings.Shortcuts.MainVideoFullscreen));
            this.treeViewShortcuts.Nodes.Add(videoNode);

            var spellCheckNode = new TreeNode(Configuration.Settings.Language.Main.Menu.SpellCheck.Title);
            spellCheckNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.SpellCheck.Title + GetShortcutText(Configuration.Settings.Shortcuts.MainSpellCheck));
            spellCheckNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.SpellCheck.FindDoubleWords + GetShortcutText(Configuration.Settings.Shortcuts.MainSpellCheckFindDoubleWords));
            spellCheckNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.SpellCheck.AddToNamesEtcList + GetShortcutText(Configuration.Settings.Shortcuts.MainSpellCheckAddWordToNames));
            this.treeViewShortcuts.Nodes.Add(spellCheckNode);

            var syncNode = new TreeNode(Configuration.Settings.Language.Main.Menu.Synchronization.Title);
            syncNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Synchronization.AdjustAllTimes + GetShortcutText(Configuration.Settings.Shortcuts.MainSynchronizationAdjustTimes));
            syncNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Synchronization.VisualSync + GetShortcutText(Configuration.Settings.Shortcuts.MainSynchronizationVisualSync));
            syncNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Synchronization.PointSync + GetShortcutText(Configuration.Settings.Shortcuts.MainSynchronizationPointSync));
            syncNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.Tools.ChangeFrameRate + GetShortcutText(Configuration.Settings.Shortcuts.MainSynchronizationChangeFrameRate));
            this.treeViewShortcuts.Nodes.Add(syncNode);

            var listViewNode = new TreeNode(Configuration.Settings.Language.Main.Controls.ListView);
            listViewNode.Nodes.Add(Configuration.Settings.Language.General.Italic + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewItalic));
            listViewNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.ContextMenu.InsertAfter + GetShortcutText(Configuration.Settings.Shortcuts.MainInsertAfter));
            listViewNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.ContextMenu.InsertBefore + GetShortcutText(Configuration.Settings.Shortcuts.MainInsertBefore));
            listViewNode.Nodes.Add(language.MergeDialog + GetShortcutText(Configuration.Settings.Shortcuts.MainMergeDialog));
            listViewNode.Nodes.Add(language.ToggleFocus + GetShortcutText(Configuration.Settings.Shortcuts.MainToggleFocus));
            listViewNode.Nodes.Add(language.ToggleDialogDashes + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewToggleDashes));
            listViewNode.Nodes.Add(language.Alignment + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewAlignment));
            listViewNode.Nodes.Add(language.CopyTextOnly + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewCopyText));
            listViewNode.Nodes.Add(language.CopyTextOnlyFromOriginalToCurrent + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewCopyTextFromOriginalToCurrent));
            listViewNode.Nodes.Add(language.AutoDurationSelectedLines + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewAutoDuration));
            listViewNode.Nodes.Add(language.ListViewColumnDelete + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewColumnDeleteText));
            listViewNode.Nodes.Add(language.ListViewColumnInsert + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewColumnInsertText));
            listViewNode.Nodes.Add(language.ListViewColumnPaste + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewColumnPaste));
            listViewNode.Nodes.Add(language.ListViewFocusWaveform + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewFocusWaveform));
            listViewNode.Nodes.Add(language.ListViewGoToNextError + GetShortcutText(Configuration.Settings.Shortcuts.MainListViewGoToNextError));
            this.treeViewShortcuts.Nodes.Add(listViewNode);

            var textBoxNode = new TreeNode(language.TextBox);
            textBoxNode.Nodes.Add(Configuration.Settings.Language.General.Italic + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxItalic));
            textBoxNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.ContextMenu.SplitLineAtCursorPosition + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxSplitAtCursor));
            textBoxNode.Nodes.Add(language.MainTextBoxMoveLastWordDown + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxMoveLastWordDown));
            textBoxNode.Nodes.Add(language.MainTextBoxMoveFirstWordFromNextUp + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxMoveFirstWordFromNextUp));
            textBoxNode.Nodes.Add(language.MainTextBoxSelectionToLower + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxSelectionToLower));
            textBoxNode.Nodes.Add(language.MainTextBoxSelectionToUpper + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxSelectionToUpper));
            textBoxNode.Nodes.Add(language.MainTextBoxToggleAutoDuration + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxToggleAutoDuration));
            textBoxNode.Nodes.Add(Configuration.Settings.Language.Main.Menu.ContextMenu.InsertAfter + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxInsertAfter));
            textBoxNode.Nodes.Add(language.MainTextBoxAutoBreak + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxAutoBreak));
            textBoxNode.Nodes.Add(language.MainTextBoxUnbreak + GetShortcutText(Configuration.Settings.Shortcuts.MainTextBoxUnbreak));
            this.treeViewShortcuts.Nodes.Add(textBoxNode);

            var createNode = new TreeNode(Configuration.Settings.Language.Main.VideoControls.Create);
            createNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.InsertNewSubtitleAtVideoPosition + GetShortcutText(Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPos));
            createNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.PlayFromJustBeforeText + GetShortcutText(Configuration.Settings.Shortcuts.MainCreatePlayFromJustBefore));
            createNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.SetStartTime + GetShortcutText(Configuration.Settings.Shortcuts.MainCreateSetStart));
            createNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.SetEndTime + GetShortcutText(Configuration.Settings.Shortcuts.MainCreateSetEnd));
            createNode.Nodes.Add(language.MainCreateStartDownEndUp + GetShortcutText(Configuration.Settings.Shortcuts.MainCreateStartDownEndUp));
            createNode.Nodes.Add(language.CreateSetEndAddNewAndGoToNew + GetShortcutText(Configuration.Settings.Shortcuts.MainCreateSetEndAddNewAndGoToNew));
            this.treeViewShortcuts.Nodes.Add(createNode);

            var translateNote = new TreeNode(Configuration.Settings.Language.Main.VideoControls.Translate);
            translateNote.Nodes.Add(language.CustomSearch1 + GetShortcutText(Configuration.Settings.Shortcuts.MainTranslateCustomSearch1));
            translateNote.Nodes.Add(language.CustomSearch2 + GetShortcutText(Configuration.Settings.Shortcuts.MainTranslateCustomSearch2));
            translateNote.Nodes.Add(language.CustomSearch3 + GetShortcutText(Configuration.Settings.Shortcuts.MainTranslateCustomSearch3));
            translateNote.Nodes.Add(language.CustomSearch4 + GetShortcutText(Configuration.Settings.Shortcuts.MainTranslateCustomSearch4));
            translateNote.Nodes.Add(language.CustomSearch5 + GetShortcutText(Configuration.Settings.Shortcuts.MainTranslateCustomSearch5));
            translateNote.Nodes.Add(language.CustomSearch6 + GetShortcutText(Configuration.Settings.Shortcuts.MainTranslateCustomSearch6));
            this.treeViewShortcuts.Nodes.Add(translateNote);

            var adjustNode = new TreeNode(Configuration.Settings.Language.Main.VideoControls.Adjust);
            adjustNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.SetstartTimeAndOffsetOfRest + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest));
            adjustNode.Nodes.Add(language.AdjustSetEndTimeAndGoToNext + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetEndAndGotoNext));
            adjustNode.Nodes.Add(language.AdjustViaEndAutoStartAndGoToNext + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustViaEndAutoStartAndGoToNext));
            adjustNode.Nodes.Add(language.AdjustSetStartAutoDurationAndGoToNext + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetStartAutoDurationAndGoToNext));
            adjustNode.Nodes.Add(language.AdjustSetEndNextStartAndGoToNext + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetEndNextStartAndGoToNext));
            adjustNode.Nodes.Add(language.AdjustStartDownEndUpAndGoToNext + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustStartDownEndUpAndGoToNext));
            adjustNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.SetStartTime + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetStart));
            adjustNode.Nodes.Add(language.AdjustSetStartTimeKeepDuration + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetStartKeepDuration));
            adjustNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.SetEndTime + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetEnd));
            adjustNode.Nodes.Add(language.AdjustSelected100MsForward + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSelected100MsForward));
            adjustNode.Nodes.Add(language.AdjustSelected100MsBack + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSelected100MsBack));
            adjustNode.Nodes.Add(language.AdjustSetEndAndOffsetTheRest + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetEndAndOffsetTheRest));
            adjustNode.Nodes.Add(language.AdjustSetEndAndOffsetTheRestAndGoToNext + GetShortcutText(Configuration.Settings.Shortcuts.MainAdjustSetEndAndOffsetTheRestAndGoToNext));
            this.treeViewShortcuts.Nodes.Add(adjustNode);

            var audioVisualizerNode = new TreeNode(language.WaveformAndSpectrogram);
            audioVisualizerNode.Nodes.Add(Configuration.Settings.Language.Waveform.ZoomIn + GetShortcutText(Configuration.Settings.Shortcuts.WaveformZoomIn));
            audioVisualizerNode.Nodes.Add(Configuration.Settings.Language.Waveform.ZoomOut + GetShortcutText(Configuration.Settings.Shortcuts.WaveformZoomOut));
            audioVisualizerNode.Nodes.Add(language.VerticalZoom + GetShortcutText(Configuration.Settings.Shortcuts.WaveformVerticalZoom));
            audioVisualizerNode.Nodes.Add(language.VerticalZoomOut + GetShortcutText(Configuration.Settings.Shortcuts.WaveformVerticalZoomOut));
            audioVisualizerNode.Nodes.Add(language.WaveformSeekSilenceForward + GetShortcutText(Configuration.Settings.Shortcuts.WaveformSearchSilenceForward));
            audioVisualizerNode.Nodes.Add(language.WaveformSeekSilenceBack + GetShortcutText(Configuration.Settings.Shortcuts.WaveformSearchSilenceBack));
            audioVisualizerNode.Nodes.Add(language.WaveformAddTextHere + GetShortcutText(Configuration.Settings.Shortcuts.WaveformAddTextHere));
            audioVisualizerNode.Nodes.Add(language.WaveformPlayNewSelection + GetShortcutText(Configuration.Settings.Shortcuts.WaveformPlaySelection));
            audioVisualizerNode.Nodes.Add(Configuration.Settings.Language.Main.VideoControls.InsertNewSubtitleAtVideoPosition + GetShortcutText(Configuration.Settings.Shortcuts.MainWaveformInsertAtCurrentPosition));
            audioVisualizerNode.Nodes.Add(language.WaveformFocusListView + GetShortcutText(Configuration.Settings.Shortcuts.WaveformFocusListView));
            this.treeViewShortcuts.Nodes.Add(audioVisualizerNode);

            foreach (TreeNode node in this.treeViewShortcuts.Nodes)
            {
                node.Text = node.Text.Replace("&", string.Empty);
                foreach (TreeNode subNode in node.Nodes)
                {
                    subNode.Text = subNode.Text.Replace("&", string.Empty);
                    foreach (TreeNode subSubNode in subNode.Nodes)
                    {
                        subSubNode.Text = subSubNode.Text.Replace("&", string.Empty);
                    }
                }
            }

            this.treeViewShortcuts.ExpandAll();

            this.groupBoxShortcuts.Text = language.Shortcuts;
            this.labelShortcut.Text = language.Shortcut;
            this.checkBoxShortcutsControl.Text = language.Control;
            this.checkBoxShortcutsAlt.Text = language.Alt;
            this.checkBoxShortcutsShift.Text = language.Shift;
            this.buttonUpdateShortcut.Text = language.UpdateShortcut;
            this.labelShortcutKey.Text = language.Key;
            this.comboBoxShortcutKey.Left = this.labelShortcutKey.Left + this.labelShortcutKey.Width;
            this.comboBoxShortcutKey.Items[0] = Configuration.Settings.Language.General.None;

            this.groupBoxListViewSyntaxColoring.Text = language.ListViewSyntaxColoring;
            this.checkBoxSyntaxColorDurationTooSmall.Text = language.SyntaxColorDurationIfTooSmall;
            this.checkBoxSyntaxColorDurationTooLarge.Text = language.SyntaxColorDurationIfTooLarge;
            this.checkBoxSyntaxColorTextTooLong.Text = language.SyntaxColorTextIfTooLong;
            this.checkBoxSyntaxColorTextMoreThanTwoLines.Text = language.SyntaxColorTextMoreThanXLines;
            this.numericUpDownSyntaxColorTextMoreThanXLines.Left = this.checkBoxSyntaxColorTextMoreThanTwoLines.Left + this.checkBoxSyntaxColorTextMoreThanTwoLines.Width + 4;
            this.checkBoxSyntaxOverlap.Text = language.SyntaxColorOverlap;
            this.buttonListViewSyntaxColorError.Text = language.SyntaxErrorColor;

            Utilities.FixLargeFonts(this, this.buttonOK);

            this.checkBoxShortcutsControl.Left = this.labelShortcut.Left + this.labelShortcut.Width + 9;
            this.checkBoxShortcutsAlt.Left = this.checkBoxShortcutsControl.Left + this.checkBoxShortcutsControl.Width + 9;
            this.checkBoxShortcutsShift.Left = this.checkBoxShortcutsAlt.Left + this.checkBoxShortcutsAlt.Width + 9;
            this.labelShortcutKey.Left = this.checkBoxShortcutsShift.Left + this.checkBoxShortcutsShift.Width + 9;
            this.comboBoxShortcutKey.Left = this.labelShortcutKey.Left + this.labelShortcutKey.Width + 2;
            this.buttonUpdateShortcut.Left = this.comboBoxShortcutKey.Left + this.comboBoxShortcutKey.Width + 15;

            this._oldVlcLocation = gs.VlcLocation;
            this._oldVlcLocationRelative = gs.VlcLocationRelative;

            this.labelPlatform.Text = (IntPtr.Size * 8) + "-bit";
        }

        /// <summary>
        /// The get relative path.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetRelativePath(string fileName)
        {
            string folder = Configuration.BaseDirectory;

            if (string.IsNullOrEmpty(fileName) || !fileName.StartsWith(folder.Substring(0, 2), StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            Uri pathUri = new Uri(fileName);
            if (!folder.EndsWith(Path.DirectorySeparatorChar))
            {
                folder += Path.DirectorySeparatorChar;
            }

            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// The get shortcut text.
        /// </summary>
        /// <param name="shortcut">
        /// The shortcut.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetShortcutText(string shortcut)
        {
            if (string.IsNullOrEmpty(shortcut))
            {
                shortcut = Configuration.Settings.Language.General.None;
            }

            return string.Format(" [{0}]", shortcut);
        }

        /// <summary>
        /// The initialize waveforms and spectrograms folder empty.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        private void InitializeWaveformsAndSpectrogramsFolderEmpty(LanguageStructure.Settings language)
        {
            string waveformsFolder = Configuration.WaveformsFolder.TrimEnd(Path.DirectorySeparatorChar);
            string spectrogramsFolder = Configuration.SpectrogramsFolder.TrimEnd(Path.DirectorySeparatorChar);
            long bytes = 0;
            int count = 0;

            if (Directory.Exists(waveformsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(waveformsFolder);

                // waveform data
                bytes = 0;
                count = 0;
                foreach (FileInfo fi in di.GetFiles("*.wav"))
                {
                    bytes += fi.Length;
                    count++;
                }
            }

            if (Directory.Exists(spectrogramsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(spectrogramsFolder);

                // spectrogram data
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    DirectoryInfo spectrogramDir = new DirectoryInfo(dir.FullName);
                    foreach (FileInfo fi in spectrogramDir.GetFiles("*.gif"))
                    {
                        bytes += fi.Length;
                        count++;
                    }

                    foreach (FileInfo fi in spectrogramDir.GetFiles("*.db"))
                    {
                        bytes += fi.Length;
                        count++;
                    }

                    string xmlFileName = Path.Combine(dir.FullName, "Info.xml");
                    if (File.Exists(xmlFileName))
                    {
                        FileInfo fi = new FileInfo(xmlFileName);
                        bytes += fi.Length;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                this.buttonWaveformsFolderEmpty.Enabled = true;
                this.labelWaveformsFolderInfo.Text = string.Format(language.WaveformAndSpectrogramsFolderInfo, count, bytes / 1024.0 / 1024.0);
            }
            else
            {
                this.buttonWaveformsFolderEmpty.Enabled = false;
                this.labelWaveformsFolderInfo.Text = string.Format(language.WaveformAndSpectrogramsFolderInfo, 0, 0);
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="newFile">
        /// The new file.
        /// </param>
        /// <param name="openFile">
        /// The open file.
        /// </param>
        /// <param name="saveFile">
        /// The save file.
        /// </param>
        /// <param name="saveFileAs">
        /// The save file as.
        /// </param>
        /// <param name="find">
        /// The find.
        /// </param>
        /// <param name="replace">
        /// The replace.
        /// </param>
        /// <param name="fixCommonErrors">
        /// The fix common errors.
        /// </param>
        /// <param name="visualSync">
        /// The visual sync.
        /// </param>
        /// <param name="spellCheck">
        /// The spell check.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="help">
        /// The help.
        /// </param>
        public void Initialize(Icon icon, Image newFile, Image openFile, Image saveFile, Image saveFileAs, Image find, Image replace, Image fixCommonErrors, Image visualSync, Image spellCheck, Image settings, Image help)
        {
            this.Icon = (Icon)icon.Clone();
            this.pictureBoxNew.Image = (Image)newFile.Clone();
            this.pictureBoxOpen.Image = (Image)openFile.Clone();
            this.pictureBoxSave.Image = (Image)saveFile.Clone();
            this.pictureBoxSaveAs.Image = (Image)saveFileAs.Clone();
            this.pictureBoxFind.Image = (Image)find.Clone();
            this.pictureBoxReplace.Image = (Image)replace.Clone();
            this.pictureBoxTBFixCommonErrors.Image = (Image)fixCommonErrors.Clone();
            this.pictureBoxVisualSync.Image = (Image)visualSync.Clone();
            this.pictureBoxSpellCheck.Image = (Image)spellCheck.Clone();
            this.pictureBoxSettings.Image = (Image)settings.Clone();
            this.pictureBoxHelp.Image = (Image)help.Clone();
        }

        /// <summary>
        /// The list word list languages.
        /// </summary>
        private void ListWordListLanguages()
        {
            // Examples: da_DK_user.xml, eng_OCRFixReplaceList.xml, en_US_names_etc.xml
            string dir = Utilities.DictionaryFolder;

            if (Directory.Exists(dir))
            {
                var cultures = new List<CultureInfo>();
                foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    string name = culture.Name;

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (Directory.GetFiles(dir, name.Replace('-', '_') + "_user.xml").Length == 1)
                        {
                            if (!cultures.Contains(culture))
                            {
                                cultures.Add(culture);
                            }
                        }

                        if (Directory.GetFiles(dir, name.Replace('-', '_') + "_names_etc.xml").Length == 1)
                        {
                            if (!cultures.Contains(culture))
                            {
                                cultures.Add(culture);
                            }
                        }

                        if (Directory.GetFiles(dir, culture.ThreeLetterISOLanguageName + "_OCRFixReplaceList.xml").Length == 1)
                        {
                            bool found = false;
                            foreach (CultureInfo ci in cultures)
                            {
                                if (ci.ThreeLetterISOLanguageName == culture.ThreeLetterISOLanguageName)
                                {
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                cultures.Add(culture);
                            }
                        }
                        else if (Directory.GetFiles(dir, culture.ThreeLetterISOLanguageName + "_OCRFixReplaceList_User.xml").Length == 1)
                        {
                            bool found = false;
                            foreach (CultureInfo ci in cultures)
                            {
                                if (ci.ThreeLetterISOLanguageName == culture.ThreeLetterISOLanguageName)
                                {
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                cultures.Add(culture);
                            }
                        }
                    }
                }

                foreach (var culture in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    if (Directory.GetFiles(dir, culture.ThreeLetterISOLanguageName + "_OCRFixReplaceList.xml").Length == 1)
                    {
                        bool found = false;
                        foreach (CultureInfo ci in cultures)
                        {
                            if (ci.ThreeLetterISOLanguageName == culture.ThreeLetterISOLanguageName)
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            cultures.Add(culture);
                        }
                    }
                    else if (Directory.GetFiles(dir, culture.ThreeLetterISOLanguageName + "_OCRFixReplaceList_User.xml").Length == 1)
                    {
                        bool found = false;
                        foreach (CultureInfo ci in cultures)
                        {
                            if (ci.ThreeLetterISOLanguageName == culture.ThreeLetterISOLanguageName)
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            cultures.Add(culture);
                        }
                    }
                }

                this.comboBoxWordListLanguage.Items.Clear();
                if (Configuration.Settings.WordLists.LastLanguage == null)
                {
                    Configuration.Settings.WordLists.LastLanguage = "en-US";
                }

                foreach (CultureInfo ci in cultures)
                {
                    this.comboBoxWordListLanguage.Items.Add(new ComboBoxLanguage { CultureInfo = ci });
                    if (ci.Name == Configuration.Settings.WordLists.LastLanguage)
                    {
                        this.comboBoxWordListLanguage.SelectedIndex = this.comboBoxWordListLanguage.Items.Count - 1;
                    }
                }

                if (this.comboBoxWordListLanguage.Items.Count > 0 && this.comboBoxWordListLanguage.SelectedIndex == -1)
                {
                    this.comboBoxWordListLanguage.SelectedIndex = 0;
                }
            }
            else
            {
                this.groupBoxWordLists.Enabled = false;
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
            GeneralSettings gs = Configuration.Settings.General;

            gs.ShowToolbarNew = this.checkBoxToolbarNew.Checked;
            gs.ShowToolbarOpen = this.checkBoxToolbarOpen.Checked;
            gs.ShowToolbarSave = this.checkBoxToolbarSave.Checked;
            gs.ShowToolbarSaveAs = this.checkBoxToolbarSaveAs.Checked;
            gs.ShowToolbarFind = this.checkBoxToolbarFind.Checked;
            gs.ShowToolbarReplace = this.checkBoxReplace.Checked;
            gs.ShowToolbarFixCommonErrors = this.checkBoxTBFixCommonErrors.Checked;
            gs.ShowToolbarVisualSync = this.checkBoxVisualSync.Checked;
            gs.ShowToolbarSettings = this.checkBoxSettings.Checked;
            gs.ShowToolbarSpellCheck = this.checkBoxSpellCheck.Checked;
            gs.ShowToolbarHelp = this.checkBoxHelp.Checked;

            gs.ShowFrameRate = this.checkBoxShowFrameRate.Checked;
            double outFrameRate;
            if (double.TryParse(this.comboBoxFrameRate.Text.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out outFrameRate))
            {
                gs.DefaultFrameRate = outFrameRate;
            }

            gs.DefaultEncoding = Encoding.UTF8.BodyName;
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.CodePage + ": " + ei.DisplayName == this.comboBoxEncoding.Text)
                {
                    gs.DefaultEncoding = this.comboBoxEncoding.Text;
                }
            }

            gs.AutoGuessAnsiEncoding = this.checkBoxAutoDetectAnsiEncoding.Checked;
            gs.SubtitleFontSize = int.Parse(this.comboBoxSubtitleFontSize.Text);
            gs.SubtitleFontBold = this.checkBoxSubtitleFontBold.Checked;
            gs.CenterSubtitleInTextBox = this.checkBoxSubtitleCenter.Checked;
            gs.SubtitleFontColor = this.panelSubtitleFontColor.BackColor;
            gs.SubtitleBackgroundColor = this.panelSubtitleBackgroundColor.BackColor;
            gs.ShowRecentFiles = this.checkBoxRememberRecentFiles.Checked;
            gs.RememberSelectedLine = this.checkBoxRememberSelectedLine.Checked;
            gs.StartLoadLastFile = this.checkBoxReopenLastOpened.Checked;
            gs.StartRememberPositionAndSize = this.checkBoxRememberWindowPosition.Checked;
            gs.StartInSourceView = this.checkBoxStartInSourceView.Checked;
            gs.RemoveBlankLinesWhenOpening = this.checkBoxRemoveBlankLinesWhenOpening.Checked;
            gs.ListViewLineSeparatorString = this.textBoxShowLineBreaksAs.Text;
            if (string.IsNullOrWhiteSpace(gs.ListViewLineSeparatorString))
            {
                gs.ListViewLineSeparatorString = "<br />";
            }

            gs.ListViewDoubleClickAction = this.comboBoxListViewDoubleClickEvent.SelectedIndex;

            gs.SubtitleMinimumDisplayMilliseconds = (int)this.numericUpDownDurationMin.Value;
            gs.SubtitleMaximumDisplayMilliseconds = (int)this.numericUpDownDurationMax.Value;
            gs.MinimumMillisecondsBetweenLines = (int)this.numericUpDownMinGapMs.Value;

            if (this.comboBoxAutoBackup.SelectedIndex == 1)
            {
                gs.AutoBackupSeconds = 60;
            }
            else if (this.comboBoxAutoBackup.SelectedIndex == 2)
            {
                gs.AutoBackupSeconds = 60 * 5;
            }
            else if (this.comboBoxAutoBackup.SelectedIndex == 3)
            {
                gs.AutoBackupSeconds = 60 * 15;
            }
            else
            {
                gs.AutoBackupSeconds = 0;
            }

            gs.CheckForUpdates = this.checkBoxCheckForUpdates.Checked;

            if (this.comboBoxTimeCodeMode.Visible)
            {
                gs.UseTimeFormatHHMMSSFF = this.comboBoxTimeCodeMode.SelectedIndex == 1;
            }

            if (this.comboBoxSpellChecker.SelectedIndex == 1)
            {
                gs.SpellChecker = "word";
            }
            else
            {
                gs.SpellChecker = "hunspell";
            }

            gs.AllowEditOfOriginalSubtitle = this.checkBoxAllowEditOfOriginalSubtitle.Checked;
            gs.PromptDeleteLines = this.checkBoxPromptDeleteLines.Checked;

            if (this.radioButtonVideoPlayerMPlayer.Checked)
            {
                gs.VideoPlayer = "MPlayer";
            }

            // else if (radioButtonVideoPlayerManagedDirectX.Checked)
            // gs.VideoPlayer = "ManagedDirectX";
            else if (this.radioButtonVideoPlayerMpcHc.Checked)
            {
                gs.VideoPlayer = "MPC-HC";
            }
            else if (this.radioButtonVideoPlayerVLC.Checked)
            {
                gs.VideoPlayer = "VLC";
            }
            else
            {
                gs.VideoPlayer = "DirectShow";
            }

            gs.VlcLocation = this.textBoxVlcPath.Text;

            gs.VideoPlayerShowStopButton = this.checkBoxVideoPlayerShowStopButton.Checked;
            gs.VideoPlayerShowMuteButton = this.checkBoxVideoPlayerShowMuteButton.Checked;
            gs.VideoPlayerShowFullscreenButton = this.checkBoxVideoPlayerShowFullscreenButton.Checked;
            gs.VideoPlayerPreviewFontSize = int.Parse(this.comboBoxlVideoPlayerPreviewFontSize.Items[0].ToString()) + this.comboBoxlVideoPlayerPreviewFontSize.SelectedIndex;
            gs.VideoPlayerPreviewFontBold = this.checkBoxVideoPlayerPreviewFontBold.Checked;

            Configuration.Settings.VideoControls.CustomSearchText1 = this.comboBoxCustomSearch1.Text;
            Configuration.Settings.VideoControls.CustomSearchText2 = this.comboBoxCustomSearch2.Text;
            Configuration.Settings.VideoControls.CustomSearchText3 = this.comboBoxCustomSearch3.Text;
            Configuration.Settings.VideoControls.CustomSearchText4 = this.comboBoxCustomSearch4.Text;
            Configuration.Settings.VideoControls.CustomSearchText5 = this.comboBoxCustomSearch5.Text;
            Configuration.Settings.VideoControls.CustomSearchText6 = this.comboBoxCustomSearch6.Text;
            Configuration.Settings.VideoControls.CustomSearchUrl1 = this.textBoxCustomSearchUrl1.Text;
            Configuration.Settings.VideoControls.CustomSearchUrl2 = this.textBoxCustomSearchUrl2.Text;
            Configuration.Settings.VideoControls.CustomSearchUrl3 = this.textBoxCustomSearchUrl3.Text;
            Configuration.Settings.VideoControls.CustomSearchUrl4 = this.textBoxCustomSearchUrl4.Text;
            Configuration.Settings.VideoControls.CustomSearchUrl5 = this.textBoxCustomSearchUrl5.Text;
            Configuration.Settings.VideoControls.CustomSearchUrl6 = this.textBoxCustomSearchUrl6.Text;

            int maxLength = (int)this.numericUpDownSubtitleLineMaximumLength.Value;
            if (maxLength > 9 && maxLength < 1000)
            {
                gs.SubtitleLineMaximumLength = maxLength;
            }
            else if (maxLength > 999)
            {
                gs.SubtitleLineMaximumLength = 999;
            }
            else
            {
                gs.SubtitleLineMaximumLength = 45;
            }

            gs.SubtitleMaximumCharactersPerSeconds = (double)this.numericUpDownMaxCharsSec.Value;

            gs.AutoWrapLineWhileTyping = this.checkBoxAutoWrapWhileTyping.Checked;

            if (this.comboBoxSubtitleFont.SelectedItem != null)
            {
                gs.SubtitleFontName = this.comboBoxSubtitleFont.SelectedItem.ToString();
            }

            ToolsSettings toolsSettings = Configuration.Settings.Tools;
            toolsSettings.VerifyPlaySeconds = this.comboBoxToolsVerifySeconds.SelectedIndex + 2;
            toolsSettings.StartSceneIndex = this.comboBoxToolsStartSceneIndex.SelectedIndex;
            toolsSettings.EndSceneIndex = this.comboBoxToolsEndSceneIndex.SelectedIndex;
            toolsSettings.MergeLinesShorterThan = this.comboBoxMergeShortLineLength.SelectedIndex + 10;
            if (toolsSettings.MergeLinesShorterThan > gs.SubtitleLineMaximumLength + 1)
            {
                toolsSettings.MergeLinesShorterThan = gs.SubtitleLineMaximumLength;
            }

            toolsSettings.MusicSymbol = this.comboBoxToolsMusicSymbol.SelectedItem.ToString();
            toolsSettings.MusicSymbolToReplace = this.textBoxMusicSymbolsToReplace.Text;
            toolsSettings.SpellCheckAutoChangeNames = this.checkBoxSpellCheckAutoChangeNames.Checked;
            toolsSettings.SpellCheckOneLetterWords = this.checkBoxSpellCheckOneLetterWords.Checked;
            toolsSettings.SpellCheckEnglishAllowInQuoteAsIng = this.checkBoxTreatINQuoteAsING.Checked;
            toolsSettings.UseNoLineBreakAfter = this.checkBoxUseDoNotBreakAfterList.Checked;
            toolsSettings.OcrFixUseHardcodedRules = this.checkBoxFixCommonOcrErrorsUsingHardcodedRules.Checked;
            toolsSettings.FixShortDisplayTimesAllowMoveStartTime = this.checkBoxFixShortDisplayTimesAllowMoveStartTime.Checked;

            WordListSettings wordListSettings = Configuration.Settings.WordLists;
            wordListSettings.UseOnlineNamesEtc = this.checkBoxNamesEtcOnline.Checked;
            wordListSettings.NamesEtcUrl = this.textBoxNamesEtcOnline.Text;
            if (this.comboBoxWordListLanguage.Items.Count > 0 && this.comboBoxWordListLanguage.SelectedIndex >= 0)
            {
                var ci = this.comboBoxWordListLanguage.Items[this.comboBoxWordListLanguage.SelectedIndex] as ComboBoxLanguage;
                if (ci != null)
                {
                    Configuration.Settings.WordLists.LastLanguage = ci.CultureInfo.Name;
                }
            }

            SubtitleSettings ssa = Configuration.Settings.SubtitleSettings;
            ssa.SsaFontName = this._ssaFontName;
            ssa.SsaFontSize = this._ssaFontSize;
            ssa.SsaFontColorArgb = this._ssaFontColor;
            ssa.SsaOutline = (int)this.numericUpDownSsaOutline.Value;
            ssa.SsaShadow = (int)this.numericUpDownSsaShadow.Value;
            ssa.SsaOpaqueBox = this.checkBoxSsaOpaqueBox.Checked;

            ProxySettings proxy = Configuration.Settings.Proxy;
            proxy.ProxyAddress = this.textBoxProxyAddress.Text;
            proxy.UserName = this.textBoxProxyUserName.Text;
            if (string.IsNullOrWhiteSpace(this.textBoxProxyPassword.Text))
            {
                proxy.Password = null;
            }
            else
            {
                proxy.EncodePassword(this.textBoxProxyPassword.Text);
            }

            proxy.Domain = this.textBoxProxyDomain.Text;

            Configuration.Settings.Tools.ListViewSyntaxColorDurationSmall = this.checkBoxSyntaxColorDurationTooSmall.Checked;
            Configuration.Settings.Tools.ListViewSyntaxColorDurationBig = this.checkBoxSyntaxColorDurationTooLarge.Checked;
            Configuration.Settings.Tools.ListViewSyntaxColorLongLines = this.checkBoxSyntaxColorTextTooLong.Checked;
            Configuration.Settings.Tools.ListViewSyntaxMoreThanXLines = this.checkBoxSyntaxColorTextMoreThanTwoLines.Checked;
            Configuration.Settings.Tools.ListViewSyntaxMoreThanXLinesX = (int)this.numericUpDownSyntaxColorTextMoreThanXLines.Value;
            Configuration.Settings.Tools.ListViewSyntaxColorOverlap = this.checkBoxSyntaxOverlap.Checked;
            Configuration.Settings.Tools.ListViewSyntaxErrorColor = this.panelListViewSyntaxColorError.BackColor;

            Configuration.Settings.VideoControls.WaveformDrawGrid = this.checkBoxWaveformShowGrid.Checked;
            Configuration.Settings.VideoControls.WaveformGridColor = this.panelWaveformGridColor.BackColor;
            Configuration.Settings.VideoControls.WaveformSelectedColor = this.panelWaveformSelectedColor.BackColor;
            Configuration.Settings.VideoControls.WaveformColor = this.panelWaveformColor.BackColor;
            Configuration.Settings.VideoControls.WaveformBackgroundColor = this.panelWaveformBackgroundColor.BackColor;
            Configuration.Settings.VideoControls.WaveformTextColor = this.panelWaveformTextColor.BackColor;
            Configuration.Settings.VideoControls.GenerateSpectrogram = this.checkBoxGenerateSpectrogram.Checked;
            if (this.comboBoxSpectrogramAppearance.SelectedIndex == 0)
            {
                Configuration.Settings.VideoControls.SpectrogramAppearance = "OneColorGradient";
            }
            else
            {
                Configuration.Settings.VideoControls.SpectrogramAppearance = "Classic";
            }

            Configuration.Settings.VideoControls.WaveformTextSize = int.Parse(this.comboBoxWaveformTextSize.Text);
            Configuration.Settings.VideoControls.WaveformTextBold = this.checkBoxWaveformTextBold.Checked;
            Configuration.Settings.VideoControls.WaveformMouseWheelScrollUpIsForward = this.checkBoxReverseMouseWheelScrollDirection.Checked;
            Configuration.Settings.VideoControls.WaveformAllowOverlap = this.checkBoxAllowOverlap.Checked;
            Configuration.Settings.VideoControls.WaveformFocusOnMouseEnter = this.checkBoxWaveformHoverFocus.Checked;
            Configuration.Settings.VideoControls.WaveformListViewFocusOnMouseEnter = this.checkBoxListViewMouseEnterFocus.Checked;
            Configuration.Settings.VideoControls.WaveformBorderHitMs = Convert.ToInt32(this.numericUpDownWaveformBorderHitMs.Value);
            gs.UseFFmpegForWaveExtraction = this.checkBoxUseFFmpeg.Checked;
            gs.FFmpegLocation = this.textBoxFFmpegPath.Text;

            // Main General
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[0].Nodes)
            {
                var indexOfBracket = node.Text.IndexOf('[');
                if (indexOfBracket >= 0)
                {
                    string text = node.Text.Substring(0, indexOfBracket).Trim();
                    if (text == Configuration.Settings.Language.Settings.GoToFirstSelectedLine.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralGoToFirstSelectedLine = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoToNextEmptyLine.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralGoToNextEmptyLine = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MergeSelectedLines.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralMergeSelectedLines = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MergeSelectedLinesOnlyFirstText.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesOnlyFirstText = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ToggleTranslationMode.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralToggleTranslationMode = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.SwitchOriginalAndTranslation.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralSwitchOriginalAndTranslation = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MergeOriginalAndTranslation.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralMergeOriginalAndTranslation = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoToPrevious.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoToCurrentSubtitleStart.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralGoToStartOfCurrentSubtitle = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoToCurrentSubtitleEnd.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralGoToEndOfCurrentSubtitle = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.WaveformPlayFirstSelectedSubtitle.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.GeneralPlayFirstSelected = GetShortcut(node.Text);
                    }
                }
            }

            // Main File
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[1].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.Menu.File.New.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileNew = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.File.Open.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileOpen = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.File.OpenKeepVideo.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileOpenKeepVideo = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.File.Save.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileSave = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.File.SaveAs.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileSaveAs = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.File.SaveOriginal.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileSaveOriginal = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.SaveOriginalSubtitleAs.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileSaveOriginalAs = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainFileSaveAll.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileSaveAll = GetShortcut(node.Text);
                    }
                    else if (text == (Configuration.Settings.Language.Main.Menu.File.Export + " -> " + Configuration.Settings.Language.Main.Menu.File.ExportEbu).Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainFileExportEbu = GetShortcut(node.Text);
                    }
                }
            }

            // Main Edit
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[2].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.Menu.Edit.Undo.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditUndo = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Edit.Redo.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditRedo = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Edit.Find.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditFind = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Edit.FindNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditFindNext = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Edit.Replace.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditReplace = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Edit.MultipleReplace.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditMultipleReplace = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Edit.GoToSubtitleNumber.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditGoToLineNumber = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.VobSubOcr.RightToLeft.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditRightToLeft = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ReverseStartAndEndingForRTL.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditReverseStartAndEndingForRTL = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ToggleTranslationAndOriginalInPreviews.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews = GetShortcut(node.Text);
                    }
                }
            }

            // Main Tools
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[3].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.Menu.Tools.FixCommonErrors.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsFixCommonErrors = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Tools.StartNumberingFrom.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsRenumber = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Tools.RemoveTextForHearingImpaired.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsRemoveTextForHI = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Tools.ChangeCasing.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsChangeCasing = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Tools.SplitLongLines.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsSplitLongLines = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Tools.MergeShortLines.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsMergeShortLines = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.ContextMenu.AutoDurationCurrentLine.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsAutoDuration = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ShowBeamer.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToolsBeamer = GetShortcut(node.Text);
                    }
                }
            }

            // Main Video
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[4].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.Menu.Video.ShowHideVideo.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideoShowHideVideo = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ToggleDockUndockOfVideoControls.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideoToggleVideoControls = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoBack1Frame.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo1FrameLeft = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoForward1Frame.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo1FrameRight = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoBack100Milliseconds.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo100MsLeft = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoForward100Milliseconds.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo100MsRight = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoBack500Milliseconds.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo500MsLeft = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoForward500Milliseconds.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo500MsRight = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoBack1Second.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo1000MsLeft = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.GoForward1Second.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideo1000MsRight = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.Fullscreen.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideoFullscreen = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.Pause.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideoPause = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.TogglePlayPause.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainVideoPlayPauseToggle = GetShortcut(node.Text);
                    }
                }
            }

            // Main Spell check
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[5].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.Menu.SpellCheck.Title.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSpellCheck = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.SpellCheck.FindDoubleWords.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSpellCheckFindDoubleWords = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.SpellCheck.AddToNamesEtcList.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSpellCheckAddWordToNames = GetShortcut(node.Text);
                    }
                }
            }

            // Main Sync
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[6].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.Menu.Synchronization.AdjustAllTimes.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSynchronizationAdjustTimes = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Synchronization.VisualSync.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSynchronizationVisualSync = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Synchronization.PointSync.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSynchronizationPointSync = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.Tools.ChangeFrameRate.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainSynchronizationChangeFrameRate = GetShortcut(node.Text);
                    }
                }
            }

            // Main List view
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[7].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.General.Italic.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewItalic = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.ContextMenu.InsertAfter.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainInsertAfter = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.ContextMenu.InsertBefore.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainInsertBefore = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MergeDialog.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainMergeDialog = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ToggleFocus.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainToggleFocus = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ToggleDialogDashes.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewToggleDashes = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.Alignment.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewAlignment = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CopyTextOnly.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewCopyText = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CopyTextOnlyFromOriginalToCurrent.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewCopyTextFromOriginalToCurrent = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AutoDurationSelectedLines.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewAutoDuration = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ListViewColumnDelete.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewColumnDeleteText = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ListViewColumnInsert.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewColumnInsertText = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ListViewColumnPaste.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewColumnPaste = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ListViewFocusWaveform.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewFocusWaveform = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.ListViewGoToNextError.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainListViewGoToNextError = GetShortcut(node.Text);
                    }
                }
            }

            // Main text box
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[8].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.General.Italic.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxItalic = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.ContextMenu.SplitLineAtCursorPosition.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxSplitAtCursor = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxMoveLastWordDown.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxMoveLastWordDown = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxMoveFirstWordFromNextUp.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxMoveFirstWordFromNextUp = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxSelectionToLower.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxSelectionToLower = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxSelectionToUpper.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxSelectionToUpper = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxToggleAutoDuration.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxToggleAutoDuration = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.Menu.ContextMenu.InsertAfter.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxInsertAfter = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxAutoBreak.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxAutoBreak = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainTextBoxUnbreak.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTextBoxUnbreak = GetShortcut(node.Text);
                    }
                }
            }

            // Create
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[9].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Main.VideoControls.InsertNewSubtitleAtVideoPosition.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPos = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.PlayFromJustBeforeText.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainCreatePlayFromJustBefore = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.SetStartTime.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainCreateSetStart = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.SetEndTime.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainCreateSetEnd = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.MainCreateStartDownEndUp.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainCreateStartDownEndUp = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CreateSetEndAddNewAndGoToNew.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainCreateSetEndAddNewAndGoToNew = GetShortcut(node.Text);
                    }
                }
            }

            // Translate
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[10].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Settings.CustomSearch1.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTranslateCustomSearch1 = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CustomSearch2.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTranslateCustomSearch2 = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CustomSearch3.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTranslateCustomSearch3 = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CustomSearch4.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTranslateCustomSearch4 = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CustomSearch5.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTranslateCustomSearch5 = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.CustomSearch6.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainTranslateCustomSearch6 = GetShortcut(node.Text);
                    }
                }
            }

            // Adjust
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[11].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Settings.AdjustViaEndAutoStartAndGoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustViaEndAutoStartAndGoToNext = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.SetstartTimeAndOffsetOfRest.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.SetEndTimeAndGoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetEndAndGotoNext = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSetStartAutoDurationAndGoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetStartAutoDurationAndGoToNext = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSetEndNextStartAndGoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetEndNextStartAndGoToNext = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustStartDownEndUpAndGoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustStartDownEndUpAndGoToNext = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.SetStartTime.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetStart = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSetStartTimeKeepDuration.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetStartKeepDuration = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.SetEndTime.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetEnd = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSelected100MsForward.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSelected100MsForward = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSelected100MsBack.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSelected100MsBack = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSetEndAndOffsetTheRest.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetEndAndOffsetTheRest = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.AdjustSetEndAndOffsetTheRestAndGoToNext.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainAdjustSetEndAndOffsetTheRestAndGoToNext = GetShortcut(node.Text);
                    }
                }
            }

            // Audio-visualizer
            foreach (TreeNode node in this.treeViewShortcuts.Nodes[12].Nodes)
            {
                if (node.Text.Contains('['))
                {
                    string text = node.Text.Substring(0, node.Text.IndexOf('[')).Trim();
                    if (text == Configuration.Settings.Language.Waveform.ZoomIn.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformZoomIn = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Waveform.ZoomOut.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformZoomOut = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.WaveformPlayNewSelection.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformPlaySelection = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.VerticalZoom.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformVerticalZoom = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.VerticalZoomOut.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformVerticalZoomOut = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.WaveformSeekSilenceForward.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformSearchSilenceForward = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.WaveformSeekSilenceBack.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformSearchSilenceBack = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.WaveformAddTextHere.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformAddTextHere = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Main.VideoControls.InsertNewSubtitleAtVideoPosition.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.MainWaveformInsertAtCurrentPosition = GetShortcut(node.Text);
                    }
                    else if (text == Configuration.Settings.Language.Settings.WaveformFocusListView.Replace("&", string.Empty))
                    {
                        Configuration.Settings.Shortcuts.WaveformFocusListView = GetShortcut(node.Text);
                    }
                }
            }

            Configuration.Settings.Save();
        }

        /// <summary>
        /// The form settings_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.comboBoxShortcutKey.Focused)
            {
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#Settings");
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The button ssa choose font click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSsaChooseFontClick(object sender, EventArgs e)
        {
            if (this.fontDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this._ssaFontName = this.fontDialogSSAStyle.Font.Name;
                this._ssaFontSize = this.fontDialogSSAStyle.Font.Size;
                this.UpdateSsaExample();
            }
        }

        /// <summary>
        /// The update ssa example.
        /// </summary>
        private void UpdateSsaExample()
        {
            this.labelSSAFont.Text = string.Format("{0}, size {1}", this.fontDialogSSAStyle.Font.Name, this.fontDialogSSAStyle.Font.Size);
            this.GeneratePreviewReal();
        }

        /// <summary>
        /// The generate preview real.
        /// </summary>
        private void GeneratePreviewReal()
        {
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
                    font = new Font(this._ssaFontName, (float)this._ssaFontSize);
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

                var measuredWidth = TextDraw.MeasureTextWidth(font, sb.ToString(), false) + 1;
                var measuredHeight = TextDraw.MeasureTextHeight(font, sb.ToString(), false) + 1;

                float left = (float)(bmp.Width - measuredWidth * 0.8 + 15) / 2;

                float top = bmp.Height - measuredHeight - 10;

                const int leftMargin = 0;
                int pathPointsStart = -1;

                if (this.checkBoxSsaOpaqueBox.Checked)
                {
                    g.FillRectangle(new SolidBrush(Color.Black), left, top, measuredWidth + 3, measuredHeight + 3);
                }

                TextDraw.DrawText(font, sf, path, sb, false, false, false, left, top, ref newLine, leftMargin, ref pathPointsStart);

                int outline = (int)this.numericUpDownSsaOutline.Value;

                // draw shadow
                if (this.numericUpDownSsaShadow.Value > 0 && !this.checkBoxSsaOpaqueBox.Checked)
                {
                    for (int i = 0; i < (int)this.numericUpDownSsaShadow.Value; i++)
                    {
                        var shadowPath = new GraphicsPath();
                        sb = new StringBuilder();
                        sb.Append("This is a test!");
                        int pathPointsStart2 = -1;
                        TextDraw.DrawText(font, sf, shadowPath, sb, false, false, false, left + i + outline, top + i + outline, ref newLine, leftMargin, ref pathPointsStart2);
                        g.FillPath(new SolidBrush(Color.FromArgb(200, Color.Black)), shadowPath);
                    }
                }

                if (outline > 0 && !this.checkBoxSsaOpaqueBox.Checked)
                {
                    g.DrawPath(new Pen(Color.Black, outline), path);
                }

                g.FillPath(new SolidBrush(Color.FromArgb(this._ssaFontColor)), path);
            }

            this.pictureBoxPreview.Image = bmp;
        }

        /// <summary>
        /// The button ssa choose color click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSsaChooseColorClick(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = Color.FromArgb(this._ssaFontColor);
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this._ssaFontColor = this.colorDialogSSAStyle.Color.ToArgb();
                this.UpdateSsaExample();
            }
        }

        /// <summary>
        /// The combo box word list language selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxWordListLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemoveNameEtc.Enabled = false;
            this.buttonAddNamesEtc.Enabled = false;
            this.buttonRemoveUserWord.Enabled = false;
            this.buttonAddUserWord.Enabled = false;
            this.buttonRemoveOcrFix.Enabled = false;
            this.buttonAddOcrFix.Enabled = false;

            this.listBoxNamesEtc.Items.Clear();
            this.listBoxUserWordLists.Items.Clear();
            this.listBoxOcrFixList.Items.Clear();
            var cb = this.comboBoxWordListLanguage.Items[this.comboBoxWordListLanguage.SelectedIndex] as ComboBoxLanguage;
            if (cb != null)
            {
                string language = this.GetCurrentWordListLanguage();

                this.buttonAddNamesEtc.Enabled = true;
                this.buttonAddUserWord.Enabled = true;
                this.buttonAddOcrFix.Enabled = true;

                // user word list
                this.LoadUserWords(language, true);

                // OCR fix words
                this.LoadOcrFixList(true);

                this.LoadNamesEtc(language, true);
            }
        }

        /// <summary>
        /// The load ocr fix list.
        /// </summary>
        /// <param name="reloadListBox">
        /// The reload list box.
        /// </param>
        private void LoadOcrFixList(bool reloadListBox)
        {
            var cb = this.comboBoxWordListLanguage.Items[this.comboBoxWordListLanguage.SelectedIndex] as ComboBoxLanguage;
            if (cb == null)
            {
                return;
            }

            if (reloadListBox)
            {
                this.listBoxOcrFixList.Items.Clear();
            }

            this._ocrFixReplaceList = OcrFixReplaceList.FromLanguageId(cb.CultureInfo.ThreeLetterISOLanguageName);
            if (reloadListBox)
            {
                this.listBoxOcrFixList.BeginUpdate();
                foreach (var pair in this._ocrFixReplaceList.WordReplaceList)
                {
                    this.listBoxOcrFixList.Items.Add(pair.Key + " --> " + pair.Value);
                }

                foreach (var pair in this._ocrFixReplaceList.PartialLineWordBoundaryReplaceList)
                {
                    this.listBoxOcrFixList.Items.Add(pair.Key + " --> " + pair.Value);
                }

                this.listBoxOcrFixList.Sorted = true;
                this.listBoxOcrFixList.EndUpdate();
            }
        }

        /// <summary>
        /// The load user words.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="reloadListBox">
        /// The reload list box.
        /// </param>
        private void LoadUserWords(string language, bool reloadListBox)
        {
            this._userWordList = new List<string>();
            Utilities.LoadUserWordList(this._userWordList, language);
            this._userWordList.Sort();

            if (reloadListBox)
            {
                this.listBoxUserWordLists.Items.Clear();
                this.listBoxUserWordLists.BeginUpdate();
                foreach (string name in this._userWordList)
                {
                    this.listBoxUserWordLists.Items.Add(name);
                }

                this.listBoxUserWordLists.EndUpdate();
            }
        }

        /// <summary>
        /// The load names etc.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="reloadListBox">
        /// The reload list box.
        /// </param>
        private void LoadNamesEtc(string language, bool reloadListBox)
        {
            var task = Task.Factory.StartNew(() =>
                {
                    // names etc
                    var namesList = new NamesList(Configuration.DictionariesFolder, language, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
                    this._wordListNamesEtc = namesList.GetAllNames();
                    this._wordListNamesEtc.Sort();
                    return this._wordListNamesEtc;
                });

            if (reloadListBox)
            {
                // reload the listbox on a continuation ui thead
                var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(originalTask =>
                    {
                        this.listBoxNamesEtc.BeginUpdate();
                        this.listBoxNamesEtc.Items.Clear();
                        foreach (var name in originalTask.Result)
                        {
                            this.listBoxNamesEtc.Items.Add(name);
                        }

                        this.listBoxNamesEtc.EndUpdate();
                    }, uiContext);
            }
        }

        /// <summary>
        /// The get current word list language.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetCurrentWordListLanguage()
        {
            var cb = this.comboBoxWordListLanguage.Items[this.comboBoxWordListLanguage.SelectedIndex] as ComboBoxLanguage;
            if (cb != null)
            {
                return cb.CultureInfo.Name.Replace('-', '_');
            }

            return null;
        }

        /// <summary>
        /// The button add names etc click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddNamesEtcClick(object sender, EventArgs e)
        {
            string language = this.GetCurrentWordListLanguage();
            string text = this.textBoxNameEtc.Text.Trim();
            if (!string.IsNullOrEmpty(language) && text.Length > 1 && !this._wordListNamesEtc.Contains(text))
            {
                var namesList = new NamesList(Configuration.DictionariesFolder, language, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
                namesList.Add(text);
                this.LoadNamesEtc(language, true);
                this.labelStatus.Text = string.Format(Configuration.Settings.Language.Settings.WordAddedX, text);
                this.textBoxNameEtc.Text = string.Empty;
                this.textBoxNameEtc.Focus();
                for (int i = 0; i < this.listBoxNamesEtc.Items.Count; i++)
                {
                    if (this.listBoxNamesEtc.Items[i].ToString() == text)
                    {
                        this.listBoxNamesEtc.SelectedIndex = i;
                        int top = i - 5;
                        if (top < 0)
                        {
                            top = 0;
                        }

                        this.listBoxNamesEtc.TopIndex = top;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show(Configuration.Settings.Language.Settings.WordAlreadyExists);
            }
        }

        /// <summary>
        /// The list box names etc selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxNamesEtcSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemoveNameEtc.Enabled = this.listBoxNamesEtc.SelectedIndex >= 0;
        }

        /// <summary>
        /// The button remove name etc click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonRemoveNameEtcClick(object sender, EventArgs e)
        {
            if (this.listBoxNamesEtc.SelectedIndices.Count == 0)
            {
                return;
            }

            string language = this.GetCurrentWordListLanguage();
            int index = this.listBoxNamesEtc.SelectedIndex;
            string text = this.listBoxNamesEtc.Items[index].ToString();
            int itemsToRemoveCount = this.listBoxNamesEtc.SelectedIndices.Count;
            if (!string.IsNullOrEmpty(language) && index >= 0)
            {
                DialogResult result;
                if (itemsToRemoveCount == 1)
                {
                    result = MessageBox.Show(string.Format(Configuration.Settings.Language.Settings.RemoveX, text), "Subtitle Edit", MessageBoxButtons.YesNo);
                }
                else
                {
                    result = MessageBox.Show(string.Format(Configuration.Settings.Language.Main.DeleteXLinesPrompt, itemsToRemoveCount), "Subtitle Edit", MessageBoxButtons.YesNo);
                }

                if (result == DialogResult.Yes)
                {
                    int removeCount = 0;
                    var namesEtc = new List<string>();
                    var globalNamesEtc = new List<string>();
                    var namesList = new NamesList(Configuration.DictionariesFolder, language, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
                    for (int idx = this.listBoxNamesEtc.SelectedIndices.Count - 1; idx >= 0; idx--)
                    {
                        index = this.listBoxNamesEtc.SelectedIndices[idx];
                        text = this.listBoxNamesEtc.Items[index].ToString();
                        namesList.Remove(text);
                        removeCount++;
                        this.listBoxNamesEtc.Items.RemoveAt(index);
                    }

                    if (removeCount > 0)
                    {
                        this.LoadNamesEtc(language, true); // reload

                        if (index < this.listBoxNamesEtc.Items.Count)
                        {
                            this.listBoxNamesEtc.SelectedIndex = index;
                        }
                        else if (this.listBoxNamesEtc.Items.Count > 0)
                        {
                            this.listBoxNamesEtc.SelectedIndex = index - 1;
                        }

                        this.listBoxNamesEtc.Focus();

                        this.buttonRemoveNameEtc.Enabled = false;
                        return;
                    }

                    if (removeCount < itemsToRemoveCount && Configuration.Settings.WordLists.UseOnlineNamesEtc && !string.IsNullOrEmpty(Configuration.Settings.WordLists.NamesEtcUrl))
                    {
                        MessageBox.Show(Configuration.Settings.Language.Settings.CannotUpdateNamesEtcOnline);
                        return;
                    }

                    if (removeCount == 0)
                    {
                        MessageBox.Show(Configuration.Settings.Language.Settings.WordNotFound);
                    }
                }
            }
        }

        /// <summary>
        /// The text box name etc key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxNameEtcKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.ButtonAddNamesEtcClick(null, null);
            }
        }

        /// <summary>
        /// The button add user word click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddUserWordClick(object sender, EventArgs e)
        {
            string language = this.GetCurrentWordListLanguage();
            string text = this.textBoxUserWord.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(language) && text.Length > 0 && !this._userWordList.Contains(text))
            {
                Utilities.AddToUserDictionary(text, language);
                this.LoadUserWords(language, true);
                this.labelStatus.Text = string.Format(Configuration.Settings.Language.Settings.WordAddedX, text);
                this.textBoxUserWord.Text = string.Empty;
                this.textBoxUserWord.Focus();

                for (int i = 0; i < this.listBoxUserWordLists.Items.Count; i++)
                {
                    if (this.listBoxUserWordLists.Items[i].ToString() == text)
                    {
                        this.listBoxUserWordLists.SelectedIndex = i;
                        int top = i - 5;
                        if (top < 0)
                        {
                            top = 0;
                        }

                        this.listBoxUserWordLists.TopIndex = top;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show(Configuration.Settings.Language.Settings.WordAlreadyExists);
            }
        }

        /// <summary>
        /// The text box user word key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxUserWordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.ButtonAddUserWordClick(null, null);
            }
        }

        /// <summary>
        /// The button remove user word click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonRemoveUserWordClick(object sender, EventArgs e)
        {
            if (this.listBoxUserWordLists.SelectedIndices.Count == 0)
            {
                return;
            }

            string language = this.GetCurrentWordListLanguage();
            int index = this.listBoxUserWordLists.SelectedIndex;
            int itemsToRemoveCount = this.listBoxUserWordLists.SelectedIndices.Count;
            string text = this.listBoxUserWordLists.Items[index].ToString();
            if (!string.IsNullOrEmpty(language) && index >= 0)
            {
                DialogResult result;
                if (itemsToRemoveCount == 1)
                {
                    result = MessageBox.Show(string.Format(Configuration.Settings.Language.Settings.RemoveX, text), "Subtitle Edit", MessageBoxButtons.YesNo);
                }
                else
                {
                    result = MessageBox.Show(string.Format(Configuration.Settings.Language.Main.DeleteXLinesPrompt, itemsToRemoveCount), "Subtitle Edit", MessageBoxButtons.YesNo);
                }

                if (result == DialogResult.Yes)
                {
                    int removeCount = 0;
                    var words = new List<string>();
                    string userWordFileName = Utilities.LoadUserWordList(words, language);

                    for (int idx = this.listBoxUserWordLists.SelectedIndices.Count - 1; idx >= 0; idx--)
                    {
                        index = this.listBoxUserWordLists.SelectedIndices[idx];
                        text = this.listBoxUserWordLists.Items[index].ToString();

                        if (words.Contains(text))
                        {
                            words.Remove(text);
                            removeCount++;
                        }

                        this.listBoxUserWordLists.Items.RemoveAt(index);
                    }

                    if (removeCount > 0)
                    {
                        words.Sort();
                        var doc = new XmlDocument();
                        doc.Load(userWordFileName);
                        doc.DocumentElement.RemoveAll();
                        foreach (string word in words)
                        {
                            XmlNode node = doc.CreateElement("word");
                            node.InnerText = word;
                            doc.DocumentElement.AppendChild(node);
                        }

                        doc.Save(userWordFileName);
                        this.LoadUserWords(language, false); // reload
                        this.buttonRemoveUserWord.Enabled = false;

                        if (index < this.listBoxUserWordLists.Items.Count)
                        {
                            this.listBoxUserWordLists.SelectedIndex = index;
                        }
                        else if (this.listBoxUserWordLists.Items.Count > 0)
                        {
                            this.listBoxUserWordLists.SelectedIndex = index - 1;
                        }

                        this.listBoxUserWordLists.Focus();
                        return;
                    }

                    if (removeCount < itemsToRemoveCount)
                    {
                        MessageBox.Show(Configuration.Settings.Language.Settings.WordNotFound);
                    }
                }
            }
        }

        /// <summary>
        /// The list box user word lists selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxUserWordListsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemoveUserWord.Enabled = this.listBoxUserWordLists.SelectedIndex >= 0;
        }

        /// <summary>
        /// The button add ocr fix click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddOcrFixClick(object sender, EventArgs e)
        {
            string key = this.textBoxOcrFixKey.Text.Trim();
            string value = this.textBoxOcrFixValue.Text.Trim();
            if (key.Length == 0 || value.Length == 0 || key == value || Utilities.IsInteger(key))
            {
                return;
            }

            var cb = this.comboBoxWordListLanguage.Items[this.comboBoxWordListLanguage.SelectedIndex] as ComboBoxLanguage;
            if (cb == null)
            {
                return;
            }

            var added = this._ocrFixReplaceList.AddWordOrPartial(key, value);
            if (!added)
            {
                MessageBox.Show(Configuration.Settings.Language.Settings.WordAlreadyExists);
                return;
            }

            this.LoadOcrFixList(true);
            this.textBoxOcrFixKey.Text = string.Empty;
            this.textBoxOcrFixValue.Text = string.Empty;
            this.textBoxOcrFixKey.Focus();

            for (int i = 0; i < this.listBoxOcrFixList.Items.Count; i++)
            {
                if (this.listBoxOcrFixList.Items[i].ToString() == key + " --> " + value)
                {
                    this.listBoxOcrFixList.SelectedIndex = i;
                    int top = i - 5;
                    if (top < 0)
                    {
                        top = 0;
                    }

                    this.listBoxOcrFixList.TopIndex = top;
                    break;
                }
            }
        }

        /// <summary>
        /// The list box ocr fix list selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxOcrFixListSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemoveOcrFix.Enabled = this.listBoxOcrFixList.SelectedIndex >= 0;
        }

        /// <summary>
        /// The button remove ocr fix click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonRemoveOcrFixClick(object sender, EventArgs e)
        {
            var cb = this.comboBoxWordListLanguage.Items[this.comboBoxWordListLanguage.SelectedIndex] as ComboBoxLanguage;
            if (cb == null)
            {
                return;
            }

            if (this.listBoxOcrFixList.SelectedIndices.Count == 0)
            {
                return;
            }

            int itemsToRemoveCount = this.listBoxOcrFixList.SelectedIndices.Count;

            int index = this.listBoxOcrFixList.SelectedIndex;
            string text = this.listBoxOcrFixList.Items[index].ToString();
            string key = text.Substring(0, text.IndexOf(" --> ", StringComparison.Ordinal));

            if (this._ocrFixReplaceList.WordReplaceList.ContainsKey(key) || this._ocrFixReplaceList.PartialLineWordBoundaryReplaceList.ContainsKey(key))
            {
                DialogResult result;
                if (itemsToRemoveCount == 1)
                {
                    result = MessageBox.Show(string.Format(Configuration.Settings.Language.Settings.RemoveX, text), "Subtitle Edit", MessageBoxButtons.YesNo);
                }
                else
                {
                    result = MessageBox.Show(string.Format(Configuration.Settings.Language.Main.DeleteXLinesPrompt, itemsToRemoveCount), "Subtitle Edit", MessageBoxButtons.YesNo);
                }

                if (result == DialogResult.Yes)
                {
                    for (int idx = this.listBoxOcrFixList.SelectedIndices.Count - 1; idx >= 0; idx--)
                    {
                        index = this.listBoxOcrFixList.SelectedIndices[idx];
                        text = this.listBoxOcrFixList.Items[index].ToString();
                        key = text.Substring(0, text.IndexOf(" --> ", StringComparison.Ordinal)).Trim();

                        if (this._ocrFixReplaceList.WordReplaceList.ContainsKey(key) || this._ocrFixReplaceList.PartialLineWordBoundaryReplaceList.ContainsKey(key))
                        {
                            this._ocrFixReplaceList.RemoveWordOrPartial(key);
                        }

                        this.listBoxOcrFixList.Items.RemoveAt(index);
                    }

                    this.LoadOcrFixList(false);
                    this.buttonRemoveOcrFix.Enabled = false;

                    if (index < this.listBoxOcrFixList.Items.Count)
                    {
                        this.listBoxOcrFixList.SelectedIndex = index;
                    }
                    else if (this.listBoxOcrFixList.Items.Count > 0)
                    {
                        this.listBoxOcrFixList.SelectedIndex = index - 1;
                    }

                    this.listBoxOcrFixList.Focus();
                }
            }
        }

        /// <summary>
        /// The text box ocr fix value key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxOcrFixValueKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.ButtonAddOcrFixClick(null, null);
            }
        }

        /// <summary>
        /// The tab control settings selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TabControlSettingsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelStatus.Text = string.Empty;
        }

        /// <summary>
        /// The list box key down search.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxKeyDownSearch(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.None || e.KeyCode == Keys.F1 || e.KeyCode == Keys.Home || e.KeyCode == Keys.End)
            {
                return;
            }

            if (TimeSpan.FromTicks(this._listBoxSearchStringLastUsed.Ticks).TotalMilliseconds + 1800 < TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds)
            {
                this._listBoxSearchString = string.Empty;
            }

            if (e.KeyCode == Keys.Delete)
            {
                if (this._listBoxSearchString.Length > 0)
                {
                    this._listBoxSearchString = this._listBoxSearchString.Remove(this._listBoxSearchString.Length - 1, 1);
                }
            }
            else
            {
                this._listBoxSearchString += e.KeyCode.ToString();
            }

            this._listBoxSearchStringLastUsed = DateTime.Now;
            this.FindAndSelectListBoxItem(sender as ListBox);
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// The find and select list box item.
        /// </summary>
        /// <param name="listBox">
        /// The list box.
        /// </param>
        private void FindAndSelectListBoxItem(ListBox listBox)
        {
            int i = 0;
            foreach (string s in listBox.Items)
            {
                if (s.StartsWith(this._listBoxSearchString, StringComparison.OrdinalIgnoreCase))
                {
                    listBox.SelectedIndex = i;
                    break;
                }

                i++;
            }
        }

        /// <summary>
        /// The list box search reset.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxSearchReset(object sender, EventArgs e)
        {
            this._listBoxSearchString = string.Empty;
        }

        /// <summary>
        /// The combo box custom search_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxCustomSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            TextBox tb = this.textBoxCustomSearchUrl1;
            if (cb == this.comboBoxCustomSearch2)
            {
                tb = this.textBoxCustomSearchUrl2;
            }
            else if (cb == this.comboBoxCustomSearch3)
            {
                tb = this.textBoxCustomSearchUrl3;
            }
            else if (cb == this.comboBoxCustomSearch4)
            {
                tb = this.textBoxCustomSearchUrl4;
            }
            else if (cb == this.comboBoxCustomSearch5)
            {
                tb = this.textBoxCustomSearchUrl5;
            }
            else if (cb == this.comboBoxCustomSearch6)
            {
                tb = this.textBoxCustomSearchUrl6;
            }

            if (cb.SelectedIndex >= 0)
            {
                if (cb.SelectedIndex == 0)
                {
                    tb.Text = "http://dictionary.reference.com/browse/{0}";
                }
                else if (cb.SelectedIndex == 1)
                {
                    tb.Text = "http://www.learnersdictionary.com/search/{0}";
                }
                else if (cb.SelectedIndex == 2)
                {
                    tb.Text = "http://www.merriam-webster.com/dictionary/{0}";
                }
                else if (cb.SelectedIndex == 3)
                {
                    tb.Text = "http://www.thefreedictionary.com/{0}";
                }
                else if (cb.SelectedIndex == 4)
                {
                    tb.Text = "http://thesaurus.com/browse/{0}";
                }
                else if (cb.SelectedIndex == 5)
                {
                    tb.Text = "http://www.urbandictionary.com/define.php?term={0}";
                }
                else if (cb.SelectedIndex == 6)
                {
                    tb.Text = "http://www.visuwords.com/?word={0}";
                }
                else if (cb.SelectedIndex == 7)
                {
                    tb.Text = "http://en.m.wikipedia.org/wiki?search={0}";
                }
            }
        }

        /// <summary>
        /// The button waveform selected color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformSelectedColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelWaveformSelectedColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelWaveformSelectedColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The button waveform color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelWaveformColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelWaveformColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The button waveform background color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformBackgroundColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelWaveformBackgroundColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelWaveformBackgroundColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The button waveform grid color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformGridColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelWaveformGridColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelWaveformGridColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The button waveform text color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformTextColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelWaveformTextColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelWaveformTextColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The button waveforms folder empty_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformsFolderEmpty_Click(object sender, EventArgs e)
        {
            string waveformsFolder = Configuration.WaveformsFolder.TrimEnd(Path.DirectorySeparatorChar);
            if (Directory.Exists(waveformsFolder))
            {
                var di = new DirectoryInfo(waveformsFolder);

                foreach (FileInfo fileName in di.GetFiles("*.wav"))
                {
                    try
                    {
                        File.Delete(fileName.FullName);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }

            string spectrogramsFolder = Configuration.SpectrogramsFolder.TrimEnd(Path.DirectorySeparatorChar);
            if (Directory.Exists(spectrogramsFolder))
            {
                var di = new DirectoryInfo(spectrogramsFolder);

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    var spectrogramDir = new DirectoryInfo(dir.FullName);
                    foreach (FileInfo fileName in spectrogramDir.GetFiles("*.gif"))
                    {
                        File.Delete(fileName.FullName);
                    }

                    string imageDbFileName = Path.Combine(dir.FullName, "Images.db");
                    if (File.Exists(imageDbFileName))
                    {
                        File.Delete(imageDbFileName);
                    }

                    string xmlFileName = Path.Combine(dir.FullName, "Info.xml");
                    if (File.Exists(xmlFileName))
                    {
                        File.Delete(xmlFileName);
                    }

                    Directory.Delete(dir.FullName);
                }
            }

            this.InitializeWaveformsAndSpectrogramsFolderEmpty(Configuration.Settings.Language.Settings);
        }

        /// <summary>
        /// The check box remember recent files_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxRememberRecentFiles_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxReopenLastOpened.Enabled = this.checkBoxRememberRecentFiles.Checked;
            this.checkBoxRememberSelectedLine.Enabled = this.checkBoxRememberRecentFiles.Checked;
        }

        /// <summary>
        /// The button waveform selected color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonWaveformSelectedColor_Click(object sender, MouseEventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelWaveformSelectedColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelWaveformSelectedColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The panel subtitle font color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelSubtitleFontColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelSubtitleFontColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelSubtitleFontColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The panel subtitle background color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelSubtitleBackgroundColor_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelSubtitleBackgroundColor.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelSubtitleBackgroundColor.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The tree view shortcuts_ after select.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void treeViewShortcuts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Nodes.Count > 0)
            {
                this.checkBoxShortcutsControl.Checked = false;
                this.checkBoxShortcutsControl.Enabled = false;
                this.checkBoxShortcutsAlt.Checked = false;
                this.checkBoxShortcutsAlt.Enabled = false;
                this.checkBoxShortcutsShift.Checked = false;
                this.checkBoxShortcutsShift.Enabled = false;
                this.comboBoxShortcutKey.SelectedIndex = 0;
                this.comboBoxShortcutKey.Enabled = false;
                this.buttonUpdateShortcut.Enabled = false;
            }
            else if (e.Node != null || e.Node.Nodes.Count == 0)
            {
                this.checkBoxShortcutsControl.Enabled = true;
                this.checkBoxShortcutsAlt.Enabled = true;
                this.checkBoxShortcutsShift.Enabled = true;

                this.checkBoxShortcutsControl.Checked = false;
                this.checkBoxShortcutsAlt.Checked = false;
                this.checkBoxShortcutsShift.Checked = false;

                this.comboBoxShortcutKey.SelectedIndex = 0;

                this.comboBoxShortcutKey.Enabled = true;
                this.buttonUpdateShortcut.Enabled = true;

                string shortcut = GetShortcut(e.Node.Text);

                string[] parts = shortcut.ToLower().Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string k in parts)
                {
                    if (k.Equals("CONTROL", StringComparison.OrdinalIgnoreCase))
                    {
                        this.checkBoxShortcutsControl.Checked = true;
                    }
                    else if (k.Equals("ALT", StringComparison.OrdinalIgnoreCase))
                    {
                        this.checkBoxShortcutsAlt.Checked = true;
                    }
                    else if (k.Equals("SHIFT", StringComparison.OrdinalIgnoreCase))
                    {
                        this.checkBoxShortcutsShift.Checked = true;
                    }
                    else
                    {
                        int i = 0;
                        foreach (string value in this.comboBoxShortcutKey.Items)
                        {
                            if (value.Equals(k, StringComparison.OrdinalIgnoreCase))
                            {
                                this.comboBoxShortcutKey.SelectedIndex = i;
                                break;
                            }

                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The get shortcut.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetShortcut(string text)
        {
            string shortcut = text.Substring(text.IndexOf('['));
            shortcut = shortcut.TrimEnd(']').TrimStart('[');
            if (shortcut == Configuration.Settings.Language.General.None)
            {
                return string.Empty;
            }

            return shortcut;
        }

        /// <summary>
        /// The button update shortcut_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonUpdateShortcut_Click(object sender, EventArgs e)
        {
            if (this.treeViewShortcuts.SelectedNode != null && this.treeViewShortcuts.SelectedNode.Text.Contains('['))
            {
                string text = this.treeViewShortcuts.SelectedNode.Text.Substring(0, this.treeViewShortcuts.SelectedNode.Text.IndexOf('[')).Trim();

                if (this.comboBoxShortcutKey.SelectedIndex == 0)
                {
                    this.treeViewShortcuts.SelectedNode.Text = text + " [" + Configuration.Settings.Language.General.None + "]";
                    return;
                }

                var sb = new StringBuilder(@"[");
                if (this.checkBoxShortcutsControl.Checked)
                {
                    sb.Append("Control+");
                }

                if (this.checkBoxShortcutsAlt.Checked)
                {
                    sb.Append("Alt+");
                }

                if (this.checkBoxShortcutsShift.Checked)
                {
                    sb.Append("Shift+");
                }

                sb.Append(this.comboBoxShortcutKey.Items[this.comboBoxShortcutKey.SelectedIndex]);
                sb.Append(']');

                if (sb.Length < 3 || sb.ToString().EndsWith("+]"))
                {
                    MessageBox.Show(string.Format(Configuration.Settings.Language.Settings.ShortcutIsNotValid, sb));
                    return;
                }

                if (sb.ToString() == "[CapsLock]")
                {
                    MessageBox.Show(string.Format(Configuration.Settings.Language.Settings.ShortcutIsNotValid, sb));
                    return;
                }

                this.treeViewShortcuts.SelectedNode.Text = text + " " + sb;

                var existsIn = new StringBuilder();
                foreach (TreeNode node in this.treeViewShortcuts.Nodes)
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if (subNode.Text.Contains(sb.ToString()) && this.treeViewShortcuts.SelectedNode.Text != subNode.Text)
                        {
                            existsIn.AppendLine(string.Format(Configuration.Settings.Language.Settings.ShortcutIsAlreadyDefinedX, node.Text + " -> " + subNode.Text));
                        }
                    }
                }

                if (existsIn.Length > 0)
                {
                    MessageBox.Show(existsIn.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// The button list view syntax color error_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonListViewSyntaxColorError_Click(object sender, EventArgs e)
        {
            this.colorDialogSSAStyle.Color = this.panelListViewSyntaxColorError.BackColor;
            if (this.colorDialogSSAStyle.ShowDialog() == DialogResult.OK)
            {
                this.panelListViewSyntaxColorError.BackColor = this.colorDialogSSAStyle.Color;
            }
        }

        /// <summary>
        /// The combo box shortcut key_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxShortcutKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Enter || e.KeyCode == Keys.None)
            {
                return;
            }

            int i = 0;
            foreach (var item in this.comboBoxShortcutKey.Items)
            {
                if (item.ToString() == e.KeyCode.ToString())
                {
                    this.comboBoxShortcutKey.SelectedIndex = i;
                    e.SuppressKeyPress = true;
                    return;
                }

                i++;
            }
        }

        /// <summary>
        /// The numeric up down ssa outline_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownSsaOutline_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateSsaExample();
        }

        /// <summary>
        /// The numeric up down ssa shadow_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownSsaShadow_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateSsaExample();
        }

        /// <summary>
        /// The check box ssa opaque box_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxSsaOpaqueBox_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownSsaOutline.Enabled = !this.checkBoxSsaOpaqueBox.Checked;
            this.numericUpDownSsaShadow.Enabled = !this.checkBoxSsaOpaqueBox.Checked;
            this.UpdateSsaExample();
        }

        /// <summary>
        /// The button browse to f fmpeg_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonBrowseToFFmpeg_Click(object sender, EventArgs e)
        {
            this.openFileDialogFFmpeg.FileName = string.Empty;
            this.openFileDialogFFmpeg.Title = Configuration.Settings.Language.Settings.WaveformBrowseToFFmpeg;
            if (!Configuration.IsRunningOnLinux() && !Configuration.IsRunningOnMac())
            {
                this.openFileDialogFFmpeg.Filter = "FFmpeg.exe|FFmpeg.exe";
            }

            if (this.openFileDialogFFmpeg.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxFFmpegPath.Text = this.openFileDialogFFmpeg.FileName;
            }
        }

        /// <summary>
        /// The check box waveform hover focus_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxWaveformHoverFocus_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxListViewMouseEnterFocus.Enabled = this.checkBoxWaveformHoverFocus.Checked;
        }

        /// <summary>
        /// The button vlc path browse_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonVlcPathBrowse_Click(object sender, EventArgs e)
        {
            this.openFileDialogFFmpeg.FileName = string.Empty;
            this.openFileDialogFFmpeg.Title = Configuration.Settings.Language.Settings.WaveformBrowseToVLC;
            if (!Configuration.IsRunningOnLinux() && !Configuration.IsRunningOnMac())
            {
                this.openFileDialogFFmpeg.Filter = "vlc.exe|vlc.exe";
            }

            if (this.openFileDialogFFmpeg.ShowDialog(this) == DialogResult.OK)
            {
                this.EnableVlc(this.openFileDialogFFmpeg.FileName);
            }
        }

        /// <summary>
        /// The enable vlc.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void EnableVlc(string fileName)
        {
            this.textBoxVlcPath.Text = Path.GetDirectoryName(fileName);
            Configuration.Settings.General.VlcLocation = this.textBoxVlcPath.Text;
            Configuration.Settings.General.VlcLocationRelative = GetRelativePath(this.textBoxVlcPath.Text);
            this.radioButtonVideoPlayerVLC.Enabled = LibVlcDynamic.IsInstalled;
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
            Configuration.Settings.General.VlcLocation = this._oldVlcLocation;
            Configuration.Settings.General.VlcLocationRelative = this._oldVlcLocationRelative;

            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The button edit do not break after list_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonEditDoNotBreakAfterList_Click(object sender, EventArgs e)
        {
            using (var form = new DoNotBreakAfterListEdit())
            {
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// The link label open dictionary folder_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabelOpenDictionaryFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string dictionaryFolder = Utilities.DictionaryFolder;
            if (!Directory.Exists(dictionaryFolder))
            {
                Directory.CreateDirectory(dictionaryFolder);
            }

            System.Diagnostics.Process.Start(dictionaryFolder);
        }

        /// <summary>
        /// The text box vlc path_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxVlcPath_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                var path = this.textBoxVlcPath.Text.Trim('\"');
                if (path.Length > 3 && Path.IsPathRooted(path) && Path.GetFileName(path).Equals("vlc.exe", StringComparison.OrdinalIgnoreCase) && File.Exists(path))
                {
                    this.EnableVlc(path);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The combo box language.
        /// </summary>
        private class ComboBoxLanguage
        {
            /// <summary>
            /// Gets or sets the culture info.
            /// </summary>
            public CultureInfo CultureInfo { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.CultureInfo.NativeName;
            }
        }
    }
}