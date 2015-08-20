// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YouTubeAnnotationsImport.cs" company="">
//   
// </copyright>
// <summary>
//   The you tube annotations import.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The you tube annotations import.
    /// </summary>
    public sealed partial class YouTubeAnnotationsImport : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YouTubeAnnotationsImport"/> class.
        /// </summary>
        /// <param name="stylesWithCount">
        /// The styles with count.
        /// </param>
        public YouTubeAnnotationsImport(Dictionary<string, int> stylesWithCount)
        {
            this.InitializeComponent();

            this.Text = "YouTube Annotations";
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            // listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = string.Empty; // style // TODO: Add better text + help text
            Utilities.FixLargeFonts(this, this.buttonOK);

            foreach (KeyValuePair<string, int> kvp in stylesWithCount)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Add(kvp.Key);
                item.SubItems.Add(kvp.Value.ToString());
                item.Checked = kvp.Key.Trim() == "speech";
                this.listViewFixes.Items.Add(item);
            }
        }

        /// <summary>
        /// Gets the selected styles.
        /// </summary>
        public List<string> SelectedStyles
        {
            get
            {
                List<string> styles = new List<string>();
                foreach (ListViewItem item in this.listViewFixes.Items)
                {
                    if (item.Checked)
                    {
                        styles.Add(item.SubItems[1].Text);
                    }
                }

                return styles;
            }
        }

        /// <summary>
        /// The you tube annotations import_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void YouTubeAnnotationsImport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}