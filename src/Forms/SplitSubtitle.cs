// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitSubtitle.cs" company="">
//   
// </copyright>
// <summary>
//   The split subtitle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The split subtitle.
    /// </summary>
    public sealed partial class SplitSubtitle : Form
    {
        /// <summary>
        /// The _encoding.
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _format.
        /// </summary>
        private SubtitleFormat _format;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitSubtitle"/> class.
        /// </summary>
        public SplitSubtitle()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.SplitSubtitle.Title;
            this.label1.Text = Configuration.Settings.Language.SplitSubtitle.Description1;
            this.label2.Text = Configuration.Settings.Language.SplitSubtitle.Description2;
            this.buttonSplit.Text = Configuration.Settings.Language.SplitSubtitle.Split;
            this.buttonDone.Text = Configuration.Settings.Language.SplitSubtitle.Done;
            this.buttonAdvanced.Text = Configuration.Settings.Language.General.Advanced;
            this.labelHourMinSecMilliSecond.Text = Configuration.Settings.Language.General.HourMinutesSecondsMilliseconds;
            this.buttonGetFrameRate.Left = this.splitTimeUpDownAdjust.Left + this.splitTimeUpDownAdjust.Width;

            this.label2.Top = this.label1.Bottom;
            if (this.Width < this.label1.Right + 5)
            {
                this.Width = this.label1.Right + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonSplit);
        }

        /// <summary>
        /// Gets a value indicating whether show advanced.
        /// </summary>
        public bool ShowAdvanced { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <param name="lengthInSeconds">
        /// The length in seconds.
        /// </param>
        public void Initialize(Subtitle subtitle, string fileName, SubtitleFormat format, Encoding encoding, double lengthInSeconds)
        {
            this.ShowAdvanced = false;
            this._subtitle = subtitle;
            this._fileName = fileName;
            this._format = format;
            this._encoding = encoding;
            this.splitTimeUpDownAdjust.TimeCode = TimeCode.FromSeconds(lengthInSeconds);
        }

        /// <summary>
        /// The form split subtitle_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormSplitSubtitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The get split time.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        private TimeSpan GetSplitTime()
        {
            return this.splitTimeUpDownAdjust.TimeCode.TimeSpan;
        }

        /// <summary>
        /// The button split click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSplitClick(object sender, EventArgs e)
        {
            var splitTime = this.GetSplitTime();
            if (splitTime.TotalSeconds > 0)
            {
                var part1 = new Subtitle();
                var part2 = new Subtitle();
                part1.Header = this._subtitle.Header;
                part2.Header = this._subtitle.Header;

                foreach (Paragraph p in this._subtitle.Paragraphs)
                {
                    if (p.StartTime.TotalMilliseconds < splitTime.TotalMilliseconds)
                    {
                        part1.Paragraphs.Add(new Paragraph(p));
                    }

                    if (p.StartTime.TotalMilliseconds >= splitTime.TotalMilliseconds)
                    {
                        part2.Paragraphs.Add(new Paragraph(p));
                    }
                    else if (p.EndTime.TotalMilliseconds > splitTime.TotalMilliseconds)
                    {
                        part1.Paragraphs[part1.Paragraphs.Count - 1].EndTime = new TimeCode(splitTime.TotalMilliseconds);
                        part2.Paragraphs.Add(new Paragraph(p) { StartTime = new TimeCode(splitTime.TotalMilliseconds) });
                    }
                }

                if (part1.Paragraphs.Count > 0 && part2.Paragraphs.Count > 0)
                {
                    this.SavePart(part1, Configuration.Settings.Language.SplitSubtitle.SavePartOneAs, Configuration.Settings.Language.SplitSubtitle.Part1);

                    part2.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(-splitTime.TotalMilliseconds));
                    part2.Renumber();
                    this.SavePart(part2, Configuration.Settings.Language.SplitSubtitle.SavePartTwoAs, Configuration.Settings.Language.SplitSubtitle.Part2);

                    this.DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show(Configuration.Settings.Language.SplitSubtitle.NothingToSplit);
            }

            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The save part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        private void SavePart(Subtitle part, string title, string name)
        {
            this.saveFileDialog1.Title = title;
            this.saveFileDialog1.FileName = name;
            Utilities.SetSaveDialogFilter(this.saveFileDialog1, this._format);
            this.saveFileDialog1.DefaultExt = "*" + this._format.Extension;
            this.saveFileDialog1.AddExtension = true;

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.saveFileDialog1.FileName;

                try
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    int index = 0;
                    foreach (SubtitleFormat format in SubtitleFormat.AllSubtitleFormats)
                    {
                        if (this.saveFileDialog1.FilterIndex == index + 1)
                        {
                            if (format.IsFrameBased)
                            {
                                part.CalculateFrameNumbersFromTimeCodesNoCheck(Configuration.Settings.General.CurrentFrameRate);
                            }

                            File.WriteAllText(fileName, part.ToText(format), this._encoding);
                        }

                        index++;
                    }
                }
                catch
                {
                    MessageBox.Show(string.Format(Configuration.Settings.Language.SplitSubtitle.UnableToSaveFileX, fileName));
                }
            }
        }

        /// <summary>
        /// The button get frame rate_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGetFrameRate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.openFileDialog1.InitialDirectory) && !string.IsNullOrEmpty(this._fileName))
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this._fileName);
            }

            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenVideoFileTitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(true);
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoInfo info = Utilities.GetVideoInfo(this.openFileDialog1.FileName);
                if (info != null && info.Success)
                {
                    this.splitTimeUpDownAdjust.TimeCode = new TimeCode(info.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// The button advanced_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAdvanced_Click(object sender, EventArgs e)
        {
            this.ShowAdvanced = true;
            this.DialogResult = DialogResult.Cancel;
        }
    }
}