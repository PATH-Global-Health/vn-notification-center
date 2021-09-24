using AutoMapper;
using Data.MongoCollections;
using Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Notification, NotificationViewModel>().ReverseMap();
            CreateMap<Notification, NotificationAddModel>().ReverseMap();
        }
    }
}
