using AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.BOMs.Dto
{
    public class BOMMapProfile : Profile
    {
        public BOMMapProfile()
        {
            CreateMap<string, MPN>().ConstructUsing(s => null);

            CreateMap<ProductDto, MPN>();

            CreateMap<BOM, ProductDto>();

            CreateMap<MPN, ProductDto>();

            CreateMap<BOM, BOMDto>();

            CreateMap<BOMDto, BOM>();
        }
    }
}
