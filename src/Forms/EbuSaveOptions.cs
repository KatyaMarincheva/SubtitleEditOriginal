// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbuSaveOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The ebu save options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The ebu save options.
    /// </summary>
    public sealed partial class EbuSaveOptions : PositionAndSizeForm
    {
        /// <summary>
        /// The _header.
        /// </summary>
        private Ebu.EbuGeneralSubtitleInformation _header;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="EbuSaveOptions"/> class.
        /// </summary>
        public EbuSaveOptions()
        {
            this.InitializeComponent();

            var language = Configuration.Settings.Language.EbuSaveOptions;
            this.Text = language.Title;
            this.tabPageHeader.Text = language.GeneralSubtitleInformation;
            this.tabPageTextAndTiming.Text = language.TextAndTimingInformation;
            this.tabPageErrors.Text = language.Errors;

            this.labelCodePageNumber.Text = language.CodePageNumber;
            this.labelDiskFormatCode.Text = language.DiskFormatCode;
            this.labelDisplayStandardCode.Text = language.DisplayStandardCode;
            this.labelCharacterCodeTable.Text = language.CharacterCodeTable;
            this.labelLanguageCode.Text = language.LanguageCode;
            this.labelOriginalProgramTitle.Text = language.OriginalProgramTitle;
            this.labelOriginalEpisodeTitle.Text = language.OriginalEpisodeTitle;
            this.labelTranslatedProgramTitle.Text = language.TranslatedProgramTitle;
            this.labelTranslatedEpisodeTitle.Text = language.TranslatedEpisodeTitle;
            this.labelTranslatorsName.Text = language.TranslatorsName;
            this.labelSubtitleListReferenceCode.Text = language.SubtitleListReferenceCode;
            this.labelCountryOfOrigin.Text = language.CountryOfOrigin;
            this.labelTimeCodeStatus.Text = language.TimeCodeStatus;
            this.labelTimeCodeStartOfProgramme.Text = language.TimeCodeStartOfProgramme;

            this.labelRevisionNumber.Text = language.RevisionNumber;
            this.labelMaxNoOfDisplayableChars.Text = language.MaxNoOfDisplayableChars;
            this.labelMaxNumberOfDisplayableRows.Text = language.MaxNumberOfDisplayableRows;
            this.labelDiskSequenceNumber.Text = language.DiskSequenceNumber;
            this.labelTotalNumberOfDisks.Text = language.TotalNumberOfDisks;

            this.buttonImport.Text = language.Import;

            this.labelJustificationCode.Text = language.JustificationCode;
            this.comboBoxJustificationCode.Items.Clear();
            this.comboBoxJustificationCode.Items.Add(language.TextUnchangedPresentation);
            this.comboBoxJustificationCode.Items.Add(language.TextLeftJustifiedText);
            this.comboBoxJustificationCode.Items.Add(language.TextCenteredText);
            this.comboBoxJustificationCode.Items.Add(language.TextRightJustifiedText);

            this.labelErrors.Text = language.Errors;

            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;

            this.timeUpDownStartTime.ForceHHMMSSFF();
        }

        /// <summary>
        /// Gets the justification code.
        /// </summary>
        public byte JustificationCode { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="justificationCode">
        /// The justification code.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        internal void Initialize(Ebu.EbuGeneralSubtitleInformation header, byte justificationCode, string fileName, Subtitle subtitle)
        {
            this._header = header;
            this._subtitle = subtitle;

            this.FillFromHeader(header);
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    this.FillHeaderFromFile(fileName);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("EbuOptions unable to read existing file: " + fileName + "  - " + ex.Message);
                }

                string title = Path.GetFileNameWithoutExtension(fileName);
                if (title.Length > 32)
                {
                    title = title.Substring(0, 32).Trim();
                }

                this.textBoxOriginalProgramTitle.Text = title;
            }

            this.comboBoxJustificationCode.SelectedIndex = justificationCode;

            this.Text = Configuration.Settings.Language.EbuSaveOptions.Title;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
        }

        /// <summary>
        /// The check errors.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        private void CheckErrors(Subtitle subtitle)
        {
            this.textBoxErrors.Text = string.Empty;
            var sb = new StringBuilder();
            int errorCount = 0;
            int i = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                var arr = p.Text.SplitToLines();
                foreach (string line in arr)
                {
                    string s = HtmlUtil.RemoveHtmlTags(line);
                    if (s.Length > this.numericUpDownMaxCharacters.Value)
                    {
                        sb.AppendLine(string.Format(Configuration.Settings.Language.EbuSaveOptions.MaxLengthError, i, this.numericUpDownMaxCharacters.Value, s.Length - this.numericUpDownMaxCharacters.Value, s));
                        errorCount++;
                    }
                }

                i++;
            }

            this.textBoxErrors.Text = sb.ToString();
            this.tabPageErrors.Text = string.Format(Configuration.Settings.Language.EbuSaveOptions.ErrorsX, errorCount);
        }

        /// <summary>
        /// The fill from header.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        private void FillFromHeader(Ebu.EbuGeneralSubtitleInformation header)
        {
            this.textBoxCodePageNumber.Text = header.CodePageNumber;

            if (header.DiskFormatCode == "STL30.01")
            {
                this.comboBoxDiscFormatCode.SelectedIndex = 1;
            }
            else
            {
                this.comboBoxDiscFormatCode.SelectedIndex = 0;
            }

            if (header.DisplayStandardCode == "0")
            {
                this.comboBoxDisplayStandardCode.SelectedIndex = 0;
            }
            else if (header.DisplayStandardCode == "1")
            {
                this.comboBoxDisplayStandardCode.SelectedIndex = 1;
            }
            else if (header.DisplayStandardCode == "2")
            {
                this.comboBoxDisplayStandardCode.SelectedIndex = 2;
            }
            else
            {
                this.comboBoxDisplayStandardCode.SelectedIndex = 3;
            }

            this.comboBoxCharacterCodeTable.SelectedIndex = int.Parse(header.CharacterCodeTableNumber);
            this.textBoxLanguageCode.Text = header.LanguageCode;
            this.textBoxOriginalProgramTitle.Text = header.OriginalProgrammeTitle.TrimEnd();
            this.textBoxOriginalEpisodeTitle.Text = header.OriginalEpisodeTitle.TrimEnd();
            this.textBoxTranslatedProgramTitle.Text = header.TranslatedProgrammeTitle.TrimEnd();
            this.textBoxTranslatedEpisodeTitle.Text = header.TranslatedEpisodeTitle.TrimEnd();
            this.textBoxTranslatorsName.Text = header.TranslatorsName.TrimEnd();
            this.textBoxSubtitleListReferenceCode.Text = header.SubtitleListReferenceCode.TrimEnd();
            this.textBoxCountryOfOrigin.Text = header.CountryOfOrigin;

            this.comboBoxTimeCodeStatus.SelectedIndex = 0;
            if (header.TimeCodeStatus == "1")
            {
                this.comboBoxTimeCodeStatus.SelectedIndex = 1;
            }

            try
            {
                // HHMMSSFF
                int hh = int.Parse(header.TimeCodeStartOfProgramme.Substring(0, 2));
                int mm = int.Parse(header.TimeCodeStartOfProgramme.Substring(2, 2));
                int ss = int.Parse(header.TimeCodeStartOfProgramme.Substring(4, 2));
                int ff = int.Parse(header.TimeCodeStartOfProgramme.Substring(6, 2));
                this.timeUpDownStartTime.TimeCode = new TimeCode(hh, mm, ss, SubtitleFormat.FramesToMillisecondsMax999(ff));
            }
            catch (Exception)
            {
                this.timeUpDownStartTime.TimeCode = new TimeCode(0);
            }

            int number;
            if (int.TryParse(header.RevisionNumber, out number))
            {
                this.numericUpDownRevisionNumber.Value = number;
            }
            else
            {
                this.numericUpDownRevisionNumber.Value = 1;
            }

            if (int.TryParse(header.MaximumNumberOfDisplayableCharactersInAnyTextRow, out number))
            {
                this.numericUpDownMaxCharacters.Value = number;
            }

            this.numericUpDownMaxRows.Value = 23;
            if (int.TryParse(header.MaximumNumberOfDisplayableRows, out number))
            {
                this.numericUpDownMaxRows.Value = number;
            }

            if (int.TryParse(header.DiskSequenceNumber, out number))
            {
                this.numericUpDownDiskSequenceNumber.Value = number;
            }
            else
            {
                this.numericUpDownDiskSequenceNumber.Value = 1;
            }

            if (int.TryParse(header.TotalNumberOfDisks, out number))
            {
                this.numericUpDownTotalNumberOfDiscs.Value = number;
            }
            else
            {
                this.numericUpDownTotalNumberOfDiscs.Value = 1;
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
            this._header.CodePageNumber = this.textBoxCodePageNumber.Text;
            if (this._header.CodePageNumber.Length < 3)
            {
                this._header.CodePageNumber = "865";
            }

            if (this.comboBoxDiscFormatCode.SelectedIndex == 1)
            {
                this._header.DiskFormatCode = "STL30.01";
            }
            else
            {
                this._header.DiskFormatCode = "STL25.01";
            }

            if (this.comboBoxDisplayStandardCode.SelectedIndex == 0)
            {
                this._header.DisplayStandardCode = "0";
            }
            else if (this.comboBoxDisplayStandardCode.SelectedIndex == 1)
            {
                this._header.DisplayStandardCode = "1";
            }
            else if (this.comboBoxDisplayStandardCode.SelectedIndex == 2)
            {
                this._header.DisplayStandardCode = "2";
            }
            else
            {
                this._header.DisplayStandardCode = " ";
            }

            this._header.CharacterCodeTableNumber = "0" + this.comboBoxCharacterCodeTable.SelectedIndex;
            this._header.LanguageCode = this.textBoxLanguageCode.Text;
            if (this._header.LanguageCode.Length != 2)
            {
                this._header.LanguageCode = "0A";
            }

            this._header.OriginalProgrammeTitle = this.textBoxOriginalProgramTitle.Text.PadRight(32, ' ');
            this._header.OriginalEpisodeTitle = this.textBoxOriginalEpisodeTitle.Text.PadRight(32, ' ');
            this._header.TranslatedProgrammeTitle = this.textBoxTranslatedProgramTitle.Text.PadRight(32, ' ');
            this._header.TranslatedEpisodeTitle = this.textBoxTranslatedEpisodeTitle.Text.PadRight(32, ' ');
            this._header.TranslatorsName = this.textBoxTranslatorsName.Text.PadRight(32, ' ');
            this._header.SubtitleListReferenceCode = this.textBoxSubtitleListReferenceCode.Text.PadRight(16, ' ');
            this._header.CountryOfOrigin = this.textBoxCountryOfOrigin.Text;
            if (this._header.CountryOfOrigin.Length != 3)
            {
                this._header.CountryOfOrigin = "USA";
            }

            this._header.TimeCodeStatus = this.comboBoxTimeCodeStatus.SelectedIndex.ToString(CultureInfo.InvariantCulture);
            this._header.TimeCodeStartOfProgramme = this.timeUpDownStartTime.TimeCode.ToHHMMSSFF().Replace(":", string.Empty);

            this._header.RevisionNumber = this.numericUpDownRevisionNumber.Value.ToString("00");
            this._header.MaximumNumberOfDisplayableCharactersInAnyTextRow = this.numericUpDownMaxCharacters.Value.ToString("00");
            this._header.MaximumNumberOfDisplayableRows = this.numericUpDownMaxRows.Value.ToString("00");
            this._header.DiskSequenceNumber = this.numericUpDownDiskSequenceNumber.Value.ToString(CultureInfo.InvariantCulture);
            this._header.TotalNumberOfDisks = this.numericUpDownTotalNumberOfDiscs.Value.ToString(CultureInfo.InvariantCulture);
            this.JustificationCode = (byte)this.comboBoxJustificationCode.SelectedIndex;
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
        /// The button import_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "EBU STL files (*.stl)|*.stl";
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.FillHeaderFromFile(this.openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// The fill header from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void FillHeaderFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var ebu = new Ebu();
                var temp = new Subtitle();
                ebu.LoadSubtitle(temp, null, fileName);
                this.FillFromHeader(ebu.Header);
                if (ebu.JustificationCodes.Count > 2 && ebu.JustificationCodes[1] == ebu.JustificationCodes[2])
                {
                    if (ebu.JustificationCodes[1] >= 0 && ebu.JustificationCodes[1] < 4)
                    {
                        this.comboBoxJustificationCode.SelectedIndex = ebu.JustificationCodes[1];
                    }
                }
            }
        }

        /// <summary>
        /// The numeric up down max characters_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMaxCharacters_ValueChanged(object sender, EventArgs e)
        {
            this.CheckErrors(this._subtitle);
        }

        /// <summary>
        /// The united states tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void unitedStatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxCodePageNumber.Text = "437";
        }

        /// <summary>
        /// The multilingual tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void multilingualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxCodePageNumber.Text = "850";
        }

        /// <summary>
        /// The portugal tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void portugalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxCodePageNumber.Text = "860";
        }

        /// <summary>
        /// The canada french tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void canadaFrenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxCodePageNumber.Text = "863";
        }

        /// <summary>
        /// The nordic tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void nordicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxCodePageNumber.Text = "865";
        }
    }
}