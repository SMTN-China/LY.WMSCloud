using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.WMS.ProduceData.ReadyMBillDetaileds.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBillDetaileds
{
    public interface IReadyMBillDetailedAppService
    {
        Task<PagedResultDto<ReadyMBillDetailedReportDto>> GetAllAsync(string readyBillId, PagedResultRequestInput input);

    }
}
