namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System.Drawing;

    public class OcrImage
    {
        public bool Italic { get; set; }

        public Bitmap Bmp { get; set; }

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