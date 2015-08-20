// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryOcrDb.cs" company="">
//   
// </copyright>
// <summary>
//   The binary ocr db.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr.Binary
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Windows.Forms;

    /// <summary>
    /// The binary ocr db.
    /// </summary>
    public class BinaryOcrDb
    {
        /// <summary>
        /// The compare images.
        /// </summary>
        public List<BinaryOcrBitmap> CompareImages = new List<BinaryOcrBitmap>();

        /// <summary>
        /// The compare images expanded.
        /// </summary>
        public List<BinaryOcrBitmap> CompareImagesExpanded = new List<BinaryOcrBitmap>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOcrDb"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public BinaryOcrDb(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOcrDb"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="loadCompareImages">
        /// The load compare images.
        /// </param>
        public BinaryOcrDb(string fileName, bool loadCompareImages)
        {
            this.FileName = fileName;
            if (loadCompareImages)
            {
                this.LoadCompareImages();
            }
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            using (Stream gz = new GZipStream(File.OpenWrite(this.FileName), CompressionMode.Compress))
            {
                foreach (BinaryOcrBitmap bob in this.CompareImages)
                {
                    bob.Save(gz);
                }

                foreach (BinaryOcrBitmap bob in this.CompareImagesExpanded)
                {
                    bob.Save(gz);
                    foreach (BinaryOcrBitmap expandedBob in bob.ExpandedList)
                    {
                        expandedBob.Save(gz);
                    }
                }
            }
        }

        /// <summary>
        /// The load compare images.
        /// </summary>
        public void LoadCompareImages()
        {
            List<BinaryOcrBitmap> list = new List<BinaryOcrBitmap>();
            List<BinaryOcrBitmap> expandList = new List<BinaryOcrBitmap>();

            if (!File.Exists(this.FileName))
            {
                this.CompareImages = list;
                return;
            }

            using (Stream gz = new GZipStream(File.OpenRead(this.FileName), CompressionMode.Decompress))
            {
                bool done = false;
                while (!done)
                {
                    BinaryOcrBitmap bob = new BinaryOcrBitmap(gz);
                    if (bob.LoadedOk)
                    {
                        if (bob.ExpandCount > 0)
                        {
                            expandList.Add(bob);
                            bob.ExpandedList = new List<BinaryOcrBitmap>();
                            for (int i = 1; i < bob.ExpandCount; i++)
                            {
                                BinaryOcrBitmap expandedBob = new BinaryOcrBitmap(gz);
                                if (expandedBob.LoadedOk)
                                {
                                    bob.ExpandedList.Add(expandedBob);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            list.Add(bob);
                        }
                    }
                    else
                    {
                        done = true;
                    }
                }
            }

            this.CompareImages = list;
            this.CompareImagesExpanded = expandList;
        }

        /// <summary>
        /// The find exact match.
        /// </summary>
        /// <param name="bob">
        /// The bob.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int FindExactMatch(BinaryOcrBitmap bob)
        {
            for (int i = 0; i < this.CompareImages.Count; i++)
            {
                BinaryOcrBitmap b = this.CompareImages[i];
                if (bob.Hash == b.Hash && bob.Width == b.Width && bob.Height == b.Height && bob.NumberOfColoredPixels == b.NumberOfColoredPixels)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The find exact match expanded.
        /// </summary>
        /// <param name="bob">
        /// The bob.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int FindExactMatchExpanded(BinaryOcrBitmap bob)
        {
            for (int i = 0; i < this.CompareImagesExpanded.Count; i++)
            {
                BinaryOcrBitmap b = this.CompareImagesExpanded[i];
                if (bob.Hash == b.Hash && bob.Width == b.Width && bob.Height == b.Height && bob.NumberOfColoredPixels == b.NumberOfColoredPixels)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="bob">
        /// The bob.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Add(BinaryOcrBitmap bob)
        {
            int index;
            if (bob.ExpandCount > 0)
            {
                index = this.FindExactMatchExpanded(bob);
                if (index == -1 || this.CompareImagesExpanded[index].ExpandCount != bob.ExpandCount)
                {
                    this.CompareImagesExpanded.Add(bob);
                }
                else
                {
                    bool allAlike = true;
                    for (int i = 0; i < bob.ExpandCount - 1; i++)
                    {
                        if (bob.ExpandedList[i].Hash != this.CompareImagesExpanded[index].ExpandedList[i].Hash)
                        {
                            allAlike = false;
                        }
                    }

                    if (!allAlike)
                    {
                        this.CompareImages.Add(bob);
                    }
                    else
                    {
                        MessageBox.Show("Expanded image already in db!");
                    }
                }
            }
            else
            {
                index = this.FindExactMatch(bob);
                if (index == -1)
                {
                    this.CompareImages.Add(bob);
                }
                else
                {
                    MessageBox.Show("Image already in db!");
                }
            }

            return index;
        }
    }
}