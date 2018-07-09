using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelShortTemps.Dto
{
    public class ReelShortTempMapProfile : Profile
    {
        public ReelShortTempMapProfile()
        {
            CreateMap<ReelShortTemp, ReelShortTempDto>();

            CreateMap<ReelShortTempDto, ReelShortTemp>();
        }
    }
}
