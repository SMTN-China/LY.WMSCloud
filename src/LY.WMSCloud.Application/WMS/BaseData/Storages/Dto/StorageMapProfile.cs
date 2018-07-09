using AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.Storages.Dto
{
    class StorageMapProfile : Profile
    {
        public StorageMapProfile()
        {
            CreateMap<StorageDto, Storage>();

            CreateMap<Storage, StorageDto>();

        }
    }
}
