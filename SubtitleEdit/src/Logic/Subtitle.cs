using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Nikse.SubtitleEdit.Logic.Enums;
using Nikse.SubtitleEdit.Logic.SubtitleFormats;

namespace Nikse.SubtitleEdit.Logic
{
    public class Subtitle
    {
        public const int MaximumHistoryItems = 100;
        private List<Paragraph> paragraphs;
        private List<HistoryItem> history;
        private SubtitleFormat format;
        private bool wasLoadedWithFrameNumbers;

        public Subtitle()
        {
            // Uncomment this to make tests pass :P
            paragraphs = new List<Paragraph>();
            history = new List<HistoryItem>();
            FileName = "Untitled";
        }

        public Subtitle(List<HistoryItem> historyItems)
            : this()
        {
            history = historyItems;
        }

        /// <summary>
        /// Copy constructor (only paragraphs)
        /// </summary>
        /// <param name="subtitle">Subtitle to copy</param>
        public Subtitle(Subtitle subtitle)
            : this()
        {
            if (subtitle == null)
            {
                return;
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                paragraphs.Add(new Paragraph(p));
            }

            wasLoadedWithFrameNumbers = subtitle.WasLoadedWithFrameNumbers;
            Header = subtitle.Header;
            Footer = subtitle.Footer;
        }

        public string Header { get; set; }

        public string Footer { get; set; }

        public string FileName { get; set; }

        public SubtitleFormat OriginalFormat
        {
            get
            {
                return format;
            }
        }

        public List<HistoryItem> HistoryItems
        {
            get { return history; }
        }

        public List<Paragraph> Paragraphs
        {
            get
            {
                return paragraphs;
            }
        }

        public bool CanUndo
        {
            get
            {
                return history.Count > 0;
            }
        }

        public bool WasLoadedWithFrameNumbers
        {
            get
            {
                return wasLoadedWithFrameNumbers;
            }

            set
            {
                wasLoadedWithFrameNumbers = value;
            }
        }

        /// <summary>
        /// Get the paragraph of index, null if out of bounds
        /// </summary>
        /// <param name="index">Index of wanted paragraph</param>
        /// <returns>Paragraph, null if index is index is out of bounds</returns>
        public Paragraph GetParagraphOrDefault(int index)
        {
            if (paragraphs == null || paragraphs.Count <= index || index < 0)
            {
                return null;
            }

            return paragraphs[index];
        }

        public Paragraph GetParagraphOrDefaultById(string id)
        {
            foreach (Paragraph p in paragraphs)
            {
                if (p.ID == id)
                {
                    return p;
                }
            }

            return null;
        }

        public SubtitleFormat ReloadLoadSubtitle(List<string> lines, string fileName)
        {
            Paragraphs.Clear();
            foreach (SubtitleFormat subtitleFormat in SubtitleFormat.AllSubtitleFormats)
            {
                if (subtitleFormat.IsMine(lines, fileName))
                {
                    subtitleFormat.LoadSubtitle(this, lines, fileName);
                    format = subtitleFormat;
                    return subtitleFormat;
                }
            }

            return null;
        }

        public SubtitleFormat LoadSubtitle(string fileName, out Encoding encoding, Encoding useThisEncoding)
        {
            return LoadSubtitle(fileName, out encoding, useThisEncoding, false);
        }

        public SubtitleFormat LoadSubtitle(string fileName, out Encoding encoding, Encoding useThisEncoding, bool batchMode)
        {
            FileName = fileName;

            paragraphs = new List<Paragraph>();

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
                    Header = null;
                    subtitleFormat.BatchMode = batchMode;
                    subtitleFormat.LoadSubtitle(this, lines, fileName);
                    format = subtitleFormat;
                    wasLoadedWithFrameNumbers = format.IsFrameBased;
                    if (wasLoadedWithFrameNumbers)
                    {
                        CalculateTimeCodesFromFrameNumbers(Configuration.Settings.General.CurrentFrameRate);
                    }
                    
                    return subtitleFormat;
                }
            }

            if (useThisEncoding == null)
            {
                return LoadSubtitle(fileName, out encoding, Encoding.Unicode);
            }

