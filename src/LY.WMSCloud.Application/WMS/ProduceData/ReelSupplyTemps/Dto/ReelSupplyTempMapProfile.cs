using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelSupplyTemps.Dto
{
    public class ReelSupplyTempMapProfile : Profile
    {
        public ReelSupplyTempMapProfile()
        {
            CreateMap<ReelSupplyTemp, ReelSupplyTempDto>();

            CreateMap<ReelSupplyTempDto, ReelSupplyTemp>();
        }
    }
}
