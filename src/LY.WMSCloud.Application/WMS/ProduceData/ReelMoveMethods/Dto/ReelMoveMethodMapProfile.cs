using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveMethods.Dto
{
    public class ReelMoveMethodMapProfile : Profile
    {

        public ReelMoveMethodMapProfile()
        {
            CreateMap<ReelMoveMethod, ReelMoveMethodDto>()
                .ForMember(m => m.OutStorageIds, opt => opt.MapFrom(s => string.Join(" | ", s.OutStorages.Select(w => w.StorageId))));

            CreateMap<ReelMoveMethodDto, ReelMoveMethod>()
                 .ForMember(m => m.AllocationTypesStr, opt => opt.MapFrom(s => string.Join(" | ", s.AllocationTypes.Select(e => e.ToString())))); ;

            CreateMap<RMMStorageMap, RMMStorageMapDto>();

            CreateMap<RMMStorageMapDto, RMMStorageMap>();
        }
    }
}
