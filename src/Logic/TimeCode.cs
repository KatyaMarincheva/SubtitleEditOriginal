// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeCode.cs" company="">
//   
// </copyright>
// <summary>
//   The time code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;

    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The time code.
    /// </summary>
    public class TimeCode
    {
        /// <summary>
        /// The base unit.
        /// </summary>
        public const double BaseUnit = 1000.0; // Base unit of time

        /// <summary>
        /// The max time.
        /// </summary>
        public static readonly TimeCode MaxTime = new TimeCode(99, 59, 59, 999);

        /// <summary>
        /// The _total milliseconds.
        /// </summary>
        private double _totalMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeCode"/> class.
        /// </summary>
        /// <param name="timeSpan">
        /// The time span.
        /// </param>
        public TimeCode(TimeSpan timeSpan)
        {
            this._totalMilliseconds = timeSpan.TotalMilliseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeCode"/> class.
        /// </summary>
        /// <param name="totalMilliseconds">
        /// The total milliseconds.
        /// </param>
        public TimeCode(double totalMilliseconds)
        {
            this._totalMilliseconds = totalMilliseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeCode"/> class.
        /// </summary>
        /// <param name="hour">
        /// The hour.
        /// </param>
        /// <param name="minute">
        /// The minute.
        /// </param>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        public TimeCode(int hour, int minute, int seconds, int milliseconds)
        {
            this._totalMilliseconds = hour * 60 * 60 * BaseUnit + minute * 60 * BaseUnit + seconds * BaseUnit + milliseconds;
        }

        /// <summary>
        /// Gets a value indicating whether is max time.
        /// </summary>
        public bool IsMaxTime
        {
            get
            {
                return Math.Abs(this._totalMilliseconds - MaxTime.TotalMilliseconds) < 0.01;
            }
        }

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        public int Hours
        {
            get
            {
                var ts = this.TimeSpan;
                return ts.Hours + ts.Days * 24;
            }

            set
            {
                var ts = this.TimeSpan;
                this._totalMilliseconds = new TimeSpan(0, value, ts.Minutes, ts.Seconds, ts.Milliseconds).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        public int Minutes
        {
            get
            {
                return this.TimeSpan.Minutes;
            }

            set
            {
                var ts = this.TimeSpan;
                this._totalMilliseconds = new TimeSpan(0, ts.Hours, value, ts.Seconds, ts.Milliseconds).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        public int Seconds
        {
            get
            {
                return this.TimeSpan.Seconds;
            }

            set
            {
                var ts = this.TimeSpan;
                this._totalMilliseconds = new TimeSpan(0, ts.Hours, ts.Minutes, value, ts.Milliseconds).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Gets or sets the milliseconds.
        /// </summary>
        public int Milliseconds
        {
            get
            {
                return this.TimeSpan.Milliseconds;
            }

            set
            {
                var ts = this.TimeSpan;
                this._totalMilliseconds = new TimeSpan(0, ts.Hours, ts.Minutes, ts.Seconds, value).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Gets or sets the total milliseconds.
        /// </summary>
        public double TotalMilliseconds
        {
            get
            {
                return this._totalMilliseconds;
            }

            set
            {
                this._totalMilliseconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the total seconds.
        /// </summary>
        public double TotalSeconds
        {
            get
            {
                return this._totalMilliseconds / BaseUnit;
            }

            set
            {
                this._totalMilliseconds = value * BaseUnit;
            }
        }

        /// <summary>
        /// Gets or sets the time span.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get
            {
                return TimeSpan.FromMilliseconds(this._totalMilliseconds);
            }

            set
            {
                this._totalMilliseconds = value.TotalMilliseconds;
            }
        }

        /// <summary>
        /// The from seconds.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        public static TimeCode FromSeconds(double seconds)
        {
            return new TimeCode(seconds * BaseUnit);
        }

        /// <summary>
        /// The parse to milliseconds.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ParseToMilliseconds(string text)
        {
            string[] parts = text.Split(new[] { ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4)
            {
                int hours;
                int minutes;
                int seconds;
                int milliseconds;
                if (int.TryParse(parts[0], out hours) && int.TryParse(parts[1], out minutes) && int.TryParse(parts[2], out seconds) && int.TryParse(parts[3], out milliseconds))
                {
                    return new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalMilliseconds;
                }
            }

            return 0;
        }

        /// <summary>
        /// The parse hhmmssff to milliseconds.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ParseHHMMSSFFToMilliseconds(string text)
        {
            string[] parts = text.Split(new[] { ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4)
            {
                int hours;
                int minutes;
                int seconds;
                int frames;
                if (int.TryParse(parts[0], out hours) && int.TryParse(parts[1], out minutes) && int.TryParse(parts[2], out seconds) && int.TryParse(parts[3], out frames))
                {
                    return new TimeCode(hours, minutes, seconds, SubtitleFormat.FramesToMillisecondsMax999(frames)).TotalMilliseconds;
                }
            }

            return 0;
        }

        /// <summary>
        /// The add time.
        /// </summary>
        /// <param name="hour">
        /// The hour.
        /// </param>
        /// <param name="minutes">
        /// The minutes.
        /// </param>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        public void AddTime(int hour, int minutes, int seconds, int milliseconds)
        {
            this.Hours += hour;
            this.Minutes += minutes;
            this.Seconds += seconds;
            this.Milliseconds += milliseconds;
        }

        /// <summary>
        /// The add time.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        public void AddTime(long milliseconds)
        {
            this._totalMilliseconds += milliseconds;
        }

        /// <summary>
        /// The add time.
        /// </summary>
        /// <param name="timeSpan">
        /// The time span.
        /// </param>
        public void AddTime(TimeSpan timeSpan)
        {
            this._totalMilliseconds += timeSpan.TotalMilliseconds;
        }

        /// <summary>
        /// The add time.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        public void AddTime(double milliseconds)
        {
            this._totalMilliseconds += milliseconds;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var ts = this.TimeSpan;
            string s = string.Format("{0:00}:{1:00}:{2:00},{3:000}", ts.Hours + ts.Days * 24, ts.Minutes, ts.Seconds, ts.Milliseconds);

            if (this.TotalMilliseconds >= 0)
            {
                return s;
            }

            return "-" + s.Replace("-", string.Empty);
        }

        /// <summary>
        /// The to short string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToShortString()
        {
            var ts = this.TimeSpan;
            string s;
            if (ts.Minutes == 0 && ts.Hours == 0 && ts.Days == 0)
            {
                s = string.Format("{0:0},{1:000}", ts.Seconds, ts.Milliseconds);
            }
            else if (ts.Hours == 0 && ts.Days == 0)
            {
                s = string.Format("{0:0}:{1:00},{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            }
            else
            {
                s = string.Format("{0:0}:{1:00}:{2:00},{3:000}", ts.Hours + ts.Days * 24, ts.Minutes, ts.Seconds, ts.Milliseconds);
            }

            if (this.TotalMilliseconds >= 0)
            {
                return s;
            }

            return "-" + s.Replace("-", string.Empty);
        }

        /// <summary>
        /// The to short string hhmmssff.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToShortStringHHMMSSFF()
        {
            var ts = this.TimeSpan;
            if (ts.Minutes == 0 && ts.Hours == 0)
            {
                return string.Format("{0:00}:{1:00}", ts.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(ts.Milliseconds));
            }

            if (ts.Hours == 0)
            {
                return string.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(ts.Milliseconds));
            }

            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(ts.Milliseconds));
        }

        /// <summary>
        /// The to hhmmssff.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToHHMMSSFF()
        {
            var ts = this.TimeSpan;
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(ts.Milliseconds));
        }

        /// <summary>
        /// The to hhmmss period ff.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToHHMMSSPeriodFF()
        {
            var ts = this.TimeSpan;
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(ts.Milliseconds));
        }
    }
}