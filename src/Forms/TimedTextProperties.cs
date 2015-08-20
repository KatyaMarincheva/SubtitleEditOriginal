// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedTextProperties.cs" company="">
//   
// </copyright>
// <summary>
//   The timed text properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The timed text properties.
    /// </summary>
    public partial class TimedTextProperties : PositionAndSizeForm
    {
        /// <summary>
        /// The _ na.
        /// </summary>
        private string _NA;

        /// <summary>
        /// The _nsmgr.
        /// </summary>
        private XmlNamespaceManager _nsmgr;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _xml.
        /// </summary>
        private XmlDocument _xml;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTextProperties"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public TimedTextProperties(Subtitle subtitle)
        {
            this.InitializeComponent();
            Application.DoEvents();

            this._subtitle = subtitle;
            this._NA = "[" + Configuration.Settings.Language.General.NotAvailable + "]";
            this.comboBoxDropMode.Items[0] = this._NA;
            this.comboBoxTimeBase.Items[0] = this._NA;
            this.comboBoxDefaultStyle.Items.Add(this._NA);
            this.comboBoxDefaultRegion.Items.Add(this._NA);

            this._xml = new XmlDocument();
            try
            {
                this._xml.LoadXml(subtitle.Header);
            }
            catch
            {
                subtitle.Header = new TimedText10().ToText(new Subtitle(), "tt");
                this._xml.LoadXml(subtitle.Header); // load default xml
            }

            this._nsmgr = new XmlNamespaceManager(this._xml.NameTable);
            this._nsmgr.AddNamespace("ttml", "http://www.w3.org/ns/ttml");

            XmlNode node = this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata/ttml:title", this._nsmgr);
            if (node != null)
            {
                this.textBoxTitle.Text = node.InnerText;
            }

            node = this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata/ttml:desc", this._nsmgr);
            if (node != null)
            {
                this.textBoxDescription.Text = node.InnerText;
            }

            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                this.comboBoxLanguage.Items.Add(ci.Name);
            }

            XmlAttribute attr = this._xml.DocumentElement.Attributes["xml:lang"];
            if (attr != null)
            {
                this.comboBoxLanguage.Text = attr.InnerText;
            }

            attr = this._xml.DocumentElement.Attributes["ttp:timeBase"];
            if (attr != null)
            {
                this.comboBoxTimeBase.Text = attr.InnerText;
            }

            this.comboBoxFrameRate.Items.Add("23.976");
            this.comboBoxFrameRate.Items.Add("24.0");
            this.comboBoxFrameRate.Items.Add("25.0");
            this.comboBoxFrameRate.Items.Add("29.97");
            this.comboBoxFrameRate.Items.Add("30.0");
            attr = this._xml.DocumentElement.Attributes["ttp:frameRate"];
            if (attr != null)
            {
                this.comboBoxFrameRate.Text = attr.InnerText;
            }

            attr = this._xml.DocumentElement.Attributes["ttp:frameRateMultiplier"];
            if (attr != null)
            {
                this.comboBoxFrameRateMultiplier.Text = attr.InnerText;
            }

            attr = this._xml.DocumentElement.Attributes["ttp:dropMode"];
            if (attr != null)
            {
                this.comboBoxDropMode.Text = attr.InnerText;
            }

            foreach (string style in TimedText10.GetStylesFromHeader(this._subtitle.Header))
            {
                this.comboBoxDefaultStyle.Items.Add(style);
                node = this._xml.DocumentElement.SelectSingleNode("ttml:body", this._nsmgr);
                if (node != null && node.Attributes["style"] != null && style == node.Attributes["style"].Value)
                {
                    this.comboBoxDefaultStyle.SelectedIndex = this.comboBoxDefaultStyle.Items.Count - 1;
                }
            }

            foreach (string region in TimedText10.GetRegionsFromHeader(this._subtitle.Header))
            {
                this.comboBoxDefaultRegion.Items.Add(region);
                node = this._xml.DocumentElement.SelectSingleNode("ttml:body", this._nsmgr);
                if (node != null && node.Attributes["region"] != null && region == node.Attributes["region"].Value)
                {
                    this.comboBoxDefaultRegion.SelectedIndex = this.comboBoxDefaultRegion.Items.Count - 1;
                }
            }

            var timeCodeFormat = Configuration.Settings.SubtitleSettings.TimedText10TimeCodeFormat.Trim().ToLowerInvariant();
            switch (timeCodeFormat)
            {
                case "seconds":
                    this.comboBoxTimeCodeFormat.SelectedIndex = 2;
                    break;
                case "milliseconds":
                    this.comboBoxTimeCodeFormat.SelectedIndex = 3;
                    break;
                case "ticks":
                    this.comboBoxTimeCodeFormat.SelectedIndex = 4;
                    break;
                case "hh:mm:ss.ms":
                    this.comboBoxTimeCodeFormat.SelectedIndex = 1;
                    break;
                case "default":
                    this.comboBoxTimeCodeFormat.SelectedIndex = 5;
                    break;
                default: // hh:mm:ss:ff
                    this.comboBoxTimeCodeFormat.SelectedIndex = 0;
                    break;
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
            XmlNode node = this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata/ttml:title", this._nsmgr);
            if (node != null)
            {
                if (string.IsNullOrWhiteSpace(this.textBoxTitle.Text) && string.IsNullOrWhiteSpace(this.textBoxDescription.Text))
                {
                    this._xml.DocumentElement.SelectSingleNode("ttml:head", this._nsmgr).RemoveChild(this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata", this._nsmgr));
                }
                else
                {
                    node.InnerText = this.textBoxTitle.Text;
                }
            }
            else if (!string.IsNullOrWhiteSpace(this.textBoxTitle.Text))
            {
                var head = this._xml.DocumentElement.SelectSingleNode("ttml:head", this._nsmgr);
                if (head == null)
                {
                    head = this._xml.CreateElement("ttml", "head", this._nsmgr.LookupNamespace("ttml"));
                    this._xml.DocumentElement.PrependChild(head);
                }

                var metadata = this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata", this._nsmgr);
                if (metadata == null)
                {
                    metadata = this._xml.CreateElement("ttml", "metadata", this._nsmgr.LookupNamespace("ttml"));
                    head.PrependChild(metadata);
                }

                var title = this._xml.CreateElement("ttml", "title", this._nsmgr.LookupNamespace("ttml"));
                metadata.InnerText = this.textBoxTitle.Text;
                metadata.AppendChild(title);
            }

            node = this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata/ttml:desc", this._nsmgr);
            if (node != null)
            {
                node.InnerText = this.textBoxDescription.Text;
            }
            else if (!string.IsNullOrWhiteSpace(this.textBoxDescription.Text))
            {
                var head = this._xml.DocumentElement.SelectSingleNode("ttml:head", this._nsmgr);
                if (head == null)
                {
                    head = this._xml.CreateElement("ttml", "head", this._nsmgr.LookupNamespace("ttml"));
                    this._xml.DocumentElement.PrependChild(head);
                }

                var metadata = this._xml.DocumentElement.SelectSingleNode("ttml:head/ttml:metadata", this._nsmgr);
                if (metadata == null)
                {
                    metadata = this._xml.CreateElement("ttml", "metadata", this._nsmgr.LookupNamespace("ttml"));
                    head.PrependChild(metadata);
                }

                var desc = this._xml.CreateElement("ttml", "desc", this._nsmgr.LookupNamespace("ttml"));
                desc.InnerText = this.textBoxDescription.Text;
                metadata.AppendChild(desc);
            }

            XmlAttribute attr = this._xml.DocumentElement.Attributes["xml:lang"];
            if (attr != null)
            {
                attr.Value = this.comboBoxLanguage.Text;
                if (attr.Value.Length == 0)
                {
                    this._xml.DocumentElement.Attributes.Remove(attr);
                }
            }
            else if (this.comboBoxLanguage.Text.Length > 0)
            {
                attr = this._xml.CreateAttribute("xml", "lang", this._nsmgr.LookupNamespace("xml"));
                attr.Value = this.comboBoxLanguage.Text;
                this._xml.DocumentElement.Attributes.Prepend(attr);
            }

            attr = this._xml.DocumentElement.Attributes["ttp:timeBase"];
            if (attr != null)
            {
                attr.InnerText = this.comboBoxTimeBase.Text;
                if (attr.Value.Length == 0)
                {
                    this._xml.DocumentElement.Attributes.Remove(attr);
                }
            }
            else if (this.comboBoxTimeBase.Text.Length > 0)
            {
                attr = this._xml.CreateAttribute("ttp", "timeBase", this._nsmgr.LookupNamespace("ttp"));
                attr.Value = this.comboBoxTimeBase.Text;
                this._xml.DocumentElement.Attributes.Append(attr);
            }

            attr = this._xml.DocumentElement.Attributes["ttp:frameRate"];
            if (attr != null)
            {
                attr.InnerText = this.comboBoxFrameRate.Text;
                if (attr.Value.Length == 0)
                {
                    this._xml.DocumentElement.Attributes.Remove(attr);
                }
            }
            else if (this.comboBoxFrameRate.Text.Length > 0)
            {
                attr = this._xml.CreateAttribute("ttp", "frameRate", this._nsmgr.LookupNamespace("ttp"));
                attr.Value = this.comboBoxFrameRate.Text;
                this._xml.DocumentElement.Attributes.Append(attr);
            }

            attr = this._xml.DocumentElement.Attributes["ttp:frameRateMultiplier"];
            if (attr != null)
            {
                attr.InnerText = this.comboBoxFrameRateMultiplier.Text;
                if (attr.Value.Length == 0)
                {
                    this._xml.DocumentElement.Attributes.Remove(attr);
                }
            }
            else if (this.comboBoxFrameRateMultiplier.Text.Length > 0)
            {
                attr = this._xml.CreateAttribute("ttp", "frameRateMultiplier", this._nsmgr.LookupNamespace("ttp"));
                attr.Value = this.comboBoxFrameRateMultiplier.Text;
                this._xml.DocumentElement.Attributes.Append(attr);
            }

            attr = this._xml.DocumentElement.Attributes["ttp:dropMode"];
            if (attr != null)
            {
                attr.InnerText = this.comboBoxDropMode.Text;
                if (attr.Value.Length == 0)
                {
                    this._xml.DocumentElement.Attributes.Remove(attr);
                }
            }
            else if (this.comboBoxDropMode.Text.Length > 0)
            {
                attr = this._xml.CreateAttribute("ttp", "dropMode", this._nsmgr.LookupNamespace("ttp"));
                attr.Value = this.comboBoxDropMode.Text;
                this._xml.DocumentElement.Attributes.Append(attr);
            }

            node = this._xml.DocumentElement.SelectSingleNode("ttml:body", this._nsmgr);
            if (node != null && node.Attributes["style"] != null)
            {
                node.Attributes["style"].Value = this.comboBoxDefaultStyle.Text;
            }
            else if (this.comboBoxDefaultStyle.Text.Length > 0 && node != null)
            {
                attr = this._xml.CreateAttribute("style");
                attr.Value = this.comboBoxDefaultStyle.Text;
                node.Attributes.Append(attr);
            }

            node = this._xml.DocumentElement.SelectSingleNode("ttml:body", this._nsmgr);
            if (node != null && node.Attributes["region"] != null)
            {
                node.Attributes["region"].Value = this.comboBoxDefaultRegion.Text;
            }
            else if (this.comboBoxDefaultRegion.Text.Length > 0 && node != null)
            {
                attr = this._xml.CreateAttribute("region");
                attr.Value = this.comboBoxDefaultRegion.Text;
                node.Attributes.Append(attr);
            }

            this._subtitle.Header = this._xml.OuterXml;

            Configuration.Settings.SubtitleSettings.TimedText10TimeCodeFormat = this.comboBoxTimeCodeFormat.SelectedItem.ToString();

            this.DialogResult = DialogResult.OK;
        }
    }
}