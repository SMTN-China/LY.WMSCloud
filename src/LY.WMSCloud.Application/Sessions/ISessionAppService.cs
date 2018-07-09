using System.Threading.Tasks;
using Abp.Application.Services;
using LY.WMSCloud.Sessions.Dto;

namespace LY.WMSCloud.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
