// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVideoPlayer.cs" company="Katya">
//   Katya.com. All rights reserved.
// </copyright>
// // <summary>
//   The VideoPlayer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The VideoPlayer interface.
    /// </summary>
    public interface IVideoPlayer
    {
        /// <summary>
        /// Gets the player name.
        /// </summary>
        string PlayerName { get; }

        /// <summary>
        /// Gets the video file name.
        /// </summary>
        string VideoFileName { get; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        int Volume { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        double Duration { get; }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        double CurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the play rate.
        /// </summary>
        double PlayRate { get; set; }

        /// <summary>
        /// Gets a value indicating whether is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// The play.
        /// </summary>
        void Play();

        /// <summary>
        /// The pause.
        /// </summary>
        void Pause();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();

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
        void Initialize(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded);

        /// <summary>
        /// The dispose video player.
        /// </summary>
        void DisposeVideoPlayer();

        /// <summary>
        /// The on video loaded.
        /// </summary>
        event EventHandler OnVideoLoaded;

        /// <summary>
        /// The on video ended.
        /// </summary>
        event EventHandler OnVideoEnded;

        /// <summary>
        /// The resize.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        void Resize(int width, int height);
    }
}