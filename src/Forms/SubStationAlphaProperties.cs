// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubStationAlphaProperties.cs" company="">
//   
// </copyright>
// <summary>
//   The sub station alpha properties.
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
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The sub station alpha properties.
    /// </summary>
    public sealed partial class SubStationAlphaProperties : PositionAndSizeForm
    {
        /// <summary>
        /// The _is sub station alpha.
        /// </summary>
        private readonly bool _isSubStationAlpha;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private readonly Subtitle _subtitle;

        /// <summary>
        /// The _video file name.
        /// </summary>
        private readonly string _videoFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubStationAlphaProperties"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="subtitleFileName">
        /// The subtitle file name.
        /// </param>
        public SubStationAlphaProperties(Subtitle subtitle, SubtitleFormat format, string videoFileName, string subtitleFileName)
        {
            this.InitializeComponent();
            this._subtitle = subtitle;
            this._isSubStationAlpha = format.Name == SubStationAlpha.NameOfFormat;
            this._videoFileName = videoFileName;

            var l = Configuration.Settings.Language.SubStationAlphaProperties;
            if (this._isSubStationAlpha)
            {
                this.Text = l.TitleSubstationAlpha;
                this.labelWrapStyle.Visible = false;
                this.comboBoxWrapStyle.Visible = false;
                this.checkBoxScaleBorderAndShadow.Visible = false;
                this.Height = this.Height - (this.comboBoxWrapStyle.Height + this.checkBoxScaleBorderAndShadow.Height + 8);
            }
            else
            {
                this.Text = l.Title;
            }

            this.comboBoxWrapStyle.SelectedIndex = 2;
            this.comboBoxCollision.SelectedIndex = 0;

            string header = subtitle.Header;
            if (subtitle.Header == null)
            {
                var ssa = new SubStationAlpha();
                var sub = new Subtitle();
                var lines = new List<string>();
                foreach (string line in subtitle.ToText(ssa).SplitToLines())
                {
                    lines.Add(line);
                }

                string title = "Untitled";
                if (!string.IsNullOrEmpty(subtitleFileName))
                {
                    title = Path.GetFileNameWithoutExtension(subtitleFileName);
                }
                else if (!string.IsNullOrEmpty(videoFileName))
                {
                    title = Path.GetFileNameWithoutExtension(videoFileName);
                }

                ssa.LoadSubtitle(sub, lines, title);
                header = sub.Header;
            }

            if (header != null)
            {
                foreach (string line in header.SplitToLines())
                {
                    string s = line.ToLowerInvariant().Trim();
                    if (s.StartsWith("title:"))
                    {
                        this.textBoxTitle.Text = s.Remove(0, 6).Trim();
                    }
                    else if (s.StartsWith("original script:"))
                    {
                        this.textBoxOriginalScript.Text = s.Remove(0, 16).Trim();
                    }
                    else if (s.StartsWith("original translation:"))
                    {
                        this.textBoxTranslation.Text = s.Remove(0, 21).Trim();
                    }
                    else if (s.StartsWith("original editing:"))
                    {
                        this.textBoxEditing.Text = s.Remove(0, 17).Trim();
                    }
                    else if (s.StartsWith("original timing:"))
                    {
                        this.textBoxTiming.Text = s.Remove(0, 16).Trim();
                    }
                    else if (s.StartsWith("synch point:"))
                    {
                        this.textBoxSyncPoint.Text = s.Remove(0, 12).Trim();
                    }
                    else if (s.StartsWith("script updated by:"))
                    {
                        this.textBoxUpdatedBy.Text = s.Remove(0, 18).Trim();
                    }
                    else if (s.StartsWith("update details:"))
                    {
                        this.textBoxUpdateDetails.Text = s.Remove(0, 15).Trim();
                    }
                    else if (s.StartsWith("collisions:"))
                    {
                        if (s.Remove(0, 11).Trim() == "reverse")
                        {
                            this.comboBoxCollision.SelectedIndex = 1;
                        }
                    }
                    else if (s.StartsWith("playresx:"))
                    {
                        int number;
                        if (int.TryParse(s.Remove(0, 9).Trim(), out number))
                        {
                            this.numericUpDownVideoWidth.Value = number;
                        }
                    }
                    else if (s.StartsWith("playresy:"))
                    {
                        int number;
                        if (int.TryParse(s.Remove(0, 9).Trim(), out number))
                        {
                            this.numericUpDownVideoHeight.Value = number;
                        }
                    }
                    else if (s.StartsWith("scaledborderandshadow:"))
                    {
                        this.checkBoxScaleBorderAndShadow.Checked = s.Remove(0, 22).Trim().Equals("yes");
                    }
                }
            }

            this.groupBoxScript.Text = l.Script;
            this.labelTitle.Text = l.ScriptTitle;
            this.labelOriginalScript.Text = l.OriginalScript;
            this.labelTranslation.Text = l.Translation;
            this.labelEditing.Text = l.Editing;
            this.labelTiming.Text = l.Timing;
            this.labelSyncPoint.Text = l.SyncPoint;
            this.labelUpdatedBy.Text = l.UpdatedBy;
            this.labelUpdateDetails.Text = l.UpdateDetails;
            this.groupBoxResolution.Text = l.Resolution;
            this.labelVideoResolution.Text = l.VideoResolution;
            this.groupBoxOptions.Text = l.Options;
            this.labelCollision.Text = l.Collision;
            this.labelWrapStyle.Text = l.WrapStyle;
            this.checkBoxScaleBorderAndShadow.Text = l.ScaleBorderAndShadow;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            Utilities.FixLargeFonts(this, this.buttonCancel);
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
        /// The get default header.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDefaultHeader()
        {
            SubtitleFormat format;
            if (this._isSubStationAlpha)
            {
                format = new SubStationAlpha();
            }
            else
            {
                format = new AdvancedSubStationAlpha();
            }

            var sub = new Subtitle();
            string text = format.ToText(sub, string.Empty);
            var lines = new List<string>();
            foreach (string line in text.SplitToLines())
            {
                lines.Add(line);
            }

            format.LoadSubtitle(sub, lines, string.Empty);
            return sub.Header.Trim();
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
            if (this._subtitle.Header == null || !this._subtitle.Header.Contains("[script info]", StringComparison.OrdinalIgnoreCase))
            {
                this._subtitle.Header = this.GetDefaultHeader();
            }

            string title = this.textBoxTitle.Text;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "untitled";
            }

            this.UpdateTag("Title", title, false);
            this.UpdateTag("Original Script", this.textBoxOriginalScript.Text, string.IsNullOrWhiteSpace(this.textBoxOriginalScript.Text));
            this.UpdateTag("Original Translation", this.textBoxTranslation.Text, string.IsNullOrWhiteSpace(this.textBoxTranslation.Text));
            this.UpdateTag("Original Editing", this.textBoxEditing.Text, string.IsNullOrWhiteSpace(this.textBoxEditing.Text));
            this.UpdateTag("Original Timing", this.textBoxTiming.Text, string.IsNullOrWhiteSpace(this.textBoxTiming.Text));
            this.UpdateTag("Synch Point", this.textBoxSyncPoint.Text, string.IsNullOrWhiteSpace(this.textBoxSyncPoint.Text));
            this.UpdateTag("Script Updated By", this.textBoxUpdatedBy.Text, string.IsNullOrWhiteSpace(this.textBoxUpdatedBy.Text));
            this.UpdateTag("Update Details", this.textBoxUpdateDetails.Text, string.IsNullOrWhiteSpace(this.textBoxUpdateDetails.Text));
            this.UpdateTag("PlayResX", this.numericUpDownVideoWidth.Value.ToString(), this.numericUpDownVideoWidth.Value == 0);
            this.UpdateTag("PlayResY", this.numericUpDownVideoHeight.Value.ToString(), this.numericUpDownVideoHeight.Value == 0);
            if (this.comboBoxCollision.SelectedIndex == 0)
            {
                this.UpdateTag("collisions", "Normal", false); // normal
            }
            else
            {
                this.UpdateTag("collisions", "Reverse", false); // reverse
            }

            if (!this._isSubStationAlpha)
            {
                this.UpdateTag("wrapstyle", this.comboBoxWrapStyle.SelectedIndex.ToString(), false);
                if (this.checkBoxScaleBorderAndShadow.Checked)
                {
                    this.UpdateTag("ScaledBorderAndShadow", "yes", false);
                }
                else
                {
                    this.UpdateTag("ScaledBorderAndShadow", "no", false);
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The update tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="remove">
        /// The remove.
        /// </param>
        private void UpdateTag(string tag, string text, bool remove)
        {
            bool scriptInfoOn = false;
            var sb = new StringBuilder();
            bool found = false;
            foreach (string line in this._subtitle.Header.SplitToLines())
            {
                if (line.StartsWith("[script info]", StringComparison.OrdinalIgnoreCase))
                {
                    scriptInfoOn = true;
                }
                else if (line.StartsWith('['))
                {
                    if (!found && scriptInfoOn && !remove)
                    {
                        sb.AppendLine(tag + ": " + text);
                    }

                    sb = new StringBuilder(sb.ToString().TrimEnd());
                    sb.AppendLine();
                    sb.AppendLine();
                    scriptInfoOn = false;
                }

                string s = line.ToLower();
                if (s.StartsWith(tag.ToLower() + ":"))
                {
                    if (!remove)
                    {
                        sb.AppendLine(line.Substring(0, tag.Length) + ": " + text);
                    }

                    found = true;
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            this._subtitle.Header = sb.ToString().Trim();
        }

        /// <summary>
        /// The sub station alpha properties_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubStationAlphaProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button get resolution from video_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGetResolutionFromVideo_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenVideoFileTitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(false);
            this.openFileDialog1.FileName = string.Empty;
            if (string.IsNullOrEmpty(this.openFileDialog1.InitialDirectory) && !string.IsNullOrEmpty(this._videoFileName))
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this._videoFileName);
                this.openFileDialog1.FileName = Path.GetFileName(this._videoFileName);
            }

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoInfo info = Utilities.GetVideoInfo(this.openFileDialog1.FileName);
                if (info != null && info.Success)
                {
                    this.numericUpDownVideoWidth.Value = info.Width;
                    this.numericUpDownVideoHeight.Value = info.Height;
                }
            }
        }
    }
}