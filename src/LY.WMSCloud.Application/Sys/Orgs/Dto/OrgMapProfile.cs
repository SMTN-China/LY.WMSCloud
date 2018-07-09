using Abp.Dependency;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.Sys.Orgs.Dto
{
    public class OrgMapProfile : Profile
    {
        public OrgMapProfile()
        {
            CreateMap<Org, OrgDto>().ForMember(o => o.ParentName, p => p.MapFrom(o => GetParentName(o.ParentId)));
            
            CreateMap<OrgDto, Org>();
        }

        string GetParentName(int? orgId)
        {
            if (orgId == null)
            {
                return null;
            }
            var repositories = IocManager.Instance.Resolve<IWMSRepositories<Org, int>>();
            return repositories.Get(orgId.Value).Name;
        }
    }
}
