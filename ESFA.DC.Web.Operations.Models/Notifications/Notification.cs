using System;
using System.Globalization;

namespace ESFA.DC.Web.Operations.Models.Notifications
{
    public class Notification
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string Headline { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
