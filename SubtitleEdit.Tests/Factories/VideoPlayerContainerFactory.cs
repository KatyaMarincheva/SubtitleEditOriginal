// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlayerContainerFactory.cs" company="">
//   
// </copyright>
// <summary>
//   A factory for Nikse.SubtitleEdit.Controls.VideoPlayerContainer instances
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// <copyright file="VideoPlayerContainerFactory.cs" company="Nikse">Nikse</copyright>

namespace Nikse.SubtitleEdit.Controls
{
    using Microsoft.Pex.Framework;

    using Moq;

    using Nikse.SubtitleEdit.Controls.Interfaces;

    /// <summary>A factory for Nikse.SubtitleEdit.Controls.VideoPlayerContainer instances</summary>
    public static partial class VideoPlayerContainerFactory
    {
        /// <summary>
        /// A factory for Nikse.SubtitleEdit.Controls.VideoPlayerContainer instances
        /// </summary>
        /// <returns>
        /// The <see cref="IVideoPlayerContainer"/>.
        /// </returns>
        [PexFactoryMethod(typeof(MockVideoPlayerContainer))]
        public static IVideoPlayerContainer Create()
        {
            var factory = new MockFactory(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            MockVideoPlayerContainer videoPlayerContainerMock = new MockVideoPlayerContainer();
            return videoPlayerContainerMock;
        }
    }
}