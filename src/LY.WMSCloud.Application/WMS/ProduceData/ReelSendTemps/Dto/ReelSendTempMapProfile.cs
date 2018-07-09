using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelSendTemps.Dto
{

    public class ReelSendTempMapProfile : Profile
    {
        public ReelSendTempMapProfile()
        {
            CreateMap<ReelSendTemp, ReelSendTempDto>();

            CreateMap<ReelSendTempDto, ReelSendTemp>();
        }
    }
}
