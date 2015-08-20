// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardSubExtract.cs" company="">
//   
// </copyright>
// <summary>
//   The hard sub extract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

 //using Nikse.SubtitleEdit.Logic.DirectShow.Custom;

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.VideoPlayers;

    /// <summary>
    /// The hard sub extract.
    /// </summary>
    public partial class HardSubExtract : Form
    {
        // private System.Windows.Forms.Timer timer1;
        /// <summary>
        /// The line checks width.
        /// </summary>
        private const int lineChecksWidth = 50;

        /// <summary>
        /// The line checks height.
        /// </summary>
        private const int lineChecksHeight = 25;

        /// <summary>
        /// The _lib vlc.
        /// </summary>
        private LibVlcDynamic _libVlc;

        // HardExtractCapture cam = null;
        /// <summary>
        /// The _video file name.
        /// </summary>
        private string _videoFileName;

        /// <summary>
        /// The _video info.
        /// </summary>
        private VideoInfo _videoInfo;

        /// <summary>
        /// The ocr file name.
        /// </summary>
        public string OcrFileName = null;

        // long startMilliseconds = 0;
        /// <summary>
        /// The subtitle from ocr.
        /// </summary>
        public Subtitle SubtitleFromOcr;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardSubExtract"/> class.
        /// </summary>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        public HardSubExtract(string videoFileName)
        {
            this.InitializeComponent();
            this._videoFileName = videoFileName;
            this.labelClickOnTextColor.Visible = false;
            this.SubtitleFromOcr = new Subtitle();
            this.labelStatus.Text = string.Empty;
            this.tbFileName.Text = videoFileName;
            this.pictureBoxCustomColor.BackColor = Color.FromArgb(233, 233, 233);
            this.SetCustumRGB();
        }

        /// <summary>
        /// The hard sub extract_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HardSubExtract_Shown(object sender, EventArgs e)
        {
            if (this.openFileDialogVideo.ShowDialog(this) == DialogResult.OK)
            {
                this._videoFileName = this.openFileDialogVideo.FileName;
                this.tbFileName.Text = this.openFileDialogVideo.FileName;
                this._videoInfo = Utilities.GetVideoInfo(this._videoFileName);
                var oldPlayer = Configuration.Settings.General.VideoPlayer;
                Configuration.Settings.General.VideoPlayer = "VLC";
                Utilities.InitializeVideoPlayerAndContainer(this._videoFileName, this._videoInfo, this.mediaPlayer, this.VideoLoaded, null);
                Configuration.Settings.General.VideoPlayer = oldPlayer;
                this._libVlc = this.mediaPlayer.VideoPlayer as LibVlcDynamic;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The video loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoLoaded(object sender, EventArgs e)
        {
            this.mediaPlayer.Stop();
            this.mediaPlayer.Volume = Configuration.Settings.General.VideoPlayerDefaultVolume;

            // pictureBox2.Image = GetSnapShot(startMilliseconds);
            this.mediaPlayer.Pause();
            this.timerRefreshProgressbar.Start();
            this.pictureBox2.Refresh();
        }

        // private Bitmap GetSnapShot(long milliseconds)
        // {
        // string fileName = Path.Combine(_folderName, Guid.NewGuid().ToString() + ".png");
        // _libVlc.CurrentPosition = milliseconds / TimeCode.BaseUnit;
        // _libVlc.TakeSnapshot(fileName, (uint)_videoInfo.Width, (uint)_videoInfo.Height);
        // int i=0;
        // while (i < 100 && !File.Exists(fileName))
        // {
        // System.Threading.Thread.Sleep(5);
        // Application.DoEvents();
        // i++;
        // }
        // System.Threading.Thread.Sleep(5);
        // Application.DoEvents();
        // Bitmap bmp = null;
        // try
        // {
        // using (var ms = new MemoryStream(File.ReadAllBytes(fileName)))
        // {
        // ms.Position = 0;
        // bmp = ((Bitmap)Bitmap.FromStream(ms)); // avoid locking file
        // }
        // File.Delete(fileName);
        // }
        // catch
        // {
        // }
        // return bmp;
        // }

        /// <summary>
        /// The picture box 2_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.pictureBox2.Image != null)
            {
                Bitmap bmp = this.pictureBox2.Image as Bitmap;
                if (bmp != null)
                {
                    this.pictureBoxCustomColor.BackColor = bmp.GetPixel(e.X, e.Y);
                    this.SetCustumRGB();
                }
            }
        }

        /// <summary>
        /// The picture box 2_ paint.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = this.pictureBox2.Image as Bitmap;
            if (bmp != null)
            {
                using (Pen p = new Pen(Brushes.Red))
                {
                    int value = Convert.ToInt32(this.numericUpDownPixelsBottom.Value);
                    if (value > bmp.Height)
                    {
                        value = bmp.Height - 2;
                    }

                    e.Graphics.DrawRectangle(p, 0, bmp.Height - value, bmp.Width - 1, bmp.Height - (bmp.Height - value) - 1);
                }
            }
        }

        /// <summary>
        /// The timer refresh progressbar tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimerRefreshProgressbarTick(object sender, EventArgs e)
        {
            this.mediaPlayer.RefreshProgressBar();
        }

        /// <summary>
        /// The numeric up down pixels bottom_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownPixelsBottom_ValueChanged(object sender, EventArgs e)
        {
            this.pictureBox2.Invalidate();
        }

        /// <summary>
        /// The start stop_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void StartStop_Click(object sender, EventArgs e)
        {
            // Cursor.Current = Cursors.WaitCursor;
            // buttonStop.Visible = true;
            // StartStop.Enabled = false;
            // cam = new HardExtractCapture(tbFileName.Text, (int)numericUpDownPixelsBottom.Value, checkBoxCustomColor.Checked, checkBoxYellow.Checked, pictureBoxCustomColor.BackColor, (int)numericUpDownCustomMaxDiff.Value);

            //// Start displaying statistics
            // this.timer1 = new System.Windows.Forms.Timer(this.components);
            // this.timer1.Interval = 1000;
            // this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // timer1.Enabled = true;
            // cam.Start();
            // cam.WaitUntilDone();
            // timer1.Enabled = false;

            //// Final update
            // tbFrameNum.Text = cam.count.ToString();
            // tbBlacks.Text = cam.blacks.ToString();

            // string fileNameNoExt = Path.GetTempFileName();

            // Cursor.Current = Cursors.Default;

            // var sub = new Subtitle();
            // for (int i = 0; i < cam.Images.Count; i++)
            // {
            // if (cam.StartTimes.Count > i)
            // {
            // Paragraph p = new Paragraph();
            // p.StartTime.TotalSeconds = cam.StartTimes[i];
            // if (cam.EndTimes.Count > i)
            // {
            // p.EndTime.TotalSeconds = cam.EndTimes[i];
            // }
            // else
            // {
            // p.EndTime.TotalSeconds = p.StartTime.TotalSeconds + 2.5;
            // }
            // p.Text = fileNameNoExt + string.Format("{0:0000}", i) + ".bmp";
            // sub.Paragraphs.Add(p);
            // var bmp = cam.Images[i].GetBitmap();
            // bmp.Save(p.Text);
            // bmp.Dispose();
            // }
            // }
            // sub.Renumber();
            // if (sub.Paragraphs.Count > 0)
            // {
            // OcrFileName = fileNameNoExt + ".srt";
            // File.WriteAllText(OcrFileName, sub.ToText(new SubRip()));
            // }
            // lock (this)
            // {
            // cam.Dispose();
            // cam = null;
            // }
            // buttonStop.Visible = false;
            // StartStop.Enabled = true;
        }

        // private void timer1_Tick(object sender, System.EventArgs e)
        // {
        // if (cam != null)
        // {
        // tbFrameNum.Text = cam.count.ToString();
        // tbBlacks.Text = cam.blacks.ToString();
        // if (cam.Images.Count > 0)
        // {
        // var old = pictureBox2.Image as Bitmap;
        // pictureBox2.Image = cam.Images[cam.Images.Count - 1].GetBitmap();
        // if (old != null)
        // old.Dispose();
        // }
        // }
        // }

        /// <summary>
        /// The picture box custom color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pictureBoxCustomColor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.pictureBoxCustomColor.BackColor;
            if (this.colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.pictureBoxCustomColor.BackColor = this.colorDialog1.Color;
                this.SetCustumRGB();
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
        /// The button stop_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            // if (cam != null)
            // cam.Cancel = true;
            // buttonStop.Visible = false;
            // StartStop.Enabled = true;
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void button1_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bmp");
            try
            {
                this._libVlc.TakeSnapshot(fileName, (uint)this._videoInfo.Width, (uint)this._videoInfo.Height);
                System.Threading.Thread.Sleep(100);
                this.pictureBox2.Image = Image.FromFile(fileName);
                System.Threading.Thread.Sleep(50);
            }
            catch (FileNotFoundException)
            {
                // the screenshot was not taken
            }
            catch
            {
                // TODO: Avoid catching all exceptions
            }
            finally
            {
                // whatever happens delete the screenshot if it exists
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// The set custum rgb.
        /// </summary>
        private void SetCustumRGB()
        {
            this.labelCustomRgb.Text = string.Format("(Red={0},Green={1},Blue={2})", this.pictureBoxCustomColor.BackColor.R, this.pictureBoxCustomColor.BackColor.G, this.pictureBoxCustomColor.BackColor.B);
        }

        /// <summary>
        /// The save image as tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Title = Configuration.Settings.Language.VobSubOcr.SaveSubtitleImageAs;
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.FileName = "Image";
            this.saveFileDialog1.Filter = "PNG image|*.png|BMP image|*.bmp|GIF image|*.gif|TIFF image|*.tiff";
            this.saveFileDialog1.FilterIndex = 0;

            DialogResult result = this.saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                using (var bmp = this.pictureBox2.Image as Bitmap)
                {
                    if (bmp == null)
                    {
                        MessageBox.Show("No image!");
                        return;
                    }

                    try
                    {
                        if (this.saveFileDialog1.FilterIndex == 0)
                        {
                            bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (this.saveFileDialog1.FilterIndex == 1)
                        {
                            bmp.Save(this.saveFileDialog1.FileName);
                        }
                        else if (this.saveFileDialog1.FilterIndex == 2)
                        {
                            bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                        }
                        else
                        {
                            bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
                if (this._libVlc != null)
                {
                    this._libVlc.Dispose();
                    this._libVlc = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}