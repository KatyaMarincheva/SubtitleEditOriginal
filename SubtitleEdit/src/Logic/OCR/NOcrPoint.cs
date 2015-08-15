namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    public class NOcrPoint
    {
        public NOcrPoint()
        {
            this.Start = new Point();
            this.End = new Point();
        }

        public NOcrPoint(Point start, Point end)
        {
            this.Start = new Point(start.X, start.Y);
            this.End = new Point(end.X, end.Y);
        }

        public Point Start { get; set; }

        public Point End { get; set; }

        public static PointF PointPixelsToPercent(Point p, int pixelWidth, int pixelHeight)
        {
            return new PointF((float)(p.X * 100.0 / pixelWidth), (float)(p.Y * 100.0 / pixelHeight));
        }

        public static Point PointPercentToPixels(PointF p, int pixelWidth, int pixelHeight)
        {
            return new Point((int)Math.Round(p.X / 100.0 * pixelWidth), (int)Math.Round(p.Y / 100.0 * pixelHeight));
        }

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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1} -> {2},{3} ", this.Start.X, this.Start.Y, this.End.X, this.End.Y);
        }

        public PointF GetStartPercent(int width, int height)
        {
            return PointPixelsToPercent(this.Start, width, height);
        }

        public PointF GetEnd(int width, int height)
        {
            return PointPixelsToPercent(this.End, width, height);
        }

        public List<Point> GetPoints()
        {
            return GetPoints(this.Start, this.End);
        }

        public List<Point> ScaledGetPoints(NOcrChar nOcrChar, int width, int height)
        {
            return GetPoints(this.GetScaledStart(nOcrChar, width, height), this.GetScaledEnd(nOcrChar, width, height));
        }

        internal Point GetScaledStart(NOcrChar ocrChar, int width, int height)
        {
            return new Point((int)Math.Round(this.Start.X * 100.0 / ocrChar.Width * width / 100.0), (int)Math.Round(this.Start.Y * 100.0 / ocrChar.Height * height / 100.0));
        }

        internal Point GetScaledEnd(NOcrChar ocrChar, int width, int height)
        {
            return new Point((int)Math.Round(this.End.X * 100.0 / ocrChar.Width * width / 100.0), (int)Math.Round(this.End.Y * 100.0 / ocrChar.Height * height / 100.0));
        }
    }
}