// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="" company="Katya">
// //      Katya.com. All rights reserved.
// // </copyright>
// // <summary>
// //   
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces
{
    using System;
    using System.Windows.Forms;

    public interface IVideoPlayer
    {
        string PlayerName { get; }

        string VideoFileName { get; }

        int Volume { get; set; }

        double Duration { get; }

        double CurrentPosition { get; set; }

        double PlayRate { get; set; }

        bool IsPaused { get; }

        bool IsPlaying { get; }

        void Play();

        void Pause();

        void Stop();

        void Initialize(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded);

        void DisposeVideoPlayer();

        event EventHandler OnVideoLoaded;

        event EventHandler OnVideoEnded;

        void Resize(int width, int height);
    }
}