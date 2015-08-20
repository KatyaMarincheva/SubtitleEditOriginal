// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NOcrDb.cs" company="">
//   
// </copyright>
// <summary>
//   The n ocr db.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// The n ocr db.
    /// </summary>
    public class NOcrDb
    {
        /// <summary>
        /// The ocr characters.
        /// </summary>
        public List<NOcrChar> OcrCharacters = new List<NOcrChar>();

        /// <summary>
        /// The ocr characters expanded.
        /// </summary>
        public List<NOcrChar> OcrCharactersExpanded = new List<NOcrChar>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrDb"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public NOcrDb(string fileName)
        {
            this.FileName = fileName;
            this.LoadOcrCharacters();
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The find exact match.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int FindExactMatch()
        {
            return -1;
        }

        /// <summary>
        /// The save.
        /// </summary>
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

        /// <summary>
        /// The load ocr characters.
        /// </summary>
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

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="ocrChar">
        /// The ocr char.
        /// </param>
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

        /// <summary>
        /// The get match.
        /// </summary>
        /// <param name="nbmp">
        /// The nbmp.
        /// </param>
        /// <returns>
        /// The <see cref="NOcrChar"/>.
        /// </returns>
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