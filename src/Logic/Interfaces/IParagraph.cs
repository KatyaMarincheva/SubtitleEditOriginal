
namespace Nikse.SubtitleEdit.Logic.Interfaces
{
    public interface IParagraph
    {
        int Number { get; set; }

        string Text { get; set; }

        TimeCode StartTime { get; set; }

        TimeCode EndTime { get; set; }

        TimeCode Duration { get; }

        int StartFrame { get; set; }

        int EndFrame { get; set; }

        bool Forced { get; set; }

        string Extra { get; set; }

        bool IsComment { get; set; }

        string Actor { get; set; }

        string Effect { get; set; }

        int Layer { get; set; }

        string ID { get; }

        string Language { get; set; }

        string Style { get; set; }

        bool NewSection { get; set; }

        int NumberOfLines { get; }

        double WordsPerMinute { get; }

        string GenerateId();

        void Adjust(double factor, double adjust);

        void CalculateFrameNumbersFromTimeCodes(double frameRate);

        void CalculateTimeCodesFromFrameNumbers(double frameRate);
    }
}