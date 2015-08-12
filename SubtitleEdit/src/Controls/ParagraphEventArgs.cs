namespace Nikse.SubtitleEdit.Controls
{
    using System;

    using Nikse.SubtitleEdit.Logic;

    public class ParagraphEventArgs : EventArgs
    {
        public Paragraph Paragraph
        {
            get; private set;
        }
        public double Seconds
        {
            get; private set;
        }
        public Paragraph BeforeParagraph
        {
            get; set;
        }
        public MouseDownParagraphType MouseDownParagraphType
        {
            get; set;
        }
        public bool MovePreviousOrNext
        {
            get; set;
        }
        public ParagraphEventArgs(Paragraph p)
        {
            Paragraph = p;
        }
        public ParagraphEventArgs(double seconds, Paragraph p)
        {
            Seconds = seconds;
            Paragraph = p;
        }
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b)
        {
            Seconds = seconds;
            Paragraph = p;
            BeforeParagraph = b;
        }
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b, MouseDownParagraphType mouseDownParagraphType)
        {
            Seconds = seconds;
            Paragraph = p;
            BeforeParagraph = b;
            MouseDownParagraphType = mouseDownParagraphType;
        }
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b, MouseDownParagraphType mouseDownParagraphType, bool movePreviousOrNext)
        {
            Seconds = seconds;
            Paragraph = p;
            BeforeParagraph = b;
            MouseDownParagraphType = mouseDownParagraphType;
            MovePreviousOrNext = movePreviousOrNext;
        }
    }
}