using LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills
{
    public interface IReadyMBillAppService : IServiceBase<ReadyMBillDto, string>
    {
        Task<ICollection<ReadyMBillDto>> GetFollowReadyMBillKeyName(string keyName);

        Task<ReadyMResultDto> ReadyM(ReadyMDto readyM);

        Task CancelReadyM(string readyMid);

        Task CloseReadyM(string readyMid);

        Task<ReadyMResultDto> ReadyFirstM(ReadyMDto readyM);

        Task<bool> BatchInsOrUpdate(ICollection<RBBatchReadyMBillDto> input);

        Task<ICollection<ReadyMBillDetailedDto>> GetDetailedById(string id);

        Task<ICollection<ReadyMBillWorkBillMapDto>> GetWorkBillById(string id);
    }
}
