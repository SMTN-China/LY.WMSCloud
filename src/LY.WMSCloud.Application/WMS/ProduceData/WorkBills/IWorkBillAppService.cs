using LY.WMSCloud.WMS.ProduceData.WorkBills.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.WorkBills
{
    public interface IWorkBillAppService : IServiceBase<WorkBillDto, string>
    {
        Task<ICollection<WorkBillDto>> GetWorkBillByKeyName(string keyName);

    }
}
