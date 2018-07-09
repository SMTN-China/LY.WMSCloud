using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.ReelMoveLogs.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveLogs
{
    public class ReelMoveLogAppService : ServiceBase<ReelMoveLog, ReelMoveLogDto, string>, IReelMoveLogAppService
    {
        public ReelMoveLogAppService(IWMSRepositories<ReelMoveLog, string> repository) : base(repository)
        {
        }

        [HttpPost]
        public async Task<PagedResultDto<ReelMoveLogDto>> GetAllByRMId(string reelMoveMethodId, PagedResultRequestInput input)
        {
            input.RequestWMSDtos.Add(new RequestWMSDto() { PropertyName = "ReelMoveMethodId", Operation = Operation.Equal, LinkOperation = LinkOperation.And, QueryValue = reelMoveMethodId });

            var res = await GetAll(input);
            return res;
        }
    }
}
