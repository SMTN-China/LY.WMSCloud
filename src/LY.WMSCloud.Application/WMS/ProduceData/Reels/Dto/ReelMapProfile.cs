using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.Reels.Dto
{
    public class ReelMapProfile : Profile
    {
        public ReelMapProfile()
        {
            //var IRepository<Setting, long> _repositoryT//
            CreateMap<Reel, ReelDto>();

            //CreateMap<Reel, ReelOutLifeDto>().ForMember(x => x.ShelfLife, opt => opt.MapFrom(x => x.PartNo.ShelfLife));

            CreateMap<ReelDto, Reel>();
        }

    }
}
