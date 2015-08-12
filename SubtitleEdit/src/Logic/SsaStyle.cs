using System.Drawing;
using System.Text;
using Nikse.SubtitleEdit.Logic.SubtitleFormats;
using System;

namespace Nikse.SubtitleEdit.Logic
{
    public class SsaStyle
    {
        public string Name { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public bool Underline { get; set; }
        public Color Primary { get; set; }
        public Color Secondary { get; set; }
        public Color Tertiary { get; set; }
        public Color Outline { get; set; }
        public Color Background { get; set; }
        public int ShadowWidth { get; set; }
        public int OutlineWidth { get; set; }
        public string Alignment { get; set; }
        public int MarginLeft { get; set; }
        public int MarginRight { get; set; }
        public int MarginVertical { get; set; }
        public string BorderStyle { get; set; }
        public string RawLine { get; set; }
        public bool LoadedFromHeader { get; set; }

        public SsaStyle()
        {
            FontName = Configuration.Settings.SubtitleSettings.SsaFontName;
            FontSize = (int)Configuration.Settings.SubtitleSettings.SsaFontSize;
            Primary = Color.FromArgb(Configuration.Settings.SubtitleSettings.SsaFontColorArgb);
            Secondary = Color.Yellow;
            Outline = Color.Black;
            Background = Color.Black;
            Alignment = "2";
            OutlineWidth = Configuration.Settings.SubtitleSettings.SsaOutline;
            ShadowWidth = Configuration.Settings.SubtitleSettings.SsaShadow;
            MarginLeft = 10;
            MarginRight = 10;
            MarginVertical = 10;
            BorderStyle = "1";
            if (Configuration.Settings.SubtitleSettings.SsaOpaqueBox)
                BorderStyle = "3";
            RawLine = string.Empty;
            LoadedFromHeader = false;
        }

        public SsaStyle(SsaStyle ssaStyle)
        {
            Name = ssaStyle.Name;
            FontName = ssaStyle.FontName;
            FontSize = ssaStyle.FontSize;

            Italic = ssaStyle.Italic;
            Bold = ssaStyle.Bold;
            Underline = ssaStyle.Underline;

            Primary = ssaStyle.Primary;
            Secondary = ssaStyle.Secondary;
            Tertiary = ssaStyle.Tertiary;
            Outline = ssaStyle.Outline;
            Background = ssaStyle.Background;

            ShadowWidth = ssaStyle.ShadowWidth;
            OutlineWidth = ssaStyle.OutlineWidth;

            Alignment = ssaStyle.Alignment;
            MarginLeft = ssaStyle.MarginLeft;
            MarginRight = ssaStyle.MarginRight;
            MarginVertical = ssaStyle.MarginVertical;

            BorderStyle = ssaStyle.BorderStyle;
            RawLine = ssaStyle.RawLine;
            LoadedFromHeader = ssaStyle.LoadedFromHeader;
        }

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