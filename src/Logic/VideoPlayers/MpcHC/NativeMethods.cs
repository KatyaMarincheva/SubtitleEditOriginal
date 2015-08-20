// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="">
//   
// </copyright>
// <summary>
//   The native methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers.MpcHC
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The enumed window.
        /// </summary>
        /// <param name="handleWindow">
        /// The handle window.
        /// </param>
        /// <param name="handles">
        /// The handles.
        /// </param>
        internal delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        #region structs, constants and enums

        /// <summary>
        /// The copy data struct.
        /// </summary>
        public struct CopyDataStruct
        {
            /// <summary>
            /// The cb data.
            /// </summary>
            public int cbData;

            /// <summary>
            /// The dw data.
            /// </summary>
            public UIntPtr dwData;

            /// <summary>
            /// The lp data.
            /// </summary>
            public IntPtr lpData;
        }

        /// <summary>
        /// The special window handles.
        /// </summary>
        public enum SpecialWindowHandles
        {
            /// <summary>
            /// The hwn d_ top.
            /// </summary>
            HWND_TOP = 0, 

            /// <summary>
            /// The hwn d_ bottom.
            /// </summary>
            HWND_BOTTOM = 1, 

            /// <summary>
            /// The hwn d_ topmost.
            /// </summary>
            HWND_TOPMOST = -1, 

            /// <summary>
            /// The hwn d_ notopmost.
            /// </summary>
            HWND_NOTOPMOST = -2
        }

        /// <summary>
        /// The set window pos flags.
        /// </summary>
        [Flags]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>
            /// The sw p_ asyncwindowpos.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000, 

            /// <summary>
            /// The sw p_ defererase.
            /// </summary>
            SWP_DEFERERASE = 0x2000, 

            /// <summary>
            /// The sw p_ drawframe.
            /// </summary>
            SWP_DRAWFRAME = 0x0020, 

            /// <summary>
            /// The sw p_ framechanged.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020, 

            /// <summary>
            /// The sw p_ hidewindow.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080, 

            /// <summary>
            /// The sw p_ noactivate.
            /// </summary>
            SWP_NOACTIVATE = 0x0010, 

            /// <summary>
            /// The sw p_ nocopybits.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100, 

            /// <summary>
            /// The sw p_ nomove.
            /// </summary>
            SWP_NOMOVE = 0x0002, 

            /// <summary>
            /// The sw p_ noownerzorder.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200, 

            /// <summary>
            /// The sw p_ noredraw.
            /// </summary>
            SWP_NOREDRAW = 0x0008, 

            /// <summary>
            /// The sw p_ noreposition.
            /// </summary>
            SWP_NOREPOSITION = 0x0200, 

            /// <summary>
            /// The sw p_ nosendchanging.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400, 

            /// <summary>
            /// The sw p_ nosize.
            /// </summary>
            SWP_NOSIZE = 0x0001, 

            /// <summary>
            /// The sw p_ nozorder.
            /// </summary>
            SWP_NOZORDER = 0x0004, 

            /// <summary>
            /// The sw p_ showwindow.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040, 
        }

        /// <summary>
        /// The show window commands.
        /// </summary>
        public enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0, 

            /// <summary>
            /// Activates and displays a window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window
            /// for the first time.
            /// </summary>
            Normal = 1, 

            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2, 

            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?

            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            ShowMaximized = 3, 

            /// <summary>
            /// Displays a window in its most recent size and position. This value
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4, 

            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5, 

            /// <summary>
            /// Minimizes the specified window and activates the next top-level
            /// window in the Z order.
            /// </summary>
            Minimize = 6, 

            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7, 

            /// <summary>
            /// Displays the window in its current size and position. This value is
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
            /// window is not activated.
            /// </summary>
            ShowNA = 8, 

            /// <summary>
            /// Activates and displays the window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9, 

            /// <summary>
            /// Sets the show state based on the SW_* value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.
            /// </summary>
            ShowDefault = 10, 

            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            /// that owns the window is not responding. This flag should only be
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        /// <summary>
        /// The windows message copy data.
        /// </summary>
        public const int WindowsMessageCopyData = 0x4A;

        #endregion structs, constants and enums

        #region Win32 API

        /// <summary>
        /// The show window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="nCmdShow">
        /// The n cmd show.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        /// <summary>
        /// The set window pos.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="hWndInsertAfter">
        /// The h wnd insert after.
        /// </param>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="Y">
        /// The y.
        /// </param>
        /// <param name="cx">
        /// The cx.
        /// </param>
        /// <param name="cy">
        /// The cy.
        /// </param>
        /// <param name="uFlags">
        /// The u flags.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <summary>
        /// The enum windows.
        /// </summary>
        /// <param name="lpEnumFunc">
        /// The lp enum func.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);

        /// <summary>
        /// The enum child windows.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumChildWindows(IntPtr window, EnumedWindow callback, ArrayList lParam);

        /// <summary>
        /// The get class name.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="nMaxCount">
        /// The n max count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="Msg">
        /// The msg.
        /// </param>
        /// <param name="wParam">
        /// The w param.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref CopyDataStruct lParam);

        /// <summary>
        /// The set parent.
        /// </summary>
        /// <param name="hWndChild">
        /// The h wnd child.
        /// </param>
        /// <param name="hWndNewParent">
        /// The h wnd new parent.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        #endregion Win32 API
    }
}