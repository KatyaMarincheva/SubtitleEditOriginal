// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubOcr.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub ocr.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.BluRaySup;
    using Nikse.SubtitleEdit.Logic.Ocr;
    using Nikse.SubtitleEdit.Logic.Ocr.Binary;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;
    using Nikse.SubtitleEdit.Logic.TransportStream;
    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    /// The vob sub ocr.
    /// </summary>
    public sealed partial class VobSubOcr : PositionAndSizeForm
    {
        /// <summary>
        /// The nocr min color.
        /// </summary>
        public const int NocrMinColor = 300;

        /// <summary>
        /// The _italic shortcut.
        /// </summary>
        private readonly Keys _italicShortcut = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainTextBoxItalic);

        /// <summary>
        /// The _main general go to next subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToNextSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle);

        /// <summary>
        /// The _main general go to prev subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToPrevSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle);

        /// <summary>
        /// The _abort.
        /// </summary>
        private volatile bool _abort;

        /// <summary>
        /// The _bdn file name.
        /// </summary>
        private string _bdnFileName;

        /// <summary>
        /// The _bdn xml original.
        /// </summary>
        private Subtitle _bdnXmlOriginal;

        /// <summary>
        /// The _bdn xml subtitle.
        /// </summary>
        private Subtitle _bdnXmlSubtitle;

        /// <summary>
        /// The _binary ocr db.
        /// </summary>
        private BinaryOcrDb _binaryOcrDb;

        /// <summary>
        /// The _binary ocr db file name.
        /// </summary>
        private string _binaryOcrDbFileName;

        /// <summary>
        /// The _bin ocr last lowercase height.
        /// </summary>
        private int _binOcrLastLowercaseHeight = -1;

        /// <summary>
        /// The _bin ocr last uppercase height.
        /// </summary>
        private int _binOcrLastUppercaseHeight = -1;

        /// <summary>
        /// The _blu ray subtitles.
        /// </summary>
        private List<BluRaySupParser.PcsData> _bluRaySubtitles;

        // Blu-ray sup
        /// <summary>
        /// The _blu ray subtitles original.
        /// </summary>
        private List<BluRaySupParser.PcsData> _bluRaySubtitlesOriginal;

        /// <summary>
        /// The _compare bitmaps.
        /// </summary>
        private List<CompareItem> _compareBitmaps;

        /// <summary>
        /// The _compare doc.
        /// </summary>
        private XmlDocument _compareDoc = new XmlDocument();

        /// <summary>
        /// The _dvb sub color.
        /// </summary>
        private Color _dvbSubColor = Color.Transparent;

        // DVB (from transport stream)
        /// <summary>
        /// The _dvb subtitles.
        /// </summary>
        private List<TransportStreamSubtitle> _dvbSubtitles;

        /// <summary>
        /// The _ic thread results.
        /// </summary>
        private string[] _icThreadResults;

        /// <summary>
        /// The _ic threads stop.
        /// </summary>
        private bool _icThreadsStop;

        /// <summary>
        /// The _import language string.
        /// </summary>
        private string _importLanguageString;

        /// <summary>
        /// The _is son.
        /// </summary>
        private bool _isSon;

        /// <summary>
        /// The _italic checked last.
        /// </summary>
        private bool _italicCheckedLast;

        /// <summary>
        /// The _language id.
        /// </summary>
        private string _languageId;

        /// <summary>
        /// The _last additions.
        /// </summary>
        private List<ImageCompareAddition> _lastAdditions = new List<ImageCompareAddition>();

        /// <summary>
        /// The _last line.
        /// </summary>
        private string _lastLine;

        /// <summary>
        /// The _lines ocred.
        /// </summary>
        private int _linesOcred = 0;

        /// <summary>
        /// The _main.
        /// </summary>
        private Main _main;

        /// <summary>
        /// The _main ocr bitmap.
        /// </summary>
        private Bitmap _mainOcrBitmap;

        /// <summary>
        /// The _main ocr index.
        /// </summary>
        private int _mainOcrIndex;

        /// <summary>
        /// The _main ocr running.
        /// </summary>
        private bool _mainOcrRunning;

        /// <summary>
        /// The _main ocr timer.
        /// </summary>
        private Timer _mainOcrTimer;

        /// <summary>
        /// The _main ocr timer max.
        /// </summary>
        private int _mainOcrTimerMax;

        /// <summary>
        /// The _manual ocr dialog position.
        /// </summary>
        private Point _manualOcrDialogPosition = new Point(-1, -1);

        /// <summary>
        /// The _modi doc.
        /// </summary>
        private object _modiDoc;

        /// <summary>
        /// The _modi enabled.
        /// </summary>
        private bool _modiEnabled;

        /// <summary>
        /// The _modi type.
        /// </summary>
        private Type _modiType;

        // SP vobsub list (mp4)
        /// <summary>
        /// The _mp 4 list.
        /// </summary>
        private List<SubPicturesWithSeparateTimeCodes> _mp4List;

        // List<NOcrChar> _nocrChars = null;
        /// <summary>
        /// The _n ocr db.
        /// </summary>
        private NOcrDb _nOcrDb;

        /// <summary>
        /// The _nocr last lowercase height.
        /// </summary>
        private int _nocrLastLowercaseHeight = -1;

        /// <summary>
        /// The _nocr last uppercase height.
        /// </summary>
        private int _nocrLastUppercaseHeight = -1;

        /// <summary>
        /// The _nocr thread results.
        /// </summary>
        private string[] _nocrThreadResults;

        /// <summary>
        /// The _nocr threads stop.
        /// </summary>
        private bool _nocrThreadsStop;

        // Dictionaries/spellchecking/fixing
        /// <summary>
        /// The _ocr fix engine.
        /// </summary>
        private OcrFixEngine _ocrFixEngine;

        /// <summary>
        /// The _palette.
        /// </summary>
        private List<Color> _palette;

        /// <summary>
        /// The _selected index.
        /// </summary>
        private int _selectedIndex = -1;

        // SP list
        /// <summary>
        /// The _sp list.
        /// </summary>
        private List<SpHeader> _spList;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle = new Subtitle();

        /// <summary>
        /// The _tesseract async index.
        /// </summary>
        private int _tesseractAsyncIndex;

        /// <summary>
        /// The _tesseract async strings.
        /// </summary>
        private string[] _tesseractAsyncStrings;

        /// <summary>
        /// The _tesseract ocr auto fixes.
        /// </summary>
        private int _tesseractOcrAutoFixes;

        /// <summary>
        /// The _tesseract thread.
        /// </summary>
        private BackgroundWorker _tesseractThread;

        /// <summary>
        /// The _un italic factor.
        /// </summary>
        private double _unItalicFactor = 0.33;

        /// <summary>
        /// The _vob sub merged packist.
        /// </summary>
        private List<VobSubMergedPack> _vobSubMergedPackist;

        // DVD rip/vobsub
        /// <summary>
        /// The _vob sub merged packist original.
        /// </summary>
        private List<VobSubMergedPack> _vobSubMergedPackistOriginal;

        /// <summary>
        /// The _vob sub ocr character.
        /// </summary>
        private VobSubOcrCharacter _vobSubOcrCharacter = new VobSubOcrCharacter();

        /// <summary>
        /// The _vob sub ocr n ocr character.
        /// </summary>
        private VobSubOcrNOcrCharacter _vobSubOcrNOcrCharacter = new VobSubOcrNOcrCharacter();

        /// <summary>
        /// The _vob sub ocr settings.
        /// </summary>
        private VobSubOcrSettings _vobSubOcrSettings;

        /// <summary>
        /// The _window start time.
        /// </summary>
        private DateTime _windowStartTime = DateTime.Now;

        // XSub (divx)
        /// <summary>
        /// The _x sub list.
        /// </summary>
        private List<XSub> _xSubList;

        /// <summary>
        /// The ok clicked.
        /// </summary>
        private bool OkClicked = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcr"/> class.
        /// </summary>
        public VobSubOcr()
        {
            this.InitializeComponent();

            // this.DoubleBuffered = true;
            SetDoubleBuffered(this.subtitleListView1);

            var language = Configuration.Settings.Language.VobSubOcr;
            this.Text = language.Title;
            this.groupBoxOcrMethod.Text = language.OcrMethod;
            this.labelTesseractLanguage.Text = language.Language;
            this.labelImageDatabase.Text = language.ImageDatabase;
            this.labelNoOfPixelsIsSpace.Text = language.NoOfPixelsIsSpace;
            this.labelMaxErrorPercent.Text = language.MaxErrorPercent;
            this.buttonNewCharacterDatabase.Text = language.New;
            this.buttonEditCharacterDatabase.Text = language.Edit;
            this.buttonStartOcr.Text = language.StartOcr;
            this.buttonStop.Text = language.Stop;
            this.labelStartFrom.Text = language.StartOcrFrom;
            this.labelStatus.Text = language.LoadingVobSubImages;
            this.groupBoxSubtitleImage.Text = language.SubtitleImage;
            this.labelSubtitleText.Text = language.SubtitleText;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.subtitleListView1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.subtitleListView1.Columns[0].Width = 45;
            this.subtitleListView1.Columns[1].Width = 90;
            this.subtitleListView1.Columns[2].Width = 90;
            this.subtitleListView1.Columns[3].Width = 70;
            this.subtitleListView1.Columns[4].Width = 150;
            this.subtitleListView1.InitializeTimestampColumnWidths(this);

            this.groupBoxImagePalette.Text = language.ImagePalette;
            this.checkBoxCustomFourColors.Text = language.UseCustomColors;
            this.checkBoxBackgroundTransparent.Text = language.Transparent;
            this.labelMinAlpha.Text = language.TransparentMinAlpha;
            this.checkBoxPatternTransparent.Text = language.Transparent;
            this.checkBoxEmphasis1Transparent.Text = language.Transparent;
            this.checkBoxEmphasis2Transparent.Text = language.Transparent;
            this.checkBoxAutoTransparentBackground.Text = language.AutoTransparentBackground;
            this.checkBoxAutoTransparentBackground.Left = this.groupBoxSubtitleImage.Width - this.checkBoxAutoTransparentBackground.Width - 2;
            this.checkBoxPromptForUnknownWords.Text = language.PromptForUnknownWords;
            this.checkBoxPromptForUnknownWords.Checked = Configuration.Settings.VobSubOcr.PromptForUnknownWords;
            this.checkBoxGuessUnknownWords.Checked = Configuration.Settings.VobSubOcr.GuessUnknownWords;

            this.groupBoxTransportStream.Text = language.TransportStream;
            this.checkBoxTransportStreamGrayscale.Text = language.TransportStreamGrayscale;
            this.checkBoxTransportStreamGetColorAndSplit.Text = language.TransportStreamGetColor;
            this.checkBoxTransportStreamGetColorAndSplit.Left = this.checkBoxTransportStreamGrayscale.Left + this.checkBoxTransportStreamGrayscale.Width + 9;

            this.groupBoxOcrAutoFix.Text = language.OcrAutoCorrectionSpellChecking;
            this.checkBoxGuessUnknownWords.Text = language.TryToGuessUnkownWords;
            this.checkBoxAutoBreakLines.Text = language.AutoBreakSubtitleIfMoreThanTwoLines;
            this.checkBoxAutoBreakLines.Checked = Configuration.Settings.VobSubOcr.AutoBreakSubtitleIfMoreThanTwoLines;
            this.tabControlLogs.TabPages[0].Text = language.AllFixes;
            this.tabControlLogs.TabPages[1].Text = language.GuessesUsed;
            this.tabControlLogs.TabPages[2].Text = language.UnknownWords;

            this.buttonUknownToNames.Text = Configuration.Settings.Language.SpellCheck.AddToNamesAndIgnoreList;
            this.buttonUknownToUserDic.Text = Configuration.Settings.Language.SpellCheck.AddToUserDictionary;
            this.buttonAddToOcrReplaceList.Text = Configuration.Settings.Language.SpellCheck.AddToOcrReplaceList;
            this.buttonGoogleIt.Text = Configuration.Settings.Language.Main.VideoControls.GoogleIt;

            this.numericUpDownPixelsIsSpace.Left = this.labelNoOfPixelsIsSpace.Left + this.labelNoOfPixelsIsSpace.Width + 5;
            this.numericUpDownMaxErrorPct.Left = this.numericUpDownPixelsIsSpace.Left;
            this.groupBoxSubtitleImage.Text = string.Empty;
            this.labelFixesMade.Text = string.Empty;
            this.labelFixesMade.Left = this.checkBoxAutoFixCommonErrors.Left + this.checkBoxAutoFixCommonErrors.Width;

            this.labelDictionaryLoaded.Text = string.Format(language.DictionaryX, string.Empty);
            this.comboBoxDictionaries.Left = this.labelDictionaryLoaded.Left + this.labelDictionaryLoaded.Width;

            this.groupBoxImageCompareMethod.Text = string.Empty; // language.OcrViaImageCompare;
            this.groupBoxModiMethod.Text = string.Empty; // language.OcrViaModi;
            this.GroupBoxTesseractMethod.Text = string.Empty;

            this.checkBoxAutoFixCommonErrors.Text = language.FixOcrErrors;
            this.checkBoxRightToLeft.Text = language.RightToLeft;
            this.checkBoxRightToLeft.Left = this.numericUpDownPixelsIsSpace.Left;
            this.groupBoxOCRControls.Text = string.Empty;

            this.FillSpellCheckDictionaries();

            this.comboBoxOcrMethod.Items.Clear();
            this.comboBoxOcrMethod.Items.Add(language.OcrViaTesseract);
            this.comboBoxOcrMethod.Items.Add(language.OcrViaImageCompare);
            this.comboBoxOcrMethod.Items.Add(language.OcrViaModi);
            if (Configuration.Settings.General.ShowBetaStuff)
            {
                this.comboBoxOcrMethod.Items.Add(language.OcrViaNOCR);
                this.comboBoxOcrMethod.Items.Add(language.OcrViaImageCompare + " NEW! ");
                this.comboBoxOcrMethod.SelectedIndex = 4;
            }

            this.checkBoxUseModiInTesseractForUnknownWords.Text = language.TryModiForUnknownWords;
            this.checkBoxTesseractItalicsOn.Checked = Configuration.Settings.VobSubOcr.UseItalicsInTesseract;
            this.checkBoxTesseractItalicsOn.Text = Configuration.Settings.Language.General.Italic;

            this.checkBoxTesseractMusicOn.Checked = Configuration.Settings.VobSubOcr.UseMusicSymbolsInTesseract;
            this.checkBoxTesseractMusicOn.Text = Configuration.Settings.Language.Settings.MusicSymbol;
            this.checkBoxTesseractMusicOn.Left = this.checkBoxTesseractItalicsOn.Left + this.checkBoxTesseractItalicsOn.Width + 15;

            if (Configuration.Settings.VobSubOcr.ItalicFactor >= 0.1 && Configuration.Settings.VobSubOcr.ItalicFactor < 1)
            {
                this._unItalicFactor = Configuration.Settings.VobSubOcr.ItalicFactor;
            }

            this.checkBoxShowOnlyForced.Text = language.ShowOnlyForcedSubtitles;
            this.checkBoxUseTimeCodesFromIdx.Text = language.UseTimeCodesFromIdx;

            this.normalToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.ContextMenu.Normal;
            this.italicToolStripMenuItem.Text = Configuration.Settings.Language.General.Italic;
            this.importTextWithMatchingTimeCodesToolStripMenuItem.Text = language.ImportTextWithMatchingTimeCodes;
            this.importNewTimeCodesToolStripMenuItem.Text = language.ImportNewTimeCodes;
            this.saveImageAsToolStripMenuItem.Text = language.SaveSubtitleImageAs;
            this.toolStripMenuItemImageSaveAs.Text = language.SaveSubtitleImageAs;
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem.Text = language.SaveAllSubtitleImagesWithHtml;
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Text = language.InspectCompareMatchesForCurrentImage;
            this.EditLastAdditionsToolStripMenuItem.Text = language.EditLastAdditions;
            this.checkBoxRightToLeft.Checked = Configuration.Settings.VobSubOcr.RightToLeft;
            this.toolStripMenuItemSetUnItalicFactor.Text = language.SetUnitalicFactor;
            this.deleteToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.ContextMenu.Delete;

            this.toolStripMenuItemExport.Text = Configuration.Settings.Language.Main.Menu.File.Export;
            this.vobSubToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.File.ExportVobSub;
            this.bDNXMLToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.File.ExportBdnXml;
            this.bluraySupToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.File.ExportBluRaySup;

            this.toolStripMenuItemClearFixes.Text = Configuration.Settings.Language.DvdSubRip.Clear;
            this.toolStripMenuItemClearGuesses.Text = Configuration.Settings.Language.DvdSubRip.Clear;
            this.clearToolStripMenuItem.Text = Configuration.Settings.Language.DvdSubRip.Clear;

            this.checkBoxNOcrCorrect.Checked = Configuration.Settings.VobSubOcr.LineOcrDraw;
            this.checkBoxNOcrItalic.Checked = Configuration.Settings.VobSubOcr.LineOcrAdvancedItalic;

            this.comboBoxTesseractLanguages.Left = this.labelTesseractLanguage.Left + this.labelTesseractLanguage.Width;
            this.buttonGetTesseractDictionaries.Left = this.comboBoxTesseractLanguages.Left + this.comboBoxTesseractLanguages.Width + 5;

            Utilities.InitializeSubtitleFont(this.subtitleListView1);
            this.subtitleListView1.AutoSizeAllColumns(this);

            Utilities.InitializeSubtitleFont(this.textBoxCurrentText);

            this.italicToolStripMenuItem.ShortcutKeys = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainListViewItalic);

            this.comboBoxTesseractLanguages.Left = this.labelTesseractLanguage.Left + this.labelTesseractLanguage.Width + 3;
            this.comboBoxModiLanguage.Left = this.label1.Left + this.label1.Width + 3;

            this.comboBoxCharacterDatabase.Left = this.labelImageDatabase.Left + this.labelImageDatabase.Width + 3;
            this.buttonNewCharacterDatabase.Left = this.comboBoxCharacterDatabase.Left + this.comboBoxCharacterDatabase.Width + 3;
            this.buttonEditCharacterDatabase.Left = this.buttonNewCharacterDatabase.Left;
            this.numericUpDownPixelsIsSpace.Left = this.labelNoOfPixelsIsSpace.Left + this.labelNoOfPixelsIsSpace.Width + 3;
            this.checkBoxRightToLeft.Left = this.numericUpDownPixelsIsSpace.Left;

            Utilities.FixLargeFonts(this, this.buttonCancel);
            this.buttonEditCharacterDatabase.Top = this.buttonNewCharacterDatabase.Top + this.buttonNewCharacterDatabase.Height + 3;

            this.splitContainerBottom.Panel1MinSize = 400;
            this.splitContainerBottom.Panel2MinSize = 250;

            this.pictureBoxBackground.Left = this.checkBoxCustomFourColors.Left + this.checkBoxCustomFourColors.Width + 8;
            this.checkBoxBackgroundTransparent.Left = this.pictureBoxBackground.Left + this.pictureBoxBackground.Width + 3;
            this.pictureBoxPattern.Left = this.checkBoxBackgroundTransparent.Left + this.checkBoxBackgroundTransparent.Width + 8;
            this.checkBoxPatternTransparent.Left = this.pictureBoxPattern.Left + this.pictureBoxPattern.Width + 3;
            this.pictureBoxEmphasis1.Left = this.checkBoxPatternTransparent.Left + this.checkBoxPatternTransparent.Width + 8;
            this.checkBoxEmphasis1Transparent.Left = this.pictureBoxEmphasis1.Left + this.pictureBoxEmphasis1.Width + 3;
            this.pictureBoxEmphasis2.Left = this.checkBoxEmphasis1Transparent.Left + this.checkBoxEmphasis1Transparent.Width + 8;
            this.checkBoxEmphasis2Transparent.Left = this.pictureBoxEmphasis2.Left + this.pictureBoxEmphasis2.Width + 3;

            try
            {
                this.numericUpDownMaxErrorPct.Value = (decimal)Configuration.Settings.VobSubOcr.AllowDifferenceInPercent;
            }
            catch
            {
                this.numericUpDownMaxErrorPct.Value = 1.1m;
            }
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the subtitle from ocr.
        /// </summary>
        public Subtitle SubtitleFromOcr
        {
            get
            {
                return this._subtitle;
            }
        }

        /// <summary>
        /// Gets the language string.
        /// </summary>
        public string LanguageString
        {
            get
            {
                if (this.comboBoxDictionaries.SelectedItem == null)
                {
                    return null;
                }

                string name = this.comboBoxDictionaries.SelectedItem.ToString();
                int start = name.LastIndexOf('[');
                int end = name.LastIndexOf(']');
                if (start >= 0 && end > start)
                {
                    start++;
                    name = name.Substring(start, end - start);
                    return name;
                }

                return null;
            }
        }

        /// <summary>
        /// The set double buffered.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public static void SetDoubleBuffered(Control c)
        {
            // Taxes: Remote Desktop Connection and painting http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (SystemInformation.TerminalServerSession)
            {
                return;
            }

            PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        /// <summary>
        /// The fill spell check dictionaries.
        /// </summary>
        private void FillSpellCheckDictionaries()
        {
            this.comboBoxDictionaries.SelectedIndexChanged -= this.comboBoxDictionaries_SelectedIndexChanged;
            this.comboBoxDictionaries.Items.Clear();
            this.comboBoxDictionaries.Items.Add(Configuration.Settings.Language.General.None);
            foreach (string name in Utilities.GetDictionaryLanguages())
            {
                this.comboBoxDictionaries.Items.Add(name);
            }

            this.comboBoxDictionaries.SelectedIndexChanged += this.comboBoxDictionaries_SelectedIndexChanged;
        }

        /// <summary>
        /// The initialize batch.
        /// </summary>
        /// <param name="vobSubFileName">
        /// The vob sub file name.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        internal void InitializeBatch(string vobSubFileName, VobSubOcrSettings vobSubOcrSettings)
        {
            this.Initialize(vobSubFileName, vobSubOcrSettings, null);
            this.FormVobSubOcr_Shown(null, null);
            this.checkBoxPromptForUnknownWords.Checked = false;

            int max = this.GetSubtitleCount();
            if (this.comboBoxOcrMethod.SelectedIndex == 0 && this._tesseractAsyncStrings == null)
            {
                this._tesseractAsyncStrings = new string[max];
                this._tesseractAsyncIndex = (int)this.numericUpDownStartNumber.Value + 5;
                this._tesseractThread = new BackgroundWorker();
                this._tesseractThread.DoWork += this.TesseractThreadDoWork;
                this._tesseractThread.RunWorkerCompleted += this.TesseractThreadRunWorkerCompleted;
                this._tesseractThread.WorkerSupportsCancellation = true;
                if (this._tesseractAsyncIndex >= 0 && this._tesseractAsyncIndex < max)
                {
                    this._tesseractThread.RunWorkerAsync(this.GetSubtitleBitmap(this._tesseractAsyncIndex));
                }
            }

            System.Threading.Thread.Sleep(1000);
            this.subtitleListView1.SelectedIndexChanged -= this.SubtitleListView1SelectedIndexChanged;
            for (int i = 0; i < max; i++)
            {
                Application.DoEvents();
                if (this._abort)
                {
                    this.SetButtonsEnabledAfterOcrDone();
                    return;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(i);

                string text = this.OcrViaTesseract(this.GetSubtitleBitmap(i), i);

                this._lastLine = text;

                text = text.Replace("<i>-</i>", "-");
                text = text.Replace("<i>a</i>", "a");
                text = text.Replace("<i>.</i>", ".");
                text = text.Replace("  ", " ");
                text = text.Trim();

                text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                text = text.Replace(Environment.NewLine + " ", Environment.NewLine);

                // max allow 2 lines
                if (this.checkBoxAutoBreakLines.Checked && text.Replace(Environment.NewLine, "*").Length + 2 <= text.Length)
                {
                    text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                    text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    while (text.Contains(Environment.NewLine + Environment.NewLine))
                    {
                        text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    }

                    if (text.Replace(Environment.NewLine, "*").Length + 2 <= text.Length)
                    {
                        text = Utilities.AutoBreakLine(text);
                    }
                }

                Application.DoEvents();
                if (this._abort)
                {
                    this.textBoxCurrentText.Text = text;
                    this.SetButtonsEnabledAfterOcrDone();
                    return;
                }

                text = text.Trim();
                text = text.Replace("  ", " ");
                text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                text = text.Replace("  ", " ");
                text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

                Paragraph p = this._subtitle.GetParagraphOrDefault(i);
                if (p != null)
                {
                    p.Text = text;
                }

                if (this.subtitleListView1.SelectedItems.Count == 1 && this.subtitleListView1.SelectedItems[0].Index == i)
                {
                    this.textBoxCurrentText.Text = text;
                }
                else
                {
                    this.subtitleListView1.SetText(i, text);
                }
            }

            this.SetButtonsEnabledAfterOcrDone();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="vobSubFileName">
        /// The vob sub file name.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="main">
        /// The main.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool Initialize(string vobSubFileName, VobSubOcrSettings vobSubOcrSettings, Main main)
        {
            this._main = main;
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this.SetOcrMethod();

            this.FileName = vobSubFileName;
            this.Text += " - " + Path.GetFileName(this.FileName);

            return this.InitializeSubIdx(vobSubFileName);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="vobSubMergedPackist">
        /// The vob sub merged packist.
        /// </param>
        /// <param name="palette">
        /// The palette.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="languageString">
        /// The language string.
        /// </param>
        internal void Initialize(List<VobSubMergedPack> vobSubMergedPackist, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string languageString)
        {
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this.SetOcrMethod();

            this._vobSubMergedPackist = vobSubMergedPackist;
            this._palette = palette;

            if (this._palette == null)
            {
                this.checkBoxCustomFourColors.Checked = true;
            }

            this.SetTesseractLanguageFromLanguageString(languageString);
            this._importLanguageString = languageString;
        }

        /// <summary>
        /// The initialize quick.
        /// </summary>
        /// <param name="vobSubMergedPackist">
        /// The vob sub merged packist.
        /// </param>
        /// <param name="palette">
        /// The palette.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="languageString">
        /// The language string.
        /// </param>
        internal void InitializeQuick(List<VobSubMergedPack> vobSubMergedPackist, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string languageString)
        {
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;
            this._vobSubMergedPackist = vobSubMergedPackist;
            this._palette = palette;

            if (this._palette == null)
            {
                this.checkBoxCustomFourColors.Checked = true;
            }

            this._importLanguageString = languageString;
            if (this._importLanguageString.Contains('(') && !this._importLanguageString.StartsWith('('))
            {
                this._importLanguageString = this._importLanguageString.Substring(0, languageString.IndexOf('(') - 1).Trim();
            }
        }

        /// <summary>
        /// The initialize batch.
        /// </summary>
        /// <param name="subtitles">
        /// The subtitles.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void InitializeBatch(List<BluRaySupParser.PcsData> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            this.Initialize(subtitles, vobSubOcrSettings, fileName);
            this.FormVobSubOcr_Shown(null, null);
            this.checkBoxPromptForUnknownWords.Checked = false;

            int max = this.GetSubtitleCount();
            if (this.comboBoxOcrMethod.SelectedIndex == 0 && this._tesseractAsyncStrings == null)
            {
                this._tesseractAsyncStrings = new string[max];
                this._tesseractAsyncIndex = (int)this.numericUpDownStartNumber.Value + 5;
                this._tesseractThread = new BackgroundWorker();
                this._tesseractThread.DoWork += this.TesseractThreadDoWork;
                this._tesseractThread.RunWorkerCompleted += this.TesseractThreadRunWorkerCompleted;
                this._tesseractThread.WorkerSupportsCancellation = true;
                if (this._tesseractAsyncIndex >= 0 && this._tesseractAsyncIndex < max)
                {
                    this._tesseractThread.RunWorkerAsync(this.GetSubtitleBitmap(this._tesseractAsyncIndex));
                }
            }

            System.Threading.Thread.Sleep(1000);
            this.subtitleListView1.SelectedIndexChanged -= this.SubtitleListView1SelectedIndexChanged;
            for (int i = 0; i < max; i++)
            {
                Application.DoEvents();
                if (this._abort)
                {
                    this.SetButtonsEnabledAfterOcrDone();
                    return;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(i);
                string text = this.OcrViaTesseract(this.GetSubtitleBitmap(i), i);

                this._lastLine = text;

                text = text.Replace("<i>-</i>", "-");
                text = text.Replace("<i>a</i>", "a");
                text = text.Replace("  ", " ");
                text = text.Trim();

                text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                text = text.Replace(Environment.NewLine + " ", Environment.NewLine);

                // max allow 2 lines
                if (this.checkBoxAutoBreakLines.Checked && text.Replace(Environment.NewLine, "*").Length + 2 <= text.Length)
                {
                    text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                    text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    while (text.Contains(Environment.NewLine + Environment.NewLine))
                    {
                        text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    }

                    if (text.Replace(Environment.NewLine, "*").Length + 2 <= text.Length)
                    {
                        text = Utilities.AutoBreakLine(text);
                    }
                }

                Application.DoEvents();
                if (this._abort)
                {
                    this.textBoxCurrentText.Text = text;
                    this.SetButtonsEnabledAfterOcrDone();
                    return;
                }

                text = text.Trim();
                text = text.Replace("  ", " ");
                text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                text = text.Replace("  ", " ");
                text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

                Paragraph p = this._subtitle.GetParagraphOrDefault(i);
                if (p != null)
                {
                    p.Text = text;
                }

                if (this.subtitleListView1.SelectedItems.Count == 1 && this.subtitleListView1.SelectedItems[0].Index == i)
                {
                    this.textBoxCurrentText.Text = text;
                }
                else
                {
                    this.subtitleListView1.SetText(i, text);
                }
            }

            this.SetButtonsEnabledAfterOcrDone();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitles">
        /// The subtitles.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(List<BluRaySupParser.PcsData> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = 11; // vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value = 11;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this.SetOcrMethod();

            this._bluRaySubtitlesOriginal = subtitles;

            this.groupBoxImagePalette.Visible = false;

            this.Text = Configuration.Settings.Language.VobSubOcr.TitleBluRay;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.Length > 40)
                {
                    fileName = Path.GetFileName(fileName);
                }

                this.Text += " - " + fileName;
            }

            this.checkBoxAutoTransparentBackground.Checked = false;
            this.checkBoxAutoTransparentBackground.Visible = false;
        }

        /// <summary>
        /// The load image compare character database list.
        /// </summary>
        private void LoadImageCompareCharacterDatabaseList()
        {
            try
            {
                if (this.comboBoxOcrMethod.SelectedIndex == 4)
                {
                    string characterDatabasePath = Configuration.OcrFolder.TrimEnd(Path.DirectorySeparatorChar);
                    if (!Directory.Exists(characterDatabasePath))
                    {
                        Directory.CreateDirectory(characterDatabasePath);
                    }

                    this.comboBoxCharacterDatabase.Items.Clear();

                    foreach (string dir in Directory.GetFiles(characterDatabasePath, "*.db"))
                    {
                        this.comboBoxCharacterDatabase.Items.Add(Path.GetFileNameWithoutExtension(dir));
                    }

                    if (this.comboBoxCharacterDatabase.Items.Count == 0)
                    {
                        this.comboBoxCharacterDatabase.Items.Add("Latin"); // if no database, create an empty one called "Latin"
                    }

                    if (this.comboBoxCharacterDatabase.SelectedIndex < 0 && this.comboBoxCharacterDatabase.Items.Count > 0)
                    {
                        this.comboBoxCharacterDatabase.SelectedIndex = 0;
                    }
                }
                else if (this.comboBoxOcrMethod.SelectedIndex == 1)
                {
                    this.comboBoxCharacterDatabase.SelectedIndexChanged -= this.ComboBoxCharacterDatabaseSelectedIndexChanged;
                    string characterDatabasePath = Configuration.VobSubCompareFolder.TrimEnd(Path.DirectorySeparatorChar);
                    if (!Directory.Exists(characterDatabasePath))
                    {
                        Directory.CreateDirectory(characterDatabasePath);
                    }

                    this.comboBoxCharacterDatabase.Items.Clear();

                    foreach (string dir in Directory.GetDirectories(characterDatabasePath))
                    {
                        this.comboBoxCharacterDatabase.Items.Add(Path.GetFileName(dir));
                    }

                    if (this.comboBoxCharacterDatabase.Items.Count == 0)
                    {
                        Directory.CreateDirectory(characterDatabasePath + Path.DirectorySeparatorChar + this._vobSubOcrSettings.LastImageCompareFolder);
                        this.comboBoxCharacterDatabase.Items.Add(this._vobSubOcrSettings.LastImageCompareFolder);
                    }

                    for (int i = 0; i < this.comboBoxCharacterDatabase.Items.Count; i++)
                    {
                        if (this.comboBoxCharacterDatabase.Items[i].ToString().Equals(this._vobSubOcrSettings.LastImageCompareFolder, StringComparison.OrdinalIgnoreCase))
                        {
                            this.comboBoxCharacterDatabase.SelectedIndex = i;
                        }
                    }

                    if (this.comboBoxCharacterDatabase.SelectedIndex < 0)
                    {
                        this.comboBoxCharacterDatabase.SelectedIndex = 0;
                    }

                    this.comboBoxCharacterDatabase.SelectedIndexChanged += this.ComboBoxCharacterDatabaseSelectedIndexChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Configuration.Settings.Language.VobSubOcr.UnableToCreateCharacterDatabaseFolder, ex.Message));
            }
        }

        /// <summary>
        /// The load image compare bitmaps.
        /// </summary>
        private void LoadImageCompareBitmaps()
        {
            this.DisposeImageCompareBitmaps();
            this._binaryOcrDb = null;

            if (this.comboBoxOcrMethod.SelectedIndex == 1)
            {
                this.LoadOldCompareImages();
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 4)
            {
                string db = Configuration.OcrFolder + this.comboBoxCharacterDatabase.SelectedItem + ".db";
                this._binaryOcrDb = new BinaryOcrDb(db, true);
            }
        }

        /// <summary>
        /// The load old compare images.
        /// </summary>
        private void LoadOldCompareImages()
        {
            this._compareBitmaps = new List<CompareItem>();
            string path = Configuration.VobSubCompareFolder + this.comboBoxCharacterDatabase.SelectedItem + Path.DirectorySeparatorChar;
            if (!File.Exists(path + "CompareDescription.xml"))
            {
                this._compareDoc.LoadXml("<OcrBitmaps></OcrBitmaps>");
            }
            else
            {
                this._compareDoc.Load(path + "CompareDescription.xml");
            }

            string databaseName = path + "Images.db";
            if (!File.Exists(databaseName))
            {
                this.labelStatus.Text = Configuration.Settings.Language.VobSubOcr.LoadingImageCompareDatabase;
                this.labelStatus.Refresh();
                using (var f = new FileStream(databaseName, FileMode.Create))
                {
                    foreach (string bmpFileName in Directory.GetFiles(path, "*.bmp"))
                    {
                        string name = Path.GetFileNameWithoutExtension(bmpFileName);

                        XmlNode node = this._compareDoc.DocumentElement.SelectSingleNode("FileName[.='" + name + "']");
                        if (node != null)
                        {
                            node.InnerText = f.Position.ToString(CultureInfo.InvariantCulture);
                            var b = new Bitmap(bmpFileName);
                            var m = new ManagedBitmap(b);
                            b.Dispose();
                            m.AppendToStream(f);
                        }
                    }
                }

                this._compareDoc.Save(path + "Images.xml");
                string text = File.ReadAllText(path + "Images.xml");
                File.WriteAllText(path + "Images.xml", text.Replace("<FileName", "<Item").Replace("</FileName>", "</Item>"));
                this.labelStatus.Text = string.Empty;
            }

            if (File.Exists(databaseName))
            {
                this.labelStatus.Text = Configuration.Settings.Language.VobSubOcr.LoadingImageCompareDatabase;
                this.labelStatus.Refresh();
                this._compareDoc.Load(path + "Images.xml");
                using (var f = new FileStream(databaseName, FileMode.Open))
                {
                    foreach (XmlNode node in this._compareDoc.DocumentElement.SelectNodes("Item"))
                    {
                        try
                        {
                            string name = node.InnerText;
                            int pos = Convert.ToInt32(name);
                            bool isItalic = node.Attributes["Italic"] != null;
                            string text = node.Attributes["Text"].InnerText;
                            int expandCount = 0;
                            if (node.Attributes["Expand"] != null)
                            {
                                if (!int.TryParse(node.Attributes["Expand"].InnerText, out expandCount))
                                {
                                    expandCount = 0;
                                }
                            }

                            f.Position = pos;
                            var mbmp = new ManagedBitmap(f);
                            this._compareBitmaps.Add(new CompareItem(mbmp, name, isItalic, expandCount, text));
                        }
                        catch
                        {
                            // MessageBox.Show(node.OuterXml);
                        }
                    }
                }

                this.labelStatus.Text = string.Empty;
            }
        }

        /// <summary>
        /// The dispose image compare bitmaps.
        /// </summary>
        private void DisposeImageCompareBitmaps()
        {
            this._compareBitmaps = null;
        }

        /// <summary>
        /// The initialize sub idx.
        /// </summary>
        /// <param name="vobSubFileName">
        /// The vob sub file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool InitializeSubIdx(string vobSubFileName)
        {
            var vobSubParser = new VobSubParser(true);
            string idxFileName = Path.ChangeExtension(vobSubFileName, ".idx");
            vobSubParser.OpenSubIdx(vobSubFileName, idxFileName);
            this._vobSubMergedPackist = vobSubParser.MergeVobSubPacks();
            this._palette = vobSubParser.IdxPalette;
            vobSubParser.VobSubPacks.Clear();

            var languageStreamIds = new List<int>();
            foreach (var pack in this._vobSubMergedPackist)
            {
                if (pack.SubPicture.Delay.TotalMilliseconds > 500 && !languageStreamIds.Contains(pack.StreamId))
                {
                    languageStreamIds.Add(pack.StreamId);
                }
            }

            if (languageStreamIds.Count > 1)
            {
                using (var chooseLanguage = new DvdSubRipChooseLanguage())
                {
                    if (this.ShowInTaskbar)
                    {
                        chooseLanguage.Icon = (Icon)this.Icon.Clone();
                        chooseLanguage.ShowInTaskbar = true;
                        chooseLanguage.ShowIcon = true;
                    }

                    chooseLanguage.Initialize(this._vobSubMergedPackist, this._palette, vobSubParser.IdxLanguages, string.Empty);
                    Form form = this._main;
                    if (form == null)
                    {
                        form = this;
                    }

                    chooseLanguage.Activate();
                    if (chooseLanguage.ShowDialog(form) == DialogResult.OK)
                    {
                        this._vobSubMergedPackist = chooseLanguage.SelectedVobSubMergedPacks;
                        this.SetTesseractLanguageFromLanguageString(chooseLanguage.SelectedLanguageString);
                        this._importLanguageString = chooseLanguage.SelectedLanguageString;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The set tesseract language from language string.
        /// </summary>
        /// <param name="languageString">
        /// The language string.
        /// </param>
        private void SetTesseractLanguageFromLanguageString(string languageString)
        {
            // try to match language from vob to Tesseract language
            if (this.comboBoxTesseractLanguages.SelectedIndex >= 0 && this.comboBoxTesseractLanguages.Items.Count > 1 && languageString != null)
            {
                languageString = languageString.ToLower();
                for (int i = 0; i < this.comboBoxTesseractLanguages.Items.Count; i++)
                {
                    var tl = this.comboBoxTesseractLanguages.Items[i] as TesseractLanguage;
                    if (tl.Text.StartsWith("Chinese") && (languageString.StartsWith("chinese") || languageString.StartsWith("中文")))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }

                    if (tl.Text.StartsWith("Korean") && (languageString.StartsWith("korean") || languageString.StartsWith("한국어")))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Swedish") && languageString.StartsWith("svenska"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Norwegian") && languageString.StartsWith("norsk"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Dutch") && languageString.StartsWith("Nederlands"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Danish") && languageString.StartsWith("dansk"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("English") && languageString.StartsWith("English"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("French") && (languageString.StartsWith("french") || languageString.StartsWith("français")))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Spannish") && (languageString.StartsWith("spannish") || languageString.StartsWith("españo")))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Finnish") && languageString.StartsWith("suomi"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Italian") && languageString.StartsWith("itali"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("German") && languageString.StartsWith("deutsch"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                    else if (tl.Text.StartsWith("Portuguese") && languageString.StartsWith("português"))
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The load bdn xml.
        /// </summary>
        private void LoadBdnXml()
        {
            this._subtitle = new Subtitle();

            this._bdnXmlSubtitle = new Subtitle();
            int max = this._bdnXmlOriginal.Paragraphs.Count;
            for (int i = 0; i < max; i++)
            {
                var x = this._bdnXmlOriginal.Paragraphs[i];
                if ((this.checkBoxShowOnlyForced.Checked && x.Forced) || this.checkBoxShowOnlyForced.Checked == false)
                {
                    this._bdnXmlSubtitle.Paragraphs.Add(new Paragraph(x));
                    var p = new Paragraph(x);
                    p.Text = string.Empty;
                    this._subtitle.Paragraphs.Add(p);
                }
            }

            this._subtitle.Renumber();

            this.FixShortDisplayTimes(this._subtitle);

            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);

            this.numericUpDownStartNumber.Maximum = max;
            if (this.numericUpDownStartNumber.Maximum > 0 && this.numericUpDownStartNumber.Minimum <= 1)
            {
                this.numericUpDownStartNumber.Value = 1;
            }

            this.buttonOK.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.buttonStartOcr.Enabled = true;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = true;
            this.buttonEditCharacterDatabase.Enabled = true;
            this.buttonStartOcr.Focus();
        }

        /// <summary>
        /// The load blu ray sup.
        /// </summary>
        private void LoadBluRaySup()
        {
            this._subtitle = new Subtitle();

            this._bluRaySubtitles = new List<BluRaySupParser.PcsData>();
            int max = this._bluRaySubtitlesOriginal.Count;
            for (int i = 0; i < max; i++)
            {
                var x = this._bluRaySubtitlesOriginal[i];
                if ((this.checkBoxShowOnlyForced.Checked && x.IsForced) || this.checkBoxShowOnlyForced.Checked == false)
                {
                    this._bluRaySubtitles.Add(x);
                    Paragraph p = new Paragraph();
                    p.StartTime = new TimeCode((x.StartTime + 45) / 90.0);
                    p.EndTime = new TimeCode((x.EndTime + 45) / 90.0);
                    this._subtitle.Paragraphs.Add(p);
                }
            }

            this._subtitle.Renumber();

            this.FixShortDisplayTimes(this._subtitle);

            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);

            this.numericUpDownStartNumber.Maximum = max;
            if (this.numericUpDownStartNumber.Maximum > 0 && this.numericUpDownStartNumber.Minimum <= 1)
            {
                this.numericUpDownStartNumber.Value = 1;
            }

            this.buttonOK.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.buttonStartOcr.Enabled = true;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = true;
            this.buttonEditCharacterDatabase.Enabled = true;
            this.buttonStartOcr.Focus();
        }

        /// <summary>
        /// The load vob rip.
        /// </summary>
        private void LoadVobRip()
        {
            this._subtitle = new Subtitle();
            this._vobSubMergedPackist = new List<VobSubMergedPack>();
            int max = this._vobSubMergedPackistOriginal.Count;
            for (int i = 0; i < max; i++)
            {
                var x = this._vobSubMergedPackistOriginal[i];
                if ((this.checkBoxShowOnlyForced.Checked && x.SubPicture.Forced) || this.checkBoxShowOnlyForced.Checked == false)
                {
                    this._vobSubMergedPackist.Add(x);
                    Paragraph p = new Paragraph(string.Empty, x.StartTime.TotalMilliseconds, x.EndTime.TotalMilliseconds);
                    if (this.checkBoxUseTimeCodesFromIdx.Checked && x.IdxLine != null)
                    {
                        double durationMilliseconds = p.Duration.TotalMilliseconds;
                        p.StartTime = new TimeCode(x.IdxLine.StartTime.TotalMilliseconds);
                        p.EndTime = new TimeCode(x.IdxLine.StartTime.TotalMilliseconds + durationMilliseconds);
                    }

                    this._subtitle.Paragraphs.Add(p);
                }
            }

            this._subtitle.Renumber();

            this.FixShortDisplayTimes(this._subtitle);

            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);

            this.numericUpDownStartNumber.Maximum = max;
            if (this.numericUpDownStartNumber.Maximum > 0 && this.numericUpDownStartNumber.Minimum <= 1)
            {
                this.numericUpDownStartNumber.Value = 1;
            }

            this.buttonOK.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.buttonStartOcr.Enabled = true;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = true;
            this.buttonEditCharacterDatabase.Enabled = true;
            this.buttonStartOcr.Focus();
        }

        /// <summary>
        /// The fix short display times.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public void FixShortDisplayTimes(Subtitle subtitle)
        {
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = this._subtitle.Paragraphs[i];
                if (p.EndTime.TotalMilliseconds <= p.StartTime.TotalMilliseconds)
                {
                    Paragraph next = this._subtitle.GetParagraphOrDefault(i + 1);
                    double newEndTime = p.StartTime.TotalMilliseconds + Configuration.Settings.VobSubOcr.DefaultMillisecondsForUnknownDurations;
                    if (next == null || (newEndTime < next.StartTime.TotalMilliseconds))
                    {
                        p.EndTime.TotalMilliseconds = newEndTime;
                    }
                    else
                    {
                        p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                    }
                }
            }
        }

        /// <summary>
        /// The get is forced.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool GetIsForced(int index)
        {
            if (this._mp4List != null)
            {
                return this._mp4List[index].Picture.Forced;
            }

            if (this._spList != null)
            {
                return this._spList[index].Picture.Forced;
            }

            if (this._bdnXmlSubtitle != null)
            {
                return false;
            }

            if (this._xSubList != null)
            {
                return false;
            }

            if (this._dvbSubtitles != null)
            {
                // return _dvbSubtitles[index]. ??
                return false;
            }

            if (this._bluRaySubtitlesOriginal != null)
            {
                return this._bluRaySubtitles[index].IsForced;
            }

            if (this.checkBoxCustomFourColors.Checked)
            {
                return this._vobSubMergedPackist[index].SubPicture.Forced;
            }

            if (this._vobSubMergedPackist != null && index < this._vobSubMergedPackist.Count)
            {
                return this._vobSubMergedPackist[index].SubPicture.Forced;
            }

            return false;
        }

        /// <summary>
        /// The get subtitle bitmap.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetSubtitleBitmap(int index)
        {
            Bitmap returnBmp = null;
            Color background;
            Color pattern;
            Color emphasis1;
            Color emphasis2;

            if (this._mp4List != null)
            {
                if (this.checkBoxCustomFourColors.Checked)
                {
                    this.GetCustomColors(out background, out pattern, out emphasis1, out emphasis2);

                    returnBmp = this._mp4List[index].Picture.GetBitmap(null, background, pattern, emphasis1, emphasis2, true);
                    if (this.checkBoxAutoTransparentBackground.Checked)
                    {
                        returnBmp.MakeTransparent();
                    }
                }
                else
                {
                    returnBmp = this._mp4List[index].Picture.GetBitmap(null, Color.Transparent, Color.Black, Color.White, Color.Black, false);
                    if (this.checkBoxAutoTransparentBackground.Checked)
                    {
                        returnBmp.MakeTransparent();
                    }
                }
            }
            else if (this._spList != null)
            {
                if (this.checkBoxCustomFourColors.Checked)
                {
                    this.GetCustomColors(out background, out pattern, out emphasis1, out emphasis2);

                    returnBmp = this._spList[index].Picture.GetBitmap(null, background, pattern, emphasis1, emphasis2, true);
                    if (this.checkBoxAutoTransparentBackground.Checked)
                    {
                        returnBmp.MakeTransparent();
                    }
                }
                else
                {
                    returnBmp = this._spList[index].Picture.GetBitmap(null, Color.Transparent, Color.Black, Color.White, Color.Black, false);
                    if (this.checkBoxAutoTransparentBackground.Checked)
                    {
                        returnBmp.MakeTransparent();
                    }
                }
            }
            else if (this._bdnXmlSubtitle != null)
            {
                if (index >= 0 && index < this._bdnXmlSubtitle.Paragraphs.Count)
                {
                    var fileNames = this._bdnXmlSubtitle.Paragraphs[index].Text.SplitToLines();
                    var bitmaps = new List<Bitmap>();
                    int maxWidth = 0;
                    int totalHeight = 0;

                    foreach (string fn in fileNames)
                    {
                        string fullFileName = Path.Combine(Path.GetDirectoryName(this._bdnFileName), fn);
                        if (!File.Exists(fullFileName))
                        {
                            // fix AVISubDetector lines
                            int idxOfIEquals = fn.IndexOf("i=", StringComparison.OrdinalIgnoreCase);
                            if (idxOfIEquals >= 0)
                            {
                                int idxOfSpace = fn.IndexOf(' ', idxOfIEquals);
                                if (idxOfSpace > 0)
                                {
                                    fullFileName = Path.Combine(Path.GetDirectoryName(this._bdnFileName), fn.Remove(0, idxOfSpace).Trim());
                                }
                            }
                        }

                        if (File.Exists(fullFileName))
                        {
                            try
                            {
                                var temp = new Bitmap(fullFileName);
                                if (temp.Width > maxWidth)
                                {
                                    maxWidth = temp.Width;
                                }

                                totalHeight += temp.Height;
                                bitmaps.Add(temp);
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }

                    Bitmap b = null;
                    if (bitmaps.Count > 1)
                    {
                        var merged = new Bitmap(maxWidth, totalHeight + 7 * bitmaps.Count);
                        int y = 0;
                        for (int k = 0; k < bitmaps.Count; k++)
                        {
                            Bitmap part = bitmaps[k];
                            if (this.checkBoxAutoTransparentBackground.Checked)
                            {
                                part.MakeTransparent();
                            }

                            using (var g = Graphics.FromImage(merged)) g.DrawImage(part, 0, y);
                            y += part.Height + 7;
                            part.Dispose();
                        }

                        b = merged;
                    }
                    else if (bitmaps.Count == 1)
                    {
                        b = bitmaps[0];
                    }

                    if (b != null)
                    {
                        if (this._isSon && this.checkBoxCustomFourColors.Checked)
                        {
                            this.GetCustomColors(out background, out pattern, out emphasis1, out emphasis2);

                            FastBitmap fbmp = new FastBitmap(b);
                            fbmp.LockImage();
                            for (int x = 0; x < fbmp.Width; x++)
                            {
                                for (int y = 0; y < fbmp.Height; y++)
                                {
                                    Color c = fbmp.GetPixel(x, y);
                                    if (c.R == Color.Red.R && c.G == Color.Red.G && c.B == Color.Red.B)
                                    {
                                        // normally anti-alias
                                        fbmp.SetPixel(x, y, emphasis2);
                                    }
                                    else if (c.R == Color.Blue.R && c.G == Color.Blue.G && c.B == Color.Blue.B)
                                    {
                                        // normally text?
                                        fbmp.SetPixel(x, y, pattern);
                                    }
                                    else if (c.R == Color.White.R && c.G == Color.White.G && c.B == Color.White.B)
                                    {
                                        // normally background
                                        fbmp.SetPixel(x, y, background);
                                    }
                                    else if (c.R == Color.Black.R && c.G == Color.Black.G && c.B == Color.Black.B)
                                    {
                                        // outline/border
                                        fbmp.SetPixel(x, y, emphasis1);
                                    }
                                    else
                                    {
                                        fbmp.SetPixel(x, y, c);
                                    }
                                }
                            }

                            fbmp.UnlockImage();
                        }

                        if (this.checkBoxAutoTransparentBackground.Checked)
                        {
                            b.MakeTransparent();
                        }

                        returnBmp = b;
                    }
                }
            }
            else if (this._xSubList != null)
            {
                if (this.checkBoxCustomFourColors.Checked)
                {
                    this.GetCustomColors(out background, out pattern, out emphasis1, out emphasis2);
                    returnBmp = this._xSubList[index].GetImage(background, pattern, emphasis1, emphasis2);
                }
                else
                {
                    returnBmp = this._xSubList[index].GetImage();
                }
            }
            else if (this._dvbSubtitles != null)
            {
                var dvbBmp = this._dvbSubtitles[index].GetActiveImage();
                var nDvbBmp = new NikseBitmap(dvbBmp);
                nDvbBmp.CropTopTransparent(2);
                nDvbBmp.CropTransparentSidesAndBottom(2, true);
                if (this.checkBoxTransportStreamGetColorAndSplit.Checked)
                {
                    this._dvbSubColor = nDvbBmp.GetBrightestColor();
                }

                if (this.checkBoxAutoTransparentBackground.Checked)
                {
                    nDvbBmp.MakeBackgroundTransparent((int)this.numericUpDownAutoTransparentAlphaMax.Value);
                }

                if (this.checkBoxTransportStreamGrayscale.Checked)
                {
                    nDvbBmp.GrayScale();
                }

                dvbBmp.Dispose();
                returnBmp = nDvbBmp.GetBitmap();
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                returnBmp = this._bluRaySubtitles[index].GetBitmap();
            }
            else if (this.checkBoxCustomFourColors.Checked)
            {
                this.GetCustomColors(out background, out pattern, out emphasis1, out emphasis2);

                returnBmp = this._vobSubMergedPackist[index].SubPicture.GetBitmap(null, background, pattern, emphasis1, emphasis2, true);
                if (this.checkBoxAutoTransparentBackground.Checked)
                {
                    returnBmp.MakeTransparent();
                }
            }
            else
            {
                returnBmp = this._vobSubMergedPackist[index].SubPicture.GetBitmap(this._palette, Color.Transparent, Color.Black, Color.White, Color.Black, false);
                if (this.checkBoxAutoTransparentBackground.Checked)
                {
                    returnBmp.MakeTransparent();
                }
            }

            if (returnBmp == null)
            {
                return null;
            }

            if (this._binaryOcrDb == null && this._nOcrDb == null)
            {
                return returnBmp;
            }

            var n = new NikseBitmap(returnBmp);
            n.MakeTwoColor(280);
            returnBmp.Dispose();
            return n.GetBitmap();
        }

        /// <summary>
        /// The get custom colors.
        /// </summary>
        /// <param name="background">
        /// The background.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="emphasis1">
        /// The emphasis 1.
        /// </param>
        /// <param name="emphasis2">
        /// The emphasis 2.
        /// </param>
        private void GetCustomColors(out Color background, out Color pattern, out Color emphasis1, out Color emphasis2)
        {
            background = this.pictureBoxBackground.BackColor;
            pattern = this.pictureBoxPattern.BackColor;
            emphasis1 = this.pictureBoxEmphasis1.BackColor;
            emphasis2 = this.pictureBoxEmphasis2.BackColor;

            if (this.checkBoxBackgroundTransparent.Checked)
            {
                background = Color.Transparent;
            }

            if (this.checkBoxPatternTransparent.Checked)
            {
                pattern = Color.Transparent;
            }

            if (this.checkBoxEmphasis1Transparent.Checked)
            {
                emphasis1 = Color.Transparent;
            }

            if (this.checkBoxEmphasis2Transparent.Checked)
            {
                emphasis2 = Color.Transparent;
            }
        }

        /// <summary>
        /// The get subtitle start time milliseconds.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long GetSubtitleStartTimeMilliseconds(int index)
        {
            if (this._mp4List != null)
            {
                return (long)this._mp4List[index].Start.TotalMilliseconds;
            }
            else if (this._spList != null)
            {
                return (long)this._spList[index].StartTime.TotalMilliseconds;
            }
            else if (this._bdnXmlSubtitle != null)
            {
                return (long)this._bdnXmlSubtitle.Paragraphs[index].StartTime.TotalMilliseconds;
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                return (this._bluRaySubtitles[index].StartTime + 45) / 90;
            }
            else if (this._xSubList != null)
            {
                return (long)this._xSubList[index].Start.TotalMilliseconds;
            }
            else if (this._dvbSubtitles != null)
            {
                return (long)this._dvbSubtitles[index].StartMilliseconds;
            }
            else
            {
                return (long)this._vobSubMergedPackist[index].StartTime.TotalMilliseconds;
            }
        }

        /// <summary>
        /// The get subtitle end time milliseconds.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long GetSubtitleEndTimeMilliseconds(int index)
        {
            if (this._mp4List != null)
            {
                return (long)this._mp4List[index].End.TotalMilliseconds;
            }
            else if (this._spList != null)
            {
                return (long)(this._spList[index].StartTime.TotalMilliseconds + this._spList[index].Picture.Delay.TotalMilliseconds);
            }
            else if (this._bdnXmlSubtitle != null)
            {
                return (long)this._bdnXmlSubtitle.Paragraphs[index].EndTime.TotalMilliseconds;
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                return (this._bluRaySubtitles[index].EndTime + 45) / 90;
            }
            else if (this._xSubList != null)
            {
                return (long)this._xSubList[index].End.TotalMilliseconds;
            }
            else if (this._dvbSubtitles != null)
            {
                return (long)this._dvbSubtitles[index].EndMilliseconds;
            }
            else
            {
                return (long)this._vobSubMergedPackist[index].EndTime.TotalMilliseconds;
            }
        }

        /// <summary>
        /// The get subtitle count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetSubtitleCount()
        {
            if (this._mp4List != null)
            {
                return this._mp4List.Count;
            }
            else if (this._spList != null)
            {
                return this._spList.Count;
            }
            else if (this._bdnXmlSubtitle != null)
            {
                return this._bdnXmlSubtitle.Paragraphs.Count;
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                return this._bluRaySubtitles.Count;
            }
            else if (this._xSubList != null)
            {
                return this._xSubList.Count;
            }
            else if (this._dvbSubtitles != null)
            {
                return this._dvbSubtitles.Count;
            }
            else
            {
                return this._vobSubMergedPackist.Count;
            }
        }

        /// <summary>
        /// The show subtitle image.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private Bitmap ShowSubtitleImage(int index)
        {
            int numberOfImages = this.GetSubtitleCount();
            Bitmap bmp;
            if (index < numberOfImages)
            {
                bmp = this.GetSubtitleBitmap(index);
                if (bmp == null)
                {
                    bmp = new Bitmap(1, 1);
                }

                this.groupBoxSubtitleImage.Text = string.Format(Configuration.Settings.Language.VobSubOcr.SubtitleImageXofY, index + 1, numberOfImages) + "   " + bmp.Width + "x" + bmp.Height;
            }
            else
            {
                this.groupBoxSubtitleImage.Text = Configuration.Settings.Language.VobSubOcr.SubtitleImage;
                bmp = new Bitmap(1, 1);
            }

            Bitmap old = this.pictureBoxSubtitleImage.Image as Bitmap;
            this.pictureBoxSubtitleImage.Image = bmp.Clone() as Bitmap;
            this.pictureBoxSubtitleImage.Invalidate();
            if (old != null)
            {
                old.Dispose();
            }

            return bmp;
        }

        /// <summary>
        /// The show subtitle image.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        private void ShowSubtitleImage(int index, Bitmap bmp)
        {
            try
            {
                int numberOfImages = this.GetSubtitleCount();
                if (index < numberOfImages)
                {
                    this.groupBoxSubtitleImage.Text = string.Format(Configuration.Settings.Language.VobSubOcr.SubtitleImageXofY, index + 1, numberOfImages) + "   " + bmp.Width + "x" + bmp.Height;
                }
                else
                {
                    this.groupBoxSubtitleImage.Text = Configuration.Settings.Language.VobSubOcr.SubtitleImage;
                }

                Bitmap old = this.pictureBoxSubtitleImage.Image as Bitmap;
                this.pictureBoxSubtitleImage.Image = bmp.Clone() as Bitmap;
                this.pictureBoxSubtitleImage.Invalidate();
                if (old != null)
                {
                    old.Dispose();
                }
            }
            catch
            {
                // can crash is user is clicking around...
            }
        }

        /// <summary>
        /// The make point italic.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="moveLeftPixels">
        /// The move left pixels.
        /// </param>
        /// <param name="unItalicFactor">
        /// The un italic factor.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        private static Point MakePointItalic(Point p, int height, int moveLeftPixels, double unItalicFactor)
        {
            return new Point((int)Math.Round(p.X + (height - p.Y) * unItalicFactor - moveLeftPixels), p.Y);
        }

        /// <summary>
        /// The n ocr find expanded match.
        /// </summary>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="nOcrChars">
        /// The n ocr chars.
        /// </param>
        /// <returns>
        /// The <see cref="NOcrChar"/>.
        /// </returns>
        private static NOcrChar NOcrFindExpandedMatch(NikseBitmap nbmp, ImageSplitterItem targetItem, List<NOcrChar> nOcrChars)
        {
            // var nbmp = new NikseBitmap(parentBitmap);
            int w = targetItem.NikseBitmap.Width;
            foreach (NOcrChar oc in nOcrChars)
            {
                if (oc.ExpandCount > 1 && oc.Width > w && targetItem.X + oc.Width < nbmp.Width)
                {
                    bool ok = true;
                    var index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.GetPoints())
                        {
                            Point p = new Point(point.X + targetItem.X, point.Y + targetItem.Y);
                            if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(p.X, p.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.GetPoints())
                        {
                            Point p = new Point(point.X + targetItem.X, point.Y + targetItem.Y);
                            if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(p.X, p.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }

                    ok = true;
                    index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, oc.Width, oc.Height - 1))
                        {
                            Point p = new Point(point.X + targetItem.X, point.Y + targetItem.Y);
                            if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(p.X, p.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, oc.Width, oc.Height - 1))
                        {
                            Point p = new Point(point.X + targetItem.X, point.Y + targetItem.Y);
                            if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(p.X, p.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The n ocr find best match.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="topMargin">
        /// The top margin.
        /// </param>
        /// <param name="italic">
        /// The italic.
        /// </param>
        /// <param name="nOcrChars">
        /// The n ocr chars.
        /// </param>
        /// <param name="unItalicFactor">
        /// The un italic factor.
        /// </param>
        /// <param name="tryItalicScaling">
        /// The try italic scaling.
        /// </param>
        /// <param name="deepSeek">
        /// The deep seek.
        /// </param>
        /// <returns>
        /// The <see cref="NOcrChar"/>.
        /// </returns>
        private static NOcrChar NOcrFindBestMatch(ImageSplitterItem targetItem, int topMargin, out bool italic, List<NOcrChar> nOcrChars, double unItalicFactor, bool tryItalicScaling, bool deepSeek)
        {
            italic = false;
            var nbmp = targetItem.NikseBitmap;
            int index = 0;
            foreach (NOcrChar oc in nOcrChars)
            {
                if (Math.Abs(oc.Width - nbmp.Width) < 3 && Math.Abs(oc.Height - nbmp.Height) < 3 && Math.Abs(oc.MarginTop - topMargin) < 3)
                { // only very accurate matches

                    bool ok = true;
                    index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    Point p = new Point(point.X - 1, point.Y);
                                    if (p.X < 0)
                                    {
                                        p.X = 1;
                                    }

                                    c = nbmp.GetPixel(p.X, p.Y);
                                    if (nbmp.Width > 20 && c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    Point p = new Point(point.X, point.Y);
                                    if (oc.Width > 19 && point.X > 0)
                                    {
                                        p.X = p.X - 1;
                                    }

                                    c = nbmp.GetPixel(p.X, p.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }
                }
            }

            foreach (NOcrChar oc in nOcrChars)
            {
                int marginTopDiff = Math.Abs(oc.MarginTop - topMargin);
                if (Math.Abs(oc.Width - nbmp.Width) < 4 && Math.Abs(oc.Height - nbmp.Height) < 4 && marginTopDiff > 4 && marginTopDiff < 9)
                { // only very accurate matches - but not for margin top

                    bool ok = true;
                    index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }
                }
            }

            // try some resize if aspect ratio is about the same
            double widthPercent = nbmp.Height * 100.0 / nbmp.Width;
            foreach (NOcrChar oc in nOcrChars)
            {
                if (!oc.IsSensitive)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 15 && oc.Width > 12 && oc.Height > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < nbmp.Height / 4)
                    {
                        bool ok = true;
                        index = 0;
                        while (index < oc.LinesForeground.Count && ok)
                        {
                            NOcrPoint op = oc.LinesForeground[index];
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }

                            index++;
                        }

                        index = 0;
                        while (index < oc.LinesBackground.Count && ok)
                        {
                            NOcrPoint op = oc.LinesBackground[index];
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }

                            index++;
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }
            }

            //// matches 2 or 3 pixels to the left
            // foreach (NOcrChar oc in nOcrChars)
            // {
            // if (!oc.IsSensitive)
            // {
            // if (Math.Abs(oc.WidthPercent - widthPercent) < 15 && oc.Width > 14 && oc.Height > 19 && nbmp.Width > 20 && nbmp.Height > 14 && Math.Abs(oc.MarginTop - topMargin) < nbmp.Height / 4)
            // {
            // bool ok = true;
            // index = 0;
            // while (index < oc.LinesForeground.Count && ok)
            // {
            // NOcrPoint op = oc.LinesForeground[index];
            // foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
            // {
            // Point p = new Point(point.X - 2, point.Y);
            // Point p1 = new Point(point.X - 1, point.Y);
            // if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height && p1.X >= 0)
            // {
            // Color c = nbmp.GetPixel(p.X, p.Y);
            // if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
            // {
            // }
            // else
            // {
            // c = nbmp.GetPixel(p1.X, p1.Y);
            // if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
            // {
            // }
            // else
            // {
            // ok = false;
            // break;
            // }
            // }
            // }
            // }
            // index++;
            // }
            // index = 0;
            // while (index < oc.LinesBackground.Count && ok)
            // {
            // NOcrPoint op = oc.LinesBackground[index];
            // foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
            // {
            // Point p = new Point(point.X - 2, point.Y);
            // Point p1 = new Point(point.X - 1, point.Y);
            // if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && point.Y < nbmp.Height && p1.X >= 0)
            // {
            // Color c = nbmp.GetPixel(p.X, p.Y);
            // if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
            // {
            // c = nbmp.GetPixel(p1.X, p1.Y);
            // if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
            // {
            // ok = false;
            // break;
            // }
            // }
            // }
            // }
            // index++;
            // }
            // if (ok)
            // return oc;
            // }
            // }
            // }

            //// matches 5 pixels lower
            // int yLower = 5;
            // widthPercent = (nbmp.Height - yLower) * 100.0 / nbmp.Width;
            // foreach (NOcrChar oc in nOcrChars)
            // {
            // if (!oc.IsSensitive)
            // {
            // if (Math.Abs(oc.WidthPercent - widthPercent) < 20 && oc.Width > 12 && oc.Height > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
            // {
            // bool ok = true;
            // index = 0;
            // while (index < oc.LinesForeground.Count && ok)
            // {
            // NOcrPoint op = oc.LinesForeground[index];
            // foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height - yLower))
            // {
            // if (point.X >= 0 && point.Y + yLower >= 0 && point.X < nbmp.Width && point.Y + yLower < nbmp.Height)
            // {
            // Color c = nbmp.GetPixel(point.X, point.Y + yLower);
            // if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
            // {
            // }
            // else
            // {
            // ok = false;
            // break;
            // }
            // }
            // }
            // index++;
            // }
            // index = 0;
            // while (index < oc.LinesBackground.Count && ok)
            // {
            // NOcrPoint op = oc.LinesBackground[index];
            // foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height - yLower))
            // {
            // if (point.X >= 0 && point.Y + yLower >= 0 && point.X < nbmp.Width && point.Y + yLower < nbmp.Height)
            // {
            // Color c = nbmp.GetPixel(point.X, point.Y + yLower);
            // if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
            // {
            // ok = false;
            // break;
            // }
            // }
            // }
            // index++;
            // }
            // if (ok)
            // return oc;
            // }
            // }
            // }
            if (deepSeek)
            {
                // if we do now draw then just try anything...
                widthPercent = nbmp.Height * 100.0 / nbmp.Width;

                foreach (NOcrChar oc in nOcrChars)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 40 && oc.Height > 12 && oc.Width > 16 && nbmp.Width > 16 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
                    {
                        bool ok = true;
                        foreach (NOcrPoint op in oc.LinesForeground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (NOcrPoint op in oc.LinesBackground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }

                foreach (NOcrChar oc in nOcrChars)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 40 && oc.Height > 12 && oc.Width > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
                    {
                        bool ok = true;
                        foreach (NOcrPoint op in oc.LinesForeground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width - 3, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (NOcrPoint op in oc.LinesBackground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width - 3, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }

                foreach (NOcrChar oc in nOcrChars)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 40 && oc.Height > 12 && oc.Width > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
                    {
                        bool ok = true;
                        foreach (NOcrPoint op in oc.LinesForeground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height - 4))
                            {
                                if (point.X >= 0 && point.Y + 4 >= 0 && point.X < nbmp.Width && point.Y + 4 < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y + 4);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (NOcrPoint op in oc.LinesBackground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height - 4))
                            {
                                if (point.X >= 0 && point.Y + 4 >= 0 && point.X < nbmp.Width && point.Y + 4 < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y + 4);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }
            }

            if (tryItalicScaling)
            {
                // int left = targetItem.X;
                // int width = targetItem.Bitmap.Width;
                // //if (left > 3)
                // //{
                // //    left -= 3;
                // //    width += 3;
                // //}
                // var temp = ImageSplitter.Copy(parentBitmap, new Rectangle(left, targetItem.Y, width , targetItem.Bitmap.Height));
                // var bitmap2 = UnItalic(temp, unItalicFactor);
                // //var nbmpUnItalic = new NikseBitmap(unItalicedBmp);
                // //nbmpUnItalic.ReplaceNonWhiteWithTransparent();
                // //Bitmap bitmap2 = nbmpUnItalic.GetBitmap();
                ////  bitmap2.Save(@"D:\Download\__" + Guid.NewGuid().ToString() + ".bmp");
                // var list = ImageSplitter.SplitBitmapToLetters(bitmap2, 10, false, false);
                // var matches = new List<NOcrChar>();
                // bool unitalicOk = true;
                // foreach (var spi in list)
                // {
                // var m = NOcrFindBestMatch(spi, topMargin, out italic, nOcrChars, unItalicFactor, false, true);
                // if (m == null)
                // {
                // if (spi.Bitmap.Width > 2)
                // {
                // unitalicOk = false;
                // break;
                // }
                // }
                // else
                // {
                // matches.Add(m);
                // }
                // }

                // if (unitalicOk && matches.Count > 0)
                // {
                // italic = true;
                // if (matches.Count == 1)
                // {
                // return matches[0];
                // }
                // else if (matches.Count > 1)
                // {
                // NOcrChar c = new NOcrChar(matches[0]);
                // c.LinesBackground.Clear();
                // c.LinesForeground.Clear();
                // c.Text = string.Empty;
                // foreach (var m in matches)
                // c.Text += m.Text;
                // return c;
                // }
                // }
                int maxMoveLeft = 9;
                if (nbmp.Width < 20)
                {
                    maxMoveLeft = 7;
                }

                if (nbmp.Width < 16)
                {
                    maxMoveLeft = 4;
                }

                for (int movePixelsLeft = 0; movePixelsLeft < maxMoveLeft; movePixelsLeft++)
                {
                    foreach (NOcrChar oc in nOcrChars)
                    {
                        if (Math.Abs(oc.WidthPercent - widthPercent) < 99 && oc.Width > 10 && nbmp.Width > 10)
                        {
                            bool ok = true;
                            var o = MakeItalicNOcrChar(oc, movePixelsLeft, unItalicFactor);
                            index = 0;
                            while (index < o.LinesForeground.Count && ok)
                            {
                                NOcrPoint op = o.LinesForeground[index];
                                foreach (Point p in op.ScaledGetPoints(o, nbmp.Width, nbmp.Height))
                                {
                                    if (p.X >= 2 && p.Y >= 2 && p.X < nbmp.Width && p.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(p.X, p.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                        }
                                        else
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }

                                index++;
                            }

                            index = 0;
                            while (index < o.LinesBackground.Count && ok)
                            {
                                NOcrPoint op = o.LinesBackground[index];
                                foreach (Point p in op.ScaledGetPoints(o, nbmp.Width, nbmp.Height))
                                {
                                    if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(p.X, p.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }

                                index++;
                            }

                            if (ok)
                            {
                                italic = true;
                                return o;
                            }
                        }
                    }
                }

                for (int movePixelsLeft = 0; movePixelsLeft < maxMoveLeft; movePixelsLeft++)
                {
                    foreach (NOcrChar oc in nOcrChars)
                    {
                        if (Math.Abs(oc.WidthPercent - widthPercent) < 99 && oc.Width > 10 && nbmp.Width > 10)
                        {
                            bool ok = true;
                            var o = MakeItalicNOcrChar(oc, movePixelsLeft, unItalicFactor);
                            index = 0;
                            while (index < o.LinesForeground.Count && ok)
                            {
                                NOcrPoint op = o.LinesForeground[index];
                                foreach (Point p in op.ScaledGetPoints(o, nbmp.Width - 4, nbmp.Height))
                                {
                                    if (p.X >= 2 && p.Y >= 2 && p.X < nbmp.Width && p.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(p.X, p.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                        }
                                        else
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }

                                index++;
                            }

                            index = 0;
                            while (index < o.LinesBackground.Count && ok)
                            {
                                NOcrPoint op = o.LinesBackground[index];
                                foreach (Point p in op.ScaledGetPoints(o, nbmp.Width - 4, nbmp.Height))
                                {
                                    if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(p.X, p.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }

                                index++;
                            }

                            if (ok)
                            {
                                italic = true;
                                return o;
                            }
                        }
                    }
                }

                for (int movePixelsLeft = 0; movePixelsLeft < maxMoveLeft; movePixelsLeft++)
                {
                    foreach (NOcrChar oc in nOcrChars)
                    {
                        if (Math.Abs(oc.WidthPercent - widthPercent) < 99 && oc.Width > 10 && nbmp.Width > 10)
                        {
                            bool ok = true;
                            var o = MakeItalicNOcrChar(oc, movePixelsLeft, unItalicFactor);
                            index = 0;
                            while (index < o.LinesForeground.Count && ok)
                            {
                                NOcrPoint op = o.LinesForeground[index];
                                foreach (Point p in op.ScaledGetPoints(o, nbmp.Width - 6, nbmp.Height))
                                {
                                    if (p.X >= 2 && p.Y >= 2 && p.X < nbmp.Width && p.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(p.X, p.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                        }
                                        else
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }

                                index++;
                            }

                            index = 0;
                            while (index < o.LinesBackground.Count && ok)
                            {
                                NOcrPoint op = o.LinesBackground[index];
                                foreach (Point p in op.ScaledGetPoints(o, nbmp.Width - 6, nbmp.Height))
                                {
                                    if (p.X >= 0 && p.Y >= 0 && p.X < nbmp.Width && p.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(p.X, p.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }

                                index++;
                            }

                            if (ok)
                            {
                                italic = true;
                                return o;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The n ocr find best match new.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="topMargin">
        /// The top margin.
        /// </param>
        /// <param name="italic">
        /// The italic.
        /// </param>
        /// <param name="nOcrDb">
        /// The n ocr db.
        /// </param>
        /// <param name="tryItalicScaling">
        /// The try italic scaling.
        /// </param>
        /// <param name="deepSeek">
        /// The deep seek.
        /// </param>
        /// <returns>
        /// The <see cref="NOcrChar"/>.
        /// </returns>
        private static NOcrChar NOcrFindBestMatchNew(ImageSplitterItem targetItem, int topMargin, out bool italic, NOcrDb nOcrDb, bool tryItalicScaling, bool deepSeek)
        {
            italic = false;
            if (nOcrDb == null)
            {
                return null;
            }

            var nbmp = targetItem.NikseBitmap;
            int index;
            foreach (NOcrChar oc in nOcrDb.OcrCharacters)
            {
                if (Math.Abs(oc.Width - nbmp.Width) < 3 && Math.Abs(oc.Height - nbmp.Height) < 3 && Math.Abs(oc.MarginTop - topMargin) < 3)
                { // only very accurate matches

                    bool ok = true;
                    index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    Point p = new Point(point.X - 1, point.Y);
                                    if (p.X < 0)
                                    {
                                        p.X = 1;
                                    }

                                    c = nbmp.GetPixel(p.X, p.Y);
                                    if (nbmp.Width > 20 && c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    Point p = new Point(point.X, point.Y);
                                    if (oc.Width > 19 && point.X > 0)
                                    {
                                        p.X = p.X - 1;
                                    }

                                    c = nbmp.GetPixel(p.X, p.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }
                }
            }

            foreach (NOcrChar oc in nOcrDb.OcrCharacters)
            {
                int marginTopDiff = Math.Abs(oc.MarginTop - topMargin);
                if (Math.Abs(oc.Width - nbmp.Width) < 4 && Math.Abs(oc.Height - nbmp.Height) < 4 && marginTopDiff > 4 && marginTopDiff < 9)
                { // only very accurate matches - but not for margin top

                    bool ok = true;
                    index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }
                }
            }

            // try some resize if aspect ratio is about the same
            double widthPercent = nbmp.Height * 100.0 / nbmp.Width;
            foreach (NOcrChar oc in nOcrDb.OcrCharacters)
            {
                if (!oc.IsSensitive)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 15 && oc.Width > 12 && oc.Height > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < nbmp.Height / 4)
                    {
                        bool ok = true;
                        index = 0;
                        while (index < oc.LinesForeground.Count && ok)
                        {
                            NOcrPoint op = oc.LinesForeground[index];
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }

                            index++;
                        }

                        index = 0;
                        while (index < oc.LinesBackground.Count && ok)
                        {
                            NOcrPoint op = oc.LinesBackground[index];
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }

                            index++;
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }
            }

            if (deepSeek)
            {
                // if we do now draw then just try anything...
                widthPercent = nbmp.Height * 100.0 / nbmp.Width;

                foreach (NOcrChar oc in nOcrDb.OcrCharacters)
                {
                    if (!oc.IsSensitive)
                    {
                        if (Math.Abs(oc.WidthPercent - widthPercent) < 40 && nbmp.Height > 11)
                        {
                            // && oc.Height > 12 && oc.Width > 16 && nbmp.Width > 16 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
                            bool ok = true;
                            foreach (NOcrPoint op in oc.LinesForeground)
                            {
                                foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                                {
                                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(point.X, point.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                        }
                                        else
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            foreach (NOcrPoint op in oc.LinesBackground)
                            {
                                foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                                {
                                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(point.X, point.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                            ok = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (ok)
                            {
                                return oc;
                            }
                        }
                    }
                }

                foreach (NOcrChar oc in nOcrDb.OcrCharacters)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 40 && oc.Height > 12 && oc.Width > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
                    {
                        bool ok = true;
                        foreach (NOcrPoint op in oc.LinesForeground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width - 3, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (NOcrPoint op in oc.LinesBackground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width - 3, nbmp.Height))
                            {
                                if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }

                foreach (NOcrChar oc in nOcrDb.OcrCharacters)
                {
                    if (Math.Abs(oc.WidthPercent - widthPercent) < 40 && oc.Height > 12 && oc.Width > 19 && nbmp.Width > 19 && nbmp.Height > 12 && Math.Abs(oc.MarginTop - topMargin) < 15)
                    {
                        bool ok = true;
                        foreach (NOcrPoint op in oc.LinesForeground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height - 4))
                            {
                                if (point.X >= 0 && point.Y + 4 >= 0 && point.X < nbmp.Width && point.Y + 4 < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y + 4);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (NOcrPoint op in oc.LinesBackground)
                        {
                            foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height - 4))
                            {
                                if (point.X >= 0 && point.Y + 4 >= 0 && point.X < nbmp.Width && point.Y + 4 < nbmp.Height)
                                {
                                    Color c = nbmp.GetPixel(point.X, point.Y + 4);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (ok)
                        {
                            return oc;
                        }
                    }
                }
            }

            if (tryItalicScaling)
            {
                // int left = targetItem.X;
                // int width = targetItem.Bitmap.Width;
                // //if (left > 3)
                // //{
                // //    left -= 3;
                // //    width += 3;
                // //}
                // var temp = ImageSplitter.Copy(parentBitmap, new Rectangle(left, targetItem.Y, width , targetItem.Bitmap.Height));
                // var bitmap2 = UnItalic(temp, unItalicFactor);
                // //var nbmpUnItalic = new NikseBitmap(unItalicedBmp);
                // //nbmpUnItalic.ReplaceNonWhiteWithTransparent();
                // //Bitmap bitmap2 = nbmpUnItalic.GetBitmap();
                ////  bitmap2.Save(@"D:\Download\__" + Guid.NewGuid().ToString() + ".bmp");
                // var list = ImageSplitter.SplitBitmapToLetters(bitmap2, 10, false, false);
                // var matches = new List<NOcrChar>();
                // bool unitalicOk = true;
                // foreach (var spi in list)
                // {
                // var m = NOcrFindBestMatch(spi, topMargin, out italic, nOcrChars, unItalicFactor, false, true);
                // if (m == null)
                // {
                // if (spi.Bitmap.Width > 2)
                // {
                // unitalicOk = false;
                // break;
                // }
                // }
                // else
                // {
                // matches.Add(m);
                // }
                // }

                // if (unitalicOk && matches.Count > 0)
                // {
                // italic = true;
                // if (matches.Count == 1)
                // {
                // return matches[0];
                // }
                // else if (matches.Count > 1)
                // {
                // NOcrChar c = new NOcrChar(matches[0]);
                // c.LinesBackground.Clear();
                // c.LinesForeground.Clear();
                // c.Text = string.Empty;
                // foreach (var m in matches)
                // c.Text += m.Text;
                // return c;
                // }
                // }
            }

            return null;
        }

        /// <summary>
        /// The make italic n ocr char.
        /// </summary>
        /// <param name="oldChar">
        /// The old char.
        /// </param>
        /// <param name="movePixelsLeft">
        /// The move pixels left.
        /// </param>
        /// <param name="unItalicFactor">
        /// The un italic factor.
        /// </param>
        /// <returns>
        /// The <see cref="NOcrChar"/>.
        /// </returns>
        private static NOcrChar MakeItalicNOcrChar(NOcrChar oldChar, int movePixelsLeft, double unItalicFactor)
        {
            var c = new NOcrChar();
            foreach (NOcrPoint op in oldChar.LinesForeground)
            {
                c.LinesForeground.Add(new NOcrPoint(MakePointItalic(op.Start, oldChar.Height, movePixelsLeft, unItalicFactor), MakePointItalic(op.End, oldChar.Height, movePixelsLeft, unItalicFactor)));
            }

            foreach (NOcrPoint op in oldChar.LinesBackground)
            {
                c.LinesBackground.Add(new NOcrPoint(MakePointItalic(op.Start, oldChar.Height, movePixelsLeft, unItalicFactor), MakePointItalic(op.End, oldChar.Height, movePixelsLeft, unItalicFactor)));
            }

            c.Text = oldChar.Text;
            c.Width = oldChar.Width;
            c.Height = oldChar.Height;
            c.MarginTop = oldChar.MarginTop;
            c.Italic = true;
            return c;
        }

        /// <summary>
        /// The get n ocr compare match.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="parentBitmap">
        /// The parent bitmap.
        /// </param>
        /// <param name="nOcrDb">
        /// The n ocr db.
        /// </param>
        /// <param name="tryItalicScaling">
        /// The try italic scaling.
        /// </param>
        /// <param name="deepSeek">
        /// The deep seek.
        /// </param>
        /// <returns>
        /// The <see cref="CompareMatch"/>.
        /// </returns>
        internal CompareMatch GetNOcrCompareMatch(ImageSplitterItem targetItem, NikseBitmap parentBitmap, NOcrDb nOcrDb, bool tryItalicScaling, bool deepSeek)
        {
            bool italic;
            var expandedResult = NOcrFindExpandedMatch(parentBitmap, targetItem, nOcrDb.OcrCharacters);
            if (expandedResult != null)
            {
                return new CompareMatch(expandedResult.Text, expandedResult.Italic, expandedResult.ExpandCount, null, expandedResult);
            }

            var result = NOcrFindBestMatchNew(targetItem, targetItem.Y - targetItem.ParentY, out italic, nOcrDb, tryItalicScaling, deepSeek);
            if (result == null)
            {
                if (this.checkBoxNOcrCorrect.Checked)
                {
                    return null;
                }

                return new CompareMatch("*", false, 0, null);
            }

            // Fix uppercase/lowercase issues (not I/l)
            if (result.Text == "e")
            {
                this._nocrLastLowercaseHeight = targetItem.NikseBitmap.Height;
            }
            else if (this._nocrLastLowercaseHeight == -1 && result.Text == "a")
            {
                this._nocrLastLowercaseHeight = targetItem.NikseBitmap.Height;
            }

            if (result.Text == "E" || result.Text == "H" || result.Text == "R" || result.Text == "D" || result.Text == "T")
            {
                this._nocrLastUppercaseHeight = targetItem.NikseBitmap.Height;
            }
            else if (this._nocrLastUppercaseHeight == -1 && result.Text == "M")
            {
                this._nocrLastUppercaseHeight = targetItem.NikseBitmap.Height;
            }

            if (result.Text == "V" || result.Text == "W" || result.Text == "U" || result.Text == "S" || result.Text == "Z" || result.Text == "O" || result.Text == "X" || result.Text == "Ø" || result.Text == "C")
            {
                if (this._nocrLastLowercaseHeight > 3 && targetItem.NikseBitmap.Height - this._nocrLastLowercaseHeight < 2)
                {
                    result.Text = result.Text.ToLower();
                }
            }
            else if (result.Text == "v" || result.Text == "w" || result.Text == "u" || result.Text == "s" || result.Text == "z" || result.Text == "o" || result.Text == "x" || result.Text == "ø" || result.Text == "c")
            {
                if (this._nocrLastUppercaseHeight > 3 && this._nocrLastUppercaseHeight - targetItem.NikseBitmap.Height < 2)
                {
                    result.Text = result.Text.ToUpper();
                }
            }

            if (italic)
            {
                return new CompareMatch(result.Text, true, 0, null, result);
            }
            else
            {
                return new CompareMatch(result.Text, result.Italic, 0, null, result);
            }
        }

        /// <summary>
        /// The get n ocr compare match new.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="parentBitmap">
        /// The parent bitmap.
        /// </param>
        /// <param name="nOcrDb">
        /// The n ocr db.
        /// </param>
        /// <param name="tryItalicScaling">
        /// The try italic scaling.
        /// </param>
        /// <param name="deepSeek">
        /// The deep seek.
        /// </param>
        /// <returns>
        /// The <see cref="CompareMatch"/>.
        /// </returns>
        internal CompareMatch GetNOcrCompareMatchNew(ImageSplitterItem targetItem, NikseBitmap parentBitmap, NOcrDb nOcrDb, bool tryItalicScaling, bool deepSeek)
        {
            var expandedResult = NOcrFindExpandedMatch(parentBitmap, targetItem, nOcrDb.OcrCharactersExpanded);
            if (expandedResult != null)
            {
                return new CompareMatch(expandedResult.Text, expandedResult.Italic, expandedResult.ExpandCount, null, expandedResult);
            }

            bool italic;
            var result = NOcrFindBestMatchNew(targetItem, targetItem.Y - targetItem.ParentY, out italic, nOcrDb, tryItalicScaling, deepSeek);
            if (result == null)
            {
                if (this.checkBoxNOcrCorrect.Checked)
                {
                    return null;
                }

                return new CompareMatch("*", false, 0, null);
            }

            // Fix uppercase/lowercase issues (not I/l)
            if (result.Text == "e")
            {
                this._nocrLastLowercaseHeight = targetItem.NikseBitmap.Height;
            }
            else if (this._nocrLastLowercaseHeight == -1 && result.Text == "a")
            {
                this._nocrLastLowercaseHeight = targetItem.NikseBitmap.Height;
            }

            if (result.Text == "E" || result.Text == "H" || result.Text == "R" || result.Text == "D" || result.Text == "T")
            {
                this._nocrLastUppercaseHeight = targetItem.NikseBitmap.Height;
            }
            else if (this._nocrLastUppercaseHeight == -1 && result.Text == "M")
            {
                this._nocrLastUppercaseHeight = targetItem.NikseBitmap.Height;
            }

            if (result.Text == "V" || result.Text == "W" || result.Text == "U" || result.Text == "S" || result.Text == "Z" || result.Text == "O" || result.Text == "X" || result.Text == "Ø" || result.Text == "C")
            {
                if (this._nocrLastLowercaseHeight > 3 && targetItem.NikseBitmap.Height - this._nocrLastLowercaseHeight < 2)
                {
                    result.Text = result.Text.ToLower();
                }
            }
            else if (result.Text == "v" || result.Text == "w" || result.Text == "u" || result.Text == "s" || result.Text == "z" || result.Text == "o" || result.Text == "x" || result.Text == "ø" || result.Text == "c")
            {
                if (this._nocrLastUppercaseHeight > 3 && this._nocrLastUppercaseHeight - targetItem.NikseBitmap.Height < 2)
                {
                    result.Text = result.Text.ToUpper();
                }
            }

            if (italic)
            {
                return new CompareMatch(result.Text, true, 0, null, result);
            }
            else
            {
                return new CompareMatch(result.Text, result.Italic, 0, null, result);
            }
        }

        /// <summary>
        /// The get n ocr compare match.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="parentBitmap">
        /// The parent bitmap.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="CompareMatch"/>.
        /// </returns>
        internal static CompareMatch GetNOcrCompareMatch(ImageSplitterItem targetItem, NikseBitmap parentBitmap, NOcrThreadParameter p)
        {
            bool italic;
            var expandedResult = NOcrFindExpandedMatch(parentBitmap, targetItem, p.NOcrChars);
            if (expandedResult != null)
            {
                return new CompareMatch(expandedResult.Text, expandedResult.Italic, expandedResult.ExpandCount, null, expandedResult);
            }

            var result = NOcrFindBestMatch(targetItem, targetItem.Y - targetItem.ParentY, out italic, p.NOcrChars, p.UnItalicFactor, p.AdvancedItalicDetection, true);
            if (result == null)
            {
                return null;
            }

            // Fix uppercase/lowercase issues (not I/l)
            if (result.Text == "e")
            {
                p.NOcrLastLowercaseHeight = targetItem.NikseBitmap.Height;
            }
            else if (p.NOcrLastLowercaseHeight == -1 && result.Text == "a")
            {
                p.NOcrLastLowercaseHeight = targetItem.NikseBitmap.Height;
            }

            if (result.Text == "E" || result.Text == "H" || result.Text == "R" || result.Text == "D" || result.Text == "T")
            {
                p.NOcrLastUppercaseHeight = targetItem.NikseBitmap.Height;
            }
            else if (p.NOcrLastUppercaseHeight == -1 && result.Text == "M")
            {
                p.NOcrLastUppercaseHeight = targetItem.NikseBitmap.Height;
            }

            if (result.Text == "V" || result.Text == "W" || result.Text == "U" || result.Text == "S" || result.Text == "Z" || result.Text == "O" || result.Text == "X" || result.Text == "Ø" || result.Text == "C")
            {
                if (p.NOcrLastLowercaseHeight > 3 && targetItem.NikseBitmap.Height - p.NOcrLastLowercaseHeight < 2)
                {
                    result.Text = result.Text.ToLower();
                }
            }
            else if (result.Text == "v" || result.Text == "w" || result.Text == "u" || result.Text == "s" || result.Text == "z" || result.Text == "o" || result.Text == "x" || result.Text == "ø" || result.Text == "c")
            {
                if (p.NOcrLastUppercaseHeight > 3 && p.NOcrLastUppercaseHeight - targetItem.NikseBitmap.Height < 2)
                {
                    result.Text = result.Text.ToUpper();
                }
            }

            if (italic)
            {
                return new CompareMatch(result.Text, true, 0, null, result);
            }

            return new CompareMatch(result.Text, result.Italic, 0, null, result);
        }

        /// <summary>
        /// The get compare match.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="parentBitmap">
        /// The parent bitmap.
        /// </param>
        /// <param name="secondBestGuess">
        /// The second best guess.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="listIndex">
        /// The list index.
        /// </param>
        /// <returns>
        /// The <see cref="CompareMatch"/>.
        /// </returns>
        private CompareMatch GetCompareMatch(ImageSplitterItem targetItem, NikseBitmap parentBitmap, out CompareMatch secondBestGuess, List<ImageSplitterItem> list, int listIndex)
        {
            secondBestGuess = null;
            int index = 0;
            int smallestDifference = 10000;
            int smallestIndex = -1;
            NikseBitmap target = targetItem.NikseBitmap;
            if (this._compareBitmaps == null)
            {
                return null;
            }

            foreach (CompareItem compareItem in this._compareBitmaps)
            {
                // check for expand match!
                if (compareItem.ExpandCount > 0 && compareItem.Bitmap.Width > target.Width && parentBitmap.Width >= compareItem.Bitmap.Width + targetItem.X)
                {
                    // &&   parentBitmap.Height >= compareItem.Bitmap.Height + targetItem.Y) //NIXE-debug-test- what not correct?
                    int minY = targetItem.Y;
                    for (int j = 1; j < compareItem.ExpandCount; j++)
                    {
                        if (list != null && list.Count > listIndex + j && list[listIndex + j].Y < minY)
                        {
                            minY = list[listIndex + j].Y;
                        }
                    }

                    if (parentBitmap.Height >= compareItem.Bitmap.Height + minY)
                    {
                        var cutBitmap = parentBitmap.CopyRectangle(new Rectangle(targetItem.X, minY, compareItem.Bitmap.Width, compareItem.Bitmap.Height));
                        int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, cutBitmap);
                        if (dif < smallestDifference && (Math.Abs(target.Height - compareItem.Bitmap.Height) <= 5 || compareItem.Text != "\""))
                        {
                            smallestDifference = dif;
                            smallestIndex = index;
                            if (dif == 0)
                            {
                                break; // foreach ending
                            }
                        }
                    }
                }

                index++;
            }

            // Search images with minor location changes
            FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, target, this._compareBitmaps);

            if (smallestDifference * 100.0 / (target.Width * target.Height) > this._vobSubOcrSettings.AllowDifferenceInPercent && target.Width < 70)
            {
                if (smallestDifference > 2 && target.Width > 25)
                {
                    var cutBitmap = target.CopyRectangle(new Rectangle(4, 0, target.Width - 4, target.Height));
                    FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, cutBitmap, this._compareBitmaps);
                }

                if (smallestDifference > 2 && target.Width > 12)
                {
                    var cutBitmap = target.CopyRectangle(new Rectangle(1, 0, target.Width - 2, target.Height));
                    FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, cutBitmap, this._compareBitmaps);
                }

                if (smallestDifference > 2 && target.Width > 12)
                {
                    var cutBitmap = target.CopyRectangle(new Rectangle(0, 0, target.Width - 2, target.Height));
                    FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, cutBitmap, this._compareBitmaps);
                }

                if (smallestDifference > 2 && target.Width > 12)
                {
                    var cutBitmap = target.CopyRectangle(new Rectangle(1, 0, target.Width - 2, target.Height));
                    int topCrop = 0;
                    var cutBitmap2 = NikseBitmapImageSplitter.CropTopAndBottom(cutBitmap, out topCrop, 2);
                    if (cutBitmap2.Height != target.Height)
                    {
                        FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, cutBitmap2, this._compareBitmaps);
                    }
                }

                if (smallestDifference > 2 && target.Width > 15)
                {
                    var cutBitmap = target.CopyRectangle(new Rectangle(1, 0, target.Width - 2, target.Height));
                    int topCrop = 0;
                    var cutBitmap2 = NikseBitmapImageSplitter.CropTopAndBottom(cutBitmap, out topCrop);
                    if (cutBitmap2.Height != target.Height)
                    {
                        FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, cutBitmap2, this._compareBitmaps);
                    }
                }

                if (smallestDifference > 2 && target.Width > 15)
                {
                    var cutBitmap = target.CopyRectangle(new Rectangle(1, 0, target.Width - 2, target.Height));
                    int topCrop = 0;
                    var cutBitmap2 = NikseBitmapImageSplitter.CropTopAndBottom(cutBitmap, out topCrop);
                    if (cutBitmap2.Height != target.Height)
                    {
                        FindBestMatch(ref index, ref smallestDifference, ref smallestIndex, cutBitmap2, this._compareBitmaps);
                    }
                }
            }

            if (smallestIndex >= 0)
            {
                double differencePercentage = smallestDifference * 100.0 / (target.Width * target.Height);
                double maxDiff = (double)this.numericUpDownMaxErrorPct.Value;
                if (differencePercentage <= maxDiff)
                {
                    var hit = this._compareBitmaps[smallestIndex];
                    return new CompareMatch(hit.Text, hit.Italic, hit.ExpandCount, hit.Name);
                }

                var guess = this._compareBitmaps[smallestIndex];
                secondBestGuess = new CompareMatch(guess.Text, guess.Italic, guess.ExpandCount, guess.Name);
            }

            return null;
        }

        /// <summary>
        /// The get compare match new.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="secondBestGuess">
        /// The second best guess.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="listIndex">
        /// The list index.
        /// </param>
        /// <returns>
        /// The <see cref="CompareMatch"/>.
        /// </returns>
        private CompareMatch GetCompareMatchNew(ImageSplitterItem targetItem, out CompareMatch secondBestGuess, List<ImageSplitterItem> list, int listIndex)
        {
            double maxDiff = (double)this.numericUpDownMaxErrorPct.Value;
            secondBestGuess = null;
            int index = 0;
            int smallestDifference = 10000;
            int smallestIndex = -1;
            NikseBitmap target = targetItem.NikseBitmap;
            if (this._binaryOcrDb == null)
            {
                return null;
            }

            var bob = new BinaryOcrBitmap(target);

            for (int k = 0; k < this._binaryOcrDb.CompareImagesExpanded.Count; k++)
            {
                var b = this._binaryOcrDb.CompareImagesExpanded[k];
                if (bob.Hash == b.Hash && bob.Width == b.Width && bob.Height == b.Height && bob.NumberOfColoredPixels == b.NumberOfColoredPixels)
                {
                    bool ok = false;
                    for (int i = 0; i < b.ExpandedList.Count; i++)
                    {
                        if (listIndex + i + 1 < list.Count && list[listIndex + i + 1].NikseBitmap != null && b.ExpandedList[i].Hash == new BinaryOcrBitmap(list[listIndex + i + 1].NikseBitmap).Hash)
                        {
                            ok = true;
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        secondBestGuess = null;
                        return new CompareMatch(b.Text, b.Italic, b.ExpandCount, b.Key);
                    }
                }
            }

            FindBestMatchNew(ref index, ref smallestDifference, ref smallestIndex, target, this._binaryOcrDb, bob, maxDiff);

            if (smallestIndex >= 0)
            {
                double differencePercentage = smallestDifference * 100.0 / (target.Width * target.Height);
                if (differencePercentage <= maxDiff)
                {
                    var hit = this._binaryOcrDb.CompareImages[smallestIndex];

                    string text = hit.Text;
                    if (smallestDifference > 0)
                    {
                        int h = hit.Height;
                        if (text == "V" || text == "W" || text == "U" || text == "S" || text == "Z" || text == "O" || text == "X" || text == "Ø" || text == "C")
                        {
                            if (this._binOcrLastLowercaseHeight > 3 && h - this._binOcrLastLowercaseHeight < 2)
                            {
                                text = text.ToLower();
                            }
                        }
                        else if (text == "v" || text == "w" || text == "u" || text == "s" || text == "z" || text == "o" || text == "x" || text == "ø" || text == "c")
                        {
                            if (this._binOcrLastUppercaseHeight > 3 && this._binOcrLastUppercaseHeight - h < 2)
                            {
                                text = text.ToUpper();
                            }
                        }
                    }
                    else
                    {
                        this.SetBinOcrLowercaseUppercase(hit.Height, text);
                    }

                    return new CompareMatch(text, hit.Italic, hit.ExpandCount, hit.Key);
                }

                var guess = this._binaryOcrDb.CompareImages[smallestIndex];
                secondBestGuess = new CompareMatch(guess.Text, guess.Italic, guess.ExpandCount, guess.Key);
            }

            return null;
        }

        /// <summary>
        /// The copy bitmap section.
        /// </summary>
        /// <param name="srcBitmap">
        /// The src bitmap.
        /// </param>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public static Bitmap CopyBitmapSection(Bitmap srcBitmap, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            g.Dispose();
            return bmp;
        }

        /// <summary>
        /// The find best match new.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="smallestDifference">
        /// The smallest difference.
        /// </param>
        /// <param name="smallestIndex">
        /// The smallest index.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="binOcrDb">
        /// The bin ocr db.
        /// </param>
        /// <param name="bob">
        /// The bob.
        /// </param>
        /// <param name="maxDiff">
        /// The max diff.
        /// </param>
        private static void FindBestMatchNew(ref int index, ref int smallestDifference, ref int smallestIndex, NikseBitmap target, BinaryOcrDb binOcrDb, BinaryOcrBitmap bob, double maxDiff)
        {
            var bobExactMatch = binOcrDb.FindExactMatch(bob);
            if (bobExactMatch >= 0)
            {
                index = bobExactMatch;
                smallestDifference = 0;
                smallestIndex = bobExactMatch;
                return;
            }

            if (maxDiff < 0.2 || target.Width < 3 || target.Height < 5)
            {
                return;
            }

            int numberOfForegroundColors = bob.NumberOfColoredPixels;
            const int minForeColorMatch = 90;

            index = 0;
            foreach (var compareItem in binOcrDb.CompareImages)
            {
                if (compareItem.Width == target.Width && compareItem.Height == target.Height)
                {
                    // precise math in size
                    if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < 3)
                    {
                        int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                        if (dif < smallestDifference)
                        {
                            smallestDifference = dif;
                            smallestIndex = index;
                            if (dif < 3)
                            {
                                break; // foreach ending
                            }
                        }
                    }
                }

                index++;
            }

            if (smallestDifference > 1)
            {
                index = 0;
                foreach (var compareItem in binOcrDb.CompareImages)
                {
                    if (compareItem.Width == target.Width && compareItem.Height == target.Height)
                    {
                        // precise math in size
                        if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < 40)
                        {
                            int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                            if (dif < smallestDifference)
                            {
                                smallestDifference = dif;
                                smallestIndex = index;
                                if (dif == 0)
                                {
                                    break; // foreach ending
                                }
                            }
                        }
                    }

                    index++;
                }
            }

            if (target.Width > 5 && smallestDifference > 2)
            {
                // for other than very narrow letter (like 'i' and 'l' and 'I'), try more sizes
                index = 0;
                foreach (var compareItem in binOcrDb.CompareImages)
                {
                    if (compareItem.Width == target.Width && compareItem.Height == target.Height - 1)
                    {
                        if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                        {
                            int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                            if (dif < smallestDifference)
                            {
                                smallestDifference = dif;
                                smallestIndex = index;
                                if (dif == 0)
                                {
                                    break; // foreach ending
                                }
                            }
                        }
                    }

                    index++;
                }

                if (smallestDifference > 2)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width && compareItem.Height == target.Height + 1)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 3)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width + 1 && compareItem.Height == target.Height + 1)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 5)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width - 1 && compareItem.Height == target.Height)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 5)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width - 1 && compareItem.Height == target.Height - 1)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 5)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width - 1 == target.Width && compareItem.Height == target.Height)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 11)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width - 2 && compareItem.Height == target.Height)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 14)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width - 3 && compareItem.Height == target.Height)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 14)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == target.Width && compareItem.Height == target.Height - 3)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 14)
                {
                    index = 0;
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width - 2 == target.Width && compareItem.Height == target.Height)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }
            }

            if (smallestDifference == 0)
            {
                if (smallestIndex > 200)
                {
                    var hit = binOcrDb.CompareImages[smallestIndex];
                    binOcrDb.CompareImages.RemoveAt(smallestIndex);
                    binOcrDb.CompareImages.Insert(0, hit);
                    smallestIndex = 0;
                    index = 0;
                }
            }
        }

        /// <summary>
        /// The find best match.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="smallestDifference">
        /// The smallest difference.
        /// </param>
        /// <param name="smallestIndex">
        /// The smallest index.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="compareBitmaps">
        /// The compare bitmaps.
        /// </param>
        private static void FindBestMatch(ref int index, ref int smallestDifference, ref int smallestIndex, NikseBitmap target, List<CompareItem> compareBitmaps)
        {
            int numberOfForegroundColors = CalculateNumberOfForegroundColors(target);
            const int minForeColorMatch = 90;

            index = 0;
            foreach (CompareItem compareItem in compareBitmaps)
            {
                if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height)
                {
                    // precise math in size
                    if (compareItem.NumberOfForegroundColors < 1)
                    {
                        compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                    }

                    if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < 3)
                    {
                        int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                        if (dif < smallestDifference)
                        {
                            smallestDifference = dif;
                            smallestIndex = index;
                            if (dif < 3)
                            {
                                break; // foreach ending
                            }
                        }
                    }
                }

                index++;
            }

            if (smallestDifference > 1)
            {
                index = 0;
                foreach (CompareItem compareItem in compareBitmaps)
                {
                    if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height)
                    {
                        // precise math in size
                        if (compareItem.NumberOfForegroundColors < 1)
                        {
                            compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                        }

                        if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < 40)
                        {
                            int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                            if (dif < smallestDifference)
                            {
                                smallestDifference = dif;
                                smallestIndex = index;
                                if (dif == 0)
                                {
                                    break; // foreach ending
                                }
                            }
                        }
                    }

                    index++;
                }
            }

            if (target.Width > 5 && smallestDifference > 2)
            {
                // for other than very narrow letter (like 'i' and 'l' and 'I'), try more sizes
                index = 0;
                foreach (CompareItem compareItem in compareBitmaps)
                {
                    if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height - 1)
                    {
                        if (compareItem.NumberOfForegroundColors == -1)
                        {
                            compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                        }

                        if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                        {
                            int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                            if (dif < smallestDifference)
                            {
                                smallestDifference = dif;
                                smallestIndex = index;
                                if (dif == 0)
                                {
                                    break; // foreach ending
                                }
                            }
                        }
                    }

                    index++;
                }

                if (smallestDifference > 2)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height + 1)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem.Bitmap);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 3)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width + 1 && compareItem.Bitmap.Height == target.Height + 1)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem.Bitmap);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 5)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width - 1 && compareItem.Bitmap.Height == target.Height)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 5)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width - 1 && compareItem.Bitmap.Height == target.Height - 1)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 5)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width - 1 == target.Width && compareItem.Bitmap.Height == target.Height)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem.Bitmap);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 10)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width - 2 && compareItem.Bitmap.Height == target.Height)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 12)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width - 3 && compareItem.Bitmap.Height == target.Height)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9 && target.Width > 12)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height - 3)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }

                if (smallestDifference > 9)
                {
                    index = 0;
                    foreach (CompareItem compareItem in compareBitmaps)
                    {
                        if (compareItem.Bitmap.Width - 2 == target.Width && compareItem.Bitmap.Height == target.Height)
                        {
                            if (compareItem.NumberOfForegroundColors == -1)
                            {
                                compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                            }

                            if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem.Bitmap);
                                if (dif < smallestDifference)
                                {
                                    smallestDifference = dif;
                                    smallestIndex = index;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }

                        index++;
                    }
                }
            }

            if (smallestDifference == 0)
            {
                if (smallestIndex > 200)
                {
                    CompareItem hit = compareBitmaps[smallestIndex];
                    compareBitmaps.RemoveAt(smallestIndex);
                    compareBitmaps.Insert(0, hit);
                    smallestIndex = 0;
                    index = 0;
                }
            }
        }

        /// <summary>
        /// The calculate number of foreground colors.
        /// </summary>
        /// <param name="nikseBitmap">
        /// The nikse bitmap.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalculateNumberOfForegroundColors(NikseBitmap nikseBitmap)
        {
            int count = 0;
            for (int y = 0; y < nikseBitmap.Height; y++)
            {
                for (int x = 0; x < nikseBitmap.Width; x++)
                {
                    Color c = nikseBitmap.GetPixel(x, y);
                    if (c.A > 100 && c.R + c.G + c.B > 200)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// The calculate number of foreground colors.
        /// </summary>
        /// <param name="managedBitmap">
        /// The managed bitmap.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalculateNumberOfForegroundColors(ManagedBitmap managedBitmap)
        {
            int count = 0;
            for (int y = 0; y < managedBitmap.Height; y++)
            {
                for (int x = 0; x < managedBitmap.Width; x++)
                {
                    Color c = managedBitmap.GetPixel(x, y);
                    if (c.A > 100 && c.R + c.G + c.B > 200)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// The save compare item.
        /// </summary>
        /// <param name="newTarget">
        /// The new target.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="isItalic">
        /// The is italic.
        /// </param>
        /// <param name="expandCount">
        /// The expand count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SaveCompareItem(NikseBitmap newTarget, string text, bool isItalic, int expandCount)
        {
            string path = Configuration.VobSubCompareFolder + this.comboBoxCharacterDatabase.SelectedItem + Path.DirectorySeparatorChar;
            string databaseName = path + "Images.db";
            FileStream f;
            long pos;
            if (!File.Exists(databaseName))
            {
                if (!Directory.Exists(Configuration.OcrFolder))
                {
                    Directory.CreateDirectory(Configuration.OcrFolder);
                }

                using (f = new FileStream(databaseName, FileMode.Create))
                {
                    pos = f.Position;
                    new ManagedBitmap(newTarget).AppendToStream(f);
                }
            }
            else
            {
                using (f = new FileStream(databaseName, FileMode.Append))
                {
                    pos = f.Position;
                    new ManagedBitmap(newTarget).AppendToStream(f);
                }
            }

            string name = pos.ToString(CultureInfo.InvariantCulture);

            if (this._compareDoc == null)
            {
                this._compareDoc = new XmlDocument();
                this._compareDoc.LoadXml("<OcrBitmaps></OcrBitmaps>");
            }

            if (this._compareBitmaps == null)
            {
                this._compareBitmaps = new List<CompareItem>();
            }

            this._compareBitmaps.Add(new CompareItem(new ManagedBitmap(newTarget), name, isItalic, expandCount, text));

            XmlElement element = this._compareDoc.CreateElement("Item");
            XmlAttribute attribute = this._compareDoc.CreateAttribute("Text");
            attribute.InnerText = text;
            element.Attributes.Append(attribute);
            if (expandCount > 0)
            {
                XmlAttribute expandSelection = this._compareDoc.CreateAttribute("Expand");
                expandSelection.InnerText = expandCount.ToString(CultureInfo.InvariantCulture);
                element.Attributes.Append(expandSelection);
            }

            if (isItalic)
            {
                XmlAttribute italic = this._compareDoc.CreateAttribute("Italic");
                italic.InnerText = "true";
                element.Attributes.Append(italic);
            }

            element.InnerText = pos.ToString(CultureInfo.InvariantCulture);
            this._compareDoc.DocumentElement.AppendChild(element);
            this._compareDoc.Save(path + "Images.xml");
            return name;
        }

        /// <summary>
        /// The save compare item new.
        /// </summary>
        /// <param name="newTarget">
        /// The new target.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="isItalic">
        /// The is italic.
        /// </param>
        /// <param name="expandList">
        /// The expand list.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SaveCompareItemNew(ImageSplitterItem newTarget, string text, bool isItalic, List<ImageSplitterItem> expandList)
        {
            int expandCount = 0;
            if (expandList != null)
            {
                expandCount = expandList.Count;
            }

            if (expandCount > 0)
            {
                var bob = new BinaryOcrBitmap(expandList[0].NikseBitmap, isItalic, expandCount, text, expandList[0].X, expandList[0].Y);
                bob.ExpandedList = new List<BinaryOcrBitmap>();
                for (int j = 1; j < expandList.Count; j++)
                {
                    var expandedBob = new BinaryOcrBitmap(expandList[j].NikseBitmap);
                    expandedBob.X = expandList[j].X;
                    expandedBob.Y = expandList[j].Y;
                    bob.ExpandedList.Add(expandedBob);
                }

                this._binaryOcrDb.Add(bob);
                this._binaryOcrDb.Save();
                return bob.Key;
            }
            else
            {
                var bob = new BinaryOcrBitmap(newTarget.NikseBitmap, isItalic, expandCount, text, newTarget.X, newTarget.Y);
                this._binaryOcrDb.Add(bob);
                this._binaryOcrDb.Save();
                return bob.Key;
            }
        }

        /// <summary>
        /// Ocr via image compare
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="listViewIndex">
        /// The list View Index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SplitAndOcrBitmapNormal(Bitmap bitmap, int listViewIndex)
        {
            if (this._ocrFixEngine == null)
            {
                this.LoadOcrFixEngine(null, this.LanguageString);
            }

            string threadText = null;
            if (this._icThreadResults != null && !string.IsNullOrEmpty(this._icThreadResults[listViewIndex]))
            {
                threadText = this._icThreadResults[listViewIndex];
            }

            string line = string.Empty;
            if (threadText == null)
            {
                var matches = new List<CompareMatch>();
                var parentBitmap = new NikseBitmap(bitmap);
                List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLetters(parentBitmap, (int)this.numericUpDownPixelsIsSpace.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom);
                int index = 0;
                bool expandSelection = false;
                bool shrinkSelection = false;
                var expandSelectionList = new List<ImageSplitterItem>();
                while (index < list.Count)
                {
                    ImageSplitterItem item = list[index];
                    if (expandSelection || shrinkSelection)
                    {
                        expandSelection = false;
                        if (shrinkSelection && index > 0)
                        {
                            shrinkSelection = false;
                        }
                        else if (index + 1 < list.Count && list[index + 1].NikseBitmap != null)
                        {
                            // only allow expand to EndOfLine or space
                            index++;
                            expandSelectionList.Add(list[index]);
                        }

                        item = GetExpandedSelection(parentBitmap, expandSelectionList, this.checkBoxRightToLeft.Checked);

                        this._vobSubOcrCharacter.Initialize(bitmap, item, this._manualOcrDialogPosition, this._italicCheckedLast, expandSelectionList.Count > 1, null, this._lastAdditions, this);
                        DialogResult result = this._vobSubOcrCharacter.ShowDialog(this);
                        this._manualOcrDialogPosition = this._vobSubOcrCharacter.FormPosition;
                        if (result == DialogResult.OK && this._vobSubOcrCharacter.ShrinkSelection)
                        {
                            shrinkSelection = true;
                            index--;
                            if (expandSelectionList.Count > 0)
                            {
                                expandSelectionList.RemoveAt(expandSelectionList.Count - 1);
                            }
                        }
                        else if (result == DialogResult.OK && this._vobSubOcrCharacter.ExpandSelection)
                        {
                            expandSelection = true;
                        }
                        else if (result == DialogResult.OK)
                        {
                            string text = this._vobSubOcrCharacter.ManualRecognizedCharacters;
                            string name = this.SaveCompareItem(item.NikseBitmap, text, this._vobSubOcrCharacter.IsItalic, expandSelectionList.Count);
                            var addition = new ImageCompareAddition(name, text, item.NikseBitmap, this._vobSubOcrCharacter.IsItalic, listViewIndex);
                            this._lastAdditions.Add(addition);
                            matches.Add(new CompareMatch(text, this._vobSubOcrCharacter.IsItalic, expandSelectionList.Count, null));
                            expandSelectionList = new List<ImageSplitterItem>();
                        }
                        else if (result == DialogResult.Abort)
                        {
                            this._abort = true;
                        }
                        else
                        {
                            matches.Add(new CompareMatch("*", false, 0, null));
                        }

                        this._italicCheckedLast = this._vobSubOcrCharacter.IsItalic;
                    }
                    else if (item.NikseBitmap == null)
                    {
                        matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                    }
                    else
                    {
                        CompareMatch bestGuess;
                        CompareMatch match = this.GetCompareMatch(item, parentBitmap, out bestGuess, list, index);
                        if (match == null)
                        {
                            this._vobSubOcrCharacter.Initialize(bitmap, item, this._manualOcrDialogPosition, this._italicCheckedLast, false, bestGuess, this._lastAdditions, this);
                            DialogResult result = this._vobSubOcrCharacter.ShowDialog(this);
                            this._manualOcrDialogPosition = this._vobSubOcrCharacter.FormPosition;
                            if (result == DialogResult.OK && this._vobSubOcrCharacter.ExpandSelection)
                            {
                                expandSelectionList.Add(item);
                                expandSelection = true;
                            }
                            else if (result == DialogResult.OK)
                            {
                                string text = this._vobSubOcrCharacter.ManualRecognizedCharacters;
                                string name = this.SaveCompareItem(item.NikseBitmap, text, this._vobSubOcrCharacter.IsItalic, 0);
                                var addition = new ImageCompareAddition(name, text, item.NikseBitmap, this._vobSubOcrCharacter.IsItalic, listViewIndex);
                                this._lastAdditions.Add(addition);
                                matches.Add(new CompareMatch(text, this._vobSubOcrCharacter.IsItalic, 0, null));
                            }
                            else if (result == DialogResult.Abort)
                            {
                                this._abort = true;
                            }
                            else
                            {
                                matches.Add(new CompareMatch("*", false, 0, null));
                            }

                            this._italicCheckedLast = this._vobSubOcrCharacter.IsItalic;
                        }
                        else
                        {
                            // found image match
                            matches.Add(new CompareMatch(match.Text, match.Italic, 0, null));
                            if (match.ExpandCount > 0)
                            {
                                index += match.ExpandCount - 1;
                            }
                        }
                    }

                    if (this._abort)
                    {
                        return string.Empty;
                    }

                    if (!expandSelection && !shrinkSelection)
                    {
                        index++;
                    }

                    if (shrinkSelection && expandSelectionList.Count < 2)
                    {
                        shrinkSelection = false;
                        expandSelectionList = new List<ImageSplitterItem>();
                    }
                }

                line = GetStringWithItalicTags(matches);
            }
            else
            {
                line = threadText;
            }

            if (this.checkBoxAutoFixCommonErrors.Checked && this._ocrFixEngine != null)
            {
                line = this._ocrFixEngine.FixOcrErrorsViaHardcodedRules(line, this._lastLine, null); // TODO: Add abbreviations list
            }

            if (this.checkBoxRightToLeft.Checked)
            {
                line = ReverseNumberStrings(line);
            }

            // OCR fix engine
            string textWithOutFixes = line;

            // OCR fix engine not loaded, when no dictionary is selected
            if (this._ocrFixEngine != null && this._ocrFixEngine.IsDictionaryLoaded)
            {
                if (this.checkBoxAutoFixCommonErrors.Checked)
                {
                    line = this._ocrFixEngine.FixOcrErrors(line, listViewIndex, this._lastLine, true, this.GetAutoGuessLevel());
                }

                int correctWords;
                int wordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(line, out correctWords);

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && string.IsNullOrWhiteSpace(textWithOutFixes.Replace("~", string.Empty)))
                {
                    this._ocrFixEngine.AutoGuessesUsed.Clear();
                    this._ocrFixEngine.UnknownWordsFound.Clear();
                    line = this._ocrFixEngine.FixUnknownWordsViaGuessOrPrompt(out wordsNotFound, line, listViewIndex, bitmap, this.checkBoxAutoFixCommonErrors.Checked, this.checkBoxPromptForUnknownWords.Checked, true, this.GetAutoGuessLevel());
                }

                if (this._ocrFixEngine.Abort)
                {
                    this.ButtonStopClick(null, null);
                    this._ocrFixEngine.Abort = false;
                    return string.Empty;
                }

                // Log used word guesses (via word replace list)
                foreach (string guess in this._ocrFixEngine.AutoGuessesUsed)
                {
                    this.listBoxLogSuggestions.Items.Add(guess);
                }

                this._ocrFixEngine.AutoGuessesUsed.Clear();

                // Log unkown words guess (found via spelling dictionaries)
                this.LogUnknownWords();

                if (wordsNotFound >= 3)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Red);
                }

                if (wordsNotFound == 2)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Orange);
                }
                else if (wordsNotFound == 1)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Yellow);
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Orange);
                }
                else
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.LightGreen);
                }
            }

            if (textWithOutFixes.Trim() != line.Trim())
            {
                this._tesseractOcrAutoFixes++;
                this.labelFixesMade.Text = string.Format(" - {0}", this._tesseractOcrAutoFixes);
                this.LogOcrFix(listViewIndex, textWithOutFixes, line);
            }

            return line;
        }

        /// <summary>
        /// Ocr via image compare
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="listViewIndex">
        /// The list View Index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SplitAndOcrBitmapNormalNew(Bitmap bitmap, int listViewIndex)
        {
            if (this._ocrFixEngine == null)
            {
                this.LoadOcrFixEngine(null, this.LanguageString);
            }

            string line = string.Empty;
            var matches = new List<CompareMatch>();
            var parentBitmap = new NikseBitmap(bitmap);
            int minLineHeight = this._binOcrLastLowercaseHeight - 3;
            if (minLineHeight < 5)
            {
                minLineHeight = this._nocrLastLowercaseHeight;
            }

            if (minLineHeight < 5)
            {
                minLineHeight = 6;
            }

            List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLettersNew(parentBitmap, (int)this.numericUpDownPixelsIsSpace.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight);
            int index = 0;
            bool expandSelection = false;
            bool shrinkSelection = false;
            var expandSelectionList = new List<ImageSplitterItem>();
            while (index < list.Count)
            {
                ImageSplitterItem item = list[index];
                if (expandSelection || shrinkSelection)
                {
                    expandSelection = false;
                    if (shrinkSelection && index > 0)
                    {
                        shrinkSelection = false;
                    }
                    else if (index + 1 < list.Count && list[index + 1].NikseBitmap != null)
                    {
                        // only allow expand to EndOfLine or space
                        index++;
                        expandSelectionList.Add(list[index]);
                    }

                    item = GetExpandedSelectionNew(parentBitmap, expandSelectionList);

                    this._vobSubOcrCharacter.Initialize(bitmap, item, this._manualOcrDialogPosition, this._italicCheckedLast, expandSelectionList.Count > 1, null, this._lastAdditions, this);
                    DialogResult result = this._vobSubOcrCharacter.ShowDialog(this);
                    this._manualOcrDialogPosition = this._vobSubOcrCharacter.FormPosition;
                    if (result == DialogResult.OK && this._vobSubOcrCharacter.ShrinkSelection)
                    {
                        shrinkSelection = true;
                        index--;
                        if (expandSelectionList.Count > 0)
                        {
                            expandSelectionList.RemoveAt(expandSelectionList.Count - 1);
                        }
                    }
                    else if (result == DialogResult.OK && this._vobSubOcrCharacter.ExpandSelection)
                    {
                        expandSelection = true;
                    }
                    else if (result == DialogResult.OK)
                    {
                        string text = this._vobSubOcrCharacter.ManualRecognizedCharacters;
                        string name = this.SaveCompareItemNew(item, text, this._vobSubOcrCharacter.IsItalic, expandSelectionList);
                        var addition = new ImageCompareAddition(name, text, item.NikseBitmap, this._vobSubOcrCharacter.IsItalic, listViewIndex);
                        this._lastAdditions.Add(addition);
                        matches.Add(new CompareMatch(text, this._vobSubOcrCharacter.IsItalic, expandSelectionList.Count, null));
                        expandSelectionList = new List<ImageSplitterItem>();
                    }
                    else if (result == DialogResult.Abort)
                    {
                        this._abort = true;
                    }
                    else
                    {
                        matches.Add(new CompareMatch("*", false, 0, null));
                    }

                    this._italicCheckedLast = this._vobSubOcrCharacter.IsItalic;
                }
                else if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                }
                else
                {
                    CompareMatch bestGuess;
                    CompareMatch match = this.GetCompareMatchNew(item, out bestGuess, list, index);
                    if (match == null)
                    {
                        // Try line OCR if no image compare match
                        if (this._nOcrDb != null && this._nOcrDb.OcrCharacters.Count > 0)
                        {
                            match = this.GetNOcrCompareMatchNew(item, parentBitmap, this._nOcrDb, true, true);
                        }
                    }

                    if (match == null)
                    {
                        this._vobSubOcrCharacter.Initialize(bitmap, item, this._manualOcrDialogPosition, this._italicCheckedLast, false, bestGuess, this._lastAdditions, this);
                        DialogResult result = this._vobSubOcrCharacter.ShowDialog(this);
                        this._manualOcrDialogPosition = this._vobSubOcrCharacter.FormPosition;
                        if (result == DialogResult.OK && this._vobSubOcrCharacter.ExpandSelection)
                        {
                            expandSelectionList.Add(item);
                            expandSelection = true;
                        }
                        else if (result == DialogResult.OK)
                        {
                            string text = this._vobSubOcrCharacter.ManualRecognizedCharacters;
                            string name = this.SaveCompareItemNew(item, text, this._vobSubOcrCharacter.IsItalic, null);
                            var addition = new ImageCompareAddition(name, text, item.NikseBitmap, this._vobSubOcrCharacter.IsItalic, listViewIndex);
                            this._lastAdditions.Add(addition);
                            matches.Add(new CompareMatch(text, this._vobSubOcrCharacter.IsItalic, 0, null));
                            this.SetBinOcrLowercaseUppercase(item.NikseBitmap.Height, text);
                        }
                        else if (result == DialogResult.Abort)
                        {
                            this._abort = true;
                        }
                        else
                        {
                            matches.Add(new CompareMatch("*", false, 0, null));
                        }

                        this._italicCheckedLast = this._vobSubOcrCharacter.IsItalic;
                    }
                    else
                    {
                        // found image match
                        matches.Add(new CompareMatch(match.Text, match.Italic, 0, null));
                        if (match.ExpandCount > 0)
                        {
                            index += match.ExpandCount - 1;
                        }
                    }
                }

                if (this._abort)
                {
                    return string.Empty;
                }

                if (!expandSelection && !shrinkSelection)
                {
                    index++;
                }

                if (shrinkSelection && expandSelectionList.Count < 2)
                {
                    shrinkSelection = false;
                    expandSelectionList = new List<ImageSplitterItem>();
                }
            }

            line = GetStringWithItalicTags(matches);

            if (this.checkBoxAutoFixCommonErrors.Checked && this._ocrFixEngine != null)
            {
                line = this._ocrFixEngine.FixOcrErrorsViaHardcodedRules(line, this._lastLine, null); // TODO: Add abbreviations list
            }

            if (this.checkBoxRightToLeft.Checked)
            {
                line = ReverseNumberStrings(line);
            }

            // OCR fix engine
            string textWithOutFixes = line;
            if (this._ocrFixEngine.IsDictionaryLoaded)
            {
                var autoGuessLevel = OcrFixEngine.AutoGuessLevel.None;
                if (this.checkBoxGuessUnknownWords.Checked)
                {
                    autoGuessLevel = OcrFixEngine.AutoGuessLevel.Aggressive;
                }

                if (this.checkBoxAutoFixCommonErrors.Checked)
                {
                    line = this._ocrFixEngine.FixOcrErrors(line, listViewIndex, this._lastLine, true, autoGuessLevel);
                }

                int correctWords;
                int wordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(line, out correctWords);

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && string.IsNullOrWhiteSpace(textWithOutFixes.Replace("~", string.Empty)))
                {
                    this._ocrFixEngine.AutoGuessesUsed.Clear();
                    this._ocrFixEngine.UnknownWordsFound.Clear();
                    line = this._ocrFixEngine.FixUnknownWordsViaGuessOrPrompt(out wordsNotFound, line, listViewIndex, bitmap, this.checkBoxAutoFixCommonErrors.Checked, this.checkBoxPromptForUnknownWords.Checked, true, autoGuessLevel);
                }

                if (this._ocrFixEngine.Abort)
                {
                    this.ButtonStopClick(null, null);
                    this._ocrFixEngine.Abort = false;
                    return string.Empty;
                }

                // Log used word guesses (via word replace list)
                foreach (string guess in this._ocrFixEngine.AutoGuessesUsed)
                {
                    this.listBoxLogSuggestions.Items.Add(guess);
                }

                this._ocrFixEngine.AutoGuessesUsed.Clear();

                // Log unkown words guess (found via spelling dictionaries)
                this.LogUnknownWords();

                if (wordsNotFound >= 3)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Red);
                }

                if (wordsNotFound == 2)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Orange);
                }
                else if (wordsNotFound == 1)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Yellow);
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Orange);
                }
                else
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.LightGreen);
                }
            }

            if (textWithOutFixes.Trim() != line.Trim())
            {
                this._tesseractOcrAutoFixes++;
                this.labelFixesMade.Text = string.Format(" - {0}", this._tesseractOcrAutoFixes);
                this.LogOcrFix(listViewIndex, textWithOutFixes, line);
            }

            return line;
        }

        /// <summary>
        /// The set bin ocr lowercase uppercase.
        /// </summary>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void SetBinOcrLowercaseUppercase(int height, string text)
        {
            if (text == "e" && (height < this._binOcrLastLowercaseHeight || this._binOcrLastLowercaseHeight < 0))
            {
                this._binOcrLastLowercaseHeight = height;
            }
            else if (this._binOcrLastLowercaseHeight == -1 && text == "a" && (height < this._binOcrLastLowercaseHeight || this._binOcrLastLowercaseHeight < 0))
            {
                this._binOcrLastLowercaseHeight = height;
            }

            if (text == "E" || text == "H" || text == "R" || text == "D" || text == "T" && height > this._binOcrLastUppercaseHeight)
            {
                this._binOcrLastUppercaseHeight = height;
            }
            else if (this._binOcrLastUppercaseHeight == -1 && text == "M")
            {
                this._binOcrLastUppercaseHeight = height;
            }
        }

        /// <summary>
        /// The save n ocr.
        /// </summary>
        private void SaveNOcr()
        {
            try
            {
                this._nOcrDb.Save();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// The load n ocr for tesseract.
        /// </summary>
        /// <param name="xmlRessourceName">
        /// The xml ressource name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<NOcrChar> LoadNOcrForTesseract(string xmlRessourceName)
        {
            var nocrChars = new List<NOcrChar>();
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream strm = asm.GetManifestResourceStream(xmlRessourceName);
            if (strm != null)
            {
                XmlDocument doc = new XmlDocument();
                var rdr = new StreamReader(strm);
                using (var zip = new System.IO.Compression.GZipStream(rdr.BaseStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    byte[] data = new byte[175000];
                    zip.Read(data, 0, 175000);
                    doc.LoadXml(Encoding.UTF8.GetString(data));
                }

                rdr.Close();

                try
                {
                    foreach (XmlNode node in doc.DocumentElement.SelectNodes("Char"))
                    {
                        var oc = new NOcrChar(node.Attributes["Text"].Value);
                        oc.Width = Convert.ToInt32(node.Attributes["Width"].Value, CultureInfo.InvariantCulture);
                        oc.Height = Convert.ToInt32(node.Attributes["Height"].Value, CultureInfo.InvariantCulture);
                        oc.MarginTop = Convert.ToInt32(node.Attributes["MarginTop"].Value, CultureInfo.InvariantCulture);
                        if (node.Attributes["Italic"] != null)
                        {
                            oc.Italic = Convert.ToBoolean(node.Attributes["Italic"].Value, CultureInfo.InvariantCulture);
                        }

                        if (node.Attributes["ExpandCount"] != null)
                        {
                            oc.ExpandCount = Convert.ToInt32(node.Attributes["ExpandCount"].Value, CultureInfo.InvariantCulture);
                        }

                        foreach (XmlNode pointNode in node.SelectNodes("Point"))
                        {
                            var op = new NOcrPoint(DecodePoint(pointNode.Attributes["Start"].Value), DecodePoint(pointNode.Attributes["End"].Value));
                            XmlAttribute a = pointNode.Attributes["On"];
                            if (a != null && Convert.ToBoolean(a.Value))
                            {
                                oc.LinesForeground.Add(op);
                            }
                            else
                            {
                                oc.LinesBackground.Add(op);
                            }
                        }

                        nocrChars.Add(oc);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }

            return nocrChars;
        }

        /// <summary>
        /// The decode point.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        private static Point DecodePoint(string text)
        {
            var arr = text.Split(',');
            return new Point(Convert.ToInt32(arr[0], CultureInfo.InvariantCulture), Convert.ToInt32(arr[1], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The nocr intialize.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        private void NOCRIntialize(Bitmap bitmap)
        {
            var nikseBitmap = new NikseBitmap(bitmap);
            List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLetters(nikseBitmap, (int)this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom);

            foreach (ImageSplitterItem item in list)
            {
                if (item.NikseBitmap != null)
                {
                    var nbmp = item.NikseBitmap;
                    nbmp.ReplaceNonWhiteWithTransparent();
                    item.Y += nbmp.CropTopTransparent(0);
                    nbmp.CropTransparentSidesAndBottom(0, true);
                    nbmp.ReplaceTransparentWith(Color.Black);
                    this.GetNOcrCompareMatch(item, nikseBitmap, this._nOcrDb, false, false);
                }
            }
        }

        /// <summary>
        /// The ocr via nocr.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="listViewIndex">
        /// The list view index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string OcrViaNOCR(Bitmap bitmap, int listViewIndex)
        {
            if (this._ocrFixEngine == null)
            {
                this.comboBoxDictionaries_SelectedIndexChanged(null, null);
            }

            string line = string.Empty;
            if (this._nocrThreadResults != null)
            {
                line = this._nocrThreadResults[listViewIndex];
            }

            if (string.IsNullOrEmpty(line))
            {
                var nbmpInput = new NikseBitmap(bitmap);

                var matches = new List<CompareMatch>();

                int minLineHeight = this._binOcrLastLowercaseHeight - 3;
                if (minLineHeight < 5)
                {
                    minLineHeight = this._nocrLastLowercaseHeight - 3;
                }

                if (minLineHeight < 5)
                {
                    minLineHeight = 5;
                }

                List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLettersNew(nbmpInput, (int)this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight);

                foreach (ImageSplitterItem item in list)
                {
                    if (item.NikseBitmap != null)
                    {
                        item.NikseBitmap.ReplaceTransparentWith(Color.Black);
                    }
                }

                int index = 0;
                bool expandSelection = false;
                bool shrinkSelection = false;
                var expandSelectionList = new List<ImageSplitterItem>();
                while (index < list.Count)
                {
                    ImageSplitterItem item = list[index];
                    if (expandSelection || shrinkSelection)
                    {
                        expandSelection = false;
                        if (shrinkSelection && index > 0)
                        {
                            shrinkSelection = false;
                        }
                        else if (index + 1 < list.Count && list[index + 1].NikseBitmap != null)
                        {
                            // only allow expand to EndOfLine or space
                            index++;
                            expandSelectionList.Add(list[index]);
                        }

                        item = GetExpandedSelection(nbmpInput, expandSelectionList, this.checkBoxRightToLeft.Checked);
                        if (item.NikseBitmap != null)
                        {
                            item.NikseBitmap.ReplaceTransparentWith(Color.Black);
                        }

                        this._vobSubOcrNOcrCharacter.Initialize(bitmap, item, this._manualOcrDialogPosition, this._italicCheckedLast, expandSelectionList.Count > 1);
                        DialogResult result = this._vobSubOcrNOcrCharacter.ShowDialog(this);
                        this._manualOcrDialogPosition = this._vobSubOcrNOcrCharacter.FormPosition;
                        if (result == DialogResult.OK && this._vobSubOcrNOcrCharacter.ShrinkSelection)
                        {
                            shrinkSelection = true;
                            index--;
                            if (expandSelectionList.Count > 0)
                            {
                                expandSelectionList.RemoveAt(expandSelectionList.Count - 1);
                            }
                        }
                        else if (result == DialogResult.OK && this._vobSubOcrNOcrCharacter.ExpandSelection)
                        {
                            expandSelection = true;
                        }
                        else if (result == DialogResult.OK)
                        {
                            var c = this._vobSubOcrNOcrCharacter.NOcrChar;
                            if (expandSelectionList.Count > 1)
                            {
                                c.ExpandCount = expandSelectionList.Count;
                            }

                            this._nOcrDb.Add(c);
                            this.SaveNOcrWithCurrentLanguage();
                            string text = this._vobSubOcrNOcrCharacter.NOcrChar.Text;

                            // string name = SaveCompareItem(item.NikseBitmap, text, _vobSubOcrNOcrCharacter.IsItalic, expandSelectionList.Count);
                            // var addition = new ImageCompareAddition(name, text, item.NikseBitmap, _vobSubOcrNOcrCharacter.IsItalic, listViewIndex);
                            // _lastAdditions.Add(addition);
                            matches.Add(new CompareMatch(text, this._vobSubOcrNOcrCharacter.IsItalic, expandSelectionList.Count, null));
                            expandSelectionList = new List<ImageSplitterItem>();
                        }
                        else if (result == DialogResult.Abort)
                        {
                            this._abort = true;
                        }
                        else
                        {
                            matches.Add(new CompareMatch("*", false, 0, null));
                        }

                        this._italicCheckedLast = this._vobSubOcrNOcrCharacter.IsItalic;
                    }
                    else if (item.NikseBitmap == null)
                    {
                        matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                    }
                    else
                    {
                        CompareMatch match = this.GetNOcrCompareMatchNew(item, nbmpInput, this._nOcrDb, this.checkBoxNOcrItalic.Checked, !this.checkBoxNOcrCorrect.Checked);
                        if (match == null)
                        {
                            this._vobSubOcrNOcrCharacter.Initialize(bitmap, item, this._manualOcrDialogPosition, this._italicCheckedLast, false);
                            DialogResult result = this._vobSubOcrNOcrCharacter.ShowDialog(this);
                            this._manualOcrDialogPosition = this._vobSubOcrNOcrCharacter.FormPosition;
                            if (result == DialogResult.OK && this._vobSubOcrNOcrCharacter.ExpandSelection)
                            {
                                expandSelectionList.Add(item);
                                expandSelection = true;
                            }
                            else if (result == DialogResult.OK)
                            {
                                this._nOcrDb.Add(this._vobSubOcrNOcrCharacter.NOcrChar);
                                this.SaveNOcrWithCurrentLanguage();
                                string text = this._vobSubOcrNOcrCharacter.NOcrChar.Text;

                                // string name = SaveCompareItem(item.NikseBitmap, text, _vobSubOcrNOcrCharacter.IsItalic, 0);
                                // var addition = new ImageCompareAddition(name, text, item.NikseBitmap, _vobSubOcrNOcrCharacter.IsItalic, listViewIndex);
                                // _lastAdditions.Add(addition);
                                matches.Add(new CompareMatch(text, this._vobSubOcrNOcrCharacter.IsItalic, 0, null));
                            }
                            else if (result == DialogResult.Abort)
                            {
                                this._abort = true;
                            }
                            else
                            {
                                matches.Add(new CompareMatch("*", false, 0, null));
                            }

                            this._italicCheckedLast = this._vobSubOcrNOcrCharacter.IsItalic;
                        }
                        else
                        {
                            // found image match
                            matches.Add(new CompareMatch(match.Text, match.Italic, 0, null));
                            if (match.ExpandCount > 0)
                            {
                                index += match.ExpandCount - 1;
                            }
                        }
                    }

                    if (this._abort)
                    {
                        return string.Empty;
                    }

                    if (!expandSelection && !shrinkSelection)
                    {
                        index++;
                    }

                    if (shrinkSelection && expandSelectionList.Count < 2)
                    {
                        shrinkSelection = false;
                        expandSelectionList = new List<ImageSplitterItem>();
                    }
                }

                line = GetStringWithItalicTags(matches);
            }

            line = this.FixNocrHardcodedStuff(line);

            // OCR fix engine
            string textWithOutFixes = line;
            if (this._ocrFixEngine.IsDictionaryLoaded)
            {
                if (this.checkBoxAutoFixCommonErrors.Checked)
                {
                    line = this._ocrFixEngine.FixOcrErrors(line, listViewIndex, this._lastLine, true, this.GetAutoGuessLevel());
                }

                int correctWords;
                int wordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(line, out correctWords);

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && string.IsNullOrWhiteSpace(textWithOutFixes.Replace("~", string.Empty)))
                {
                    this._ocrFixEngine.AutoGuessesUsed.Clear();
                    this._ocrFixEngine.UnknownWordsFound.Clear();
                    line = this._ocrFixEngine.FixUnknownWordsViaGuessOrPrompt(out wordsNotFound, line, listViewIndex, bitmap, this.checkBoxAutoFixCommonErrors.Checked, this.checkBoxPromptForUnknownWords.Checked, true, this.GetAutoGuessLevel());
                }

                if (this._ocrFixEngine.Abort)
                {
                    this.ButtonStopClick(null, null);
                    this._ocrFixEngine.Abort = false;
                    return string.Empty;
                }

                // Log used word guesses (via word replace list)
                foreach (string guess in this._ocrFixEngine.AutoGuessesUsed)
                {
                    this.listBoxLogSuggestions.Items.Add(guess);
                }

                this._ocrFixEngine.AutoGuessesUsed.Clear();

                // Log unkown words guess (found via spelling dictionaries)
                this.LogUnknownWords();

                if (wordsNotFound >= 3)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Red);
                }

                if (wordsNotFound == 2)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Orange);
                }
                else if (wordsNotFound == 1)
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Yellow);
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.Orange);
                }
                else
                {
                    this.subtitleListView1.SetBackgroundColor(listViewIndex, Color.LightGreen);
                }
            }

            if (textWithOutFixes.Trim() != line.Trim())
            {
                this._tesseractOcrAutoFixes++;
                this.labelFixesMade.Text = string.Format(" - {0}", this._tesseractOcrAutoFixes);
                this.LogOcrFix(listViewIndex, textWithOutFixes, line);
            }

            return line;
        }

        /// <summary>
        /// The fix nocr hardcoded stuff.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixNocrHardcodedStuff(string line)
        {
            // fix I/l
            int start = line.IndexOf('I');
            while (start > 0)
            {
                if (start > 0 && char.IsLower(line[start - 1]))
                {
                    line = line.Remove(start, 1).Insert(start, "l");
                }
                else if (start < line.Length - 1 && char.IsLower(line[start + 1]))
                {
                    line = line.Remove(start, 1).Insert(start, "l");
                }

                start++;
                start = line.IndexOf('I', start);
            }

            start = line.IndexOf('l');
            while (start > 0)
            {
                if (start < line.Length - 1 && char.IsUpper(line[start + 1]))
                {
                    line = line.Remove(start, 1).Insert(start, "I");
                }

                start++;
                start = line.IndexOf('l', start);
            }

            if (line.Contains('l'))
            {
                if (line.StartsWith('l'))
                {
                    line = @"I" + line.Substring(1);
                }

                if (line.StartsWith("<i>l"))
                {
                    line = line.Remove(3, 1).Insert(3, "I");
                }

                if (line.StartsWith("- l"))
                {
                    line = line.Remove(2, 1).Insert(2, "I");
                }

                if (line.StartsWith("-l"))
                {
                    line = line.Remove(1, 1).Insert(1, "I");
                }

                line = line.Replace(". l", ". I");
                line = line.Replace("? l", "? I");
                line = line.Replace("! l", "! I");
                line = line.Replace(": l", ": I");
                line = line.Replace("." + Environment.NewLine + "l", "." + Environment.NewLine + "I");
                line = line.Replace("?" + Environment.NewLine + "l", "?" + Environment.NewLine + "I");
                line = line.Replace("!" + Environment.NewLine + "l", "!" + Environment.NewLine + "I");
                line = line.Replace("." + Environment.NewLine + "- l", "." + Environment.NewLine + "- I");
                line = line.Replace("?" + Environment.NewLine + "- l", "?" + Environment.NewLine + "- I");
                line = line.Replace("!" + Environment.NewLine + "- l", "!" + Environment.NewLine + "- I");
                line = line.Replace("." + Environment.NewLine + "-l", "." + Environment.NewLine + "-I");
                line = line.Replace("?" + Environment.NewLine + "-l", "?" + Environment.NewLine + "-I");
                line = line.Replace("!" + Environment.NewLine + "-l", "!" + Environment.NewLine + "-I");
                line = line.Replace(" lq", " Iq");
                line = line.Replace(" lw", " Iw");
                line = line.Replace(" lr", " Ir");
                line = line.Replace(" lt", " It");
                line = line.Replace(" lp", " Ip");
                line = line.Replace(" ls", " Is");
                line = line.Replace(" ld", " Id");
                line = line.Replace(" lf", " If");
                line = line.Replace(" lg", " Ig");
                line = line.Replace(" lh", " Ih");
                line = line.Replace(" lj", " Ij");
                line = line.Replace(" lk", " Ik");
                line = line.Replace(" ll", " Il");
                line = line.Replace(" lz", " Iz");
                line = line.Replace(" lx", " Ix");
                line = line.Replace(" lc", " Ic");
                line = line.Replace(" lv", " Iv");
                line = line.Replace(" lb", " Ib");
                line = line.Replace(" ln", " In");
                line = line.Replace(" lm", " Im");
            }

            if (line.Contains('I'))
            {
                line = line.Replace("II", "ll");
            }

            // fix periods with space between
            line = line.Replace(".   .", "..");
            line = line.Replace(".  .", "..");
            line = line.Replace(". .", "..");
            line = line.Replace(". .", "..");
            line = line.Replace(" ." + Environment.NewLine, "." + Environment.NewLine);
            if (line.EndsWith(" ."))
            {
                line = line.Remove(line.Length - 2, 1);
            }

            // fix no space before comma
            line = line.Replace(" ,", ",");

            // fix O => 0
            if (line.Contains('O'))
            {
                line = line.Replace(", OOO", ",000");
                line = line.Replace(",OOO", ",000");
                line = line.Replace(". OOO", ".000");
                line = line.Replace(".OOO", ".000");

                line = line.Replace("1O", "10");
                line = line.Replace("2O", "20");
                line = line.Replace("3O", "30");
                line = line.Replace("4O", "40");
                line = line.Replace("5O", "50");
                line = line.Replace("6O", "60");
                line = line.Replace("7O", "70");
                line = line.Replace("8O", "80");
                line = line.Replace("9O", "90");

                line = line.Replace("O1", "01");
                line = line.Replace("O2", "02");
                line = line.Replace("O3", "03");
                line = line.Replace("O4", "04");
                line = line.Replace("O5", "05");
                line = line.Replace("O6", "06");
                line = line.Replace("O7", "07");
                line = line.Replace("O8", "08");
                line = line.Replace("O9", "09");

                line = line.Replace("O-O", "0-0");
                line = line.Replace("O-1", "0-1");
                line = line.Replace("O-2", "0-2");
                line = line.Replace("O-3", "0-3");
                line = line.Replace("O-4", "0-4");
                line = line.Replace("O-5", "0-5");
                line = line.Replace("O-6", "0-6");
                line = line.Replace("O-7", "0-7");
                line = line.Replace("O-8", "0-8");
                line = line.Replace("O-9", "0-9");
                line = line.Replace("1-O", "1-0");
                line = line.Replace("2-O", "2-0");
                line = line.Replace("3-O", "3-0");
                line = line.Replace("4-O", "4-0");
                line = line.Replace("5-O", "5-0");
                line = line.Replace("6-O", "6-0");
                line = line.Replace("7-O", "7-0");
                line = line.Replace("8-O", "8-0");
                line = line.Replace("9-O", "9-0");
            }

            if (this.checkBoxAutoFixCommonErrors.Checked && this._ocrFixEngine != null)
            {
                line = this._ocrFixEngine.FixOcrErrorsViaHardcodedRules(line, this._lastLine, null); // TODO: Add abbreviations list
            }

            if (this.checkBoxRightToLeft.Checked)
            {
                line = ReverseNumberStrings(line);
            }

            return line;
        }

        /// <summary>
        /// The reverse number strings.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ReverseNumberStrings(string line)
        {
            Regex regex = new Regex(@"\b\d+\b");
            var matches = regex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Length > 1)
                {
                    string number = string.Empty;
                    for (int i = match.Index; i < match.Index + match.Length; i++)
                    {
                        number = line[i] + number;
                    }

                    line = line.Remove(match.Index, match.Length).Insert(match.Index, number);
                }
            }

            return line;
        }

        /// <summary>
        /// The get expanded selection.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="expandSelectionList">
        /// The expand selection list.
        /// </param>
        /// <param name="rightToLeft">
        /// The right to left.
        /// </param>
        /// <returns>
        /// The <see cref="ImageSplitterItem"/>.
        /// </returns>
        internal static ImageSplitterItem GetExpandedSelection(NikseBitmap bitmap, List<ImageSplitterItem> expandSelectionList, bool rightToLeft)
        {
            if (rightToLeft)
            {
                int minimumX = expandSelectionList[expandSelectionList.Count - 1].X - expandSelectionList[expandSelectionList.Count - 1].NikseBitmap.Width;
                int maximumX = expandSelectionList[0].X;
                int minimumY = expandSelectionList[0].Y;
                int maximumY = expandSelectionList[0].Y + expandSelectionList[0].NikseBitmap.Height;
                foreach (ImageSplitterItem item in expandSelectionList)
                {
                    if (item.Y < minimumY)
                    {
                        minimumY = item.Y;
                    }

                    if (item.Y + item.NikseBitmap.Height > maximumY)
                    {
                        maximumY = item.Y + item.NikseBitmap.Height;
                    }
                }

                var part = bitmap.CopyRectangle(new Rectangle(minimumX, minimumY, maximumX - minimumX, maximumY - minimumY));
                return new ImageSplitterItem(minimumX, minimumY, part);
            }
            else
            {
                int minimumX = expandSelectionList[0].X;
                int maximumX = expandSelectionList[expandSelectionList.Count - 1].X + expandSelectionList[expandSelectionList.Count - 1].NikseBitmap.Width;
                int minimumY = expandSelectionList[0].Y;
                int maximumY = expandSelectionList[0].Y + expandSelectionList[0].NikseBitmap.Height;
                foreach (ImageSplitterItem item in expandSelectionList)
                {
                    if (item.Y < minimumY)
                    {
                        minimumY = item.Y;
                    }

                    if (item.Y + item.NikseBitmap.Height > maximumY)
                    {
                        maximumY = item.Y + item.NikseBitmap.Height;
                    }
                }

                var part = bitmap.CopyRectangle(new Rectangle(minimumX, minimumY, maximumX - minimumX, maximumY - minimumY));
                return new ImageSplitterItem(minimumX, minimumY, part);
            }
        }

        /// <summary>
        /// The get expanded selection new.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="expandSelectionList">
        /// The expand selection list.
        /// </param>
        /// <returns>
        /// The <see cref="ImageSplitterItem"/>.
        /// </returns>
        internal static ImageSplitterItem GetExpandedSelectionNew(NikseBitmap bitmap, List<ImageSplitterItem> expandSelectionList)
        {
            int minimumX = expandSelectionList[0].X;
            int maximumX = expandSelectionList[expandSelectionList.Count - 1].X + expandSelectionList[expandSelectionList.Count - 1].NikseBitmap.Width;
            int minimumY = expandSelectionList[0].Y;
            int maximumY = expandSelectionList[0].Y + expandSelectionList[0].NikseBitmap.Height;
            var nbmp = new NikseBitmap(bitmap.Width, bitmap.Height);
            foreach (ImageSplitterItem item in expandSelectionList)
            {
                for (int y = 0; y < item.NikseBitmap.Height; y++)
                {
                    for (int x = 0; x < item.NikseBitmap.Width; x++)
                    {
                        int a = item.NikseBitmap.GetAlpha(x, y);
                        if (a > 100)
                        {
                            nbmp.SetPixel(item.X + x, item.Y + y, Color.White);
                        }
                    }
                }

                if (item.Y < minimumY)
                {
                    minimumY = item.Y;
                }

                if (item.Y + item.NikseBitmap.Height > maximumY)
                {
                    maximumY = item.Y + item.NikseBitmap.Height;
                }
            }

            nbmp.CropTransparentSidesAndBottom(0, true);
            int topCropping;
            nbmp = NikseBitmapImageSplitter.CropTopAndBottom(nbmp, out topCropping);

            return new ImageSplitterItem(minimumX, minimumY, nbmp);
        }

        /// <summary>
        /// The get string with italic tags.
        /// </summary>
        /// <param name="matches">
        /// The matches.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetStringWithItalicTags(List<CompareMatch> matches)
        {
            StringBuilder paragraph = new StringBuilder();
            StringBuilder line = new StringBuilder();
            StringBuilder word = new StringBuilder();
            int lettersItalics = 0;
            int lettersNonItalics = 0;
            int lineLettersNonItalics = 0;
            int wordItalics = 0;
            int wordNonItalics = 0;
            bool isItalic = false;
            bool allItalic = true;
            for (int i = 0; i < matches.Count; i++)
            {
                string text = matches[i].Text;
                if (text != null)
                {
                    bool italic = matches[i].Italic;
                    if (text == " ")
                    {
                        ItalicsWord(line, ref word, ref lettersItalics, ref lettersNonItalics, ref wordItalics, ref wordNonItalics, ref isItalic, " ");
                    }
                    else if (text == Environment.NewLine)
                    {
                        ItalicsWord(line, ref word, ref lettersItalics, ref lettersNonItalics, ref wordItalics, ref wordNonItalics, ref isItalic, string.Empty);
                        ItalianLine(paragraph, ref line, ref allItalic, ref wordItalics, ref wordNonItalics, ref isItalic, Environment.NewLine, lineLettersNonItalics);
                        lineLettersNonItalics = 0;
                    }
                    else if (italic)
                    {
                        word.Append(text);
                        lettersItalics += text.Length;
                        lineLettersNonItalics += text.Length;
                    }
                    else
                    {
                        word.Append(text);
                        lettersNonItalics += text.Length;
                    }
                }
            }

            if (word.Length > 0)
            {
                ItalicsWord(line, ref word, ref lettersItalics, ref lettersNonItalics, ref wordItalics, ref wordNonItalics, ref isItalic, string.Empty);
            }

            if (line.Length > 0)
            {
                ItalianLine(paragraph, ref line, ref allItalic, ref wordItalics, ref wordNonItalics, ref isItalic, string.Empty, lineLettersNonItalics);
            }

            if (allItalic && matches.Count > 0)
            {
                var temp = HtmlUtil.RemoveOpenCloseTags(paragraph.ToString(), HtmlUtil.TagItalic);
                paragraph.Clear();
                paragraph.Append("<i>");
                paragraph.Append(temp);
                paragraph.Append("</i>");
            }

            return paragraph.ToString();
        }

        /// <summary>
        /// The italian line.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="allItalic">
        /// The all italic.
        /// </param>
        /// <param name="wordItalics">
        /// The word italics.
        /// </param>
        /// <param name="wordNonItalics">
        /// The word non italics.
        /// </param>
        /// <param name="isItalic">
        /// The is italic.
        /// </param>
        /// <param name="appendString">
        /// The append string.
        /// </param>
        /// <param name="lineLettersNonItalics">
        /// The line letters non italics.
        /// </param>
        private static void ItalianLine(StringBuilder paragraph, ref StringBuilder line, ref bool allItalic, ref int wordItalics, ref int wordNonItalics, ref bool isItalic, string appendString, int lineLettersNonItalics)
        {
            if (isItalic)
            {
                line.Append("</i>");
                isItalic = false;
            }

            if (wordItalics > 0 && (wordNonItalics == 0 || wordNonItalics < 2 && lineLettersNonItalics < 3 && line.ToString().TrimStart().StartsWith('-')))
            {
                paragraph.Append("<i>");
                paragraph.Append(HtmlUtil.RemoveOpenCloseTags(line.ToString(), HtmlUtil.TagItalic));
                paragraph.Append("</i>");
                paragraph.Append(appendString);
            }
            else
            {
                allItalic = false;

                if (wordItalics > 0)
                {
                    string temp = line.ToString().Replace(" </i>", "</i> ");
                    line.Clear();
                    line.Append(temp);
                }

                paragraph.Append(line);
                paragraph.Append(appendString);
            }

            line.Clear();
            wordItalics = 0;
            wordNonItalics = 0;
        }

        /// <summary>
        /// The italics word.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="lettersItalics">
        /// The letters italics.
        /// </param>
        /// <param name="lettersNonItalics">
        /// The letters non italics.
        /// </param>
        /// <param name="wordItalics">
        /// The word italics.
        /// </param>
        /// <param name="wordNonItalics">
        /// The word non italics.
        /// </param>
        /// <param name="isItalic">
        /// The is italic.
        /// </param>
        /// <param name="appendString">
        /// The append string.
        /// </param>
        private static void ItalicsWord(StringBuilder line, ref StringBuilder word, ref int lettersItalics, ref int lettersNonItalics, ref int wordItalics, ref int wordNonItalics, ref bool isItalic, string appendString)
        {
            if (lettersItalics >= lettersNonItalics && lettersItalics > 0)
            {
                if (!isItalic)
                {
                    line.Append("<i>");
                }

                line.Append(word + appendString);
                wordItalics++;
                isItalic = true;
            }
            else
            {
                if (isItalic)
                {
                    line.Append("</i>");
                    isItalic = false;
                }

                line.Append(word);
                line.Append(appendString);
                wordNonItalics++;
            }

            word = new StringBuilder();
            lettersItalics = 0;
            lettersNonItalics = 0;
        }

        /// <summary>
        /// The form vob sub ocr_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormVobSubOcr_Shown(object sender, EventArgs e)
        {
            this.checkBoxUseModiInTesseractForUnknownWords.Checked = Configuration.Settings.VobSubOcr.UseModiInTesseractForUnknownWords;
            if (this._mp4List != null)
            {
                this.checkBoxShowOnlyForced.Visible = false;
                this.checkBoxUseTimeCodesFromIdx.Visible = false;

                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = true;
                this.buttonStartOcr.Enabled = true;
                this.buttonStop.Enabled = false;
                this.buttonNewCharacterDatabase.Enabled = true;
                this.buttonEditCharacterDatabase.Enabled = true;
                this.buttonStartOcr.Focus();
            }
            else if (this._spList != null)
            {
                this.checkBoxShowOnlyForced.Visible = false;
                this.checkBoxUseTimeCodesFromIdx.Visible = false;

                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = true;
                this.buttonStartOcr.Enabled = true;
                this.buttonStop.Enabled = false;
                this.buttonNewCharacterDatabase.Enabled = true;
                this.buttonEditCharacterDatabase.Enabled = true;
                this.buttonStartOcr.Focus();
            }
            else if (this._dvbSubtitles != null)
            {
                this.checkBoxShowOnlyForced.Visible = false;
                this.checkBoxUseTimeCodesFromIdx.Visible = false;

                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = true;
                this.buttonStartOcr.Enabled = true;
                this.buttonStop.Enabled = false;
                this.buttonNewCharacterDatabase.Enabled = true;
                this.buttonEditCharacterDatabase.Enabled = true;
                this.buttonStartOcr.Focus();
            }
            else if (this._bdnXmlOriginal != null)
            {
                this.LoadBdnXml();
                bool hasForcedSubtitles = false;
                foreach (var x in this._bdnXmlOriginal.Paragraphs)
                {
                    if (x.Forced)
                    {
                        hasForcedSubtitles = true;
                        break;
                    }
                }

                this.checkBoxShowOnlyForced.Enabled = hasForcedSubtitles;
                this.checkBoxUseTimeCodesFromIdx.Visible = false;
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                this.numericUpDownMaxErrorPct.Value = (decimal)Configuration.Settings.VobSubOcr.BlurayAllowDifferenceInPercent;
                this.LoadBluRaySup();
                bool hasForcedSubtitles = false;
                foreach (var x in this._bluRaySubtitlesOriginal)
                {
                    if (x.IsForced)
                    {
                        hasForcedSubtitles = true;
                        break;
                    }
                }

                this.checkBoxShowOnlyForced.Enabled = hasForcedSubtitles;
                this.checkBoxUseTimeCodesFromIdx.Visible = false;
            }
            else if (this._xSubList != null)
            {
                this.checkBoxShowOnlyForced.Visible = false;
                this.checkBoxUseTimeCodesFromIdx.Visible = false;

                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = true;
                this.buttonStartOcr.Enabled = true;
                this.buttonStop.Enabled = false;
                this.buttonNewCharacterDatabase.Enabled = true;
                this.buttonEditCharacterDatabase.Enabled = true;
                this.buttonStartOcr.Focus();
            }
            else
            {
                this.ReadyVobSubRip();
            }

            this.VobSubOcr_Resize(null, null);
        }

        /// <summary>
        /// The do hide.
        /// </summary>
        public void DoHide()
        {
            this.SetVisibleCore(false);
        }

        /// <summary>
        /// The ready vob sub rip.
        /// </summary>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle ReadyVobSubRip()
        {
            this._vobSubMergedPackistOriginal = new List<VobSubMergedPack>();
            bool hasIdxTimeCodes = false;
            bool hasForcedSubtitles = false;
            if (this._vobSubMergedPackist == null)
            {
                return null;
            }

            foreach (var x in this._vobSubMergedPackist)
            {
                this._vobSubMergedPackistOriginal.Add(x);
                if (x.IdxLine != null)
                {
                    hasIdxTimeCodes = true;
                }

                if (x.SubPicture.Forced)
                {
                    hasForcedSubtitles = true;
                }
            }

            this.checkBoxUseTimeCodesFromIdx.CheckedChanged -= this.checkBoxUseTimeCodesFromIdx_CheckedChanged;
            this.checkBoxUseTimeCodesFromIdx.Visible = hasIdxTimeCodes;
            this.checkBoxUseTimeCodesFromIdx.Checked = hasIdxTimeCodes;
            this.checkBoxUseTimeCodesFromIdx.CheckedChanged += this.checkBoxUseTimeCodesFromIdx_CheckedChanged;
            this.checkBoxShowOnlyForced.Enabled = hasForcedSubtitles;
            this.LoadVobRip();
            return this._subtitle;
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
            this.OkClicked = true; // don't ask about discard changes
            if (this._dvbSubtitles != null && this.checkBoxTransportStreamGetColorAndSplit.Checked)
            {
                this.MergeDvbForEachSubImage();
            }

            if (Configuration.Settings.VobSubOcr.XOrMorePixelsMakesSpace != (int)this.numericUpDownPixelsIsSpace.Value && this._bluRaySubtitlesOriginal == null)
            {
                Configuration.Settings.VobSubOcr.XOrMorePixelsMakesSpace = (int)this.numericUpDownPixelsIsSpace.Value;
                Configuration.Settings.Save();
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The set buttons enabled after ocr done.
        /// </summary>
        private void SetButtonsEnabledAfterOcrDone()
        {
            this.buttonOK.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.buttonStartOcr.Enabled = true;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = true;
            this.buttonEditCharacterDatabase.Enabled = true;
            this._mainOcrRunning = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.subtitleListView1.MultiSelect = true;
        }

        /// <summary>
        /// The image compare thread do work.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ImageCompareThreadDoWork(object sender, DoWorkEventArgs e)
        {
            var p = (ImageCompareThreadParameter)e.Argument;
            e.Result = p;
            Bitmap bitmap = p.Picture;
            var matches = new List<CompareMatch>();
            List<ImageSplitterItem> lines = NikseBitmapImageSplitter.SplitVertical(bitmap);
            List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLetters(lines, p.NumberOfPixelsIsSpace, p.RightToLeft, Configuration.Settings.VobSubOcr.TopToBottom);

            int outerIndex = 0;
            while (outerIndex < list.Count)
            {
                ImageSplitterItem item = list[outerIndex];
                if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                }
                else
                {
                    var target = item.NikseBitmap;
                    int numberOfForegroundColors = CalculateNumberOfForegroundColors(target);

                    int smallestDifference = 10000;
                    int smallestIndex = -1;

                    int index;
                    if (smallestDifference > 0)
                    {
                        index = 0;
                        foreach (CompareItem compareItem in p.CompareBitmaps)
                        {
                            if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height)
                            {
                                if (compareItem.NumberOfForegroundColors < 1)
                                {
                                    compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                                }

                                if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < 30)
                                {
                                    int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                    if (dif < smallestDifference)
                                    {
                                        smallestDifference = dif;
                                        smallestIndex = index;
                                        if (dif < 0.2)
                                        {
                                            break; // foreach ending
                                        }
                                    }
                                }
                            }

                            index++;
                        }
                    }

                    if (smallestDifference > 1 && target.Width < 55 && target.Width > 5)
                    {
                        index = 0;
                        foreach (CompareItem compareItem in p.CompareBitmaps)
                        {
                            if (compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height + 1)
                            {
                                if (compareItem.NumberOfForegroundColors == -1)
                                {
                                    compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                                }

                                if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < 50)
                                {
                                    int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem.Bitmap);
                                    if (dif < smallestDifference)
                                    {
                                        smallestDifference = dif;
                                        smallestIndex = index;
                                        if (dif < 0.5)
                                        {
                                            break; // foreach ending
                                        }
                                    }
                                }
                            }

                            index++;
                        }
                    }

                    if (smallestDifference > 1 && target.Width < 55 && target.Width > 5)
                    {
                        index = 0;
                        foreach (CompareItem compareItem in p.CompareBitmaps)
                        {
                            if (compareItem.Bitmap.Width == target.Width - 1 && compareItem.Bitmap.Height == target.Height || compareItem.Bitmap.Width == target.Width - 1 && compareItem.Bitmap.Height == target.Height - 1 || compareItem.Bitmap.Width == target.Width && compareItem.Bitmap.Height == target.Height - 1)
                            {
                                if (compareItem.NumberOfForegroundColors < 1)
                                {
                                    compareItem.NumberOfForegroundColors = CalculateNumberOfForegroundColors(compareItem.Bitmap);
                                }

                                if (Math.Abs(compareItem.NumberOfForegroundColors - numberOfForegroundColors) < 55)
                                {
                                    int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem.Bitmap, target);
                                    if (dif < smallestDifference)
                                    {
                                        smallestDifference = dif;
                                        smallestIndex = index;
                                        if (dif < 0.5)
                                        {
                                            break; // foreach ending
                                        }
                                    }
                                }
                            }

                            index++;
                        }
                    }

                    double differencePercentage = smallestDifference * 100.0 / (item.NikseBitmap.Width * item.NikseBitmap.Height);
                    double maxDiff = p.MaxErrorPercent;
                    if (differencePercentage <= maxDiff && smallestIndex >= 0)
                    {
                        var hit = p.CompareBitmaps[smallestIndex];
                        var match = new CompareMatch(hit.Text, hit.Italic, hit.ExpandCount, hit.Name);
                        matches.Add(new CompareMatch(match.Text, match.Italic, 0, null));
                        if (match.ExpandCount > 0)
                        {
                            outerIndex += match.ExpandCount - 1;
                        }
                    }
                    else
                    {
                        p.Result = string.Empty;
                        return;
                    }
                }

                outerIndex++;
            }

            bitmap.Dispose();
            p.Result = GetStringWithItalicTags(matches);
        }

        /// <summary>
        /// The image compare thread run worker completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ImageCompareThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var p = (ImageCompareThreadParameter)e.Result;
            if (!this._icThreadsStop)
            {
                if (string.IsNullOrEmpty(this._icThreadResults[p.Index]))
                {
                    this._icThreadResults[p.Index] = p.Result;
                }

                p.Index += p.Increment;
                while (p.Index <= this._mainOcrIndex)
                {
                    p.Index += p.Increment;
                }

                p.Picture.Dispose();
                if (p.Index < this._subtitle.Paragraphs.Count)
                {
                    p.Result = string.Empty;
                    p.Picture = this.GetSubtitleBitmap(p.Index);
                    p.Self.RunWorkerAsync(p);
                }
            }
            else
            {
                this._mainOcrRunning = false;
            }
        }

        /// <summary>
        /// The nocr fast check.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string NocrFastCheck(Bitmap bitmap)
        {
            var nbmpInput = new NikseBitmap(bitmap);
            nbmpInput.ReplaceNonWhiteWithTransparent();

            var matches = new List<CompareMatch>();
            List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLetters(nbmpInput, (int)this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom);

            foreach (ImageSplitterItem item in list)
            {
                if (item.NikseBitmap != null)
                {
                    item.NikseBitmap.ReplaceTransparentWith(Color.Black);
                }
            }

            int index = 0;

            while (index < list.Count)
            {
                ImageSplitterItem item = list[index];
                if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                }
                else
                {
                    CompareMatch match = null;

                    var nbmp = item.NikseBitmap;
                    int topMargin = item.Y - item.ParentY;
                    foreach (NOcrChar oc in this._nOcrDb.OcrCharacters)
                    {
                        if (Math.Abs(oc.Width - nbmp.Width) < 3 && Math.Abs(oc.Height - nbmp.Height) < 4 && Math.Abs(oc.MarginTop - topMargin) < 4)
                        { // only very accurate matches

                            bool ok = true;
                            var index2 = 0;
                            while (index2 < oc.LinesForeground.Count && ok)
                            {
                                NOcrPoint op = oc.LinesForeground[index2];
                                foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                                {
                                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(point.X, point.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                        }
                                        else
                                        {
                                            Point p = new Point(point.X - 1, point.Y);
                                            if (p.X < 0)
                                            {
                                                p.X = 1;
                                            }

                                            c = nbmp.GetPixel(p.X, p.Y);
                                            if (nbmp.Width > 20 && c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                            {
                                            }
                                            else
                                            {
                                                ok = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                index2++;
                            }

                            index2 = 0;
                            while (index2 < oc.LinesBackground.Count && ok)
                            {
                                NOcrPoint op = oc.LinesBackground[index2];
                                foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                                {
                                    if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                                    {
                                        Color c = nbmp.GetPixel(point.X, point.Y);
                                        if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                        {
                                            Point p = new Point(point.X, point.Y);
                                            if (oc.Width > 19 && point.X > 0)
                                            {
                                                p.X = p.X - 1;
                                            }

                                            c = nbmp.GetPixel(p.X, p.Y);
                                            if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                            {
                                                ok = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                index2++;
                            }

                            if (ok)
                            {
                                match = new CompareMatch(oc.Text, oc.Italic, 0, null);
                            }
                        }
                    }

                    if (match == null)
                    {
                        matches.Add(new CompareMatch("*", false, 0, null));
                    }
                    else
                    {
                        // found image match
                        matches.Add(new CompareMatch(match.Text, match.Italic, 0, null));
                        if (match.ExpandCount > 0)
                        {
                            index += match.ExpandCount - 1;
                        }
                    }
                }

                index++;
            }

            return GetStringWithItalicTags(matches);
        }

        /// <summary>
        /// The n ocr thread do work.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void NOcrThreadDoWork(object sender, DoWorkEventArgs e)
        {
            var p = (NOcrThreadParameter)e.Argument;
            e.Result = p;
            var nbmpInput = new NikseBitmap(p.Picture);
            nbmpInput.ReplaceNonWhiteWithTransparent();

            var matches = new List<CompareMatch>();
            int minLineHeight = p.NOcrLastLowercaseHeight;
            if (minLineHeight < 10)
            {
                minLineHeight = 22;
            }

            int maxLineHeight = p.NOcrLastUppercaseHeight;
            if (maxLineHeight < 10)
            {
                minLineHeight = 80;
            }

            List<ImageSplitterItem> list = NikseBitmapImageSplitter.SplitBitmapToLettersNew(nbmpInput, p.NumberOfPixelsIsSpace, p.RightToLeft, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight);
            foreach (ImageSplitterItem item in list)
            {
                if (item.NikseBitmap != null)
                {
                    var nbmp = item.NikseBitmap;

                    // nbmp.ReplaceNonWhiteWithTransparent();
                    item.Y += nbmp.CropTopTransparent(0);
                    nbmp.CropTransparentSidesAndBottom(0, true);
                    nbmp.ReplaceTransparentWith(Color.Black);
                }
            }

            int index = 0;
            while (index < list.Count)
            {
                ImageSplitterItem item = list[index];
                if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                }
                else
                {
                    CompareMatch match = GetNOcrCompareMatch(item, nbmpInput, p);
                    if (match == null)
                    {
                        p.Result = string.Empty;
                        return;
                    }
                    else
                    {
                        // found image match
                        matches.Add(new CompareMatch(match.Text, match.Italic, 0, null));
                        if (match.ExpandCount > 0)
                        {
                            index += match.ExpandCount - 1;
                        }
                    }
                }

                index++;
            }

            p.Result = GetStringWithItalicTags(matches);
        }

        /// <summary>
        /// The n ocr thread run worker completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NOcrThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var p = (NOcrThreadParameter)e.Result;
            Application.DoEvents();
            if (!this._nocrThreadsStop)
            {
                if (string.IsNullOrEmpty(this._nocrThreadResults[p.Index]))
                {
                    this._nocrThreadResults[p.Index] = p.Result;
                }

                p.Index += p.Increment;
                p.Picture.Dispose();
                if (p.Index < this._subtitle.Paragraphs.Count)
                {
                    p.Result = string.Empty;
                    p.Picture = this.GetSubtitleBitmap(p.Index);
                    p.Self.RunWorkerAsync(p);
                }
            }
        }

        /// <summary>
        /// The tesseract thread do work.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TesseractThreadDoWork(object sender, DoWorkEventArgs e)
        {
            var bitmap = (Bitmap)e.Argument;
            if (bitmap != null)
            {
                if (this._tesseractAsyncIndex >= 0 && this._tesseractAsyncIndex < this._tesseractAsyncStrings.Length)
                {
                    if (string.IsNullOrEmpty(this._tesseractAsyncStrings[this._tesseractAsyncIndex]))
                    {
                        this._tesseractAsyncStrings[this._tesseractAsyncIndex] = this.Tesseract3DoOcrViaExe(bitmap, this._languageId, "-psm 6"); // 6 = Assume a single uniform block of text.);
                    }
                }

                bitmap.Dispose();
            }
        }

        /// <summary>
        /// The tesseract thread run worker completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TesseractThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this._tesseractAsyncIndex++;
                if (this._tesseractAsyncIndex >= 0 && this._tesseractAsyncIndex < this._tesseractAsyncStrings.Length)
                {
                    this._tesseractThread.RunWorkerAsync(this.GetSubtitleBitmap(this._tesseractAsyncIndex));
                }
            }
        }

        /// <summary>
        /// The button start ocr click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartOcrClick(object sender, EventArgs e)
        {
            Configuration.Settings.VobSubOcr.RightToLeft = this.checkBoxRightToLeft.Checked;
            this._lastLine = null;
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = true;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;

            this._abort = false;

            int max = this.GetSubtitleCount();

            if (this.comboBoxOcrMethod.SelectedIndex == 0 && this._tesseractAsyncStrings == null)
            {
                this._nOcrDb = null;
                this._tesseractAsyncStrings = new string[max];
                this._tesseractAsyncIndex = (int)this.numericUpDownStartNumber.Value + 5;
                this._tesseractThread = new BackgroundWorker();
                this._tesseractThread.DoWork += this.TesseractThreadDoWork;
                this._tesseractThread.RunWorkerCompleted += this.TesseractThreadRunWorkerCompleted;
                this._tesseractThread.WorkerSupportsCancellation = true;
                if (this._tesseractAsyncIndex >= 0 && this._tesseractAsyncIndex < max)
                {
                    this._tesseractThread.RunWorkerAsync(this.GetSubtitleBitmap(this._tesseractAsyncIndex));
                }
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 1)
            {
                if (this._compareBitmaps == null)
                {
                    this.LoadImageCompareBitmaps();
                }
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 3)
            {
                if (this._nOcrDb == null)
                {
                    this.LoadNOcrWithCurrentLanguage();
                }

                if (this._nOcrDb == null)
                {
                    MessageBox.Show("Fatal - No NOCR dictionary loaded!");
                    this.SetButtonsEnabledAfterOcrDone();
                    return;
                }

                this._nocrThreadsStop = false;
                this._nocrThreadResults = new string[this._subtitle.Paragraphs.Count];
                int noOfThreads = Environment.ProcessorCount - 1;
                if (noOfThreads >= max)
                {
                    noOfThreads = max - 1;
                }

                int start = (int)this.numericUpDownStartNumber.Value + 5;
                if (noOfThreads >= 1 && max > 5)
                {
                    // finder letter size (uppercase/lowercase)
                    int testIndex = 0;
                    while (testIndex < 6 && (this._nocrLastLowercaseHeight == -1 || this._nocrLastUppercaseHeight == -1))
                    {
                        this.NOCRIntialize(this.GetSubtitleBitmap(testIndex));
                        testIndex++;
                    }

                    for (int i = 0; i < noOfThreads; i++)
                    {
                        if (start + i < max)
                        {
                            var bw = new BackgroundWorker();
                            var p = new NOcrThreadParameter(this.GetSubtitleBitmap(start + i), start + i, this._nOcrDb.OcrCharacters, bw, noOfThreads, this._unItalicFactor, this.checkBoxNOcrItalic.Checked, (int)this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value, this.checkBoxRightToLeft.Checked);
                            p.NOcrLastLowercaseHeight = this._nocrLastLowercaseHeight;
                            p.NOcrLastUppercaseHeight = this._nocrLastUppercaseHeight;
                            bw.DoWork += NOcrThreadDoWork;
                            bw.RunWorkerCompleted += this.NOcrThreadRunWorkerCompleted;
                            bw.RunWorkerAsync(p);
                        }
                    }
                }
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 4)
            {
                if (this._binaryOcrDb == null)
                {
                    this._binaryOcrDbFileName = Configuration.OcrFolder + "Latin.db";
                    this._binaryOcrDb = new BinaryOcrDb(this._binaryOcrDbFileName, true);
                }

                this._nOcrDb = new NOcrDb(this._binaryOcrDb.FileName.Replace(".db", ".nocr"));
            }

            this.progressBar1.Maximum = max;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = true;

            this._mainOcrTimerMax = max;
            this._mainOcrIndex = (int)this.numericUpDownStartNumber.Value - 1;
            this._mainOcrTimer = new Timer();
            this._mainOcrTimer.Tick += this.mainOcrTimer_Tick;
            this._mainOcrTimer.Interval = 5;
            this._mainOcrRunning = true;
            this.subtitleListView1.MultiSelect = false;
            this.mainOcrTimer_Tick(null, null);

            if (this.comboBoxOcrMethod.SelectedIndex == 1)
            {
                this._icThreadsStop = false;
                this._icThreadResults = new string[this._subtitle.Paragraphs.Count];
                int noOfThreads = Environment.ProcessorCount - 2; // -1 or -2?
                if (noOfThreads >= max)
                {
                    noOfThreads = max - 1;
                }

                int start = (int)this.numericUpDownStartNumber.Value + 5;
                if (noOfThreads > 2)
                {
                    noOfThreads = 2; // Threading is not really good - subtitle picture creation should probably be threaded also/instead
                }

                for (int i = 0; i < noOfThreads; i++)
                {
                    if (start + i < max)
                    {
                        Application.DoEvents();
                        var bw = new BackgroundWorker();
                        var p = new ImageCompareThreadParameter(this.GetSubtitleBitmap(start + i), start + i, this._compareBitmaps, bw, noOfThreads, (int)this.numericUpDownPixelsIsSpace.Value, this.checkBoxRightToLeft.Checked, (float)this.numericUpDownMaxErrorPct.Value);
                        bw.DoWork += ImageCompareThreadDoWork;
                        bw.RunWorkerCompleted += this.ImageCompareThreadRunWorkerCompleted;
                        bw.RunWorkerAsync(p);
                    }
                }
            }
        }

        /// <summary>
        /// The main loop.
        /// </summary>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MainLoop(int max, int i)
        {
            if (i >= max)
            {
                this.SetButtonsEnabledAfterOcrDone();
                this._mainOcrRunning = false;
                return true;
            }

            var bmp = this.ShowSubtitleImage(i);
            var startTime = new TimeCode(this.GetSubtitleStartTimeMilliseconds(i));
            var endTime = new TimeCode(this.GetSubtitleEndTimeMilliseconds(i));
            this.labelStatus.Text = string.Format("{0} / {1}: {2} - {3}", i + 1, max, startTime, endTime);
            this.progressBar1.Value = i + 1;
            this.labelStatus.Refresh();
            this.progressBar1.Refresh();
            if (this._abort)
            {
                bmp.Dispose();
                this.SetButtonsEnabledAfterOcrDone();
                this._mainOcrRunning = false;
                return true;
            }

            this._mainOcrBitmap = bmp;

            int j = i;
            this.subtitleListView1.Items[j].Selected = true;
            if (j < max - 1)
            {
                j++;
            }

            if (j < max - 1)
            {
                j++;
            }

            this.subtitleListView1.Items[j].EnsureVisible();

            string text = string.Empty;
            if (this.comboBoxOcrMethod.SelectedIndex == 0)
            {
                text = this.OcrViaTesseract(bmp, i);
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 1)
            {
                text = this.SplitAndOcrBitmapNormal(bmp, i);
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 2)
            {
                text = this.CallModi(i);
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 3)
            {
                text = this.OcrViaNOCR(bmp, i);
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 4)
            {
                text = this.SplitAndOcrBitmapNormalNew(bmp, i);
            }

            this._lastLine = text;

            text = text.Replace("<i>-</i>", "-");
            text = text.Replace("<i>a</i>", "a");
            text = text.Replace("<i>.</i>", ".");
            text = text.Replace("  ", " ");
            text = text.Trim();

            text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
            text = text.Replace(Environment.NewLine + " ", Environment.NewLine);

            // max allow 2 lines
            if (this.checkBoxAutoBreakLines.Checked && text.Replace(Environment.NewLine, "*").Length + 2 <= text.Length)
            {
                text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                while (text.Contains(Environment.NewLine + Environment.NewLine))
                {
                    text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                }

                if (text.Replace(Environment.NewLine, "*").Length + 2 <= text.Length)
                {
                    text = Utilities.AutoBreakLine(text);
                }
            }

            if (this._dvbSubtitles != null && this.checkBoxTransportStreamGetColorAndSplit.Checked)
            {
                text = Utilities.UnbreakLine(text);
                if (this._dvbSubColor != Color.Transparent)
                {
                    text = "<font color=\"" + ColorTranslator.ToHtml(this._dvbSubColor) + "\">" + text + "</font>";
                }
            }

            this._linesOcred++;

            if (this._abort)
            {
                this.textBoxCurrentText.Text = text;
                this._mainOcrRunning = false;
                this.SetButtonsEnabledAfterOcrDone();
                this._nocrThreadsStop = true;
                this._icThreadsStop = true;
                return true;
            }

            text = text.Trim();
            text = text.Replace("  ", " ");
            text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            text = text.Replace("  ", " ");
            text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            Paragraph p = this._subtitle.GetParagraphOrDefault(i);
            if (p != null)
            {
                p.Text = text;
            }

            if (this.subtitleListView1.SelectedItems.Count == 1 && this.subtitleListView1.SelectedItems[0].Index == i)
            {
                this.textBoxCurrentText.Text = text;
            }
            else
            {
                this.subtitleListView1.SetText(i, text);
            }

            return false;
        }

        /// <summary>
        /// The main ocr timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mainOcrTimer_Tick(object sender, EventArgs e)
        {
            this._mainOcrTimer.Stop();
            bool done = this.MainLoop(this._mainOcrTimerMax, this._mainOcrIndex);
            if (done || this._abort)
            {
                this.SetButtonsEnabledAfterOcrDone();
            }
            else
            {
                this._mainOcrIndex++;
                this._mainOcrTimer.Start();
            }
        }

        /// <summary>
        /// The load n ocr with current language.
        /// </summary>
        private void LoadNOcrWithCurrentLanguage()
        {
            string fileName = this.GetNOcrLanguageFileName();
            if (!string.IsNullOrEmpty(fileName))
            {
                this._nOcrDb = new NOcrDb(fileName);
            }
        }

        /// <summary>
        /// The save n ocr with current language.
        /// </summary>
        internal void SaveNOcrWithCurrentLanguage()
        {
            this.SaveNOcr();
        }

        /// <summary>
        /// The resize bitmap.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private static Bitmap ResizeBitmap(Bitmap b, int width, int height)
        {
            var result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result)) g.DrawImage(b, 0, 0, width, height);
            return result;
        }

        /// <summary>
        /// The un italic.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public static Bitmap UnItalic(Bitmap bmp, double factor)
        {
            int left = (int)(bmp.Height * factor);
            Bitmap unItaliced = new Bitmap(bmp.Width + left + 4, bmp.Height);

            Point[] destinationPoints = { new Point(0, 0), // destination for upper-left point of  original
                                          new Point(bmp.Width, 0), // destination for upper-right point of original
                                          new Point(left, bmp.Height) // destination for lower-left point of original
                                        };

            using (var g = Graphics.FromImage(unItaliced))
            {
                g.DrawImage(bmp, destinationPoints);
            }

            return unItaliced;
        }

        /// <summary>
        /// The tesseract 3 do ocr via exe.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="psmMode">
        /// The psm mode.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string Tesseract3DoOcrViaExe(Bitmap bmp, string language, string psmMode)
        {
            // change yellow color to white - easier for Tesseract
            var nbmp = new NikseBitmap(bmp);
            nbmp.ReplaceYellowWithWhite(); // optimized replace

            string tempTiffFileName = Path.GetTempPath() + Guid.NewGuid() + ".png";
            string tempTextFileName;
            using (var b = nbmp.GetBitmap())
            {
                b.Save(tempTiffFileName, System.Drawing.Imaging.ImageFormat.Png);
                tempTextFileName = Path.GetTempPath() + Guid.NewGuid();
            }

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(Configuration.TesseractFolder + "tesseract.exe");
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Arguments = "\"" + tempTiffFileName + "\" \"" + tempTextFileName + "\" -l " + language;

                if (this.checkBoxTesseractMusicOn.Checked)
                {
                    process.StartInfo.Arguments += "+music";
                }

                if (!string.IsNullOrEmpty(psmMode))
                {
                    process.StartInfo.Arguments += " " + psmMode.Trim();
                }

                process.StartInfo.Arguments += " hocr";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                if (Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.FileName = "tesseract";
                }
                else
                {
                    process.StartInfo.WorkingDirectory = Configuration.TesseractFolder;
                }

                try
                {
                    process.Start();
                }
                catch
                {
                    MessageBox.Show("Unable to start 'Tesseract' - make sure tesseract-ocr 3.x is installed!");
                    throw;
                }

                process.WaitForExit(5000);
            }

            string result = string.Empty;
            string outputFileName = tempTextFileName + ".html";
            if (!File.Exists(outputFileName))
            {
                outputFileName = tempTextFileName + ".hocr";
            }

            try
            {
                if (File.Exists(outputFileName))
                {
                    result = File.ReadAllText(outputFileName);
                    result = ParseHocr(result);
                    File.Delete(outputFileName);
                }

                File.Delete(tempTiffFileName);
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// The parse hocr.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ParseHocr(string html)
        {
            string s = html.Replace("<em>", "@001_____").Replace("</em>", "@002_____");

            int first = s.IndexOf('<');
            while (first >= 0)
            {
                int last = s.IndexOf('>');
                if (last > 0)
                {
                    s = s.Remove(first, last - first + 1);
                    first = s.IndexOf('<');
                }
                else
                {
                    first = -1;
                }
            }

            s = s.Trim();
            s = s.Replace("@001_____", "<i>").Replace("@002_____", "</i>");
            while (s.Contains("  "))
            {
                s = s.Replace("  ", " ");
            }

            s = s.Replace("</i> <i>", " ");

            // html escape decoding
            s = s.Replace("&amp;", "&");
            s = s.Replace("&lt;", "<");
            s = s.Replace("&gt;", ">");
            s = s.Replace("&quot;", "\"");
            s = s.Replace("&#39;", "'");
            s = s.Replace("&apos;", "'");

            while (s.Contains("\n\n"))
            {
                s = s.Replace("\n\n", "\n");
            }

            s = s.Replace("</i>\n<i>", "\n");
            s = s.Replace("\n", Environment.NewLine);

            return s;
        }

        /// <summary>
        /// The has single letters.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasSingleLetters(string line)
        {
            if (!this._ocrFixEngine.IsDictionaryLoaded || !this._ocrFixEngine.SpellCheckDictionaryName.StartsWith("en_"))
            {
                return false;
            }

            if (line.Contains('[') && line.Contains(']'))
            {
                line = line.Replace("[", string.Empty).Replace("]", string.Empty);
            }

            line = HtmlUtil.RemoveOpenCloseTags(line, HtmlUtil.TagItalic);

            int count = 0;
            var arr = line.Replace("a.m", string.Empty).Replace("p.m", string.Empty).Replace("o.r", string.Empty).Replace("e.g", string.Empty).Replace("Ph.D", string.Empty).Replace("d.t.s", string.Empty).Split(new[] { ' ', '.', '?', '!', '(', ')', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arr)
            {
                if (s.Length == 1 && !@"♪♫-:'”1234567890&aAI""".Contains(s))
                {
                    count++;
                }
            }

            return count > 0;
        }

        /// <summary>
        /// The ocr via tesseract.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string OcrViaTesseract(Bitmap bitmap, int index)
        {
            if (bitmap == null)
            {
                return string.Empty;
            }

            if (this._ocrFixEngine == null)
            {
                this.comboBoxDictionaries_SelectedIndexChanged(null, null);
            }

            const int badWords = 0;
            string textWithOutFixes;
            if (this._tesseractAsyncStrings != null && !string.IsNullOrEmpty(this._tesseractAsyncStrings[index]))
            {
                textWithOutFixes = this._tesseractAsyncStrings[index];
            }
            else
            {
                if (this._tesseractAsyncIndex <= index)
                {
                    this._tesseractAsyncIndex = index + 10;
                }

                textWithOutFixes = this.Tesseract3DoOcrViaExe(bitmap, this._languageId, "-psm 6"); // 6 = Assume a single uniform block of text.
            }

            if ((!textWithOutFixes.Contains(Environment.NewLine) || Utilities.CountTagInText(textWithOutFixes, '\n') > 2) && (textWithOutFixes.Length < 17 || bitmap.Height < 50))
            {
                string psm = this.Tesseract3DoOcrViaExe(bitmap, this._languageId, "-psm 7"); // 7 = Treat the image as a single text line.
                if (textWithOutFixes != psm)
                {
                    if (string.IsNullOrWhiteSpace(textWithOutFixes))
                    {
                        textWithOutFixes = psm;
                    }
                    else if (psm.Length > textWithOutFixes.Length)
                    {
                        if (!psm.Contains('9') && textWithOutFixes.Contains('9') || !psm.Contains('6') && textWithOutFixes.Contains('6') || !psm.Contains('5') && textWithOutFixes.Contains('5') || !psm.Contains('3') && textWithOutFixes.Contains('3') || !psm.Contains('1') && textWithOutFixes.Contains('1') || !psm.Contains('$') && textWithOutFixes.Contains('$') || !psm.Contains('•') && textWithOutFixes.Contains('•') || !psm.Contains('Y') && textWithOutFixes.Contains('Y') || !psm.Contains('\'') && textWithOutFixes.Contains('\'') || !psm.Contains('€') && textWithOutFixes.Contains('€'))
                        {
                            textWithOutFixes = psm;
                        }
                    }
                    else if (psm.Length == textWithOutFixes.Length && (!psm.Contains('0') && textWithOutFixes.Contains('0') || // these chars are often mistaken
                                                                       !psm.Contains('9') && textWithOutFixes.Contains('9') || !psm.Contains('8') && textWithOutFixes.Contains('8') || !psm.Contains('5') && textWithOutFixes.Contains('5') || !psm.Contains('3') && textWithOutFixes.Contains('3') || !psm.Contains('1') && textWithOutFixes.Contains('1') || !psm.Contains('$') && textWithOutFixes.Contains('$') || !psm.Contains('€') && textWithOutFixes.Contains('€') || !psm.Contains('•') && textWithOutFixes.Contains('•') || !psm.Contains('Y') && textWithOutFixes.Contains('Y') || !psm.Contains('\'') && textWithOutFixes.Contains('\'') || !psm.Contains('/') && textWithOutFixes.Contains('/') || !psm.Contains('(') && textWithOutFixes.Contains('(') || !psm.Contains(')') && textWithOutFixes.Contains(')') || !psm.Contains('_') && textWithOutFixes.Contains('_')))
                    {
                        textWithOutFixes = psm;
                    }
                    else if (psm.Length == textWithOutFixes.Length && psm.EndsWith('.') && !textWithOutFixes.EndsWith('.'))
                    {
                        textWithOutFixes = psm;
                    }
                }
            }

            if (!this.checkBoxTesseractItalicsOn.Checked)
            {
                textWithOutFixes = HtmlUtil.RemoveOpenCloseTags(textWithOutFixes, HtmlUtil.TagItalic);
            }

            // Sometimes Tesseract has problems with small fonts - it helps to make the image larger
            if (HtmlUtil.RemoveOpenCloseTags(textWithOutFixes, HtmlUtil.TagItalic).Replace("@", string.Empty).Replace("%", string.Empty).Replace("|", string.Empty).Trim().Length < 3 || Utilities.CountTagInText(textWithOutFixes, '\n') > 2)
            {
                string rs = this.TesseractResizeAndRetry(bitmap);
                textWithOutFixes = rs;
                if (!this.checkBoxTesseractItalicsOn.Checked)
                {
                    textWithOutFixes = HtmlUtil.RemoveOpenCloseTags(textWithOutFixes, HtmlUtil.TagItalic);
                }
            }

            // fix italics
            textWithOutFixes = FixItalics(textWithOutFixes);

            int numberOfWords = textWithOutFixes.Split((" " + Environment.NewLine).ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;

            string line = textWithOutFixes.Trim();
            if (this._ocrFixEngine.IsDictionaryLoaded)
            {
                if (this.checkBoxAutoFixCommonErrors.Checked)
                {
                    line = this._ocrFixEngine.FixOcrErrors(line, index, this._lastLine, true, this.GetAutoGuessLevel());
                }

                int correctWords;
                int wordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(line, out correctWords);
                int oldCorrectWords = correctWords;

                if (wordsNotFound > 0 || correctWords == 0)
                {
                    List<string> oldUnkownWords = new List<string>();
                    oldUnkownWords.AddRange(this._ocrFixEngine.UnknownWordsFound);
                    this._ocrFixEngine.UnknownWordsFound.Clear();

                    string newUnfixedText = this.TesseractResizeAndRetry(bitmap);
                    string newText = this._ocrFixEngine.FixOcrErrors(newUnfixedText, index, this._lastLine, true, this.GetAutoGuessLevel());
                    int newWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(newText, out correctWords);

                    if (wordsNotFound == 1 && newWordsNotFound == 1 && newUnfixedText.EndsWith("!!") && textWithOutFixes.EndsWith('u') && newText.Length > 1)
                    {
                        this._ocrFixEngine.UnknownWordsFound.Clear();
                        newText = textWithOutFixes.Substring(0, textWithOutFixes.Length - 1) + "!!";
                        newWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(newText, out correctWords);
                    }
                    else if ((!newText.Contains('9') || textWithOutFixes.Contains('9')) && (!newText.Replace("</i>", string.Empty).Contains('/') || textWithOutFixes.Replace("</i>", string.Empty).Contains('/')) && !string.IsNullOrWhiteSpace(newUnfixedText) && newWordsNotFound < wordsNotFound || (newWordsNotFound == wordsNotFound && newText.EndsWith('!') && textWithOutFixes.EndsWith('l')))
                    {
                        wordsNotFound = newWordsNotFound;
                        if (textWithOutFixes.Length > 3 && textWithOutFixes.EndsWith("...") && !newText.EndsWith('.') && !newText.EndsWith(',') && !newText.EndsWith('!') && !newText.EndsWith('?') && !newText.EndsWith("</i>"))
                        {
                            newText = newText.TrimEnd() + "...";
                        }
                        else if (textWithOutFixes.Length > 0 && textWithOutFixes.EndsWith('.') && !newText.EndsWith('.') && !newText.EndsWith(',') && !newText.EndsWith('!') && !newText.EndsWith('?') && !newText.EndsWith("</i>"))
                        {
                            newText = newText.TrimEnd() + ".";
                        }
                        else if (textWithOutFixes.Length > 0 && textWithOutFixes.EndsWith('?') && !newText.EndsWith('.') && !newText.EndsWith(',') && !newText.EndsWith('!') && !newText.EndsWith('?') && !newText.EndsWith("</i>"))
                        {
                            newText = newText.TrimEnd() + "?";
                        }

                        textWithOutFixes = newUnfixedText;
                        line = FixItalics(newText);
                    }
                    else if (correctWords > oldCorrectWords + 1 || (correctWords > oldCorrectWords && !textWithOutFixes.Contains(' ')))
                    {
                        wordsNotFound = newWordsNotFound;
                        textWithOutFixes = newUnfixedText;
                        line = newText;
                    }
                    else
                    {
                        this._ocrFixEngine.UnknownWordsFound.Clear();
                        this._ocrFixEngine.UnknownWordsFound.AddRange(oldUnkownWords);
                    }
                }

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && textWithOutFixes.Replace("~", string.Empty).Trim().Length < 2)
                {
                    if (this._bluRaySubtitles != null && !line.Contains("<i>"))
                    {
                        this._ocrFixEngine.AutoGuessesUsed.Clear();
                        this._ocrFixEngine.UnknownWordsFound.Clear();

                        // which is best - normal image or one color image?
                        var nbmp = new NikseBitmap(bitmap);
                        nbmp.MakeOneColor(Color.White);
                        Bitmap oneColorBitmap = nbmp.GetBitmap();
                        string oneColorText = this.Tesseract3DoOcrViaExe(oneColorBitmap, this._languageId, "-psm 6"); // 6 = Assume a single uniform block of text.
                        oneColorBitmap.Dispose();
                        nbmp = null;

                        if (oneColorText.Length > 1 && !oneColorText.Contains("CD") && (!oneColorText.Contains('0') || line.Contains('0')) && (!oneColorText.Contains('2') || line.Contains('2')) && (!oneColorText.Contains('3') || line.Contains('4')) && (!oneColorText.Contains('5') || line.Contains('5')) && (!oneColorText.Contains('9') || line.Contains('9')) && (!oneColorText.Contains('•') || line.Contains('•')) && (!oneColorText.Contains(')') || line.Contains(')')) && Utilities.CountTagInText(oneColorText, '(') < 2 && Utilities.CountTagInText(oneColorText, ')') < 2 && Utilities.GetNumberOfLines(oneColorText) < 4)
                        {
                            int modiCorrectWords;
                            int modiWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(oneColorText, out modiCorrectWords);
                            string modiTextOcrFixed = oneColorText;
                            if (this.checkBoxAutoFixCommonErrors.Checked)
                            {
                                modiTextOcrFixed = this._ocrFixEngine.FixOcrErrors(oneColorText, index, this._lastLine, false, this.GetAutoGuessLevel());
                            }

                            int modiOcrCorrectedCorrectWords;
                            int modiOcrCorrectedWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(modiTextOcrFixed, out modiOcrCorrectedCorrectWords);
                            if (modiOcrCorrectedWordsNotFound <= modiWordsNotFound)
                            {
                                oneColorText = modiTextOcrFixed;
                                modiWordsNotFound = modiOcrCorrectedWordsNotFound;
                                modiCorrectWords = modiOcrCorrectedCorrectWords;
                            }

                            if (modiWordsNotFound < wordsNotFound || (textWithOutFixes.Length == 1 && modiWordsNotFound == 0))
                            {
                                line = FixItalics(oneColorText); // use one-color text
                                wordsNotFound = modiWordsNotFound;
                                correctWords = modiCorrectWords;
                                if (this.checkBoxAutoFixCommonErrors.Checked)
                                {
                                    line = this._ocrFixEngine.FixOcrErrors(line, index, this._lastLine, true, this.GetAutoGuessLevel());
                                }
                            }
                            else if (wordsNotFound == modiWordsNotFound && oneColorText.EndsWith('!') && (line.EndsWith('l') || line.EndsWith('ﬂ')))
                            {
                                line = FixItalics(oneColorText);
                                wordsNotFound = modiWordsNotFound;
                                correctWords = modiCorrectWords;
                                if (this.checkBoxAutoFixCommonErrors.Checked)
                                {
                                    line = this._ocrFixEngine.FixOcrErrors(line, index, this._lastLine, true, this.GetAutoGuessLevel());
                                }
                            }
                        }
                    }
                }

                if (this.checkBoxTesseractItalicsOn.Checked)
                {
                    if (line.Contains("<i>") || wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && textWithOutFixes.Replace("~", string.Empty).Trim().Length < 2)
                    {
                        this._ocrFixEngine.AutoGuessesUsed.Clear();
                        this._ocrFixEngine.UnknownWordsFound.Clear();

                        // which is best - normal image or de-italic'ed? We find out here
                        var unItalicedBmp = UnItalic(bitmap, this._unItalicFactor);
                        string unItalicText = this.Tesseract3DoOcrViaExe(unItalicedBmp, this._languageId, "-psm 6"); // 6 = Assume a single uniform block of text.
                        unItalicedBmp.Dispose();

                        if (unItalicText.Length > 1)
                        {
                            int modiCorrectWords;
                            int modiWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(unItalicText, out modiCorrectWords);
                            string modiTextOcrFixed = unItalicText;
                            if (this.checkBoxAutoFixCommonErrors.Checked)
                            {
                                modiTextOcrFixed = this._ocrFixEngine.FixOcrErrors(unItalicText, index, this._lastLine, false, this.GetAutoGuessLevel());
                            }

                            int modiOcrCorrectedCorrectWords;
                            int modiOcrCorrectedWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(modiTextOcrFixed, out modiOcrCorrectedCorrectWords);
                            if (modiOcrCorrectedWordsNotFound <= modiWordsNotFound)
                            {
                                unItalicText = modiTextOcrFixed;
                                modiWordsNotFound = modiOcrCorrectedWordsNotFound;
                                modiCorrectWords = modiOcrCorrectedCorrectWords;
                            }

                            bool ok = modiWordsNotFound < wordsNotFound || (textWithOutFixes.Length == 1 && modiWordsNotFound == 0);

                            if (!ok)
                            {
                                ok = wordsNotFound == modiWordsNotFound && unItalicText.EndsWith('!') && (line.EndsWith('l') || line.EndsWith('ﬂ'));
                            }

                            if (!ok)
                            {
                                ok = wordsNotFound == modiWordsNotFound && line.StartsWith("<i>") && line.EndsWith("</i>");
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, '/') > Utilities.CountTagInText(line, '/') + 1)
                            {
                                ok = false;
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, '\\') > Utilities.CountTagInText(line, '\\'))
                            {
                                ok = false;
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, ')') > Utilities.CountTagInText(line, ')') + 1)
                            {
                                ok = false;
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, '(') > Utilities.CountTagInText(line, '(') + 1)
                            {
                                ok = false;
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, '$') > Utilities.CountTagInText(line, '$') + 1)
                            {
                                ok = false;
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, '€') > Utilities.CountTagInText(line, '€') + 1)
                            {
                                ok = false;
                            }

                            if (ok && Utilities.CountTagInText(unItalicText, '•') > Utilities.CountTagInText(line, '•'))
                            {
                                ok = false;
                            }

                            if (ok)
                            {
                                wordsNotFound = modiWordsNotFound;
                                correctWords = modiCorrectWords;

                                line = HtmlUtil.RemoveOpenCloseTags(line, HtmlUtil.TagItalic).Trim();

                                if (line.Length > 7 && unItalicText.Length > 7 && unItalicText.StartsWith("I ") && line.StartsWith(unItalicText.Remove(0, 2).Substring(0, 4)))
                                {
                                    unItalicText = unItalicText.Remove(0, 2);
                                }

                                if (this.checkBoxTesseractMusicOn.Checked)
                                {
                                    if ((line.StartsWith("J' ") || line.StartsWith("J“ ") || line.StartsWith("J* ") || line.StartsWith("♪ ")) && unItalicText.Length > 3 && HtmlUtil.RemoveOpenCloseTags(unItalicText, HtmlUtil.TagItalic).Substring(1, 2) == "' ")
                                    {
                                        unItalicText = "♪ " + unItalicText.Remove(0, 2).TrimStart();
                                    }

                                    if ((line.StartsWith("J' ") || line.StartsWith("J“ ") || line.StartsWith("J* ") || line.StartsWith("♪ ")) && unItalicText.Length > 3 && HtmlUtil.RemoveOpenCloseTags(unItalicText, HtmlUtil.TagItalic)[1] == ' ')
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 2).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if ((line.StartsWith("J' ") || line.StartsWith("J“ ") || line.StartsWith("J* ") || line.StartsWith("♪ ")) && unItalicText.Length > 3 && HtmlUtil.RemoveOpenCloseTags(unItalicText, HtmlUtil.TagItalic)[2] == ' ')
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 2).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if (unItalicText.StartsWith("J'") && (line.StartsWith('♪') || textWithOutFixes.StartsWith('♪') || textWithOutFixes.StartsWith("<i>♪") || unItalicText.EndsWith('♪')))
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 2).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if ((line.StartsWith("J` ") || line.StartsWith("J“ ") || line.StartsWith("J' ") || line.StartsWith("J* ")) && unItalicText.StartsWith("S "))
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 2).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if ((line.StartsWith("J` ") || line.StartsWith("J“ ") || line.StartsWith("J' ") || line.StartsWith("J* ")) && unItalicText.StartsWith("<i>S</i> "))
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 8).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if (unItalicText.StartsWith(";'") && (line.StartsWith('♪') || textWithOutFixes.StartsWith('♪') || textWithOutFixes.StartsWith("<i>♪") || unItalicText.EndsWith('♪')))
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 2).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if (unItalicText.StartsWith(",{*") && (line.StartsWith('♪') || textWithOutFixes.StartsWith('♪') || textWithOutFixes.StartsWith("<i>♪") || unItalicText.EndsWith('♪')))
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = "♪ " + unItalicText.Remove(0, 3).TrimStart();
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }

                                    if (unItalicText.EndsWith("J'") && (line.EndsWith('♪') || textWithOutFixes.EndsWith('♪') || textWithOutFixes.EndsWith("♪</i>") || unItalicText.StartsWith('♪')))
                                    {
                                        bool ita = unItalicText.StartsWith("<i>") && unItalicText.EndsWith("</i>");
                                        unItalicText = HtmlUtil.RemoveHtmlTags(unItalicText);
                                        unItalicText = unItalicText.Remove(unItalicText.Length - 3, 2).TrimEnd() + " ♪";
                                        if (ita)
                                        {
                                            unItalicText = "<i>" + unItalicText + "</i>";
                                        }
                                    }
                                }

                                if (unItalicText.StartsWith('[') && !line.StartsWith('['))
                                {
                                    unItalicText = unItalicText.Remove(0, 1);
                                    if (unItalicText.EndsWith(']'))
                                    {
                                        unItalicText = unItalicText.TrimEnd(']');
                                    }
                                }

                                if (unItalicText.StartsWith('{') && !line.StartsWith('{'))
                                {
                                    unItalicText = unItalicText.Remove(0, 1);
                                    if (unItalicText.EndsWith('}'))
                                    {
                                        unItalicText = unItalicText.TrimEnd('}');
                                    }
                                }

                                if (unItalicText.EndsWith('}') && !line.EndsWith('}'))
                                {
                                    unItalicText = unItalicText.TrimEnd('}');
                                }

                                if (line.EndsWith("...") && unItalicText.EndsWith("”!"))
                                {
                                    unItalicText = unItalicText.TrimEnd('!').TrimEnd('”') + ".";
                                }

                                if (line.EndsWith("...") && unItalicText.EndsWith("\"!"))
                                {
                                    unItalicText = unItalicText.TrimEnd('!').TrimEnd('"') + ".";
                                }

                                if (line.EndsWith('.') && !unItalicText.EndsWith('.') && !unItalicText.EndsWith(".</i>"))
                                {
                                    string post = string.Empty;
                                    if (unItalicText.EndsWith("</i>"))
                                    {
                                        post = "</i>";
                                        unItalicText = unItalicText.Remove(unItalicText.Length - 4);
                                    }

                                    if (unItalicText.EndsWith('\'') && !line.EndsWith("'."))
                                    {
                                        unItalicText = unItalicText.TrimEnd('\'');
                                    }

                                    unItalicText += "." + post;
                                }

                                if (unItalicText.EndsWith('.') && !unItalicText.EndsWith("...") && !unItalicText.EndsWith("...</i>") && line.EndsWith("..."))
                                {
                                    string post = string.Empty;
                                    if (unItalicText.EndsWith("</i>"))
                                    {
                                        post = "</i>";
                                        unItalicText = unItalicText.Remove(unItalicText.Length - 4);
                                    }

                                    unItalicText += ".." + post;
                                }

                                if (unItalicText.EndsWith("..") && !unItalicText.EndsWith("...") && !unItalicText.EndsWith("...</i>") && line.EndsWith("..."))
                                {
                                    string post = string.Empty;
                                    if (unItalicText.EndsWith("</i>"))
                                    {
                                        post = "</i>";
                                        unItalicText = unItalicText.Remove(unItalicText.Length - 4);
                                    }

                                    unItalicText += "." + post;
                                }

                                if (line.EndsWith('!') && !unItalicText.EndsWith('!') && !unItalicText.EndsWith("!</i>"))
                                {
                                    if (unItalicText.EndsWith("!'"))
                                    {
                                        unItalicText = unItalicText.TrimEnd('\'');
                                    }
                                    else
                                    {
                                        if (unItalicText.EndsWith("l</i>") && this._ocrFixEngine != null)
                                        {
                                            string w = unItalicText.Substring(0, unItalicText.Length - 4);
                                            int wIdx = w.Length - 1;
                                            while (wIdx >= 0 && !@" .,!?<>:;'-$@£()[]<>/""".Contains(w[wIdx]))
                                            {
                                                wIdx--;
                                            }

                                            if (wIdx + 1 < w.Length && unItalicText.Length > 5)
                                            {
                                                w = w.Substring(wIdx + 1);
                                                if (!this._ocrFixEngine.DoSpell(w))
                                                {
                                                    unItalicText = unItalicText.Remove(unItalicText.Length - 5, 1);
                                                }
                                            }

                                            unItalicText = unItalicText.Insert(unItalicText.Length - 4, "!");
                                        }
                                        else if (unItalicText.EndsWith('l') && this._ocrFixEngine != null)
                                        {
                                            string w = unItalicText;
                                            int wIdx = w.Length - 1;
                                            while (wIdx >= 0 && !@" .,!?<>:;'-$@£()[]<>/""".Contains(w[wIdx]))
                                            {
                                                wIdx--;
                                            }

                                            if (wIdx + 1 < w.Length && unItalicText.Length > 5)
                                            {
                                                w = w.Substring(wIdx + 1);
                                                if (!this._ocrFixEngine.DoSpell(w))
                                                {
                                                    unItalicText = unItalicText.Remove(unItalicText.Length - 1, 1);
                                                }
                                            }

                                            unItalicText += "!";
                                        }
                                        else
                                        {
                                            unItalicText += "!";
                                        }
                                    }
                                }

                                if (line.EndsWith('?') && !unItalicText.EndsWith('?') && !unItalicText.EndsWith("?</i>"))
                                {
                                    if (unItalicText.EndsWith("?'"))
                                    {
                                        unItalicText = unItalicText.TrimEnd('\'');
                                    }
                                    else
                                    {
                                        unItalicText += "?";
                                    }
                                }

                                line = HtmlUtil.RemoveOpenCloseTags(unItalicText, HtmlUtil.TagItalic);
                                if (this.checkBoxAutoFixCommonErrors.Checked)
                                {
                                    if (line.Contains("'.") && !textWithOutFixes.Contains("'.") && textWithOutFixes.Contains(':') && !line.EndsWith("'.") && Configuration.Settings.Tools.OcrFixUseHardcodedRules)
                                    {
                                        line = line.Replace("'.", ":");
                                    }

                                    line = this._ocrFixEngine.FixOcrErrors(line, index, this._lastLine, true, this.GetAutoGuessLevel());
                                }

                                line = "<i>" + line + "</i>";
                            }
                            else
                            {
                                unItalicText = unItalicText.Replace("</i>", string.Empty);
                                if (line.EndsWith("</i>") && unItalicText.EndsWith('.'))
                                {
                                    line = line.Remove(line.Length - 4, 4);
                                    if (line.EndsWith('-'))
                                    {
                                        line = line.TrimEnd('-') + ".";
                                    }

                                    if (Utilities.AllLetters.Contains(line.Substring(line.Length - 1)))
                                    {
                                        line += ".";
                                    }

                                    line += "</i>";
                                }
                            }
                        }
                    }
                }

                if (this.checkBoxTesseractMusicOn.Checked)
                {
                    if (line == "[J'J'J~]" || line == "[J'J'J']")
                    {
                        line = "[ ♪ ♪ ♪ ]";
                    }

                    line = line.Replace(" J' ", " ♪ ");

                    if (line.StartsWith("J'"))
                    {
                        line = "♪ " + line.Remove(0, 2).TrimStart();
                    }

                    if (line.StartsWith("<i>J'"))
                    {
                        line = "<i>♪ " + line.Remove(0, 5).TrimStart();
                    }

                    if (line.StartsWith("[J'"))
                    {
                        line = "[♪ " + line.Remove(0, 3).TrimStart();
                    }

                    if (line.StartsWith("<i>[J'"))
                    {
                        line = "<i>[♪ " + line.Remove(0, 6).TrimStart();
                    }

                    if (line.EndsWith("J'"))
                    {
                        line = line.Remove(line.Length - 2, 2).TrimEnd() + " ♪";
                    }

                    if (line.EndsWith("J'</i>"))
                    {
                        line = line.Remove(line.Length - 6, 6).TrimEnd() + " ♪</i>";
                    }

                    if (line.Contains(Environment.NewLine + "J'"))
                    {
                        line = line.Replace(Environment.NewLine + "J'", Environment.NewLine + "♪ ");
                        line = line.Replace("  ", " ");
                    }

                    if (line.Contains("J'" + Environment.NewLine))
                    {
                        line = line.Replace("J'" + Environment.NewLine, " ♪" + Environment.NewLine);
                        line = line.Replace("  ", " ");
                    }
                }

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && textWithOutFixes.Replace("~", string.Empty).Trim().Length < 2)
                {
                    this._ocrFixEngine.AutoGuessesUsed.Clear();
                    this._ocrFixEngine.UnknownWordsFound.Clear();

                    if (this._modiEnabled && this.checkBoxUseModiInTesseractForUnknownWords.Checked)
                    {
                        // which is best - modi or Tesseract - we find out here
                        string modiText = this.CallModi(index);

                        if (modiText.Length == 0)
                        {
                            modiText = this.CallModi(index); // retry... strange MODI
                        }

                        if (modiText.Length == 0)
                        {
                            modiText = this.CallModi(index); // retry... strange MODI
                        }

                        if (modiText.Length > 1 && !modiText.Contains("CD") && (!modiText.Contains('0') || line.Contains('0')) && (!modiText.Contains('2') || line.Contains('2')) && (!modiText.Contains('3') || line.Contains('4')) && (!modiText.Contains('5') || line.Contains('5')) && (!modiText.Contains('9') || line.Contains('9')) && (!modiText.Contains('•') || line.Contains('•')) && (!modiText.Contains(')') || line.Contains(')')) && Utilities.CountTagInText(modiText, '(') < 2 && Utilities.CountTagInText(modiText, ')') < 2 && Utilities.GetNumberOfLines(modiText) < 4)
                        {
                            int modiWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(modiText, out correctWords);
                            {
                                // if (modiWordsNotFound > 0)
                                string modiTextOcrFixed = modiText;
                                if (this.checkBoxAutoFixCommonErrors.Checked)
                                {
                                    modiTextOcrFixed = this._ocrFixEngine.FixOcrErrors(modiText, index, this._lastLine, false, this.GetAutoGuessLevel());
                                }

                                int modiOcrCorrectedWordsNotFound = this._ocrFixEngine.CountUnknownWordsViaDictionary(modiTextOcrFixed, out correctWords);
                                if (modiOcrCorrectedWordsNotFound <= modiWordsNotFound)
                                {
                                    modiText = modiTextOcrFixed;
                                }
                            }

                            if (modiWordsNotFound < wordsNotFound || (textWithOutFixes.Length == 1 && modiWordsNotFound == 0))
                            {
                                line = modiText; // use the modi OCR'ed text
                            }
                            else if (wordsNotFound == modiWordsNotFound && modiText.EndsWith('!') && (line.EndsWith('l') || line.EndsWith('ﬂ')))
                            {
                                line = modiText;
                            }
                        }

                        // take the best option - before OCR fixing, which we do again to save suggestions and prompt for user input
                        line = this._ocrFixEngine.FixUnknownWordsViaGuessOrPrompt(out wordsNotFound, line, index, bitmap, this.checkBoxAutoFixCommonErrors.Checked, this.checkBoxPromptForUnknownWords.Checked, true, this.GetAutoGuessLevel());
                    }
                    else
                    { // fix some error manually (modi not available)
                        line = this._ocrFixEngine.FixUnknownWordsViaGuessOrPrompt(out wordsNotFound, line, index, bitmap, this.checkBoxAutoFixCommonErrors.Checked, this.checkBoxPromptForUnknownWords.Checked, true, this.GetAutoGuessLevel());
                    }
                }

                if (this._ocrFixEngine.Abort)
                {
                    this.ButtonStopClick(null, null);
                    this._ocrFixEngine.Abort = false;
                    return string.Empty;
                }

                // check Tesseract... find an other way to do this...
                // string tmp = HtmlUtil.RemoveHtmlTags(line).Trim();
                // if (!tmp.TrimEnd().EndsWith("..."))
                // {
                // tmp = tmp.TrimEnd('.').TrimEnd();
                // if (tmp.Length > 2 && Utilities.LowercaseLetters.Contains(tmp[tmp.Length - 1]))
                // {
                // if (_nocrChars == null)
                // _nocrChars = LoadNOcrForTesseract("Nikse.SubtitleEdit.Resources.nOCR_TesseractHelper.xml.zip");
                // string text = HtmlUtil.RemoveHtmlTags(NocrFastCheck(bitmap).TrimEnd());
                // string post = string.Empty;
                // if (line.EndsWith("</i>"))
                // {
                // post = "</i>";
                // line = line.Remove(line.Length - 4, 4).Trim();
                // }
                // if (text.EndsWith('.'))
                // {
                // line = line.TrimEnd('.').Trim();
                // while (text.EndsWith('.') || text.EndsWith(' '))
                // {
                // line += text.Substring(text.Length - 1).Trim();
                // text = text.Remove(text.Length - 1, 1);
                // }
                // }
                // else if (text.EndsWith('l') && text.EndsWith('!') && !text.EndsWith("l!"))
                // {
                // line = line.Remove(line.Length - 1, 1) + "!";
                // }
                // line += post;
                // }
                // }

                // Log used word guesses (via word replace list)
                foreach (string guess in this._ocrFixEngine.AutoGuessesUsed)
                {
                    this.listBoxLogSuggestions.Items.Add(guess);
                }

                this._ocrFixEngine.AutoGuessesUsed.Clear();

                // Log unkown words guess (found via spelling dictionaries)
                this.LogUnknownWords();

                if (wordsNotFound >= 3)
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Red);
                }

                if (wordsNotFound == 2)
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Orange);
                }
                else if (wordsNotFound == 1 || line.Length == 1 || line.Contains('_') || this.HasSingleLetters(line))
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Yellow);
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Orange);
                }
                else
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen);
                }
            }
            else
            { // no dictionary :(
                if (this.checkBoxAutoFixCommonErrors.Checked)
                {
                    line = this._ocrFixEngine.FixOcrErrors(line, index, this._lastLine, true, this.GetAutoGuessLevel());
                }

                if (badWords >= numberOfWords)
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Red);
                }
                else if (badWords >= numberOfWords / 2)
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Orange);
                }
                else if (badWords > 0 || line.Contains('_') || this.HasSingleLetters(line))
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Yellow);
                }
                else if (string.IsNullOrWhiteSpace(HtmlUtil.RemoveOpenCloseTags(line, HtmlUtil.TagItalic)))
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.Orange);
                }
                else
                {
                    this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen);
                }
            }

            if (textWithOutFixes.Trim() != line.Trim())
            {
                this._tesseractOcrAutoFixes++;
                this.labelFixesMade.Text = string.Format(" - {0}", this._tesseractOcrAutoFixes);
                this.LogOcrFix(index, textWithOutFixes, line);
            }

            if (this._vobSubMergedPackist != null)
            {
                bitmap.Dispose();
            }

            return line;
        }

        /// <summary>
        /// The fix italics.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FixItalics(string s)
        {
            int italicStartCount = Utilities.CountTagInText(s, "<i>");
            if (italicStartCount == 0)
            {
                return s;
            }

            s = s.Replace(Environment.NewLine + " ", Environment.NewLine);
            s = s.Replace(Environment.NewLine + " ", Environment.NewLine);
            s = s.Replace(" " + Environment.NewLine, Environment.NewLine);
            s = s.Replace(" " + Environment.NewLine, Environment.NewLine);
            s = s.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            s = s.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            if (italicStartCount == 1 && s.Contains("<i>-</i>"))
            {
                s = s.Replace("<i>-</i>", "-");
                s = s.Replace("  ", " ");
            }

            if (s.Contains("</i> / <i>"))
            {
                s = s.Replace("</i> / <i>", " I ").Replace("  ", " ");
            }

            if (s.StartsWith("/ <i>"))
            {
                s = ("<i>I " + s.Remove(0, 5)).Replace("  ", " ");
            }

            if (s.StartsWith("I <i>"))
            {
                s = ("<i>I " + s.Remove(0, 5)).Replace("  ", " ");
            }
            else if (italicStartCount == 1 && s.Length > 20 && s.IndexOf("<i>", StringComparison.Ordinal) > 1 && s.IndexOf("<i>", StringComparison.Ordinal) < 10 && s.EndsWith("</i>"))
            {
                s = "<i>" + HtmlUtil.RemoveOpenCloseTags(s, HtmlUtil.TagItalic) + "</i>";
            }

            s = s.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine);

            return HtmlUtil.FixInvalidItalicTags(s);
        }

        /// <summary>
        /// The log unknown words.
        /// </summary>
        private void LogUnknownWords()
        {
            foreach (string unknownWord in this._ocrFixEngine.UnknownWordsFound)
            {
                this.listBoxUnknownWords.Items.Add(unknownWord);
            }

            this._ocrFixEngine.UnknownWordsFound.Clear();
        }

        /// <summary>
        /// The tesseract resize and retry.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string TesseractResizeAndRetry(Bitmap bitmap)
        {
            string result = this.Tesseract3DoOcrViaExe(ResizeBitmap(bitmap, bitmap.Width * 3, bitmap.Height * 2), this._languageId, null);
            if (string.IsNullOrWhiteSpace(result))
            {
                result = this.Tesseract3DoOcrViaExe(ResizeBitmap(bitmap, bitmap.Width * 4, bitmap.Height * 2), this._languageId, "-psm 7");
            }

            return result.TrimEnd();
        }

        /// <summary>
        /// The log ocr fix.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="oldLine">
        /// The old line.
        /// </param>
        /// <param name="newLine">
        /// The new line.
        /// </param>
        private void LogOcrFix(int index, string oldLine, string newLine)
        {
            this.listBoxLog.Items.Add(string.Format("#{0}: {1} -> {2}", index + 1, oldLine.Replace(Environment.NewLine, " "), newLine.Replace(Environment.NewLine, " ")));
        }

        /// <summary>
        /// The call modi.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CallModi(int i)
        {
            Bitmap bmp;
            try
            {
                var tmp = this.GetSubtitleBitmap(i);
                if (tmp == null)
                {
                    return string.Empty;
                }

                bmp = tmp.Clone() as Bitmap;
                tmp.Dispose();
            }
            catch
            {
                return string.Empty;
            }

            var mp = new ModiParameter { Bitmap = bmp, Text = string.Empty, Language = this.GetModiLanguage() };

            // We call in a seperate thread... or app will crash sometimes :(
            var modiThread = new System.Threading.Thread(DoWork);
            modiThread.Start(mp);
            modiThread.Join(3000); // wait max 3 seconds
            modiThread.Abort();

            if (!string.IsNullOrEmpty(mp.Text) && mp.Text.Length > 3 && mp.Text.EndsWith(";0]"))
            {
                mp.Text = mp.Text.Substring(0, mp.Text.Length - 3);
            }

            // Try to avoid blank lines by resizing image
            if (string.IsNullOrEmpty(mp.Text))
            {
                bmp = ResizeBitmap(bmp, (int)(bmp.Width * 1.2), (int)(bmp.Height * 1.2));
                mp = new ModiParameter { Bitmap = bmp, Text = string.Empty, Language = this.GetModiLanguage() };

                // We call in a seperate thread... or app will crash sometimes :(
                modiThread = new System.Threading.Thread(DoWork);
                modiThread.Start(mp);
                modiThread.Join(3000); // wait max 3 seconds
                modiThread.Abort();
            }

            int k = 0;
            while (string.IsNullOrEmpty(mp.Text) && k < 5)
            {
                if (string.IsNullOrEmpty(mp.Text))
                {
                    bmp = ResizeBitmap(bmp, (int)(bmp.Width * 1.3), (int)(bmp.Height * 1.4)); // a bit scaling
                    mp = new ModiParameter { Bitmap = bmp, Text = string.Empty, Language = this.GetModiLanguage() };

                    // We call in a seperate thread... or app will crash sometimes :(
                    modiThread = new System.Threading.Thread(DoWork);
                    modiThread.Start(mp);
                    modiThread.Join(3000); // wait max 3 seconds
                    modiThread.Abort();
                    k++;
                }
            }

            if (bmp != null)
            {
                bmp.Dispose();
            }

            if (mp.Text != null)
            {
                mp.Text = mp.Text.Replace("•", "o");
            }

            return mp.Text;
        }

        /// <summary>
        /// The do work.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public static void DoWork(object data)
        {
            var paramter = (ModiParameter)data;
            string fileName = Path.GetTempPath() + Path.DirectorySeparatorChar + Guid.NewGuid() + ".bmp";
            object ocrResult = null;
            try
            {
                paramter.Bitmap.Save(fileName);

                Type modiDocType = Type.GetTypeFromProgID("MODI.Document");
                object modiDoc = Activator.CreateInstance(modiDocType);
                modiDocType.InvokeMember("Create", BindingFlags.InvokeMethod, null, modiDoc, new object[] { fileName });

                modiDocType.InvokeMember("OCR", BindingFlags.InvokeMethod, null, modiDoc, new object[] { paramter.Language, true, true });

                object images = modiDocType.InvokeMember("Images", BindingFlags.GetProperty, null, modiDoc, new object[] { });
                Type imagesType = images.GetType();

                object item = imagesType.InvokeMember("Item", BindingFlags.GetProperty, null, images, new object[] { "0" });
                Type itemType = item.GetType();

                object layout = itemType.InvokeMember("Layout", BindingFlags.GetProperty, null, item, new object[] { });
                Type layoutType = layout.GetType();
                ocrResult = layoutType.InvokeMember("Text", BindingFlags.GetProperty, null, layout, new object[] { });

                modiDocType.InvokeMember("Close", BindingFlags.InvokeMethod, null, modiDoc, new object[] { false });
            }
            catch
            {
                paramter.Text = string.Empty;
            }

            try
            {
                File.Delete(fileName);
            }
            catch
            {
            }

            if (ocrResult != null)
            {
                paramter.Text = ocrResult.ToString().Trim();
            }
        }

        /// <summary>
        /// The initialize modi.
        /// </summary>
        private void InitializeModi()
        {
            this._modiEnabled = false;
            this.checkBoxUseModiInTesseractForUnknownWords.Enabled = false;
            this.comboBoxModiLanguage.Enabled = false;
            try
            {
                this.InitializeModiLanguages();

                this._modiType = Type.GetTypeFromProgID("MODI.Document");
                this._modiDoc = Activator.CreateInstance(this._modiType);

                this._modiEnabled = this._modiDoc != null;
                this.comboBoxModiLanguage.Enabled = this._modiEnabled;
                this.checkBoxUseModiInTesseractForUnknownWords.Enabled = this._modiEnabled;
            }
            catch
            {
                this._modiEnabled = false;
            }

            if (!this._modiEnabled && this.comboBoxOcrMethod.Items.Count == 3)
            {
                this.comboBoxOcrMethod.Items.RemoveAt(2);
            }
        }

        /// <summary>
        /// The initialize tesseract.
        /// </summary>
        private void InitializeTesseract()
        {
            if (!Directory.Exists(Configuration.TesseractFolder))
            {
                Directory.CreateDirectory(Configuration.TesseractFolder);
                if (!Configuration.IsRunningOnLinux() && !Configuration.IsRunningOnMac())
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "xcopy";
                    startInfo.Arguments = "\"" + Path.Combine(Configuration.TesseractOriginalFolder, "*.*") + "\" \"" + Configuration.TesseractFolder + "\" /s";
                    MessageBox.Show(startInfo.Arguments);
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
            }

            string dir = Path.Combine(Configuration.TesseractFolder, "tessdata");
            if (Directory.Exists(dir))
            {
                var list = new List<string>();
                this.comboBoxTesseractLanguages.Items.Clear();
                foreach (var culture in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    string tesseractName = culture.ThreeLetterISOLanguageName;
                    if (culture.LCID == 0x4 && !File.Exists(dir + Path.DirectorySeparatorChar + tesseractName + ".traineddata"))
                    {
                        tesseractName = "chi_sim";
                    }

                    if (culture.Name == "zh-CHT" && !File.Exists(dir + Path.DirectorySeparatorChar + tesseractName + ".traineddata"))
                    {
                        tesseractName = "chi_tra";
                    }

                    if (tesseractName == "fas" && !File.Exists(dir + Path.DirectorySeparatorChar + tesseractName + ".traineddata"))
                    {
                        tesseractName = "per";
                    }

                    if (tesseractName == "nob" && !File.Exists(dir + Path.DirectorySeparatorChar + tesseractName + ".traineddata"))
                    {
                        tesseractName = "nor";
                    }

                    string trainDataFileName = dir + Path.DirectorySeparatorChar + tesseractName + ".traineddata";
                    if (!list.Contains(culture.ThreeLetterISOLanguageName) && File.Exists(trainDataFileName))
                    {
                        if (culture.ThreeLetterISOLanguageName != "zho")
                        {
                            list.Add(culture.ThreeLetterISOLanguageName);
                        }

                        this.comboBoxTesseractLanguages.Items.Add(new TesseractLanguage { Id = tesseractName, Text = culture.EnglishName });
                    }
                }
            }

            if (this.comboBoxTesseractLanguages.Items.Count > 0)
            {
                for (int i = 0; i < this.comboBoxTesseractLanguages.Items.Count; i++)
                {
                    if ((this.comboBoxTesseractLanguages.Items[i] as TesseractLanguage).Id == Configuration.Settings.VobSubOcr.TesseractLastLanguage)
                    {
                        this.comboBoxTesseractLanguages.SelectedIndex = i;
                    }
                }

                if (this.comboBoxTesseractLanguages.SelectedIndex == -1)
                {
                    this.comboBoxTesseractLanguages.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// The initialize modi languages.
        /// </summary>
        private void InitializeModiLanguages()
        {
            foreach (ModiLanguage ml in ModiLanguage.AllLanguages)
            {
                this.comboBoxModiLanguage.Items.Add(ml);
                if (ml.Id == this._vobSubOcrSettings.LastModiLanguageId)
                {
                    this.comboBoxModiLanguage.SelectedIndex = this.comboBoxModiLanguage.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// The get modi language.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetModiLanguage()
        {
            if (this.comboBoxModiLanguage.SelectedIndex < 0)
            {
                return ModiLanguage.DefaultLanguageId;
            }

            return ((ModiLanguage)this.comboBoxModiLanguage.SelectedItem).Id;
        }

        /// <summary>
        /// The button stop click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStopClick(object sender, EventArgs e)
        {
            if (this._mainOcrTimer != null)
            {
                this._mainOcrTimer.Stop();
            }

            this._abort = true;
            this._nocrThreadsStop = true;
            this._icThreadsStop = true;
            this.buttonStop.Enabled = false;
            this.progressBar1.Visible = false;
            this.labelStatus.Text = string.Empty;
            this.SetButtonsEnabledAfterOcrDone();
        }

        /// <summary>
        /// The subtitle list view 1 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count > 0)
            {
                try
                {
                    this._selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                }
                catch
                {
                    return;
                }

                this.textBoxCurrentText.Text = this._subtitle.Paragraphs[this._selectedIndex].Text;
                if (this._mainOcrRunning && this._mainOcrBitmap != null)
                {
                    this.ShowSubtitleImage(this._selectedIndex, this._mainOcrBitmap);
                }
                else
                {
                    // TODO: Refactor ShowSubtitleImage
                    var bmp = this.ShowSubtitleImage(this._selectedIndex);
                    bmp.Dispose();
                }

                this.numericUpDownStartNumber.Value = this._selectedIndex + 1;
            }
            else
            {
                this._selectedIndex = -1;
                this.textBoxCurrentText.Text = string.Empty;
            }
        }

        /// <summary>
        /// The text box current text text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxCurrentTextTextChanged(object sender, EventArgs e)
        {
            if (this._selectedIndex >= 0)
            {
                string text = this.textBoxCurrentText.Text.TrimEnd();
                this._subtitle.Paragraphs[this._selectedIndex].Text = text;
                this.subtitleListView1.SetText(this._selectedIndex, text);
            }

            FixVerticalScrollBars(this.textBoxCurrentText);
        }

        /// <summary>
        /// The fix vertical scroll bars.
        /// </summary>
        /// <param name="tb">
        /// The tb.
        /// </param>
        private static void FixVerticalScrollBars(TextBox tb)
        {
            if (Utilities.GetNumberOfLines(tb.Text) > 5)
            {
                tb.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                tb.ScrollBars = ScrollBars.None;
            }
        }

        /// <summary>
        /// The button new character database click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonNewCharacterDatabaseClick(object sender, EventArgs e)
        {
            using (var newFolder = new VobSubOcrNewFolder(this.comboBoxOcrMethod.SelectedIndex == 1))
            {
                if (newFolder.ShowDialog(this) == DialogResult.OK)
                {
                    if (this.comboBoxOcrMethod.SelectedIndex == 4)
                    {
                        try
                        {
                            string fileName = Path.Combine(Configuration.OcrFolder, newFolder.FolderName + ".db");
                            if (File.Exists(fileName))
                            {
                                MessageBox.Show("OCR db already exists!");
                                return;
                            }

                            this.comboBoxCharacterDatabase.Items.Add(newFolder.FolderName);
                            this.comboBoxCharacterDatabase.SelectedIndex = this.comboBoxCharacterDatabase.Items.Count - 1;
                            this._binaryOcrDb = new BinaryOcrDb(fileName);
                            this._binaryOcrDb.Save();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }
                    }
                    else
                    {
                        this._vobSubOcrSettings.LastImageCompareFolder = newFolder.FolderName;
                        this.LoadImageCompareCharacterDatabaseList();
                        this.LoadImageCompareBitmaps();
                    }
                }
            }
        }

        /// <summary>
        /// The combo box character database selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxCharacterDatabaseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxOcrMethod.SelectedIndex == 4)
            {
                this._binaryOcrDbFileName = Configuration.OcrFolder + this.comboBoxCharacterDatabase.SelectedItem + ".db";
            }

            this.LoadImageCompareBitmaps();
            if (this._vobSubOcrSettings != null)
            {
                this._vobSubOcrSettings.LastImageCompareFolder = this.comboBoxCharacterDatabase.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// The combo box modi language selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxModiLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            this._vobSubOcrSettings.LastModiLanguageId = this.GetModiLanguage();
        }

        /// <summary>
        /// The button edit character database click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonEditCharacterDatabaseClick(object sender, EventArgs e)
        {
            this.EditImageCompareCharacters(null, null);
        }

        /// <summary>
        /// The edit image compare characters.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public DialogResult EditImageCompareCharacters(string name, string text)
        {
            using (var formVobSubEditCharacters = new VobSubEditCharacters(this.comboBoxCharacterDatabase.SelectedItem.ToString(), null, this._binaryOcrDb))
            {
                formVobSubEditCharacters.Initialize(name, text);
                DialogResult result = formVobSubEditCharacters.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (this._binaryOcrDb != null)
                    {
                        this._binaryOcrDb.Save();
                    }
                    else
                    {
                        this._compareDoc = formVobSubEditCharacters.ImageCompareDocument;
                        string path = Configuration.VobSubCompareFolder + this.comboBoxCharacterDatabase.SelectedItem + Path.DirectorySeparatorChar;
                        this._compareDoc.Save(path + "Images.xml");
                        this.Cursor = Cursors.WaitCursor;
                        if (formVobSubEditCharacters.ChangesMade)
                        {
                            this._binaryOcrDb.LoadCompareImages();
                        }

                        this.Cursor = Cursors.Default;
                    }

                    return result;
                }

                this.Cursor = Cursors.WaitCursor;
                if (formVobSubEditCharacters.ChangesMade)
                {
                    this._binaryOcrDb.LoadCompareImages();
                }

                this.Cursor = Cursors.Default;
                return result;
            }
        }

        /// <summary>
        /// The vob sub ocr_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#importvobsub");
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt)
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex++;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
            }
            else if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt)
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex--;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G)
            {
                using (var goToLine = new GoToLine())
                {
                    goToLine.Initialize(1, this.subtitleListView1.Items.Count);
                    if (goToLine.ShowDialog(this) == DialogResult.OK)
                    {
                        this.subtitleListView1.SelectNone();
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].Selected = true;
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].EnsureVisible();
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].Focused = true;
                    }
                }
            }
            else if (this._mainGeneralGoToNextSubtitle == e.KeyData)
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex++;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (this._mainGeneralGoToPrevSubtitle == e.KeyData)
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex--;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The combo box tesseract languages selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxTesseractLanguagesSelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration.Settings.VobSubOcr.TesseractLastLanguage = (this.comboBoxTesseractLanguages.SelectedItem as TesseractLanguage).Id;
            if (this._ocrFixEngine != null)
            {
                this._ocrFixEngine.Dispose();
            }

            this._ocrFixEngine = null;
            this.LoadOcrFixEngine(null, null);
        }

        /// <summary>
        /// The load ocr fix engine.
        /// </summary>
        /// <param name="threeLetterISOLanguageName">
        /// The three letter iso language name.
        /// </param>
        /// <param name="hunspellName">
        /// The hunspell name.
        /// </param>
        private void LoadOcrFixEngine(string threeLetterISOLanguageName, string hunspellName)
        {
            if (string.IsNullOrEmpty(threeLetterISOLanguageName) && this.comboBoxTesseractLanguages.SelectedItem != null)
            {
                this._languageId = (this.comboBoxTesseractLanguages.SelectedItem as TesseractLanguage).Id;
                threeLetterISOLanguageName = this._languageId;
            }

            if (this._ocrFixEngine != null)
            {
                this._ocrFixEngine.Dispose();
            }

            this._ocrFixEngine = new OcrFixEngine(threeLetterISOLanguageName, hunspellName, this);
            if (this._ocrFixEngine.IsDictionaryLoaded)
            {
                string loadedDictionaryName = this._ocrFixEngine.SpellCheckDictionaryName;
                int i = 0;
                this.comboBoxDictionaries.SelectedIndexChanged -= this.comboBoxDictionaries_SelectedIndexChanged;
                foreach (string item in this.comboBoxDictionaries.Items)
                {
                    if (item.Contains("[" + loadedDictionaryName + "]"))
                    {
                        this.comboBoxDictionaries.SelectedIndex = i;
                    }

                    i++;
                }

                this.comboBoxDictionaries.SelectedIndexChanged += this.comboBoxDictionaries_SelectedIndexChanged;
                this.comboBoxDictionaries.Left = this.labelDictionaryLoaded.Left + this.labelDictionaryLoaded.Width;
                this.comboBoxDictionaries.Width = this.groupBoxOcrAutoFix.Width - (this.comboBoxDictionaries.Left + 10 + this.buttonSpellCheckDownload.Width);
            }
            else
            {
                this.comboBoxDictionaries.SelectedIndex = 0;
            }

            if (this._modiEnabled && this.checkBoxUseModiInTesseractForUnknownWords.Checked)
            {
                string tesseractLanguageText = (this.comboBoxTesseractLanguages.SelectedItem as TesseractLanguage).Text;
                int i = 0;
                foreach (var modiLanguage in this.comboBoxModiLanguage.Items)
                {
                    if ((modiLanguage as ModiLanguage).Text == tesseractLanguageText)
                    {
                        this.comboBoxModiLanguage.SelectedIndex = i;
                    }

                    i++;
                }
            }

            this.comboBoxModiLanguage.SelectedIndex = -1;
        }

        /// <summary>
        /// The combo box ocr method selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxOcrMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            this._icThreadsStop = true;
            this._binaryOcrDb = null;
            this._nOcrDb = null;
            if (this.comboBoxOcrMethod.SelectedIndex == 0)
            {
                this.ShowOcrMethodGroupBox(this.GroupBoxTesseractMethod);
                Configuration.Settings.VobSubOcr.LastOcrMethod = "Tesseract";
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 1)
            {
                this.ShowOcrMethodGroupBox(this.groupBoxImageCompareMethod);
                Configuration.Settings.VobSubOcr.LastOcrMethod = "BitmapCompare";
                this.checkBoxPromptForUnknownWords.Checked = false;
                this.LoadImageCompareCharacterDatabaseList();
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 2)
            {
                this.ShowOcrMethodGroupBox(this.groupBoxModiMethod);
                Configuration.Settings.VobSubOcr.LastOcrMethod = "MODI";
            }
            else if (this.comboBoxOcrMethod.SelectedIndex == 3)
            {
                this.ShowOcrMethodGroupBox(this.groupBoxNOCR);
                Configuration.Settings.VobSubOcr.LastOcrMethod = "nOCR";
                this.SetSpellCheckLanguage(Configuration.Settings.VobSubOcr.LineOcrLastSpellCheck);

                this.comboBoxNOcrLanguage.Items.Clear();
                int index = 0;
                int selIndex = 0;
                foreach (string fileName in Directory.GetFiles(Configuration.OcrFolder, "*.nocr"))
                {
                    string s = Path.GetFileNameWithoutExtension(fileName);
                    if (s == Configuration.Settings.VobSubOcr.LineOcrLastLanguages)
                    {
                        selIndex = index;
                    }

                    this.comboBoxNOcrLanguage.Items.Add(s);
                    index++;
                }

                if (this.comboBoxNOcrLanguage.Items.Count > 0)
                {
                    this.comboBoxNOcrLanguage.SelectedIndex = selIndex;
                }
            }
            else
            {
                this.ShowOcrMethodGroupBox(this.groupBoxImageCompareMethod);
                Configuration.Settings.VobSubOcr.LastOcrMethod = "BitmapCompare";
                this.checkBoxPromptForUnknownWords.Checked = false;
                this.numericUpDownMaxErrorPct.Minimum = 0;
                this._binaryOcrDb = new BinaryOcrDb(this._binaryOcrDbFileName, true);
                this.LoadImageCompareCharacterDatabaseList();
            }

            this.SubtitleListView1SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The get n ocr language file name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetNOcrLanguageFileName()
        {
            if (this.comboBoxNOcrLanguage.SelectedIndex < 0)
            {
                return null;
            }

            return Configuration.OcrFolder + this.comboBoxNOcrLanguage.Items[this.comboBoxNOcrLanguage.SelectedIndex] + ".nocr";
        }

        /// <summary>
        /// The show ocr method group box.
        /// </summary>
        /// <param name="groupBox">
        /// The group box.
        /// </param>
        private void ShowOcrMethodGroupBox(GroupBox groupBox)
        {
            this.GroupBoxTesseractMethod.Visible = false;
            this.groupBoxImageCompareMethod.Visible = false;
            this.groupBoxModiMethod.Visible = false;
            this.groupBoxNOCR.Visible = false;

            groupBox.Visible = true;
            groupBox.BringToFront();
            groupBox.Left = this.comboBoxOcrMethod.Left;
            groupBox.Top = 50;
        }

        /// <summary>
        /// The list box log selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxLogSelectedIndexChanged(object sender, EventArgs e)
        {
            var lb = sender as ListBox;
            if (lb != null && lb.SelectedIndex >= 0)
            {
                string text = lb.Items[lb.SelectedIndex].ToString();
                if (text.Contains(':'))
                {
                    string number = text.Substring(1, text.IndexOf(':') - 1);
                    this.subtitleListView1.SelectIndexAndEnsureVisible(int.Parse(number) - 1);
                }
            }
        }

        /// <summary>
        /// The context menu strip listview opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ContextMenuStripListviewOpening(object sender, CancelEventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count == 0)
            {
                e.Cancel = true;
            }

            if (this.contextMenuStripListview.SourceControl == this.subtitleListView1)
            {
                this.normalToolStripMenuItem.Visible = true;
                this.italicToolStripMenuItem.Visible = true;
                this.toolStripSeparator1.Visible = true;
                this.toolStripSeparator1.Visible = this.subtitleListView1.SelectedItems.Count == 1;
                this.saveImageAsToolStripMenuItem.Visible = this.subtitleListView1.SelectedItems.Count == 1;
            }
            else
            {
                this.normalToolStripMenuItem.Visible = false;
                this.italicToolStripMenuItem.Visible = false;
                this.toolStripSeparator1.Visible = false;
                this.saveImageAsToolStripMenuItem.Visible = true;
            }

            if (this.comboBoxOcrMethod.SelectedIndex == 1 || this.comboBoxOcrMethod.SelectedIndex == 4)
            {
                // image compare
                this.toolStripSeparatorImageCompare.Visible = true;
                this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Visible = true;
                this.EditLastAdditionsToolStripMenuItem.Visible = this._lastAdditions != null && this._lastAdditions.Count > 0;
            }
            else
            {
                this.toolStripSeparatorImageCompare.Visible = false;
                this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Visible = false;
                this.EditLastAdditionsToolStripMenuItem.Visible = false;
            }

            if (this.comboBoxOcrMethod.SelectedIndex == 3)
            {
                // nocr compare
                this.toolStripMenuItemInspectNOcrMatches.Visible = true;
                this.toolStripSeparatorImageCompare.Visible = true;
                this.nOcrTrainingToolStripMenuItem.Visible = true;
            }
            else
            {
                this.toolStripMenuItemInspectNOcrMatches.Visible = false;
                this.toolStripSeparatorImageCompare.Visible = false;
                this.nOcrTrainingToolStripMenuItem.Visible = false;
            }
        }

        /// <summary>
        /// The save image as tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SaveImageAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.saveFileDialog1.Title = Configuration.Settings.Language.VobSubOcr.SaveSubtitleImageAs;
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.FileName = "Image" + this._selectedIndex;
            this.saveFileDialog1.Filter = "PNG image|*.png|BMP image|*.bmp|GIF image|*.gif|TIFF image|*.tiff";
            this.saveFileDialog1.FilterIndex = 0;

            DialogResult result = this.saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Bitmap bmp = this.GetSubtitleBitmap(this._selectedIndex);
                if (bmp == null)
                {
                    MessageBox.Show("No image!");
                    return;
                }

                try
                {
                    if (this.saveFileDialog1.FilterIndex == 0)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 1)
                    {
                        bmp.Save(this.saveFileDialog1.FileName);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 2)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    bmp.Dispose();
                }
            }
        }

        /// <summary>
        /// The normal tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NormalToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this._subtitle.Paragraphs.Count > 0 && this.subtitleListView1.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in this.subtitleListView1.SelectedItems)
                {
                    Paragraph p = this._subtitle.GetParagraphOrDefault(item.Index);
                    if (p != null)
                    {
                        p.Text = HtmlUtil.RemoveHtmlTags(p.Text);
                        this.subtitleListView1.SetText(item.Index, p.Text);
                        if (item.Index == this._selectedIndex)
                        {
                            this.textBoxCurrentText.Text = p.Text;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The italic tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ItalicToolStripMenuItemClick(object sender, EventArgs e)
        {
            const string tag = "i";
            if (this._subtitle.Paragraphs.Count > 0 && this.subtitleListView1.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in this.subtitleListView1.SelectedItems)
                {
                    Paragraph p = this._subtitle.GetParagraphOrDefault(item.Index);
                    if (p != null)
                    {
                        if (p.Text.Contains("<" + tag + ">"))
                        {
                            p.Text = p.Text.Replace("<" + tag + ">", string.Empty);
                            p.Text = p.Text.Replace("</" + tag + ">", string.Empty);
                        }

                        p.Text = string.Format("<{0}>{1}</{0}>", tag, p.Text);
                        this.subtitleListView1.SetText(item.Index, p.Text);
                        if (item.Index == this._selectedIndex)
                        {
                            this.textBoxCurrentText.Text = p.Text;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The check box custom four colors checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxCustomFourColorsCheckedChanged(object sender, EventArgs e)
        {
            this.ResetTesseractThread();
            if (this.checkBoxCustomFourColors.Checked)
            {
                this.pictureBoxPattern.BackColor = Color.White;
                this.pictureBoxEmphasis1.BackColor = Color.Black;
                this.pictureBoxEmphasis2.BackColor = Color.Black;
                this.checkBoxBackgroundTransparent.Enabled = true;
                this.checkBoxPatternTransparent.Enabled = true;
                this.checkBoxEmphasis1Transparent.Enabled = true;
                this.checkBoxEmphasis2Transparent.Enabled = true;
            }
            else
            {
                this.pictureBoxPattern.BackColor = Color.Gray;
                this.pictureBoxEmphasis1.BackColor = Color.Gray;
                this.pictureBoxEmphasis2.BackColor = Color.Gray;
                this.checkBoxBackgroundTransparent.Enabled = false;
                this.checkBoxPatternTransparent.Enabled = false;
                this.checkBoxEmphasis1Transparent.Enabled = false;
                this.checkBoxEmphasis2Transparent.Enabled = false;
            }

            this.SubtitleListView1SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The reset tesseract thread.
        /// </summary>
        private void ResetTesseractThread()
        {
            if (this._tesseractThread != null)
            {
                this._tesseractThread.CancelAsync();
                for (int i = 0; i < this._tesseractAsyncStrings.Length; i++)
                {
                    this._tesseractAsyncStrings[i] = string.Empty;
                }

                this._tesseractAsyncIndex = 0;
            }
        }

        /// <summary>
        /// The picture box color chooser click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PictureBoxColorChooserClick(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                (sender as PictureBox).BackColor = this.colorDialog1.Color;
            }

            this.SubtitleListView1SelectedIndexChanged(null, null);
            this.ResetTesseractThread();
        }

        /// <summary>
        /// The check box pattern transparent checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxPatternTransparentCheckedChanged(object sender, EventArgs e)
        {
            this.SubtitleListView1SelectedIndexChanged(null, null);
            this.ResetTesseractThread();
        }

        /// <summary>
        /// The check box emphasis 1 transparent checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxEmphasis1TransparentCheckedChanged(object sender, EventArgs e)
        {
            this.SubtitleListView1SelectedIndexChanged(null, null);
            this.ResetTesseractThread();
        }

        /// <summary>
        /// The check box emphasis 2 transparent checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxEmphasis2TransparentCheckedChanged(object sender, EventArgs e)
        {
            this.SubtitleListView1SelectedIndexChanged(null, null);
            this.ResetTesseractThread();
        }

        /// <summary>
        /// The check box show only forced_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxShowOnlyForced_CheckedChanged(object sender, EventArgs e)
        {
            if (this._tesseractThread != null)
            {
                this._tesseractThread.CancelAsync();
                int i = 0;
                while (i < 10 && this._tesseractThread.IsBusy)
                {
                    System.Threading.Thread.Sleep(100);
                    i++;
                }

                this._tesseractAsyncStrings = null;
            }

            Subtitle oldSubtitle = new Subtitle(this._subtitle);
            this.subtitleListView1.BeginUpdate();
            if (this._bdnXmlOriginal != null)
            {
                this.LoadBdnXml();
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                this.LoadBluRaySup();
            }
            else
            {
                this.LoadVobRip();
            }

            for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
            {
                Paragraph current = this._subtitle.Paragraphs[i];
                foreach (Paragraph old in oldSubtitle.Paragraphs)
                {
                    if (current.StartTime.TotalMilliseconds == old.StartTime.TotalMilliseconds && current.Duration.TotalMilliseconds == old.Duration.TotalMilliseconds)
                    {
                        current.Text = old.Text;
                        break;
                    }
                }
            }

            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.EndUpdate();
        }

        /// <summary>
        /// The check box use time codes from idx_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxUseTimeCodesFromIdx_CheckedChanged(object sender, EventArgs e)
        {
            Subtitle oldSubtitle = new Subtitle(this._subtitle);
            this.subtitleListView1.BeginUpdate();
            this.LoadVobRip();
            for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = oldSubtitle.GetParagraphOrDefault(i);
                if (p != null && p.Text.Length > 0)
                {
                    this._subtitle.Paragraphs[i].Text = p.Text;
                }
            }

            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.EndUpdate();
        }

        /// <summary>
        /// The set spell check language.
        /// </summary>
        /// <param name="languageString">
        /// The language string.
        /// </param>
        private void SetSpellCheckLanguage(string languageString)
        {
            if (string.IsNullOrEmpty(languageString))
            {
                return;
            }

            int i = 0;
            foreach (string item in this.comboBoxDictionaries.Items)
            {
                if (item.Contains("[" + languageString + "]"))
                {
                    this.comboBoxDictionaries.SelectedIndex = i;
                }

                i++;
            }
        }

        /// <summary>
        /// The combo box dictionaries_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxDictionaries_SelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration.Settings.General.SpellCheckLanguage = this.LanguageString;
            string threeLetterISOLanguageName = string.Empty;
            if (this.LanguageString == null)
            {
                if (this._ocrFixEngine != null)
                {
                    this._ocrFixEngine.Dispose();
                }

                this._ocrFixEngine = new OcrFixEngine(string.Empty, string.Empty, this);
                return;
            }

            try
            {
                if (this._ocrFixEngine != null)
                {
                    this._ocrFixEngine.Dispose();
                }

                this._ocrFixEngine = null;
                var ci = new CultureInfo(this.LanguageString.Replace("_", "-"));
                threeLetterISOLanguageName = ci.ThreeLetterISOLanguageName;
            }
            catch
            {
            }

            this.LoadOcrFixEngine(threeLetterISOLanguageName, this.LanguageString);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="bdnSubtitle">
        /// The bdn subtitle.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="isSon">
        /// The is son.
        /// </param>
        internal void Initialize(Subtitle bdnSubtitle, VobSubOcrSettings vobSubOcrSettings, bool isSon)
        {
            this._bdnXmlOriginal = bdnSubtitle;
            this._bdnFileName = bdnSubtitle.FileName;
            this._isSon = isSon;
            if (this._isSon)
            {
                this.checkBoxCustomFourColors.Checked = true;
                this.pictureBoxBackground.BackColor = Color.Transparent;
                this.pictureBoxPattern.BackColor = Color.DarkGray;
                this.pictureBoxEmphasis1.BackColor = Color.Black;
                this.pictureBoxEmphasis2.BackColor = Color.White;
            }

            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = 11;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this.SetOcrMethod();

            this.groupBoxImagePalette.Visible = isSon;

            this.Text = Configuration.Settings.Language.VobSubOcr.TitleBluRay;
            this.Text += " - " + Path.GetFileName(this._bdnFileName);

            this.checkBoxAutoTransparentBackground.Checked = true;
        }

        /// <summary>
        /// The set ocr method.
        /// </summary>
        private void SetOcrMethod()
        {
            if (Configuration.Settings.VobSubOcr.LastOcrMethod == "BitmapCompare" && this.comboBoxOcrMethod.Items.Count > 1)
            {
                this.comboBoxOcrMethod.SelectedIndex = 1;
            }
            else if (Configuration.Settings.VobSubOcr.LastOcrMethod == "MODI" && this.comboBoxOcrMethod.Items.Count > 2)
            {
                this.comboBoxOcrMethod.SelectedIndex = 2;
            }
            else if (Configuration.Settings.VobSubOcr.LastOcrMethod == "nOCR" && this.comboBoxOcrMethod.Items.Count > 3)
            {
                this.comboBoxOcrMethod.SelectedIndex = 3;
            }
            else
            {
                this.comboBoxOcrMethod.SelectedIndex = 0;
            }

            if (this.comboBoxOcrMethod.Items.Count > 4)
            {
                this.comboBoxOcrMethod.SelectedIndex = 4;
            }
        }

        /// <summary>
        /// The start ocr from delayed.
        /// </summary>
        internal void StartOcrFromDelayed()
        {
            if (this._lastAdditions.Count > 0)
            {
                var last = this._lastAdditions[this._lastAdditions.Count - 1];
                this.numericUpDownStartNumber.Value = last.Index + 1;

                // Simulate a click on ButtonStartOcr in 200ms.
                var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                Utilities.TaskDelay(200).ContinueWith(_ => this.ButtonStartOcrClick(null, null), uiContext);
            }
        }

        /// <summary>
        /// The vob sub ocr_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcr_Resize(object sender, EventArgs e)
        {
            const int originalTopHeight = 105;

            int adjustPercent = (int)(this.Height * 0.15);
            this.groupBoxSubtitleImage.Height = originalTopHeight + adjustPercent;
            this.groupBoxOcrMethod.Height = this.groupBoxSubtitleImage.Height;

            this.splitContainerBottom.Top = this.groupBoxSubtitleImage.Top + this.groupBoxSubtitleImage.Height + 5;
            this.splitContainerBottom.Height = this.progressBar1.Top - (this.splitContainerBottom.Top + 20);
            this.checkBoxUseTimeCodesFromIdx.Left = this.groupBoxOCRControls.Left + 1;

            this.listBoxUnknownWords.Top = this.listBoxLog.Top;
            this.listBoxUnknownWords.Left = this.listBoxLog.Left;
        }

        /// <summary>
        /// The import text with matching time codes tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void importTextWithMatchingTimeCodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenSubtitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetOpenDialogFilter();
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = this.openFileDialog1.FileName;
                if (!File.Exists(fileName))
                {
                    return;
                }

                var fi = new FileInfo(fileName);
                if (fi.Length > 1024 * 1024 * 10)
                {
                    // max 10 mb
                    if (MessageBox.Show(string.Format(Configuration.Settings.Language.Main.FileXIsLargerThan10MB + Environment.NewLine + Environment.NewLine + Configuration.Settings.Language.Main.ContinueAnyway, fileName), this.Text, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                Subtitle sub = new Subtitle();
                Encoding encoding;
                SubtitleFormat format = sub.LoadSubtitle(fileName, out encoding, null);
                if (format == null)
                {
                    return;
                }

                int index = 0;
                foreach (Paragraph p in sub.Paragraphs)
                {
                    foreach (Paragraph currentP in this._subtitle.Paragraphs)
                    {
                        if (string.IsNullOrEmpty(currentP.Text) && Math.Abs(p.StartTime.TotalMilliseconds - currentP.StartTime.TotalMilliseconds) <= 40)
                        {
                            currentP.Text = p.Text;
                            this.subtitleListView1.SetText(index, p.Text);
                            break;
                        }
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// The save all images with html index view tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void saveAllImagesWithHtmlIndexViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.progressBar1.Maximum = this._subtitle.Paragraphs.Count - 1;
                this.progressBar1.Value = 0;
                this.progressBar1.Visible = true;
                int imagesSavedCount = 0;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<html>");
                sb.AppendLine("<head><title>Subtitle images</title></head>");
                sb.AppendLine("<body>");
                for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
                {
                    this.progressBar1.Value = i;
                    Bitmap bmp = this.GetSubtitleBitmap(i);
                    string numberString = string.Format("{0:0000}", i + 1);
                    if (bmp != null)
                    {
                        string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + ".png");
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        imagesSavedCount++;
                        Paragraph p = this._subtitle.Paragraphs[i];
                        string text = string.Empty;
                        if (!string.IsNullOrEmpty(p.Text))
                        {
                            string backgroundColor = ColorTranslator.ToHtml(this.subtitleListView1.GetBackgroundColor(i));
                            text = "<br /><div style='font-size:22px; background-color:" + backgroundColor + "'>" + WebUtility.HtmlEncode(p.Text.Replace("<i>", "@1__").Replace("</i>", "@2__")).Replace("@1__", "<i>").Replace("@2__", "</i>").Replace(Environment.NewLine, "<br />") + "</div>";
                        }

                        sb.AppendLine(string.Format("#{3}:{0}->{1}<div style='text-align:center'><img src='{2}.png' />" + text + "</div><br /><hr />", p.StartTime.ToShortString(), p.EndTime.ToShortString(), numberString, i + 1));
                        bmp.Dispose();
                    }
                }

                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                string htmlFileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, "index.html");
                File.WriteAllText(htmlFileName, sb.ToString());
                this.progressBar1.Visible = false;
                MessageBox.Show(string.Format("{0} images saved in {1}", imagesSavedCount, this.folderBrowserDialog1.SelectedPath));
                Process.Start(htmlFileName);
            }
        }

        /// <summary>
        /// The inspect image compare matches for current image tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void inspectImageCompareMatchesForCurrentImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count != 1)
            {
                return;
            }

            if (this._compareBitmaps == null)
            {
                this.LoadImageCompareBitmaps();
            }

            this.Cursor = Cursors.WaitCursor;
            Bitmap bitmap = this.GetSubtitleBitmap(this.subtitleListView1.SelectedItems[0].Index);
            NikseBitmap parentBitmap = new NikseBitmap(bitmap);
            var matches = new List<CompareMatch>();
            List<ImageSplitterItem> list;

            if (this._binaryOcrDb == null)
            {
                list = NikseBitmapImageSplitter.SplitBitmapToLetters(parentBitmap, (int)this.numericUpDownPixelsIsSpace.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom);
            }
            else
            {
                int minLineHeight = this._binOcrLastLowercaseHeight - 3;
                if (minLineHeight < 5)
                {
                    minLineHeight = 5;
                }

                list = NikseBitmapImageSplitter.SplitBitmapToLettersNew(parentBitmap, (int)this.numericUpDownPixelsIsSpace.Value, this.checkBoxRightToLeft.Checked, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight);
            }

            int index = 0;
            var imageSources = new List<Bitmap>();
            while (index < list.Count)
            {
                ImageSplitterItem item = list[index];
                if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                    imageSources.Add(null);
                }
                else
                {
                    CompareMatch bestGuess;
                    CompareMatch match;
                    if (this._binaryOcrDb != null)
                    {
                        match = this.GetCompareMatchNew(item, out bestGuess, list, index);
                    }
                    else
                    {
                        match = this.GetCompareMatch(item, parentBitmap, out bestGuess, list, index);
                    }

                    if (match == null)
                    {
                        matches.Add(new CompareMatch(Configuration.Settings.Language.VobSubOcr.NoMatch, false, 0, null));
                        imageSources.Add(item.NikseBitmap.GetBitmap());
                    }
                    else
                    {
                        // found image match
                        if (match.ExpandCount > 0)
                        {
                            List<ImageSplitterItem> expandSelectionList = new List<ImageSplitterItem>();
                            for (int i = 0; i < match.ExpandCount; i++)
                            {
                                expandSelectionList.Add(list[index + i]);
                            }

                            item = GetExpandedSelectionNew(parentBitmap, expandSelectionList);
                            matches.Add(new CompareMatch(match.Text, match.Italic, 0, match.Name, item));
                            imageSources.Add(item.NikseBitmap.GetBitmap());
                        }
                        else
                        {
                            matches.Add(new CompareMatch(match.Text, match.Italic, 0, match.Name, item));
                            imageSources.Add(item.NikseBitmap.GetBitmap());
                        }

                        if (match.ExpandCount > 0)
                        {
                            index += match.ExpandCount - 1;
                        }
                    }
                }

                index++;
            }

            this.Cursor = Cursors.Default;
            using (var inspect = new VobSubOcrCharacterInspect())
            {
                inspect.Initialize(this.comboBoxCharacterDatabase.SelectedItem.ToString(), matches, imageSources, this._binaryOcrDb);
                if (inspect.ShowDialog(this) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    if (this._binaryOcrDb != null)
                    {
                        this._binaryOcrDb.Save();
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    this._compareDoc = inspect.ImageCompareDocument;
                    string path = Configuration.VobSubCompareFolder + this.comboBoxCharacterDatabase.SelectedItem + Path.DirectorySeparatorChar;
                    this._compareDoc.Save(path + "Images.xml");
                    this.LoadImageCompareBitmaps();
                    this.Cursor = Cursors.Default;
                }
            }

            if (this._binaryOcrDb != null)
            {
                this._binaryOcrDb.LoadCompareImages();
            }

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The inspect last additions tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void inspectLastAdditionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var formVobSubEditCharacters = new VobSubEditCharacters(this.comboBoxCharacterDatabase.SelectedItem.ToString(), this._lastAdditions, this._binaryOcrDb))
            {
                if (formVobSubEditCharacters.ShowDialog(this) == DialogResult.OK)
                {
                    this._lastAdditions = formVobSubEditCharacters.Additions;
                    if (this._binaryOcrDb != null)
                    {
                        this._binaryOcrDb.Save();
                    }
                    else
                    {
                        this._compareDoc = formVobSubEditCharacters.ImageCompareDocument;
                        string path = Configuration.VobSubCompareFolder + this.comboBoxCharacterDatabase.SelectedItem + Path.DirectorySeparatorChar;
                        this._compareDoc.Save(path + "Images.xml");
                    }
                }
            }
        }

        /// <summary>
        /// The check box auto transparent background_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxAutoTransparentBackground_CheckedChanged(object sender, EventArgs e)
        {
            this.ResetTesseractThread();
            this.SubtitleListView1SelectedIndexChanged(null, null);
            if (this.checkBoxAutoTransparentBackground.Checked && this._dvbSubtitles != null)
            {
                this.numericUpDownAutoTransparentAlphaMax.Visible = true;
            }
            else
            {
                this.numericUpDownAutoTransparentAlphaMax.Visible = false;
            }

            this.labelMinAlpha.Visible = this.numericUpDownAutoTransparentAlphaMax.Visible;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="palette">
        /// The palette.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="spList">
        /// The sp list.
        /// </param>
        internal void Initialize(string fileName, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, List<SpHeader> spList)
        {
            this._spList = spList;
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this._palette = palette;

            if (this._palette == null)
            {
                this.checkBoxCustomFourColors.Checked = true;
            }

            this.SetOcrMethod();

            this.FileName = fileName;
            this.Text += " - " + Path.GetFileName(this.FileName);

            foreach (SpHeader header in this._spList)
            {
                Paragraph p = new Paragraph(string.Empty, header.StartTime.TotalMilliseconds, header.StartTime.TotalMilliseconds + header.Picture.Delay.TotalMilliseconds);
                this._subtitle.Paragraphs.Add(p);
            }

            this._subtitle.Renumber();
            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The text box current text_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxCurrentText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == this._italicShortcut)
            {
                // Ctrl+I (or custom) = Italic
                TextBox tb = this.textBoxCurrentText;
                string text = tb.SelectedText;
                int selectionStart = tb.SelectionStart;
                if (text.Contains("<i>"))
                {
                    text = HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagItalic);
                }
                else
                {
                    text = string.Format("<i>{0}</i>", text);
                }

                tb.SelectedText = text;
                tb.SelectionStart = selectionStart;
                tb.SelectionLength = text.Length;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subPicturesWithTimeCodes">
        /// The sub pictures with time codes.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(List<SubPicturesWithSeparateTimeCodes> subPicturesWithTimeCodes, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            this._mp4List = subPicturesWithTimeCodes;

            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            if (this._palette == null)
            {
                this.checkBoxCustomFourColors.Checked = true;
            }

            this.SetOcrMethod();

            this.FileName = fileName;
            this.Text += " - " + Path.GetFileName(this.FileName);

            foreach (SubPicturesWithSeparateTimeCodes subItem in this._mp4List)
            {
                var p = new Paragraph(string.Empty, subItem.Start.TotalMilliseconds, subItem.End.TotalMilliseconds);
                this._subtitle.Paragraphs.Add(p);
            }

            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subPictures">
        /// The sub pictures.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(List<XSub> subPictures, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            this._xSubList = subPictures;

            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this.checkBoxCustomFourColors.Enabled = true;
            this.checkBoxCustomFourColors.Checked = true;
            this.checkBoxAutoTransparentBackground.Enabled = true;
            this.checkBoxAutoTransparentBackground.Enabled = false;

            this.SetOcrMethod();

            this.FileName = fileName;
            this.Text += " - " + Path.GetFileName(this.FileName);

            foreach (XSub subItem in this._xSubList)
            {
                var p = new Paragraph(string.Empty, subItem.Start.TotalMilliseconds, subItem.End.TotalMilliseconds);
                this._subtitle.Paragraphs.Add(p);
            }

            this._subtitle.Renumber();
            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The has changes been made.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasChangesBeenMade()
        {
            var secondsSinceOcrWindowOpened = DateTime.Now.Subtract(this._windowStartTime).TotalSeconds;
            if (this._subtitle != null && this._subtitle.Paragraphs.Count > 10 && secondsSinceOcrWindowOpened > 10)
            {
                int numberOfLinesWithText = 0;
                foreach (var p in this._subtitle.Paragraphs)
                {
                    if (p != null && !string.IsNullOrWhiteSpace(p.Text))
                    {
                        numberOfLinesWithText++;
                    }
                }

                // ocr'ed more than 10 lines - or perhaps manually translated more than 10 lines in at least 30 seconds
                if (this._linesOcred > 10 || (numberOfLinesWithText > 10 && secondsSinceOcrWindowOpened > 30))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The vob sub ocr_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcr_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.OkClicked && this.HasChangesBeenMade())
            {
                if (MessageBox.Show(Configuration.Settings.Language.VobSubOcr.DiscardText, Configuration.Settings.Language.VobSubOcr.DiscardTitle, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            this._icThreadsStop = true;
            this._abort = true;
            this._nocrThreadsStop = true;
            if (this._mainOcrTimer != null)
            {
                this._mainOcrTimer.Stop();
            }

            if (this._tesseractThread != null)
            {
                this._tesseractThread.CancelAsync();
            }

            this._tesseractAsyncIndex = 10000;

            System.Threading.Thread.Sleep(100);
            this.DisposeImageCompareBitmaps();

            Configuration.Settings.VobSubOcr.UseItalicsInTesseract = this.checkBoxTesseractItalicsOn.Checked;
            Configuration.Settings.VobSubOcr.ItalicFactor = this._unItalicFactor;
            Configuration.Settings.VobSubOcr.UseModiInTesseractForUnknownWords = this.checkBoxUseModiInTesseractForUnknownWords.Checked;
            Configuration.Settings.VobSubOcr.PromptForUnknownWords = this.checkBoxPromptForUnknownWords.Checked;
            Configuration.Settings.VobSubOcr.GuessUnknownWords = this.checkBoxGuessUnknownWords.Checked;
            Configuration.Settings.VobSubOcr.AutoBreakSubtitleIfMoreThanTwoLines = this.checkBoxAutoBreakLines.Checked;
            Configuration.Settings.VobSubOcr.LineOcrDraw = this.checkBoxNOcrCorrect.Checked;
            Configuration.Settings.VobSubOcr.LineOcrAdvancedItalic = this.checkBoxNOcrItalic.Checked;

            if (this._bluRaySubtitlesOriginal != null)
            {
                Configuration.Settings.VobSubOcr.BlurayAllowDifferenceInPercent = (double)this.numericUpDownMaxErrorPct.Value;
            }
            else
            {
                Configuration.Settings.VobSubOcr.AllowDifferenceInPercent = (double)this.numericUpDownMaxErrorPct.Value;
            }

            if (this.comboBoxOcrMethod.SelectedIndex == 3)
            {
                // line OCR
                Configuration.Settings.VobSubOcr.LineOcrLastSpellCheck = this.LanguageString;
                if (this.comboBoxNOcrLanguage.Items.Count > 0 && this.comboBoxNOcrLanguage.SelectedIndex >= 0)
                {
                    Configuration.Settings.VobSubOcr.LineOcrLastLanguages = this.comboBoxNOcrLanguage.Items[this.comboBoxNOcrLanguage.SelectedIndex].ToString();
                }
            }
        }

        /// <summary>
        /// The subtitle list view 1_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                this.subtitleListView1.SelectedIndexChanged -= this.SubtitleListView1SelectedIndexChanged;
                this.subtitleListView1.BeginUpdate();
                for (int i = 0; i < this.subtitleListView1.Items.Count; i++)
                {
                    this.subtitleListView1.Items[i].Selected = true;
                }

                this.subtitleListView1.EndUpdate();
                this.subtitleListView1.SelectedIndexChanged += this.SubtitleListView1SelectedIndexChanged;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
            {
                this.subtitleListView1.SelectedIndexChanged -= this.SubtitleListView1SelectedIndexChanged;
                this.subtitleListView1.BeginUpdate();
                for (int i = 0; i < this.subtitleListView1.Items.Count; i++)
                {
                    this.subtitleListView1.Items[i].Selected = false;
                }

                this.subtitleListView1.EndUpdate();
                this.subtitleListView1.SelectedIndexChanged += this.SubtitleListView1SelectedIndexChanged;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Delete)
            {
                this.DeleteToolStripMenuItemClick(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
                this.subtitleListView1.Focus();
            }
        }

        /// <summary>
        /// The delete tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count == 0)
            {
                return;
            }

            string askText;
            if (this.subtitleListView1.SelectedItems.Count > 1)
            {
                askText = string.Format(Configuration.Settings.Language.Main.DeleteXLinesPrompt, this.subtitleListView1.SelectedItems.Count);
            }
            else
            {
                askText = Configuration.Settings.Language.Main.DeleteOneLinePrompt;
            }

            if (Configuration.Settings.General.PromptDeleteLines && MessageBox.Show(askText, string.Empty, MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            this.ResetTesseractThread();

            int selIdx = this.subtitleListView1.SelectedItems[0].Index;
            List<int> indices = new List<int>();
            foreach (int idx in this.subtitleListView1.SelectedIndices)
            {
                indices.Add(idx);
            }

            indices.Reverse();

            if (this._mp4List != null)
            {
                foreach (int idx in indices)
                {
                    this._mp4List.RemoveAt(idx);
                }
            }
            else if (this._spList != null)
            {
                foreach (int idx in indices)
                {
                    this._spList.RemoveAt(idx);
                }
            }
            else if (this._dvbSubtitles != null)
            {
                foreach (int idx in indices)
                {
                    this._dvbSubtitles.RemoveAt(idx);
                }
            }
            else if (this._bdnXmlSubtitle != null)
            {
                foreach (int idx in indices)
                {
                    this._bdnXmlSubtitle.Paragraphs.RemoveAt(idx);
                }
            }
            else if (this._xSubList != null)
            {
                foreach (int idx in indices)
                {
                    this._xSubList.RemoveAt(idx);
                }
            }
            else if (this._bluRaySubtitlesOriginal != null)
            {
                foreach (int idx in indices)
                {
                    var x1 = this._bluRaySubtitles[idx];
                    int i = 0;
                    while (i < this._bluRaySubtitlesOriginal.Count)
                    {
                        var x2 = this._bluRaySubtitlesOriginal[i];
                        if (x2.StartTime == x1.StartTime)
                        {
                            this._bluRaySubtitlesOriginal.Remove(x2);
                            break;
                        }

                        i++;
                    }

                    this._bluRaySubtitles.RemoveAt(idx);
                }
            }
            else
            {
                foreach (int idx in indices)
                {
                    var x1 = this._vobSubMergedPackist[idx];
                    int i = 0;
                    while (i < this._vobSubMergedPackistOriginal.Count)
                    {
                        var x2 = this._vobSubMergedPackistOriginal[i];
                        if (x2.StartTime.TotalMilliseconds == x1.StartTime.TotalMilliseconds)
                        {
                            this._vobSubMergedPackistOriginal.Remove(x2);
                            break;
                        }

                        i++;
                    }

                    this._vobSubMergedPackist.RemoveAt(idx);
                }
            }

            foreach (int idx in indices)
            {
                this._subtitle.Paragraphs.RemoveAt(idx);
            }

            this.subtitleListView1.Fill(this._subtitle);

            if (selIdx < this.subtitleListView1.Items.Count)
            {
                this.subtitleListView1.SelectIndexAndEnsureVisible(selIdx, true);
            }
            else
            {
                this.subtitleListView1.SelectIndexAndEnsureVisible(this.subtitleListView1.Items.Count - 1, true);
            }
        }

        /// <summary>
        /// The button uknown to names_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonUknownToNames_Click(object sender, EventArgs e)
        {
            if (this.listBoxUnknownWords.Items.Count > 0 && this.listBoxUnknownWords.SelectedItems.Count > 0)
            {
                string text = this.listBoxUnknownWords.SelectedItems[0].ToString();
                if (text.Contains(':'))
                {
                    if (this._ocrFixEngine == null)
                    {
                        this.comboBoxDictionaries_SelectedIndexChanged(null, null);
                    }

                    text = text.Substring(text.IndexOf(':') + 1).Trim();
                    using (var form = new AddToNamesList())
                    {
                        form.Initialize(this._subtitle, this.comboBoxDictionaries.Text, text);
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            this.comboBoxDictionaries_SelectedIndexChanged(null, null);
                            this.ShowStatus(string.Format(Configuration.Settings.Language.Main.NameXAddedToNamesEtcList, form.NewName));
                        }
                        else if (!string.IsNullOrEmpty(form.NewName))
                        {
                            MessageBox.Show(string.Format(Configuration.Settings.Language.Main.NameXNotAddedToNamesEtcList, form.NewName));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The button uknown to user dic_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonUknownToUserDic_Click(object sender, EventArgs e)
        {
            if (this.listBoxUnknownWords.Items.Count > 0 && this.listBoxUnknownWords.SelectedItems.Count > 0)
            {
                string text = this.listBoxUnknownWords.SelectedItems[0].ToString();
                if (text.Contains(':'))
                {
                    text = text.Substring(text.IndexOf(':') + 1).Trim().ToLower();
                    using (var form = new AddToUserDic())
                    {
                        form.Initialize(this.comboBoxDictionaries.Text, text);
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            this.comboBoxDictionaries_SelectedIndexChanged(null, null);
                            this.ShowStatus(string.Format(Configuration.Settings.Language.Main.WordXAddedToUserDic, form.NewWord));
                        }
                        else if (!string.IsNullOrEmpty(form.NewWord))
                        {
                            MessageBox.Show(string.Format(Configuration.Settings.Language.Main.WordXNotAddedToUserDic, form.NewWord));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The button add to ocr replace list_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddToOcrReplaceList_Click(object sender, EventArgs e)
        {
            if (this.listBoxUnknownWords.Items.Count > 0 && this.listBoxUnknownWords.SelectedItems.Count > 0)
            {
                string text = this.listBoxUnknownWords.SelectedItems[0].ToString();
                if (text.Contains(':'))
                {
                    text = text.Substring(text.IndexOf(':') + 1).Trim().ToLower();
                    using (var form = new AddToOcrReplaceList())
                    {
                        form.Initialize(this._languageId, this.comboBoxDictionaries.Text, text);
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            this.comboBoxDictionaries_SelectedIndexChanged(null, null);
                            this.ShowStatus(string.Format(Configuration.Settings.Language.Main.OcrReplacePairXAdded, form.NewSource, form.NewTarget));
                        }
                        else
                        {
                            MessageBox.Show(string.Format(Configuration.Settings.Language.Main.OcrReplacePairXNotAdded, form.NewSource, form.NewTarget));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The button google it_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGoogleIt_Click(object sender, EventArgs e)
        {
            if (this.listBoxUnknownWords.Items.Count > 0 && this.listBoxUnknownWords.SelectedItems.Count > 0)
            {
                string text = this.listBoxUnknownWords.SelectedItems[0].ToString();
                if (text.Contains(':'))
                {
                    text = text.Substring(text.IndexOf(':') + 1).Trim();
                    Process.Start("https://www.google.com/search?q=" + Utilities.UrlEncode(text));
                }
            }
        }

        /// <summary>
        /// The list box copy to clipboard_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxCopyToClipboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                ListBox lb = sender as ListBox;
                if (lb != null && lb.Items.Count > 0 && lb.SelectedItems.Count > 0)
                {
                    try
                    {
                        string text = lb.SelectedItems[0].ToString();
                        Clipboard.SetText(text);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// The tool strip menu item set un italic factor_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void toolStripMenuItemSetUnItalicFactor_Click(object sender, EventArgs e)
        {
            using (var form = new VobSubOcrSetItalicFactor(this.GetSubtitleBitmap(this._selectedIndex), this._unItalicFactor))
            {
                form.ShowDialog(this);
                this._unItalicFactor = form.GetUnItalicFactor();
            }
        }

        /// <summary>
        /// The vob sub tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void vobSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var exportBdnXmlPng = new ExportPngXml())
            {
                exportBdnXmlPng.InitializeFromVobSubOcr(this._subtitle, new SubRip(), "VOBSUB", this.FileName, this, this._importLanguageString);
                exportBdnXmlPng.ShowDialog(this);
            }
        }

        /// <summary>
        /// The bluray sup tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void bluraySupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var exportBdnXmlPng = new ExportPngXml())
            {
                exportBdnXmlPng.InitializeFromVobSubOcr(this._subtitle, new SubRip(), "BLURAYSUP", this.FileName, this, this._importLanguageString);
                exportBdnXmlPng.ShowDialog(this);
            }
        }

        /// <summary>
        /// The b dnxml tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void bDNXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var exportBdnXmlPng = new ExportPngXml())
            {
                exportBdnXmlPng.InitializeFromVobSubOcr(this._subtitle, new SubRip(), "BDNXML", this.FileName, this, this._importLanguageString);
                exportBdnXmlPng.ShowDialog(this);
            }
        }

        /// <summary>
        /// The clear tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listBoxUnknownWords.Items.Clear();
        }

        /// <summary>
        /// The tool strip menu item clear fixes_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void toolStripMenuItemClearFixes_Click(object sender, EventArgs e)
        {
            this.listBoxLog.Items.Clear();
        }

        /// <summary>
        /// The tool strip menu item clear guesses_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void toolStripMenuItemClearGuesses_Click(object sender, EventArgs e)
        {
            this.listBoxLogSuggestions.Items.Clear();
        }

        /// <summary>
        /// The button get tesseract dictionaries_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGetTesseractDictionaries_Click(object sender, EventArgs e)
        {
            using (var form = new GetTesseractDictionaries())
            {
                form.ShowDialog(this);
            }

            this.InitializeTesseract();
        }

        /// <summary>
        /// The tool strip menu item inspect n ocr matches_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void toolStripMenuItemInspectNOcrMatches_Click(object sender, EventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count != 1)
            {
                return;
            }

            if (this._nOcrDb == null)
            {
                this.LoadNOcrWithCurrentLanguage();
                if (this._nOcrDb == null)
                {
                    MessageBox.Show("Unable to load OCR database.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            this.Cursor = Cursors.WaitCursor;
            Bitmap bitmap = this.GetSubtitleBitmap(this.subtitleListView1.SelectedItems[0].Index);
            bool oldPrompt = this.checkBoxPromptForUnknownWords.Checked;
            string result = this.OcrViaNOCR(bitmap, this.subtitleListView1.SelectedItems[0].Index);
            this.checkBoxPromptForUnknownWords.Checked = oldPrompt;
            this.Cursor = Cursors.Default;
            using (var inspect = new VobSubNOcrCharacterInspect())
            {
                bool oldCorrect = this.checkBoxNOcrCorrect.Checked;
                this.checkBoxNOcrCorrect.Checked = false;
                inspect.Initialize(bitmap, (int)this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value, this.checkBoxRightToLeft.Checked, this._nOcrDb, this);
                if (inspect.ShowDialog(this) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    this.SaveNOcrWithCurrentLanguage();
                    this.Cursor = Cursors.Default;
                }

                this.checkBoxNOcrCorrect.Checked = oldCorrect;
            }
        }

        /// <summary>
        /// The button line ocr edit language_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonLineOcrEditLanguage_Click(object sender, EventArgs e)
        {
            if (this._nOcrDb == null)
            {
                if (string.IsNullOrEmpty(this.GetNOcrLanguageFileName()))
                {
                    MessageBox.Show("No line OCR language loaded - please re-install");
                    return;
                }

                this.LoadNOcrWithCurrentLanguage();
            }

            using (var form = new VobSubNOcrEdit(this._nOcrDb.OcrCharacters, null))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    this.SaveNOcrWithCurrentLanguage();
                }
                else
                {
                    this.LoadNOcrWithCurrentLanguage();
                }
            }
        }

        /// <summary>
        /// The button line ocr new language_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonLineOcrNewLanguage_Click(object sender, EventArgs e)
        {
            using (var newFolder = new VobSubOcrNewFolder(this.comboBoxOcrMethod.SelectedIndex == 1))
            {
                if (newFolder.ShowDialog(this) == DialogResult.OK)
                {
                    string s = newFolder.FolderName;
                    if (string.IsNullOrEmpty(s))
                    {
                        return;
                    }

                    s = s.Replace("?", string.Empty).Replace("/", string.Empty).Replace("*", string.Empty).Replace("\\", string.Empty);
                    if (string.IsNullOrEmpty(s))
                    {
                        return;
                    }

                    if (File.Exists(Configuration.DictionariesFolder + "nOCR_" + newFolder.FolderName + ".xml"))
                    {
                        MessageBox.Show("Line OCR language file already exists!");
                        return;
                    }

                    this._nOcrDb = null;
                    this.comboBoxNOcrLanguage.Items.Add(s);
                    this.comboBoxNOcrLanguage.SelectedIndex = this.comboBoxNOcrLanguage.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// The combo box n ocr language_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxNOcrLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._nOcrDb = null;
        }

        /// <summary>
        /// The button spell check download_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSpellCheckDownload_Click(object sender, EventArgs e)
        {
            using (var form = new GetDictionaries())
            {
                form.ShowDialog(this);
            }

            this.FillSpellCheckDictionaries();
            if (this.comboBoxDictionaries.Items.Count > 0)
            {
                this.comboBoxDictionaries.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The show status.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        private void ShowStatus(string msg)
        {
            this.labelStatus.Text = msg;
            this.timerHideStatus.Start();
        }

        /// <summary>
        /// The timer hide status_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timerHideStatus_Tick(object sender, EventArgs e)
        {
            this.timerHideStatus.Stop();
            this.labelStatus.Text = string.Empty;
        }

        /// <summary>
        /// The d ost tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void dOSTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var exportBdnXmlPng = new ExportPngXml())
            {
                exportBdnXmlPng.InitializeFromVobSubOcr(this._subtitle, new SubRip(), "DOST", this.FileName, this, this._importLanguageString);
                exportBdnXmlPng.ShowDialog(this);
            }
        }

        // TODO: Get language from ts file
        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitles">
        /// The subtitles.
        /// </param>
        /// <param name="vobSubOcrSettings">
        /// The vob sub ocr settings.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(List<TransportStreamSubtitle> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.buttonStartOcr.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonNewCharacterDatabase.Enabled = false;
            this.buttonEditCharacterDatabase.Enabled = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.numericUpDownPixelsIsSpace.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this.numericUpDownNumberOfPixelsIsSpaceNOCR.Value = vobSubOcrSettings.XOrMorePixelsMakesSpace;
            this._vobSubOcrSettings = vobSubOcrSettings;

            this.InitializeModi();
            this.InitializeTesseract();
            this.LoadImageCompareCharacterDatabaseList();

            this.SetOcrMethod();

            this._dvbSubtitles = subtitles;

            this.ShowDvbSubs();

            this.FileName = fileName;
            this.Text += " - " + Path.GetFileName(fileName);

            this.groupBoxImagePalette.Visible = false;
            this.groupBoxTransportStream.Left = this.groupBoxImagePalette.Left;
            this.groupBoxTransportStream.Top = this.groupBoxImagePalette.Top;
            this.groupBoxTransportStream.Visible = true;
            this.checkBoxTransportStreamGetColorAndSplit.Visible = subtitles.Count > 0 && subtitles[0].IsDvbSub;

            // SetTesseractLanguageFromLanguageString(languageString);
            // _importLanguageString = languageString;
        }

        /// <summary>
        /// The check box transport stream grayscale_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxTransportStreamGrayscale_CheckedChanged(object sender, EventArgs e)
        {
            this.SubtitleListView1SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The check box transport stream get color and split_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxTransportStreamGetColorAndSplit_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxTransportStreamGetColorAndSplit.Checked)
            {
                this.SplitDvbForEachSubImage();
            }
            else
            {
                this.MergeDvbForEachSubImage();
            }

            this.SubtitleListView1SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The merge dvb for each sub image.
        /// </summary>
        private void MergeDvbForEachSubImage()
        {
            int i = 0;
            while (i < this._dvbSubtitles.Count)
            {
                var dvbSub = this._dvbSubtitles[i];
                dvbSub.ActiveImageIndex = null;
                if (i < this._dvbSubtitles.Count - 1 && dvbSub.Pes == this._dvbSubtitles[i + 1].Pes)
                {
                    var next = this._subtitle.GetParagraphOrDefault(i + 1);
                    if (!string.IsNullOrEmpty(next.Text))
                    {
                        var p = this._subtitle.Paragraphs[i];
                        p.Text = (p.Text + Environment.NewLine + next.Text).Trim();
                    }

                    this._subtitle.Paragraphs.RemoveAt(i + 1);
                    this._dvbSubtitles.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }

            this._tesseractAsyncStrings = null;
            this._subtitle.Renumber();
            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The split dvb for each sub image.
        /// </summary>
        private void SplitDvbForEachSubImage()
        {
            var list = new List<TransportStreamSubtitle>();
            foreach (var dvbSub in this._dvbSubtitles)
            {
                if (dvbSub.ActiveImageIndex == null)
                {
                    for (int i = 0; i < dvbSub.Pes.ObjectDataList.Count; i++)
                    {
                        var newDbvSub = new Logic.TransportStream.TransportStreamSubtitle();
                        newDbvSub.Pes = dvbSub.Pes;
                        newDbvSub.ActiveImageIndex = i;
                        newDbvSub.StartMilliseconds = dvbSub.StartMilliseconds;
                        newDbvSub.EndMilliseconds = dvbSub.EndMilliseconds;
                        if (newDbvSub.Pes.ObjectDataList[i].TopFieldDataBlockLength > 8)
                        {
                            list.Add(newDbvSub);
                        }
                    }
                }
                else
                {
                    list.Add(dvbSub);
                }
            }

            this._dvbSubtitles = list;
            this._tesseractAsyncStrings = null;
            this.ShowDvbSubs();
        }

        /// <summary>
        /// The show dvb subs.
        /// </summary>
        private void ShowDvbSubs()
        {
            this._subtitle.Paragraphs.Clear();
            foreach (var sub in this._dvbSubtitles)
            {
                this._subtitle.Paragraphs.Add(new Paragraph(string.Empty, sub.StartMilliseconds, sub.EndMilliseconds));
            }

            this._subtitle.Renumber();
            this.subtitleListView1.Fill(this._subtitle);
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The numeric up down auto transparent alpha max_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownAutoTransparentAlphaMax_ValueChanged(object sender, EventArgs e)
        {
            this.SubtitleListView1SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The tool strip menu item image save as_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void toolStripMenuItemImageSaveAs_Click(object sender, EventArgs e)
        {
            this.SaveImageAsToolStripMenuItemClick(sender, e);
        }

        /// <summary>
        /// The n ocr training tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void nOcrTrainingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new VobSubNOcrTrain())
            {
                form.Initialize(this._nOcrDb);
                form.Show(this);
            }
        }

        /// <summary>
        /// The get auto guess level.
        /// </summary>
        /// <returns>
        /// The <see cref="AutoGuessLevel"/>.
        /// </returns>
        private OcrFixEngine.AutoGuessLevel GetAutoGuessLevel()
        {
            var autoGuessLevel = OcrFixEngine.AutoGuessLevel.None;
            if (this.checkBoxGuessUnknownWords.Checked)
            {
                autoGuessLevel = OcrFixEngine.AutoGuessLevel.Aggressive;
            }

            return autoGuessLevel;
        }

        /// <summary>
        /// The import new time codes tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void importNewTimeCodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenSubtitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetOpenDialogFilter();
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = this.openFileDialog1.FileName;
                if (!File.Exists(fileName))
                {
                    return;
                }

                var fi = new FileInfo(fileName);
                if (fi.Length > 1024 * 1024 * 10)
                {
                    // max 10 mb
                    if (MessageBox.Show(string.Format(Configuration.Settings.Language.Main.FileXIsLargerThan10MB + Environment.NewLine + Environment.NewLine + Configuration.Settings.Language.Main.ContinueAnyway, fileName), this.Text, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                Subtitle sub = new Subtitle();
                Encoding encoding;
                SubtitleFormat format = sub.LoadSubtitle(fileName, out encoding, null);
                if (format == null)
                {
                    return;
                }

                int index = 0;
                foreach (Paragraph p in sub.Paragraphs)
                {
                    if (index < this._subtitle.Paragraphs.Count)
                    {
                        Paragraph currentP = this._subtitle.Paragraphs[index];
                        currentP.StartTime.TotalMilliseconds = p.StartTime.TotalMilliseconds;
                        currentP.EndTime.TotalMilliseconds = p.EndTime.TotalMilliseconds;
                        this.subtitleListView1.SetStartTime(index, currentP);
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
                if (this._ocrFixEngine != null)
                {
                    this._ocrFixEngine.Dispose();
                    this._ocrFixEngine = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// The compare item.
        /// </summary>
        internal class CompareItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CompareItem"/> class.
            /// </summary>
            /// <param name="bmp">
            /// The bmp.
            /// </param>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="isItalic">
            /// The is italic.
            /// </param>
            /// <param name="expandCount">
            /// The expand count.
            /// </param>
            /// <param name="text">
            /// The text.
            /// </param>
            public CompareItem(ManagedBitmap bmp, string name, bool isItalic, int expandCount, string text)
            {
                this.Bitmap = bmp;
                this.Name = name;
                this.Italic = isItalic;
                this.ExpandCount = expandCount;
                this.NumberOfForegroundColors = -1;
                this.Text = text;
            }

            /// <summary>
            /// Gets the bitmap.
            /// </summary>
            public ManagedBitmap Bitmap { get; private set; }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether italic.
            /// </summary>
            public bool Italic { get; set; }

            /// <summary>
            /// Gets the expand count.
            /// </summary>
            public int ExpandCount { get; private set; }

            /// <summary>
            /// Gets or sets the number of foreground colors.
            /// </summary>
            public int NumberOfForegroundColors { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }
        }

        /// <summary>
        /// The sub pictures with separate time codes.
        /// </summary>
        internal class SubPicturesWithSeparateTimeCodes
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SubPicturesWithSeparateTimeCodes"/> class.
            /// </summary>
            /// <param name="subPicture">
            /// The sub picture.
            /// </param>
            /// <param name="start">
            /// The start.
            /// </param>
            /// <param name="end">
            /// The end.
            /// </param>
            public SubPicturesWithSeparateTimeCodes(SubPicture subPicture, TimeSpan start, TimeSpan end)
            {
                this.Picture = subPicture;
                this.Start = start;
                this.End = end;
            }

            /// <summary>
            /// Gets the picture.
            /// </summary>
            public SubPicture Picture { get; private set; }

            /// <summary>
            /// Gets the start.
            /// </summary>
            public TimeSpan Start { get; private set; }

            /// <summary>
            /// Gets the end.
            /// </summary>
            public TimeSpan End { get; private set; }
        }

        /// <summary>
        /// The n ocr thread parameter.
        /// </summary>
        internal class NOcrThreadParameter
        {
            /// <summary>
            /// The n ocr last lowercase height.
            /// </summary>
            public int NOcrLastLowercaseHeight;

            /// <summary>
            /// The n ocr last uppercase height.
            /// </summary>
            public int NOcrLastUppercaseHeight;

            /// <summary>
            /// The number of pixels is space.
            /// </summary>
            public int NumberOfPixelsIsSpace;

            /// <summary>
            /// The right to left.
            /// </summary>
            public bool RightToLeft;

            /// <summary>
            /// Initializes a new instance of the <see cref="NOcrThreadParameter"/> class.
            /// </summary>
            /// <param name="picture">
            /// The picture.
            /// </param>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="nOcrChars">
            /// The n ocr chars.
            /// </param>
            /// <param name="self">
            /// The self.
            /// </param>
            /// <param name="increment">
            /// The increment.
            /// </param>
            /// <param name="unItalicFactor">
            /// The un italic factor.
            /// </param>
            /// <param name="advancedItalicDetection">
            /// The advanced italic detection.
            /// </param>
            /// <param name="numberOfPixelsIsSpace">
            /// The number of pixels is space.
            /// </param>
            /// <param name="rightToLeft">
            /// The right to left.
            /// </param>
            public NOcrThreadParameter(Bitmap picture, int index, List<NOcrChar> nOcrChars, BackgroundWorker self, int increment, double unItalicFactor, bool advancedItalicDetection, int numberOfPixelsIsSpace, bool rightToLeft)
            {
                this.Self = self;
                this.Picture = picture;
                this.Index = index;
                this.NOcrChars = new List<NOcrChar>();
                foreach (NOcrChar c in nOcrChars)
                {
                    this.NOcrChars.Add(new NOcrChar(c));
                }

                this.Increment = increment;
                this.UnItalicFactor = unItalicFactor;
                this.AdvancedItalicDetection = advancedItalicDetection;
                this.NOcrLastLowercaseHeight = -1;
                this.NOcrLastUppercaseHeight = -1;
                this.NumberOfPixelsIsSpace = numberOfPixelsIsSpace;
                this.RightToLeft = rightToLeft;
            }

            /// <summary>
            /// Gets or sets the picture.
            /// </summary>
            public Bitmap Picture { get; set; }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the increment.
            /// </summary>
            public int Increment { get; set; }

            /// <summary>
            /// Gets or sets the result.
            /// </summary>
            public string Result { get; set; }

            /// <summary>
            /// Gets or sets the n ocr chars.
            /// </summary>
            public List<NOcrChar> NOcrChars { get; set; }

            /// <summary>
            /// Gets or sets the self.
            /// </summary>
            public BackgroundWorker Self { get; set; }

            /// <summary>
            /// Gets or sets the un italic factor.
            /// </summary>
            public double UnItalicFactor { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether advanced italic detection.
            /// </summary>
            public bool AdvancedItalicDetection { get; set; }
        }

        /// <summary>
        /// The image compare thread parameter.
        /// </summary>
        internal class ImageCompareThreadParameter
        {
            /// <summary>
            /// The max error percent.
            /// </summary>
            public float MaxErrorPercent;

            /// <summary>
            /// The number of pixels is space.
            /// </summary>
            public int NumberOfPixelsIsSpace;

            /// <summary>
            /// The right to left.
            /// </summary>
            public bool RightToLeft;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageCompareThreadParameter"/> class.
            /// </summary>
            /// <param name="picture">
            /// The picture.
            /// </param>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="compareBitmaps">
            /// The compare bitmaps.
            /// </param>
            /// <param name="self">
            /// The self.
            /// </param>
            /// <param name="increment">
            /// The increment.
            /// </param>
            /// <param name="numberOfPixelsIsSpace">
            /// The number of pixels is space.
            /// </param>
            /// <param name="rightToLeft">
            /// The right to left.
            /// </param>
            /// <param name="maxErrorPercent">
            /// The max error percent.
            /// </param>
            public ImageCompareThreadParameter(Bitmap picture, int index, List<CompareItem> compareBitmaps, BackgroundWorker self, int increment, int numberOfPixelsIsSpace, bool rightToLeft, float maxErrorPercent)
            {
                this.Self = self;
                this.Picture = picture;
                this.Index = index;
                this.CompareBitmaps = new List<CompareItem>();
                foreach (CompareItem c in compareBitmaps)
                {
                    this.CompareBitmaps.Add(c);
                }

                this.Increment = increment;
                this.NumberOfPixelsIsSpace = numberOfPixelsIsSpace;
                this.RightToLeft = rightToLeft;
                this.MaxErrorPercent = maxErrorPercent;
            }

            /// <summary>
            /// Gets or sets the picture.
            /// </summary>
            public Bitmap Picture { get; set; }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the increment.
            /// </summary>
            public int Increment { get; set; }

            /// <summary>
            /// Gets or sets the result.
            /// </summary>
            public string Result { get; set; }

            /// <summary>
            /// Gets or sets the compare bitmaps.
            /// </summary>
            public List<CompareItem> CompareBitmaps { get; set; }

            /// <summary>
            /// Gets or sets the self.
            /// </summary>
            public BackgroundWorker Self { get; set; }
        }

        /// <summary>
        /// The compare match.
        /// </summary>
        internal class CompareMatch
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CompareMatch"/> class.
            /// </summary>
            /// <param name="text">
            /// The text.
            /// </param>
            /// <param name="italic">
            /// The italic.
            /// </param>
            /// <param name="expandCount">
            /// The expand count.
            /// </param>
            /// <param name="name">
            /// The name.
            /// </param>
            public CompareMatch(string text, bool italic, int expandCount, string name)
            {
                this.Text = text;
                this.Italic = italic;
                this.ExpandCount = expandCount;
                this.Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CompareMatch"/> class.
            /// </summary>
            /// <param name="text">
            /// The text.
            /// </param>
            /// <param name="italic">
            /// The italic.
            /// </param>
            /// <param name="expandCount">
            /// The expand count.
            /// </param>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="character">
            /// The character.
            /// </param>
            public CompareMatch(string text, bool italic, int expandCount, string name, NOcrChar character)
                : this(text, italic, expandCount, name)
            {
                this.NOcrCharacter = character;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CompareMatch"/> class.
            /// </summary>
            /// <param name="text">
            /// The text.
            /// </param>
            /// <param name="italic">
            /// The italic.
            /// </param>
            /// <param name="expandCount">
            /// The expand count.
            /// </param>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="imageSplitterItem">
            /// The image splitter item.
            /// </param>
            public CompareMatch(string text, bool italic, int expandCount, string name, ImageSplitterItem imageSplitterItem)
                : this(text, italic, expandCount, name)
            {
                this.ImageSplitterItem = imageSplitterItem;
            }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether italic.
            /// </summary>
            public bool Italic { get; set; }

            /// <summary>
            /// Gets or sets the expand count.
            /// </summary>
            public int ExpandCount { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the n ocr character.
            /// </summary>
            public NOcrChar NOcrCharacter { get; set; }

            /// <summary>
            /// Gets or sets the image splitter item.
            /// </summary>
            public ImageSplitterItem ImageSplitterItem { get; set; }

            /// <summary>
            /// Gets or sets the x.
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Gets or sets the y.
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                if (this.Italic)
                {
                    return this.Text + " (italic)";
                }

                if (this.Text == null)
                {
                    return string.Empty;
                }

                return this.Text;
            }
        }

        /// <summary>
        /// The image compare addition.
        /// </summary>
        internal class ImageCompareAddition
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ImageCompareAddition"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="text">
            /// The text.
            /// </param>
            /// <param name="image">
            /// The image.
            /// </param>
            /// <param name="italic">
            /// The italic.
            /// </param>
            /// <param name="index">
            /// The index.
            /// </param>
            public ImageCompareAddition(string name, string text, NikseBitmap image, bool italic, int index)
            {
                this.Name = name;
                this.Text = text;
                this.Image = image;
                this.Text = text;
                this.Italic = italic;
                this.Index = index;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            public NikseBitmap Image { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether italic.
            /// </summary>
            public bool Italic { get; set; }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                if (this.Image == null)
                {
                    return this.Text;
                }

                if (this.Italic)
                {
                    return this.Text + " (" + this.Image.Width + "x" + this.Image.Height + ", italic)";
                }

                return this.Text + " (" + this.Image.Width + "x" + this.Image.Height + ")";
            }
        }

        /// <summary>
        /// The tesseract language.
        /// </summary>
        private class TesseractLanguage
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public string Id { get; set; }

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

        /// <summary>
        /// The modi parameter.
        /// </summary>
        private class ModiParameter
        {
            /// <summary>
            /// Gets or sets the bitmap.
            /// </summary>
            public Bitmap Bitmap { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public int Language { get; set; }
        }
    }
}