using AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.StorageLocationTypes.Dto
{
    public class StorageLocationTypeMapProfile : Profile
    {
        public StorageLocationTypeMapProfile()
        {
            CreateMap<StorageLocationTypeDto, StorageLocationType>();
            CreateMap<StorageLocationType, StorageLocationTypeDto>();
        }
    }
}
