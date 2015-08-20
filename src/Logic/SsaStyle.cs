// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SsaStyle.cs" company="">
//   
// </copyright>
// <summary>
//   The ssa style.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Drawing;
    using System.Text;

    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The ssa style.
    /// </summary>
    public class SsaStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SsaStyle"/> class.
        /// </summary>
        public SsaStyle()
        {
            this.FontName = Configuration.Settings.SubtitleSettings.SsaFontName;
            this.FontSize = (int)Configuration.Settings.SubtitleSettings.SsaFontSize;
            this.Primary = Color.FromArgb(Configuration.Settings.SubtitleSettings.SsaFontColorArgb);
            this.Secondary = Color.Yellow;
            this.Outline = Color.Black;
            this.Background = Color.Black;
            this.Alignment = "2";
            this.OutlineWidth = Configuration.Settings.SubtitleSettings.SsaOutline;
            this.ShadowWidth = Configuration.Settings.SubtitleSettings.SsaShadow;
            this.MarginLeft = 10;
            this.MarginRight = 10;
            this.MarginVertical = 10;
            this.BorderStyle = "1";
            if (Configuration.Settings.SubtitleSettings.SsaOpaqueBox)
            {
                this.BorderStyle = "3";
            }

            this.RawLine = string.Empty;
            this.LoadedFromHeader = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SsaStyle"/> class.
        /// </summary>
        /// <param name="ssaStyle">
        /// The ssa style.
        /// </param>
        public SsaStyle(SsaStyle ssaStyle)
        {
            this.Name = ssaStyle.Name;
            this.FontName = ssaStyle.FontName;
            this.FontSize = ssaStyle.FontSize;

            this.Italic = ssaStyle.Italic;
            this.Bold = ssaStyle.Bold;
            this.Underline = ssaStyle.Underline;

            this.Primary = ssaStyle.Primary;
            this.Secondary = ssaStyle.Secondary;
            this.Tertiary = ssaStyle.Tertiary;
            this.Outline = ssaStyle.Outline;
            this.Background = ssaStyle.Background;

            this.ShadowWidth = ssaStyle.ShadowWidth;
            this.OutlineWidth = ssaStyle.OutlineWidth;

            this.Alignment = ssaStyle.Alignment;
            this.MarginLeft = ssaStyle.MarginLeft;
            this.MarginRight = ssaStyle.MarginRight;
            this.MarginVertical = ssaStyle.MarginVertical;

            this.BorderStyle = ssaStyle.BorderStyle;
            this.RawLine = ssaStyle.RawLine;
            this.LoadedFromHeader = ssaStyle.LoadedFromHeader;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether italic.
        /// </summary>
        public bool Italic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bold.
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether underline.
        /// </summary>
        public bool Underline { get; set; }

        /// <summary>
        /// Gets or sets the primary.
        /// </summary>
        public Color Primary { get; set; }

        /// <summary>
        /// Gets or sets the secondary.
        /// </summary>
        public Color Secondary { get; set; }

        /// <summary>
        /// Gets or sets the tertiary.
        /// </summary>
        public Color Tertiary { get; set; }

        /// <summary>
        /// Gets or sets the outline.
        /// </summary>
        public Color Outline { get; set; }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// Gets or sets the shadow width.
        /// </summary>
        public int ShadowWidth { get; set; }

        /// <summary>
        /// Gets or sets the outline width.
        /// </summary>
        public int OutlineWidth { get; set; }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        public string Alignment { get; set; }

        /// <summary>
        /// Gets or sets the margin left.
        /// </summary>
        public int MarginLeft { get; set; }

        /// <summary>
        /// Gets or sets the margin right.
        /// </summary>
        public int MarginRight { get; set; }

        /// <summary>
        /// Gets or sets the margin vertical.
        /// </summary>
        public int MarginVertical { get; set; }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        public string BorderStyle { get; set; }

        /// <summary>
        /// Gets or sets the raw line.
        /// </summary>
        public string RawLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether loaded from header.
        /// </summary>
        public bool LoadedFromHeader { get; set; }

        /// <summary>
        /// The to raw ssa.
        /// </summary>
        /// <param name="styleFormat">
        /// The style format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string ToRawSsa(string styleFormat)
        {
            var sb = new StringBuilder();
            sb.Append("Style: ");
            var format = styleFormat.ToLower().Substring(8).Split(',');
            for (int i = 0; i < format.Length; i++)
            {
                string f = format[i].Trim().ToLower();
                switch (f)
                {
                    case "name":
                        sb.Append(this.Name);
                        break;
                    case "fontname":
                        sb.Append(this.FontName);
                        break;
                    case "fontsize":
                        sb.Append(this.FontSize);
                        break;
                    case "primarycolour":
                        sb.Append(ColorTranslator.ToWin32(this.Primary));
                        break;
                    case "secondarycolour":
                        sb.Append(ColorTranslator.ToWin32(this.Secondary));
                        break;
                    case "tertiarycolour":
                        sb.Append(ColorTranslator.ToWin32(this.Tertiary));
                        break;
                    case "outlinecolour":
                        sb.Append(ColorTranslator.ToWin32(this.Outline));
                        break;
                    case "backcolour":
                        sb.Append(ColorTranslator.ToWin32(this.Background));
                        break;
                    case "bold":
                        sb.Append(Convert.ToInt32(this.Bold));
                        break;
                    case "italic":
                        sb.Append(Convert.ToInt32(this.Italic));
                        break;
                    case "underline":
                        sb.Append(Convert.ToInt32(this.Underline));
                        break;
                    case "outline":
                        sb.Append(this.Outline);
                        break;
                    case "shadow":
                        sb.Append(this.OutlineWidth);
                        break;
                    case "marginl":
                        sb.Append(this.MarginLeft);
                        break;
                    case "marginr":
                        sb.Append(this.MarginRight);
                        break;
                    case "marginv":
                        sb.Append(this.MarginVertical);
                        break;
                    case "borderstyle":
                        sb.Append(this.BorderStyle);
                        break;
                    case "encoding":
                        sb.Append('1');
                        break;
                    case "strikeout":
                        sb.Append('0');
                        break;
                    case "scalex":
                        sb.Append("100");
                        break;
                    case "scaley":
                        sb.Append("100");
                        break;
                    case "spacing":
                        sb.Append('0');
                        break;
                    case "angle":
                        sb.Append('0');
                        break;
                }

                sb.Append(',');
            }

            string s = sb.ToString().Trim();
            return s.Substring(0, s.Length - 1);
        }

        /// <summary>
        /// The to raw ass.
        /// </summary>
        /// <param name="styleFormat">
        /// The style format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string ToRawAss(string styleFormat)
        {
            var sb = new StringBuilder();
            sb.Append("Style: ");
            var format = styleFormat.ToLower().Substring(8).Split(',');
            for (int i = 0; i < format.Length; i++)
            {
                string f = format[i].Trim().ToLower();
                switch (f)
                {
                    case "name":
                        sb.Append(this.Name);
                        break;
                    case "fontname":
                        sb.Append(this.FontName);
                        break;
                    case "fontsize":
                        sb.Append(this.FontSize);
                        break;
                    case "primarycolour":
                        sb.Append(AdvancedSubStationAlpha.GetSsaColorString(this.Primary));
                        break;
                    case "secondarycolour":
                        sb.Append(AdvancedSubStationAlpha.GetSsaColorString(this.Secondary));
                        break;
                    case "tertiarycolour":
                        sb.Append(AdvancedSubStationAlpha.GetSsaColorString(this.Tertiary));
                        break;
                    case "outlinecolour":
                        sb.Append(AdvancedSubStationAlpha.GetSsaColorString(this.Outline));
                        break;
                    case "backcolour":
                        sb.Append(AdvancedSubStationAlpha.GetSsaColorString(this.Background));
                        break;
                    case "bold":
                        sb.Append(Convert.ToInt32(this.Bold));
                        break;
                    case "italic":
                        sb.Append(Convert.ToInt32(this.Italic));
                        break;
                    case "underline":
                        sb.Append(Convert.ToInt32(this.Underline));
                        break;
                    case "outline":
                        sb.Append(this.OutlineWidth);
                        break;
                    case "shadow":
                        sb.Append(this.ShadowWidth);
                        break;
                    case "alignment":
                        sb.Append(this.Alignment);
                        break;
                    case "marginl":
                        sb.Append(this.MarginLeft);
                        break;
                    case "marginr":
                        sb.Append(this.MarginRight);
                        break;
                    case "marginv":
                        sb.Append(this.MarginVertical);
                        break;
                    case "borderstyle":
                        sb.Append(this.BorderStyle);
                        break;
                    case "encoding":
                        sb.Append('1');
                        break;
                    case "strikeout":
                        sb.Append('0');
                        break;
                    case "scalex":
                        sb.Append("100");
                        break;
                    case "scaley":
                        sb.Append("100");
                        break;
                    case "spacing":
                        sb.Append('0');
                        break;
                    case "angle":
                        sb.Append('0');
                        break;
                }

                sb.Append(',');
            }

            string s = sb.ToString().Trim();
            return s.Substring(0, s.Length - 1);
        }
    }
}