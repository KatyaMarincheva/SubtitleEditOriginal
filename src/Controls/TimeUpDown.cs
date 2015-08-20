// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeUpDown.cs" company="">
//   
// </copyright>
// <summary>
//   The time up down.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The time up down.
    /// </summary>
    public partial class TimeUpDown : UserControl
    {
        /// <summary>
        /// The time mode.
        /// </summary>
        public enum TimeMode
        {
            /// <summary>
            /// The hhmmssms.
            /// </summary>
            HHMMSSMS, 

            /// <summary>
            /// The hhmmssff.
            /// </summary>
            HHMMSSFF
        }

        /// <summary>
        /// The numeric up down value.
        /// </summary>
        private const int NumericUpDownValue = 50;

        /// <summary>
        /// The _force hhmmssff.
        /// </summary>
        private bool _forceHHMMSSFF = false;

        /// <summary>
        /// The time code changed.
        /// </summary>
        public EventHandler TimeCodeChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeUpDown"/> class.
        /// </summary>
        public TimeUpDown()
        {
            this.InitializeComponent();
            this.numericUpDown1.ValueChanged += this.NumericUpDownValueChanged;
            this.numericUpDown1.Value = NumericUpDownValue;
            this.maskedTextBox1.InsertKeyMode = InsertKeyMode.Overwrite;
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        public TimeMode Mode
        {
            get
            {
                if (this._forceHHMMSSFF)
                {
                    return TimeMode.HHMMSSFF;
                }

                if (Configuration.Settings == null)
                {
                    return TimeMode.HHMMSSMS;
                }

                if (Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    return TimeMode.HHMMSSFF;
                }

                return TimeMode.HHMMSSMS;
            }
        }

        /// <summary>
        /// Gets the masked text box.
        /// </summary>
        public MaskedTextBox MaskedTextBox
        {
            get
            {
                return this.maskedTextBox1;
            }
        }

        /// <summary>
        /// Gets or sets the time code.
        /// </summary>
        public TimeCode TimeCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.maskedTextBox1.Text.Replace(".", string.Empty).Replace(",", string.Empty).Replace(":", string.Empty)))
                {
                    return TimeCode.MaxTime;
                }

                string startTime = this.maskedTextBox1.Text;
                startTime = startTime.Replace(' ', '0');
                char[] splitChars = { ':', ',', '.' };
                if (this.Mode == TimeMode.HHMMSSMS)
                {
                    if (startTime.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                    {
                        startTime += "000";
                    }

                    string[] times = startTime.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                    if (times.Length == 4)
                    {
                        int hours;
                        int.TryParse(times[0], out hours);

                        int minutes;
                        int.TryParse(times[1], out minutes);

                        int seconds;
                        int.TryParse(times[2], out seconds);

                        int milliSeconds;
                        int.TryParse(times[3].PadRight(3, '0'), out milliSeconds);

                        var tc = new TimeCode(hours, minutes, seconds, milliSeconds);
                        if (hours < 0 && tc.TotalMilliseconds > 0)
                        {
                            tc.TotalMilliseconds *= -1;
                        }

                        return tc;
                    }
                }
                else
                {
                    if (startTime.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) || startTime.EndsWith(':'))
                    {
                        startTime += "00";
                    }

                    string[] times = startTime.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                    if (times.Length == 4)
                    {
                        int hours;
                        int.TryParse(times[0], out hours);

                        int minutes;
                        int.TryParse(times[1], out minutes);

                        int seconds;
                        int.TryParse(times[2], out seconds);

                        int milliSeconds;
                        if (int.TryParse(times[3], out milliSeconds))
                        {
                            milliSeconds = Logic.SubtitleFormats.SubtitleFormat.FramesToMillisecondsMax999(milliSeconds);
                        }

                        return new TimeCode(hours, minutes, seconds, milliSeconds);
                    }
                }

                return null;
            }

            set
            {
                if (value == null || value.TotalMilliseconds >= TimeCode.MaxTime.TotalMilliseconds - 0.1)
                {
                    this.maskedTextBox1.Text = string.Empty;
                    return;
                }

                if (this.Mode == TimeMode.HHMMSSMS)
                {
                    if (value.TotalMilliseconds < 0)
                    {
                        this.maskedTextBox1.Mask = "-00:00:00.000";
                    }
                    else
                    {
                        this.maskedTextBox1.Mask = "00:00:00.000";
                    }

                    this.maskedTextBox1.Text = value.ToString();
                }
                else
                {
                    this.maskedTextBox1.Mask = "00:00:00:00";
                    this.maskedTextBox1.Text = value.ToHHMMSSFF();
                }
            }
        }

        /// <summary>
        /// The force hhmmssff.
        /// </summary>
        internal void ForceHHMMSSFF()
        {
            this._forceHHMMSSFF = true;
            this.maskedTextBox1.Mask = "00:00:00:00";
        }

        /// <summary>
        /// The numeric up down value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            double? milliseconds = this.GetTotalMilliseconds();
            if (milliseconds.HasValue)
            {
                if (milliseconds.Value >= TimeCode.MaxTime.TotalMilliseconds - 0.1)
                {
                    milliseconds = 0;
                }

                if (this.Mode == TimeMode.HHMMSSMS)
                {
                    if (this.numericUpDown1.Value > NumericUpDownValue)
                    {
                        this.SetTotalMilliseconds(milliseconds.Value + 100);
                    }
                    else if (this.numericUpDown1.Value < NumericUpDownValue)
                    {
                        this.SetTotalMilliseconds(milliseconds.Value - 100);
                    }
                }
                else
                {
                    if (this.numericUpDown1.Value > NumericUpDownValue)
                    {
                        this.SetTotalMilliseconds(milliseconds.Value + Logic.SubtitleFormats.SubtitleFormat.FramesToMilliseconds(1));
                    }
                    else if (this.numericUpDown1.Value < NumericUpDownValue)
                    {
                        if (milliseconds.Value - 100 > 0)
                        {
                            this.SetTotalMilliseconds(milliseconds.Value - Logic.SubtitleFormats.SubtitleFormat.FramesToMilliseconds(1));
                        }
                        else if (milliseconds.Value > 0)
                        {
                            this.SetTotalMilliseconds(0);
                        }
                    }
                }

                if (this.TimeCodeChanged != null)
                {
                    this.TimeCodeChanged.Invoke(this, e);
                }
            }

            this.numericUpDown1.Value = NumericUpDownValue;
        }

        /// <summary>
        /// The set total milliseconds.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        public void SetTotalMilliseconds(double milliseconds)
        {
            if (this.Mode == TimeMode.HHMMSSMS)
            {
                if (milliseconds < 0)
                {
                    this.maskedTextBox1.Mask = "-00:00:00.000";
                }
                else
                {
                    this.maskedTextBox1.Mask = "00:00:00.000";
                }

                this.maskedTextBox1.Text = new TimeCode(milliseconds).ToString();
            }
            else
            {
                var tc = new TimeCode(milliseconds);
                this.maskedTextBox1.Text = tc.ToString().Substring(0, 9) + string.Format("{0:00}", Logic.SubtitleFormats.SubtitleFormat.MillisecondsToFrames(tc.Milliseconds));
            }
        }

        /// <summary>
        /// The get total milliseconds.
        /// </summary>
        /// <returns>
        /// The <see cref="double?"/>.
        /// </returns>
        public double? GetTotalMilliseconds()
        {
            TimeCode tc = this.TimeCode;
            if (tc != null)
            {
                return tc.TotalMilliseconds;
            }

            return null;
        }

        /// <summary>
        /// The masked text box 1 key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MaskedTextBox1KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.numericUpDown1.UpButton();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.numericUpDown1.DownButton();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (this.TimeCodeChanged != null)
                {
                    this.TimeCodeChanged.Invoke(this, e);
                }

                e.SuppressKeyPress = true;
            }
        }
    }
}