// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DCinemaPropertiesInterop.cs" company="">
//   
// </copyright>
// <summary>
//   The d cinema properties interop.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms.DCinema
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The d cinema properties interop.
    /// </summary>
    public sealed partial class DCinemaPropertiesInterop : DCinemaPropertiesForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCinemaPropertiesInterop"/> class.
        /// </summary>
        public DCinemaPropertiesInterop()
        {
            this.InitializeComponent();

            var l = Configuration.Settings.Language.DCinemaProperties;
            this.Text = l.Title;
            this.labelSubtitleID.Text = l.SubtitleId;
            this.labelMovieTitle.Text = l.MovieTitle;
            this.labelReelNumber.Text = l.ReelNumber;
            this.labelLanguage.Text = l.Language;
            this.groupBoxFont.Text = l.Font;
            this.labelFontId.Text = l.FontId;
            this.labelFontUri.Text = l.FontUri;
            this.labelFontColor.Text = l.FontColor;
            this.buttonFontColor.Text = l.ChooseColor;
            this.labelEffect.Text = l.FontEffect;
            this.labelEffectColor.Text = l.FontEffectColor;
            this.buttonFontEffectColor.Text = l.ChooseColor;
            this.labelFontSize.Text = l.FontSize;
            this.labelTopBottomMargin.Text = l.TopBottomMargin;
            this.labelZPosition.Text = l.ZPosition;
            this.labelZPositionHelp.Text = l.ZPositionHelp;
            this.labelFadeUpTime.Text = l.FadeUpTime;
            this.labelFadeDownTime.Text = l.FadeDownTime;

            foreach (CultureInfo x in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                this.comboBoxLanguage.Items.Add(x.EnglishName);
            }

            this.comboBoxLanguage.Sorted = true;

            var ss = Configuration.Settings.SubtitleSettings;
            if (!string.IsNullOrEmpty(ss.CurrentDCinemaSubtitleId))
            {
                this.textBoxSubtitleID.Text = ss.CurrentDCinemaSubtitleId;
                this.textBoxMovieTitle.Text = ss.CurrentDCinemaMovieTitle;
                int number;
                if (int.TryParse(ss.CurrentDCinemaReelNumber, out number))
                {
                    if (this.numericUpDownReelNumber.Minimum <= number && this.numericUpDownReelNumber.Maximum >= number)
                    {
                        this.numericUpDownReelNumber.Value = number;
                    }
                }

                this.comboBoxLanguage.Text = ss.CurrentDCinemaLanguage;
                this.textBoxFontID.Text = ss.CurrentDCinemaFontId;
                this.textBoxFontUri.Text = ss.CurrentDCinemaFontUri;
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

                if (this.numericUpDownFadeUp.Minimum <= ss.DCinemaFadeUpTime && this.numericUpDownFadeUp.Maximum >= ss.DCinemaFadeUpTime)
                {
                    this.numericUpDownFadeUp.Value = ss.DCinemaFadeUpTime;
                }
                else
                {
                    this.numericUpDownFadeUp.Value = 5;
                }

                if (this.numericUpDownFadeDown.Minimum <= ss.DCinemaFadeDownTime && this.numericUpDownFadeDown.Maximum >= ss.DCinemaFadeDownTime)
                {
                    this.numericUpDownFadeDown.Value = ss.DCinemaFadeDownTime;
                }
                else
                {
                    this.numericUpDownFadeDown.Value = 5;
                }

                decimal zPosition = (decimal)ss.DCinemaZPosition;
                if (this.numericUpDownZPosition.Minimum <= zPosition && this.numericUpDownZPosition.Maximum >= zPosition)
                {
                    this.numericUpDownZPosition.Value = zPosition;
                }
                else
                {
                    this.numericUpDownZPosition.Value = 0;
                }
            }

            Utilities.FixLargeFonts(this, this.buttonCancel);
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
            string hex = Guid.NewGuid().ToString().Replace("-", string.Empty);
            this.textBoxSubtitleID.Text = hex.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
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
            var ss = Configuration.Settings.SubtitleSettings;
            ss.CurrentDCinemaSubtitleId = this.textBoxSubtitleID.Text;
            ss.CurrentDCinemaMovieTitle = this.textBoxMovieTitle.Text;
            ss.CurrentDCinemaReelNumber = this.numericUpDownReelNumber.Value.ToString();
            ss.CurrentDCinemaLanguage = this.comboBoxLanguage.Text;
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
            ss.DCinemaFadeUpTime = (int)this.numericUpDownFadeUp.Value;
            ss.DCinemaFadeDownTime = (int)this.numericUpDownFadeDown.Value;
            ss.DCinemaZPosition = (double)this.numericUpDownZPosition.Value;

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
    }
}