// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineRunner.cs" company="">
//   
// </copyright>
// <summary>
//   The command line runner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UpdateAssemblyInfo
{
    using System.Diagnostics;

    /// <summary>
    /// The command line runner.
    /// </summary>
    public class CommandLineRunner
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// The run command and get output.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="workingFolder">
        /// The working folder.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RunCommandAndGetOutput(string command, string arguments, string workingFolder)
        {
            var p = new Process();
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.WorkingDirectory = workingFolder;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += this.OutputDataReceived;

            try
            {
                p.Start();
            }
            catch
            {
                return false;
            }

            p.BeginOutputReadLine(); // Async reading of output to prevent deadlock
            if (p.WaitForExit(5000))
            {
                return p.ExitCode == 0;
            }

            return false;
        }

        /// <summary>
        /// The output data received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null)
            {
                this.Result = e.Data;
            }
        }
    }
}