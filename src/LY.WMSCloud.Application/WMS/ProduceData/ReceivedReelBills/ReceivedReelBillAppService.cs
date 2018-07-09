using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.ReceivedReelBills.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReceivedReelBills
{
    public class ReceivedReelBillAppService : ServiceBase<ReceivedReelBill, ReceivedReelBillDto, string>, IReceivedReelBillAppService
    {
        public ReceivedReelBillAppService(IWMSRepositories<ReceivedReelBill, string> repository) : base(repository)
        {
        }
    }
}
