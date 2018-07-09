using AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.StorageLocations.Dto
{
    public class MShelfMapProfile : Profile
    {
        public MShelfMapProfile()
        {
            CreateMap<StorageLocationDto, StorageLocation>();
            CreateMap<StorageLocation, StorageLocationDto>();

        }
    }
}
