using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    public class ReadyMResultDto
    {
        public string ReReadyMBillId { get; set; }
        public bool Success { get; set; }

        public string Msg { get; set; }
    }
}
