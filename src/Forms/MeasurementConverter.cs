// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The measurement converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The measurement converter.
    /// </summary>
    public partial class MeasurementConverter : Form
    {
        /// <summary>
        /// The _default back color.
        /// </summary>
        private Color _defaultBackColor = Color.White;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementConverter"/> class.
        /// </summary>
        public MeasurementConverter()
        {
            this.InitializeComponent();

            var l = Configuration.Settings.Language.MeasurementConverter;
            this.Text = l.Title;
            this.labelConvertFrom.Text = l.ConvertFrom;
            this.labelConvertTo.Text = l.ConvertTo;
            this.linkLabel1.Text = l.CopyToClipboard;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;

            this._defaultBackColor = this.textBoxInput.BackColor;
            this.textBoxInput.Text = "1";

            this.comboBoxFrom.Items.Add(l.Fahrenheit);
            this.comboBoxFrom.Items.Add(l.Celsius);

            this.comboBoxFrom.Items.Add(l.Miles);
            this.comboBoxFrom.Items.Add(l.Kilometers);
            this.comboBoxFrom.Items.Add(l.Meters);
            this.comboBoxFrom.Items.Add(l.Yards);
            this.comboBoxFrom.Items.Add(l.Feet);
            this.comboBoxFrom.Items.Add(l.Inches);

            this.comboBoxFrom.Items.Add(l.Pounds);
            this.comboBoxFrom.Items.Add(l.Kilos);

            this.comboBoxFrom.SelectedIndex = 0;

            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The combo box from_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = this.comboBoxFrom.SelectedItem.ToString();
            this.comboBoxTo.Items.Clear();
            var l = Configuration.Settings.Language.MeasurementConverter;
            if (text == l.Fahrenheit)
            {
                this.comboBoxTo.Items.Add(l.Celsius);
            }
            else if (text == l.Celsius)
            {
                this.comboBoxTo.Items.Add(l.Fahrenheit);
            }
            else if (text == l.Miles)
            {
                this.comboBoxTo.Items.Add(l.Kilometers);
                this.comboBoxTo.Items.Add(l.Meters);
                this.comboBoxTo.Items.Add(l.Yards);
                this.comboBoxTo.Items.Add(l.Feet);
                this.comboBoxTo.Items.Add(l.Inches);
            }
            else if (text == l.Kilometers)
            {
                this.comboBoxTo.Items.Add(l.Miles);
                this.comboBoxTo.Items.Add(l.Meters);
                this.comboBoxTo.Items.Add(l.Yards);
                this.comboBoxTo.Items.Add(l.Feet);
                this.comboBoxTo.Items.Add(l.Inches);
            }
            else if (text == l.Meters)
            {
                this.comboBoxTo.Items.Add(l.Miles);
                this.comboBoxTo.Items.Add(l.Kilometers);
                this.comboBoxTo.Items.Add(l.Yards);
                this.comboBoxTo.Items.Add(l.Feet);
                this.comboBoxTo.Items.Add(l.Inches);
            }
            else if (text == l.Yards)
            {
                this.comboBoxTo.Items.Add(l.Miles);
                this.comboBoxTo.Items.Add(l.Kilometers);
                this.comboBoxTo.Items.Add(l.Meters);
                this.comboBoxTo.Items.Add(l.Feet);
                this.comboBoxTo.Items.Add(l.Inches);
            }
            else if (text == l.Feet)
            {
                this.comboBoxTo.Items.Add(l.Miles);
                this.comboBoxTo.Items.Add(l.Kilometers);
                this.comboBoxTo.Items.Add(l.Meters);
                this.comboBoxTo.Items.Add(l.Yards);
                this.comboBoxTo.Items.Add(l.Inches);
            }
            else if (text == l.Inches)
            {
                this.comboBoxTo.Items.Add(l.Miles);
                this.comboBoxTo.Items.Add(l.Kilometers);
                this.comboBoxTo.Items.Add(l.Meters);
                this.comboBoxTo.Items.Add(l.Yards);
                this.comboBoxTo.Items.Add(l.Feet);
            }
            else if (text == l.Pounds)
            {
                this.comboBoxTo.Items.Add(l.Kilos);
            }
            else if (text == l.Kilos)
            {
                this.comboBoxTo.Items.Add(l.Pounds);
            }

            if (this.comboBoxTo.Items.Count > 0)
            {
                this.comboBoxTo.SelectedIndex = 0;
            }

            this.textBoxInput_TextChanged(null, null);
        }

        /// <summary>
        /// The show result.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        private void ShowResult(double d)
        {
            this.textBoxResult.Text = string.Format("{0:0.##}", d);
        }

        /// <summary>
        /// The combo box to_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxInput_TextChanged(null, null);
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
            this.Close();
        }

        /// <summary>
        /// The link label 1_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.textBoxResult.Text.Length > 0)
            {
                Clipboard.SetText(this.textBoxResult.Text);
            }
        }

        /// <summary>
        /// The text box input_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxInput_TextChanged(object sender, EventArgs e)
        {
            if (this.comboBoxFrom.SelectedIndex == -1 || this.comboBoxTo.SelectedIndex == -1)
            {
                return;
            }

            double d;
            if (!double.TryParse(this.textBoxInput.Text, out d))
            {
                this.textBoxInput.BackColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor;
                return;
            }

            this.textBoxInput.BackColor = this._defaultBackColor;

            string text = this.comboBoxFrom.SelectedItem.ToString();
            string textTo = this.comboBoxTo.SelectedItem.ToString();
            var l = Configuration.Settings.Language.MeasurementConverter;
            if (text == l.Fahrenheit)
            {
                this.ShowResult((d - 32) * 5 / 9);
            }
            else if (text == l.Celsius)
            {
                this.ShowResult(Convert.ToDouble(d) * 1.80 + 32);
            }
            else if (text == l.Miles)
            {
                if (textTo == l.Kilometers)
                {
                    this.ShowResult(Convert.ToDouble(d) / 0.621371192);
                }
                else if (textTo == l.Meters)
                {
                    this.ShowResult(Convert.ToDouble(d) / 0.000621371192);
                }
                else if (textTo == l.Yards)
                {
                    this.ShowResult(Convert.ToDouble(d) * 1760);
                }
                else if (textTo == l.Feet)
                {
                    this.ShowResult(Convert.ToDouble(d) * 5280);
                }
                else if (textTo == l.Inches)
                {
                    this.ShowResult(Convert.ToDouble(d) * 63360);
                }
            }
            else if (text == l.Kilometers)
            {
                if (textTo == l.Miles)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.621371192);
                }
                else if (textTo == l.Yards)
                {
                    this.ShowResult(Convert.ToDouble(d) * 1093.61);
                }
                else if (textTo == l.Meters)
                {
                    this.ShowResult(Convert.ToDouble(d) * TimeCode.BaseUnit);
                }
                else if (textTo == l.Feet)
                {
                    this.ShowResult(Convert.ToDouble(d) / 0.0003048);
                }
                else if (textTo == l.Inches)
                {
                    this.ShowResult(Convert.ToDouble(d) * 39370.0787);
                }
            }
            else if (text == l.Meters)
            {
                if (textTo == l.Miles)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.000621371192);
                }
                else if (textTo == l.Kilometers)
                {
                    this.ShowResult(Convert.ToDouble(d) / TimeCode.BaseUnit);
                }
                else if (textTo == l.Yards)
                {
                    this.ShowResult(Convert.ToDouble(d) * 1.0936133);
                }
                else if (textTo == l.Feet)
                {
                    this.ShowResult(Convert.ToDouble(d) * 3.28084);
                }
                else if (textTo == l.Inches)
                {
                    this.ShowResult(Convert.ToDouble(d) * 39.3700787);
                }
            }
            else if (text == l.Yards)
            {
                if (textTo == l.Kilometers)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.0009144);
                }
                else if (textTo == l.Miles)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.000568181818);
                }
                else if (textTo == l.Meters)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.9144);
                }
                else if (textTo == l.Feet)
                {
                    this.ShowResult(Convert.ToDouble(d) * 3);
                }
                else if (textTo == l.Inches)
                {
                    this.ShowResult(Convert.ToDouble(d) * 36);
                }
            }
            else if (text == l.Feet)
            {
                if (textTo == l.Kilometers)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.0003048);
                }
                else if (textTo == l.Miles)
                {
                    this.ShowResult(Convert.ToDouble(d) / 5280);
                }
                else if (textTo == l.Meters)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.3048);
                }
                else if (textTo == l.Yards)
                {
                    this.ShowResult(Convert.ToDouble(d) / 3);
                }
                else if (textTo == l.Inches)
                {
                    this.ShowResult(Convert.ToDouble(d) * 12);
                }
            }
            else if (text == l.Inches)
            {
                if (textTo == l.Kilometers)
                {
                    this.ShowResult(Convert.ToDouble(d) / 39370.0787);
                }
                else if (textTo == l.Miles)
                {
                    this.ShowResult(Convert.ToDouble(d) / 63360);
                }
                else if (textTo == l.Meters)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.0254);
                }
                else if (textTo == l.Yards)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.0277777778);
                }
                else if (textTo == l.Feet)
                {
                    this.ShowResult(Convert.ToDouble(d) * 0.0833333333);
                }
            }
            else if (text == l.Pounds)
            {
                this.ShowResult(Convert.ToDouble(d) * 0.45359237);
            }
            else if (text == l.Kilos)
            {
                this.ShowResult(Convert.ToDouble(d) / 0.45359237);
            }
        }

        /// <summary>
        /// The text box input_ key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            this.textBoxInput_TextChanged(null, null);
        }

        /// <summary>
        /// The text box input_ key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                return;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Back) || (e.KeyChar == '.') || (e.KeyChar == ',') || (e.KeyChar == '-'))
            {
                return;
            }

            e.Handled = true;
        }
    }
}