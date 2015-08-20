// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="">
//   
// </copyright>
// <summary>
//   The native methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native methods.
    /// </summary>
    internal static class NativeMethods
    {
        #region Hunspell

        /// <summary>
        /// The hunspell_create.
        /// </summary>
        /// <param name="affpath">
        /// The affpath.
        /// </param>
        /// <param name="dpath">
        /// The dpath.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("libhunspell", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr Hunspell_create(string affpath, string dpath);

        /// <summary>
        /// The hunspell_destroy.
        /// </summary>
        /// <param name="hunspellHandle">
        /// The hunspell handle.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("libhunspell")]
        internal static extern IntPtr Hunspell_destroy(IntPtr hunspellHandle);

        /// <summary>
        /// The hunspell_spell.
        /// </summary>
        /// <param name="hunspellHandle">
        /// The hunspell handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libhunspell", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern int Hunspell_spell(IntPtr hunspellHandle, string word);

        /// <summary>
        /// The hunspell_suggest.
        /// </summary>
        /// <param name="hunspellHandle">
        /// The hunspell handle.
        /// </param>
        /// <param name="slst">
        /// The slst.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libhunspell", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern int Hunspell_suggest(IntPtr hunspellHandle, IntPtr slst, string word);

        /// <summary>
        /// The hunspell_free_list.
        /// </summary>
        /// <param name="hunspellHandle">
        /// The hunspell handle.
        /// </param>
        /// <param name="slst">
        /// The slst.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        [DllImport("libhunspell")]
        internal static extern void Hunspell_free_list(IntPtr hunspellHandle, IntPtr slst, int n);

        #endregion Hunspell

        #region Win32 API

        // Win32 API functions for dynamically loading DLLs
        /// <summary>
        /// The load library.
        /// </summary>
        /// <param name="dllToLoad">
        /// The dll to load.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr LoadLibrary(string dllToLoad);

        /// <summary>
        /// The get proc address.
        /// </summary>
        /// <param name="hModule">
        /// The h module.
        /// </param>
        /// <param name="procedureName">
        /// The procedure name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        /// <summary>
        /// The free library.
        /// </summary>
        /// <param name="hModule">
        /// The h module.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// The get key state.
        /// </summary>
        /// <param name="vKey">
        /// The v key.
        /// </param>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int vKey);

        /// <summary>
        /// The attach console.
        /// </summary>
        /// <param name="dwProcessId">
        /// The dw process id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AttachConsole(int dwProcessId);

        /// <summary>
        /// The free console.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeConsole();

        /// <summary>
        /// The set window pos.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="hWndInsertAfter">
        /// The h wnd insert after.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="wFlags">
        /// The w flags.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int width, int height, int wFlags);

        #endregion Win32 API

        #region VLC

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
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern IntPtr libvlc_new(int argc, [MarshalAs(UnmanagedType.LPArray)] string[] argv);

        /// <summary>
        /// The libvlc_release.
        /// </summary>
        /// <param name="libVlc">
        /// The lib vlc.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_release(IntPtr libVlc);

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
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern IntPtr libvlc_media_new_path(IntPtr instance, byte[] input);

        /// <summary>
        /// The libvlc_media_player_new_from_media.
        /// </summary>
        /// <param name="media">
        /// The media.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern IntPtr libvlc_media_player_new_from_media(IntPtr media);

        /// <summary>
        /// The libvlc_media_release.
        /// </summary>
        /// <param name="media">
        /// The media.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_release(IntPtr media);

        // LibVLC Audio Controls - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__audio.html
        /// <summary>
        /// The libvlc_audio_get_track_count.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern int libvlc_audio_get_track_count(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_audio_get_track.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern int libvlc_audio_get_track(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_audio_set_track.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="trackNumber">
        /// The track number.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern int libvlc_audio_set_track(IntPtr mediaPlayer, int trackNumber);

        // LibVLC Audio Controls - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__audio.html
        /// <summary>
        /// The libvlc_audio_get_volume.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern int libvlc_audio_get_volume(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_audio_set_volume.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="volume">
        /// The volume.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_audio_set_volume(IntPtr mediaPlayer, int volume);

        // LibVLC media player - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__media__player.html
        /// <summary>
        /// The libvlc_media_player_play.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_player_play(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_stop.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_player_stop(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_pause.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_player_pause(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_set_hwnd.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="windowsHandle">
        /// The windows handle.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_player_set_hwnd(IntPtr mediaPlayer, IntPtr windowsHandle);

        /// <summary>
        /// The libvlc_media_player_get_time.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern long libvlc_media_player_get_time(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_set_time.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_player_set_time(IntPtr mediaPlayer, long position);

        /// <summary>
        /// The libvlc_media_player_get_state.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern byte libvlc_media_player_get_state(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_get_length.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern long libvlc_media_player_get_length(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_list_player_release.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        [DllImport("libvlc")]
        internal static extern void libvlc_media_list_player_release(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_get_rate.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern float libvlc_media_player_get_rate(IntPtr mediaPlayer);

        /// <summary>
        /// The libvlc_media_player_set_rate.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        /// <param name="rate">
        /// The rate.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("libvlc")]
        internal static extern int libvlc_media_player_set_rate(IntPtr mediaPlayer, float rate);

        #endregion VLC
    }
}