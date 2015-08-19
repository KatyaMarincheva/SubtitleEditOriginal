using Nikse.SubtitleEdit.Logic;
using System.ComponentModel;
using System.Drawing;
using Nikse.SubtitleEdit.Logic.VideoPlayers;
using System.Windows.Forms;
using Nikse.SubtitleEdit.Controls;
// <copyright file="VideoPlayerContainerFactory.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;
using Moq;

namespace Nikse.SubtitleEdit.Controls
{
    using Nikse.SubtitleEdit.Controls.Interfaces;

    /// <summary>A factory for Nikse.SubtitleEdit.Controls.VideoPlayerContainer instances</summary>
    public static partial class VideoPlayerContainerFactory
    {
        /// <summary>A factory for Nikse.SubtitleEdit.Controls.VideoPlayerContainer instances</summary>
        [PexFactoryMethod(typeof(MockVideoPlayerContainer))]
        public static IVideoPlayerContainer Create()
        {
            var factory = new MockFactory(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            MockVideoPlayerContainer videoPlayerContainerMock = new MockVideoPlayerContainer();
            return videoPlayerContainerMock;
        }
    }
}
