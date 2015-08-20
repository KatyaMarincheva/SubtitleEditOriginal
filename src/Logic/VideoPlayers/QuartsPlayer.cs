// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuartsPlayer.cs" company="">
//   
// </copyright>
// <summary>
//   The quarts player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

 //http://msdn.microsoft.com/en-us/library/dd375454%28VS.85%29.aspx
//http://msdn.microsoft.com/en-us/library/dd387928%28v=vs.85%29.aspx
namespace Nikse.SubtitleEdit.Logic.VideoPlayers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using QuartzTypeLib;

    /// <summary>
    /// The quarts player.
    /// </summary>
    public class QuartsPlayer : VideoPlayer, IDisposable
    {
        /// <summary>
        /// The _is paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// The _media position.
        /// </summary>
        private IMediaPosition _mediaPosition;

        /// <summary>
        /// The _owner.
        /// </summary>
        private Control _owner;

        /// <summary>
        /// The _quartz filgraph manager.
        /// </summary>
        private FilgraphManager _quartzFilgraphManager;

        /// <summary>
        /// The _quartz video.
        /// </summary>
        private IVideoWindow _quartzVideo;

        /// <summary>
        /// The _source height.
        /// </summary>
        private int _sourceHeight;

        /// <summary>
        /// The _source width.
        /// </summary>
        private int _sourceWidth;

        /// <summary>
        /// The _video end timer.
        /// </summary>
        private Timer _videoEndTimer;

        /// <summary>
        /// The _video loader.
        /// </summary>
        private BackgroundWorker _videoLoader;

        /// <summary>
        /// Gets the player name.
        /// </summary>
        public override string PlayerName
        {
            get
            {
                return "DirectShow";
            }
        }

        /// <summary>
        /// In DirectX -10000 is silent and 0 is full volume.
        /// Also, -3500 to 0 seems to be all you can hear! Not much use for -3500 to -9999...
        /// </summary>
        public override int Volume
        {
            get
            {
                try
                {
                    return ((this._quartzFilgraphManager as IBasicAudio).Volume / 35) + 100;
                }
                catch
                {
                    return 0;
                }
            }

            set
            {
                try
                {
                    if (value == 0)
                    {
                        (this._quartzFilgraphManager as IBasicAudio).Volume = -10000;
                    }
                    else
                    {
                        (this._quartzFilgraphManager as IBasicAudio).Volume = (value - 100) * 35;
                    }
                }
                catch
                {
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
                try
                {
                    return this._mediaPosition.Duration;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public override double CurrentPosition
        {
            get
            {
                try
                {
                    return this._mediaPosition.CurrentPosition;
                }
                catch
                {
                    return 0;
                }
            }

            set
            {
                if (value >= 0 && value <= this.Duration)
                {
                    this._mediaPosition.CurrentPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the play rate.
        /// </summary>
        public override double PlayRate
        {
            get
            {
                return this._mediaPosition.Rate;
            }

            set
            {
                if (value >= 0 && value <= 2.0)
                {
                    this._mediaPosition.Rate = value;
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
                return this._isPaused;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                return !this.IsPaused;
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
        /// The on video loaded.
        /// </summary>
        public override event EventHandler OnVideoLoaded;

        /// <summary>
        /// The on video ended.
        /// </summary>
        public override event EventHandler OnVideoEnded;

        /// <summary>
        /// The play.
        /// </summary>
        public override void Play()
        {
            this._quartzFilgraphManager.Run();
            this._isPaused = false;
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public override void Pause()
        {
            this._quartzFilgraphManager.Pause();
            this._isPaused = true;
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public override void Stop()
        {
            this._quartzFilgraphManager.Stop();
            this._isPaused = true;
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
            const int wsChild = 0x40000000;

            string ext = System.IO.Path.GetExtension(videoFileName).ToLower();
            bool isAudio = ext == ".mp3" || ext == ".wav" || ext == ".wma" || ext == ".m4a";

            this.OnVideoLoaded = onVideoLoaded;
            this.OnVideoEnded = onVideoEnded;

            this.VideoFileName = videoFileName;
            this._owner = ownerControl;
            this._quartzFilgraphManager = new FilgraphManager();
            this._quartzFilgraphManager.RenderFile(this.VideoFileName);

            if (!isAudio)
            {
                this._quartzVideo = this._quartzFilgraphManager as IVideoWindow;
                this._quartzVideo.Owner = (int)ownerControl.Handle;
                this._quartzVideo.SetWindowPosition(0, 0, ownerControl.Width, ownerControl.Height);
                this._quartzVideo.WindowStyle = wsChild;
            }

            // Play();
            if (!isAudio)
            {
                (this._quartzFilgraphManager as IBasicVideo).GetVideoSize(out this._sourceWidth, out this._sourceHeight);
            }

            this._owner.Resize += this.OwnerControlResize;
            this._mediaPosition = (IMediaPosition)this._quartzFilgraphManager;
            if (this.OnVideoLoaded != null)
            {
                this._videoLoader = new BackgroundWorker();
                this._videoLoader.RunWorkerCompleted += this.VideoLoaderRunWorkerCompleted;
                this._videoLoader.DoWork += this.VideoLoaderDoWork;
                this._videoLoader.RunWorkerAsync();
            }

            this.OwnerControlResize(this, null);
            this._videoEndTimer = new Timer { Interval = 500 };
            this._videoEndTimer.Tick += this.VideoEndTimerTick;
            this._videoEndTimer.Start();

            if (!isAudio)
            {
                this._quartzVideo.MessageDrain = (int)ownerControl.Handle;
            }
        }

        /// <summary>
        /// The get video info.
        /// </summary>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        public static VideoInfo GetVideoInfo(string videoFileName)
        {
            var info = new VideoInfo { Success = false };

            try
            {
                var quartzFilgraphManager = new FilgraphManager();
                quartzFilgraphManager.RenderFile(videoFileName);
                int width;
                int height;
                (quartzFilgraphManager as IBasicVideo).GetVideoSize(out width, out height);

                info.Width = width;
                info.Height = height;
                var basicVideo2 = quartzFilgraphManager as IBasicVideo2;
                if (basicVideo2.AvgTimePerFrame > 0)
                {
                    info.FramesPerSecond = 1 / basicVideo2.AvgTimePerFrame;
                }

                info.Success = true;
                var iMediaPosition = quartzFilgraphManager as IMediaPosition;
                info.TotalMilliseconds = iMediaPosition.Duration * 1000;
                info.TotalSeconds = iMediaPosition.Duration;
                info.TotalFrames = info.TotalSeconds * info.FramesPerSecond;
                info.VideoCodec = string.Empty; // TODO: Get real codec names from quartzFilgraphManager.FilterCollection;

                Marshal.ReleaseComObject(quartzFilgraphManager);
            }
            catch
            {
            }

            return info;
        }

        /// <summary>
        /// The video loader do work.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoLoaderDoWork(object sender, DoWorkEventArgs e)
        {
            // int i = 0;
            // while (CurrentPosition < 1 && i < 100)
            // {
            Application.DoEvents();

            // System.Threading.Thread.Sleep(5);
            // i++;
            // }
        }

        /// <summary>
        /// The video loader run worker completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoLoaderRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.OnVideoLoaded != null)
            {
                try
                {
                    this.OnVideoLoaded.Invoke(this._quartzFilgraphManager, new EventArgs());
                }
                catch
                {
                }
            }

            this._videoEndTimer = null;
        }

        /// <summary>
        /// The video end timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoEndTimerTick(object sender, EventArgs e)
        {
            if (this._isPaused == false && this._quartzFilgraphManager != null && this.CurrentPosition >= this.Duration)
            {
                this._isPaused = true;
                if (this.OnVideoEnded != null && this._quartzFilgraphManager != null)
                {
                    this.OnVideoEnded.Invoke(this._quartzFilgraphManager, new EventArgs());
                }
            }
        }

        /// <summary>
        /// The owner control resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OwnerControlResize(object sender, EventArgs e)
        {
            if (this._quartzVideo == null)
            {
                return;
            }

            // calc new scaled size with correct aspect ratio
            float factorX = this._owner.Width / (float)this._sourceWidth;
            float factorY = this._owner.Height / (float)this._sourceHeight;

            if (factorX > factorY)
            {
                this._quartzVideo.Width = (int)(this._sourceWidth * factorY);
                this._quartzVideo.Height = (int)(this._sourceHeight * factorY);
            }
            else
            {
                this._quartzVideo.Width = (int)(this._sourceWidth * factorX);
                this._quartzVideo.Height = (int)(this._sourceHeight * factorX);
            }

            this._quartzVideo.Left = (this._owner.Width - this._quartzVideo.Width) / 2;
            this._quartzVideo.Top = (this._owner.Height - this._quartzVideo.Height) / 2;
        }

        /// <summary>
        /// The dispose video player.
        /// </summary>
        public override void DisposeVideoPlayer()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(this.DisposeQuarts, this._quartzFilgraphManager);
        }

        /// <summary>
        /// The dispose quarts.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        private void DisposeQuarts(object player)
        {
            this.Dispose();
        }

        /// <summary>
        /// The release unmanged resources.
        /// </summary>
        private void ReleaseUnmangedResources()
        {
            try
            {
                if (this._quartzVideo != null)
                {
                    this._quartzVideo.Owner = -1;
                }
            }
            catch
            {
            }

            if (this._quartzFilgraphManager != null)
            {
                try
                {
                    this._quartzFilgraphManager.Stop();
                    Marshal.ReleaseComObject(this._quartzFilgraphManager);
                    this._quartzFilgraphManager = null;
                }
                catch
                {
                }
            }

            this._quartzVideo = null;
        }

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
                if (this._videoEndTimer != null)
                {
                    this._videoEndTimer.Dispose();
                    this._videoEndTimer = null;
                }

                if (this._videoLoader != null)
                {
                    this._videoLoader.Dispose();
                    this._videoLoader = null;
                }
            }

            this.ReleaseUnmangedResources();
        }
    }
}