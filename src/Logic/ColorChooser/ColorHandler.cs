// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ColorHandler.cs">
//   
// </copyright>
// <summary>
//   The color handler.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.ColorChooser
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The color handler.
    /// </summary>
    public class ColorHandler
    {
        // Handle conversions between RGB and HSV
        // (and Color types, as well).
        /// <summary>
        /// The hs vto rgb.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="h">
        /// The h.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="v">
        /// The v.
        /// </param>
        /// <returns>
        /// The <see cref="ARGB"/>.
        /// </returns>
        public static ARGB HSVtoRGB(int a, int h, int s, int v)
        {
            // H, S, and V must all be between 0 and 255.
            return HSVtoRGB(new HSV(a, h, s, v));
        }

        /// <summary>
        /// The hs vto color.
        /// </summary>
        /// <param name="hsv">
        /// The hsv.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color HSVtoColor(HSV hsv)
        {
            ARGB argb = HSVtoRGB(hsv);
            return Color.FromArgb(argb.Alpha, argb.Red, argb.Green, argb.Blue);
        }

        /// <summary>
        /// The hs vto color.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="h">
        /// The h.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="v">
        /// The v.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color HSVtoColor(int a, int h, int s, int v)
        {
            return HSVtoColor(new HSV(a, h, s, v));
        }

        /// <summary>
        /// The hs vto rgb.
        /// </summary>
        /// <param name="HSV">
        /// The hsv.
        /// </param>
        /// <returns>
        /// The <see cref="ARGB"/>.
        /// </returns>
        public static ARGB HSVtoRGB(HSV HSV)
        {
            // HSV contains values scaled as in the color wheel:
            // that is, all from 0 to 255.

            // for ( this code to work, HSV.Hue needs
            // to be scaled from 0 to 360 (it//s the angle of the selected
            // point within the circle). HSV.Saturation and HSV.value must be
            // scaled to be between 0 and 1.
            double r = 0;
            double g = 0;
            double b = 0;

            // Scale Hue to be between 0 and 360. Saturation
            // and value scale to be between 0 and 1.
            double h = ((double)HSV.Hue / 255 * 360) % 360;
            double s = (double)HSV.Saturation / 255;
            double v = (double)HSV.Value / 255;

            if (s == 0)
            {
                // If s is 0, all colors are the same.
                // This is some flavor of gray.
                r = v;
                g = v;
                b = v;
            }
            else
            {
                // The color wheel consists of 6 sectors.
                // Figure out which sector you//re in.
                double sectorPos = h / 60;
                int sectorNumber = (int)Math.Floor(sectorPos);

                // get the fractional part of the sector.
                // That is, how many degrees into the sector
                // are you?
                double fractionalSector = sectorPos - sectorNumber;

                // Calculate values for the three axes
                // of the color.
                double p = v * (1 - s);
                double q = v * (1 - (s * fractionalSector));
                double t = v * (1 - (s * (1 - fractionalSector)));

                // Assign the fractional colors to r, g, and b
                // based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }

            // return an RGB structure, with values scaled
            // to be between 0 and 255.
            return new ARGB(HSV.Alpha, (int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        /// <summary>
        /// The rg bto hsv.
        /// </summary>
        /// <param name="argb">
        /// The argb.
        /// </param>
        /// <returns>
        /// The <see cref="HSV"/>.
        /// </returns>
        public static HSV RGBtoHSV(ARGB argb)
        {
            // In this function, R, G, and B values must be scaled
            // to be between 0 and 1.
            // HSV.Hue will be a value between 0 and 360, and
            // HSV.Saturation and value are between 0 and 1.
            // The code must scale these to be between 0 and 255 for
            // the purposes of this application.
            double r = (double)argb.Red / 255;
            double g = (double)argb.Green / 255;
            double b = (double)argb.Blue / 255;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);

            double h;
            double s;
            double v = max;

            double delta = max - min;
            if (max == 0 || delta == 0)
            {
                // R, G, and B must be 0, or all the same.
                // In this case, S is 0, and H is undefined.
                // Using H = 0 is as good as any...
                s = 0;
                h = 0;
            }
            else
            {
                s = delta / max;
                if (r == max)
                {
                    // Between Yellow and Magenta
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    // Between Cyan and Yellow
                    h = 2 + (b - r) / delta;
                }
                else
                {
                    // Between Magenta and Cyan
                    h = 4 + (r - g) / delta;
                }
            }

            // Scale h to be between 0 and 360.
            // This may require adding 360, if the value
            // is negative.
            h *= 60;
            if (h < 0)
            {
                h += 360;
            }

            // Scale to the requirements of this
            // application. All values are between 0 and 255.
            return new HSV(argb.Alpha, (int)(h / 360 * 255), (int)(s * 255), (int)(v * 255));
        }

        #region Nested type: ARGB

        /// <summary>
        /// The argb.
        /// </summary>
        public struct ARGB
        {
            // All values are between 0 and 255.
            /// <summary>
            /// Initializes a new instance of the <see cref="ARGB"/> struct.
            /// </summary>
            /// <param name="a">
            /// The a.
            /// </param>
            /// <param name="r">
            /// The r.
            /// </param>
            /// <param name="g">
            /// The g.
            /// </param>
            /// <param name="b">
            /// The b.
            /// </param>
            public ARGB(int a, int r, int g, int b)
                : this()
            {
                this.Alpha = a;
                this.Red = r;
                this.Green = g;
                this.Blue = b;
            }

            /// <summary>
            /// Gets or sets the alpha.
            /// </summary>
            public int Alpha { get; set; }

            /// <summary>
            /// Gets or sets the red.
            /// </summary>
            public int Red { get; set; }

            /// <summary>
            /// Gets or sets the green.
            /// </summary>
            public int Green { get; set; }

            /// <summary>
            /// Gets or sets the blue.
            /// </summary>
            public int Blue { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return string.Format("({0}, {1}, {2} {3})", this.Alpha, this.Red, this.Green, this.Blue);
            }
        }

        #endregion Nested type: ARGB

        #region Nested type: HSV

        /// <summary>
        /// The hsv.
        /// </summary>
        public struct HSV
        {
            // All values are between 0 and 255.
            /// <summary>
            /// Initializes a new instance of the <see cref="HSV"/> struct.
            /// </summary>
            /// <param name="a">
            /// The a.
            /// </param>
            /// <param name="h">
            /// The h.
            /// </param>
            /// <param name="s">
            /// The s.
            /// </param>
            /// <param name="v">
            /// The v.
            /// </param>
            public HSV(int a, int h, int s, int v)
                : this()
            {
                this.Alpha = a;
                this.Hue = h;
                this.Saturation = s;
                this.Value = v;
            }

            /// <summary>
            /// Gets or sets the alpha.
            /// </summary>
            public int Alpha { get; set; }

            /// <summary>
            /// Gets or sets the hue.
            /// </summary>
            public int Hue { get; set; }

            /// <summary>
            /// Gets or sets the saturation.
            /// </summary>
            public int Saturation { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return string.Format("({0}, {1}, {2})", this.Hue, this.Saturation, this.Value);
            }
        }

        #endregion Nested type: HSV
    }
}