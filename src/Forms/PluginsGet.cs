// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsGet.cs" company="">
//   
// </copyright>
// <summary>
//   The plugins get.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The plugins get.
    /// </summary>
    public sealed partial class PluginsGet : Form
    {
        /// <summary>
        /// The _language.
        /// </summary>
        private readonly LanguageStructure.PluginsGet _language;

        /// <summary>
        /// The _plugin doc.
        /// </summary>
        private readonly XmlDocument _pluginDoc = new XmlDocument();

        /// <summary>
        /// The _downloaded plugin name.
        /// </summary>
        private string _downloadedPluginName;

        /// <summary>
        /// The _update all list names.
        /// </summary>
        private List<string> _updateAllListNames;

        /// <summary>
        /// The _update all list urls.
        /// </summary>
        private List<string> _updateAllListUrls;

        /// <summary>
        /// The _updating all plugins.
        /// </summary>
        private bool _updatingAllPlugins;

        /// <summary>
        /// The _updating all plugins count.
        /// </summary>
        private int _updatingAllPluginsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsGet"/> class.
        /// </summary>
        public PluginsGet()
        {
            this.InitializeComponent();
            this._language = Configuration.Settings.Language.PluginsGet;
            this.Text = this._language.Title;
            this.tabPageInstalledPlugins.Text = this._language.InstalledPlugins;
            this.tabPageGetPlugins.Text = this._language.GetPlugins;

            this.buttonDownload.Text = this._language.Download;
            this.buttonRemove.Text = this._language.Remove;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.linkLabelOpenPluginFolder.Text = this._language.OpenPluginsFolder;
            this.labelDescription1.Text = this._language.GetPluginsInfo1;
            this.labelClickDownload.Text = this._language.GetPluginsInfo2;

            this.columnHeaderName.Text = Configuration.Settings.Language.General.Name;
            this.columnHeaderDescription.Text = this._language.Description;
            this.columnHeaderVersion.Text = this._language.Version;
            this.columnHeaderDate.Text = this._language.Date;

            this.columnHeaderInsName.Text = Configuration.Settings.Language.General.Name;
            this.columnHeaderInsDescription.Text = this._language.Description;
            this.columnHeaderInsVersion.Text = this._language.Version;
            this.columnHeaderInsType.Text = this._language.Type;

            this.buttonUpdateAll.Visible = false;
            try
            {
                this.labelPleaseWait.Text = Configuration.Settings.Language.General.PleaseWait;
                this.Refresh();
                this.ShowInstalledPlugins();
                string url = GetPluginXmlFileUrl();
                var wc = new WebClient { Proxy = Utilities.GetProxy(), Encoding = System.Text.Encoding.UTF8 };
                wc.Headers.Add("Accept-Encoding", string.Empty);
                wc.DownloadStringCompleted += this.wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri(url));
            }
            catch (Exception exception)
            {
                this.labelPleaseWait.Text = string.Empty;
                this.buttonOK.Enabled = true;
                this.buttonDownload.Enabled = true;
                this.listViewGetPlugins.Enabled = true;
                MessageBox.Show(exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace);
            }
        }

        /// <summary>
        /// The get plugin xml file url.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPluginXmlFileUrl()
        {
            if (Environment.Version.Major < 4)
            {
                return "https://raw.github.com/SubtitleEdit/plugins/master/Plugins2.xml"; // .net 2-3.5
            }

            return "https://raw.github.com/SubtitleEdit/plugins/master/Plugins4.xml"; // .net 4-?
        }

        /// <summary>
        /// The wc_ download string completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            this.labelPleaseWait.Text = string.Empty;
            if (e.Error != null)
            {
                MessageBox.Show(string.Format(this._language.UnableToDownloadPluginListX, e.Error.Message));
                if (e.Error.InnerException != null)
                {
                    MessageBox.Show(e.Error.InnerException.Source + ": " + e.Error.InnerException.Message + Environment.NewLine + Environment.NewLine + e.Error.InnerException.StackTrace);
                }

                return;
            }

            this._updateAllListUrls = new List<string>();
            this._updateAllListNames = new List<string>();
            this.listViewGetPlugins.BeginUpdate();
            try
            {
                this._pluginDoc.LoadXml(e.Result);
                string[] arr = this._pluginDoc.DocumentElement.SelectSingleNode("SubtitleEditVersion").InnerText.Split(new[] { '.', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                double requiredVersion = Convert.ToDouble(arr[0] + "." + arr[1], CultureInfo.InvariantCulture);

                string[] versionInfo = Utilities.AssemblyVersion.Split('.');
                double currentVersion = Convert.ToDouble(versionInfo[0] + "." + versionInfo[1], CultureInfo.InvariantCulture);

                if (currentVersion < requiredVersion)
                {
                    MessageBox.Show(this._language.NewVersionOfSubtitleEditRequired);
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                foreach (XmlNode node in this._pluginDoc.DocumentElement.SelectNodes("Plugin"))
                {
                    var item = new ListViewItem(node.SelectSingleNode("Name").InnerText.Trim('.'));
                    item.SubItems.Add(node.SelectSingleNode("Description").InnerText);
                    item.SubItems.Add(node.SelectSingleNode("Version").InnerText);
                    item.SubItems.Add(node.SelectSingleNode("Date").InnerText);
                    this.listViewGetPlugins.Items.Add(item);

                    foreach (ListViewItem installed in this.listViewInstalledPlugins.Items)
                    {
                        var installedVer = Convert.ToDouble(installed.SubItems[2].Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                        var currentVer = Convert.ToDouble(node.SelectSingleNode("Version").InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);

                        if (string.Compare(installed.Text, node.SelectSingleNode("Name").InnerText.Trim('.'), StringComparison.OrdinalIgnoreCase) == 0 && installedVer < currentVer)
                        {
                            // item.BackColor = Color.LightGreen;
                            installed.BackColor = Color.LightPink;
                            installed.SubItems[1].Text = this._language.UpdateAvailable + " " + installed.SubItems[1].Text;
                            this.buttonUpdateAll.Visible = true;
                            this._updateAllListUrls.Add(node.SelectSingleNode("Url").InnerText);
                            this._updateAllListNames.Add(node.SelectSingleNode("Name").InnerText);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format(this._language.UnableToDownloadPluginListX, exception.Source + ": " + exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace));
            }

            this.listViewGetPlugins.EndUpdate();

            if (this._updateAllListUrls.Count > 0)
            {
                this.buttonUpdateAll.BackColor = Color.LightGreen;
                if (Configuration.Settings.Language.PluginsGet.UpdateAllX != null)
                {
                    this.buttonUpdateAll.Text = string.Format(Configuration.Settings.Language.PluginsGet.UpdateAllX, this._updateAllListUrls.Count);
                }
                else
                {
                    this.buttonUpdateAll.Text = Configuration.Settings.Language.PluginsGet.UpdateAll;
                }

                this.buttonUpdateAll.Visible = true;
            }
        }

        /// <summary>
        /// The show installed plugins.
        /// </summary>
        private void ShowInstalledPlugins()
        {
            string path = Configuration.PluginsDirectory;
            if (!Directory.Exists(path))
            {
                return;
            }

            this.listViewInstalledPlugins.BeginUpdate();
            this.listViewInstalledPlugins.Items.Clear();
            foreach (string pluginFileName in Directory.GetFiles(path, "*.DLL"))
            {
                string name, description, text, shortcut, actionType;
                decimal version;
                MethodInfo mi;
                Main.GetPropertiesAndDoAction(pluginFileName, out name, out text, out version, out description, out actionType, out shortcut, out mi);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(actionType) && mi != null)
                {
                    try
                    {
                        var item = new ListViewItem(name.Trim('.'));
                        item.Tag = pluginFileName;
                        item.SubItems.Add(description);
                        item.SubItems.Add(version.ToString());
                        item.SubItems.Add(actionType);
                        this.listViewInstalledPlugins.Items.Add(item);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error loading plugin:" + pluginFileName + ": " + exception.Message);
                    }
                }
            }

            this.listViewInstalledPlugins.EndUpdate();
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
            if (this.listViewGetPlugins.SelectedItems.Count == 0)
            {
                return;
            }

            try
            {
                this.labelPleaseWait.Text = Configuration.Settings.Language.General.PleaseWait;
                this.buttonOK.Enabled = false;
                this.buttonDownload.Enabled = false;
                this.listViewGetPlugins.Enabled = false;
                this.Refresh();
                this.Cursor = Cursors.WaitCursor;

                int index = this.listViewGetPlugins.SelectedItems[0].Index;
                string url = this._pluginDoc.DocumentElement.SelectNodes("Plugin")[index].SelectSingleNode("Url").InnerText;
                this._downloadedPluginName = this._pluginDoc.DocumentElement.SelectNodes("Plugin")[index].SelectSingleNode("Name").InnerText;

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
                this.listViewGetPlugins.Enabled = true;
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
            this.labelPleaseWait.Text = string.Empty;
            if (e.Error != null)
            {
                MessageBox.Show(Configuration.Settings.Language.GetTesseractDictionaries.DownloadFailed);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            string pluginsFolder = Configuration.PluginsDirectory;
            if (!Directory.Exists(pluginsFolder))
            {
                try
                {
                    Directory.CreateDirectory(pluginsFolder);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Unable to create plugin folder " + pluginsFolder + ": " + exception.Message);
                    return;
                }
            }

            var ms = new MemoryStream(e.Result);
            using (ZipExtractor zip = ZipExtractor.Open(ms))
            {
                List<ZipExtractor.ZipFileEntry> dir = zip.ReadCentralDir();

                // Extract dic/aff files in dictionary folder
                foreach (ZipExtractor.ZipFileEntry entry in dir)
                {
                    string fileName = Path.GetFileName(entry.FilenameInZip);
                    string fullPath = Path.Combine(pluginsFolder, fileName);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch
                        {
                            MessageBox.Show(string.Format("{0} already exists - unable to overwrite it", fullPath));
                            this.Cursor = Cursors.Default;
                            this.labelPleaseWait.Text = string.Empty;
                            this.buttonOK.Enabled = true;
                            this.buttonDownload.Enabled = true;
                            this.listViewGetPlugins.Enabled = true;
                            return;
                        }
                    }

                    zip.ExtractFile(entry, fullPath);
                }
            }

            this.Cursor = Cursors.Default;
            this.labelPleaseWait.Text = string.Empty;
            this.buttonOK.Enabled = true;
            this.buttonDownload.Enabled = true;
            this.listViewGetPlugins.Enabled = true;
            if (this._updatingAllPlugins)
            {
                this._updatingAllPluginsCount++;
                if (this._updatingAllPluginsCount == this._updateAllListUrls.Count)
                {
                    MessageBox.Show(string.Format(this._language.XPluginsUpdated, this._updatingAllPluginsCount));
                }
            }
            else
            {
                MessageBox.Show(string.Format(this._language.PluginXDownloaded, this._downloadedPluginName));
            }

            this.ShowInstalledPlugins();
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
            string pluginsFolder = Configuration.PluginsDirectory;
            if (!Directory.Exists(pluginsFolder))
            {
                try
                {
                    Directory.CreateDirectory(pluginsFolder);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Unable to create plugin folder " + pluginsFolder + ": " + exception.Message);
                    return;
                }
            }

            try
            {
                System.Diagnostics.Process.Start(pluginsFolder);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Cannot open folder: " + pluginsFolder + Environment.NewLine + Environment.NewLine + exception.Source + ":" + exception.Message);
            }
        }

        /// <summary>
        /// The plugins get_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PluginsGet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#plugins");
                e.SuppressKeyPress = true;
            }
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
            if (this.listViewInstalledPlugins.SelectedItems.Count < 1)
            {
                return;
            }

            string fileName = this.listViewInstalledPlugins.SelectedItems[0].Tag.ToString();
            int index = this.listViewInstalledPlugins.SelectedItems[0].Index;
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    return;
                }
            }

            this.listViewInstalledPlugins.Items.RemoveAt(index);
            if (index >= this.listViewInstalledPlugins.Items.Count)
            {
                index--;
            }

            if (index >= 0)
            {
                this.listViewInstalledPlugins.Items[index].Selected = true;
                this.listViewInstalledPlugins.Items[index].Focused = true;
            }
        }

        /// <summary>
        /// The button update all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonUpdateAll_Click(object sender, EventArgs e)
        {
            this.buttonUpdateAll.Enabled = false;
            this.buttonUpdateAll.BackColor = DefaultBackColor;
            try
            {
                this.labelPleaseWait.Text = Configuration.Settings.Language.General.PleaseWait;
                this.buttonOK.Enabled = false;
                this.buttonDownload.Enabled = false;
                this.listViewGetPlugins.Enabled = false;
                this.Refresh();
                this.Cursor = Cursors.WaitCursor;
                this._updatingAllPluginsCount = 0;
                this._updatingAllPlugins = true;
                for (int i = 0; i < this._updateAllListUrls.Count; i++)
                {
                    var wc = new WebClient { Proxy = Utilities.GetProxy() };
                    wc.DownloadDataCompleted += this.wc_DownloadDataCompleted;
                    wc.DownloadDataAsync(new Uri(this._updateAllListUrls[i]));
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception exception)
            {
                this.labelPleaseWait.Text = string.Empty;
                this.buttonOK.Enabled = true;
                this.buttonDownload.Enabled = true;
                this.listViewGetPlugins.Enabled = true;
                this.Cursor = Cursors.Default;
                MessageBox.Show(exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace);
            }
        }
    }
}