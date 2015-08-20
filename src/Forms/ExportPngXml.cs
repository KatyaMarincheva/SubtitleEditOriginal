// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportPngXml.cs" company="">
//   
// </copyright>
// <summary>
//   The export png xml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;
    using Nikse.SubtitleEdit.Logic.VideoPlayers;
    using Nikse.SubtitleEdit.Logic.VobSub;

    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// The export png xml.
    /// </summary>
    public sealed partial class ExportPngXml : PositionAndSizeForm
    {
        /// <summary>
        /// The box multi line text.
        /// </summary>
        private const string BoxMultiLineText = "BoxMultiLine";

        /// <summary>
        /// The box single line text.
        /// </summary>
        private const string BoxSingleLineText = "BoxSingleLine";

        /// <summary>
        /// The _preview timer.
        /// </summary>
        private readonly Timer _previewTimer = new System.Windows.Forms.Timer();

        /// <summary>
        /// The _border color.
        /// </summary>
        private Color _borderColor = Color.Black;

        /// <summary>
        /// The _border width.
        /// </summary>
        private float _borderWidth = 2.0f;

        /// <summary>
        /// The _export type.
        /// </summary>
        private string _exportType = "BDNXML";

        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _format.
        /// </summary>
        private SubtitleFormat _format;

        /// <summary>
        /// The _is loading.
        /// </summary>
        private bool _isLoading = true;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _subtitle color.
        /// </summary>
        private Color _subtitleColor = Color.White;

        /// <summary>
        /// The _subtitle font bold.
        /// </summary>
        private bool _subtitleFontBold;

        /// <summary>
        /// The _subtitle font name.
        /// </summary>
        private string _subtitleFontName = "Verdana";

        /// <summary>
        /// The _subtitle font size.
        /// </summary>
        private float _subtitleFontSize = 25.0f;

        /// <summary>
        /// The _video file name.
        /// </summary>
        private string _videoFileName;

        /// <summary>
        /// The _vob sub ocr.
        /// </summary>
        private VobSubOcr _vobSubOcr;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPngXml"/> class.
        /// </summary>
        public ExportPngXml()
        {
            this.InitializeComponent();

            var toolTip = new ToolTip { ShowAlways = true };
            toolTip.SetToolTip(this.panelFullFrameBackground, Configuration.Settings.Language.ExportPngXml.ChooseBackgroundColor);

            this.comboBoxImageFormat.SelectedIndex = 4;
            this._subtitleColor = Configuration.Settings.Tools.ExportFontColor;
            this._borderColor = Configuration.Settings.Tools.ExportBorderColor;
            this._previewTimer.Tick += this.previewTimer_Tick;
            this._previewTimer.Interval = 100;
        }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        private double FrameRate
        {
            get
            {
                if (this.comboBoxFrameRate.SelectedItem == null)
                {
                    return 25;
                }

                string s = this.comboBoxFrameRate.SelectedItem.ToString();
                s = s.Replace(",", ".").Trim();
                double d;
                if (double.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d))
                {
                    return d;
                }

                return 25;
            }
        }

        /// <summary>
        /// Gets the image format.
        /// </summary>
        private ImageFormat ImageFormat
        {
            get
            {
                var imageFormat = ImageFormat.Png;
                if (this.comboBoxImageFormat.SelectedIndex == 0)
                {
                    imageFormat = ImageFormat.Bmp;
                }
                else if (this.comboBoxImageFormat.SelectedIndex == 1)
                {
                    imageFormat = ImageFormat.Exif;
                }
                else if (this.comboBoxImageFormat.SelectedIndex == 2)
                {
                    imageFormat = ImageFormat.Gif;
                }
                else if (this.comboBoxImageFormat.SelectedIndex == 3)
                {
                    imageFormat = ImageFormat.Jpeg;
                }
                else if (this.comboBoxImageFormat.SelectedIndex == 4)
                {
                    imageFormat = ImageFormat.Png;
                }
                else if (this.comboBoxImageFormat.SelectedIndex == 5)
                {
                    imageFormat = ImageFormat.Tiff;
                }

                if (string.Compare(this.comboBoxImageFormat.Text, "tga", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return ImageFormat.Icon;
                }

                return imageFormat;
            }
        }

        /// <summary>
        /// The preview timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void previewTimer_Tick(object sender, EventArgs e)
        {
            this._previewTimer.Stop();
            this.GeneratePreview();
        }

        /// <summary>
        /// The bdn xml time code.
        /// </summary>
        /// <param name="timecode">
        /// The timecode.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BdnXmlTimeCode(TimeCode timecode)
        {
            var fr = this.FrameRate;
            var tc = new TimeCode(timecode.TotalMilliseconds * (Math.Ceiling(fr) / fr));
            int frames = SubtitleFormat.MillisecondsToFramesMaxFrameRate(tc.Milliseconds);
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", tc.Hours, tc.Minutes, tc.Seconds, frames);
        }

        /// <summary>
        /// The get alignment from paragraph.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="ContentAlignment"/>.
        /// </returns>
        private static ContentAlignment GetAlignmentFromParagraph(MakeBitmapParameter p, SubtitleFormat format, Subtitle subtitle)
        {
            var alignment = ContentAlignment.BottomCenter;
            if (p.AlignLeft)
            {
                alignment = ContentAlignment.BottomLeft;
            }
            else if (p.AlignRight)
            {
                alignment = ContentAlignment.BottomRight;
            }

            if (format.HasStyleSupport && !string.IsNullOrEmpty(p.P.Extra))
            {
                if (format.GetType() == typeof(SubStationAlpha))
                {
                    var style = AdvancedSubStationAlpha.GetSsaStyle(p.P.Extra, subtitle.Header);
                    alignment = GetSsaAlignment("{\\a" + style.Alignment + "}", alignment);
                }
                else if (format.GetType() == typeof(AdvancedSubStationAlpha))
                {
                    var style = AdvancedSubStationAlpha.GetSsaStyle(p.P.Extra, subtitle.Header);
                    alignment = GetAssAlignment("{\\an" + style.Alignment + "}", alignment);
                }
            }

            string text = p.P.Text;
            if (format.GetType() == typeof(SubStationAlpha) && text.Length > 5)
            {
                text = p.P.Text.Substring(0, 6);
                alignment = GetSsaAlignment(text, alignment);
            }
            else if (text.Length > 6)
            {
                text = p.P.Text.Substring(0, 6);
                alignment = GetAssAlignment(text, alignment);
            }

            return alignment;
        }

        /// <summary>
        /// The get ssa alignment.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="defaultAlignment">
        /// The default alignment.
        /// </param>
        /// <returns>
        /// The <see cref="ContentAlignment"/>.
        /// </returns>
        private static ContentAlignment GetSsaAlignment(string text, ContentAlignment defaultAlignment)
        {
            // 1: Bottom left
            // 2: Bottom center
            // 3: Bottom right
            // 9: Middle left
            // 10: Middle center
            // 11: Middle right
            // 5: Top left
            // 6: Top center
            // 7: Top right
            switch (text)
            {
                case "{\\a1}":
                    return ContentAlignment.BottomLeft;
                case "{\\a2}":
                    return ContentAlignment.BottomCenter;
                case "{\\a3}":
                    return ContentAlignment.BottomRight;
                case "{\\a9}":
                    return ContentAlignment.MiddleLeft;
                case "{\\a10}":
                    return ContentAlignment.MiddleCenter;
                case "{\\a11}":
                    return ContentAlignment.MiddleRight;
                case "{\\a5}":
                    return ContentAlignment.TopLeft;
                case "{\\a6}":
                    return ContentAlignment.TopCenter;
                case "{\\a7}":
                    return ContentAlignment.TopRight;
            }

            return defaultAlignment;
        }

        /// <summary>
        /// The get ass alignment.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="defaultAlignment">
        /// The default alignment.
        /// </param>
        /// <returns>
        /// The <see cref="ContentAlignment"/>.
        /// </returns>
        private static ContentAlignment GetAssAlignment(string text, ContentAlignment defaultAlignment)
        {
            // 1: Bottom left
            // 2: Bottom center
            // 3: Bottom right
            // 4: Middle left
            // 5: Middle center
            // 6: Middle right
            // 7: Top left
            // 8: Top center
            // 9: Top right
            switch (text)
            {
                case "{\\an1}":
                    return ContentAlignment.BottomLeft;
                case "{\\an2}":
                    return ContentAlignment.BottomCenter;
                case "{\\an3}":
                    return ContentAlignment.BottomRight;
                case "{\\an4}":
                    return ContentAlignment.MiddleLeft;
                case "{\\an5}":
                    return ContentAlignment.MiddleCenter;
                case "{\\an6}":
                    return ContentAlignment.MiddleRight;
                case "{\\an7}":
                    return ContentAlignment.TopLeft;
                case "{\\an8}":
                    return ContentAlignment.TopCenter;
                case "{\\an9}":
                    return ContentAlignment.TopRight;
            }

            return defaultAlignment;
        }

        /// <summary>
        /// The do work.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public static void DoWork(object data)
        {
            var parameter = (MakeBitmapParameter)data;

            parameter.LineJoin = Configuration.Settings.Tools.ExportPenLineJoin;
            parameter.Bitmap = GenerateImageFromTextWithStyle(parameter);
            if (parameter.Type == "BLURAYSUP")
            {
                MakeBluRaySupImage(parameter);
            }
        }

        /// <summary>
        /// The make blu ray sup image.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        private static void MakeBluRaySupImage(MakeBitmapParameter param)
        {
            var brSub = new Logic.BluRaySup.BluRaySupPicture { StartTime = (long)param.P.StartTime.TotalMilliseconds, EndTime = (long)param.P.EndTime.TotalMilliseconds, Width = param.ScreenWidth, Height = param.ScreenHeight, IsForced = param.Forced };
            if (param.FullFrame)
            {
                var nbmp = new NikseBitmap(param.Bitmap);
                nbmp.ReplaceTransparentWith(param.FullFrameBackgroundcolor);
                using (var bmp = nbmp.GetBitmap())
                {
                    int top = param.ScreenHeight - (param.Bitmap.Height + param.BottomMargin);
                    int left = (param.ScreenWidth - param.Bitmap.Width) / 2;

                    var b = new NikseBitmap(param.ScreenWidth, param.ScreenHeight);
                    {
                        b.Fill(param.FullFrameBackgroundcolor);
                        using (var fullSize = b.GetBitmap())
                        {
                            if (param.Alignment == ContentAlignment.BottomLeft || param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.TopLeft)
                            {
                                left = param.LeftRightMargin;
                            }
                            else if (param.Alignment == ContentAlignment.BottomRight || param.Alignment == ContentAlignment.MiddleRight || param.Alignment == ContentAlignment.TopRight)
                            {
                                left = param.ScreenWidth - param.Bitmap.Width - param.LeftRightMargin;
                            }

                            if (param.Alignment == ContentAlignment.TopLeft || param.Alignment == ContentAlignment.TopCenter || param.Alignment == ContentAlignment.TopRight)
                            {
                                top = param.BottomMargin;
                            }

                            if (param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.MiddleCenter || param.Alignment == ContentAlignment.MiddleRight)
                            {
                                top = param.ScreenHeight - (param.Bitmap.Height / 2);
                            }

                            using (var g = Graphics.FromImage(fullSize))
                            {
                                g.DrawImage(bmp, left, top);
                                g.Dispose();
                            }

                            param.Buffer = Logic.BluRaySup.BluRaySupPicture.CreateSupFrame(brSub, fullSize, param.FramesPerSeconds, 0, 0, ContentAlignment.BottomCenter);
                        }
                    }
                }
            }
            else
            {
                param.Buffer = Logic.BluRaySup.BluRaySupPicture.CreateSupFrame(brSub, param.Bitmap, param.FramesPerSeconds, param.BottomMargin, param.LeftRightMargin, param.Alignment);
            }
        }

        /// <summary>
        /// The make make bitmap parameter.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="screenWidth">
        /// The screen width.
        /// </param>
        /// <param name="screenHeight">
        /// The screen height.
        /// </param>
        /// <returns>
        /// The <see cref="MakeBitmapParameter"/>.
        /// </returns>
        private MakeBitmapParameter MakeMakeBitmapParameter(int index, int screenWidth, int screenHeight)
        {
            var parameter = new MakeBitmapParameter { Type = this._exportType, SubtitleColor = this._subtitleColor, SubtitleFontName = this._subtitleFontName, SubtitleFontSize = this._subtitleFontSize, SubtitleFontBold = this._subtitleFontBold, BorderColor = this._borderColor, BorderWidth = this._borderWidth, SimpleRendering = this.checkBoxSimpleRender.Checked, AlignLeft = this.comboBoxHAlign.SelectedIndex == 0, AlignRight = this.comboBoxHAlign.SelectedIndex == 2, ScreenWidth = screenWidth, ScreenHeight = screenHeight, VideoResolution = this.comboBoxResolution.Text, Bitmap = null, FramesPerSeconds = this.FrameRate, BottomMargin = this.comboBoxBottomMargin.SelectedIndex, LeftRightMargin = this.comboBoxLeftRightMargin.SelectedIndex, Saved = false, Alignment = ContentAlignment.BottomCenter, Type3D = this.comboBox3D.SelectedIndex, Depth3D = (int)this.numericUpDownDepth3D.Value, BackgroundColor = Color.Transparent, SavDialogFileName = this.saveFileDialog1.FileName, ShadowColor = this.panelShadowColor.BackColor, ShadowWidth = this.comboBoxShadowWidth.SelectedIndex, ShadowAlpha = (int)this.numericUpDownShadowTransparency.Value, LineHeight = (int)this.numericUpDownLineSpacing.Value, FullFrame = this.checkBoxFullFrameImage.Checked, FullFrameBackgroundcolor = this.panelFullFrameBackground.BackColor, };
            if (index < this._subtitle.Paragraphs.Count)
            {
                parameter.P = this._subtitle.Paragraphs[index];
                parameter.Alignment = GetAlignmentFromParagraph(parameter, this._format, this._subtitle);
                parameter.Forced = this.subtitleListView1.Items[index].Checked;

                if (this._format.HasStyleSupport && !string.IsNullOrEmpty(parameter.P.Extra))
                {
                    if (this._format.GetType() == typeof(SubStationAlpha))
                    {
                        var style = AdvancedSubStationAlpha.GetSsaStyle(parameter.P.Extra, this._subtitle.Header);
                        parameter.SubtitleColor = style.Primary;
                        parameter.SubtitleFontBold = style.Bold;
                        parameter.SubtitleFontSize = style.FontSize;
                        parameter.SubtitleFontName = style.FontName;
                        if (style.BorderStyle == "3")
                        {
                            parameter.BackgroundColor = style.Background;
                        }
                    }
                    else if (this._format.GetType() == typeof(AdvancedSubStationAlpha))
                    {
                        var style = AdvancedSubStationAlpha.GetSsaStyle(parameter.P.Extra, this._subtitle.Header);
                        parameter.SubtitleColor = style.Primary;
                        parameter.SubtitleFontBold = style.Bold;
                        parameter.SubtitleFontSize = style.FontSize;
                        parameter.SubtitleFontName = style.FontName;
                        if (style.BorderStyle == "3")
                        {
                            parameter.BackgroundColor = style.Outline;
                        }
                    }
                }

                if (this.comboBoxBorderWidth.SelectedItem.ToString() == Configuration.Settings.Language.ExportPngXml.BorderStyleBoxForEachLine)
                {
                    parameter.BoxSingleLine = true;
                    parameter.BackgroundColor = this.panelBorderColor.BackColor;
                    parameter.BorderWidth = 0;
                }
                else if (this.comboBoxBorderWidth.SelectedItem.ToString() == Configuration.Settings.Language.ExportPngXml.BorderStyleOneBox)
                {
                    parameter.BoxSingleLine = true;
                    parameter.BackgroundColor = this.panelBorderColor.BackColor;
                    parameter.BorderWidth = 0;
                }
                else
                {
                    this._borderWidth = float.Parse(Utilities.RemoveNonNumbers(this.comboBoxBorderWidth.SelectedItem.ToString()));
                }
            }
            else
            {
                parameter.P = null;
            }

            return parameter;
        }

        /// <summary>
        /// The button export click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonExportClick(object sender, EventArgs e)
        {
            this.FixStartEndWithSameTimeCode();

            var errors = new List<string>();
            this.buttonExport.Enabled = false;
            this.SetupImageParameters();

            if (!string.IsNullOrEmpty(this._fileName))
            {
                this.saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(this._fileName);
            }

            if (this._exportType == "BLURAYSUP")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveBluRraySupAs;
                this.saveFileDialog1.DefaultExt = "*.sup";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "Blu-Ray sup|*.sup";
            }
            else if (this._exportType == "VOBSUB")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveVobSubAs;
                this.saveFileDialog1.DefaultExt = "*.sub";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "VobSub|*.sub";
            }
            else if (this._exportType == "FAB")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveFabImageScriptAs;
                this.saveFileDialog1.DefaultExt = "*.txt";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "FAB image scripts|*.txt";
            }
            else if (this._exportType == "STL")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveDvdStudioProStlAs;
                this.saveFileDialog1.DefaultExt = "*.txt";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "DVD Studio Pro STL|*.stl";
            }
            else if (this._exportType == "FCP")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveFcpAs;
                this.saveFileDialog1.DefaultExt = "*.xml";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "Xml files|*.xml";
            }
            else if (this._exportType == "DOST")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveDostAs;
                this.saveFileDialog1.DefaultExt = "*.dost";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "Dost files|*.dost";
            }
            else if (this._exportType == "DCINEMA_INTEROP")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SaveDigitalCinemaInteropAs;
                this.saveFileDialog1.DefaultExt = "*.xml";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "Xml files|*.xml";
            }
            else if (this._exportType == "EDL")
            {
                this.saveFileDialog1.Title = Configuration.Settings.Language.ExportPngXml.SavePremiereEdlAs;
                this.saveFileDialog1.DefaultExt = "*.edl";
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.Filter = "EDL files|*.edl";
            }

            if (this._exportType == "BLURAYSUP" && this.saveFileDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "VOBSUB" && this.saveFileDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "BDNXML" && this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "FAB" && this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "IMAGE/FRAME" && this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "STL" && this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "SPUMUX" && this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "FCP" && this.saveFileDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "DOST" && this.saveFileDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "DCINEMA_INTEROP" && this.saveFileDialog1.ShowDialog(this) == DialogResult.OK || this._exportType == "EDL" && this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                int width;
                int height;
                this.GetResolution(out width, out height);

                FileStream binarySubtitleFile = null;
                VobSubWriter vobSubWriter = null;
                if (this._exportType == "BLURAYSUP")
                {
                    binarySubtitleFile = new FileStream(this.saveFileDialog1.FileName, FileMode.Create);
                }
                else if (this._exportType == "VOBSUB")
                {
                    vobSubWriter = new VobSubWriter(this.saveFileDialog1.FileName, width, height, this.comboBoxBottomMargin.SelectedIndex, this.comboBoxLeftRightMargin.SelectedIndex, 32, this._subtitleColor, this._borderColor, !this.checkBoxTransAntiAliase.Checked, IfoParser.ArrayOfLanguage[this.comboBoxLanguage.SelectedIndex], IfoParser.ArrayOfLanguageCode[this.comboBoxLanguage.SelectedIndex]);
                }

                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = this._subtitle.Paragraphs.Count - 1;
                this.progressBar1.Visible = true;

                int border = this.comboBoxBottomMargin.SelectedIndex;
                int imagesSavedCount = 0;
                var sb = new StringBuilder();
                if (this._exportType == "STL")
                {
                    sb.AppendLine("$SetFilePathToken =" + this.folderBrowserDialog1.SelectedPath);
                    sb.AppendLine();
                }

                if (this._vobSubOcr != null)
                {
                    int i = 0;
                    foreach (Paragraph p in this._subtitle.Paragraphs)
                    {
                        var mp = this.MakeMakeBitmapParameter(i, width, height);
                        mp.Bitmap = this._vobSubOcr.GetSubtitleBitmap(i);

                        if (this._exportType == "BLURAYSUP")
                        {
                            MakeBluRaySupImage(mp);
                        }

                        imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, mp, i);
                        i++;
                        this.progressBar1.Refresh();
                        Application.DoEvents();
                        if (i < this.progressBar1.Maximum)
                        {
                            this.progressBar1.Value = i;
                        }
                    }
                }
                else
                {
                    var threadEqual = new Thread(DoWork);
                    var paramEqual = this.MakeMakeBitmapParameter(0, width, height);

                    var threadUnEqual = new Thread(DoWork);
                    var paramUnEqual = this.MakeMakeBitmapParameter(1, width, height);

                    threadEqual.Start(paramEqual);
                    int i = 1;
                    for (; i < this._subtitle.Paragraphs.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            paramEqual = this.MakeMakeBitmapParameter(i, width, height);
                            threadEqual = new Thread(DoWork);
                            threadEqual.Start(paramEqual);

                            if (threadUnEqual.ThreadState == ThreadState.Running)
                            {
                                threadUnEqual.Join();
                            }

                            imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, paramUnEqual, i);
                            if (!string.IsNullOrEmpty(paramUnEqual.Error))
                            {
                                errors.Add(paramUnEqual.Error);
                            }
                        }
                        else
                        {
                            paramUnEqual = this.MakeMakeBitmapParameter(i, width, height);
                            threadUnEqual = new Thread(DoWork);
                            threadUnEqual.Start(paramUnEqual);

                            if (threadEqual.ThreadState == ThreadState.Running)
                            {
                                threadEqual.Join();
                            }

                            imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, paramEqual, i);
                            if (!string.IsNullOrEmpty(paramEqual.Error))
                            {
                                errors.Add(paramEqual.Error);
                            }
                        }

                        this.progressBar1.Refresh();
                        Application.DoEvents();
                        this.progressBar1.Value = i;
                    }

                    if (i % 2 == 0)
                    {
                        if (threadEqual.ThreadState == ThreadState.Running)
                        {
                            threadEqual.Join();
                        }

                        imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, paramEqual, i);
                        if (threadUnEqual.ThreadState == ThreadState.Running)
                        {
                            threadUnEqual.Join();
                        }

                        imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, paramUnEqual, i);
                    }
                    else
                    {
                        if (threadUnEqual.ThreadState == ThreadState.Running)
                        {
                            threadUnEqual.Join();
                        }

                        imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, paramUnEqual, i);
                        if (threadEqual.ThreadState == ThreadState.Running)
                        {
                            threadEqual.Join();
                        }

                        imagesSavedCount = this.WriteParagraph(width, sb, border, height, imagesSavedCount, vobSubWriter, binarySubtitleFile, paramEqual, i);
                    }
                }

                if (errors.Count > 0)
                {
                    var errorSb = new StringBuilder();
                    for (int i = 0; i < 20; i++)
                    {
                        if (i < errors.Count)
                        {
                            errorSb.AppendLine(errors[i]);
                        }
                    }

                    if (errors.Count > 20)
                    {
                        errorSb.AppendLine("...");
                    }

                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.SomeLinesWereTooLongX, errorSb));
                }

                this.progressBar1.Visible = false;
                if (this._exportType == "BLURAYSUP")
                {
                    binarySubtitleFile.Close();
                    MessageBox.Show(string.Format(Configuration.Settings.Language.Main.SavedSubtitleX, this.saveFileDialog1.FileName));
                }
                else if (this._exportType == "VOBSUB")
                {
                    vobSubWriter.WriteIdxFile();
                    vobSubWriter.Dispose();
                    MessageBox.Show(string.Format(Configuration.Settings.Language.Main.SavedSubtitleX, this.saveFileDialog1.FileName));
                }
                else if (this._exportType == "FAB")
                {
                    File.WriteAllText(Path.Combine(this.folderBrowserDialog1.SelectedPath, "Fab_Image_script.txt"), sb.ToString());
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, this.folderBrowserDialog1.SelectedPath));
                }
                else if (this._exportType == "IMAGE/FRAME")
                {
                    var empty = new Bitmap(width, height);
                    imagesSavedCount++;
                    string numberString = string.Format("{0:00000}", imagesSavedCount);
                    string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + "." + this.comboBoxImageFormat.Text.ToLower());
                    this.SaveImage(empty, fileName, this.ImageFormat);

                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, this.folderBrowserDialog1.SelectedPath));
                }
                else if (this._exportType == "STL")
                {
                    File.WriteAllText(Path.Combine(this.folderBrowserDialog1.SelectedPath, "DVD_Studio_Pro_Image_script.stl"), sb.ToString());
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, this.folderBrowserDialog1.SelectedPath));
                }
                else if (this._exportType == "SPUMUX")
                {
                    string s = "<subpictures>" + Environment.NewLine + "\t<stream>" + Environment.NewLine + sb + "\t</stream>" + Environment.NewLine + "</subpictures>";
                    File.WriteAllText(Path.Combine(this.folderBrowserDialog1.SelectedPath, "spu.xml"), s);
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, this.folderBrowserDialog1.SelectedPath));
                }
                else if (this._exportType == "FCP")
                {
                    string fileNameNoPath = Path.GetFileName(this.saveFileDialog1.FileName);
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(fileNameNoPath);

                    int duration = 0;
                    if (this._subtitle.Paragraphs.Count > 0)
                    {
                        duration = (int)Math.Round(this._subtitle.Paragraphs[this._subtitle.Paragraphs.Count - 1].EndTime.TotalSeconds * 25.0);
                    }

                    string s = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<!DOCTYPE xmeml[]>" + Environment.NewLine + "<xmeml version=\"4\">" + Environment.NewLine + "  <sequence id=\"" + fileNameNoExt + "\">" + Environment.NewLine + "    <updatebehavior>add</updatebehavior>" + Environment.NewLine + "    <name>" + fileNameNoExt + @"</name>
    <duration>" + duration.ToString(CultureInfo.InvariantCulture) + @"</duration>
    <rate>
      <ntsc>FALSE</ntsc>
      <timebase>25</timebase>
    </rate>
    <timecode>
      <rate>
        <ntsc>FALSE</ntsc>
        <timebase>25</timebase>
      </rate>
      <string>00:00:00:00</string>
      <frame>0</frame>
      <source>source</source>
      <displayformat>NDF</displayformat>
    </timecode>
    <in>0</in>
    <out>[OUT]</out>
    <media>
      <video>
        <format>
          <samplecharacteristics>
            <rate>
              <timebase>25</timebase>
            </rate>
            <width>1920</width>
            <height>1080</height>
            <anamorphic>FALSE</anamorphic>
            <pixelaspectratio>square</pixelaspectratio>
            <fielddominance>none</fielddominance>
            <colordepth>32</colordepth>
          </samplecharacteristics>
        </format>
        <track>
          <enabled>TRUE</enabled>
          <locked>FALSE</locked>
        </track>
        <track>
" + sb + @"   <enabled>TRUE</enabled>
          <locked>FALSE</locked>
        </track>
      </video>
      <audio>
        <track>
          <enabled>TRUE</enabled>
          <locked>FALSE</locked>
          <outputchannelindex>1</outputchannelindex>
        </track>
        <track>
          <enabled>TRUE</enabled>
          <locked>FALSE</locked>
          <outputchannelindex>2</outputchannelindex>
        </track>
        <track>
          <enabled>TRUE</enabled>
          <locked>FALSE</locked>
          <outputchannelindex>3</outputchannelindex>
        </track>
        <track>
          <enabled>TRUE</enabled>
          <locked>FALSE</locked>
          <outputchannelindex>4</outputchannelindex>
        </track>
      </audio>
    </media>
    <ismasterclip>FALSE</ismasterclip>
  </sequence>
</xmeml>";
                    s = s.Replace("<timebase>25</timebase>", "<timebase>" + this.comboBoxFrameRate.Text + "</timebase>");

                    if (this._subtitle.Paragraphs.Count > 0)
                    {
                        var end = (int)Math.Round(this._subtitle.Paragraphs[this._subtitle.Paragraphs.Count - 1].EndTime.TotalSeconds * this.FrameRate);
                        end ++;
                        s = s.Replace("[OUT]", end.ToString(CultureInfo.InvariantCulture));
                    }

                    if (this.comboBoxLanguage.Text == "NTSC")
                    {
                        s = s.Replace("<ntsc>FALSE</ntsc>", "<ntsc>TRUE</ntsc>");
                    }

                    s = s.Replace("<width>1920</width>", "<width>" + width.ToString(CultureInfo.InvariantCulture) + "</width>");
                    s = s.Replace("<height>1080</height>", "<height>" + height.ToString(CultureInfo.InvariantCulture) + "</height>");

                    if (this.comboBoxImageFormat.Text.Contains("8-bit"))
                    {
                        s = s.Replace("<colordepth>32</colordepth>", "<colordepth>8</colordepth>");
                    }

                    File.WriteAllText(Path.Combine(this.folderBrowserDialog1.SelectedPath, this.saveFileDialog1.FileName), s);
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, Path.GetDirectoryName(this.saveFileDialog1.FileName)));
                }
                else if (this._exportType == "DOST")
                {
                    string header = @"$FORMAT=480
$VERSION=1.2
$ULEAD=TRUE
$DROP=[DROPVALUE]" + Environment.NewLine + Environment.NewLine + "NO\tINTIME\t\tOUTTIME\t\tXPOS\tYPOS\tFILENAME\tFADEIN\tFADEOUT";

                    string dropValue = "30000";
                    if (this.comboBoxFrameRate.Items[this.comboBoxFrameRate.SelectedIndex].ToString() == "23.98")
                    {
                        dropValue = "23976";
                    }
                    else if (this.comboBoxFrameRate.Items[this.comboBoxFrameRate.SelectedIndex].ToString() == "24")
                    {
                        dropValue = "24000";
                    }
                    else if (this.comboBoxFrameRate.Items[this.comboBoxFrameRate.SelectedIndex].ToString() == "25")
                    {
                        dropValue = "25000";
                    }
                    else if (this.comboBoxFrameRate.Items[this.comboBoxFrameRate.SelectedIndex].ToString() == "29.97")
                    {
                        dropValue = "29970";
                    }
                    else if (this.comboBoxFrameRate.Items[this.comboBoxFrameRate.SelectedIndex].ToString() == "30")
                    {
                        dropValue = "30000";
                    }
                    else if (this.comboBoxFrameRate.Items[this.comboBoxFrameRate.SelectedIndex].ToString() == "59.94")
                    {
                        dropValue = "59940";
                    }

                    header = header.Replace("[DROPVALUE]", dropValue);

                    File.WriteAllText(this.saveFileDialog1.FileName, header + Environment.NewLine + sb);
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, Path.GetDirectoryName(this.saveFileDialog1.FileName)));
                }
                else if (this._exportType == "DCINEMA_INTEROP")
                {
                    var doc = new XmlDocument();
                    string title = "unknown";
                    if (!string.IsNullOrEmpty(this._fileName))
                    {
                        title = Path.GetFileNameWithoutExtension(this._fileName);
                    }

                    string guid = Guid.NewGuid().ToString().Replace("-", string.Empty).Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
                    doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + "<DCSubtitle Version=\"1.1\">" + Environment.NewLine + "<SubtitleID>" + guid + "</SubtitleID>" + Environment.NewLine + "<MovieTitle>" + title + "</MovieTitle>" + Environment.NewLine + "<ReelNumber>1</ReelNumber>" + Environment.NewLine + "<Language>English</Language>" + Environment.NewLine + sb + "</DCSubtitle>");
                    string fName = this.saveFileDialog1.FileName;
                    if (!fName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        fName += ".xml";
                    }

                    File.WriteAllText(fName, SubtitleFormat.ToUtf8XmlString(doc));
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, Path.GetDirectoryName(fName)));
                }
                else if (this._exportType == "EDL")
                {
                    string header = "TITLE: ( no title )" + Environment.NewLine + Environment.NewLine;
                    File.WriteAllText(this.saveFileDialog1.FileName, header + sb);
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, Path.GetDirectoryName(this.saveFileDialog1.FileName)));
                }
                else if (this._exportType == "DCINEMA_INTEROP")
                {
                    var doc = new XmlDocument();
                    string title = "unknown";
                    if (!string.IsNullOrEmpty(this._fileName))
                    {
                        title = Path.GetFileNameWithoutExtension(this._fileName);
                    }

                    string guid = Guid.NewGuid().ToString().Replace("-", string.Empty).Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
                    doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + "<DCSubtitle Version=\"1.1\">" + Environment.NewLine + "<SubtitleID>" + guid + "</SubtitleID>" + Environment.NewLine + "<MovieTitle>" + title + "</MovieTitle>" + Environment.NewLine + "<ReelNumber>1</ReelNumber>" + Environment.NewLine + "<Language>English</Language>" + Environment.NewLine + sb + "</DCSubtitle>");
                    string fName = this.saveFileDialog1.FileName;
                    if (!fName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        fName += ".xml";
                    }

                    File.WriteAllText(fName, SubtitleFormat.ToUtf8XmlString(doc));
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, Path.GetDirectoryName(fName)));
                }
                else
                {
                    int resW;
                    int resH;
                    this.GetResolution(out resW, out resH);
                    string videoFormat = "1080p";
                    if (resW == 1920 && resH == 1080)
                    {
                        videoFormat = "1080p";
                    }
                    else if (resW == 1280 && resH == 720)
                    {
                        videoFormat = "720p";
                    }
                    else if (resW == 848 && resH == 480)
                    {
                        videoFormat = "480p";
                    }
                    else if (resW > 0 && resH > 0)
                    {
                        videoFormat = resW + "x" + resH;
                    }

                    var doc = new XmlDocument();
                    Paragraph first = this._subtitle.Paragraphs[0];
                    Paragraph last = this._subtitle.Paragraphs[this._subtitle.Paragraphs.Count - 1];
                    doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + "<BDN Version=\"0.93\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"BD-03-006-0093b BDN File Format.xsd\">" + Environment.NewLine + "<Description>" + Environment.NewLine + "<Name Title=\"subtitle_exp\" Content=\"\"/>" + Environment.NewLine + "<Language Code=\"eng\"/>" + Environment.NewLine + "<Format VideoFormat=\"" + videoFormat + "\" FrameRate=\"" + this.FrameRate.ToString(CultureInfo.InvariantCulture) + "\" DropFrame=\"False\"/>" + Environment.NewLine + "<Events Type=\"Graphic\" FirstEventInTC=\"" + this.BdnXmlTimeCode(first.StartTime) + "\" LastEventOutTC=\"" + this.BdnXmlTimeCode(last.EndTime) + "\" NumberofEvents=\"" + imagesSavedCount.ToString(CultureInfo.InvariantCulture) + "\"/>" + Environment.NewLine + "</Description>" + Environment.NewLine + "<Events>" + Environment.NewLine + "</Events>" + Environment.NewLine + "</BDN>");
                    XmlNode events = doc.DocumentElement.SelectSingleNode("Events");
                    doc.PreserveWhitespace = true;
                    events.InnerXml = sb.ToString();
                    File.WriteAllText(Path.Combine(this.folderBrowserDialog1.SelectedPath, "BDN_Index.xml"), FormatUtf8Xml(doc), Encoding.UTF8);
                    MessageBox.Show(string.Format(Configuration.Settings.Language.ExportPngXml.XImagesSavedInY, imagesSavedCount, this.folderBrowserDialog1.SelectedPath));
                }
            }

            this.buttonExport.Enabled = true;
        }

        /// <summary>
        /// The format utf 8 xml.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatUtf8Xml(XmlDocument doc)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                doc.Save(writer);
            }

            return sb.ToString().Replace(" encoding=\"utf-16\"", " encoding=\"utf-8\"").Trim(); // "replace hack" due to missing encoding (encoding only works if it's the only parameter...)
        }

        /// <summary>
        /// The save image.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="imageFormat">
        /// The image format.
        /// </param>
        private void SaveImage(Bitmap bmp, string fileName, ImageFormat imageFormat)
        {
            if (Equals(imageFormat, ImageFormat.Icon))
            {
                var nikseBitmap = new NikseBitmap(bmp);
                nikseBitmap.SaveAsTarga(fileName);
            }
            else
            {
                bmp.Save(fileName, imageFormat);
            }
        }

        /// <summary>
        /// The fix start end with same time code.
        /// </summary>
        private void FixStartEndWithSameTimeCode()
        {
            for (int i = 0; i < this._subtitle.Paragraphs.Count - 1; i++)
            {
                Paragraph p = this._subtitle.Paragraphs[i];
                Paragraph next = this._subtitle.Paragraphs[i + 1];
                if (Math.Abs(p.EndTime.TotalMilliseconds - next.StartTime.TotalMilliseconds) < 0.1)
                {
                    p.EndTime.TotalMilliseconds--;
                }
            }
        }

        /// <summary>
        /// The set resolution.
        /// </summary>
        /// <param name="xAndY">
        /// The x and y.
        /// </param>
        private void SetResolution(string xAndY)
        {
            if (string.IsNullOrEmpty(xAndY))
            {
                return;
            }

            xAndY = xAndY.ToLower();
            if (Regex.IsMatch(xAndY, @"\d+x\d+", RegexOptions.IgnoreCase))
            {
                for (int i = 0; i < this.comboBoxResolution.Items.Count; i++)
                {
                    if (this.comboBoxResolution.Items[i].ToString().Contains(xAndY))
                    {
                        this.comboBoxResolution.SelectedIndex = i;
                        return;
                    }
                }

                this.comboBoxResolution.Items[this.comboBoxResolution.Items.Count - 1] = xAndY;
                this.comboBoxResolution.SelectedIndex = this.comboBoxResolution.Items.Count - 1;
            }
        }

        /// <summary>
        /// The get resolution.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        private void GetResolution(out int width, out int height)
        {
            width = 1920;
            height = 1080;
            if (this.comboBoxResolution.SelectedIndex < 0)
            {
                return;
            }

            string text = this.comboBoxResolution.Text.Trim();

            if (this._exportType == "FCP")
            {
                if (text == "NTSC-601")
                {
                    width = 720;
                    height = 480;
                    return;
                }

                if (text == "PAL-601")
                {
                    width = 720;
                    height = 576;
                    return;
                }

                if (text == "square")
                {
                    width = 640;
                    height = 480;
                    return;
                }

                if (text == "DVCPROHD-720P")
                {
                    width = 1280;
                    height = 720;
                    return;
                }

                if (text == "HD-(960x720)")
                {
                    width = 960;
                    height = 720;
                    return;
                }

                if (text == "DVCPROHD-1080i60")
                {
                    width = 1920;
                    height = 1080;
                    return;
                }

                if (text == "HD-(1280x1080)")
                {
                    width = 1280;
                    height = 1080;
                    return;
                }

                if (text == "DVCPROHD-1080i50")
                {
                    width = 1920;
                    height = 1080;
                    return;
                }

                if (text == "HD-(1440x1080)")
                {
                    width = 1440;
                    height = 1080;
                    return;
                }
            }

            if (text.Contains('('))
            {
                text = text.Remove(0, text.IndexOf('(')).Trim();
            }

            text = text.TrimStart('(').TrimEnd(')').Trim();
            string[] arr = text.Split('x');
            width = int.Parse(arr[0]);
            height = int.Parse(arr[1]);
        }

        /// <summary>
        /// The write paragraph.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="border">
        /// The border.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="imagesSavedCount">
        /// The images saved count.
        /// </param>
        /// <param name="vobSubWriter">
        /// The vob sub writer.
        /// </param>
        /// <param name="binarySubtitleFile">
        /// The binary subtitle file.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int WriteParagraph(int width, StringBuilder sb, int border, int height, int imagesSavedCount, VobSubWriter vobSubWriter, FileStream binarySubtitleFile, MakeBitmapParameter param, int i)
        {
            if (param.Bitmap != null)
            {
                if (this._exportType == "BLURAYSUP")
                {
                    if (!param.Saved)
                    {
                        binarySubtitleFile.Write(param.Buffer, 0, param.Buffer.Length);
                    }

                    param.Saved = true;
                }
                else if (this._exportType == "VOBSUB")
                {
                    if (!param.Saved)
                    {
                        vobSubWriter.WriteParagraph(param.P, param.Bitmap, param.Alignment);
                    }

                    param.Saved = true;
                }
                else if (this._exportType == "FAB")
                {
                    if (!param.Saved)
                    {
                        string numberString = string.Format("IMAGE{0:000}", i);
                        string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + "." + this.comboBoxImageFormat.Text.ToLower());

                        if (this.checkBoxFullFrameImage.Visible && this.checkBoxFullFrameImage.Checked)
                        {
                            var nbmp = new NikseBitmap(param.Bitmap);
                            nbmp.ReplaceTransparentWith(this.panelFullFrameBackground.BackColor);
                            using (var bmp = nbmp.GetBitmap())
                            {
                                // param.Bitmap.Save(fileName, ImageFormat);
                                imagesSavedCount++;

                                // RACE001.TIF 00;00;02;02 00;00;03;15 000 000 720 480
                                // RACE002.TIF 00;00;05;18 00;00;09;20 000 000 720 480
                                int top = param.ScreenHeight - (param.Bitmap.Height + param.BottomMargin);
                                int left = (param.ScreenWidth - param.Bitmap.Width) / 2;

                                var b = new NikseBitmap(param.ScreenWidth, param.ScreenHeight);
                                {
                                    b.Fill(this.panelFullFrameBackground.BackColor);
                                    using (var fullSize = b.GetBitmap())
                                    {
                                        if (param.Alignment == ContentAlignment.BottomLeft || param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.TopLeft)
                                        {
                                            left = param.LeftRightMargin;
                                        }
                                        else if (param.Alignment == ContentAlignment.BottomRight || param.Alignment == ContentAlignment.MiddleRight || param.Alignment == ContentAlignment.TopRight)
                                        {
                                            left = param.ScreenWidth - param.Bitmap.Width - param.LeftRightMargin;
                                        }

                                        if (param.Alignment == ContentAlignment.TopLeft || param.Alignment == ContentAlignment.TopCenter || param.Alignment == ContentAlignment.TopRight)
                                        {
                                            top = param.BottomMargin;
                                        }

                                        if (param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.MiddleCenter || param.Alignment == ContentAlignment.MiddleRight)
                                        {
                                            top = param.ScreenHeight - (param.Bitmap.Height / 2);
                                        }

                                        using (var g = Graphics.FromImage(fullSize))
                                        {
                                            g.DrawImage(bmp, left, top);
                                            g.Dispose();
                                        }

                                        this.SaveImage(fullSize, fileName, this.ImageFormat);
                                    }
                                }

                                left = 0;
                                top = 0;
                                sb.AppendLine(string.Format("{0} {1} {2} {3} {4} {5} {6}", Path.GetFileName(fileName), FormatFabTime(param.P.StartTime, param), FormatFabTime(param.P.EndTime, param), left, top, left + param.ScreenWidth, top + param.ScreenHeight));
                            }
                        }
                        else
                        {
                            this.SaveImage(param.Bitmap, fileName, this.ImageFormat);

                            imagesSavedCount++;

                            // RACE001.TIF 00;00;02;02 00;00;03;15 000 000 720 480
                            // RACE002.TIF 00;00;05;18 00;00;09;20 000 000 720 480
                            int top = param.ScreenHeight - (param.Bitmap.Height + param.BottomMargin);
                            int left = (param.ScreenWidth - param.Bitmap.Width) / 2;

                            if (param.Alignment == ContentAlignment.BottomLeft || param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.TopLeft)
                            {
                                left = param.LeftRightMargin;
                            }
                            else if (param.Alignment == ContentAlignment.BottomRight || param.Alignment == ContentAlignment.MiddleRight || param.Alignment == ContentAlignment.TopRight)
                            {
                                left = param.ScreenWidth - param.Bitmap.Width - param.LeftRightMargin;
                            }

                            if (param.Alignment == ContentAlignment.TopLeft || param.Alignment == ContentAlignment.TopCenter || param.Alignment == ContentAlignment.TopRight)
                            {
                                top = param.BottomMargin;
                            }

                            if (param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.MiddleCenter || param.Alignment == ContentAlignment.MiddleRight)
                            {
                                top = param.ScreenHeight - (param.Bitmap.Height / 2);
                            }

                            sb.AppendLine(string.Format("{0} {1} {2} {3} {4} {5} {6}", Path.GetFileName(fileName), FormatFabTime(param.P.StartTime, param), FormatFabTime(param.P.EndTime, param), left, top, left + param.Bitmap.Width, top + param.Bitmap.Height));
                        }

                        param.Saved = true;
                    }
                }
                else if (this._exportType == "STL")
                {
                    if (!param.Saved)
                    {
                        string numberString = string.Format("IMAGE{0:000}", i);
                        string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + "." + this.comboBoxImageFormat.Text.ToLower());
                        this.SaveImage(param.Bitmap, fileName, this.ImageFormat);

                        imagesSavedCount++;

                        const string paragraphWriteFormat = "{0} , {1} , {2}\r\n";
                        const string timeFormat = "{0:00}:{1:00}:{2:00}:{3:00}";

                        double factor = TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate;
                        string startTime = string.Format(timeFormat, param.P.StartTime.Hours, param.P.StartTime.Minutes, param.P.StartTime.Seconds, (int)Math.Round(param.P.StartTime.Milliseconds / factor));
                        string endTime = string.Format(timeFormat, param.P.EndTime.Hours, param.P.EndTime.Minutes, param.P.EndTime.Seconds, (int)Math.Round(param.P.EndTime.Milliseconds / factor));
                        sb.AppendFormat(paragraphWriteFormat, startTime, endTime, fileName);

                        param.Saved = true;
                    }
                }
                else if (this._exportType == "SPUMUX")
                {
                    if (!param.Saved)
                    {
                        string numberString = string.Format("IMAGE{0:000}", i);
                        string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + "." + this.comboBoxImageFormat.Text.ToLower());

                        foreach (var encoder in ImageCodecInfo.GetImageEncoders())
                        {
                            if (encoder.FormatID == ImageFormat.Png.Guid)
                            {
                                var parameters = new EncoderParameters();
                                parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8);

                                var nbmp = new NikseBitmap(param.Bitmap);
                                var b = nbmp.ConverTo8BitsPerPixel();
                                b.Save(fileName, encoder, parameters);
                                b.Dispose();

                                break;
                            }
                        }

                        imagesSavedCount++;

                        const string paragraphWriteFormat = "\t\t<spu start=\"{0}\" end=\"{1}\" image=\"{2}\"  />";
                        const string timeFormat = "{0:00}:{1:00}:{2:00}:{3:00}";

                        double factor = TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate;
                        string startTime = string.Format(timeFormat, param.P.StartTime.Hours, param.P.StartTime.Minutes, param.P.StartTime.Seconds, (int)Math.Round(param.P.StartTime.Milliseconds / factor));
                        string endTime = string.Format(timeFormat, param.P.EndTime.Hours, param.P.EndTime.Minutes, param.P.EndTime.Seconds, (int)Math.Round(param.P.EndTime.Milliseconds / factor));
                        sb.AppendLine(string.Format(paragraphWriteFormat, startTime, endTime, fileName));

                        param.Saved = true;
                    }
                }
                else if (this._exportType == "FCP")
                {
                    if (!param.Saved)
                    {
                        string numberString = string.Format(Path.GetFileNameWithoutExtension(Path.GetFileName(param.SavDialogFileName)) + "{0:0000}", i);
                        string fileName = numberString + "." + this.comboBoxImageFormat.Text.ToLower();
                        string fileNameNoPath = Path.GetFileName(fileName);
                        string fileNameNoExt = Path.GetFileNameWithoutExtension(fileNameNoPath);
                        string template = " <clipitem id=\"" + fileNameNoPath + "\">" + Environment.NewLine +

                                          // <pathurl>file://localhost/" + fileNameNoPath.Replace(" ", "%20") + @"</pathurl>
                                          @"            <name>" + fileNameNoPath + @"</name>
            <duration>[DURATION]</duration>
            <rate>
              <ntsc>FALSE</ntsc>
              <timebase>25</timebase>
            </rate>
            <in>[IN]</in>
            <out>[OUT]</out>
            <start>[START]</start>
            <end>[END]</end>
            <pixelaspectratio>" + param.VideoResolution + @"</pixelaspectratio>
            <stillframe>TRUE</stillframe>
            <anamorphic>FALSE</anamorphic>
            <alphatype>straight</alphatype>
            <masterclipid>" + fileNameNoPath + @"1</masterclipid>" + Environment.NewLine + "           <file id=\"" + fileNameNoExt + "\">" + @"
              <name>" + fileNameNoPath + @"</name>
              <pathurl>" + fileNameNoPath.Replace(" ", "%20") + @"</pathurl>
              <rate>
                <timebase>25</timebase>
              </rate>
              <duration>[DURATION]</duration>
              <width>" + param.ScreenWidth + @"</width>
              <height>" + param.ScreenHeight + @"</height>
              <media>
                <video>
                  <duration>[DURATION]</duration>
                  <stillframe>TRUE</stillframe>
                  <samplecharacteristics>
                    <width>" + param.ScreenWidth + @"</width>
                    <height>" + param.ScreenHeight + @"</height>
                  </samplecharacteristics>
                </video>
              </media>
            </file>
            <sourcetrack>
              <mediatype>video</mediatype>
            </sourcetrack>
            <fielddominance>none</fielddominance>
          </clipitem>";

                        fileName = Path.Combine(Path.GetDirectoryName(param.SavDialogFileName), fileName);

                        if (this.comboBoxImageFormat.Text == "8-bit png")
                        {
                            foreach (var encoder in ImageCodecInfo.GetImageEncoders())
                            {
                                if (encoder.FormatID == ImageFormat.Png.Guid)
                                {
                                    var parameters = new EncoderParameters();
                                    parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8);

                                    var nbmp = new NikseBitmap(param.Bitmap);
                                    var b = nbmp.ConverTo8BitsPerPixel();
                                    b.Save(fileName, encoder, parameters);
                                    b.Dispose();

                                    break;
                                }
                            }
                        }
                        else
                        {
                            this.SaveImage(param.Bitmap, fileName, this.ImageFormat);
                        }

                        imagesSavedCount++;

                        int duration = (int)Math.Round(param.P.Duration.TotalSeconds * param.FramesPerSeconds);
                        int start = (int)Math.Round(param.P.StartTime.TotalSeconds * param.FramesPerSeconds);
                        int end = (int)Math.Round(param.P.EndTime.TotalSeconds * param.FramesPerSeconds);

                        template = template.Replace("[DURATION]", duration.ToString(CultureInfo.InvariantCulture));
                        template = template.Replace("[IN]", start.ToString(CultureInfo.InvariantCulture));
                        template = template.Replace("[OUT]", end.ToString(CultureInfo.InvariantCulture));
                        template = template.Replace("[START]", start.ToString(CultureInfo.InvariantCulture));
                        template = template.Replace("[END]", end.ToString(CultureInfo.InvariantCulture));
                        sb.AppendLine(template);

                        param.Saved = true;
                    }
                }
                else if (this._exportType == "DOST")
                {
                    if (!param.Saved)
                    {
                        string numberString = string.Format("{0:0000}", i);
                        string fileName = Path.Combine(Path.GetDirectoryName(this.saveFileDialog1.FileName), Path.GetFileNameWithoutExtension(this.saveFileDialog1.FileName).Replace(" ", "_")) + "_" + numberString + ".png";

                        foreach (var encoder in ImageCodecInfo.GetImageEncoders())
                        {
                            if (encoder.FormatID == ImageFormat.Png.Guid)
                            {
                                var parameters = new EncoderParameters();
                                parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8);

                                var nbmp = new NikseBitmap(param.Bitmap);
                                var b = nbmp.ConverTo8BitsPerPixel();
                                b.Save(fileName, encoder, parameters);
                                b.Dispose();

                                break;
                            }
                        }

                        imagesSavedCount++;

                        const string paragraphWriteFormat = "{0}\t{1}\t{2}\t{4}\t{5}\t{3}\t0\t0";

                        int top = param.ScreenHeight - (param.Bitmap.Height + param.BottomMargin);
                        int left = (param.ScreenWidth - param.Bitmap.Width) / 2;
                        if (param.Alignment == ContentAlignment.BottomLeft || param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.TopLeft)
                        {
                            left = param.LeftRightMargin;
                        }
                        else if (param.Alignment == ContentAlignment.BottomRight || param.Alignment == ContentAlignment.MiddleRight || param.Alignment == ContentAlignment.TopRight)
                        {
                            left = param.ScreenWidth - param.Bitmap.Width - param.LeftRightMargin;
                        }

                        if (param.Alignment == ContentAlignment.TopLeft || param.Alignment == ContentAlignment.TopCenter || param.Alignment == ContentAlignment.TopRight)
                        {
                            top = param.BottomMargin;
                        }

                        if (param.Alignment == ContentAlignment.MiddleLeft || param.Alignment == ContentAlignment.MiddleCenter || param.Alignment == ContentAlignment.MiddleRight)
                        {
                            top = param.ScreenHeight - (param.Bitmap.Height / 2);
                        }

                        string startTime = this.BdnXmlTimeCode(param.P.StartTime);
                        string endTime = this.BdnXmlTimeCode(param.P.EndTime);
                        sb.AppendLine(string.Format(paragraphWriteFormat, numberString, startTime, endTime, Path.GetFileName(fileName), left, top));

                        param.Saved = true;
                    }
                }
                else if (this._exportType == "IMAGE/FRAME")
                {
                    if (!param.Saved)
                    {
                        var imageFormat = this.ImageFormat;

                        int lastFrame = imagesSavedCount;
                        int startFrame = (int)Math.Round(param.P.StartTime.TotalMilliseconds / (TimeCode.BaseUnit / param.FramesPerSeconds));
                        var empty = new Bitmap(param.ScreenWidth, param.ScreenHeight);

                        if (imagesSavedCount == 0 && this.checkBoxSkipEmptyFrameAtStart.Checked)
                        {
                        }
                        else
                        {
                            // Save empty picture for each frame up to start frame
                            for (int k = lastFrame + 1; k < startFrame; k++)
                            {
                                string numberString = string.Format("{0:00000}", k);
                                string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + "." + this.comboBoxImageFormat.Text.ToLower());
                                empty.Save(fileName, imageFormat);
                                imagesSavedCount++;
                            }
                        }

                        int endFrame = (int)Math.Round(param.P.EndTime.TotalMilliseconds / (TimeCode.BaseUnit / param.FramesPerSeconds));
                        var fullSize = new Bitmap(param.ScreenWidth, param.ScreenHeight);
                        Graphics g = Graphics.FromImage(fullSize);
                        g.DrawImage(param.Bitmap, (param.ScreenWidth - param.Bitmap.Width) / 2, param.ScreenHeight - (param.Bitmap.Height + param.BottomMargin));
                        g.Dispose();

                        if (imagesSavedCount > startFrame)
                        {
                            startFrame = imagesSavedCount; // no overlapping
                        }

                        // Save sub picture for each frame in duration
                        for (int k = startFrame; k <= endFrame; k++)
                        {
                            string numberString = string.Format("{0:00000}", k);
                            string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + "." + this.comboBoxImageFormat.Text.ToLower());
                            fullSize.Save(fileName, imageFormat);
                            imagesSavedCount++;
                        }

                        fullSize.Dispose();
                        param.Saved = true;
                    }
                }
                else if (this._exportType == "DCINEMA_INTEROP")
                {
                    if (!param.Saved)
                    {
                        string numberString = string.Format("{0:0000}", i);
                        string fileName = Path.Combine(Path.GetDirectoryName(this.saveFileDialog1.FileName), numberString + ".png");
                        param.Bitmap.Save(fileName, ImageFormat.Png);
                        imagesSavedCount++;
                        param.Saved = true;
                        sb.AppendLine("<Subtitle FadeDownTime=\"" + 0 + "\" FadeUpTime=\"" + 0 + "\" TimeOut=\"" + DCSubtitle.ConvertToTimeString(param.P.EndTime) + "\" TimeIn=\"" + DCSubtitle.ConvertToTimeString(param.P.StartTime) + "\" SpotNumber=\"" + param.P.Number + "\">");
                        if (param.Depth3D == 0)
                        {
                            sb.AppendLine("<Image VPosition=\"9.7\" VAlign=\"bottom\" HAlign=\"center\">" + numberString + ".png" + "</Image>");
                        }
                        else
                        {
                            sb.AppendLine("<Image VPosition=\"9.7\" ZPosition=\"" + param.Depth3D + "\" VAlign=\"bottom\" HAlign=\"center\">" + numberString + ".png" + "</Image>");
                        }

                        sb.AppendLine("</Subtitle>");
                    }
                }
                else if (this._exportType == "EDL")
                {
                    if (!param.Saved)
                    {
                        // 001  7M6C7986 V     C        14:14:55:21 14:15:16:24 01:00:10:18 01:00:31:21
                        var fileName1 = "IMG" + i.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');

                        var fullSize = new Bitmap(param.ScreenWidth, param.ScreenHeight);
                        using (var g = Graphics.FromImage(fullSize))
                        {
                            g.DrawImage(param.Bitmap, (param.ScreenWidth - param.Bitmap.Width) / 2, param.ScreenHeight - (param.Bitmap.Height + param.BottomMargin));
                        }

                        var fileName2 = Path.Combine(Path.GetDirectoryName(param.SavDialogFileName), fileName1 + ".PNG");
                        fullSize.Save(fileName2, ImageFormat.Png);
                        fullSize.Dispose();

                        string line = string.Format("{0:000}  {1}  V     C        {2} {3} {4} {5}", i, fileName1, new TimeCode(0).ToHHMMSSFF(), param.P.Duration.ToHHMMSSFF(), param.P.StartTime.ToHHMMSSFF(), param.P.EndTime.ToHHMMSSFF());
                        sb.AppendLine(line);
                        sb.AppendLine();

                        imagesSavedCount++;
                        param.Saved = true;
                    }
                }
                else
                {
                    // BDNXML
                    if (!param.Saved)
                    {
                        string numberString = string.Format("{0:0000}", i);
                        string fileName = Path.Combine(this.folderBrowserDialog1.SelectedPath, numberString + ".png");

                        if (this.comboBoxImageFormat.Text == "Png 8-bit")
                        {
                            foreach (var encoder in ImageCodecInfo.GetImageEncoders())
                            {
                                if (encoder.FormatID == ImageFormat.Png.Guid)
                                {
                                    var parameters = new EncoderParameters();
                                    parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8);

                                    var nbmp = new NikseBitmap(param.Bitmap);
                                    var b = nbmp.ConverTo8BitsPerPixel();
                                    b.Save(fileName, encoder, parameters);
                                    b.Dispose();

                                    break;
                                }
                            }
                        }
                        else
                        {
                            param.Bitmap.Save(fileName, ImageFormat.Png);
                        }

                        imagesSavedCount++;

                        // <Event InTC="00:00:24:07" OutTC="00:00:31:13" Forced="False">
                        // <Graphic Width="696" Height="111" X="612" Y="930">subtitle_exp_0001.png</Graphic>
                        // </Event>
                        sb.AppendLine("<Event InTC=\"" + this.BdnXmlTimeCode(param.P.StartTime) + "\" OutTC=\"" + this.BdnXmlTimeCode(param.P.EndTime) + "\" Forced=\"" + param.Forced.ToString().ToLower() + "\">");

                        int x = (width - param.Bitmap.Width) / 2;
                        int y = height - (param.Bitmap.Height + param.BottomMargin);
                        switch (param.Alignment)
                        {
                            case ContentAlignment.BottomLeft:
                                x = border;
                                y = height - (param.Bitmap.Height + param.BottomMargin);
                                break;
                            case ContentAlignment.BottomRight:
                                x = height - param.Bitmap.Width - border;
                                y = height - (param.Bitmap.Height + param.BottomMargin);
                                break;
                            case ContentAlignment.MiddleCenter:
                                x = (width - param.Bitmap.Width) / 2;
                                y = (height - param.Bitmap.Height) / 2;
                                break;
                            case ContentAlignment.MiddleLeft:
                                x = border;
                                y = (height - param.Bitmap.Height) / 2;
                                break;
                            case ContentAlignment.MiddleRight:
                                x = width - param.Bitmap.Width - border;
                                y = (height - param.Bitmap.Height) / 2;
                                break;
                            case ContentAlignment.TopCenter:
                                x = (width - param.Bitmap.Width) / 2;
                                y = border;
                                break;
                            case ContentAlignment.TopLeft:
                                x = border;
                                y = border;
                                break;
                            case ContentAlignment.TopRight:
                                x = width - param.Bitmap.Width - border;
                                y = border;
                                break;
                        }

                        sb.AppendLine("  <Graphic Width=\"" + param.Bitmap.Width.ToString(CultureInfo.InvariantCulture) + "\" Height=\"" + param.Bitmap.Height.ToString(CultureInfo.InvariantCulture) + "\" X=\"" + x.ToString(CultureInfo.InvariantCulture) + "\" Y=\"" + y.ToString(CultureInfo.InvariantCulture) + "\">" + numberString + ".png</Graphic>");
                        sb.AppendLine("</Event>");
                        param.Saved = true;
                    }
                }
            }

            return imagesSavedCount;
        }

        /// <summary>
        /// The format fab time.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatFabTime(TimeCode time, MakeBitmapParameter param)
        {
            if (param.Bitmap.Width == 720 && param.Bitmap.Width == 480)
            {
                // NTSC
                return string.Format("{0:00};{1:00};{2:00};{3:00}", time.Hours, time.Minutes, time.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(time.Milliseconds));
            }

            // drop frame
            if (Math.Abs(param.FramesPerSeconds - 24 * (999 / 1000)) < 0.01 || Math.Abs(param.FramesPerSeconds - 29 * (999 / 1000)) < 0.01 || Math.Abs(param.FramesPerSeconds - 59 * (999 / 1000)) < 0.01)
            {
                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(time.Milliseconds));
            }

            return string.Format("{0:00};{1:00};{2:00};{3:00}", time.Hours, time.Minutes, time.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The setup image parameters.
        /// </summary>
        private void SetupImageParameters()
        {
            if (this._isLoading)
            {
                return;
            }

            if (this.subtitleListView1.SelectedItems.Count > 0 && this._format.HasStyleSupport)
            {
                Paragraph p = this._subtitle.Paragraphs[this.subtitleListView1.SelectedItems[0].Index];
                if (this._format.GetType() == typeof(AdvancedSubStationAlpha) || this._format.GetType() == typeof(SubStationAlpha))
                {
                    if (!string.IsNullOrEmpty(p.Extra))
                    {
                        this.comboBoxSubtitleFont.Enabled = false;
                        this.comboBoxSubtitleFontSize.Enabled = false;
                        this.buttonBorderColor.Enabled = false;
                        this.comboBoxHAlign.Enabled = false;
                        this.panelBorderColor.Enabled = false;
                        this.checkBoxBold.Enabled = false;
                        this.buttonColor.Enabled = false;
                        this.panelColor.Enabled = false;
                        this.comboBoxBorderWidth.Enabled = false;
                        this.comboBoxBottomMargin.Enabled = false;

                        SsaStyle style = AdvancedSubStationAlpha.GetSsaStyle(p.Extra, this._subtitle.Header);
                        if (style != null)
                        {
                            this.panelColor.BackColor = style.Primary;
                            if (this._format.GetType() == typeof(AdvancedSubStationAlpha))
                            {
                                this.panelBorderColor.BackColor = style.Outline;
                            }
                            else
                            {
                                this.panelBorderColor.BackColor = style.Background;
                            }

                            int i;
                            for (i = 0; i < this.comboBoxSubtitleFont.Items.Count; i++)
                            {
                                if (this.comboBoxSubtitleFont.Items[i].ToString().Equals(style.FontName, StringComparison.OrdinalIgnoreCase))
                                {
                                    this.comboBoxSubtitleFont.SelectedIndex = i;
                                }
                            }

                            for (i = 0; i < this.comboBoxSubtitleFontSize.Items.Count; i++)
                            {
                                if (this.comboBoxSubtitleFontSize.Items[i].ToString().Equals(style.FontSize.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                                {
                                    this.comboBoxSubtitleFontSize.SelectedIndex = i;
                                }
                            }

                            this.checkBoxBold.Checked = style.Bold;
                            for (i = 0; i < this.comboBoxBorderWidth.Items.Count; i++)
                            {
                                if (Utilities.RemoveNonNumbers(this.comboBoxBorderWidth.Items[i].ToString()).Equals(style.OutlineWidth.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                                {
                                    this.comboBoxBorderWidth.SelectedIndex = i;
                                }
                            }
                        }
                    }
                }
                else if (this._format.GetType() == typeof(TimedText10))
                {
                    if (!string.IsNullOrEmpty(p.Extra))
                    {
                    }
                }
            }

            this._subtitleColor = this.panelColor.BackColor;
            this._borderColor = this.panelBorderColor.BackColor;
            this._subtitleFontName = this.comboBoxSubtitleFont.SelectedItem.ToString();
            this._subtitleFontSize = float.Parse(this.comboBoxSubtitleFontSize.SelectedItem.ToString());
            this._subtitleFontBold = this.checkBoxBold.Checked;

            if (this.comboBoxBorderWidth.SelectedItem.ToString() == Configuration.Settings.Language.ExportPngXml.BorderStyleBoxForEachLine)
            {
                this._borderWidth = 0;
            }
            else if (this.comboBoxBorderWidth.SelectedItem.ToString() == Configuration.Settings.Language.ExportPngXml.BorderStyleOneBox)
            {
                this._borderWidth = 0;
            }
            else
            {
                this._borderWidth = float.Parse(Utilities.RemoveNonNumbers(this.comboBoxBorderWidth.SelectedItem.ToString()));
            }
        }

        /// <summary>
        /// The set font.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="fontSize">
        /// The font size.
        /// </param>
        /// <returns>
        /// The <see cref="Font"/>.
        /// </returns>
        private static Font SetFont(MakeBitmapParameter parameter, float fontSize)
        {
            Font font;
            try
            {
                var fontStyle = FontStyle.Regular;
                if (parameter.SubtitleFontBold)
                {
                    fontStyle = FontStyle.Bold;
                }

                font = new Font(parameter.SubtitleFontName, fontSize, fontStyle);
            }
            catch (Exception exception)
            {
                try
                {
                    var fontStyle = FontStyle.Regular;
                    if (!parameter.SubtitleFontBold)
                    {
                        fontStyle = FontStyle.Bold;
                    }

                    font = new Font(parameter.SubtitleFontName, fontSize, fontStyle);
                }
                catch
                {
                    MessageBox.Show(exception.Message);

                    if (FontFamily.Families[0].IsStyleAvailable(FontStyle.Regular))
                    {
                        font = new Font(FontFamily.Families[0].Name, fontSize);
                    }
                    else if (FontFamily.Families.Length > 1 && FontFamily.Families[1].IsStyleAvailable(FontStyle.Regular))
                    {
                        font = new Font(FontFamily.Families[1].Name, fontSize);
                    }
                    else if (FontFamily.Families.Length > 2 && FontFamily.Families[1].IsStyleAvailable(FontStyle.Regular))
                    {
                        font = new Font(FontFamily.Families[2].Name, fontSize);
                    }
                    else
                    {
                        font = new Font("Arial", fontSize);
                    }
                }
            }

            return font;
        }

        /// <summary>
        /// The generate image from text with style.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="mbp">
        /// The mbp.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private Bitmap GenerateImageFromTextWithStyle(Paragraph p, out MakeBitmapParameter mbp)
        {
            mbp = new MakeBitmapParameter();
            mbp.P = p;

            if (this._vobSubOcr != null)
            {
                var index = this._subtitle.GetIndex(p);
                if (index >= 0)
                {
                    return this._vobSubOcr.GetSubtitleBitmap(index);
                }
            }

            mbp.AlignLeft = this.comboBoxHAlign.SelectedIndex == 0;
            mbp.AlignRight = this.comboBoxHAlign.SelectedIndex == 2;
            mbp.SimpleRendering = this.checkBoxSimpleRender.Checked;
            mbp.BorderWidth = this._borderWidth;
            mbp.BorderColor = this._borderColor;
            mbp.SubtitleFontName = this._subtitleFontName;
            mbp.SubtitleColor = this._subtitleColor;
            mbp.SubtitleFontSize = this._subtitleFontSize;
            mbp.SubtitleFontBold = this._subtitleFontBold;
            mbp.LineHeight = (int)this.numericUpDownLineSpacing.Value;
            mbp.FullFrame = this.checkBoxFullFrameImage.Checked;
            mbp.FullFrameBackgroundcolor = this.panelFullFrameBackground.BackColor;

            if (this._format.HasStyleSupport && !string.IsNullOrEmpty(p.Extra))
            {
                if (this._format.GetType() == typeof(SubStationAlpha))
                {
                    var style = AdvancedSubStationAlpha.GetSsaStyle(p.Extra, this._subtitle.Header);
                    mbp.SubtitleColor = style.Primary;
                    mbp.SubtitleFontBold = style.Bold;
                    mbp.SubtitleFontSize = style.FontSize;
                    if (style.BorderStyle == "3")
                    {
                        mbp.BackgroundColor = style.Background;
                    }
                }
                else if (this._format.GetType() == typeof(AdvancedSubStationAlpha))
                {
                    var style = AdvancedSubStationAlpha.GetSsaStyle(p.Extra, this._subtitle.Header);
                    mbp.SubtitleColor = style.Primary;
                    mbp.SubtitleFontBold = style.Bold;
                    mbp.SubtitleFontSize = style.FontSize;
                    if (style.BorderStyle == "3")
                    {
                        mbp.BackgroundColor = style.Outline;
                    }
                }
            }

            if (this.comboBoxBorderWidth.SelectedItem.ToString() == Configuration.Settings.Language.ExportPngXml.BorderStyleBoxForEachLine)
            {
                this._borderWidth = 0;
                mbp.BackgroundColor = this.panelBorderColor.BackColor;
                mbp.BoxSingleLine = true;
            }
            else if (this.comboBoxBorderWidth.SelectedItem.ToString() == Configuration.Settings.Language.ExportPngXml.BorderStyleOneBox)
            {
                this._borderWidth = 0;
                mbp.BackgroundColor = this.panelBorderColor.BackColor;
            }

            int width;
            int height;
            this.GetResolution(out width, out height);
            mbp.ScreenWidth = width;
            mbp.ScreenHeight = height;
            mbp.VideoResolution = this.comboBoxResolution.Text;
            mbp.Type3D = this.comboBox3D.SelectedIndex;
            mbp.Depth3D = (int)this.numericUpDownDepth3D.Value;
            mbp.BottomMargin = this.comboBoxBottomMargin.SelectedIndex;
            mbp.ShadowWidth = this.comboBoxShadowWidth.SelectedIndex;
            mbp.ShadowAlpha = (int)this.numericUpDownShadowTransparency.Value;
            mbp.ShadowColor = this.panelShadowColor.BackColor;
            mbp.LineHeight = (int)this.numericUpDownLineSpacing.Value;
            mbp.Forced = this.subtitleListView1.Items[this._subtitle.GetIndex(p)].Checked;
            mbp.LineJoin = Configuration.Settings.Tools.ExportPenLineJoin;
            var bmp = GenerateImageFromTextWithStyle(mbp);
            if (this._exportType == "VOBSUB" || this._exportType == "STL" || this._exportType == "SPUMUX")
            {
                var nbmp = new NikseBitmap(bmp);
                nbmp.ConverToFourColors(Color.Transparent, this._subtitleColor, this._borderColor, !this.checkBoxTransAntiAliase.Checked);
                var temp = nbmp.GetBitmap();
                bmp.Dispose();
                return temp;
            }

            return bmp;
        }

        /// <summary>
        /// The calc width via draw.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalcWidthViaDraw(string text, MakeBitmapParameter parameter)
        {
            // text = HtmlUtil.RemoveHtmlTags(text, true).Trim();
            text = text.Trim();
            var path = new GraphicsPath();
            var sb = new StringBuilder();
            int i = 0;
            bool isItalic = false;
            bool isBold = parameter.SubtitleFontBold;
            const float top = 5f;
            bool newLine = false;
            float left = 1.0f;
            float leftMargin = left;
            int newLinePathPoint = -1;
            Color c = parameter.SubtitleColor;
            var colorStack = new Stack<Color>();
            var lastText = new StringBuilder();
            var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            var bmp = new Bitmap(parameter.ScreenWidth, 200);
            var g = Graphics.FromImage(bmp);

            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Font font = SetFont(parameter, parameter.SubtitleFontSize);
            while (i < text.Length)
            {
                if (text.Substring(i).StartsWith("<font ", StringComparison.OrdinalIgnoreCase))
                {
                    float addLeft = 0;
                    int oldPathPointIndex = path.PointCount;
                    if (oldPathPointIndex < 0)
                    {
                        oldPathPointIndex = 0;
                    }

                    if (sb.Length > 0)
                    {
                        lastText.Append(sb);
                        TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    }

                    if (path.PointCount > 0)
                    {
                        var list = (PointF[])path.PathPoints.Clone(); // avoid using very slow path.PathPoints indexer!!!
                        for (int k = oldPathPointIndex; k < list.Length; k++)
                        {
                            if (list[k].X > addLeft)
                            {
                                addLeft = list[k].X;
                            }
                        }
                    }

                    if (path.PointCount == 0)
                    {
                        addLeft = left;
                    }
                    else if (addLeft < 0.01)
                    {
                        addLeft = left + 2;
                    }

                    left = addLeft;

                    DrawShadowAndPath(parameter, g, path);
                    var p2 = new SolidBrush(c);
                    g.FillPath(p2, path);
                    p2.Dispose();
                    path.Reset();
                    path = new GraphicsPath();
                    sb = new StringBuilder();

                    int endIndex = text.Substring(i).IndexOf('>');
                    if (endIndex < 0)
                    {
                        i += 9999;
                    }
                    else
                    {
                        string fontContent = text.Substring(i, endIndex);
                        if (fontContent.Contains(" color="))
                        {
                            string[] arr = fontContent.Substring(fontContent.IndexOf(" color=", StringComparison.Ordinal) + 7).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length > 0)
                            {
                                string fontColor = arr[0].Trim('\'').Trim('"').Trim('\'');
                                try
                                {
                                    colorStack.Push(c); // save old color
                                    if (fontColor.StartsWith("rgb("))
                                    {
                                        arr = fontColor.Remove(0, 4).TrimEnd(')').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        c = Color.FromArgb(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
                                    }
                                    else
                                    {
                                        c = ColorTranslator.FromHtml(fontColor);
                                    }
                                }
                                catch
                                {
                                    c = parameter.SubtitleColor;
                                }
                            }
                        }

                        i += endIndex;
                    }
                }
                else if (text.Substring(i).StartsWith("</font>", StringComparison.OrdinalIgnoreCase))
                {
                    if (text.Substring(i).ToLower().Replace("</font>", string.Empty).Length > 0)
                    {
                        if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                        {
                            string t = sb.ToString();
                            sb.Clear();
                            sb.Append(' ');
                            sb.Append(t);
                        }

                        float addLeft = 0;
                        int oldPathPointIndex = path.PointCount - 1;
                        if (oldPathPointIndex < 0)
                        {
                            oldPathPointIndex = 0;
                        }

                        if (sb.Length > 0)
                        {
                            if (lastText.Length > 0 && left > 2)
                            {
                                left -= 1.5f;
                            }

                            lastText.Append(sb);

                            TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                        }

                        if (path.PointCount > 0)
                        {
                            var list = (PointF[])path.PathPoints.Clone(); // avoid using very slow path.PathPoints indexer!!!
                            for (int k = oldPathPointIndex; k < list.Length; k++)
                            {
                                if (list[k].X > addLeft)
                                {
                                    addLeft = list[k].X;
                                }
                            }
                        }

                        if (addLeft < 0.01)
                        {
                            addLeft = left + 2;
                        }

                        left = addLeft;

                        DrawShadowAndPath(parameter, g, path);
                        g.FillPath(new SolidBrush(c), path);
                        path.Reset();
                        sb = new StringBuilder();
                        if (colorStack.Count > 0)
                        {
                            c = colorStack.Pop();
                        }

                        if (left >= 3)
                        {
                            left -= 2.5f;
                        }
                    }

                    i += 6;
                }
                else if (text.Substring(i).StartsWith("<i>", StringComparison.OrdinalIgnoreCase))
                {
                    if (sb.Length > 0)
                    {
                        lastText.Append(sb);
                        TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    }

                    isItalic = true;
                    i += 2;
                }
                else if (text.Substring(i).StartsWith("</i>", StringComparison.OrdinalIgnoreCase) && isItalic)
                {
                    if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                    {
                        string t = sb.ToString();
                        sb.Clear();
                        sb.Append(' ');
                        sb.Append(t);
                    }

                    lastText.Append(sb);
                    TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    isItalic = false;
                    i += 3;
                }
                else if (text.Substring(i).StartsWith("<b>", StringComparison.OrdinalIgnoreCase))
                {
                    if (sb.Length > 0)
                    {
                        lastText.Append(sb);
                        TextDraw.DrawText(font, sf, path, sb, isItalic, isBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    }

                    isBold = true;
                    i += 2;
                }
                else if (text.Substring(i).StartsWith("</b>", StringComparison.OrdinalIgnoreCase) && isBold)
                {
                    if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                    {
                        string t = sb.ToString();
                        sb.Clear();
                        sb.Append(' ');
                        sb.Append(t);
                    }

                    lastText.Append(sb);
                    TextDraw.DrawText(font, sf, path, sb, isItalic, isBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                    isBold = false;
                    i += 3;
                }
                else
                {
                    sb.Append(text[i]);
                }

                i++;
            }

            if (sb.Length > 0)
            {
                TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
            }

            DrawShadowAndPath(parameter, g, path);
            g.FillPath(new SolidBrush(c), path);
            g.Dispose();

            var nbmp = new NikseBitmap(bmp);
            nbmp.CropTransparentSidesAndBottom(0, true);
            bmp.Dispose();
            font.Dispose();
            sf.Dispose();
            return nbmp.Width;
        }

        /// <summary>
        /// The generate image from text with style.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private static Bitmap GenerateImageFromTextWithStyle(MakeBitmapParameter parameter)
        {
            Bitmap bmp = null;
            if (!parameter.SimpleRendering && parameter.P.Text.Contains(Environment.NewLine) && (parameter.BoxSingleLine || parameter.P.Text.Contains(BoxSingleLineText)))
            {
                string old = parameter.P.Text;
                int oldType3D = parameter.Type3D;
                if (parameter.Type3D == 2)
                {
                    // Half-Top/Bottom 3D
                    parameter.Type3D = 0; // fix later
                }

                Color oldBackgroundColor = parameter.BackgroundColor;
                if (parameter.P.Text.Contains(BoxSingleLineText))
                {
                    parameter.P.Text = parameter.P.Text.Replace("<" + BoxSingleLineText + ">", string.Empty).Replace("</" + BoxSingleLineText + ">", string.Empty);
                    parameter.BackgroundColor = parameter.BorderColor;
                }

                bool italicOn = false;
                string fontTag = string.Empty;
                foreach (string line in parameter.P.Text.Replace(Environment.NewLine, "\n").Split('\n'))
                {
                    parameter.P.Text = line;
                    if (italicOn)
                    {
                        parameter.P.Text = "<i>" + parameter.P.Text;
                    }

                    italicOn = parameter.P.Text.Contains("<i>") && !parameter.P.Text.Contains("</i>");

                    parameter.P.Text = fontTag + parameter.P.Text;
                    if (parameter.P.Text.Contains("<font ") && !parameter.P.Text.Contains("</font>"))
                    {
                        int start = parameter.P.Text.LastIndexOf("<font ", StringComparison.Ordinal);
                        int end = parameter.P.Text.IndexOf('>', start);
                        fontTag = parameter.P.Text.Substring(start, end - start + 1);
                    }

                    var lineImage = GenerateImageFromTextWithStyleInner(parameter);
                    if (bmp == null)
                    {
                        bmp = lineImage;
                    }
                    else
                    {
                        int w = Math.Max(bmp.Width, lineImage.Width);
                        int h = bmp.Height + lineImage.Height;

                        int l1;
                        if (parameter.AlignLeft)
                        {
                            l1 = 0;
                        }
                        else if (parameter.AlignRight)
                        {
                            l1 = w - bmp.Width;
                        }
                        else
                        {
                            l1 = (int)Math.Round((w - bmp.Width) / 2.0);
                        }

                        int l2;
                        if (parameter.AlignLeft)
                        {
                            l2 = 0;
                        }
                        else if (parameter.AlignRight)
                        {
                            l2 = w - lineImage.Width;
                        }
                        else
                        {
                            l2 = (int)Math.Round((w - lineImage.Width) / 2.0);
                        }

                        if (parameter.LineHeight > lineImage.Height)
                        {
                            h += parameter.LineHeight - lineImage.Height;
                            var largeImage = new Bitmap(w, h);
                            var g = Graphics.FromImage(largeImage);
                            g.DrawImageUnscaled(bmp, new Point(l1, 0));
                            g.DrawImageUnscaled(lineImage, new Point(l2, bmp.Height + parameter.LineHeight - lineImage.Height));
                            bmp.Dispose();
                            bmp = largeImage;
                            g.Dispose();
                        }
                        else
                        {
                            var largeImage = new Bitmap(w, h);
                            var g = Graphics.FromImage(largeImage);
                            g.DrawImageUnscaled(bmp, new Point(l1, 0));
                            g.DrawImageUnscaled(lineImage, new Point(l2, bmp.Height));
                            bmp.Dispose();
                            bmp = largeImage;
                            g.Dispose();
                        }
                    }
                }

                parameter.P.Text = old;
                parameter.Type3D = oldType3D;
                parameter.BackgroundColor = oldBackgroundColor;

                if (parameter.Type3D == 2)
                {
                    // Half-side-by-side 3D - due to per line we need to do this after making lines
                    var newBmp = Make3DTopBottom(parameter, new NikseBitmap(bmp)).GetBitmap();
                    if (bmp != null)
                    {
                        bmp.Dispose();
                    }

                    bmp = newBmp;
                }
            }
            else
            {
                Color oldBackgroundColor = parameter.BackgroundColor;
                string oldText = parameter.P.Text;
                if (parameter.P.Text.Contains(BoxMultiLineText) || parameter.P.Text.Contains(BoxSingleLineText))
                {
                    parameter.P.Text = parameter.P.Text.Replace("<" + BoxMultiLineText + ">", string.Empty).Replace("</" + BoxMultiLineText + ">", string.Empty);
                    parameter.P.Text = parameter.P.Text.Replace("<" + BoxSingleLineText + ">", string.Empty).Replace("</" + BoxSingleLineText + ">", string.Empty);
                    parameter.BackgroundColor = parameter.BorderColor;
                }

                bmp = GenerateImageFromTextWithStyleInner(parameter);
                parameter.P.Text = oldText;
                parameter.BackgroundColor = oldBackgroundColor;
            }

            return bmp;
        }

        /// <summary>
        /// The generate image from text with style inner.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private static Bitmap GenerateImageFromTextWithStyleInner(MakeBitmapParameter parameter)
        {
            string text = parameter.P.Text;

            text = RemoveSubStationAlphaFormatting(text);

            text = text.Replace("<I>", "<i>");
            text = text.Replace("</I>", "</i>");
            text = HtmlUtil.FixInvalidItalicTags(text);

            text = text.Replace("<B>", "<b>");
            text = text.Replace("</B>", "</b>");

            // no support for underline
            text = HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagUnderline);

            Font font = null;
            Bitmap bmp = null;
            try
            {
                font = SetFont(parameter, parameter.SubtitleFontSize);
                var lineHeight = parameter.LineHeight; // (textSize.Height * 0.64f);

                SizeF textSize;
                using (var bmpTemp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmpTemp))
                {
                    textSize = g.MeasureString(HtmlUtil.RemoveHtmlTags(text), font);
                }

                int sizeX = (int)(textSize.Width * 1.8) + 150;
                int sizeY = (int)(textSize.Height * 0.9) + 50;
                if (sizeX < 1)
                {
                    sizeX = 1;
                }

                if (sizeY < 1)
                {
                    sizeY = 1;
                }

                if (parameter.BackgroundColor != Color.Transparent)
                {
                    var nbmpTemp = new NikseBitmap(sizeX, sizeY);
                    nbmpTemp.Fill(parameter.BackgroundColor);
                    bmp = nbmpTemp.GetBitmap();
                }
                else
                {
                    bmp = new Bitmap(sizeX, sizeY);
                }

                // align lines with gjpqy, a bit lower
                var lines = text.SplitToLines();
                int baseLinePadding = 13;
                if (parameter.SubtitleFontSize < 30)
                {
                    baseLinePadding = 12;
                }

                if (parameter.SubtitleFontSize < 25)
                {
                    baseLinePadding = 9;
                }

                if (lines.Length > 0)
                {
                    var lastLine = lines[lines.Length - 1];
                    if (lastLine.Contains(new[] { 'g', 'j', 'p', 'q', 'y', ',', 'ý', 'ę', 'ç', 'Ç' }))
                    {
                        var textNoBelow = lastLine.Replace('g', 'a').Replace('j', 'a').Replace('p', 'a').Replace('q', 'a').Replace('y', 'a').Replace(',', 'a').Replace('ý', 'a').Replace('ę', 'a').Replace('ç', 'a').Replace('Ç', 'a');
                        baseLinePadding -= (int)Math.Round(TextDraw.MeasureTextHeight(font, lastLine, parameter.SubtitleFontBold) - TextDraw.MeasureTextHeight(font, textNoBelow, parameter.SubtitleFontBold));
                    }
                    else
                    {
                        baseLinePadding += 1;
                    }

                    if (baseLinePadding < 0)
                    {
                        baseLinePadding = 0;
                    }
                }

                // TODO: Better baseline - test http://bobpowell.net/formattingtext.aspx
                // float baselineOffset=font.SizeInPoints/font.FontFamily.GetEmHeight(font.Style)*font.FontFamily.GetCellAscent(font.Style);
                // float baselineOffsetPixels = g.DpiY/72f*baselineOffset;
                // baseLinePadding = (int)Math.Round(baselineOffsetPixels);
                var lefts = new List<float>();
                if (text.Contains("<font", StringComparison.OrdinalIgnoreCase) || text.Contains("<i>", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string line in text.SplitToLines())
                    {
                        var lineNoHtml = HtmlUtil.RemoveOpenCloseTags(line, HtmlUtil.TagItalic, HtmlUtil.TagFont);
                        if (parameter.AlignLeft)
                        {
                            lefts.Add(5);
                        }
                        else if (parameter.AlignRight)
                        {
                            lefts.Add(bmp.Width - CalcWidthViaDraw(lineNoHtml, parameter) - 15); // calculate via drawing+crop
                        }
                        else
                        {
                            lefts.Add((float)((bmp.Width - CalcWidthViaDraw(lineNoHtml, parameter) + 5.0) / 2.0)); // calculate via drawing+crop
                        }
                    }
                }
                else
                {
                    foreach (var line in HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagItalic, HtmlUtil.TagFont).SplitToLines())
                    {
                        if (parameter.AlignLeft)
                        {
                            lefts.Add(5);
                        }
                        else if (parameter.AlignRight)
                        {
                            lefts.Add(bmp.Width - (TextDraw.MeasureTextWidth(font, line, parameter.SubtitleFontBold) + 15));
                        }
                        else
                        {
                            lefts.Add((float)((bmp.Width - TextDraw.MeasureTextWidth(font, line, parameter.SubtitleFontBold) + 15) / 2.0));
                        }
                    }
                }

                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

                using (var g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    if (parameter.SimpleRendering)
                    {
                        if (text.StartsWith("<font ", StringComparison.Ordinal) && Utilities.CountTagInText(text, "<font") == 1)
                        {
                            parameter.SubtitleColor = Utilities.GetColorFromFontString(text, parameter.SubtitleColor);
                        }

                        text = HtmlUtil.RemoveHtmlTags(text, true); // TODO: Perhaps check single color...
                        var brush = new SolidBrush(parameter.BorderColor);
                        int x = 3;
                        const int y = 3;
                        sf.Alignment = StringAlignment.Near;
                        if (parameter.AlignLeft)
                        {
                            sf.Alignment = StringAlignment.Near;
                        }
                        else if (parameter.AlignRight)
                        {
                            sf.Alignment = StringAlignment.Far;
                            x = parameter.ScreenWidth - 5;
                        }
                        else
                        {
                            sf.Alignment = StringAlignment.Center;
                            x = parameter.ScreenWidth / 2;
                        }

                        bmp = new Bitmap(parameter.ScreenWidth, sizeY);

                        Graphics surface = Graphics.FromImage(bmp);
                        surface.CompositingQuality = CompositingQuality.HighSpeed;
                        surface.InterpolationMode = InterpolationMode.Default;
                        surface.SmoothingMode = SmoothingMode.HighSpeed;
                        surface.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        for (int j = 0; j < parameter.BorderWidth; j++)
                        {
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y + 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y + 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y + 1 + j }, sf);

                            surface.DrawString(text, font, brush, new PointF { X = x - j, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j, Y = y + 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j + 1, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j + 1, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j + 1, Y = y + 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j - 1, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j - 1, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j - 1, Y = y + 1 + j }, sf);

                            surface.DrawString(text, font, brush, new PointF { X = x - j, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j, Y = y + 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j + 1, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j + 1, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j + 1, Y = y + 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j - 1, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j - 1, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - j - 1, Y = y + 1 - j }, sf);

                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y + 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y + 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y + 1 - j }, sf);

                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j, Y = y + 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j + 1, Y = y + 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y - 1 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y - 0 + j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + j - 1, Y = y + 1 + j }, sf);

                            surface.DrawString(text, font, brush, new PointF { X = x, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x, Y = y + 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + 1, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + 1, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x + 1, Y = y + 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - 1, Y = y - 1 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - 1, Y = y - 0 - j }, sf);
                            surface.DrawString(text, font, brush, new PointF { X = x - 1, Y = y + 1 - j }, sf);
                        }

                        brush.Dispose();
                        brush = new SolidBrush(parameter.SubtitleColor);
                        surface.CompositingQuality = CompositingQuality.HighQuality;
                        surface.SmoothingMode = SmoothingMode.HighQuality;
                        surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        surface.DrawString(text, font, brush, new PointF { X = x, Y = y }, sf);
                        surface.Dispose();
                        brush.Dispose();
                    }
                    else
                    {
                        var path = new GraphicsPath();
                        var sb = new StringBuilder();
                        bool isItalic = false;
                        bool isBold = parameter.SubtitleFontBold;
                        float left = 5;
                        if (lefts.Count > 0)
                        {
                            left = lefts[0];
                        }

                        float top = 5;
                        bool newLine = false;
                        int lineNumber = 0;
                        float leftMargin = left;
                        int newLinePathPoint = -1;
                        Color c = parameter.SubtitleColor;
                        var colorStack = new Stack<Color>();
                        var lastText = new StringBuilder();
                        int numberOfCharsOnCurrentLine = 0;
                        for (var i = 0; i < text.Length; i++)
                        {
                            if (text.Substring(i).StartsWith("<font ", StringComparison.OrdinalIgnoreCase))
                            {
                                float addLeft = 0;
                                int oldPathPointIndex = path.PointCount;
                                if (oldPathPointIndex < 0)
                                {
                                    oldPathPointIndex = 0;
                                }

                                if (sb.Length > 0)
                                {
                                    lastText.Append(sb);
                                    TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                                }

                                if (path.PointCount > 0)
                                {
                                    var list = (PointF[])path.PathPoints.Clone(); // avoid using very slow path.PathPoints indexer!!!
                                    for (int k = oldPathPointIndex; k < list.Length; k++)
                                    {
                                        if (list[k].X > addLeft)
                                        {
                                            addLeft = list[k].X;
                                        }
                                    }
                                }

                                if (path.PointCount == 0)
                                {
                                    addLeft = left;
                                }
                                else if (addLeft < 0.01)
                                {
                                    addLeft = left + 2;
                                }

                                left = addLeft;

                                DrawShadowAndPath(parameter, g, path);
                                var p2 = new SolidBrush(c);
                                g.FillPath(p2, path);
                                p2.Dispose();
                                path.Reset();
                                path = new GraphicsPath();
                                sb = new StringBuilder();

                                int endIndex = text.Substring(i).IndexOf('>');
                                if (endIndex < 0)
                                {
                                    i += 9999;
                                }
                                else
                                {
                                    string fontContent = text.Substring(i, endIndex);
                                    if (fontContent.Contains(" color="))
                                    {
                                        string[] arr = fontContent.Substring(fontContent.IndexOf(" color=", StringComparison.Ordinal) + 7).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (arr.Length > 0)
                                        {
                                            string fontColor = arr[0].Trim('\'').Trim('"').Trim('\'');
                                            try
                                            {
                                                colorStack.Push(c); // save old color
                                                if (fontColor.StartsWith("rgb(", StringComparison.Ordinal))
                                                {
                                                    arr = fontColor.Remove(0, 4).TrimEnd(')').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    c = Color.FromArgb(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
                                                }
                                                else
                                                {
                                                    c = ColorTranslator.FromHtml(fontColor);
                                                }
                                            }
                                            catch
                                            {
                                                c = parameter.SubtitleColor;
                                            }
                                        }
                                    }

                                    i += endIndex;
                                }
                            }
                            else if (text.Substring(i).StartsWith("</font>", StringComparison.OrdinalIgnoreCase))
                            {
                                if (text.Substring(i).ToLower().Replace("</font>", string.Empty).Length > 0)
                                {
                                    if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                                    {
                                        string t = sb.ToString();
                                        sb.Clear();
                                        sb.Append(' ');
                                        sb.Append(t);
                                    }

                                    float addLeft = 0;
                                    int oldPathPointIndex = path.PointCount - 1;
                                    if (oldPathPointIndex < 0)
                                    {
                                        oldPathPointIndex = 0;
                                    }

                                    if (sb.Length > 0)
                                    {
                                        if (lastText.Length > 0 && left > 2)
                                        {
                                            left -= 1.5f;
                                        }

                                        lastText.Append(sb);

                                        TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                                    }

                                    if (path.PointCount > 0)
                                    {
                                        var list = (PointF[])path.PathPoints.Clone(); // avoid using very slow path.PathPoints indexer!!!
                                        for (int k = oldPathPointIndex; k < list.Length; k++)
                                        {
                                            if (list[k].X > addLeft)
                                            {
                                                addLeft = list[k].X;
                                            }
                                        }
                                    }

                                    if (addLeft < 0.01)
                                    {
                                        addLeft = left + 2;
                                    }

                                    left = addLeft;

                                    DrawShadowAndPath(parameter, g, path);
                                    g.FillPath(new SolidBrush(c), path);
                                    path.Reset();
                                    sb = new StringBuilder();
                                    if (colorStack.Count > 0)
                                    {
                                        c = colorStack.Pop();
                                    }

                                    if (left >= 3)
                                    {
                                        left -= 2.5f;
                                    }
                                }

                                i += 6;
                            }
                            else if (text.Substring(i).StartsWith("<i>", StringComparison.OrdinalIgnoreCase))
                            {
                                if (sb.Length > 0)
                                {
                                    lastText.Append(sb);
                                    TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                                }

                                isItalic = true;
                                i += 2;
                            }
                            else if (text.Substring(i).StartsWith("</i>", StringComparison.OrdinalIgnoreCase) && isItalic)
                            {
                                if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                                {
                                    string t = sb.ToString();
                                    sb.Clear();
                                    sb.Append(' ');
                                    sb.Append(t);
                                }

                                lastText.Append(sb);
                                TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                                isItalic = false;
                                i += 3;
                            }
                            else if (text.Substring(i).StartsWith("<b>", StringComparison.OrdinalIgnoreCase))
                            {
                                if (sb.Length > 0)
                                {
                                    lastText.Append(sb);
                                    TextDraw.DrawText(font, sf, path, sb, isItalic, isBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                                }

                                isBold = true;
                                i += 2;
                            }
                            else if (text.Substring(i).StartsWith("</b>", StringComparison.OrdinalIgnoreCase) && isBold)
                            {
                                if (lastText.EndsWith(' ') && !sb.StartsWith(' '))
                                {
                                    string t = sb.ToString();
                                    sb.Clear();
                                    sb.Append(' ');
                                    sb.Append(t);
                                }

                                lastText.Append(sb);
                                TextDraw.DrawText(font, sf, path, sb, isItalic, isBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                                isBold = false;
                                i += 3;
                            }
                            else if (text.Substring(i).StartsWith(Environment.NewLine, StringComparison.Ordinal))
                            {
                                lastText.Append(sb);
                                TextDraw.DrawText(font, sf, path, sb, isItalic, isBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);

                                top += lineHeight;
                                newLine = true;
                                i += Environment.NewLine.Length - 1;
                                lineNumber++;
                                if (lineNumber < lefts.Count)
                                {
                                    leftMargin = lefts[lineNumber];
                                    left = leftMargin;
                                }

                                numberOfCharsOnCurrentLine = 0;
                            }
                            else
                            {
                                if (numberOfCharsOnCurrentLine != 0 || text[i] != ' ')
                                {
                                    sb.Append(text[i]);
                                    numberOfCharsOnCurrentLine++;
                                }
                            }
                        }

                        if (sb.Length > 0)
                        {
                            TextDraw.DrawText(font, sf, path, sb, isItalic, parameter.SubtitleFontBold, false, left, top, ref newLine, leftMargin, ref newLinePathPoint);
                        }

                        DrawShadowAndPath(parameter, g, path);
                        g.FillPath(new SolidBrush(c), path);
                    }
                }

                sf.Dispose();

                var nbmp = new NikseBitmap(bmp);
                if (parameter.BackgroundColor == Color.Transparent)
                {
                    nbmp.CropTransparentSidesAndBottom(baseLinePadding, true);
                    nbmp.CropTransparentSidesAndBottom(2, false);
                }
                else
                {
                    nbmp.CropSidesAndBottom(4, parameter.BackgroundColor, true);
                    nbmp.CropTop(4, parameter.BackgroundColor);
                }

                if (nbmp.Width > parameter.ScreenWidth)
                {
                    parameter.Error = "#" + parameter.P.Number.ToString(CultureInfo.InvariantCulture) + ": " + nbmp.Width.ToString(CultureInfo.InvariantCulture) + " > " + parameter.ScreenWidth.ToString(CultureInfo.InvariantCulture);
                }

                if (parameter.Type3D == 1)
                {
                    // Half-side-by-side 3D
                    Bitmap singleBmp = nbmp.GetBitmap();
                    Bitmap singleHalfBmp = ScaleToHalfWidth(singleBmp);
                    singleBmp.Dispose();
                    var sideBySideBmp = new Bitmap(parameter.ScreenWidth, singleHalfBmp.Height);
                    int singleWidth = parameter.ScreenWidth / 2;
                    int singleLeftMargin = (singleWidth - singleHalfBmp.Width) / 2;

                    using (Graphics gSideBySide = Graphics.FromImage(sideBySideBmp))
                    {
                        gSideBySide.DrawImage(singleHalfBmp, singleLeftMargin + parameter.Depth3D, 0);
                        gSideBySide.DrawImage(singleHalfBmp, singleWidth + singleLeftMargin - parameter.Depth3D, 0);
                    }

                    nbmp = new NikseBitmap(sideBySideBmp);
                    if (parameter.BackgroundColor == Color.Transparent)
                    {
                        nbmp.CropTransparentSidesAndBottom(2, true);
                    }
                    else
                    {
                        nbmp.CropSidesAndBottom(4, parameter.BackgroundColor, true);
                    }
                }
                else if (parameter.Type3D == 2)
                {
                    // Half-Top/Bottom 3D
                    nbmp = Make3DTopBottom(parameter, nbmp);
                }

                return nbmp.GetBitmap();
            }
            finally
            {
                if (font != null)
                {
                    font.Dispose();
                }

                if (bmp != null)
                {
                    bmp.Dispose();
                }
            }
        }

        /// <summary>
        /// The make 3 d top bottom.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <returns>
        /// The <see cref="NikseBitmap"/>.
        /// </returns>
        private static NikseBitmap Make3DTopBottom(MakeBitmapParameter parameter, NikseBitmap nbmp)
        {
            Bitmap singleBmp = nbmp.GetBitmap();
            Bitmap singleHalfBmp = ScaleToHalfHeight(singleBmp);
            singleBmp.Dispose();
            var topBottomBmp = new Bitmap(parameter.ScreenWidth, parameter.ScreenHeight - parameter.BottomMargin);
            int singleHeight = parameter.ScreenHeight / 2;
            int leftM = (parameter.ScreenWidth / 2) - (singleHalfBmp.Width / 2);

            using (Graphics gTopBottom = Graphics.FromImage(topBottomBmp))
            {
                gTopBottom.DrawImage(singleHalfBmp, leftM + parameter.Depth3D, singleHeight - singleHalfBmp.Height - parameter.BottomMargin);
                gTopBottom.DrawImage(singleHalfBmp, leftM - parameter.Depth3D, parameter.ScreenHeight - parameter.BottomMargin - singleHalfBmp.Height);
            }

            nbmp = new NikseBitmap(topBottomBmp);
            if (parameter.BackgroundColor == Color.Transparent)
            {
                nbmp.CropTop(2, Color.Transparent);
                nbmp.CropTransparentSidesAndBottom(2, false);
            }
            else
            {
                nbmp.CropTop(4, parameter.BackgroundColor);
                nbmp.CropSidesAndBottom(4, parameter.BackgroundColor, false);
            }

            return nbmp;
        }

        /// <summary>
        /// The draw shadow and path.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        private static void DrawShadowAndPath(MakeBitmapParameter parameter, Graphics g, GraphicsPath path)
        {
            if (parameter.ShadowWidth > 0)
            {
                var shadowPath = (GraphicsPath)path.Clone();
                for (int k = 0; k < parameter.ShadowWidth; k++)
                {
                    var translateMatrix = new Matrix();
                    translateMatrix.Translate(1, 1);
                    shadowPath.Transform(translateMatrix);

                    var p1 = new Pen(Color.FromArgb(parameter.ShadowAlpha, parameter.ShadowColor), parameter.BorderWidth);
                    SetLineJoin(parameter.LineJoin, p1);
                    g.DrawPath(p1, shadowPath);
                    p1.Dispose();
                }
            }

            if (parameter.BorderWidth > 0)
            {
                var p1 = new Pen(parameter.BorderColor, parameter.BorderWidth);
                SetLineJoin(parameter.LineJoin, p1);
                g.DrawPath(p1, path);
                p1.Dispose();
            }
        }

        /// <summary>
        /// The set line join.
        /// </summary>
        /// <param name="lineJoin">
        /// The line join.
        /// </param>
        /// <param name="pen">
        /// The pen.
        /// </param>
        private static void SetLineJoin(string lineJoin, Pen pen)
        {
            if (!string.IsNullOrWhiteSpace(lineJoin))
            {
                if (string.Compare(lineJoin, "Round", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pen.LineJoin = LineJoin.Round;
                }
                else if (string.Compare(lineJoin, "Bevel", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pen.LineJoin = LineJoin.Bevel;
                }
                else if (string.Compare(lineJoin, "Miter", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pen.LineJoin = LineJoin.Miter;
                }
                else if (string.Compare(lineJoin, "MiterClipped", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pen.LineJoin = LineJoin.MiterClipped;
                }
            }
        }

        /// <summary>
        /// The scale to half width.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private static Bitmap ScaleToHalfWidth(Bitmap bmp)
        {
            int w = bmp.Width / 2;
            var newImage = new Bitmap(w, bmp.Height);
            using (var gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(bmp, new Rectangle(0, 0, w, bmp.Height));
            }

            return newImage;
        }

        /// <summary>
        /// The scale to half height.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        private static Bitmap ScaleToHalfHeight(Bitmap bmp)
        {
            int h = bmp.Height / 2;
            var newImage = new Bitmap(bmp.Width, h);
            using (var gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, h));
            }

            return newImage;
        }

        /// <summary>
        /// The remove sub station alpha formatting.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveSubStationAlphaFormatting(string s)
        {
            int indexOfBegin = s.IndexOf('{');
            while (indexOfBegin >= 0 && s.IndexOf('}') > indexOfBegin)
            {
                int indexOfEnd = s.IndexOf('}');
                s = s.Remove(indexOfBegin, (indexOfEnd - indexOfBegin) + 1);
                indexOfBegin = s.IndexOf('{');
            }

            return s;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="exportType">
        /// The export type.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="videoInfo">
        /// The video info.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        internal void Initialize(Subtitle subtitle, SubtitleFormat format, string exportType, string fileName, VideoInfo videoInfo, string videoFileName)
        {
            this._exportType = exportType;
            this._fileName = fileName;
            this._format = format;
            this._videoFileName = videoFileName;
            if (exportType == "BLURAYSUP")
            {
                this.Text = "Blu-ray SUP";
            }
            else if (exportType == "VOBSUB")
            {
                this.Text = "VobSub (sub/idx)";
            }
            else if (exportType == "FAB")
            {
                this.Text = "FAB Image Script";
            }
            else if (exportType == "IMAGE/FRAME")
            {
                this.Text = "Image per frame";
            }
            else if (exportType == "STL")
            {
                this.Text = "DVD Studio Pro STL";
            }
            else if (exportType == "FCP")
            {
                this.Text = "Final Cut Pro";
            }
            else if (exportType == "DOST")
            {
                this.Text = "DOST";
            }
            else if (exportType == "EDL")
            {
                this.Text = "EDL";
            }
            else if (exportType == "DCINEMA_INTEROP")
            {
                this.Text = "DCinema interop/png";
            }
            else
            {
                this.Text = Configuration.Settings.Language.ExportPngXml.Title;
            }

            if (this._exportType == "VOBSUB" && !string.IsNullOrEmpty(Configuration.Settings.Tools.ExportVobSubFontName))
            {
                this._subtitleFontName = Configuration.Settings.Tools.ExportVobSubFontName;
            }
            else if ((this._exportType == "BLURAYSUP" || this._exportType == "DOST") && !string.IsNullOrEmpty(Configuration.Settings.Tools.ExportBluRayFontName))
            {
                this._subtitleFontName = Configuration.Settings.Tools.ExportBluRayFontName;
            }
            else if (this._exportType == "FCP" && !string.IsNullOrEmpty(Configuration.Settings.Tools.ExportFcpFontName))
            {
                this._subtitleFontName = Configuration.Settings.Tools.ExportFcpFontName;
            }
            else if (!string.IsNullOrEmpty(Configuration.Settings.Tools.ExportFontNameOther))
            {
                this._subtitleFontName = Configuration.Settings.Tools.ExportFontNameOther;
            }

            if (this._exportType == "VOBSUB" && Configuration.Settings.Tools.ExportVobSubFontSize > 0)
            {
                this._subtitleFontSize = Configuration.Settings.Tools.ExportVobSubFontSize;
            }
            else if ((this._exportType == "BLURAYSUP" || this._exportType == "DOST") && Configuration.Settings.Tools.ExportBluRayFontSize > 0)
            {
                this._subtitleFontSize = Configuration.Settings.Tools.ExportBluRayFontSize;
            }
            else if (this._exportType == "FCP" && Configuration.Settings.Tools.ExportFcpFontSize > 0)
            {
                this._subtitleFontSize = Configuration.Settings.Tools.ExportFcpFontSize;
            }
            else if (Configuration.Settings.Tools.ExportLastFontSize > 0)
            {
                this._subtitleFontSize = Configuration.Settings.Tools.ExportLastFontSize;
            }

            if (this._exportType == "FCP")
            {
                this.comboBoxImageFormat.Items.Add("8-bit png");
                int i = 0;
                foreach (string item in this.comboBoxImageFormat.Items)
                {
                    if (item == Configuration.Settings.Tools.ExportFcpImageType)
                    {
                        this.comboBoxImageFormat.SelectedIndex = i;
                    }

                    i++;
                }
            }

            if (this._exportType == "VOBSUB")
            {
                this.comboBoxSubtitleFontSize.SelectedIndex = 7;
                int i = 0;
                foreach (string item in this.comboBoxSubtitleFontSize.Items)
                {
                    if (item == Convert.ToInt32(this._subtitleFontSize).ToString(CultureInfo.InvariantCulture))
                    {
                        this.comboBoxSubtitleFontSize.SelectedIndex = i;
                    }

                    i++;
                }

                this.checkBoxSimpleRender.Checked = Configuration.Settings.Tools.ExportVobSubSimpleRendering;
                this.checkBoxTransAntiAliase.Checked = Configuration.Settings.Tools.ExportVobAntiAliasingWithTransparency;
            }
            else if (this._exportType == "BLURAYSUP" || this._exportType == "DOST" || this._exportType == "FCP")
            {
                this.comboBoxSubtitleFontSize.SelectedIndex = 16;
                int i = 0;
                foreach (string item in this.comboBoxSubtitleFontSize.Items)
                {
                    if (item == Convert.ToInt32(this._subtitleFontSize).ToString(CultureInfo.InvariantCulture))
                    {
                        this.comboBoxSubtitleFontSize.SelectedIndex = i;
                    }

                    i++;
                }
            }
            else
            {
                this.comboBoxSubtitleFontSize.SelectedIndex = 16;
                int i = 0;
                foreach (string item in this.comboBoxSubtitleFontSize.Items)
                {
                    if (item == Convert.ToInt32(this._subtitleFontSize).ToString(CultureInfo.InvariantCulture))
                    {
                        this.comboBoxSubtitleFontSize.SelectedIndex = i;
                    }

                    i++;
                }
            }

            this.groupBoxImageSettings.Text = Configuration.Settings.Language.ExportPngXml.ImageSettings;
            this.labelSubtitleFont.Text = Configuration.Settings.Language.ExportPngXml.FontFamily;
            this.labelSubtitleFontSize.Text = Configuration.Settings.Language.ExportPngXml.FontSize;
            this.labelResolution.Text = Configuration.Settings.Language.ExportPngXml.VideoResolution;
            this.buttonColor.Text = Configuration.Settings.Language.ExportPngXml.FontColor;
            this.checkBoxBold.Text = Configuration.Settings.Language.General.Bold;
            this.checkBoxSimpleRender.Text = Configuration.Settings.Language.ExportPngXml.SimpleRendering;
            this.checkBoxTransAntiAliase.Text = Configuration.Settings.Language.ExportPngXml.AntiAliasingWithTransparency;

            this.normalToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.ContextMenu.Normal;
            this.italicToolStripMenuItem.Text = Configuration.Settings.Language.General.Italic;
            this.boxSingleLineToolStripMenuItem.Text = Configuration.Settings.Language.ExportPngXml.BoxSingleLine;
            this.boxMultiLineToolStripMenuItem.Text = Configuration.Settings.Language.ExportPngXml.BoxMultiLine;

            this.comboBox3D.Items.Clear();
            this.comboBox3D.Items.Add(Configuration.Settings.Language.General.None);
            this.comboBox3D.Items.Add(Configuration.Settings.Language.ExportPngXml.SideBySide3D);
            this.comboBox3D.Items.Add(Configuration.Settings.Language.ExportPngXml.HalfTopBottom3D);
            this.comboBox3D.SelectedIndex = 0;

            this.labelDepth.Text = Configuration.Settings.Language.ExportPngXml.Depth;

            this.numericUpDownDepth3D.Left = this.labelDepth.Left + this.labelDepth.Width + 3;

            this.label3D.Text = Configuration.Settings.Language.ExportPngXml.Text3D;

            this.comboBox3D.Left = this.label3D.Left + this.label3D.Width + 3;

            this.buttonBorderColor.Text = Configuration.Settings.Language.ExportPngXml.BorderColor;

            // labelBorderWidth.Text = Configuration.Settings.Language.ExportPngXml.BorderWidth;
            this.labelBorderWidth.Text = Configuration.Settings.Language.ExportPngXml.BorderStyle;
            this.labelImageFormat.Text = Configuration.Settings.Language.ExportPngXml.ImageFormat;
            this.checkBoxFullFrameImage.Text = Configuration.Settings.Language.ExportPngXml.FullFrameImage;

            this.buttonExport.Text = Configuration.Settings.Language.ExportPngXml.ExportAllLines;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Ok;
            this.labelLanguage.Text = Configuration.Settings.Language.ChooseLanguage.Language;
            this.labelFrameRate.Text = Configuration.Settings.Language.General.FrameRate;
            this.labelHorizontalAlign.Text = Configuration.Settings.Language.ExportPngXml.Align;
            this.labelBottomMargin.Text = Configuration.Settings.Language.ExportPngXml.BottomMargin;
            this.labelLeftRightMargin.Text = Configuration.Settings.Language.ExportPngXml.LeftRightMargin;
            if (Configuration.Settings.Language.ExportPngXml.Left != null && Configuration.Settings.Language.ExportPngXml.Center != null && Configuration.Settings.Language.ExportPngXml.Right != null)
            {
                this.comboBoxHAlign.Items.Clear();
                this.comboBoxHAlign.Items.Add(Configuration.Settings.Language.ExportPngXml.Left);
                this.comboBoxHAlign.Items.Add(Configuration.Settings.Language.ExportPngXml.Center);
                this.comboBoxHAlign.Items.Add(Configuration.Settings.Language.ExportPngXml.Right);
            }

            this.buttonShadowColor.Text = Configuration.Settings.Language.ExportPngXml.ShadowColor;
            this.labelShadowWidth.Text = Configuration.Settings.Language.ExportPngXml.ShadowWidth;
            this.labelShadowTransparency.Text = Configuration.Settings.Language.ExportPngXml.Transparency;
            this.labelLineHeight.Text = Configuration.Settings.Language.ExportPngXml.LineHeight;

            this.linkLabelPreview.Text = Configuration.Settings.Language.General.Preview;
            this.linkLabelPreview.Left = this.groupBoxExportImage.Width - this.linkLabelPreview.Width - 3;

            this.saveImageAsToolStripMenuItem.Text = Configuration.Settings.Language.ExportPngXml.SaveImageAs;

            this.SubtitleListView1InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.subtitleListView1);
            this.SubtitleListView1AutoSizeAllColumns();

            this._subtitle = new Subtitle(subtitle);
            this._subtitle.Header = subtitle.Header;
            this._subtitle.Footer = subtitle.Footer;

            this.panelColor.BackColor = this._subtitleColor;
            this.panelBorderColor.BackColor = this._borderColor;
            this.InitBorderStyle();
            this.comboBoxHAlign.SelectedIndex = 1;
            this.comboBoxResolution.SelectedIndex = 3;

            if (Configuration.Settings.Tools.ExportLastShadowTransparency <= this.numericUpDownShadowTransparency.Maximum && Configuration.Settings.Tools.ExportLastShadowTransparency > 0)
            {
                this.numericUpDownShadowTransparency.Value = Configuration.Settings.Tools.ExportLastShadowTransparency;
            }

            if ((this._exportType == "BLURAYSUP" || this._exportType == "DOST") && !string.IsNullOrEmpty(Configuration.Settings.Tools.ExportBluRayVideoResolution))
            {
                this.SetResolution(Configuration.Settings.Tools.ExportBluRayVideoResolution);
            }

            if (exportType == "VOBSUB")
            {
                this.comboBoxBorderWidth.SelectedIndex = 6;
                if (this._exportType == "VOBSUB" && !string.IsNullOrEmpty(Configuration.Settings.Tools.ExportVobSubVideoResolution))
                {
                    this.SetResolution(Configuration.Settings.Tools.ExportVobSubVideoResolution);
                }
                else
                {
                    this.comboBoxResolution.SelectedIndex = 8;
                }

                this.labelLanguage.Visible = true;
                this.comboBoxLanguage.Visible = true;
                this.comboBoxLanguage.Items.Clear();
                string languageCode = Utilities.AutoDetectGoogleLanguageOrNull(subtitle);
                if (languageCode == null)
                {
                    languageCode = Configuration.Settings.Tools.ExportVobSubLanguage;
                }

                for (int i = 0; i < IfoParser.ArrayOfLanguage.Count; i++)
                {
                    this.comboBoxLanguage.Items.Add(IfoParser.ArrayOfLanguage[i]);
                    if (IfoParser.ArrayOfLanguageCode[i] == languageCode || IfoParser.ArrayOfLanguage[i] == languageCode)
                    {
                        this.comboBoxLanguage.SelectedIndex = i;
                    }
                }

                if (this.comboBoxLanguage.SelectedIndex == -1 && this.comboBoxLanguage.Items.Count > 25)
                {
                    this.comboBoxLanguage.SelectedIndex = 25;
                }
            }

            bool showImageFormat = exportType == "FAB" || exportType == "IMAGE/FRAME" || exportType == "STL" || exportType == "FCP" || exportType == "BDNXML";
            this.checkBoxFullFrameImage.Visible = exportType == "FAB" || exportType == "BLURAYSUP";
            this.comboBoxImageFormat.Visible = showImageFormat;
            this.labelImageFormat.Visible = showImageFormat;
            this.labelFrameRate.Visible = exportType == "BDNXML" || exportType == "BLURAYSUP" || exportType == "DOST" || exportType == "IMAGE/FRAME";
            this.comboBoxFrameRate.Visible = exportType == "BDNXML" || exportType == "BLURAYSUP" || exportType == "DOST" || exportType == "IMAGE/FRAME" || exportType == "FAB" || exportType == "FCP";
            this.checkBoxTransAntiAliase.Visible = exportType == "VOBSUB";
            if (exportType == "BDNXML")
            {
                this.labelFrameRate.Top = this.labelLanguage.Top;
                this.comboBoxFrameRate.Top = this.comboBoxLanguage.Top;
                this.comboBoxFrameRate.Items.Add("23.976");
                this.comboBoxFrameRate.Items.Add("24");
                this.comboBoxFrameRate.Items.Add("25");
                this.comboBoxFrameRate.Items.Add("29.97");
                this.comboBoxFrameRate.Items.Add("30");
                this.comboBoxFrameRate.Items.Add("50");
                this.comboBoxFrameRate.Items.Add("59.94");
                this.comboBoxFrameRate.SelectedIndex = 2;

                this.comboBoxImageFormat.Items.Clear();
                this.comboBoxImageFormat.Items.Add("Png 32-bit");
                this.comboBoxImageFormat.Items.Add("Png 8-bit");
                if (this.comboBoxImageFormat.Items[1].ToString() == Configuration.Settings.Tools.ExportBdnXmlImageType)
                {
                    this.comboBoxImageFormat.SelectedIndex = 1;
                }
                else
                {
                    this.comboBoxImageFormat.SelectedIndex = 0;
                }
            }
            else if (exportType == "DOST")
            {
                this.labelFrameRate.Top = this.labelLanguage.Top;
                this.comboBoxFrameRate.Top = this.comboBoxLanguage.Top;
                this.comboBoxFrameRate.Items.Add("23.98");
                this.comboBoxFrameRate.Items.Add("24");
                this.comboBoxFrameRate.Items.Add("25");
                this.comboBoxFrameRate.Items.Add("29.97");
                this.comboBoxFrameRate.Items.Add("30");
                this.comboBoxFrameRate.Items.Add("59.94");
                this.comboBoxFrameRate.SelectedIndex = 2;
            }
            else if (exportType == "IMAGE/FRAME")
            {
                this.labelFrameRate.Top = this.labelLanguage.Top;
                this.comboBoxFrameRate.Top = this.comboBoxLanguage.Top;
                this.comboBoxFrameRate.Items.Add("23.976");
                this.comboBoxFrameRate.Items.Add("24");
                this.comboBoxFrameRate.Items.Add("25");
                this.comboBoxFrameRate.Items.Add("29.97");
                this.comboBoxFrameRate.Items.Add("30");
                this.comboBoxFrameRate.Items.Add("50");
                this.comboBoxFrameRate.Items.Add("59.94");
                this.comboBoxFrameRate.Items.Add("60");
                this.comboBoxFrameRate.SelectedIndex = 2;
            }
            else if (exportType == "BLURAYSUP")
            {
                this.labelFrameRate.Top = this.labelLanguage.Top;
                this.comboBoxFrameRate.Top = this.comboBoxLanguage.Top;
                this.comboBoxFrameRate.Items.Add("23.976");
                this.comboBoxFrameRate.Items.Add("24");
                this.comboBoxFrameRate.Items.Add("25");
                this.comboBoxFrameRate.Items.Add("29.97");
                this.comboBoxFrameRate.Items.Add("50");
                this.comboBoxFrameRate.Items.Add("59.94");
                this.comboBoxFrameRate.SelectedIndex = 1;
                this.comboBoxFrameRate.DropDownStyle = ComboBoxStyle.DropDownList;

                this.checkBoxFullFrameImage.Top = this.comboBoxImageFormat.Top + 2;
                this.panelFullFrameBackground.Top = this.checkBoxFullFrameImage.Top;
            }
            else if (exportType == "FAB")
            {
                this.labelFrameRate.Visible = true;
                this.comboBoxFrameRate.Items.Add("23.976");
                this.comboBoxFrameRate.Items.Add("24");
                this.comboBoxFrameRate.Items.Add("25");
                this.comboBoxFrameRate.Items.Add("29.97");
                this.comboBoxFrameRate.Items.Add("50");
                this.comboBoxFrameRate.Items.Add("59.94");
                this.comboBoxFrameRate.SelectedIndex = 1;
                this.comboBoxFrameRate.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else if (exportType == "FCP")
            {
                this.labelFrameRate.Visible = true;
                this.comboBoxFrameRate.Items.Add("23.976");
                this.comboBoxFrameRate.Items.Add("24");
                this.comboBoxFrameRate.Items.Add("25");
                this.comboBoxFrameRate.Items.Add("29.97");
                this.comboBoxFrameRate.Items.Add("50");
                this.comboBoxFrameRate.Items.Add("59.94");
                this.comboBoxFrameRate.Items.Add("60");
                this.comboBoxFrameRate.SelectedIndex = 2;
                this.comboBoxFrameRate.DropDownStyle = ComboBoxStyle.DropDownList;
            }

            if (this.comboBoxFrameRate.Items.Count >= 2)
            {
                this.SetLastFrameRate(Configuration.Settings.Tools.ExportLastFrameRate);
            }

            this.panelShadowColor.BackColor = Configuration.Settings.Tools.ExportShadowColor;

            for (int i = 0; i < 1000; i++)
            {
                this.comboBoxBottomMargin.Items.Add(i);
            }

            if (Configuration.Settings.Tools.ExportBottomMargin >= 0 && Configuration.Settings.Tools.ExportBottomMargin < this.comboBoxBottomMargin.Items.Count)
            {
                this.comboBoxBottomMargin.SelectedIndex = Configuration.Settings.Tools.ExportBottomMargin;
            }

            if (exportType == "BLURAYSUP" || exportType == "IMAGE/FRAME" && Configuration.Settings.Tools.ExportBluRayBottomMargin >= 0 && Configuration.Settings.Tools.ExportBluRayBottomMargin < this.comboBoxBottomMargin.Items.Count)
            {
                this.comboBoxBottomMargin.SelectedIndex = Configuration.Settings.Tools.ExportBluRayBottomMargin;
            }

            if (this._exportType == "BLURAYSUP" || this._exportType == "VOBSUB" || this._exportType == "IMAGE/FRAME" || this._exportType == "BDNXML" || this._exportType == "DOST" || this._exportType == "FAB" || this._exportType == "EDL")
            {
                this.comboBoxBottomMargin.Visible = true;
                this.labelBottomMargin.Visible = true;

                this.comboBoxLeftRightMargin.Visible = true;
                this.labelLeftRightMargin.Visible = true;
                this.comboBoxLeftRightMargin.SelectedIndex = 10;
            }
            else
            {
                this.comboBoxBottomMargin.Visible = false;
                this.labelBottomMargin.Visible = false;

                this.comboBoxLeftRightMargin.Visible = false;
                this.labelLeftRightMargin.Visible = false;
            }

            this.checkBoxSkipEmptyFrameAtStart.Visible = exportType == "IMAGE/FRAME";

            foreach (var x in FontFamily.Families)
            {
                if (x.IsStyleAvailable(FontStyle.Regular) || x.IsStyleAvailable(FontStyle.Bold))
                {
                    this.comboBoxSubtitleFont.Items.Add(x.Name);
                    if (x.Name.Equals(this._subtitleFontName, StringComparison.OrdinalIgnoreCase))
                    {
                        this.comboBoxSubtitleFont.SelectedIndex = this.comboBoxSubtitleFont.Items.Count - 1;
                    }
                }
            }

            if (this.comboBoxSubtitleFont.SelectedIndex == -1)
            {
                this.comboBoxSubtitleFont.SelectedIndex = 0; // take first font if default font not found (e.g. linux)
            }

            if (videoInfo != null && videoInfo.Height > 0 && videoInfo.Width > 0)
            {
                this.comboBoxResolution.Items[this.comboBoxResolution.Items.Count - 1] = videoInfo.Width + "x" + videoInfo.Height;
                this.comboBoxResolution.SelectedIndex = this.comboBoxResolution.Items.Count - 1;
            }

            if (this._subtitleFontSize == Configuration.Settings.Tools.ExportLastFontSize && Configuration.Settings.Tools.ExportLastLineHeight >= this.numericUpDownLineSpacing.Minimum && Configuration.Settings.Tools.ExportLastLineHeight <= this.numericUpDownLineSpacing.Maximum && Configuration.Settings.Tools.ExportLastLineHeight > 0)
            {
                this.numericUpDownLineSpacing.Value = Configuration.Settings.Tools.ExportLastLineHeight;
            }

            if (Configuration.Settings.Tools.ExportLastBorderWidth >= 0 && Configuration.Settings.Tools.ExportLastBorderWidth < this.comboBoxBorderWidth.Items.Count)
            {
                try
                {
                    this.comboBoxBorderWidth.SelectedIndex = Configuration.Settings.Tools.ExportLastBorderWidth;
                }
                catch
                {
                }
            }

            this.checkBoxBold.Checked = Configuration.Settings.Tools.ExportLastFontBold;

            if (Configuration.Settings.Tools.Export3DType >= 0 && Configuration.Settings.Tools.Export3DType < this.comboBox3D.Items.Count)
            {
                this.comboBox3D.SelectedIndex = Configuration.Settings.Tools.Export3DType;
            }

            if (Configuration.Settings.Tools.Export3DDepth >= this.numericUpDownDepth3D.Minimum && Configuration.Settings.Tools.Export3DDepth <= this.numericUpDownDepth3D.Maximum)
            {
                this.numericUpDownDepth3D.Value = Configuration.Settings.Tools.Export3DDepth;
            }

            if (Configuration.Settings.Tools.ExportHorizontalAlignment >= 0 && Configuration.Settings.Tools.ExportHorizontalAlignment < this.comboBoxHAlign.Items.Count)
            {
                this.comboBoxHAlign.SelectedIndex = Configuration.Settings.Tools.ExportHorizontalAlignment;
            }

            if (exportType == "DCINEMA_INTEROP")
            {
                this.comboBox3D.Visible = false;
                this.numericUpDownDepth3D.Enabled = true;
                this.labelDepth.Enabled = true;
                this.labelDepth.Text = Configuration.Settings.Language.DCinemaProperties.ZPosition;
            }

            if (this._exportType == "FCP")
            {
                this.comboBoxResolution.Items.Clear();
                this.comboBoxResolution.Items.Add("NTSC-601");
                this.comboBoxResolution.Items.Add("PAL-601");
                this.comboBoxResolution.Items.Add("square");
                this.comboBoxResolution.Items.Add("DVCPROHD-720P");
                this.comboBoxResolution.Items.Add("HD-(960x720)");
                this.comboBoxResolution.Items.Add("DVCPROHD-1080i60");
                this.comboBoxResolution.Items.Add("HD-(1280x1080)");
                this.comboBoxResolution.Items.Add("DVCPROHD-1080i50");
                this.comboBoxResolution.Items.Add("HD-(1440x1080)");
                this.comboBoxResolution.SelectedIndex = 3; // 720p
                this.buttonCustomResolution.Visible = true; // we still allow for custom resolutions

                this.labelLanguage.Text = "NTSC/PAL";
                this.comboBoxLanguage.Items.Clear();
                this.comboBoxLanguage.Items.Add("PAL");
                this.comboBoxLanguage.Items.Add("NTSC");
                this.comboBoxLanguage.SelectedIndex = 0;
                this.comboBoxLanguage.Visible = true;
                this.labelLanguage.Visible = true;
            }

            this.comboBoxShadowWidth.SelectedIndex = 0;
            bool shadowVisible = this._exportType == "BDNXML" || this._exportType == "BLURAYSUP" || this._exportType == "DOST" || this._exportType == "IMAGE/FRAME" || this._exportType == "FCP" || this._exportType == "DCINEMA_INTEROP" || this._exportType == "EDL";
            this.labelShadowWidth.Visible = shadowVisible;
            this.buttonShadowColor.Visible = shadowVisible;
            this.comboBoxShadowWidth.Visible = shadowVisible;
            if (shadowVisible && Configuration.Settings.Tools.ExportBluRayShadow < this.comboBoxShadowWidth.Items.Count)
            {
                this.comboBoxShadowWidth.SelectedIndex = Configuration.Settings.Tools.ExportBluRayShadow;
            }

            this.panelShadowColor.Visible = shadowVisible;
            this.labelShadowTransparency.Visible = shadowVisible;
            this.numericUpDownShadowTransparency.Visible = shadowVisible;

            if (exportType == "BLURAYSUP" || exportType == "VOBSUB" || exportType == "BDNXML")
            {
                this.subtitleListView1.CheckBoxes = true;
                this.subtitleListView1.Columns.Insert(0, Configuration.Settings.Language.ExportPngXml.Forced);

                this.SubtitleListView1Fill(this._subtitle);

                if (this._vobSubOcr != null)
                {
                    for (int index = 0; index < this._subtitle.Paragraphs.Count; index++)
                    {
                        if (this._vobSubOcr.GetIsForced(index))
                        {
                            this.subtitleListView1.Items[index].Checked = true;
                        }
                    }
                }

                this.SubtitleListView1SelectIndexAndEnsureVisible(0);
            }
            else
            {
                this.SubtitleListView1Fill(this._subtitle);
                this.SubtitleListView1SelectIndexAndEnsureVisible(0);
            }
        }

        /// <summary>
        /// The init border style.
        /// </summary>
        private void InitBorderStyle()
        {
            this.comboBoxBorderWidth.Items.Clear();

            this.comboBoxBorderWidth.Items.Add(Configuration.Settings.Language.ExportPngXml.BorderStyleBoxForEachLine);
            this.comboBoxBorderWidth.Items.Add(Configuration.Settings.Language.ExportPngXml.BorderStyleOneBox);

            for (int i = 0; i < 16; i++)
            {
                this.comboBoxBorderWidth.Items.Add(string.Format(Configuration.Settings.Language.ExportPngXml.BorderStyleNormalWidthX, i));
            }

            this.comboBoxBorderWidth.SelectedIndex = 4;
        }

        /// <summary>
        /// The set last frame rate.
        /// </summary>
        /// <param name="lastFrameRate">
        /// The last frame rate.
        /// </param>
        private void SetLastFrameRate(double lastFrameRate)
        {
            for (int i = 0; i < this.comboBoxFrameRate.Items.Count; i++)
            {
                double d;
                if (double.TryParse(this.comboBoxFrameRate.Items[i].ToString().Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d))
                {
                    if (Math.Abs(lastFrameRate - d) < 0.01)
                    {
                        this.comboBoxFrameRate.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// The initialize from vob sub ocr.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="exportType">
        /// The export type.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="vobSubOcr">
        /// The vob sub ocr.
        /// </param>
        /// <param name="languageString">
        /// The language string.
        /// </param>
        internal void InitializeFromVobSubOcr(Subtitle subtitle, SubtitleFormat format, string exportType, string fileName, VobSubOcr vobSubOcr, string languageString)
        {
            this._vobSubOcr = vobSubOcr;
            this.Initialize(subtitle, format, exportType, fileName, null, this._videoFileName);

            // set language
            if (!string.IsNullOrEmpty(languageString))
            {
                if (languageString.Contains('(') && languageString[0] != '(')
                {
                    languageString = languageString.Substring(0, languageString.IndexOf('(') - 1).Trim();
                }

                for (int i = 0; i < this.comboBoxLanguage.Items.Count; i++)
                {
                    string l = this.comboBoxLanguage.Items[i].ToString();
                    if (l == languageString && i < this.comboBoxLanguage.Items.Count)
                    {
                        this.comboBoxLanguage.SelectedIndex = i;
                    }
                }
            }

            // Disable options not available when exporting existing images
            this.comboBoxSubtitleFont.Enabled = false;
            this.comboBoxSubtitleFontSize.Enabled = false;

            this.buttonColor.Visible = false;
            this.panelColor.Visible = false;
            this.checkBoxBold.Visible = false;
            this.checkBoxSimpleRender.Visible = false;
            this.comboBox3D.Enabled = false;
            this.numericUpDownDepth3D.Enabled = false;

            this.buttonBorderColor.Visible = false;
            this.panelBorderColor.Visible = false;
            this.labelBorderWidth.Visible = false;
            this.comboBoxBorderWidth.Visible = false;

            this.buttonShadowColor.Visible = false;
            this.panelShadowColor.Visible = false;
            this.labelShadowWidth.Visible = false;
            this.comboBoxShadowWidth.Visible = false;
            this.labelShadowTransparency.Visible = false;
            this.numericUpDownShadowTransparency.Visible = false;
            this.labelLineHeight.Visible = false;
            this.numericUpDownLineSpacing.Visible = false;
        }

        /// <summary>
        /// The subtitle list view 1_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._previewTimer.Stop();
            this._previewTimer.Start();
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            this.SetupImageParameters();
            if (this.subtitleListView1.SelectedItems.Count > 0)
            {
                MakeBitmapParameter mbp;
                var bmp = this.GenerateImageFromTextWithStyle(this._subtitle.Paragraphs[this.subtitleListView1.SelectedItems[0].Index], out mbp);
                if (this.checkBoxFullFrameImage.Visible && this.checkBoxFullFrameImage.Checked)
                {
                    var nbmp = new NikseBitmap(bmp);
                    nbmp.ReplaceTransparentWith(this.panelFullFrameBackground.BackColor);
                    bmp.Dispose();
                    bmp = nbmp.GetBitmap();
                }
                else
                {
                    this.groupBoxExportImage.BackColor = DefaultBackColor;
                }

                this.pictureBox1.Image = bmp;

                int w = this.groupBoxExportImage.Width - 4;
                this.pictureBox1.Width = bmp.Width;
                this.pictureBox1.Height = bmp.Height;
                this.pictureBox1.Top = this.groupBoxExportImage.Height - bmp.Height - int.Parse(this.comboBoxBottomMargin.Text);
                this.pictureBox1.Left = (w - bmp.Width) / 2;
                var alignment = GetAlignmentFromParagraph(mbp, this._format, this._subtitle);

                // fix alignment from UI
                if (this.comboBoxHAlign.Visible && alignment == ContentAlignment.BottomCenter && this._format.GetType() != typeof(AdvancedSubStationAlpha) && this._format.GetType() != typeof(SubStationAlpha))
                {
                    if (this.comboBoxHAlign.SelectedIndex == 0)
                    {
                        alignment = ContentAlignment.BottomLeft;
                    }
                    else if (this.comboBoxHAlign.SelectedIndex == 2)
                    {
                        alignment = ContentAlignment.BottomRight;
                    }
                }

                if (this.comboBoxHAlign.Visible)
                {
                    if (this.comboBoxLeftRightMargin.Visible)
                    {
                        if (alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.TopLeft)
                        {
                            this.pictureBox1.Left = int.Parse(this.comboBoxLeftRightMargin.Text);
                        }
                        else if (alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.MiddleRight || alignment == ContentAlignment.TopRight)
                        {
                            this.pictureBox1.Left = w - bmp.Width - int.Parse(this.comboBoxLeftRightMargin.Text);
                        }
                    }

                    if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.MiddleCenter || alignment == ContentAlignment.MiddleRight)
                    {
                        this.pictureBox1.Top = (this.groupBoxExportImage.Height - 4 - bmp.Height) / 2;
                    }
                    else if (this.comboBoxBottomMargin.Visible && alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.TopRight)
                    {
                        this.pictureBox1.Top = int.Parse(this.comboBoxBottomMargin.Text);
                    }
                }

                if (bmp.Width > this.groupBoxExportImage.Width + 20 || bmp.Height > this.groupBoxExportImage.Height + 20)
                {
                    this.pictureBox1.Left = 5;
                    this.pictureBox1.Top = 20;
                    this.pictureBox1.Width = this.groupBoxExportImage.Width - 10;
                    this.pictureBox1.Height = this.groupBoxExportImage.Height - 30;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                }

                this.groupBoxExportImage.Text = string.Format("{0}x{1}", bmp.Width, bmp.Height);
                if (!string.IsNullOrEmpty(mbp.Error))
                {
                    this.groupBoxExportImage.BackColor = Color.Red;
                    this.groupBoxExportImage.Text = this.groupBoxExportImage.Text + " - " + mbp.Error;
                }
                else
                {
                    if (this.checkBoxFullFrameImage.Visible && this.checkBoxFullFrameImage.Checked)
                    {
                        this.groupBoxExportImage.BackColor = this.panelFullFrameBackground.BackColor;
                    }
                    else
                    {
                        this.groupBoxExportImage.BackColor = this.groupBoxImageSettings.BackColor;
                    }
                }
            }
        }

        /// <summary>
        /// The button color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonColor_Click(object sender, EventArgs e)
        {
            bool showAlpha = this._exportType == "FAB" || this._exportType == "BDNXML";
            using (var colorChooser = new ColorChooser { Color = this.panelColor.BackColor, ShowAlpha = showAlpha })
            {
                if (colorChooser.ShowDialog() == DialogResult.OK)
                {
                    this.panelColor.BackColor = colorChooser.Color;
                    this.subtitleListView1_SelectedIndexChanged(null, null);
                }
            }
        }

        /// <summary>
        /// The panel color_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelColor_MouseClick(object sender, MouseEventArgs e)
        {
            this.buttonColor_Click(null, null);
        }

        /// <summary>
        /// The button border color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonBorderColor_Click(object sender, EventArgs e)
        {
            using (var colorChooser = new ColorChooser { Color = this.panelBorderColor.BackColor })
            {
                if (colorChooser.ShowDialog() == DialogResult.OK)
                {
                    this.panelBorderColor.BackColor = colorChooser.Color;
                    this.subtitleListView1_SelectedIndexChanged(null, null);
                }
            }
        }

        /// <summary>
        /// The panel border color_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelBorderColor_MouseClick(object sender, MouseEventArgs e)
        {
            this.buttonBorderColor_Click(null, null);
        }

        /// <summary>
        /// The combo box subtitle font_ selected value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxSubtitleFont_SelectedValueChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The combo box subtitle font size_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxSubtitleFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                var mbp = new MakeBitmapParameter();
                mbp.SubtitleFontName = this._subtitleFontName;
                mbp.SubtitleFontSize = float.Parse(this.comboBoxSubtitleFontSize.SelectedItem.ToString());
                mbp.SubtitleFontBold = this._subtitleFontBold;
                var fontSize = g.DpiY * mbp.SubtitleFontSize / 72;
                Font font = SetFont(mbp, fontSize);

                SizeF textSize = g.MeasureString("Hj!", font);
                int lineHeight = (int)Math.Round(textSize.Height * 0.64f);
                if (lineHeight >= this.numericUpDownLineSpacing.Minimum && lineHeight <= this.numericUpDownLineSpacing.Maximum && lineHeight != this.numericUpDownLineSpacing.Value)
                {
                    this.numericUpDownLineSpacing.Value = lineHeight;
                }
                else if (lineHeight > this.numericUpDownLineSpacing.Maximum)
                {
                    this.numericUpDownLineSpacing.Value = this.numericUpDownLineSpacing.Maximum;
                }
            }

            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The combo box border width_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxBorderWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The check box anti alias_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxAntiAlias_CheckedChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The export png xml_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXml_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#export");
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G && this.subtitleListView1.Items.Count > 1)
            {
                using (var goToLine = new GoToLine())
                {
                    goToLine.Initialize(1, this.subtitleListView1.Items.Count);
                    if (goToLine.ShowDialog(this) == DialogResult.OK)
                    {
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].Selected = true;
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].EnsureVisible();
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].Focused = true;
                    }
                }
            }
        }

        /// <summary>
        /// The export png xml_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXml_Shown(object sender, EventArgs e)
        {
            this._isLoading = false;
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The combo box h align_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxHAlign_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The check box bold_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxBold_CheckedChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The button custom resolution_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonCustomResolution_Click(object sender, EventArgs e)
        {
            using (var cr = new ChooseResolution())
            {
                if (cr.ShowDialog(this) == DialogResult.OK)
                {
                    this.comboBoxResolution.Items[this.comboBoxResolution.Items.Count - 1] = cr.VideoWidth + "x" + cr.VideoHeight;
                    this.comboBoxResolution.SelectedIndex = this.comboBoxResolution.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// The export png xml_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXml_ResizeEnd(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
            this.subtitleListView1.Columns[this.subtitleListView1.Columns.Count - 1].Width = -2;
        }

        /// <summary>
        /// The combo box bottom margin_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxBottomMargin_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The combo box resolution_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The export png xml_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXml_SizeChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The export png xml_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportPngXml_FormClosing(object sender, FormClosingEventArgs e)
        {
            int width;
            int height;
            this.GetResolution(out width, out height);
            string res = string.Format("{0}x{1}", width, height);

            if (this._exportType == "VOBSUB")
            {
                Configuration.Settings.Tools.ExportVobSubFontName = this._subtitleFontName;
                Configuration.Settings.Tools.ExportVobSubFontSize = (int)this._subtitleFontSize;
                Configuration.Settings.Tools.ExportVobSubVideoResolution = res;
                Configuration.Settings.Tools.ExportVobSubLanguage = this.comboBoxLanguage.Text;
                Configuration.Settings.Tools.ExportVobSubSimpleRendering = this.checkBoxSimpleRender.Checked;
                Configuration.Settings.Tools.ExportVobAntiAliasingWithTransparency = this.checkBoxTransAntiAliase.Checked;
            }
            else if (this._exportType == "BLURAYSUP")
            {
                Configuration.Settings.Tools.ExportBluRayFontName = this._subtitleFontName;
                Configuration.Settings.Tools.ExportBluRayFontSize = (int)this._subtitleFontSize;
                Configuration.Settings.Tools.ExportBluRayVideoResolution = res;
            }
            else if (this._exportType == "BDNXML")
            {
                Configuration.Settings.Tools.ExportBdnXmlImageType = this.comboBoxImageFormat.SelectedItem.ToString();
            }
            else if (this._exportType == "FCP")
            {
                Configuration.Settings.Tools.ExportFcpFontName = this._subtitleFontName;
                Configuration.Settings.Tools.ExportFcpFontSize = (int)this._subtitleFontSize;
                if (this.comboBoxImageFormat.SelectedItem != null)
                {
                    Configuration.Settings.Tools.ExportFcpImageType = this.comboBoxImageFormat.SelectedItem.ToString();
                }
            }

            Configuration.Settings.Tools.ExportLastShadowTransparency = (int)this.numericUpDownShadowTransparency.Value;
            Configuration.Settings.Tools.ExportLastFrameRate = this.FrameRate;
            Configuration.Settings.Tools.ExportShadowColor = this.panelShadowColor.BackColor;
            Configuration.Settings.Tools.ExportFontColor = this._subtitleColor;
            Configuration.Settings.Tools.ExportBorderColor = this._borderColor;
            if (this._exportType == "BLURAYSUP" || this._exportType == "DOST")
            {
                Configuration.Settings.Tools.ExportBluRayBottomMargin = this.comboBoxBottomMargin.SelectedIndex;
            }
            else
            {
                Configuration.Settings.Tools.ExportBottomMargin = this.comboBoxBottomMargin.SelectedIndex;
            }

            Configuration.Settings.Tools.ExportHorizontalAlignment = this.comboBoxHAlign.SelectedIndex;
            Configuration.Settings.Tools.Export3DType = this.comboBox3D.SelectedIndex;
            Configuration.Settings.Tools.Export3DDepth = (int)this.numericUpDownDepth3D.Value;

            if (this.comboBoxShadowWidth.Visible)
            {
                Configuration.Settings.Tools.ExportBluRayShadow = this.comboBoxShadowWidth.SelectedIndex;
            }

            Configuration.Settings.Tools.ExportFontNameOther = this._subtitleFontName;
            Configuration.Settings.Tools.ExportLastFontSize = (int)this._subtitleFontSize;
            Configuration.Settings.Tools.ExportLastLineHeight = (int)this.numericUpDownLineSpacing.Value;
            Configuration.Settings.Tools.ExportLastBorderWidth = this.comboBoxBorderWidth.SelectedIndex;
            Configuration.Settings.Tools.ExportLastFontBold = this.checkBoxBold.Checked;
        }

        /// <summary>
        /// The numeric up down depth 3 d_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownDepth3D_ValueChanged(object sender, EventArgs e)
        {
            if (!this.timerPreview.Enabled)
            {
                this.timerPreview.Start();
            }
        }

        /// <summary>
        /// The combo box 3 d_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBox3D_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox3D.SelectedIndex == 0)
            {
                this.labelDepth.Enabled = false;
                this.numericUpDownDepth3D.Enabled = false;
            }
            else
            {
                this.labelDepth.Enabled = true;
                this.numericUpDownDepth3D.Enabled = true;
            }

            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The timer preview_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timerPreview_Tick(object sender, EventArgs e)
        {
            this.timerPreview.Stop();
            this.subtitleListView1_SelectedIndexChanged(null, null);
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
            if (this.subtitleListView1.SelectedItems.Count != 1)
            {
                return;
            }

            int selectedIndex = this.subtitleListView1.SelectedItems[0].Index;

            this.saveFileDialog1.Title = Configuration.Settings.Language.VobSubOcr.SaveSubtitleImageAs;
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.FileName = "Image" + selectedIndex;
            this.saveFileDialog1.Filter = "PNG image|*.png|BMP image|*.bmp|GIF image|*.gif|TIFF image|*.tiff";
            this.saveFileDialog1.FilterIndex = 0;

            DialogResult result = this.saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var bmp = this.pictureBox1.Image as Bitmap;
                if (bmp == null)
                {
                    MessageBox.Show("No image!");
                    return;
                }

                try
                {
                    if (this.saveFileDialog1.FilterIndex == 0)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, ImageFormat.Png);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 1)
                    {
                        bmp.Save(this.saveFileDialog1.FileName);
                    }
                    else if (this.saveFileDialog1.FilterIndex == 2)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, ImageFormat.Gif);
                    }
                    else
                    {
                        bmp.Save(this.saveFileDialog1.FileName, ImageFormat.Tiff);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        /// <summary>
        /// The button shadow color_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonShadowColor_Click(object sender, EventArgs e)
        {
            using (var colorChooser = new ColorChooser { Color = this.panelShadowColor.BackColor })
            {
                if (colorChooser.ShowDialog() == DialogResult.OK)
                {
                    this.panelShadowColor.BackColor = colorChooser.Color;
                    this.subtitleListView1_SelectedIndexChanged(null, null);
                    this.numericUpDownShadowTransparency.Value = colorChooser.Color.A;
                }
            }
        }

        /// <summary>
        /// The panel shadow color_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelShadowColor_MouseClick(object sender, MouseEventArgs e)
        {
            this.buttonShadowColor_Click(sender, e);
        }

        /// <summary>
        /// The combo box shadow width_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxShadowWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The numeric up down shadow transparency_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownShadowTransparency_ValueChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The combo box subtitle font_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxSubtitleFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            var bmp = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(bmp))
            {
                var mbp = new MakeBitmapParameter { SubtitleFontName = this._subtitleFontName, SubtitleFontSize = float.Parse(this.comboBoxSubtitleFontSize.SelectedItem.ToString()), SubtitleFontBold = this._subtitleFontBold };
                var fontSize = g.DpiY * mbp.SubtitleFontSize / 72;
                Font font = SetFont(mbp, fontSize);

                SizeF textSize = g.MeasureString("Hj!", font);
                int lineHeight = (int)Math.Round(textSize.Height * 0.64f);
                if (lineHeight >= this.numericUpDownLineSpacing.Minimum && lineHeight <= this.numericUpDownLineSpacing.Maximum && lineHeight != this.numericUpDownLineSpacing.Value)
                {
                    this.numericUpDownLineSpacing.Value = lineHeight;
                }
            }

            bmp.Dispose();
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The numeric up down line spacing_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownLineSpacing_ValueChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The list view toggle tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        private void ListViewToggleTag(string tag)
        {
            if (this._subtitle.Paragraphs.Count > 0 && this.subtitleListView1.SelectedItems.Count > 0)
            {
                var indexes = new List<int>();
                foreach (ListViewItem item in this.subtitleListView1.SelectedItems)
                {
                    indexes.Add(item.Index);
                }

                this.subtitleListView1.BeginUpdate();
                foreach (int i in indexes)
                {
                    if (tag == BoxMultiLineText)
                    {
                        this._subtitle.Paragraphs[i].Text = this._subtitle.Paragraphs[i].Text.Replace("<" + BoxSingleLineText + ">", string.Empty).Replace("</" + BoxSingleLineText + ">", string.Empty);
                    }
                    else if (tag == BoxSingleLineText)
                    {
                        this._subtitle.Paragraphs[i].Text = this._subtitle.Paragraphs[i].Text.Replace("<" + BoxMultiLineText + ">", string.Empty).Replace("</" + BoxMultiLineText + ">", string.Empty);
                    }

                    if (this._subtitle.Paragraphs[i].Text.Contains("<" + tag + ">"))
                    {
                        this._subtitle.Paragraphs[i].Text = this._subtitle.Paragraphs[i].Text.Replace("<" + tag + ">", string.Empty);
                        this._subtitle.Paragraphs[i].Text = this._subtitle.Paragraphs[i].Text.Replace("</" + tag + ">", string.Empty);
                    }
                    else
                    {
                        int indexOfEndBracket = this._subtitle.Paragraphs[i].Text.IndexOf('}');
                        if (this._subtitle.Paragraphs[i].Text.StartsWith("{\\") && indexOfEndBracket > 1 && indexOfEndBracket < 6)
                        {
                            this._subtitle.Paragraphs[i].Text = string.Format("{2}<{0}>{1}</{0}>", tag, this._subtitle.Paragraphs[i].Text.Remove(0, indexOfEndBracket + 1), this._subtitle.Paragraphs[i].Text.Substring(0, indexOfEndBracket + 1));
                        }
                        else
                        {
                            this._subtitle.Paragraphs[i].Text = string.Format("<{0}>{1}</{0}>", tag, this._subtitle.Paragraphs[i].Text);
                        }
                    }

                    this.SubtitleListView1SetText(i, this._subtitle.Paragraphs[i].Text);
                }

                this.subtitleListView1.EndUpdate();
            }
        }

        /// <summary>
        /// The box multi line tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void boxMultiLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ListViewToggleTag(BoxMultiLineText);
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The box single line tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void boxSingleLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ListViewToggleTag(BoxSingleLineText);
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The italic tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ListViewToggleTag("i");
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The normal tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._subtitle.Paragraphs.Count > 0 && this.subtitleListView1.SelectedItems.Count > 0)
            {
                bool isSsa = this._format.Name == SubStationAlpha.NameOfFormat || this._format.Name == AdvancedSubStationAlpha.NameOfFormat;

                foreach (ListViewItem item in this.subtitleListView1.SelectedItems)
                {
                    Paragraph p = this._subtitle.GetParagraphOrDefault(item.Index);
                    if (p != null)
                    {
                        int indexOfEndBracket = p.Text.IndexOf('}');
                        if (p.Text.StartsWith("{\\") && indexOfEndBracket > 1 && indexOfEndBracket < 6)
                        {
                            p.Text = p.Text.Remove(0, indexOfEndBracket + 1);
                        }

                        p.Text = HtmlUtil.RemoveHtmlTags(p.Text);
                        p.Text = p.Text.Replace("<" + BoxSingleLineText + ">", string.Empty).Replace("</" + BoxSingleLineText + ">", string.Empty);
                        p.Text = p.Text.Replace("<" + BoxMultiLineText + ">", string.Empty).Replace("</" + BoxMultiLineText + ">", string.Empty);

                        if (isSsa)
                        {
                            p.Text = RemoveSsaStyle(p.Text);
                        }

                        this.SubtitleListView1SetText(item.Index, p.Text);
                    }
                }
            }

            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The remove ssa style.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveSsaStyle(string text)
        {
            int indexOfBegin = text.IndexOf('{');
            while (indexOfBegin >= 0 && text.IndexOf('}') > indexOfBegin)
            {
                int indexOfEnd = text.IndexOf('}');
                text = text.Remove(indexOfBegin, (indexOfEnd - indexOfBegin) + 1);
                indexOfBegin = text.IndexOf('{');
            }

            return text;
        }

        /// <summary>
        /// The subtitle list view 1_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView1_KeyDown(object sender, KeyEventArgs e)
        {
            var italicShortCut = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainTextBoxItalic);
            if (e.KeyData == italicShortCut)
            {
                this.ListViewToggleTag("i");
                this.subtitleListView1_SelectedIndexChanged(null, null);
            }
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                // SelectAll
                this.subtitleListView1.BeginUpdate();
                foreach (ListViewItem item in this.subtitleListView1.Items)
                {
                    item.Selected = true;
                }

                this.subtitleListView1.EndUpdate();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
            {
                // SelectFirstSelectedItemOnly
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    bool skipFirst = true;
                    foreach (ListViewItem item in this.subtitleListView1.SelectedItems)
                    {
                        if (skipFirst)
                        {
                            skipFirst = false;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }

                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.I && e.Modifiers == (Keys.Control | Keys.Shift))
            {
                // InverseSelection
                this.subtitleListView1.BeginUpdate();
                foreach (ListViewItem item in this.subtitleListView1.Items)
                {
                    item.Selected = !item.Selected;
                }

                this.subtitleListView1.EndUpdate();
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The subtitle list view 1 select none.
        /// </summary>
        public void SubtitleListView1SelectNone()
        {
            foreach (ListViewItem item in this.subtitleListView1.SelectedItems)
            {
                item.Selected = false;
            }
        }

        /// <summary>
        /// The subtitle list view 1 select index and ensure visible.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        private void SubtitleListView1SelectIndexAndEnsureVisible(int index)
        {
            if (index < 0 || index >= this.subtitleListView1.Items.Count || this.subtitleListView1.Items.Count == 0)
            {
                return;
            }

            if (this.subtitleListView1.TopItem == null)
            {
                return;
            }

            int bottomIndex = this.subtitleListView1.TopItem.Index + ((this.Height - 25) / 16);
            int itemsBeforeAfterCount = ((bottomIndex - this.subtitleListView1.TopItem.Index) / 2) - 1;
            if (itemsBeforeAfterCount < 0)
            {
                itemsBeforeAfterCount = 1;
            }

            int beforeIndex = index - itemsBeforeAfterCount;
            if (beforeIndex < 0)
            {
                beforeIndex = 0;
            }

            int afterIndex = index + itemsBeforeAfterCount;
            if (afterIndex >= this.subtitleListView1.Items.Count)
            {
                afterIndex = this.subtitleListView1.Items.Count - 1;
            }

            this.SubtitleListView1SelectNone();
            if (this.subtitleListView1.TopItem.Index <= beforeIndex && bottomIndex > afterIndex)
            {
                this.subtitleListView1.Items[index].Selected = true;
                this.subtitleListView1.Items[index].EnsureVisible();
                return;
            }

            this.subtitleListView1.Items[beforeIndex].EnsureVisible();
            this.subtitleListView1.EnsureVisible(beforeIndex);
            this.subtitleListView1.Items[afterIndex].EnsureVisible();
            this.subtitleListView1.EnsureVisible(afterIndex);
            this.subtitleListView1.Items[index].Selected = true;
            this.subtitleListView1.Items[index].EnsureVisible();
        }

        /// <summary>
        /// The subtitle list view 1 add.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        private void SubtitleListView1Add(Paragraph paragraph)
        {
            var item = new ListViewItem(paragraph.Number.ToString(CultureInfo.InvariantCulture)) { Tag = paragraph };
            ListViewItem.ListViewSubItem subItem;

            if (this.subtitleListView1.CheckBoxes)
            {
                item.Text = string.Empty;
                subItem = new ListViewItem.ListViewSubItem(item, paragraph.Number.ToString(CultureInfo.InvariantCulture)) { Tag = paragraph };
                item.SubItems.Add(subItem);
            }

            if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
            {
                if (paragraph.StartTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.StartTime.ToHHMMSSFF());
                }

                item.SubItems.Add(subItem);

                if (paragraph.EndTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.EndTime.ToHHMMSSFF());
                }

                item.SubItems.Add(subItem);

                subItem = new ListViewItem.ListViewSubItem(item, string.Format("{0},{1:00}", paragraph.Duration.Seconds, SubtitleFormat.MillisecondsToFramesMaxFrameRate(paragraph.Duration.Milliseconds)));
                item.SubItems.Add(subItem);
            }
            else
            {
                if (paragraph.StartTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.StartTime.ToString());
                }

                item.SubItems.Add(subItem);

                if (paragraph.EndTime.IsMaxTime)
                {
                    subItem = new ListViewItem.ListViewSubItem(item, "-");
                }
                else
                {
                    subItem = new ListViewItem.ListViewSubItem(item, paragraph.EndTime.ToString());
                }

                item.SubItems.Add(subItem);

                subItem = new ListViewItem.ListViewSubItem(item, string.Format("{0},{1:000}", paragraph.Duration.Seconds, paragraph.Duration.Milliseconds));
                item.SubItems.Add(subItem);
            }

            subItem = new ListViewItem.ListViewSubItem(item, paragraph.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            subItem.Font = new Font(this._subtitleFontName, this.Font.Size);

            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(subItem);

            this.subtitleListView1.Items.Add(item);
        }

        /// <summary>
        /// The subtitle list view 1 fill.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        private void SubtitleListView1Fill(Subtitle subtitle)
        {
            this.subtitleListView1.BeginUpdate();
            this.subtitleListView1.Items.Clear();
            foreach (Paragraph paragraph in subtitle.Paragraphs)
            {
                this.SubtitleListView1Add(paragraph);
            }

            this.subtitleListView1.EndUpdate();
        }

        /// <summary>
        /// The subtitle list view 1 auto size all columns.
        /// </summary>
        private void SubtitleListView1AutoSizeAllColumns()
        {
            int columnIndexNumber = 0;
            int columnIndexStart = 1;
            int columnIndexEnd = 2;
            int columnIndexDuration = 3;
            int columnIndexText = 4;

            if (this.subtitleListView1.CheckBoxes)
            {
                this.subtitleListView1.Columns[0].Width = 60;
                columnIndexNumber++;
                columnIndexStart++;
                columnIndexEnd++;
                columnIndexDuration++;
                columnIndexText++;
            }

            var setings = Configuration.Settings;
            if (setings != null && setings.General.ListViewColumnsRememberSize && setings.General.ListViewNumberWidth > 1)
            {
                this.subtitleListView1.Columns[columnIndexNumber].Width = setings.General.ListViewNumberWidth;
            }
            else
            {
                this.subtitleListView1.Columns[columnIndexNumber].Width = 55;
            }

            this.subtitleListView1.Columns[columnIndexStart].Width = 90;
            this.subtitleListView1.Columns[columnIndexEnd].Width = 90;
            this.subtitleListView1.Columns[columnIndexDuration].Width = 60;
            this.subtitleListView1.Columns[columnIndexText].Width = -2;
        }

        /// <summary>
        /// The subtitle list view 1 initialize language.
        /// </summary>
        /// <param name="general">
        /// The general.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        private void SubtitleListView1InitializeLanguage(LanguageStructure.General general, Logic.Settings settings)
        {
            int columnIndexNumber = 0;
            int columnIndexStart = 1;
            int columnIndexEnd = 2;
            int columnIndexDuration = 3;
            int columnIndexText = 4;

            if (this.subtitleListView1.CheckBoxes)
            {
                columnIndexNumber++;
                columnIndexStart++;
                columnIndexEnd++;
                columnIndexDuration++;
                columnIndexText++;
            }

            this.subtitleListView1.Columns[columnIndexNumber].Text = general.NumberSymbol;
            this.subtitleListView1.Columns[columnIndexStart].Text = general.StartTime;
            this.subtitleListView1.Columns[columnIndexEnd].Text = general.EndTime;
            this.subtitleListView1.Columns[columnIndexDuration].Text = general.Duration;
            this.subtitleListView1.Columns[columnIndexText].Text = general.Text;
            this.subtitleListView1.ForeColor = settings.General.SubtitleFontColor;
            this.subtitleListView1.BackColor = settings.General.SubtitleBackgroundColor;
        }

        /// <summary>
        /// The subtitle list view 1 set text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void SubtitleListView1SetText(int index, string text)
        {
            int columnIndexText = 4;

            if (this.subtitleListView1.CheckBoxes)
            {
                columnIndexText++;
            }

            this.subtitleListView1.Items[index].SubItems[columnIndexText].Text = text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString);
        }

        /// <summary>
        /// The fill preview background.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        private void FillPreviewBackground(Bitmap bmp, Graphics g, Paragraph p)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this._videoFileName) && LibVlcDynamic.IsInstalled)
                {
                    using (var vlc = new LibVlcDynamic())
                    {
                        vlc.Initialize(this.panelVlcTemp, this._videoFileName, null, null);
                        Application.DoEvents();
                        vlc.Volume = 0;
                        vlc.Pause();
                        vlc.CurrentPosition = p.StartTime.TotalSeconds;
                        Application.DoEvents();
                        var fileName = Path.GetTempFileName() + ".bmp";
                        vlc.TakeSnapshot(fileName, (uint)bmp.Width, (uint)bmp.Height);
                        Application.DoEvents();
                        Thread.Sleep(200);
                        using (var tempBmp = new Bitmap(fileName))
                        {
                            g.DrawImageUnscaled(tempBmp, new Point(0, 0));
                        }
                    }

                    return;
                }
            }
            catch
            {
                // Was unable to grap screenshot via vlc
            }

            // Draw background with generated image
            var rect = new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1);
            using (var br = new LinearGradientBrush(rect, Color.Black, Color.Black, 0, false))
            {
                var cb = new ColorBlend { Positions = new[] { 0, 1 / 6f, 2 / 6f, 3 / 6f, 4 / 6f, 5 / 6f, 1 }, Colors = new[] { Color.Black, Color.Black, Color.White, Color.Black, Color.Black, Color.White, Color.Black } };
                br.InterpolationColors = cb;
                br.RotateTransform(0);
                g.FillRectangle(br, rect);
            }
        }

        /// <summary>
        /// The link label preview_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabelPreview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabelPreview.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                int width;
                int height;
                this.GetResolution(out width, out height);
                using (var bmp = new Bitmap(width, height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        var p = this._subtitle.Paragraphs[this.subtitleListView1.SelectedItems[0].Index];
                        this.FillPreviewBackground(bmp, g, p);

                        var nBmp = new NikseBitmap(this.pictureBox1.Image as Bitmap);
                        nBmp.CropSidesAndBottom(100, Color.Transparent, true);
                        using (var textBmp = nBmp.GetBitmap())
                        {
                            var bp = this.MakeMakeBitmapParameter(this.subtitleListView1.SelectedItems[0].Index, width, height);
                            var alignment = GetAlignmentFromParagraph(bp, this._format, this._subtitle);
                            if (this.comboBoxHAlign.Visible && alignment == ContentAlignment.BottomCenter && this._format.GetType() != typeof(AdvancedSubStationAlpha) && this._format.GetType() != typeof(SubStationAlpha))
                            {
                                if (this.comboBoxHAlign.SelectedIndex == 0)
                                {
                                    alignment = ContentAlignment.BottomLeft;
                                }
                                else if (this.comboBoxHAlign.SelectedIndex == 2)
                                {
                                    alignment = ContentAlignment.BottomRight;
                                }
                            }

                            int x = (bmp.Width - textBmp.Width) / 2;
                            if (alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.TopLeft)
                            {
                                x = int.Parse(this.comboBoxBottomMargin.Text);
                            }
                            else if (alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.MiddleRight || alignment == ContentAlignment.TopRight)
                            {
                                x = bmp.Width - textBmp.Width - int.Parse(this.comboBoxBottomMargin.Text);
                            }

                            int y = bmp.Height - textBmp.Height - int.Parse(this.comboBoxBottomMargin.Text);
                            if (alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.TopLeft)
                            {
                                x = int.Parse(this.comboBoxBottomMargin.Text);
                            }
                            else if (alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.MiddleRight || alignment == ContentAlignment.TopRight)
                            {
                                x = bmp.Width - textBmp.Width - int.Parse(this.comboBoxBottomMargin.Text);
                            }

                            if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.MiddleCenter || alignment == ContentAlignment.MiddleRight)
                            {
                                y = (this.groupBoxExportImage.Height - 4 - textBmp.Height) / 2;
                            }
                            else if (alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.TopRight)
                            {
                                y = int.Parse(this.comboBoxBottomMargin.Text);
                            }

                            g.DrawImageUnscaled(textBmp, new Point(x, y));
                        }
                    }

                    using (var form = new ExportPngXmlPreview(bmp))
                    {
                        this.Cursor = Cursors.Default;
                        form.ShowDialog(this);
                    }
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.linkLabelPreview.Enabled = true;
            }
        }

        /// <summary>
        /// The combo box left right margin_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxLeftRightMargin_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The panel full frame background_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelFullFrameBackground_Click(object sender, EventArgs e)
        {
            using (var colorChooser = new ColorChooser { Color = this.panelFullFrameBackground.BackColor, Text = Configuration.Settings.Language.ExportPngXml.ChooseBackgroundColor })
            {
                if (colorChooser.ShowDialog() == DialogResult.OK)
                {
                    this.panelFullFrameBackground.BackColor = colorChooser.Color;
                    this.subtitleListView1_SelectedIndexChanged(null, null);
                }
            }
        }

        /// <summary>
        /// The check box full frame image_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxFullFrameImage_CheckedChanged(object sender, EventArgs e)
        {
            this.subtitleListView1_SelectedIndexChanged(null, null);
            this.panelFullFrameBackground.Visible = this.checkBoxFullFrameImage.Checked;
        }

        /// <summary>
        /// The make bitmap parameter.
        /// </summary>
        private class MakeBitmapParameter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MakeBitmapParameter"/> class.
            /// </summary>
            public MakeBitmapParameter()
            {
                this.BackgroundColor = Color.Transparent;
            }

            /// <summary>
            /// Gets or sets the bitmap.
            /// </summary>
            public Bitmap Bitmap { get; set; }

            /// <summary>
            /// Gets or sets the p.
            /// </summary>
            public Paragraph P { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the subtitle color.
            /// </summary>
            public Color SubtitleColor { get; set; }

            /// <summary>
            /// Gets or sets the subtitle font name.
            /// </summary>
            public string SubtitleFontName { get; set; }

            /// <summary>
            /// Gets or sets the subtitle font size.
            /// </summary>
            public float SubtitleFontSize { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether subtitle font bold.
            /// </summary>
            public bool SubtitleFontBold { get; set; }

            /// <summary>
            /// Gets or sets the border color.
            /// </summary>
            public Color BorderColor { get; set; }

            /// <summary>
            /// Gets or sets the border width.
            /// </summary>
            public float BorderWidth { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether box single line.
            /// </summary>
            public bool BoxSingleLine { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether simple rendering.
            /// </summary>
            public bool SimpleRendering { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether align left.
            /// </summary>
            public bool AlignLeft { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether align right.
            /// </summary>
            public bool AlignRight { get; set; }

            /// <summary>
            /// Gets or sets the buffer.
            /// </summary>
            public byte[] Buffer { get; set; }

            /// <summary>
            /// Gets or sets the screen width.
            /// </summary>
            public int ScreenWidth { get; set; }

            /// <summary>
            /// Gets or sets the screen height.
            /// </summary>
            public int ScreenHeight { get; set; }

            /// <summary>
            /// Gets or sets the video resolution.
            /// </summary>
            public string VideoResolution { get; set; }

            /// <summary>
            /// Gets or sets the type 3 d.
            /// </summary>
            public int Type3D { get; set; }

            /// <summary>
            /// Gets or sets the depth 3 d.
            /// </summary>
            public int Depth3D { get; set; }

            /// <summary>
            /// Gets or sets the frames per seconds.
            /// </summary>
            public double FramesPerSeconds { get; set; }

            /// <summary>
            /// Gets or sets the bottom margin.
            /// </summary>
            public int BottomMargin { get; set; }

            /// <summary>
            /// Gets or sets the left right margin.
            /// </summary>
            public int LeftRightMargin { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether saved.
            /// </summary>
            public bool Saved { get; set; }

            /// <summary>
            /// Gets or sets the alignment.
            /// </summary>
            public ContentAlignment Alignment { get; set; }

            /// <summary>
            /// Gets or sets the background color.
            /// </summary>
            public Color BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the sav dialog file name.
            /// </summary>
            public string SavDialogFileName { get; set; }

            /// <summary>
            /// Gets or sets the error.
            /// </summary>
            public string Error { get; set; }

            /// <summary>
            /// Gets or sets the line join.
            /// </summary>
            public string LineJoin { get; set; }

            /// <summary>
            /// Gets or sets the shadow color.
            /// </summary>
            public Color ShadowColor { get; set; }

            /// <summary>
            /// Gets or sets the shadow width.
            /// </summary>
            public int ShadowWidth { get; set; }

            /// <summary>
            /// Gets or sets the shadow alpha.
            /// </summary>
            public int ShadowAlpha { get; set; }

            /// <summary>
            /// Gets or sets the line height.
            /// </summary>
            public int LineHeight { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether forced.
            /// </summary>
            public bool Forced { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether full frame.
            /// </summary>
            public bool FullFrame { get; set; }

            /// <summary>
            /// Gets or sets the full frame backgroundcolor.
            /// </summary>
            public Color FullFrameBackgroundcolor { get; set; }
        }
    }
}