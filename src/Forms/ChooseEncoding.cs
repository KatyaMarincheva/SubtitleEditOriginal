// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChooseEncoding.cs" company="">
//   
// </copyright>
// <summary>
//   The choose encoding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The choose encoding.
    /// </summary>
    public sealed partial class ChooseEncoding : Form
    {
        /// <summary>
        /// The _encoding.
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// The _file buffer.
        /// </summary>
        private byte[] _fileBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseEncoding"/> class.
        /// </summary>
        public ChooseEncoding()
        {
            this.InitializeComponent();

            LanguageStructure.ChooseEncoding language = Configuration.Settings.Language.ChooseEncoding;
            this.Text = language.Title;
            this.LabelPreview.Text = Configuration.Settings.Language.General.Preview;
            this.listView1.Columns[0].Text = language.CodePage;
            this.listView1.Columns[1].Text = Configuration.Settings.Language.General.Name;
            this.listView1.Columns[2].Text = language.DisplayName;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void Initialize(string fileName)
        {
            try
            {
                var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                int length = (int)file.Length;
                if (length > 100000)
                {
                    length = 100000;
                }

                file.Position = 0;
                this._fileBuffer = new byte[length];
                file.Read(this._fileBuffer, 0, length);

                for (int i = 0; i < length; i++)
                {
                    if (this._fileBuffer[i] == 0)
                    {
                        this._fileBuffer[i] = 32;
                    }
                }

                file.Close();
            }
            catch
            {
                this._fileBuffer = new byte[0];
            }

            Encoding encoding = Utilities.DetectAnsiEncoding(this._fileBuffer);
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                var item = new ListViewItem(new[] { ei.CodePage.ToString(), ei.Name, ei.DisplayName });
                this.listView1.Items.Add(item);
                if (ei.CodePage == encoding.CodePage)
                {
                    item.Selected = true;
                }
            }

            this.listView1.ListViewItemSorter = new ListViewSorter { ColumnNumber = 0, IsNumber = true };
        }

        /// <summary>
        /// The form choose encoding_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormChooseEncoding_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The get encoding.
        /// </summary>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        internal Encoding GetEncoding()
        {
            return this._encoding;
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
            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show(Configuration.Settings.Language.ChooseEncoding.PleaseSelectAnEncoding);
            }
            else
            {
                this._encoding = Encoding.GetEncoding(int.Parse(this.listView1.SelectedItems[0].Text));
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The list view 1_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                Encoding encoding = Encoding.GetEncoding(int.Parse(this.listView1.SelectedItems[0].Text));
                this.textBoxPreview.Text = encoding.GetString(this._fileBuffer);
                this.LabelPreview.Text = Configuration.Settings.Language.General.Preview + " - " + encoding.EncodingName;
            }
        }

        /// <summary>
        /// The choose encoding_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChooseEncoding_Load(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count >= 1)
            {
                this.listView1.EnsureVisible(this.listView1.SelectedItems[0].Index);
            }
        }

        /// <summary>
        /// The list view 1_ column click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewSorter sorter = (ListViewSorter)this.listView1.ListViewItemSorter;

            if (e.Column == sorter.ColumnNumber)
            {
                sorter.Descending = !sorter.Descending; // inverse sort direction
            }
            else
            {
                sorter.ColumnNumber = e.Column;
                sorter.Descending = false;
                sorter.IsNumber = e.Column == 0; // only index 1 is numeric
            }

            this.listView1.Sort();
        }

        /// <summary>
        /// The list view sorter.
        /// </summary>
        private class ListViewSorter : System.Collections.IComparer
        {
            /// <summary>
            /// Gets or sets the column number.
            /// </summary>
            public int ColumnNumber { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is number.
            /// </summary>
            public bool IsNumber { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether descending.
            /// </summary>
            public bool Descending { get; set; }

            /// <summary>
            /// The compare.
            /// </summary>
            /// <param name="o1">
            /// The o 1.
            /// </param>
            /// <param name="o2">
            /// The o 2.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            public int Compare(object o1, object o2)
            {
                var lvi1 = o1 as ListViewItem;
                var lvi2 = o2 as ListViewItem;
                if (lvi1 == null || lvi2 == null)
                {
                    return 0;
                }

                if (this.Descending)
                {
                    ListViewItem temp = lvi1;
                    lvi1 = lvi2;
                    lvi2 = temp;
                }

                if (this.IsNumber)
                {
                    int i1 = int.Parse(lvi1.SubItems[this.ColumnNumber].Text);
                    int i2 = int.Parse(lvi2.SubItems[this.ColumnNumber].Text);

                    if (i1 > i2)
                    {
                        return 1;
                    }

                    if (i1 == i2)
                    {
                        return 0;
                    }

                    return -1;
                }

                return string.Compare(lvi2.SubItems[this.ColumnNumber].Text, lvi1.SubItems[this.ColumnNumber].Text, StringComparison.Ordinal);
            }
        }
    }
}