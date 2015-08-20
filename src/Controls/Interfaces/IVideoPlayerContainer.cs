// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVideoPlayerContainer.cs" company="Katya">
//   Katya.com. All rights reserved.
// </copyright>
// // <summary>
//   The VideoPlayerContainer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Controls.Interfaces
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic.Interfaces;
    using Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces;

    /// <summary>
    /// The VideoPlayerContainer interface.
    /// </summary>
    public interface IVideoPlayerContainer
    {
        /// <summary>
        /// Gets the panel player.
        /// </summary>
        Panel PanelPlayer { get; }

        /// <summary>
        /// Gets or sets the font size factor.
        /// </summary>
        float FontSizeFactor { get; set; }

        /// <summary>
        /// Gets or sets the video player.
        /// </summary>
        IVideoPlayer VideoPlayer { get; set; }

        /// <summary>
        /// Gets the text box.
        /// </summary>
        RichTextBoxViewOnly TextBox { get; }

        /// <summary>
        /// Gets or sets the video width.
        /// </summary>
        int VideoWidth { get; set; }

        /// <summary>
        /// Gets or sets the video height.
        /// </summary>
        int VideoHeight { get; set; }

        /// <summary>
        /// Gets or sets the text right to left.
        /// </summary>
        RightToLeft TextRightToLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show stop button.
        /// </summary>
        bool ShowStopButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show mute button.
        /// </summary>
        bool ShowMuteButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show fullscreen button.
        /// </summary>
        bool ShowFullscreenButton { get; set; }

        /// <summary>
        /// Gets the last paragraph.
        /// </summary>
        IParagraph LastParagraph { get; }

        /// <summary>
        /// Gets or sets the subtitle text.
        /// </summary>
        string SubtitleText { get; set; }

        /// <summary>
        /// The on button clicked.
        /// </summary>
        event EventHandler OnButtonClicked;

        /// <summary>
        /// The enable mouse wheel step.
        /// </summary>
        void EnableMouseWheelStep();

        /// <summary>
        /// The set player name.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        void SetPlayerName(string s);

        /// <summary>
        /// The reset time label.
        /// </summary>
        void ResetTimeLabel();

        /// <summary>
        /// The add mouse wheel event.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        void AddMouseWheelEvent(Control control);

        /// <summary>
        /// The control mouse wheel.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void ControlMouseWheel(object sender, MouseEventArgs e);

        /// <summary>
        /// The make subtitles panel.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        Control MakeSubtitlesPanel();

        /// <summary>
        /// The set subtitle font.
        /// </summary>
        void SetSubtitleFont();

        /// <summary>
        /// The subtitle text box mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void SubtitleTextBoxMouseClick(object sender, MouseEventArgs e);

        /// <summary>
        /// The set subtitle text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        void SetSubtitleText(string text, IParagraph p);

        /// <summary>
        /// The panel player mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PanelPlayerMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The initialize volume.
        /// </summary>
        /// <param name="defaultVolume">
        /// The default volume.
        /// </param>
        void InitializeVolume(double defaultVolume);

        /// <summary>
        /// The make player panel.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        Control MakePlayerPanel();

        /// <summary>
        /// The hide controls.
        /// </summary>
        void HideControls();

        /// <summary>
        /// The show controls.
        /// </summary>
        void ShowControls();

        /// <summary>
        /// The make controls panel.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        Control MakeControlsPanel();

        /// <summary>
        /// The video player container resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void VideoPlayerContainerResize(object sender, EventArgs e);

        #region PlayPauseButtons

        /// <summary>
        /// The refresh play pause buttons.
        /// </summary>
        void RefreshPlayPauseButtons();

        /// <summary>
        /// The hide all play images.
        /// </summary>
        void HideAllPlayImages();

        /// <summary>
        /// The picture box play mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPlayMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box play over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPlayOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box play over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPlayOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box play over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPlayOverMouseUp(object sender, MouseEventArgs e);

        /// <summary>
        /// The hide all pause images.
        /// </summary>
        void HideAllPauseImages();

        /// <summary>
        /// The picture box pause mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPauseMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box pause over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPauseOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box pause over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPauseOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box pause over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxPauseOverMouseUp(object sender, MouseEventArgs e);

        #endregion PlayPauseButtons

        #region StopButtons

        /// <summary>
        /// The hide all stop images.
        /// </summary>
        void HideAllStopImages();

        /// <summary>
        /// The picture box stop mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxStopMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box stop over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxStopOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box stop over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxStopOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box stop over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxStopOverMouseUp(object sender, MouseEventArgs e);

        #endregion StopButtons

        #region FullscreenButtons

        /// <summary>
        /// The hide all fullscreen images.
        /// </summary>
        void HideAllFullscreenImages();

        /// <summary>
        /// The show full screen controls.
        /// </summary>
        void ShowFullScreenControls();

        /// <summary>
        /// The show non full screen controls.
        /// </summary>
        void ShowNonFullScreenControls();

        /// <summary>
        /// The picture box fullscreen mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFullscreenMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box fullscreen over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFullscreenOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box fullscreen over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFullscreenOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box fullscreen over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFullscreenOverMouseUp(object sender, MouseEventArgs e);

        #endregion FullscreenButtons

        #region Mute buttons

        /// <summary>
        /// The hide all mute images.
        /// </summary>
        void HideAllMuteImages();

        /// <summary>
        /// The picture box mute mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxMuteMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box mute over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxMuteOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box mute over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxMuteOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box mute over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxMuteOverMouseUp(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box mute down click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxMuteDownClick(object sender, EventArgs e);

        #endregion Mute buttons

        #region Reverse buttons

        /// <summary>
        /// The hide all reverse images.
        /// </summary>
        void HideAllReverseImages();

        /// <summary>
        /// The picture box reverse mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxReverseMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box reverse over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxReverseOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box reverse over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxReverseOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box reverse over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxReverseOverMouseUp(object sender, MouseEventArgs e);

        #endregion Reverse buttons

        #region Fast forward buttons

        /// <summary>
        /// The hide all fast forward images.
        /// </summary>
        void HideAllFastForwardImages();

        /// <summary>
        /// The picture box fast forward mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFastForwardMouseEnter(object sender, EventArgs e);

        /// <summary>
        /// The picture box fast forward over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFastForwardOverMouseLeave(object sender, EventArgs e);

        /// <summary>
        /// The picture box fast forward over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFastForwardOverMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box fast forward over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxFastForwardOverMouseUp(object sender, MouseEventArgs e);

        #endregion Fast forward buttons

        #region Progress bars

        /// <summary>
        /// The set progress bar position.
        /// </summary>
        /// <param name="mouseX">
        /// The mouse x.
        /// </param>
        void SetProgressBarPosition(int mouseX);

        /// <summary>
        /// The picture box progressbar background mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxProgressbarBackgroundMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box progress bar mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxProgressBarMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The refresh progress bar.
        /// </summary>
        void RefreshProgressBar();

        /// <summary>
        /// The set volume bar position.
        /// </summary>
        /// <param name="mouseX">
        /// The mouse x.
        /// </param>
        void SetVolumeBarPosition(int mouseX);

        /// <summary>
        /// The picture box volume bar background mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxVolumeBarBackgroundMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The picture box volume bar mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void PictureBoxVolumeBarMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// The refresh volume bar.
        /// </summary>
        void RefreshVolumeBar();

        #endregion Progress bars

        #region VideoPlayer functions

        /// <summary>
        /// The play.
        /// </summary>
        void Play();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();

        /// <summary>
        /// The pause.
        /// </summary>
        void Pause();

        /// <summary>
        /// The toggle play pause.
        /// </summary>
        void TogglePlayPause();

        /// <summary>
        /// Gets a value indicating whether is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether mute.
        /// </summary>
        bool Mute { get; set; }

        #endregion VideoPlayer functions
    }
}