// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="" company="Katya">
// //      Katya.com. All rights reserved.
// // </copyright>
// // <summary>
// //   
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls.Interfaces
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Interfaces;
    using Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces;

    public interface IVideoPlayerContainer
    {
        Panel PanelPlayer { get; }

        float FontSizeFactor { get; set; }

        IVideoPlayer VideoPlayer { get; set; }

        RichTextBoxViewOnly TextBox { get; }

        int VideoWidth { get; set; }

        int VideoHeight { get; set; }

        RightToLeft TextRightToLeft { get; set; }

        bool ShowStopButton { get; set; }

        bool ShowMuteButton { get; set; }

        bool ShowFullscreenButton { get; set; }

        IParagraph LastParagraph { get; }

        string SubtitleText { get; set; }

        event EventHandler OnButtonClicked;

        void EnableMouseWheelStep();

        void SetPlayerName(string s);

        void ResetTimeLabel();

        void AddMouseWheelEvent(Control control);

        void ControlMouseWheel(object sender, MouseEventArgs e);

        Control MakeSubtitlesPanel();

        void SetSubtitleFont();

        void SubtitleTextBoxMouseClick(object sender, MouseEventArgs e);

        void SetSubtitleText(string text, IParagraph p);

        void PanelPlayerMouseDown(object sender, MouseEventArgs e);

        void InitializeVolume(double defaultVolume);

        Control MakePlayerPanel();

        void HideControls();

        void ShowControls();

        Control MakeControlsPanel();

        void VideoPlayerContainerResize(object sender, EventArgs e);

        #region PlayPauseButtons

        void RefreshPlayPauseButtons();

        void HideAllPlayImages();

        void PictureBoxPlayMouseEnter(object sender, EventArgs e);

        void PictureBoxPlayOverMouseLeave(object sender, EventArgs e);

        void PictureBoxPlayOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxPlayOverMouseUp(object sender, MouseEventArgs e);

        void HideAllPauseImages();

        void PictureBoxPauseMouseEnter(object sender, EventArgs e);

        void PictureBoxPauseOverMouseLeave(object sender, EventArgs e);

        void PictureBoxPauseOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxPauseOverMouseUp(object sender, MouseEventArgs e);

        #endregion PlayPauseButtons

        #region StopButtons

        void HideAllStopImages();

        void PictureBoxStopMouseEnter(object sender, EventArgs e);

        void PictureBoxStopOverMouseLeave(object sender, EventArgs e);

        void PictureBoxStopOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxStopOverMouseUp(object sender, MouseEventArgs e);

        #endregion StopButtons

        #region FullscreenButtons

        void HideAllFullscreenImages();

        void ShowFullScreenControls();

        void ShowNonFullScreenControls();

        void PictureBoxFullscreenMouseEnter(object sender, EventArgs e);

        void PictureBoxFullscreenOverMouseLeave(object sender, EventArgs e);

        void PictureBoxFullscreenOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxFullscreenOverMouseUp(object sender, MouseEventArgs e);

        #endregion FullscreenButtons

        #region Mute buttons

        void HideAllMuteImages();

        void PictureBoxMuteMouseEnter(object sender, EventArgs e);

        void PictureBoxMuteOverMouseLeave(object sender, EventArgs e);

        void PictureBoxMuteOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxMuteOverMouseUp(object sender, MouseEventArgs e);

        void PictureBoxMuteDownClick(object sender, EventArgs e);

        #endregion Mute buttons

        #region Reverse buttons

        void HideAllReverseImages();

        void PictureBoxReverseMouseEnter(object sender, EventArgs e);

        void PictureBoxReverseOverMouseLeave(object sender, EventArgs e);

        void PictureBoxReverseOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxReverseOverMouseUp(object sender, MouseEventArgs e);

        #endregion Reverse buttons

        #region Fast forward buttons

        void HideAllFastForwardImages();

        void PictureBoxFastForwardMouseEnter(object sender, EventArgs e);

        void PictureBoxFastForwardOverMouseLeave(object sender, EventArgs e);

        void PictureBoxFastForwardOverMouseDown(object sender, MouseEventArgs e);

        void PictureBoxFastForwardOverMouseUp(object sender, MouseEventArgs e);

        #endregion Fast forward buttons

        #region Progress bars

        void SetProgressBarPosition(int mouseX);

        void PictureBoxProgressbarBackgroundMouseDown(object sender, MouseEventArgs e);

        void PictureBoxProgressBarMouseDown(object sender, MouseEventArgs e);

        void RefreshProgressBar();

        void SetVolumeBarPosition(int mouseX);

        void PictureBoxVolumeBarBackgroundMouseDown(object sender, MouseEventArgs e);

        void PictureBoxVolumeBarMouseDown(object sender, MouseEventArgs e);

        void RefreshVolumeBar();

        #endregion Progress bars

        #region VideoPlayer functions

        void Play();

        void Stop();

        void Pause();

        void TogglePlayPause();

        bool IsPaused { get; }

        double Volume { get; set; }

        /// <summary>
        ///     Video offset in seconds
        /// </summary>
        double Offset { get; set; }

        /// <summary>
        ///     Current position in seconds
        /// </summary>
        double CurrentPosition { get; set; }

        /// <summary>
        ///     Total duration in seconds
        /// </summary>
        double Duration { get; }

        bool Mute { get; set; }

        #endregion VideoPlayer functions
    }
}