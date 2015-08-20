// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateLogEntry.cs" company="">
//   
// </copyright>
// <summary>
//   The update log entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Networking
{
    using System;

    /// <summary>
    /// The update log entry.
    /// </summary>
    public class UpdateLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLogEntry"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public UpdateLogEntry(int id, string userName, int index, string action)
        {
            this.Id = id;
            this.UserName = userName;
            this.Index = index;
            this.OccuredAt = DateTime.Now;
            this.Action = action;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the occured at.
        /// </summary>
        public DateTime OccuredAt { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}:{2:00} {3}: {4}", this.OccuredAt.Hour, this.OccuredAt.Minute, this.OccuredAt.Second, this.UserName, this.Action);
        }
    }
}