using Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Core
{
    public interface INotificationService
    {
        ResultModel Add(NotificationAddModel model);
    }
    public class NotificationService : INotificationService
    {
        public ResultModel Add(NotificationAddModel model)
        {
            return null;
        }
    }
}
