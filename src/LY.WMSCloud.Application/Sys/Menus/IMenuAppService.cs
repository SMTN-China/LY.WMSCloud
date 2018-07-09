using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LY.WMSCloud.Sys.Menus.Dto;

namespace LY.WMSCloud.Sys.Menus
{
    public interface IMenuAppService : IServiceBase<MenuDto, int>
    {
        Task<ICollection<MenuCDto>> GetMenu();

        Task<ICollection<MenuDto>> GetChild(int id = -1);
    }
}
