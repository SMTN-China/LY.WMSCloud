using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.PrintReels.Dto
{
    public class PrintReelMapProfile : Profile
    {
        public PrintReelMapProfile()
        {
            //var IRepository<Setting, long> _repositoryT//
            CreateMap<PrintReel, PrintReelDto>();

            //CreateMap<Reel, ReelOutLifeDto>().ForMember(x => x.ShelfLife, opt => opt.MapFrom(x => x.PartNo.ShelfLife));

            CreateMap<PrintReelDto, PrintReel>();
        }

    }
}
