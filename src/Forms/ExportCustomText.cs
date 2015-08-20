// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportCustomText.cs" company="">
//   
// </copyright>
// <summary>
//   The export custom text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The export custom text.
    /// </summary>
    public partial class ExportCustomText : PositionAndSizeForm
    {
        /// <summary>
        /// The _subtitle.
        /// </summary>
        private readonly Subtitle _subtitle;

        /// <summary>
        /// The _templates.
        /// </summary>
        private readonly List<string> _templates = new List<string>();

        /// <summary>
        /// The _title.
        /// </summary>
        private readonly string _title;

        /// <summary>
        /// The _translated.
        /// </summary>
        private readonly Subtitle _translated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCustomText"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public ExportCustomText(Subtitle subtitle, Subtitle original, string title)
        {
            this.InitializeComponent();

            if (original == null || original.Paragraphs == null || original.Paragraphs.Count == 0)
            {
                this._subtitle = subtitle;
            }
            else
            {
                this._subtitle = original;
                this._translated = subtitle;
            }

            this._title = title;

            this.comboBoxEncoding.Items.Clear();
            int encodingSelectedIndex = 0;
            this.comboBoxEncoding.Items.Add(Encoding.UTF8.EncodingName);
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.Name != Encoding.UTF8.BodyName && ei.CodePage >= 949 && !ei.DisplayName.Contains("EBCDIC") && ei.CodePage != 1047)
                {
                    this.comboBoxEncoding.Items.Add(ei.CodePage + ": " + ei.DisplayName);
                    if (ei.Name == Configuration.Settings.General.DefaultEncoding)
                    {
                        encodingSelectedIndex = this.comboBoxEncoding.Items.Count - 1;
                    }
                }
            }

            this.comboBoxEncoding.SelectedIndex = encodingSelectedIndex;
            if (string.IsNullOrEmpty(Configuration.Settings.Tools.ExportCustomTemplates))
            {
                this._templates.Add("SubRipÆÆ{number}\r\n{start} --> {end}\r\n{text}\r\n\r\nÆhh:mm:ss,zzzÆ[Do not modify]Æ");
            }
            else
            {
                foreach (string template in Configuration.Settings.Tools.ExportCustomTemplates.Split('æ'))
                {
                    this._templates.Add(template);
                }
            }

            this.ShowTemplates(this._templates);

            var l = Configuration.Settings.Language.ExportCustomText;
            this.Text = l.Title;
            this.groupBoxFormats.Text = l.Formats;
            this.buttonSave.Text = l.SaveAs;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.groupBoxPreview.Text = Configuration.Settings.Language.General.Preview;
            this.labelEncoding.Text = Configuration.Settings.Language.Main.Controls.FileEncoding;
            this.columnHeader1.Text = Configuration.Settings.Language.General.Name;
            this.columnHeader2.Text = Configuration.Settings.Language.General.Text;
            this.buttonNew.Text = l.New;
            this.buttonEdit.Text = l.Edit;
            this.buttonDelete.Text = l.Delete;
            this.deleteToolStripMenuItem.Text = l.Delete;
            this.editToolStripMenuItem.Text = l.Edit;
            this.newToolStripMenuItem.Text = l.New;
        }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public override sealed string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// The show templates.
        /// </summary>
        /// <param name="templates">
        /// The templates.
        /// </param>
        private void ShowTemplates(List<string> templates)
        {
            this.listViewTemplates.Items.Clear();
            foreach (string s in templates)
            {
                var arr = s.Split('Æ');
                if (arr.Length == 6)
                {
                    var lvi = new ListViewItem(arr[0]);
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, arr[2].Replace(Environment.NewLine, "<br />")));
                    this.listViewTemplates.Items.Add(lvi);
                }
            }

            if (this.listViewTemplates.Items.Count > 0)
            {
                this.listViewTemplates.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// The button new_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonNew_Click(object sender, EventArgs e)
        {
            this.New();
        }

        /// <summary>
        /// The new.
        /// </summary>
        private void New()
        {
            using (var form = new ExportCustomTextFormat("NewÆÆ{number}\r\n{start} --> {end}\r\n{text}\r\n\r\nÆhh:mm:ss,zzzÆ[Do not modify]Æ"))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    this._templates.Add(form.FormatOk);
                    this.ShowTemplates(this._templates);
                    this.listViewTemplates.Items[this.listViewTemplates.Items.Count - 1].Selected = true;
                }
            }

            this.SaveTemplates();
        }

        /// <summary>
        /// The save templates.
        /// </summary>
        private void SaveTemplates()
        {
            var sb = new StringBuilder();
            foreach (string template in this._templates)
            {
                sb.Append(template + 'æ');
            }

            Configuration.Settings.Tools.ExportCustomTemplates = sb.ToString().TrimEnd('æ');
        }

        /// <summary>
        /// The button edit_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            this.Edit();
        }

        /// <summary>
        /// The edit.
        /// </summary>
        private void Edit()
        {
            if (this.listViewTemplates.SelectedItems.Count == 1)
            {
                int idx = this.listViewTemplates.SelectedItems[0].Index;
                using (var form = new ExportCustomTextFormat(this._templates[idx]))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        this._templates[idx] = form.FormatOk;
                        this.ShowTemplates(this._templates);
                        if (idx < this.listViewTemplates.Items.Count)
                        {
                            this.listViewTemplates.Items[idx].Selected = true;
                        }
                    }
                }
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
        /// The get current encoding.
        /// </summary>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        private Encoding GetCurrentEncoding()
        {
            if (this.comboBoxEncoding.Text == Encoding.UTF8.BodyName || this.comboBoxEncoding.Text == Encoding.UTF8.EncodingName || this.comboBoxEncoding.Text == "utf-8")
            {
                return Encoding.UTF8;
            }

            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.CodePage + ": " + ei.DisplayName == this.comboBoxEncoding.Text)
                {
                    return ei.GetEncoding();
                }
            }

            return Encoding.UTF8;
        }

        /// <summary>
        /// The button save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Title = Configuration.Settings.Language.ExportCustomText.SaveSubtitleAs;
            if (!string.IsNullOrEmpty(this._title))
            {
                this.saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(this._title) + ".txt";
            }

            this.saveFileDialog1.Filter = Configuration.Settings.Language.General.AllFiles + "|*.*";
            if (this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(this.saveFileDialog1.FileName, this.GenerateText(this._subtitle, this._translated, this._title), this.GetCurrentEncoding());
                    this.LogMessage = string.Format(Configuration.Settings.Language.ExportCustomText.SubtitleExportedInCustomFormatToX, this.saveFileDialog1.FileName);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        /// <summary>
        /// The generate text.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateText(Subtitle subtitle, Subtitle translation, string title)
        {
            if (this.listViewTemplates.SelectedItems.Count != 1)
            {
                return string.Empty;
            }

            if (title == null)
            {
                title = string.Empty;
            }
            else
            {
                title = Path.GetFileNameWithoutExtension(title);
            }

            try
            {
                int idx = this.listViewTemplates.SelectedItems[0].Index;
                var arr = this._templates[idx].Split('Æ');
                var sb = new StringBuilder();
                sb.Append(ExportCustomTextFormat.GetHeaderOrFooter(title, subtitle, arr[1]));
                string template = ExportCustomTextFormat.GetParagraphTemplate(arr[2]);
                for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
                {
                    Paragraph p = this._subtitle.Paragraphs[i];
                    string start = ExportCustomTextFormat.GetTimeCode(p.StartTime, arr[3]);
                    string end = ExportCustomTextFormat.GetTimeCode(p.EndTime, arr[3]);
                    string text = ExportCustomTextFormat.GetText(p.Text, arr[4]);

                    string translationText = string.Empty;
                    if (translation != null && translation.Paragraphs != null && translation.Paragraphs.Count > 0)
                    {
                        var trans = Utilities.GetOriginalParagraph(idx, p, translation.Paragraphs);
                        if (trans != null)
                        {
                            translationText = trans.Text;
                        }
                    }

                    string paragraph = ExportCustomTextFormat.GetParagraph(template, start, end, text, translationText, i, p.Duration, arr[3]);
                    sb.Append(paragraph);
                }

                sb.Append(ExportCustomTextFormat.GetHeaderOrFooter(title, subtitle, arr[5]));
                return sb.ToString();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// The list view templates_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxPreview.Text = this.GenerateText(this._subtitle, this._translated, this._title);
            this.buttonSave.Enabled = this.listViewTemplates.SelectedItems.Count == 1;
        }

        /// <summary>
        /// The delete tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Delete();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        private void Delete()
        {
            if (this.listViewTemplates.SelectedItems.Count != 1)
            {
                return;
            }

            int idx = this.listViewTemplates.SelectedItems[0].Index;
            for (int i = this.listViewTemplates.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = this.listViewTemplates.Items[i];
                if (item.Selected)
                {
                    string name = item.Text;
                    for (int j = this._templates.Count - 1; j > 0; j--)
                    {
                        if (this._templates[j].StartsWith(name + "ÆÆ"))
                        {
                            this._templates.RemoveAt(j);
                        }
                    }

                    item.Remove();
                }
            }

            if (idx >= this.listViewTemplates.Items.Count)
            {
                idx--;
            }

            if (idx >= 0)
            {
                this.listViewTemplates.Items[idx].Selected = true;
            }

            this.SaveTemplates();
        }

        /// <summary>
        /// The export custom text_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportCustomText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button delete_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            this.Delete();
        }

        /// <summary>
        /// The edit tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Edit();
        }

        /// <summary>
        /// The new tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.New();
        }

        /// <summary>
        /// The context menu strip 1_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listViewTemplates.SelectedItems.Count == 0)
            {
                this.toolStripMenuItem2.Visible = false;
                this.editToolStripMenuItem.Visible = false;
                this.deleteToolStripMenuItem.Visible = false;
            }
            else
            {
                this.toolStripMenuItem2.Visible = true;
                this.editToolStripMenuItem.Visible = true;
                this.deleteToolStripMenuItem.Visible = true;
            }
        }
    }
}