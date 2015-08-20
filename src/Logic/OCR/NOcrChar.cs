﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NOcrChar.cs" company="">
//   
// </copyright>
// <summary>
//   The n ocr char.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The n ocr char.
    /// </summary>
    public class NOcrChar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrChar"/> class.
        /// </summary>
        public NOcrChar()
        {
            this.LinesForeground = new List<NOcrPoint>();
            this.LinesBackground = new List<NOcrPoint>();
            this.Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrChar"/> class.
        /// </summary>
        /// <param name="old">
        /// The old.
        /// </param>
        public NOcrChar(NOcrChar old)
        {
            this.LinesForeground = new List<NOcrPoint>();
            this.LinesBackground = new List<NOcrPoint>();
            this.Text = old.Text;
            this.Width = old.Width;
            this.Height = old.Height;
            this.MarginTop = old.MarginTop;
            this.Italic = old.Italic;
            foreach (NOcrPoint p in old.LinesForeground)
            {
                this.LinesForeground.Add(new NOcrPoint(new Point(p.Start.X, p.Start.Y), new Point(p.End.X, p.End.Y)));
            }

            foreach (NOcrPoint p in old.LinesBackground)
            {
                this.LinesBackground.Add(new NOcrPoint(new Point(p.Start.X, p.Start.Y), new Point(p.End.X, p.End.Y)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrChar"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public NOcrChar(string text)
            : this()
        {
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NOcrChar"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public NOcrChar(Stream stream)
        {
            try
            {
                byte[] buffer = new byte[9];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read < buffer.Length)
                {
                    this.LoadedOk = false;
                    return;
                }

                this.Width = buffer[0] << 8 | buffer[1];
                this.Height = buffer[2] << 8 | buffer[3];

                this.MarginTop = buffer[4] << 8 | buffer[5];

                this.Italic = buffer[6] != 0;

                this.ExpandCount = buffer[7];

                int textLen = buffer[8];
                if (textLen > 0)
                {
                    buffer = new byte[textLen];
                    stream.Read(buffer, 0, buffer.Length);
                    this.Text = Encoding.UTF8.GetString(buffer);
                }

                this.LinesForeground = ReadPoints(stream);
                this.LinesBackground = ReadPoints(stream);

                this.LoadedOk = true;
            }
            catch
            {
                this.LoadedOk = false;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the margin top.
        /// </summary>
        public int MarginTop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether italic.
        /// </summary>
        public bool Italic { get; set; }

        /// <summary>
        /// Gets the lines foreground.
        /// </summary>
        public List<NOcrPoint> LinesForeground { get; private set; }

        /// <summary>
        /// Gets the lines background.
        /// </summary>
        public List<NOcrPoint> LinesBackground { get; private set; }

        /// <summary>
        /// Gets or sets the expand count.
        /// </summary>
        public int ExpandCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether loaded ok.
        /// </summary>
        public bool LoadedOk { get; private set; }

        /// <summary>
        /// Gets the width percent.
        /// </summary>
        public double WidthPercent
        {
            get
            {
                return this.Height * 100.0 / this.Width;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is sensitive.
        /// </summary>
        public bool IsSensitive
        {
            get
            {
                return this.Text == "." || this.Text == "," || this.Text == "'" || this.Text == "-" || this.Text == ":" || this.Text == "\"";
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Text;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        internal void Save(Stream stream)
        {
            WriteInt16(stream, (ushort)this.Width);
            WriteInt16(stream, (ushort)this.Height);

            WriteInt16(stream, (ushort)this.MarginTop);

            stream.WriteByte(Convert.ToByte(this.Italic));
            stream.WriteByte(Convert.ToByte(this.ExpandCount));

            if (this.Text == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                byte[] textBuffer = Encoding.UTF8.GetBytes(this.Text);
                stream.WriteByte((byte)textBuffer.Length);
                stream.Write(textBuffer, 0, textBuffer.Length);
            }

            WritePoints(stream, this.LinesForeground);
            WritePoints(stream, this.LinesBackground);
        }

        /// <summary>
        /// The read points.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<NOcrPoint> ReadPoints(Stream stream)
        {
            List<NOcrPoint> list = new List<NOcrPoint>();
            int length = stream.ReadByte() << 8 | stream.ReadByte();
            byte[] buffer = new byte[8];
            for (int i = 0; i < length; i++)
            {
                stream.Read(buffer, 0, buffer.Length);
                NOcrPoint point = new NOcrPoint { Start = new Point(buffer[0] << 8 | buffer[1], buffer[2] << 8 | buffer[3]), End = new Point(buffer[4] << 8 | buffer[5], buffer[6] << 8 | buffer[7]) };
                list.Add(point);
            }

            return list;
        }

        /// <summary>
        /// The write points.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="points">
        /// The points.
        /// </param>
        private static void WritePoints(Stream stream, List<NOcrPoint> points)
        {
            WriteInt16(stream, (ushort)points.Count);
            foreach (NOcrPoint nOcrPoint in points)
            {
                WriteInt16(stream, (ushort)nOcrPoint.Start.X);
                WriteInt16(stream, (ushort)nOcrPoint.Start.Y);
                WriteInt16(stream, (ushort)nOcrPoint.End.X);
                WriteInt16(stream, (ushort)nOcrPoint.End.Y);
            }
        }

        /// <summary>
        /// The write int 16.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        private static void WriteInt16(Stream stream, ushort val)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)((val & 0xFF00) >> 8);
            buffer[1] = (byte)(val & 0x00FF);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}