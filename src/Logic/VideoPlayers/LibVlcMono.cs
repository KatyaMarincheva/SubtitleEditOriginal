// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LibVlcMono.cs" company="">
//   
// </copyright>
// <summary>
//   The lib vlc mono.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers
{
    using System;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The lib vlc mono.
    /// </summary>
    internal class LibVlcMono : VideoPlayer, IDisposable
    {
        /// <summary>
        /// The _lib vlc.
        /// </summary>
        private IntPtr _libVlc;

        /// <summary>
        /// The _lib vlc dll.
        /// </summary>
        private IntPtr _libVlcDLL;

        /// <summary>
        /// The _media player.
        /// </summary>
        private IntPtr _mediaPlayer;

        /// <summary>
        /// The _owner control.
        /// </summary>
        private Control _ownerControl;

        /// <summary>
        /// The _parent form.
        /// </summary>
        private Form _parentForm;

        /// <summary>
        /// The _video end timer.
        /// </summary>
        private Timer _videoEndTimer;

        /// <summary>
        /// The _video loaded timer.
        /// </summary>
        private Timer _videoLoadedTimer;

        /// <summary>
        /// Gets the player name.
        /// </summary>
        public override string PlayerName
        {
            get
            {
                return "VLC Lib Mono";
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public override int Volume
        {
            get
            {
                return NativeMethods.libvlc_audio_get_volume(this._mediaPlayer);
            }

            set
            {
                NativeMethods.libvlc_audio_set_volume(this._mediaPlayer, value);
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public override double Duration
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_length(this._mediaPlayer) / TimeCode.BaseUnit;
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public override double CurrentPosition
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_time(this._mediaPlayer) / TimeCode.BaseUnit;
            }

            set
            {
                NativeMethods.libvlc_media_player_set_time(this._mediaPlayer, (long)(value * TimeCode.BaseUnit));
            }
        }

        /// <summary>
        /// Gets or sets the play rate.
        /// </summary>
        public override double PlayRate
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_rate(this._mediaPlayer);
            }

            set
            {
                if (value >= 0 && value <= 2.0)
                {
                    NativeMethods.libvlc_media_player_set_rate(this._mediaPlayer, (float)value);
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
                const int Paused = 4;
                int state = NativeMethods.libvlc_media_player_get_state(this._mediaPlayer);
                return state == Paused;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                const int Playing = 3;
                int state = NativeMethods.libvlc_media_player_get_state(this._mediaPlayer);
                return state == Playing;
            }
        }

        /// <summary>
        /// Gets the audio track count.
        /// </summary>
        public int AudioTrackCount
        {
            get
            {
                return NativeMethods.libvlc_audio_get_track_count(this._mediaPlayer) - 1;
            }
        }

        /// <summary>
        /// Gets or sets the audio track number.
        /// </summary>
        public int AudioTrackNumber
        {
            get
            {
                return NativeMethods.libvlc_audio_get_track(this._mediaPlayer) - 1;
            }

            set
            {
                NativeMethods.libvlc_audio_set_track(this._mediaPlayer, value + 1);
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
            NativeMethods.libvlc_media_player_play(this._mediaPlayer);
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public override void Pause()
        {
            if (!this.IsPaused)
            {
                NativeMethods.libvlc_media_player_pause(this._mediaPlayer);
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public override void Stop()
        {
            NativeMethods.libvlc_media_player_stop(this._mediaPlayer);
        }

        /// <summary>
        /// The make second media player.
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
        /// <returns>
        /// The <see cref="LibVlcMono"/>.
        /// </returns>
        public LibVlcMono MakeSecondMediaPlayer(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded)
        {
            LibVlcMono newVlc = new LibVlcMono();
            newVlc._libVlc = this._libVlc;
            newVlc._libVlcDLL = this._libVlcDLL;
            newVlc._ownerControl = ownerControl;
            if (ownerControl != null)
            {
                newVlc._parentForm = ownerControl.FindForm();
            }

            newVlc.OnVideoLoaded = onVideoLoaded;
            newVlc.OnVideoEnded = onVideoEnded;

            if (!string.IsNullOrEmpty(videoFileName))
            {
                IntPtr media = NativeMethods.libvlc_media_new_path(this._libVlc, Encoding.UTF8.GetBytes(videoFileName + "\0"));
                newVlc._mediaPlayer = NativeMethods.libvlc_media_player_new_from_media(media);
                NativeMethods.libvlc_media_release(media);

                // Linux: libvlc_media_player_set_xdrawable (_mediaPlayer, xdrawable);
                // Mac: libvlc_media_player_set_nsobject (_mediaPlayer, view);
                var ownerHandle = ownerControl == null ? IntPtr.Zero : ownerControl.Handle;
                NativeMethods.libvlc_media_player_set_hwnd(newVlc._mediaPlayer, ownerHandle); // windows

                if (onVideoEnded != null)
                {
                    newVlc._videoEndTimer = new Timer { Interval = 500 };
                    newVlc._videoEndTimer.Tick += this.VideoEndTimerTick;
                    newVlc._videoEndTimer.Start();
                }

                NativeMethods.libvlc_media_player_play(newVlc._mediaPlayer);
                newVlc._videoLoadedTimer = new Timer { Interval = 500 };
                newVlc._videoLoadedTimer.Tick += newVlc.VideoLoadedTimer_Tick;
                newVlc._videoLoadedTimer.Start();
            }

            return newVlc;
        }

        /// <summary>
        /// The video loaded timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoLoadedTimer_Tick(object sender, EventArgs e)
        {
            int i = 0;
            while (!this.IsPlaying && i < 50)
            {
                System.Threading.Thread.Sleep(100);
                i++;
            }

            NativeMethods.libvlc_media_player_pause(this._mediaPlayer);
            this._videoLoadedTimer.Stop();

            if (this.OnVideoLoaded != null)
            {
                this.OnVideoLoaded.Invoke(this._mediaPlayer, new EventArgs());
            }
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
            this._ownerControl = ownerControl;
            if (ownerControl != null)
            {
                this._parentForm = ownerControl.FindForm();
            }

            this.OnVideoLoaded = onVideoLoaded;
            this.OnVideoEnded = onVideoEnded;

            if (!string.IsNullOrEmpty(videoFileName))
            {
                string[] initParameters = { "--no-sub-autodetect-file" }; // , "--no-video-title-show" }; // TODO: Put in options/config file
                this._libVlc = NativeMethods.libvlc_new(initParameters.Length, initParameters);
                IntPtr media = NativeMethods.libvlc_media_new_path(this._libVlc, Encoding.UTF8.GetBytes(videoFileName + "\0"));
                this._mediaPlayer = NativeMethods.libvlc_media_player_new_from_media(media);
                NativeMethods.libvlc_media_release(media);

                // Linux: libvlc_media_player_set_xdrawable (_mediaPlayer, xdrawable);
                // Mac: libvlc_media_player_set_nsobject (_mediaPlayer, view);
                var ownerHandle = ownerControl == null ? IntPtr.Zero : ownerControl.Handle;
                NativeMethods.libvlc_media_player_set_hwnd(this._mediaPlayer, ownerHandle); // windows

                if (onVideoEnded != null)
                {
                    this._videoEndTimer = new Timer { Interval = 500 };
                    this._videoEndTimer.Tick += this.VideoEndTimerTick;
                    this._videoEndTimer.Start();
                }

                NativeMethods.libvlc_media_player_play(this._mediaPlayer);
                this._videoLoadedTimer = new Timer { Interval = 500 };
                this._videoLoadedTimer.Tick += this.VideoLoadedTimer_Tick;
                this._videoLoadedTimer.Start();
            }
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
            const int Ended = 6;
            int state = NativeMethods.libvlc_media_player_get_state(this._mediaPlayer);
            if (state == Ended)
            {
                // hack to make sure VLC is in ready state
                this.Stop();
                this.Play();
                this.Pause();
                this.OnVideoEnded.Invoke(this._mediaPlayer, new EventArgs());
            }
        }

        /// <summary>
        /// The dispose video player.
        /// </summary>
        public override void DisposeVideoPlayer()
        {
            if (this._videoLoadedTimer != null)
            {
                this._videoLoadedTimer.Stop();
            }

            if (this._videoEndTimer != null)
            {
                this._videoEndTimer.Stop();
            }

            System.Threading.ThreadPool.QueueUserWorkItem(this.DisposeVLC, this);
        }

        /// <summary>
        /// The dispose vlc.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        private void DisposeVLC(object player)
        {
            this.ReleaseUnmangedResources();
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
        /// Finalizes an instance of the <see cref="LibVlcMono"/> class. 
        /// </summary>
        ~LibVlcMono()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// The release unmanged resources.
        /// </summary>
        private void ReleaseUnmangedResources()
        {
            try
            {
                if (this._mediaPlayer != IntPtr.Zero)
                {
                    NativeMethods.libvlc_media_player_stop(this._mediaPlayer);
                    NativeMethods.libvlc_media_list_player_release(this._mediaPlayer);
                    this._mediaPlayer = IntPtr.Zero;
                }

                if (this._libVlc != IntPtr.Zero)
                {
                    NativeMethods.libvlc_release(this._libVlc);
                    this._libVlc = IntPtr.Zero;
                }
            }
            catch
            {
            }
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
                if (this._videoLoadedTimer != null)
                {
                    this._videoLoadedTimer.Dispose();
                    this._videoLoadedTimer = null;
                }

                if (this._videoEndTimer != null)
                {
                    this._videoEndTimer.Dispose();
                    this._videoEndTimer = null;
                }
            }

            this.ReleaseUnmangedResources();
        }
    }
}