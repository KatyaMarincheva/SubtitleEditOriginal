namespace Nikse.SubtitleEdit.Logic.Networking
{
    using System;

    public class UpdateLogEntry
    {
        public UpdateLogEntry(int id, string userName, int index, string action)
        {
            this.Id = id;
            this.UserName = userName;
            this.Index = index;
            this.OccuredAt = DateTime.Now;
            this.Action = action;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public int Index { get; set; }

        public DateTime OccuredAt { get; set; }

        public string Action { get; set; }

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}:{2:00} {3}: {4}", this.OccuredAt.Hour, this.OccuredAt.Minute, this.OccuredAt.Second, this.UserName, this.Action);
        }
    }
}