            return null;
        }

        public void MakeHistoryForUndo(string description, SubtitleFormat subtitleFormat, DateTime fileModified, Subtitle original, string originalSubtitleFileName, int lineNumber, int linePosition, int linePositionAlternate)
        {
            // don't fill memory with history - use a max rollback points
            if (history.Count > MaximumHistoryItems)
            {
                history.RemoveAt(0);
            }

            history.Add(new HistoryItem(history.Count, this, description, FileName, fileModified, subtitleFormat.FriendlyName, original, originalSubtitleFileName, lineNumber, linePosition, linePositionAlternate));
        }

        public string UndoHistory(int index, out string subtitleFormatFriendlyName, out DateTime fileModified, out Subtitle originalSubtitle, out string originalSubtitleFileName)
        {
            paragraphs.Clear();
            foreach (Paragraph p in history[index].Subtitle.Paragraphs)
            {
                paragraphs.Add(new Paragraph(p));
            }

            subtitleFormatFriendlyName = history[index].SubtitleFormatFriendlyName;
            FileName = history[index].FileName;
            fileModified = history[index].FileModified;
            originalSubtitle = new Subtitle(history[index].OriginalSubtitle);
            originalSubtitleFileName = history[index].OriginalSubtitleFileName;

            return FileName;
        }

        /// <summary>
        /// Creates subtitle as text in it's native format
        /// </summary>
        /// <param name="format">Format to output</param>
        /// <returns>Native format as text string</returns>
        public string ToText(SubtitleFormat format)
        {
            return format.ToText(this, Path.GetFileNameWithoutExtension(FileName));
        }

        public void AddTimeToAllParagraphs(TimeSpan time)
        {
            foreach (Paragraph p in Paragraphs)
            {
                p.StartTime.AddTime(time);
                p.EndTime.AddTime(time);
            }
        }

        /// <summary>
        /// Calculate the time codes from frame number/frame rate
        /// </summary>
        /// <param name="frameRate">Number of frames per second</param>
        /// <returns>True if times could be calculated</returns>
        public bool CalculateTimeCodesFromFrameNumbers(double frameRate)
        {
            if (format == null || format.IsTimeBased)
            {
                return false;
            }

            foreach (Paragraph p in Paragraphs)
            {
                p.CalculateTimeCodesFromFrameNumbers(frameRate);
            }

            return true;
        }

        /// <summary>
        /// Calculate the frame numbers from time codes/frame rate
        /// </summary>
        /// <param name="frameRate"></param>
        /// <returns></returns>
        public bool CalculateFrameNumbersFromTimeCodes(double frameRate)
        {
            if (format == null || format.IsFrameBased)
            {
                return false;
            }

            foreach (Paragraph p in Paragraphs)
            {
                p.CalculateFrameNumbersFromTimeCodes(frameRate);
            }

            FixEqualOrJustOverlappingFrameNumbers();

            return true;
        }

        public void CalculateFrameNumbersFromTimeCodesNoCheck(double frameRate)
        {
            foreach (Paragraph p in Paragraphs)
            {
                p.CalculateFrameNumbersFromTimeCodes(frameRate);
            }

            FixEqualOrJustOverlappingFrameNumbers();
        }

        public void ChangeFrameRate(double oldFrameRate, double newFrameRate)
        {
            foreach (Paragraph p in Paragraphs)
            {
                double startFrame = p.StartTime.TotalMilliseconds / TimeCode.BaseUnit * oldFrameRate;
                double endFrame = p.EndTime.TotalMilliseconds / TimeCode.BaseUnit * oldFrameRate;
                p.StartTime.TotalMilliseconds = startFrame * (TimeCode.BaseUnit / newFrameRate);
                p.EndTime.TotalMilliseconds = endFrame * (TimeCode.BaseUnit / newFrameRate);
                p.CalculateFrameNumbersFromTimeCodes(newFrameRate);
            }
        }

        public void AdjustDisplayTimeUsingPercent(double percent, ListView.SelectedIndexCollection selectedIndexes)
        {
            for (int i = 0; i < paragraphs.Count; i++)
            {
                if (selectedIndexes == null || selectedIndexes.Contains(i))
                {
                    double nextStartMilliseconds = paragraphs[paragraphs.Count - 1].EndTime.TotalMilliseconds + TimeCode.BaseUnit;
                    if (i + 1 < paragraphs.Count)
                    {
                        nextStartMilliseconds = paragraphs[i + 1].StartTime.TotalMilliseconds;
                    }

                    double newEndMilliseconds = paragraphs[i].EndTime.TotalMilliseconds;
                    newEndMilliseconds = paragraphs[i].StartTime.TotalMilliseconds + (((newEndMilliseconds - paragraphs[i].StartTime.TotalMilliseconds) * percent) / 100);
                    if (newEndMilliseconds > nextStartMilliseconds)
                    {
                        newEndMilliseconds = nextStartMilliseconds - 1;
                    }
                    
                    paragraphs[i].EndTime.TotalMilliseconds = newEndMilliseconds;
                }
            }
        }

        public void AdjustDisplayTimeUsingSeconds(double seconds, ListView.SelectedIndexCollection selectedIndexes)
        {
            for (int i = 0; i < paragraphs.Count; i++)
            {
                if (selectedIndexes == null || selectedIndexes.Contains(i))
                {
                    double nextStartMilliseconds = paragraphs[paragraphs.Count - 1].EndTime.TotalMilliseconds + TimeCode.BaseUnit;
                    if (i + 1 < paragraphs.Count)
                    {
                        nextStartMilliseconds = paragraphs[i + 1].StartTime.TotalMilliseconds;
                    }

                    double newEndMilliseconds = paragraphs[i].EndTime.TotalMilliseconds + (seconds * TimeCode.BaseUnit);
                    if (newEndMilliseconds > nextStartMilliseconds)
                    {
                        newEndMilliseconds = nextStartMilliseconds - 1;
                    }

                    if (seconds < 0)
                    {
                        if (paragraphs[i].StartTime.TotalMilliseconds + 100 > newEndMilliseconds)
                        {
                            paragraphs[i].EndTime.TotalMilliseconds = paragraphs[i].StartTime.TotalMilliseconds + 100;
                        }
                        else
                        {
                            paragraphs[i].EndTime.TotalMilliseconds = newEndMilliseconds;
                        }
                    }
                    else
                    {
                        paragraphs[i].EndTime.TotalMilliseconds = newEndMilliseconds;
                    }
                }
            }
        }

        public void RecalculateDisplayTimes(double maxCharactersPerSecond, ListView.SelectedIndexCollection selectedIndexes)
        {
            for (int i = 0; i < paragraphs.Count; i++)
            {
                if (selectedIndexes == null || selectedIndexes.Contains(i))
                {
                    Paragraph p = paragraphs[i];
                    double duration = Utilities.GetOptimalDisplayMilliseconds(p.Text);
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration;
                    while (Utilities.GetCharactersPerSecond(p) > maxCharactersPerSecond)
                    {
                        duration++;
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration;
                    }

                    Paragraph next = GetParagraphOrDefault(i + 1);
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

        public void Renumber(int startNumber = 1)
        {
            foreach (Paragraph p in paragraphs)
            {
                p.Number = startNumber++;
            }
        }

        public int GetIndex(Paragraph p)
        {
            if (p == null)
            {
                return -1;
            }

            int index = paragraphs.IndexOf(p);
            if (index >= 0)
            {
                return index;
            }

            for (int i = 0; i < paragraphs.Count; i++)
            {
                if (p.ID == paragraphs[i].ID)
                {
                    return i;
                }

                if (i < paragraphs.Count - 1 && p.ID == paragraphs[i + 1].ID)
                {
                    return i + 1;
                }

                if (p.StartTime.TotalMilliseconds == paragraphs[i].StartTime.TotalMilliseconds &&
                    p.EndTime.TotalMilliseconds == paragraphs[i].EndTime.TotalMilliseconds)
                {
                    return i;
                }

                if (p.Number == paragraphs[i].Number && (p.StartTime.TotalMilliseconds == paragraphs[i].StartTime.TotalMilliseconds ||
                    p.EndTime.TotalMilliseconds == paragraphs[i].EndTime.TotalMilliseconds))
                {
                    return i;
                }

                if (p.Text == paragraphs[i].Text && (p.StartTime.TotalMilliseconds == paragraphs[i].StartTime.TotalMilliseconds ||
                    p.EndTime.TotalMilliseconds == paragraphs[i].EndTime.TotalMilliseconds))
                {
                    return i;
                }
            }

            return -1;
        }

        public Paragraph GetFirstAlike(Paragraph p)
        {
            foreach (Paragraph item in paragraphs)
            {
                if (p.StartTime.TotalMilliseconds == item.StartTime.TotalMilliseconds &&
                    p.EndTime.TotalMilliseconds == item.EndTime.TotalMilliseconds &&
                    p.Text == item.Text)
                {
                    return item;
                }
            }

            return null;
        }

        public Paragraph GetFirstParagraphByLineNumber(int number)
        {
            foreach (Paragraph p in paragraphs)
            {
                if (p.Number == number)
                {
                    return p;
                }
            }

            return null;
        }

        public int RemoveEmptyLines()
        {
            int count = paragraphs.Count;
            if (count > 0)
            {
                int firstNumber = paragraphs[0].Number;
                for (int i = paragraphs.Count - 1; i >= 0; i--)
                {
                    Paragraph p = paragraphs[i];
                    if (string.IsNullOrWhiteSpace(p.Text))
                    {
                        paragraphs.RemoveAt(i);
                    }
                }

                if (count != paragraphs.Count)
                {
                    Renumber(firstNumber);
                }
            }

            return count - paragraphs.Count;
        }

        /// <summary>
        /// Sort subtitle paragraphs
        /// </summary>
        /// <param name="sortCriteria">Paragraph sort criteria</param>
        public void Sort(SubtitleSortCriteria sortCriteria)
        {
            switch (sortCriteria)
            {
                case SubtitleSortCriteria.Number:
                    paragraphs.Sort((p1, p2) => p1.Number.CompareTo(p2.Number));
                    break;
                case SubtitleSortCriteria.StartTime:
                    paragraphs.Sort((p1, p2) => p1.StartTime.TotalMilliseconds.CompareTo(p2.StartTime.TotalMilliseconds));
                    break;
                case SubtitleSortCriteria.EndTime:
                    paragraphs.Sort((p1, p2) => p1.EndTime.TotalMilliseconds.CompareTo(p2.EndTime.TotalMilliseconds));
                    break;
                case SubtitleSortCriteria.Duration:
                    paragraphs.Sort((p1, p2) => p1.Duration.TotalMilliseconds.CompareTo(p2.Duration.TotalMilliseconds));
                    break;
                case SubtitleSortCriteria.Text:
                    paragraphs.Sort((p1, p2) => string.Compare(p1.Text, p2.Text, StringComparison.Ordinal));
                    break;
                case SubtitleSortCriteria.TextMaxLineLength:
                    paragraphs.Sort((p1, p2) => Utilities.GetMaxLineLength(p1.Text).CompareTo(Utilities.GetMaxLineLength(p2.Text)));
                    break;
                case SubtitleSortCriteria.TextTotalLength:
                    paragraphs.Sort((p1, p2) => p1.Text.Length.CompareTo(p2.Text.Length));
                    break;
                case SubtitleSortCriteria.TextNumberOfLines:
                    paragraphs.Sort((p1, p2) => p1.NumberOfLines.CompareTo(p2.NumberOfLines));
                    break;
                case SubtitleSortCriteria.TextCharactersPerSeconds:
                    paragraphs.Sort((p1, p2) => Utilities.GetCharactersPerSecond(p1).CompareTo(Utilities.GetCharactersPerSecond(p2)));
                    break;
                case SubtitleSortCriteria.WordsPerMinute:
                    paragraphs.Sort((p1, p2) => p1.WordsPerMinute.CompareTo(p2.WordsPerMinute));
                    break;
                case SubtitleSortCriteria.Style:
                    paragraphs.Sort((p1, p2) => string.Compare(p1.Extra, p2.Extra, StringComparison.Ordinal));
                    break;
            }
        }

        public void InsertParagraphInCorrectTimeOrder(Paragraph newParagraph)
        {
            for (int i = 0; i < Paragraphs.Count; i++)
            {
                Paragraph p = Paragraphs[i];
                if (newParagraph.StartTime.TotalMilliseconds < p.StartTime.TotalMilliseconds)
                {
                    Paragraphs.Insert(i, newParagraph);
                    return;
                }
            }

            Paragraphs.Add(newParagraph);
        }

        private void FixEqualOrJustOverlappingFrameNumbers()
        {
            for (int i = 0; i < Paragraphs.Count - 1; i++)
            {
                Paragraph p = Paragraphs[i];
                Paragraph next = GetParagraphOrDefault(i + 1);
                if (next != null && (p.EndFrame == next.StartFrame || p.EndFrame == next.StartFrame + 1))
                {
                    p.EndFrame = next.StartFrame - 1;
                }
            }
        }
    }
}