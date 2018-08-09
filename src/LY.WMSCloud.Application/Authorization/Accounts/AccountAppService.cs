using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Zero.Configuration;
using LY.WMSCloud.Authorization.Accounts.Dto;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.MultiTenancy.Dto;
using Microsoft.EntityFrameworkCore;

namespace LY.WMSCloud.Authorization.Accounts
{
    public class AccountAppService : WMSCloudAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager)
        {
            _userRegistrationManager = userRegistrationManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        public async Task<ICollection<TenantDto>> GetTenantByKeyName(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                var resE = await TenantManager.Tenants.Take(10).ToListAsync();
                return resE.MapTo<List<TenantDto>>();
            }

            List<TenantDto> host = new List<TenantDto>() { new TenantDto() { Id = 0, Name = "Ö÷»ú", TenancyName = "Host" } };
            var res = await TenantManager.Tenants.Where(t => t.Name.Contains(keyName)).Take(10).ToListAsync();

            return host.Union(res.MapTo<List<TenantDto>>()).ToList();
        }
    }
}
