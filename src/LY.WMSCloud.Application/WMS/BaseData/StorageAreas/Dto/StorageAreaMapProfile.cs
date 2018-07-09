using AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.StorageAreas.Dto
{
    public class StorageAreaMapProfile : Profile
    {
        public StorageAreaMapProfile()
        {
            CreateMap<StorageAreaDto, StorageArea>();
            CreateMap<StorageArea, StorageAreaDto>();
        }
    }
}
