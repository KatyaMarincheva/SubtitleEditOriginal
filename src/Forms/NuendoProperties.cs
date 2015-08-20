// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuendoProperties.cs" company="">
//   
// </copyright>
// <summary>
//   The nuendo properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The nuendo properties.
    /// </summary>
    public partial class NuendoProperties : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuendoProperties"/> class.
        /// </summary>
        public NuendoProperties()
        {
            this.InitializeComponent();
            this.labelStatus.Text = string.Empty;
            this.textBoxCharacterFile.Text = Configuration.Settings.SubtitleSettings.NuendoCharacterListFile;
        }

        /// <summary>
        /// Gets or sets the character list file.
        /// </summary>
        public string CharacterListFile { get; set; }

        /// <summary>
        /// The button choose character_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonChooseCharacter_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Csv files|*.csv";
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxCharacterFile.Text = this.openFileDialog1.FileName;

                int count = LoadCharacters(this.openFileDialog1.FileName).Count;
                if (count == 0)
                {
                    this.labelStatus.Text = "No characters found!";
                }
                else
                {
                    this.labelStatus.Text = string.Format("{0} characters found", count);
                }
            }
        }

        /// <summary>
        /// The csv 2 properties_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Csv2Properties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
            this.CharacterListFile = this.textBoxCharacterFile.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The load characters.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> LoadCharacters(string fileName)
        {
            int lineNumber = 0;
            var separator = new[] { ';' };
            List<string> characters = new List<string>();
            foreach (string line in System.IO.File.ReadAllLines(fileName))
            {
                string[] parts = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    try
                    {
                        string text = Utilities.FixQuotes(parts[0]);
                        if (!string.IsNullOrWhiteSpace(text) && !characters.Contains(text))
                        {
                            if (parts.Length > 1)
                            {
                                text += " [" + Utilities.FixQuotes(parts[1]) + "]";
                            }

                            if (lineNumber != 0 || (!text.StartsWith("character [", StringComparison.OrdinalIgnoreCase) && !text.Equals("character", StringComparison.OrdinalIgnoreCase)))
                            {
                                characters.Add(text);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                lineNumber++;
            }

            return characters;
        }
    }
}