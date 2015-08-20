// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Forms;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The 
        /// entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += Application_ThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        /// <summary>
        /// The application_ thread exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (!(e.Exception is System.Globalization.CultureNotFoundException))
            {
                // To avoid error when changing language (on some computers) - see https://github.com/SubtitleEdit/subtitleedit/issues/719
                ShowThreadExceptionDialog("Unhandled exception in SubtitleEdit.exe", e.Exception);
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only
        // log the event, and inform the user about it.
        /// <summary>
        /// The current domain_ unhandled exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = (Exception)e.ExceptionObject;
                const string errorMsg = "An error occurred in Subtitle Edit. Please contact the adminstrator with the following information:\n\n";

                // Since we can't prevent the app from terminating, log this to the event log.
                if (!EventLog.SourceExists("ThreadException"))
                {
                    EventLog.CreateEventSource("ThreadException", "Application");
                }

                // Create an EventLog instance and assign its source.
                using (var eventLog = new EventLog { Source = "ThreadException" })
                {
                    eventLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
                }
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error in Subtitle Edit", "Fatal Non-UI Error. Could not write the error to the event log. Reason: " + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// The show thread exception dialog.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ShowThreadExceptionDialog(string title, Exception e)
        {
            var errorMsg = "An application error occurred. Please contact the adminstrator with the following information:\n\n" + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
        }
    }
}