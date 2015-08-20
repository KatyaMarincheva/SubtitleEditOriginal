// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MPlayer.cs" company="">
//   
// </copyright>
// <summary>
//   The m player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// The m player.
    /// </summary>
    public class MPlayer : VideoPlayer, IDisposable
    {
        /// <summary>
        /// The _ended.
        /// </summary>
        private bool _ended = false;

        /// <summary>
        /// The _last length in seconds.
        /// </summary>
        private TimeSpan _lastLengthInSeconds = TimeSpan.FromDays(0);

        /// <summary>
        /// The _length in seconds.
        /// </summary>
        private TimeSpan _lengthInSeconds;

        /// <summary>
        /// The _loaded.
        /// </summary>
        private bool _loaded = false;

        /// <summary>
        /// The _mplayer.
        /// </summary>
        private Process _mplayer;

        /// <summary>
        /// The _pause counts.
        /// </summary>
        private int _pauseCounts = 0;

        /// <summary>
        /// The _paused.
        /// </summary>
        private bool _paused;

        /// <summary>
        /// The _pause position.
        /// </summary>
        private double? _pausePosition = null; // Hack to hold precise seeking when paused

        /// <summary>
        /// The _speed.
        /// </summary>
        private double _speed = 1.0;

        /// <summary>
        /// The _time position.
        /// </summary>
        private double _timePosition;

        /// <summary>
        /// The _timer.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// The _video file name.
        /// </summary>
        private string _videoFileName;

        /// <summary>
        /// The _volume.
        /// </summary>
        private float _volume;

        /// <summary>
        /// The _wait for change.
        /// </summary>
        private bool _waitForChange = false;

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the frames per second.
        /// </summary>
        public float FramesPerSecond { get; private set; }

        /// <summary>
        /// Gets the video format.
        /// </summary>
        public string VideoFormat { get; private set; }

        /// <summary>
        /// Gets the video codec.
        /// </summary>
        public string VideoCodec { get; private set; }

        /// <summary>
        /// Gets the player name.
        /// </summary>
        public override string PlayerName
        {
            get
            {
                return "MPlayer";
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public override int Volume
        {
            get
            {
                return (int)this._volume;
            }

            set
            {
                if (value >= 0 && value <= 100)
                {
                    this._volume = value;
                    this.SetProperty("volume", value.ToString(), true);
                }
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public override double Duration
        {
            get
            {
                return this._lengthInSeconds.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public override double CurrentPosition
        {
            get
            {
                if (this._paused && this._pausePosition != null)
                {
                    if (this._pausePosition < 0)
                    {
                        return 0;
                    }

                    return this._pausePosition.Value;
                }

                return this._timePosition;
            }

            set
            {
                // NOTE: FOR ACCURATE SEARCH USE MPlayer2 - http://www.mplayer2.org/)
                this._timePosition = value;
                if (this.IsPaused && value <= this.Duration)
                {
                    this._pausePosition = value;
                }

                this._mplayer.StandardInput.WriteLine(string.Format("pausing_keep seek {0:0.0} 2", value));
            }
        }

        /// <summary>
        /// Gets or sets the play rate.
        /// </summary>
        public override double PlayRate
        {
            get
            {
                return this._speed;
            }

            set
            {
                if (value >= 0 && value <= 2.0)
                {
                    this._speed = value;
                    this.SetProperty("speed", value.ToString(CultureInfo.InvariantCulture), true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether is paused.
        /// </summary>
        public override bool IsPaused
        {
            get
            {
                return this._paused;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                return !this._paused;
            }
        }

        /// <summary>
        /// Gets the get m player file name.
        /// </summary>
        public static string GetMPlayerFileName
        {
            get
            {
                if (Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac())
                {
                    return "mplayer";
                }

                string fileName = Path.Combine(Configuration.BaseDirectory, "mplayer2.exe");
                if (File.Exists(fileName))
                {
                    return fileName;
                }

                fileName = Path.Combine(Configuration.BaseDirectory, "mplayer.exe");
                if (File.Exists(fileName))
                {
                    return fileName;
                }

                fileName = @"C:\Program Files (x86)\SMPlayer\mplayer\mplayer.exe";
                if (File.Exists(fileName))
                {
                    return fileName;
                }

                fileName = @"C:\Program Files (x86)\mplayer\mplayer.exe";
                if (File.Exists(fileName))
                {
                    return fileName;
                }

                fileName = @"C:\Program Files\mplayer\mplayer.exe";
                if (File.Exists(fileName))
                {
                    return fileName;
                }

                fileName = @"C:\Program Files\SMPlayer\mplayer\mplayer.exe";
                if (File.Exists(fileName))
                {
                    return fileName;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is installed.
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                return GetMPlayerFileName != null;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The play.
        /// </summary>
        public override void Play()
        {
            this._mplayer.StandardInput.WriteLine("pause");
            this._pauseCounts = 0;
            this._paused = false;
            this._pausePosition = null;
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public override void Pause()
        {
            if (!this._paused)
            {
                this._mplayer.StandardInput.WriteLine("pause");
            }

            this._pauseCounts = 0;
            this._paused = true;
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public override void Stop()
        {
            this.CurrentPosition = 0;
            this.Pause();
            this._mplayer.StandardInput.WriteLine("pausing_keep_force seek 0 2");
            this._pauseCounts = 0;
            this._paused = true;
            this._lastLengthInSeconds = this._lengthInSeconds;
            this._pausePosition = null;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="ownerControl">
        /// The owner control.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="onVideoLoaded">
        /// The on video loaded.
        /// </param>
        /// <param name="onVideoEnded">
        /// The on video ended.
        /// </param>
        public override void Initialize(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded)
        {
            this._loaded = false;
            this._videoFileName = videoFileName;
            string mplayerExeName = GetMPlayerFileName;
            if (!string.IsNullOrEmpty(mplayerExeName))
            {
                this._mplayer = new Process();
                this._mplayer.StartInfo.FileName = mplayerExeName;

                // vo options: gl, gl2, directx:noaccel
                if (Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac())
                {
                    this._mplayer.StartInfo.Arguments = "-nofs -quiet -slave -idle -nosub -noautosub -loop 0 -osdlevel 0 -vsync -wid " + ownerControl.Handle.ToInt32() + " \"" + videoFileName + "\" ";
                }
                else
                {
                    this._mplayer.StartInfo.Arguments = "-nofs -quiet -slave -idle -nosub -noautosub -loop 0 -osdlevel 0 -vo direct3d -wid " + (int)ownerControl.Handle + " \"" + videoFileName + "\" ";
                }

                this._mplayer.StartInfo.UseShellExecute = false;
                this._mplayer.StartInfo.RedirectStandardInput = true;
                this._mplayer.StartInfo.RedirectStandardOutput = true;
                this._mplayer.StartInfo.CreateNoWindow = true;
                this._mplayer.OutputDataReceived += this.MPlayerOutputDataReceived;

                try
                {
                    this._mplayer.Start();
                }
                catch
                {
                    MessageBox.Show("Unable to start MPlayer - make sure MPlayer is installed!");
                    throw;
                }

                this._mplayer.StandardInput.NewLine = "\n";
                this._mplayer.BeginOutputReadLine(); // Async reading of output to prevent deadlock

                // static properties
                this.GetProperty("width", true);
                this.GetProperty("height", true);
                this.GetProperty("fps", true);
                this.GetProperty("video_format", true);
                this.GetProperty("video_codec", true);
                this.GetProperty("length", true);

                // semi static variable
                this.GetProperty("volume", true);

                // start timer to collect variable properties
                this._timer = new Timer();
                this._timer.Interval = 1000;
                this._timer.Tick += this.timer_Tick;
                this._timer.Start();

                this.OnVideoLoaded = onVideoLoaded;
                this.OnVideoEnded = onVideoEnded;
            }
        }

        /// <summary>
        /// The timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // variable properties
            this._mplayer.StandardInput.WriteLine("pausing_keep_force get_property time_pos");
            this._mplayer.StandardInput.WriteLine("pausing_keep_force get_property pause");

            if (!this._ended && this.OnVideoEnded != null && this._lengthInSeconds.TotalSeconds == this.Duration)
            {
                // _ended = true;
                // OnVideoEnded.Invoke(this, null);
            }
            else if (this._lengthInSeconds.TotalSeconds < this.Duration)
            {
                this._ended = false;
            }

            if (this.OnVideoLoaded != null && this._loaded)
            {
                this._timer.Stop();
                this._loaded = false;
                this.OnVideoLoaded.Invoke(this, null);
                this._timer.Interval = 100;
                this._timer.Start();
            }

            if (this._lengthInSeconds != this._lastLengthInSeconds)
            {
                this._paused = false;
            }

            this._lastLengthInSeconds = this._lengthInSeconds;
        }

        /// <summary>
        /// The m player output data received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MPlayerOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            Debug.WriteLine("MPlayer: " + e.Data);

            if (e.Data.StartsWith("Playing "))
            {
                this._loaded = true;
                return;
            }

            if (e.Data.StartsWith("Exiting..."))
            {
                this._ended = true;
                if (this._loaded)
                {
                    this._mplayer.StandardInput.WriteLine("loadfile " + this._videoFileName);
                    if (this.OnVideoEnded != null)
                    {
                        this.OnVideoEnded.Invoke(this, null);
                    }
                }

                return;
            }

            int indexOfEqual = e.Data.IndexOf('=');
            if (indexOfEqual > 0 && indexOfEqual + 1 < e.Data.Length && e.Data.StartsWith("ANS_"))
            {
                string code = e.Data.Substring(0, indexOfEqual);
                string value = e.Data.Substring(indexOfEqual + 1);

                switch (code)
                {
                    // Examples:
                    // ANS_time_pos=8.299958, ANS_width=624, ANS_height=352, ANS_fps=23.976025, ANS_video_format=1145656920, ANS_video_format=1145656920, ANS_video_codec=ffodivx,
                    // ANS_length=1351.600213, ANS_volume=100.000000
                    case "ANS_time_pos":
                        this._timePosition = Convert.ToDouble(value.Replace(",", "."), CultureInfo.InvariantCulture);
                        break;
                    case "ANS_width":
                        this.Width = Convert.ToInt32(value);
                        break;
                    case "ANS_height":
                        this.Height = Convert.ToInt32(value);
                        break;
                    case "ANS_fps":
                        double d;
                        if (double.TryParse(value, out d))
                        {
                            this.FramesPerSecond = (float)Convert.ToDouble(value.Replace(",", "."), CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            this.FramesPerSecond = 25.0f;
                        }

                        break;
                    case "ANS_video_format":
                        this.VideoFormat = value;
                        break;
                    case "ANS_video_codec":
                        this.VideoCodec = value;
                        break;
                    case "ANS_length":
                        this._lengthInSeconds = TimeSpan.FromSeconds(Convert.ToDouble(value.Replace(",", "."), CultureInfo.InvariantCulture));
                        break;
                    case "ANS_volume":
                        this._volume = (float)Convert.ToDouble(value.Replace(",", "."), CultureInfo.InvariantCulture);
                        break;
                    case "ANS_pause":
                        if (value == "yes" || value == "1")
                        {
                            this._pauseCounts++;
                        }
                        else
                        {
                            this._pauseCounts--;
                        }

                        if (this._pauseCounts > 3)
                        {
                            this._paused = true;
                        }
                        else if (this._pauseCounts < -3)
                        {
                            this._paused = false;
                            this._pausePosition = null;
                        }
                        else if (Math.Abs(this._pauseCounts) > 10)
                        {
                            this._pauseCounts = 0;
                        }

                        break;
                }

                this._waitForChange = false;
            }
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="keepPause">
        /// The keep pause.
        /// </param>
        private void GetProperty(string propertyName, bool keepPause)
        {
            if (keepPause)
            {
                this._mplayer.StandardInput.WriteLine("pausing_keep get_property " + propertyName);
            }
            else
            {
                this._mplayer.StandardInput.WriteLine("get_property " + propertyName);
            }
        }

        /// <summary>
        /// The set property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="keepPause">
        /// The keep pause.
        /// </param>
        private void SetProperty(string propertyName, string value, bool keepPause)
        {
            if (keepPause)
            {
                this._mplayer.StandardInput.WriteLine("pausing_keep set_property " + propertyName + " " + value);
            }
            else
            {
                this._mplayer.StandardInput.WriteLine("set_property " + propertyName + " " + value);
            }

            this.UglySleep();
        }

        /// <summary>
        /// The ugly sleep.
        /// </summary>
        private void UglySleep()
        {
            this._waitForChange = true;
            int i = 0;

            while (i < 100 && this._waitForChange)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(2);
                i++;
            }

            this._waitForChange = false;
        }

        /// <summary>
        /// The dispose video player.
        /// </summary>
        public override void DisposeVideoPlayer()
        {
            this._timer.Stop();
            if (this._mplayer != null)
            {
                this._mplayer.OutputDataReceived -= this.MPlayerOutputDataReceived;
                this._mplayer.StandardInput.WriteLine("quit");
            }
        }

        /// <summary>
        /// The on video loaded.
        /// </summary>
        public override event EventHandler OnVideoLoaded;

        /// <summary>
        /// The on video ended.
        /// </summary>
        public override event EventHandler OnVideoEnded;

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._mplayer != null)
                {
                    this._mplayer.Dispose();
                    this._mplayer = null;
                }

                if (this._timer != null)
                {
                    this._timer.Dispose();
                    this._timer = null;
                }
            }
        }
    }
}