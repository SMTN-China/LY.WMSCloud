using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LY.WMSCloud.MultiTenancy.Dto;

namespace LY.WMSCloud.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
