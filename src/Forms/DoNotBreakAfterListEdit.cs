// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotBreakAfterListEdit.cs" company="">
//   
// </copyright>
// <summary>
//   The do not break after list edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The do not break after list edit.
    /// </summary>
    public sealed partial class DoNotBreakAfterListEdit : Form
    {
        /// <summary>
        /// The _languages.
        /// </summary>
        private readonly List<string> _languages = new List<string>();

        /// <summary>
        /// The _no break after list.
        /// </summary>
        private List<NoBreakAfterItem> _noBreakAfterList = new List<NoBreakAfterItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotBreakAfterListEdit"/> class.
        /// </summary>
        public DoNotBreakAfterListEdit()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.Settings.UseDoNotBreakAfterList;
            this.labelLanguage.Text = Configuration.Settings.Language.ChooseLanguage.Language;
            this.buttonRemoveNoBreakAfter.Text = Configuration.Settings.Language.DvdSubRip.Remove;
            this.buttonAddNoBreakAfter.Text = Configuration.Settings.Language.DvdSubRip.Add;
            this.radioButtonText.Text = Configuration.Settings.Language.General.Text;
            this.radioButtonRegEx.Text = Configuration.Settings.Language.MultipleReplace.RegularExpression;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            this.radioButtonRegEx.Left = this.radioButtonText.Left + this.radioButtonText.Width + 10;
            foreach (string fileName in Directory.GetFiles(Configuration.DictionariesFolder, "*_NoBreakAfterList.xml"))
            {
                try
                {
                    string s = Path.GetFileName(fileName);
                    string languageId = s.Substring(0, s.IndexOf('_'));
                    var ci = CultureInfo.GetCultureInfoByIetfLanguageTag(languageId);
                    this.comboBoxDictionaries.Items.Add(ci.EnglishName + " (" + ci.NativeName + ")");
                    this._languages.Add(fileName);
                }
                catch
                {
                }
            }

            if (this.comboBoxDictionaries.Items.Count > 0)
            {
                this.comboBoxDictionaries.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The do not break after list edit_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DoNotBreakAfterListEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The combo box dictionaries_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxDictionaries_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = this.comboBoxDictionaries.SelectedIndex;
            if (idx >= 0)
            {
                this._noBreakAfterList = new List<NoBreakAfterItem>();
                var doc = new XmlDocument();
                doc.Load(this._languages[idx]);
                foreach (XmlNode node in doc.DocumentElement)
                {
                    if (!string.IsNullOrEmpty(node.InnerText))
                    {
                        if (node.Attributes["RegEx"] != null && node.Attributes["RegEx"].InnerText.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            var r = new Regex(node.InnerText, RegexOptions.Compiled);
                            this._noBreakAfterList.Add(new NoBreakAfterItem(r, node.InnerText));
                        }
                        else
                        {
                            this._noBreakAfterList.Add(new NoBreakAfterItem(node.InnerText));
                        }
                    }
                }

                this._noBreakAfterList.Sort();
                this.ShowBreakAfterList(this._noBreakAfterList);
            }
        }

        /// <summary>
        /// The show break after list.
        /// </summary>
        /// <param name="noBreakAfterList">
        /// The no break after list.
        /// </param>
        private void ShowBreakAfterList(List<NoBreakAfterItem> noBreakAfterList)
        {
            this.listBoxNoBreakAfter.Items.Clear();
            foreach (var item in noBreakAfterList)
            {
                if (item.Text != null)
                {
                    this.listBoxNoBreakAfter.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// The button remove name etc_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemoveNameEtc_Click(object sender, EventArgs e)
        {
            int first = 0;
            var list = new List<int>();
            foreach (int i in this.listBoxNoBreakAfter.SelectedIndices)
            {
                list.Add(i);
            }

            if (list.Count > 0)
            {
                first = list[0];
            }

            list.Sort();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                this._noBreakAfterList.RemoveAt(list[i]);
            }

            this.ShowBreakAfterList(this._noBreakAfterList);
            if (first >= this._noBreakAfterList.Count)
            {
                first = this._noBreakAfterList.Count - 1;
            }

            if (first >= 0)
            {
                this.listBoxNoBreakAfter.SelectedIndex = first;
            }

            this.comboBoxDictionaries.Enabled = false;
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
            int idx = this.comboBoxDictionaries.SelectedIndex;
            if (idx >= 0)
            {
                var doc = new XmlDocument();
                doc.LoadXml("<NoBreakAfterList />");
                foreach (NoBreakAfterItem item in this._noBreakAfterList)
                {
                    XmlNode node = doc.CreateElement("Item");
                    node.InnerText = item.Text;
                    if (item.Regex != null)
                    {
                        XmlAttribute attribute = doc.CreateAttribute("RegEx");
                        attribute.InnerText = true.ToString();
                        node.Attributes.Append(attribute);
                    }

                    doc.DocumentElement.AppendChild(node);
                }

                doc.Save(this._languages[idx]);
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button add names etc_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddNamesEtc_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxNoBreakAfter.Text))
            {
                return;
            }

            NoBreakAfterItem item;
            if (this.radioButtonText.Checked)
            {
                item = new NoBreakAfterItem(this.textBoxNoBreakAfter.Text);
            }
            else
            {
                if (!Utilities.IsValidRegex(this.textBoxNoBreakAfter.Text))
                {
                    MessageBox.Show(Configuration.Settings.Language.General.RegularExpressionIsNotValid);
                    return;
                }

                item = new NoBreakAfterItem(new Regex(this.textBoxNoBreakAfter.Text), this.textBoxNoBreakAfter.Text);
            }

            foreach (NoBreakAfterItem nbai in this._noBreakAfterList)
            {
                if ((nbai.Regex == null && item.Regex == null) || (nbai.Regex != null && item.Regex != null) && nbai.Text == item.Text)
                {
                    MessageBox.Show("Text already defined");
                    this.textBoxNoBreakAfter.Focus();
                    this.textBoxNoBreakAfter.SelectAll();
                    return;
                }
            }

            this._noBreakAfterList.Add(item);
            this.comboBoxDictionaries.Enabled = false;
            this.ShowBreakAfterList(this._noBreakAfterList);
            for (int i = 0; i < this.listBoxNoBreakAfter.Items.Count; i++)
            {
                if (this.listBoxNoBreakAfter.Items[i].ToString() == item.Text)
                {
                    this.listBoxNoBreakAfter.SelectedIndex = i;
                    return;
                }
            }

            this.textBoxNoBreakAfter.Text = string.Empty;
        }

        /// <summary>
        /// The radio button checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (sender == this.radioButtonRegEx)
            {
                this.textBoxNoBreakAfter.ContextMenu = FindReplaceDialogHelper.GetRegExContextMenu(this.textBoxNoBreakAfter);
            }
            else
            {
                this.textBoxNoBreakAfter.ContextMenuStrip = null;
            }
        }

        /// <summary>
        /// The list box names etc_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxNamesEtc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = this.listBoxNoBreakAfter.SelectedIndex;
            if (idx >= 0 && idx < this._noBreakAfterList.Count)
            {
                NoBreakAfterItem item = this._noBreakAfterList[idx];
                this.textBoxNoBreakAfter.Text = item.Text;
                bool isRegEx = item.Regex != null;
                this.radioButtonRegEx.Checked = isRegEx;
                this.radioButtonText.Checked = !isRegEx;
            }
        }

        /// <summary>
        /// The text box no break after_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxNoBreakAfter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.buttonAddNamesEtc_Click(sender, e);
            }
        }
    }
}