namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;

    public class NOcrDb
    {
        public List<NOcrChar> OcrCharacters = new List<NOcrChar>();

        public List<NOcrChar> OcrCharactersExpanded = new List<NOcrChar>();

        public NOcrDb(string fileName)
        {
            this.FileName = fileName;
            this.LoadOcrCharacters();
        }

        public string FileName { get; private set; }

        public static int FindExactMatch()
        {
            return -1;
        }

        public void Save()
        {
            using (Stream gz = new GZipStream(File.OpenWrite(this.FileName), CompressionMode.Compress))
            {
                foreach (NOcrChar ocrChar in this.OcrCharacters)
                {
                    ocrChar.Save(gz);
                }

                foreach (NOcrChar ocrChar in this.OcrCharactersExpanded)
                {
                    ocrChar.Save(gz);
                }
            }
        }

        public void LoadOcrCharacters()
        {
            List<NOcrChar> list = new List<NOcrChar>();
            List<NOcrChar> listExpanded = new List<NOcrChar>();

            if (!File.Exists(this.FileName))
            {
                this.OcrCharacters = list;
                this.OcrCharactersExpanded = listExpanded;
                return;
            }

            using (Stream gz = new GZipStream(File.OpenRead(this.FileName), CompressionMode.Decompress))
            {
                bool done = false;
                while (!done)
                {
                    NOcrChar ocrChar = new NOcrChar(gz);
                    if (ocrChar.LoadedOk)
                    {
                        if (ocrChar.ExpandCount > 0)
                        {
                            listExpanded.Add(ocrChar);
                        }
                        else
                        {
                            list.Add(ocrChar);
                        }
                    }
                    else
                    {
                        done = true;
                    }
                }
            }

            this.OcrCharacters = list;
            this.OcrCharactersExpanded = listExpanded;
        }

        public void Add(NOcrChar ocrChar)
        {
            if (ocrChar.ExpandCount > 0)
            {
                this.OcrCharactersExpanded.Insert(0, ocrChar);
            }
            else
            {
                this.OcrCharacters.Insert(0, ocrChar);
            }
        }

        public NOcrChar GetMatch(NikseBitmap nbmp)
        {
            const int NocrMinColor = 300;
            const int topMargin = 1;
            double widthPercent = nbmp.Height * 100.0 / nbmp.Width;

            foreach (NOcrChar oc in this.OcrCharacters)
            {
                if (Math.Abs(widthPercent - oc.WidthPercent) < 20 && Math.Abs(oc.MarginTop - topMargin) < 5)
                { // only very accurate matches

                    bool ok = true;
                    int index = 0;
                    while (index < oc.LinesForeground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesForeground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                }
                                else
                                {
                                    Point p = new Point(point.X - 1, point.Y);
                                    if (p.X < 0)
                                    {
                                        p.X = 1;
                                    }

                                    c = nbmp.GetPixel(p.X, p.Y);
                                    if (nbmp.Width > 20 && c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                    }

                    index = 0;
                    while (index < oc.LinesBackground.Count && ok)
                    {
                        NOcrPoint op = oc.LinesBackground[index];
                        foreach (Point point in op.ScaledGetPoints(oc, nbmp.Width, nbmp.Height))
                        {
                            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
                            {
                                Color c = nbmp.GetPixel(point.X, point.Y);
                                if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                {
                                    Point p = new Point(point.X, point.Y);
                                    if (oc.Width > 19 && point.X > 0)
                                    {
                                        p.X = p.X - 1;
                                    }

                                    c = nbmp.GetPixel(p.X, p.Y);
                                    if (c.A > 150 && c.R + c.G + c.B > NocrMinColor)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                    }

                    if (ok)
                    {
                        return oc;
                    }
                }
            }

            return null;
        }
    }
}