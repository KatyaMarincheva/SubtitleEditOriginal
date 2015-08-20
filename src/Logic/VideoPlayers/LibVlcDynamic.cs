// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LibVlcDynamic.cs" company="">
//   
// </copyright>
// <summary>
//   The lib vlc dynamic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The lib vlc dynamic.
    /// </summary>
    public class LibVlcDynamic : VideoPlayer, IDisposable
    {
        /// <summary>
        /// Callback prototype to display a picture. When the video frame needs to be shown, as determined by the media playback clock, the display callback is invoked.
        /// </summary>
        /// <param name="opaque">Private pointer as passed to SetCallbacks()</param>
        /// <param name="picture">Private pointer returned from the LockCallback callback</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisplayCallbackDelegate(IntPtr opaque, IntPtr picture);

        // [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // public unsafe delegate void* LockEventHandler(void* opaque, void** plane);

        // [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // public unsafe delegate void UnlockEventHandler(void* opaque, void* picture, void** plane);

        /// <summary>
        /// Callback prototype to allocate and lock a picture buffer. Whenever a new video frame needs to be decoded, the lock callback is invoked. Depending on the video chroma, one or three pixel planes of adequate dimensions must be returned via the second parameter. Those planes must be aligned on 32-bytes boundaries.
        /// </summary>
        /// <param name="opaque">Private pointer as passed to SetCallbacks()</param>
        /// <param name="planes">Planes start address of the pixel planes (LibVLC allocates the array of void pointers, this callback must initialize the array)</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LockCallbackDelegate(IntPtr opaque, ref IntPtr planes);

        /// <summary>
        /// Callback prototype to unlock a picture buffer. When the video frame decoding is complete, the unlock callback is invoked. This callback might not be needed at all. It is only an indication that the application can now read the pixel values if it needs to.
        /// </summary>
        /// <param name="opaque">Private pointer as passed to SetCallbacks()</param>
        /// <param name="picture">Private pointer returned from the LockCallback callback</param>
        /// <param name="planes">Pixel planes as defined by the @ref libvlc_video_lock_cb callback (this parameter is only for convenience)</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnlockCallbackDelegate(IntPtr opaque, IntPtr picture, ref IntPtr planes);

        /// <summary>
        /// The _lib vlc.
        /// </summary>
        private IntPtr _libVlc;

        /// <summary>
        /// The _libvlc_audio_get_delay.
        /// </summary>
        private libvlc_audio_get_delay _libvlc_audio_get_delay;

        /// <summary>
        /// The _libvlc_audio_get_track.
        /// </summary>
        private libvlc_audio_get_track _libvlc_audio_get_track;

        /// <summary>
        /// The _libvlc_audio_get_track_count.
        /// </summary>
        private libvlc_audio_get_track_count _libvlc_audio_get_track_count;

        /// <summary>
        /// The _libvlc_audio_get_volume.
        /// </summary>
        private libvlc_audio_get_volume _libvlc_audio_get_volume;

        /// <summary>
        /// The _libvlc_audio_set_track.
        /// </summary>
        private libvlc_audio_set_track _libvlc_audio_set_track;

        /// <summary>
        /// The _libvlc_audio_set_volume.
        /// </summary>
        private libvlc_audio_set_volume _libvlc_audio_set_volume;

        /// <summary>
        /// The _libvlc_media_new_path.
        /// </summary>
        private libvlc_media_new_path _libvlc_media_new_path;

        /// <summary>
        /// The _libvlc_media_player_get_length.
        /// </summary>
        private libvlc_media_player_get_length _libvlc_media_player_get_length;

        /// <summary>
        /// The _libvlc_media_player_get_rate.
        /// </summary>
        private libvlc_media_player_get_rate _libvlc_media_player_get_rate;

        /// <summary>
        /// The _libvlc_media_player_get_state.
        /// </summary>
        private libvlc_media_player_get_state _libvlc_media_player_get_state;

        /// <summary>
        /// The _libvlc_media_player_get_time.
        /// </summary>
        private libvlc_media_player_get_time _libvlc_media_player_get_time;

        /// <summary>
        /// The _libvlc_media_player_is_playing.
        /// </summary>
        private libvlc_media_player_is_playing _libvlc_media_player_is_playing;

        /// <summary>
        /// The _libvlc_media_player_new_from_media.
        /// </summary>
        private libvlc_media_player_new_from_media _libvlc_media_player_new_from_media;

        /// <summary>
        /// The _libvlc_media_player_next_frame.
        /// </summary>
        private libvlc_media_player_next_frame _libvlc_media_player_next_frame;

        /// <summary>
        /// The _libvlc_media_player_play.
        /// </summary>
        private libvlc_media_player_play _libvlc_media_player_play;

        /// <summary>
        /// The _libvlc_media_player_release.
        /// </summary>
        private libvlc_media_player_release _libvlc_media_player_release;

        /// <summary>
        /// The _libvlc_media_player_set_hwnd.
        /// </summary>
        private libvlc_media_player_set_hwnd _libvlc_media_player_set_hwnd;

        /// <summary>
        /// The _libvlc_media_player_set_pause.
        /// </summary>
        private libvlc_media_player_set_pause _libvlc_media_player_set_pause;

        /// <summary>
        /// The _libvlc_media_player_set_rate.
        /// </summary>
        private libvlc_media_player_set_rate _libvlc_media_player_set_rate;

        /// <summary>
        /// The _libvlc_media_player_set_time.
        /// </summary>
        private libvlc_media_player_set_time _libvlc_media_player_set_time;

        /// <summary>
        /// The _libvlc_media_player_stop.
        /// </summary>
        private libvlc_media_player_stop _libvlc_media_player_stop;

        /// <summary>
        /// The _libvlc_media_release.
        /// </summary>
        private libvlc_media_release _libvlc_media_release;

        /// <summary>
        /// The _libvlc_new.
        /// </summary>
        private libvlc_new _libvlc_new;

        /// <summary>
        /// The _libvlc_release.
        /// </summary>
        private libvlc_release _libvlc_release;

        /// <summary>
        /// The _libvlc_video_get_size.
        /// </summary>
        private libvlc_video_get_size _libvlc_video_get_size;

        /// <summary>
        /// The _libvlc_video_set_callbacks.
        /// </summary>
        private libvlc_video_set_callbacks _libvlc_video_set_callbacks;

        /// <summary>
        /// The _libvlc_video_set_format.
        /// </summary>
        private libvlc_video_set_format _libvlc_video_set_format;

        /// <summary>
        /// The _libvlc_video_set_spu.
        /// </summary>
        private libvlc_video_set_spu _libvlc_video_set_spu;

        /// <summary>
        /// The _libvlc_video_take_snapshot.
        /// </summary>
        private libvlc_video_take_snapshot _libvlc_video_take_snapshot;

        /// <summary>
        /// The _lib vlc dll.
        /// </summary>
        private IntPtr _libVlcDLL;

        /// <summary>
        /// The _media player.
        /// </summary>
        private IntPtr _mediaPlayer;

        /// <summary>
        /// The _mouse timer.
        /// </summary>
        private Timer _mouseTimer;

        /// <summary>
        /// The _owner control.
        /// </summary>
        private Control _ownerControl;

        /// <summary>
        /// The _parent form.
        /// </summary>
        private Form _parentForm;

        /// <summary>
        /// The _pause position.
        /// </summary>
        private double? _pausePosition = null; // Hack to hold precise seeking when paused

        /// <summary>
        /// The _video end timer.
        /// </summary>
        private Timer _videoEndTimer;

        /// <summary>
        /// The _video loaded timer.
        /// </summary>
        private Timer _videoLoadedTimer;

        /// <summary>
        /// Gets a value indicating whether is installed.
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                using (var vlc = new LibVlcDynamic())
                {
                    vlc.Initialize(null, null, null, null);
                    return vlc.IsAllMethodsLoaded();
                }
            }
        }

        /// <summary>
        /// Gets the player name.
        /// </summary>
        public override string PlayerName
        {
            get
            {
                return "VLC Lib Dynamic";
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public override int Volume
        {
            get
            {
                return this._libvlc_audio_get_volume(this._mediaPlayer);
            }

            set
            {
                this._libvlc_audio_set_volume(this._mediaPlayer, value);
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public override double Duration
        {
            get
            {
                return this._libvlc_media_player_get_length(this._mediaPlayer) / TimeCode.BaseUnit;
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public override double CurrentPosition
        {
            get
            {
                if (this._pausePosition != null)
                {
                    if (this._pausePosition < 0)
                    {
                        return 0;
                    }

                    return this._pausePosition.Value;
                }

                return this._libvlc_media_player_get_time(this._mediaPlayer) / TimeCode.BaseUnit;
            }

            set
            {
                if (this.IsPaused && value <= this.Duration)
                {
                    this._pausePosition = value;
                }

                this._libvlc_media_player_set_time(this._mediaPlayer, (long)(value * TimeCode.BaseUnit + 0.5));
            }
        }

        /// <summary>
        /// Gets or sets the play rate.
        /// </summary>
        public override double PlayRate
        {
            get
            {
                return this._libvlc_media_player_get_rate(this._mediaPlayer);
            }

            set
            {
                if (value >= 0 && value <= 2.0)
                {
                    this._libvlc_media_player_set_rate(this._mediaPlayer, (float)value);
                }
            }
        }

        /// <summary>
        /// Gets the vlc state.
        /// </summary>
        public int VlcState
        {
            get
            {
                return this._libvlc_media_player_get_state(this._mediaPlayer);
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
                int state = this._libvlc_media_player_get_state(this._mediaPlayer);
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
                int state = this._libvlc_media_player_get_state(this._mediaPlayer);
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
                return this._libvlc_audio_get_track_count(this._mediaPlayer) - 1;
            }
        }

        /// <summary>
        /// Gets or sets the audio track number.
        /// </summary>
        public int AudioTrackNumber
        {
            get
            {
                return this._libvlc_audio_get_track(this._mediaPlayer) - 1;
            }

            set
            {
                this._libvlc_audio_set_track(this._mediaPlayer, value + 1);
            }
        }

        /// <summary>
        /// Audio delay in milliseconds
        /// </summary>
        public long AudioDelay
        {
            get
            {
                return this._libvlc_audio_get_delay(this._mediaPlayer) / 1000; // converts microseconds to milliseconds
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
        /// The get dll type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object GetDllType(Type type, string name)
        {
            IntPtr address = NativeMethods.GetProcAddress(this._libVlcDLL, name);
            if (address != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer(address, type);
            }

            return null;
        }

        /// <summary>
        /// The load lib vlc dynamic.
        /// </summary>
        private void LoadLibVlcDynamic()
        {
            this._libvlc_new = (libvlc_new)this.GetDllType(typeof(libvlc_new), "libvlc_new");

            // _libvlc_get_version = (libvlc_get_version)GetDllType(typeof(libvlc_get_version), "libvlc_get_version");
            this._libvlc_release = (libvlc_release)this.GetDllType(typeof(libvlc_release), "libvlc_release");

            this._libvlc_media_new_path = (libvlc_media_new_path)this.GetDllType(typeof(libvlc_media_new_path), "libvlc_media_new_path");
            this._libvlc_media_player_new_from_media = (libvlc_media_player_new_from_media)this.GetDllType(typeof(libvlc_media_player_new_from_media), "libvlc_media_player_new_from_media");
            this._libvlc_media_release = (libvlc_media_release)this.GetDllType(typeof(libvlc_media_release), "libvlc_media_release");

            this._libvlc_video_get_size = (libvlc_video_get_size)this.GetDllType(typeof(libvlc_video_get_size), "libvlc_video_get_size");
            this._libvlc_audio_get_track_count = (libvlc_audio_get_track_count)this.GetDllType(typeof(libvlc_audio_get_track_count), "libvlc_audio_get_track_count");
            this._libvlc_audio_get_track = (libvlc_audio_get_track)this.GetDllType(typeof(libvlc_audio_get_track), "libvlc_audio_get_track");
            this._libvlc_audio_set_track = (libvlc_audio_set_track)this.GetDllType(typeof(libvlc_audio_set_track), "libvlc_audio_set_track");
            this._libvlc_video_take_snapshot = (libvlc_video_take_snapshot)this.GetDllType(typeof(libvlc_video_take_snapshot), "libvlc_video_take_snapshot");

            this._libvlc_audio_get_volume = (libvlc_audio_get_volume)this.GetDllType(typeof(libvlc_audio_get_volume), "libvlc_audio_get_volume");
            this._libvlc_audio_set_volume = (libvlc_audio_set_volume)this.GetDllType(typeof(libvlc_audio_set_volume), "libvlc_audio_set_volume");

            this._libvlc_media_player_play = (libvlc_media_player_play)this.GetDllType(typeof(libvlc_media_player_play), "libvlc_media_player_play");
            this._libvlc_media_player_stop = (libvlc_media_player_stop)this.GetDllType(typeof(libvlc_media_player_stop), "libvlc_media_player_stop");

            // _libvlc_media_player_pause = (libvlc_media_player_pause)GetDllType(typeof(libvlc_media_player_pause), "libvlc_media_player_pause");
            this._libvlc_media_player_set_hwnd = (libvlc_media_player_set_hwnd)this.GetDllType(typeof(libvlc_media_player_set_hwnd), "libvlc_media_player_set_hwnd");
            this._libvlc_media_player_is_playing = (libvlc_media_player_is_playing)this.GetDllType(typeof(libvlc_media_player_is_playing), "libvlc_media_player_is_playing");
            this._libvlc_media_player_set_pause = (libvlc_media_player_set_pause)this.GetDllType(typeof(libvlc_media_player_set_pause), "libvlc_media_player_set_pause");
            this._libvlc_media_player_get_time = (libvlc_media_player_get_time)this.GetDllType(typeof(libvlc_media_player_get_time), "libvlc_media_player_get_time");
            this._libvlc_media_player_set_time = (libvlc_media_player_set_time)this.GetDllType(typeof(libvlc_media_player_set_time), "libvlc_media_player_set_time");

            // _libvlc_media_player_get_fps = (libvlc_media_player_get_fps)GetDllType(typeof(libvlc_media_player_get_fps), "libvlc_media_player_get_fps");
            this._libvlc_media_player_get_state = (libvlc_media_player_get_state)this.GetDllType(typeof(libvlc_media_player_get_state), "libvlc_media_player_get_state");
            this._libvlc_media_player_get_length = (libvlc_media_player_get_length)this.GetDllType(typeof(libvlc_media_player_get_length), "libvlc_media_player_get_length");
            this._libvlc_media_player_release = (libvlc_media_player_release)this.GetDllType(typeof(libvlc_media_player_release), "libvlc_media_player_release");
            this._libvlc_media_player_get_rate = (libvlc_media_player_get_rate)this.GetDllType(typeof(libvlc_media_player_get_rate), "libvlc_media_player_get_rate");
            this._libvlc_media_player_set_rate = (libvlc_media_player_set_rate)this.GetDllType(typeof(libvlc_media_player_set_rate), "libvlc_media_player_set_rate");
            this._libvlc_media_player_next_frame = (libvlc_media_player_next_frame)this.GetDllType(typeof(libvlc_media_player_next_frame), "libvlc_media_player_next_frame");
            this._libvlc_video_set_spu = (libvlc_video_set_spu)this.GetDllType(typeof(libvlc_video_set_spu), "libvlc_video_set_spu");
            this._libvlc_video_set_callbacks = (libvlc_video_set_callbacks)this.GetDllType(typeof(libvlc_video_set_callbacks), "libvlc_video_set_callbacks");
            this._libvlc_video_set_format = (libvlc_video_set_format)this.GetDllType(typeof(libvlc_video_set_format), "libvlc_video_set_format");
            this._libvlc_audio_get_delay = (libvlc_audio_get_delay)this.GetDllType(typeof(libvlc_audio_get_delay), "libvlc_audio_get_delay");
        }

        /// <summary>
        /// The is all methods loaded.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsAllMethodsLoaded()
        {
            return this._libvlc_new != null &&

                   // _libvlc_get_version != null &&
                   this._libvlc_release != null && this._libvlc_media_new_path != null && this._libvlc_media_player_new_from_media != null && this._libvlc_media_release != null && this._libvlc_video_get_size != null && this._libvlc_audio_get_volume != null && this._libvlc_audio_set_volume != null && this._libvlc_media_player_play != null && this._libvlc_media_player_stop != null &&

                   // _libvlc_media_player_pause != null &&
                   this._libvlc_media_player_set_hwnd != null && this._libvlc_media_player_is_playing != null && this._libvlc_media_player_get_time != null && this._libvlc_media_player_set_time != null &&

                   // _libvlc_media_player_get_fps != null &&
                   this._libvlc_media_player_get_state != null && this._libvlc_media_player_get_length != null && this._libvlc_media_player_release != null && this._libvlc_media_player_get_rate != null && this._libvlc_media_player_set_rate != null;
        }

        /// <summary>
        /// The get next frame.
        /// </summary>
        public void GetNextFrame()
        {
            this._libvlc_media_player_next_frame(this._mediaPlayer);
        }

        /// <summary>
        /// The play.
        /// </summary>
        public override void Play()
        {
            this._libvlc_media_player_play(this._mediaPlayer);
            this._pausePosition = null;
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public override void Pause()
        {
            int i = 0;
            this._libvlc_media_player_set_pause(this._mediaPlayer, 1);
            int state = this.VlcState;
            while (state != 4 && i < 50)
            {
                System.Threading.Thread.Sleep(10);
                i++;
                state = this.VlcState;
            }

            this._libvlc_media_player_set_pause(this._mediaPlayer, 1);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public override void Stop()
        {
            this._libvlc_media_player_stop(this._mediaPlayer);
            this._pausePosition = null;
        }

        /// <summary>
        /// The take snapshot.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TakeSnapshot(string fileName, uint width, uint height)
        {
            if (this._libvlc_video_take_snapshot == null)
            {
                return false;
            }

            return this._libvlc_video_take_snapshot(this._mediaPlayer, 0, Encoding.UTF8.GetBytes(fileName + "\0"), width, height) == 1;
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
        /// The <see cref="LibVlcDynamic"/>.
        /// </returns>
        public LibVlcDynamic MakeSecondMediaPlayer(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded)
        {
            var newVlc = new LibVlcDynamic { _libVlc = this._libVlc, _libVlcDLL = this._libVlcDLL, _ownerControl = ownerControl };
            if (ownerControl != null)
            {
                newVlc._parentForm = ownerControl.FindForm();
            }

            newVlc.LoadLibVlcDynamic();

            newVlc.OnVideoLoaded = onVideoLoaded;
            newVlc.OnVideoEnded = onVideoEnded;

            if (!string.IsNullOrEmpty(videoFileName))
            {
                IntPtr media = this._libvlc_media_new_path(this._libVlc, Encoding.UTF8.GetBytes(videoFileName + "\0"));
                newVlc._mediaPlayer = this._libvlc_media_player_new_from_media(media);
                this._libvlc_media_release(media);

                // Linux: libvlc_media_player_set_xdrawable (_mediaPlayer, xdrawable);
                // Mac: libvlc_media_player_set_nsobject (_mediaPlayer, view);
                this._libvlc_media_player_set_hwnd(newVlc._mediaPlayer, ownerControl.Handle); // windows

                if (onVideoEnded != null)
                {
                    newVlc._videoEndTimer = new Timer { Interval = 500 };
                    newVlc._videoEndTimer.Tick += this.VideoEndTimerTick;
                    newVlc._videoEndTimer.Start();
                }

                this._libvlc_media_player_play(newVlc._mediaPlayer);
                newVlc._videoLoadedTimer = new Timer { Interval = 100 };
                newVlc._videoLoadedTimer.Tick += newVlc.VideoLoadedTimer_Tick;
                newVlc._videoLoadedTimer.Start();

                newVlc._mouseTimer = new Timer { Interval = 25 };
                newVlc._mouseTimer.Tick += newVlc.MouseTimerTick;
                newVlc._mouseTimer.Start();
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
            this._videoLoadedTimer.Stop();
            int i = 0;
            while (!this.IsPlaying && i < 50)
            {
                System.Threading.Thread.Sleep(100);
                i++;
            }

            this.Pause();
            if (this._libvlc_video_set_spu != null)
            {
                this._libvlc_video_set_spu(this._mediaPlayer, -1); // turn of embedded subtitles
            }

            if (this.OnVideoLoaded != null)
            {
                this.OnVideoLoaded.Invoke(this._mediaPlayer, new EventArgs());
            }
        }

        /// <summary>
        /// The get vlc path.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetVlcPath(string fileName)
        {
            if (Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac())
            {
                return null;
            }

            var path = Path.Combine(Configuration.BaseDirectory, @"VLC\" + fileName);
            if (File.Exists(path))
            {
                return path;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.General.VlcLocation))
            {
                if (Configuration.Settings.General.VlcLocation.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    Configuration.Settings.General.VlcLocation = Path.GetDirectoryName(Configuration.Settings.General.VlcLocation);
                }

                path = Path.Combine(Configuration.Settings.General.VlcLocation, fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.General.VlcLocationRelative))
            {
                try
                {
                    path = Configuration.Settings.General.VlcLocationRelative;
                    if (path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        path = Path.GetDirectoryName(path);
                    }

                    path = Path.Combine(path, fileName);

                    string path2 = Path.GetFullPath(path);
                    if (File.Exists(path2))
                    {
                        return path2;
                    }

                    while (path.StartsWith(".."))
                    {
                        path = path.Remove(0, 3);
                        path2 = Path.GetFullPath(path);
                        if (File.Exists(path2))
                        {
                            return path2;
                        }
                    }
                }
                catch
                {
                }
            }

            // XP via registry path
            path = RegistryUtil.GetValue(@"SOFTWARE\VideoLAN\VLC", "InstallDir");
            if (path != null && Directory.Exists(path))
            {
                path = Path.Combine(path, fileName);
            }

            if (File.Exists(path))
            {
                return path;
            }

            // Winows 7 via registry path
            path = RegistryUtil.GetValue(@"SOFTWARE\Wow6432Node\VideoLAN\VLC", "InstallDir");
            if (path != null && Directory.Exists(path))
            {
                path = Path.Combine(path, fileName);
            }

            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(@"C:\Program Files (x86)\VideoLAN\VLC", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(@"C:\Program Files\VideoLAN\VLC", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(@"C:\Program Files (x86)\VLC", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(@"C:\Program Files\VLC", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"VideoLAN\VLC\" + fileName);
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"VLC\" + fileName);
            if (File.Exists(path))
            {
                return path;
            }

            return null;
        }

        /// <summary>
        /// The initialize and start frame grabbing.
        /// </summary>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="lock">
        /// The lock.
        /// </param>
        /// <param name="unlock">
        /// The unlock.
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="opaque">
        /// The opaque.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool InitializeAndStartFrameGrabbing(string videoFileName, uint width, uint height, LockCallbackDelegate @lock, UnlockCallbackDelegate unlock, DisplayCallbackDelegate display, IntPtr opaque)
        {
            string dllFile = GetVlcPath("libvlc.dll");
            if (!File.Exists(dllFile) || string.IsNullOrEmpty(videoFileName))
            {
                return false;
            }

            Directory.SetCurrentDirectory(Path.GetDirectoryName(dllFile));
            this._libVlcDLL = NativeMethods.LoadLibrary(dllFile);
            this.LoadLibVlcDynamic();
            string[] initParameters = { "--no-skip-frames" };
            this._libVlc = this._libvlc_new(initParameters.Length, initParameters);
            IntPtr media = this._libvlc_media_new_path(this._libVlc, Encoding.UTF8.GetBytes(videoFileName + "\0"));
            this._mediaPlayer = this._libvlc_media_player_new_from_media(media);
            this._libvlc_media_release(media);

            this._libvlc_video_set_format(this._mediaPlayer, "RV24", width, height, 3 * width);

            // _libvlc_video_set_format(_mediaPlayer,"RV32", width, height, 4 * width);

            // _libvlc_video_set_callbacks(_mediaPlayer, @lock, unlock, display, opaque);
            this._libvlc_video_set_callbacks(this._mediaPlayer, @lock, unlock, display, opaque);
            this._libvlc_audio_set_volume(this._mediaPlayer, 0);
            this._libvlc_media_player_set_rate(this._mediaPlayer, 9f);

            // _libvlc_media_player_play(_mediaPlayer);
            return true;
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

            string dllFile = GetVlcPath("libvlc.dll");
            if (File.Exists(dllFile))
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(dllFile));
                this._libVlcDLL = NativeMethods.LoadLibrary(dllFile);
                this.LoadLibVlcDynamic();
            }
            else if (!Directory.Exists(videoFileName))
            {
                return;
            }

            this.OnVideoLoaded = onVideoLoaded;
            this.OnVideoEnded = onVideoEnded;

            if (!string.IsNullOrEmpty(videoFileName))
            {
                string[] initParameters = { "--no-sub-autodetect-file" }; // , "--ffmpeg-hw" }; //, "--no-video-title-show" }; // TODO: Put in options/config file
                this._libVlc = this._libvlc_new(initParameters.Length, initParameters);
                IntPtr media = this._libvlc_media_new_path(this._libVlc, Encoding.UTF8.GetBytes(videoFileName + "\0"));
                this._mediaPlayer = this._libvlc_media_player_new_from_media(media);
                this._libvlc_media_release(media);

                // Linux: libvlc_media_player_set_xdrawable (_mediaPlayer, xdrawable);
                // Mac: libvlc_media_player_set_nsobject (_mediaPlayer, view);
                if (ownerControl != null)
                {
                    this._libvlc_media_player_set_hwnd(this._mediaPlayer, ownerControl.Handle); // windows

                    // hack: sometimes vlc opens in it's own windows - this code seems to prevent this
                    for (int j = 0; j < 50; j++)
                    {
                        System.Threading.Thread.Sleep(10);
                        Application.DoEvents();
                    }

                    this._libvlc_media_player_set_hwnd(this._mediaPlayer, ownerControl.Handle); // windows
                }

                if (onVideoEnded != null)
                {
                    this._videoEndTimer = new Timer { Interval = 500 };
                    this._videoEndTimer.Tick += this.VideoEndTimerTick;
                    this._videoEndTimer.Start();
                }

                this._libvlc_media_player_play(this._mediaPlayer);
                this._videoLoadedTimer = new Timer { Interval = 100 };
                this._videoLoadedTimer.Tick += this.VideoLoadedTimer_Tick;
                this._videoLoadedTimer.Start();

                this._mouseTimer = new Timer { Interval = 25 };
                this._mouseTimer.Tick += this.MouseTimerTick;
                this._mouseTimer.Start();
            }
        }

        /// <summary>
        /// The is left mouse button down.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsLeftMouseButtonDown()
        {
            const int KEY_PRESSED = 0x8000;
            const int VK_LBUTTON = 0x1;
            return Convert.ToBoolean(NativeMethods.GetKeyState(VK_LBUTTON) & KEY_PRESSED);
        }

        /// <summary>
        /// The mouse timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MouseTimerTick(object sender, EventArgs e)
        {
            this._mouseTimer.Stop();
            if (this._parentForm != null && this._ownerControl != null && this._ownerControl.Visible && this._parentForm.ContainsFocus && IsLeftMouseButtonDown())
            {
                var p = this._ownerControl.PointToClient(Control.MousePosition);
                if (p.X > 0 && p.X < this._ownerControl.Width && p.Y > 0 && p.Y < this._ownerControl.Height)
                {
                    if (this.IsPlaying)
                    {
                        this.Pause();
                    }
                    else
                    {
                        this.Play();
                    }

                    int i = 0;
                    while (IsLeftMouseButtonDown() && i < 200)
                    {
                        System.Threading.Thread.Sleep(2);
                        Application.DoEvents();
                        i++;
                    }
                }
            }

            this._mouseTimer.Start();
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
            int state = this._libvlc_media_player_get_state(this._mediaPlayer);
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
            this.Dispose();
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
        /// The release unmanged resources.
        /// </summary>
        private void ReleaseUnmangedResources()
        {
            try
            {
                lock (this)
                {
                    if (this._mediaPlayer != IntPtr.Zero)
                    {
                        this._libvlc_media_player_stop(this._mediaPlayer);

                        // _libvlc_media_player_release(_mediaPlayer); // CRASHES in visual sync / point sync!
                        this._mediaPlayer = IntPtr.Zero;

                        // _libvlc_media_list_player_release(_mediaPlayer);
                    }

                    if (this._libvlc_release != null && this._libVlc != IntPtr.Zero)
                    {
                        this._libvlc_release(this._libVlc);
                        this._libVlc = IntPtr.Zero;
                    }

                    // if (_libVlcDLL != IntPtr.Zero)
                    // {
                    // FreeLibrary(_libVlcDLL);  // CRASHES in visual sync / point sync!
                    // _libVlcDLL = IntPtr.Zero;
                    // }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LibVlcDynamic"/> class. 
        /// </summary>
        ~LibVlcDynamic()
        {
            this.Dispose(false);
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

                if (this._mouseTimer != null)
                {
                    this._mouseTimer.Dispose();
                    this._mouseTimer = null;
                }
            }

            this.ReleaseUnmangedResources();
        }

        // LibVLC Core - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__core.html
        /// <summary>
        /// The libvlc_new.
        /// </summary>
        /// <param name="argc">
        /// The argc.
        /// </param>
        /// <param name="argv">
        /// The argv.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_new(int argc, [MarshalAs(UnmanagedType.LPArray)] string[] argv);

        // [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // private delegate IntPtr libvlc_get_version();
        // private libvlc_get_version _libvlc_get_version;

        /// <summary>
        /// The libvlc_release.
        /// </summary>
        /// <param name="libVlc">
        /// The lib vlc.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_release(IntPtr libVlc);

        // LibVLC Media - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__media.html
        /// <summary>
        /// The libvlc_media_new_path.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_media_new_path(IntPtr instance, byte[] input);

        /// <summary>
        /// The libvlc_media_release.
        /// </summary>
        /// <param name="media">
        /// The media.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_release(IntPtr media);

        // LibVLC Video Controls - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__video.html#g8f55326b8b51aecb59d8b8a446c3f118
        /// <summary>
        /// The libvlc_video_get_size.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_video_get_size(IntPtr mediaPlayer, uint number, out uint x, out uint y);

        /// <summary>
        /// The libvlc_video_take_snapshot.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="num">
        /// The num.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_video_take_snapshot(IntPtr mediaPlayer, byte num, byte[] filePath, uint width, uint height);

        /// <summary>
        /// The libvlc_video_set_callbacks.
        /// </summary>
        /// <param name="playerInstance">
        /// The player instance.
        /// </param>
        /// <param name="lock">
        /// The lock.
        /// </param>
        /// <param name="unlock">
        /// The unlock.
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="opaque">
        /// The opaque.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_video_set_callbacks(IntPtr playerInstance, LockCallbackDelegate @lock, UnlockCallbackDelegate unlock, DisplayCallbackDelegate display, IntPtr opaque);

        /// <summary>
        /// The libvlc_video_set_format.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="chroma">
        /// The chroma.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="pitch">
        /// The pitch.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_video_set_format(IntPtr mediaPlayer, string chroma, uint width, uint height, uint pitch);

        // LibVLC Audio Controls - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__audio.html
        /// <summary>
        /// The libvlc_audio_get_volume.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_audio_get_volume(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_audio_set_volume.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="volume">
        /// The volume.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_audio_set_volume(IntPtr mediaPlayer, int volume);

        /// <summary>
        /// The libvlc_audio_get_track_count.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_audio_get_track_count(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_audio_get_track.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_audio_get_track(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_audio_set_track.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="trackNumber">
        /// The track number.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_audio_set_track(IntPtr mediaPlayer, int trackNumber);

        /// <summary>
        /// The libvlc_audio_get_delay.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int64 libvlc_audio_get_delay(IntPtr mediaPlayer);

        // LibVLC media player - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__media__player.html
        /// <summary>
        /// The libvlc_media_player_new_from_media.
        /// </summary>
        /// <param name="media">
        /// The media.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_media_player_new_from_media(IntPtr media);

        /// <summary>
        /// The libvlc_media_player_play.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_play(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_stop.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_stop(IntPtr mediaPlayer);

        // [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // private delegate void libvlc_media_player_pause(IntPtr mediaPlayer);
        // private libvlc_media_player_pause _libvlc_media_player_pause;

        /// <summary>
        /// The libvlc_media_player_set_hwnd.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="windowsHandle">
        /// The windows handle.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_set_hwnd(IntPtr mediaPlayer, IntPtr windowsHandle);

        /// <summary>
        /// The libvlc_media_player_is_playing.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_is_playing(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_set_pause.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="doPause">
        /// The do pause.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_set_pause(IntPtr mediaPlayer, int doPause);

        /// <summary>
        /// The libvlc_media_player_get_time.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int64 libvlc_media_player_get_time(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_set_time.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_set_time(IntPtr mediaPlayer, long position);

        // [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // private delegate float libvlc_media_player_get_fps(IntPtr mediaPlayer);
        // private libvlc_media_player_get_fps _libvlc_media_player_get_fps;

        /// <summary>
        /// The libvlc_media_player_get_state.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate byte libvlc_media_player_get_state(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_get_length.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int64 libvlc_media_player_get_length(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_release.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_release(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_get_rate.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float libvlc_media_player_get_rate(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_set_rate.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="rate">
        /// The rate.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_set_rate(IntPtr mediaPlayer, float rate);

        /// <summary>
        /// The libvlc_media_player_next_frame.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_next_frame(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_video_set_spu.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="trackNumber">
        /// The track number.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_video_set_spu(IntPtr mediaPlayer, int trackNumber);
    }
}