using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveLogs.Dto
{
    public class ReelMoveLogMapProfile : Profile
    {
        public ReelMoveLogMapProfile()
        {
            CreateMap<ReelMoveLog, ReelMoveLogDto>();
            CreateMap<ReelMoveLogDto, ReelMoveLog>();
        }
    }
}
