using LY.WMSCloud.WMS.BaseData.Slots.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Slots
{
    public interface ISlotAppService : IServiceBase<SlotDto, string>
    {
        Task<ICollection<BatchSlotListDto>> BatchEdit(BatchSlotDto batchSlot);
    }
}
