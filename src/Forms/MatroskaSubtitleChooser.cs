// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatroskaSubtitleChooser.cs" company="">
//   
// </copyright>
// <summary>
//   The matroska subtitle chooser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes;

    /// <summary>
    /// The matroska subtitle chooser.
    /// </summary>
    public sealed partial class MatroskaSubtitleChooser : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatroskaSubtitleChooser"/> class.
        /// </summary>
        public MatroskaSubtitleChooser()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.MatroskaSubtitleChooser.Title;
            this.labelChoose.Text = Configuration.Settings.Language.MatroskaSubtitleChooser.PleaseChoose;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the selected index.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.listBox1.SelectedIndex;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitleInfoList">
        /// The subtitle info list.
        /// </param>
        internal void Initialize(List<MatroskaTrackInfo> subtitleInfoList)
        {
            var format = Configuration.Settings.Language.MatroskaSubtitleChooser.TrackXLanguageYTypeZ;
            foreach (var info in subtitleInfoList)
            {
                var track = string.Format(!string.IsNullOrWhiteSpace(info.Name) ? "{0} - {1}" : "{0}", info.TrackNumber, info.Name);
                this.listBox1.Items.Add(string.Format(format, track, info.Language, info.CodecId));
            }

            this.listBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="mp4SubtitleTracks">
        /// The mp 4 subtitle tracks.
        /// </param>
        internal void Initialize(List<Trak> mp4SubtitleTracks)
        {
            int i = 0;
            foreach (var track in mp4SubtitleTracks)
            {
                i++;
                string handler = track.Mdia.HandlerName;
                if (handler != null && handler.Length > 1)
                {
                    handler = " - " + handler;
                }

                string s = string.Format("{0}: {1} - {2}{3}", i, track.Mdia.Mdhd.Iso639ThreeLetterCode, track.Mdia.Mdhd.LanguageString, handler);
                this.listBox1.Items.Add(s);
            }

            this.listBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// The form matroska subtitle chooser_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormMatroskaSubtitleChooser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The list box 1_ mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}