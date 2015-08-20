// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoError.cs" company="">
//   
// </copyright>
// <summary>
//   The video error.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The video error.
    /// </summary>
    public partial class VideoError : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoError"/> class.
        /// </summary>
        public VideoError()
        {
            this.InitializeComponent();
            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets a value indicating whether is 64 bit os.
        /// </summary>
        public static bool Is64BitOS
        {
            get
            {
                return false;
            }

            // get { return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null); }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="videoInfo">
        /// The video info.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Initialize(string fileName, VideoInfo videoInfo, Exception exception)
        {
            bool isWindowsXpOrVista = Environment.OSVersion.Version.Major < 7;

            var sb = new StringBuilder();
            sb.AppendLine("There seems to be missing a codec (or file is not a valid video file)!");
            sb.AppendLine();
            if (Configuration.Settings.General.VideoPlayer != "VLC")
            {
                sb.AppendLine("You need to install/update LAV Filters - DirectShow Media Splitter and Decoders: http://code.google.com/p/lavfilters/");
                sb.AppendLine();
                sb.AppendLine("NOTE: Subtitle Edit can also use VLC media player as built-in video player. See Options -> Settings -> Video Player");
                sb.AppendLine("http://www.videolan.org/vlc/");
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("VLC media player was unable to play this file - you can change video player via Options -> Settings -> Video Player");
                sb.AppendLine("Latest version of VLC is available here: http://www.videolan.org/vlc/");
                sb.AppendLine();
            }

            if (videoInfo != null && !string.IsNullOrEmpty(videoInfo.VideoCodec))
            {
                sb.AppendLine(string.Format("The file {0} is encoded with {1}!", Path.GetFileName(fileName), videoInfo.VideoCodec.Replace('\0', ' ')));
                sb.AppendLine(string.Empty);
            }

            sb.AppendLine(string.Empty);

            sb.AppendLine("You can read more about codecs and media formats here:");
            sb.AppendLine(" - http://www.free-codecs.com/guides/What_Codecs_Should_I_Use.htm");
            sb.AppendLine(string.Empty);

            sb.AppendLine("Other useful utilities:");
            sb.AppendLine(" - http://mediaarea.net/MediaInfo");
            sb.AppendLine(" - http://www.free-codecs.com/download/Codec_Tweak_Tool.htm");
            sb.AppendLine(" - http://www.free-codecs.com/download/DirectShow_Filter_Manager.htm");
            sb.AppendLine(" - http://www.headbands.com/gspot/");

            sb.AppendLine(string.Empty);
            sb.Append("Note that Subtitle Edit is a " + (IntPtr.Size * 8) + "-bit program, and hence requires " + (IntPtr.Size * 8) + "-bit codecs!");

            this.richTextBoxMessage.Text = sb.ToString();

            if (exception != null)
            {
                this.textBoxError.Text = "Message: " + exception.Message + Environment.NewLine + "Source: " + exception.Source + Environment.NewLine + Environment.NewLine + "Stack Trace: " + Environment.NewLine + exception.StackTrace;
            }

            this.Text += fileName;
        }

        /// <summary>
        /// The video error_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoError_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The rich text box message_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}