// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetDictionaries.cs" company="">
//   
// </copyright>
// <summary>
//   The get dictionaries.
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
    /// The get dictionaries.
    /// </summary>
    public sealed partial class GetDictionaries : Form
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
        /// Initializes a new instance of the <see cref="GetDictionaries"/> class.
        /// </summary>
        public GetDictionaries()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.GetDictionaries.Title;
            this.labelDescription1.Text = Configuration.Settings.Language.GetDictionaries.DescriptionLine1;
            this.labelDescription2.Text = Configuration.Settings.Language.GetDictionaries.DescriptionLine2;
            this.linkLabelOpenDictionaryFolder.Text = Configuration.Settings.Language.GetDictionaries.OpenDictionariesFolder;
            this.labelChooseLanguageAndClickDownload.Text = Configuration.Settings.Language.GetDictionaries.ChooseLanguageAndClickDownload;
            this.buttonDownload.Text = Configuration.Settings.Language.GetDictionaries.Download;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.labelPleaseWait.Text = string.Empty;

            this.LoadDictionaryList("Nikse.SubtitleEdit.Resources.OpenOfficeDictionaries.xml.gz");
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
                    byte[] data = new byte[275000];
                    int read = zip.Read(data, 0, 275000);
                    byte[] data2 = new byte[read];
                    Buffer.BlockCopy(data, 0, data2, 0, read);
                    string s = System.Text.Encoding.UTF8.GetString(data2).Trim();
                    doc.LoadXml(s);
                }

                foreach (XmlNode node in doc.DocumentElement.SelectNodes("Dictionary"))
                {
                    string englishName = node.SelectSingleNode("EnglishName").InnerText;
                    string nativeName = node.SelectSingleNode("NativeName").InnerText;
                    string downloadLink = node.SelectSingleNode("DownloadLink").InnerText;

                    string description = string.Empty;
                    if (node.SelectSingleNode("Description") != null)
                    {
                        description = node.SelectSingleNode("Description").InnerText;
                    }

                    if (!string.IsNullOrEmpty(downloadLink))
                    {
                        string name = englishName;
                        if (!string.IsNullOrEmpty(nativeName))
                        {
                            name += " - " + nativeName;
                        }

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
        /// The form get dictionaries_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormGetDictionaries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#spellcheck");
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The link label 4 link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LinkLabel4LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string dictionaryFolder = Utilities.DictionaryFolder;
            if (!Directory.Exists(dictionaryFolder))
            {
                Directory.CreateDirectory(dictionaryFolder);
            }

            System.Diagnostics.Process.Start(dictionaryFolder);
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
            if (e.Error != null && this._xmlName == "Nikse.SubtitleEdit.Resources.OpenOfficeDictionaries.xml.zip")
            {
                MessageBox.Show("Unable to connect to extensions.services.openoffice.org... Switching host - please re-try!");
                this.LoadDictionaryList("Nikse.SubtitleEdit.Resources.HunspellDictionaries.xml.gz");
                this.labelPleaseWait.Text = string.Empty;
                this.buttonOK.Enabled = true;
                this.buttonDownload.Enabled = true;
                this.comboBoxDictionaries.Enabled = true;
                this.Cursor = Cursors.Default;
                return;
            }
            else if (e.Error != null)
            {
                MessageBox.Show(Configuration.Settings.Language.GetTesseractDictionaries.DownloadFailed + Environment.NewLine + Environment.NewLine + e.Error.Message);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            string dictionaryFolder = Utilities.DictionaryFolder;
            if (!Directory.Exists(dictionaryFolder))
            {
                Directory.CreateDirectory(dictionaryFolder);
            }

            int index = this.comboBoxDictionaries.SelectedIndex;

            using (var ms = new MemoryStream(e.Result))
            using (ZipExtractor zip = ZipExtractor.Open(ms))
            {
                List<ZipExtractor.ZipFileEntry> dir = zip.ReadCentralDir();

                // Extract dic/aff files in dictionary folder
                bool found = false;
                ExtractDic(dictionaryFolder, zip, dir, ref found);

                if (!found)
                {
                    // check zip inside zip
                    foreach (ZipExtractor.ZipFileEntry entry in dir)
                    {
                        if (entry.FilenameInZip.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var innerMs = new MemoryStream())
                            {
                                zip.ExtractFile(entry, innerMs);
                                ZipExtractor innerZip = ZipExtractor.Open(innerMs);
                                List<ZipExtractor.ZipFileEntry> innerDir = innerZip.ReadCentralDir();
                                ExtractDic(dictionaryFolder, innerZip, innerDir, ref found);
                            }
                        }
                    }
                }
            }

            this.Cursor = Cursors.Default;
            this.labelPleaseWait.Text = string.Empty;
            this.buttonOK.Enabled = true;
            this.buttonDownload.Enabled = true;
            this.comboBoxDictionaries.Enabled = true;
            MessageBox.Show(string.Format(Configuration.Settings.Language.GetDictionaries.XDownloaded, this.comboBoxDictionaries.Items[index]));
        }

        /// <summary>
        /// The extract dic.
        /// </summary>
        /// <param name="dictionaryFolder">
        /// The dictionary folder.
        /// </param>
        /// <param name="zip">
        /// The zip.
        /// </param>
        /// <param name="dir">
        /// The dir.
        /// </param>
        /// <param name="found">
        /// The found.
        /// </param>
        private static void ExtractDic(string dictionaryFolder, ZipExtractor zip, List<ZipExtractor.ZipFileEntry> dir, ref bool found)
        {
            foreach (ZipExtractor.ZipFileEntry entry in dir)
            {
                if (entry.FilenameInZip.EndsWith(".dic", StringComparison.OrdinalIgnoreCase) || entry.FilenameInZip.EndsWith(".aff", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = Path.GetFileName(entry.FilenameInZip);

                    // French fix
                    if (fileName.StartsWith("fr-moderne", StringComparison.Ordinal))
                    {
                        fileName = fileName.Replace("fr-moderne", "fr_FR");
                    }

                    // German fix
                    if (fileName.StartsWith("de_DE_frami", StringComparison.Ordinal))
                    {
                        fileName = fileName.Replace("de_DE_frami", "de_DE");
                    }

                    // Russian fix
                    if (fileName.StartsWith("russian-aot", StringComparison.Ordinal))
                    {
                        fileName = fileName.Replace("russian-aot", "ru_RU");
                    }

                    string path = Path.Combine(dictionaryFolder, fileName);
                    zip.ExtractFile(entry, path);

                    found = true;
                }
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
            int index = this.comboBoxDictionaries.SelectedIndex;
            this.labelPleaseWait.Text = this._descriptions[index];
        }
    }
}