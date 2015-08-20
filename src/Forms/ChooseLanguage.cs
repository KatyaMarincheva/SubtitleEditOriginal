// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChooseLanguage.cs" company="">
//   
// </copyright>
// <summary>
//   The choose language.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The choose language.
    /// </summary>
    public sealed partial class ChooseLanguage : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseLanguage"/> class.
        /// </summary>
        public ChooseLanguage()
        {
            this.InitializeComponent();

            List<string> list = new List<string>();
            if (Directory.Exists(Path.Combine(Configuration.BaseDirectory, "Languages")))
            {
                string[] versionInfo = Utilities.AssemblyVersion.Split('.');
                string currentVersion = string.Format("{0}.{1}.{2}", versionInfo[0], versionInfo[1], versionInfo[2]);

                foreach (string fileName in Directory.GetFiles(Path.Combine(Configuration.BaseDirectory, "Languages"), "*.xml"))
                {
                    string cultureName = Path.GetFileNameWithoutExtension(fileName);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fileName);
                    try
                    {
                        string version = doc.DocumentElement.SelectSingleNode("General/Version").InnerText;
                        if (version == currentVersion)
                        {
                            list.Add(cultureName);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            list.Sort();
            this.comboBoxLanguages.Items.Add(new CultureListItem(CultureInfo.CreateSpecificCulture("en-US")));
            foreach (string cultureName in list)
            {
                try
                {
                    var ci = CultureInfo.CreateSpecificCulture(cultureName);
                    if (!ci.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase))
                    {
                        ci = CultureInfo.GetCultureInfo(cultureName);
                    }

                    this.comboBoxLanguages.Items.Add(new CultureListItem(ci));
                }
                catch (ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine(cultureName + " is not a valid culture");
                }
            }

            int index = 0;
            for (int i = 0; i < this.comboBoxLanguages.Items.Count; i++)
            {
                var item = (CultureListItem)this.comboBoxLanguages.Items[i];
                if (item.Name == Configuration.Settings.Language.General.CultureName)
                {
                    index = i;
                }
            }

            this.comboBoxLanguages.SelectedIndex = index;

            this.Text = Configuration.Settings.Language.ChooseLanguage.Title;
            this.labelLanguage.Text = Configuration.Settings.Language.ChooseLanguage.Language;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        public string CultureName
        {
            get
            {
                int index = this.comboBoxLanguages.SelectedIndex;
                if (index == -1)
                {
                    return "en-US";
                }
                else
                {
                    return (this.comboBoxLanguages.Items[index] as CultureListItem).Name;
                }
            }
        }

        /// <summary>
        /// The change language_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChangeLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.Shift && e.Control && e.Alt && e.KeyCode == Keys.L)
            {
                Configuration.Settings.Language.Save(Path.Combine(Configuration.BaseDirectory, "LanguageMaster.xml"));
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#translate");
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The culture list item.
        /// </summary>
        public class CultureListItem
        {
            /// <summary>
            /// The _culture info.
            /// </summary>
            private CultureInfo _cultureInfo;

            /// <summary>
            /// Initializes a new instance of the <see cref="CultureListItem"/> class.
            /// </summary>
            /// <param name="cultureInfo">
            /// The culture info.
            /// </param>
            public CultureListItem(CultureInfo cultureInfo)
            {
                this._cultureInfo = cultureInfo;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name
            {
                get
                {
                    return this._cultureInfo.Name;
                }
            }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return char.ToUpper(this._cultureInfo.NativeName[0]) + this._cultureInfo.NativeName.Substring(1);
            }
        }
    }
}