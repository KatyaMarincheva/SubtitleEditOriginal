// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Watermark.cs" company="">
//   
// </copyright>
// <summary>
//   The watermark.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The watermark.
    /// </summary>
    public sealed partial class Watermark : Form
    {
        /// <summary>
        /// The zero width space.
        /// </summary>
        private const char ZeroWidthSpace = '\u200B';

        /// <summary>
        /// The zero width no break space.
        /// </summary>
        private const char zeroWidthNoBreakSpace = '\uFEFF';

        /// <summary>
        /// The _first selected index.
        /// </summary>
        private int _firstSelectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Watermark"/> class.
        /// </summary>
        public Watermark()
        {
            this.InitializeComponent();
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="firstSelectedIndex">
        /// The first selected index.
        /// </param>
        internal void Initialize(Subtitle subtitle, int firstSelectedIndex)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(p.Text);
            }

            string watermark = ReadWaterMark(sb.ToString().Trim());
            this.LabelWatermark.Text = string.Format("Watermark: {0}", watermark);
            if (watermark.Length == 0)
            {
                this.buttonRemove.Enabled = false;
                this.textBoxWatermark.Focus();
            }
            else
            {
                this.groupBoxGenerate.Enabled = false;
                this.buttonOK.Focus();
            }

            this._firstSelectedIndex = firstSelectedIndex;
            Paragraph current = subtitle.GetParagraphOrDefault(this._firstSelectedIndex);
            if (current != null)
            {
                this.radioButtonCurrentLine.Text = this.radioButtonCurrentLine.Text + " " + current.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString);
            }
            else
            {
                this.radioButtonCurrentLine.Enabled = false;
            }
        }

        /// <summary>
        /// The read water mark.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ReadWaterMark(string input)
        {
            if (!input.Contains(ZeroWidthSpace))
            {
                return string.Empty;
            }

            int i = 0;
            StringBuilder sb = new StringBuilder();
            bool letterOn = false;
            int letter = 0;
            while (i < input.Length)
            {
                var c = input[i];
                if (c == ZeroWidthSpace)
                {
                    if (letter > 0)
                    {
                        sb.Append(Encoding.ASCII.GetString(new[] { (byte)letter }));
                    }

                    letterOn = true;
                    letter = 0;
                }
                else if (c == zeroWidthNoBreakSpace && letterOn)
                {
                    letter++;
                }
                else
                {
                    if (letter > 0)
                    {
                        sb.Append(Encoding.ASCII.GetString(new[] { (byte)letter }));
                    }

                    letterOn = false;
                    letter = 0;
                }

                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// The add water mark.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        private void AddWaterMark(Subtitle subtitle, string input)
        {
            if (subtitle == null || subtitle.Paragraphs.Count == 0)
            {
                return;
            }

            byte[] buffer = Encoding.ASCII.GetBytes(input);

            if (this.radioButtonCurrentLine.Checked)
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte b in buffer)
                {
                    sb.Append(ZeroWidthSpace);
                    for (int i = 0; i < b; i++)
                    {
                        sb.Append(zeroWidthNoBreakSpace);
                    }
                }

                Paragraph p = subtitle.GetParagraphOrDefault(this._firstSelectedIndex);
                if (p != null)
                {
                    if (p.Text.Length > 1)
                    {
                        p.Text = p.Text.Insert(p.Text.Length / 2, sb.ToString());
                    }
                    else
                    {
                        p.Text = sb + p.Text;
                    }
                }
            }
            else
            {
                Random r = new Random();
                List<int> indices = new List<int>();
                foreach (byte b in buffer)
                {
                    int number = r.Next(subtitle.Paragraphs.Count - 1);
                    if (indices.Contains(number))
                    {
                        number = r.Next(subtitle.Paragraphs.Count - 1);
                    }

                    if (indices.Contains(number))
                    {
                        number = r.Next(subtitle.Paragraphs.Count - 1);
                    }

                    indices.Add(number);
                }

                indices.Sort();
                int j = 0;
                foreach (byte b in buffer)
                {
                    StringBuilder sb = new StringBuilder();
                    Paragraph p = subtitle.Paragraphs[indices[j]];
                    sb.Append(ZeroWidthSpace);
                    for (int i = 0; i < b; i++)
                    {
                        sb.Append(zeroWidthNoBreakSpace);
                    }

                    if (p.Text.Length > 1)
                    {
                        p.Text = p.Text.Insert(p.Text.Length / 2, sb.ToString());
                    }
                    else
                    {
                        p.Text = sb + p.Text;
                    }

                    j++;
                }
            }
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
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The add or remove.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        internal void AddOrRemove(Subtitle subtitle)
        {
            if (this.groupBoxGenerate.Enabled)
            {
                this.AddWaterMark(subtitle, this.textBoxWatermark.Text);
            }
            else
            {
                RemoveWaterMark(subtitle);
            }
        }

        /// <summary>
        /// The remove water mark.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        private static void RemoveWaterMark(Subtitle subtitle)
        {
            var zws = ZeroWidthSpace.ToString(CultureInfo.InvariantCulture);
            var zwnbs = zeroWidthNoBreakSpace.ToString(CultureInfo.InvariantCulture);
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                p.Text = p.Text.Replace(zws, string.Empty).Replace(zwnbs, string.Empty);
            }
        }

        /// <summary>
        /// The button generate_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
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
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The watermark_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Watermark_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}