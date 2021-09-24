using AutoMapper;
using Data.DataAccess;
using Data.MongoCollections;
using Data.ViewModels;
using MongoDB.Driver;
using Services.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Core
{
    public interface INotificationService
    {
        ResultModel Get(Guid userId, int pageIndex, int pageSize);
        ResultModel Add(NotificationAddModel model);
        ResultModel Seen(Guid userId, Guid notificationId);
        ResultModel CountUnSeen(Guid userId);

        ResultModel Delete(Guid id);
    }
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _dbContext;
        private readonly INotificationHub _notificationHub;
        private readonly IMapper _mapper;

        public NotificationService(AppDbContext dbContext, INotificationHub notificationHub, IMapper mapper)
        {
            _dbContext = dbContext;
            _notificationHub = notificationHub;
            _mapper = mapper;
        }

        public ResultModel Get(Guid userId, int pageIndex, int pageSize)
        {
            var result = new ResultModel();
            try
            {
                var notifies = _dbContext.Notifications.Find(f => f.UserId == userId && f.IsDeleted == false)
                     .ToList()
                     .OrderByDescending(_n => _n.DateCreated)
                     .ToList();

                result.Data = new PagingModel()
                {
                    Data = _mapper.Map<List<Notification>, List<NotificationViewModel>>(notifies.Skip((pageIndex) * pageSize).Take(pageSize).ToList()),
                    PageCount = (int)Math.Ceiling((double)notifies.Count / pageSize)
                };
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessages = e.Message + "\n" + (e.InnerException != null ? e.InnerException.Message : "") + "\n ***Trace*** \n" + e.StackTrace;
            }
            return result;
        }
        public ResultModel Add(NotificationAddModel model)
        {
            var result = new ResultModel();
            try
            {
                var notify = _mapper.Map<NotificationAddModel, Notification>(model);

                _dbContext.Notifications.InsertOne(notify);

                var notifyView = _mapper.Map<Notification, NotificationViewModel>(notify);

                _notificationHub.NewNotification(notifyView, model.UserId + "");

                var newNotifiCount = _dbContext.Notifications.CountDocuments(f => f.UserId == model.UserId && f.IsDeleted == false && f.Seen == false);

                _notificationHub.NewNotificationCount(int.Parse(newNotifiCount.ToString()), model.UserId + "");

                result.Data = notify.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessages = e.Message + "\n" + (e.InnerException != null ? e.InnerException.Message : "") + "\n ***Trace*** \n" + e.StackTrace;
            }
            return result;
        }
        public ResultModel Seen(Guid userId, Guid notificationId)
        {
            var result = new ResultModel();
            try
            {
                var notification = _dbContext.Notifications.Find(f => f.UserId == userId && f.Id == notificationId).FirstOrDefault();
                if (notification == null) throw new Exception("Invalid notification id");

                notification.DateUpdated = DateTime.Now;
                notification.Seen = true;

                _dbContext.Notifications.FindOneAndReplace(f => f.Id == notification.Id, notification);

                result.Data = notification.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessages = e.Message + "\n" + (e.InnerException != null ? e.InnerException.Message : "") + "\n ***Trace*** \n" + e.StackTrace;
            }
            return result;
        }
        public ResultModel CountUnSeen(Guid userId)
        {
            var result = new ResultModel();
            try
            {
                var unSeen = _dbContext.Notifications.CountDocuments(f => f.UserId == userId && f.IsDeleted == false && f.Seen == false);

                result.Data = unSeen;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessages = e.Message + "\n" + (e.InnerException != null ? e.InnerException.Message : "") + "\n ***Trace*** \n" + e.StackTrace;
            }
            return result;
        }
        public ResultModel Delete(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var notification = _dbContext.Notifications.Find(f => f.Id == id).FirstOrDefault();
                if (notification == null) throw new Exception("Invalid notification id");

                notification.DateUpdated = DateTime.Now;
                notification.IsDeleted = true;

                _dbContext.Notifications.FindOneAndReplace(f => f.Id == notification.Id, notification);

                result.Data = notification.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessages = e.Message + "\n" + (e.InnerException != null ? e.InnerException.Message : "") + "\n ***Trace*** \n" + e.StackTrace;
            }
            return result;
        }
    }
}
