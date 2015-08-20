// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NOcrPoint.cs" company="">
//   
// </copyright>
// <summary>
//   The n ocr point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    /// <summary>
    /// The n ocr point.
    /// </summary>
    public class NOcrPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrPoint"/> class.
        /// </summary>
        public NOcrPoint()
        {
            this.Start = new Point();
            this.End = new Point();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrPoint"/> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        public NOcrPoint(Point start, Point end)
        {
            this.Start = new Point(start.X, start.Y);
            this.End = new Point(end.X, end.Y);
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public Point Start { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public Point End { get; set; }

        /// <summary>
        /// The point pixels to percent.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="pixelWidth">
        /// The pixel width.
        /// </param>
        /// <param name="pixelHeight">
        /// The pixel height.
        /// </param>
        /// <returns>
        /// The <see cref="PointF"/>.
        /// </returns>
        public static PointF PointPixelsToPercent(Point p, int pixelWidth, int pixelHeight)
        {
            return new PointF((float)(p.X * 100.0 / pixelWidth), (float)(p.Y * 100.0 / pixelHeight));
        }

        /// <summary>
        /// The point percent to pixels.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="pixelWidth">
        /// The pixel width.
        /// </param>
        /// <param name="pixelHeight">
        /// The pixel height.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        public static Point PointPercentToPixels(PointF p, int pixelWidth, int pixelHeight)
        {
            return new Point((int)Math.Round(p.X / 100.0 * pixelWidth), (int)Math.Round(p.Y / 100.0 * pixelHeight));
        }

        /// <summary>
        /// The get points.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Point> GetPoints(Point start, Point end)
        {
            List<Point> list = new List<Point>();
            int x1 = start.X;
            int x2 = end.X;
            int y1 = start.Y;
            int y2 = end.Y;
            if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
            {
                if (x1 > x2)
                {
                    x2 = start.X;
                    x1 = end.X;
                    y2 = start.Y;
                    y1 = end.Y;
                }

                double factor = (double)(y2 - y1) / (x2 - x1);
                for (int i = x1; i <= x2; i++)
                {
                    list.Add(new Point(i, (int)Math.Round(y1 + factor * (i - x1))));
                }
            }
            else
            {
                if (y1 > y2)
                {
                    x2 = start.X;
                    x1 = end.X;
                    y2 = start.Y;
                    y1 = end.Y;
                }

                double factor = (double)(x2 - x1) / (y2 - y1);
                for (int i = y1; i <= y2; i++)
                {
                    list.Add(new Point((int)Math.Round(x1 + factor * (i - y1)), i));
                }
            }

            return list;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1} -> {2},{3} ", this.Start.X, this.Start.Y, this.End.X, this.End.Y);
        }

        /// <summary>
        /// The get start percent.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="PointF"/>.
        /// </returns>
        public PointF GetStartPercent(int width, int height)
        {
            return PointPixelsToPercent(this.Start, width, height);
        }

        /// <summary>
        /// The get end.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="PointF"/>.
        /// </returns>
        public PointF GetEnd(int width, int height)
        {
            return PointPixelsToPercent(this.End, width, height);
        }

        /// <summary>
        /// The get points.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Point> GetPoints()
        {
            return GetPoints(this.Start, this.End);
        }

        /// <summary>
        /// The scaled get points.
        /// </summary>
        /// <param name="nOcrChar">
        /// The n ocr char.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Point> ScaledGetPoints(NOcrChar nOcrChar, int width, int height)
        {
            return GetPoints(this.GetScaledStart(nOcrChar, width, height), this.GetScaledEnd(nOcrChar, width, height));
        }

        /// <summary>
        /// The get scaled start.
        /// </summary>
        /// <param name="ocrChar">
        /// The ocr char.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        internal Point GetScaledStart(NOcrChar ocrChar, int width, int height)
        {
            return new Point((int)Math.Round(this.Start.X * 100.0 / ocrChar.Width * width / 100.0), (int)Math.Round(this.Start.Y * 100.0 / ocrChar.Height * height / 100.0));
        }

        /// <summary>
        /// The get scaled end.
        /// </summary>
        /// <param name="ocrChar">
        /// The ocr char.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        internal Point GetScaledEnd(NOcrChar ocrChar, int width, int height)
        {
            return new Point((int)Math.Round(this.End.X * 100.0 / ocrChar.Width * width / 100.0), (int)Math.Round(this.End.Y * 100.0 / ocrChar.Height * height / 100.0));
        }
    }
}