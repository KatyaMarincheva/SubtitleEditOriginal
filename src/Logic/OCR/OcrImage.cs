// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrImage.cs" company="">
//   
// </copyright>
// <summary>
//   The ocr image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System.Drawing;

    /// <summary>
    /// The ocr image.
    /// </summary>
    public class OcrImage
    {
        /// <summary>
        /// Gets or sets a value indicating whether italic.
        /// </summary>
        public bool Italic { get; set; }

        /// <summary>
        /// Gets or sets the bmp.
        /// </summary>
        public Bitmap Bmp { get; set; }

        /// <summary>
        /// The get training data.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public double[] GetTrainingData(int size)
        {
            int i = 0;
            double[] data = new double[size];
            for (int y = 0; y < this.Bmp.Height; y++)
            {
                for (int x = 0; x < this.Bmp.Width; x++)
                {
                    Color c = this.Bmp.GetPixel(x, y);
                    if (i < size)
                    {
                        if (c == Color.Transparent)
                        {
                            data[i] = -0.5;
                        }
                        else
                        {
                            int value = c.R + c.R + c.B;
                            data[i] = value / 766.0;
                        }
                    }

                    i++;
                }
            }

            return data;
        }
    }
}