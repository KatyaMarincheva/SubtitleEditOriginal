// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportStreamSubtitleChooser.cs" company="">
//   
// </copyright>
// <summary>
//   The transport stream subtitle chooser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.TransportStream;

    /// <summary>
    /// The transport stream subtitle chooser.
    /// </summary>
    public partial class TransportStreamSubtitleChooser : PositionAndSizeForm
    {
        /// <summary>
        /// The _ts parser.
        /// </summary>
        private TransportStreamParser _tsParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportStreamSubtitleChooser"/> class.
        /// </summary>
        public TransportStreamSubtitleChooser()
        {
            this.InitializeComponent();
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
                return this.listBoxTracks.SelectedIndex;
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
            this.DialogResult = DialogResult.OK;
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
        /// The transport stream subtitle chooser_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TransportStreamSubtitleChooser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="tsParser">
        /// The ts parser.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(TransportStreamParser tsParser, string fileName)
        {
            this._tsParser = tsParser;
            this.Text = string.Format(Configuration.Settings.Language.TransportStreamSubtitleChooser.Title, fileName);

            foreach (int id in tsParser.SubtitlePacketIds)
            {
                this.listBoxTracks.Items.Add(string.Format(Configuration.Settings.Language.TransportStreamSubtitleChooser.PidLine, id, tsParser.GetDvbSubtitles(id).Count));
            }

            this.listBoxTracks.SelectedIndex = 0;
        }

        /// <summary>
        /// The list box tracks_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = this.listBoxTracks.SelectedIndex;
            if (idx < 0)
            {
                return;
            }

            this.listBoxSubtitles.Items.Clear();
            int pid = this._tsParser.SubtitlePacketIds[idx];
            var list = this._tsParser.GetDvbSubtitles(pid);
            int i = 0;
            foreach (var sub in list)
            {
                i++;
                var start = new TimeCode(sub.StartMilliseconds);
                var end = new TimeCode(sub.EndMilliseconds);
                this.listBoxSubtitles.Items.Add(string.Format(Configuration.Settings.Language.TransportStreamSubtitleChooser.SubLine, i, start, end, sub.NumberOfImages));
            }

            if (list.Count > 0)
            {
                this.listBoxSubtitles.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The list box subtitles_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBoxSubtitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = this.listBoxSubtitles.SelectedIndex;
            if (idx < 0)
            {
                return;
            }

            int pid = this._tsParser.SubtitlePacketIds[this.listBoxTracks.SelectedIndex];
            var list = this._tsParser.GetDvbSubtitles(pid);

            var dvbBmp = list[idx].GetActiveImage();
            var nDvbBmp = new NikseBitmap(dvbBmp);
            nDvbBmp.CropTopTransparent(2);
            nDvbBmp.CropTransparentSidesAndBottom(2, true);
            dvbBmp.Dispose();
            var oldImage = this.pictureBox1.Image;
            this.pictureBox1.Image = nDvbBmp.GetBitmap();
            if (oldImage != null)
            {
                oldImage.Dispose();
            }
        }
    }
}