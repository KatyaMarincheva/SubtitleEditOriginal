// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpcHc.cs" company="">
//   
// </copyright>
// <summary>
//   The mpc hc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers.MpcHC
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The mpc hc.
    /// </summary>
    public class MpcHc : VideoPlayer, IDisposable
    {
        /// <summary>
        /// The mode play.
        /// </summary>
        private const string ModePlay = "0";

        /// <summary>
        /// The mode pause.
        /// </summary>
        private const string ModePause = "1";

        /// <summary>
        /// The state loaded.
        /// </summary>
        private const string StateLoaded = "2";

        /// <summary>
        /// The _duration in seconds.
        /// </summary>
        private double _durationInSeconds = 0;

        /// <summary>
        /// The _form.
        /// </summary>
        private MessageHandlerWindow _form;

        /// <summary>
        /// The _initial height.
        /// </summary>
        private int _initialHeight;

        /// <summary>
        /// The _initial width.
        /// </summary>
        private int _initialWidth;

        /// <summary>
        /// The _loaded.
        /// </summary>
        private int _loaded = 0;

        /// <summary>
        /// The _message handler handle.
        /// </summary>
        private IntPtr _messageHandlerHandle = IntPtr.Zero;

        /// <summary>
        /// The _mpc handle.
        /// </summary>
        private IntPtr _mpcHandle = IntPtr.Zero;

        /// <summary>
        /// The _play mode.
        /// </summary>
        private string _playMode = string.Empty;

        /// <summary>
        /// The _position in seconds.
        /// </summary>
        private double _positionInSeconds = 0;

        /// <summary>
        /// The _position timer.
        /// </summary>
        private Timer _positionTimer;

        /// <summary>
        /// The _process.
        /// </summary>
        private Process _process;

        /// <summary>
        /// The _start info.
        /// </summary>
        private ProcessStartInfo _startInfo;

        /// <summary>
        /// The _video file name.
        /// </summary>
        private string _videoFileName;

        /// <summary>
        /// The _video handle.
        /// </summary>
        private IntPtr _videoHandle = IntPtr.Zero;

        /// <summary>
        /// The _video panel handle.
        /// </summary>
        private IntPtr _videoPanelHandle = IntPtr.Zero;

        /// <summary>
        /// The _volume.
        /// </summary>
        private int _volume = 75;

        /// <summary>
        /// Gets the player name.
        /// </summary>
        public override string PlayerName
        {
            get
            {
                return "MPC-HC";
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public override int Volume
        {
            get
            {
                return this._volume;
            }

            set
            {
                // MPC-HC moves from 0-100 in steps of 5
                for (int i = 0; i < 100; i += 5)
                {
                    this.SendMpcMessage(MpcHcCommand.DecreaseVolume);
                }

                for (int _volume = 0; _volume < value; _volume += 5)
                {
                    this.SendMpcMessage(MpcHcCommand.IncreaseVolume);
                }
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public override double Duration
        {
            get
            {
                return this._durationInSeconds;
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public override double CurrentPosition
        {
            get
            {
                return this._positionInSeconds;
            }

            set
            {
                this.SendMpcMessage(MpcHcCommand.SetPosition, string.Format(CultureInfo.InvariantCulture, "{0:0.000}", value));
            }
        }

        /// <summary>
        /// Gets a value indicating whether is paused.
        /// </summary>
        public override bool IsPaused
        {
            get
            {
                return this._playMode == ModePause;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                return this._playMode == ModePlay;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is installed.
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The play.
        /// </summary>
        public override void Play()
        {
            this._playMode = ModePlay;
            this.SendMpcMessage(MpcHcCommand.Play);
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public override void Pause()
        {
            this._playMode = ModePause;
            this.SendMpcMessage(MpcHcCommand.Pause);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public override void Stop()
        {
            this.SendMpcMessage(MpcHcCommand.Stop);
        }

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
        public override void Initialize(Control ownerControl, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded)
        {
            if (ownerControl == null)
            {
                return;
            }

            this.VideoFileName = videoFileName;
            this.OnVideoLoaded = onVideoLoaded;
            this.OnVideoEnded = onVideoEnded;

            this._initialWidth = ownerControl.Width;
            this._initialHeight = ownerControl.Height;
            this._form = new MessageHandlerWindow();
            this._form.OnCopyData += this.OnCopyData;
            this._form.Show();
            this._form.Hide();
            this._videoPanelHandle = ownerControl.Handle;
            this._messageHandlerHandle = this._form.Handle;
            this._videoFileName = videoFileName;
            this._startInfo = new ProcessStartInfo();
            this._startInfo.FileName = GetMpcHcFileName();
            this._startInfo.Arguments = "/new /minimized /slave " + this._messageHandlerHandle;
            this._process = Process.Start(this._startInfo);
            this._process.WaitForInputIdle();
            this._positionTimer = new Timer();
            this._positionTimer.Interval = 100;
            this._positionTimer.Tick += this.PositionTimerTick;
        }

        /// <summary>
        /// The position timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PositionTimerTick(object sender, EventArgs e)
        {
            this.SendMpcMessage(MpcHcCommand.GetCurrentPosition);
        }

        /// <summary>
        /// The on copy data.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnCopyData(object sender, EventArgs e)
        {
            var message = (Message)sender;
            var cds = (NativeMethods.CopyDataStruct)Marshal.PtrToStructure(message.LParam, typeof(NativeMethods.CopyDataStruct));
            var command = cds.dwData.ToUInt32();
            var param = Marshal.PtrToStringAuto(cds.lpData);
            var multiParam = param.Split('|');

            switch (cds.dwData.ToUInt32())
            {
                case MpcHcCommand.Connect:
                    this._positionTimer.Stop();
                    this._mpcHandle = (IntPtr)Convert.ToInt64(Marshal.PtrToStringAuto(cds.lpData));
                    this.SendMpcMessage(MpcHcCommand.OpenFile, this._videoFileName);
                    this._positionTimer.Start();
                    break;
                case MpcHcCommand.PlayMode:
                    this._playMode = param;
                    if (param == ModePlay && this._loaded == 0)
                    {
                        this._loaded = 1;
                        if (!this.HijackMpcHc())
                        {
                            Application.DoEvents();
                            this.HijackMpcHc();
                        }
                    }

                    break;
                case MpcHcCommand.NowPlaying:
                    if (this._loaded == 1)
                    {
                        this._loaded = 2;
                        this._durationInSeconds = double.Parse(multiParam[4], CultureInfo.InvariantCulture);
                        this.Pause();
                        this.Resize(this._initialWidth, this._initialHeight);
                        if (this.OnVideoLoaded != null)
                        {
                            this.OnVideoLoaded.Invoke(this, new EventArgs());
                        }

                        this.SendMpcMessage(MpcHcCommand.SetSubtitleTrack, "-1");
                    }

                    break;
                case MpcHcCommand.NotifyEndOfStream:
                    if (this.OnVideoEnded != null)
                    {
                        this.OnVideoEnded.Invoke(this, new EventArgs());
                    }

                    break;
                case MpcHcCommand.CurrentPosition:
                    this._positionInSeconds = double.Parse(param, CultureInfo.InvariantCulture);
                    break;
            }
        }

        /// <summary>
        /// The get window handle.
        /// </summary>
        /// <param name="windowHandle">
        /// The window handle.
        /// </param>
        /// <param name="windowHandles">
        /// The window handles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }

        /// <summary>
        /// The get child windows.
        /// </summary>
        /// <returns>
        /// The <see cref="ArrayList"/>.
        /// </returns>
        private ArrayList GetChildWindows()
        {
            var windowHandles = new ArrayList();
            NativeMethods.EnumedWindow callBackPtr = GetWindowHandle;
            NativeMethods.EnumChildWindows(this._process.MainWindowHandle, callBackPtr, windowHandles);
            return windowHandles;
        }

        /// <summary>
        /// The is window mpc hc video.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsWindowMpcHcVideo(IntPtr hWnd)
        {
            var className = new StringBuilder(256);
            int returnCode = NativeMethods.GetClassName(hWnd, className, className.Capacity); // Get the window class name
            if (returnCode != 0)
            {
                return className.ToString().EndsWith(":b:0000000000010003:0000000000000006:0000000000000000"); // MPC-HC video class???
            }

            return false;
        }

        /// <summary>
        /// The hijack mpc hc.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HijackMpcHc()
        {
            IntPtr handle = this._process.MainWindowHandle;
            var handles = this.GetChildWindows();
            foreach (var h in handles)
            {
                if (IsWindowMpcHcVideo((IntPtr)h))
                {
                    this._videoHandle = (IntPtr)h;
                    NativeMethods.SetParent((IntPtr)h, this._videoPanelHandle);
                    NativeMethods.SetWindowPos(handle, (IntPtr)NativeMethods.SpecialWindowHandles.HWND_TOP, -9999, -9999, 0, 0, NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The resize.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public override void Resize(int width, int height)
        {
            NativeMethods.ShowWindow(this._process.MainWindowHandle, NativeMethods.ShowWindowCommands.ShowNoActivate);
            NativeMethods.SetWindowPos(this._videoHandle, (IntPtr)NativeMethods.SpecialWindowHandles.HWND_TOP, 0, 0, width, height, NativeMethods.SetWindowPosFlags.SWP_NOREPOSITION);
            NativeMethods.ShowWindow(this._process.MainWindowHandle, NativeMethods.ShowWindowCommands.Hide);
        }

        /// <summary>
        /// The get mpc hc file name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetMpcHcFileName()
        {
            string path;

            if (IntPtr.Size == 8)
            {
                // 64-bit
                path = Path.Combine(Configuration.BaseDirectory, @"MPC-HC\mpc-hc64.exe");
                if (File.Exists(path))
                {
                    return path;
                }

                if (!string.IsNullOrEmpty(Configuration.Settings.General.MpcHcLocation))
                {
                    path = Path.GetDirectoryName(Configuration.Settings.General.MpcHcLocation);
                    if (File.Exists(path) && path.EndsWith("mpc-hc64.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        return path;
                    }

                    if (Directory.Exists(Configuration.Settings.General.MpcHcLocation))
                    {
                        path = Path.Combine(Configuration.Settings.General.MpcHcLocation, @"MPC-HC\mpc-hc64.exe");
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }

                path = RegistryUtil.GetValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{2ACBF1FA-F5C3-4B19-A774-B22A31F231B9}_is1", "InstallLocation");
                if (path != null)
                {
                    path = Path.Combine(path, "mpc-hc64.exe");
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"MPC-HC\mpc-hc64.exe");
                if (File.Exists(path))
                {
                    return path;
                }

                path = @"C:\Program Files\MPC-HC\mpc-hc64.exe";
                if (File.Exists(path))
                {
                    return path;
                }

                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"K-Lite Codec Pack\MPC-HC\mpc-hc64.exe");
                if (File.Exists(path))
                {
                    return path;
                }

                path = @"C:\Program Files (x86)\K-Lite Codec Pack\MPC-HC64\mpc-hc64.exe";
                if (File.Exists(path))
                {
                    return path;
                }

                path = @"C:\Program Files (x86)\MPC-HC\mpc-hc64.exe";
                if (File.Exists(path))
                {
                    return path;
                }
            }
            else
            {
                path = Path.Combine(Configuration.BaseDirectory, @"MPC-HC\mpc-hc.exe");
                if (File.Exists(path))
                {
                    return path;
                }

                if (!string.IsNullOrEmpty(Configuration.Settings.General.MpcHcLocation))
                {
                    path = Path.GetDirectoryName(Configuration.Settings.General.MpcHcLocation);
                    if (File.Exists(path) && path.EndsWith("mpc-hc.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        return path;
                    }

                    if (Directory.Exists(Configuration.Settings.General.MpcHcLocation))
                    {
                        path = Path.Combine(Configuration.Settings.General.MpcHcLocation, @"MPC-HC\mpc-hc.exe");
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }

                path = RegistryUtil.GetValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{2624B969-7135-4EB1-B0F6-2D8C397B45F7}_is1", "InstallLocation");
                if (path != null)
                {
                    path = Path.Combine(path, "mpc-hc.exe");
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"MPC-HC\mpc-hc.exe");
                if (File.Exists(path))
                {
                    return path;
                }

                path = @"C:\Program Files (x86)\MPC-HC\mpc-hc.exe";
                if (File.Exists(path))
                {
                    return path;
                }

                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"K-Lite Codec Pack\MPC-HC\mpc-hc.exe");
                if (File.Exists(path))
                {
                    return path;
                }

                path = @"C:\Program Files\MPC-HC\mpc-hc.exe";
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// The dispose video player.
        /// </summary>
        public override void DisposeVideoPlayer()
        {
            this.Dispose();
        }

        /// <summary>
        /// The on video loaded.
        /// </summary>
        public override event EventHandler OnVideoLoaded;

        /// <summary>
        /// The on video ended.
        /// </summary>
        public override event EventHandler OnVideoEnded;

        /// <summary>
        /// The release unmanged resources.
        /// </summary>
        private void ReleaseUnmangedResources()
        {
            try
            {
                lock (this)
                {
                    if (this._mpcHandle != IntPtr.Zero)
                    {
                        this.SendMpcMessage(MpcHcCommand.CloseApplication);
                        this._mpcHandle = IntPtr.Zero;
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MpcHc"/> class. 
        /// </summary>
        ~MpcHc()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // release managed resources
                    if (this._positionTimer != null)
                    {
                        this._positionTimer.Stop();
                        this._positionTimer.Dispose();
                        this._positionTimer = null;
                    }

                    if (this._form != null)
                    {
                        this._form.OnCopyData -= this.OnCopyData;

                        // _form.Dispose(); this gives an error when doing File -> Exit...
                        this._form = null;
                    }

                    if (this._process != null)
                    {
                        this._process.Dispose();
                        this._process = null;
                    }

                    this._startInfo = null;
                }

                this.ReleaseUnmangedResources();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// The send mpc message.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        private void SendMpcMessage(uint command)
        {
            this.SendMpcMessage(command, string.Empty);
        }

        /// <summary>
        /// The send mpc message.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        private void SendMpcMessage(uint command, string parameter)
        {
            if (this._mpcHandle == IntPtr.Zero || this._messageHandlerHandle == IntPtr.Zero)
            {
                return;
            }

            parameter += (char)0;
            NativeMethods.CopyDataStruct cds;
            cds.dwData = (UIntPtr)command;
            cds.cbData = parameter.Length * Marshal.SystemDefaultCharSize;
            cds.lpData = Marshal.StringToCoTaskMemAuto(parameter);
            NativeMethods.SendMessage(this._mpcHandle, NativeMethods.WindowsMessageCopyData, this._messageHandlerHandle, ref cds);
        }
    }
}