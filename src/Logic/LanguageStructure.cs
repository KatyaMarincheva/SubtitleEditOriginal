// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageStructure.cs" company="">
//   
// </copyright>
// <summary>
//   The language structure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    // The language classes are built for easy xml-serialization (makes save/load code simple)
    /// <summary>
    /// The language structure.
    /// </summary>
    public class LanguageStructure
    {
        /// <summary>
        /// The general.
        /// </summary>
        public class General
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// Gets or sets the translated by.
            /// </summary>
            public string TranslatedBy { get; set; }

            /// <summary>
            /// Gets or sets the culture name.
            /// </summary>
            public string CultureName { get; set; }

            /// <summary>
            /// Gets or sets the help file.
            /// </summary>
            public string HelpFile { get; set; }

            /// <summary>
            /// Gets or sets the ok.
            /// </summary>
            public string Ok { get; set; }

            /// <summary>
            /// Gets or sets the cancel.
            /// </summary>
            public string Cancel { get; set; }

            /// <summary>
            /// Gets or sets the apply.
            /// </summary>
            public string Apply { get; set; }

            /// <summary>
            /// Gets or sets the none.
            /// </summary>
            public string None { get; set; }

            /// <summary>
            /// Gets or sets the all.
            /// </summary>
            public string All { get; set; }

            /// <summary>
            /// Gets or sets the preview.
            /// </summary>
            public string Preview { get; set; }

            /// <summary>
            /// Gets or sets the subtitle files.
            /// </summary>
            public string SubtitleFiles { get; set; }

            /// <summary>
            /// Gets or sets the all files.
            /// </summary>
            public string AllFiles { get; set; }

            /// <summary>
            /// Gets or sets the video files.
            /// </summary>
            public string VideoFiles { get; set; }

            /// <summary>
            /// Gets or sets the audio files.
            /// </summary>
            public string AudioFiles { get; set; }

            /// <summary>
            /// Gets or sets the open subtitle.
            /// </summary>
            public string OpenSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the open video file.
            /// </summary>
            public string OpenVideoFile { get; set; }

            /// <summary>
            /// Gets or sets the open video file title.
            /// </summary>
            public string OpenVideoFileTitle { get; set; }

            /// <summary>
            /// Gets or sets the no video loaded.
            /// </summary>
            public string NoVideoLoaded { get; set; }

            /// <summary>
            /// Gets or sets the video information.
            /// </summary>
            public string VideoInformation { get; set; }

            /// <summary>
            /// Gets or sets the position x.
            /// </summary>
            public string PositionX { get; set; }

            /// <summary>
            /// Gets or sets the start time.
            /// </summary>
            public string StartTime { get; set; }

            /// <summary>
            /// Gets or sets the end time.
            /// </summary>
            public string EndTime { get; set; }

            /// <summary>
            /// Gets or sets the duration.
            /// </summary>
            public string Duration { get; set; }

            /// <summary>
            /// Gets or sets the number symbol.
            /// </summary>
            public string NumberSymbol { get; set; }

            /// <summary>
            /// Gets or sets the number.
            /// </summary>
            public string Number { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the hour minutes seconds milliseconds.
            /// </summary>
            public string HourMinutesSecondsMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the bold.
            /// </summary>
            public string Bold { get; set; }

            /// <summary>
            /// Gets or sets the italic.
            /// </summary>
            public string Italic { get; set; }

            /// <summary>
            /// Gets or sets the underline.
            /// </summary>
            public string Underline { get; set; }

            /// <summary>
            /// Gets or sets the visible.
            /// </summary>
            public string Visible { get; set; }

            /// <summary>
            /// Gets or sets the frame rate.
            /// </summary>
            public string FrameRate { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the file name x and size.
            /// </summary>
            public string FileNameXAndSize { get; set; }

            /// <summary>
            /// Gets or sets the resolution x.
            /// </summary>
            public string ResolutionX { get; set; }

            /// <summary>
            /// Gets or sets the frame rate x.
            /// </summary>
            public string FrameRateX { get; set; }

            /// <summary>
            /// Gets or sets the total frames x.
            /// </summary>
            public string TotalFramesX { get; set; }

            /// <summary>
            /// Gets or sets the video encoding x.
            /// </summary>
            public string VideoEncodingX { get; set; }

            /// <summary>
            /// Gets or sets the single line lengths.
            /// </summary>
            public string SingleLineLengths { get; set; }

            /// <summary>
            /// Gets or sets the total length x.
            /// </summary>
            public string TotalLengthX { get; set; }

            /// <summary>
            /// Gets or sets the total length x split line.
            /// </summary>
            public string TotalLengthXSplitLine { get; set; }

            /// <summary>
            /// Gets or sets the split line.
            /// </summary>
            public string SplitLine { get; set; }

            /// <summary>
            /// Gets or sets the not available.
            /// </summary>
            public string NotAvailable { get; set; }

            /// <summary>
            /// Gets or sets the overlap previous line x.
            /// </summary>
            public string OverlapPreviousLineX { get; set; }

            /// <summary>
            /// Gets or sets the overlap x.
            /// </summary>
            public string OverlapX { get; set; }

            /// <summary>
            /// Gets or sets the overlap next x.
            /// </summary>
            public string OverlapNextX { get; set; }

            /// <summary>
            /// Gets or sets the negative.
            /// </summary>
            public string Negative { get; set; }

            /// <summary>
            /// Gets or sets the regular expression is not valid.
            /// </summary>
            public string RegularExpressionIsNotValid { get; set; }

            /// <summary>
            /// Gets or sets the subtitle saved.
            /// </summary>
            public string SubtitleSaved { get; set; }

            /// <summary>
            /// Gets or sets the current subtitle.
            /// </summary>
            public string CurrentSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the original text.
            /// </summary>
            public string OriginalText { get; set; }

            /// <summary>
            /// Gets or sets the open original subtitle file.
            /// </summary>
            public string OpenOriginalSubtitleFile { get; set; }

            /// <summary>
            /// Gets or sets the please wait.
            /// </summary>
            public string PleaseWait { get; set; }

            /// <summary>
            /// Gets or sets the session key.
            /// </summary>
            public string SessionKey { get; set; }

            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// Gets or sets the user name already in use.
            /// </summary>
            public string UserNameAlreadyInUse { get; set; }

            /// <summary>
            /// Gets or sets the web service url.
            /// </summary>
            public string WebServiceUrl { get; set; }

            /// <summary>
            /// Gets or sets the ip.
            /// </summary>
            public string IP { get; set; }

            /// <summary>
            /// Gets or sets the video window title.
            /// </summary>
            public string VideoWindowTitle { get; set; }

            /// <summary>
            /// Gets or sets the audio window title.
            /// </summary>
            public string AudioWindowTitle { get; set; }

            /// <summary>
            /// Gets or sets the controls window title.
            /// </summary>
            public string ControlsWindowTitle { get; set; }

            /// <summary>
            /// Gets or sets the advanced.
            /// </summary>
            public string Advanced { get; set; }

            /// <summary>
            /// Gets or sets the style.
            /// </summary>
            public string Style { get; set; }

            /// <summary>
            /// Gets or sets the style language.
            /// </summary>
            public string StyleLanguage { get; set; }

            /// <summary>
            /// Gets or sets the character.
            /// </summary>
            public string Character { get; set; }

            /// <summary>
            /// Gets or sets the class.
            /// </summary>
            public string Class { get; set; }

            /// <summary>
            /// Gets or sets the general text.
            /// </summary>
            public string GeneralText { get; set; }

            /// <summary>
            /// Gets or sets the line number.
            /// </summary>
            public string LineNumber { get; set; }

            /// <summary>
            /// Gets or sets the before.
            /// </summary>
            public string Before { get; set; }

            /// <summary>
            /// Gets or sets the after.
            /// </summary>
            public string After { get; set; }

            /// <summary>
            /// Gets or sets the size.
            /// </summary>
            public string Size { get; set; }
        }

        /// <summary>
        /// The about.
        /// </summary>
        public class About
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the about text 1.
            /// </summary>
            public string AboutText1 { get; set; }
        }

        /// <summary>
        /// The add to names.
        /// </summary>
        public class AddToNames
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// The add to ocr replace list.
        /// </summary>
        public class AddToOcrReplaceList
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// The add to user dictionary.
        /// </summary>
        public class AddToUserDictionary
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// The add waveform.
        /// </summary>
        public class AddWaveform
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the source video file.
            /// </summary>
            public string SourceVideoFile { get; set; }

            /// <summary>
            /// Gets or sets the generate waveform data.
            /// </summary>
            public string GenerateWaveformData { get; set; }

            /// <summary>
            /// Gets or sets the please wait.
            /// </summary>
            public string PleaseWait { get; set; }

            /// <summary>
            /// Gets or sets the vlc media player not found title.
            /// </summary>
            public string VlcMediaPlayerNotFoundTitle { get; set; }

            /// <summary>
            /// Gets or sets the vlc media player not found.
            /// </summary>
            public string VlcMediaPlayerNotFound { get; set; }

            /// <summary>
            /// Gets or sets the go to vlc media player home page.
            /// </summary>
            public string GoToVlcMediaPlayerHomePage { get; set; }

            /// <summary>
            /// Gets or sets the generating peak file.
            /// </summary>
            public string GeneratingPeakFile { get; set; }

            /// <summary>
            /// Gets or sets the generating spectrogram.
            /// </summary>
            public string GeneratingSpectrogram { get; set; }

            /// <summary>
            /// Gets or sets the extracting seconds.
            /// </summary>
            public string ExtractingSeconds { get; set; }

            /// <summary>
            /// Gets or sets the extracting minutes.
            /// </summary>
            public string ExtractingMinutes { get; set; }

            /// <summary>
            /// Gets or sets the wave file not found.
            /// </summary>
            public string WaveFileNotFound { get; set; }

            /// <summary>
            /// Gets or sets the wave file malformed.
            /// </summary>
            public string WaveFileMalformed { get; set; }

            /// <summary>
            /// Gets or sets the low disk space.
            /// </summary>
            public string LowDiskSpace { get; set; }

            /// <summary>
            /// Gets or sets the free disk space.
            /// </summary>
            public string FreeDiskSpace { get; set; }
        }

        /// <summary>
        /// The add waveform batch.
        /// </summary>
        public class AddWaveformBatch
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the extracting audio.
            /// </summary>
            public string ExtractingAudio { get; set; }

            /// <summary>
            /// Gets or sets the calculating.
            /// </summary>
            public string Calculating { get; set; }

            /// <summary>
            /// Gets or sets the done.
            /// </summary>
            public string Done { get; set; }

            /// <summary>
            /// Gets or sets the error.
            /// </summary>
            public string Error { get; set; }
        }

        /// <summary>
        /// The adjust display duration.
        /// </summary>
        public class AdjustDisplayDuration
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the adjust via.
            /// </summary>
            public string AdjustVia { get; set; }

            /// <summary>
            /// Gets or sets the seconds.
            /// </summary>
            public string Seconds { get; set; }

            /// <summary>
            /// Gets or sets the percent.
            /// </summary>
            public string Percent { get; set; }

            /// <summary>
            /// Gets or sets the recalculate.
            /// </summary>
            public string Recalculate { get; set; }

            /// <summary>
            /// Gets or sets the add seconds.
            /// </summary>
            public string AddSeconds { get; set; }

            /// <summary>
            /// Gets or sets the set as percent.
            /// </summary>
            public string SetAsPercent { get; set; }

            /// <summary>
            /// Gets or sets the note.
            /// </summary>
            public string Note { get; set; }

            /// <summary>
            /// Gets or sets the please select a value from the drop down list.
            /// </summary>
            public string PleaseSelectAValueFromTheDropDownList { get; set; }

            /// <summary>
            /// Gets or sets the please choose.
            /// </summary>
            public string PleaseChoose { get; set; }
        }

        /// <summary>
        /// The apply duration limits.
        /// </summary>
        public class ApplyDurationLimits
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the fixes available.
            /// </summary>
            public string FixesAvailable { get; set; }

            /// <summary>
            /// Gets or sets the unable to fix.
            /// </summary>
            public string UnableToFix { get; set; }
        }

        /// <summary>
        /// The auto break unbreak lines.
        /// </summary>
        public class AutoBreakUnbreakLines
        {
            /// <summary>
            /// Gets or sets the title auto break.
            /// </summary>
            public string TitleAutoBreak { get; set; }

            /// <summary>
            /// Gets or sets the title unbreak.
            /// </summary>
            public string TitleUnbreak { get; set; }

            /// <summary>
            /// Gets or sets the lines found x.
            /// </summary>
            public string LinesFoundX { get; set; }

            /// <summary>
            /// Gets or sets the only break lines longer than.
            /// </summary>
            public string OnlyBreakLinesLongerThan { get; set; }

            /// <summary>
            /// Gets or sets the only unbreak lines longer than.
            /// </summary>
            public string OnlyUnbreakLinesLongerThan { get; set; }
        }

        /// <summary>
        /// The batch convert.
        /// </summary>
        public class BatchConvert
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the input.
            /// </summary>
            public string Input { get; set; }

            /// <summary>
            /// Gets or sets the input description.
            /// </summary>
            public string InputDescription { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            public string Status { get; set; }

            /// <summary>
            /// Gets or sets the output.
            /// </summary>
            public string Output { get; set; }

            /// <summary>
            /// Gets or sets the choose output folder.
            /// </summary>
            public string ChooseOutputFolder { get; set; }

            /// <summary>
            /// Gets or sets the overwrite existing files.
            /// </summary>
            public string OverwriteExistingFiles { get; set; }

            /// <summary>
            /// Gets or sets the style.
            /// </summary>
            public string Style { get; set; }

            /// <summary>
            /// Gets or sets the convert options.
            /// </summary>
            public string ConvertOptions { get; set; }

            /// <summary>
            /// Gets or sets the remove formatting.
            /// </summary>
            public string RemoveFormatting { get; set; }

            /// <summary>
            /// Gets or sets the remove text for hi.
            /// </summary>
            public string RemoveTextForHI { get; set; }

            /// <summary>
            /// Gets or sets the overwrite original files.
            /// </summary>
            public string OverwriteOriginalFiles { get; set; }

            /// <summary>
            /// Gets or sets the redo casing.
            /// </summary>
            public string RedoCasing { get; set; }

            /// <summary>
            /// Gets or sets the convert.
            /// </summary>
            public string Convert { get; set; }

            /// <summary>
            /// Gets or sets the nothing to convert.
            /// </summary>
            public string NothingToConvert { get; set; }

            /// <summary>
            /// Gets or sets the please choose output folder.
            /// </summary>
            public string PleaseChooseOutputFolder { get; set; }

            /// <summary>
            /// Gets or sets the not converted.
            /// </summary>
            public string NotConverted { get; set; }

            /// <summary>
            /// Gets or sets the converted.
            /// </summary>
            public string Converted { get; set; }

            /// <summary>
            /// Gets or sets the converted x.
            /// </summary>
            public string ConvertedX { get; set; }

            /// <summary>
            /// Gets or sets the settings.
            /// </summary>
            public string Settings { get; set; }

            /// <summary>
            /// Gets or sets the split long lines.
            /// </summary>
            public string SplitLongLines { get; set; }

            /// <summary>
            /// Gets or sets the auto balance.
            /// </summary>
            public string AutoBalance { get; set; }

            /// <summary>
            /// Gets or sets the scan folder.
            /// </summary>
            public string ScanFolder { get; set; }

            /// <summary>
            /// Gets or sets the scanning folder.
            /// </summary>
            public string ScanningFolder { get; set; }

            /// <summary>
            /// Gets or sets the recursive.
            /// </summary>
            public string Recursive { get; set; }

            /// <summary>
            /// Gets or sets the set min ms between subtitles.
            /// </summary>
            public string SetMinMsBetweenSubtitles { get; set; }

            /// <summary>
            /// Gets or sets the plain text.
            /// </summary>
            public string PlainText { get; set; }

            /// <summary>
            /// Gets or sets the ocr.
            /// </summary>
            public string Ocr { get; set; }

            /// <summary>
            /// Gets or sets the filter.
            /// </summary>
            public string Filter { get; set; }

            /// <summary>
            /// Gets or sets the filter skipped.
            /// </summary>
            public string FilterSkipped { get; set; }

            /// <summary>
            /// Gets or sets the filter srt no utf 8 bom.
            /// </summary>
            public string FilterSrtNoUtf8BOM { get; set; }

            /// <summary>
            /// Gets or sets the filter more than two lines.
            /// </summary>
            public string FilterMoreThanTwoLines { get; set; }

            /// <summary>
            /// Gets or sets the filter contains.
            /// </summary>
            public string FilterContains { get; set; }

            /// <summary>
            /// Gets or sets the fix common errors error x.
            /// </summary>
            public string FixCommonErrorsErrorX { get; set; }

            /// <summary>
            /// Gets or sets the multiple replace error x.
            /// </summary>
            public string MultipleReplaceErrorX { get; set; }

            /// <summary>
            /// Gets or sets the auto balance error x.
            /// </summary>
            public string AutoBalanceErrorX { get; set; }
        }

        /// <summary>
        /// The beamer.
        /// </summary>
        public class Beamer
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }
        }

        /// <summary>
        /// The change casing.
        /// </summary>
        public class ChangeCasing
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the change casing to.
            /// </summary>
            public string ChangeCasingTo { get; set; }

            /// <summary>
            /// Gets or sets the normal casing.
            /// </summary>
            public string NormalCasing { get; set; }

            /// <summary>
            /// Gets or sets the fix names casing.
            /// </summary>
            public string FixNamesCasing { get; set; }

            /// <summary>
            /// Gets or sets the fix only names casing.
            /// </summary>
            public string FixOnlyNamesCasing { get; set; }

            /// <summary>
            /// Gets or sets the only change all uppercase lines.
            /// </summary>
            public string OnlyChangeAllUppercaseLines { get; set; }

            /// <summary>
            /// Gets or sets the all uppercase.
            /// </summary>
            public string AllUppercase { get; set; }

            /// <summary>
            /// Gets or sets the all lowercase.
            /// </summary>
            public string AllLowercase { get; set; }
        }

        /// <summary>
        /// The change casing names.
        /// </summary>
        public class ChangeCasingNames
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the names found in subtitle x.
            /// </summary>
            public string NamesFoundInSubtitleX { get; set; }

            /// <summary>
            /// Gets or sets the enabled.
            /// </summary>
            public string Enabled { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the lines found x.
            /// </summary>
            public string LinesFoundX { get; set; }
        }

        /// <summary>
        /// The change frame rate.
        /// </summary>
        public class ChangeFrameRate
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the convert frame rate of subtitle.
            /// </summary>
            public string ConvertFrameRateOfSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the from frame rate.
            /// </summary>
            public string FromFrameRate { get; set; }

            /// <summary>
            /// Gets or sets the to frame rate.
            /// </summary>
            public string ToFrameRate { get; set; }

            /// <summary>
            /// Gets or sets the frame rate not correct.
            /// </summary>
            public string FrameRateNotCorrect { get; set; }

            /// <summary>
            /// Gets or sets the frame rate not changed.
            /// </summary>
            public string FrameRateNotChanged { get; set; }
        }

        /// <summary>
        /// The change speed in percent.
        /// </summary>
        public class ChangeSpeedInPercent
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the info.
            /// </summary>
            public string Info { get; set; }

            /// <summary>
            /// Gets or sets the custom.
            /// </summary>
            public string Custom { get; set; }

            /// <summary>
            /// Gets or sets the to drop frame.
            /// </summary>
            public string ToDropFrame { get; set; }

            /// <summary>
            /// Gets or sets the from drop frame.
            /// </summary>
            public string FromDropFrame { get; set; }
        }

        /// <summary>
        /// The check for updates.
        /// </summary>
        public class CheckForUpdates
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the checking for updates.
            /// </summary>
            public string CheckingForUpdates { get; set; }

            /// <summary>
            /// Gets or sets the checking for updates failed x.
            /// </summary>
            public string CheckingForUpdatesFailedX { get; set; }

            /// <summary>
            /// Gets or sets the checking for updates none available.
            /// </summary>
            public string CheckingForUpdatesNoneAvailable { get; set; }

            /// <summary>
            /// Gets or sets the checking for updates new version.
            /// </summary>
            public string CheckingForUpdatesNewVersion { get; set; }

            /// <summary>
            /// Gets or sets the install update.
            /// </summary>
            public string InstallUpdate { get; set; }

            /// <summary>
            /// Gets or sets the no updates.
            /// </summary>
            public string NoUpdates { get; set; }
        }

        /// <summary>
        /// The choose audio track.
        /// </summary>
        public class ChooseAudioTrack
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }
        }

        /// <summary>
        /// The choose encoding.
        /// </summary>
        public class ChooseEncoding
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the code page.
            /// </summary>
            public string CodePage { get; set; }

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            /// Gets or sets the please select an encoding.
            /// </summary>
            public string PleaseSelectAnEncoding { get; set; }
        }

        /// <summary>
        /// The choose language.
        /// </summary>
        public class ChooseLanguage
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Language { get; set; }
        }

        /// <summary>
        /// The color chooser.
        /// </summary>
        public class ColorChooser
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the red.
            /// </summary>
            public string Red { get; set; }

            /// <summary>
            /// Gets or sets the green.
            /// </summary>
            public string Green { get; set; }

            /// <summary>
            /// Gets or sets the blue.
            /// </summary>
            public string Blue { get; set; }

            /// <summary>
            /// Gets or sets the alpha.
            /// </summary>
            public string Alpha { get; set; }
        }

        /// <summary>
        /// The column paste.
        /// </summary>
        public class ColumnPaste
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the choose column.
            /// </summary>
            public string ChooseColumn { get; set; }

            /// <summary>
            /// Gets or sets the overwrite shift cells down.
            /// </summary>
            public string OverwriteShiftCellsDown { get; set; }

            /// <summary>
            /// Gets or sets the overwrite.
            /// </summary>
            public string Overwrite { get; set; }

            /// <summary>
            /// Gets or sets the shift cells down.
            /// </summary>
            public string ShiftCellsDown { get; set; }

            /// <summary>
            /// Gets or sets the time codes only.
            /// </summary>
            public string TimeCodesOnly { get; set; }

            /// <summary>
            /// Gets or sets the text only.
            /// </summary>
            public string TextOnly { get; set; }

            /// <summary>
            /// Gets or sets the original text only.
            /// </summary>
            public string OriginalTextOnly { get; set; }
        }

        /// <summary>
        /// The compare subtitles.
        /// </summary>
        public class CompareSubtitles
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the previous difference.
            /// </summary>
            public string PreviousDifference { get; set; }

            /// <summary>
            /// Gets or sets the next difference.
            /// </summary>
            public string NextDifference { get; set; }

            /// <summary>
            /// Gets or sets the subtitles not alike.
            /// </summary>
            public string SubtitlesNotAlike { get; set; }

            /// <summary>
            /// Gets or sets the x number of difference.
            /// </summary>
            public string XNumberOfDifference { get; set; }

            /// <summary>
            /// Gets or sets the x number of difference and percent changed.
            /// </summary>
            public string XNumberOfDifferenceAndPercentChanged { get; set; }

            /// <summary>
            /// Gets or sets the x number of difference and percent letters changed.
            /// </summary>
            public string XNumberOfDifferenceAndPercentLettersChanged { get; set; }

            /// <summary>
            /// Gets or sets the show only differences.
            /// </summary>
            public string ShowOnlyDifferences { get; set; }

            /// <summary>
            /// Gets or sets the ignore line breaks.
            /// </summary>
            public string IgnoreLineBreaks { get; set; }

            /// <summary>
            /// Gets or sets the only look for differences in text.
            /// </summary>
            public string OnlyLookForDifferencesInText { get; set; }

            /// <summary>
            /// Gets or sets the cannot compare with image based subtitles.
            /// </summary>
            public string CannotCompareWithImageBasedSubtitles { get; set; }
        }

        /// <summary>
        /// The d cinema properties.
        /// </summary>
        public class DCinemaProperties
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title smpte.
            /// </summary>
            public string TitleSmpte { get; set; }

            /// <summary>
            /// Gets or sets the subtitle id.
            /// </summary>
            public string SubtitleId { get; set; }

            /// <summary>
            /// Gets or sets the generate id.
            /// </summary>
            public string GenerateId { get; set; }

            /// <summary>
            /// Gets or sets the movie title.
            /// </summary>
            public string MovieTitle { get; set; }

            /// <summary>
            /// Gets or sets the reel number.
            /// </summary>
            public string ReelNumber { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the issue date.
            /// </summary>
            public string IssueDate { get; set; }

            /// <summary>
            /// Gets or sets the edit rate.
            /// </summary>
            public string EditRate { get; set; }

            /// <summary>
            /// Gets or sets the time code rate.
            /// </summary>
            public string TimeCodeRate { get; set; }

            /// <summary>
            /// Gets or sets the start time.
            /// </summary>
            public string StartTime { get; set; }

            /// <summary>
            /// Gets or sets the font.
            /// </summary>
            public string Font { get; set; }

            /// <summary>
            /// Gets or sets the font id.
            /// </summary>
            public string FontId { get; set; }

            /// <summary>
            /// Gets or sets the font uri.
            /// </summary>
            public string FontUri { get; set; }

            /// <summary>
            /// Gets or sets the font color.
            /// </summary>
            public string FontColor { get; set; }

            /// <summary>
            /// Gets or sets the font effect.
            /// </summary>
            public string FontEffect { get; set; }

            /// <summary>
            /// Gets or sets the font effect color.
            /// </summary>
            public string FontEffectColor { get; set; }

            /// <summary>
            /// Gets or sets the font size.
            /// </summary>
            public string FontSize { get; set; }

            /// <summary>
            /// Gets or sets the top bottom margin.
            /// </summary>
            public string TopBottomMargin { get; set; }

            /// <summary>
            /// Gets or sets the fade up time.
            /// </summary>
            public string FadeUpTime { get; set; }

            /// <summary>
            /// Gets or sets the fade down time.
            /// </summary>
            public string FadeDownTime { get; set; }

            /// <summary>
            /// Gets or sets the z position.
            /// </summary>
            public string ZPosition { get; set; }

            /// <summary>
            /// Gets or sets the z position help.
            /// </summary>
            public string ZPositionHelp { get; set; }

            /// <summary>
            /// Gets or sets the choose color.
            /// </summary>
            public string ChooseColor { get; set; }

            /// <summary>
            /// Gets or sets the generate.
            /// </summary>
            public string Generate { get; set; }
        }

        /// <summary>
        /// The durations bridge gaps.
        /// </summary>
        public class DurationsBridgeGaps
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the gaps bridged x.
            /// </summary>
            public string GapsBridgedX { get; set; }

            /// <summary>
            /// Gets or sets the gap to next.
            /// </summary>
            public string GapToNext { get; set; }

            /// <summary>
            /// Gets or sets the bridge gaps smaller than x part 1.
            /// </summary>
            public string BridgeGapsSmallerThanXPart1 { get; set; }

            /// <summary>
            /// Gets or sets the bridge gaps smaller than x part 2.
            /// </summary>
            public string BridgeGapsSmallerThanXPart2 { get; set; }

            /// <summary>
            /// Gets or sets the min milliseconds between lines.
            /// </summary>
            public string MinMillisecondsBetweenLines { get; set; }

            /// <summary>
            /// Gets or sets the prolong end time.
            /// </summary>
            public string ProlongEndTime { get; set; }

            /// <summary>
            /// Gets or sets the divide even.
            /// </summary>
            public string DivideEven { get; set; }
        }

        /// <summary>
        /// The dvd sub rip.
        /// </summary>
        public class DvdSubRip
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the dvd group title.
            /// </summary>
            public string DvdGroupTitle { get; set; }

            /// <summary>
            /// Gets or sets the ifo file.
            /// </summary>
            public string IfoFile { get; set; }

            /// <summary>
            /// Gets or sets the ifo files.
            /// </summary>
            public string IfoFiles { get; set; }

            /// <summary>
            /// Gets or sets the vob files.
            /// </summary>
            public string VobFiles { get; set; }

            /// <summary>
            /// Gets or sets the add.
            /// </summary>
            public string Add { get; set; }

            /// <summary>
            /// Gets or sets the remove.
            /// </summary>
            public string Remove { get; set; }

            /// <summary>
            /// Gets or sets the clear.
            /// </summary>
            public string Clear { get; set; }

            /// <summary>
            /// Gets or sets the move up.
            /// </summary>
            public string MoveUp { get; set; }

            /// <summary>
            /// Gets or sets the move down.
            /// </summary>
            public string MoveDown { get; set; }

            /// <summary>
            /// Gets or sets the languages.
            /// </summary>
            public string Languages { get; set; }

            /// <summary>
            /// Gets or sets the pal ntsc.
            /// </summary>
            public string PalNtsc { get; set; }

            /// <summary>
            /// Gets or sets the pal.
            /// </summary>
            public string Pal { get; set; }

            /// <summary>
            /// Gets or sets the ntsc.
            /// </summary>
            public string Ntsc { get; set; }

            /// <summary>
            /// Gets or sets the start ripping.
            /// </summary>
            public string StartRipping { get; set; }

            /// <summary>
            /// Gets or sets the abort.
            /// </summary>
            public string Abort { get; set; }

            /// <summary>
            /// Gets or sets the aborted by user.
            /// </summary>
            public string AbortedByUser { get; set; }

            /// <summary>
            /// Gets or sets the reading subtitle data.
            /// </summary>
            public string ReadingSubtitleData { get; set; }

            /// <summary>
            /// Gets or sets the ripping vob file xof yz.
            /// </summary>
            public string RippingVobFileXofYZ { get; set; }

            /// <summary>
            /// Gets or sets the wrong ifo type.
            /// </summary>
            public string WrongIfoType { get; set; }
        }

        /// <summary>
        /// The dvd sub rip choose language.
        /// </summary>
        public class DvdSubRipChooseLanguage
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the choose language stream id.
            /// </summary>
            public string ChooseLanguageStreamId { get; set; }

            /// <summary>
            /// Gets or sets the unknown language.
            /// </summary>
            public string UnknownLanguage { get; set; }

            /// <summary>
            /// Gets or sets the subtitle image xof y and width x height.
            /// </summary>
            public string SubtitleImageXofYAndWidthXHeight { get; set; }

            /// <summary>
            /// Gets or sets the subtitle image.
            /// </summary>
            public string SubtitleImage { get; set; }
        }

        /// <summary>
        /// The ebu save options.
        /// </summary>
        public class EbuSaveOptions
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the general subtitle information.
            /// </summary>
            public string GeneralSubtitleInformation { get; set; }

            /// <summary>
            /// Gets or sets the code page number.
            /// </summary>
            public string CodePageNumber { get; set; }

            /// <summary>
            /// Gets or sets the disk format code.
            /// </summary>
            public string DiskFormatCode { get; set; }

            /// <summary>
            /// Gets or sets the display standard code.
            /// </summary>
            public string DisplayStandardCode { get; set; }

            /// <summary>
            /// Gets or sets the character code table.
            /// </summary>
            public string CharacterCodeTable { get; set; }

            /// <summary>
            /// Gets or sets the language code.
            /// </summary>
            public string LanguageCode { get; set; }

            /// <summary>
            /// Gets or sets the original program title.
            /// </summary>
            public string OriginalProgramTitle { get; set; }

            /// <summary>
            /// Gets or sets the original episode title.
            /// </summary>
            public string OriginalEpisodeTitle { get; set; }

            /// <summary>
            /// Gets or sets the translated program title.
            /// </summary>
            public string TranslatedProgramTitle { get; set; }

            /// <summary>
            /// Gets or sets the translated episode title.
            /// </summary>
            public string TranslatedEpisodeTitle { get; set; }

            /// <summary>
            /// Gets or sets the translators name.
            /// </summary>
            public string TranslatorsName { get; set; }

            /// <summary>
            /// Gets or sets the subtitle list reference code.
            /// </summary>
            public string SubtitleListReferenceCode { get; set; }

            /// <summary>
            /// Gets or sets the country of origin.
            /// </summary>
            public string CountryOfOrigin { get; set; }

            /// <summary>
            /// Gets or sets the time code status.
            /// </summary>
            public string TimeCodeStatus { get; set; }

            /// <summary>
            /// Gets or sets the time code start of programme.
            /// </summary>
            public string TimeCodeStartOfProgramme { get; set; }

            /// <summary>
            /// Gets or sets the revision number.
            /// </summary>
            public string RevisionNumber { get; set; }

            /// <summary>
            /// Gets or sets the max no of displayable chars.
            /// </summary>
            public string MaxNoOfDisplayableChars { get; set; }

            /// <summary>
            /// Gets or sets the max number of displayable rows.
            /// </summary>
            public string MaxNumberOfDisplayableRows { get; set; }

            /// <summary>
            /// Gets or sets the disk sequence number.
            /// </summary>
            public string DiskSequenceNumber { get; set; }

            /// <summary>
            /// Gets or sets the total number of disks.
            /// </summary>
            public string TotalNumberOfDisks { get; set; }

            /// <summary>
            /// Gets or sets the import.
            /// </summary>
            public string Import { get; set; }

            /// <summary>
            /// Gets or sets the text and timing information.
            /// </summary>
            public string TextAndTimingInformation { get; set; }

            /// <summary>
            /// Gets or sets the justification code.
            /// </summary>
            public string JustificationCode { get; set; }

            /// <summary>
            /// Gets or sets the errors.
            /// </summary>
            public string Errors { get; set; }

            /// <summary>
            /// Gets or sets the errors x.
            /// </summary>
            public string ErrorsX { get; set; }

            /// <summary>
            /// Gets or sets the max length error.
            /// </summary>
            public string MaxLengthError { get; set; }

            /// <summary>
            /// Gets or sets the text unchanged presentation.
            /// </summary>
            public string TextUnchangedPresentation { get; set; }

            /// <summary>
            /// Gets or sets the text left justified text.
            /// </summary>
            public string TextLeftJustifiedText { get; set; }

            /// <summary>
            /// Gets or sets the text centered text.
            /// </summary>
            public string TextCenteredText { get; set; }

            /// <summary>
            /// Gets or sets the text right justified text.
            /// </summary>
            public string TextRightJustifiedText { get; set; }
        }

        /// <summary>
        /// The effect karaoke.
        /// </summary>
        public class EffectKaraoke
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the choose color.
            /// </summary>
            public string ChooseColor { get; set; }

            /// <summary>
            /// Gets or sets the total milliseconds.
            /// </summary>
            public string TotalMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the end delay in milliseconds.
            /// </summary>
            public string EndDelayInMilliseconds { get; set; }
        }

        /// <summary>
        /// The effect typewriter.
        /// </summary>
        public class EffectTypewriter
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the total milliseconds.
            /// </summary>
            public string TotalMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the end delay in milliseconds.
            /// </summary>
            public string EndDelayInMilliseconds { get; set; }
        }

        /// <summary>
        /// The export custom text.
        /// </summary>
        public class ExportCustomText
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the formats.
            /// </summary>
            public string Formats { get; set; }

            /// <summary>
            /// Gets or sets the new.
            /// </summary>
            public string New { get; set; }

            /// <summary>
            /// Gets or sets the edit.
            /// </summary>
            public string Edit { get; set; }

            /// <summary>
            /// Gets or sets the delete.
            /// </summary>
            public string Delete { get; set; }

            /// <summary>
            /// Gets or sets the save as.
            /// </summary>
            public string SaveAs { get; set; }

            /// <summary>
            /// Gets or sets the save subtitle as.
            /// </summary>
            public string SaveSubtitleAs { get; set; }

            /// <summary>
            /// Gets or sets the subtitle exported in custom format to x.
            /// </summary>
            public string SubtitleExportedInCustomFormatToX { get; set; }
        }

        /// <summary>
        /// The export custom text format.
        /// </summary>
        public class ExportCustomTextFormat
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the template.
            /// </summary>
            public string Template { get; set; }

            /// <summary>
            /// Gets or sets the header.
            /// </summary>
            public string Header { get; set; }

            /// <summary>
            /// Gets or sets the text line.
            /// </summary>
            public string TextLine { get; set; }

            /// <summary>
            /// Gets or sets the time code.
            /// </summary>
            public string TimeCode { get; set; }

            /// <summary>
            /// Gets or sets the new line.
            /// </summary>
            public string NewLine { get; set; }

            /// <summary>
            /// Gets or sets the footer.
            /// </summary>
            public string Footer { get; set; }

            /// <summary>
            /// Gets or sets the do not modify.
            /// </summary>
            public string DoNotModify { get; set; }
        }

        /// <summary>
        /// The export png xml.
        /// </summary>
        public class ExportPngXml
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the image settings.
            /// </summary>
            public string ImageSettings { get; set; }

            /// <summary>
            /// Gets or sets the font family.
            /// </summary>
            public string FontFamily { get; set; }

            /// <summary>
            /// Gets or sets the font size.
            /// </summary>
            public string FontSize { get; set; }

            /// <summary>
            /// Gets or sets the font color.
            /// </summary>
            public string FontColor { get; set; }

            /// <summary>
            /// Gets or sets the border color.
            /// </summary>
            public string BorderColor { get; set; }

            /// <summary>
            /// Gets or sets the border width.
            /// </summary>
            public string BorderWidth { get; set; }

            /// <summary>
            /// Gets or sets the border style.
            /// </summary>
            public string BorderStyle { get; set; }

            /// <summary>
            /// Gets or sets the border style one box.
            /// </summary>
            public string BorderStyleOneBox { get; set; }

            /// <summary>
            /// Gets or sets the border style box for each line.
            /// </summary>
            public string BorderStyleBoxForEachLine { get; set; }

            /// <summary>
            /// Gets or sets the border style normal width x.
            /// </summary>
            public string BorderStyleNormalWidthX { get; set; }

            /// <summary>
            /// Gets or sets the shadow color.
            /// </summary>
            public string ShadowColor { get; set; }

            /// <summary>
            /// Gets or sets the shadow width.
            /// </summary>
            public string ShadowWidth { get; set; }

            /// <summary>
            /// Gets or sets the transparency.
            /// </summary>
            public string Transparency { get; set; }

            /// <summary>
            /// Gets or sets the image format.
            /// </summary>
            public string ImageFormat { get; set; }

            /// <summary>
            /// Gets or sets the full frame image.
            /// </summary>
            public string FullFrameImage { get; set; }

            /// <summary>
            /// Gets or sets the simple rendering.
            /// </summary>
            public string SimpleRendering { get; set; }

            /// <summary>
            /// Gets or sets the anti aliasing with transparency.
            /// </summary>
            public string AntiAliasingWithTransparency { get; set; }

            /// <summary>
            /// Gets or sets the text 3 d.
            /// </summary>
            public string Text3D { get; set; }

            /// <summary>
            /// Gets or sets the side by side 3 d.
            /// </summary>
            public string SideBySide3D { get; set; }

            /// <summary>
            /// Gets or sets the half top bottom 3 d.
            /// </summary>
            public string HalfTopBottom3D { get; set; }

            /// <summary>
            /// Gets or sets the depth.
            /// </summary>
            public string Depth { get; set; }

            /// <summary>
            /// Gets or sets the export all lines.
            /// </summary>
            public string ExportAllLines { get; set; }

            /// <summary>
            /// Gets or sets the x images saved in y.
            /// </summary>
            public string XImagesSavedInY { get; set; }

            /// <summary>
            /// Gets or sets the video resolution.
            /// </summary>
            public string VideoResolution { get; set; }

            /// <summary>
            /// Gets or sets the align.
            /// </summary>
            public string Align { get; set; }

            /// <summary>
            /// Gets or sets the left.
            /// </summary>
            public string Left { get; set; }

            /// <summary>
            /// Gets or sets the right.
            /// </summary>
            public string Right { get; set; }

            /// <summary>
            /// Gets or sets the center.
            /// </summary>
            public string Center { get; set; }

            /// <summary>
            /// Gets or sets the bottom margin.
            /// </summary>
            public string BottomMargin { get; set; }

            /// <summary>
            /// Gets or sets the left right margin.
            /// </summary>
            public string LeftRightMargin { get; set; }

            /// <summary>
            /// Gets or sets the save blu rray sup as.
            /// </summary>
            public string SaveBluRraySupAs { get; set; }

            /// <summary>
            /// Gets or sets the save vob sub as.
            /// </summary>
            public string SaveVobSubAs { get; set; }

            /// <summary>
            /// Gets or sets the save fab image script as.
            /// </summary>
            public string SaveFabImageScriptAs { get; set; }

            /// <summary>
            /// Gets or sets the save dvd studio pro stl as.
            /// </summary>
            public string SaveDvdStudioProStlAs { get; set; }

            /// <summary>
            /// Gets or sets the save digital cinema interop as.
            /// </summary>
            public string SaveDigitalCinemaInteropAs { get; set; }

            /// <summary>
            /// Gets or sets the save premiere edl as.
            /// </summary>
            public string SavePremiereEdlAs { get; set; }

            /// <summary>
            /// Gets or sets the save fcp as.
            /// </summary>
            public string SaveFcpAs { get; set; }

            /// <summary>
            /// Gets or sets the save dost as.
            /// </summary>
            public string SaveDostAs { get; set; }

            /// <summary>
            /// Gets or sets the some lines were too long x.
            /// </summary>
            public string SomeLinesWereTooLongX { get; set; }

            /// <summary>
            /// Gets or sets the line height.
            /// </summary>
            public string LineHeight { get; set; }

            /// <summary>
            /// Gets or sets the box single line.
            /// </summary>
            public string BoxSingleLine { get; set; }

            /// <summary>
            /// Gets or sets the box multi line.
            /// </summary>
            public string BoxMultiLine { get; set; }

            /// <summary>
            /// Gets or sets the forced.
            /// </summary>
            public string Forced { get; set; }

            /// <summary>
            /// Gets or sets the choose background color.
            /// </summary>
            public string ChooseBackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the save image as.
            /// </summary>
            public string SaveImageAs { get; set; }
        }

        /// <summary>
        /// The export text.
        /// </summary>
        public class ExportText
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the preview.
            /// </summary>
            public string Preview { get; set; }

            /// <summary>
            /// Gets or sets the export options.
            /// </summary>
            public string ExportOptions { get; set; }

            /// <summary>
            /// Gets or sets the format text.
            /// </summary>
            public string FormatText { get; set; }

            /// <summary>
            /// Gets or sets the none.
            /// </summary>
            public string None { get; set; }

            /// <summary>
            /// Gets or sets the merge all lines.
            /// </summary>
            public string MergeAllLines { get; set; }

            /// <summary>
            /// Gets or sets the unbreak lines.
            /// </summary>
            public string UnbreakLines { get; set; }

            /// <summary>
            /// Gets or sets the remove styling.
            /// </summary>
            public string RemoveStyling { get; set; }

            /// <summary>
            /// Gets or sets the show line numbers.
            /// </summary>
            public string ShowLineNumbers { get; set; }

            /// <summary>
            /// Gets or sets the add new line after line number.
            /// </summary>
            public string AddNewLineAfterLineNumber { get; set; }

            /// <summary>
            /// Gets or sets the show time code.
            /// </summary>
            public string ShowTimeCode { get; set; }

            /// <summary>
            /// Gets or sets the add new line after time code.
            /// </summary>
            public string AddNewLineAfterTimeCode { get; set; }

            /// <summary>
            /// Gets or sets the add new line after texts.
            /// </summary>
            public string AddNewLineAfterTexts { get; set; }

            /// <summary>
            /// Gets or sets the add new line between subtitles.
            /// </summary>
            public string AddNewLineBetweenSubtitles { get; set; }

            /// <summary>
            /// Gets or sets the time code format.
            /// </summary>
            public string TimeCodeFormat { get; set; }

            /// <summary>
            /// Gets or sets the srt.
            /// </summary>
            public string Srt { get; set; }

            /// <summary>
            /// Gets or sets the milliseconds.
            /// </summary>
            public string Milliseconds { get; set; }

            /// <summary>
            /// Gets or sets the hhmmssff.
            /// </summary>
            public string HHMMSSFF { get; set; }

            /// <summary>
            /// Gets or sets the time code separator.
            /// </summary>
            public string TimeCodeSeparator { get; set; }
        }

        /// <summary>
        /// The extract date time info.
        /// </summary>
        public class ExtractDateTimeInfo
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the open video file.
            /// </summary>
            public string OpenVideoFile { get; set; }

            /// <summary>
            /// Gets or sets the start from.
            /// </summary>
            public string StartFrom { get; set; }

            /// <summary>
            /// Gets or sets the date time format.
            /// </summary>
            public string DateTimeFormat { get; set; }

            /// <summary>
            /// Gets or sets the example.
            /// </summary>
            public string Example { get; set; }

            /// <summary>
            /// Gets or sets the generate subtitle.
            /// </summary>
            public string GenerateSubtitle { get; set; }
        }

        /// <summary>
        /// The find dialog.
        /// </summary>
        public class FindDialog
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the find.
            /// </summary>
            public string Find { get; set; }

            /// <summary>
            /// Gets or sets the normal.
            /// </summary>
            public string Normal { get; set; }

            /// <summary>
            /// Gets or sets the case sensitive.
            /// </summary>
            public string CaseSensitive { get; set; }

            /// <summary>
            /// Gets or sets the regular expression.
            /// </summary>
            public string RegularExpression { get; set; }
        }

        /// <summary>
        /// The find subtitle line.
        /// </summary>
        public class FindSubtitleLine
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the find.
            /// </summary>
            public string Find { get; set; }

            /// <summary>
            /// Gets or sets the find next.
            /// </summary>
            public string FindNext { get; set; }
        }

        /// <summary>
        /// The fix common errors.
        /// </summary>
        public class FixCommonErrors
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the step 1.
            /// </summary>
            public string Step1 { get; set; }

            /// <summary>
            /// Gets or sets the what to fix.
            /// </summary>
            public string WhatToFix { get; set; }

            /// <summary>
            /// Gets or sets the example.
            /// </summary>
            public string Example { get; set; }

            /// <summary>
            /// Gets or sets the select all.
            /// </summary>
            public string SelectAll { get; set; }

            /// <summary>
            /// Gets or sets the inverse selection.
            /// </summary>
            public string InverseSelection { get; set; }

            /// <summary>
            /// Gets or sets the back.
            /// </summary>
            public string Back { get; set; }

            /// <summary>
            /// Gets or sets the next.
            /// </summary>
            public string Next { get; set; }

            /// <summary>
            /// Gets or sets the step 2.
            /// </summary>
            public string Step2 { get; set; }

            /// <summary>
            /// Gets or sets the fixes.
            /// </summary>
            public string Fixes { get; set; }

            /// <summary>
            /// Gets or sets the log.
            /// </summary>
            public string Log { get; set; }

            /// <summary>
            /// Gets or sets the function.
            /// </summary>
            public string Function { get; set; }

            /// <summary>
            /// Gets or sets the removed empty line.
            /// </summary>
            public string RemovedEmptyLine { get; set; }

            /// <summary>
            /// Gets or sets the removed empty line at top.
            /// </summary>
            public string RemovedEmptyLineAtTop { get; set; }

            /// <summary>
            /// Gets or sets the removed empty line at bottom.
            /// </summary>
            public string RemovedEmptyLineAtBottom { get; set; }

            /// <summary>
            /// Gets or sets the removed empty lines unsed line breaks.
            /// </summary>
            public string RemovedEmptyLinesUnsedLineBreaks { get; set; }

            /// <summary>
            /// Gets or sets the empty lines removed x.
            /// </summary>
            public string EmptyLinesRemovedX { get; set; }

            /// <summary>
            /// Gets or sets the fix overlapping display times.
            /// </summary>
            public string FixOverlappingDisplayTimes { get; set; }

            /// <summary>
            /// Gets or sets the fix short display times.
            /// </summary>
            public string FixShortDisplayTimes { get; set; }

            /// <summary>
            /// Gets or sets the fix long display times.
            /// </summary>
            public string FixLongDisplayTimes { get; set; }

            /// <summary>
            /// Gets or sets the fix invalid italic tags.
            /// </summary>
            public string FixInvalidItalicTags { get; set; }

            /// <summary>
            /// Gets or sets the remove unneeded spaces.
            /// </summary>
            public string RemoveUnneededSpaces { get; set; }

            /// <summary>
            /// Gets or sets the remove unneeded periods.
            /// </summary>
            public string RemoveUnneededPeriods { get; set; }

            /// <summary>
            /// Gets or sets the fix missing spaces.
            /// </summary>
            public string FixMissingSpaces { get; set; }

            /// <summary>
            /// Gets or sets the break long lines.
            /// </summary>
            public string BreakLongLines { get; set; }

            /// <summary>
            /// Gets or sets the remove line breaks.
            /// </summary>
            public string RemoveLineBreaks { get; set; }

            /// <summary>
            /// Gets or sets the remove line breaks all.
            /// </summary>
            public string RemoveLineBreaksAll { get; set; }

            /// <summary>
            /// Gets or sets the fix uppercase i insinde lowercase words.
            /// </summary>
            public string FixUppercaseIInsindeLowercaseWords { get; set; }

            /// <summary>
            /// Gets or sets the fix double apostrophes.
            /// </summary>
            public string FixDoubleApostrophes { get; set; }

            /// <summary>
            /// Gets or sets the add periods.
            /// </summary>
            public string AddPeriods { get; set; }

            /// <summary>
            /// Gets or sets the start with uppercase letter after paragraph.
            /// </summary>
            public string StartWithUppercaseLetterAfterParagraph { get; set; }

            /// <summary>
            /// Gets or sets the start with uppercase letter after period inside paragraph.
            /// </summary>
            public string StartWithUppercaseLetterAfterPeriodInsideParagraph { get; set; }

            /// <summary>
            /// Gets or sets the start with uppercase letter after colon.
            /// </summary>
            public string StartWithUppercaseLetterAfterColon { get; set; }

            /// <summary>
            /// Gets or sets the fix lowercase i to uppercase i.
            /// </summary>
            public string FixLowercaseIToUppercaseI { get; set; }

            /// <summary>
            /// Gets or sets the fix common ocr errors.
            /// </summary>
            public string FixCommonOcrErrors { get; set; }

            /// <summary>
            /// Gets or sets the common ocr errors fixed.
            /// </summary>
            public string CommonOcrErrorsFixed { get; set; }

            /// <summary>
            /// Gets or sets the remove space between number.
            /// </summary>
            public string RemoveSpaceBetweenNumber { get; set; }

            /// <summary>
            /// Gets or sets the fix dialogs on one line.
            /// </summary>
            public string FixDialogsOnOneLine { get; set; }

            /// <summary>
            /// Gets or sets the remove space between numbers fixed.
            /// </summary>
            public string RemoveSpaceBetweenNumbersFixed { get; set; }

            /// <summary>
            /// Gets or sets the fix turkish ansi.
            /// </summary>
            public string FixTurkishAnsi { get; set; }

            /// <summary>
            /// Gets or sets the fix danish letter i.
            /// </summary>
            public string FixDanishLetterI { get; set; }

            /// <summary>
            /// Gets or sets the fix spanish inverted question and exclamation marks.
            /// </summary>
            public string FixSpanishInvertedQuestionAndExclamationMarks { get; set; }

            /// <summary>
            /// Gets or sets the add missing quote.
            /// </summary>
            public string AddMissingQuote { get; set; }

            /// <summary>
            /// Gets or sets the add missing quotes.
            /// </summary>
            public string AddMissingQuotes { get; set; }

            /// <summary>
            /// Gets or sets the fix hyphens.
            /// </summary>
            public string FixHyphens { get; set; }

            /// <summary>
            /// Gets or sets the fix hyphens add.
            /// </summary>
            public string FixHyphensAdd { get; set; }

            /// <summary>
            /// Gets or sets the fix hyphen.
            /// </summary>
            public string FixHyphen { get; set; }

            /// <summary>
            /// Gets or sets the x hyphens fixed.
            /// </summary>
            public string XHyphensFixed { get; set; }

            /// <summary>
            /// Gets or sets the add missing quotes example.
            /// </summary>
            public string AddMissingQuotesExample { get; set; }

            /// <summary>
            /// Gets or sets the x missing quotes added.
            /// </summary>
            public string XMissingQuotesAdded { get; set; }

            /// <summary>
            /// Gets or sets the fix 3 plus lines.
            /// </summary>
            public string Fix3PlusLines { get; set; }

            /// <summary>
            /// Gets or sets the fix 3 plus line.
            /// </summary>
            public string Fix3PlusLine { get; set; }

            /// <summary>
            /// Gets or sets the x 3 plus lines fixed.
            /// </summary>
            public string X3PlusLinesFixed { get; set; }

            /// <summary>
            /// Gets or sets the analysing.
            /// </summary>
            public string Analysing { get; set; }

            /// <summary>
            /// Gets or sets the nothing to fix.
            /// </summary>
            public string NothingToFix { get; set; }

            /// <summary>
            /// Gets or sets the fixes found x.
            /// </summary>
            public string FixesFoundX { get; set; }

            /// <summary>
            /// Gets or sets the x fixes applied.
            /// </summary>
            public string XFixesApplied { get; set; }

            /// <summary>
            /// Gets or sets the nothing to fix but.
            /// </summary>
            public string NothingToFixBut { get; set; }

            /// <summary>
            /// Gets or sets the fix lowercase i to uppercase i checked but current language is not english.
            /// </summary>
            public string FixLowercaseIToUppercaseICheckedButCurrentLanguageIsNotEnglish { get; set; }

            /// <summary>
            /// Gets or sets the continue.
            /// </summary>
            public string Continue { get; set; }

            /// <summary>
            /// Gets or sets the continue anyway.
            /// </summary>
            public string ContinueAnyway { get; set; }

            /// <summary>
            /// Gets or sets the unchecked fix lowercase i to uppercase i.
            /// </summary>
            public string UncheckedFixLowercaseIToUppercaseI { get; set; }

            /// <summary>
            /// Gets or sets the x is changed to uppercase.
            /// </summary>
            public string XIsChangedToUppercase { get; set; }

            /// <summary>
            /// Gets or sets the fix first letter to uppercase after paragraph.
            /// </summary>
            public string FixFirstLetterToUppercaseAfterParagraph { get; set; }

            /// <summary>
            /// Gets or sets the merge short line.
            /// </summary>
            public string MergeShortLine { get; set; }

            /// <summary>
            /// Gets or sets the merge short line all.
            /// </summary>
            public string MergeShortLineAll { get; set; }

            /// <summary>
            /// Gets or sets the x line breaks added.
            /// </summary>
            public string XLineBreaksAdded { get; set; }

            /// <summary>
            /// Gets or sets the break long line.
            /// </summary>
            public string BreakLongLine { get; set; }

            /// <summary>
            /// Gets or sets the fix long display time.
            /// </summary>
            public string FixLongDisplayTime { get; set; }

            /// <summary>
            /// Gets or sets the fix invalid italic tag.
            /// </summary>
            public string FixInvalidItalicTag { get; set; }

            /// <summary>
            /// Gets or sets the fix short display time.
            /// </summary>
            public string FixShortDisplayTime { get; set; }

            /// <summary>
            /// Gets or sets the fix overlapping display time.
            /// </summary>
            public string FixOverlappingDisplayTime { get; set; }

            /// <summary>
            /// Gets or sets the fix invalid italic tags example.
            /// </summary>
            public string FixInvalidItalicTagsExample { get; set; }

            /// <summary>
            /// Gets or sets the remove unneeded spaces example.
            /// </summary>
            public string RemoveUnneededSpacesExample { get; set; }

            /// <summary>
            /// Gets or sets the remove unneeded periods example.
            /// </summary>
            public string RemoveUnneededPeriodsExample { get; set; }

            /// <summary>
            /// Gets or sets the fix missing spaces example.
            /// </summary>
            public string FixMissingSpacesExample { get; set; }

            /// <summary>
            /// Gets or sets the fix uppercase i insinde lowercase words example.
            /// </summary>
            public string FixUppercaseIInsindeLowercaseWordsExample { get; set; }

            /// <summary>
            /// Gets or sets the fix lowercase i to uppercase i example.
            /// </summary>
            public string FixLowercaseIToUppercaseIExample { get; set; }

            /// <summary>
            /// Gets or sets the start time later than end time.
            /// </summary>
            public string StartTimeLaterThanEndTime { get; set; }

            /// <summary>
            /// Gets or sets the unable to fix start time later than end time.
            /// </summary>
            public string UnableToFixStartTimeLaterThanEndTime { get; set; }

            /// <summary>
            /// Gets or sets the x fixed to yz.
            /// </summary>
            public string XFixedToYZ { get; set; }

            /// <summary>
            /// Gets or sets the unable to fix text xy.
            /// </summary>
            public string UnableToFixTextXY { get; set; }

            /// <summary>
            /// Gets or sets the x overlapping timestamps fixed.
            /// </summary>
            public string XOverlappingTimestampsFixed { get; set; }

            /// <summary>
            /// Gets or sets the x display times prolonged.
            /// </summary>
            public string XDisplayTimesProlonged { get; set; }

            /// <summary>
            /// Gets or sets the x invalid html tags fixed.
            /// </summary>
            public string XInvalidHtmlTagsFixed { get; set; }

            /// <summary>
            /// Gets or sets the x display times shortned.
            /// </summary>
            public string XDisplayTimesShortned { get; set; }

            /// <summary>
            /// Gets or sets the x lines unbreaked.
            /// </summary>
            public string XLinesUnbreaked { get; set; }

            /// <summary>
            /// Gets or sets the unneeded space.
            /// </summary>
            public string UnneededSpace { get; set; }

            /// <summary>
            /// Gets or sets the x unneeded spaces removed.
            /// </summary>
            public string XUnneededSpacesRemoved { get; set; }

            /// <summary>
            /// Gets or sets the unneeded period.
            /// </summary>
            public string UnneededPeriod { get; set; }

            /// <summary>
            /// Gets or sets the x unneeded periods removed.
            /// </summary>
            public string XUnneededPeriodsRemoved { get; set; }

            /// <summary>
            /// Gets or sets the fix missing space.
            /// </summary>
            public string FixMissingSpace { get; set; }

            /// <summary>
            /// Gets or sets the x missing spaces added.
            /// </summary>
            public string XMissingSpacesAdded { get; set; }

            /// <summary>
            /// Gets or sets the fix uppercase i inside lowercase word.
            /// </summary>
            public string FixUppercaseIInsideLowercaseWord { get; set; }

            /// <summary>
            /// Gets or sets the x periods added.
            /// </summary>
            public string XPeriodsAdded { get; set; }

            /// <summary>
            /// Gets or sets the fix missing period at end of line.
            /// </summary>
            public string FixMissingPeriodAtEndOfLine { get; set; }

            /// <summary>
            /// Gets or sets the x double apostrophes fixed.
            /// </summary>
            public string XDoubleApostrophesFixed { get; set; }

            /// <summary>
            /// Gets or sets the x uppercase is found inside lowercase words.
            /// </summary>
            public string XUppercaseIsFoundInsideLowercaseWords { get; set; }

            /// <summary>
            /// Gets or sets the refresh fixes.
            /// </summary>
            public string RefreshFixes { get; set; }

            /// <summary>
            /// Gets or sets the apply fixes.
            /// </summary>
            public string ApplyFixes { get; set; }

            /// <summary>
            /// Gets or sets the auto break.
            /// </summary>
            public string AutoBreak { get; set; }

            /// <summary>
            /// Gets or sets the unbreak.
            /// </summary>
            public string Unbreak { get; set; }

            /// <summary>
            /// Gets or sets the fix double dash.
            /// </summary>
            public string FixDoubleDash { get; set; }

            /// <summary>
            /// Gets or sets the fix double greater than.
            /// </summary>
            public string FixDoubleGreaterThan { get; set; }

            /// <summary>
            /// Gets or sets the fix ellipses start.
            /// </summary>
            public string FixEllipsesStart { get; set; }

            /// <summary>
            /// Gets or sets the fix missing open bracket.
            /// </summary>
            public string FixMissingOpenBracket { get; set; }

            /// <summary>
            /// Gets or sets the fix music notation.
            /// </summary>
            public string FixMusicNotation { get; set; }

            /// <summary>
            /// Gets or sets the x fix double dash.
            /// </summary>
            public string XFixDoubleDash { get; set; }

            /// <summary>
            /// Gets or sets the x fix double greater than.
            /// </summary>
            public string XFixDoubleGreaterThan { get; set; }

            /// <summary>
            /// Gets or sets the x fix ellipses start.
            /// </summary>
            public string XFixEllipsesStart { get; set; }

            /// <summary>
            /// Gets or sets the x fix missing open bracket.
            /// </summary>
            public string XFixMissingOpenBracket { get; set; }

            /// <summary>
            /// Gets or sets the x fix music notation.
            /// </summary>
            public string XFixMusicNotation { get; set; }

            /// <summary>
            /// Gets or sets the fix double dash example.
            /// </summary>
            public string FixDoubleDashExample { get; set; }

            /// <summary>
            /// Gets or sets the fix double greater than example.
            /// </summary>
            public string FixDoubleGreaterThanExample { get; set; }

            /// <summary>
            /// Gets or sets the fix ellipses start example.
            /// </summary>
            public string FixEllipsesStartExample { get; set; }

            /// <summary>
            /// Gets or sets the fix missing open bracket example.
            /// </summary>
            public string FixMissingOpenBracketExample { get; set; }

            /// <summary>
            /// Gets or sets the fix music notation example.
            /// </summary>
            public string FixMusicNotationExample { get; set; }

            /// <summary>
            /// Gets or sets the number of important log messages.
            /// </summary>
            public string NumberOfImportantLogMessages { get; set; }

            /// <summary>
            /// Gets or sets the fixed ok xy.
            /// </summary>
            public string FixedOkXY { get; set; }

            /// <summary>
            /// Gets or sets the fix ocr error example.
            /// </summary>
            public string FixOcrErrorExample { get; set; }

            /// <summary>
            /// Gets or sets the fix space between numbers example.
            /// </summary>
            public string FixSpaceBetweenNumbersExample { get; set; }

            /// <summary>
            /// Gets or sets the fix dialogs one line example.
            /// </summary>
            public string FixDialogsOneLineExample { get; set; }
        }

        /// <summary>
        /// The get dictionaries.
        /// </summary>
        public class GetDictionaries
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description line 1.
            /// </summary>
            public string DescriptionLine1 { get; set; }

            /// <summary>
            /// Gets or sets the description line 2.
            /// </summary>
            public string DescriptionLine2 { get; set; }

            /// <summary>
            /// Gets or sets the get dictionaries here.
            /// </summary>
            public string GetDictionariesHere { get; set; }

            /// <summary>
            /// Gets or sets the open open office wiki.
            /// </summary>
            public string OpenOpenOfficeWiki { get; set; }

            /// <summary>
            /// Gets or sets the get all dictionaries.
            /// </summary>
            public string GetAllDictionaries { get; set; }

            /// <summary>
            /// Gets or sets the choose language and click download.
            /// </summary>
            public string ChooseLanguageAndClickDownload { get; set; }

            /// <summary>
            /// Gets or sets the open dictionaries folder.
            /// </summary>
            public string OpenDictionariesFolder { get; set; }

            /// <summary>
            /// Gets or sets the download.
            /// </summary>
            public string Download { get; set; }

            /// <summary>
            /// Gets or sets the x downloaded.
            /// </summary>
            public string XDownloaded { get; set; }
        }

        /// <summary>
        /// The get tesseract dictionaries.
        /// </summary>
        public class GetTesseractDictionaries
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description line 1.
            /// </summary>
            public string DescriptionLine1 { get; set; }

            /// <summary>
            /// Gets or sets the download failed.
            /// </summary>
            public string DownloadFailed { get; set; }

            /// <summary>
            /// Gets or sets the get dictionaries here.
            /// </summary>
            public string GetDictionariesHere { get; set; }

            /// <summary>
            /// Gets or sets the open open office wiki.
            /// </summary>
            public string OpenOpenOfficeWiki { get; set; }

            /// <summary>
            /// Gets or sets the get all dictionaries.
            /// </summary>
            public string GetAllDictionaries { get; set; }

            /// <summary>
            /// Gets or sets the choose language and click download.
            /// </summary>
            public string ChooseLanguageAndClickDownload { get; set; }

            /// <summary>
            /// Gets or sets the open dictionaries folder.
            /// </summary>
            public string OpenDictionariesFolder { get; set; }

            /// <summary>
            /// Gets or sets the download.
            /// </summary>
            public string Download { get; set; }

            /// <summary>
            /// Gets or sets the x downloaded.
            /// </summary>
            public string XDownloaded { get; set; }
        }

        /// <summary>
        /// The google translate.
        /// </summary>
        public class GoogleTranslate
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the from.
            /// </summary>
            public string From { get; set; }

            /// <summary>
            /// Gets or sets the to.
            /// </summary>
            public string To { get; set; }

            /// <summary>
            /// Gets or sets the translate.
            /// </summary>
            public string Translate { get; set; }

            /// <summary>
            /// Gets or sets the please wait.
            /// </summary>
            public string PleaseWait { get; set; }

            /// <summary>
            /// Gets or sets the powered by google translate.
            /// </summary>
            public string PoweredByGoogleTranslate { get; set; }

            /// <summary>
            /// Gets or sets the powered by microsoft translate.
            /// </summary>
            public string PoweredByMicrosoftTranslate { get; set; }
        }

        /// <summary>
        /// The google or microsoft translate.
        /// </summary>
        public class GoogleOrMicrosoftTranslate
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the from.
            /// </summary>
            public string From { get; set; }

            /// <summary>
            /// Gets or sets the to.
            /// </summary>
            public string To { get; set; }

            /// <summary>
            /// Gets or sets the translate.
            /// </summary>
            public string Translate { get; set; }

            /// <summary>
            /// Gets or sets the source text.
            /// </summary>
            public string SourceText { get; set; }

            /// <summary>
            /// Gets or sets the google translate.
            /// </summary>
            public string GoogleTranslate { get; set; }

            /// <summary>
            /// Gets or sets the microsoft translate.
            /// </summary>
            public string MicrosoftTranslate { get; set; }
        }

        /// <summary>
        /// The go to line.
        /// </summary>
        public class GoToLine
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the x is not a valid number.
            /// </summary>
            public string XIsNotAValidNumber { get; set; }
        }

        /// <summary>
        /// The import images.
        /// </summary>
        public class ImportImages
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the image files.
            /// </summary>
            public string ImageFiles { get; set; }

            /// <summary>
            /// Gets or sets the input.
            /// </summary>
            public string Input { get; set; }

            /// <summary>
            /// Gets or sets the input description.
            /// </summary>
            public string InputDescription { get; set; }
        }

        /// <summary>
        /// The import scene changes.
        /// </summary>
        public class ImportSceneChanges
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the open text file.
            /// </summary>
            public string OpenTextFile { get; set; }

            /// <summary>
            /// Gets or sets the import options.
            /// </summary>
            public string ImportOptions { get; set; }

            /// <summary>
            /// Gets or sets the text files.
            /// </summary>
            public string TextFiles { get; set; }

            /// <summary>
            /// Gets or sets the time codes.
            /// </summary>
            public string TimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the frames.
            /// </summary>
            public string Frames { get; set; }

            /// <summary>
            /// Gets or sets the seconds.
            /// </summary>
            public string Seconds { get; set; }

            /// <summary>
            /// Gets or sets the milliseconds.
            /// </summary>
            public string Milliseconds { get; set; }
        }

        /// <summary>
        /// The import text.
        /// </summary>
        public class ImportText
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the one subtitle is one file.
            /// </summary>
            public string OneSubtitleIsOneFile { get; set; }

            /// <summary>
            /// Gets or sets the open text file.
            /// </summary>
            public string OpenTextFile { get; set; }

            /// <summary>
            /// Gets or sets the open text files.
            /// </summary>
            public string OpenTextFiles { get; set; }

            /// <summary>
            /// Gets or sets the import options.
            /// </summary>
            public string ImportOptions { get; set; }

            /// <summary>
            /// Gets or sets the splitting.
            /// </summary>
            public string Splitting { get; set; }

            /// <summary>
            /// Gets or sets the auto split text.
            /// </summary>
            public string AutoSplitText { get; set; }

            /// <summary>
            /// Gets or sets the one line is one subtitle.
            /// </summary>
            public string OneLineIsOneSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the line break.
            /// </summary>
            public string LineBreak { get; set; }

            /// <summary>
            /// Gets or sets the split at blank lines.
            /// </summary>
            public string SplitAtBlankLines { get; set; }

            /// <summary>
            /// Gets or sets the merge short lines.
            /// </summary>
            public string MergeShortLines { get; set; }

            /// <summary>
            /// Gets or sets the remove empty lines.
            /// </summary>
            public string RemoveEmptyLines { get; set; }

            /// <summary>
            /// Gets or sets the remove lines without letters.
            /// </summary>
            public string RemoveLinesWithoutLetters { get; set; }

            /// <summary>
            /// Gets or sets the generate time codes.
            /// </summary>
            public string GenerateTimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the gap between subtitles.
            /// </summary>
            public string GapBetweenSubtitles { get; set; }

            /// <summary>
            /// Gets or sets the auto.
            /// </summary>
            public string Auto { get; set; }

            /// <summary>
            /// Gets or sets the fixed.
            /// </summary>
            public string Fixed { get; set; }

            /// <summary>
            /// Gets or sets the refresh.
            /// </summary>
            public string Refresh { get; set; }

            /// <summary>
            /// Gets or sets the text files.
            /// </summary>
            public string TextFiles { get; set; }

            /// <summary>
            /// Gets or sets the preview lines modified x.
            /// </summary>
            public string PreviewLinesModifiedX { get; set; }

            /// <summary>
            /// Gets or sets the time codes.
            /// </summary>
            public string TimeCodes { get; set; }
        }

        /// <summary>
        /// The interjections.
        /// </summary>
        public class Interjections
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }
        }

        /// <summary>
        /// The join subtitles.
        /// </summary>
        public class JoinSubtitles
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the information.
            /// </summary>
            public string Information { get; set; }

            /// <summary>
            /// Gets or sets the number of lines.
            /// </summary>
            public string NumberOfLines { get; set; }

            /// <summary>
            /// Gets or sets the start time.
            /// </summary>
            public string StartTime { get; set; }

            /// <summary>
            /// Gets or sets the end time.
            /// </summary>
            public string EndTime { get; set; }

            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the join.
            /// </summary>
            public string Join { get; set; }

            /// <summary>
            /// Gets or sets the total number of lines x.
            /// </summary>
            public string TotalNumberOfLinesX { get; set; }

            /// <summary>
            /// Gets or sets the note.
            /// </summary>
            public string Note { get; set; }
        }

        /// <summary>
        /// The main.
        /// </summary>
        public class Main
        {
            /// <summary>
            /// Gets or sets the menu.
            /// </summary>
            public MainMenu Menu { get; set; }

            /// <summary>
            /// Gets or sets the controls.
            /// </summary>
            public MainControls Controls { get; set; }

            /// <summary>
            /// Gets or sets the video controls.
            /// </summary>
            public MainVideoControls VideoControls { get; set; }

            /// <summary>
            /// Gets or sets the save changes to untitled.
            /// </summary>
            public string SaveChangesToUntitled { get; set; }

            /// <summary>
            /// Gets or sets the save changes to x.
            /// </summary>
            public string SaveChangesToX { get; set; }

            /// <summary>
            /// Gets or sets the save changes to untitled original.
            /// </summary>
            public string SaveChangesToUntitledOriginal { get; set; }

            /// <summary>
            /// Gets or sets the save changes to original x.
            /// </summary>
            public string SaveChangesToOriginalX { get; set; }

            /// <summary>
            /// Gets or sets the save subtitle as.
            /// </summary>
            public string SaveSubtitleAs { get; set; }

            /// <summary>
            /// Gets or sets the save original subtitle as.
            /// </summary>
            public string SaveOriginalSubtitleAs { get; set; }

            /// <summary>
            /// Gets or sets the no subtitle loaded.
            /// </summary>
            public string NoSubtitleLoaded { get; set; }

            /// <summary>
            /// Gets or sets the visual sync selected lines.
            /// </summary>
            public string VisualSyncSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the visual sync title.
            /// </summary>
            public string VisualSyncTitle { get; set; }

            /// <summary>
            /// Gets or sets the before visual sync.
            /// </summary>
            public string BeforeVisualSync { get; set; }

            /// <summary>
            /// Gets or sets the visual sync performed on selected lines.
            /// </summary>
            public string VisualSyncPerformedOnSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the visual sync performed.
            /// </summary>
            public string VisualSyncPerformed { get; set; }

            /// <summary>
            /// Gets or sets the import this vob sub subtitle.
            /// </summary>
            public string ImportThisVobSubSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the file x is larger than 10 mb.
            /// </summary>
            public string FileXIsLargerThan10MB { get; set; }

            /// <summary>
            /// Gets or sets the continue anyway.
            /// </summary>
            public string ContinueAnyway { get; set; }

            /// <summary>
            /// Gets or sets the before load of.
            /// </summary>
            public string BeforeLoadOf { get; set; }

            /// <summary>
            /// Gets or sets the loaded subtitle x.
            /// </summary>
            public string LoadedSubtitleX { get; set; }

            /// <summary>
            /// Gets or sets the loaded empty or short.
            /// </summary>
            public string LoadedEmptyOrShort { get; set; }

            /// <summary>
            /// Gets or sets the file is empty or short.
            /// </summary>
            public string FileIsEmptyOrShort { get; set; }

            /// <summary>
            /// Gets or sets the file not found.
            /// </summary>
            public string FileNotFound { get; set; }

            /// <summary>
            /// Gets or sets the saved subtitle x.
            /// </summary>
            public string SavedSubtitleX { get; set; }

            /// <summary>
            /// Gets or sets the saved original subtitle x.
            /// </summary>
            public string SavedOriginalSubtitleX { get; set; }

            /// <summary>
            /// Gets or sets the file on disk modified.
            /// </summary>
            public string FileOnDiskModified { get; set; }

            /// <summary>
            /// Gets or sets the overwrite modified file.
            /// </summary>
            public string OverwriteModifiedFile { get; set; }

            /// <summary>
            /// Gets or sets the file x is read only.
            /// </summary>
            public string FileXIsReadOnly { get; set; }

            /// <summary>
            /// Gets or sets the unable to save subtitle x.
            /// </summary>
            public string UnableToSaveSubtitleX { get; set; }

            /// <summary>
            /// Gets or sets the before new.
            /// </summary>
            public string BeforeNew { get; set; }

            /// <summary>
            /// Gets or sets the new.
            /// </summary>
            public string New { get; set; }

            /// <summary>
            /// Gets or sets the before converting to x.
            /// </summary>
            public string BeforeConvertingToX { get; set; }

            /// <summary>
            /// Gets or sets the converted to x.
            /// </summary>
            public string ConvertedToX { get; set; }

            /// <summary>
            /// Gets or sets the before show earlier.
            /// </summary>
            public string BeforeShowEarlier { get; set; }

            /// <summary>
            /// Gets or sets the before show later.
            /// </summary>
            public string BeforeShowLater { get; set; }

            /// <summary>
            /// Gets or sets the line number x.
            /// </summary>
            public string LineNumberX { get; set; }

            /// <summary>
            /// Gets or sets the open video file.
            /// </summary>
            public string OpenVideoFile { get; set; }

            /// <summary>
            /// Gets or sets the new frame rate used to calculate time codes.
            /// </summary>
            public string NewFrameRateUsedToCalculateTimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the new frame rate used to calculate frame numbers.
            /// </summary>
            public string NewFrameRateUsedToCalculateFrameNumbers { get; set; }

            /// <summary>
            /// Gets or sets the find continue.
            /// </summary>
            public string FindContinue { get; set; }

            /// <summary>
            /// Gets or sets the find continue title.
            /// </summary>
            public string FindContinueTitle { get; set; }

            /// <summary>
            /// Gets or sets the replace continue not found.
            /// </summary>
            public string ReplaceContinueNotFound { get; set; }

            /// <summary>
            /// Gets or sets the replace x continue.
            /// </summary>
            public string ReplaceXContinue { get; set; }

            /// <summary>
            /// Gets or sets the replace continue title.
            /// </summary>
            public string ReplaceContinueTitle { get; set; }

            /// <summary>
            /// Gets or sets the searching for x from line y.
            /// </summary>
            public string SearchingForXFromLineY { get; set; }

            /// <summary>
            /// Gets or sets the x found at line number y.
            /// </summary>
            public string XFoundAtLineNumberY { get; set; }

            /// <summary>
            /// Gets or sets the x not found.
            /// </summary>
            public string XNotFound { get; set; }

            /// <summary>
            /// Gets or sets the before replace.
            /// </summary>
            public string BeforeReplace { get; set; }

            /// <summary>
            /// Gets or sets the match found x.
            /// </summary>
            public string MatchFoundX { get; set; }

            /// <summary>
            /// Gets or sets the no match found x.
            /// </summary>
            public string NoMatchFoundX { get; set; }

            /// <summary>
            /// Gets or sets the found nothing to replace.
            /// </summary>
            public string FoundNothingToReplace { get; set; }

            /// <summary>
            /// Gets or sets the replace count x.
            /// </summary>
            public string ReplaceCountX { get; set; }

            /// <summary>
            /// Gets or sets the no x found at line y.
            /// </summary>
            public string NoXFoundAtLineY { get; set; }

            /// <summary>
            /// Gets or sets the one replacement made.
            /// </summary>
            public string OneReplacementMade { get; set; }

            /// <summary>
            /// Gets or sets the before changes made in source view.
            /// </summary>
            public string BeforeChangesMadeInSourceView { get; set; }

            /// <summary>
            /// Gets or sets the unable to parse source view.
            /// </summary>
            public string UnableToParseSourceView { get; set; }

            /// <summary>
            /// Gets or sets the go to line number x.
            /// </summary>
            public string GoToLineNumberX { get; set; }

            /// <summary>
            /// Gets or sets the create adjust changes applied.
            /// </summary>
            public string CreateAdjustChangesApplied { get; set; }

            /// <summary>
            /// Gets or sets the selected lines.
            /// </summary>
            public string SelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the before display time adjustment.
            /// </summary>
            public string BeforeDisplayTimeAdjustment { get; set; }

            /// <summary>
            /// Gets or sets the display time adjusted x.
            /// </summary>
            public string DisplayTimeAdjustedX { get; set; }

            /// <summary>
            /// Gets or sets the display times adjusted x.
            /// </summary>
            public string DisplayTimesAdjustedX { get; set; }

            /// <summary>
            /// Gets or sets the star time adjusted x.
            /// </summary>
            public string StarTimeAdjustedX { get; set; }

            /// <summary>
            /// Gets or sets the before common error fixes.
            /// </summary>
            public string BeforeCommonErrorFixes { get; set; }

            /// <summary>
            /// Gets or sets the common errors fixed in selected lines.
            /// </summary>
            public string CommonErrorsFixedInSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the common errors fixed.
            /// </summary>
            public string CommonErrorsFixed { get; set; }

            /// <summary>
            /// Gets or sets the before renumbering.
            /// </summary>
            public string BeforeRenumbering { get; set; }

            /// <summary>
            /// Gets or sets the renumbered starting from x.
            /// </summary>
            public string RenumberedStartingFromX { get; set; }

            /// <summary>
            /// Gets or sets the before removal of texting for hearing impaired.
            /// </summary>
            public string BeforeRemovalOfTextingForHearingImpaired { get; set; }

            /// <summary>
            /// Gets or sets the texting for hearing impaired removed one line.
            /// </summary>
            public string TextingForHearingImpairedRemovedOneLine { get; set; }

            /// <summary>
            /// Gets or sets the texting for hearing impaired removed x lines.
            /// </summary>
            public string TextingForHearingImpairedRemovedXLines { get; set; }

            /// <summary>
            /// Gets or sets the subtitle splitted.
            /// </summary>
            public string SubtitleSplitted { get; set; }

            /// <summary>
            /// Gets or sets the subtitle append prompt.
            /// </summary>
            public string SubtitleAppendPrompt { get; set; }

            /// <summary>
            /// Gets or sets the subtitle append prompt title.
            /// </summary>
            public string SubtitleAppendPromptTitle { get; set; }

            /// <summary>
            /// Gets or sets the open subtitle to append.
            /// </summary>
            public string OpenSubtitleToAppend { get; set; }

            /// <summary>
            /// Gets or sets the append via visual sync title.
            /// </summary>
            public string AppendViaVisualSyncTitle { get; set; }

            /// <summary>
            /// Gets or sets the append synchronized subtitle prompt.
            /// </summary>
            public string AppendSynchronizedSubtitlePrompt { get; set; }

            /// <summary>
            /// Gets or sets the before append.
            /// </summary>
            public string BeforeAppend { get; set; }

            /// <summary>
            /// Gets or sets the subtitle appended x.
            /// </summary>
            public string SubtitleAppendedX { get; set; }

            /// <summary>
            /// Gets or sets the subtitle not appended.
            /// </summary>
            public string SubtitleNotAppended { get; set; }

            /// <summary>
            /// Gets or sets the google translate.
            /// </summary>
            public string GoogleTranslate { get; set; }

            /// <summary>
            /// Gets or sets the microsoft translate.
            /// </summary>
            public string MicrosoftTranslate { get; set; }

            /// <summary>
            /// Gets or sets the before google translation.
            /// </summary>
            public string BeforeGoogleTranslation { get; set; }

            /// <summary>
            /// Gets or sets the selected lines translated.
            /// </summary>
            public string SelectedLinesTranslated { get; set; }

            /// <summary>
            /// Gets or sets the subtitle translated.
            /// </summary>
            public string SubtitleTranslated { get; set; }

            /// <summary>
            /// Gets or sets the translate swedish to danish.
            /// </summary>
            public string TranslateSwedishToDanish { get; set; }

            /// <summary>
            /// Gets or sets the translate swedish to danish warning.
            /// </summary>
            public string TranslateSwedishToDanishWarning { get; set; }

            /// <summary>
            /// Gets or sets the translating via nikse dk mt.
            /// </summary>
            public string TranslatingViaNikseDkMt { get; set; }

            /// <summary>
            /// Gets or sets the before swedish to danish translation.
            /// </summary>
            public string BeforeSwedishToDanishTranslation { get; set; }

            /// <summary>
            /// Gets or sets the translation from swedish to danish complete.
            /// </summary>
            public string TranslationFromSwedishToDanishComplete { get; set; }

            /// <summary>
            /// Gets or sets the translation from swedish to danish failed.
            /// </summary>
            public string TranslationFromSwedishToDanishFailed { get; set; }

            /// <summary>
            /// Gets or sets the before undo.
            /// </summary>
            public string BeforeUndo { get; set; }

            /// <summary>
            /// Gets or sets the undo performed.
            /// </summary>
            public string UndoPerformed { get; set; }

            /// <summary>
            /// Gets or sets the redo performed.
            /// </summary>
            public string RedoPerformed { get; set; }

            /// <summary>
            /// Gets or sets the nothing to undo.
            /// </summary>
            public string NothingToUndo { get; set; }

            /// <summary>
            /// Gets or sets the invalid language name x.
            /// </summary>
            public string InvalidLanguageNameX { get; set; }

            /// <summary>
            /// Gets or sets the unable to change language.
            /// </summary>
            public string UnableToChangeLanguage { get; set; }

            /// <summary>
            /// Gets or sets the do not display message again.
            /// </summary>
            public string DoNotDisplayMessageAgain { get; set; }

            /// <summary>
            /// Gets or sets the number of corrected words.
            /// </summary>
            public string NumberOfCorrectedWords { get; set; }

            /// <summary>
            /// Gets or sets the number of skipped words.
            /// </summary>
            public string NumberOfSkippedWords { get; set; }

            /// <summary>
            /// Gets or sets the number of correct words.
            /// </summary>
            public string NumberOfCorrectWords { get; set; }

            /// <summary>
            /// Gets or sets the number of words added to dictionary.
            /// </summary>
            public string NumberOfWordsAddedToDictionary { get; set; }

            /// <summary>
            /// Gets or sets the number of name hits.
            /// </summary>
            public string NumberOfNameHits { get; set; }

            /// <summary>
            /// Gets or sets the spell check.
            /// </summary>
            public string SpellCheck { get; set; }

            /// <summary>
            /// Gets or sets the before spell check.
            /// </summary>
            public string BeforeSpellCheck { get; set; }

            /// <summary>
            /// Gets or sets the spell check changed x to y.
            /// </summary>
            public string SpellCheckChangedXToY { get; set; }

            /// <summary>
            /// Gets or sets the before adding tag x.
            /// </summary>
            public string BeforeAddingTagX { get; set; }

            /// <summary>
            /// Gets or sets the tag x added.
            /// </summary>
            public string TagXAdded { get; set; }

            /// <summary>
            /// Gets or sets the line x of y.
            /// </summary>
            public string LineXOfY { get; set; }

            /// <summary>
            /// Gets or sets the x lines saved as y.
            /// </summary>
            public string XLinesSavedAsY { get; set; }

            /// <summary>
            /// Gets or sets the x lines deleted.
            /// </summary>
            public string XLinesDeleted { get; set; }

            /// <summary>
            /// Gets or sets the before deleting x lines.
            /// </summary>
            public string BeforeDeletingXLines { get; set; }

            /// <summary>
            /// Gets or sets the delete x lines prompt.
            /// </summary>
            public string DeleteXLinesPrompt { get; set; }

            /// <summary>
            /// Gets or sets the one line deleted.
            /// </summary>
            public string OneLineDeleted { get; set; }

            /// <summary>
            /// Gets or sets the before deleting one line.
            /// </summary>
            public string BeforeDeletingOneLine { get; set; }

            /// <summary>
            /// Gets or sets the delete one line prompt.
            /// </summary>
            public string DeleteOneLinePrompt { get; set; }

            /// <summary>
            /// Gets or sets the before insert line.
            /// </summary>
            public string BeforeInsertLine { get; set; }

            /// <summary>
            /// Gets or sets the line inserted.
            /// </summary>
            public string LineInserted { get; set; }

            /// <summary>
            /// Gets or sets the before line updated in list view.
            /// </summary>
            public string BeforeLineUpdatedInListView { get; set; }

            /// <summary>
            /// Gets or sets the before setting font to normal.
            /// </summary>
            public string BeforeSettingFontToNormal { get; set; }

            /// <summary>
            /// Gets or sets the before split line.
            /// </summary>
            public string BeforeSplitLine { get; set; }

            /// <summary>
            /// Gets or sets the line splitted.
            /// </summary>
            public string LineSplitted { get; set; }

            /// <summary>
            /// Gets or sets the before merge lines.
            /// </summary>
            public string BeforeMergeLines { get; set; }

            /// <summary>
            /// Gets or sets the lines merged.
            /// </summary>
            public string LinesMerged { get; set; }

            /// <summary>
            /// Gets or sets the before setting color.
            /// </summary>
            public string BeforeSettingColor { get; set; }

            /// <summary>
            /// Gets or sets the before setting font name.
            /// </summary>
            public string BeforeSettingFontName { get; set; }

            /// <summary>
            /// Gets or sets the before type writer effect.
            /// </summary>
            public string BeforeTypeWriterEffect { get; set; }

            /// <summary>
            /// Gets or sets the before karaoke effect.
            /// </summary>
            public string BeforeKaraokeEffect { get; set; }

            /// <summary>
            /// Gets or sets the before importing dvd subtitle.
            /// </summary>
            public string BeforeImportingDvdSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the open matroska file.
            /// </summary>
            public string OpenMatroskaFile { get; set; }

            /// <summary>
            /// Gets or sets the matroska files.
            /// </summary>
            public string MatroskaFiles { get; set; }

            /// <summary>
            /// Gets or sets the no subtitles found.
            /// </summary>
            public string NoSubtitlesFound { get; set; }

            /// <summary>
            /// Gets or sets the not a valid matroska file x.
            /// </summary>
            public string NotAValidMatroskaFileX { get; set; }

            /// <summary>
            /// Gets or sets the bluray not subtitles found.
            /// </summary>
            public string BlurayNotSubtitlesFound { get; set; }

            /// <summary>
            /// Gets or sets the parsing matroska file.
            /// </summary>
            public string ParsingMatroskaFile { get; set; }

            /// <summary>
            /// Gets or sets the parsing transport stream file.
            /// </summary>
            public string ParsingTransportStreamFile { get; set; }

            /// <summary>
            /// Gets or sets the before import from matroska file.
            /// </summary>
            public string BeforeImportFromMatroskaFile { get; set; }

            /// <summary>
            /// Gets or sets the subtitle imported from matroska file.
            /// </summary>
            public string SubtitleImportedFromMatroskaFile { get; set; }

            /// <summary>
            /// Gets or sets the drop file x not accepted.
            /// </summary>
            public string DropFileXNotAccepted { get; set; }

            /// <summary>
            /// Gets or sets the drop only one file.
            /// </summary>
            public string DropOnlyOneFile { get; set; }

            /// <summary>
            /// Gets or sets the before create adjust lines.
            /// </summary>
            public string BeforeCreateAdjustLines { get; set; }

            /// <summary>
            /// Gets or sets the open ansi subtitle.
            /// </summary>
            public string OpenAnsiSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the before change casing.
            /// </summary>
            public string BeforeChangeCasing { get; set; }

            /// <summary>
            /// Gets or sets the casing complete message no names.
            /// </summary>
            public string CasingCompleteMessageNoNames { get; set; }

            /// <summary>
            /// Gets or sets the casing complete message only names.
            /// </summary>
            public string CasingCompleteMessageOnlyNames { get; set; }

            /// <summary>
            /// Gets or sets the casing complete message.
            /// </summary>
            public string CasingCompleteMessage { get; set; }

            /// <summary>
            /// Gets or sets the before change frame rate.
            /// </summary>
            public string BeforeChangeFrameRate { get; set; }

            /// <summary>
            /// Gets or sets the before adjust speed in percent.
            /// </summary>
            public string BeforeAdjustSpeedInPercent { get; set; }

            /// <summary>
            /// Gets or sets the frame rate changed from x to y.
            /// </summary>
            public string FrameRateChangedFromXToY { get; set; }

            /// <summary>
            /// Gets or sets the idx file not found warning.
            /// </summary>
            public string IdxFileNotFoundWarning { get; set; }

            /// <summary>
            /// Gets or sets the invalid vob sub header.
            /// </summary>
            public string InvalidVobSubHeader { get; set; }

            /// <summary>
            /// Gets or sets the open vob sub file.
            /// </summary>
            public string OpenVobSubFile { get; set; }

            /// <summary>
            /// Gets or sets the vob sub files.
            /// </summary>
            public string VobSubFiles { get; set; }

            /// <summary>
            /// Gets or sets the open blu ray sup file.
            /// </summary>
            public string OpenBluRaySupFile { get; set; }

            /// <summary>
            /// Gets or sets the blu ray sup files.
            /// </summary>
            public string BluRaySupFiles { get; set; }

            /// <summary>
            /// Gets or sets the open x sub files.
            /// </summary>
            public string OpenXSubFiles { get; set; }

            /// <summary>
            /// Gets or sets the x sub files.
            /// </summary>
            public string XSubFiles { get; set; }

            /// <summary>
            /// Gets or sets the before importing vob sub file.
            /// </summary>
            public string BeforeImportingVobSubFile { get; set; }

            /// <summary>
            /// Gets or sets the before importing blu ray sup file.
            /// </summary>
            public string BeforeImportingBluRaySupFile { get; set; }

            /// <summary>
            /// Gets or sets the before importing bdn xml.
            /// </summary>
            public string BeforeImportingBdnXml { get; set; }

            /// <summary>
            /// Gets or sets the before show selected lines earlier later.
            /// </summary>
            public string BeforeShowSelectedLinesEarlierLater { get; set; }

            /// <summary>
            /// Gets or sets the show all lines x seconds lines earlier.
            /// </summary>
            public string ShowAllLinesXSecondsLinesEarlier { get; set; }

            /// <summary>
            /// Gets or sets the show all lines x seconds lines later.
            /// </summary>
            public string ShowAllLinesXSecondsLinesLater { get; set; }

            /// <summary>
            /// Gets or sets the show selected lines x seconds lines earlier.
            /// </summary>
            public string ShowSelectedLinesXSecondsLinesEarlier { get; set; }

            /// <summary>
            /// Gets or sets the show selected lines x seconds lines later.
            /// </summary>
            public string ShowSelectedLinesXSecondsLinesLater { get; set; }

            /// <summary>
            /// Gets or sets the show selection and forward x seconds lines earlier.
            /// </summary>
            public string ShowSelectionAndForwardXSecondsLinesEarlier { get; set; }

            /// <summary>
            /// Gets or sets the show selection and forward x seconds lines later.
            /// </summary>
            public string ShowSelectionAndForwardXSecondsLinesLater { get; set; }

            /// <summary>
            /// Gets or sets the show selected lines earlier later performed.
            /// </summary>
            public string ShowSelectedLinesEarlierLaterPerformed { get; set; }

            /// <summary>
            /// Gets or sets the double words via reg ex.
            /// </summary>
            public string DoubleWordsViaRegEx { get; set; }

            /// <summary>
            /// Gets or sets the before sort x.
            /// </summary>
            public string BeforeSortX { get; set; }

            /// <summary>
            /// Gets or sets the sorted by x.
            /// </summary>
            public string SortedByX { get; set; }

            /// <summary>
            /// Gets or sets the before auto balance selected lines.
            /// </summary>
            public string BeforeAutoBalanceSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the number of lines auto balanced x.
            /// </summary>
            public string NumberOfLinesAutoBalancedX { get; set; }

            /// <summary>
            /// Gets or sets the before remove line breaks in selected lines.
            /// </summary>
            public string BeforeRemoveLineBreaksInSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the number of with removed line break x.
            /// </summary>
            public string NumberOfWithRemovedLineBreakX { get; set; }

            /// <summary>
            /// Gets or sets the before multiple replace.
            /// </summary>
            public string BeforeMultipleReplace { get; set; }

            /// <summary>
            /// Gets or sets the number of lines replaced x.
            /// </summary>
            public string NumberOfLinesReplacedX { get; set; }

            /// <summary>
            /// Gets or sets the name x added to names etc list.
            /// </summary>
            public string NameXAddedToNamesEtcList { get; set; }

            /// <summary>
            /// Gets or sets the name x not added to names etc list.
            /// </summary>
            public string NameXNotAddedToNamesEtcList { get; set; }

            /// <summary>
            /// Gets or sets the word x added to user dic.
            /// </summary>
            public string WordXAddedToUserDic { get; set; }

            /// <summary>
            /// Gets or sets the word x not added to user dic.
            /// </summary>
            public string WordXNotAddedToUserDic { get; set; }

            /// <summary>
            /// Gets or sets the ocr replace pair x added.
            /// </summary>
            public string OcrReplacePairXAdded { get; set; }

            /// <summary>
            /// Gets or sets the ocr replace pair x not added.
            /// </summary>
            public string OcrReplacePairXNotAdded { get; set; }

            /// <summary>
            /// Gets or sets the x lines selected.
            /// </summary>
            public string XLinesSelected { get; set; }

            /// <summary>
            /// Gets or sets the unicode music symbols ansi warning.
            /// </summary>
            public string UnicodeMusicSymbolsAnsiWarning { get; set; }

            /// <summary>
            /// Gets or sets the unicode characters ansi warning.
            /// </summary>
            public string UnicodeCharactersAnsiWarning { get; set; }

            /// <summary>
            /// Gets or sets the negative time warning.
            /// </summary>
            public string NegativeTimeWarning { get; set; }

            /// <summary>
            /// Gets or sets the before merge short lines.
            /// </summary>
            public string BeforeMergeShortLines { get; set; }

            /// <summary>
            /// Gets or sets the before split long lines.
            /// </summary>
            public string BeforeSplitLongLines { get; set; }

            /// <summary>
            /// Gets or sets the merged short lines x.
            /// </summary>
            public string MergedShortLinesX { get; set; }

            /// <summary>
            /// Gets or sets the before durations bridge gap.
            /// </summary>
            public string BeforeDurationsBridgeGap { get; set; }

            /// <summary>
            /// Gets or sets the before set minimum display time between paragraphs.
            /// </summary>
            public string BeforeSetMinimumDisplayTimeBetweenParagraphs { get; set; }

            /// <summary>
            /// Gets or sets the x minimum display time between paragraphs changed.
            /// </summary>
            public string XMinimumDisplayTimeBetweenParagraphsChanged { get; set; }

            /// <summary>
            /// Gets or sets the before import text.
            /// </summary>
            public string BeforeImportText { get; set; }

            /// <summary>
            /// Gets or sets the text imported.
            /// </summary>
            public string TextImported { get; set; }

            /// <summary>
            /// Gets or sets the before point synchronization.
            /// </summary>
            public string BeforePointSynchronization { get; set; }

            /// <summary>
            /// Gets or sets the point synchronization done.
            /// </summary>
            public string PointSynchronizationDone { get; set; }

            /// <summary>
            /// Gets or sets the before time code import.
            /// </summary>
            public string BeforeTimeCodeImport { get; set; }

            /// <summary>
            /// Gets or sets the time code imported from xy.
            /// </summary>
            public string TimeCodeImportedFromXY { get; set; }

            /// <summary>
            /// Gets or sets the before insert subtitle at video position.
            /// </summary>
            public string BeforeInsertSubtitleAtVideoPosition { get; set; }

            /// <summary>
            /// Gets or sets the before set start time and offset the rest.
            /// </summary>
            public string BeforeSetStartTimeAndOffsetTheRest { get; set; }

            /// <summary>
            /// Gets or sets the before set end time and offset the rest.
            /// </summary>
            public string BeforeSetEndTimeAndOffsetTheRest { get; set; }

            /// <summary>
            /// Gets or sets the before set end and video position.
            /// </summary>
            public string BeforeSetEndAndVideoPosition { get; set; }

            /// <summary>
            /// Gets or sets the continue with current spell check.
            /// </summary>
            public string ContinueWithCurrentSpellCheck { get; set; }

            /// <summary>
            /// Gets or sets the characters per second.
            /// </summary>
            public string CharactersPerSecond { get; set; }

            /// <summary>
            /// Gets or sets the get frame rate from video file.
            /// </summary>
            public string GetFrameRateFromVideoFile { get; set; }

            /// <summary>
            /// Gets or sets the network message.
            /// </summary>
            public string NetworkMessage { get; set; }

            /// <summary>
            /// Gets or sets the network update.
            /// </summary>
            public string NetworkUpdate { get; set; }

            /// <summary>
            /// Gets or sets the network insert.
            /// </summary>
            public string NetworkInsert { get; set; }

            /// <summary>
            /// Gets or sets the network delete.
            /// </summary>
            public string NetworkDelete { get; set; }

            /// <summary>
            /// Gets or sets the network new user.
            /// </summary>
            public string NetworkNewUser { get; set; }

            /// <summary>
            /// Gets or sets the network bye user.
            /// </summary>
            public string NetworkByeUser { get; set; }

            /// <summary>
            /// Gets or sets the network unable to connect to server.
            /// </summary>
            public string NetworkUnableToConnectToServer { get; set; }

            /// <summary>
            /// Gets or sets the user and action.
            /// </summary>
            public string UserAndAction { get; set; }

            /// <summary>
            /// Gets or sets the network mode.
            /// </summary>
            public string NetworkMode { get; set; }

            /// <summary>
            /// Gets or sets the x started session y at z.
            /// </summary>
            public string XStartedSessionYAtZ { get; set; }

            /// <summary>
            /// Gets or sets the spell cheking via word x line y of x.
            /// </summary>
            public string SpellChekingViaWordXLineYOfX { get; set; }

            /// <summary>
            /// Gets or sets the unable to start word.
            /// </summary>
            public string UnableToStartWord { get; set; }

            /// <summary>
            /// Gets or sets the spell check aborted x corrections.
            /// </summary>
            public string SpellCheckAbortedXCorrections { get; set; }

            /// <summary>
            /// Gets or sets the spell check completed x corrections.
            /// </summary>
            public string SpellCheckCompletedXCorrections { get; set; }

            /// <summary>
            /// Gets or sets the open other subtitle.
            /// </summary>
            public string OpenOtherSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the before toggle dialog dashes.
            /// </summary>
            public string BeforeToggleDialogDashes { get; set; }

            /// <summary>
            /// Gets or sets the export plain text as.
            /// </summary>
            public string ExportPlainTextAs { get; set; }

            /// <summary>
            /// Gets or sets the text files.
            /// </summary>
            public string TextFiles { get; set; }

            /// <summary>
            /// Gets or sets the subtitle exported.
            /// </summary>
            public string SubtitleExported { get; set; }

            /// <summary>
            /// Gets or sets the line number x error reading from source line y.
            /// </summary>
            public string LineNumberXErrorReadingFromSourceLineY { get; set; }

            /// <summary>
            /// Gets or sets the line number x error reading time code from source line y.
            /// </summary>
            public string LineNumberXErrorReadingTimeCodeFromSourceLineY { get; set; }

            /// <summary>
            /// Gets or sets the line number x expected number from source line y.
            /// </summary>
            public string LineNumberXExpectedNumberFromSourceLineY { get; set; }

            /// <summary>
            /// Gets or sets the before guessing time codes.
            /// </summary>
            public string BeforeGuessingTimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the before auto duration.
            /// </summary>
            public string BeforeAutoDuration { get; set; }

            /// <summary>
            /// Gets or sets the before column paste.
            /// </summary>
            public string BeforeColumnPaste { get; set; }

            /// <summary>
            /// Gets or sets the before column delete.
            /// </summary>
            public string BeforeColumnDelete { get; set; }

            /// <summary>
            /// Gets or sets the before column import text.
            /// </summary>
            public string BeforeColumnImportText { get; set; }

            /// <summary>
            /// Gets or sets the before column shift cells down.
            /// </summary>
            public string BeforeColumnShiftCellsDown { get; set; }

            /// <summary>
            /// Gets or sets the error loading plugin x error y.
            /// </summary>
            public string ErrorLoadingPluginXErrorY { get; set; }

            /// <summary>
            /// Gets or sets the before running plugin x version y.
            /// </summary>
            public string BeforeRunningPluginXVersionY { get; set; }

            /// <summary>
            /// Gets or sets the unable to read plugin result.
            /// </summary>
            public string UnableToReadPluginResult { get; set; }

            /// <summary>
            /// Gets or sets the unable to create backup directory.
            /// </summary>
            public string UnableToCreateBackupDirectory { get; set; }

            /// <summary>
            /// Gets or sets the before display subtitle join.
            /// </summary>
            public string BeforeDisplaySubtitleJoin { get; set; }

            /// <summary>
            /// Gets or sets the subtitles joined.
            /// </summary>
            public string SubtitlesJoined { get; set; }

            /// <summary>
            /// Gets or sets the status log.
            /// </summary>
            public string StatusLog { get; set; }

            /// <summary>
            /// Gets or sets the x scene changes imported.
            /// </summary>
            public string XSceneChangesImported { get; set; }

            /// <summary>
            /// Gets or sets the plugin x executed.
            /// </summary>
            public string PluginXExecuted { get; set; }

            /// <summary>
            /// Gets or sets the not a valid x sub file.
            /// </summary>
            public string NotAValidXSubFile { get; set; }

            /// <summary>
            /// Gets or sets the before merge lines with same text.
            /// </summary>
            public string BeforeMergeLinesWithSameText { get; set; }

            /// <summary>
            /// Gets or sets the import time codes different number of lines warning.
            /// </summary>
            public string ImportTimeCodesDifferentNumberOfLinesWarning { get; set; }

            /// <summary>
            /// Gets or sets the parsing transport stream.
            /// </summary>
            public string ParsingTransportStream { get; set; }

            /// <summary>
            /// Gets or sets the error load idx.
            /// </summary>
            public string ErrorLoadIdx { get; set; }

            /// <summary>
            /// Gets or sets the error load rar.
            /// </summary>
            public string ErrorLoadRar { get; set; }

            /// <summary>
            /// Gets or sets the error load zip.
            /// </summary>
            public string ErrorLoadZip { get; set; }

            /// <summary>
            /// Gets or sets the error load png.
            /// </summary>
            public string ErrorLoadPng { get; set; }

            /// <summary>
            /// Gets or sets the error load jpg.
            /// </summary>
            public string ErrorLoadJpg { get; set; }

            /// <summary>
            /// Gets or sets the error load srr.
            /// </summary>
            public string ErrorLoadSrr { get; set; }

            /// <summary>
            /// Gets or sets the error load torrent.
            /// </summary>
            public string ErrorLoadTorrent { get; set; }

            /// <summary>
            /// Gets or sets the error load binary zeroes.
            /// </summary>
            public string ErrorLoadBinaryZeroes { get; set; }

            /// <summary>
            /// Gets or sets the error directory drop not allowed.
            /// </summary>
            public string ErrorDirectoryDropNotAllowed { get; set; }

            /// <summary>
            /// Gets or sets the no support encrypted vob sub.
            /// </summary>
            public string NoSupportEncryptedVobSub { get; set; }

            /// <summary>
            /// Gets or sets the no support here blu ray sup.
            /// </summary>
            public string NoSupportHereBluRaySup { get; set; }

            /// <summary>
            /// Gets or sets the no support here dvd sup.
            /// </summary>
            public string NoSupportHereDvdSup { get; set; }

            /// <summary>
            /// Gets or sets the no support here vob sub.
            /// </summary>
            public string NoSupportHereVobSub { get; set; }

            /// <summary>
            /// Gets or sets the no support here divx.
            /// </summary>
            public string NoSupportHereDivx { get; set; }

            /// <summary>
            /// The main menu.
            /// </summary>
            public class MainMenu
            {
                /// <summary>
                /// Gets or sets the file.
                /// </summary>
                public FileMenu File { get; set; }

                /// <summary>
                /// Gets or sets the edit.
                /// </summary>
                public EditMenu Edit { get; set; }

                /// <summary>
                /// Gets or sets the tools.
                /// </summary>
                public ToolsMenu Tools { get; set; }

                /// <summary>
                /// Gets or sets the video.
                /// </summary>
                public VideoMenu Video { get; set; }

                /// <summary>
                /// Gets or sets the spell check.
                /// </summary>
                public SpellCheckMenu SpellCheck { get; set; }

                /// <summary>
                /// Gets or sets the synchronization.
                /// </summary>
                public SynchronizationkMenu Synchronization { get; set; }

                /// <summary>
                /// Gets or sets the auto translate.
                /// </summary>
                public AutoTranslateMenu AutoTranslate { get; set; }

                /// <summary>
                /// Gets or sets the options.
                /// </summary>
                public OptionsMenu Options { get; set; }

                /// <summary>
                /// Gets or sets the networking.
                /// </summary>
                public NetworkingMenu Networking { get; set; }

                /// <summary>
                /// Gets or sets the help.
                /// </summary>
                public HelpMenu Help { get; set; }

                /// <summary>
                /// Gets or sets the tool bar.
                /// </summary>
                public ToolBarMenu ToolBar { get; set; }

                /// <summary>
                /// Gets or sets the context menu.
                /// </summary>
                public ListViewContextMenu ContextMenu { get; set; }

                /// <summary>
                /// The file menu.
                /// </summary>
                public class FileMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the new.
                    /// </summary>
                    public string New { get; set; }

                    /// <summary>
                    /// Gets or sets the open.
                    /// </summary>
                    public string Open { get; set; }

                    /// <summary>
                    /// Gets or sets the open keep video.
                    /// </summary>
                    public string OpenKeepVideo { get; set; }

                    /// <summary>
                    /// Gets or sets the reopen.
                    /// </summary>
                    public string Reopen { get; set; }

                    /// <summary>
                    /// Gets or sets the save.
                    /// </summary>
                    public string Save { get; set; }

                    /// <summary>
                    /// Gets or sets the save as.
                    /// </summary>
                    public string SaveAs { get; set; }

                    /// <summary>
                    /// Gets or sets the restore auto backup.
                    /// </summary>
                    public string RestoreAutoBackup { get; set; }

                    /// <summary>
                    /// Gets or sets the advanced sub station alpha properties.
                    /// </summary>
                    public string AdvancedSubStationAlphaProperties { get; set; }

                    /// <summary>
                    /// Gets or sets the sub station alpha properties.
                    /// </summary>
                    public string SubStationAlphaProperties { get; set; }

                    /// <summary>
                    /// Gets or sets the ebu properties.
                    /// </summary>
                    public string EbuProperties { get; set; }

                    /// <summary>
                    /// Gets or sets the pac properties.
                    /// </summary>
                    public string PacProperties { get; set; }

                    /// <summary>
                    /// Gets or sets the open original.
                    /// </summary>
                    public string OpenOriginal { get; set; }

                    /// <summary>
                    /// Gets or sets the save original.
                    /// </summary>
                    public string SaveOriginal { get; set; }

                    /// <summary>
                    /// Gets or sets the close original.
                    /// </summary>
                    public string CloseOriginal { get; set; }

                    /// <summary>
                    /// Gets or sets the open containing folder.
                    /// </summary>
                    public string OpenContainingFolder { get; set; }

                    /// <summary>
                    /// Gets or sets the compare.
                    /// </summary>
                    public string Compare { get; set; }

                    /// <summary>
                    /// Gets or sets the statistics.
                    /// </summary>
                    public string Statistics { get; set; }

                    /// <summary>
                    /// Gets or sets the plugins.
                    /// </summary>
                    public string Plugins { get; set; }

                    /// <summary>
                    /// Gets or sets the import ocr from dvd.
                    /// </summary>
                    public string ImportOcrFromDvd { get; set; }

                    /// <summary>
                    /// Gets or sets the import ocr vob sub subtitle.
                    /// </summary>
                    public string ImportOcrVobSubSubtitle { get; set; }

                    /// <summary>
                    /// Gets or sets the import blu ray sup file.
                    /// </summary>
                    public string ImportBluRaySupFile { get; set; }

                    /// <summary>
                    /// Gets or sets the import x sub.
                    /// </summary>
                    public string ImportXSub { get; set; }

                    /// <summary>
                    /// Gets or sets the import subtitle from matroska file.
                    /// </summary>
                    public string ImportSubtitleFromMatroskaFile { get; set; }

                    /// <summary>
                    /// Gets or sets the import subtitle with manual chosen encoding.
                    /// </summary>
                    public string ImportSubtitleWithManualChosenEncoding { get; set; }

                    /// <summary>
                    /// Gets or sets the import text.
                    /// </summary>
                    public string ImportText { get; set; }

                    /// <summary>
                    /// Gets or sets the import images.
                    /// </summary>
                    public string ImportImages { get; set; }

                    /// <summary>
                    /// Gets or sets the import timecodes.
                    /// </summary>
                    public string ImportTimecodes { get; set; }

                    /// <summary>
                    /// Gets or sets the export.
                    /// </summary>
                    public string Export { get; set; }

                    /// <summary>
                    /// Gets or sets the export bdn xml.
                    /// </summary>
                    public string ExportBdnXml { get; set; }

                    /// <summary>
                    /// Gets or sets the export blu ray sup.
                    /// </summary>
                    public string ExportBluRaySup { get; set; }

                    /// <summary>
                    /// Gets or sets the export vob sub.
                    /// </summary>
                    public string ExportVobSub { get; set; }

                    /// <summary>
                    /// Gets or sets the export cavena 890.
                    /// </summary>
                    public string ExportCavena890 { get; set; }

                    /// <summary>
                    /// Gets or sets the export ebu.
                    /// </summary>
                    public string ExportEbu { get; set; }

                    /// <summary>
                    /// Gets or sets the export pac.
                    /// </summary>
                    public string ExportPac { get; set; }

                    /// <summary>
                    /// Gets or sets the export plain text.
                    /// </summary>
                    public string ExportPlainText { get; set; }

                    /// <summary>
                    /// Gets or sets the export adobe encore fab image script.
                    /// </summary>
                    public string ExportAdobeEncoreFabImageScript { get; set; }

                    /// <summary>
                    /// Gets or sets the export korean ats file pair.
                    /// </summary>
                    public string ExportKoreanAtsFilePair { get; set; }

                    /// <summary>
                    /// Gets or sets the export avid stl.
                    /// </summary>
                    public string ExportAvidStl { get; set; }

                    /// <summary>
                    /// Gets or sets the export dvd studio pro stl.
                    /// </summary>
                    public string ExportDvdStudioProStl { get; set; }

                    /// <summary>
                    /// Gets or sets the export cap maker plus.
                    /// </summary>
                    public string ExportCapMakerPlus { get; set; }

                    /// <summary>
                    /// Gets or sets the export captions inc.
                    /// </summary>
                    public string ExportCaptionsInc { get; set; }

                    /// <summary>
                    /// Gets or sets the export cheetah cap.
                    /// </summary>
                    public string ExportCheetahCap { get; set; }

                    /// <summary>
                    /// Gets or sets the export ultech 130.
                    /// </summary>
                    public string ExportUltech130 { get; set; }

                    /// <summary>
                    /// Gets or sets the export custom text format.
                    /// </summary>
                    public string ExportCustomTextFormat { get; set; }

                    /// <summary>
                    /// Gets or sets the exit.
                    /// </summary>
                    public string Exit { get; set; }
                }

                /// <summary>
                /// The edit menu.
                /// </summary>
                public class EditMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the undo.
                    /// </summary>
                    public string Undo { get; set; }

                    /// <summary>
                    /// Gets or sets the redo.
                    /// </summary>
                    public string Redo { get; set; }

                    /// <summary>
                    /// Gets or sets the show undo history.
                    /// </summary>
                    public string ShowUndoHistory { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode symbol.
                    /// </summary>
                    public string InsertUnicodeSymbol { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters.
                    /// </summary>
                    public string InsertUnicodeControlCharacters { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters lrm.
                    /// </summary>
                    public string InsertUnicodeControlCharactersLRM { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters rlm.
                    /// </summary>
                    public string InsertUnicodeControlCharactersRLM { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters lre.
                    /// </summary>
                    public string InsertUnicodeControlCharactersLRE { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters rle.
                    /// </summary>
                    public string InsertUnicodeControlCharactersRLE { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters lro.
                    /// </summary>
                    public string InsertUnicodeControlCharactersLRO { get; set; }

                    /// <summary>
                    /// Gets or sets the insert unicode control characters rlo.
                    /// </summary>
                    public string InsertUnicodeControlCharactersRLO { get; set; }

                    /// <summary>
                    /// Gets or sets the find.
                    /// </summary>
                    public string Find { get; set; }

                    /// <summary>
                    /// Gets or sets the find next.
                    /// </summary>
                    public string FindNext { get; set; }

                    /// <summary>
                    /// Gets or sets the replace.
                    /// </summary>
                    public string Replace { get; set; }

                    /// <summary>
                    /// Gets or sets the multiple replace.
                    /// </summary>
                    public string MultipleReplace { get; set; }

                    /// <summary>
                    /// Gets or sets the go to subtitle number.
                    /// </summary>
                    public string GoToSubtitleNumber { get; set; }

                    /// <summary>
                    /// Gets or sets the right to left mode.
                    /// </summary>
                    public string RightToLeftMode { get; set; }

                    /// <summary>
                    /// Gets or sets the fix trl via unicode control characters.
                    /// </summary>
                    public string FixTrlViaUnicodeControlCharacters { get; set; }

                    /// <summary>
                    /// Gets or sets the reverse right to left start end.
                    /// </summary>
                    public string ReverseRightToLeftStartEnd { get; set; }

                    /// <summary>
                    /// Gets or sets the show original text in audio and video preview.
                    /// </summary>
                    public string ShowOriginalTextInAudioAndVideoPreview { get; set; }

                    /// <summary>
                    /// Gets or sets the modify selection.
                    /// </summary>
                    public string ModifySelection { get; set; }

                    /// <summary>
                    /// Gets or sets the inverse selection.
                    /// </summary>
                    public string InverseSelection { get; set; }
                }

                /// <summary>
                /// The tools menu.
                /// </summary>
                public class ToolsMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the adjust display duration.
                    /// </summary>
                    public string AdjustDisplayDuration { get; set; }

                    /// <summary>
                    /// Gets or sets the apply duration limits.
                    /// </summary>
                    public string ApplyDurationLimits { get; set; }

                    /// <summary>
                    /// Gets or sets the durations bridge gap.
                    /// </summary>
                    public string DurationsBridgeGap { get; set; }

                    /// <summary>
                    /// Gets or sets the fix common errors.
                    /// </summary>
                    public string FixCommonErrors { get; set; }

                    /// <summary>
                    /// Gets or sets the start numbering from.
                    /// </summary>
                    public string StartNumberingFrom { get; set; }

                    /// <summary>
                    /// Gets or sets the remove text for hearing impaired.
                    /// </summary>
                    public string RemoveTextForHearingImpaired { get; set; }

                    /// <summary>
                    /// Gets or sets the change casing.
                    /// </summary>
                    public string ChangeCasing { get; set; }

                    /// <summary>
                    /// Gets or sets the change frame rate.
                    /// </summary>
                    public string ChangeFrameRate { get; set; }

                    /// <summary>
                    /// Gets or sets the change speed in percent.
                    /// </summary>
                    public string ChangeSpeedInPercent { get; set; }

                    /// <summary>
                    /// Gets or sets the merge short lines.
                    /// </summary>
                    public string MergeShortLines { get; set; }

                    /// <summary>
                    /// Gets or sets the merge duplicate text.
                    /// </summary>
                    public string MergeDuplicateText { get; set; }

                    /// <summary>
                    /// Gets or sets the merge same time codes.
                    /// </summary>
                    public string MergeSameTimeCodes { get; set; }

                    /// <summary>
                    /// Gets or sets the split long lines.
                    /// </summary>
                    public string SplitLongLines { get; set; }

                    /// <summary>
                    /// Gets or sets the minimum display time between paragraphs.
                    /// </summary>
                    public string MinimumDisplayTimeBetweenParagraphs { get; set; }

                    /// <summary>
                    /// Gets or sets the sort by.
                    /// </summary>
                    public string SortBy { get; set; }

                    /// <summary>
                    /// Gets or sets the number.
                    /// </summary>
                    public string Number { get; set; }

                    /// <summary>
                    /// Gets or sets the start time.
                    /// </summary>
                    public string StartTime { get; set; }

                    /// <summary>
                    /// Gets or sets the end time.
                    /// </summary>
                    public string EndTime { get; set; }

                    /// <summary>
                    /// Gets or sets the duration.
                    /// </summary>
                    public string Duration { get; set; }

                    /// <summary>
                    /// Gets or sets the text alphabetically.
                    /// </summary>
                    public string TextAlphabetically { get; set; }

                    /// <summary>
                    /// Gets or sets the text single line maximum length.
                    /// </summary>
                    public string TextSingleLineMaximumLength { get; set; }

                    /// <summary>
                    /// Gets or sets the text total length.
                    /// </summary>
                    public string TextTotalLength { get; set; }

                    /// <summary>
                    /// Gets or sets the text number of lines.
                    /// </summary>
                    public string TextNumberOfLines { get; set; }

                    /// <summary>
                    /// Gets or sets the text number of characters per seconds.
                    /// </summary>
                    public string TextNumberOfCharactersPerSeconds { get; set; }

                    /// <summary>
                    /// Gets or sets the words per minute.
                    /// </summary>
                    public string WordsPerMinute { get; set; }

                    /// <summary>
                    /// Gets or sets the style.
                    /// </summary>
                    public string Style { get; set; }

                    /// <summary>
                    /// Gets or sets the ascending.
                    /// </summary>
                    public string Ascending { get; set; }

                    /// <summary>
                    /// Gets or sets the descending.
                    /// </summary>
                    public string Descending { get; set; }

                    /// <summary>
                    /// Gets or sets the make new empty translation from current subtitle.
                    /// </summary>
                    public string MakeNewEmptyTranslationFromCurrentSubtitle { get; set; }

                    /// <summary>
                    /// Gets or sets the batch convert.
                    /// </summary>
                    public string BatchConvert { get; set; }

                    /// <summary>
                    /// Gets or sets the generate time as text.
                    /// </summary>
                    public string GenerateTimeAsText { get; set; }

                    /// <summary>
                    /// Gets or sets the measurement converter.
                    /// </summary>
                    public string MeasurementConverter { get; set; }

                    /// <summary>
                    /// Gets or sets the split subtitle.
                    /// </summary>
                    public string SplitSubtitle { get; set; }

                    /// <summary>
                    /// Gets or sets the append subtitle.
                    /// </summary>
                    public string AppendSubtitle { get; set; }

                    /// <summary>
                    /// Gets or sets the join subtitles.
                    /// </summary>
                    public string JoinSubtitles { get; set; }
                }

                /// <summary>
                /// The video menu.
                /// </summary>
                public class VideoMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the open video.
                    /// </summary>
                    public string OpenVideo { get; set; }

                    /// <summary>
                    /// Gets or sets the open dvd.
                    /// </summary>
                    public string OpenDvd { get; set; }

                    /// <summary>
                    /// Gets or sets the choose audio track.
                    /// </summary>
                    public string ChooseAudioTrack { get; set; }

                    /// <summary>
                    /// Gets or sets the close video.
                    /// </summary>
                    public string CloseVideo { get; set; }

                    /// <summary>
                    /// Gets or sets the import scene changes.
                    /// </summary>
                    public string ImportSceneChanges { get; set; }

                    /// <summary>
                    /// Gets or sets the remove scene changes.
                    /// </summary>
                    public string RemoveSceneChanges { get; set; }

                    /// <summary>
                    /// Gets or sets the waveform batch generate.
                    /// </summary>
                    public string WaveformBatchGenerate { get; set; }

                    /// <summary>
                    /// Gets or sets the show hide video.
                    /// </summary>
                    public string ShowHideVideo { get; set; }

                    /// <summary>
                    /// Gets or sets the show hide waveform.
                    /// </summary>
                    public string ShowHideWaveform { get; set; }

                    /// <summary>
                    /// Gets or sets the show hide waveform and spectrogram.
                    /// </summary>
                    public string ShowHideWaveformAndSpectrogram { get; set; }

                    /// <summary>
                    /// Gets or sets the un dock video controls.
                    /// </summary>
                    public string UnDockVideoControls { get; set; }

                    /// <summary>
                    /// Gets or sets the re dock video controls.
                    /// </summary>
                    public string ReDockVideoControls { get; set; }
                }

                /// <summary>
                /// The spell check menu.
                /// </summary>
                public class SpellCheckMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the spell check.
                    /// </summary>
                    public string SpellCheck { get; set; }

                    /// <summary>
                    /// Gets or sets the spell check from current line.
                    /// </summary>
                    public string SpellCheckFromCurrentLine { get; set; }

                    /// <summary>
                    /// Gets or sets the find double words.
                    /// </summary>
                    public string FindDoubleWords { get; set; }

                    /// <summary>
                    /// Gets or sets the find double lines.
                    /// </summary>
                    public string FindDoubleLines { get; set; }

                    /// <summary>
                    /// Gets or sets the get dictionaries.
                    /// </summary>
                    public string GetDictionaries { get; set; }

                    /// <summary>
                    /// Gets or sets the add to names etc list.
                    /// </summary>
                    public string AddToNamesEtcList { get; set; }
                }

                /// <summary>
                /// The synchronizationk menu.
                /// </summary>
                public class SynchronizationkMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the adjust all times.
                    /// </summary>
                    public string AdjustAllTimes { get; set; }

                    /// <summary>
                    /// Gets or sets the visual sync.
                    /// </summary>
                    public string VisualSync { get; set; }

                    /// <summary>
                    /// Gets or sets the point sync.
                    /// </summary>
                    public string PointSync { get; set; }

                    /// <summary>
                    /// Gets or sets the point sync via other subtitle.
                    /// </summary>
                    public string PointSyncViaOtherSubtitle { get; set; }
                }

                /// <summary>
                /// The auto translate menu.
                /// </summary>
                public class AutoTranslateMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the translate powered by google.
                    /// </summary>
                    public string TranslatePoweredByGoogle { get; set; }

                    /// <summary>
                    /// Gets or sets the translate powered by microsoft.
                    /// </summary>
                    public string TranslatePoweredByMicrosoft { get; set; }

                    /// <summary>
                    /// Gets or sets the translate from swedish to danish.
                    /// </summary>
                    public string TranslateFromSwedishToDanish { get; set; }
                }

                /// <summary>
                /// The options menu.
                /// </summary>
                public class OptionsMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the settings.
                    /// </summary>
                    public string Settings { get; set; }

                    /// <summary>
                    /// Gets or sets the choose language.
                    /// </summary>
                    public string ChooseLanguage { get; set; }
                }

                /// <summary>
                /// The networking menu.
                /// </summary>
                public class NetworkingMenu
                {
                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the start new session.
                    /// </summary>
                    public string StartNewSession { get; set; }

                    /// <summary>
                    /// Gets or sets the join session.
                    /// </summary>
                    public string JoinSession { get; set; }

                    /// <summary>
                    /// Gets or sets the show session info and log.
                    /// </summary>
                    public string ShowSessionInfoAndLog { get; set; }

                    /// <summary>
                    /// Gets or sets the chat.
                    /// </summary>
                    public string Chat { get; set; }

                    /// <summary>
                    /// Gets or sets the leave session.
                    /// </summary>
                    public string LeaveSession { get; set; }
                }

                /// <summary>
                /// The help menu.
                /// </summary>
                public class HelpMenu
                {
                    /// <summary>
                    /// Gets or sets the check for updates.
                    /// </summary>
                    public string CheckForUpdates { get; set; }

                    /// <summary>
                    /// Gets or sets the title.
                    /// </summary>
                    public string Title { get; set; }

                    /// <summary>
                    /// Gets or sets the help.
                    /// </summary>
                    public string Help { get; set; }

                    /// <summary>
                    /// Gets or sets the about.
                    /// </summary>
                    public string About { get; set; }
                }

                /// <summary>
                /// The tool bar menu.
                /// </summary>
                public class ToolBarMenu
                {
                    /// <summary>
                    /// Gets or sets the new.
                    /// </summary>
                    public string New { get; set; }

                    /// <summary>
                    /// Gets or sets the open.
                    /// </summary>
                    public string Open { get; set; }

                    /// <summary>
                    /// Gets or sets the save.
                    /// </summary>
                    public string Save { get; set; }

                    /// <summary>
                    /// Gets or sets the save as.
                    /// </summary>
                    public string SaveAs { get; set; }

                    /// <summary>
                    /// Gets or sets the find.
                    /// </summary>
                    public string Find { get; set; }

                    /// <summary>
                    /// Gets or sets the replace.
                    /// </summary>
                    public string Replace { get; set; }

                    /// <summary>
                    /// Gets or sets the fix common errors.
                    /// </summary>
                    public string FixCommonErrors { get; set; }

                    /// <summary>
                    /// Gets or sets the visual sync.
                    /// </summary>
                    public string VisualSync { get; set; }

                    /// <summary>
                    /// Gets or sets the spell check.
                    /// </summary>
                    public string SpellCheck { get; set; }

                    /// <summary>
                    /// Gets or sets the settings.
                    /// </summary>
                    public string Settings { get; set; }

                    /// <summary>
                    /// Gets or sets the help.
                    /// </summary>
                    public string Help { get; set; }

                    /// <summary>
                    /// Gets or sets the show hide waveform.
                    /// </summary>
                    public string ShowHideWaveform { get; set; }

                    /// <summary>
                    /// Gets or sets the show hide video.
                    /// </summary>
                    public string ShowHideVideo { get; set; }
                }

                /// <summary>
                /// The list view context menu.
                /// </summary>
                public class ListViewContextMenu
                {
                    /// <summary>
                    /// Gets or sets the advanced sub station alpha set style.
                    /// </summary>
                    public string AdvancedSubStationAlphaSetStyle { get; set; }

                    /// <summary>
                    /// Gets or sets the sub station alpha set style.
                    /// </summary>
                    public string SubStationAlphaSetStyle { get; set; }

                    /// <summary>
                    /// Gets or sets the sub station alpha styles.
                    /// </summary>
                    public string SubStationAlphaStyles { get; set; }

                    /// <summary>
                    /// Gets or sets the advanced sub station alpha styles.
                    /// </summary>
                    public string AdvancedSubStationAlphaStyles { get; set; }

                    /// <summary>
                    /// Gets or sets the timed text set style.
                    /// </summary>
                    public string TimedTextSetStyle { get; set; }

                    /// <summary>
                    /// Gets or sets the timed text styles.
                    /// </summary>
                    public string TimedTextStyles { get; set; }

                    /// <summary>
                    /// Gets or sets the timed text set language.
                    /// </summary>
                    public string TimedTextSetLanguage { get; set; }

                    /// <summary>
                    /// Gets or sets the sami set style.
                    /// </summary>
                    public string SamiSetStyle { get; set; }

                    /// <summary>
                    /// Gets or sets the nuendo set style.
                    /// </summary>
                    public string NuendoSetStyle { get; set; }

                    /// <summary>
                    /// Gets or sets the cut.
                    /// </summary>
                    public string Cut { get; set; }

                    /// <summary>
                    /// Gets or sets the copy.
                    /// </summary>
                    public string Copy { get; set; }

                    /// <summary>
                    /// Gets or sets the paste.
                    /// </summary>
                    public string Paste { get; set; }

                    /// <summary>
                    /// Gets or sets the delete.
                    /// </summary>
                    public string Delete { get; set; }

                    /// <summary>
                    /// Gets or sets the split line at cursor position.
                    /// </summary>
                    public string SplitLineAtCursorPosition { get; set; }

                    /// <summary>
                    /// Gets or sets the auto duration current line.
                    /// </summary>
                    public string AutoDurationCurrentLine { get; set; }

                    /// <summary>
                    /// Gets or sets the select all.
                    /// </summary>
                    public string SelectAll { get; set; }

                    /// <summary>
                    /// Gets or sets the insert first line.
                    /// </summary>
                    public string InsertFirstLine { get; set; }

                    /// <summary>
                    /// Gets or sets the insert before.
                    /// </summary>
                    public string InsertBefore { get; set; }

                    /// <summary>
                    /// Gets or sets the insert after.
                    /// </summary>
                    public string InsertAfter { get; set; }

                    /// <summary>
                    /// Gets or sets the insert subtitle after.
                    /// </summary>
                    public string InsertSubtitleAfter { get; set; }

                    /// <summary>
                    /// Gets or sets the copy to clipboard.
                    /// </summary>
                    public string CopyToClipboard { get; set; }

                    /// <summary>
                    /// Gets or sets the column.
                    /// </summary>
                    public string Column { get; set; }

                    /// <summary>
                    /// Gets or sets the column delete text.
                    /// </summary>
                    public string ColumnDeleteText { get; set; }

                    /// <summary>
                    /// Gets or sets the column delete text and shift cells up.
                    /// </summary>
                    public string ColumnDeleteTextAndShiftCellsUp { get; set; }

                    /// <summary>
                    /// Gets or sets the column insert empty text and shift cells down.
                    /// </summary>
                    public string ColumnInsertEmptyTextAndShiftCellsDown { get; set; }

                    /// <summary>
                    /// Gets or sets the column insert text from subtitle.
                    /// </summary>
                    public string ColumnInsertTextFromSubtitle { get; set; }

                    /// <summary>
                    /// Gets or sets the column import text and shift cells down.
                    /// </summary>
                    public string ColumnImportTextAndShiftCellsDown { get; set; }

                    /// <summary>
                    /// Gets or sets the column paste from clipboard.
                    /// </summary>
                    public string ColumnPasteFromClipboard { get; set; }

                    /// <summary>
                    /// Gets or sets the column copy original text to current.
                    /// </summary>
                    public string ColumnCopyOriginalTextToCurrent { get; set; }

                    /// <summary>
                    /// Gets or sets the split.
                    /// </summary>
                    public string Split { get; set; }

                    /// <summary>
                    /// Gets or sets the merge selected lines.
                    /// </summary>
                    public string MergeSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the merge selected lines as dialog.
                    /// </summary>
                    public string MergeSelectedLinesAsDialog { get; set; }

                    /// <summary>
                    /// Gets or sets the merge with line before.
                    /// </summary>
                    public string MergeWithLineBefore { get; set; }

                    /// <summary>
                    /// Gets or sets the merge with line after.
                    /// </summary>
                    public string MergeWithLineAfter { get; set; }

                    /// <summary>
                    /// Gets or sets the normal.
                    /// </summary>
                    public string Normal { get; set; }

                    /// <summary>
                    /// Gets or sets the underline.
                    /// </summary>
                    public string Underline { get; set; }

                    /// <summary>
                    /// Gets or sets the color.
                    /// </summary>
                    public string Color { get; set; }

                    /// <summary>
                    /// Gets or sets the font name.
                    /// </summary>
                    public string FontName { get; set; }

                    /// <summary>
                    /// Gets or sets the alignment.
                    /// </summary>
                    public string Alignment { get; set; }

                    /// <summary>
                    /// Gets or sets the auto balance selected lines.
                    /// </summary>
                    public string AutoBalanceSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the remove line breaks from selected lines.
                    /// </summary>
                    public string RemoveLineBreaksFromSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the typewriter effect.
                    /// </summary>
                    public string TypewriterEffect { get; set; }

                    /// <summary>
                    /// Gets or sets the karaoke effect.
                    /// </summary>
                    public string KaraokeEffect { get; set; }

                    /// <summary>
                    /// Gets or sets the show selected lines earlier later.
                    /// </summary>
                    public string ShowSelectedLinesEarlierLater { get; set; }

                    /// <summary>
                    /// Gets or sets the visual sync selected lines.
                    /// </summary>
                    public string VisualSyncSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the google and microsoft translate selected line.
                    /// </summary>
                    public string GoogleAndMicrosoftTranslateSelectedLine { get; set; }

                    /// <summary>
                    /// Gets or sets the google translate selected lines.
                    /// </summary>
                    public string GoogleTranslateSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the adjust display duration for selected lines.
                    /// </summary>
                    public string AdjustDisplayDurationForSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the fix common errors in selected lines.
                    /// </summary>
                    public string FixCommonErrorsInSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the change casing for selected lines.
                    /// </summary>
                    public string ChangeCasingForSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the save selected lines.
                    /// </summary>
                    public string SaveSelectedLines { get; set; }

                    /// <summary>
                    /// Gets or sets the web vtt set new voice.
                    /// </summary>
                    public string WebVTTSetNewVoice { get; set; }

                    /// <summary>
                    /// Gets or sets the web vtt remove voices.
                    /// </summary>
                    public string WebVTTRemoveVoices { get; set; }
                }
            }

            /// <summary>
            /// The main controls.
            /// </summary>
            public class MainControls
            {
                /// <summary>
                /// Gets or sets the subtitle format.
                /// </summary>
                public string SubtitleFormat { get; set; }

                /// <summary>
                /// Gets or sets the file encoding.
                /// </summary>
                public string FileEncoding { get; set; }

                /// <summary>
                /// Gets or sets the list view.
                /// </summary>
                public string ListView { get; set; }

                /// <summary>
                /// Gets or sets the source view.
                /// </summary>
                public string SourceView { get; set; }

                /// <summary>
                /// Gets or sets the undo changes in edit panel.
                /// </summary>
                public string UndoChangesInEditPanel { get; set; }

                /// <summary>
                /// Gets or sets the previous.
                /// </summary>
                public string Previous { get; set; }

                /// <summary>
                /// Gets or sets the next.
                /// </summary>
                public string Next { get; set; }

                /// <summary>
                /// Gets or sets the auto break.
                /// </summary>
                public string AutoBreak { get; set; }

                /// <summary>
                /// Gets or sets the unbreak.
                /// </summary>
                public string Unbreak { get; set; }
            }

            /// <summary>
            /// The main video controls.
            /// </summary>
            public class MainVideoControls
            {
                /// <summary>
                /// Gets or sets the translate.
                /// </summary>
                public string Translate { get; set; }

                /// <summary>
                /// Gets or sets the create.
                /// </summary>
                public string Create { get; set; }

                /// <summary>
                /// Gets or sets the adjust.
                /// </summary>
                public string Adjust { get; set; }

                /// <summary>
                /// Gets or sets the select current element while playing.
                /// </summary>
                public string SelectCurrentElementWhilePlaying { get; set; }

                // translation helper
                /// <summary>
                /// Gets or sets the auto repeat.
                /// </summary>
                public string AutoRepeat { get; set; }

                /// <summary>
                /// Gets or sets the auto repeat on.
                /// </summary>
                public string AutoRepeatOn { get; set; }

                /// <summary>
                /// Gets or sets the auto repeat count.
                /// </summary>
                public string AutoRepeatCount { get; set; }

                /// <summary>
                /// Gets or sets the auto continue.
                /// </summary>
                public string AutoContinue { get; set; }

                /// <summary>
                /// Gets or sets the auto continue on.
                /// </summary>
                public string AutoContinueOn { get; set; }

                /// <summary>
                /// Gets or sets the delay in seconds.
                /// </summary>
                public string DelayInSeconds { get; set; }

                /// <summary>
                /// Gets or sets the original text.
                /// </summary>
                public string OriginalText { get; set; }

                /// <summary>
                /// Gets or sets the previous.
                /// </summary>
                public string Previous { get; set; }

                /// <summary>
                /// Gets or sets the stop.
                /// </summary>
                public string Stop { get; set; }

                /// <summary>
                /// Gets or sets the play current.
                /// </summary>
                public string PlayCurrent { get; set; }

                /// <summary>
                /// Gets or sets the next.
                /// </summary>
                public string Next { get; set; }

                /// <summary>
                /// Gets or sets the playing.
                /// </summary>
                public string Playing { get; set; }

                /// <summary>
                /// Gets or sets the repeating last time.
                /// </summary>
                public string RepeatingLastTime { get; set; }

                /// <summary>
                /// Gets or sets the repeating x times left.
                /// </summary>
                public string RepeatingXTimesLeft { get; set; }

                /// <summary>
                /// Gets or sets the auto continue in one second.
                /// </summary>
                public string AutoContinueInOneSecond { get; set; }

                /// <summary>
                /// Gets or sets the auto continue in x seconds.
                /// </summary>
                public string AutoContinueInXSeconds { get; set; }

                /// <summary>
                /// Gets or sets the still typing auto continue stopped.
                /// </summary>
                public string StillTypingAutoContinueStopped { get; set; }

                // create/adjust
                /// <summary>
                /// Gets or sets the insert new subtitle at video position.
                /// </summary>
                public string InsertNewSubtitleAtVideoPosition { get; set; }

                /// <summary>
                /// Gets or sets the auto.
                /// </summary>
                public string Auto { get; set; }

                /// <summary>
                /// Gets or sets the play from just before text.
                /// </summary>
                public string PlayFromJustBeforeText { get; set; }

                /// <summary>
                /// Gets or sets the pause.
                /// </summary>
                public string Pause { get; set; }

                /// <summary>
                /// Gets or sets the go to subtitle position and pause.
                /// </summary>
                public string GoToSubtitlePositionAndPause { get; set; }

                /// <summary>
                /// Gets or sets the set start time.
                /// </summary>
                public string SetStartTime { get; set; }

                /// <summary>
                /// Gets or sets the set end time and go to next.
                /// </summary>
                public string SetEndTimeAndGoToNext { get; set; }

                /// <summary>
                /// Gets or sets the adjusted via end time.
                /// </summary>
                public string AdjustedViaEndTime { get; set; }

                /// <summary>
                /// Gets or sets the set end time.
                /// </summary>
                public string SetEndTime { get; set; }

                /// <summary>
                /// Gets or sets the setstart time and offset of rest.
                /// </summary>
                public string SetstartTimeAndOffsetOfRest { get; set; }

                /// <summary>
                /// Gets or sets the search text online.
                /// </summary>
                public string SearchTextOnline { get; set; }

                /// <summary>
                /// Gets or sets the google translate.
                /// </summary>
                public string GoogleTranslate { get; set; }

                /// <summary>
                /// Gets or sets the google it.
                /// </summary>
                public string GoogleIt { get; set; }

                /// <summary>
                /// Gets or sets the seconds back short.
                /// </summary>
                public string SecondsBackShort { get; set; }

                /// <summary>
                /// Gets or sets the seconds forward short.
                /// </summary>
                public string SecondsForwardShort { get; set; }

                /// <summary>
                /// Gets or sets the video position.
                /// </summary>
                public string VideoPosition { get; set; }

                /// <summary>
                /// Gets or sets the translate tip.
                /// </summary>
                public string TranslateTip { get; set; }

                /// <summary>
                /// Gets or sets the create tip.
                /// </summary>
                public string CreateTip { get; set; }

                /// <summary>
                /// Gets or sets the adjust tip.
                /// </summary>
                public string AdjustTip { get; set; }

                /// <summary>
                /// Gets or sets the before changing time in waveform x.
                /// </summary>
                public string BeforeChangingTimeInWaveformX { get; set; }

                /// <summary>
                /// Gets or sets the new text insert at x.
                /// </summary>
                public string NewTextInsertAtX { get; set; }

                /// <summary>
                /// Gets or sets the center.
                /// </summary>
                public string Center { get; set; }

                /// <summary>
                /// Gets or sets the play rate.
                /// </summary>
                public string PlayRate { get; set; }

                /// <summary>
                /// Gets or sets the slow.
                /// </summary>
                public string Slow { get; set; }

                /// <summary>
                /// Gets or sets the normal.
                /// </summary>
                public string Normal { get; set; }

                /// <summary>
                /// Gets or sets the fast.
                /// </summary>
                public string Fast { get; set; }

                /// <summary>
                /// Gets or sets the very fast.
                /// </summary>
                public string VeryFast { get; set; }
            }
        }

        /// <summary>
        /// The matroska subtitle chooser.
        /// </summary>
        public class MatroskaSubtitleChooser
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the please choose.
            /// </summary>
            public string PleaseChoose { get; set; }

            /// <summary>
            /// Gets or sets the track x language y type z.
            /// </summary>
            public string TrackXLanguageYTypeZ { get; set; }
        }

        /// <summary>
        /// The measurement converter.
        /// </summary>
        public class MeasurementConverter
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the convert from.
            /// </summary>
            public string ConvertFrom { get; set; }

            /// <summary>
            /// Gets or sets the convert to.
            /// </summary>
            public string ConvertTo { get; set; }

            /// <summary>
            /// Gets or sets the copy to clipboard.
            /// </summary>
            public string CopyToClipboard { get; set; }

            /// <summary>
            /// Gets or sets the celsius.
            /// </summary>
            public string Celsius { get; set; }

            /// <summary>
            /// Gets or sets the fahrenheit.
            /// </summary>
            public string Fahrenheit { get; set; }

            /// <summary>
            /// Gets or sets the miles.
            /// </summary>
            public string Miles { get; set; }

            /// <summary>
            /// Gets or sets the kilometers.
            /// </summary>
            public string Kilometers { get; set; }

            /// <summary>
            /// Gets or sets the meters.
            /// </summary>
            public string Meters { get; set; }

            /// <summary>
            /// Gets or sets the yards.
            /// </summary>
            public string Yards { get; set; }

            /// <summary>
            /// Gets or sets the feet.
            /// </summary>
            public string Feet { get; set; }

            /// <summary>
            /// Gets or sets the inches.
            /// </summary>
            public string Inches { get; set; }

            /// <summary>
            /// Gets or sets the pounds.
            /// </summary>
            public string Pounds { get; set; }

            /// <summary>
            /// Gets or sets the kilos.
            /// </summary>
            public string Kilos { get; set; }
        }

        /// <summary>
        /// The merge double lines.
        /// </summary>
        public class MergeDoubleLines
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the max milliseconds between lines.
            /// </summary>
            public string MaxMillisecondsBetweenLines { get; set; }

            /// <summary>
            /// Gets or sets the include incrementing.
            /// </summary>
            public string IncludeIncrementing { get; set; }
        }

        /// <summary>
        /// The merge short lines.
        /// </summary>
        public class MergeShortLines
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the maximum characters.
            /// </summary>
            public string MaximumCharacters { get; set; }

            /// <summary>
            /// Gets or sets the maximum milliseconds between.
            /// </summary>
            public string MaximumMillisecondsBetween { get; set; }

            /// <summary>
            /// Gets or sets the number of merges x.
            /// </summary>
            public string NumberOfMergesX { get; set; }

            /// <summary>
            /// Gets or sets the merged text.
            /// </summary>
            public string MergedText { get; set; }

            /// <summary>
            /// Gets or sets the only merge continuation lines.
            /// </summary>
            public string OnlyMergeContinuationLines { get; set; }
        }

        /// <summary>
        /// The merge text with same time codes.
        /// </summary>
        public class MergeTextWithSameTimeCodes
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the max difference milliseconds.
            /// </summary>
            public string MaxDifferenceMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the re break lines.
            /// </summary>
            public string ReBreakLines { get; set; }

            /// <summary>
            /// Gets or sets the number of merges x.
            /// </summary>
            public string NumberOfMergesX { get; set; }

            /// <summary>
            /// Gets or sets the merged text.
            /// </summary>
            public string MergedText { get; set; }
        }

        /// <summary>
        /// The modify selection.
        /// </summary>
        public class ModifySelection
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the rule.
            /// </summary>
            public string Rule { get; set; }

            /// <summary>
            /// Gets or sets the case sensitive.
            /// </summary>
            public string CaseSensitive { get; set; }

            /// <summary>
            /// Gets or sets the do with matches.
            /// </summary>
            public string DoWithMatches { get; set; }

            /// <summary>
            /// Gets or sets the make new selection.
            /// </summary>
            public string MakeNewSelection { get; set; }

            /// <summary>
            /// Gets or sets the add to current selection.
            /// </summary>
            public string AddToCurrentSelection { get; set; }

            /// <summary>
            /// Gets or sets the subtract from current selection.
            /// </summary>
            public string SubtractFromCurrentSelection { get; set; }

            /// <summary>
            /// Gets or sets the intersect with current selection.
            /// </summary>
            public string IntersectWithCurrentSelection { get; set; }

            /// <summary>
            /// Gets or sets the matching lines x.
            /// </summary>
            public string MatchingLinesX { get; set; }

            /// <summary>
            /// Gets or sets the contains.
            /// </summary>
            public string Contains { get; set; }

            /// <summary>
            /// Gets or sets the starts with.
            /// </summary>
            public string StartsWith { get; set; }

            /// <summary>
            /// Gets or sets the ends with.
            /// </summary>
            public string EndsWith { get; set; }

            /// <summary>
            /// Gets or sets the no contains.
            /// </summary>
            public string NoContains { get; set; }

            /// <summary>
            /// Gets or sets the reg ex.
            /// </summary>
            public string RegEx { get; set; }

            /// <summary>
            /// Gets or sets the unequal lines.
            /// </summary>
            public string UnequalLines { get; set; }

            /// <summary>
            /// Gets or sets the equal lines.
            /// </summary>
            public string EqualLines { get; set; }
        }

        /// <summary>
        /// The multiple replace.
        /// </summary>
        public class MultipleReplace
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the find what.
            /// </summary>
            public string FindWhat { get; set; }

            /// <summary>
            /// Gets or sets the replace with.
            /// </summary>
            public string ReplaceWith { get; set; }

            /// <summary>
            /// Gets or sets the normal.
            /// </summary>
            public string Normal { get; set; }

            /// <summary>
            /// Gets or sets the case sensitive.
            /// </summary>
            public string CaseSensitive { get; set; }

            /// <summary>
            /// Gets or sets the regular expression.
            /// </summary>
            public string RegularExpression { get; set; }

            /// <summary>
            /// Gets or sets the lines found x.
            /// </summary>
            public string LinesFoundX { get; set; }

            /// <summary>
            /// Gets or sets the delete.
            /// </summary>
            public string Delete { get; set; }

            /// <summary>
            /// Gets or sets the add.
            /// </summary>
            public string Add { get; set; }

            /// <summary>
            /// Gets or sets the update.
            /// </summary>
            public string Update { get; set; }

            /// <summary>
            /// Gets or sets the enabled.
            /// </summary>
            public string Enabled { get; set; }

            /// <summary>
            /// Gets or sets the search type.
            /// </summary>
            public string SearchType { get; set; }

            /// <summary>
            /// Gets or sets the remove all.
            /// </summary>
            public string RemoveAll { get; set; }

            /// <summary>
            /// Gets or sets the import.
            /// </summary>
            public string Import { get; set; }

            /// <summary>
            /// Gets or sets the export.
            /// </summary>
            public string Export { get; set; }

            /// <summary>
            /// Gets or sets the import rules title.
            /// </summary>
            public string ImportRulesTitle { get; set; }

            /// <summary>
            /// Gets or sets the export rules title.
            /// </summary>
            public string ExportRulesTitle { get; set; }

            /// <summary>
            /// Gets or sets the rules.
            /// </summary>
            public string Rules { get; set; }

            /// <summary>
            /// Gets or sets the move to top.
            /// </summary>
            public string MoveToTop { get; set; }

            /// <summary>
            /// Gets or sets the move to bottom.
            /// </summary>
            public string MoveToBottom { get; set; }
        }

        /// <summary>
        /// The network chat.
        /// </summary>
        public class NetworkChat
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the send.
            /// </summary>
            public string Send { get; set; }
        }

        /// <summary>
        /// The network join.
        /// </summary>
        public class NetworkJoin
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the information.
            /// </summary>
            public string Information { get; set; }

            /// <summary>
            /// Gets or sets the join.
            /// </summary>
            public string Join { get; set; }
        }

        /// <summary>
        /// The network log and info.
        /// </summary>
        public class NetworkLogAndInfo
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the log.
            /// </summary>
            public string Log { get; set; }
        }

        /// <summary>
        /// The network start.
        /// </summary>
        public class NetworkStart
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the connection to.
            /// </summary>
            public string ConnectionTo { get; set; }

            /// <summary>
            /// Gets or sets the information.
            /// </summary>
            public string Information { get; set; }

            /// <summary>
            /// Gets or sets the start.
            /// </summary>
            public string Start { get; set; }
        }

        /// <summary>
        /// The open video dvd.
        /// </summary>
        public class OpenVideoDvd
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the open dvd from.
            /// </summary>
            public string OpenDvdFrom { get; set; }

            /// <summary>
            /// Gets or sets the disc.
            /// </summary>
            public string Disc { get; set; }

            /// <summary>
            /// Gets or sets the folder.
            /// </summary>
            public string Folder { get; set; }

            /// <summary>
            /// Gets or sets the choose drive.
            /// </summary>
            public string ChooseDrive { get; set; }

            /// <summary>
            /// Gets or sets the choose folder.
            /// </summary>
            public string ChooseFolder { get; set; }
        }

        /// <summary>
        /// The plugins get.
        /// </summary>
        public class PluginsGet
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the installed plugins.
            /// </summary>
            public string InstalledPlugins { get; set; }

            /// <summary>
            /// Gets or sets the get plugins.
            /// </summary>
            public string GetPlugins { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// Gets or sets the date.
            /// </summary>
            public string Date { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the open plugins folder.
            /// </summary>
            public string OpenPluginsFolder { get; set; }

            /// <summary>
            /// Gets or sets the get plugins info 1.
            /// </summary>
            public string GetPluginsInfo1 { get; set; }

            /// <summary>
            /// Gets or sets the get plugins info 2.
            /// </summary>
            public string GetPluginsInfo2 { get; set; }

            /// <summary>
            /// Gets or sets the plugin x downloaded.
            /// </summary>
            public string PluginXDownloaded { get; set; }

            /// <summary>
            /// Gets or sets the download.
            /// </summary>
            public string Download { get; set; }

            /// <summary>
            /// Gets or sets the remove.
            /// </summary>
            public string Remove { get; set; }

            /// <summary>
            /// Gets or sets the update all x.
            /// </summary>
            public string UpdateAllX { get; set; }

            /// <summary>
            /// Gets or sets the unable to download plugin list x.
            /// </summary>
            public string UnableToDownloadPluginListX { get; set; }

            /// <summary>
            /// Gets or sets the new version of subtitle edit required.
            /// </summary>
            public string NewVersionOfSubtitleEditRequired { get; set; }

            /// <summary>
            /// Gets or sets the update available.
            /// </summary>
            public string UpdateAvailable { get; set; }

            /// <summary>
            /// Gets or sets the update all.
            /// </summary>
            public string UpdateAll { get; set; }

            /// <summary>
            /// Gets or sets the x plugins updated.
            /// </summary>
            public string XPluginsUpdated { get; set; }
        }

        /// <summary>
        /// The regular expression context menu.
        /// </summary>
        public class RegularExpressionContextMenu
        {
            /// <summary>
            /// Gets or sets the word boundary.
            /// </summary>
            public string WordBoundary { get; set; }

            /// <summary>
            /// Gets or sets the non word boundary.
            /// </summary>
            public string NonWordBoundary { get; set; }

            /// <summary>
            /// Gets or sets the new line.
            /// </summary>
            public string NewLine { get; set; }

            /// <summary>
            /// Gets or sets the new line short.
            /// </summary>
            public string NewLineShort { get; set; }

            /// <summary>
            /// Gets or sets the any digit.
            /// </summary>
            public string AnyDigit { get; set; }

            /// <summary>
            /// Gets or sets the non digit.
            /// </summary>
            public string NonDigit { get; set; }

            /// <summary>
            /// Gets or sets the any character.
            /// </summary>
            public string AnyCharacter { get; set; }

            /// <summary>
            /// Gets or sets the any whitespace.
            /// </summary>
            public string AnyWhitespace { get; set; }

            /// <summary>
            /// Gets or sets the non space character.
            /// </summary>
            public string NonSpaceCharacter { get; set; }

            /// <summary>
            /// Gets or sets the zero or more.
            /// </summary>
            public string ZeroOrMore { get; set; }

            /// <summary>
            /// Gets or sets the one or more.
            /// </summary>
            public string OneOrMore { get; set; }

            /// <summary>
            /// Gets or sets the in character group.
            /// </summary>
            public string InCharacterGroup { get; set; }

            /// <summary>
            /// Gets or sets the not in character group.
            /// </summary>
            public string NotInCharacterGroup { get; set; }
        }

        /// <summary>
        /// The remove text from hear impaired.
        /// </summary>
        public class RemoveTextFromHearImpaired
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the remove text conditions.
            /// </summary>
            public string RemoveTextConditions { get; set; }

            /// <summary>
            /// Gets or sets the remove text between.
            /// </summary>
            public string RemoveTextBetween { get; set; }

            /// <summary>
            /// Gets or sets the square brackets.
            /// </summary>
            public string SquareBrackets { get; set; }

            /// <summary>
            /// Gets or sets the brackets.
            /// </summary>
            public string Brackets { get; set; }

            /// <summary>
            /// Gets or sets the parentheses.
            /// </summary>
            public string Parentheses { get; set; }

            /// <summary>
            /// Gets or sets the question marks.
            /// </summary>
            public string QuestionMarks { get; set; }

            /// <summary>
            /// Gets or sets the and.
            /// </summary>
            public string And { get; set; }

            /// <summary>
            /// Gets or sets the remove text before colon.
            /// </summary>
            public string RemoveTextBeforeColon { get; set; }

            /// <summary>
            /// Gets or sets the only if text is uppercase.
            /// </summary>
            public string OnlyIfTextIsUppercase { get; set; }

            /// <summary>
            /// Gets or sets the only if in separate line.
            /// </summary>
            public string OnlyIfInSeparateLine { get; set; }

            /// <summary>
            /// Gets or sets the lines found x.
            /// </summary>
            public string LinesFoundX { get; set; }

            /// <summary>
            /// Gets or sets the remove text if contains.
            /// </summary>
            public string RemoveTextIfContains { get; set; }

            /// <summary>
            /// Gets or sets the remove text if all uppercase.
            /// </summary>
            public string RemoveTextIfAllUppercase { get; set; }

            /// <summary>
            /// Gets or sets the remove interjections.
            /// </summary>
            public string RemoveInterjections { get; set; }

            /// <summary>
            /// Gets or sets the edit interjections.
            /// </summary>
            public string EditInterjections { get; set; }
        }

        /// <summary>
        /// The replace dialog.
        /// </summary>
        public class ReplaceDialog
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the find what.
            /// </summary>
            public string FindWhat { get; set; }

            /// <summary>
            /// Gets or sets the normal.
            /// </summary>
            public string Normal { get; set; }

            /// <summary>
            /// Gets or sets the case sensitive.
            /// </summary>
            public string CaseSensitive { get; set; }

            /// <summary>
            /// Gets or sets the regular expression.
            /// </summary>
            public string RegularExpression { get; set; }

            /// <summary>
            /// Gets or sets the replace with.
            /// </summary>
            public string ReplaceWith { get; set; }

            /// <summary>
            /// Gets or sets the find.
            /// </summary>
            public string Find { get; set; }

            /// <summary>
            /// Gets or sets the replace.
            /// </summary>
            public string Replace { get; set; }

            /// <summary>
            /// Gets or sets the replace all.
            /// </summary>
            public string ReplaceAll { get; set; }
        }

        /// <summary>
        /// The restore auto backup.
        /// </summary>
        public class RestoreAutoBackup
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the information.
            /// </summary>
            public string Information { get; set; }

            /// <summary>
            /// Gets or sets the date and time.
            /// </summary>
            public string DateAndTime { get; set; }

            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the extension.
            /// </summary>
            public string Extension { get; set; }

            /// <summary>
            /// Gets or sets the no backed up files found.
            /// </summary>
            public string NoBackedUpFilesFound { get; set; }
        }

        /// <summary>
        /// The seek silence.
        /// </summary>
        public class SeekSilence
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the search direction.
            /// </summary>
            public string SearchDirection { get; set; }

            /// <summary>
            /// Gets or sets the forward.
            /// </summary>
            public string Forward { get; set; }

            /// <summary>
            /// Gets or sets the back.
            /// </summary>
            public string Back { get; set; }

            /// <summary>
            /// Gets or sets the length in seconds.
            /// </summary>
            public string LengthInSeconds { get; set; }

            /// <summary>
            /// Gets or sets the max volume.
            /// </summary>
            public string MaxVolume { get; set; }
        }

        /// <summary>
        /// The set minimum display time between paragraphs.
        /// </summary>
        public class SetMinimumDisplayTimeBetweenParagraphs
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the preview lines modified x.
            /// </summary>
            public string PreviewLinesModifiedX { get; set; }

            /// <summary>
            /// Gets or sets the show only modified lines.
            /// </summary>
            public string ShowOnlyModifiedLines { get; set; }

            /// <summary>
            /// Gets or sets the minimum milliseconds between paragraphs.
            /// </summary>
            public string MinimumMillisecondsBetweenParagraphs { get; set; }

            /// <summary>
            /// Gets or sets the frame info.
            /// </summary>
            public string FrameInfo { get; set; }

            /// <summary>
            /// Gets or sets the one frame xis y milliseconds.
            /// </summary>
            public string OneFrameXisYMilliseconds { get; set; }
        }

        /// <summary>
        /// The set sync point.
        /// </summary>
        public class SetSyncPoint
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the sync point time code.
            /// </summary>
            public string SyncPointTimeCode { get; set; }

            /// <summary>
            /// Gets or sets the three seconds back.
            /// </summary>
            public string ThreeSecondsBack { get; set; }

            /// <summary>
            /// Gets or sets the half a second back.
            /// </summary>
            public string HalfASecondBack { get; set; }

            /// <summary>
            /// Gets or sets the half a second forward.
            /// </summary>
            public string HalfASecondForward { get; set; }

            /// <summary>
            /// Gets or sets the three seconds forward.
            /// </summary>
            public string ThreeSecondsForward { get; set; }
        }

        /// <summary>
        /// The settings.
        /// </summary>
        public class Settings
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the general.
            /// </summary>
            public string General { get; set; }

            /// <summary>
            /// Gets or sets the toolbar.
            /// </summary>
            public string Toolbar { get; set; }

            /// <summary>
            /// Gets or sets the video player.
            /// </summary>
            public string VideoPlayer { get; set; }

            /// <summary>
            /// Gets or sets the waveform and spectrogram.
            /// </summary>
            public string WaveformAndSpectrogram { get; set; }

            /// <summary>
            /// Gets or sets the tools.
            /// </summary>
            public string Tools { get; set; }

            /// <summary>
            /// Gets or sets the word lists.
            /// </summary>
            public string WordLists { get; set; }

            /// <summary>
            /// Gets or sets the ssa style.
            /// </summary>
            public string SsaStyle { get; set; }

            /// <summary>
            /// Gets or sets the proxy.
            /// </summary>
            public string Proxy { get; set; }

            /// <summary>
            /// Gets or sets the show tool bar buttons.
            /// </summary>
            public string ShowToolBarButtons { get; set; }

            /// <summary>
            /// Gets or sets the new.
            /// </summary>
            public string New { get; set; }

            /// <summary>
            /// Gets or sets the open.
            /// </summary>
            public string Open { get; set; }

            /// <summary>
            /// Gets or sets the save.
            /// </summary>
            public string Save { get; set; }

            /// <summary>
            /// Gets or sets the save as.
            /// </summary>
            public string SaveAs { get; set; }

            /// <summary>
            /// Gets or sets the find.
            /// </summary>
            public string Find { get; set; }

            /// <summary>
            /// Gets or sets the replace.
            /// </summary>
            public string Replace { get; set; }

            /// <summary>
            /// Gets or sets the visual sync.
            /// </summary>
            public string VisualSync { get; set; }

            /// <summary>
            /// Gets or sets the spell check.
            /// </summary>
            public string SpellCheck { get; set; }

            /// <summary>
            /// Gets or sets the settings name.
            /// </summary>
            public string SettingsName { get; set; }

            /// <summary>
            /// Gets or sets the help.
            /// </summary>
            public string Help { get; set; }

            /// <summary>
            /// Gets or sets the show frame rate.
            /// </summary>
            public string ShowFrameRate { get; set; }

            /// <summary>
            /// Gets or sets the default frame rate.
            /// </summary>
            public string DefaultFrameRate { get; set; }

            /// <summary>
            /// Gets or sets the default file encoding.
            /// </summary>
            public string DefaultFileEncoding { get; set; }

            /// <summary>
            /// Gets or sets the auto detect ansi encoding.
            /// </summary>
            public string AutoDetectAnsiEncoding { get; set; }

            /// <summary>
            /// Gets or sets the subtitle line maximum length.
            /// </summary>
            public string SubtitleLineMaximumLength { get; set; }

            /// <summary>
            /// Gets or sets the maximum characters per second.
            /// </summary>
            public string MaximumCharactersPerSecond { get; set; }

            /// <summary>
            /// Gets or sets the auto wrap while typing.
            /// </summary>
            public string AutoWrapWhileTyping { get; set; }

            /// <summary>
            /// Gets or sets the duration minimum milliseconds.
            /// </summary>
            public string DurationMinimumMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the duration maximum milliseconds.
            /// </summary>
            public string DurationMaximumMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the minimum gap milliseconds.
            /// </summary>
            public string MinimumGapMilliseconds { get; set; }

            /// <summary>
            /// Gets or sets the subtitle font.
            /// </summary>
            public string SubtitleFont { get; set; }

            /// <summary>
            /// Gets or sets the subtitle font size.
            /// </summary>
            public string SubtitleFontSize { get; set; }

            /// <summary>
            /// Gets or sets the subtitle bold.
            /// </summary>
            public string SubtitleBold { get; set; }

            /// <summary>
            /// Gets or sets the subtitle center.
            /// </summary>
            public string SubtitleCenter { get; set; }

            /// <summary>
            /// Gets or sets the subtitle font color.
            /// </summary>
            public string SubtitleFontColor { get; set; }

            /// <summary>
            /// Gets or sets the subtitle background color.
            /// </summary>
            public string SubtitleBackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the spell checker.
            /// </summary>
            public string SpellChecker { get; set; }

            /// <summary>
            /// Gets or sets the remember recent files.
            /// </summary>
            public string RememberRecentFiles { get; set; }

            /// <summary>
            /// Gets or sets the start with last file loaded.
            /// </summary>
            public string StartWithLastFileLoaded { get; set; }

            /// <summary>
            /// Gets or sets the remember selected line.
            /// </summary>
            public string RememberSelectedLine { get; set; }

            /// <summary>
            /// Gets or sets the remember position and size.
            /// </summary>
            public string RememberPositionAndSize { get; set; }

            /// <summary>
            /// Gets or sets the start in source view.
            /// </summary>
            public string StartInSourceView { get; set; }

            /// <summary>
            /// Gets or sets the remove blank lines when opening.
            /// </summary>
            public string RemoveBlankLinesWhenOpening { get; set; }

            /// <summary>
            /// Gets or sets the show line breaks as.
            /// </summary>
            public string ShowLineBreaksAs { get; set; }

            /// <summary>
            /// Gets or sets the main list view double click action.
            /// </summary>
            public string MainListViewDoubleClickAction { get; set; }

            /// <summary>
            /// Gets or sets the main list view nothing.
            /// </summary>
            public string MainListViewNothing { get; set; }

            /// <summary>
            /// Gets or sets the main list view video go to position and pause.
            /// </summary>
            public string MainListViewVideoGoToPositionAndPause { get; set; }

            /// <summary>
            /// Gets or sets the main list view video go to position and play.
            /// </summary>
            public string MainListViewVideoGoToPositionAndPlay { get; set; }

            /// <summary>
            /// Gets or sets the main list view edit text.
            /// </summary>
            public string MainListViewEditText { get; set; }

            /// <summary>
            /// Gets or sets the main list view video go to position minus 1 sec and pause.
            /// </summary>
            public string MainListViewVideoGoToPositionMinus1SecAndPause { get; set; }

            /// <summary>
            /// Gets or sets the main list view video go to position minus half sec and pause.
            /// </summary>
            public string MainListViewVideoGoToPositionMinusHalfSecAndPause { get; set; }

            /// <summary>
            /// Gets or sets the main list view video go to position minus 1 sec and play.
            /// </summary>
            public string MainListViewVideoGoToPositionMinus1SecAndPlay { get; set; }

            /// <summary>
            /// Gets or sets the main list view edit text and pause.
            /// </summary>
            public string MainListViewEditTextAndPause { get; set; }

            /// <summary>
            /// Gets or sets the auto backup.
            /// </summary>
            public string AutoBackup { get; set; }

            /// <summary>
            /// Gets or sets the auto backup every minute.
            /// </summary>
            public string AutoBackupEveryMinute { get; set; }

            /// <summary>
            /// Gets or sets the auto backup every five minutes.
            /// </summary>
            public string AutoBackupEveryFiveMinutes { get; set; }

            /// <summary>
            /// Gets or sets the auto backup every fifteen minutes.
            /// </summary>
            public string AutoBackupEveryFifteenMinutes { get; set; }

            /// <summary>
            /// Gets or sets the check for updates.
            /// </summary>
            public string CheckForUpdates { get; set; }

            /// <summary>
            /// Gets or sets the allow edit of original subtitle.
            /// </summary>
            public string AllowEditOfOriginalSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the prompt delete lines.
            /// </summary>
            public string PromptDeleteLines { get; set; }

            /// <summary>
            /// Gets or sets the time code mode.
            /// </summary>
            public string TimeCodeMode { get; set; }

            /// <summary>
            /// Gets or sets the time code mode hhmmssms.
            /// </summary>
            public string TimeCodeModeHHMMSSMS { get; set; }

            /// <summary>
            /// Gets or sets the time code mode hhmmssff.
            /// </summary>
            public string TimeCodeModeHHMMSSFF { get; set; }

            /// <summary>
            /// Gets or sets the video engine.
            /// </summary>
            public string VideoEngine { get; set; }

            /// <summary>
            /// Gets or sets the direct show.
            /// </summary>
            public string DirectShow { get; set; }

            /// <summary>
            /// Gets or sets the direct show description.
            /// </summary>
            public string DirectShowDescription { get; set; }

            /// <summary>
            /// Gets or sets the managed direct x.
            /// </summary>
            public string ManagedDirectX { get; set; }

            /// <summary>
            /// Gets or sets the managed direct x description.
            /// </summary>
            public string ManagedDirectXDescription { get; set; }

            /// <summary>
            /// Gets or sets the mpc hc.
            /// </summary>
            public string MpcHc { get; set; }

            /// <summary>
            /// Gets or sets the mpc hc description.
            /// </summary>
            public string MpcHcDescription { get; set; }

            /// <summary>
            /// Gets or sets the m player.
            /// </summary>
            public string MPlayer { get; set; }

            /// <summary>
            /// Gets or sets the m player description.
            /// </summary>
            public string MPlayerDescription { get; set; }

            /// <summary>
            /// Gets or sets the vlc media player.
            /// </summary>
            public string VlcMediaPlayer { get; set; }

            /// <summary>
            /// Gets or sets the vlc media player description.
            /// </summary>
            public string VlcMediaPlayerDescription { get; set; }

            /// <summary>
            /// Gets or sets the vlc browse to label.
            /// </summary>
            public string VlcBrowseToLabel { get; set; }

            /// <summary>
            /// Gets or sets the show stop button.
            /// </summary>
            public string ShowStopButton { get; set; }

            /// <summary>
            /// Gets or sets the show mute button.
            /// </summary>
            public string ShowMuteButton { get; set; }

            /// <summary>
            /// Gets or sets the show fullscreen button.
            /// </summary>
            public string ShowFullscreenButton { get; set; }

            /// <summary>
            /// Gets or sets the preview font size.
            /// </summary>
            public string PreviewFontSize { get; set; }

            /// <summary>
            /// Gets or sets the main window video controls.
            /// </summary>
            public string MainWindowVideoControls { get; set; }

            /// <summary>
            /// Gets or sets the custom search text and url.
            /// </summary>
            public string CustomSearchTextAndUrl { get; set; }

            /// <summary>
            /// Gets or sets the waveform appearance.
            /// </summary>
            public string WaveformAppearance { get; set; }

            /// <summary>
            /// Gets or sets the waveform grid color.
            /// </summary>
            public string WaveformGridColor { get; set; }

            /// <summary>
            /// Gets or sets the waveform show grid lines.
            /// </summary>
            public string WaveformShowGridLines { get; set; }

            /// <summary>
            /// Gets or sets the reverse mouse wheel scroll direction.
            /// </summary>
            public string ReverseMouseWheelScrollDirection { get; set; }

            /// <summary>
            /// Gets or sets the waveform allow overlap.
            /// </summary>
            public string WaveformAllowOverlap { get; set; }

            /// <summary>
            /// Gets or sets the waveform focus mouse enter.
            /// </summary>
            public string WaveformFocusMouseEnter { get; set; }

            /// <summary>
            /// Gets or sets the waveform list view focus mouse enter.
            /// </summary>
            public string WaveformListViewFocusMouseEnter { get; set; }

            /// <summary>
            /// Gets or sets the waveform border hit ms 1.
            /// </summary>
            public string WaveformBorderHitMs1 { get; set; }

            /// <summary>
            /// Gets or sets the waveform border hit ms 2.
            /// </summary>
            public string WaveformBorderHitMs2 { get; set; }

            /// <summary>
            /// Gets or sets the waveform color.
            /// </summary>
            public string WaveformColor { get; set; }

            /// <summary>
            /// Gets or sets the waveform selected color.
            /// </summary>
            public string WaveformSelectedColor { get; set; }

            /// <summary>
            /// Gets or sets the waveform background color.
            /// </summary>
            public string WaveformBackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the waveform text color.
            /// </summary>
            public string WaveformTextColor { get; set; }

            /// <summary>
            /// Gets or sets the waveform text font size.
            /// </summary>
            public string WaveformTextFontSize { get; set; }

            /// <summary>
            /// Gets or sets the waveform and spectrograms folder empty.
            /// </summary>
            public string WaveformAndSpectrogramsFolderEmpty { get; set; }

            /// <summary>
            /// Gets or sets the waveform and spectrograms folder info.
            /// </summary>
            public string WaveformAndSpectrogramsFolderInfo { get; set; }

            /// <summary>
            /// Gets or sets the spectrogram.
            /// </summary>
            public string Spectrogram { get; set; }

            /// <summary>
            /// Gets or sets the generate spectrogram.
            /// </summary>
            public string GenerateSpectrogram { get; set; }

            /// <summary>
            /// Gets or sets the spectrogram appearance.
            /// </summary>
            public string SpectrogramAppearance { get; set; }

            /// <summary>
            /// Gets or sets the spectrogram one color gradient.
            /// </summary>
            public string SpectrogramOneColorGradient { get; set; }

            /// <summary>
            /// Gets or sets the spectrogram classic.
            /// </summary>
            public string SpectrogramClassic { get; set; }

            /// <summary>
            /// Gets or sets the waveform use f fmpeg.
            /// </summary>
            public string WaveformUseFFmpeg { get; set; }

            /// <summary>
            /// Gets or sets the waveform f fmpeg path.
            /// </summary>
            public string WaveformFFmpegPath { get; set; }

            /// <summary>
            /// Gets or sets the waveform browse to f fmpeg.
            /// </summary>
            public string WaveformBrowseToFFmpeg { get; set; }

            /// <summary>
            /// Gets or sets the waveform browse to vlc.
            /// </summary>
            public string WaveformBrowseToVLC { get; set; }

            /// <summary>
            /// Gets or sets the sub station alpha style.
            /// </summary>
            public string SubStationAlphaStyle { get; set; }

            /// <summary>
            /// Gets or sets the choose font.
            /// </summary>
            public string ChooseFont { get; set; }

            /// <summary>
            /// Gets or sets the choose color.
            /// </summary>
            public string ChooseColor { get; set; }

            /// <summary>
            /// Gets or sets the ssa outline.
            /// </summary>
            public string SsaOutline { get; set; }

            /// <summary>
            /// Gets or sets the ssa shadow.
            /// </summary>
            public string SsaShadow { get; set; }

            /// <summary>
            /// Gets or sets the ssa opaque box.
            /// </summary>
            public string SsaOpaqueBox { get; set; }

            /// <summary>
            /// Gets or sets the testing 123.
            /// </summary>
            public string Testing123 { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the names ignore lists.
            /// </summary>
            public string NamesIgnoreLists { get; set; }

            /// <summary>
            /// Gets or sets the add name etc.
            /// </summary>
            public string AddNameEtc { get; set; }

            /// <summary>
            /// Gets or sets the add word.
            /// </summary>
            public string AddWord { get; set; }

            /// <summary>
            /// Gets or sets the remove.
            /// </summary>
            public string Remove { get; set; }

            /// <summary>
            /// Gets or sets the add pair.
            /// </summary>
            public string AddPair { get; set; }

            /// <summary>
            /// Gets or sets the user word list.
            /// </summary>
            public string UserWordList { get; set; }

            /// <summary>
            /// Gets or sets the ocr fix list.
            /// </summary>
            public string OcrFixList { get; set; }

            /// <summary>
            /// Gets or sets the location.
            /// </summary>
            public string Location { get; set; }

            /// <summary>
            /// Gets or sets the use online names etc.
            /// </summary>
            public string UseOnlineNamesEtc { get; set; }

            /// <summary>
            /// Gets or sets the word added x.
            /// </summary>
            public string WordAddedX { get; set; }

            /// <summary>
            /// Gets or sets the word already exists.
            /// </summary>
            public string WordAlreadyExists { get; set; }

            /// <summary>
            /// Gets or sets the word not found.
            /// </summary>
            public string WordNotFound { get; set; }

            /// <summary>
            /// Gets or sets the remove x.
            /// </summary>
            public string RemoveX { get; set; }

            /// <summary>
            /// Gets or sets the cannot update names etc online.
            /// </summary>
            public string CannotUpdateNamesEtcOnline { get; set; }

            /// <summary>
            /// Gets or sets the proxy server settings.
            /// </summary>
            public string ProxyServerSettings { get; set; }

            /// <summary>
            /// Gets or sets the proxy address.
            /// </summary>
            public string ProxyAddress { get; set; }

            /// <summary>
            /// Gets or sets the proxy authentication.
            /// </summary>
            public string ProxyAuthentication { get; set; }

            /// <summary>
            /// Gets or sets the proxy user name.
            /// </summary>
            public string ProxyUserName { get; set; }

            /// <summary>
            /// Gets or sets the proxy password.
            /// </summary>
            public string ProxyPassword { get; set; }

            /// <summary>
            /// Gets or sets the proxy domain.
            /// </summary>
            public string ProxyDomain { get; set; }

            /// <summary>
            /// Gets or sets the play x seconds and back.
            /// </summary>
            public string PlayXSecondsAndBack { get; set; }

            /// <summary>
            /// Gets or sets the start scene index.
            /// </summary>
            public string StartSceneIndex { get; set; }

            /// <summary>
            /// Gets or sets the end scene index.
            /// </summary>
            public string EndSceneIndex { get; set; }

            /// <summary>
            /// Gets or sets the first plus x.
            /// </summary>
            public string FirstPlusX { get; set; }

            /// <summary>
            /// Gets or sets the last minus x.
            /// </summary>
            public string LastMinusX { get; set; }

            /// <summary>
            /// Gets or sets the fix commonerrors.
            /// </summary>
            public string FixCommonerrors { get; set; }

            /// <summary>
            /// Gets or sets the merge lines shorter than.
            /// </summary>
            public string MergeLinesShorterThan { get; set; }

            /// <summary>
            /// Gets or sets the music symbol.
            /// </summary>
            public string MusicSymbol { get; set; }

            /// <summary>
            /// Gets or sets the music symbols to replace.
            /// </summary>
            public string MusicSymbolsToReplace { get; set; }

            /// <summary>
            /// Gets or sets the fix common ocr errors use hardcoded rules.
            /// </summary>
            public string FixCommonOcrErrorsUseHardcodedRules { get; set; }

            /// <summary>
            /// Gets or sets the fix commonerrors fix short display times allow move start time.
            /// </summary>
            public string FixCommonerrorsFixShortDisplayTimesAllowMoveStartTime { get; set; }

            /// <summary>
            /// Gets or sets the shortcuts.
            /// </summary>
            public string Shortcuts { get; set; }

            /// <summary>
            /// Gets or sets the shortcut.
            /// </summary>
            public string Shortcut { get; set; }

            /// <summary>
            /// Gets or sets the control.
            /// </summary>
            public string Control { get; set; }

            /// <summary>
            /// Gets or sets the alt.
            /// </summary>
            public string Alt { get; set; }

            /// <summary>
            /// Gets or sets the shift.
            /// </summary>
            public string Shift { get; set; }

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the text box.
            /// </summary>
            public string TextBox { get; set; }

            /// <summary>
            /// Gets or sets the update shortcut.
            /// </summary>
            public string UpdateShortcut { get; set; }

            /// <summary>
            /// Gets or sets the shortcut is not valid.
            /// </summary>
            public string ShortcutIsNotValid { get; set; }

            /// <summary>
            /// Gets or sets the toggle dock undock of video controls.
            /// </summary>
            public string ToggleDockUndockOfVideoControls { get; set; }

            /// <summary>
            /// Gets or sets the create set end add new and go to new.
            /// </summary>
            public string CreateSetEndAddNewAndGoToNew { get; set; }

            /// <summary>
            /// Gets or sets the adjust via end auto start and go to next.
            /// </summary>
            public string AdjustViaEndAutoStartAndGoToNext { get; set; }

            /// <summary>
            /// Gets or sets the adjust set end time and go to next.
            /// </summary>
            public string AdjustSetEndTimeAndGoToNext { get; set; }

            /// <summary>
            /// Gets or sets the adjust set start auto duration and go to next.
            /// </summary>
            public string AdjustSetStartAutoDurationAndGoToNext { get; set; }

            /// <summary>
            /// Gets or sets the adjust set end next start and go to next.
            /// </summary>
            public string AdjustSetEndNextStartAndGoToNext { get; set; }

            /// <summary>
            /// Gets or sets the adjust start down end up and go to next.
            /// </summary>
            public string AdjustStartDownEndUpAndGoToNext { get; set; }

            /// <summary>
            /// Gets or sets the adjust selected 100 ms forward.
            /// </summary>
            public string AdjustSelected100MsForward { get; set; }

            /// <summary>
            /// Gets or sets the adjust selected 100 ms back.
            /// </summary>
            public string AdjustSelected100MsBack { get; set; }

            /// <summary>
            /// Gets or sets the adjust set start time keep duration.
            /// </summary>
            public string AdjustSetStartTimeKeepDuration { get; set; }

            /// <summary>
            /// Gets or sets the adjust set end and offset the rest.
            /// </summary>
            public string AdjustSetEndAndOffsetTheRest { get; set; }

            /// <summary>
            /// Gets or sets the adjust set end and offset the rest and go to next.
            /// </summary>
            public string AdjustSetEndAndOffsetTheRestAndGoToNext { get; set; }

            /// <summary>
            /// Gets or sets the main create start down end up.
            /// </summary>
            public string MainCreateStartDownEndUp { get; set; }

            /// <summary>
            /// Gets or sets the merge dialog.
            /// </summary>
            public string MergeDialog { get; set; }

            /// <summary>
            /// Gets or sets the go to next.
            /// </summary>
            public string GoToNext { get; set; }

            /// <summary>
            /// Gets or sets the go to previous.
            /// </summary>
            public string GoToPrevious { get; set; }

            /// <summary>
            /// Gets or sets the go to current subtitle start.
            /// </summary>
            public string GoToCurrentSubtitleStart { get; set; }

            /// <summary>
            /// Gets or sets the go to current subtitle end.
            /// </summary>
            public string GoToCurrentSubtitleEnd { get; set; }

            /// <summary>
            /// Gets or sets the toggle focus.
            /// </summary>
            public string ToggleFocus { get; set; }

            /// <summary>
            /// Gets or sets the toggle dialog dashes.
            /// </summary>
            public string ToggleDialogDashes { get; set; }

            /// <summary>
            /// Gets or sets the alignment.
            /// </summary>
            public string Alignment { get; set; }

            /// <summary>
            /// Gets or sets the copy text only.
            /// </summary>
            public string CopyTextOnly { get; set; }

            /// <summary>
            /// Gets or sets the copy text only from original to current.
            /// </summary>
            public string CopyTextOnlyFromOriginalToCurrent { get; set; }

            /// <summary>
            /// Gets or sets the auto duration selected lines.
            /// </summary>
            public string AutoDurationSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the reverse start and ending for rtl.
            /// </summary>
            public string ReverseStartAndEndingForRTL { get; set; }

            /// <summary>
            /// Gets or sets the vertical zoom.
            /// </summary>
            public string VerticalZoom { get; set; }

            /// <summary>
            /// Gets or sets the vertical zoom out.
            /// </summary>
            public string VerticalZoomOut { get; set; }

            /// <summary>
            /// Gets or sets the waveform seek silence forward.
            /// </summary>
            public string WaveformSeekSilenceForward { get; set; }

            /// <summary>
            /// Gets or sets the waveform seek silence back.
            /// </summary>
            public string WaveformSeekSilenceBack { get; set; }

            /// <summary>
            /// Gets or sets the waveform add text here.
            /// </summary>
            public string WaveformAddTextHere { get; set; }

            /// <summary>
            /// Gets or sets the waveform play new selection.
            /// </summary>
            public string WaveformPlayNewSelection { get; set; }

            /// <summary>
            /// Gets or sets the waveform play first selected subtitle.
            /// </summary>
            public string WaveformPlayFirstSelectedSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the waveform focus list view.
            /// </summary>
            public string WaveformFocusListView { get; set; }

            /// <summary>
            /// Gets or sets the go back 1 frame.
            /// </summary>
            public string GoBack1Frame { get; set; }

            /// <summary>
            /// Gets or sets the go forward 1 frame.
            /// </summary>
            public string GoForward1Frame { get; set; }

            /// <summary>
            /// Gets or sets the go back 100 milliseconds.
            /// </summary>
            public string GoBack100Milliseconds { get; set; }

            /// <summary>
            /// Gets or sets the go forward 100 milliseconds.
            /// </summary>
            public string GoForward100Milliseconds { get; set; }

            /// <summary>
            /// Gets or sets the go back 500 milliseconds.
            /// </summary>
            public string GoBack500Milliseconds { get; set; }

            /// <summary>
            /// Gets or sets the go forward 500 milliseconds.
            /// </summary>
            public string GoForward500Milliseconds { get; set; }

            /// <summary>
            /// Gets or sets the go back 1 second.
            /// </summary>
            public string GoBack1Second { get; set; }

            /// <summary>
            /// Gets or sets the go forward 1 second.
            /// </summary>
            public string GoForward1Second { get; set; }

            /// <summary>
            /// Gets or sets the toggle play pause.
            /// </summary>
            public string TogglePlayPause { get; set; }

            /// <summary>
            /// Gets or sets the pause.
            /// </summary>
            public string Pause { get; set; }

            /// <summary>
            /// Gets or sets the fullscreen.
            /// </summary>
            public string Fullscreen { get; set; }

            /// <summary>
            /// Gets or sets the custom search 1.
            /// </summary>
            public string CustomSearch1 { get; set; }

            /// <summary>
            /// Gets or sets the custom search 2.
            /// </summary>
            public string CustomSearch2 { get; set; }

            /// <summary>
            /// Gets or sets the custom search 3.
            /// </summary>
            public string CustomSearch3 { get; set; }

            /// <summary>
            /// Gets or sets the custom search 4.
            /// </summary>
            public string CustomSearch4 { get; set; }

            /// <summary>
            /// Gets or sets the custom search 5.
            /// </summary>
            public string CustomSearch5 { get; set; }

            /// <summary>
            /// Gets or sets the custom search 6.
            /// </summary>
            public string CustomSearch6 { get; set; }

            /// <summary>
            /// Gets or sets the syntax coloring.
            /// </summary>
            public string SyntaxColoring { get; set; }

            /// <summary>
            /// Gets or sets the list view syntax coloring.
            /// </summary>
            public string ListViewSyntaxColoring { get; set; }

            /// <summary>
            /// Gets or sets the syntax color duration if too small.
            /// </summary>
            public string SyntaxColorDurationIfTooSmall { get; set; }

            /// <summary>
            /// Gets or sets the syntax color duration if too large.
            /// </summary>
            public string SyntaxColorDurationIfTooLarge { get; set; }

            /// <summary>
            /// Gets or sets the syntax color text if too long.
            /// </summary>
            public string SyntaxColorTextIfTooLong { get; set; }

            /// <summary>
            /// Gets or sets the syntax color text more than x lines.
            /// </summary>
            public string SyntaxColorTextMoreThanXLines { get; set; }

            /// <summary>
            /// Gets or sets the syntax color overlap.
            /// </summary>
            public string SyntaxColorOverlap { get; set; }

            /// <summary>
            /// Gets or sets the syntax error color.
            /// </summary>
            public string SyntaxErrorColor { get; set; }

            /// <summary>
            /// Gets or sets the go to first selected line.
            /// </summary>
            public string GoToFirstSelectedLine { get; set; }

            /// <summary>
            /// Gets or sets the go to next empty line.
            /// </summary>
            public string GoToNextEmptyLine { get; set; }

            /// <summary>
            /// Gets or sets the merge selected lines.
            /// </summary>
            public string MergeSelectedLines { get; set; }

            /// <summary>
            /// Gets or sets the merge selected lines only first text.
            /// </summary>
            public string MergeSelectedLinesOnlyFirstText { get; set; }

            /// <summary>
            /// Gets or sets the toggle translation mode.
            /// </summary>
            public string ToggleTranslationMode { get; set; }

            /// <summary>
            /// Gets or sets the switch original and translation.
            /// </summary>
            public string SwitchOriginalAndTranslation { get; set; }

            /// <summary>
            /// Gets or sets the merge original and translation.
            /// </summary>
            public string MergeOriginalAndTranslation { get; set; }

            /// <summary>
            /// Gets or sets the shortcut is already defined x.
            /// </summary>
            public string ShortcutIsAlreadyDefinedX { get; set; }

            /// <summary>
            /// Gets or sets the toggle translation and original in previews.
            /// </summary>
            public string ToggleTranslationAndOriginalInPreviews { get; set; }

            /// <summary>
            /// Gets or sets the list view column delete.
            /// </summary>
            public string ListViewColumnDelete { get; set; }

            /// <summary>
            /// Gets or sets the list view column insert.
            /// </summary>
            public string ListViewColumnInsert { get; set; }

            /// <summary>
            /// Gets or sets the list view column paste.
            /// </summary>
            public string ListViewColumnPaste { get; set; }

            /// <summary>
            /// Gets or sets the list view focus waveform.
            /// </summary>
            public string ListViewFocusWaveform { get; set; }

            /// <summary>
            /// Gets or sets the list view go to next error.
            /// </summary>
            public string ListViewGoToNextError { get; set; }

            /// <summary>
            /// Gets or sets the show beamer.
            /// </summary>
            public string ShowBeamer { get; set; }

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
            /// Gets or sets the main text box auto break.
            /// </summary>
            public string MainTextBoxAutoBreak { get; set; }

            /// <summary>
            /// Gets or sets the main text box unbreak.
            /// </summary>
            public string MainTextBoxUnbreak { get; set; }

            /// <summary>
            /// Gets or sets the main file save all.
            /// </summary>
            public string MainFileSaveAll { get; set; }

            /// <summary>
            /// Gets or sets the miscellaneous.
            /// </summary>
            public string Miscellaneous { get; set; }

            /// <summary>
            /// Gets or sets the use do not break after list.
            /// </summary>
            public string UseDoNotBreakAfterList { get; set; }
        }

        /// <summary>
        /// The set video offset.
        /// </summary>
        public class SetVideoOffset
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the relative to current video position.
            /// </summary>
            public string RelativeToCurrentVideoPosition { get; set; }
        }

        /// <summary>
        /// The show earlier later.
        /// </summary>
        public class ShowEarlierLater
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title all.
            /// </summary>
            public string TitleAll { get; set; }

            /// <summary>
            /// Gets or sets the show earlier.
            /// </summary>
            public string ShowEarlier { get; set; }

            /// <summary>
            /// Gets or sets the show later.
            /// </summary>
            public string ShowLater { get; set; }

            /// <summary>
            /// Gets or sets the total adjustment x.
            /// </summary>
            public string TotalAdjustmentX { get; set; }

            /// <summary>
            /// Gets or sets the all lines.
            /// </summary>
            public string AllLines { get; set; }

            /// <summary>
            /// Gets or sets the selected lines only.
            /// </summary>
            public string SelectedLinesOnly { get; set; }

            /// <summary>
            /// Gets or sets the selected lines and forward.
            /// </summary>
            public string SelectedLinesAndForward { get; set; }
        }

        /// <summary>
        /// The show history.
        /// </summary>
        public class ShowHistory
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the select rollback point.
            /// </summary>
            public string SelectRollbackPoint { get; set; }

            /// <summary>
            /// Gets or sets the time.
            /// </summary>
            public string Time { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the compare history items.
            /// </summary>
            public string CompareHistoryItems { get; set; }

            /// <summary>
            /// Gets or sets the compare with current.
            /// </summary>
            public string CompareWithCurrent { get; set; }

            /// <summary>
            /// Gets or sets the rollback.
            /// </summary>
            public string Rollback { get; set; }
        }

        /// <summary>
        /// The spell check.
        /// </summary>
        public class SpellCheck
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the full text.
            /// </summary>
            public string FullText { get; set; }

            /// <summary>
            /// Gets or sets the word not found.
            /// </summary>
            public string WordNotFound { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the change.
            /// </summary>
            public string Change { get; set; }

            /// <summary>
            /// Gets or sets the change all.
            /// </summary>
            public string ChangeAll { get; set; }

            /// <summary>
            /// Gets or sets the skip once.
            /// </summary>
            public string SkipOnce { get; set; }

            /// <summary>
            /// Gets or sets the skip all.
            /// </summary>
            public string SkipAll { get; set; }

            /// <summary>
            /// Gets or sets the add to user dictionary.
            /// </summary>
            public string AddToUserDictionary { get; set; }

            /// <summary>
            /// Gets or sets the add to names and ignore list.
            /// </summary>
            public string AddToNamesAndIgnoreList { get; set; }

            /// <summary>
            /// Gets or sets the add to ocr replace list.
            /// </summary>
            public string AddToOcrReplaceList { get; set; }

            /// <summary>
            /// Gets or sets the abort.
            /// </summary>
            public string Abort { get; set; }

            /// <summary>
            /// Gets or sets the use.
            /// </summary>
            public string Use { get; set; }

            /// <summary>
            /// Gets or sets the use always.
            /// </summary>
            public string UseAlways { get; set; }

            /// <summary>
            /// Gets or sets the suggestions.
            /// </summary>
            public string Suggestions { get; set; }

            /// <summary>
            /// Gets or sets the spell check progress.
            /// </summary>
            public string SpellCheckProgress { get; set; }

            /// <summary>
            /// Gets or sets the edit whole text.
            /// </summary>
            public string EditWholeText { get; set; }

            /// <summary>
            /// Gets or sets the edit word only.
            /// </summary>
            public string EditWordOnly { get; set; }

            /// <summary>
            /// Gets or sets the add x to names etc.
            /// </summary>
            public string AddXToNamesEtc { get; set; }

            /// <summary>
            /// Gets or sets the auto fix names.
            /// </summary>
            public string AutoFixNames { get; set; }

            /// <summary>
            /// Gets or sets the check one letter words.
            /// </summary>
            public string CheckOneLetterWords { get; set; }

            /// <summary>
            /// Gets or sets the treat in quote as ing.
            /// </summary>
            public string TreatINQuoteAsING { get; set; }

            /// <summary>
            /// Gets or sets the image text.
            /// </summary>
            public string ImageText { get; set; }

            /// <summary>
            /// Gets or sets the spell check completed.
            /// </summary>
            public string SpellCheckCompleted { get; set; }

            /// <summary>
            /// Gets or sets the spell check aborted.
            /// </summary>
            public string SpellCheckAborted { get; set; }

            /// <summary>
            /// Gets or sets the undo x.
            /// </summary>
            public string UndoX { get; set; }
        }

        /// <summary>
        /// The split.
        /// </summary>
        public class Split
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the split options.
            /// </summary>
            public string SplitOptions { get; set; }

            /// <summary>
            /// Gets or sets the lines.
            /// </summary>
            public string Lines { get; set; }

            /// <summary>
            /// Gets or sets the characters.
            /// </summary>
            public string Characters { get; set; }

            /// <summary>
            /// Gets or sets the number of equal parts.
            /// </summary>
            public string NumberOfEqualParts { get; set; }

            /// <summary>
            /// Gets or sets the subtitle info.
            /// </summary>
            public string SubtitleInfo { get; set; }

            /// <summary>
            /// Gets or sets the number of lines x.
            /// </summary>
            public string NumberOfLinesX { get; set; }

            /// <summary>
            /// Gets or sets the number of characters x.
            /// </summary>
            public string NumberOfCharactersX { get; set; }

            /// <summary>
            /// Gets or sets the output.
            /// </summary>
            public string Output { get; set; }

            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the output folder.
            /// </summary>
            public string OutputFolder { get; set; }

            /// <summary>
            /// Gets or sets the do split.
            /// </summary>
            public string DoSplit { get; set; }

            /// <summary>
            /// Gets or sets the basic.
            /// </summary>
            public string Basic { get; set; }
        }

        /// <summary>
        /// The split long lines.
        /// </summary>
        public class SplitLongLines
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the single line maximum length.
            /// </summary>
            public string SingleLineMaximumLength { get; set; }

            /// <summary>
            /// Gets or sets the line maximum length.
            /// </summary>
            public string LineMaximumLength { get; set; }

            /// <summary>
            /// Gets or sets the line continuation begin end strings.
            /// </summary>
            public string LineContinuationBeginEndStrings { get; set; }

            /// <summary>
            /// Gets or sets the number of splits.
            /// </summary>
            public string NumberOfSplits { get; set; }

            /// <summary>
            /// Gets or sets the longest single line is x at y.
            /// </summary>
            public string LongestSingleLineIsXAtY { get; set; }

            /// <summary>
            /// Gets or sets the longest line is x at y.
            /// </summary>
            public string LongestLineIsXAtY { get; set; }
        }

        /// <summary>
        /// The split subtitle.
        /// </summary>
        public class SplitSubtitle
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description 1.
            /// </summary>
            public string Description1 { get; set; }

            /// <summary>
            /// Gets or sets the description 2.
            /// </summary>
            public string Description2 { get; set; }

            /// <summary>
            /// Gets or sets the split.
            /// </summary>
            public string Split { get; set; }

            /// <summary>
            /// Gets or sets the done.
            /// </summary>
            public string Done { get; set; }

            /// <summary>
            /// Gets or sets the nothing to split.
            /// </summary>
            public string NothingToSplit { get; set; }

            /// <summary>
            /// Gets or sets the save part one as.
            /// </summary>
            public string SavePartOneAs { get; set; }

            /// <summary>
            /// Gets or sets the save part two as.
            /// </summary>
            public string SavePartTwoAs { get; set; }

            /// <summary>
            /// Gets or sets the part 1.
            /// </summary>
            public string Part1 { get; set; }

            /// <summary>
            /// Gets or sets the part 2.
            /// </summary>
            public string Part2 { get; set; }

            /// <summary>
            /// Gets or sets the unable to save file x.
            /// </summary>
            public string UnableToSaveFileX { get; set; }

            /// <summary>
            /// Gets or sets the overwrite existing files.
            /// </summary>
            public string OverwriteExistingFiles { get; set; }

            /// <summary>
            /// Gets or sets the folder not found x.
            /// </summary>
            public string FolderNotFoundX { get; set; }

            /// <summary>
            /// Gets or sets the untitled.
            /// </summary>
            public string Untitled { get; set; }
        }

        /// <summary>
        /// The start numbering from.
        /// </summary>
        public class StartNumberingFrom
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the start from number.
            /// </summary>
            public string StartFromNumber { get; set; }

            /// <summary>
            /// Gets or sets the please enter a valid number.
            /// </summary>
            public string PleaseEnterAValidNumber { get; set; }
        }

        /// <summary>
        /// The statistics.
        /// </summary>
        public class Statistics
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title with file name.
            /// </summary>
            public string TitleWithFileName { get; set; }

            /// <summary>
            /// Gets or sets the general statistics.
            /// </summary>
            public string GeneralStatistics { get; set; }

            /// <summary>
            /// Gets or sets the most used.
            /// </summary>
            public string MostUsed { get; set; }

            /// <summary>
            /// Gets or sets the most used lines.
            /// </summary>
            public string MostUsedLines { get; set; }

            /// <summary>
            /// Gets or sets the most used words.
            /// </summary>
            public string MostUsedWords { get; set; }

            /// <summary>
            /// Gets or sets the nothing found.
            /// </summary>
            public string NothingFound { get; set; }

            /// <summary>
            /// Gets or sets the number of lines x.
            /// </summary>
            public string NumberOfLinesX { get; set; }

            /// <summary>
            /// Gets or sets the length in format xin characters y.
            /// </summary>
            public string LengthInFormatXinCharactersY { get; set; }

            /// <summary>
            /// Gets or sets the number of characters in text only.
            /// </summary>
            public string NumberOfCharactersInTextOnly { get; set; }

            /// <summary>
            /// Gets or sets the total chars per second.
            /// </summary>
            public string TotalCharsPerSecond { get; set; }

            /// <summary>
            /// Gets or sets the number of italic tags.
            /// </summary>
            public string NumberOfItalicTags { get; set; }

            /// <summary>
            /// Gets or sets the number of bold tags.
            /// </summary>
            public string NumberOfBoldTags { get; set; }

            /// <summary>
            /// Gets or sets the number of underline tags.
            /// </summary>
            public string NumberOfUnderlineTags { get; set; }

            /// <summary>
            /// Gets or sets the number of font tags.
            /// </summary>
            public string NumberOfFontTags { get; set; }

            /// <summary>
            /// Gets or sets the number of alignment tags.
            /// </summary>
            public string NumberOfAlignmentTags { get; set; }

            /// <summary>
            /// Gets or sets the line length minimum.
            /// </summary>
            public string LineLengthMinimum { get; set; }

            /// <summary>
            /// Gets or sets the line length maximum.
            /// </summary>
            public string LineLengthMaximum { get; set; }

            /// <summary>
            /// Gets or sets the line length average.
            /// </summary>
            public string LineLengthAverage { get; set; }

            /// <summary>
            /// Gets or sets the lines per subtitle average.
            /// </summary>
            public string LinesPerSubtitleAverage { get; set; }

            /// <summary>
            /// Gets or sets the single line length minimum.
            /// </summary>
            public string SingleLineLengthMinimum { get; set; }

            /// <summary>
            /// Gets or sets the single line length maximum.
            /// </summary>
            public string SingleLineLengthMaximum { get; set; }

            /// <summary>
            /// Gets or sets the single line length average.
            /// </summary>
            public string SingleLineLengthAverage { get; set; }

            /// <summary>
            /// Gets or sets the duration minimum.
            /// </summary>
            public string DurationMinimum { get; set; }

            /// <summary>
            /// Gets or sets the duration maximum.
            /// </summary>
            public string DurationMaximum { get; set; }

            /// <summary>
            /// Gets or sets the duration average.
            /// </summary>
            public string DurationAverage { get; set; }

            /// <summary>
            /// Gets or sets the characters per second minimum.
            /// </summary>
            public string CharactersPerSecondMinimum { get; set; }

            /// <summary>
            /// Gets or sets the characters per second maximum.
            /// </summary>
            public string CharactersPerSecondMaximum { get; set; }

            /// <summary>
            /// Gets or sets the characters per second average.
            /// </summary>
            public string CharactersPerSecondAverage { get; set; }

            /// <summary>
            /// Gets or sets the export.
            /// </summary>
            public string Export { get; set; }
        }

        /// <summary>
        /// The sub station alpha properties.
        /// </summary>
        public class SubStationAlphaProperties
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title substation alpha.
            /// </summary>
            public string TitleSubstationAlpha { get; set; }

            /// <summary>
            /// Gets or sets the script.
            /// </summary>
            public string Script { get; set; }

            /// <summary>
            /// Gets or sets the script title.
            /// </summary>
            public string ScriptTitle { get; set; }

            /// <summary>
            /// Gets or sets the original script.
            /// </summary>
            public string OriginalScript { get; set; }

            /// <summary>
            /// Gets or sets the translation.
            /// </summary>
            public string Translation { get; set; }

            /// <summary>
            /// Gets or sets the editing.
            /// </summary>
            public string Editing { get; set; }

            /// <summary>
            /// Gets or sets the timing.
            /// </summary>
            public string Timing { get; set; }

            /// <summary>
            /// Gets or sets the sync point.
            /// </summary>
            public string SyncPoint { get; set; }

            /// <summary>
            /// Gets or sets the updated by.
            /// </summary>
            public string UpdatedBy { get; set; }

            /// <summary>
            /// Gets or sets the update details.
            /// </summary>
            public string UpdateDetails { get; set; }

            /// <summary>
            /// Gets or sets the resolution.
            /// </summary>
            public string Resolution { get; set; }

            /// <summary>
            /// Gets or sets the video resolution.
            /// </summary>
            public string VideoResolution { get; set; }

            /// <summary>
            /// Gets or sets the options.
            /// </summary>
            public string Options { get; set; }

            /// <summary>
            /// Gets or sets the wrap style.
            /// </summary>
            public string WrapStyle { get; set; }

            /// <summary>
            /// Gets or sets the collision.
            /// </summary>
            public string Collision { get; set; }

            /// <summary>
            /// Gets or sets the scale border and shadow.
            /// </summary>
            public string ScaleBorderAndShadow { get; set; }
        }

        /// <summary>
        /// The sub station alpha styles.
        /// </summary>
        public class SubStationAlphaStyles
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title substation alpha.
            /// </summary>
            public string TitleSubstationAlpha { get; set; }

            /// <summary>
            /// Gets or sets the styles.
            /// </summary>
            public string Styles { get; set; }

            /// <summary>
            /// Gets or sets the properties.
            /// </summary>
            public string Properties { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the font.
            /// </summary>
            public string Font { get; set; }

            /// <summary>
            /// Gets or sets the font name.
            /// </summary>
            public string FontName { get; set; }

            /// <summary>
            /// Gets or sets the font size.
            /// </summary>
            public string FontSize { get; set; }

            /// <summary>
            /// Gets or sets the use count.
            /// </summary>
            public string UseCount { get; set; }

            /// <summary>
            /// Gets or sets the primary.
            /// </summary>
            public string Primary { get; set; }

            /// <summary>
            /// Gets or sets the secondary.
            /// </summary>
            public string Secondary { get; set; }

            /// <summary>
            /// Gets or sets the tertiary.
            /// </summary>
            public string Tertiary { get; set; }

            /// <summary>
            /// Gets or sets the outline.
            /// </summary>
            public string Outline { get; set; }

            /// <summary>
            /// Gets or sets the shadow.
            /// </summary>
            public string Shadow { get; set; }

            /// <summary>
            /// Gets or sets the back.
            /// </summary>
            public string Back { get; set; }

            /// <summary>
            /// Gets or sets the alignment.
            /// </summary>
            public string Alignment { get; set; }

            /// <summary>
            /// Gets or sets the top left.
            /// </summary>
            public string TopLeft { get; set; }

            /// <summary>
            /// Gets or sets the top center.
            /// </summary>
            public string TopCenter { get; set; }

            /// <summary>
            /// Gets or sets the top right.
            /// </summary>
            public string TopRight { get; set; }

            /// <summary>
            /// Gets or sets the middle left.
            /// </summary>
            public string MiddleLeft { get; set; }

            /// <summary>
            /// Gets or sets the middle center.
            /// </summary>
            public string MiddleCenter { get; set; }

            /// <summary>
            /// Gets or sets the middle right.
            /// </summary>
            public string MiddleRight { get; set; }

            /// <summary>
            /// Gets or sets the bottom left.
            /// </summary>
            public string BottomLeft { get; set; }

            /// <summary>
            /// Gets or sets the bottom center.
            /// </summary>
            public string BottomCenter { get; set; }

            /// <summary>
            /// Gets or sets the bottom right.
            /// </summary>
            public string BottomRight { get; set; }

            /// <summary>
            /// Gets or sets the colors.
            /// </summary>
            public string Colors { get; set; }

            /// <summary>
            /// Gets or sets the margins.
            /// </summary>
            public string Margins { get; set; }

            /// <summary>
            /// Gets or sets the margin left.
            /// </summary>
            public string MarginLeft { get; set; }

            /// <summary>
            /// Gets or sets the margin right.
            /// </summary>
            public string MarginRight { get; set; }

            /// <summary>
            /// Gets or sets the margin vertical.
            /// </summary>
            public string MarginVertical { get; set; }

            /// <summary>
            /// Gets or sets the border.
            /// </summary>
            public string Border { get; set; }

            /// <summary>
            /// Gets or sets the plus shadow.
            /// </summary>
            public string PlusShadow { get; set; }

            /// <summary>
            /// Gets or sets the opaque box.
            /// </summary>
            public string OpaqueBox { get; set; }

            /// <summary>
            /// Gets or sets the import.
            /// </summary>
            public string Import { get; set; }

            /// <summary>
            /// Gets or sets the export.
            /// </summary>
            public string Export { get; set; }

            /// <summary>
            /// Gets or sets the copy.
            /// </summary>
            public string Copy { get; set; }

            /// <summary>
            /// Gets or sets the copy of y.
            /// </summary>
            public string CopyOfY { get; set; }

            /// <summary>
            /// Gets or sets the copy x of y.
            /// </summary>
            public string CopyXOfY { get; set; }

            /// <summary>
            /// Gets or sets the new.
            /// </summary>
            public string New { get; set; }

            /// <summary>
            /// Gets or sets the remove.
            /// </summary>
            public string Remove { get; set; }

            /// <summary>
            /// Gets or sets the remove all.
            /// </summary>
            public string RemoveAll { get; set; }

            /// <summary>
            /// Gets or sets the import style from file.
            /// </summary>
            public string ImportStyleFromFile { get; set; }

            /// <summary>
            /// Gets or sets the export style to file.
            /// </summary>
            public string ExportStyleToFile { get; set; }

            /// <summary>
            /// Gets or sets the choose style.
            /// </summary>
            public string ChooseStyle { get; set; }

            /// <summary>
            /// Gets or sets the style already exits.
            /// </summary>
            public string StyleAlreadyExits { get; set; }

            /// <summary>
            /// Gets or sets the style x exported to file y.
            /// </summary>
            public string StyleXExportedToFileY { get; set; }

            /// <summary>
            /// Gets or sets the style x imported from file y.
            /// </summary>
            public string StyleXImportedFromFileY { get; set; }
        }

        /// <summary>
        /// The point sync.
        /// </summary>
        public class PointSync
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title via other subtitle.
            /// </summary>
            public string TitleViaOtherSubtitle { get; set; }

            /// <summary>
            /// Gets or sets the sync help.
            /// </summary>
            public string SyncHelp { get; set; }

            /// <summary>
            /// Gets or sets the set sync point.
            /// </summary>
            public string SetSyncPoint { get; set; }

            /// <summary>
            /// Gets or sets the remove sync point.
            /// </summary>
            public string RemoveSyncPoint { get; set; }

            /// <summary>
            /// Gets or sets the sync points x.
            /// </summary>
            public string SyncPointsX { get; set; }

            /// <summary>
            /// Gets or sets the info.
            /// </summary>
            public string Info { get; set; }

            /// <summary>
            /// Gets or sets the apply sync.
            /// </summary>
            public string ApplySync { get; set; }
        }

        /// <summary>
        /// The transport stream subtitle chooser.
        /// </summary>
        public class TransportStreamSubtitleChooser
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the pid line.
            /// </summary>
            public string PidLine { get; set; }

            /// <summary>
            /// Gets or sets the sub line.
            /// </summary>
            public string SubLine { get; set; }
        }

        /// <summary>
        /// The unknown subtitle.
        /// </summary>
        public class UnknownSubtitle
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            public string Message { get; set; }
        }

        /// <summary>
        /// The visual sync.
        /// </summary>
        public class VisualSync
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the start scene.
            /// </summary>
            public string StartScene { get; set; }

            /// <summary>
            /// Gets or sets the end scene.
            /// </summary>
            public string EndScene { get; set; }

            /// <summary>
            /// Gets or sets the synchronize.
            /// </summary>
            public string Synchronize { get; set; }

            /// <summary>
            /// Gets or sets the half a second back.
            /// </summary>
            public string HalfASecondBack { get; set; }

            /// <summary>
            /// Gets or sets the three seconds back.
            /// </summary>
            public string ThreeSecondsBack { get; set; }

            /// <summary>
            /// Gets or sets the play x seconds and back.
            /// </summary>
            public string PlayXSecondsAndBack { get; set; }

            /// <summary>
            /// Gets or sets the find text.
            /// </summary>
            public string FindText { get; set; }

            /// <summary>
            /// Gets or sets the go to sub position.
            /// </summary>
            public string GoToSubPosition { get; set; }

            /// <summary>
            /// Gets or sets the keep changes title.
            /// </summary>
            public string KeepChangesTitle { get; set; }

            /// <summary>
            /// Gets or sets the keep changes message.
            /// </summary>
            public string KeepChangesMessage { get; set; }

            /// <summary>
            /// Gets or sets the synchronization done.
            /// </summary>
            public string SynchronizationDone { get; set; }

            /// <summary>
            /// Gets or sets the start scene must come before end scene.
            /// </summary>
            public string StartSceneMustComeBeforeEndScene { get; set; }

            /// <summary>
            /// Gets or sets the tip.
            /// </summary>
            public string Tip { get; set; }
        }

        /// <summary>
        /// The vob sub edit characters.
        /// </summary>
        public class VobSubEditCharacters
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the choose character.
            /// </summary>
            public string ChooseCharacter { get; set; }

            /// <summary>
            /// Gets or sets the image compare files.
            /// </summary>
            public string ImageCompareFiles { get; set; }

            /// <summary>
            /// Gets or sets the current compare image.
            /// </summary>
            public string CurrentCompareImage { get; set; }

            /// <summary>
            /// Gets or sets the text associated with image.
            /// </summary>
            public string TextAssociatedWithImage { get; set; }

            /// <summary>
            /// Gets or sets the is italic.
            /// </summary>
            public string IsItalic { get; set; }

            /// <summary>
            /// Gets or sets the update.
            /// </summary>
            public string Update { get; set; }

            /// <summary>
            /// Gets or sets the delete.
            /// </summary>
            public string Delete { get; set; }

            /// <summary>
            /// Gets or sets the image double size.
            /// </summary>
            public string ImageDoubleSize { get; set; }

            /// <summary>
            /// Gets or sets the image file not found.
            /// </summary>
            public string ImageFileNotFound { get; set; }

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            public string Image { get; set; }
        }

        /// <summary>
        /// The vob sub ocr.
        /// </summary>
        public class VobSubOcr
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the title blu ray.
            /// </summary>
            public string TitleBluRay { get; set; }

            /// <summary>
            /// Gets or sets the ocr method.
            /// </summary>
            public string OcrMethod { get; set; }

            /// <summary>
            /// Gets or sets the ocr via modi.
            /// </summary>
            public string OcrViaModi { get; set; }

            /// <summary>
            /// Gets or sets the ocr via tesseract.
            /// </summary>
            public string OcrViaTesseract { get; set; }

            /// <summary>
            /// Gets or sets the ocr via nocr.
            /// </summary>
            public string OcrViaNOCR { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the ocr via image compare.
            /// </summary>
            public string OcrViaImageCompare { get; set; }

            /// <summary>
            /// Gets or sets the image database.
            /// </summary>
            public string ImageDatabase { get; set; }

            /// <summary>
            /// Gets or sets the no of pixels is space.
            /// </summary>
            public string NoOfPixelsIsSpace { get; set; }

            /// <summary>
            /// Gets or sets the max error percent.
            /// </summary>
            public string MaxErrorPercent { get; set; }

            /// <summary>
            /// Gets or sets the new.
            /// </summary>
            public string New { get; set; }

            /// <summary>
            /// Gets or sets the edit.
            /// </summary>
            public string Edit { get; set; }

            /// <summary>
            /// Gets or sets the start ocr.
            /// </summary>
            public string StartOcr { get; set; }

            /// <summary>
            /// Gets or sets the stop.
            /// </summary>
            public string Stop { get; set; }

            /// <summary>
            /// Gets or sets the start ocr from.
            /// </summary>
            public string StartOcrFrom { get; set; }

            /// <summary>
            /// Gets or sets the loading vob sub images.
            /// </summary>
            public string LoadingVobSubImages { get; set; }

            /// <summary>
            /// Gets or sets the loading image compare database.
            /// </summary>
            public string LoadingImageCompareDatabase { get; set; }

            /// <summary>
            /// Gets or sets the converting image compare database.
            /// </summary>
            public string ConvertingImageCompareDatabase { get; set; }

            /// <summary>
            /// Gets or sets the subtitle image.
            /// </summary>
            public string SubtitleImage { get; set; }

            /// <summary>
            /// Gets or sets the subtitle text.
            /// </summary>
            public string SubtitleText { get; set; }

            /// <summary>
            /// Gets or sets the unable to create character database folder.
            /// </summary>
            public string UnableToCreateCharacterDatabaseFolder { get; set; }

            /// <summary>
            /// Gets or sets the subtitle image xof y.
            /// </summary>
            public string SubtitleImageXofY { get; set; }

            /// <summary>
            /// Gets or sets the image palette.
            /// </summary>
            public string ImagePalette { get; set; }

            /// <summary>
            /// Gets or sets the use custom colors.
            /// </summary>
            public string UseCustomColors { get; set; }

            /// <summary>
            /// Gets or sets the transparent.
            /// </summary>
            public string Transparent { get; set; }

            /// <summary>
            /// Gets or sets the transparent min alpha.
            /// </summary>
            public string TransparentMinAlpha { get; set; }

            /// <summary>
            /// Gets or sets the transport stream.
            /// </summary>
            public string TransportStream { get; set; }

            /// <summary>
            /// Gets or sets the transport stream grayscale.
            /// </summary>
            public string TransportStreamGrayscale { get; set; }

            /// <summary>
            /// Gets or sets the transport stream get color.
            /// </summary>
            public string TransportStreamGetColor { get; set; }

            /// <summary>
            /// Gets or sets the prompt for unknown words.
            /// </summary>
            public string PromptForUnknownWords { get; set; }

            /// <summary>
            /// Gets or sets the try to guess unkown words.
            /// </summary>
            public string TryToGuessUnkownWords { get; set; }

            /// <summary>
            /// Gets or sets the auto break subtitle if more than two lines.
            /// </summary>
            public string AutoBreakSubtitleIfMoreThanTwoLines { get; set; }

            /// <summary>
            /// Gets or sets the all fixes.
            /// </summary>
            public string AllFixes { get; set; }

            /// <summary>
            /// Gets or sets the guesses used.
            /// </summary>
            public string GuessesUsed { get; set; }

            /// <summary>
            /// Gets or sets the unknown words.
            /// </summary>
            public string UnknownWords { get; set; }

            /// <summary>
            /// Gets or sets the ocr auto correction spell checking.
            /// </summary>
            public string OcrAutoCorrectionSpellChecking { get; set; }

            /// <summary>
            /// Gets or sets the fix ocr errors.
            /// </summary>
            public string FixOcrErrors { get; set; }

            /// <summary>
            /// Gets or sets the import text with matching time codes.
            /// </summary>
            public string ImportTextWithMatchingTimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the import new time codes.
            /// </summary>
            public string ImportNewTimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the save subtitle image as.
            /// </summary>
            public string SaveSubtitleImageAs { get; set; }

            /// <summary>
            /// Gets or sets the save all subtitle images as bdn xml.
            /// </summary>
            public string SaveAllSubtitleImagesAsBdnXml { get; set; }

            /// <summary>
            /// Gets or sets the save all subtitle images with html.
            /// </summary>
            public string SaveAllSubtitleImagesWithHtml { get; set; }

            /// <summary>
            /// Gets or sets the x images saved in y.
            /// </summary>
            public string XImagesSavedInY { get; set; }

            /// <summary>
            /// Gets or sets the try modi for unknown words.
            /// </summary>
            public string TryModiForUnknownWords { get; set; }

            /// <summary>
            /// Gets or sets the dictionary x.
            /// </summary>
            public string DictionaryX { get; set; }

            /// <summary>
            /// Gets or sets the right to left.
            /// </summary>
            public string RightToLeft { get; set; }

            /// <summary>
            /// Gets or sets the show only forced subtitles.
            /// </summary>
            public string ShowOnlyForcedSubtitles { get; set; }

            /// <summary>
            /// Gets or sets the use time codes from idx.
            /// </summary>
            public string UseTimeCodesFromIdx { get; set; }

            /// <summary>
            /// Gets or sets the no match.
            /// </summary>
            public string NoMatch { get; set; }

            /// <summary>
            /// Gets or sets the auto transparent background.
            /// </summary>
            public string AutoTransparentBackground { get; set; }

            /// <summary>
            /// Gets or sets the inspect compare matches for current image.
            /// </summary>
            public string InspectCompareMatchesForCurrentImage { get; set; }

            /// <summary>
            /// Gets or sets the edit last additions.
            /// </summary>
            public string EditLastAdditions { get; set; }

            /// <summary>
            /// Gets or sets the set unitalic factor.
            /// </summary>
            public string SetUnitalicFactor { get; set; }

            /// <summary>
            /// Gets or sets the discard title.
            /// </summary>
            public string DiscardTitle { get; set; }

            /// <summary>
            /// Gets or sets the discard text.
            /// </summary>
            public string DiscardText { get; set; }
        }

        /// <summary>
        /// The vob sub ocr character.
        /// </summary>
        public class VobSubOcrCharacter
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the shrink selection.
            /// </summary>
            public string ShrinkSelection { get; set; }

            /// <summary>
            /// Gets or sets the expand selection.
            /// </summary>
            public string ExpandSelection { get; set; }

            /// <summary>
            /// Gets or sets the subtitle image.
            /// </summary>
            public string SubtitleImage { get; set; }

            /// <summary>
            /// Gets or sets the characters.
            /// </summary>
            public string Characters { get; set; }

            /// <summary>
            /// Gets or sets the characters as text.
            /// </summary>
            public string CharactersAsText { get; set; }

            /// <summary>
            /// Gets or sets the italic.
            /// </summary>
            public string Italic { get; set; }

            /// <summary>
            /// Gets or sets the abort.
            /// </summary>
            public string Abort { get; set; }

            /// <summary>
            /// Gets or sets the skip.
            /// </summary>
            public string Skip { get; set; }

            /// <summary>
            /// Gets or sets the nordic.
            /// </summary>
            public string Nordic { get; set; }

            /// <summary>
            /// Gets or sets the spanish.
            /// </summary>
            public string Spanish { get; set; }

            /// <summary>
            /// Gets or sets the german.
            /// </summary>
            public string German { get; set; }

            /// <summary>
            /// Gets or sets the auto submit on first char.
            /// </summary>
            public string AutoSubmitOnFirstChar { get; set; }

            /// <summary>
            /// Gets or sets the edit last x.
            /// </summary>
            public string EditLastX { get; set; }
        }

        /// <summary>
        /// The vob sub ocr character inspect.
        /// </summary>
        public class VobSubOcrCharacterInspect
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the inspect items.
            /// </summary>
            public string InspectItems { get; set; }

            /// <summary>
            /// Gets or sets the add better match.
            /// </summary>
            public string AddBetterMatch { get; set; }
        }

        /// <summary>
        /// The vob sub ocr new folder.
        /// </summary>
        public class VobSubOcrNewFolder
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            public string Message { get; set; }
        }

        /// <summary>
        /// The vob sub ocr set italic factor.
        /// </summary>
        public class VobSubOcrSetItalicFactor
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// The waveform.
        /// </summary>
        public class Waveform
        {
            /// <summary>
            /// Gets or sets the click to add waveform.
            /// </summary>
            public string ClickToAddWaveform { get; set; }

            /// <summary>
            /// Gets or sets the click to add waveform and spectrogram.
            /// </summary>
            public string ClickToAddWaveformAndSpectrogram { get; set; }

            /// <summary>
            /// Gets or sets the seconds.
            /// </summary>
            public string Seconds { get; set; }

            /// <summary>
            /// Gets or sets the zoom in.
            /// </summary>
            public string ZoomIn { get; set; }

            /// <summary>
            /// Gets or sets the zoom out.
            /// </summary>
            public string ZoomOut { get; set; }

            /// <summary>
            /// Gets or sets the add paragraph here.
            /// </summary>
            public string AddParagraphHere { get; set; }

            /// <summary>
            /// Gets or sets the add paragraph here and paste text.
            /// </summary>
            public string AddParagraphHereAndPasteText { get; set; }

            /// <summary>
            /// Gets or sets the focus text box.
            /// </summary>
            public string FocusTextBox { get; set; }

            /// <summary>
            /// Gets or sets the delete paragraph.
            /// </summary>
            public string DeleteParagraph { get; set; }

            /// <summary>
            /// Gets or sets the split.
            /// </summary>
            public string Split { get; set; }

            /// <summary>
            /// Gets or sets the split at cursor.
            /// </summary>
            public string SplitAtCursor { get; set; }

            /// <summary>
            /// Gets or sets the merge with previous.
            /// </summary>
            public string MergeWithPrevious { get; set; }

            /// <summary>
            /// Gets or sets the merge with next.
            /// </summary>
            public string MergeWithNext { get; set; }

            /// <summary>
            /// Gets or sets the play selection.
            /// </summary>
            public string PlaySelection { get; set; }

            /// <summary>
            /// Gets or sets the show waveform and spectrogram.
            /// </summary>
            public string ShowWaveformAndSpectrogram { get; set; }

            /// <summary>
            /// Gets or sets the show waveform only.
            /// </summary>
            public string ShowWaveformOnly { get; set; }

            /// <summary>
            /// Gets or sets the show spectrogram only.
            /// </summary>
            public string ShowSpectrogramOnly { get; set; }

            /// <summary>
            /// Gets or sets the guess time codes.
            /// </summary>
            public string GuessTimeCodes { get; set; }

            /// <summary>
            /// Gets or sets the seek silence.
            /// </summary>
            public string SeekSilence { get; set; }
        }

        /// <summary>
        /// The waveform generate time codes.
        /// </summary>
        public class WaveformGenerateTimeCodes
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the start from.
            /// </summary>
            public string StartFrom { get; set; }

            /// <summary>
            /// Gets or sets the current video position.
            /// </summary>
            public string CurrentVideoPosition { get; set; }

            /// <summary>
            /// Gets or sets the beginning.
            /// </summary>
            public string Beginning { get; set; }

            /// <summary>
            /// Gets or sets the delete lines.
            /// </summary>
            public string DeleteLines { get; set; }

            /// <summary>
            /// Gets or sets the from current video position.
            /// </summary>
            public string FromCurrentVideoPosition { get; set; }

            /// <summary>
            /// Gets or sets the detect options.
            /// </summary>
            public string DetectOptions { get; set; }

            /// <summary>
            /// Gets or sets the scan blocks of ms.
            /// </summary>
            public string ScanBlocksOfMs { get; set; }

            /// <summary>
            /// Gets or sets the block average vol min 1.
            /// </summary>
            public string BlockAverageVolMin1 { get; set; }

            /// <summary>
            /// Gets or sets the block average vol min 2.
            /// </summary>
            public string BlockAverageVolMin2 { get; set; }

            /// <summary>
            /// Gets or sets the block average vol max 1.
            /// </summary>
            public string BlockAverageVolMax1 { get; set; }

            /// <summary>
            /// Gets or sets the block average vol max 2.
            /// </summary>
            public string BlockAverageVolMax2 { get; set; }

            /// <summary>
            /// Gets or sets the split long lines at 1.
            /// </summary>
            public string SplitLongLinesAt1 { get; set; }

            /// <summary>
            /// Gets or sets the split long lines at 2.
            /// </summary>
            public string SplitLongLinesAt2 { get; set; }

            /// <summary>
            /// Gets or sets the other.
            /// </summary>
            public string Other { get; set; }
        }

        /// <summary>
        /// The web vtt new voice.
        /// </summary>
        public class WebVttNewVoice
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the voice name.
            /// </summary>
            public string VoiceName { get; set; }
        }
    }
}