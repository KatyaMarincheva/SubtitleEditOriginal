// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="">
//   
// </copyright>
// <summary>
//   The recent file entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    // The settings classes are built for easy xml-serialization (makes save/load code simple)
    // ...but the built-in serialization is too slow - so a custom (de-)serialization has been used!

    /// <summary>
    /// The recent file entry.
    /// </summary>
    public class RecentFileEntry
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the video file name.
        /// </summary>
        public string VideoFileName { get; set; }

        /// <summary>
        /// Gets or sets the first visible index.
        /// </summary>
        public int FirstVisibleIndex { get; set; }

        /// <summary>
        /// Gets or sets the first selected index.
        /// </summary>
        public int FirstSelectedIndex { get; set; }
    }

    /// <summary>
    /// The recent files settings.
    /// </summary>
    public class RecentFilesSettings
    {
        /// <summary>
        /// The max recent files.
        /// </summary>
        private const int MaxRecentFiles = 25;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentFilesSettings"/> class.
        /// </summary>
        public RecentFilesSettings()
        {
            this.Files = new List<RecentFileEntry>();
        }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        [XmlArrayItem("FileName")]
        public List<RecentFileEntry> Files { get; set; }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="firstVisibleIndex">
        /// The first visible index.
        /// </param>
        /// <param name="firstSelectedIndex">
        /// The first selected index.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        public void Add(string fileName, int firstVisibleIndex, int firstSelectedIndex, string videoFileName, string originalFileName)
        {
            var newList = new List<RecentFileEntry> { new RecentFileEntry { FileName = fileName, FirstVisibleIndex = firstVisibleIndex, FirstSelectedIndex = firstSelectedIndex, VideoFileName = videoFileName, OriginalFileName = originalFileName } };
            int index = 0;
            foreach (var oldRecentFile in this.Files)
            {
                if (!fileName.Equals(oldRecentFile.FileName, StringComparison.OrdinalIgnoreCase) && index < MaxRecentFiles)
                {
                    newList.Add(new RecentFileEntry { FileName = oldRecentFile.FileName, FirstVisibleIndex = oldRecentFile.FirstVisibleIndex, FirstSelectedIndex = oldRecentFile.FirstSelectedIndex, VideoFileName = oldRecentFile.VideoFileName, OriginalFileName = oldRecentFile.OriginalFileName });
                }

                index++;
            }

            this.Files = newList;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        public void Add(string fileName, string videoFileName, string originalFileName)
        {
            var newList = new List<RecentFileEntry>();
            foreach (var oldRecentFile in this.Files)
            {
                if (fileName.Equals(oldRecentFile.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    newList.Add(new RecentFileEntry { FileName = oldRecentFile.FileName, FirstVisibleIndex = oldRecentFile.FirstVisibleIndex, FirstSelectedIndex = oldRecentFile.FirstSelectedIndex, VideoFileName = oldRecentFile.VideoFileName, OriginalFileName = oldRecentFile.OriginalFileName });
                }
            }

            if (newList.Count == 0)
            {
                newList.Add(new RecentFileEntry { FileName = fileName, FirstVisibleIndex = -1, FirstSelectedIndex = -1, VideoFileName = videoFileName, OriginalFileName = originalFileName });
            }

            int index = 0;
            foreach (var oldRecentFile in this.Files)
            {
                if (!fileName.Equals(oldRecentFile.FileName, StringComparison.OrdinalIgnoreCase) && index < MaxRecentFiles)
                {
                    newList.Add(new RecentFileEntry { FileName = oldRecentFile.FileName, FirstVisibleIndex = oldRecentFile.FirstVisibleIndex, FirstSelectedIndex = oldRecentFile.FirstSelectedIndex, VideoFileName = oldRecentFile.VideoFileName, OriginalFileName = oldRecentFile.OriginalFileName });
                }

                index++;
            }

            this.Files = newList;
        }
    }

    /// <summary>
    /// The tools settings.
    /// </summary>
    public class ToolsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsSettings"/> class.
        /// </summary>
        public ToolsSettings()
        {
            this.StartSceneIndex = 1;
            this.EndSceneIndex = 1;
            this.VerifyPlaySeconds = 2;
            this.MergeLinesShorterThan = 33;
            this.FixShortDisplayTimesAllowMoveStartTime = false;
            this.MusicSymbol = "♪";
            this.MusicSymbolToReplace = "âª â¶ â™ª âTª ã¢â™âª ?t×3 ?t¤3 #";
            this.UnicodeSymbolsToInsert = "♪;♫;☺;☹;♥;©;☮;☯;Σ;∞;≡;⇒;π";
            this.SpellCheckAutoChangeNames = true;
            this.OcrFixUseHardcodedRules = true;
            this.Interjections = "Ah;Ahh;Ahhh;Ahhhh;Eh;Ehh;Ehhh;Hm;Hmm;Hmmm;Huh;Mm;Mmm;Mmmm;Phew;Gah;Oh;Ohh;Ohhh;Ow;Oww;Owww;Ugh;Ughh;Uh;Uhh;Uhhh;Whew";
            this.MicrosoftBingApiId = "C2C2E9A508E6748F0494D68DFD92FAA1FF9B0BA4";
            this.GoogleApiKey = "ABQIAAAA4j5cWwa3lDH0RkZceh7PjBTDmNAghl5kWSyuukQ0wtoJG8nFBxRPlalq-gAvbeCXMCkmrysqjXV1Gw";
            this.UseGooleApiPaidService = false;
            this.GoogleTranslateLastTargetLanguage = "en";
            this.SpellCheckOneLetterWords = true;
            this.SpellCheckEnglishAllowInQuoteAsIng = false;
            this.SpellCheckShowCompletedMessage = true;
            this.ListViewSyntaxColorDurationSmall = true;
            this.ListViewSyntaxColorDurationBig = true;
            this.ListViewSyntaxColorOverlap = true;
            this.ListViewSyntaxColorLongLines = true;
            this.ListViewSyntaxMoreThanXLines = true;
            this.ListViewSyntaxMoreThanXLinesX = 2;
            this.ListViewSyntaxErrorColor = Color.FromArgb(255, 180, 150);
            this.ListViewUnfocusedSelectedColor = Color.LightBlue;
            this.SplitAdvanced = false;
            this.SplitNumberOfParts = 3;
            this.SplitVia = "Lines";
            this.NewEmptyTranslationText = string.Empty;
            this.BatchConvertLanguage = "en";
            this.ModifySelectionRule = "Contains";
            this.ModifySelectionText = string.Empty;
            this.GenerateTimeCodePatterns = "HH:mm:ss;yyyy-MM-dd;dddd dd MMMM yyyy <br>HH:mm:ss;dddd dd MMMM yyyy <br>hh:mm:ss tt;s";
            this.MusicSymbolStyle = "Double"; // 'Double' or 'Single'
            this.ExportFontColor = Color.White;
            this.ExportBorderColor = Color.Black;
            this.ExportShadowColor = Color.Black;
            this.ExportBottomMargin = 15;
            this.ExportHorizontalAlignment = 1; // 1=center (0=left, 2=right)
            this.ExportVobSubSimpleRendering = true;
            this.ExportVobAntiAliasingWithTransparency = true;
            this.ExportBluRayBottomMargin = 20;
            this.ExportBluRayShadow = 1;
            this.Export3DType = 0;
            this.Export3DDepth = 0;
            this.ExportLastShadowTransparency = 200;
            this.ExportLastFrameRate = 24.0d;
            this.ExportPenLineJoin = "Round";
            this.ExportFcpImageType = "Bmp";
            this.ExportLastBorderWidth = 2;
            this.BridgeGapMilliseconds = 100;
            this.ExportCustomTemplates = "SubRipÆÆ{number}\r\n{start} --> {end}\r\n{text}\r\n\r\nÆhh:mm:ss,zzzÆ[Do not modify]ÆæMicroDVDÆÆ{{start}}{{end}}{text}\r\nÆffÆ||Æ";
            this.UseNoLineBreakAfter = false;
            this.NoLineBreakAfterEnglish = " Mrs.; Ms.; Mr.; Dr.; a; an; the; my; my own; your; his; our; their; it's; is; are;'s; 're; would;'ll;'ve;'d; will; that; which; who; whom; whose; whichever; whoever; wherever; each; either; every; all; both; few; many; sevaral; all; any; most; been; been doing; none; some; my own; your own; his own; her own; our own; their own; I; she; he; as per; as regards; into; onto; than; where as; abaft; aboard; about; above; across; afore; after; against; along; alongside; amid; amidst; among; amongst; anenst; apropos; apud; around; as; aside; astride; at; athwart; atop; barring; before; behind; below; beneath; beside; besides; between; betwixt; beyond; but; by; circa; ca; concerning; despite; down; during; except; excluding; following; for; forenenst; from; given; in; including; inside; into; lest; like; minus; modulo; near; next; of; off; on; onto; opposite; out; outside; over; pace; past; per; plus; pro; qua; regarding; round; sans; save; since; than; through; thru; throughout; thruout; till; to; toward; towards; under; underneath; unlike; until; unto; up; upon; versus; vs; via; vice; with; within; without; considering; respecting; one; two; another; three; our; five; six; seven; eight; nine; ten; eleven; twelve; thirteen; fourteen; fifteen; sixteen; seventeen; eighteen; nineteen; twenty; thirty; forty; fifty; sixty; seventy; eighty; ninety; hundred; thousand; million; billion; trillion; while; however; what; zero; little; enough; after; although; and; as; if; though; although; because; before; both; but; even; how; than; nor; or; only; unless; until; yet; was; were";
            this.FindHistory = new List<string>();
            this.ImportTextLineBreak = "|";
        }

        /// <summary>
        /// Gets or sets the start scene index.
        /// </summary>
        public int StartSceneIndex { get; set; }

        /// <summary>
        /// Gets or sets the end scene index.
        /// </summary>
        public int EndSceneIndex { get; set; }

        /// <summary>
        /// Gets or sets the verify play seconds.
        /// </summary>
        public int VerifyPlaySeconds { get; set; }

        /// <summary>
        /// Gets or sets the merge lines shorter than.
        /// </summary>
        public int MergeLinesShorterThan { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix short display times allow move start time.
        /// </summary>
        public bool FixShortDisplayTimesAllowMoveStartTime { get; set; }

        /// <summary>
        /// Gets or sets the music symbol.
        /// </summary>
        public string MusicSymbol { get; set; }

        /// <summary>
        /// Gets or sets the music symbol to replace.
        /// </summary>
        public string MusicSymbolToReplace { get; set; }

        /// <summary>
        /// Gets or sets the unicode symbols to insert.
        /// </summary>
        public string UnicodeSymbolsToInsert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spell check auto change names.
        /// </summary>
        public bool SpellCheckAutoChangeNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spell check one letter words.
        /// </summary>
        public bool SpellCheckOneLetterWords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spell check english allow in quote as ing.
        /// </summary>
        public bool SpellCheckEnglishAllowInQuoteAsIng { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spell check show completed message.
        /// </summary>
        public bool SpellCheckShowCompletedMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ocr fix use hardcoded rules.
        /// </summary>
        public bool OcrFixUseHardcodedRules { get; set; }

        /// <summary>
        /// Gets or sets the interjections.
        /// </summary>
        public string Interjections { get; set; }

        /// <summary>
        /// Gets or sets the microsoft bing api id.
        /// </summary>
        public string MicrosoftBingApiId { get; set; }

        /// <summary>
        /// Gets or sets the google api key.
        /// </summary>
        public string GoogleApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use goole api paid service.
        /// </summary>
        public bool UseGooleApiPaidService { get; set; }

        /// <summary>
        /// Gets or sets the google translate last target language.
        /// </summary>
        public string GoogleTranslateLastTargetLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether list view syntax color duration small.
        /// </summary>
        public bool ListViewSyntaxColorDurationSmall { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether list view syntax color duration big.
        /// </summary>
        public bool ListViewSyntaxColorDurationBig { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether list view syntax color overlap.
        /// </summary>
        public bool ListViewSyntaxColorOverlap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether list view syntax color long lines.
        /// </summary>
        public bool ListViewSyntaxColorLongLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether list view syntax more than x lines.
        /// </summary>
        public bool ListViewSyntaxMoreThanXLines { get; set; }

        /// <summary>
        /// Gets or sets the list view syntax more than x lines x.
        /// </summary>
        public int ListViewSyntaxMoreThanXLinesX { get; set; }

        /// <summary>
        /// Gets or sets the list view syntax error color.
        /// </summary>
        public Color ListViewSyntaxErrorColor { get; set; }

        /// <summary>
        /// Gets or sets the list view unfocused selected color.
        /// </summary>
        public Color ListViewUnfocusedSelectedColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether split advanced.
        /// </summary>
        public bool SplitAdvanced { get; set; }

        /// <summary>
        /// Gets or sets the split output folder.
        /// </summary>
        public string SplitOutputFolder { get; set; }

        /// <summary>
        /// Gets or sets the split number of parts.
        /// </summary>
        public int SplitNumberOfParts { get; set; }

        /// <summary>
        /// Gets or sets the split via.
        /// </summary>
        public string SplitVia { get; set; }

        /// <summary>
        /// Gets or sets the last show earlier or later selection.
        /// </summary>
        public string LastShowEarlierOrLaterSelection { get; set; }

        /// <summary>
        /// Gets or sets the new empty translation text.
        /// </summary>
        public string NewEmptyTranslationText { get; set; }

        /// <summary>
        /// Gets or sets the batch convert output folder.
        /// </summary>
        public string BatchConvertOutputFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert overwrite existing.
        /// </summary>
        public bool BatchConvertOverwriteExisting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert overwrite original.
        /// </summary>
        public bool BatchConvertOverwriteOriginal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert remove formatting.
        /// </summary>
        public bool BatchConvertRemoveFormatting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert fix casing.
        /// </summary>
        public bool BatchConvertFixCasing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert remove text for hi.
        /// </summary>
        public bool BatchConvertRemoveTextForHI { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert fix common errors.
        /// </summary>
        public bool BatchConvertFixCommonErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert multiple replace.
        /// </summary>
        public bool BatchConvertMultipleReplace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert split long lines.
        /// </summary>
        public bool BatchConvertSplitLongLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert auto balance.
        /// </summary>
        public bool BatchConvertAutoBalance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether batch convert set min display time between subtitles.
        /// </summary>
        public bool BatchConvertSetMinDisplayTimeBetweenSubtitles { get; set; }

        /// <summary>
        /// Gets or sets the batch convert language.
        /// </summary>
        public string BatchConvertLanguage { get; set; }

        /// <summary>
        /// Gets or sets the batch convert format.
        /// </summary>
        public string BatchConvertFormat { get; set; }

        /// <summary>
        /// Gets or sets the modify selection text.
        /// </summary>
        public string ModifySelectionText { get; set; }

        /// <summary>
        /// Gets or sets the modify selection rule.
        /// </summary>
        public string ModifySelectionRule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether modify selection case sensitive.
        /// </summary>
        public bool ModifySelectionCaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets the export vob sub font name.
        /// </summary>
        public string ExportVobSubFontName { get; set; }

        /// <summary>
        /// Gets or sets the export vob sub font size.
        /// </summary>
        public int ExportVobSubFontSize { get; set; }

        /// <summary>
        /// Gets or sets the export vob sub video resolution.
        /// </summary>
        public string ExportVobSubVideoResolution { get; set; }

        /// <summary>
        /// Gets or sets the export vob sub language.
        /// </summary>
        public string ExportVobSubLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether export vob sub simple rendering.
        /// </summary>
        public bool ExportVobSubSimpleRendering { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether export vob anti aliasing with transparency.
        /// </summary>
        public bool ExportVobAntiAliasingWithTransparency { get; set; }

        /// <summary>
        /// Gets or sets the export blu ray font name.
        /// </summary>
        public string ExportBluRayFontName { get; set; }

        /// <summary>
        /// Gets or sets the export blu ray font size.
        /// </summary>
        public int ExportBluRayFontSize { get; set; }

        /// <summary>
        /// Gets or sets the export fcp font name.
        /// </summary>
        public string ExportFcpFontName { get; set; }

        /// <summary>
        /// Gets or sets the export font name other.
        /// </summary>
        public string ExportFontNameOther { get; set; }

        /// <summary>
        /// Gets or sets the export fcp font size.
        /// </summary>
        public int ExportFcpFontSize { get; set; }

        /// <summary>
        /// Gets or sets the export fcp image type.
        /// </summary>
        public string ExportFcpImageType { get; set; }

        /// <summary>
        /// Gets or sets the export bdn xml image type.
        /// </summary>
        public string ExportBdnXmlImageType { get; set; }

        /// <summary>
        /// Gets or sets the export last font size.
        /// </summary>
        public int ExportLastFontSize { get; set; }

        /// <summary>
        /// Gets or sets the export last line height.
        /// </summary>
        public int ExportLastLineHeight { get; set; }

        /// <summary>
        /// Gets or sets the export last border width.
        /// </summary>
        public int ExportLastBorderWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether export last font bold.
        /// </summary>
        public bool ExportLastFontBold { get; set; }

        /// <summary>
        /// Gets or sets the export blu ray video resolution.
        /// </summary>
        public string ExportBluRayVideoResolution { get; set; }

        /// <summary>
        /// Gets or sets the export font color.
        /// </summary>
        public Color ExportFontColor { get; set; }

        /// <summary>
        /// Gets or sets the export border color.
        /// </summary>
        public Color ExportBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the export shadow color.
        /// </summary>
        public Color ExportShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the export bottom margin.
        /// </summary>
        public int ExportBottomMargin { get; set; }

        /// <summary>
        /// Gets or sets the export horizontal alignment.
        /// </summary>
        public int ExportHorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the export blu ray bottom margin.
        /// </summary>
        public int ExportBluRayBottomMargin { get; set; }

        /// <summary>
        /// Gets or sets the export blu ray shadow.
        /// </summary>
        public int ExportBluRayShadow { get; set; }

        /// <summary>
        /// Gets or sets the export 3 d type.
        /// </summary>
        public int Export3DType { get; set; }

        /// <summary>
        /// Gets or sets the export 3 d depth.
        /// </summary>
        public int Export3DDepth { get; set; }

        /// <summary>
        /// Gets or sets the export last shadow transparency.
        /// </summary>
        public int ExportLastShadowTransparency { get; set; }

        /// <summary>
        /// Gets or sets the export last frame rate.
        /// </summary>
        public double ExportLastFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the export pen line join.
        /// </summary>
        public string ExportPenLineJoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix common errors fix overlap allow equal end start.
        /// </summary>
        public bool FixCommonErrorsFixOverlapAllowEqualEndStart { get; set; }

        /// <summary>
        /// Gets or sets the import text splitting.
        /// </summary>
        public string ImportTextSplitting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether import text merge short lines.
        /// </summary>
        public bool ImportTextMergeShortLines { get; set; }

        /// <summary>
        /// Gets or sets the import text line break.
        /// </summary>
        public string ImportTextLineBreak { get; set; }

        /// <summary>
        /// Gets or sets the generate time code patterns.
        /// </summary>
        public string GenerateTimeCodePatterns { get; set; }

        /// <summary>
        /// Gets or sets the music symbol style.
        /// </summary>
        public string MusicSymbolStyle { get; set; }

        /// <summary>
        /// Gets or sets the bridge gap milliseconds.
        /// </summary>
        public int BridgeGapMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the export custom templates.
        /// </summary>
        public string ExportCustomTemplates { get; set; }

        /// <summary>
        /// Gets or sets the change casing choice.
        /// </summary>
        public string ChangeCasingChoice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use no line break after.
        /// </summary>
        public bool UseNoLineBreakAfter { get; set; }

        /// <summary>
        /// Gets or sets the no line break after english.
        /// </summary>
        public string NoLineBreakAfterEnglish { get; set; }

        /// <summary>
        /// Gets or sets the find history.
        /// </summary>
        public List<string> FindHistory { get; set; }
    }

    /// <summary>
    /// The word list settings.
    /// </summary>
    public class WordListSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordListSettings"/> class.
        /// </summary>
        public WordListSettings()
        {
            this.LastLanguage = "en-US";
            this.NamesEtcUrl = "https://raw.githubusercontent.com/SubtitleEdit/subtitleedit/master/Dictionaries/names_etc.xml";
        }

        /// <summary>
        /// Gets or sets the last language.
        /// </summary>
        public string LastLanguage { get; set; }

        /// <summary>
        /// Gets or sets the names etc url.
        /// </summary>
        public string NamesEtcUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use online names etc.
        /// </summary>
        public bool UseOnlineNamesEtc { get; set; }
    }

    /// <summary>
    /// The subtitle settings.
    /// </summary>
    public class SubtitleSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtitleSettings"/> class.
        /// </summary>
        public SubtitleSettings()
        {
            this.SsaFontName = "Arial";
            this.SsaFontSize = 20;
            this.SsaFontColorArgb = Color.FromArgb(255, 255, 255).ToArgb();
            this.SsaOutline = 2;
            this.SsaShadow = 1;
            this.SsaOpaqueBox = false;

            this.DCinemaFontFile = "Arial.ttf";
            this.DCinemaLoadFontResource = "urn:uuid:3dec6dc0-39d0-498d-97d0-928d2eb78391";
            this.DCinemaFontSize = 42;
            this.DCinemaBottomMargin = 8;
            this.DCinemaZPosition = 0;
            this.DCinemaFadeUpTime = 5;
            this.DCinemaFadeDownTime = 5;

            this.SamiDisplayTwoClassesAsTwoSubtitles = true;
            this.SamiHtmlEncodeMode = 0;

            this.TimedText10TimeCodeFormat = "Source";

            this.FcpFontSize = 18;
            this.FcpFontName = "Lucida Grande";
        }

        /// <summary>
        /// Gets or sets the ssa font name.
        /// </summary>
        public string SsaFontName { get; set; }

        /// <summary>
        /// Gets or sets the ssa font size.
        /// </summary>
        public double SsaFontSize { get; set; }

        /// <summary>
        /// Gets or sets the ssa font color argb.
        /// </summary>
        public int SsaFontColorArgb { get; set; }

        /// <summary>
        /// Gets or sets the ssa outline.
        /// </summary>
        public int SsaOutline { get; set; }

        /// <summary>
        /// Gets or sets the ssa shadow.
        /// </summary>
        public int SsaShadow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ssa opaque box.
        /// </summary>
        public bool SsaOpaqueBox { get; set; }

        /// <summary>
        /// Gets or sets the d cinema font file.
        /// </summary>
        public string DCinemaFontFile { get; set; }

        /// <summary>
        /// Gets or sets the d cinema load font resource.
        /// </summary>
        public string DCinemaLoadFontResource { get; set; }

        /// <summary>
        /// Gets or sets the d cinema font size.
        /// </summary>
        public int DCinemaFontSize { get; set; }

        /// <summary>
        /// Gets or sets the d cinema bottom margin.
        /// </summary>
        public int DCinemaBottomMargin { get; set; }

        /// <summary>
        /// Gets or sets the d cinema z position.
        /// </summary>
        public double DCinemaZPosition { get; set; }

        /// <summary>
        /// Gets or sets the d cinema fade up time.
        /// </summary>
        public int DCinemaFadeUpTime { get; set; }

        /// <summary>
        /// Gets or sets the d cinema fade down time.
        /// </summary>
        public int DCinemaFadeDownTime { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema subtitle id.
        /// </summary>
        public string CurrentDCinemaSubtitleId { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema movie title.
        /// </summary>
        public string CurrentDCinemaMovieTitle { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema reel number.
        /// </summary>
        public string CurrentDCinemaReelNumber { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema issue date.
        /// </summary>
        public string CurrentDCinemaIssueDate { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema language.
        /// </summary>
        public string CurrentDCinemaLanguage { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema edit rate.
        /// </summary>
        public string CurrentDCinemaEditRate { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema time code rate.
        /// </summary>
        public string CurrentDCinemaTimeCodeRate { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema start time.
        /// </summary>
        public string CurrentDCinemaStartTime { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema font id.
        /// </summary>
        public string CurrentDCinemaFontId { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema font uri.
        /// </summary>
        public string CurrentDCinemaFontUri { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema font color.
        /// </summary>
        public Color CurrentDCinemaFontColor { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema font effect.
        /// </summary>
        public string CurrentDCinemaFontEffect { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema font effect color.
        /// </summary>
        public Color CurrentDCinemaFontEffectColor { get; set; }

        /// <summary>
        /// Gets or sets the current d cinema font size.
        /// </summary>
        public int CurrentDCinemaFontSize { get; set; }

        /// <summary>
        /// Gets or sets the current cavena 890 language id line 1.
        /// </summary>
        public int CurrentCavena890LanguageIdLine1 { get; set; }

        /// <summary>
        /// Gets or sets the current cavena 890 language id line 2.
        /// </summary>
        public int CurrentCavena890LanguageIdLine2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cheetah caption alway write end time.
        /// </summary>
        public bool CheetahCaptionAlwayWriteEndTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sami display two classes as two subtitles.
        /// </summary>
        public bool SamiDisplayTwoClassesAsTwoSubtitles { get; set; }

        /// <summary>
        /// Gets or sets the sami html encode mode.
        /// </summary>
        public int SamiHtmlEncodeMode { get; set; }

        /// <summary>
        /// Gets or sets the timed text 10 time code format.
        /// </summary>
        public string TimedText10TimeCodeFormat { get; set; }

        /// <summary>
        /// Gets or sets the timed text 10 time code format source.
        /// </summary>
        public string TimedText10TimeCodeFormatSource { get; set; }

        /// <summary>
        /// Gets or sets the fcp font size.
        /// </summary>
        public int FcpFontSize { get; set; }

        /// <summary>
        /// Gets or sets the fcp font name.
        /// </summary>
        public string FcpFontName { get; set; }

        /// <summary>
        /// Gets or sets the nuendo character list file.
        /// </summary>
        public string NuendoCharacterListFile { get; set; }

        /// <summary>
        /// The initialize d ciname settings.
        /// </summary>
        /// <param name="smpte">
        /// The smpte.
        /// </param>
        public void InitializeDCinameSettings(bool smpte)
        {
            if (smpte)
            {
                this.CurrentDCinemaSubtitleId = "urn:uuid:" + Guid.NewGuid();
                this.CurrentDCinemaLanguage = "en";
                this.CurrentDCinemaFontUri = this.DCinemaLoadFontResource;
                this.CurrentDCinemaFontId = "theFontId";
            }
            else
            {
                string hex = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
                hex = hex.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
                this.CurrentDCinemaSubtitleId = hex;
                this.CurrentDCinemaLanguage = "English";
                this.CurrentDCinemaFontUri = this.DCinemaFontFile;
                this.CurrentDCinemaFontId = "Arial";
            }

            this.CurrentDCinemaIssueDate = DateTime.Now.ToString("s") + ".000-00:00";
            this.CurrentDCinemaMovieTitle = "title";
            this.CurrentDCinemaReelNumber = "1";
            this.CurrentDCinemaFontColor = Color.White;
            this.CurrentDCinemaFontEffect = "border";
            this.CurrentDCinemaFontEffectColor = Color.Black;
            this.CurrentDCinemaFontSize = this.DCinemaFontSize;
            this.CurrentCavena890LanguageIdLine1 = -1;
            this.CurrentCavena890LanguageIdLine2 = -1;
        }
    }

    /// <summary>
    /// The proxy settings.
    /// </summary>
    public class ProxySettings
    {
        /// <summary>
        /// Gets or sets the proxy address.
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// The decode password.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecodePassword()
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(this.Password));
        }

        /// <summary>
        /// The encode password.
        /// </summary>
        /// <param name="unencryptedPassword">
        /// The unencrypted password.
        /// </param>
        public void EncodePassword(string unencryptedPassword)
        {
            this.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(unencryptedPassword));
        }
    }

    /// <summary>
    /// The fix common errors settings.
    /// </summary>
    public class FixCommonErrorsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixCommonErrorsSettings"/> class.
        /// </summary>
        public FixCommonErrorsSettings()
        {
            this.EmptyLinesTicked = true;
            this.OverlappingDisplayTimeTicked = true;
            this.TooShortDisplayTimeTicked = true;
            this.TooLongDisplayTimeTicked = true;
            this.InvalidItalicTagsTicked = true;
            this.BreakLongLinesTicked = true;
            this.MergeShortLinesTicked = true;
            this.UnneededPeriodsTicked = true;
            this.UnneededSpacesTicked = true;
            this.MissingSpacesTicked = true;
            this.UppercaseIInsideLowercaseWordTicked = true;
            this.DoubleApostropheToQuoteTicked = true;
            this.AddPeriodAfterParagraphTicked = false;
            this.StartWithUppercaseLetterAfterParagraphTicked = true;
            this.StartWithUppercaseLetterAfterPeriodInsideParagraphTicked = false;
            this.StartWithUppercaseLetterAfterColonTicked = false;
            this.AloneLowercaseIToUppercaseIEnglishTicked = false;
            this.TurkishAnsiTicked = false;
            this.DanishLetterITicked = false;
            this.FixDoubleDashTicked = true;
            this.FixDoubleGreaterThanTicked = true;
            this.FixEllipsesStartTicked = true;
            this.FixMissingOpenBracketTicked = true;
            this.FixMusicNotationTicked = true;
        }

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        public string StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the start size.
        /// </summary>
        public string StartSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether empty lines ticked.
        /// </summary>
        public bool EmptyLinesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether overlapping display time ticked.
        /// </summary>
        public bool OverlappingDisplayTimeTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether too short display time ticked.
        /// </summary>
        public bool TooShortDisplayTimeTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether too long display time ticked.
        /// </summary>
        public bool TooLongDisplayTimeTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether invalid italic tags ticked.
        /// </summary>
        public bool InvalidItalicTagsTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether break long lines ticked.
        /// </summary>
        public bool BreakLongLinesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether merge short lines ticked.
        /// </summary>
        public bool MergeShortLinesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether merge short lines all ticked.
        /// </summary>
        public bool MergeShortLinesAllTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unneeded spaces ticked.
        /// </summary>
        public bool UnneededSpacesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unneeded periods ticked.
        /// </summary>
        public bool UnneededPeriodsTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether missing spaces ticked.
        /// </summary>
        public bool MissingSpacesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether add missing quotes ticked.
        /// </summary>
        public bool AddMissingQuotesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix 3 plus lines ticked.
        /// </summary>
        public bool Fix3PlusLinesTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix hyphens ticked.
        /// </summary>
        public bool FixHyphensTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix hyphens add ticked.
        /// </summary>
        public bool FixHyphensAddTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether uppercase i inside lowercase word ticked.
        /// </summary>
        public bool UppercaseIInsideLowercaseWordTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether double apostrophe to quote ticked.
        /// </summary>
        public bool DoubleApostropheToQuoteTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether add period after paragraph ticked.
        /// </summary>
        public bool AddPeriodAfterParagraphTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start with uppercase letter after paragraph ticked.
        /// </summary>
        public bool StartWithUppercaseLetterAfterParagraphTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start with uppercase letter after period inside paragraph ticked.
        /// </summary>
        public bool StartWithUppercaseLetterAfterPeriodInsideParagraphTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start with uppercase letter after colon ticked.
        /// </summary>
        public bool StartWithUppercaseLetterAfterColonTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether alone lowercase i to uppercase i english ticked.
        /// </summary>
        public bool AloneLowercaseIToUppercaseIEnglishTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix ocr errors via replace list ticked.
        /// </summary>
        public bool FixOcrErrorsViaReplaceListTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove space between number ticked.
        /// </summary>
        public bool RemoveSpaceBetweenNumberTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix dialogs on one line ticked.
        /// </summary>
        public bool FixDialogsOnOneLineTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether turkish ansi ticked.
        /// </summary>
        public bool TurkishAnsiTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether danish letter i ticked.
        /// </summary>
        public bool DanishLetterITicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spanish inverted question and exclamation marks ticked.
        /// </summary>
        public bool SpanishInvertedQuestionAndExclamationMarksTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix double dash ticked.
        /// </summary>
        public bool FixDoubleDashTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix double greater than ticked.
        /// </summary>
        public bool FixDoubleGreaterThanTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix ellipses start ticked.
        /// </summary>
        public bool FixEllipsesStartTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix missing open bracket ticked.
        /// </summary>
        public bool FixMissingOpenBracketTicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fix music notation ticked.
        /// </summary>
        public bool FixMusicNotationTicked { get; set; }
    }

    /// <summary>
    /// The general settings.
    /// </summary>
    public class GeneralSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralSettings"/> class.
        /// </summary>
        public GeneralSettings()
        {
            this.ShowToolbarNew = true;
            this.ShowToolbarOpen = true;
            this.ShowToolbarSave = true;
            this.ShowToolbarSaveAs = false;
            this.ShowToolbarFind = true;
            this.ShowToolbarReplace = true;
            this.ShowToolbarFixCommonErrors = false;
            this.ShowToolbarVisualSync = true;
            this.ShowToolbarSpellCheck = true;
            this.ShowToolbarSettings = false;
            this.ShowToolbarHelp = true;

            this.ShowVideoPlayer = false;
            this.ShowAudioVisualizer = false;
            this.ShowWaveform = true;
            this.ShowSpectrogram = true;
            this.ShowFrameRate = false;
            this.DefaultFrameRate = 23.976;
            this.CurrentFrameRate = this.DefaultFrameRate;
            this.SubtitleFontName = "Tahoma";
            if (Environment.OSVersion.Version.Major < 6)
            {
                // 6 == Vista/Win2008Server/Win7
                this.SubtitleFontName = "Times New Roman";
            }

            this.SubtitleFontSize = 8;
            this.SubtitleFontBold = false;
            this.SubtitleFontColor = Color.Black;
            this.SubtitleBackgroundColor = Color.White;
            this.CenterSubtitleInTextBox = false;
            this.DefaultSubtitleFormat = "SubRip";
            this.DefaultEncoding = Encoding.UTF8.EncodingName;
            this.AutoConvertToUtf8 = false;
            this.AutoGuessAnsiEncoding = false;
            this.ShowRecentFiles = true;
            this.RememberSelectedLine = true;
            this.StartLoadLastFile = true;
            this.StartRememberPositionAndSize = true;
            this.SubtitleLineMaximumLength = 43;
            this.SubtitleMinimumDisplayMilliseconds = 1000;
            this.SubtitleMaximumDisplayMilliseconds = 8 * 1000;
            this.MinimumMillisecondsBetweenLines = 24;
            this.SetStartEndHumanDelay = 100;
            this.AutoWrapLineWhileTyping = false;
            this.SubtitleMaximumCharactersPerSeconds = 25.0;
            this.SubtitleOptimalCharactersPerSeconds = 15.0;
            this.SpellCheckLanguage = null;
            this.VideoPlayer = string.Empty;
            this.VideoPlayerDefaultVolume = 75;
            this.VideoPlayerPreviewFontSize = 10;
            this.VideoPlayerPreviewFontBold = true;
            this.VideoPlayerShowStopButton = true;
            this.VideoPlayerShowMuteButton = true;
            this.VideoPlayerShowFullscreenButton = true;
            this.ListViewLineSeparatorString = "<br />";
            this.ListViewDoubleClickAction = 1;
            this.UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWZYXÆØÃÅÄÖÉÈÁÂÀÇÊÍÓÔÕÚŁАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯĞİŞÜÙÁÌÑÎ";
            this.DefaultAdjustMilliseconds = 1000;
            this.AutoRepeatOn = true;
            this.AutoRepeatCount = 2;
            this.AutoContinueOn = false;
            this.SyncListViewWithVideoWhilePlaying = false;
            this.AutoBackupSeconds = 60 * 15;
            this.SpellChecker = "hunspell";
            this.AllowEditOfOriginalSubtitle = true;
            this.PromptDeleteLines = true;
            this.Undocked = false;
            this.UndockedVideoPosition = "-32000;-32000";
            this.UndockedWaveformPosition = "-32000;-32000";
            this.UndockedVideoControlsPosition = "-32000;-32000";
            this.SmallDelayMilliseconds = 500;
            this.LargeDelayMilliseconds = 5000;
            this.OpenSubtitleExtraExtensions = "*.mp4;*.m4v;*.mkv;*.ts"; // matroska/mp4/m4v files (can contain subtitles)
            this.ListViewColumnsRememberSize = true;
            this.VlcWaveTranscodeSettings = "acodec=s16l"; // "acodec=s16l,channels=1,ab=64,samplerate=8000";
            this.UseTimeFormatHHMMSSFF = false;
            this.ClearStatusBarAfterSeconds = 10;
            this.MoveVideo100Or500MsPlaySmallSample = false;
            this.DisableVideoAutoLoading = false;
            this.RightToLeftMode = false;
            this.LastSaveAsFormat = string.Empty;
            this.CheckForUpdates = true;
            this.LastCheckForUpdates = DateTime.Now;
            this.ShowBetaStuff = false;
            this.NewEmptyDefaultMs = 2000;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar new.
        /// </summary>
        public bool ShowToolbarNew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar open.
        /// </summary>
        public bool ShowToolbarOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar save.
        /// </summary>
        public bool ShowToolbarSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar save as.
        /// </summary>
        public bool ShowToolbarSaveAs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar find.
        /// </summary>
        public bool ShowToolbarFind { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar replace.
        /// </summary>
        public bool ShowToolbarReplace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar fix common errors.
        /// </summary>
        public bool ShowToolbarFixCommonErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar visual sync.
        /// </summary>
        public bool ShowToolbarVisualSync { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar spell check.
        /// </summary>
        public bool ShowToolbarSpellCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar settings.
        /// </summary>
        public bool ShowToolbarSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show toolbar help.
        /// </summary>
        public bool ShowToolbarHelp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show video player.
        /// </summary>
        public bool ShowVideoPlayer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show audio visualizer.
        /// </summary>
        public bool ShowAudioVisualizer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show waveform.
        /// </summary>
        public bool ShowWaveform { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show spectrogram.
        /// </summary>
        public bool ShowSpectrogram { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show frame rate.
        /// </summary>
        public bool ShowFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the default frame rate.
        /// </summary>
        public double DefaultFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the current frame rate.
        /// </summary>
        public double CurrentFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the default subtitle format.
        /// </summary>
        public string DefaultSubtitleFormat { get; set; }

        /// <summary>
        /// Gets or sets the default encoding.
        /// </summary>
        public string DefaultEncoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto convert to utf 8.
        /// </summary>
        public bool AutoConvertToUtf8 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto guess ansi encoding.
        /// </summary>
        public bool AutoGuessAnsiEncoding { get; set; }

        /// <summary>
        /// Gets or sets the subtitle font name.
        /// </summary>
        public string SubtitleFontName { get; set; }

        /// <summary>
        /// Gets or sets the subtitle font size.
        /// </summary>
        public int SubtitleFontSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subtitle font bold.
        /// </summary>
        public bool SubtitleFontBold { get; set; }

        /// <summary>
        /// Gets or sets the subtitle font color.
        /// </summary>
        public Color SubtitleFontColor { get; set; }

        /// <summary>
        /// Gets or sets the subtitle background color.
        /// </summary>
        public Color SubtitleBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether center subtitle in text box.
        /// </summary>
        public bool CenterSubtitleInTextBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show recent files.
        /// </summary>
        public bool ShowRecentFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember selected line.
        /// </summary>
        public bool RememberSelectedLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start load last file.
        /// </summary>
        public bool StartLoadLastFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start remember position and size.
        /// </summary>
        public bool StartRememberPositionAndSize { get; set; }

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        public string StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the start size.
        /// </summary>
        public string StartSize { get; set; }

        /// <summary>
        /// Gets or sets the split container main splitter distance.
        /// </summary>
        public int SplitContainerMainSplitterDistance { get; set; }

        /// <summary>
        /// Gets or sets the split container 1 splitter distance.
        /// </summary>
        public int SplitContainer1SplitterDistance { get; set; }

        /// <summary>
        /// Gets or sets the split container list view and text splitter distance.
        /// </summary>
        public int SplitContainerListViewAndTextSplitterDistance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start in source view.
        /// </summary>
        public bool StartInSourceView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove blank lines when opening.
        /// </summary>
        public bool RemoveBlankLinesWhenOpening { get; set; }

        /// <summary>
        /// Gets or sets the subtitle line maximum length.
        /// </summary>
        public int SubtitleLineMaximumLength { get; set; }

        /// <summary>
        /// Gets or sets the subtitle minimum display milliseconds.
        /// </summary>
        public int SubtitleMinimumDisplayMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the subtitle maximum display milliseconds.
        /// </summary>
        public int SubtitleMaximumDisplayMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the minimum milliseconds between lines.
        /// </summary>
        public int MinimumMillisecondsBetweenLines { get; set; }

        /// <summary>
        /// Gets or sets the set start end human delay.
        /// </summary>
        public int SetStartEndHumanDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto wrap line while typing.
        /// </summary>
        public bool AutoWrapLineWhileTyping { get; set; }

        /// <summary>
        /// Gets or sets the subtitle maximum characters per seconds.
        /// </summary>
        public double SubtitleMaximumCharactersPerSeconds { get; set; }

        /// <summary>
        /// Gets or sets the subtitle optimal characters per seconds.
        /// </summary>
        public double SubtitleOptimalCharactersPerSeconds { get; set; }

        /// <summary>
        /// Gets or sets the spell check language.
        /// </summary>
        public string SpellCheckLanguage { get; set; }

        /// <summary>
        /// Gets or sets the video player.
        /// </summary>
        public string VideoPlayer { get; set; }

        /// <summary>
        /// Gets or sets the video player default volume.
        /// </summary>
        public int VideoPlayerDefaultVolume { get; set; }

        /// <summary>
        /// Gets or sets the video player preview font size.
        /// </summary>
        public int VideoPlayerPreviewFontSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether video player preview font bold.
        /// </summary>
        public bool VideoPlayerPreviewFontBold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether video player show stop button.
        /// </summary>
        public bool VideoPlayerShowStopButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether video player show fullscreen button.
        /// </summary>
        public bool VideoPlayerShowFullscreenButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether video player show mute button.
        /// </summary>
        public bool VideoPlayerShowMuteButton { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the list view line separator string.
        /// </summary>
        public string ListViewLineSeparatorString { get; set; }

        /// <summary>
        /// Gets or sets the list view double click action.
        /// </summary>
        public int ListViewDoubleClickAction { get; set; }

        /// <summary>
        /// Gets or sets the uppercase letters.
        /// </summary>
        public string UppercaseLetters { get; set; }

        /// <summary>
        /// Gets or sets the default adjust milliseconds.
        /// </summary>
        public int DefaultAdjustMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto repeat on.
        /// </summary>
        public bool AutoRepeatOn { get; set; }

        /// <summary>
        /// Gets or sets the auto repeat count.
        /// </summary>
        public int AutoRepeatCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto continue on.
        /// </summary>
        public bool AutoContinueOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sync list view with video while playing.
        /// </summary>
        public bool SyncListViewWithVideoWhilePlaying { get; set; }

        /// <summary>
        /// Gets or sets the auto backup seconds.
        /// </summary>
        public int AutoBackupSeconds { get; set; }

        /// <summary>
        /// Gets or sets the spell checker.
        /// </summary>
        public string SpellChecker { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow edit of original subtitle.
        /// </summary>
        public bool AllowEditOfOriginalSubtitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prompt delete lines.
        /// </summary>
        public bool PromptDeleteLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether undocked.
        /// </summary>
        public bool Undocked { get; set; }

        /// <summary>
        /// Gets or sets the undocked video position.
        /// </summary>
        public string UndockedVideoPosition { get; set; }

        /// <summary>
        /// Gets or sets the undocked waveform position.
        /// </summary>
        public string UndockedWaveformPosition { get; set; }

        /// <summary>
        /// Gets or sets the undocked video controls position.
        /// </summary>
        public string UndockedVideoControlsPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform center.
        /// </summary>
        public bool WaveformCenter { get; set; }

        /// <summary>
        /// Gets or sets the small delay milliseconds.
        /// </summary>
        public int SmallDelayMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the large delay milliseconds.
        /// </summary>
        public int LargeDelayMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show original as preview if available.
        /// </summary>
        public bool ShowOriginalAsPreviewIfAvailable { get; set; }

        /// <summary>
        /// Gets or sets the last pac code page.
        /// </summary>
        public int LastPacCodePage { get; set; }

        /// <summary>
        /// Gets or sets the open subtitle extra extensions.
        /// </summary>
        public string OpenSubtitleExtraExtensions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether list view columns remember size.
        /// </summary>
        public bool ListViewColumnsRememberSize { get; set; }

        /// <summary>
        /// Gets or sets the list view number width.
        /// </summary>
        public int ListViewNumberWidth { get; set; }

        /// <summary>
        /// Gets or sets the list view start width.
        /// </summary>
        public int ListViewStartWidth { get; set; }

        /// <summary>
        /// Gets or sets the list view end width.
        /// </summary>
        public int ListViewEndWidth { get; set; }

        /// <summary>
        /// Gets or sets the list view duration width.
        /// </summary>
        public int ListViewDurationWidth { get; set; }

        /// <summary>
        /// Gets or sets the list view text width.
        /// </summary>
        public int ListViewTextWidth { get; set; }

        /// <summary>
        /// Gets or sets the vlc wave transcode settings.
        /// </summary>
        public string VlcWaveTranscodeSettings { get; set; }

        /// <summary>
        /// Gets or sets the vlc location.
        /// </summary>
        public string VlcLocation { get; set; }

        /// <summary>
        /// Gets or sets the vlc location relative.
        /// </summary>
        public string VlcLocationRelative { get; set; }

        /// <summary>
        /// Gets or sets the mpc hc location.
        /// </summary>
        public string MpcHcLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use f fmpeg for wave extraction.
        /// </summary>
        public bool UseFFmpegForWaveExtraction { get; set; }

        /// <summary>
        /// Gets or sets the f fmpeg location.
        /// </summary>
        public string FFmpegLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use time format hhmmssff.
        /// </summary>
        public bool UseTimeFormatHHMMSSFF { get; set; }

        /// <summary>
        /// Gets or sets the clear status bar after seconds.
        /// </summary>
        public int ClearStatusBarAfterSeconds { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether move video 100 or 500 ms play small sample.
        /// </summary>
        public bool MoveVideo100Or500MsPlaySmallSample { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disable video auto loading.
        /// </summary>
        public bool DisableVideoAutoLoading { get; set; }

        /// <summary>
        /// Gets or sets the new empty default ms.
        /// </summary>
        public int NewEmptyDefaultMs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether right to left mode.
        /// </summary>
        public bool RightToLeftMode { get; set; }

        /// <summary>
        /// Gets or sets the last save as format.
        /// </summary>
        public string LastSaveAsFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether check for updates.
        /// </summary>
        public bool CheckForUpdates { get; set; }

        /// <summary>
        /// Gets or sets the last check for updates.
        /// </summary>
        public DateTime LastCheckForUpdates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show beta stuff.
        /// </summary>
        public bool ShowBetaStuff { get; set; }
    }

    /// <summary>
    /// The video controls settings.
    /// </summary>
    public class VideoControlsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoControlsSettings"/> class.
        /// </summary>
        public VideoControlsSettings()
        {
            this.CustomSearchText1 = "The Free Dictionary";
            this.CustomSearchUrl1 = "http://www.thefreedictionary.com/{0}";
            this.CustomSearchText2 = "Wikipedia";
            this.CustomSearchUrl2 = "http://en.m.wikipedia.org/wiki?search={0}";

            this.LastActiveTab = "Translate";
            this.WaveformDrawGrid = true;
            this.WaveformAllowOverlap = false;
            this.WaveformBorderHitMs = 15;
            this.WaveformGridColor = Color.FromArgb(255, 20, 20, 18);
            this.WaveformColor = Color.GreenYellow;
            this.WaveformSelectedColor = Color.Red;
            this.WaveformBackgroundColor = Color.Black;
            this.WaveformTextColor = Color.Gray;
            this.WaveformTextSize = 9;
            this.WaveformTextBold = true;
            this.WaveformDoubleClickOnNonParagraphAction = "PlayPause";
            this.WaveformDoubleClickOnNonParagraphAction = string.Empty;
            this.WaveformMouseWheelScrollUpIsForward = true;
            this.SpectrogramAppearance = "OneColorGradient";
            this.WaveformMinimumSampleRate = 126;
            this.WaveformSeeksSilenceDurationSeconds = 0.3;
            this.WaveformSeeksSilenceMaxVolume = 10;
        }

        /// <summary>
        /// Gets or sets the custom search text 1.
        /// </summary>
        public string CustomSearchText1 { get; set; }

        /// <summary>
        /// Gets or sets the custom search text 2.
        /// </summary>
        public string CustomSearchText2 { get; set; }

        /// <summary>
        /// Gets or sets the custom search text 3.
        /// </summary>
        public string CustomSearchText3 { get; set; }

        /// <summary>
        /// Gets or sets the custom search text 4.
        /// </summary>
        public string CustomSearchText4 { get; set; }

        /// <summary>
        /// Gets or sets the custom search text 5.
        /// </summary>
        public string CustomSearchText5 { get; set; }

        /// <summary>
        /// Gets or sets the custom search text 6.
        /// </summary>
        public string CustomSearchText6 { get; set; }

        /// <summary>
        /// Gets or sets the custom search url 1.
        /// </summary>
        public string CustomSearchUrl1 { get; set; }

        /// <summary>
        /// Gets or sets the custom search url 2.
        /// </summary>
        public string CustomSearchUrl2 { get; set; }

        /// <summary>
        /// Gets or sets the custom search url 3.
        /// </summary>
        public string CustomSearchUrl3 { get; set; }

        /// <summary>
        /// Gets or sets the custom search url 4.
        /// </summary>
        public string CustomSearchUrl4 { get; set; }

        /// <summary>
        /// Gets or sets the custom search url 5.
        /// </summary>
        public string CustomSearchUrl5 { get; set; }

        /// <summary>
        /// Gets or sets the custom search url 6.
        /// </summary>
        public string CustomSearchUrl6 { get; set; }

        /// <summary>
        /// Gets or sets the last active tab.
        /// </summary>
        public string LastActiveTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform draw grid.
        /// </summary>
        public bool WaveformDrawGrid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform allow overlap.
        /// </summary>
        public bool WaveformAllowOverlap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform focus on mouse enter.
        /// </summary>
        public bool WaveformFocusOnMouseEnter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform list view focus on mouse enter.
        /// </summary>
        public bool WaveformListViewFocusOnMouseEnter { get; set; }

        /// <summary>
        /// Gets or sets the waveform border hit ms.
        /// </summary>
        public int WaveformBorderHitMs { get; set; }

        /// <summary>
        /// Gets or sets the waveform grid color.
        /// </summary>
        public Color WaveformGridColor { get; set; }

        /// <summary>
        /// Gets or sets the waveform color.
        /// </summary>
        public Color WaveformColor { get; set; }

        /// <summary>
        /// Gets or sets the waveform selected color.
        /// </summary>
        public Color WaveformSelectedColor { get; set; }

        /// <summary>
        /// Gets or sets the waveform background color.
        /// </summary>
        public Color WaveformBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the waveform text color.
        /// </summary>
        public Color WaveformTextColor { get; set; }

        /// <summary>
        /// Gets or sets the waveform text size.
        /// </summary>
        public int WaveformTextSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform text bold.
        /// </summary>
        public bool WaveformTextBold { get; set; }

        /// <summary>
        /// Gets or sets the waveform double click on non paragraph action.
        /// </summary>
        public string WaveformDoubleClickOnNonParagraphAction { get; set; }

        /// <summary>
        /// Gets or sets the waveform right click on non paragraph action.
        /// </summary>
        public string WaveformRightClickOnNonParagraphAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether waveform mouse wheel scroll up is forward.
        /// </summary>
        public bool WaveformMouseWheelScrollUpIsForward { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether generate spectrogram.
        /// </summary>
        public bool GenerateSpectrogram { get; set; }

        /// <summary>
        /// Gets or sets the spectrogram appearance.
        /// </summary>
        public string SpectrogramAppearance { get; set; }

        /// <summary>
        /// Gets or sets the waveform minimum sample rate.
        /// </summary>
        public int WaveformMinimumSampleRate { get; set; }

        /// <summary>
        /// Gets or sets the waveform seeks silence duration seconds.
        /// </summary>
        public double WaveformSeeksSilenceDurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets the waveform seeks silence max volume.
        /// </summary>
        public int WaveformSeeksSilenceMaxVolume { get; set; }
    }

    /// <summary>
    /// The vob sub ocr settings.
    /// </summary>
    public class VobSubOcrSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcrSettings"/> class.
        /// </summary>
        public VobSubOcrSettings()
        {
            this.XOrMorePixelsMakesSpace = 8;
            this.AllowDifferenceInPercent = 1.0;
            this.BlurayAllowDifferenceInPercent = 7.5;
            this.LastImageCompareFolder = "English";
            this.LastModiLanguageId = 9;
            this.LastOcrMethod = "Tesseract";
            this.UseItalicsInTesseract = true;
            this.UseMusicSymbolsInTesseract = true;
            this.RightToLeft = false;
            this.TopToBottom = true;
            this.DefaultMillisecondsForUnknownDurations = 5000;
            this.PromptForUnknownWords = true;
            this.GuessUnknownWords = true;
            this.AutoBreakSubtitleIfMoreThanTwoLines = true;
            this.ItalicFactor = 0.2;
        }

        /// <summary>
        /// Gets or sets the x or more pixels makes space.
        /// </summary>
        public int XOrMorePixelsMakesSpace { get; set; }

        /// <summary>
        /// Gets or sets the allow difference in percent.
        /// </summary>
        public double AllowDifferenceInPercent { get; set; }

        /// <summary>
        /// Gets or sets the bluray allow difference in percent.
        /// </summary>
        public double BlurayAllowDifferenceInPercent { get; set; }

        /// <summary>
        /// Gets or sets the last image compare folder.
        /// </summary>
        public string LastImageCompareFolder { get; set; }

        /// <summary>
        /// Gets or sets the last modi language id.
        /// </summary>
        public int LastModiLanguageId { get; set; }

        /// <summary>
        /// Gets or sets the last ocr method.
        /// </summary>
        public string LastOcrMethod { get; set; }

        /// <summary>
        /// Gets or sets the tesseract last language.
        /// </summary>
        public string TesseractLastLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use modi in tesseract for unknown words.
        /// </summary>
        public bool UseModiInTesseractForUnknownWords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use italics in tesseract.
        /// </summary>
        public bool UseItalicsInTesseract { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use music symbols in tesseract.
        /// </summary>
        public bool UseMusicSymbolsInTesseract { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether right to left.
        /// </summary>
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether top to bottom.
        /// </summary>
        public bool TopToBottom { get; set; }

        /// <summary>
        /// Gets or sets the default milliseconds for unknown durations.
        /// </summary>
        public int DefaultMillisecondsForUnknownDurations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prompt for unknown words.
        /// </summary>
        public bool PromptForUnknownWords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether guess unknown words.
        /// </summary>
        public bool GuessUnknownWords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto break subtitle if more than two lines.
        /// </summary>
        public bool AutoBreakSubtitleIfMoreThanTwoLines { get; set; }

        /// <summary>
        /// Gets or sets the italic factor.
        /// </summary>
        public double ItalicFactor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether line ocr draw.
        /// </summary>
        public bool LineOcrDraw { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether line ocr advanced italic.
        /// </summary>
        public bool LineOcrAdvancedItalic { get; set; }

        /// <summary>
        /// Gets or sets the line ocr last languages.
        /// </summary>
        public string LineOcrLastLanguages { get; set; }

        /// <summary>
        /// Gets or sets the line ocr last spell check.
        /// </summary>
        public string LineOcrLastSpellCheck { get; set; }

        /// <summary>
        /// Gets or sets the line ocr x or more pixels makes space.
        /// </summary>
        public int LineOcrXOrMorePixelsMakesSpace { get; set; }

        /// <summary>
        /// Gets or sets the line ocr min line height.
        /// </summary>
        public int LineOcrMinLineHeight { get; set; }

        /// <summary>
        /// Gets or sets the line ocr max line height.
        /// </summary>
        public int LineOcrMaxLineHeight { get; set; }
    }

    /// <summary>
    /// The multiple search and replace setting.
    /// </summary>
    public class MultipleSearchAndReplaceSetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the find what.
        /// </summary>
        public string FindWhat { get; set; }

        /// <summary>
        /// Gets or sets the replace with.
        /// </summary>
        public string ReplaceWith { get; set; }

        /// <summary>
        /// Gets or sets the search type.
        /// </summary>
        public string SearchType { get; set; }
    }

    /// <summary>
    /// The network settings.
    /// </summary>
    public class NetworkSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSettings"/> class.
        /// </summary>
        public NetworkSettings()
        {
            this.UserName = string.Empty;
            this.SessionKey = "DemoSession"; // TODO: Leave blank or use guid
            this.WebServiceUrl = "http://www.nikse.dk/se/SeService.asmx";
            this.PollIntervalSeconds = 5;
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the web service url.
        /// </summary>
        public string WebServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the session key.
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// Gets or sets the poll interval seconds.
        /// </summary>
        public int PollIntervalSeconds { get; set; }
    }

    /// <summary>
    /// The shortcuts.
    /// </summary>
    public class Shortcuts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shortcuts"/> class.
        /// </summary>
        public Shortcuts()
        {
            this.GeneralGoToFirstSelectedLine = "Control+L";
            this.GeneralMergeSelectedLines = "Control+Shift+M";
            this.GeneralToggleTranslationMode = "Control+Shift+O";
            this.GeneralSwitchOriginalAndTranslation = "Control+Alt+O";
            this.GeneralMergeOriginalAndTranslation = "Control+Alt+Shift+M";
            this.GeneralGoToNextSubtitle = "Shift+Return";
            this.GeneralGoToPrevSubtitle = string.Empty;
            this.GeneralGoToStartOfCurrentSubtitle = string.Empty;
            this.GeneralGoToEndOfCurrentSubtitle = string.Empty;
            this.MainFileNew = "Control+N";
            this.MainFileOpen = "Control+O";
            this.MainFileSave = "Control+S";
            this.MainFileSaveOriginal = string.Empty;
            this.MainFileSaveOriginalAs = string.Empty;
            this.MainFileSaveAs = string.Empty;
            this.MainFileSaveAll = string.Empty;
            this.MainFileExportEbu = string.Empty;
            this.MainEditUndo = "Control+Z";
            this.MainEditRedo = "Control+Y";
            this.MainEditFind = "Control+F";
            this.MainEditFindNext = "F3";
            this.MainEditReplace = "Control+H";
            this.MainEditMultipleReplace = "Control+Alt+M";
            this.MainEditGoToLineNumber = "Control+G";
            this.MainEditRightToLeft = "Control+Shift+Alt+R";
            this.MainEditInverseSelection = "Control+Shift+I";
            this.MainToolsFixCommonErrors = "Control+Shift+F";
            this.MainToolsFixCommonErrorsPreview = "Control+P";
            this.MainToolsMergeShortLines = string.Empty;
            this.MainToolsSplitLongLines = string.Empty;
            this.MainToolsRenumber = "Control+Shift+N";
            this.MainToolsRemoveTextForHI = "Control+Shift+H";
            this.MainToolsChangeCasing = "Control+Shift+C";
            this.MainVideoPlayPauseToggle = "Control+P";
            this.MainVideoPause = "Control+Alt+P";
            this.MainVideoShowHideVideo = "Control+Q";
            this.MainVideo1FrameLeft = string.Empty;
            this.MainVideo1FrameRight = string.Empty;
            this.MainVideo100MsLeft = string.Empty;
            this.MainVideo100MsRight = string.Empty;
            this.MainVideo500MsLeft = "Alt+Left";
            this.MainVideo500MsRight = "Alt+Right";
            this.MainVideo1000MsLeft = string.Empty;
            this.MainVideo1000MsRight = string.Empty;
            this.MainVideoFullscreen = "Alt+Return";
            this.MainSpellCheck = "Control+Shift+S";
            this.MainSpellCheckFindDoubleWords = "Control+Shift+D";
            this.MainSpellCheckAddWordToNames = "Control+Shift+L";
            this.MainSynchronizationAdjustTimes = "Control+Shift+A";
            this.MainSynchronizationVisualSync = "Control+Shift+V";
            this.MainSynchronizationPointSync = "Control+Shift+P";
            this.MainSynchronizationChangeFrameRate = string.Empty;
            this.MainListViewItalic = "Control+I";
            this.MainEditReverseStartAndEndingForRTL = string.Empty;
            this.MainTextBoxItalic = "Control+I";
            this.MainTextBoxSplitAtCursor = "Control+Alt+V";
            this.MainToolsAutoDuration = string.Empty;
            this.MainTextBoxSelectionToLower = "Control+U";
            this.MainTextBoxSelectionToUpper = "Control+Shift+U";
            this.MainTextBoxToggleAutoDuration = string.Empty;
            this.MainToolsBeamer = "Control+Shift+Alt+B";
            this.MainCreateInsertSubAtVideoPos = string.Empty;
            this.MainCreatePlayFromJustBefore = string.Empty;
            this.MainCreateSetStart = string.Empty;
            this.MainCreateSetEnd = string.Empty;
            this.MainCreateSetEndAddNewAndGoToNew = string.Empty;
            this.MainCreateStartDownEndUp = string.Empty;
            this.MainAdjustSetStartAndOffsetTheRest = "Control+Space";
            this.MainAdjustSetEndAndOffsetTheRest = string.Empty;
            this.MainAdjustSetEndAndOffsetTheRestAndGoToNext = string.Empty;
            this.MainAdjustSetEndAndGotoNext = string.Empty;
            this.MainAdjustViaEndAutoStartAndGoToNext = string.Empty;
            this.MainAdjustSetStartAutoDurationAndGoToNext = string.Empty;
            this.MainAdjustSetEndNextStartAndGoToNext = string.Empty;
            this.MainAdjustStartDownEndUpAndGoToNext = string.Empty;
            this.MainAdjustSetStart = string.Empty;
            this.MainAdjustSetStartKeepDuration = string.Empty;
            this.MainAdjustSetEnd = string.Empty;
            this.MainAdjustSelected100MsForward = string.Empty;
            this.MainAdjustSelected100MsBack = string.Empty;
            this.MainInsertAfter = "Alt+Insert";
            this.MainWaveformInsertAtCurrentPosition = "Insert";
            this.MainInsertBefore = "Control+Shift+Insert";
            this.MainTextBoxInsertAfter = "Alt+Insert";
            this.MainTextBoxAutoBreak = "Control+R";
            this.MainTextBoxUnbreak = string.Empty;
            this.MainMergeDialog = string.Empty;
            this.WaveformVerticalZoom = "Shift+Add";
            this.WaveformVerticalZoomOut = "Shift+Subtract";
            this.WaveformPlaySelection = string.Empty;
            this.GeneralPlayFirstSelected = string.Empty;
            this.WaveformSearchSilenceForward = string.Empty;
            this.WaveformSearchSilenceBack = string.Empty;
            this.WaveformAddTextHere = string.Empty;
        }

        /// <summary>
        /// Gets or sets the general go to first selected line.
        /// </summary>
        public string GeneralGoToFirstSelectedLine { get; set; }

        /// <summary>
        /// Gets or sets the general go to next empty line.
        /// </summary>
        public string GeneralGoToNextEmptyLine { get; set; }

        /// <summary>
        /// Gets or sets the general merge selected lines.
        /// </summary>
        public string GeneralMergeSelectedLines { get; set; }

        /// <summary>
        /// Gets or sets the general merge selected lines only first text.
        /// </summary>
        public string GeneralMergeSelectedLinesOnlyFirstText { get; set; }

        /// <summary>
        /// Gets or sets the general toggle translation mode.
        /// </summary>
        public string GeneralToggleTranslationMode { get; set; }

        /// <summary>
        /// Gets or sets the general switch original and translation.
        /// </summary>
        public string GeneralSwitchOriginalAndTranslation { get; set; }

        /// <summary>
        /// Gets or sets the general merge original and translation.
        /// </summary>
        public string GeneralMergeOriginalAndTranslation { get; set; }

        /// <summary>
        /// Gets or sets the general go to next subtitle.
        /// </summary>
        public string GeneralGoToNextSubtitle { get; set; }

        /// <summary>
        /// Gets or sets the general go to prev subtitle.
        /// </summary>
        public string GeneralGoToPrevSubtitle { get; set; }

        /// <summary>
        /// Gets or sets the general go to start of current subtitle.
        /// </summary>
        public string GeneralGoToStartOfCurrentSubtitle { get; set; }

        /// <summary>
        /// Gets or sets the general go to end of current subtitle.
        /// </summary>
        public string GeneralGoToEndOfCurrentSubtitle { get; set; }

        /// <summary>
        /// Gets or sets the general play first selected.
        /// </summary>
        public string GeneralPlayFirstSelected { get; set; }

        /// <summary>
        /// Gets or sets the main file new.
        /// </summary>
        public string MainFileNew { get; set; }

        /// <summary>
        /// Gets or sets the main file open.
        /// </summary>
        public string MainFileOpen { get; set; }

        /// <summary>
        /// Gets or sets the main file open keep video.
        /// </summary>
        public string MainFileOpenKeepVideo { get; set; }

        /// <summary>
        /// Gets or sets the main file save.
        /// </summary>
        public string MainFileSave { get; set; }

        /// <summary>
        /// Gets or sets the main file save original.
        /// </summary>
        public string MainFileSaveOriginal { get; set; }

        /// <summary>
        /// Gets or sets the main file save original as.
        /// </summary>
        public string MainFileSaveOriginalAs { get; set; }

        /// <summary>
        /// Gets or sets the main file save as.
        /// </summary>
        public string MainFileSaveAs { get; set; }

        /// <summary>
        /// Gets or sets the main file save all.
        /// </summary>
        public string MainFileSaveAll { get; set; }

        /// <summary>
        /// Gets or sets the main file export ebu.
        /// </summary>
        public string MainFileExportEbu { get; set; }

        /// <summary>
        /// Gets or sets the main edit undo.
        /// </summary>
        public string MainEditUndo { get; set; }

        /// <summary>
        /// Gets or sets the main edit redo.
        /// </summary>
        public string MainEditRedo { get; set; }

        /// <summary>
        /// Gets or sets the main edit find.
        /// </summary>
        public string MainEditFind { get; set; }

        /// <summary>
        /// Gets or sets the main edit find next.
        /// </summary>
        public string MainEditFindNext { get; set; }

        /// <summary>
        /// Gets or sets the main edit replace.
        /// </summary>
        public string MainEditReplace { get; set; }

        /// <summary>
        /// Gets or sets the main edit multiple replace.
        /// </summary>
        public string MainEditMultipleReplace { get; set; }

        /// <summary>
        /// Gets or sets the main edit go to line number.
        /// </summary>
        public string MainEditGoToLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the main edit right to left.
        /// </summary>
        public string MainEditRightToLeft { get; set; }

        /// <summary>
        /// Gets or sets the main edit reverse start and ending for rtl.
        /// </summary>
        public string MainEditReverseStartAndEndingForRTL { get; set; }

        /// <summary>
        /// Gets or sets the main edit toggle translation original in previews.
        /// </summary>
        public string MainEditToggleTranslationOriginalInPreviews { get; set; }

        /// <summary>
        /// Gets or sets the main edit inverse selection.
        /// </summary>
        public string MainEditInverseSelection { get; set; }

        /// <summary>
        /// Gets or sets the main edit modify selection.
        /// </summary>
        public string MainEditModifySelection { get; set; }

        /// <summary>
        /// Gets or sets the main tools fix common errors.
        /// </summary>
        public string MainToolsFixCommonErrors { get; set; }

        /// <summary>
        /// Gets or sets the main tools fix common errors preview.
        /// </summary>
        public string MainToolsFixCommonErrorsPreview { get; set; }

        /// <summary>
        /// Gets or sets the main tools merge short lines.
        /// </summary>
        public string MainToolsMergeShortLines { get; set; }

        /// <summary>
        /// Gets or sets the main tools split long lines.
        /// </summary>
        public string MainToolsSplitLongLines { get; set; }

        /// <summary>
        /// Gets or sets the main tools renumber.
        /// </summary>
        public string MainToolsRenumber { get; set; }

        /// <summary>
        /// Gets or sets the main tools remove text for hi.
        /// </summary>
        public string MainToolsRemoveTextForHI { get; set; }

        /// <summary>
        /// Gets or sets the main tools change casing.
        /// </summary>
        public string MainToolsChangeCasing { get; set; }

        /// <summary>
        /// Gets or sets the main tools auto duration.
        /// </summary>
        public string MainToolsAutoDuration { get; set; }

        /// <summary>
        /// Gets or sets the main tools batch convert.
        /// </summary>
        public string MainToolsBatchConvert { get; set; }

        /// <summary>
        /// Gets or sets the main tools beamer.
        /// </summary>
        public string MainToolsBeamer { get; set; }

        /// <summary>
        /// Gets or sets the main video pause.
        /// </summary>
        public string MainVideoPause { get; set; }

        /// <summary>
        /// Gets or sets the main video play pause toggle.
        /// </summary>
        public string MainVideoPlayPauseToggle { get; set; }

        /// <summary>
        /// Gets or sets the main video show hide video.
        /// </summary>
        public string MainVideoShowHideVideo { get; set; }

        /// <summary>
        /// Gets or sets the main video toggle video controls.
        /// </summary>
        public string MainVideoToggleVideoControls { get; set; }

        /// <summary>
        /// Gets or sets the main video 1 frame left.
        /// </summary>
        public string MainVideo1FrameLeft { get; set; }

        /// <summary>
        /// Gets or sets the main video 1 frame right.
        /// </summary>
        public string MainVideo1FrameRight { get; set; }

        /// <summary>
        /// Gets or sets the main video 100 ms left.
        /// </summary>
        public string MainVideo100MsLeft { get; set; }

        /// <summary>
        /// Gets or sets the main video 100 ms right.
        /// </summary>
        public string MainVideo100MsRight { get; set; }

        /// <summary>
        /// Gets or sets the main video 500 ms left.
        /// </summary>
        public string MainVideo500MsLeft { get; set; }

        /// <summary>
        /// Gets or sets the main video 500 ms right.
        /// </summary>
        public string MainVideo500MsRight { get; set; }

        /// <summary>
        /// Gets or sets the main video 1000 ms left.
        /// </summary>
        public string MainVideo1000MsLeft { get; set; }

        /// <summary>
        /// Gets or sets the main video 1000 ms right.
        /// </summary>
        public string MainVideo1000MsRight { get; set; }

        /// <summary>
        /// Gets or sets the main video fullscreen.
        /// </summary>
        public string MainVideoFullscreen { get; set; }

        /// <summary>
        /// Gets or sets the main spell check.
        /// </summary>
        public string MainSpellCheck { get; set; }

        /// <summary>
        /// Gets or sets the main spell check find double words.
        /// </summary>
        public string MainSpellCheckFindDoubleWords { get; set; }

        /// <summary>
        /// Gets or sets the main spell check add word to names.
        /// </summary>
        public string MainSpellCheckAddWordToNames { get; set; }

        /// <summary>
        /// Gets or sets the main synchronization adjust times.
        /// </summary>
        public string MainSynchronizationAdjustTimes { get; set; }

        /// <summary>
        /// Gets or sets the main synchronization visual sync.
        /// </summary>
        public string MainSynchronizationVisualSync { get; set; }

        /// <summary>
        /// Gets or sets the main synchronization point sync.
        /// </summary>
        public string MainSynchronizationPointSync { get; set; }

        /// <summary>
        /// Gets or sets the main synchronization change frame rate.
        /// </summary>
        public string MainSynchronizationChangeFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the main list view italic.
        /// </summary>
        public string MainListViewItalic { get; set; }

        /// <summary>
        /// Gets or sets the main list view toggle dashes.
        /// </summary>
        public string MainListViewToggleDashes { get; set; }

        /// <summary>
        /// Gets or sets the main list view alignment.
        /// </summary>
        public string MainListViewAlignment { get; set; }

        /// <summary>
        /// Gets or sets the main list view copy text.
        /// </summary>
        public string MainListViewCopyText { get; set; }

        /// <summary>
        /// Gets or sets the main list view copy text from original to current.
        /// </summary>
        public string MainListViewCopyTextFromOriginalToCurrent { get; set; }

        /// <summary>
        /// Gets or sets the main list view auto duration.
        /// </summary>
        public string MainListViewAutoDuration { get; set; }

        /// <summary>
        /// Gets or sets the main list view column delete text.
        /// </summary>
        public string MainListViewColumnDeleteText { get; set; }

        /// <summary>
        /// Gets or sets the main list view column insert text.
        /// </summary>
        public string MainListViewColumnInsertText { get; set; }

        /// <summary>
        /// Gets or sets the main list view column paste.
        /// </summary>
        public string MainListViewColumnPaste { get; set; }

        /// <summary>
        /// Gets or sets the main list view focus waveform.
        /// </summary>
        public string MainListViewFocusWaveform { get; set; }

        /// <summary>
        /// Gets or sets the main list view go to next error.
        /// </summary>
        public string MainListViewGoToNextError { get; set; }

        /// <summary>
        /// Gets or sets the main text box italic.
        /// </summary>
        public string MainTextBoxItalic { get; set; }

        /// <summary>
        /// Gets or sets the main text box split at cursor.
        /// </summary>
        public string MainTextBoxSplitAtCursor { get; set; }

        /// <summary>
        /// Gets or sets the main text box move last word down.
        /// </summary>
        public string MainTextBoxMoveLastWordDown { get; set; }

        /// <summary>
        /// Gets or sets the main text box move first word from next up.
        /// </summary>
        public string MainTextBoxMoveFirstWordFromNextUp { get; set; }

        /// <summary>
        /// Gets or sets the main text box selection to lower.
        /// </summary>
        public string MainTextBoxSelectionToLower { get; set; }

        /// <summary>
        /// Gets or sets the main text box selection to upper.
        /// </summary>
        public string MainTextBoxSelectionToUpper { get; set; }

        /// <summary>
        /// Gets or sets the main text box toggle auto duration.
        /// </summary>
        public string MainTextBoxToggleAutoDuration { get; set; }

        /// <summary>
        /// Gets or sets the main create insert sub at video pos.
        /// </summary>
        public string MainCreateInsertSubAtVideoPos { get; set; }

        /// <summary>
        /// Gets or sets the main create play from just before.
        /// </summary>
        public string MainCreatePlayFromJustBefore { get; set; }

        /// <summary>
        /// Gets or sets the main create set start.
        /// </summary>
        public string MainCreateSetStart { get; set; }

        /// <summary>
        /// Gets or sets the main create set end.
        /// </summary>
        public string MainCreateSetEnd { get; set; }

        /// <summary>
        /// Gets or sets the main create set end add new and go to new.
        /// </summary>
        public string MainCreateSetEndAddNewAndGoToNew { get; set; }

        /// <summary>
        /// Gets or sets the main create start down end up.
        /// </summary>
        public string MainCreateStartDownEndUp { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set start and offset the rest.
        /// </summary>
        public string MainAdjustSetStartAndOffsetTheRest { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set end and offset the rest.
        /// </summary>
        public string MainAdjustSetEndAndOffsetTheRest { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set end and offset the rest and go to next.
        /// </summary>
        public string MainAdjustSetEndAndOffsetTheRestAndGoToNext { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set end and goto next.
        /// </summary>
        public string MainAdjustSetEndAndGotoNext { get; set; }

        /// <summary>
        /// Gets or sets the main adjust via end auto start and go to next.
        /// </summary>
        public string MainAdjustViaEndAutoStartAndGoToNext { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set start auto duration and go to next.
        /// </summary>
        public string MainAdjustSetStartAutoDurationAndGoToNext { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set end next start and go to next.
        /// </summary>
        public string MainAdjustSetEndNextStartAndGoToNext { get; set; }

        /// <summary>
        /// Gets or sets the main adjust start down end up and go to next.
        /// </summary>
        public string MainAdjustStartDownEndUpAndGoToNext { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set start.
        /// </summary>
        public string MainAdjustSetStart { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set start keep duration.
        /// </summary>
        public string MainAdjustSetStartKeepDuration { get; set; }

        /// <summary>
        /// Gets or sets the main adjust set end.
        /// </summary>
        public string MainAdjustSetEnd { get; set; }

        /// <summary>
        /// Gets or sets the main adjust selected 100 ms forward.
        /// </summary>
        public string MainAdjustSelected100MsForward { get; set; }

        /// <summary>
        /// Gets or sets the main adjust selected 100 ms back.
        /// </summary>
        public string MainAdjustSelected100MsBack { get; set; }

        /// <summary>
        /// Gets or sets the main insert after.
        /// </summary>
        public string MainInsertAfter { get; set; }

        /// <summary>
        /// Gets or sets the main text box insert after.
        /// </summary>
        public string MainTextBoxInsertAfter { get; set; }

        /// <summary>
        /// Gets or sets the main text box auto break.
        /// </summary>
        public string MainTextBoxAutoBreak { get; set; }

        /// <summary>
        /// Gets or sets the main text box unbreak.
        /// </summary>
        public string MainTextBoxUnbreak { get; set; }

        /// <summary>
        /// Gets or sets the main waveform insert at current position.
        /// </summary>
        public string MainWaveformInsertAtCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the main insert before.
        /// </summary>
        public string MainInsertBefore { get; set; }

        /// <summary>
        /// Gets or sets the main merge dialog.
        /// </summary>
        public string MainMergeDialog { get; set; }

        /// <summary>
        /// Gets or sets the main toggle focus.
        /// </summary>
        public string MainToggleFocus { get; set; }

        /// <summary>
        /// Gets or sets the waveform vertical zoom.
        /// </summary>
        public string WaveformVerticalZoom { get; set; }

        /// <summary>
        /// Gets or sets the waveform vertical zoom out.
        /// </summary>
        public string WaveformVerticalZoomOut { get; set; }

        /// <summary>
        /// Gets or sets the waveform zoom in.
        /// </summary>
        public string WaveformZoomIn { get; set; }

        /// <summary>
        /// Gets or sets the waveform zoom out.
        /// </summary>
        public string WaveformZoomOut { get; set; }

        /// <summary>
        /// Gets or sets the waveform play selection.
        /// </summary>
        public string WaveformPlaySelection { get; set; }

        /// <summary>
        /// Gets or sets the waveform search silence forward.
        /// </summary>
        public string WaveformSearchSilenceForward { get; set; }

        /// <summary>
        /// Gets or sets the waveform search silence back.
        /// </summary>
        public string WaveformSearchSilenceBack { get; set; }

        /// <summary>
        /// Gets or sets the waveform add text here.
        /// </summary>
        public string WaveformAddTextHere { get; set; }

        /// <summary>
        /// Gets or sets the waveform focus list view.
        /// </summary>
        public string WaveformFocusListView { get; set; }

        /// <summary>
        /// Gets or sets the main translate custom search 1.
        /// </summary>
        public string MainTranslateCustomSearch1 { get; set; }

        /// <summary>
        /// Gets or sets the main translate custom search 2.
        /// </summary>
        public string MainTranslateCustomSearch2 { get; set; }

        /// <summary>
        /// Gets or sets the main translate custom search 3.
        /// </summary>
        public string MainTranslateCustomSearch3 { get; set; }

        /// <summary>
        /// Gets or sets the main translate custom search 4.
        /// </summary>
        public string MainTranslateCustomSearch4 { get; set; }

        /// <summary>
        /// Gets or sets the main translate custom search 5.
        /// </summary>
        public string MainTranslateCustomSearch5 { get; set; }

        /// <summary>
        /// Gets or sets the main translate custom search 6.
        /// </summary>
        public string MainTranslateCustomSearch6 { get; set; }
    }

    /// <summary>
    /// The remove text for hearing impaired settings.
    /// </summary>
    public class RemoveTextForHearingImpairedSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveTextForHearingImpairedSettings"/> class.
        /// </summary>
        public RemoveTextForHearingImpairedSettings()
        {
            this.RemoveTextBetweenBrackets = true;
            this.RemoveTextBetweenParentheses = true;
            this.RemoveTextBetweenCurlyBrackets = true;
            this.RemoveTextBetweenQuestionMarks = true;
            this.RemoveTextBetweenCustom = false;
            this.RemoveTextBetweenCustomBefore = "¶";
            this.RemoveTextBetweenCustomAfter = "¶";
            this.RemoveTextBeforeColon = true;
            this.RemoveTextBeforeColonOnlyIfUppercase = true;
            this.RemoveIfContainsText = "¶";
        }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between brackets.
        /// </summary>
        public bool RemoveTextBetweenBrackets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between parentheses.
        /// </summary>
        public bool RemoveTextBetweenParentheses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between curly brackets.
        /// </summary>
        public bool RemoveTextBetweenCurlyBrackets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between question marks.
        /// </summary>
        public bool RemoveTextBetweenQuestionMarks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between custom.
        /// </summary>
        public bool RemoveTextBetweenCustom { get; set; }

        /// <summary>
        /// Gets or sets the remove text between custom before.
        /// </summary>
        public string RemoveTextBetweenCustomBefore { get; set; }

        /// <summary>
        /// Gets or sets the remove text between custom after.
        /// </summary>
        public string RemoveTextBetweenCustomAfter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between only seperate lines.
        /// </summary>
        public bool RemoveTextBetweenOnlySeperateLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text before colon.
        /// </summary>
        public bool RemoveTextBeforeColon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text before colon only if uppercase.
        /// </summary>
        public bool RemoveTextBeforeColonOnlyIfUppercase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text before colon only on separate line.
        /// </summary>
        public bool RemoveTextBeforeColonOnlyOnSeparateLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove interjections.
        /// </summary>
        public bool RemoveInterjections { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove if contains.
        /// </summary>
        public bool RemoveIfContains { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove if all uppercase.
        /// </summary>
        public bool RemoveIfAllUppercase { get; set; }

        /// <summary>
        /// Gets or sets the remove if contains text.
        /// </summary>
        public string RemoveIfContainsText { get; set; }
    }

    /// <summary>
    /// The subtitle beaming.
    /// </summary>
    public class SubtitleBeaming
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtitleBeaming"/> class.
        /// </summary>
        public SubtitleBeaming()
        {
            this.FontName = "Verdana";
            this.FontSize = 30;
            this.FontColor = Color.White;
            this.BorderColor = Color.DarkGray;
            this.BorderWidth = 2;
        }

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font color.
        /// </summary>
        public Color FontColor { get; set; }

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the border width.
        /// </summary>
        public int BorderWidth { get; set; }
    }

    /// <summary>
    /// The settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Settings"/> class from being created.
        /// </summary>
        private Settings()
        {
            this.RecentFiles = new RecentFilesSettings();
            this.General = new GeneralSettings();
            this.Tools = new ToolsSettings();
            this.WordLists = new WordListSettings();
            this.SubtitleSettings = new SubtitleSettings();
            this.Proxy = new ProxySettings();
            this.CommonErrors = new FixCommonErrorsSettings();
            this.VobSubOcr = new VobSubOcrSettings();
            this.VideoControls = new VideoControlsSettings();
            this.NetworkSettings = new NetworkSettings();
            this.MultipleSearchAndReplaceList = new List<MultipleSearchAndReplaceSetting>();
            this.Language = new Language();
            this.Shortcuts = new Shortcuts();
            this.RemoveTextForHearingImpaired = new RemoveTextForHearingImpairedSettings();
            this.SubtitleBeaming = new SubtitleBeaming();
        }

        /// <summary>
        /// Gets or sets the recent files.
        /// </summary>
        public RecentFilesSettings RecentFiles { get; set; }

        /// <summary>
        /// Gets or sets the general.
        /// </summary>
        public GeneralSettings General { get; set; }

        /// <summary>
        /// Gets or sets the tools.
        /// </summary>
        public ToolsSettings Tools { get; set; }

        /// <summary>
        /// Gets or sets the subtitle settings.
        /// </summary>
        public SubtitleSettings SubtitleSettings { get; set; }

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        public ProxySettings Proxy { get; set; }

        /// <summary>
        /// Gets or sets the word lists.
        /// </summary>
        public WordListSettings WordLists { get; set; }

        /// <summary>
        /// Gets or sets the common errors.
        /// </summary>
        public FixCommonErrorsSettings CommonErrors { get; set; }

        /// <summary>
        /// Gets or sets the vob sub ocr.
        /// </summary>
        public VobSubOcrSettings VobSubOcr { get; set; }

        /// <summary>
        /// Gets or sets the video controls.
        /// </summary>
        public VideoControlsSettings VideoControls { get; set; }

        /// <summary>
        /// Gets or sets the network settings.
        /// </summary>
        public NetworkSettings NetworkSettings { get; set; }

        /// <summary>
        /// Gets or sets the shortcuts.
        /// </summary>
        public Shortcuts Shortcuts { get; set; }

        /// <summary>
        /// Gets or sets the remove text for hearing impaired.
        /// </summary>
        public RemoveTextForHearingImpairedSettings RemoveTextForHearingImpaired { get; set; }

        /// <summary>
        /// Gets or sets the subtitle beaming.
        /// </summary>
        public SubtitleBeaming SubtitleBeaming { get; set; }

        /// <summary>
        /// Gets or sets the multiple search and replace list.
        /// </summary>
        [XmlArrayItem("MultipleSearchAndReplaceItem")]
        public List<MultipleSearchAndReplaceSetting> MultipleSearchAndReplaceList { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [XmlIgnore]
        public Language Language { get; set; }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            // this is too slow: Serialize(Configuration.SettingsFileName, this);
            CustomSerialize(Configuration.SettingsFileName, this);
        }

        // private static void Serialize(string fileName, Settings settings)
        // {
        // var s = new XmlSerializer(typeof(Settings));
        // TextWriter w = new StreamWriter(fileName);
        // s.Serialize(w, settings);
        // w.Close();
        // }

        /// <summary>
        /// The get settings.
        /// </summary>
        /// <returns>
        /// The <see cref="Settings"/>.
        /// </returns>
        public static Settings GetSettings()
        {
            var settings = new Settings();
            var settingsFileName = Configuration.SettingsFileName;
            if (File.Exists(settingsFileName))
            {
                try
                {
                    // too slow... :(  - settings = Deserialize(settingsFileName); // 688 msecs
                    settings = CustomDeserialize(settingsFileName); // 15 msecs

                    if (settings.General.AutoConvertToUtf8)
                    {
                        settings.General.DefaultEncoding = Encoding.UTF8.EncodingName;
                    }
                }
                catch
                {
                    settings = new Settings();
                }

                if (!string.IsNullOrEmpty(settings.General.ListViewLineSeparatorString))
                {
                    settings.General.ListViewLineSeparatorString = settings.General.ListViewLineSeparatorString.Replace("\n", string.Empty).Replace("\r", string.Empty);
                }

                if (string.IsNullOrWhiteSpace(settings.General.ListViewLineSeparatorString))
                {
                    settings.General.ListViewLineSeparatorString = "<br />";
                }

                if (settings.Shortcuts.GeneralToggleTranslationMode == "Control+U" && settings.Shortcuts.MainTextBoxSelectionToLower == "Control+U")
                {
                    settings.Shortcuts.GeneralToggleTranslationMode = "Control+Shift+O";
                    settings.Shortcuts.GeneralSwitchOriginalAndTranslation = "Control+Alt+O";
                }
            }

            return settings;
        }

        // private static Settings Deserialize(string fileName)
        // {
        // var r = new StreamReader(fileName);
        // var s = new XmlSerializer(typeof(Settings));
        // var settings = (Settings)s.Deserialize(r);
        // r.Close();

        // if (settings.RecentFiles == null)
        // settings.RecentFiles = new RecentFilesSettings();
        // if (settings.General == null)
        // settings.General = new GeneralSettings();
        // if (settings.SsaStyle == null)
        // settings.SsaStyle = new SsaStyleSettings();
        // if (settings.CommonErrors == null)
        // settings.CommonErrors = new FixCommonErrorsSettings();
        // if (settings.VideoControls == null)
        // settings.VideoControls = new VideoControlsSettings();
        // if (settings.VobSubOcr == null)
        // settings.VobSubOcr = new VobSubOcrSettings();
        // if (settings.MultipleSearchAndReplaceList == null)
        // settings.MultipleSearchAndReplaceList = new List<MultipleSearchAndReplaceSetting>();
        // if (settings.NetworkSettings == null)
        // settings.NetworkSettings = new NetworkSettings();
        // if (settings.Shortcuts == null)
        // settings.Shortcuts = new Shortcuts();

        // return settings;
        // }

        /// <summary>
        /// A faster serializer than xml serializer... which is insanely slow (first time)!!!!
        /// This method is auto-generated with XmlSerializerGenerator
        /// </summary>
        /// <param name="fileName">
        /// File name of xml settings file to load
        /// </param>
        /// <returns>
        /// Newly loaded settings
        /// </returns>
        private static Settings CustomDeserialize(string fileName)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };

            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            doc.Load(stream);
            stream.Close();

            var settings = new Settings();

            settings.RecentFiles = new RecentFilesSettings();
            XmlNode node = doc.DocumentElement.SelectSingleNode("RecentFiles");
            foreach (XmlNode listNode in node.SelectNodes("FileNames/FileName"))
            {
                string firstVisibleIndex = "-1";
                if (listNode.Attributes["FirstVisibleIndex"] != null)
                {
                    firstVisibleIndex = listNode.Attributes["FirstVisibleIndex"].Value;
                }

                string firstSelectedIndex = "-1";
                if (listNode.Attributes["FirstSelectedIndex"] != null)
                {
                    firstSelectedIndex = listNode.Attributes["FirstSelectedIndex"].Value;
                }

                string videoFileName = null;
                if (listNode.Attributes["VideoFileName"] != null)
                {
                    videoFileName = listNode.Attributes["VideoFileName"].Value;
                }

                string originalFileName = null;
                if (listNode.Attributes["OriginalFileName"] != null)
                {
                    originalFileName = listNode.Attributes["OriginalFileName"].Value;
                }

                settings.RecentFiles.Files.Add(new RecentFileEntry { FileName = listNode.InnerText, FirstVisibleIndex = int.Parse(firstVisibleIndex), FirstSelectedIndex = int.Parse(firstSelectedIndex), VideoFileName = videoFileName, OriginalFileName = originalFileName });
            }

            settings.General = new GeneralSettings();
            node = doc.DocumentElement.SelectSingleNode("General");
            XmlNode subNode = node.SelectSingleNode("ShowToolbarNew");
            if (subNode != null)
            {
                settings.General.ShowToolbarNew = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarOpen");
            if (subNode != null)
            {
                settings.General.ShowToolbarOpen = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarSave");
            if (subNode != null)
            {
                settings.General.ShowToolbarSave = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarSaveAs");
            if (subNode != null)
            {
                settings.General.ShowToolbarSaveAs = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarFind");
            if (subNode != null)
            {
                settings.General.ShowToolbarFind = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarReplace");
            if (subNode != null)
            {
                settings.General.ShowToolbarReplace = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarFixCommonErrors");
            if (subNode != null)
            {
                settings.General.ShowToolbarFixCommonErrors = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarVisualSync");
            if (subNode != null)
            {
                settings.General.ShowToolbarVisualSync = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarSpellCheck");
            if (subNode != null)
            {
                settings.General.ShowToolbarSpellCheck = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarSettings");
            if (subNode != null)
            {
                settings.General.ShowToolbarSettings = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowToolbarHelp");
            if (subNode != null)
            {
                settings.General.ShowToolbarHelp = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowFrameRate");
            if (subNode != null)
            {
                settings.General.ShowFrameRate = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowVideoPlayer");
            if (subNode != null)
            {
                settings.General.ShowVideoPlayer = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowAudioVisualizer");
            if (subNode != null)
            {
                settings.General.ShowAudioVisualizer = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowWaveform");
            if (subNode != null)
            {
                settings.General.ShowWaveform = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowSpectrogram");
            if (subNode != null)
            {
                settings.General.ShowSpectrogram = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("DefaultFrameRate");
            if (subNode != null)
            {
                settings.General.DefaultFrameRate = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
                if (settings.General.DefaultFrameRate > 23975)
                {
                    settings.General.DefaultFrameRate = 23.976;
                }

                settings.General.CurrentFrameRate = settings.General.DefaultFrameRate;
            }

            subNode = node.SelectSingleNode("DefaultSubtitleFormat");
            if (subNode != null)
            {
                settings.General.DefaultSubtitleFormat = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("DefaultEncoding");
            if (subNode != null)
            {
                settings.General.DefaultEncoding = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("AutoConvertToUtf8");
            if (subNode != null)
            {
                settings.General.AutoConvertToUtf8 = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoGuessAnsiEncoding");
            if (subNode != null)
            {
                settings.General.AutoGuessAnsiEncoding = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("_subtitleFontName");
            if (subNode != null)
            {
                settings.General.SubtitleFontName = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("SubtitleFontSize");
            if (subNode != null)
            {
                settings.General.SubtitleFontSize = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SubtitleFontBold");
            if (subNode != null)
            {
                settings.General.SubtitleFontBold = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SubtitleFontColor");
            if (subNode != null)
            {
                settings.General.SubtitleFontColor = Color.FromArgb(Convert.ToInt32(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("SubtitleBackgroundColor");
            if (subNode != null)
            {
                settings.General.SubtitleBackgroundColor = Color.FromArgb(Convert.ToInt32(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("CenterSubtitleInTextBox");
            if (subNode != null)
            {
                settings.General.CenterSubtitleInTextBox = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowRecentFiles");
            if (subNode != null)
            {
                settings.General.ShowRecentFiles = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("RememberSelectedLine");
            if (subNode != null)
            {
                settings.General.RememberSelectedLine = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartLoadLastFile");
            if (subNode != null)
            {
                settings.General.StartLoadLastFile = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartRememberPositionAndSize");
            if (subNode != null)
            {
                settings.General.StartRememberPositionAndSize = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartPosition");
            if (subNode != null)
            {
                settings.General.StartPosition = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("StartSize");
            if (subNode != null)
            {
                settings.General.StartSize = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("SplitContainerMainSplitterDistance");
            if (subNode != null)
            {
                settings.General.SplitContainerMainSplitterDistance = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SplitContainer1SplitterDistance");
            if (subNode != null)
            {
                settings.General.SplitContainer1SplitterDistance = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SplitContainerListViewAndTextSplitterDistance");
            if (subNode != null)
            {
                settings.General.SplitContainerListViewAndTextSplitterDistance = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartInSourceView");
            if (subNode != null)
            {
                settings.General.StartInSourceView = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("RemoveBlankLinesWhenOpening");
            if (subNode != null)
            {
                settings.General.RemoveBlankLinesWhenOpening = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SubtitleLineMaximumLength");
            if (subNode != null)
            {
                settings.General.SubtitleLineMaximumLength = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SubtitleMinimumDisplayMilliseconds");
            if (subNode != null)
            {
                settings.General.SubtitleMinimumDisplayMilliseconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SubtitleMaximumDisplayMilliseconds");
            if (subNode != null)
            {
                settings.General.SubtitleMaximumDisplayMilliseconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("MinimumMillisecondsBetweenLines");
            if (subNode == null)
            {
                // TODO: Remove in 3.5
                subNode = node.SelectSingleNode("MininumMillisecondsBetweenLines");
            }

            if (subNode != null)
            {
                settings.General.MinimumMillisecondsBetweenLines = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SetStartEndHumanDelay");
            if (subNode != null)
            {
                settings.General.SetStartEndHumanDelay = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoWrapLineWhileTyping");
            if (subNode != null)
            {
                settings.General.AutoWrapLineWhileTyping = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SubtitleMaximumCharactersPerSeconds");
            if (subNode != null)
            {
                settings.General.SubtitleMaximumCharactersPerSeconds = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("SubtitleOptimalCharactersPerSeconds");
            if (subNode != null)
            {
                settings.General.SubtitleOptimalCharactersPerSeconds = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("SpellCheckLanguage");
            if (subNode != null)
            {
                settings.General.SpellCheckLanguage = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("VideoPlayer");
            if (subNode != null)
            {
                settings.General.VideoPlayer = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("VideoPlayerDefaultVolume");
            if (subNode != null)
            {
                settings.General.VideoPlayerDefaultVolume = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("VideoPlayerPreviewFontSize");
            if (subNode != null)
            {
                settings.General.VideoPlayerPreviewFontSize = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("VideoPlayerPreviewFontBold");
            if (subNode != null)
            {
                settings.General.VideoPlayerPreviewFontBold = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("VideoPlayerShowStopButton");
            if (subNode != null)
            {
                settings.General.VideoPlayerShowStopButton = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("VideoPlayerShowMuteButton");
            if (subNode != null)
            {
                settings.General.VideoPlayerShowMuteButton = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("VideoPlayerShowFullscreenButton");
            if (subNode != null)
            {
                settings.General.VideoPlayerShowFullscreenButton = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("Language");
            if (subNode != null)
            {
                settings.General.Language = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ListViewLineSeparatorString");
            if (subNode != null)
            {
                settings.General.ListViewLineSeparatorString = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ListViewDoubleClickAction");
            if (subNode != null)
            {
                settings.General.ListViewDoubleClickAction = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UppercaseLetters");
            if (subNode != null)
            {
                settings.General.UppercaseLetters = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("DefaultAdjustMilliseconds");
            if (subNode != null)
            {
                settings.General.DefaultAdjustMilliseconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoRepeatOn");
            if (subNode != null)
            {
                settings.General.AutoRepeatOn = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoRepeatCount");
            if (subNode != null)
            {
                settings.General.AutoRepeatCount = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SyncListViewWithVideoWhilePlaying");
            if (subNode != null)
            {
                settings.General.SyncListViewWithVideoWhilePlaying = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoContinueOn");
            if (subNode != null)
            {
                settings.General.AutoContinueOn = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoBackupSeconds");
            if (subNode != null)
            {
                settings.General.AutoBackupSeconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SpellChecker");
            if (subNode != null)
            {
                settings.General.SpellChecker = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("AllowEditOfOriginalSubtitle");
            if (subNode != null)
            {
                settings.General.AllowEditOfOriginalSubtitle = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("PromptDeleteLines");
            if (subNode != null)
            {
                settings.General.PromptDeleteLines = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("Undocked");
            if (subNode != null)
            {
                settings.General.Undocked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UndockedVideoPosition");
            if (subNode != null)
            {
                settings.General.UndockedVideoPosition = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UndockedWaveformPosition");
            if (subNode != null)
            {
                settings.General.UndockedWaveformPosition = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UndockedVideoControlsPosition");
            if (subNode != null)
            {
                settings.General.UndockedVideoControlsPosition = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("WaveformCenter");
            if (subNode != null)
            {
                settings.General.WaveformCenter = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SmallDelayMilliseconds");
            if (subNode != null)
            {
                settings.General.SmallDelayMilliseconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LargeDelayMilliseconds");
            if (subNode != null)
            {
                settings.General.LargeDelayMilliseconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ShowOriginalAsPreviewIfAvailable");
            if (subNode != null)
            {
                settings.General.ShowOriginalAsPreviewIfAvailable = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LastPacCodePage");
            if (subNode != null)
            {
                settings.General.LastPacCodePage = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("OpenSubtitleExtraExtensions");
            if (subNode != null)
            {
                settings.General.OpenSubtitleExtraExtensions = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("ListViewColumnsRememberSize");
            if (subNode != null)
            {
                settings.General.ListViewColumnsRememberSize = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ListViewNumberWidth");
            if (subNode != null)
            {
                settings.General.ListViewNumberWidth = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ListViewStartWidth");
            if (subNode != null)
            {
                settings.General.ListViewStartWidth = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ListViewEndWidth");
            if (subNode != null)
            {
                settings.General.ListViewEndWidth = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ListViewDurationWidth");
            if (subNode != null)
            {
                settings.General.ListViewDurationWidth = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ListViewTextWidth");
            if (subNode != null)
            {
                settings.General.ListViewTextWidth = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("VlcWaveTranscodeSettings");
            if (subNode != null)
            {
                settings.General.VlcWaveTranscodeSettings = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("VlcLocation");
            if (subNode != null)
            {
                settings.General.VlcLocation = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("VlcLocationRelative");
            if (subNode != null)
            {
                settings.General.VlcLocationRelative = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("MpcHcLocation");
            if (subNode != null)
            {
                settings.General.MpcHcLocation = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("UseFFmpegForWaveExtraction");
            if (subNode != null)
            {
                settings.General.UseFFmpegForWaveExtraction = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("FFmpegLocation");
            if (subNode != null)
            {
                settings.General.FFmpegLocation = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("UseTimeFormatHHMMSSFF");
            if (subNode != null)
            {
                settings.General.UseTimeFormatHHMMSSFF = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ClearStatusBarAfterSeconds");
            if (subNode != null)
            {
                settings.General.ClearStatusBarAfterSeconds = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("Company");
            if (subNode != null)
            {
                settings.General.Company = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("DisableVideoAutoLoading");
            if (subNode != null)
            {
                settings.General.DisableVideoAutoLoading = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("RightToLeftMode");
            if (subNode != null)
            {
                settings.General.RightToLeftMode = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("LastSaveAsFormat");
            if (subNode != null)
            {
                settings.General.LastSaveAsFormat = subNode.InnerText.Trim();
            }

            subNode = node.SelectSingleNode("CheckForUpdates");
            if (subNode != null)
            {
                settings.General.CheckForUpdates = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("LastCheckForUpdates");
            if (subNode != null)
            {
                settings.General.LastCheckForUpdates = Convert.ToDateTime(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("ShowBetaStuff");
            if (subNode != null)
            {
                settings.General.ShowBetaStuff = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("NewEmptyDefaultMs");
            if (subNode != null)
            {
                settings.General.NewEmptyDefaultMs = Convert.ToInt32(subNode.InnerText.Trim());
            }

            subNode = node.SelectSingleNode("MoveVideo100Or500MsPlaySmallSample");
            if (subNode != null)
            {
                settings.General.MoveVideo100Or500MsPlaySmallSample = Convert.ToBoolean(subNode.InnerText.Trim());
            }

            settings.Tools = new ToolsSettings();
            node = doc.DocumentElement.SelectSingleNode("Tools");
            subNode = node.SelectSingleNode("StartSceneIndex");
            if (subNode != null)
            {
                settings.Tools.StartSceneIndex = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("EndSceneIndex");
            if (subNode != null)
            {
                settings.Tools.EndSceneIndex = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("VerifyPlaySeconds");
            if (subNode != null)
            {
                settings.Tools.VerifyPlaySeconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("MergeLinesShorterThan");
            if (subNode != null)
            {
                settings.Tools.MergeLinesShorterThan = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixShortDisplayTimesAllowMoveStartTime");
            if (subNode != null)
            {
                settings.Tools.FixShortDisplayTimesAllowMoveStartTime = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("MusicSymbol");
            if (subNode != null)
            {
                settings.Tools.MusicSymbol = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("MusicSymbolToReplace");
            if (subNode != null)
            {
                settings.Tools.MusicSymbolToReplace = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UnicodeSymbolsToInsert");
            if (subNode != null)
            {
                settings.Tools.UnicodeSymbolsToInsert = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("SpellCheckAutoChangeNames");
            if (subNode != null)
            {
                settings.Tools.SpellCheckAutoChangeNames = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SpellCheckOneLetterWords");
            if (subNode != null)
            {
                settings.Tools.SpellCheckOneLetterWords = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SpellCheckEnglishAllowInQuoteAsIng");
            if (subNode != null)
            {
                settings.Tools.SpellCheckEnglishAllowInQuoteAsIng = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SpellCheckShowCompletedMessage");
            if (subNode != null)
            {
                settings.Tools.SpellCheckShowCompletedMessage = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("OcrFixUseHardcodedRules");
            if (subNode != null)
            {
                settings.Tools.OcrFixUseHardcodedRules = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("Interjections");
            if (subNode != null)
            {
                settings.Tools.Interjections = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("MicrosoftBingApiId");
            if (subNode != null)
            {
                settings.Tools.MicrosoftBingApiId = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("GoogleApiKey");
            if (subNode != null)
            {
                settings.Tools.GoogleApiKey = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UseGooleApiPaidService");
            if (subNode != null)
            {
                settings.Tools.UseGooleApiPaidService = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("GoogleTranslateLastTargetLanguage");
            if (subNode != null)
            {
                settings.Tools.GoogleTranslateLastTargetLanguage = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ListViewSyntaxColorDurationSmall");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxColorDurationSmall = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ListViewSyntaxColorDurationBig");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxColorDurationBig = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ListViewSyntaxColorLongLines");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxColorLongLines = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ListViewSyntaxMoreThanXLines");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxMoreThanXLines = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ListViewSyntaxMoreThanXLinesX");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxMoreThanXLinesX = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ListViewSyntaxColorOverlap");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxColorOverlap = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ListViewSyntaxErrorColor");
            if (subNode != null)
            {
                settings.Tools.ListViewSyntaxErrorColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("ListViewUnfocusedSelectedColor");
            if (subNode != null)
            {
                settings.Tools.ListViewUnfocusedSelectedColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("SplitAdvanced");
            if (subNode != null)
            {
                settings.Tools.SplitAdvanced = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SplitOutputFolder");
            if (subNode != null)
            {
                settings.Tools.SplitOutputFolder = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("SplitNumberOfParts");
            if (subNode != null)
            {
                settings.Tools.SplitNumberOfParts = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SplitVia");
            if (subNode != null)
            {
                settings.Tools.SplitVia = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("NewEmptyTranslationText");
            if (subNode != null)
            {
                settings.Tools.NewEmptyTranslationText = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("BatchConvertOutputFolder");
            if (subNode != null)
            {
                settings.Tools.BatchConvertOutputFolder = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("BatchConvertOverwriteExisting");
            if (subNode != null)
            {
                settings.Tools.BatchConvertOverwriteExisting = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertOverwriteOriginal");
            if (subNode != null)
            {
                settings.Tools.BatchConvertOverwriteOriginal = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertRemoveFormatting");
            if (subNode != null)
            {
                settings.Tools.BatchConvertRemoveFormatting = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertFixCasing");
            if (subNode != null)
            {
                settings.Tools.BatchConvertFixCasing = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertRemoveTextForHI");
            if (subNode != null)
            {
                settings.Tools.BatchConvertRemoveTextForHI = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertFixCommonErrors");
            if (subNode != null)
            {
                settings.Tools.BatchConvertFixCommonErrors = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertMultipleReplace");
            if (subNode != null)
            {
                settings.Tools.BatchConvertMultipleReplace = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertAutoBalance");
            if (subNode != null)
            {
                settings.Tools.BatchConvertAutoBalance = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertSplitLongLines");
            if (subNode != null)
            {
                settings.Tools.BatchConvertSplitLongLines = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertSetMinDisplayTimeBetweenSubtitles");
            if (subNode != null)
            {
                settings.Tools.BatchConvertSetMinDisplayTimeBetweenSubtitles = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BatchConvertLanguage");
            if (subNode != null)
            {
                settings.Tools.BatchConvertLanguage = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("BatchConvertFormat");
            if (subNode != null)
            {
                settings.Tools.BatchConvertFormat = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ModifySelectionRule");
            if (subNode != null)
            {
                settings.Tools.ModifySelectionRule = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ModifySelectionText");
            if (subNode != null)
            {
                settings.Tools.ModifySelectionText = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ModifySelectionCaseSensitive");
            if (subNode != null)
            {
                settings.Tools.ModifySelectionCaseSensitive = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportVobSubFontName");
            if (subNode != null)
            {
                settings.Tools.ExportVobSubFontName = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportVobSubFontSize");
            if (subNode != null)
            {
                settings.Tools.ExportVobSubFontSize = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportVobSubVideoResolution");
            if (subNode != null)
            {
                settings.Tools.ExportVobSubVideoResolution = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportVobSubSimpleRendering");
            if (subNode != null)
            {
                settings.Tools.ExportVobSubSimpleRendering = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportVobAntiAliasingWithTransparency");
            if (subNode != null)
            {
                settings.Tools.ExportVobAntiAliasingWithTransparency = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportVobSubLanguage");
            if (subNode != null)
            {
                settings.Tools.ExportVobSubLanguage = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportBluRayFontName");
            if (subNode != null)
            {
                settings.Tools.ExportBluRayFontName = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportBluRayFontSize");
            if (subNode != null)
            {
                settings.Tools.ExportBluRayFontSize = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportFcpFontName");
            if (subNode != null)
            {
                settings.Tools.ExportFcpFontName = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportFontNameOther");
            if (subNode != null)
            {
                settings.Tools.ExportFontNameOther = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportFcpFontSize");
            if (subNode != null)
            {
                settings.Tools.ExportFcpFontSize = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportFcpImageType");
            if (subNode != null)
            {
                settings.Tools.ExportFcpImageType = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportBdnXmlImageType");
            if (subNode != null)
            {
                settings.Tools.ExportBdnXmlImageType = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportLastFontSize");
            if (subNode != null)
            {
                settings.Tools.ExportLastFontSize = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportLastLineHeight");
            if (subNode != null)
            {
                settings.Tools.ExportLastLineHeight = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportLastBorderWidth");
            if (subNode != null)
            {
                settings.Tools.ExportLastBorderWidth = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportLastFontBold");
            if (subNode != null)
            {
                settings.Tools.ExportLastFontBold = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportBluRayVideoResolution");
            if (subNode != null)
            {
                settings.Tools.ExportBluRayVideoResolution = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ExportFontColor");
            if (subNode != null)
            {
                settings.Tools.ExportFontColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("ExportBorderColor");
            if (subNode != null)
            {
                settings.Tools.ExportBorderColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("ExportShadowColor");
            if (subNode != null)
            {
                settings.Tools.ExportShadowColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("ExportBottomMargin");
            if (subNode != null)
            {
                settings.Tools.ExportBottomMargin = int.Parse(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportHorizontalAlignment");
            if (subNode != null)
            {
                settings.Tools.ExportHorizontalAlignment = int.Parse(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportBluRayBottomMargin");
            if (subNode != null)
            {
                settings.Tools.ExportBluRayBottomMargin = int.Parse(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportBluRayShadow");
            if (subNode != null)
            {
                settings.Tools.ExportBluRayShadow = int.Parse(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("Export3DType");
            if (subNode != null)
            {
                settings.Tools.Export3DType = int.Parse(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("Export3DDepth");
            if (subNode != null)
            {
                settings.Tools.Export3DDepth = int.Parse(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportLastShadowTransparency");
            if (subNode != null)
            {
                settings.Tools.ExportLastShadowTransparency = int.Parse(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("ExportLastFrameRate");
            if (subNode != null)
            {
                settings.Tools.ExportLastFrameRate = double.Parse(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("ExportPenLineJoin");
            if (subNode != null)
            {
                settings.Tools.ExportPenLineJoin = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("FixCommonErrorsFixOverlapAllowEqualEndStart");
            if (subNode != null)
            {
                settings.Tools.FixCommonErrorsFixOverlapAllowEqualEndStart = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ImportTextSplitting");
            if (subNode != null)
            {
                settings.Tools.ImportTextSplitting = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ImportTextMergeShortLines");
            if (subNode != null)
            {
                settings.Tools.ImportTextMergeShortLines = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ImportTextLineBreak");
            if (subNode != null)
            {
                settings.Tools.ImportTextLineBreak = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("GenerateTimeCodePatterns");
            if (subNode != null)
            {
                settings.Tools.GenerateTimeCodePatterns = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("MusicSymbolStyle");
            if (subNode != null)
            {
                settings.Tools.MusicSymbolStyle = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("BridgeGapMilliseconds");
            if (subNode != null)
            {
                settings.Tools.BridgeGapMilliseconds = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ExportCustomTemplates");
            if (subNode != null)
            {
                settings.Tools.ExportCustomTemplates = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("ChangeCasingChoice");
            if (subNode != null)
            {
                settings.Tools.ChangeCasingChoice = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UseNoLineBreakAfter");
            if (subNode != null)
            {
                settings.Tools.UseNoLineBreakAfter = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("NoLineBreakAfterEnglish");
            if (subNode != null)
            {
                settings.Tools.NoLineBreakAfterEnglish = subNode.InnerText.Replace("  ", " ");
            }

            subNode = node.SelectSingleNode("FindHistory");
            if (subNode != null)
            {
                foreach (XmlNode findItem in subNode.ChildNodes)
                {
                    if (findItem.Name == "Text")
                    {
                        settings.Tools.FindHistory.Add(findItem.InnerText);
                    }
                }
            }

            settings.SubtitleSettings = new SubtitleSettings();
            node = doc.DocumentElement.SelectSingleNode("SubtitleSettings");
            if (node != null)
            {
                subNode = node.SelectSingleNode("SsaFontName");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SsaFontName = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("SsaFontSize");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SsaFontSize = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
                }

                subNode = node.SelectSingleNode("SsaFontColorArgb");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SsaFontColorArgb = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("SsaOutline");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SsaOutline = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("SsaShadow");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SsaShadow = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("SsaOpaqueBox");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SsaOpaqueBox = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("DCinemaFontFile");
                if (subNode != null)
                {
                    settings.SubtitleSettings.DCinemaFontFile = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("DCinemaFontSize");
                if (subNode != null)
                {
                    settings.SubtitleSettings.DCinemaFontSize = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("DCinemaBottomMargin");
                if (subNode != null)
                {
                    settings.SubtitleSettings.DCinemaBottomMargin = Convert.ToInt32(subNode.InnerText, CultureInfo.InvariantCulture);
                }

                subNode = node.SelectSingleNode("DCinemaZPosition");
                if (subNode != null)
                {
                    settings.SubtitleSettings.DCinemaZPosition = Convert.ToDouble(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("DCinemaFadeUpTime");
                if (subNode != null)
                {
                    settings.SubtitleSettings.DCinemaFadeUpTime = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("DCinemaFadeDownTime");
                if (subNode != null)
                {
                    settings.SubtitleSettings.DCinemaFadeDownTime = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("SamiDisplayTwoClassesAsTwoSubtitles");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SamiDisplayTwoClassesAsTwoSubtitles = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("SamiHtmlEncodeMode");
                if (subNode != null)
                {
                    settings.SubtitleSettings.SamiHtmlEncodeMode = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("TimedText10TimeCodeFormat");
                if (subNode != null)
                {
                    settings.SubtitleSettings.TimedText10TimeCodeFormat = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("FcpFontSize");
                if (subNode != null)
                {
                    settings.SubtitleSettings.FcpFontSize = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("FcpFontName");
                if (subNode != null)
                {
                    settings.SubtitleSettings.FcpFontName = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("CheetahCaptionAlwayWriteEndTime");
                if (subNode != null)
                {
                    settings.SubtitleSettings.CheetahCaptionAlwayWriteEndTime = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("NuendoCharacterListFile");
                if (subNode != null)
                {
                    settings.SubtitleSettings.NuendoCharacterListFile = subNode.InnerText;
                }
            }

            settings.Proxy = new ProxySettings();
            node = doc.DocumentElement.SelectSingleNode("Proxy");
            subNode = node.SelectSingleNode("ProxyAddress");
            if (subNode != null)
            {
                settings.Proxy.ProxyAddress = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UserName");
            if (subNode != null)
            {
                settings.Proxy.UserName = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("Password");
            if (subNode != null)
            {
                settings.Proxy.Password = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("Domain");
            if (subNode != null)
            {
                settings.Proxy.Domain = subNode.InnerText;
            }

            settings.WordLists = new WordListSettings();
            node = doc.DocumentElement.SelectSingleNode("WordLists");
            subNode = node.SelectSingleNode("LastLanguage");
            if (subNode != null)
            {
                settings.WordLists.LastLanguage = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("NamesEtcUrl");
            if (subNode != null)
            {
                settings.WordLists.NamesEtcUrl = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UseOnlineNamesEtc");
            if (subNode != null)
            {
                settings.WordLists.UseOnlineNamesEtc = Convert.ToBoolean(subNode.InnerText);
            }

            settings.CommonErrors = new FixCommonErrorsSettings();
            node = doc.DocumentElement.SelectSingleNode("CommonErrors");
            subNode = node.SelectSingleNode("StartPosition");
            if (subNode != null)
            {
                settings.CommonErrors.StartPosition = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("StartSize");
            if (subNode != null)
            {
                settings.CommonErrors.StartSize = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("EmptyLinesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.EmptyLinesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("OverlappingDisplayTimeTicked");
            if (subNode != null)
            {
                settings.CommonErrors.OverlappingDisplayTimeTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("TooShortDisplayTimeTicked");
            if (subNode != null)
            {
                settings.CommonErrors.TooShortDisplayTimeTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("TooLongDisplayTimeTicked");
            if (subNode != null)
            {
                settings.CommonErrors.TooLongDisplayTimeTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("InvalidItalicTagsTicked");
            if (subNode != null)
            {
                settings.CommonErrors.InvalidItalicTagsTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("BreakLongLinesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.BreakLongLinesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("MergeShortLinesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.MergeShortLinesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("MergeShortLinesAllTicked");
            if (subNode != null)
            {
                settings.CommonErrors.MergeShortLinesAllTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UnneededSpacesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.UnneededSpacesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UnneededPeriodsTicked");
            if (subNode != null)
            {
                settings.CommonErrors.UnneededPeriodsTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("MissingSpacesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.MissingSpacesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AddMissingQuotesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.AddMissingQuotesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("Fix3PlusLinesTicked");
            if (subNode != null)
            {
                settings.CommonErrors.Fix3PlusLinesTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixHyphensTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixHyphensTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixHyphensAddTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixHyphensAddTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UppercaseIInsideLowercaseWordTicked");
            if (subNode != null)
            {
                settings.CommonErrors.UppercaseIInsideLowercaseWordTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("DoubleApostropheToQuoteTicked");
            if (subNode != null)
            {
                settings.CommonErrors.DoubleApostropheToQuoteTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AddPeriodAfterParagraphTicked");
            if (subNode != null)
            {
                settings.CommonErrors.AddPeriodAfterParagraphTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartWithUppercaseLetterAfterParagraphTicked");
            if (subNode != null)
            {
                settings.CommonErrors.StartWithUppercaseLetterAfterParagraphTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartWithUppercaseLetterAfterPeriodInsideParagraphTicked");
            if (subNode != null)
            {
                settings.CommonErrors.StartWithUppercaseLetterAfterPeriodInsideParagraphTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("StartWithUppercaseLetterAfterColonTicked");
            if (subNode != null)
            {
                settings.CommonErrors.StartWithUppercaseLetterAfterColonTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AloneLowercaseIToUppercaseIEnglishTicked");
            if (subNode != null)
            {
                settings.CommonErrors.AloneLowercaseIToUppercaseIEnglishTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixOcrErrorsViaReplaceListTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixOcrErrorsViaReplaceListTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("RemoveSpaceBetweenNumberTicked");
            if (subNode != null)
            {
                settings.CommonErrors.RemoveSpaceBetweenNumberTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixDialogsOnOneLineTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixDialogsOnOneLineTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("TurkishAnsiTicked");
            if (subNode != null)
            {
                settings.CommonErrors.TurkishAnsiTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("DanishLetterITicked");
            if (subNode != null)
            {
                settings.CommonErrors.DanishLetterITicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SpanishInvertedQuestionAndExclamationMarksTicked");
            if (subNode != null)
            {
                settings.CommonErrors.SpanishInvertedQuestionAndExclamationMarksTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixDoubleDashTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixDoubleDashTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixDoubleGreaterThanTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixDoubleGreaterThanTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixEllipsesStartTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixEllipsesStartTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixMissingOpenBracketTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixMissingOpenBracketTicked = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("FixMusicNotationTicked");
            if (subNode != null)
            {
                settings.CommonErrors.FixMusicNotationTicked = Convert.ToBoolean(subNode.InnerText);
            }

            settings.VideoControls = new VideoControlsSettings();
            node = doc.DocumentElement.SelectSingleNode("VideoControls");
            subNode = node.SelectSingleNode("CustomSearchText1");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchText1 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchText2");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchText2 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchText3");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchText3 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchText4");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchText4 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchText5");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchText5 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchText6");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchText6 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl1");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl1 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl1");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl1 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl2");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl2 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl3");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl3 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl4");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl4 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl5");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl5 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("CustomSearchUrl6");
            if (subNode != null)
            {
                settings.VideoControls.CustomSearchUrl6 = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("LastActiveTab");
            if (subNode != null)
            {
                settings.VideoControls.LastActiveTab = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("WaveformDrawGrid");
            if (subNode != null)
            {
                settings.VideoControls.WaveformDrawGrid = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformAllowOverlap");
            if (subNode != null)
            {
                settings.VideoControls.WaveformAllowOverlap = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformFocusOnMouseEnter");
            if (subNode != null)
            {
                settings.VideoControls.WaveformFocusOnMouseEnter = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformListViewFocusOnMouseEnter");
            if (subNode != null)
            {
                settings.VideoControls.WaveformListViewFocusOnMouseEnter = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformBorderHitMs");
            if (subNode != null)
            {
                settings.VideoControls.WaveformBorderHitMs = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformGridColor");
            if (subNode != null)
            {
                settings.VideoControls.WaveformGridColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("WaveformColor");
            if (subNode != null)
            {
                settings.VideoControls.WaveformColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("WaveformSelectedColor");
            if (subNode != null)
            {
                settings.VideoControls.WaveformSelectedColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("WaveformBackgroundColor");
            if (subNode != null)
            {
                settings.VideoControls.WaveformBackgroundColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("WaveformTextColor");
            if (subNode != null)
            {
                settings.VideoControls.WaveformTextColor = Color.FromArgb(int.Parse(subNode.InnerText));
            }

            subNode = node.SelectSingleNode("WaveformTextSize");
            if (subNode != null)
            {
                settings.VideoControls.WaveformTextSize = Convert.ToInt32(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("WaveformTextBold");
            if (subNode != null)
            {
                settings.VideoControls.WaveformTextBold = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformDoubleClickOnNonParagraphAction");
            if (subNode != null)
            {
                settings.VideoControls.WaveformDoubleClickOnNonParagraphAction = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("WaveformRightClickOnNonParagraphAction");
            if (subNode != null)
            {
                settings.VideoControls.WaveformRightClickOnNonParagraphAction = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("WaveformMouseWheelScrollUpIsForward");
            if (subNode != null)
            {
                settings.VideoControls.WaveformMouseWheelScrollUpIsForward = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("GenerateSpectrogram");
            if (subNode != null)
            {
                settings.VideoControls.GenerateSpectrogram = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("SpectrogramAppearance");
            if (subNode != null)
            {
                settings.VideoControls.SpectrogramAppearance = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("WaveformMinimumSampleRate");
            if (subNode == null)
            {
                // TODO: Remove in 3.5
                subNode = node.SelectSingleNode("WaveformMininumSampleRate");
            }

            if (subNode != null)
            {
                settings.VideoControls.WaveformMinimumSampleRate = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("WaveformSeeksSilenceDurationSeconds");
            if (subNode != null)
            {
                settings.VideoControls.WaveformSeeksSilenceDurationSeconds = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("WaveformSeeksSilenceMaxVolume");
            if (subNode != null)
            {
                settings.VideoControls.WaveformSeeksSilenceMaxVolume = Convert.ToInt32(subNode.InnerText);
            }

            settings.NetworkSettings = new NetworkSettings();
            node = doc.DocumentElement.SelectSingleNode("NetworkSettings");
            if (node != null)
            {
                subNode = node.SelectSingleNode("SessionKey");
                if (subNode != null)
                {
                    settings.NetworkSettings.SessionKey = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("UserName");
                if (subNode != null)
                {
                    settings.NetworkSettings.UserName = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WebServiceUrl");
                if (subNode != null)
                {
                    settings.NetworkSettings.WebServiceUrl = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("PollIntervalSeconds");
                if (subNode != null)
                {
                    settings.NetworkSettings.PollIntervalSeconds = Convert.ToInt32(subNode.InnerText);
                }
            }

            settings.VobSubOcr = new VobSubOcrSettings();
            node = doc.DocumentElement.SelectSingleNode("VobSubOcr");
            subNode = node.SelectSingleNode("XOrMorePixelsMakesSpace");
            if (subNode != null)
            {
                settings.VobSubOcr.XOrMorePixelsMakesSpace = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AllowDifferenceInPercent");
            if (subNode != null)
            {
                settings.VobSubOcr.AllowDifferenceInPercent = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("BlurayAllowDifferenceInPercent");
            if (subNode != null)
            {
                settings.VobSubOcr.BlurayAllowDifferenceInPercent = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("LastImageCompareFolder");
            if (subNode != null)
            {
                settings.VobSubOcr.LastImageCompareFolder = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("LastModiLanguageId");
            if (subNode != null)
            {
                settings.VobSubOcr.LastModiLanguageId = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LastOcrMethod");
            if (subNode != null)
            {
                settings.VobSubOcr.LastOcrMethod = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("TesseractLastLanguage");
            if (subNode != null)
            {
                settings.VobSubOcr.TesseractLastLanguage = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("UseModiInTesseractForUnknownWords");
            if (subNode != null)
            {
                settings.VobSubOcr.UseModiInTesseractForUnknownWords = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UseItalicsInTesseract");
            if (subNode != null)
            {
                settings.VobSubOcr.UseItalicsInTesseract = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("UseMusicSymbolsInTesseract");
            if (subNode != null)
            {
                settings.VobSubOcr.UseMusicSymbolsInTesseract = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("RightToLeft");
            if (subNode != null)
            {
                settings.VobSubOcr.RightToLeft = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("TopToBottom");
            if (subNode != null)
            {
                settings.VobSubOcr.TopToBottom = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("DefaultMillisecondsForUnknownDurations");
            if (subNode != null)
            {
                settings.VobSubOcr.DefaultMillisecondsForUnknownDurations = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("PromptForUnknownWords");
            if (subNode != null)
            {
                settings.VobSubOcr.PromptForUnknownWords = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("GuessUnknownWords");
            if (subNode != null)
            {
                settings.VobSubOcr.GuessUnknownWords = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("AutoBreakSubtitleIfMoreThanTwoLines");
            if (subNode != null)
            {
                settings.VobSubOcr.AutoBreakSubtitleIfMoreThanTwoLines = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("ItalicFactor");
            if (subNode != null)
            {
                settings.VobSubOcr.ItalicFactor = Convert.ToDouble(subNode.InnerText, CultureInfo.InvariantCulture);
            }

            subNode = node.SelectSingleNode("LineOcrDraw");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrDraw = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LineOcrAdvancedItalic");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrAdvancedItalic = Convert.ToBoolean(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LineOcrLastLanguages");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrLastLanguages = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("LineOcrLastSpellCheck");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrLastSpellCheck = subNode.InnerText;
            }

            subNode = node.SelectSingleNode("LineOcrXOrMorePixelsMakesSpace");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrXOrMorePixelsMakesSpace = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LineOcrMinLineHeight");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrMinLineHeight = Convert.ToInt32(subNode.InnerText);
            }

            subNode = node.SelectSingleNode("LineOcrMaxLineHeight");
            if (subNode != null)
            {
                settings.VobSubOcr.LineOcrMaxLineHeight = Convert.ToInt32(subNode.InnerText);
            }

            foreach (XmlNode listNode in doc.DocumentElement.SelectNodes("MultipleSearchAndReplaceList/MultipleSearchAndReplaceItem"))
            {
                var item = new MultipleSearchAndReplaceSetting();
                subNode = listNode.SelectSingleNode("Enabled");
                if (subNode != null)
                {
                    item.Enabled = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = listNode.SelectSingleNode("FindWhat");
                if (subNode != null)
                {
                    item.FindWhat = subNode.InnerText;
                }

                subNode = listNode.SelectSingleNode("ReplaceWith");
                if (subNode != null)
                {
                    item.ReplaceWith = subNode.InnerText;
                }

                subNode = listNode.SelectSingleNode("SearchType");
                if (subNode != null)
                {
                    item.SearchType = subNode.InnerText;
                }

                settings.MultipleSearchAndReplaceList.Add(item);
            }

            settings.Shortcuts = new Shortcuts();
            node = doc.DocumentElement.SelectSingleNode("Shortcuts");
            if (node != null)
            {
                subNode = node.SelectSingleNode("GeneralGoToFirstSelectedLine");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralGoToFirstSelectedLine = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralGoToNextEmptyLine");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralGoToNextEmptyLine = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralMergeSelectedLines");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralMergeSelectedLines = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralMergeSelectedLinesOnlyFirstText");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralMergeSelectedLinesOnlyFirstText = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralToggleTranslationMode");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralToggleTranslationMode = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralSwitchOriginalAndTranslation");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralSwitchOriginalAndTranslation = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralMergeOriginalAndTranslation");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralMergeOriginalAndTranslation = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralGoToNextSubtitle");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralGoToNextSubtitle = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralGoToPrevSubtitle");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralGoToPrevSubtitle = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralGoToEndOfCurrentSubtitle");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralGoToEndOfCurrentSubtitle = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralGoToStartOfCurrentSubtitle");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralGoToStartOfCurrentSubtitle = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("GeneralPlayFirstSelected");
                if (subNode != null)
                {
                    settings.Shortcuts.GeneralPlayFirstSelected = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileNew");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileNew = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileOpen");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileOpen = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileOpenKeepVideo");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileOpenKeepVideo = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileSave");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileSave = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileSaveOriginal");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileSaveOriginal = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileSaveOriginalAs");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileSaveOriginalAs = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileSaveAs");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileSaveAs = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileSaveAll");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileSaveAll = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainFileExportEbu");
                if (subNode != null)
                {
                    settings.Shortcuts.MainFileExportEbu = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditUndo");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditUndo = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditRedo");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditRedo = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditFind");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditFind = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditFindNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditFindNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditReplace");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditReplace = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditMultipleReplace");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditMultipleReplace = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditGoToLineNumber");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditGoToLineNumber = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditRightToLeft");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditRightToLeft = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsFixCommonErrors");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsFixCommonErrors = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsFixCommonErrorsPreview");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsFixCommonErrorsPreview = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsMergeShortLines");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsMergeShortLines = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsSplitLongLines");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsSplitLongLines = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsRenumber");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsRenumber = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsRemoveTextForHI");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsRemoveTextForHI = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsChangeCasing");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsChangeCasing = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsAutoDuration");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsAutoDuration = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsBatchConvert");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsBatchConvert = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsBeamer");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToolsBeamer = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToolsToggleTranslationOriginalInPreviews");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditInverseSelection");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditInverseSelection = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditModifySelection");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditModifySelection = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideoPause");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideoPause = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideoPlayPauseToggle");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideoPlayPauseToggle = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideoShowHideVideo");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideoShowHideVideo = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideoToggleVideoControls");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideoToggleVideoControls = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo1FrameLeft");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo1FrameLeft = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo1FrameRight");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo1FrameRight = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo100MsLeft");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo100MsLeft = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo100MsRight");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo100MsRight = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo500MsLeft");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo500MsLeft = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo500MsRight");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo500MsRight = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo1000MsLeft");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo1000MsLeft = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideo1000MsRight");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideo1000MsRight = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainVideoFullscreen");
                if (subNode != null)
                {
                    settings.Shortcuts.MainVideoFullscreen = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSpellCheck");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSpellCheck = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSpellCheckFindDoubleWords");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSpellCheckFindDoubleWords = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSpellCheckAddWordToNames");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSpellCheckAddWordToNames = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSynchronizationAdjustTimes");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSynchronizationAdjustTimes = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSynchronizationVisualSync");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSynchronizationVisualSync = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSynchronizationPointSync");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSynchronizationPointSync = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainSynchronizationChangeFrameRate");
                if (subNode != null)
                {
                    settings.Shortcuts.MainSynchronizationChangeFrameRate = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewItalic");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewItalic = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewToggleDashes");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewToggleDashes = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewAlignment");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewAlignment = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewCopyText");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewCopyText = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewCopyTextFromOriginalToCurrent");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewCopyTextFromOriginalToCurrent = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewAutoDuration");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewAutoDuration = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewColumnDeleteText");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewColumnDeleteText = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewColumnInsertText");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewColumnInsertText = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewColumnPaste");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewColumnPaste = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewFocusWaveform");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewFocusWaveform = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainListViewGoToNextError");
                if (subNode != null)
                {
                    settings.Shortcuts.MainListViewGoToNextError = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainEditReverseStartAndEndingForRTL");
                if (subNode != null)
                {
                    settings.Shortcuts.MainEditReverseStartAndEndingForRTL = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxItalic");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxItalic = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxSplitAtCursor");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxSplitAtCursor = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxMoveLastWordDown");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxMoveLastWordDown = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxMoveFirstWordFromNextUp");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxMoveFirstWordFromNextUp = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxSelectionToLower");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxSelectionToLower = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxSelectionToUpper");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxSelectionToUpper = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxToggleAutoDuration");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxToggleAutoDuration = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainCreateInsertSubAtVideoPos");
                if (subNode != null)
                {
                    settings.Shortcuts.MainCreateInsertSubAtVideoPos = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainCreatePlayFromJustBefore");
                if (subNode != null)
                {
                    settings.Shortcuts.MainCreatePlayFromJustBefore = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainCreateSetStart");
                if (subNode != null)
                {
                    settings.Shortcuts.MainCreateSetStart = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainCreateSetEnd");
                if (subNode != null)
                {
                    settings.Shortcuts.MainCreateSetEnd = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainCreateSetEndAddNewAndGoToNew");
                if (subNode != null)
                {
                    settings.Shortcuts.MainCreateSetEndAddNewAndGoToNew = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainCreateStartDownEndUp");
                if (subNode != null)
                {
                    settings.Shortcuts.MainCreateStartDownEndUp = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetStartAndOffsetTheRest");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetEndAndOffsetTheRest");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetEndAndOffsetTheRest = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetEndAndOffsetTheRestAndGoToNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetEndAndOffsetTheRestAndGoToNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetEndAndGotoNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetEndAndGotoNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustViaEndAutoStartAndGoToNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustViaEndAutoStartAndGoToNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetStartAutoDurationAndGoToNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetStartAutoDurationAndGoToNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetEndNextStartAndGoToNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetEndNextStartAndGoToNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustStartDownEndUpAndGoToNext");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustStartDownEndUpAndGoToNext = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetStart");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetStart = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetStartKeepDuration");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetStartKeepDuration = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSetEnd");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSetEnd = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSelected100MsForward");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSelected100MsForward = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainAdjustSelected100MsBack");
                if (subNode != null)
                {
                    settings.Shortcuts.MainAdjustSelected100MsBack = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainInsertAfter");
                if (subNode != null)
                {
                    settings.Shortcuts.MainInsertAfter = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxInsertAfter");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxInsertAfter = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxAutoBreak");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxAutoBreak = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTextBoxUnbreak");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTextBoxUnbreak = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainWaveformInsertAtCurrentPosition");
                if (subNode != null)
                {
                    settings.Shortcuts.MainWaveformInsertAtCurrentPosition = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainInsertBefore");
                if (subNode != null)
                {
                    settings.Shortcuts.MainInsertBefore = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainMergeDialog");
                if (subNode != null)
                {
                    settings.Shortcuts.MainMergeDialog = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainToggleFocus");
                if (subNode != null)
                {
                    settings.Shortcuts.MainToggleFocus = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformVerticalZoom");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformVerticalZoom = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformVerticalZoomOut");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformVerticalZoomOut = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformZoomIn");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformZoomIn = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformZoomOut");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformZoomOut = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformPlaySelection");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformPlaySelection = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformSearchSilenceForward");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformSearchSilenceForward = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformSearchSilenceBack");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformSearchSilenceBack = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformAddTextHere");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformAddTextHere = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("WaveformFocusListView");
                if (subNode != null)
                {
                    settings.Shortcuts.WaveformFocusListView = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTranslateCustomSearch1");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTranslateCustomSearch1 = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTranslateCustomSearch2");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTranslateCustomSearch2 = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTranslateCustomSearch3");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTranslateCustomSearch3 = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTranslateCustomSearch4");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTranslateCustomSearch4 = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTranslateCustomSearch5");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTranslateCustomSearch5 = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("MainTranslateCustomSearch6");
                if (subNode != null)
                {
                    settings.Shortcuts.MainTranslateCustomSearch6 = subNode.InnerText;
                }
            }

            settings.RemoveTextForHearingImpaired = new RemoveTextForHearingImpairedSettings();
            node = doc.DocumentElement.SelectSingleNode("RemoveTextForHearingImpaired");
            if (node != null)
            {
                subNode = node.SelectSingleNode("RemoveTextBetweenBrackets");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenParentheses");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenCurlyBrackets");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenQuestionMarks");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenCustom");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustom = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenCustomBefore");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomBefore = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenCustomAfter");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomAfter = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("RemoveTextBetweenOnlySeperateLines");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBeforeColon");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBeforeColonOnlyIfUppercase");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveTextBeforeColonOnlyOnSeparateLine");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveIfAllUppercase");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveInterjections");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveInterjections = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveIfContains");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveIfContains = Convert.ToBoolean(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("RemoveIfContainsText");
                if (subNode != null)
                {
                    settings.RemoveTextForHearingImpaired.RemoveIfContainsText = subNode.InnerText;
                }
            }

            settings.SubtitleBeaming = new SubtitleBeaming();
            node = doc.DocumentElement.SelectSingleNode("SubtitleBeaming");
            if (node != null)
            {
                subNode = node.SelectSingleNode("FontName");
                if (subNode != null)
                {
                    settings.SubtitleBeaming.FontName = subNode.InnerText;
                }

                subNode = node.SelectSingleNode("FontColor");
                if (subNode != null)
                {
                    settings.SubtitleBeaming.FontColor = Color.FromArgb(Convert.ToInt32(subNode.InnerText));
                }

                subNode = node.SelectSingleNode("FontSize");
                if (subNode != null)
                {
                    settings.SubtitleBeaming.FontSize = Convert.ToInt32(subNode.InnerText);
                }

                subNode = node.SelectSingleNode("BorderColor");
                if (subNode != null)
                {
                    settings.SubtitleBeaming.BorderColor = Color.FromArgb(Convert.ToInt32(subNode.InnerText));
                }

                subNode = node.SelectSingleNode("BorderWidth");
                if (subNode != null)
                {
                    settings.SubtitleBeaming.BorderWidth = Convert.ToInt32(subNode.InnerText);
                }
            }

            return settings;
        }

        /// <summary>
        /// The custom serialize.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        private static void CustomSerialize(string fileName, Settings settings)
        {
            var xws = new XmlWriterSettings { Indent = true };
            var sb = new StringBuilder();
            using (var textWriter = XmlWriter.Create(sb, xws))
            {
                textWriter.WriteStartDocument();

                textWriter.WriteStartElement("Settings", string.Empty);

                textWriter.WriteStartElement("RecentFiles", string.Empty);
                textWriter.WriteStartElement("FileNames", string.Empty);
                foreach (var item in settings.RecentFiles.Files)
                {
                    textWriter.WriteStartElement("FileName");
                    if (item.OriginalFileName != null)
                    {
                        textWriter.WriteAttributeString("OriginalFileName", item.OriginalFileName);
                    }

                    if (item.VideoFileName != null)
                    {
                        textWriter.WriteAttributeString("VideoFileName", item.VideoFileName);
                    }

                    textWriter.WriteAttributeString("FirstVisibleIndex", item.FirstVisibleIndex.ToString(CultureInfo.InvariantCulture));
                    textWriter.WriteAttributeString("FirstSelectedIndex", item.FirstSelectedIndex.ToString(CultureInfo.InvariantCulture));
                    textWriter.WriteString(item.FileName);
                    textWriter.WriteEndElement();
                }

                textWriter.WriteEndElement();
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("General", string.Empty);
                textWriter.WriteElementString("ShowToolbarNew", settings.General.ShowToolbarNew.ToString());
                textWriter.WriteElementString("ShowToolbarOpen", settings.General.ShowToolbarOpen.ToString());
                textWriter.WriteElementString("ShowToolbarSave", settings.General.ShowToolbarSave.ToString());
                textWriter.WriteElementString("ShowToolbarSaveAs", settings.General.ShowToolbarSaveAs.ToString());
                textWriter.WriteElementString("ShowToolbarFind", settings.General.ShowToolbarFind.ToString());
                textWriter.WriteElementString("ShowToolbarReplace", settings.General.ShowToolbarReplace.ToString());
                textWriter.WriteElementString("ShowToolbarFixCommonErrors", settings.General.ShowToolbarFixCommonErrors.ToString());
                textWriter.WriteElementString("ShowToolbarVisualSync", settings.General.ShowToolbarVisualSync.ToString());
                textWriter.WriteElementString("ShowToolbarSpellCheck", settings.General.ShowToolbarSpellCheck.ToString());
                textWriter.WriteElementString("ShowToolbarSettings", settings.General.ShowToolbarSettings.ToString());
                textWriter.WriteElementString("ShowToolbarHelp", settings.General.ShowToolbarHelp.ToString());
                textWriter.WriteElementString("ShowFrameRate", settings.General.ShowFrameRate.ToString());
                textWriter.WriteElementString("ShowVideoPlayer", settings.General.ShowVideoPlayer.ToString());
                textWriter.WriteElementString("ShowAudioVisualizer", settings.General.ShowAudioVisualizer.ToString());
                textWriter.WriteElementString("ShowWaveform", settings.General.ShowWaveform.ToString());
                textWriter.WriteElementString("ShowSpectrogram", settings.General.ShowSpectrogram.ToString());
                textWriter.WriteElementString("DefaultFrameRate", settings.General.DefaultFrameRate.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("DefaultSubtitleFormat", settings.General.DefaultSubtitleFormat);
                textWriter.WriteElementString("DefaultEncoding", settings.General.DefaultEncoding);
                textWriter.WriteElementString("AutoConvertToUtf8", settings.General.AutoConvertToUtf8.ToString());
                textWriter.WriteElementString("AutoGuessAnsiEncoding", settings.General.AutoGuessAnsiEncoding.ToString());
                textWriter.WriteElementString("_subtitleFontName", settings.General.SubtitleFontName);
                textWriter.WriteElementString("SubtitleFontSize", settings.General.SubtitleFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SubtitleFontBold", settings.General.SubtitleFontBold.ToString());
                textWriter.WriteElementString("SubtitleFontColor", settings.General.SubtitleFontColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SubtitleBackgroundColor", settings.General.SubtitleBackgroundColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("CenterSubtitleInTextBox", settings.General.CenterSubtitleInTextBox.ToString());
                textWriter.WriteElementString("ShowRecentFiles", settings.General.ShowRecentFiles.ToString());
                textWriter.WriteElementString("RememberSelectedLine", settings.General.RememberSelectedLine.ToString());
                textWriter.WriteElementString("StartLoadLastFile", settings.General.StartLoadLastFile.ToString());
                textWriter.WriteElementString("StartRememberPositionAndSize", settings.General.StartRememberPositionAndSize.ToString());
                textWriter.WriteElementString("StartPosition", settings.General.StartPosition);
                textWriter.WriteElementString("StartSize", settings.General.StartSize);
                textWriter.WriteElementString("SplitContainerMainSplitterDistance", settings.General.SplitContainerMainSplitterDistance.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SplitContainer1SplitterDistance", settings.General.SplitContainer1SplitterDistance.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SplitContainerListViewAndTextSplitterDistance", settings.General.SplitContainerListViewAndTextSplitterDistance.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("StartInSourceView", settings.General.StartInSourceView.ToString());
                textWriter.WriteElementString("RemoveBlankLinesWhenOpening", settings.General.RemoveBlankLinesWhenOpening.ToString());
                textWriter.WriteElementString("SubtitleLineMaximumLength", settings.General.SubtitleLineMaximumLength.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SubtitleMinimumDisplayMilliseconds", settings.General.SubtitleMinimumDisplayMilliseconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SubtitleMaximumDisplayMilliseconds", settings.General.SubtitleMaximumDisplayMilliseconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("MinimumMillisecondsBetweenLines", settings.General.MinimumMillisecondsBetweenLines.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SetStartEndHumanDelay", settings.General.SetStartEndHumanDelay.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("AutoWrapLineWhileTyping", settings.General.AutoWrapLineWhileTyping.ToString());
                textWriter.WriteElementString("SubtitleMaximumCharactersPerSeconds", settings.General.SubtitleMaximumCharactersPerSeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SubtitleOptimalCharactersPerSeconds", settings.General.SubtitleOptimalCharactersPerSeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SpellCheckLanguage", settings.General.SpellCheckLanguage);
                textWriter.WriteElementString("VideoPlayer", settings.General.VideoPlayer);
                textWriter.WriteElementString("VideoPlayerDefaultVolume", settings.General.VideoPlayerDefaultVolume.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("VideoPlayerPreviewFontSize", settings.General.VideoPlayerPreviewFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("VideoPlayerPreviewFontBold", settings.General.VideoPlayerPreviewFontBold.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("VideoPlayerShowStopButton", settings.General.VideoPlayerShowStopButton.ToString());
                textWriter.WriteElementString("VideoPlayerShowStopMute", settings.General.VideoPlayerShowMuteButton.ToString());
                textWriter.WriteElementString("VideoPlayerShowStopFullscreen", settings.General.VideoPlayerShowFullscreenButton.ToString());
                textWriter.WriteElementString("Language", settings.General.Language);
                textWriter.WriteElementString("ListViewLineSeparatorString", settings.General.ListViewLineSeparatorString);
                textWriter.WriteElementString("ListViewDoubleClickAction", settings.General.ListViewDoubleClickAction.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("UppercaseLetters", settings.General.UppercaseLetters);
                textWriter.WriteElementString("DefaultAdjustMilliseconds", settings.General.DefaultAdjustMilliseconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("AutoRepeatOn", settings.General.AutoRepeatOn.ToString());
                textWriter.WriteElementString("AutoRepeatCount", settings.General.AutoRepeatCount.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("AutoContinueOn", settings.General.AutoContinueOn.ToString());
                textWriter.WriteElementString("SyncListViewWithVideoWhilePlaying", settings.General.SyncListViewWithVideoWhilePlaying.ToString());
                textWriter.WriteElementString("AutoBackupSeconds", settings.General.AutoBackupSeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SpellChecker", settings.General.SpellChecker);
                textWriter.WriteElementString("AllowEditOfOriginalSubtitle", settings.General.AllowEditOfOriginalSubtitle.ToString());
                textWriter.WriteElementString("PromptDeleteLines", settings.General.PromptDeleteLines.ToString());
                textWriter.WriteElementString("Undocked", settings.General.Undocked.ToString());
                textWriter.WriteElementString("UndockedVideoPosition", settings.General.UndockedVideoPosition);
                textWriter.WriteElementString("UndockedWaveformPosition", settings.General.UndockedWaveformPosition);
                textWriter.WriteElementString("UndockedVideoControlsPosition", settings.General.UndockedVideoControlsPosition);
                textWriter.WriteElementString("WaveformCenter", settings.General.WaveformCenter.ToString());
                textWriter.WriteElementString("SmallDelayMilliseconds", settings.General.SmallDelayMilliseconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("LargeDelayMilliseconds", settings.General.LargeDelayMilliseconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ShowOriginalAsPreviewIfAvailable", settings.General.ShowOriginalAsPreviewIfAvailable.ToString());
                textWriter.WriteElementString("LastPacCodePage", settings.General.LastPacCodePage.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("OpenSubtitleExtraExtensions", settings.General.OpenSubtitleExtraExtensions);
                textWriter.WriteElementString("ListViewColumnsRememberSize", settings.General.ListViewColumnsRememberSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewNumberWidth", settings.General.ListViewNumberWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewStartWidth", settings.General.ListViewStartWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewEndWidth", settings.General.ListViewEndWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewDurationWidth", settings.General.ListViewDurationWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewTextWidth", settings.General.ListViewTextWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("VlcWaveTranscodeSettings", settings.General.VlcWaveTranscodeSettings);
                textWriter.WriteElementString("VlcLocation", settings.General.VlcLocation);
                textWriter.WriteElementString("VlcLocationRelative", settings.General.VlcLocationRelative);
                textWriter.WriteElementString("MpcHcLocation", settings.General.MpcHcLocation);
                textWriter.WriteElementString("UseFFmpegForWaveExtraction", settings.General.UseFFmpegForWaveExtraction.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("FFmpegLocation", settings.General.FFmpegLocation);
                textWriter.WriteElementString("UseTimeFormatHHMMSSFF", settings.General.UseTimeFormatHHMMSSFF.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ClearStatusBarAfterSeconds", settings.General.ClearStatusBarAfterSeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("Company", settings.General.Company);
                textWriter.WriteElementString("MoveVideo100Or500MsPlaySmallSample", settings.General.MoveVideo100Or500MsPlaySmallSample.ToString());
                textWriter.WriteElementString("DisableVideoAutoLoading", settings.General.DisableVideoAutoLoading.ToString());
                textWriter.WriteElementString("RightToLeftMode", settings.General.RightToLeftMode.ToString());
                textWriter.WriteElementString("LastSaveAsFormat", settings.General.LastSaveAsFormat);
                textWriter.WriteElementString("CheckForUpdates", settings.General.CheckForUpdates.ToString());
                textWriter.WriteElementString("LastCheckForUpdates", settings.General.LastCheckForUpdates.ToString("yyyy-MM-dd"));
                textWriter.WriteElementString("ShowBetaStuff", settings.General.ShowBetaStuff.ToString());
                textWriter.WriteElementString("NewEmptyDefaultMs", settings.General.NewEmptyDefaultMs.ToString(CultureInfo.InvariantCulture));

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("Tools", string.Empty);
                textWriter.WriteElementString("StartSceneIndex", settings.Tools.StartSceneIndex.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("EndSceneIndex", settings.Tools.EndSceneIndex.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("VerifyPlaySeconds", settings.Tools.VerifyPlaySeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("MergeLinesShorterThan", settings.Tools.MergeLinesShorterThan.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("FixShortDisplayTimesAllowMoveStartTime", settings.Tools.FixShortDisplayTimesAllowMoveStartTime.ToString());
                textWriter.WriteElementString("MusicSymbol", settings.Tools.MusicSymbol);
                textWriter.WriteElementString("MusicSymbolToReplace", settings.Tools.MusicSymbolToReplace);
                textWriter.WriteElementString("UnicodeSymbolsToInsert", settings.Tools.UnicodeSymbolsToInsert);
                textWriter.WriteElementString("SpellCheckAutoChangeNames", settings.Tools.SpellCheckAutoChangeNames.ToString());
                textWriter.WriteElementString("SpellCheckOneLetterWords", settings.Tools.SpellCheckOneLetterWords.ToString());
                textWriter.WriteElementString("SpellCheckEnglishAllowInQuoteAsIng", settings.Tools.SpellCheckEnglishAllowInQuoteAsIng.ToString());
                textWriter.WriteElementString("SpellCheckShowCompletedMessage", settings.Tools.SpellCheckShowCompletedMessage.ToString());
                textWriter.WriteElementString("OcrFixUseHardcodedRules", settings.Tools.OcrFixUseHardcodedRules.ToString());
                textWriter.WriteElementString("Interjections", settings.Tools.Interjections);
                textWriter.WriteElementString("MicrosoftBingApiId", settings.Tools.MicrosoftBingApiId);
                textWriter.WriteElementString("GoogleApiKey", settings.Tools.GoogleApiKey);
                textWriter.WriteElementString("UseGooleApiPaidService", settings.Tools.UseGooleApiPaidService.ToString());
                textWriter.WriteElementString("GoogleTranslateLastTargetLanguage", settings.Tools.GoogleTranslateLastTargetLanguage);
                textWriter.WriteElementString("ListViewSyntaxColorDurationSmall", settings.Tools.ListViewSyntaxColorDurationSmall.ToString());
                textWriter.WriteElementString("ListViewSyntaxColorDurationBig", settings.Tools.ListViewSyntaxColorDurationBig.ToString());
                textWriter.WriteElementString("ListViewSyntaxColorLongLines", settings.Tools.ListViewSyntaxColorLongLines.ToString());
                textWriter.WriteElementString("ListViewSyntaxMoreThanXLines", settings.Tools.ListViewSyntaxMoreThanXLines.ToString());
                textWriter.WriteElementString("ListViewSyntaxMoreThanXLinesX", settings.Tools.ListViewSyntaxMoreThanXLinesX.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewSyntaxColorOverlap", settings.Tools.ListViewSyntaxColorOverlap.ToString());
                textWriter.WriteElementString("ListViewSyntaxErrorColor", settings.Tools.ListViewSyntaxErrorColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ListViewUnfocusedSelectedColor", settings.Tools.ListViewUnfocusedSelectedColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SplitAdvanced", settings.Tools.SplitAdvanced.ToString());
                textWriter.WriteElementString("SplitOutputFolder", settings.Tools.SplitOutputFolder);
                textWriter.WriteElementString("SplitNumberOfParts", settings.Tools.SplitNumberOfParts.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SplitVia", settings.Tools.SplitVia);
                textWriter.WriteElementString("NewEmptyTranslationText", settings.Tools.NewEmptyTranslationText);
                textWriter.WriteElementString("BatchConvertOutputFolder", settings.Tools.BatchConvertOutputFolder);
                textWriter.WriteElementString("BatchConvertOverwriteExisting", settings.Tools.BatchConvertOverwriteExisting.ToString());
                textWriter.WriteElementString("BatchConvertOverwriteOriginal", settings.Tools.BatchConvertOverwriteOriginal.ToString());
                textWriter.WriteElementString("BatchConvertRemoveFormatting", settings.Tools.BatchConvertRemoveFormatting.ToString());
                textWriter.WriteElementString("BatchConvertFixCasing", settings.Tools.BatchConvertFixCasing.ToString());
                textWriter.WriteElementString("BatchConvertRemoveTextForHI", settings.Tools.BatchConvertRemoveTextForHI.ToString());
                textWriter.WriteElementString("BatchConvertSplitLongLines", settings.Tools.BatchConvertSplitLongLines.ToString());
                textWriter.WriteElementString("BatchConvertFixCommonErrors", settings.Tools.BatchConvertFixCommonErrors.ToString());
                textWriter.WriteElementString("BatchConvertMultipleReplace", settings.Tools.BatchConvertMultipleReplace.ToString());
                textWriter.WriteElementString("BatchConvertAutoBalance", settings.Tools.BatchConvertAutoBalance.ToString());
                textWriter.WriteElementString("BatchConvertSetMinDisplayTimeBetweenSubtitles", settings.Tools.BatchConvertSetMinDisplayTimeBetweenSubtitles.ToString());
                textWriter.WriteElementString("BatchConvertLanguage", settings.Tools.BatchConvertLanguage);
                textWriter.WriteElementString("BatchConvertFormat", settings.Tools.BatchConvertFormat);
                textWriter.WriteElementString("ModifySelectionRule", settings.Tools.ModifySelectionRule);
                textWriter.WriteElementString("ModifySelectionText", settings.Tools.ModifySelectionText);
                textWriter.WriteElementString("ModifySelectionCaseSensitive", settings.Tools.ModifySelectionCaseSensitive.ToString());
                textWriter.WriteElementString("ExportVobSubFontName", settings.Tools.ExportVobSubFontName);
                textWriter.WriteElementString("ExportVobSubFontSize", settings.Tools.ExportVobSubFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportVobSubVideoResolution", settings.Tools.ExportVobSubVideoResolution);
                textWriter.WriteElementString("ExportVobSubLanguage", settings.Tools.ExportVobSubLanguage);
                textWriter.WriteElementString("ExportVobSubSimpleRendering", settings.Tools.ExportVobSubSimpleRendering.ToString());
                textWriter.WriteElementString("ExportVobAntiAliasingWithTransparency", settings.Tools.ExportVobAntiAliasingWithTransparency.ToString());
                textWriter.WriteElementString("ExportBluRayFontName", settings.Tools.ExportBluRayFontName);
                textWriter.WriteElementString("ExportBluRayFontSize", settings.Tools.ExportBluRayFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportFcpFontName", settings.Tools.ExportFcpFontName);
                textWriter.WriteElementString("ExportFontNameOther", settings.Tools.ExportFontNameOther);
                textWriter.WriteElementString("ExportFcpFontSize", settings.Tools.ExportFcpFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportFcpImageType", settings.Tools.ExportFcpImageType);
                textWriter.WriteElementString("ExportBdnXmlImageType", settings.Tools.ExportBdnXmlImageType);
                textWriter.WriteElementString("ExportLastFontSize", settings.Tools.ExportLastFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportLastLineHeight", settings.Tools.ExportLastLineHeight.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportLastBorderWidth", settings.Tools.ExportLastBorderWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportLastFontBold", settings.Tools.ExportLastFontBold.ToString());
                textWriter.WriteElementString("ExportBluRayVideoResolution", settings.Tools.ExportBluRayVideoResolution);
                textWriter.WriteElementString("ExportFontColor", settings.Tools.ExportFontColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportBorderColor", settings.Tools.ExportBorderColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportShadowColor", settings.Tools.ExportShadowColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportBottomMargin", settings.Tools.ExportBottomMargin.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportHorizontalAlignment", settings.Tools.ExportHorizontalAlignment.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportBluRayBottomMargin", settings.Tools.ExportBluRayBottomMargin.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportBluRayShadow", settings.Tools.ExportBluRayShadow.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("Export3DType", settings.Tools.Export3DType.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("Export3DDepth", settings.Tools.Export3DDepth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportLastShadowTransparency", settings.Tools.ExportLastShadowTransparency.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportLastFrameRate", settings.Tools.ExportLastFrameRate.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportPenLineJoin", settings.Tools.ExportPenLineJoin);
                textWriter.WriteElementString("FixCommonErrorsFixOverlapAllowEqualEndStart", settings.Tools.FixCommonErrorsFixOverlapAllowEqualEndStart.ToString());
                textWriter.WriteElementString("ImportTextSplitting", settings.Tools.ImportTextSplitting);
                textWriter.WriteElementString("ImportTextMergeShortLines", settings.Tools.ImportTextMergeShortLines.ToString());
                textWriter.WriteElementString("ImportTextLineBreak", settings.Tools.ImportTextLineBreak);
                textWriter.WriteElementString("GenerateTimeCodePatterns", settings.Tools.GenerateTimeCodePatterns);
                textWriter.WriteElementString("MusicSymbolStyle", settings.Tools.MusicSymbolStyle);
                textWriter.WriteElementString("BridgeGapMilliseconds", settings.Tools.BridgeGapMilliseconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("ExportCustomTemplates", settings.Tools.ExportCustomTemplates);
                textWriter.WriteElementString("ChangeCasingChoice", settings.Tools.ChangeCasingChoice);
                textWriter.WriteElementString("UseNoLineBreakAfter", settings.Tools.UseNoLineBreakAfter.ToString());
                textWriter.WriteElementString("NoLineBreakAfterEnglish", settings.Tools.NoLineBreakAfterEnglish);
                if (settings.Tools.FindHistory != null && settings.Tools.FindHistory.Count > 0)
                {
                    const int maximumFindHistoryItems = 10;
                    textWriter.WriteStartElement("FindHistory", string.Empty);
                    int maxIndex = settings.Tools.FindHistory.Count;
                    if (maxIndex > maximumFindHistoryItems)
                    {
                        maxIndex = maximumFindHistoryItems;
                    }

                    for (int index = 0; index < maxIndex; index++)
                    {
                        var text = settings.Tools.FindHistory[index];
                        textWriter.WriteElementString("Text", text);
                    }

                    textWriter.WriteEndElement();
                }

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("SubtitleSettings", string.Empty);
                textWriter.WriteElementString("SsaFontName", settings.SubtitleSettings.SsaFontName);
                textWriter.WriteElementString("SsaFontSize", settings.SubtitleSettings.SsaFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SsaFontColorArgb", settings.SubtitleSettings.SsaFontColorArgb.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SsaOutline", settings.SubtitleSettings.SsaOutline.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SsaShadow", settings.SubtitleSettings.SsaShadow.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SsaOpaqueBox", settings.SubtitleSettings.SsaOpaqueBox.ToString());
                textWriter.WriteElementString("DCinemaFontFile", settings.SubtitleSettings.DCinemaFontFile);
                textWriter.WriteElementString("DCinemaFontSize", settings.SubtitleSettings.DCinemaFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("DCinemaBottomMargin", settings.SubtitleSettings.DCinemaBottomMargin.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("DCinemaZPosition", settings.SubtitleSettings.DCinemaZPosition.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("DCinemaFadeUpTime", settings.SubtitleSettings.DCinemaFadeUpTime.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("DCinemaFadeDownTime", settings.SubtitleSettings.DCinemaFadeDownTime.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("SamiDisplayTwoClassesAsTwoSubtitles", settings.SubtitleSettings.SamiDisplayTwoClassesAsTwoSubtitles.ToString());
                textWriter.WriteElementString("SamiFullHtmlEncode", settings.SubtitleSettings.SamiHtmlEncodeMode.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("TimedText10TimeCodeFormat", settings.SubtitleSettings.TimedText10TimeCodeFormat);
                textWriter.WriteElementString("FcpFontSize", settings.SubtitleSettings.FcpFontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("FcpFontName", settings.SubtitleSettings.FcpFontName);
                textWriter.WriteElementString("CheetahCaptionAlwayWriteEndTime", settings.SubtitleSettings.CheetahCaptionAlwayWriteEndTime.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("NuendoCharacterListFile", settings.SubtitleSettings.NuendoCharacterListFile);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("Proxy", string.Empty);
                textWriter.WriteElementString("ProxyAddress", settings.Proxy.ProxyAddress);
                textWriter.WriteElementString("UserName", settings.Proxy.UserName);
                textWriter.WriteElementString("Password", settings.Proxy.Password);
                textWriter.WriteElementString("Domain", settings.Proxy.Domain);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("WordLists", string.Empty);
                textWriter.WriteElementString("LastLanguage", settings.WordLists.LastLanguage);
                textWriter.WriteElementString("NamesEtcUrl", settings.WordLists.NamesEtcUrl);
                textWriter.WriteElementString("UseOnlineNamesEtc", settings.WordLists.UseOnlineNamesEtc.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("CommonErrors", string.Empty);
                textWriter.WriteElementString("StartPosition", settings.CommonErrors.StartPosition);
                textWriter.WriteElementString("StartSize", settings.CommonErrors.StartSize);
                textWriter.WriteElementString("EmptyLinesTicked", settings.CommonErrors.EmptyLinesTicked.ToString());
                textWriter.WriteElementString("OverlappingDisplayTimeTicked", settings.CommonErrors.OverlappingDisplayTimeTicked.ToString());
                textWriter.WriteElementString("TooShortDisplayTimeTicked", settings.CommonErrors.TooShortDisplayTimeTicked.ToString());
                textWriter.WriteElementString("TooLongDisplayTimeTicked", settings.CommonErrors.TooLongDisplayTimeTicked.ToString());
                textWriter.WriteElementString("InvalidItalicTagsTicked", settings.CommonErrors.InvalidItalicTagsTicked.ToString());
                textWriter.WriteElementString("BreakLongLinesTicked", settings.CommonErrors.BreakLongLinesTicked.ToString());
                textWriter.WriteElementString("MergeShortLinesTicked", settings.CommonErrors.MergeShortLinesTicked.ToString());
                textWriter.WriteElementString("MergeShortLinesAllTicked", settings.CommonErrors.MergeShortLinesAllTicked.ToString());
                textWriter.WriteElementString("UnneededSpacesTicked", settings.CommonErrors.UnneededSpacesTicked.ToString());
                textWriter.WriteElementString("UnneededPeriodsTicked", settings.CommonErrors.UnneededPeriodsTicked.ToString());
                textWriter.WriteElementString("MissingSpacesTicked", settings.CommonErrors.MissingSpacesTicked.ToString());
                textWriter.WriteElementString("AddMissingQuotesTicked", settings.CommonErrors.AddMissingQuotesTicked.ToString());
                textWriter.WriteElementString("Fix3PlusLinesTicked", settings.CommonErrors.Fix3PlusLinesTicked.ToString());
                textWriter.WriteElementString("FixHyphensTicked", settings.CommonErrors.FixHyphensTicked.ToString());
                textWriter.WriteElementString("FixHyphensAddTicked", settings.CommonErrors.FixHyphensAddTicked.ToString());
                textWriter.WriteElementString("UppercaseIInsideLowercaseWordTicked", settings.CommonErrors.UppercaseIInsideLowercaseWordTicked.ToString());
                textWriter.WriteElementString("DoubleApostropheToQuoteTicked", settings.CommonErrors.DoubleApostropheToQuoteTicked.ToString());
                textWriter.WriteElementString("AddPeriodAfterParagraphTicked", settings.CommonErrors.AddPeriodAfterParagraphTicked.ToString());
                textWriter.WriteElementString("StartWithUppercaseLetterAfterParagraphTicked", settings.CommonErrors.StartWithUppercaseLetterAfterParagraphTicked.ToString());
                textWriter.WriteElementString("StartWithUppercaseLetterAfterPeriodInsideParagraphTicked", settings.CommonErrors.StartWithUppercaseLetterAfterPeriodInsideParagraphTicked.ToString());
                textWriter.WriteElementString("StartWithUppercaseLetterAfterColonTicked", settings.CommonErrors.StartWithUppercaseLetterAfterColonTicked.ToString());
                textWriter.WriteElementString("AloneLowercaseIToUppercaseIEnglishTicked", settings.CommonErrors.AloneLowercaseIToUppercaseIEnglishTicked.ToString());
                textWriter.WriteElementString("FixOcrErrorsViaReplaceListTicked", settings.CommonErrors.FixOcrErrorsViaReplaceListTicked.ToString());
                textWriter.WriteElementString("RemoveSpaceBetweenNumberTicked", settings.CommonErrors.RemoveSpaceBetweenNumberTicked.ToString());
                textWriter.WriteElementString("FixDialogsOnOneLineTicked", settings.CommonErrors.FixDialogsOnOneLineTicked.ToString());
                textWriter.WriteElementString("TurkishAnsiTicked", settings.CommonErrors.TurkishAnsiTicked.ToString());
                textWriter.WriteElementString("DanishLetterITicked", settings.CommonErrors.DanishLetterITicked.ToString());
                textWriter.WriteElementString("SpanishInvertedQuestionAndExclamationMarksTicked", settings.CommonErrors.SpanishInvertedQuestionAndExclamationMarksTicked.ToString());
                textWriter.WriteElementString("FixDoubleDashTicked", settings.CommonErrors.FixDoubleDashTicked.ToString());
                textWriter.WriteElementString("FixDoubleGreaterThanTicked", settings.CommonErrors.FixDoubleGreaterThanTicked.ToString());
                textWriter.WriteElementString("FixEllipsesStartTicked", settings.CommonErrors.FixEllipsesStartTicked.ToString());
                textWriter.WriteElementString("FixMissingOpenBracketTicked", settings.CommonErrors.FixMissingOpenBracketTicked.ToString());
                textWriter.WriteElementString("FixMusicNotationTicked", settings.CommonErrors.FixMusicNotationTicked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("VideoControls", string.Empty);
                textWriter.WriteElementString("CustomSearchText1", settings.VideoControls.CustomSearchText1);
                textWriter.WriteElementString("CustomSearchText2", settings.VideoControls.CustomSearchText2);
                textWriter.WriteElementString("CustomSearchText3", settings.VideoControls.CustomSearchText3);
                textWriter.WriteElementString("CustomSearchText4", settings.VideoControls.CustomSearchText4);
                textWriter.WriteElementString("CustomSearchText5", settings.VideoControls.CustomSearchText5);
                textWriter.WriteElementString("CustomSearchText6", settings.VideoControls.CustomSearchText6);
                textWriter.WriteElementString("CustomSearchUrl1", settings.VideoControls.CustomSearchUrl1);
                textWriter.WriteElementString("CustomSearchUrl2", settings.VideoControls.CustomSearchUrl2);
                textWriter.WriteElementString("CustomSearchUrl3", settings.VideoControls.CustomSearchUrl3);
                textWriter.WriteElementString("CustomSearchUrl4", settings.VideoControls.CustomSearchUrl4);
                textWriter.WriteElementString("CustomSearchUrl5", settings.VideoControls.CustomSearchUrl5);
                textWriter.WriteElementString("CustomSearchUrl6", settings.VideoControls.CustomSearchUrl6);
                textWriter.WriteElementString("LastActiveTab", settings.VideoControls.LastActiveTab);
                textWriter.WriteElementString("WaveformDrawGrid", settings.VideoControls.WaveformDrawGrid.ToString());
                textWriter.WriteElementString("WaveformAllowOverlap", settings.VideoControls.WaveformAllowOverlap.ToString());
                textWriter.WriteElementString("WaveformFocusOnMouseEnter", settings.VideoControls.WaveformFocusOnMouseEnter.ToString());
                textWriter.WriteElementString("WaveformListViewFocusOnMouseEnter", settings.VideoControls.WaveformListViewFocusOnMouseEnter.ToString());
                textWriter.WriteElementString("WaveformBorderHitMs", settings.VideoControls.WaveformBorderHitMs.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformGridColor", settings.VideoControls.WaveformGridColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformColor", settings.VideoControls.WaveformColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformSelectedColor", settings.VideoControls.WaveformSelectedColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformBackgroundColor", settings.VideoControls.WaveformBackgroundColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformTextColor", settings.VideoControls.WaveformTextColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformTextSize", settings.VideoControls.WaveformTextSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformTextBold", settings.VideoControls.WaveformTextBold.ToString());
                textWriter.WriteElementString("WaveformDoubleClickOnNonParagraphAction", settings.VideoControls.WaveformDoubleClickOnNonParagraphAction);
                textWriter.WriteElementString("WaveformRightClickOnNonParagraphAction", settings.VideoControls.WaveformRightClickOnNonParagraphAction);
                textWriter.WriteElementString("WaveformMouseWheelScrollUpIsForward", settings.VideoControls.WaveformMouseWheelScrollUpIsForward.ToString());
                textWriter.WriteElementString("GenerateSpectrogram", settings.VideoControls.GenerateSpectrogram.ToString());
                textWriter.WriteElementString("SpectrogramAppearance", settings.VideoControls.SpectrogramAppearance);
                textWriter.WriteElementString("WaveformMinimumSampleRate", settings.VideoControls.WaveformMinimumSampleRate.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformSeeksSilenceDurationSeconds", settings.VideoControls.WaveformSeeksSilenceDurationSeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("WaveformSeeksSilenceMaxVolume", settings.VideoControls.WaveformSeeksSilenceMaxVolume.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("NetworkSettings", string.Empty);
                textWriter.WriteElementString("SessionKey", settings.NetworkSettings.SessionKey);
                textWriter.WriteElementString("UserName", settings.NetworkSettings.UserName);
                textWriter.WriteElementString("WebServiceUrl", settings.NetworkSettings.WebServiceUrl);
                textWriter.WriteElementString("PollIntervalSeconds", settings.NetworkSettings.PollIntervalSeconds.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("VobSubOcr", string.Empty);
                textWriter.WriteElementString("XOrMorePixelsMakesSpace", settings.VobSubOcr.XOrMorePixelsMakesSpace.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("AllowDifferenceInPercent", settings.VobSubOcr.AllowDifferenceInPercent.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("BlurayAllowDifferenceInPercent", settings.VobSubOcr.BlurayAllowDifferenceInPercent.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("LastImageCompareFolder", settings.VobSubOcr.LastImageCompareFolder);
                textWriter.WriteElementString("LastModiLanguageId", settings.VobSubOcr.LastModiLanguageId.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("LastOcrMethod", settings.VobSubOcr.LastOcrMethod);
                textWriter.WriteElementString("TesseractLastLanguage", settings.VobSubOcr.TesseractLastLanguage);
                textWriter.WriteElementString("UseModiInTesseractForUnknownWords", settings.VobSubOcr.UseModiInTesseractForUnknownWords.ToString());
                textWriter.WriteElementString("UseItalicsInTesseract", settings.VobSubOcr.UseItalicsInTesseract.ToString());
                textWriter.WriteElementString("UseMusicSymbolsInTesseract", settings.VobSubOcr.UseMusicSymbolsInTesseract.ToString());
                textWriter.WriteElementString("RightToLeft", settings.VobSubOcr.RightToLeft.ToString());
                textWriter.WriteElementString("TopToBottom", settings.VobSubOcr.TopToBottom.ToString());
                textWriter.WriteElementString("DefaultMillisecondsForUnknownDurations", settings.VobSubOcr.DefaultMillisecondsForUnknownDurations.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("PromptForUnknownWords", settings.VobSubOcr.PromptForUnknownWords.ToString());
                textWriter.WriteElementString("GuessUnknownWords", settings.VobSubOcr.GuessUnknownWords.ToString());
                textWriter.WriteElementString("AutoBreakSubtitleIfMoreThanTwoLines", settings.VobSubOcr.AutoBreakSubtitleIfMoreThanTwoLines.ToString());
                textWriter.WriteElementString("ItalicFactor", settings.VobSubOcr.ItalicFactor.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("LineOcrDraw", settings.VobSubOcr.LineOcrDraw.ToString());
                textWriter.WriteElementString("LineOcrAdvancedItalic", settings.VobSubOcr.LineOcrAdvancedItalic.ToString());
                textWriter.WriteElementString("LineOcrLastLanguages", settings.VobSubOcr.LineOcrLastLanguages);
                textWriter.WriteElementString("LineOcrLastSpellCheck", settings.VobSubOcr.LineOcrLastSpellCheck);
                textWriter.WriteElementString("LineOcrXOrMorePixelsMakesSpace", settings.VobSubOcr.LineOcrXOrMorePixelsMakesSpace.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("LineOcrMinLineHeight", settings.VobSubOcr.LineOcrMinLineHeight.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("LineOcrMaxLineHeight", settings.VobSubOcr.LineOcrMaxLineHeight.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("MultipleSearchAndReplaceList", string.Empty);
                foreach (var item in settings.MultipleSearchAndReplaceList)
                {
                    textWriter.WriteStartElement("MultipleSearchAndReplaceItem", string.Empty);
                    textWriter.WriteElementString("Enabled", item.Enabled.ToString());
                    textWriter.WriteElementString("FindWhat", item.FindWhat);
                    textWriter.WriteElementString("ReplaceWith", item.ReplaceWith);
                    textWriter.WriteElementString("SearchType", item.SearchType);
                    textWriter.WriteEndElement();
                }

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("Shortcuts", string.Empty);
                textWriter.WriteElementString("GeneralGoToFirstSelectedLine", settings.Shortcuts.GeneralGoToFirstSelectedLine);
                textWriter.WriteElementString("GeneralGoToNextEmptyLine", settings.Shortcuts.GeneralGoToNextEmptyLine);
                textWriter.WriteElementString("GeneralMergeSelectedLines", settings.Shortcuts.GeneralMergeSelectedLines);
                textWriter.WriteElementString("GeneralMergeSelectedLinesOnlyFirstText", settings.Shortcuts.GeneralMergeSelectedLinesOnlyFirstText);
                textWriter.WriteElementString("GeneralToggleTranslationMode", settings.Shortcuts.GeneralToggleTranslationMode);
                textWriter.WriteElementString("GeneralSwitchOriginalAndTranslation", settings.Shortcuts.GeneralSwitchOriginalAndTranslation);
                textWriter.WriteElementString("GeneralMergeOriginalAndTranslation", settings.Shortcuts.GeneralMergeOriginalAndTranslation);
                textWriter.WriteElementString("GeneralGoToNextSubtitle", settings.Shortcuts.GeneralGoToNextSubtitle);
                textWriter.WriteElementString("GeneralGoToPrevSubtitle", settings.Shortcuts.GeneralGoToPrevSubtitle);
                textWriter.WriteElementString("GeneralGoToEndOfCurrentSubtitle", settings.Shortcuts.GeneralGoToEndOfCurrentSubtitle);
                textWriter.WriteElementString("GeneralGoToStartOfCurrentSubtitle", settings.Shortcuts.GeneralGoToStartOfCurrentSubtitle);
                textWriter.WriteElementString("GeneralPlayFirstSelected", settings.Shortcuts.GeneralPlayFirstSelected);
                textWriter.WriteElementString("MainFileNew", settings.Shortcuts.MainFileNew);
                textWriter.WriteElementString("MainFileOpen", settings.Shortcuts.MainFileOpen);
                textWriter.WriteElementString("MainFileOpenKeepVideo", settings.Shortcuts.MainFileOpenKeepVideo);
                textWriter.WriteElementString("MainFileSave", settings.Shortcuts.MainFileSave);
                textWriter.WriteElementString("MainFileSaveOriginal", settings.Shortcuts.MainFileSaveOriginal);
                textWriter.WriteElementString("MainFileSaveOriginalAs", settings.Shortcuts.MainFileSaveOriginalAs);
                textWriter.WriteElementString("MainFileSaveAs", settings.Shortcuts.MainFileSaveAs);
                textWriter.WriteElementString("MainFileSaveAll", settings.Shortcuts.MainFileSaveAll);
                textWriter.WriteElementString("MainFileExportEbu", settings.Shortcuts.MainFileExportEbu);
                textWriter.WriteElementString("MainEditUndo", settings.Shortcuts.MainEditUndo);
                textWriter.WriteElementString("MainEditRedo", settings.Shortcuts.MainEditRedo);
                textWriter.WriteElementString("MainEditFind", settings.Shortcuts.MainEditFind);
                textWriter.WriteElementString("MainEditFindNext", settings.Shortcuts.MainEditFindNext);
                textWriter.WriteElementString("MainEditReplace", settings.Shortcuts.MainEditReplace);
                textWriter.WriteElementString("MainEditMultipleReplace", settings.Shortcuts.MainEditMultipleReplace);
                textWriter.WriteElementString("MainEditGoToLineNumber", settings.Shortcuts.MainEditGoToLineNumber);
                textWriter.WriteElementString("MainEditRightToLeft", settings.Shortcuts.MainEditRightToLeft);
                textWriter.WriteElementString("MainToolsFixCommonErrors", settings.Shortcuts.MainToolsFixCommonErrors);
                textWriter.WriteElementString("MainToolsFixCommonErrorsPreview", settings.Shortcuts.MainToolsFixCommonErrorsPreview);
                textWriter.WriteElementString("MainToolsMergeShortLines", settings.Shortcuts.MainToolsMergeShortLines);
                textWriter.WriteElementString("MainToolsSplitLongLines", settings.Shortcuts.MainToolsSplitLongLines);
                textWriter.WriteElementString("MainToolsRenumber", settings.Shortcuts.MainToolsRenumber);
                textWriter.WriteElementString("MainToolsRemoveTextForHI", settings.Shortcuts.MainToolsRemoveTextForHI);
                textWriter.WriteElementString("MainToolsChangeCasing", settings.Shortcuts.MainToolsChangeCasing);
                textWriter.WriteElementString("MainToolsAutoDuration", settings.Shortcuts.MainToolsAutoDuration);
                textWriter.WriteElementString("MainToolsBatchConvert", settings.Shortcuts.MainToolsBatchConvert);
                textWriter.WriteElementString("MainToolsBeamer", settings.Shortcuts.MainToolsBeamer);
                textWriter.WriteElementString("MainToolsToggleTranslationOriginalInPreviews", settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews);
                textWriter.WriteElementString("MainEditInverseSelection", settings.Shortcuts.MainEditInverseSelection);
                textWriter.WriteElementString("MainEditModifySelection", settings.Shortcuts.MainEditModifySelection);
                textWriter.WriteElementString("MainVideoPause", settings.Shortcuts.MainVideoPause);
                textWriter.WriteElementString("MainVideoPlayPauseToggle", settings.Shortcuts.MainVideoPlayPauseToggle);
                textWriter.WriteElementString("MainVideoShowHideVideo", settings.Shortcuts.MainVideoShowHideVideo);
                textWriter.WriteElementString("MainVideoToggleVideoControls", settings.Shortcuts.MainVideoToggleVideoControls);
                textWriter.WriteElementString("MainVideo1FrameLeft", settings.Shortcuts.MainVideo1FrameLeft);
                textWriter.WriteElementString("MainVideo1FrameRight", settings.Shortcuts.MainVideo1FrameRight);
                textWriter.WriteElementString("MainVideo100MsLeft", settings.Shortcuts.MainVideo100MsLeft);
                textWriter.WriteElementString("MainVideo100MsRight", settings.Shortcuts.MainVideo100MsRight);
                textWriter.WriteElementString("MainVideo500MsLeft", settings.Shortcuts.MainVideo500MsLeft);
                textWriter.WriteElementString("MainVideo500MsRight", settings.Shortcuts.MainVideo500MsRight);
                textWriter.WriteElementString("MainVideo1000MsLeft", settings.Shortcuts.MainVideo1000MsLeft);
                textWriter.WriteElementString("MainVideo1000MsRight", settings.Shortcuts.MainVideo1000MsRight);
                textWriter.WriteElementString("MainVideoFullscreen", settings.Shortcuts.MainVideoFullscreen);
                textWriter.WriteElementString("MainSpellCheck", settings.Shortcuts.MainSpellCheck);
                textWriter.WriteElementString("MainSpellCheckFindDoubleWords", settings.Shortcuts.MainSpellCheckFindDoubleWords);
                textWriter.WriteElementString("MainSpellCheckAddWordToNames", settings.Shortcuts.MainSpellCheckAddWordToNames);
                textWriter.WriteElementString("MainSynchronizationAdjustTimes", settings.Shortcuts.MainSynchronizationAdjustTimes);
                textWriter.WriteElementString("MainSynchronizationVisualSync", settings.Shortcuts.MainSynchronizationVisualSync);
                textWriter.WriteElementString("MainSynchronizationPointSync", settings.Shortcuts.MainSynchronizationPointSync);
                textWriter.WriteElementString("MainSynchronizationChangeFrameRate", settings.Shortcuts.MainSynchronizationChangeFrameRate);
                textWriter.WriteElementString("MainListViewItalic", settings.Shortcuts.MainListViewItalic);
                textWriter.WriteElementString("MainListViewToggleDashes", settings.Shortcuts.MainListViewToggleDashes);
                textWriter.WriteElementString("MainListViewAlignment", settings.Shortcuts.MainListViewAlignment);
                textWriter.WriteElementString("MainListViewCopyText", settings.Shortcuts.MainListViewCopyText);
                textWriter.WriteElementString("MainListViewCopyTextFromOriginalToCurrent", settings.Shortcuts.MainListViewCopyTextFromOriginalToCurrent);
                textWriter.WriteElementString("MainListViewAutoDuration", settings.Shortcuts.MainListViewAutoDuration);
                textWriter.WriteElementString("MainListViewColumnDeleteText", settings.Shortcuts.MainListViewColumnDeleteText);
                textWriter.WriteElementString("MainListViewColumnInsertText", settings.Shortcuts.MainListViewColumnInsertText);
                textWriter.WriteElementString("MainListViewColumnPaste", settings.Shortcuts.MainListViewColumnPaste);
                textWriter.WriteElementString("MainListViewFocusWaveform", settings.Shortcuts.MainListViewFocusWaveform);
                textWriter.WriteElementString("MainListViewGoToNextError", settings.Shortcuts.MainListViewGoToNextError);
                textWriter.WriteElementString("MainEditReverseStartAndEndingForRTL", settings.Shortcuts.MainEditReverseStartAndEndingForRTL);
                textWriter.WriteElementString("MainTextBoxItalic", settings.Shortcuts.MainTextBoxItalic);
                textWriter.WriteElementString("MainTextBoxSplitAtCursor", settings.Shortcuts.MainTextBoxSplitAtCursor);
                textWriter.WriteElementString("MainTextBoxMoveLastWordDown", settings.Shortcuts.MainTextBoxMoveLastWordDown);
                textWriter.WriteElementString("MainTextBoxMoveFirstWordFromNextUp", settings.Shortcuts.MainTextBoxMoveFirstWordFromNextUp);
                textWriter.WriteElementString("MainTextBoxSelectionToLower", settings.Shortcuts.MainTextBoxSelectionToLower);
                textWriter.WriteElementString("MainTextBoxSelectionToUpper", settings.Shortcuts.MainTextBoxSelectionToUpper);
                textWriter.WriteElementString("MainTextBoxToggleAutoDuration", settings.Shortcuts.MainTextBoxToggleAutoDuration);
                textWriter.WriteElementString("MainCreateInsertSubAtVideoPos", settings.Shortcuts.MainCreateInsertSubAtVideoPos);
                textWriter.WriteElementString("MainCreatePlayFromJustBefore", settings.Shortcuts.MainCreatePlayFromJustBefore);
                textWriter.WriteElementString("MainCreateSetStart", settings.Shortcuts.MainCreateSetStart);
                textWriter.WriteElementString("MainCreateSetEnd", settings.Shortcuts.MainCreateSetEnd);
                textWriter.WriteElementString("MainCreateSetEndAddNewAndGoToNew", settings.Shortcuts.MainCreateSetEndAddNewAndGoToNew);
                textWriter.WriteElementString("MainCreateStartDownEndUp", settings.Shortcuts.MainCreateStartDownEndUp);
                textWriter.WriteElementString("MainAdjustSetStartAndOffsetTheRest", settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest);
                textWriter.WriteElementString("MainAdjustSetEndAndOffsetTheRest", settings.Shortcuts.MainAdjustSetEndAndOffsetTheRest);
                textWriter.WriteElementString("MainAdjustSetEndAndOffsetTheRestAndGoToNext", settings.Shortcuts.MainAdjustSetEndAndOffsetTheRestAndGoToNext);
                textWriter.WriteElementString("MainAdjustSetEndAndGotoNext", settings.Shortcuts.MainAdjustSetEndAndGotoNext);
                textWriter.WriteElementString("MainAdjustViaEndAutoStartAndGoToNext", settings.Shortcuts.MainAdjustViaEndAutoStartAndGoToNext);
                textWriter.WriteElementString("MainAdjustSetStartAutoDurationAndGoToNext", settings.Shortcuts.MainAdjustSetStartAutoDurationAndGoToNext);
                textWriter.WriteElementString("MainAdjustSetEndNextStartAndGoToNext", settings.Shortcuts.MainAdjustSetEndNextStartAndGoToNext);
                textWriter.WriteElementString("MainAdjustStartDownEndUpAndGoToNext", settings.Shortcuts.MainAdjustStartDownEndUpAndGoToNext);
                textWriter.WriteElementString("MainAdjustSetStart", settings.Shortcuts.MainAdjustSetStart);
                textWriter.WriteElementString("MainAdjustSetStartKeepDuration", settings.Shortcuts.MainAdjustSetStartKeepDuration);
                textWriter.WriteElementString("MainAdjustSetEnd", settings.Shortcuts.MainAdjustSetEnd);
                textWriter.WriteElementString("MainAdjustSelected100MsForward", settings.Shortcuts.MainAdjustSelected100MsForward);
                textWriter.WriteElementString("MainAdjustSelected100MsBack", settings.Shortcuts.MainAdjustSelected100MsBack);
                textWriter.WriteElementString("MainInsertAfter", settings.Shortcuts.MainInsertAfter);
                textWriter.WriteElementString("MainTextBoxInsertAfter", settings.Shortcuts.MainTextBoxInsertAfter);
                textWriter.WriteElementString("MainTextBoxAutoBreak", settings.Shortcuts.MainTextBoxAutoBreak);
                textWriter.WriteElementString("MainTextBoxUnbreak", settings.Shortcuts.MainTextBoxUnbreak);
                textWriter.WriteElementString("MainWaveformInsertAtCurrentPosition", settings.Shortcuts.MainWaveformInsertAtCurrentPosition);
                textWriter.WriteElementString("MainInsertBefore", settings.Shortcuts.MainInsertBefore);
                textWriter.WriteElementString("MainMergeDialog", settings.Shortcuts.MainMergeDialog);
                textWriter.WriteElementString("MainToggleFocus", settings.Shortcuts.MainToggleFocus);
                textWriter.WriteElementString("WaveformVerticalZoom", settings.Shortcuts.WaveformVerticalZoom);
                textWriter.WriteElementString("WaveformVerticalZoomOut", settings.Shortcuts.WaveformVerticalZoomOut);
                textWriter.WriteElementString("WaveformZoomIn", settings.Shortcuts.WaveformZoomIn);
                textWriter.WriteElementString("WaveformZoomOut", settings.Shortcuts.WaveformZoomOut);
                textWriter.WriteElementString("WaveformPlaySelection", settings.Shortcuts.WaveformPlaySelection);
                textWriter.WriteElementString("WaveformSearchSilenceForward", settings.Shortcuts.WaveformSearchSilenceForward);
                textWriter.WriteElementString("WaveformSearchSilenceBack", settings.Shortcuts.WaveformSearchSilenceBack);
                textWriter.WriteElementString("WaveformAddTextHere", settings.Shortcuts.WaveformAddTextHere);
                textWriter.WriteElementString("WaveformFocusListView", settings.Shortcuts.WaveformFocusListView);
                textWriter.WriteElementString("MainTranslateCustomSearch1", settings.Shortcuts.MainTranslateCustomSearch1);
                textWriter.WriteElementString("MainTranslateCustomSearch2", settings.Shortcuts.MainTranslateCustomSearch2);
                textWriter.WriteElementString("MainTranslateCustomSearch3", settings.Shortcuts.MainTranslateCustomSearch3);
                textWriter.WriteElementString("MainTranslateCustomSearch4", settings.Shortcuts.MainTranslateCustomSearch4);
                textWriter.WriteElementString("MainTranslateCustomSearch5", settings.Shortcuts.MainTranslateCustomSearch5);
                textWriter.WriteElementString("MainTranslateCustomSearch6", settings.Shortcuts.MainTranslateCustomSearch6);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("RemoveTextForHearingImpaired", string.Empty);
                textWriter.WriteElementString("RemoveTextBetweenBrackets", settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets.ToString());
                textWriter.WriteElementString("RemoveTextBetweenParentheses", settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses.ToString());
                textWriter.WriteElementString("RemoveTextBetweenCurlyBrackets", settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets.ToString());
                textWriter.WriteElementString("RemoveTextBetweenQuestionMarks", settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks.ToString());
                textWriter.WriteElementString("RemoveTextBetweenCustom", settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustom.ToString());
                textWriter.WriteElementString("RemoveTextBetweenCustomBefore", settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomBefore);
                textWriter.WriteElementString("RemoveTextBetweenCustomAfter", settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomAfter);
                textWriter.WriteElementString("RemoveTextBetweenOnlySeperateLines", settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines.ToString());
                textWriter.WriteElementString("RemoveTextBeforeColon", settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon.ToString());
                textWriter.WriteElementString("RemoveTextBeforeColonOnlyIfUppercase", settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase.ToString());
                textWriter.WriteElementString("RemoveTextBeforeColonOnlyOnSeparateLine", settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine.ToString());
                textWriter.WriteElementString("RemoveInterjections", settings.RemoveTextForHearingImpaired.RemoveInterjections.ToString());
                textWriter.WriteElementString("RemoveIfAllUppercase", settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase.ToString());
                textWriter.WriteElementString("RemoveIfContains", settings.RemoveTextForHearingImpaired.RemoveIfContains.ToString());
                textWriter.WriteElementString("RemoveIfContainsText", settings.RemoveTextForHearingImpaired.RemoveIfContainsText);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("SubtitleBeaming", string.Empty);
                textWriter.WriteElementString("FontName", settings.SubtitleBeaming.FontName);
                textWriter.WriteElementString("FontColor", settings.SubtitleBeaming.FontColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("FontSize", settings.SubtitleBeaming.FontSize.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("BorderColor", settings.SubtitleBeaming.BorderColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                textWriter.WriteElementString("BorderWidth", settings.SubtitleBeaming.BorderWidth.ToString(CultureInfo.InvariantCulture));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteEndDocument();
                textWriter.Flush();

                try
                {
                    File.WriteAllText(fileName, sb.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\""), Encoding.UTF8);
                }
                catch
                {
                }
            }
        }
    }
}