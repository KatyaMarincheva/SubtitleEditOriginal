// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ColorWheel.cs">
//   
// </copyright>
// <summary>
//   The color wheel.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.ColorChooser
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Properties;

    /// <summary>
    /// The color wheel.
    /// </summary>
    public class ColorWheel : IDisposable
    {
        // These resources should be disposed
        // of when you're done with them.
        #region Delegates

        /// <summary>
        /// The color changed event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs e);

        #endregion Delegates

        // Keep track of the current mouse state.
        #region MouseState enum

        /// <summary>
        /// The mouse state.
        /// </summary>
        public enum MouseState
        {
            /// <summary>
            /// The mouse up.
            /// </summary>
            MouseUp, 

            /// <summary>
            /// The click on color.
            /// </summary>
            ClickOnColor, 

            /// <summary>
            /// The drag in color.
            /// </summary>
            DragInColor, 

            /// <summary>
            /// The click on brightness.
            /// </summary>
            ClickOnBrightness, 

            /// <summary>
            /// The drag in brightness.
            /// </summary>
            DragInBrightness, 

            /// <summary>
            /// The click outside region.
            /// </summary>
            ClickOutsideRegion, 

            /// <summary>
            /// The drag outside region.
            /// </summary>
            DragOutsideRegion
        }

        #endregion MouseState enum

        // The code needs to convert back and forth between
        // degrees and radians. There are 2*PI radians in a
        // full circle, and 360 degrees. This constant allows
        // you to convert back and forth.
        /// <summary>
        /// The degree s_ pe r_ radian.
        /// </summary>
        private const double DEGREES_PER_RADIAN = 180.0 / Math.PI;

        // COLOR_COUNT represents the number of distinct colors
        // used to create the circular gradient. Its value
        // is somewhat arbitrary -- change this to 6, for
        // example, to see what happens. 1536 (6 * 256) seems
        // a good compromise -- it's enough to get a full
        // range of colors, but it doesn't overwhelm the processor
        // attempting to generate the image. The color wheel
        // contains 6 sections, and each section displays
        // 256 colors. Seems like a reasonable compromise.
        /// <summary>
        /// The colo r_ count.
        /// </summary>
        private const int COLOR_COUNT = 6 * 256;

        /// <summary>
        /// The brightness max.
        /// </summary>
        private readonly int brightnessMax;

        /// <summary>
        /// The brightness min.
        /// </summary>
        private readonly int brightnessMin;

        /// <summary>
        /// The brightness rectangle.
        /// </summary>
        private readonly Rectangle brightnessRectangle;

        /// <summary>
        /// The brightness region.
        /// </summary>
        private readonly Region brightnessRegion;

        /// <summary>
        /// The brightness scaling.
        /// </summary>
        private readonly double brightnessScaling;

        /// <summary>
        /// The brightness x.
        /// </summary>
        private readonly int brightnessX;

        /// <summary>
        /// The color region.
        /// </summary>
        private readonly Region colorRegion;

        /// <summary>
        /// The radius.
        /// </summary>
        private readonly int radius;

        /// <summary>
        /// The selected color rectangle.
        /// </summary>
        private readonly Rectangle selectedColorRectangle;

        /// <summary>
        /// The argb.
        /// </summary>
        private ColorHandler.ARGB argb;

        // Locations for the two "pointers" on the form.
        /// <summary>
        /// The brightness.
        /// </summary>
        private int brightness = byte.MaxValue;

        /// <summary>
        /// The brightness point.
        /// </summary>
        private Point brightnessPoint;

        /// <summary>
        /// The center point.
        /// </summary>
        private Point centerPoint;

        /// <summary>
        /// The color changed.
        /// </summary>
        public ColorChangedEventHandler ColorChanged;

        /// <summary>
        /// The color image.
        /// </summary>
        private Bitmap colorImage;

        /// <summary>
        /// The color point.
        /// </summary>
        private Point colorPoint;

        /// <summary>
        /// The color rectangle.
        /// </summary>
        private Rectangle colorRectangle;

        /// <summary>
        /// The current state.
        /// </summary>
        private MouseState currentState = MouseState.MouseUp;

        /// <summary>
        /// The full color.
        /// </summary>
        private Color fullColor;

        /// <summary>
        /// The g.
        /// </summary>
        private Graphics g;

        // selectedColor is the actual value selected
        // by the user. fullColor is the same color,
        // with its brightness set to 255.
        /// <summary>
        /// The hsv.
        /// </summary>
        private ColorHandler.HSV HSV;

        /// <summary>
        /// The selected color.
        /// </summary>
        private Color selectedColor = Color.White;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWheel"/> class.
        /// </summary>
        /// <param name="colorRectangle">
        /// The color rectangle.
        /// </param>
        /// <param name="brightnessRectangle">
        /// The brightness rectangle.
        /// </param>
        /// <param name="selectedColorRectangle">
        /// The selected color rectangle.
        /// </param>
        public ColorWheel(Rectangle colorRectangle, Rectangle brightnessRectangle, Rectangle selectedColorRectangle)
        {
            // Caller must provide locations for color wheel
            // (colorRectangle), brightness "strip" (brightnessRectangle)
            // and location to display selected color (selectedColorRectangle).
            using (GraphicsPath path = new GraphicsPath())
            {
                // Store away locations for later use.
                this.colorRectangle = colorRectangle;
                this.brightnessRectangle = brightnessRectangle;
                this.selectedColorRectangle = selectedColorRectangle;

                // Calculate the center of the circle.
                // Start with the location, then offset
                // the point by the radius.
                // Use the smaller of the width and height of
                // the colorRectangle value.
                this.radius = Math.Min(colorRectangle.Width, colorRectangle.Height) / 2;
                this.centerPoint = colorRectangle.Location;
                this.centerPoint.Offset(this.radius, this.radius);

                // Start the pointer in the center.
                this.colorPoint = this.centerPoint;

                // Create a region corresponding to the color circle.
                // Code uses this later to determine if a specified
                // point is within the region, using the IsVisible
                // method.
                path.AddEllipse(colorRectangle);
                this.colorRegion = new Region(path);

                // set { the range for the brightness selector.
                this.brightnessMin = this.brightnessRectangle.Top;
                this.brightnessMax = this.brightnessRectangle.Bottom;

                // Create a region corresponding to the
                // brightness rectangle, with a little extra
                // "breathing room".
                path.AddRectangle(new Rectangle(brightnessRectangle.Left, brightnessRectangle.Top - 10, brightnessRectangle.Width + 10, brightnessRectangle.Height + 20));

                // Create region corresponding to brightness
                // rectangle. Later code uses this to
                // determine if a specified point is within
                // the region, using the IsVisible method.
                this.brightnessRegion = new Region(path);

                // Set the location for the brightness indicator "marker".
                // Also calculate the scaling factor, scaling the height
                // to be between 0 and 255.
                this.brightnessX = brightnessRectangle.Left + brightnessRectangle.Width;
                this.brightnessScaling = (double)255 / (this.brightnessMax - this.brightnessMin);

                // Calculate the location of the brightness
                // pointer. Assume it's at the highest position.
                this.brightnessPoint = new Point(this.brightnessX, this.brightnessMax);

                // Create the bitmap that contains the circular gradient.
                this.CreateGradient();
            }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color
        {
            get
            {
                return this.selectedColor;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The set mouse up.
        /// </summary>
        public void SetMouseUp()
        {
            // Indicate that the user has
            // released the mouse.
            this.currentState = MouseState.MouseUp;
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="HSV">
        /// The hsv.
        /// </param>
        public void Draw(Graphics g, ColorHandler.HSV HSV)
        {
            // Given HSV values, update the screen.
            this.g = g;
            this.HSV = HSV;
            this.CalcCoordsAndUpdate(this.HSV);
            this.UpdateDisplay();
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="argb">
        /// The argb.
        /// </param>
        public void Draw(Graphics g, ColorHandler.ARGB argb)
        {
            // Given RGB values, calculate HSV and then update the screen.
            this.g = g;
            this.HSV = ColorHandler.RGBtoHSV(argb);
            this.CalcCoordsAndUpdate(this.HSV);
            this.UpdateDisplay();
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="mousePoint">
        /// The mouse point.
        /// </param>
        public void Draw(Graphics g, Point mousePoint)
        {
            try
            {
                // You've moved the mouse.
                // Now update the screen to match.

                // Keep track of the previous color pointer point,
                // so you can put the mouse there in case the
                // user has clicked outside the circle.
                Point newColorPoint = this.colorPoint;
                Point newBrightnessPoint = this.brightnessPoint;

                // Store this away for later use.
                this.g = g;

                if (this.currentState == MouseState.MouseUp)
                {
                    if (!mousePoint.IsEmpty)
                    {
                        if (this.colorRegion.IsVisible(mousePoint))
                        {
                            // Is the mouse point within the color circle?
                            // If so, you just clicked on the color wheel.
                            this.currentState = MouseState.ClickOnColor;
                        }
                        else if (this.brightnessRegion.IsVisible(mousePoint))
                        {
                            // Is the mouse point within the brightness area?
                            // You clicked on the brightness area.
                            this.currentState = MouseState.ClickOnBrightness;
                        }
                        else
                        {
                            // Clicked outside the color and the brightness
                            // regions. In that case, just put the
                            // pointers back where they were.
                            this.currentState = MouseState.ClickOutsideRegion;
                        }
                    }
                }

                switch (this.currentState)
                {
                    case MouseState.ClickOnBrightness:
                    case MouseState.DragInBrightness:

                        // Calculate new color information
                        // based on the brightness, which may have changed.
                        Point newPoint = mousePoint;
                        if (newPoint.Y < this.brightnessMin)
                        {
                            newPoint.Y = this.brightnessMin;
                        }
                        else if (newPoint.Y > this.brightnessMax)
                        {
                            newPoint.Y = this.brightnessMax;
                        }

                        newBrightnessPoint = new Point(this.brightnessX, newPoint.Y);
                        this.brightness = (int)((this.brightnessMax - newPoint.Y) * this.brightnessScaling);
                        this.HSV.Value = this.brightness;
                        this.brightness = byte.MaxValue;
                        this.argb = ColorHandler.HSVtoRGB(this.HSV);
                        this.brightness = (this.argb.Red + this.argb.Green + this.argb.Blue) / 3;
                        break;

                    case MouseState.ClickOnColor:
                    case MouseState.DragInColor:

                        // Calculate new color information
                        // based on selected color, which may have changed.
                        newColorPoint = mousePoint;

                        // Calculate x and y distance from the center,
                        // and then calculate the angle corresponding to the
                        // new location.
                        Point delta = new Point(mousePoint.X - this.centerPoint.X, mousePoint.Y - this.centerPoint.Y);
                        int degrees = CalcDegrees(delta);

                        // Calculate distance from the center to the new point
                        // as a fraction of the radius. Use your old friend,
                        // the Pythagorean theorem, to calculate this value.
                        double distance = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y) / this.radius;

                        if (this.currentState == MouseState.DragInColor)
                        {
                            if (distance > 1)
                            {
                                // Mouse is down, and outside the circle, but you
                                // were previously dragging in the color circle.
                                // What to do?
                                // In that case, move the point to the edge of the
                                // circle at the correct angle.
                                distance = 1;
                                newColorPoint = GetPoint(degrees, this.radius, this.centerPoint);
                            }
                        }

                        // Calculate the new HSV and RGB values.
                        this.HSV.Hue = degrees * 255 / 360;
                        this.HSV.Saturation = (int)(distance * 255);
                        this.brightness = byte.MaxValue;
                        this.HSV.Value = this.brightness;
                        this.argb = ColorHandler.HSVtoRGB(this.HSV);
                        if (this.argb.Red < 0 || this.argb.Red > byte.MaxValue || this.argb.Green < 0 || this.argb.Green > byte.MaxValue || this.argb.Blue < 0 || this.argb.Blue > byte.MaxValue)
                        {
                            this.UpdateDisplay();
                            return;
                        }

                        this.brightness = (this.argb.Red + this.argb.Green + this.argb.Blue) / 3;
                        this.fullColor = ColorHandler.HSVtoColor(this.HSV.Alpha, this.HSV.Hue, this.HSV.Saturation, 255);
                        break;
                }

                this.selectedColor = ColorHandler.HSVtoColor(this.HSV);

                // Raise an event back to the parent form,
                // so the form can update any UI it's using
                // to display selected color values.
                this.OnColorChanged(this.argb, this.HSV);

                // On the way out, set the new state.
                switch (this.currentState)
                {
                    case MouseState.ClickOnBrightness:
                        this.currentState = MouseState.DragInBrightness;
                        break;
                    case MouseState.ClickOnColor:
                        this.currentState = MouseState.DragInColor;
                        break;
                    case MouseState.ClickOutsideRegion:
                        this.currentState = MouseState.DragOutsideRegion;
                        break;
                }

                // Store away the current points for next time.
                this.colorPoint = newColorPoint;
                this.brightnessPoint = newBrightnessPoint;

                // Draw the gradients and points.
                this.UpdateDisplay();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// The on color changed.
        /// </summary>
        /// <param name="argb">
        /// The argb.
        /// </param>
        /// <param name="HSV">
        /// The hsv.
        /// </param>
        protected void OnColorChanged(ColorHandler.ARGB argb, ColorHandler.HSV HSV)
        {
            ColorChangedEventArgs e = new ColorChangedEventArgs(argb, HSV);
            this.ColorChanged(this, e);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of graphic resources
                if (this.colorImage != null)
                {
                    this.colorImage.Dispose();
                }

                if (this.colorRegion != null)
                {
                    this.colorRegion.Dispose();
                }

                if (this.brightnessRegion != null)
                {
                    this.brightnessRegion.Dispose();
                }

                if (this.g != null)
                {
                    this.g.Dispose();
                }
            }
        }

        /// <summary>
        /// The calc degrees.
        /// </summary>
        /// <param name="pt">
        /// The pt.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalcDegrees(Point pt)
        {
            int degrees;

            if (pt.X == 0)
            {
                // The point is on the y-axis. Determine whether
                // it's above or below the x-axis, and return the
                // corresponding angle. Note that the orientation of the
                // y-coordinate is backwards. That is, A positive Y value
                // indicates a point BELOW the x-axis.
                if (pt.Y > 0)
                {
                    degrees = 270;
                }
                else
                {
                    degrees = 90;
                }
            }
            else
            {
                // This value needs to be multiplied
                // by -1 because the y-coordinate
                // is opposite from the normal direction here.
                // That is, a y-coordinate that's "higher" on
                // the form has a lower y-value, in this coordinate
                // system. So everything's off by a factor of -1 when
                // performing the ratio calculations.
                degrees = (int)(-Math.Atan((double)pt.Y / pt.X) * DEGREES_PER_RADIAN);

                // If the x-coordinate of the selected point
                // is to the left of the center of the circle, you
                // need to add 180 degrees to the angle. ArcTan only
                // gives you a value on the right-hand side of the circle.
                if (pt.X < 0)
                {
                    degrees += 180;
                }

                // Ensure that the return value is
                // between 0 and 360.
                degrees = (degrees + 360) % 360;
            }

            return degrees;
        }

        /// <summary>
        /// The get colors.
        /// </summary>
        /// <returns>
        /// The <see cref="Color[]"/>.
        /// </returns>
        private static Color[] GetColors()
        {
            // Create an array of COLOR_COUNT
            // colors, looping through all the
            // hues between 0 and 255, broken
            // into COLOR_COUNT intervals. HSV is
            // particularly well-suited for this,
            // because the only value that changes
            // as you create colors is the Hue.
            Color[] Colors = new Color[COLOR_COUNT];

            for (int i = 0; i <= COLOR_COUNT - 1; i++)
            {
                Colors[i] = ColorHandler.HSVtoColor(255, (int)((double)(i * 255) / COLOR_COUNT), 255, 255);
            }

            return Colors;
        }

        /// <summary>
        /// The get points.
        /// </summary>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="centerPoint">
        /// The center point.
        /// </param>
        /// <returns>
        /// The <see cref="Point[]"/>.
        /// </returns>
        private static Point[] GetPoints(double radius, Point centerPoint)
        {
            // Generate the array of points that describe
            // the locations of the COLOR_COUNT colors to be
            // displayed on the color wheel.
            Point[] Points = new Point[COLOR_COUNT];

            for (int i = 0; i <= COLOR_COUNT - 1; i++)
            {
                Points[i] = GetPoint((double)(i * 360) / COLOR_COUNT, radius, centerPoint);
            }

            return Points;
        }

        /// <summary>
        /// The get point.
        /// </summary>
        /// <param name="degrees">
        /// The degrees.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="centerPoint">
        /// The center point.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        private static Point GetPoint(double degrees, double radius, Point centerPoint)
        {
            // Given the center of a circle and its radius, along
            // with the angle corresponding to the point, find the coordinates.
            // In other words, conver  t from polar to rectangular coordinates.
            double radians = degrees / DEGREES_PER_RADIAN;

            return new Point((int)(centerPoint.X + Math.Floor(radius * Math.Cos(radians))), (int)(centerPoint.Y - Math.Floor(radius * Math.Sin(radians))));
        }

        /// <summary>
        /// The calc brightness point.
        /// </summary>
        /// <param name="brightness">
        /// The brightness.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        private Point CalcBrightnessPoint(int brightness)
        {
            // Take the value for brightness (0 to 255), scale to the
            // scaling used in the brightness bar, then add the value
            // to the bottom of the bar. return the correct point at which
            // to display the brightness pointer.
            return new Point(this.brightnessX, (int)(this.brightnessMax - brightness / this.brightnessScaling));
        }

        /// <summary>
        /// The update display.
        /// </summary>
        private void UpdateDisplay()
        {
            // Update the gradients, and place the
            // pointers correctly based on colors and
            // brightness.
            using (Brush selectedBrush = new SolidBrush(this.selectedColor))
            {
                // Draw the saved color wheel image.
                this.g.DrawImage(this.colorImage, this.colorRectangle);

                // Draw the "selected color" rectangle.
                using (TextureBrush textureBrush = new TextureBrush(Resources.TransparentBackground))
                {
                    this.g.FillRectangle(textureBrush, this.selectedColorRectangle);
                }

                this.g.FillRectangle(selectedBrush, this.selectedColorRectangle);
                this.g.DrawRectangle(Pens.Black, this.selectedColorRectangle);

                // Draw the "brightness" rectangle.
                this.DrawLinearGradient(this.fullColor);

                // Draw the two pointers.
                this.DrawColorPointer(this.colorPoint);
                this.DrawBrightnessPointer(this.brightnessPoint);
            }
        }

        /// <summary>
        /// The calc coords and update.
        /// </summary>
        /// <param name="HSV">
        /// The hsv.
        /// </param>
        private void CalcCoordsAndUpdate(ColorHandler.HSV HSV)
        {
            // Convert color to real-world coordinates and then calculate
            // the various points. HSV.Hue represents the degrees (0 to 360),
            // HSV.Saturation represents the radius.
            // This procedure doesn't draw anything--it simply
            // updates class-level variables. The UpdateDisplay
            // procedure uses these values to update the screen.

            // Given the angle (HSV.Hue), and distance from
            // the center (HSV.Saturation), and the center,
            // calculate the point corresponding to
            // the selected color, on the color wheel.
            this.colorPoint = GetPoint((double)HSV.Hue / 255 * 360, (double)HSV.Saturation / 255 * this.radius, this.centerPoint);

            // Given the brightness (HSV.value), calculate the
            // point corresponding to the brightness indicator.
            this.brightnessPoint = this.CalcBrightnessPoint(HSV.Value);

            // Store information about the selected color.
            this.brightness = HSV.Value;
            this.selectedColor = ColorHandler.HSVtoColor(HSV);
            this.argb = ColorHandler.HSVtoRGB(HSV);

            // The full color is the same as HSV, except that the
            // brightness is set to full (255). This is the top-most
            // color in the brightness gradient.
            this.fullColor = ColorHandler.HSVtoColor(HSV.Alpha, HSV.Hue, HSV.Saturation, 255);
        }

        /// <summary>
        /// The draw linear gradient.
        /// </summary>
        /// <param name="TopColor">
        /// The top color.
        /// </param>
        private void DrawLinearGradient(Color TopColor)
        {
            // Given the top color, draw a linear gradient
            // ranging from black to the top color. Use the
            // brightness rectangle as the area to fill.
            using (LinearGradientBrush lgb = new LinearGradientBrush(this.brightnessRectangle, TopColor, Color.Black, LinearGradientMode.Vertical))
            {
                this.g.FillRectangle(lgb, this.brightnessRectangle);
            }
        }

        /// <summary>
        /// The create gradient.
        /// </summary>
        private void CreateGradient()
        {
            // Create a new PathGradientBrush, supplying
            // an array of points created by calling
            // the GetPoints method.
            using (PathGradientBrush pgb = new PathGradientBrush(GetPoints(this.radius, new Point(this.radius, this.radius))))
            {
                // Set the various properties. Note the SurroundColors
                // property, which contains an array of points,
                // in a one-to-one relationship with the points
                // that created the gradient.
                pgb.CenterColor = Color.White;
                pgb.CenterPoint = new PointF(this.radius, this.radius);
                pgb.SurroundColors = GetColors();

                // Create a new bitmap containing
                // the color wheel gradient, so the
                // code only needs to do all this
                // work once. Later code uses the bitmap
                // rather than recreating the gradient.
                this.colorImage = new Bitmap(this.colorRectangle.Width, this.colorRectangle.Height, PixelFormat.Format32bppArgb);

                using (Graphics newGraphics = Graphics.FromImage(this.colorImage))
                {
                    newGraphics.FillEllipse(pgb, 0, 0, this.colorRectangle.Width, this.colorRectangle.Height);
                }
            }
        }

        /// <summary>
        /// The draw color pointer.
        /// </summary>
        /// <param name="pt">
        /// The pt.
        /// </param>
        private void DrawColorPointer(Point pt)
        {
            // Given a point, draw the color selector.
            // The constant SIZE represents half
            // the width -- the square will be twice
            // this value in width and height.
            const int SIZE = 3;
            this.g.DrawRectangle(Pens.Black, pt.X - SIZE, pt.Y - SIZE, SIZE * 2, SIZE * 2);
        }

        /// <summary>
        /// The draw brightness pointer.
        /// </summary>
        /// <param name="pt">
        /// The pt.
        /// </param>
        private void DrawBrightnessPointer(Point pt)
        {
            // Draw a triangle for the
            // brightness indicator that "points"
            // at the provided point.
            const int HEIGHT = 10;
            const int WIDTH = 7;

            Point[] Points = new Point[3];
            Points[0] = pt;
            Points[1] = new Point(pt.X + WIDTH, pt.Y + HEIGHT / 2);
            Points[2] = new Point(pt.X + WIDTH, pt.Y - HEIGHT / 2);
            this.g.FillPolygon(Brushes.Black, Points);
        }
    }
}