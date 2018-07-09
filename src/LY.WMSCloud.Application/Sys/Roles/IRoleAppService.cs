using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Roles.Dto;

namespace LY.WMSCloud.Roles
{
    public interface IRoleAppService : IServiceBase<RoleDto, int, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();

        Task<ICollection<string>> GetPermissions(int id);
    }
}
