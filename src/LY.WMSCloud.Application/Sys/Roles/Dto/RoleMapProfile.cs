using AutoMapper;
using Abp.Authorization;
using Abp.Authorization.Roles;
using LY.WMSCloud.Authorization.Roles;
using Abp.Dependency;

namespace LY.WMSCloud.Roles.Dto
{
    public class RoleMapProfile : Profile
    {
        public RoleMapProfile()
        {
            // Role and permission
            CreateMap<Permission, string>().ConvertUsing(r => r.Name);
            CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

            CreateMap<CreateRoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
            CreateMap<RoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());

            CreateMap<Permission, PermissionDto>();
            CreateMap<PermissionDto, Permission>();


            CreateMap<Role, RoleDto>().ForMember(x => x.OrgName, opt => opt.MapFrom(x => GetOrgName(x.OrgId)));
        }

        string GetOrgName(int? orgId)
        {
            if (orgId == null)
            {
                return null;
            }
            var repositories = IocManager.Instance.Resolve<IWMSRepositories<Entities.Org, int>>();
            return repositories.Get(orgId.Value).Name;
        }
    }
}
