using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.WMS.ProduceData.ReelMoveLogs.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveLogs
{
    public interface IReelMoveLogAppService : IServiceBase<ReelMoveLogDto, string>
    {
        Task<PagedResultDto<ReelMoveLogDto>> GetAllByRMId(string reelMoveMethodId, PagedResultRequestInput input);

    }
}
