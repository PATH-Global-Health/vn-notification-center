using System;
using System.Collections.Generic;
using System.Text;

namespace Data.ViewModels
{
    public class NotificationViewModel
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public bool Seen { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }

        public Guid UserId { get; set; }
    }

    public class NotificationAddModel
    {
        public string Action { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }

    public class NotificationSeenModel
    {
        public Guid NotificationId { get; set; }
    }
}
