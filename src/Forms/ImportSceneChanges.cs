// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportSceneChanges.cs" company="">
//   
// </copyright>
// <summary>
//   The import scene changes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The import scene changes.
    /// </summary>
    public partial class ImportSceneChanges : PositionAndSizeForm
    {
        /// <summary>
        /// The _frame rate.
        /// </summary>
        private double _frameRate = 25;

        /// <summary>
        /// The scene changes in seconds.
        /// </summary>
        public List<double> SceneChangesInSeconds = new List<double>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportSceneChanges"/> class.
        /// </summary>
        /// <param name="videoInfo">
        /// The video info.
        /// </param>
        public ImportSceneChanges(VideoInfo videoInfo)
        {
            this.InitializeComponent();
            if (videoInfo != null && videoInfo.FramesPerSecond > 1)
            {
                this._frameRate = videoInfo.FramesPerSecond;
            }

            this.Text = Configuration.Settings.Language.ImportSceneChanges.Title;
            this.groupBoxImportText.Text = Configuration.Settings.Language.ImportSceneChanges.Title;
            this.buttonOpenText.Text = Configuration.Settings.Language.ImportSceneChanges.OpenTextFile;
            this.groupBoxImportOptions.Text = Configuration.Settings.Language.ImportSceneChanges.ImportOptions;
            this.radioButtonFrames.Text = Configuration.Settings.Language.ImportSceneChanges.Frames;
            this.radioButtonSeconds.Text = Configuration.Settings.Language.ImportSceneChanges.Seconds;
            this.radioButtonMilliseconds.Text = Configuration.Settings.Language.ImportSceneChanges.Milliseconds;
            this.groupBoxTimeCodes.Text = Configuration.Settings.Language.ImportSceneChanges.TimeCodes;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The button open text_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOpenText_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = this.buttonOpenText.Text;
            this.openFileDialog1.Filter = Configuration.Settings.Language.ImportText.TextFiles + "|*.txt;*.scenechange|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.LoadTextFile(this.openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// The load text file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void LoadTextFile(string fileName)
        {
            try
            {
                Encoding encoding = Utilities.GetEncodingFromFile(fileName);
                string s = File.ReadAllText(fileName, encoding).Trim();
                if (s.Contains('.'))
                {
                    this.radioButtonSeconds.Checked = true;
                }

                if (s.Contains('.') && s.Contains(':'))
                {
                    this.radioButtonHHMMSSMS.Checked = true;
                }

                if (!s.Contains(Environment.NewLine) && s.Contains(';'))
                {
                    var sb = new StringBuilder();
                    foreach (string line in s.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            sb.AppendLine(line.Trim());
                        }
                    }

                    this.textBoxText.Text = sb.ToString();
                }
                else
                {
                    this.textBoxText.Text = s;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// The button o k_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SceneChangesInSeconds = new List<double>();
            foreach (string line in this.textBoxText.Lines)
            {
                if (this.radioButtonHHMMSSMS.Checked)
                {
                    // Parse string (HH:MM:SS.ms)
                    string[] timeParts = line.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);

                    // If 4 parts were found...
                    if (timeParts.Length == 4)
                    {
                        this.SceneChangesInSeconds.Add(Convert.ToDouble(timeParts[0]) * 3600.0 + Convert.ToDouble(timeParts[1]) * 60.0 + Convert.ToDouble(timeParts[2]) + Convert.ToDouble(timeParts[3]) / TimeCode.BaseUnit);
                    }
                }
                else
                {
                    double d;
                    if (double.TryParse(line, out d))
                    {
                        if (this.radioButtonFrames.Checked)
                        {
                            this.SceneChangesInSeconds.Add(d / this._frameRate);
                        }
                        else if (this.radioButtonSeconds.Checked)
                        {
                            this.SceneChangesInSeconds.Add(d);
                        }
                        else if (this.radioButtonMilliseconds.Checked)
                        {
                            this.SceneChangesInSeconds.Add(d / TimeCode.BaseUnit);
                        }
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The import scene changes_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ImportSceneChanges_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}