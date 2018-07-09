using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.UPHs.Dto
{
    public class UPHMapProfile : Profile
    {
        public UPHMapProfile()
        {
            CreateMap<UPH, UPHDto>();

            CreateMap<UPHDto, UPH>();
        }
    }
}
