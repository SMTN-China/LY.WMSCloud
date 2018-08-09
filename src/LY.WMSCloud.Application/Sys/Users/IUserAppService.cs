using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Roles.Dto;
using LY.WMSCloud.Sys.Orgs.Dto;
using LY.WMSCloud.Users.Dto;

namespace LY.WMSCloud.Users
{
    public interface IUserAppService : IServiceBase<UserDto, long, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<ICollection<RoleDto>> GetRole(long id);

        Task<ICollection<OrgDto>> GetOrgRole(long id);

        Task<UserDto> ChangePwd(string oldPwd, string newPwd);

        Task<UserDto> ChangeUserInfoAsync(UserDto user);

        Task RePwd(long id);

    }
}
