// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DCinemaPropertiesSmpte.cs" company="">
//   
// </copyright>
// <summary>
//   The d cinema properties smpte.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms.DCinema
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The d cinema properties smpte.
    /// </summary>
    public sealed partial class DCinemaPropertiesSmpte : DCinemaPropertiesForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCinemaPropertiesSmpte"/> class.
        /// </summary>
        public DCinemaPropertiesSmpte()
        {
            this.InitializeComponent();

            var l = Configuration.Settings.Language.DCinemaProperties;
            this.Text = l.TitleSmpte;
            this.labelSubtitleID.Text = l.SubtitleId;
            this.labelMovieTitle.Text = l.MovieTitle;
            this.labelReelNumber.Text = l.ReelNumber;
            this.labelLanguage.Text = l.Language;
            this.labelIssueDate.Text = l.IssueDate;
            this.labelEditRate.Text = l.EditRate;
            this.labelTimeCodeRate.Text = l.TimeCodeRate;
            this.labelStartTime.Text = l.StartTime;
            this.groupBoxFont.Text = l.Font;
            this.labelFontId.Text = l.FontId;
            this.labelFontUri.Text = l.FontUri;
            this.labelFontColor.Text = l.FontColor;
            this.buttonFontColor.Text = l.ChooseColor;
            this.labelEffect.Text = l.FontEffect;
            this.labelEffectColor.Text = l.FontEffectColor;
            this.buttonFontEffectColor.Text = l.ChooseColor;
            this.labelFontSize.Text = l.FontSize;
            this.buttonGenerateID.Text = l.Generate;
            this.buttonGenFontUri.Text = l.Generate;

            foreach (CultureInfo x in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                this.comboBoxLanguage.Items.Add(x.TwoLetterISOLanguageName);
            }

            this.comboBoxLanguage.Sorted = true;

            var ss = Configuration.Settings.SubtitleSettings;
            if (!string.IsNullOrEmpty(ss.CurrentDCinemaSubtitleId))
            {
                this.textBoxSubtitleID.Text = ss.CurrentDCinemaSubtitleId;
                int number;
                if (int.TryParse(ss.CurrentDCinemaReelNumber, out number) && this.numericUpDownReelNumber.Minimum <= number && this.numericUpDownReelNumber.Maximum >= number)
                {
                    this.numericUpDownReelNumber.Value = number;
                }

                this.textBoxMovieTitle.Text = ss.CurrentDCinemaMovieTitle;
                this.comboBoxLanguage.Text = ss.CurrentDCinemaLanguage;
                this.textBoxFontID.Text = ss.CurrentDCinemaFontId;
                this.textBoxEditRate.Text = ss.CurrentDCinemaEditRate;
                this.comboBoxTimeCodeRate.Text = ss.CurrentDCinemaTimeCodeRate;

                this.timeUpDownStartTime.ForceHHMMSSFF();
                if (string.IsNullOrEmpty(ss.CurrentDCinemaStartTime))
                {
                    ss.CurrentDCinemaStartTime = "00:00:00:00";
                }

                this.timeUpDownStartTime.MaskedTextBox.Text = ss.CurrentDCinemaStartTime;

                this.textBoxFontUri.Text = ss.CurrentDCinemaFontUri;
                this.textBoxIssueDate.Text = ss.CurrentDCinemaIssueDate;
                this.panelFontColor.BackColor = ss.CurrentDCinemaFontColor;
                if (ss.CurrentDCinemaFontEffect == "border")
                {
                    this.comboBoxFontEffect.SelectedIndex = 1;
                }
                else if (ss.CurrentDCinemaFontEffect == "shadow")
                {
                    this.comboBoxFontEffect.SelectedIndex = 2;
                }
                else
                {
                    this.comboBoxFontEffect.SelectedIndex = 0;
                }

                this.panelFontEffectColor.BackColor = ss.CurrentDCinemaFontEffectColor;
                this.numericUpDownFontSize.Value = ss.CurrentDCinemaFontSize;
                if (this.numericUpDownTopBottomMargin.Minimum <= ss.DCinemaBottomMargin && this.numericUpDownTopBottomMargin.Maximum >= ss.DCinemaBottomMargin)
                {
                    this.numericUpDownTopBottomMargin.Value = ss.DCinemaBottomMargin;
                }
                else
                {
                    this.numericUpDownTopBottomMargin.Value = 8;
                }
            }

            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// The button font color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonFontColor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.panelFontColor.BackColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.panelFontColor.BackColor = this.colorDialog1.Color;
            }
        }

        /// <summary>
        /// The button font effect color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonFontEffectColor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.panelFontEffectColor.BackColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.panelFontEffectColor.BackColor = this.colorDialog1.Color;
            }
        }

        /// <summary>
        /// The button generate i d_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGenerateID_Click(object sender, EventArgs e)
        {
            this.textBoxSubtitleID.Text = this.GenerateID();
        }

        /// <summary>
        /// The button today_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonToday_Click(object sender, EventArgs e)
        {
            this.textBoxIssueDate.Text = DateTime.Now.ToString("s") + ".000-00:00";
        }

        /// <summary>
        /// The button o k_ click_1.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOK_Click_1(object sender, EventArgs e)
        {
            var ss = Configuration.Settings.SubtitleSettings;
            ss.CurrentDCinemaSubtitleId = this.textBoxSubtitleID.Text;
            ss.CurrentDCinemaMovieTitle = this.textBoxMovieTitle.Text;
            ss.CurrentDCinemaReelNumber = this.numericUpDownReelNumber.Value.ToString();
            ss.CurrentDCinemaEditRate = this.textBoxEditRate.Text;
            ss.CurrentDCinemaTimeCodeRate = this.comboBoxTimeCodeRate.Text;
            ss.CurrentDCinemaStartTime = this.timeUpDownStartTime.TimeCode.ToHHMMSSFF();
            if (this.comboBoxLanguage.SelectedItem != null)
            {
                ss.CurrentDCinemaLanguage = this.comboBoxLanguage.SelectedItem.ToString();
            }
            else
            {
                ss.CurrentDCinemaLanguage = string.Empty;
            }

            ss.CurrentDCinemaIssueDate = this.textBoxIssueDate.Text;
            ss.CurrentDCinemaFontId = this.textBoxFontID.Text;
            ss.CurrentDCinemaFontUri = this.textBoxFontUri.Text;
            ss.CurrentDCinemaFontColor = this.panelFontColor.BackColor;
            if (this.comboBoxFontEffect.SelectedIndex == 1)
            {
                ss.CurrentDCinemaFontEffect = "border";
            }
            else if (this.comboBoxFontEffect.SelectedIndex == 2)
            {
                ss.CurrentDCinemaFontEffect = "shadow";
            }
            else
            {
                ss.CurrentDCinemaFontEffect = string.Empty;
            }

            ss.CurrentDCinemaFontEffectColor = this.panelFontEffectColor.BackColor;
            ss.CurrentDCinemaFontSize = (int)this.numericUpDownFontSize.Value;
            ss.DCinemaBottomMargin = (int)this.numericUpDownTopBottomMargin.Value;

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBoxFontUri.Text = this.GenerateID();
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
        /// The generate id.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateID()
        {
            var hex = Guid.NewGuid().ToString().Replace("-", string.Empty);
            return "urn:uuid:" + hex.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
        }
    }
}