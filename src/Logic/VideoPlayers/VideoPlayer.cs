// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlayer.cs" company="">
//   
// </copyright>
// <summary>
//   The video player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces;

    /// <summary>
    /// The video player.
    /// </summary>
    public abstract class VideoPlayer : IVideoPlayer
    {
        /// <summary>
        /// Gets the player name.
        /// </summary>
        public abstract string PlayerName { get; }

        /// <summary>
        /// Gets or sets the video file name.
        /// </summary>
        public string VideoFileName { get; protected set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public abstract int Volume { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public abstract double Duration { get; }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public abstract double CurrentPosition { get; set; }

        /// <summary>
        /// 1.0 is normal playback speed, 0.5 is half speed, and 2.0 is twice speed.
        /// </summary>
        public virtual double PlayRate { get; set; }

        /// <summary>
        /// The play.
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// The pause.
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// The stop.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Gets a value indicating whether is paused.
        /// </summary>
        public abstract bool IsPaused { get; }

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        public abstract bool IsPlaying { get; }

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
        public abstract void Initialize(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded);

        /// <summary>
        /// The dispose video player.
        /// </summary>
        public abstract void DisposeVideoPlayer();

        /// <summary>
        /// The on video loaded.
        /// </summary>
        public abstract event EventHandler OnVideoLoaded;

        /// <summary>
        /// The on video ended.
        /// </summary>
        public abstract event EventHandler OnVideoEnded;

        /// <summary>
        /// The resize.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public virtual void Resize(int width, int height)
        {
        }
    }
}