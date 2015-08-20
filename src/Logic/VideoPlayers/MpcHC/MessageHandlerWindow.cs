// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerWindow.cs" company="">
//   
// </copyright>
// <summary>
//   The message handler window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VideoPlayers.MpcHC
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The message handler window.
    /// </summary>
    public partial class MessageHandlerWindow : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerWindow"/> class.
        /// </summary>
        public MessageHandlerWindow()
        {
            this.InitializeComponent();
            this.Text = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// The on copy data.
        /// </summary>
        public event EventHandler OnCopyData;

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WindowsMessageCopyData && this.OnCopyData != null)
            {
                this.OnCopyData.Invoke(m, new EventArgs());
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            base.OnLoad(e);
        }
    }
}