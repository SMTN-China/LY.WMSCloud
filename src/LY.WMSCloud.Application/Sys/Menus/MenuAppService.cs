using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Sys.Menus.Dto;

namespace LY.WMSCloud.Sys.Menus
{
    public class MenuAppService : ServiceBase<Menu, MenuDto, int>, IMenuAppService
    {
        readonly IRepository<RolePermissionSetting, long> _repositoryRP;
        readonly IWMSRepositories<Menu, int> _repository;
        public MenuAppService(
            IWMSRepositories<Menu, int> repository,
            IRepository<RolePermissionSetting, long> repositoryRP,
            IRepository<User, long> userRepository) : base(repository)
        {
            _repositoryRP = repositoryRP;
            _repository = repository;
        }
        public override Task<MenuDto> Create(MenuDto input)
        {
            CheckCreatePermission();
            input.TenantId = AbpSession.TenantId;
            return base.Create(input);
        }


        public async Task<ICollection<MenuDto>> GetChild(int id = -1)
        {
            if (id == -1)
            {
                var res = await _repository.GetAllListAsync(m => m.Group);

                return ObjectMapper.Map<List<MenuDto>>(res);
            }
            else
            {
                var res = await _repository.GetAllListAsync(m => m.ParentId == id);

                return ObjectMapper.Map<List<MenuDto>>(res);
            }
           
        }
        
        public async Task<ICollection<MenuCDto>> GetMenu()
        {
            // 查询角色拥有菜单，及其公共菜单,菜单最多三级
            var menus = await _repository.GetAll()

                .Include(m => m.Children)
                .ThenInclude(m => m.Children)
                .ThenInclude(m => m.Children)
                .OrderBy(m => m.Index)
                .Where(m => m.Group)
                .ToListAsync();
            menus = OrderByIndex(menus);

            return ObjectMapper.Map<List<MenuCDto>>(menus);
        }

        List<Menu> OrderByIndex(List<Menu> menus)
        {
            menus = menus.OrderBy(m => m.Index).ToList();

            foreach (var itemMenus in menus)
            {
                if (itemMenus.Children != null && itemMenus.Children.Count > 0)
                {
                    itemMenus.Children = OrderByIndex(itemMenus.Children.ToList());
                }
            }

            return menus.ToList();
        }
    }
}
