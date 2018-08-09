using Abp.Dependency;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.Sys.Menus.Dto
{
    public class MenuMapProfile : Profile
    {
        public MenuMapProfile()
        {
            CreateMap<Menu, MenuDto>().ForMember(m => m.ParentName, o => o.MapFrom(m => GetParentName(m.ParentId)))
                .ForMember(m => m.Name, o => o.MapFrom(m => m.Text));

            CreateMap<Menu, MenuCDto>().ForMember(m => m.Name, o => o.MapFrom(m => m.Text))

                .ForMember(m => m.Hide, o => o.MapFrom(m => !m.IsActive));

            CreateMap<MenuDto, Menu>();
        }

        string GetParentName(int? menuId)
        {
            if (menuId == null)
            {
                return null;
            }
            var repositories = IocManager.Instance.Resolve<IWMSRepositories<Menu, int>>();
            return repositories.Get(menuId.Value).Text;
        }
    }
}
