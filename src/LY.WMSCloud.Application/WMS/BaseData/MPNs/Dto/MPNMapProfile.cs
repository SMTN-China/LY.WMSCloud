using AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.MPNs.Dto
{
    public class MPNMapProfile : Profile
    {
        public MPNMapProfile()
        {
            CreateMap<MPNDto, MPN>();
            CreateMap<MPN, MPNDto>();
        }
    }
}
