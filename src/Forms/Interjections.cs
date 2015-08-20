// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interjections.cs" company="">
//   
// </copyright>
// <summary>
//   The interjections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The interjections.
    /// </summary>
    public sealed partial class Interjections : Form
    {
        /// <summary>
        /// The _interjections.
        /// </summary>
        private List<string> _interjections;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interjections"/> class.
        /// </summary>
        public Interjections()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The get interjections semi colon seperated string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetInterjectionsSemiColonSeperatedString()
        {
            var sb = new StringBuilder();
            foreach (string s in this._interjections)
            {
                sb.Append(';');
                sb.Append(s.Trim());
            }

            return sb.ToString().Trim(';');
        }

        /// <summary>
        /// The interjections_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Interjections_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#remove_text_for_hi");
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="semiColonSeperatedList">
        /// The semi colon seperated list.
        /// </param>
        public void Initialize(string semiColonSeperatedList)
        {
            this._interjections = new List<string>();
            string[] arr = semiColonSeperatedList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arr)
            {
                this._interjections.Add(s.Trim());
            }

            this.FillListBox();
            this.Text = Configuration.Settings.Language.Interjections.Title;

            // Add to interjections (or general)
            this.buttonRemove.Text = Configuration.Settings.Language.Settings.Remove;
            this.buttonAdd.Text = Configuration.Settings.Language.MultipleReplace.Add;

            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The button add_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string text = this.textBoxInterjection.Text.Trim();
            if (text.Length > 1 && !this._interjections.Contains(text))
            {
                this._interjections.Add(text);
                this.FillListBox();
                this.textBoxInterjection.Text = string.Empty;
                this.textBoxInterjection.Focus();
                for (int i = 0; i < this.listBoxInterjections.Items.Count; i++)
                {
                    if (this.listBoxInterjections.Items[i].ToString() == text)
                    {
                        this.listBoxInterjections.SelectedIndex = i;
                        int top = i - 5;
                        if (top < 0)
                        {
                            top = 0;
                        }

                        this.listBoxInterjections.TopIndex = top;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show(Configuration.Settings.Language.Settings.WordAlreadyExists);
            }
        }

        /// <summary>
        /// The fill list box.
        /// </summary>
        private void FillListBox()
        {
            this._interjections.Sort();
            this.listBoxInterjections.BeginUpdate();
            this.listBoxInterjections.Items.Clear();
            foreach (string s in this._interjections)
            {
                this.listBoxInterjections.Items.Add(s);
            }

            this.listBoxInterjections.EndUpdate();
        }

        /// <summary>
        /// The button remove_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int index = this.listBoxInterjections.SelectedIndex;
            string text = this.listBoxInterjections.Items[index].ToString();
            if (index >= 0)
            {
                if (MessageBox.Show(string.Format(Configuration.Settings.Language.Settings.RemoveX, text), null, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this._interjections.Remove(text);
                    this.listBoxInterjections.Items.RemoveAt(index);
                    if (index < this.listBoxInterjections.Items.Count)
                    {
                        this.listBoxInterjections.SelectedIndex = index;
                    }
                    else if (this.listBoxInterjections.Items.Count > 0)
                    {
                        this.listBoxInterjections.SelectedIndex = index - 1;
                    }

                    this.listBoxInterjections.Focus();

                    return;
                }

                MessageBox.Show(Configuration.Settings.Language.Settings.WordNotFound);
            }
        }

        /// <summary>
        /// The list box interjections_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxInterjections_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemove.Enabled = this.listBoxInterjections.SelectedIndex >= 0;
        }

        /// <summary>
        /// The text box interjection_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxInterjection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.buttonAdd_Click(null, null);
            }
        }
    }
}