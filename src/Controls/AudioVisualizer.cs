// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioVisualizer.cs" company="">
//   
// </copyright>
// <summary>
//   The audio visualizer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The audio visualizer.
    /// </summary>
    public partial class AudioVisualizer : UserControl
    {
        /// <summary>
        /// The paragraph event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public delegate void ParagraphEventHandler(object sender, ParagraphEventArgs e);

        /// <summary>
        /// The minimum selection milliseconds.
        /// </summary>
        private const int MinimumSelectionMilliseconds = 100;

        /// <summary>
        /// The zoom minimum.
        /// </summary>
        public const double ZoomMinimum = 0.1;

        /// <summary>
        /// The zoom maximum.
        /// </summary>
        public const double ZoomMaximum = 2.5;

        /// <summary>
        /// The _button down time ticks.
        /// </summary>
        private long _buttonDownTimeTicks;

        /// <summary>
        /// The _current paragraph.
        /// </summary>
        private Paragraph _currentParagraph;

        /// <summary>
        /// The _current video position seconds.
        /// </summary>
        private double _currentVideoPositionSeconds = -1;

        /// <summary>
        /// The _first move.
        /// </summary>
        private bool _firstMove = true;

        /// <summary>
        /// The _gap at start.
        /// </summary>
        private double _gapAtStart = -1;

        /// <summary>
        /// The _image width.
        /// </summary>
        private double _imageWidth;

        /// <summary>
        /// The _mouse down.
        /// </summary>
        private bool _mouseDown;

        /// <summary>
        /// The _mouse down paragraph.
        /// </summary>
        private Paragraph _mouseDownParagraph;

        /// <summary>
        /// The _mouse down paragraph type.
        /// </summary>
        private MouseDownParagraphType _mouseDownParagraphType = MouseDownParagraphType.Start;

        /// <summary>
        /// The _mouse move end x.
        /// </summary>
        private int _mouseMoveEndX = -1;

        /// <summary>
        /// The _mouse move last x.
        /// </summary>
        private int _mouseMoveLastX = -1;

        /// <summary>
        /// The _mouse move start x.
        /// </summary>
        private int _mouseMoveStartX = -1;

        /// <summary>
        /// The _move whole start difference milliseconds.
        /// </summary>
        private double _moveWholeStartDifferenceMilliseconds = -1;

        /// <summary>
        /// The _next paragraph.
        /// </summary>
        private Paragraph _nextParagraph;

        /// <summary>
        /// The _nfft.
        /// </summary>
        private int _nfft;

        /// <summary>
        /// The _no clear.
        /// </summary>
        private bool _noClear;

        /// <summary>
        /// The _old paragraph.
        /// </summary>
        private Paragraph _oldParagraph;

        /// <summary>
        /// The _previous and next paragraphs.
        /// </summary>
        private List<Paragraph> _previousAndNextParagraphs = new List<Paragraph>();

        /// <summary>
        /// The _prev paragraph.
        /// </summary>
        private Paragraph _prevParagraph;

        /// <summary>
        /// The _sample duration.
        /// </summary>
        private double _sampleDuration;

        /// <summary>
        /// The _scene changes.
        /// </summary>
        private List<double> _sceneChanges = new List<double>();

        /// <summary>
        /// The _seconds per image.
        /// </summary>
        private double _secondsPerImage;

        /// <summary>
        /// The _selected indices.
        /// </summary>
        private ListView.SelectedIndexCollection _selectedIndices;

        /// <summary>
        /// The _selected paragraph.
        /// </summary>
        private Paragraph _selectedParagraph;

        /// <summary>
        /// The _spectrogram background worker.
        /// </summary>
        private BackgroundWorker _spectrogramBackgroundWorker;

        /// <summary>
        /// The _spectrogram bitmaps.
        /// </summary>
        private List<Bitmap> _spectrogramBitmaps = new List<Bitmap>();

        /// <summary>
        /// The _spectrogram directory.
        /// </summary>
        private string _spectrogramDirectory;

        /// <summary>
        /// The _start position seconds.
        /// </summary>
        private double _startPositionSeconds;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _temp show spectrogram.
        /// </summary>
        private bool _tempShowSpectrogram;

        /// <summary>
        /// The _wave peaks.
        /// </summary>
        private WavePeakGenerator _wavePeaks;

        /// <summary>
        /// The _whole paragraph max milliseconds.
        /// </summary>
        private double _wholeParagraphMaxMilliseconds = double.MaxValue;

        /// <summary>
        /// The _whole paragraph min milliseconds.
        /// </summary>
        private double _wholeParagraphMinMilliseconds;

        /// <summary>
        /// The _zoom factor.
        /// </summary>
        private double _zoomFactor = 1.0; // 1.0=no zoom

        /// <summary>
        /// The closeness for border selection.
        /// </summary>
        public int ClosenessForBorderSelection = 15;

        /// <summary>
        /// The insert at video position shortcut.
        /// </summary>
        public Keys InsertAtVideoPositionShortcut = Keys.None;

        /// <summary>
        /// The mouse wheel scroll up is forward.
        /// </summary>
        public bool MouseWheelScrollUpIsForward = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizer"/> class.
        /// </summary>
        public AudioVisualizer()
        {
            this.InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            this.WaveformNotLoadedText = "Click to add waveform/spectrogram";
            this.MouseWheel += this.WaveformMouseWheel;

            this.BackgroundColor = Color.Black;
            this.Color = Color.GreenYellow;
            this.SelectedColor = Color.Red;
            this.ParagraphColor = Color.LimeGreen;
            this.TextColor = Color.Gray;
            this.TextSize = 9;
            this.TextBold = true;
            this.GridColor = Color.FromArgb(255, 20, 20, 18);
            this.DrawGridLines = true;
            this.AllowNewSelection = true;
            this.ShowSpectrogram = true;
            this.ShowWaveform = true;
            this.VerticalZoomPercent = 1.0;
            this.InsertAtVideoPositionShortcut = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainWaveformInsertAtCurrentPosition);
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        public double ZoomFactor
        {
            get
            {
                return this._zoomFactor;
            }

            set
            {
                if (value < ZoomMinimum)
                {
                    this._zoomFactor = ZoomMinimum;
                }
                else if (value > ZoomMaximum)
                {
                    this._zoomFactor = ZoomMaximum;
                }
                else
                {
                    this._zoomFactor = value;
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// Scene changes (seconds)
        /// </summary>
        public List<double> SceneChanges
        {
            get
            {
                return this._sceneChanges;
            }

            set
            {
                this._sceneChanges = value;
            }
        }

        /// <summary>
        /// Video offset in seconds
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Gets a value indicating whether is spectrogram available.
        /// </summary>
        public bool IsSpectrogramAvailable
        {
            get
            {
                return this._spectrogramBitmaps != null && this._spectrogramBitmaps.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show spectrogram.
        /// </summary>
        public bool ShowSpectrogram { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow overlap.
        /// </summary>
        public bool AllowOverlap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show waveform.
        /// </summary>
        public bool ShowWaveform { get; set; }

        /// <summary>
        /// Gets or sets the start position seconds.
        /// </summary>
        public double StartPositionSeconds
        {
            get
            {
                return this._startPositionSeconds;
            }

            set
            {
                if (value < 0)
                {
                    this._startPositionSeconds = 0;
                }
                else
                {
                    this._startPositionSeconds = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the new selection paragraph.
        /// </summary>
        public Paragraph NewSelectionParagraph { get; set; }

        /// <summary>
        /// Gets the right clicked paragraph.
        /// </summary>
        public Paragraph RightClickedParagraph { get; private set; }

        /// <summary>
        /// Gets the right clicked seconds.
        /// </summary>
        public double RightClickedSeconds { get; private set; }

        /// <summary>
        /// Gets or sets the waveform not loaded text.
        /// </summary>
        public string WaveformNotLoadedText { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Gets or sets the paragraph color.
        /// </summary>
        public Color ParagraphColor { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Gets or sets the text size.
        /// </summary>
        public float TextSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether text bold.
        /// </summary>
        public bool TextBold { get; set; }

        /// <summary>
        /// Gets or sets the grid color.
        /// </summary>
        public Color GridColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether draw grid lines.
        /// </summary>
        public bool DrawGridLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow new selection.
        /// </summary>
        public bool AllowNewSelection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether locked.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the vertical zoom percent.
        /// </summary>
        public double VerticalZoomPercent { get; set; }

        /// <summary>
        /// Gets the end position seconds.
        /// </summary>
        public double EndPositionSeconds
        {
            get
            {
                return this.XPositionToSeconds(this.Width);
            }
        }

        /// <summary>
        /// Gets or sets the wave peaks.
        /// </summary>
        public WavePeakGenerator WavePeaks
        {
            get
            {
                return this._wavePeaks;
            }

            set
            {
                this._zoomFactor = 1.0;
                this._currentParagraph = null;
                this._selectedParagraph = null;
                this._buttonDownTimeTicks = 0;
                this._mouseMoveLastX = -1;
                this._mouseMoveStartX = -1;
                this._moveWholeStartDifferenceMilliseconds = -1;
                this._mouseMoveEndX = -1;
                this._mouseDown = false;
                this._mouseDownParagraph = null;
                this._mouseDownParagraphType = MouseDownParagraphType.Start;
                this._previousAndNextParagraphs = new List<Paragraph>();
                this._currentVideoPositionSeconds = -1;
                this._subtitle = null;
                this._noClear = false;
                this._wavePeaks = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether prevent overlap.
        /// </summary>
        private bool PreventOverlap
        {
            get
            {
                if (ModifierKeys == Keys.Shift)
                {
                    return this.AllowOverlap;
                }

                return !this.AllowOverlap;
            }
        }

        /// <summary>
        /// Gets a value indicating whether allow move prev or next.
        /// </summary>
        private bool AllowMovePrevOrNext
        {
            get
            {
                return this._gapAtStart >= 0 && this._gapAtStart < 500 && ModifierKeys == Keys.Alt;
            }
        }

        /// <summary>
        /// The on new selection right clicked.
        /// </summary>
        public event ParagraphEventHandler OnNewSelectionRightClicked;

        /// <summary>
        /// The on paragraph right clicked.
        /// </summary>
        public event ParagraphEventHandler OnParagraphRightClicked;

        /// <summary>
        /// The on non paragraph right clicked.
        /// </summary>
        public event ParagraphEventHandler OnNonParagraphRightClicked;

        /// <summary>
        /// The on position selected.
        /// </summary>
        public event ParagraphEventHandler OnPositionSelected;

        /// <summary>
        /// The on time changed.
        /// </summary>
        public event ParagraphEventHandler OnTimeChanged;

        /// <summary>
        /// The on time changed and offset rest.
        /// </summary>
        public event ParagraphEventHandler OnTimeChangedAndOffsetRest;

        /// <summary>
        /// The on single click.
        /// </summary>
        public event ParagraphEventHandler OnSingleClick;

        /// <summary>
        /// The on double click non paragraph.
        /// </summary>
        public event ParagraphEventHandler OnDoubleClickNonParagraph;

        /// <summary>
        /// The on pause.
        /// </summary>
        public event EventHandler OnPause;

        /// <summary>
        /// The on zoomed changed.
        /// </summary>
        public event EventHandler OnZoomedChanged;

        /// <summary>
        /// The insert at video position.
        /// </summary>
        public event EventHandler InsertAtVideoPosition;

        /// <summary>
        /// The reset spectrogram.
        /// </summary>
        public void ResetSpectrogram()
        {
            if (this._spectrogramBitmaps != null)
            {
                for (int i = 0; i < this._spectrogramBitmaps.Count; i++)
                {
                    try
                    {
                        Bitmap bmp = this._spectrogramBitmaps[i];
                        bmp.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            }

            this._spectrogramBitmaps = new List<Bitmap>();
        }

        /// <summary>
        /// The clear selection.
        /// </summary>
        public void ClearSelection()
        {
            this._mouseDown = false;
            this._mouseDownParagraph = null;
            this._mouseMoveStartX = -1;
            this._mouseMoveEndX = -1;
            this.Invalidate();
        }

        /// <summary>
        /// The nearest subtitles.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="currentVideoPositionSeconds">
        /// The current video position seconds.
        /// </param>
        /// <param name="subtitleIndex">
        /// The subtitle index.
        /// </param>
        public void NearestSubtitles(Subtitle subtitle, double currentVideoPositionSeconds, int subtitleIndex)
        {
            this._previousAndNextParagraphs.Clear();
            this._currentParagraph = null;

            var positionMilliseconds = (int)Math.Round(currentVideoPositionSeconds * TimeCode.BaseUnit);
            if (this._selectedParagraph != null && this._selectedParagraph.StartTime.TotalMilliseconds < positionMilliseconds && this._selectedParagraph.EndTime.TotalMilliseconds > positionMilliseconds)
            {
                this._currentParagraph = this._selectedParagraph;
                for (int j = 1; j < 12; j++)
                {
                    Paragraph nextParagraph = subtitle.GetParagraphOrDefault(subtitleIndex - j);
                    this._previousAndNextParagraphs.Add(nextParagraph);
                }

                for (int j = 1; j < 10; j++)
                {
                    Paragraph nextParagraph = subtitle.GetParagraphOrDefault(subtitleIndex + j);
                    this._previousAndNextParagraphs.Add(nextParagraph);
                }
            }
            else
            {
                for (int i = 0; i < subtitle.Paragraphs.Count; i++)
                {
                    Paragraph p = subtitle.Paragraphs[i];
                    if (p.EndTime.TotalMilliseconds > positionMilliseconds)
                    {
                        this._currentParagraph = p;
                        for (int j = 1; j < 10; j++)
                        {
                            Paragraph nextParagraph = subtitle.GetParagraphOrDefault(i - j);
                            this._previousAndNextParagraphs.Add(nextParagraph);
                        }

                        for (int j = 1; j < 10; j++)
                        {
                            Paragraph nextParagraph = subtitle.GetParagraphOrDefault(i + j);
                            this._previousAndNextParagraphs.Add(nextParagraph);
                        }

                        break;
                    }
                }

                if (this._previousAndNextParagraphs.Count == 0)
                {
                    for (int i = 0; i < subtitle.Paragraphs.Count; i++)
                    {
                        Paragraph p = subtitle.Paragraphs[i];
                        if (p.EndTime.TotalMilliseconds > this.StartPositionSeconds * 1000)
                        {
                            this._currentParagraph = p;
                            for (int j = 1; j < 10; j++)
                            {
                                Paragraph nextParagraph = subtitle.GetParagraphOrDefault(i - j);
                                this._previousAndNextParagraphs.Add(nextParagraph);
                            }

                            for (int j = 1; j < 10; j++)
                            {
                                Paragraph nextParagraph = subtitle.GetParagraphOrDefault(i + j);
                                this._previousAndNextParagraphs.Add(nextParagraph);
                            }

                            break;
                        }
                    }

                    if (this._previousAndNextParagraphs.Count == 0 && this._subtitle.Paragraphs.Count > 0)
                    {
                        int i = this._subtitle.Paragraphs.Count;
                        for (int j = 1; j < 10; j++)
                        {
                            Paragraph nextParagraph = subtitle.GetParagraphOrDefault(i - j);
                            this._previousAndNextParagraphs.Add(nextParagraph);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The set position.
        /// </summary>
        /// <param name="startPositionSeconds">
        /// The start position seconds.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="currentVideoPositionSeconds">
        /// The current video position seconds.
        /// </param>
        /// <param name="subtitleIndex">
        /// The subtitle index.
        /// </param>
        /// <param name="selectedIndexes">
        /// The selected indexes.
        /// </param>
        public void SetPosition(double startPositionSeconds, Subtitle subtitle, double currentVideoPositionSeconds, int subtitleIndex, ListView.SelectedIndexCollection selectedIndexes)
        {
            this.StartPositionSeconds = startPositionSeconds;
            this._selectedIndices = selectedIndexes;
            this._subtitle = new Subtitle();
            foreach (var p in subtitle.Paragraphs)
            {
                if (!p.StartTime.IsMaxTime)
                {
                    this._subtitle.Paragraphs.Add(p);
                }
            }

            this._currentVideoPositionSeconds = currentVideoPositionSeconds;
            this._selectedParagraph = this._subtitle.GetParagraphOrDefault(subtitleIndex);
            this.NearestSubtitles(subtitle, currentVideoPositionSeconds, subtitleIndex);
            this.Invalidate();
        }

        /// <summary>
        /// The calculate height.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="imageHeight">
        /// The image height.
        /// </param>
        /// <param name="maxHeight">
        /// The max height.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalculateHeight(double value, int imageHeight, int maxHeight)
        {
            double percentage = value / maxHeight;
            var result = (int)Math.Round((percentage * imageHeight) + (imageHeight / 2.0));
            return imageHeight - result;
        }

        /// <summary>
        /// The is selected index.
        /// </summary>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <param name="lastCurrentEnd">
        /// The last current end.
        /// </param>
        /// <param name="selectedParagraphs">
        /// The selected paragraphs.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsSelectedIndex(int pos, ref int lastCurrentEnd, List<Paragraph> selectedParagraphs)
        {
            if (pos < lastCurrentEnd)
            {
                return true;
            }

            if (this._selectedIndices == null || this._subtitle == null)
            {
                return false;
            }

            foreach (Paragraph p in selectedParagraphs)
            {
                if (pos >= p.StartFrame && pos <= p.EndFrame)
                {
                    // not really frames...
                    lastCurrentEnd = p.EndFrame;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The waveform paint.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        internal void WaveformPaint(object sender, PaintEventArgs e)
        {
            if (this._wavePeaks != null && this._wavePeaks.AllSamples != null)
            {
                var selectedParagraphs = new List<Paragraph>();
                if (this._selectedIndices != null)
                {
                    var n = this._wavePeaks.Header.SampleRate * this._zoomFactor;
                    try
                    {
                        foreach (int index in this._selectedIndices)
                        {
                            Paragraph p = this._subtitle.Paragraphs[index];
                            if (p != null)
                            {
                                p = new Paragraph(p);

                                // not really frames... just using them as position markers for better performance
                                p.StartFrame = (int)(p.StartTime.TotalSeconds * n);
                                p.EndFrame = (int)(p.EndTime.TotalSeconds * n);
                                selectedParagraphs.Add(p);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                // var otherParagraphs = new List<Paragraph>();
                // var nOther = _wavePeaks.Header.SampleRate * _zoomFactor;
                // try
                // {
                // foreach (Paragraph p in _subtitle.Paragraphs) //int index in _selectedIndices)
                // {
                // var p2 = new Paragraph(p) { StartFrame = (int)(p.StartTime.TotalSeconds*nOther), EndFrame = (int)(p.EndTime.TotalSeconds*nOther) };
                // // not really frames... just using them as position markers for better performance
                // otherParagraphs.Add(p2);
                // }
                // }
                // catch
                // {
                // }
                if (this.StartPositionSeconds < 0)
                {
                    this.StartPositionSeconds = 0;
                }

                if (this.XPositionToSeconds(this.Width) > this._wavePeaks.Header.LengthInSeconds)
                {
                    this.StartPositionSeconds = this._wavePeaks.Header.LengthInSeconds - ((this.Width / (double)this._wavePeaks.Header.SampleRate) / this._zoomFactor);
                }

                Graphics graphics = e.Graphics;
                int begin = this.SecondsToXPosition(this.StartPositionSeconds);
                var beginNoZoomFactor = (int)Math.Round(this.StartPositionSeconds * this._wavePeaks.Header.SampleRate); // do not use zoom factor here!

                int start = -1;
                int end = -1;
                if (this._selectedParagraph != null)
                {
                    start = this.SecondsToXPosition(this._selectedParagraph.StartTime.TotalSeconds);
                    end = this.SecondsToXPosition(this._selectedParagraph.EndTime.TotalSeconds);
                }

                int imageHeight = this.Height;
                var maxHeight = (int)(Math.Max(Math.Abs(this._wavePeaks.DataMinValue), Math.Abs(this._wavePeaks.DataMaxValue)) + 0.5);
                maxHeight = (int)(maxHeight * this.VerticalZoomPercent);
                if (maxHeight < 0)
                {
                    maxHeight = 1000;
                }

                this.DrawBackground(graphics);
                int x = 0;
                int y = this.Height / 2;

                if (this.IsSpectrogramAvailable && this.ShowSpectrogram)
                {
                    this.DrawSpectrogramBitmap(this.StartPositionSeconds, graphics);
                    if (this.ShowWaveform)
                    {
                        imageHeight -= this._nfft / 2;
                    }
                }

                // using (var penOther = new Pen(ParagraphColor))
                using (var penNormal = new Pen(this.Color))
                using (var penSelected = new Pen(this.SelectedColor))
                {
                    // selected paragraph
                    var pen = penNormal;
                    int lastCurrentEnd = -1;

                    // int lastOtherCurrentEnd = -1;
                    if (this.ShowWaveform)
                    {
                        if (this._zoomFactor > 0.9999 && this.ZoomFactor < 1.00001)
                        {
                            for (int i = 0; i < this._wavePeaks.AllSamples.Count && i < this.Width; i++)
                            {
                                int n = begin + i;
                                if (n < this._wavePeaks.AllSamples.Count)
                                {
                                    int newY = CalculateHeight(this._wavePeaks.AllSamples[n], imageHeight, maxHeight);

                                    // for (int tempX = x; tempX <= i; tempX++)
                                    // graphics.DrawLine(pen, tempX, y, tempX, newY);
                                    graphics.DrawLine(pen, x, y, i, newY);

                                    // graphics.FillRectangle(new SolidBrush(Color), x, y, 1, 1); // draw pixel instead of line
                                    x = i;
                                    y = newY;
                                    if (n <= end && n >= start)
                                    {
                                        pen = penSelected;
                                    }
                                    else if (this.IsSelectedIndex(n, ref lastCurrentEnd, selectedParagraphs))
                                    {
                                        pen = penSelected;
                                    }

                                    // else if (IsSelectedIndex(n, ref lastOtherCurrentEnd, otherParagraphs))
                                    // pen = penOther;
                                    else
                                    {
                                        pen = penNormal;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // calculate lines with zoom factor
                            float x2 = 0;
                            float x3 = 0;
                            for (int i = 0; i < this._wavePeaks.AllSamples.Count && ((int)Math.Round(x3)) < this.Width; i++)
                            {
                                if (beginNoZoomFactor + i < this._wavePeaks.AllSamples.Count)
                                {
                                    int newY = CalculateHeight(this._wavePeaks.AllSamples[beginNoZoomFactor + i], imageHeight, maxHeight);
                                    x3 = (float)(this._zoomFactor * i);
                                    graphics.DrawLine(pen, x2, y, x3, newY);
                                    x2 = x3;
                                    y = newY;
                                    var n = (int)(begin + x3);
                                    if (n <= end && n >= start)
                                    {
                                        pen = penSelected;
                                    }
                                    else if (this.IsSelectedIndex(n, ref lastCurrentEnd, selectedParagraphs))
                                    {
                                        pen = penSelected;
                                    }

                                    // else if (IsSelectedIndex(n, ref lastOtherCurrentEnd, otherParagraphs))
                                    // pen = penOther;
                                    else
                                    {
                                        pen = penNormal;
                                    }
                                }
                            }
                        }
                    }

                    this.DrawTimeLine(this.StartPositionSeconds, e, imageHeight);

                    // scene changes
                    if (this._sceneChanges != null)
                    {
                        foreach (var d in this._sceneChanges)
                        {
                            if (d > this.StartPositionSeconds && d < this.StartPositionSeconds + 20)
                            {
                                int pos = this.SecondsToXPosition(d) - begin;
                                if (pos > 0 && pos < this.Width)
                                {
                                    using (var p = new Pen(Color.AntiqueWhite))
                                    {
                                        graphics.DrawLine(p, pos, 0, pos, this.Height);
                                    }
                                }
                            }
                        }
                    }

                    // current video position
                    if (this._currentVideoPositionSeconds > 0)
                    {
                        int videoPosition = this.SecondsToXPosition(this._currentVideoPositionSeconds);
                        videoPosition -= begin;
                        if (videoPosition > 0 && videoPosition < this.Width)
                        {
                            using (var p = new Pen(Color.Turquoise))
                            {
                                graphics.DrawLine(p, videoPosition, 0, videoPosition, this.Height);
                            }
                        }
                    }

                    // mark paragraphs
                    using (var textBrush = new SolidBrush(this.TextColor))
                    {
                        this.DrawParagraph(this._currentParagraph, e, begin, textBrush);
                        foreach (Paragraph p in this._previousAndNextParagraphs)
                        {
                            this.DrawParagraph(p, e, begin, textBrush);
                        }
                    }

                    // current selection
                    if (this.NewSelectionParagraph != null)
                    {
                        int currentRegionLeft = this.SecondsToXPosition(this.NewSelectionParagraph.StartTime.TotalSeconds - this.StartPositionSeconds);
                        int currentRegionRight = this.SecondsToXPosition(this.NewSelectionParagraph.EndTime.TotalSeconds - this.StartPositionSeconds);

                        int currentRegionWidth = currentRegionRight - currentRegionLeft;
                        using (var brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
                        {
                            if (currentRegionLeft >= 0 && currentRegionLeft <= this.Width)
                            {
                                graphics.FillRectangle(brush, currentRegionLeft, 0, currentRegionWidth, graphics.VisibleClipBounds.Height);

                                if (currentRegionWidth > 40)
                                {
                                    using (var tBrush = new SolidBrush(Color.Turquoise))
                                    {
                                        graphics.DrawString(string.Format("{0:0.###} {1}", (double)currentRegionWidth / this._wavePeaks.Header.SampleRate / this._zoomFactor, Configuration.Settings.Language.Waveform.Seconds), this.Font, tBrush, new PointF(currentRegionLeft + 3, this.Height - 32));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.DrawBackground(e.Graphics);

                var textBrush = new SolidBrush(this.TextColor);
                var textFont = new Font(this.Font.FontFamily, 8);

                if (this.Width > 90)
                {
                    e.Graphics.DrawString(this.WaveformNotLoadedText, textFont, textBrush, new PointF(this.Width / 2 - 65, this.Height / 2 - 10));
                }
                else
                {
                    using (var stringFormat = new StringFormat())
                    {
                        stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                        e.Graphics.DrawString(this.WaveformNotLoadedText, textFont, textBrush, new PointF(1, 10), stringFormat);
                    }
                }

                textBrush.Dispose();
                textFont.Dispose();
            }

            if (this.Focused)
            {
                using (var p = new Pen(this.SelectedColor))
                {
                    e.Graphics.DrawRectangle(p, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                }
            }
        }

        /// <summary>
        /// The draw background.
        /// </summary>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        private void DrawBackground(Graphics graphics)
        {
            graphics.Clear(this.BackgroundColor);

            if (this.DrawGridLines)
            {
                if (this._wavePeaks == null)
                {
                    using (var pen = new Pen(new SolidBrush(this.GridColor)))
                    {
                        for (int i = 0; i < this.Width; i += 10)
                        {
                            graphics.DrawLine(pen, i, 0, i, this.Height);
                            graphics.DrawLine(pen, 0, i, this.Width, i);
                        }
                    }
                }
                else
                {
                    double interval = 0.1 * this._wavePeaks.Header.SampleRate * this._zoomFactor; // pixels that is 0.1 second
                    if (this.ZoomFactor < 0.4)
                    {
                        interval = 1.0 * this._wavePeaks.Header.SampleRate * this._zoomFactor; // pixels that is 1 second
                    }

                    int start = this.SecondsToXPosition(this.StartPositionSeconds) % ((int)Math.Round(interval));
                    using (var pen = new Pen(new SolidBrush(this.GridColor)))
                    {
                        for (double i = start; i < this.Width; i += interval)
                        {
                            var j = (int)Math.Round(i);
                            graphics.DrawLine(pen, j, 0, j, this.Height);
                            graphics.DrawLine(pen, 0, j, this.Width, j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The draw time line.
        /// </summary>
        /// <param name="startPositionSeconds">
        /// The start position seconds.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="imageHeight">
        /// The image height.
        /// </param>
        private void DrawTimeLine(double startPositionSeconds, PaintEventArgs e, int imageHeight)
        {
            var start = (int)Math.Round(startPositionSeconds + 0.5);
            double seconds = start - this.StartPositionSeconds;
            float position = this.SecondsToXPosition(seconds);
            var pen = new Pen(this.TextColor);
            var textBrush = new SolidBrush(this.TextColor);
            var textFont = new Font(this.Font.FontFamily, 7);
            while (position < this.Width)
            {
                var n = this._zoomFactor * this._wavePeaks.Header.SampleRate;
                if (n > 38 || (int)Math.Round(this.StartPositionSeconds + seconds) % 5 == 0)
                {
                    e.Graphics.DrawLine(pen, position, imageHeight, position, imageHeight - 10);
                    e.Graphics.DrawString(GetDisplayTime(this.StartPositionSeconds + seconds), textFont, textBrush, new PointF(position + 2, imageHeight - 13));
                }

                seconds += 0.5;
                position = this.SecondsToXPosition(seconds);

                if (n > 64)
                {
                    e.Graphics.DrawLine(pen, position, imageHeight, position, imageHeight - 5);
                }

                seconds += 0.5;
                position = this.SecondsToXPosition(seconds);
            }

            pen.Dispose();
            textBrush.Dispose();
        }

        /// <summary>
        /// The get display time.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetDisplayTime(double seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            if (ts.Minutes == 0 && ts.Hours == 0)
            {
                return ts.Seconds.ToString(CultureInfo.InvariantCulture);
            }

            if (ts.Hours == 0)
            {
                return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            }

            return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        /// <summary>
        /// The draw paragraph.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="begin">
        /// The begin.
        /// </param>
        /// <param name="textBrush">
        /// The text brush.
        /// </param>
        private void DrawParagraph(Paragraph paragraph, PaintEventArgs e, int begin, SolidBrush textBrush)
        {
            if (paragraph == null)
            {
                return;
            }

            int currentRegionLeft = this.SecondsToXPosition(paragraph.StartTime.TotalSeconds) - begin;
            int currentRegionRight = this.SecondsToXPosition(paragraph.EndTime.TotalSeconds) - begin;
            int currentRegionWidth = currentRegionRight - currentRegionLeft;
            var drawingStyle = this.TextBold ? FontStyle.Bold : FontStyle.Regular;
            using (var brush = new SolidBrush(Color.FromArgb(42, 255, 255, 255)))
            {
                // back color for paragraphs
                e.Graphics.FillRectangle(brush, currentRegionLeft, 0, currentRegionWidth, e.Graphics.VisibleClipBounds.Height);

                var pen = new Pen(new SolidBrush(Color.FromArgb(175, 0, 100, 0))) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid, Width = 2 };
                e.Graphics.DrawLine(pen, currentRegionLeft, 0, currentRegionLeft, e.Graphics.VisibleClipBounds.Height);
                pen.Dispose();
                pen = new Pen(new SolidBrush(Color.FromArgb(175, 110, 10, 10))) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash, Width = 2 };
                e.Graphics.DrawLine(pen, currentRegionRight - 1, 0, currentRegionRight - 1, e.Graphics.VisibleClipBounds.Height);

                var n = this._zoomFactor * this._wavePeaks.Header.SampleRate;
                if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    if (n > 80)
                    {
                        using (var font = new Font(this.Font.FontFamily, this.TextSize, drawingStyle))
                        {
                            e.Graphics.DrawString(paragraph.Text.Replace(Environment.NewLine, "  "), font, textBrush, new PointF(currentRegionLeft + 3, 10));
                            e.Graphics.DrawString("#" + paragraph.Number + "  " + paragraph.StartTime.ToShortStringHHMMSSFF() + " --> " + paragraph.EndTime.ToShortStringHHMMSSFF(), font, textBrush, new PointF(currentRegionLeft + 3, this.Height - 32));
                        }
                    }
                    else if (n > 51)
                    {
                        e.Graphics.DrawString("#" + paragraph.Number + "  " + paragraph.StartTime.ToShortStringHHMMSSFF(), this.Font, textBrush, new PointF(currentRegionLeft + 3, this.Height - 32));
                    }
                    else if (n > 25)
                    {
                        e.Graphics.DrawString("#" + paragraph.Number, this.Font, textBrush, new PointF(currentRegionLeft + 3, this.Height - 32));
                    }
                }
                else
                {
                    if (n > 80)
                    {
                        using (var font = new Font(Configuration.Settings.General.SubtitleFontName, this.TextSize, drawingStyle))
                        {
                            using (var blackBrush = new SolidBrush(Color.Black))
                            {
                                var text = HtmlUtil.RemoveHtmlTags(paragraph.Text, true);
                                text = text.Replace(Environment.NewLine, "  ");

                                int w = currentRegionRight - currentRegionLeft;
                                int actualWidth = (int)e.Graphics.MeasureString(text, font).Width;
                                bool shortned = false;
                                while (actualWidth > w - 12 && text.Length > 1)
                                {
                                    text = text.Remove(text.Length - 1);
                                    actualWidth = (int)e.Graphics.MeasureString(text, font).Width;
                                    shortned = true;
                                }

                                if (shortned)
                                {
                                    text = text.TrimEnd() + "…";
                                }

                                // poor mans outline + text
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 3, 11 - 7));
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 3, 9 - 7));
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 2, 10 - 7));
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 4, 10 - 7));
                                e.Graphics.DrawString(text, font, textBrush, new PointF(currentRegionLeft + 3, 10 - 7));

                                text = "#" + paragraph.Number + "  " + paragraph.Duration.ToShortString();
                                actualWidth = (int)e.Graphics.MeasureString(text, font).Width;
                                if (actualWidth >= w)
                                {
                                    text = paragraph.Duration.ToShortString();
                                }

                                int top = this.Height - 14 - (int)e.Graphics.MeasureString("#", font).Height;

                                // poor mans outline + text
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 3, top + 1));
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 3, top - 1));
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 2, top));
                                e.Graphics.DrawString(text, font, blackBrush, new PointF(currentRegionLeft + 4, top));
                                e.Graphics.DrawString(text, font, textBrush, new PointF(currentRegionLeft + 3, top));
                            }
                        }
                    }
                    else if (n > 51)
                    {
                        e.Graphics.DrawString("#" + paragraph.Number + "  " + paragraph.StartTime.ToShortString(), this.Font, textBrush, new PointF(currentRegionLeft + 3, this.Height - 32));
                    }
                    else if (n > 25)
                    {
                        e.Graphics.DrawString("#" + paragraph.Number, this.Font, textBrush, new PointF(currentRegionLeft + 3, this.Height - 32));
                    }
                }

                pen.Dispose();
            }
        }

        /// <summary>
        /// The x position to seconds.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private double XPositionToSeconds(double x)
        {
            return this.StartPositionSeconds + (x / this._wavePeaks.Header.SampleRate) / this._zoomFactor;
        }

        /// <summary>
        /// The seconds to x position.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int SecondsToXPosition(double seconds)
        {
            return (int)Math.Round(seconds * this._wavePeaks.Header.SampleRate * this._zoomFactor);
        }

        /// <summary>
        /// The waveform mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseDown(object sender, MouseEventArgs e)
        {
            if (this._wavePeaks == null)
            {
                return;
            }

            Paragraph oldMouseDownParagraph = null;
            this._mouseDownParagraphType = MouseDownParagraphType.None;
            this._gapAtStart = -1;
            this._firstMove = true;
            if (e.Button == MouseButtons.Left)
            {
                this._buttonDownTimeTicks = DateTime.Now.Ticks;

                this.Cursor = Cursors.VSplit;
                double seconds = this.XPositionToSeconds(e.X);
                var milliseconds = (int)(seconds * TimeCode.BaseUnit);

                if (this.SetParagrapBorderHit(milliseconds, this.NewSelectionParagraph))
                {
                    if (this._mouseDownParagraph != null)
                    {
                        oldMouseDownParagraph = new Paragraph(this._mouseDownParagraph);
                    }

                    if (this._mouseDownParagraphType == MouseDownParagraphType.Start)
                    {
                        this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                        this.NewSelectionParagraph.StartTime.TotalMilliseconds = milliseconds;
                        this._mouseMoveStartX = e.X;
                        this._mouseMoveEndX = this.SecondsToXPosition(this.NewSelectionParagraph.EndTime.TotalSeconds);
                    }
                    else
                    {
                        this._mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                        this.NewSelectionParagraph.EndTime.TotalMilliseconds = milliseconds;
                        this._mouseMoveStartX = this.SecondsToXPosition(this.NewSelectionParagraph.StartTime.TotalSeconds);
                        this._mouseMoveEndX = e.X;
                    }

                    this.SetMinMaxViaSeconds(seconds);
                }
                else if (this.SetParagrapBorderHit(milliseconds, this._selectedParagraph) || this.SetParagrapBorderHit(milliseconds, this._currentParagraph) || this.SetParagrapBorderHit(milliseconds, this._previousAndNextParagraphs))
                {
                    if (this._mouseDownParagraph != null)
                    {
                        oldMouseDownParagraph = new Paragraph(this._mouseDownParagraph);
                    }

                    this.NewSelectionParagraph = null;

                    int curIdx = this._subtitle.Paragraphs.IndexOf(this._mouseDownParagraph);
                    if (this._mouseDownParagraphType == MouseDownParagraphType.Start)
                    {
                        if (curIdx > 0)
                        {
                            var prev = this._subtitle.Paragraphs[curIdx - 1];
                            if (prev.EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines < milliseconds)
                            {
                                this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                            }
                        }
                        else
                        {
                            this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                        }
                    }
                    else
                    {
                        if (curIdx < this._subtitle.Paragraphs.Count - 1)
                        {
                            var next = this._subtitle.Paragraphs[curIdx + 1];
                            if (milliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines < next.StartTime.TotalMilliseconds)
                            {
                                this._mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                            }
                        }
                        else
                        {
                            this._mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                        }
                    }

                    this.SetMinAndMax();
                }
                else
                {
                    Paragraph p = this.GetParagraphAtMilliseconds(milliseconds);
                    if (p != null)
                    {
                        this._oldParagraph = new Paragraph(p);
                        this._mouseDownParagraph = p;
                        oldMouseDownParagraph = new Paragraph(this._mouseDownParagraph);
                        this._mouseDownParagraphType = MouseDownParagraphType.Whole;
                        this._moveWholeStartDifferenceMilliseconds = (this.XPositionToSeconds(e.X) * TimeCode.BaseUnit) - p.StartTime.TotalMilliseconds;
                        this.Cursor = Cursors.Hand;
                        this.SetMinAndMax();
                    }
                    else if (!this.AllowNewSelection)
                    {
                        this.Cursor = Cursors.Default;
                    }

                    if (p == null)
                    {
                        this.SetMinMaxViaSeconds(seconds);
                    }

                    this.NewSelectionParagraph = null;
                    this._mouseMoveStartX = e.X;
                    this._mouseMoveEndX = e.X;
                }

                if (this._mouseDownParagraphType == MouseDownParagraphType.Start)
                {
                    if (this._subtitle != null && this._mouseDownParagraph != null)
                    {
                        int curIdx = this._subtitle.Paragraphs.IndexOf(this._mouseDownParagraph);

                        // if (curIdx > 0)
                        // _gapAtStart = _subtitle.Paragraphs[curIdx].StartTime.TotalMilliseconds - _subtitle.Paragraphs[curIdx - 1].EndTime.TotalMilliseconds;
                        if (curIdx > 0)
                        {
                            this._gapAtStart = oldMouseDownParagraph.StartTime.TotalMilliseconds - this._subtitle.Paragraphs[curIdx - 1].EndTime.TotalMilliseconds;
                        }
                    }
                }
                else if (this._mouseDownParagraphType == MouseDownParagraphType.End)
                {
                    if (this._subtitle != null && this._mouseDownParagraph != null)
                    {
                        int curIdx = this._subtitle.Paragraphs.IndexOf(this._mouseDownParagraph);

                        // if (curIdx >= 0 && curIdx < _subtitle.Paragraphs.Count - 1)
                        // _gapAtStart = _subtitle.Paragraphs[curIdx + 1].StartTime.TotalMilliseconds - _subtitle.Paragraphs[curIdx].EndTime.TotalMilliseconds;
                        if (curIdx >= 0 && curIdx < this._subtitle.Paragraphs.Count - 1)
                        {
                            this._gapAtStart = this._subtitle.Paragraphs[curIdx + 1].StartTime.TotalMilliseconds - oldMouseDownParagraph.EndTime.TotalMilliseconds;
                        }
                    }
                }

                this._mouseDown = true;
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    double seconds = this.XPositionToSeconds(e.X);
                    var milliseconds = (int)(seconds * TimeCode.BaseUnit);

                    double currentRegionLeft = Math.Min(this._mouseMoveStartX, this._mouseMoveEndX);
                    double currentRegionRight = Math.Max(this._mouseMoveStartX, this._mouseMoveEndX);
                    currentRegionLeft = this.XPositionToSeconds(currentRegionLeft);
                    currentRegionRight = this.XPositionToSeconds(currentRegionRight);

                    if (this.OnNewSelectionRightClicked != null && seconds > currentRegionLeft && seconds < currentRegionRight)
                    {
                        if (this._mouseMoveStartX >= 0 && this._mouseMoveEndX >= 0)
                        {
                            if (currentRegionRight - currentRegionLeft > 0.1)
                            {
                                // not too small subtitles
                                var paragraph = new Paragraph { StartTime = TimeCode.FromSeconds(currentRegionLeft), EndTime = TimeCode.FromSeconds(currentRegionRight) };
                                if (this.PreventOverlap)
                                {
                                    if (paragraph.StartTime.TotalMilliseconds <= this._wholeParagraphMinMilliseconds)
                                    {
                                        paragraph.StartTime.TotalMilliseconds = this._wholeParagraphMinMilliseconds + 1;
                                    }

                                    if (paragraph.EndTime.TotalMilliseconds >= this._wholeParagraphMaxMilliseconds)
                                    {
                                        paragraph.EndTime.TotalMilliseconds = this._wholeParagraphMaxMilliseconds - 1;
                                    }
                                }

                                this.OnNewSelectionRightClicked.Invoke(this, new ParagraphEventArgs(paragraph));
                                this.NewSelectionParagraph = paragraph;
                                this.RightClickedParagraph = null;
                                this._noClear = true;
                            }
                        }
                    }
                    else
                    {
                        Paragraph p = this.GetParagraphAtMilliseconds(milliseconds);
                        this.RightClickedParagraph = p;
                        this.RightClickedSeconds = seconds;
                        if (p != null)
                        {
                            if (this.OnParagraphRightClicked != null)
                            {
                                this.NewSelectionParagraph = null;
                                this.OnParagraphRightClicked.Invoke(this, new ParagraphEventArgs(seconds, p));
                            }
                        }
                        else
                        {
                            if (this.OnNonParagraphRightClicked != null)
                            {
                                this.OnNonParagraphRightClicked.Invoke(this, new ParagraphEventArgs(seconds, null));
                            }
                        }
                    }
                }

                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// The set min max via seconds.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        private void SetMinMaxViaSeconds(double seconds)
        {
            this._wholeParagraphMinMilliseconds = 0;
            this._wholeParagraphMaxMilliseconds = double.MaxValue;
            if (this._subtitle != null)
            {
                Paragraph prev = null;
                Paragraph next = null;
                for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
                {
                    Paragraph p2 = this._subtitle.Paragraphs[i];
                    if (p2.StartTime.TotalSeconds < seconds)
                    {
                        prev = p2;
                    }
                    else if (p2.EndTime.TotalSeconds > seconds)
                    {
                        next = p2;
                        break;
                    }
                }

                if (prev != null)
                {
                    this._wholeParagraphMinMilliseconds = prev.EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                }

                if (next != null)
                {
                    this._wholeParagraphMaxMilliseconds = next.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                }
            }
        }

        /// <summary>
        /// The set min and max.
        /// </summary>
        private void SetMinAndMax()
        {
            this._wholeParagraphMinMilliseconds = 0;
            this._wholeParagraphMaxMilliseconds = double.MaxValue;
            if (this._subtitle != null && this._mouseDownParagraph != null)
            {
                int curIdx = this._subtitle.Paragraphs.IndexOf(this._mouseDownParagraph);
                if (curIdx >= 0)
                {
                    if (curIdx > 0)
                    {
                        this._wholeParagraphMinMilliseconds = this._subtitle.Paragraphs[curIdx - 1].EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                    }

                    if (curIdx < this._subtitle.Paragraphs.Count - 1)
                    {
                        this._wholeParagraphMaxMilliseconds = this._subtitle.Paragraphs[curIdx + 1].StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                    }
                }
            }
        }

        /// <summary>
        /// The set min and max move start.
        /// </summary>
        private void SetMinAndMaxMoveStart()
        {
            this._wholeParagraphMinMilliseconds = 0;
            this._wholeParagraphMaxMilliseconds = double.MaxValue;
            if (this._subtitle != null && this._mouseDownParagraph != null)
            {
                int curIdx = this._subtitle.Paragraphs.IndexOf(this._mouseDownParagraph);
                if (curIdx >= 0)
                {
                    var gap = Math.Abs(this._subtitle.Paragraphs[curIdx - 1].EndTime.TotalMilliseconds - this._subtitle.Paragraphs[curIdx].StartTime.TotalMilliseconds);
                    this._wholeParagraphMinMilliseconds = this._subtitle.Paragraphs[curIdx - 1].StartTime.TotalMilliseconds + gap + 200;
                }
            }
        }

        /// <summary>
        /// The set min and max move end.
        /// </summary>
        private void SetMinAndMaxMoveEnd()
        {
            this._wholeParagraphMinMilliseconds = 0;
            this._wholeParagraphMaxMilliseconds = double.MaxValue;
            if (this._subtitle != null && this._mouseDownParagraph != null)
            {
                int curIdx = this._subtitle.Paragraphs.IndexOf(this._mouseDownParagraph);
                if (curIdx >= 0)
                {
                    if (curIdx < this._subtitle.Paragraphs.Count - 1)
                    {
                        var gap = Math.Abs(this._subtitle.Paragraphs[curIdx].EndTime.TotalMilliseconds - this._subtitle.Paragraphs[curIdx + 1].StartTime.TotalMilliseconds);
                        this._wholeParagraphMaxMilliseconds = this._subtitle.Paragraphs[curIdx + 1].EndTime.TotalMilliseconds - gap - 200;
                    }
                }
            }
        }

        /// <summary>
        /// The set paragrap border hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SetParagrapBorderHit(int milliseconds, List<Paragraph> paragraphs)
        {
            foreach (Paragraph p in paragraphs)
            {
                bool hit = this.SetParagrapBorderHit(milliseconds, p);
                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The get paragraph at milliseconds.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        private Paragraph GetParagraphAtMilliseconds(int milliseconds)
        {
            Paragraph p = null;
            if (IsParagrapHit(milliseconds, this._selectedParagraph))
            {
                p = this._selectedParagraph;
            }
            else if (IsParagrapHit(milliseconds, this._currentParagraph))
            {
                p = this._currentParagraph;
            }

            if (p == null)
            {
                foreach (Paragraph pNext in this._previousAndNextParagraphs)
                {
                    if (IsParagrapHit(milliseconds, pNext))
                    {
                        p = pNext;
                        break;
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// The set paragrap border hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SetParagrapBorderHit(int milliseconds, Paragraph paragraph)
        {
            if (paragraph == null)
            {
                return false;
            }

            if (this.IsParagrapBorderStartHit(milliseconds, paragraph.StartTime.TotalMilliseconds))
            {
                this._oldParagraph = new Paragraph(paragraph);
                this._mouseDownParagraph = paragraph;
                this._mouseDownParagraphType = MouseDownParagraphType.Start;
                return true;
            }

            if (this.IsParagrapBorderEndHit(milliseconds, paragraph.EndTime.TotalMilliseconds))
            {
                this._oldParagraph = new Paragraph(paragraph);
                this._mouseDownParagraph = paragraph;
                this._mouseDownParagraphType = MouseDownParagraphType.End;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The waveform mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseMove(object sender, MouseEventArgs e)
        {
            if (this._wavePeaks == null)
            {
                return;
            }

            int oldMouseMoveLastX = this._mouseMoveLastX;
            if (e.X < 0 && this.StartPositionSeconds > 0.1 && this._mouseDown)
            {
                if (e.X < this._mouseMoveLastX)
                {
                    this.StartPositionSeconds -= 0.1;
                    if (this._mouseDownParagraph == null)
                    {
                        this._mouseMoveEndX = 0;
                        this._mouseMoveStartX += (int)(this._wavePeaks.Header.SampleRate * 0.1);
                        this.OnPositionSelected.Invoke(this, new ParagraphEventArgs(this.StartPositionSeconds, null));
                    }
                }

                this._mouseMoveLastX = e.X;
                this.Invalidate();
                return;
            }

            if (e.X > this.Width && this.StartPositionSeconds + 0.1 < this._wavePeaks.Header.LengthInSeconds && this._mouseDown)
            {
                {
                    // if (e.X > _mouseMoveLastX) // not much room for moving mouse cursor, so just scroll right
                    this.StartPositionSeconds += 0.1;
                    if (this._mouseDownParagraph == null)
                    {
                        this._mouseMoveEndX = this.Width;
                        this._mouseMoveStartX -= (int)(this._wavePeaks.Header.SampleRate * 0.1);
                        this.OnPositionSelected.Invoke(this, new ParagraphEventArgs(this.StartPositionSeconds, null));
                    }
                }

                this._mouseMoveLastX = e.X;
                this.Invalidate();
                return;
            }

            this._mouseMoveLastX = e.X;

            if (e.X < 0 || e.X > this.Width)
            {
                return;
            }

            if (e.Button == MouseButtons.None)
            {
                double seconds = this.XPositionToSeconds(e.X);
                var milliseconds = (int)(seconds * TimeCode.BaseUnit);

                if (this.IsParagrapBorderHit(milliseconds, this.NewSelectionParagraph))
                {
                    this.Cursor = Cursors.VSplit;
                }
                else if (this.IsParagrapBorderHit(milliseconds, this._selectedParagraph) || this.IsParagrapBorderHit(milliseconds, this._currentParagraph) || this.IsParagrapBorderHit(milliseconds, this._previousAndNextParagraphs))
                {
                    this.Cursor = Cursors.VSplit;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (oldMouseMoveLastX == e.X)
                {
                    return; // no horizontal movement
                }

                if (this._mouseDown)
                {
                    if (this._mouseDownParagraph != null)
                    {
                        double seconds = this.XPositionToSeconds(e.X);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        var subtitleIndex = this._subtitle.GetIndex(this._mouseDownParagraph);
                        this._prevParagraph = this._subtitle.GetParagraphOrDefault(subtitleIndex - 1);
                        this._nextParagraph = this._subtitle.GetParagraphOrDefault(subtitleIndex + 1);

                        if (this._firstMove && Math.Abs(oldMouseMoveLastX - e.X) < Configuration.Settings.General.MinimumMillisecondsBetweenLines && this.GetParagraphAtMilliseconds(milliseconds) == null)
                        {
                            if (this._mouseDownParagraphType == MouseDownParagraphType.Start && this._prevParagraph != null && Math.Abs(this._mouseDownParagraph.StartTime.TotalMilliseconds - this._prevParagraph.EndTime.TotalMilliseconds) <= this.ClosenessForBorderSelection + 15)
                            {
                                return; // do not decide which paragraph to move yet
                            }

                            if (this._mouseDownParagraphType == MouseDownParagraphType.End && this._nextParagraph != null && Math.Abs(this._mouseDownParagraph.EndTime.TotalMilliseconds - this._nextParagraph.StartTime.TotalMilliseconds) <= this.ClosenessForBorderSelection + 15)
                            {
                                return; // do not decide which paragraph to move yet
                            }
                        }

                        if (ModifierKeys != Keys.Alt)
                        {
                            // decide which paragraph to move
                            if (this._firstMove && e.X > oldMouseMoveLastX && this._nextParagraph != null && this._mouseDownParagraphType == MouseDownParagraphType.End)
                            {
                                if (milliseconds >= this._nextParagraph.StartTime.TotalMilliseconds && milliseconds < this._nextParagraph.EndTime.TotalMilliseconds)
                                {
                                    this._mouseDownParagraph = this._nextParagraph;
                                    this._mouseDownParagraphType = MouseDownParagraphType.Start;
                                }
                            }
                            else if (this._firstMove && e.X < oldMouseMoveLastX && this._prevParagraph != null && this._mouseDownParagraphType == MouseDownParagraphType.Start)
                            {
                                if (milliseconds <= this._prevParagraph.EndTime.TotalMilliseconds && milliseconds > this._prevParagraph.StartTime.TotalMilliseconds)
                                {
                                    this._mouseDownParagraph = this._prevParagraph;
                                    this._mouseDownParagraphType = MouseDownParagraphType.End;
                                }
                            }
                        }

                        this._firstMove = false;

                        if (this._mouseDownParagraphType == MouseDownParagraphType.Start)
                        {
                            if (this._mouseDownParagraph.EndTime.TotalMilliseconds - milliseconds > MinimumSelectionMilliseconds)
                            {
                                if (this.AllowMovePrevOrNext)
                                {
                                    this.SetMinAndMaxMoveStart();
                                }
                                else
                                {
                                    this.SetMinAndMax();
                                }

                                this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                                if (this.PreventOverlap && this._mouseDownParagraph.StartTime.TotalMilliseconds <= this._wholeParagraphMinMilliseconds)
                                {
                                    this._mouseDownParagraph.StartTime.TotalMilliseconds = this._wholeParagraphMinMilliseconds + 1;
                                }

                                if (this.NewSelectionParagraph != null)
                                {
                                    this.NewSelectionParagraph.StartTime.TotalMilliseconds = milliseconds;
                                    if (this.PreventOverlap && this.NewSelectionParagraph.StartTime.TotalMilliseconds <= this._wholeParagraphMinMilliseconds)
                                    {
                                        this.NewSelectionParagraph.StartTime.TotalMilliseconds = this._wholeParagraphMinMilliseconds + 1;
                                    }

                                    this._mouseMoveStartX = e.X;
                                }
                                else
                                {
                                    if (this.OnTimeChanged != null)
                                    {
                                        this.OnTimeChanged.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph, this._oldParagraph, this._mouseDownParagraphType, this.AllowMovePrevOrNext));
                                    }

                                    this.Refresh();
                                    return;
                                }
                            }
                        }
                        else if (this._mouseDownParagraphType == MouseDownParagraphType.End)
                        {
                            if (milliseconds - this._mouseDownParagraph.StartTime.TotalMilliseconds > MinimumSelectionMilliseconds)
                            {
                                if (this.AllowMovePrevOrNext)
                                {
                                    this.SetMinAndMaxMoveEnd();
                                }
                                else
                                {
                                    this.SetMinAndMax();
                                }

                                this._mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                                if (this.PreventOverlap && this._mouseDownParagraph.EndTime.TotalMilliseconds >= this._wholeParagraphMaxMilliseconds)
                                {
                                    this._mouseDownParagraph.EndTime.TotalMilliseconds = this._wholeParagraphMaxMilliseconds - 1;
                                }

                                if (this.NewSelectionParagraph != null)
                                {
                                    this.NewSelectionParagraph.EndTime.TotalMilliseconds = milliseconds;
                                    if (this.PreventOverlap && this.NewSelectionParagraph.EndTime.TotalMilliseconds >= this._wholeParagraphMaxMilliseconds)
                                    {
                                        this.NewSelectionParagraph.EndTime.TotalMilliseconds = this._wholeParagraphMaxMilliseconds - 1;
                                    }

                                    this._mouseMoveEndX = e.X;
                                }
                                else
                                {
                                    // SHOW DEBUG MSG                     SolidBrush tBrush = new SolidBrush(Color.Turquoise);
                                    // var g = this.CreateGraphics();
                                    // g.DrawString("AllowMovePrevOrNext: " + AllowMovePrevOrNext.ToString() + ", GapStart: " + _gapAtStart.ToString(), Font, tBrush, new PointF(100, 100));
                                    // tBrush.Dispose();
                                    // g.Dispose();
                                    if (this.OnTimeChanged != null)
                                    {
                                        this.OnTimeChanged.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph, this._oldParagraph, this._mouseDownParagraphType, this.AllowMovePrevOrNext));
                                    }

                                    this.Refresh();
                                    return;
                                }
                            }
                        }
                        else if (this._mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            double durationMilliseconds = this._mouseDownParagraph.Duration.TotalMilliseconds;

                            this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds - this._moveWholeStartDifferenceMilliseconds;
                            this._mouseDownParagraph.EndTime.TotalMilliseconds = this._mouseDownParagraph.StartTime.TotalMilliseconds + durationMilliseconds;

                            if (this.PreventOverlap && this._mouseDownParagraph.EndTime.TotalMilliseconds >= this._wholeParagraphMaxMilliseconds)
                            {
                                this._mouseDownParagraph.EndTime.TotalMilliseconds = this._wholeParagraphMaxMilliseconds - 1;
                                this._mouseDownParagraph.StartTime.TotalMilliseconds = this._mouseDownParagraph.EndTime.TotalMilliseconds - durationMilliseconds;
                            }
                            else if (this.PreventOverlap && this._mouseDownParagraph.StartTime.TotalMilliseconds <= this._wholeParagraphMinMilliseconds)
                            {
                                this._mouseDownParagraph.StartTime.TotalMilliseconds = this._wholeParagraphMinMilliseconds + 1;
                                this._mouseDownParagraph.EndTime.TotalMilliseconds = this._mouseDownParagraph.StartTime.TotalMilliseconds + durationMilliseconds;
                            }

                            if (this.OnTimeChanged != null)
                            {
                                this.OnTimeChanged.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph, this._oldParagraph));
                            }
                        }
                    }
                    else
                    {
                        this._mouseMoveEndX = e.X;
                        if (this.NewSelectionParagraph == null && Math.Abs(this._mouseMoveEndX - this._mouseMoveStartX) > 2)
                        {
                            if (this.AllowNewSelection)
                            {
                                this.NewSelectionParagraph = new Paragraph();
                            }
                        }

                        if (this.NewSelectionParagraph != null)
                        {
                            int start = Math.Min(this._mouseMoveStartX, this._mouseMoveEndX);
                            int end = Math.Max(this._mouseMoveStartX, this._mouseMoveEndX);

                            var startTotalSeconds = this.XPositionToSeconds(start);
                            var endTotalSeconds = this.XPositionToSeconds(end);

                            if (this.PreventOverlap && endTotalSeconds * TimeCode.BaseUnit >= this._wholeParagraphMaxMilliseconds)
                            {
                                this.NewSelectionParagraph.EndTime.TotalMilliseconds = this._wholeParagraphMaxMilliseconds - 1;
                                this.Invalidate();
                                return;
                            }

                            if (this.PreventOverlap && startTotalSeconds * TimeCode.BaseUnit <= this._wholeParagraphMinMilliseconds)
                            {
                                this.NewSelectionParagraph.StartTime.TotalMilliseconds = this._wholeParagraphMinMilliseconds + 1;
                                this.Invalidate();
                                return;
                            }

                            this.NewSelectionParagraph.StartTime.TotalSeconds = startTotalSeconds;
                            this.NewSelectionParagraph.EndTime.TotalSeconds = endTotalSeconds;
                        }
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// The is paragrap border hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsParagrapBorderHit(int milliseconds, List<Paragraph> paragraphs)
        {
            foreach (Paragraph p in paragraphs)
            {
                bool hit = this.IsParagrapBorderHit(milliseconds, p);
                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The is paragrap border hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsParagrapBorderHit(int milliseconds, Paragraph paragraph)
        {
            if (paragraph == null)
            {
                return false;
            }

            return this.IsParagrapBorderStartHit(milliseconds, paragraph.StartTime.TotalMilliseconds) || this.IsParagrapBorderEndHit(milliseconds, paragraph.EndTime.TotalMilliseconds);
        }

        /// <summary>
        /// The is paragrap border start hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="startMs">
        /// The start ms.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsParagrapBorderStartHit(double milliseconds, double startMs)
        {
            return Math.Abs(milliseconds - (startMs - 5)) - 10 <= this.ClosenessForBorderSelection;
        }

        /// <summary>
        /// The is paragrap border end hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="endMs">
        /// The end ms.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsParagrapBorderEndHit(double milliseconds, double endMs)
        {
            return Math.Abs(milliseconds - (endMs - 22)) - 7 <= this.ClosenessForBorderSelection;
        }

        /// <summary>
        /// The waveform mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this._mouseDown)
                {
                    if (this._mouseDownParagraph != null)
                    {
                        this._mouseDownParagraph = null;
                    }
                    else
                    {
                        this._mouseMoveEndX = e.X;
                    }

                    this._mouseDown = false;
                }
            }

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The waveform mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            this._mouseDown = false;
            this.Invalidate();
        }

        /// <summary>
        /// The waveform mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseEnter(object sender, EventArgs e)
        {
            if (this._wavePeaks == null || this._wavePeaks.Header == null)
            {
                return;
            }

            if (this._noClear)
            {
                this._noClear = false;
            }
            else
            {
                this.Cursor = Cursors.Default;
                this._mouseDown = false;
                this._mouseDownParagraph = null;
                this._mouseMoveStartX = -1;
                this._mouseMoveEndX = -1;
                this.Invalidate();
            }

            if (this.NewSelectionParagraph != null)
            {
                this._mouseMoveStartX = this.SecondsToXPosition(this.NewSelectionParagraph.StartTime.TotalSeconds - this.StartPositionSeconds);
                this._mouseMoveEndX = this.SecondsToXPosition(this.NewSelectionParagraph.EndTime.TotalSeconds - this.StartPositionSeconds);
            }
        }

        /// <summary>
        /// The waveform mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this._wavePeaks == null)
            {
                return;
            }

            this._mouseDown = false;
            this._mouseDownParagraph = null;
            this._mouseMoveStartX = -1;
            this._mouseMoveEndX = -1;

            if (e.Button == MouseButtons.Left)
            {
                if (this.OnPause != null)
                {
                    this.OnPause.Invoke(sender, null);
                }

                double seconds = this.XPositionToSeconds(e.X);
                var milliseconds = (int)(seconds * TimeCode.BaseUnit);

                Paragraph p = null;
                if (IsParagrapHit(milliseconds, this._selectedParagraph))
                {
                    p = this._selectedParagraph;
                }
                else if (IsParagrapHit(milliseconds, this._currentParagraph))
                {
                    p = this._currentParagraph;
                }

                if (p == null)
                {
                    foreach (Paragraph p2 in this._previousAndNextParagraphs)
                    {
                        if (IsParagrapHit(milliseconds, p2))
                        {
                            p = p2;
                            break;
                        }
                    }
                }

                if (p != null)
                {
                    seconds = p.StartTime.TotalSeconds;
                    double endSeconds = p.EndTime.TotalSeconds;
                    if (seconds < this.StartPositionSeconds)
                    {
                        this.StartPositionSeconds = p.StartTime.TotalSeconds + 0.1; // move earlier - show whole selected paragraph
                    }
                    else if (endSeconds > this.EndPositionSeconds)
                    {
                        double newStartPos = this.StartPositionSeconds + (endSeconds - this.EndPositionSeconds); // move later, so whole selected paragraph is visible
                        if (newStartPos < seconds)
                        {
                            // but only if visibile screen is wide enough
                            this.StartPositionSeconds = newStartPos;
                        }
                    }
                }

                if (this.OnDoubleClickNonParagraph != null)
                {
                    this.OnDoubleClickNonParagraph.Invoke(this, new ParagraphEventArgs(seconds, p));
                }
            }
        }

        /// <summary>
        /// The is paragrap hit.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsParagrapHit(int milliseconds, Paragraph paragraph)
        {
            if (paragraph == null)
            {
                return false;
            }

            return milliseconds >= paragraph.StartTime.TotalMilliseconds && milliseconds <= paragraph.EndTime.TotalMilliseconds;
        }

        /// <summary>
        /// The waveform mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.OnSingleClick != null)
            {
                int diff = Math.Abs(this._mouseMoveStartX - e.X);
                if (this._mouseMoveStartX == -1 || this._mouseMoveEndX == -1 || diff < 10 && TimeSpan.FromTicks(DateTime.Now.Ticks - this._buttonDownTimeTicks).TotalSeconds < 0.25)
                {
                    if (ModifierKeys == Keys.Shift && this._selectedParagraph != null)
                    {
                        double seconds = this.XPositionToSeconds(e.X);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        if (this._mouseDownParagraphType == MouseDownParagraphType.None || this._mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            if (seconds < this._selectedParagraph.EndTime.TotalSeconds)
                            {
                                this._oldParagraph = new Paragraph(this._selectedParagraph);
                                this._mouseDownParagraph = this._selectedParagraph;
                                this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                                if (this.OnTimeChanged != null)
                                {
                                    this.OnTimeChanged.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph, this._oldParagraph));
                                }
                            }
                        }

                        return;
                    }

                    if (ModifierKeys == Keys.Control && this._selectedParagraph != null)
                    {
                        double seconds = this.XPositionToSeconds(e.X);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        if (this._mouseDownParagraphType == MouseDownParagraphType.None || this._mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            if (seconds > this._selectedParagraph.StartTime.TotalSeconds)
                            {
                                this._oldParagraph = new Paragraph(this._selectedParagraph);
                                this._mouseDownParagraph = this._selectedParagraph;
                                this._mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                                if (this.OnTimeChanged != null)
                                {
                                    this.OnTimeChanged.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph, this._oldParagraph));
                                }
                            }
                        }

                        return;
                    }

                    if (ModifierKeys == (Keys.Control | Keys.Shift) && this._selectedParagraph != null)
                    {
                        double seconds = this.XPositionToSeconds(e.X);
                        if (this._mouseDownParagraphType == MouseDownParagraphType.None || this._mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            this._oldParagraph = new Paragraph(this._selectedParagraph);
                            this._mouseDownParagraph = this._selectedParagraph;
                            if (this.OnTimeChangedAndOffsetRest != null)
                            {
                                this.OnTimeChangedAndOffsetRest.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph));
                            }
                        }

                        return;
                    }

                    if (ModifierKeys == Keys.Alt && this._selectedParagraph != null)
                    {
                        double seconds = this.XPositionToSeconds(e.X);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        if (this._mouseDownParagraphType == MouseDownParagraphType.None || this._mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            this._oldParagraph = new Paragraph(this._selectedParagraph);
                            this._mouseDownParagraph = this._selectedParagraph;
                            double durationMilliseconds = this._mouseDownParagraph.Duration.TotalMilliseconds;
                            this._mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                            this._mouseDownParagraph.EndTime.TotalMilliseconds = this._mouseDownParagraph.StartTime.TotalMilliseconds + durationMilliseconds;
                            if (this.OnTimeChanged != null)
                            {
                                this.OnTimeChanged.Invoke(this, new ParagraphEventArgs(seconds, this._mouseDownParagraph, this._oldParagraph));
                            }
                        }

                        return;
                    }

                    if (this._mouseDownParagraphType == MouseDownParagraphType.None || this._mouseDownParagraphType == MouseDownParagraphType.Whole)
                    {
                        this.OnSingleClick.Invoke(this, new ParagraphEventArgs(this.XPositionToSeconds(e.X), null));
                    }
                }
            }
        }

        /// <summary>
        /// The waveform key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Add)
            {
                this.ZoomIn();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Subtract)
            {
                this.ZoomOut();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Z)
            {
                if (this.StartPositionSeconds > 0.1)
                {
                    this.StartPositionSeconds -= 0.1;
                    this.OnPositionSelected.Invoke(this, new ParagraphEventArgs(this.StartPositionSeconds, null));
                    this.Invalidate();
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.X)
            {
                if (this.StartPositionSeconds + 0.1 < this._wavePeaks.Header.LengthInSeconds)
                {
                    this.StartPositionSeconds += 0.1;
                    this.OnPositionSelected.Invoke(this, new ParagraphEventArgs(this.StartPositionSeconds, null));
                    this.Invalidate();
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.C)
            {
                this.Locked = !this.Locked;
                this.Invalidate();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == this.InsertAtVideoPositionShortcut)
            {
                if (this.InsertAtVideoPosition != null)
                {
                    this.InsertAtVideoPosition.Invoke(this, null);
                    e.SuppressKeyPress = true;
                }
            }
        }

        /// <summary>
        /// The find data below threshold.
        /// </summary>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        /// <param name="durationInSeconds">
        /// The duration in seconds.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double FindDataBelowThreshold(int threshold, double durationInSeconds)
        {
            int begin = this.SecondsToXPosition(this._currentVideoPositionSeconds + 1);
            int length = this.SecondsToXPosition(durationInSeconds);

            int hitCount = 0;
            for (int i = begin; i < this._wavePeaks.AllSamples.Count; i++)
            {
                if (i > 0 && i < this._wavePeaks.AllSamples.Count && Math.Abs(this._wavePeaks.AllSamples[i]) <= threshold)
                {
                    hitCount++;
                }
                else
                {
                    hitCount = 0;
                }

                if (hitCount > length)
                {
                    double seconds = ((i - (length / 2)) / (double)this._wavePeaks.Header.SampleRate) / this._zoomFactor;
                    if (seconds >= 0)
                    {
                        this.StartPositionSeconds = seconds;
                        if (this.StartPositionSeconds > 1)
                        {
                            this.StartPositionSeconds -= 1;
                        }

                        this.OnSingleClick.Invoke(this, new ParagraphEventArgs(seconds, null));
                        this.Invalidate();
                    }

                    return seconds;
                }
            }

            return -1;
        }

        /// <summary>
        /// The find data below threshold back.
        /// </summary>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        /// <param name="durationInSeconds">
        /// The duration in seconds.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double FindDataBelowThresholdBack(int threshold, double durationInSeconds)
        {
            int begin = this.SecondsToXPosition(this._currentVideoPositionSeconds - 1);
            int length = this.SecondsToXPosition(durationInSeconds);

            int hitCount = 0;
            for (int i = begin; i > 0; i--)
            {
                if (i > 0 && i < this._wavePeaks.AllSamples.Count && Math.Abs(this._wavePeaks.AllSamples[i]) <= threshold)
                {
                    hitCount++;
                }
                else
                {
                    hitCount = 0;
                }

                if (hitCount > length)
                {
                    double seconds = (i + (length / 2)) / (double)this._wavePeaks.Header.SampleRate / this._zoomFactor;
                    if (seconds >= 0)
                    {
                        this.StartPositionSeconds = seconds;
                        if (this.StartPositionSeconds > 1)
                        {
                            this.StartPositionSeconds -= 1;
                        }
                        else
                        {
                            this.StartPositionSeconds = 0;
                        }

                        this.OnSingleClick.Invoke(this, new ParagraphEventArgs(seconds, null));
                        this.Invalidate();
                    }

                    return seconds;
                }
            }

            return -1;
        }

        /// <summary>
        /// The zoom in.
        /// </summary>
        public void ZoomIn()
        {
            this.ZoomFactor = this.ZoomFactor + 0.1;
            if (this.OnZoomedChanged != null)
            {
                this.OnZoomedChanged.Invoke(this, null);
            }
        }

        /// <summary>
        /// The zoom out.
        /// </summary>
        public void ZoomOut()
        {
            this.ZoomFactor = this.ZoomFactor - 0.1;
            if (this.OnZoomedChanged != null)
            {
                this.OnZoomedChanged.Invoke(this, null);
            }
        }

        /// <summary>
        /// The waveform mouse wheel.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WaveformMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta;
            if (!this.MouseWheelScrollUpIsForward)
            {
                delta = delta * -1;
            }

            if (this.Locked)
            {
                this.OnPositionSelected.Invoke(this, new ParagraphEventArgs(this._currentVideoPositionSeconds + (delta / 256.0), null));
            }
            else
            {
                this.StartPositionSeconds += delta / 256.0;
                if (this._currentVideoPositionSeconds < this.StartPositionSeconds || this._currentVideoPositionSeconds >= this.EndPositionSeconds)
                {
                    this.OnPositionSelected.Invoke(this, new ParagraphEventArgs(this.StartPositionSeconds, null));
                }
            }

            this.Invalidate();
        }

        /////////////////////////////////////////////////

        /// <summary>
        /// The initialize spectrogram.
        /// </summary>
        /// <param name="spectrogramDirectory">
        /// The spectrogram directory.
        /// </param>
        public void InitializeSpectrogram(string spectrogramDirectory)
        {
            this._spectrogramBitmaps = new List<Bitmap>();
            this._tempShowSpectrogram = this.ShowSpectrogram;
            this.ShowSpectrogram = false;
            if (Directory.Exists(spectrogramDirectory))
            {
                this._spectrogramDirectory = spectrogramDirectory;
                this._spectrogramBackgroundWorker = new System.ComponentModel.BackgroundWorker();
                this._spectrogramBackgroundWorker.DoWork += this.LoadSpectrogramBitmapsAsync;
                this._spectrogramBackgroundWorker.RunWorkerCompleted += this.LoadSpectrogramBitmapsCompleted;
                this._spectrogramBackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// The load spectrogram bitmaps completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LoadSpectrogramBitmapsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.LoadSpectrogramInfo(this._spectrogramDirectory);
            this.ShowSpectrogram = this._tempShowSpectrogram;
            if (this._spectrogramBackgroundWorker != null)
            {
                this._spectrogramBackgroundWorker.Dispose();
            }
        }

        /// <summary>
        /// The load spectrogram bitmaps async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LoadSpectrogramBitmapsAsync(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (var count = 0;; count++)
                {
                    var fileName = Path.Combine(this._spectrogramDirectory, count + ".gif");
                    this._spectrogramBitmaps.Add((Bitmap)Image.FromFile(fileName));
                }
            }
            catch (FileNotFoundException)
            {
                // no more files
            }
        }

        /// <summary>
        /// The initialize spectrogram.
        /// </summary>
        /// <param name="spectrogramBitmaps">
        /// The spectrogram bitmaps.
        /// </param>
        /// <param name="spectrogramDirectory">
        /// The spectrogram directory.
        /// </param>
        public void InitializeSpectrogram(List<Bitmap> spectrogramBitmaps, string spectrogramDirectory)
        {
            this._spectrogramBitmaps = spectrogramBitmaps;
            this.LoadSpectrogramInfo(spectrogramDirectory);
        }

        /// <summary>
        /// The load spectrogram info.
        /// </summary>
        /// <param name="spectrogramDirectory">
        /// The spectrogram directory.
        /// </param>
        private void LoadSpectrogramInfo(string spectrogramDirectory)
        {
            try
            {
                var doc = new XmlDocument();
                string xmlInfoFileName = Path.Combine(spectrogramDirectory, "Info.xml");
                if (File.Exists(xmlInfoFileName))
                {
                    doc.Load(xmlInfoFileName);
                    this._sampleDuration = Convert.ToDouble(doc.DocumentElement.SelectSingleNode("SampleDuration").InnerText, CultureInfo.InvariantCulture);
                    this._secondsPerImage = Convert.ToDouble(doc.DocumentElement.SelectSingleNode("SecondsPerImage").InnerText, CultureInfo.InvariantCulture);
                    this._nfft = Convert.ToInt32(doc.DocumentElement.SelectSingleNode("NFFT").InnerText, CultureInfo.InvariantCulture);
                    this._imageWidth = Convert.ToInt32(doc.DocumentElement.SelectSingleNode("ImageWidth").InnerText, CultureInfo.InvariantCulture);
                    this.ShowSpectrogram = true;
                }
                else
                {
                    this.ShowSpectrogram = false;
                }
            }
            catch
            {
                this.ShowSpectrogram = false;
            }
        }

        /// <summary>
        /// The draw spectrogram bitmap.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        private void DrawSpectrogramBitmap(double seconds, Graphics graphics)
        {
            double duration = this.EndPositionSeconds - this.StartPositionSeconds;
            var width = (int)(duration / this._sampleDuration);

            var bmpDestination = new Bitmap(width, 128); // calculate width
            var gfx = Graphics.FromImage(bmpDestination);

            double startRow = seconds / this._secondsPerImage;
            var bitmapIndex = (int)startRow;
            var subtractValue = (int)Math.Round((startRow - bitmapIndex) * this._imageWidth);

            int i = 0;
            while (i * this._imageWidth < width && i + bitmapIndex < this._spectrogramBitmaps.Count)
            {
                var bmp = this._spectrogramBitmaps[i + bitmapIndex];
                gfx.DrawImageUnscaled(bmp, new Point(bmp.Width * i - subtractValue, 0));
                i++;
            }

            if (i + bitmapIndex < this._spectrogramBitmaps.Count && subtractValue > 0)
            {
                var bmp = this._spectrogramBitmaps[i + bitmapIndex];
                gfx.DrawImageUnscaled(bmp, new Point(bmp.Width * i - subtractValue, 0));
            }

            gfx.Dispose();

            if (this.ShowWaveform)
            {
                graphics.DrawImage(bmpDestination, new Rectangle(0, this.Height - bmpDestination.Height, this.Width, bmpDestination.Height));
            }
            else
            {
                graphics.DrawImage(bmpDestination, new Rectangle(0, 0, this.Width, this.Height));
            }

            bmpDestination.Dispose();
        }

        /// <summary>
        /// The get average volume for next milliseconds.
        /// </summary>
        /// <param name="sampleIndex">
        /// The sample index.
        /// </param>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private double GetAverageVolumeForNextMilliseconds(int sampleIndex, int milliseconds)
        {
            int length = this.SecondsToXPosition(milliseconds / TimeCode.BaseUnit);
            if (length < 9)
            {
                length = 9;
            }

            double v = 0;
            int count = 0;
            for (int i = sampleIndex; i < sampleIndex + length; i++)
            {
                if (i > 0 && i < this._wavePeaks.AllSamples.Count)
                {
                    v += Math.Abs(this._wavePeaks.AllSamples[i]);
                    count++;
                }
            }

            if (count == 0)
            {
                return 0;
            }

            return v / count;
        }

        /// <summary>
        /// The generate time codes.
        /// </summary>
        /// <param name="startFromSeconds">
        /// The start from seconds.
        /// </param>
        /// <param name="minimumVolumePercent">
        /// The minimum volume percent.
        /// </param>
        /// <param name="maximumVolumePercent">
        /// The maximum volume percent.
        /// </param>
        /// <param name="defaultMilliseconds">
        /// The default milliseconds.
        /// </param>
        internal void GenerateTimeCodes(double startFromSeconds, int minimumVolumePercent, int maximumVolumePercent, int defaultMilliseconds)
        {
            int begin = this.SecondsToXPosition(startFromSeconds);

            double average = 0;
            for (int k = begin; k < this._wavePeaks.AllSamples.Count; k++)
            {
                average += Math.Abs(this._wavePeaks.AllSamples[k]);
            }

            average = average / (this._wavePeaks.AllSamples.Count - begin);

            var maxThreshold = (int)(this._wavePeaks.DataMaxValue * (maximumVolumePercent / 100.0));
            var silenceThreshold = (int)(average * (minimumVolumePercent / 100.0));

            int length50Ms = this.SecondsToXPosition(0.050);
            double secondsPerParagraph = defaultMilliseconds / TimeCode.BaseUnit;
            int minBetween = this.SecondsToXPosition(Configuration.Settings.General.MinimumMillisecondsBetweenLines / TimeCode.BaseUnit);
            bool subtitleOn = false;
            int i = begin;
            while (i < this._wavePeaks.AllSamples.Count)
            {
                if (subtitleOn)
                {
                    var currentLengthInSeconds = this.XPositionToSeconds(i - begin) - this.StartPositionSeconds;
                    if (currentLengthInSeconds > 1.0)
                    {
                        subtitleOn = this.EndParagraphDueToLowVolume(silenceThreshold, begin, true, i);
                        if (!subtitleOn)
                        {
                            begin = i + minBetween;
                            i = begin;
                        }
                    }

                    if (subtitleOn && currentLengthInSeconds >= secondsPerParagraph)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            subtitleOn = this.EndParagraphDueToLowVolume(silenceThreshold, begin, true, i + (j * length50Ms));
                            if (!subtitleOn)
                            {
                                i += j * length50Ms;
                                begin = i + minBetween;
                                i = begin;
                                break;
                            }
                        }

                        if (subtitleOn)
                        {
                            // force break
                            var p = new Paragraph(string.Empty, (this.XPositionToSeconds(begin) - this.StartPositionSeconds) * TimeCode.BaseUnit, (this.XPositionToSeconds(i) - this.StartPositionSeconds) * TimeCode.BaseUnit);
                            this._subtitle.Paragraphs.Add(p);
                            begin = i + minBetween;
                            i = begin;
                        }
                    }
                }
                else
                {
                    double avgVol = this.GetAverageVolumeForNextMilliseconds(i, 100);
                    if (avgVol > silenceThreshold)
                    {
                        if (avgVol < maxThreshold)
                        {
                            subtitleOn = true;
                            begin = i;
                        }
                    }
                }

                i++;
            }
        }

        /// <summary>
        /// The end paragraph due to low volume.
        /// </summary>
        /// <param name="silenceThreshold">
        /// The silence threshold.
        /// </param>
        /// <param name="begin">
        /// The begin.
        /// </param>
        /// <param name="subtitleOn">
        /// The subtitle on.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool EndParagraphDueToLowVolume(double silenceThreshold, int begin, bool subtitleOn, int i)
        {
            double avgVol = this.GetAverageVolumeForNextMilliseconds(i, 100);
            if (avgVol < silenceThreshold)
            {
                var p = new Paragraph(string.Empty, (this.XPositionToSeconds(begin) - this.StartPositionSeconds) * TimeCode.BaseUnit, (this.XPositionToSeconds(i) - this.StartPositionSeconds) * TimeCode.BaseUnit);
                this._subtitle.Paragraphs.Add(p);
                subtitleOn = false;
            }

            return subtitleOn;
        }
    }
}