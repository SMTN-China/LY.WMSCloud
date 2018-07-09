
using LY.WMSCloud.WMS.ProduceData.ReceivedReelBills.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.Reels.Dto
{
    public class GetReceivedsResult
    {
        public string Msg { get; set; }

        public ICollection<ReceivedReelBillDto> ReceivedReelBills { get; set; }
    }
}
