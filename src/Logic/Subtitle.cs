// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subtitle.cs" company="">
//   
// </copyright>
// <summary>
//   The subtitle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic.Enums;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The subtitle.
    /// </summary>
    public class Subtitle
    {
        /// <summary>
        /// The maximum history items.
        /// </summary>
        public const int MaximumHistoryItems = 100;

        /// <summary>
        /// The format.
        /// </summary>
        private SubtitleFormat format;

        /// <summary>
        /// The history.
        /// </summary>
        private List<HistoryItem> history;

        /// <summary>
        /// The paragraphs.
        /// </summary>
        private List<Paragraph> paragraphs;

        /// <summary>
        /// The was loaded with frame numbers.
        /// </summary>
        private bool wasLoadedWithFrameNumbers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle"/> class.
        /// </summary>
        public Subtitle()
        {
            // Uncomment this to make tests pass :P
            this.paragraphs = new List<Paragraph>();
            this.history = new List<HistoryItem>();
            this.FileName = "Untitled";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle"/> class.
        /// </summary>
        /// <param name="historyItems">
        /// The history items.
        /// </param>
        public Subtitle(List<HistoryItem> historyItems)
            : this()
        {
            this.history = historyItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle"/> class. 
        /// Copy constructor (only paragraphs)
        /// </summary>
        /// <param name="subtitle">
        /// Subtitle to copy
        /// </param>
        public Subtitle(Subtitle subtitle)
            : this()
        {
            if (subtitle == null)
            {
                return;
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                this.paragraphs.Add(new Paragraph(p));
            }

            this.wasLoadedWithFrameNumbers = subtitle.WasLoadedWithFrameNumbers;
            this.Header = subtitle.Header;
            this.Footer = subtitle.Footer;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the original format.
        /// </summary>
        public SubtitleFormat OriginalFormat
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// Gets the history items.
        /// </summary>
        public List<HistoryItem> HistoryItems
        {
            get
            {
                return this.history;
            }
        }

        /// <summary>
        /// Gets the paragraphs.
        /// </summary>
        public List<Paragraph> Paragraphs
        {
            get
            {
                return this.paragraphs;
            }
        }

        /// <summary>
        /// Gets a value indicating whether can undo.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return this.history.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether was loaded with frame numbers.
        /// </summary>
        public bool WasLoadedWithFrameNumbers
        {
            get
            {
                return this.wasLoadedWithFrameNumbers;
            }

            set
            {
                this.wasLoadedWithFrameNumbers = value;
            }
        }

        /// <summary>
        /// Get the paragraph of index, null if out of bounds
        /// </summary>
        /// <param name="index">
        /// Index of wanted paragraph
        /// </param>
        /// <returns>
        /// Paragraph, null if index is index is out of bounds
        /// </returns>
        public Paragraph GetParagraphOrDefault(int index)
        {
            if (this.paragraphs == null || this.paragraphs.Count <= index || index < 0)
            {
                return null;
            }

            return this.paragraphs[index];
        }

        /// <summary>
        /// The get paragraph or default by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        public Paragraph GetParagraphOrDefaultById(string id)
        {
            foreach (Paragraph p in this.paragraphs)
            {
                if (p.ID == id)
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        /// The reload load subtitle.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="SubtitleFormat"/>.
        /// </returns>
        public SubtitleFormat ReloadLoadSubtitle(List<string> lines, string fileName)
        {
            this.Paragraphs.Clear();
            foreach (SubtitleFormat subtitleFormat in SubtitleFormat.AllSubtitleFormats)
            {
                if (subtitleFormat.IsMine(lines, fileName))
                {
                    subtitleFormat.LoadSubtitle(this, lines, fileName);
                    this.format = subtitleFormat;
                    return subtitleFormat;
                }
            }

            return null;
        }

        /// <summary>
        /// The load subtitle.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <param name="useThisEncoding">
        /// The use this encoding.
        /// </param>
        /// <returns>
        /// The <see cref="SubtitleFormat"/>.
        /// </returns>
        public SubtitleFormat LoadSubtitle(string fileName, out Encoding encoding, Encoding useThisEncoding)
        {
            return this.LoadSubtitle(fileName, out encoding, useThisEncoding, false);
        }

        /// <summary>
        /// The load subtitle.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <param name="useThisEncoding">
        /// The use this encoding.
        /// </param>
        /// <param name="batchMode">
        /// The batch mode.
        /// </param>
        /// <returns>
        /// The <see cref="SubtitleFormat"/>.
        /// </returns>
        public SubtitleFormat LoadSubtitle(string fileName, out Encoding encoding, Encoding useThisEncoding, bool batchMode)
        {
            this.FileName = fileName;

            this.paragraphs = new List<Paragraph>();

            var lines = new List<string>();
            StreamReader sr;
            if (useThisEncoding != null)
            {
                try
                {
                    sr = new StreamReader(fileName, useThisEncoding);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    encoding = Encoding.UTF8;
                    return null;
                }
            }
            else
            {
                try
                {
                    sr = new StreamReader(fileName, Utilities.GetEncodingFromFile(fileName), true);
                }
                catch
                {
                    try
                    {
                        Stream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        sr = new StreamReader(fs);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        encoding = Encoding.UTF8;
                        return null;
                    }
                }
            }

            encoding = sr.CurrentEncoding;
            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }

            sr.Close();

            foreach (SubtitleFormat subtitleFormat in SubtitleFormat.AllSubtitleFormats)
            {
                if (subtitleFormat.IsMine(lines, fileName))
                {
                    this.Header = null;
                    subtitleFormat.BatchMode = batchMode;
                    subtitleFormat.LoadSubtitle(this, lines, fileName);
                    this.format = subtitleFormat;
                    this.wasLoadedWithFrameNumbers = this.format.IsFrameBased;
                    if (this.wasLoadedWithFrameNumbers)
                    {
                        this.CalculateTimeCodesFromFrameNumbers(Configuration.Settings.General.CurrentFrameRate);
                    }

                    return subtitleFormat;
                }
            }

            if (useThisEncoding == null)
            {
                return this.LoadSubtitle(fileName, out encoding, Encoding.Unicode);
            }

            return null;
        }

        /// <summary>
        /// The make history for undo.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="subtitleFormat">
        /// The subtitle format.
        /// </param>
        /// <param name="fileModified">
        /// The file modified.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <param name="originalSubtitleFileName">
        /// The original subtitle file name.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="linePosition">
        /// The line position.
        /// </param>
        /// <param name="linePositionAlternate">
        /// The line position alternate.
        /// </param>
        public void MakeHistoryForUndo(string description, SubtitleFormat subtitleFormat, DateTime fileModified, Subtitle original, string originalSubtitleFileName, int lineNumber, int linePosition, int linePositionAlternate)
        {
            // don't fill memory with history - use a max rollback points
            if (this.history.Count > MaximumHistoryItems)
            {
                this.history.RemoveAt(0);
            }

            this.history.Add(new HistoryItem(this.history.Count, this, description, this.FileName, fileModified, subtitleFormat.FriendlyName, original, originalSubtitleFileName, lineNumber, linePosition, linePositionAlternate));
        }

        /// <summary>
        /// The undo history.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="subtitleFormatFriendlyName">
        /// The subtitle format friendly name.
        /// </param>
        /// <param name="fileModified">
        /// The file modified.
        /// </param>
        /// <param name="originalSubtitle">
        /// The original subtitle.
        /// </param>
        /// <param name="originalSubtitleFileName">
        /// The original subtitle file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string UndoHistory(int index, out string subtitleFormatFriendlyName, out DateTime fileModified, out Subtitle originalSubtitle, out string originalSubtitleFileName)
        {
            this.paragraphs.Clear();
            foreach (Paragraph p in this.history[index].Subtitle.Paragraphs)
            {
                this.paragraphs.Add(new Paragraph(p));
            }

            subtitleFormatFriendlyName = this.history[index].SubtitleFormatFriendlyName;
            this.FileName = this.history[index].FileName;
            fileModified = this.history[index].FileModified;
            originalSubtitle = new Subtitle(this.history[index].OriginalSubtitle);
            originalSubtitleFileName = this.history[index].OriginalSubtitleFileName;

            return this.FileName;
        }

        /// <summary>
        /// Creates subtitle as text in it's native format
        /// </summary>
        /// <param name="format">
        /// Format to output
        /// </param>
        /// <returns>
        /// Native format as text string
        /// </returns>
        public string ToText(SubtitleFormat format)
        {
            return format.ToText(this, Path.GetFileNameWithoutExtension(this.FileName));
        }

        /// <summary>
        /// The add time to all paragraphs.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        public void AddTimeToAllParagraphs(TimeSpan time)
        {
            foreach (Paragraph p in this.Paragraphs)
            {
                p.StartTime.AddTime(time);
                p.EndTime.AddTime(time);
            }
        }

        /// <summary>
        /// Calculate the time codes from frame number/frame rate
        /// </summary>
        /// <param name="frameRate">
        /// Number of frames per second
        /// </param>
        /// <returns>
        /// True if times could be calculated
        /// </returns>
        public bool CalculateTimeCodesFromFrameNumbers(double frameRate)
        {
            if (this.format == null || this.format.IsTimeBased)
            {
                return false;
            }

            foreach (Paragraph p in this.Paragraphs)
            {
                p.CalculateTimeCodesFromFrameNumbers(frameRate);
            }

            return true;
        }

        /// <summary>
        /// Calculate the frame numbers from time codes/frame rate
        /// </summary>
        /// <param name="frameRate">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CalculateFrameNumbersFromTimeCodes(double frameRate)
        {
            if (this.format == null || this.format.IsFrameBased)
            {
                return false;
            }

            foreach (Paragraph p in this.Paragraphs)
            {
                p.CalculateFrameNumbersFromTimeCodes(frameRate);
            }

            this.FixEqualOrJustOverlappingFrameNumbers();

            return true;
        }

        /// <summary>
        /// The calculate frame numbers from time codes no check.
        /// </summary>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        public void CalculateFrameNumbersFromTimeCodesNoCheck(double frameRate)
        {
            foreach (Paragraph p in this.Paragraphs)
            {
                p.CalculateFrameNumbersFromTimeCodes(frameRate);
            }

            this.FixEqualOrJustOverlappingFrameNumbers();
        }

        /// <summary>
        /// The change frame rate.
        /// </summary>
        /// <param name="oldFrameRate">
        /// The old frame rate.
        /// </param>
        /// <param name="newFrameRate">
        /// The new frame rate.
        /// </param>
        public void ChangeFrameRate(double oldFrameRate, double newFrameRate)
        {
            foreach (Paragraph p in this.Paragraphs)
            {
                double startFrame = p.StartTime.TotalMilliseconds / TimeCode.BaseUnit * oldFrameRate;
                double endFrame = p.EndTime.TotalMilliseconds / TimeCode.BaseUnit * oldFrameRate;
                p.StartTime.TotalMilliseconds = startFrame * (TimeCode.BaseUnit / newFrameRate);
                p.EndTime.TotalMilliseconds = endFrame * (TimeCode.BaseUnit / newFrameRate);
                p.CalculateFrameNumbersFromTimeCodes(newFrameRate);
            }
        }

        /// <summary>
        /// The adjust display time using percent.
        /// </summary>
        /// <param name="percent">
        /// The percent.
        /// </param>
        /// <param name="selectedIndexes">
        /// The selected indexes.
        /// </param>
        public void AdjustDisplayTimeUsingPercent(double percent, ListView.SelectedIndexCollection selectedIndexes)
        {
            for (int i = 0; i < this.paragraphs.Count; i++)
            {
                if (selectedIndexes == null || selectedIndexes.Contains(i))
                {
                    double nextStartMilliseconds = this.paragraphs[this.paragraphs.Count - 1].EndTime.TotalMilliseconds + TimeCode.BaseUnit;
                    if (i + 1 < this.paragraphs.Count)
                    {
                        nextStartMilliseconds = this.paragraphs[i + 1].StartTime.TotalMilliseconds;
                    }

                    double newEndMilliseconds = this.paragraphs[i].EndTime.TotalMilliseconds;
                    newEndMilliseconds = this.paragraphs[i].StartTime.TotalMilliseconds + (((newEndMilliseconds - this.paragraphs[i].StartTime.TotalMilliseconds) * percent) / 100);
                    if (newEndMilliseconds > nextStartMilliseconds)
                    {
                        newEndMilliseconds = nextStartMilliseconds - 1;
                    }

                    this.paragraphs[i].EndTime.TotalMilliseconds = newEndMilliseconds;
                }
            }
        }

        /// <summary>
        /// The adjust display time using seconds.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="selectedIndexes">
        /// The selected indexes.
        /// </param>
        public void AdjustDisplayTimeUsingSeconds(double seconds, ListView.SelectedIndexCollection selectedIndexes)
        {
            for (int i = 0; i < this.paragraphs.Count; i++)
            {
                if (selectedIndexes == null || selectedIndexes.Contains(i))
                {
                    double nextStartMilliseconds = this.paragraphs[this.paragraphs.Count - 1].EndTime.TotalMilliseconds + TimeCode.BaseUnit;
                    if (i + 1 < this.paragraphs.Count)
                    {
                        nextStartMilliseconds = this.paragraphs[i + 1].StartTime.TotalMilliseconds;
                    }

                    double newEndMilliseconds = this.paragraphs[i].EndTime.TotalMilliseconds + (seconds * TimeCode.BaseUnit);
                    if (newEndMilliseconds > nextStartMilliseconds)
                    {
                        newEndMilliseconds = nextStartMilliseconds - 1;
                    }

                    if (seconds < 0)
                    {
                        if (this.paragraphs[i].StartTime.TotalMilliseconds + 100 > newEndMilliseconds)
                        {
                            this.paragraphs[i].EndTime.TotalMilliseconds = this.paragraphs[i].StartTime.TotalMilliseconds + 100;
                        }
                        else
                        {
                            this.paragraphs[i].EndTime.TotalMilliseconds = newEndMilliseconds;
                        }
                    }
                    else
                    {
                        this.paragraphs[i].EndTime.TotalMilliseconds = newEndMilliseconds;
                    }
                }
            }
        }

        /// <summary>
        /// The recalculate display times.
        /// </summary>
        /// <param name="maxCharactersPerSecond">
        /// The max characters per second.
        /// </param>
        /// <param name="selectedIndexes">
        /// The selected indexes.
        /// </param>
        public void RecalculateDisplayTimes(double maxCharactersPerSecond, ListView.SelectedIndexCollection selectedIndexes)
        {
            for (int i = 0; i < this.paragraphs.Count; i++)
            {
                if (selectedIndexes == null || selectedIndexes.Contains(i))
                {
                    Paragraph p = this.paragraphs[i];
                    double duration = Utilities.GetOptimalDisplayMilliseconds(p.Text);
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration;
                    while (Utilities.GetCharactersPerSecond(p) > maxCharactersPerSecond)
                    {
                        duration++;
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration;
                    }

                    Paragraph next = this.GetParagraphOrDefault(i + 1);
                    if (next != null && p.StartTime.TotalMilliseconds + duration + Configuration.Settings.General.MinimumMillisecondsBetweenLines > next.StartTime.TotalMilliseconds)
                    {
                        p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                        if (p.Duration.TotalMilliseconds <= 0)
                        {
                            p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The renumber.
        /// </summary>
        /// <param name="startNumber">
        /// The start number.
        /// </param>
        public void Renumber(int startNumber = 1)
        {
            foreach (Paragraph p in this.paragraphs)
            {
                p.Number = startNumber++;
            }
        }

        /// <summary>
        /// The get index.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetIndex(Paragraph p)
        {
            if (p == null)
            {
                return -1;
            }

            int index = this.paragraphs.IndexOf(p);
            if (index >= 0)
            {
                return index;
            }

            for (int i = 0; i < this.paragraphs.Count; i++)
            {
                if (p.ID == this.paragraphs[i].ID)
                {
                    return i;
                }

                if (i < this.paragraphs.Count - 1 && p.ID == this.paragraphs[i + 1].ID)
                {
                    return i + 1;
                }

                if (p.StartTime.TotalMilliseconds == this.paragraphs[i].StartTime.TotalMilliseconds && p.EndTime.TotalMilliseconds == this.paragraphs[i].EndTime.TotalMilliseconds)
                {
                    return i;
                }

                if (p.Number == this.paragraphs[i].Number && (p.StartTime.TotalMilliseconds == this.paragraphs[i].StartTime.TotalMilliseconds || p.EndTime.TotalMilliseconds == this.paragraphs[i].EndTime.TotalMilliseconds))
                {
                    return i;
                }

                if (p.Text == this.paragraphs[i].Text && (p.StartTime.TotalMilliseconds == this.paragraphs[i].StartTime.TotalMilliseconds || p.EndTime.TotalMilliseconds == this.paragraphs[i].EndTime.TotalMilliseconds))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The get first alike.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        public Paragraph GetFirstAlike(Paragraph p)
        {
            foreach (Paragraph item in this.paragraphs)
            {
                if (p.StartTime.TotalMilliseconds == item.StartTime.TotalMilliseconds && p.EndTime.TotalMilliseconds == item.EndTime.TotalMilliseconds && p.Text == item.Text)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// The get first paragraph by line number.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        public Paragraph GetFirstParagraphByLineNumber(int number)
        {
            foreach (Paragraph p in this.paragraphs)
            {
                if (p.Number == number)
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        /// The remove empty lines.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int RemoveEmptyLines()
        {
            int count = this.paragraphs.Count;
            if (count > 0)
            {
                int firstNumber = this.paragraphs[0].Number;
                for (int i = this.paragraphs.Count - 1; i >= 0; i--)
                {
                    Paragraph p = this.paragraphs[i];
                    if (string.IsNullOrWhiteSpace(p.Text))
                    {
                        this.paragraphs.RemoveAt(i);
                    }
                }

                if (count != this.paragraphs.Count)
                {
                    this.Renumber(firstNumber);
                }
            }

            return count - this.paragraphs.Count;
        }

        /// <summary>
        /// Sort subtitle paragraphs
        /// </summary>
        /// <param name="sortCriteria">
        /// Paragraph sort criteria
        /// </param>
        public void Sort(SubtitleSortCriteria sortCriteria)
        {
            switch (sortCriteria)
            {
                case SubtitleSortCriteria.Number:
                    this.paragraphs.Sort((p1, p2) => p1.Number.CompareTo(p2.Number));
                    break;
                case SubtitleSortCriteria.StartTime:
                    this.paragraphs.Sort((p1, p2) => p1.StartTime.TotalMilliseconds.CompareTo(p2.StartTime.TotalMilliseconds));
                    break;
                case SubtitleSortCriteria.EndTime:
                    this.paragraphs.Sort((p1, p2) => p1.EndTime.TotalMilliseconds.CompareTo(p2.EndTime.TotalMilliseconds));
                    break;
                case SubtitleSortCriteria.Duration:
                    this.paragraphs.Sort((p1, p2) => p1.Duration.TotalMilliseconds.CompareTo(p2.Duration.TotalMilliseconds));
                    break;
                case SubtitleSortCriteria.Text:
                    this.paragraphs.Sort((p1, p2) => string.Compare(p1.Text, p2.Text, StringComparison.Ordinal));
                    break;
                case SubtitleSortCriteria.TextMaxLineLength:
                    this.paragraphs.Sort((p1, p2) => Utilities.GetMaxLineLength(p1.Text).CompareTo(Utilities.GetMaxLineLength(p2.Text)));
                    break;
                case SubtitleSortCriteria.TextTotalLength:
                    this.paragraphs.Sort((p1, p2) => p1.Text.Length.CompareTo(p2.Text.Length));
                    break;
                case SubtitleSortCriteria.TextNumberOfLines:
                    this.paragraphs.Sort((p1, p2) => p1.NumberOfLines.CompareTo(p2.NumberOfLines));
                    break;
                case SubtitleSortCriteria.TextCharactersPerSeconds:
                    this.paragraphs.Sort((p1, p2) => Utilities.GetCharactersPerSecond(p1).CompareTo(Utilities.GetCharactersPerSecond(p2)));
                    break;
                case SubtitleSortCriteria.WordsPerMinute:
                    this.paragraphs.Sort((p1, p2) => p1.WordsPerMinute.CompareTo(p2.WordsPerMinute));
                    break;
                case SubtitleSortCriteria.Style:
                    this.paragraphs.Sort((p1, p2) => string.Compare(p1.Extra, p2.Extra, StringComparison.Ordinal));
                    break;
            }
        }

        /// <summary>
        /// The insert paragraph in correct time order.
        /// </summary>
        /// <param name="newParagraph">
        /// The new paragraph.
        /// </param>
        public void InsertParagraphInCorrectTimeOrder(Paragraph newParagraph)
        {
            for (int i = 0; i < this.Paragraphs.Count; i++)
            {
                Paragraph p = this.Paragraphs[i];
                if (newParagraph.StartTime.TotalMilliseconds < p.StartTime.TotalMilliseconds)
                {
                    this.Paragraphs.Insert(i, newParagraph);
                    return;
                }
            }

            this.Paragraphs.Add(newParagraph);
        }

        /// <summary>
        /// The fix equal or just overlapping frame numbers.
        /// </summary>
        private void FixEqualOrJustOverlappingFrameNumbers()
        {
            for (int i = 0; i < this.Paragraphs.Count - 1; i++)
            {
                Paragraph p = this.Paragraphs[i];
                Paragraph next = this.GetParagraphOrDefault(i + 1);
                if (next != null && (p.EndFrame == next.StartFrame || p.EndFrame == next.StartFrame + 1))
                {
                    p.EndFrame = next.StartFrame - 1;
                }
            }
        }
    }
}