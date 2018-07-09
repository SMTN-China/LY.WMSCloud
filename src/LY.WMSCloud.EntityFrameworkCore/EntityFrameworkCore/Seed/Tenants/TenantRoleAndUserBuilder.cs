using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using LY.WMSCloud.Authorization;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly WMSCloudDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(WMSCloudDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            // 初始化组织            
            var adminOrgForHost = _context.Orgs.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Code == "Root");
            if (adminOrgForHost == null)
            {
                adminOrgForHost = _context.Orgs.Add(new Entities.Org()
                {
                    Code = "Root",
                    Name = _context.Tenants.FirstOrDefault(t => t.Id == _tenantId).Name,
                    TenantId = _tenantId,
                    IsActive = true
                }).Entity;
                _context.SaveChanges();
            }




            // Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true, OrgId = adminOrgForHost.Id }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new WMSCloudAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = _tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    })
                );
                _context.SaveChanges();
            }

            // Admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                // User account of admin user
                if (_tenantId == 1)
                {
                    _context.UserAccounts.Add(new UserAccount
                    {
                        TenantId = _tenantId,
                        UserId = adminUser.Id,
                        UserName = AbpUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    _context.SaveChanges();
                }
            }

            // 初始化菜单
            var adminMenuForHost = _context.Menus.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId);
            if (adminMenuForHost == null)
            {
                adminMenuForHost = _context.Menus.Add(new Menu()
                {
                    Text = "主菜单",
                    Group = true,
                    I18n = "主菜单",
                    TenantId = _tenantId,
                    Acl = PermissionNames.Pages_MainMenu,
                    IsActive = true,
                    CreatorUserId = adminUser.Id
                }).Entity;

                adminMenuForHost = _context.Menus.Add(new Menu()
                {
                    Text = "系统维护",
                    I18n = "系统维护",
                    Icon = "icon-settings",
                    ParentId = adminMenuForHost.Id,
                    TenantId = _tenantId,
                    Acl = PermissionNames.Pages_Sys,
                    IsActive = true,
                    CreatorUserId = adminUser.Id
                }).Entity;

                _context.Menus.Add(new Menu()
                {
                    Text = "用户管理",
                    I18n = "用户管理",
                    Acl = PermissionNames.Pages_Users,
                    TenantId = _tenantId,
                    Link = "/sys/user",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUser.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "组织管理",
                    I18n = "组织管理",
                    Acl = PermissionNames.Pages_Orgs,
                    TenantId = _tenantId,
                    Link = "/sys/org",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUser.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "菜单管理",
                    I18n = "菜单管理",
                    Acl = PermissionNames.Pages_Menus,
                    TenantId = _tenantId,
                    Link = "/sys/menu",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUser.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "角色管理",
                    I18n = "角色管理",
                    Acl = PermissionNames.Pages_Orgs,
                    TenantId = _tenantId,
                    Link = "/sys/role",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUser.Id
                });
                _context.SaveChanges();
            }

        }
    }
}
