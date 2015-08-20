// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpcHcCommand.cs" company="">
//   
// </copyright>
// <summary>
//   HPC-HC API commands - https://github.com/mpc-hc/mpc-hc/blob/master/src/mpc-hc/MpcApi.h
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers.MpcHC
{
    using System;

    /// <summary>
    /// HPC-HC API commands - https://github.com/mpc-hc/mpc-hc/blob/master/src/mpc-hc/MpcApi.h
    /// </summary>
    public static class MpcHcCommand
    {
        /// <summary>
        /// The connect.
        /// </summary>
        public const uint Connect = 0x50000000;

        /// <summary>
        /// The state.
        /// </summary>
        public const uint State = 0x50000001;

        /// <summary>
        /// The play mode.
        /// </summary>
        public const uint PlayMode = 0x50000002;

        /// <summary>
        /// The now playing.
        /// </summary>
        public const uint NowPlaying = 0x50000003;

        /// <summary>
        /// The current position.
        /// </summary>
        public const uint CurrentPosition = 0x50000007;

        /// <summary>
        /// The notify end of stream.
        /// </summary>
        public const uint NotifyEndOfStream = 0x50000009;

        /// <summary>
        /// The disconnect.
        /// </summary>
        public const uint Disconnect = 0x5000000B;

        /// <summary>
        /// The version.
        /// </summary>
        public const uint Version = 0x5000000A;

        /// <summary>
        /// The open file.
        /// </summary>
        public const uint OpenFile = 0xA0000000;

        /// <summary>
        /// The stop.
        /// </summary>
        public const uint Stop = 0xA0000001;

        /// <summary>
        /// The play.
        /// </summary>
        public const uint Play = 0xA0000004;

        /// <summary>
        /// The pause.
        /// </summary>
        public const uint Pause = 0xA0000005;

        /// <summary>
        /// The set position.
        /// </summary>
        public const uint SetPosition = 0xA0002000;

        /// <summary>
        /// The set subtitle track.
        /// </summary>
        public const uint SetSubtitleTrack = 0xA0002005;

        /// <summary>
        /// The get current position.
        /// </summary>
        public const uint GetCurrentPosition = 0xA0003004;

        /// <summary>
        /// The increase volume.
        /// </summary>
        public const uint IncreaseVolume = 0xA0004003;

        /// <summary>
        /// The decrease volume.
        /// </summary>
        public const uint DecreaseVolume = 0xA0004004;

        /// <summary>
        /// The close application.
        /// </summary>
        public const uint CloseApplication = 0xA0004006;

        /// <summary>
        /// The set speed.
        /// </summary>
        public const uint SetSpeed = 0xA0004008;
    }
}