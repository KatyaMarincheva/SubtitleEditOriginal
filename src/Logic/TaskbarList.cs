// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskbarList.cs" company="">
//   
// </copyright>
// <summary>
//   Windows 7+ taskbar list - http://msdn.microsoft.com/en-us/library/windows/desktop/dd391692%28v=vs.85%29.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Windows 7+ taskbar list - http://msdn.microsoft.com/en-us/library/windows/desktop/dd391692%28v=vs.85%29.aspx
    /// </summary>
    public static class TaskbarList
    {
        /// <summary>
        /// The supported lazy.
        /// </summary>
        private static readonly Lazy<bool> SupportedLazy = new Lazy<bool>(() => Environment.OSVersion.Version >= new Version(6, 1));

        /// <summary>
        /// The taskbar list lazy.
        /// </summary>
        private static readonly Lazy<ITaskbarList3> TaskbarListLazy = new Lazy<ITaskbarList3>(() => (ITaskbarList3)new CLSID_TaskbarList());

        /// <summary>
        /// Gets a value indicating whether supported.
        /// </summary>
        public static bool Supported
        {
            get
            {
                return SupportedLazy.Value;
            }
        }

        /// <summary>
        /// Gets the taskbar.
        /// </summary>
        internal static ITaskbarList3 Taskbar
        {
            get
            {
                return TaskbarListLazy.Value;
            }
        }

        /// <summary>
        /// The mark fullscreen window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="fullScreen">
        /// The full screen.
        /// </param>
        public static void MarkFullscreenWindow(IntPtr hwnd, bool fullScreen)
        {
            if (Supported && hwnd != IntPtr.Zero)
            {
                Taskbar.MarkFullscreenWindow(hwnd, fullScreen ? 1 : 0);
            }
        }

        /// <summary>
        /// The set progress state.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        public static void SetProgressState(IntPtr hwnd, TaskbarButtonProgressFlags state)
        {
            if (Supported && hwnd != IntPtr.Zero)
            {
                Taskbar.SetProgressState(hwnd, state);
            }
        }

        /// <summary>
        /// The set progress value.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        public static void SetProgressValue(IntPtr hwnd, double value, double max)
        {
            if (Supported && hwnd != IntPtr.Zero)
            {
                Taskbar.SetProgressValue(hwnd, (ulong)value, (ulong)max);
            }
        }

        /// <summary>
        /// The clsi d_ taskbar list.
        /// </summary>
        [ClassInterface(ClassInterfaceType.None)]
        [ComImport]
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        private class CLSID_TaskbarList
        {
        }
    }

    /// <summary>Extends ITaskbarList2 by exposing methods that support the unified launching and switching
    /// taskbar button functionality added in Windows 7. This functionality includes thumbnail representations
    /// and switch targets based on individual tabs in a tabbed application, thumbnail toolbars, notification
    /// and status overlays, and progress indicators.</summary>
    [ComImport]
    [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITaskbarList3
    {
        // ITaskbarList

        /// <summary>Initializes the taskbar list object. This method must be
        /// called before any other ITaskbarList methods can be called.</summary>
        void HrInit();

        /// <summary>
        /// Adds an item to the taskbar.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window to be
        /// added to the taskbar.
        /// </param>
        void AddTab(IntPtr hWnd);

        /// <summary>
        /// Deletes an item from the taskbar.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window to be deleted
        /// from the taskbar.
        /// </param>
        void DeleteTab(IntPtr hWnd);

        /// <summary>
        /// Activates an item on the taskbar. The window is not actually activated;
        /// the window’s item on the taskbar is merely displayed as active.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window on the taskbar to be displayed as active.
        /// </param>
        void ActivateTab(IntPtr hWnd);

        /// <summary>
        /// Marks a taskbar item as active but does not visually activate it.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window to be marked as active.
        /// </param>
        void SetActiveAlt(IntPtr hWnd);

        // ITaskbarList2

        /// <summary>
        /// Marks a window as full-screen
        /// </summary>
        /// <param name="hWnd">
        /// </param>
        /// <param name="fullscreen">
        /// </param>
        void MarkFullscreenWindow(IntPtr hWnd, int fullscreen);

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button to show
        /// the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window whose associated taskbar button is being used as
        /// a progress indicator.
        /// </param>
        /// <param name="ullCompleted">
        /// An application-defined value that indicates the proportion of the
        /// operation that has been completed at the time the method is called.
        /// </param>
        /// <param name="ullTotal">
        /// An application-defined value that specifies the value ullCompleted will
        /// have when the operation is complete.
        /// </param>
        void SetProgressValue(IntPtr hWnd, ulong ullCompleted, ulong ullTotal);

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window in which the progress of an operation is being
        /// shown. This window’s associated taskbar button will display the progress bar.
        /// </param>
        /// <param name="tbpFlags">
        /// Flags that control the current state of the progress button. Specify
        /// only one of the following flags; all states are mutually exclusive of all others.
        /// </param>
        void SetProgressState(IntPtr hWnd, TaskbarButtonProgressFlags tbpFlags);

        /// <summary>
        /// Informs the taskbar that a new tab or document thumbnail has been provided for
        /// display in an application’s taskbar group flyout.
        /// </summary>
        /// <param name="hWndTab">
        /// Handle of the tab or document window. This value is required and cannot
        /// be NULL.
        /// </param>
        /// <param name="hWndMDI">
        /// Handle of the application’s main window. This value tells the taskbar
        /// which application’s preview group to attach the new thumbnail to. This value is required and
        /// cannot be NULL.
        /// </param>
        void RegisterTab(IntPtr hWndTab, IntPtr hWndMDI);

        /// <summary>
        /// Removes a thumbnail from an application’s preview group when that tab or document is closed in the application.
        /// </summary>
        /// <param name="hWndTab">
        /// The handle of the tab window whose thumbnail is being removed. This is the same
        /// value with which the thumbnail was registered as part the group through ITaskbarList3::RegisterTab.
        /// This value is required and cannot be NULL.
        /// </param>
        void UnregisterTab(IntPtr hWndTab);

        /// <summary>
        /// Inserts a new thumbnail into a tabbed-document interface (TDI) or multiple-document
        /// interface (MDI) application’s group flyout or moves an existing thumbnail to a new position in
        /// the application’s group.
        /// </summary>
        /// <param name="hWndTab">
        /// The handle of the tab window whose thumbnail is being placed. This value
        /// is required, must already be registered through ITaskbarList3::RegisterTab, and cannot be NULL.
        /// </param>
        /// <param name="hWndInsertBefore">
        /// The handle of the tab window whose thumbnail that hwndTab is
        /// inserted to the left of. This handle must already be registered through ITaskbarList3::RegisterTab.
        /// If this value is NULL, the new thumbnail is added to the end of the list.
        /// </param>
        void SetTabOrder(IntPtr hWndTab, IntPtr hWndInsertBefore);

        /// <summary>
        /// Informs the taskbar that a tab or document window has been made the active window.
        /// </summary>
        /// <param name="hWndTab">
        /// Handle of the active tab window. This handle must already be registered
        /// through ITaskbarList3::RegisterTab. This value can be NULL if no tab is active.
        /// </param>
        /// <param name="hWndMDI">
        /// Handle of the application’s main window. This value tells the taskbar
        /// which group the thumbnail is a member of. This value is required and cannot be NULL.
        /// </param>
        /// <param name="tbatFlags">
        /// None, one, or both of the following values that specify a thumbnail
        /// and peek view to use in place of a representation of the specific tab or document.
        /// </param>
        void SetTabActive(IntPtr hWndTab, IntPtr hWndMDI, uint tbatFlags);

        /// <summary>
        /// Adds a thumbnail toolbar with a specified set of buttons to the thumbnail image of a window
        /// in a taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window whose thumbnail representation will receive the toolbar.
        /// This handle must belong to the calling process.
        /// </param>
        /// <param name="cButtons">
        /// The number of buttons defined in the array pointed to by pButton. The maximum
        /// number of buttons allowed is 7.
        /// </param>
        /// <param name="pButton">
        /// A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an
        /// individual button to be added to the toolbar. Buttons cannot be added or deleted later, so this must
        /// be the full defined set. Buttons also cannot be reordered, so their order in the array, which is the
        /// order in which they are displayed left to right, will be their permanent order.
        /// </param>
        void ThumbBarAddButtons(IntPtr hWnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButton);

        /// <summary>
        /// Shows, enables, disables, or hides buttons in a thumbnail toolbar as required by the
        /// window’s current state. A thumbnail toolbar is a toolbar embedded in a thumbnail image of a window
        /// in a taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window whose thumbnail representation contains the toolbar.
        /// </param>
        /// <param name="cButtons">
        /// The number of buttons defined in the array pointed to by pButton.
        /// The maximum number of buttons allowed is 7. This array contains only structures that represent existing buttons that are being updated.
        /// </param>
        /// <param name="pButton">
        /// A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an individual button. If the button already exists
        /// (the iId value is already defined), then that existing button is updated with the information provided in the structure.
        /// </param>
        void ThumbBarUpdateButtons(IntPtr hWnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButton);

        /// <summary>
        /// Specifies an image list that contains button images for a toolbar embedded in
        /// a thumbnail image of a window in a taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window whose thumbnail representation contains the
        /// toolbar to be updated. This handle must belong to the calling process.
        /// </param>
        /// <param name="himl">
        /// The handle of the image list that contains all button images to be used in the toolbar.
        /// </param>
        void ThumbBarSetImageList(IntPtr hWnd, IntPtr himl);

        /// <summary>
        /// Applies an overlay to a taskbar button to indicate application status or a notification to the user.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window whose associated taskbar button receives the overlay.
        /// This handle must belong to a calling process associated with the button’s application and must be
        /// a valid HWND or the call is ignored.
        /// </param>
        /// <param name="hIcon">
        /// The handle of an icon to use as the overlay. This should be a small icon,
        /// measuring 16×16 pixels at 96 dots per inch (dpi). If an overlay icon is already applied to the
        /// taskbar button, that existing overlay is replaced.
        /// </param>
        /// <param name="pszDescription">
        /// A pointer to a string that provides an alt text version of the
        /// information conveyed by the overlay, for accessibility purposes.
        /// </param>
        void SetOverlayIcon(IntPtr hWnd, IntPtr hIcon, string pszDescription);

        /// <summary>
        /// Specifies or updates the text of the tooltip that is displayed when the mouse
        /// pointer rests on an individual preview thumbnail in a taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">
        /// The handle to the window whose thumbnail displays the tooltip.
        /// This handle must belong to the calling process.
        /// </param>
        /// <param name="pszTip">
        /// The pointer to the text to be displayed in the tooltip. This value can
        /// be NULL, in which case the title of the window specified by hwnd is used as the tooltip.
        /// </param>
        void SetThumbnailTooltip(IntPtr hWnd, string pszTip);

        /// <summary>
        /// Selects a portion of a window’s client area to display as that window’s thumbnail in the taskbar.
        /// </summary>
        /// <param name="hWnd">
        /// The handle to a window represented in the taskbar.
        /// </param>
        /// <param name="prcClip">
        /// A pointer to a RECT structure that specifies a selection within the window’s
        /// client area, relative to the upper-left corner of that client area. To clear a clip that is already
        /// in place and return to the default display of the thumbnail, set this parameter to NULL.
        /// </param>
        void SetThumbnailClip(IntPtr hWnd, IntPtr prcClip);
    }

    /// <summary>
    /// The thumb button mask.
    /// </summary>
    internal enum ThumbButtonMask
    {
        /// <summary>
        /// The bitmap.
        /// </summary>
        Bitmap = 0x1, 

        /// <summary>
        /// The icon.
        /// </summary>
        Icon = 0x2, 

        /// <summary>
        /// The tooltip.
        /// </summary>
        Tooltip = 0x4, 

        /// <summary>
        /// The flags.
        /// </summary>
        Flags = 0x8
    }

    /// <summary>
    /// The thumb button.
    /// </summary>
    public struct ThumbButton
    {
#pragma warning disable 0169

        /// <summary>
        /// The _mask.
        /// </summary>
        private ThumbButtonMask _mask;

        /// <summary>
        /// The _id.
        /// </summary>
        private uint _id;

        /// <summary>
        /// The _bitmap.
        /// </summary>
        private uint _bitmap;

        /// <summary>
        /// The _icon.
        /// </summary>
        private IntPtr _icon;

        /// <summary>
        /// The tip.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string Tip;

        /// <summary>
        /// The _flags.
        /// </summary>
        private ThumbButtonFlags _flags;

#pragma warning restore 0169
    }

    /// <summary>
    /// The thumb button flags.
    /// </summary>
    [Flags]
    internal enum ThumbButtonFlags
    {
        /// <summary>
        /// The enabled.
        /// </summary>
        Enabled = 0, 

        /// <summary>
        /// The disabled.
        /// </summary>
        Disabled = 0x1, 

        /// <summary>
        /// The dismission click.
        /// </summary>
        DismissionClick = 0x2, 

        /// <summary>
        /// The no background.
        /// </summary>
        NoBackground = 0x4, 

        /// <summary>
        /// The hidden.
        /// </summary>
        Hidden = 0x8, 

        /// <summary>
        /// The non interactive.
        /// </summary>
        NonInteractive = 0x10
    }

    /// <summary>
    /// The taskbar button progress flags.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum TaskbarButtonProgressFlags
    {
        /// <summary>
        /// The no progress.
        /// </summary>
        NoProgress = 0, 

        /// <summary>
        /// The indeterminate.
        /// </summary>
        Indeterminate = 0x1, 

        /// <summary>
        /// The normal.
        /// </summary>
        Normal = 0x2, 

        /// <summary>
        /// The error.
        /// </summary>
        Error = 0x4, 

        /// <summary>
        /// The paused.
        /// </summary>
        Paused = 0x8
    }
}