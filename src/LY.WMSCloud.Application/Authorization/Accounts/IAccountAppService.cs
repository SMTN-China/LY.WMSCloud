using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using LY.WMSCloud.Authorization.Accounts.Dto;
using LY.WMSCloud.MultiTenancy.Dto;

namespace LY.WMSCloud.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        Task<ICollection<TenantDto>> GetTenantByKeyName(string keyName);

    }
}
