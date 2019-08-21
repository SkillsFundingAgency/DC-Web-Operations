using System;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class Schedule
    {
        public long Id { get; set; }

        public bool Enabled { get; set; }

        public bool? MinuteIsCadence { get; set; }

        public byte? Minute { get; set; }

        public byte? Hour { get; set; }

        public byte? DayOfTheMonth { get; set; }

        public byte? Month { get; set; }

        public byte? DayOfTheWeek { get; set; }

        public int CollectionId { get; set; }

        public bool ExecuteOnceOnly { get; set; }

        public DateTime? LastExecuteDateTime { get; set; }

        public bool Paused { get; set; }

        public virtual Collection Collection { get; set; }
    }
}
