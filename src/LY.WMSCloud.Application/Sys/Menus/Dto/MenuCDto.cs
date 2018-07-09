using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.Sys.Menus.Dto
{
    [AutoMapTo(typeof(Menu))]
    public class MenuCDto: MenuDto
    {
        public ICollection<MenuCDto> Children { get; set; }
    }
}
