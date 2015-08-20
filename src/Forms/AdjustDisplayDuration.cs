// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustDisplayDuration.cs" company="">
//   
// </copyright>
// <summary>
//   The adjust display duration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The adjust display duration.
    /// </summary>
    public sealed partial class AdjustDisplayDuration : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustDisplayDuration"/> class.
        /// </summary>
        public AdjustDisplayDuration()
        {
            this.InitializeComponent();
            this.Icon = Properties.Resources.SubtitleEditFormIcon;

            this.comboBoxPercent.SelectedIndex = 0;
            this.comboBoxSeconds.SelectedIndex = 0;

            for (int i = 0; i < this.comboBoxSeconds.Items.Count; i++)
            {
                string s = this.comboBoxSeconds.Items[i].ToString();
                s = s.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                this.comboBoxSeconds.Items[i] = s;
            }

            this.numericUpDownMaxCharsSec.Value = (decimal)Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds;

            LanguageStructure.AdjustDisplayDuration language = Configuration.Settings.Language.AdjustDisplayDuration;
            this.Text = language.Title;
            this.groupBoxAdjustVia.Text = language.AdjustVia;
            this.radioButtonSeconds.Text = language.Seconds;
            this.radioButtonPercent.Text = language.Percent;
            this.radioButtonAutoRecalculate.Text = language.Recalculate;
            this.labelMaxCharsPerSecond.Text = Configuration.Settings.Language.Settings.MaximumCharactersPerSecond;
            this.labelAddSeconds.Text = language.AddSeconds;
            this.labelAddInPercent.Text = language.SetAsPercent;
            this.labelNote.Text = language.Note;
            this.comboBoxSeconds.Items[0] = language.PleaseChoose;
            this.comboBoxPercent.Items[0] = language.PleaseChoose;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.FixLargeFonts();
        }

        /// <summary>
        /// Gets the adjust value.
        /// </summary>
        public string AdjustValue
        {
            get
            {
                if (this.radioButtonPercent.Checked)
                {
                    return this.comboBoxPercent.Text;
                }

                if (this.radioButtonAutoRecalculate.Checked)
                {
                    return this.radioButtonAutoRecalculate.Text + ", " + this.labelMaxCharsPerSecond.Text + ": " + this.numericUpDownMaxCharsSec.Value; // TODO: Make language string with string.Format
                }

                return this.comboBoxSeconds.Text;
            }
        }

        /// <summary>
        /// Gets a value indicating whether adjust using percent.
        /// </summary>
        public bool AdjustUsingPercent
        {
            get
            {
                return this.radioButtonPercent.Checked;
            }
        }

        /// <summary>
        /// Gets a value indicating whether adjust using seconds.
        /// </summary>
        public bool AdjustUsingSeconds
        {
            get
            {
                return this.radioButtonSeconds.Checked;
            }
        }

        /// <summary>
        /// Gets the max characters per second.
        /// </summary>
        public decimal MaxCharactersPerSecond
        {
            get
            {
                return this.numericUpDownMaxCharsSec.Value;
            }
        }

        /// <summary>
        /// The fix large fonts.
        /// </summary>
        private void FixLargeFonts()
        {
            if (this.labelNote.Left + this.labelNote.Width + 5 > this.Width)
            {
                this.Width = this.labelNote.Left + this.labelNote.Width + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The form adjust display time_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormAdjustDisplayTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The radio button percent checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonPercentCheckedChanged(object sender, EventArgs e)
        {
            this.FixEnabled();
        }

        /// <summary>
        /// The fix enabled.
        /// </summary>
        private void FixEnabled()
        {
            if (this.radioButtonPercent.Checked)
            {
                this.comboBoxPercent.Enabled = true;
                this.comboBoxSeconds.Enabled = false;
                this.numericUpDownMaxCharsSec.Enabled = false;
            }
            else if (this.radioButtonSeconds.Checked)
            {
                this.comboBoxPercent.Enabled = false;
                this.comboBoxSeconds.Enabled = true;
                this.numericUpDownMaxCharsSec.Enabled = false;
            }
            else
            {
                this.comboBoxPercent.Enabled = false;
                this.comboBoxSeconds.Enabled = false;
                this.numericUpDownMaxCharsSec.Enabled = true;
            }
        }

        /// <summary>
        /// The radio button seconds checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonSecondsCheckedChanged(object sender, EventArgs e)
        {
            this.FixEnabled();
        }

        /// <summary>
        /// The button ok click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (this.radioButtonSeconds.Checked && this.comboBoxSeconds.SelectedIndex < 1)
            {
                MessageBox.Show(Configuration.Settings.Language.AdjustDisplayDuration.PleaseSelectAValueFromTheDropDownList);
                this.comboBoxSeconds.Focus();
            }
            else if (this.radioButtonPercent.Checked && this.comboBoxPercent.SelectedIndex < 1)
            {
                MessageBox.Show(Configuration.Settings.Language.AdjustDisplayDuration.PleaseSelectAValueFromTheDropDownList);
                this.comboBoxPercent.Focus();
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The radio button auto recalculate_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonAutoRecalculate_CheckedChanged(object sender, EventArgs e)
        {
            this.FixEnabled();
        }
    }
}