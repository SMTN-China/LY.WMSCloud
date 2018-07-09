using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using LY.WMSCloud.Authorization;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.EntityFrameworkCore.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly WMSCloudDbContext _context;

        public HostRoleAndUserCreator(WMSCloudDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            // 初始化组织
            var adminOrgForHost = _context.Orgs.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Code == StaticRoleNames.Host.Org);
            if (adminOrgForHost == null)
            {
                adminOrgForHost = _context.Orgs.Add(new Entities.Org() { Code = StaticRoleNames.Host.Org, IsActive = true }).Entity;
                _context.SaveChanges();
            }

            // Admin role for host

            var adminRoleForHost = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin)
                {
                    IsStatic = true,
                    IsDefault = true,
                    OrgId = adminOrgForHost.Id
                }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role for host

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == null && p.RoleId == adminRoleForHost.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new WMSCloudAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = null,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRoleForHost.Id
                    })
                );
                _context.SaveChanges();
            }

            // Admin user for host

            var adminUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = AbpUserBase.AdminUserName,
                    Name = "admin",
                    Surname = "admin",
                    EmailAddress = "admin@aspnetboilerplate.com",
                    IsEmailConfirmed = true,
                    IsActive = true
                };

                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, "123qwe");
                user.SetNormalizedNames();

                adminUserForHost = _context.Users.Add(user).Entity;
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                _context.SaveChanges();

                // User account of admin user
                _context.UserAccounts.Add(new UserAccount
                {
                    TenantId = null,
                    UserId = adminUserForHost.Id,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = adminUserForHost.EmailAddress
                });
                _context.SaveChanges();
            }


            // 初始化菜单
            var adminMenuForHost = _context.Menus.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null);
            if (adminMenuForHost == null)
            {
                adminMenuForHost = _context.Menus.Add(new Menu()
                {
                    Text = "主菜单",
                    Group = true,
                    I18n = "主菜单",
                    IsActive = true,
                    Acl = PermissionNames.Pages_MainMenu,
                    CreatorUserId = adminUserForHost.Id
                }).Entity;

                adminMenuForHost = _context.Menus.Add(new Menu()
                {
                    Text = "系统维护",
                    I18n = "系统维护",
                    Icon = "icon-settings",
                    ParentId = adminMenuForHost.Id,
                    Acl = PermissionNames.Pages_Sys,
                    IsActive = true,
                    CreatorUserId = adminUserForHost.Id
                }).Entity;

                _context.Menus.Add(new Menu()
                {
                    Text = "用户管理",
                    I18n = "用户管理",
                    Acl = PermissionNames.Pages_Users,
                    Link = "/sys/user",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUserForHost.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "组织管理",
                    I18n = "组织管理",
                    Acl = PermissionNames.Pages_Orgs,
                    Link = "/sys/org",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUserForHost.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "菜单管理",
                    I18n = "菜单管理",
                    Acl = PermissionNames.Pages_Menus,
                    Link = "/sys/menu",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUserForHost.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "角色管理",
                    I18n = "角色管理",
                    Acl = PermissionNames.Pages_Roles,
                    Link = "/sys/role",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUserForHost.Id
                });
                _context.Menus.Add(new Menu()
                {
                    Text = "租户管理",
                    I18n = "租户管理",
                    Acl = PermissionNames.Pages_Tenants,
                    Link = "/sys/tenant",
                    ParentId = adminMenuForHost.Id,
                    IsActive = true,
                    CreatorUserId = adminUserForHost.Id
                });

                _context.SaveChanges();
            }

        }
    }
}
