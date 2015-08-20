// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTesseractDictionaries.cs" company="">
//   
// </copyright>
// <summary>
//   The get tesseract dictionaries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The get tesseract dictionaries.
    /// </summary>
    public partial class GetTesseractDictionaries : Form
    {
        /// <summary>
        /// The _descriptions.
        /// </summary>
        private List<string> _descriptions = new List<string>();

        /// <summary>
        /// The _dictionary download links.
        /// </summary>
        private List<string> _dictionaryDownloadLinks = new List<string>();

        /// <summary>
        /// The _xml name.
        /// </summary>
        private string _xmlName = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTesseractDictionaries"/> class.
        /// </summary>
        public GetTesseractDictionaries()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.GetTesseractDictionaries.Title;
            this.labelDescription1.Text = Configuration.Settings.Language.GetTesseractDictionaries.DescriptionLine1;
            this.linkLabelOpenDictionaryFolder.Text = Configuration.Settings.Language.GetTesseractDictionaries.OpenDictionariesFolder;
            this.labelChooseLanguageAndClickDownload.Text = Configuration.Settings.Language.GetTesseractDictionaries.ChooseLanguageAndClickDownload;
            this.buttonDownload.Text = Configuration.Settings.Language.GetTesseractDictionaries.Download;
            this.labelPleaseWait.Text = string.Empty;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.LoadDictionaryList("Nikse.SubtitleEdit.Resources.TesseractDictionaries.xml.gz");
            this.FixLargeFonts();
        }

        /// <summary>
        /// The load dictionary list.
        /// </summary>
        /// <param name="xmlRessourceName">
        /// The xml ressource name.
        /// </param>
        private void LoadDictionaryList(string xmlRessourceName)
        {
            this._dictionaryDownloadLinks = new List<string>();
            this._descriptions = new List<string>();
            this._xmlName = xmlRessourceName;
            Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            Stream strm = asm.GetManifestResourceStream(this._xmlName);
            if (strm != null)
            {
                this.comboBoxDictionaries.Items.Clear();
                XmlDocument doc = new XmlDocument();
                using (var rdr = new StreamReader(strm))
                using (var zip = new GZipStream(rdr.BaseStream, CompressionMode.Decompress))
                {
                    byte[] data = new byte[175000];
                    zip.Read(data, 0, 175000);
                    doc.LoadXml(System.Text.Encoding.UTF8.GetString(data));
                }

                foreach (XmlNode node in doc.DocumentElement.SelectNodes("Dictionary"))
                {
                    string englishName = node.SelectSingleNode("EnglishName").InnerText;
                    string downloadLink = node.SelectSingleNode("DownloadLink").InnerText;

                    string description = string.Empty;
                    if (node.SelectSingleNode("Description") != null)
                    {
                        description = node.SelectSingleNode("Description").InnerText;
                    }

                    if (!string.IsNullOrEmpty(downloadLink))
                    {
                        string name = englishName;

                        this.comboBoxDictionaries.Items.Add(name);
                        this._dictionaryDownloadLinks.Add(downloadLink);
                        this._descriptions.Add(description);
                    }

                    this.comboBoxDictionaries.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// The fix large fonts.
        /// </summary>
        private void FixLargeFonts()
        {
            if (this.labelDescription1.Left + this.labelDescription1.Width + 5 > this.Width)
            {
                this.Width = this.labelDescription1.Left + this.labelDescription1.Width + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The button download_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                this.labelPleaseWait.Text = Configuration.Settings.Language.General.PleaseWait;
                this.buttonOK.Enabled = false;
                this.buttonDownload.Enabled = false;
                this.comboBoxDictionaries.Enabled = false;
                this.Refresh();
                this.Cursor = Cursors.WaitCursor;

                int index = this.comboBoxDictionaries.SelectedIndex;
                string url = this._dictionaryDownloadLinks[index];

                var wc = new WebClient { Proxy = Utilities.GetProxy() };
                wc.DownloadDataCompleted += this.wc_DownloadDataCompleted;
                wc.DownloadDataAsync(new Uri(url));
                this.Cursor = Cursors.Default;
            }
            catch (Exception exception)
            {
                this.labelPleaseWait.Text = string.Empty;
                this.buttonOK.Enabled = true;
                this.buttonDownload.Enabled = true;
                this.comboBoxDictionaries.Enabled = true;
                this.Cursor = Cursors.Default;
                MessageBox.Show(exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace);
            }
        }

        /// <summary>
        /// The wc_ download data completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(Configuration.Settings.Language.GetTesseractDictionaries.DownloadFailed);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            string dictionaryFolder = Configuration.TesseractDataFolder;
            if (!Directory.Exists(dictionaryFolder))
            {
                Directory.CreateDirectory(dictionaryFolder);
            }

            int index = this.comboBoxDictionaries.SelectedIndex;

            var tempFileName = Path.GetTempFileName() + ".tar";
            using (var ms = new MemoryStream(e.Result))
            using (var fs = new FileStream(tempFileName, FileMode.Create))
            using (var zip = new GZipStream(ms, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[1024];
                int nRead;
                while ((nRead = zip.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, nRead);
                }
            }

            using (var tr = new TarReader(tempFileName))
            {
                foreach (var th in tr.Files)
                {
                    string fn = Path.Combine(dictionaryFolder, Path.GetFileName(th.FileName.Trim()));
                    th.WriteData(fn);
                }
            }

            File.Delete(tempFileName);

            this.Cursor = Cursors.Default;
            this.labelPleaseWait.Text = string.Empty;
            this.buttonOK.Enabled = true;
            this.buttonDownload.Enabled = true;
            this.comboBoxDictionaries.Enabled = true;
            MessageBox.Show(string.Format(Configuration.Settings.Language.GetDictionaries.XDownloaded, this.comboBoxDictionaries.Items[index]));
        }

        /// <summary>
        /// The link label open dictionary folder_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabelOpenDictionaryFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string dictionaryFolder = Configuration.TesseractDataFolder;
            if (!Directory.Exists(dictionaryFolder))
            {
                Directory.CreateDirectory(dictionaryFolder);
            }

            System.Diagnostics.Process.Start(dictionaryFolder);
        }

        /// <summary>
        /// The get tesseract dictionaries_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetTesseractDictionaries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#importvobsub");
                e.SuppressKeyPress = true;
            }
        }
    }
}