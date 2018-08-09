using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Runtime.Session;
using LY.WMSCloud.Authorization;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.Roles.Dto;
using LY.WMSCloud.Users.Dto;
using LY.WMSCloud.Entities;
using Abp.Linq.Extensions;
using LY.WMSCloud.Sys.Orgs.Dto;
using Microsoft.AspNetCore.Mvc;
using Abp.MultiTenancy;

namespace LY.WMSCloud.Users
{
    public class UserAppService : ServiceBase<User, UserDto, long, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWMSRepositories<User, long> _mesRepositories;
        private readonly IRepository<Org> _orgRepository;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;


        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IRepository<Org> orgRepository,
            IWMSRepositories<User, long> mesRepositories,
            LogInManager logInManager,
            IPasswordHasher<User> passwordHasher,
            ITenantCache tenantCache,
            UserRegistrationManager userRegistrationManager
            )

            : base(mesRepositories)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _tenantCache = tenantCache;
            _mesRepositories = mesRepositories;
            _userRegistrationManager = userRegistrationManager;
            _orgRepository = orgRepository;
            _logInManager = logInManager;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;

            CheckErrors(await _userManager.CreateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();



            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return await Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            if (input.LanguageName == "zh-CN")
            {
                input.LanguageName = "zh-Hans";
            }
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestInput input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            return await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestInput input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<ICollection<RoleDto>> GetRole(long id)
        {
            var roleNames = await _userManager.GetRolesAsync(new User() { Id = id });

            var roles = await _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync();

            return ObjectMapper.Map<List<RoleDto>>(roles);
        }

        public async Task<ICollection<OrgDto>> GetOrgRole(long id)
        {
            // 获取用户角色
            var roleNames = await _userManager.GetRolesAsync(new User() { Id = id });

            // 获取组织
            var uOrgs = await _roleManager.Roles.Include(r => r.Org).Where(r => roleNames.Contains(r.Name)).Select(r => r.Org.Id).ToListAsync();

            // 获取组织角色
            var uOrgRoles = await _orgRepository.GetAllIncluding(r => r.Roles).Where(r => uOrgs.Contains(r.Id)).ToListAsync();


            return ObjectMapper.Map<List<OrgDto>>(uOrgRoles);
        }

        public async Task<UserDto> ChangePwd(string oldPwd, string newPwd)
        {
            // 获取当前用户
            var user = await UserManager.FindByIdAsync(AbpSession.UserId.ToString());

            var loginResult = await _logInManager.LoginAsync(user.UserName, oldPwd, GetTenancyNameOrNull());

            // 校验旧密码是否正确
            if (loginResult.Identity == null)
            {
                throw new LYException("旧密码输入错误");
            }

            var res = await UserManager.ChangePasswordAsyncNoValid(user, newPwd);
            if (res.Succeeded)
            {
                return ObjectMapper.Map<UserDto>(loginResult.User);
            }
            else
            {
                throw new LYException(res.Errors);
            }
        }
        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        [HttpPut]
        public async Task<UserDto> ChangeUserInfoAsync(UserDto input)
        {

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return ObjectMapper.Map<UserDto>(user);
        }
        [HttpPost]
        public async Task RePwd(long id)
        {
            await UserManager.ChangePasswordAsyncNoValid(await _userManager.FindByIdAsync(id.ToString()), "123qwe");
        }
    }
}
