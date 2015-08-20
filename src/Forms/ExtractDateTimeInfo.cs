// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtractDateTimeInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The extract date time info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4;

    /// <summary>
    /// The extract date time info.
    /// </summary>
    public partial class ExtractDateTimeInfo : PositionAndSizeForm
    {
        /// <summary>
        /// The _formats.
        /// </summary>
        private List<string> _formats = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractDateTimeInfo"/> class.
        /// </summary>
        public ExtractDateTimeInfo()
        {
            this.InitializeComponent();
            this.comboBoxDateTimeFormats.SelectedIndex = 0;
            this.labelVideoFileName.Text = string.Empty;
            this.timeUpDownStartTime.TimeCode = new TimeCode(0, 0, 0, 0);
            this.timeUpDownDuration.TimeCode = new TimeCode(1, 0, 0, 0);
            this.comboBoxDateTimeFormats.Items.Clear();
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            foreach (string format in Configuration.Settings.Tools.GenerateTimeCodePatterns.Split(';'))
            {
                this._formats.Add(format);
                this.comboBoxDateTimeFormats.Items.Add(format);
            }

            if (this._formats.Count > 0)
            {
                this.comboBoxDateTimeFormats.SelectedIndex = 0;
            }

            var l = Configuration.Settings.Language.ExtractDateTimeInfo;
            this.Text = l.Title;
            this.labelChooseVideoFile.Text = l.OpenVideoFile;
            this.labelStartFrom.Text = l.StartFrom;
            this.labelDuration.Text = Configuration.Settings.Language.General.Duration;
            this.labelExample.Text = l.Example;
            this.buttonOK.Text = l.GenerateSubtitle;
            this.labelWriteFormat.Text = l.DateTimeFormat;
        }

        /// <summary>
        /// Gets the date time subtitle.
        /// </summary>
        public Subtitle DateTimeSubtitle { get; private set; }

        /// <summary>
        /// Gets the video file name.
        /// </summary>
        public string VideoFileName { get; private set; }

        /// <summary>
        /// The decode format.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string DecodeFormat(DateTime dateTime, string format)
        {
            try
            {
                var sb = new StringBuilder();
                foreach (string s in format.Replace("<br />", "|").Replace("<BR />", "|").Replace("<br/>", "|").Replace("<BR/>", "|").Replace("<br>", "|").Replace("<BR>", "|").Split('|'))
                {
                    sb.AppendLine(dateTime.ToString(s));
                }

                return sb.ToString().Trim();
            }
            catch
            {
                return "Error";
            }
        }

        /// <summary>
        /// The button open video_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOpenVideo_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.VideoFileName = this.openFileDialog1.FileName;
                this.labelVideoFileName.Text = this.VideoFileName;

                DateTime start;
                double durationInSeconds;
                string ext = Path.GetExtension(this.VideoFileName).ToLower();
                if (ext == ".mp4" || ext == ".m4v" || ext == ".3gp")
                {
                    MP4Parser mp4Parser = new MP4Parser(this.VideoFileName);
                    start = mp4Parser.CreationDate;
                    durationInSeconds = mp4Parser.Duration.TotalSeconds;
                }
                else
                {
                    var fi = new FileInfo(this.VideoFileName);
                    start = fi.CreationTime;
                    VideoInfo vi = Utilities.GetVideoInfo(this.VideoFileName);
                    durationInSeconds = vi.TotalMilliseconds / TimeCode.BaseUnit;
                    if (durationInSeconds < 1)
                    {
                        MessageBox.Show("Unable to get duration");
                        durationInSeconds = 60 * 60;
                    }
                }

                this.dateTimePicker1.Value = start;
                this.timeUpDownStartTime.TimeCode = new TimeCode(start.Hour, start.Minute, start.Second, start.Millisecond);
                this.timeUpDownDuration.TimeCode = TimeCode.FromSeconds(durationInSeconds);
            }
        }

        /// <summary>
        /// The get start date time.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private DateTime GetStartDateTime()
        {
            return new DateTime(this.dateTimePicker1.Value.Year, this.dateTimePicker1.Value.Month, this.dateTimePicker1.Value.Day, this.timeUpDownStartTime.TimeCode.Hours, this.timeUpDownStartTime.TimeCode.Minutes, this.timeUpDownStartTime.TimeCode.Seconds, this.timeUpDownStartTime.TimeCode.Milliseconds);
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
            this.DateTimeSubtitle = new Subtitle();
            DateTime start = this.GetStartDateTime();
            double durationInSeconds = this.timeUpDownDuration.TimeCode.TotalSeconds;
            for (int i = 0; i < durationInSeconds; i++)
            {
                Paragraph p = new Paragraph();
                p.Text = this.FormatDateTime(start);
                start = start.AddSeconds(1);
                p.StartTime = TimeCode.FromSeconds(i);
                p.EndTime = TimeCode.FromSeconds(i + 0.999);
                this.DateTimeSubtitle.Paragraphs.Add(p);
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The format date time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FormatDateTime(DateTime dt)
        {
            return DecodeFormat(dt, this.comboBoxDateTimeFormats.Text);
        }

        /// <summary>
        /// The combo box date time formats_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxDateTimeFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxExample.Text = this.FormatDateTime(DateTime.Now);
        }

        /// <summary>
        /// The combo box date time formats_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxDateTimeFormats_TextChanged(object sender, EventArgs e)
        {
            this.textBoxExample.Text = this.FormatDateTime(DateTime.Now);
        }

        /// <summary>
        /// The extract date time info_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtractDateTimeInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}