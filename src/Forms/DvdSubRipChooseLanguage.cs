// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DvdSubRipChooseLanguage.cs" company="">
//   
// </copyright>
// <summary>
//   The dvd sub rip choose language.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    /// The dvd sub rip choose language.
    /// </summary>
    public sealed partial class DvdSubRipChooseLanguage : Form
    {
        /// <summary>
        /// The _languages.
        /// </summary>
        private List<string> _languages;

        /// <summary>
        /// The _merged vob sub packs.
        /// </summary>
        private List<VobSubMergedPack> _mergedVobSubPacks;

        /// <summary>
        /// The _palette.
        /// </summary>
        private List<Color> _palette;

        /// <summary>
        /// Initializes a new instance of the <see cref="DvdSubRipChooseLanguage"/> class.
        /// </summary>
        public DvdSubRipChooseLanguage()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.DvdSubRipChooseLanguage.Title;
            this.labelChooseLanguage.Text = Configuration.Settings.Language.DvdSubRipChooseLanguage.ChooseLanguageStreamId;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonSaveAs.Text = Configuration.Settings.Language.Main.Menu.File.SaveAs;
            this.groupBoxImage.Text = Configuration.Settings.Language.DvdSubRipChooseLanguage.SubtitleImage;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the selected vob sub merged packs.
        /// </summary>
        public List<VobSubMergedPack> SelectedVobSubMergedPacks { get; private set; }

        /// <summary>
        /// Gets the selected language string.
        /// </summary>
        public string SelectedLanguageString { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="mergedVobSubPacks">
        /// The merged vob sub packs.
        /// </param>
        /// <param name="palette">
        /// The palette.
        /// </param>
        /// <param name="languages">
        /// The languages.
        /// </param>
        /// <param name="selectedLanguage">
        /// The selected language.
        /// </param>
        internal void Initialize(List<VobSubMergedPack> mergedVobSubPacks, List<Color> palette, List<string> languages, string selectedLanguage)
        {
            this._mergedVobSubPacks = mergedVobSubPacks;
            this._palette = palette;

            var uniqueLanguageStreamIds = new List<int>();
            foreach (var pack in mergedVobSubPacks)
            {
                if (!uniqueLanguageStreamIds.Contains(pack.StreamId))
                {
                    uniqueLanguageStreamIds.Add(pack.StreamId);
                }
            }

            this.comboBoxLanguages.Items.Clear();
            foreach (string languageName in languages)
            {
                if (uniqueLanguageStreamIds.Contains(GetLanguageIdFromString(languageName)))
                {
                    // only list languages actually found in vob
                    this.comboBoxLanguages.Items.Add(languageName);
                    if (languageName == selectedLanguage)
                    {
                        this.comboBoxLanguages.SelectedIndex = this.comboBoxLanguages.Items.Count - 1;
                    }

                    uniqueLanguageStreamIds.Remove(GetLanguageIdFromString(languageName));
                }
            }

            foreach (var existingLanguageId in uniqueLanguageStreamIds)
            {
                this.comboBoxLanguages.Items.Add(string.Format(Configuration.Settings.Language.DvdSubRipChooseLanguage.UnknownLanguage + " (0x{0:x})", existingLanguageId)); // subtitle track not supplied from IFO
            }

            if (this.comboBoxLanguages.Items.Count > 0 && this.comboBoxLanguages.SelectedIndex < 0)
            {
                this.comboBoxLanguages.SelectedIndex = 0;
            }

            this._languages = languages;
        }

        /// <summary>
        /// The show in srt format.
        /// </summary>
        /// <param name="ts">
        /// The ts.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ShowInSrtFormat(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00},{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        /// <summary>
        /// The list box 1 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = this.listBox1.Items[this.listBox1.SelectedIndex] as SubListBoxItem;

            Bitmap bmp = x.SubPack.SubPicture.GetBitmap(this._palette, Color.Transparent, Color.Wheat, Color.Black, Color.DarkGray, false);
            if (bmp.Width > this.pictureBoxImage.Width || bmp.Height > this.pictureBoxImage.Height)
            {
                float width = bmp.Width;
                float height = bmp.Height;
                while (width > this.pictureBoxImage.Width || height > this.pictureBoxImage.Height)
                {
                    width = width * 95 / 100;
                    height = height * 95 / 100;
                }

                var temp = new Bitmap((int)width, (int)height);
                using (var g = Graphics.FromImage(temp)) g.DrawImage(bmp, 0, 0, (int)width, (int)height);
                bmp = temp;
            }

            this.pictureBoxImage.Image = bmp;
            this.groupBoxImage.Text = string.Format(Configuration.Settings.Language.DvdSubRipChooseLanguage.SubtitleImageXofYAndWidthXHeight, this.listBox1.SelectedIndex + 1, this.listBox1.Items.Count, bmp.Width, bmp.Height);
        }

        /// <summary>
        /// The get language id from string.
        /// </summary>
        /// <param name="currentLanguage">
        /// The current language.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetLanguageIdFromString(string currentLanguage)
        {
            currentLanguage = currentLanguage.Substring(currentLanguage.IndexOf("0x", StringComparison.Ordinal) + 2).TrimEnd(')');
            return Convert.ToInt32(currentLanguage, 16);
        }

        /// <summary>
        /// The combo box languages selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxLanguagesSelectedIndexChanged(object sender, EventArgs e)
        {
            int chosenStreamId = GetLanguageIdFromString(this.comboBoxLanguages.Items[this.comboBoxLanguages.SelectedIndex].ToString());

            this.listBox1.Items.Clear();
            for (int i = 0; i < this._mergedVobSubPacks.Count; i++)
            {
                var x = this._mergedVobSubPacks[i];
                if (x.StreamId == chosenStreamId)
                {
                    string s = string.Format("#{0:000}: Stream-id=0X{1:X} - {2} --> {3}", i, x.StreamId, ShowInSrtFormat(x.StartTime), ShowInSrtFormat(x.EndTime));
                    this.listBox1.Items.Add(new SubListBoxItem(s, x));
                }
            }

            if (this.listBox1.Items.Count > 0)
            {
                this.listBox1.SelectedIndex = 0;
            }
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
            if (this._languages != null && this.comboBoxLanguages.SelectedIndex >= 0 && this.comboBoxLanguages.SelectedIndex < this._languages.Count)
            {
                this.SelectedLanguageString = this._languages[this.comboBoxLanguages.SelectedIndex];
            }
            else
            {
                this.SelectedLanguageString = null;
            }

            this.SelectedVobSubMergedPacks = new List<VobSubMergedPack>();
            foreach (var x in this.listBox1.Items)
            {
                this.SelectedVobSubMergedPacks.Add((x as SubListBoxItem).SubPack);
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button cancel click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The dvd sub rip show subtitles_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DvdSubRipShowSubtitles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button save as_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            if (this._languages != null && this.comboBoxLanguages.SelectedIndex >= 0 && this.comboBoxLanguages.SelectedIndex < this._languages.Count)
            {
                this.SelectedLanguageString = this._languages[this.comboBoxLanguages.SelectedIndex];
            }
            else
            {
                this.SelectedLanguageString = null;
            }

            var subs = new List<VobSubMergedPack>();
            foreach (var x in this.listBox1.Items)
            {
                subs.Add((x as SubListBoxItem).SubPack);
            }

            using (var formSubOcr = new VobSubOcr())
            {
                formSubOcr.InitializeQuick(subs, this._palette, Configuration.Settings.VobSubOcr, this.SelectedLanguageString);
                var subtitle = formSubOcr.ReadyVobSubRip();

                using (var exportBdnXmlPng = new ExportPngXml())
                {
                    exportBdnXmlPng.InitializeFromVobSubOcr(subtitle, new Logic.SubtitleFormats.SubRip(), "VOBSUB", "DVD", formSubOcr, this.SelectedLanguageString);
                    exportBdnXmlPng.ShowDialog(this);
                }
            }
        }

        /// <summary>
        /// The sub list box item.
        /// </summary>
        private class SubListBoxItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SubListBoxItem"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="subPack">
            /// The sub pack.
            /// </param>
            public SubListBoxItem(string name, VobSubMergedPack subPack)
            {
                this.Name = name;
                this.SubPack = subPack;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the sub pack.
            /// </summary>
            public VobSubMergedPack SubPack { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}