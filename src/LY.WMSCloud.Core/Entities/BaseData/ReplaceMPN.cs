using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Entities.BaseData
{
    public class ReplaceMPN : EntitieTenantBase
    {
        public string OPartNoId { get; set; }

        public string RPartNoId { get; set; }
        public ReplaceMPNType ReplaceMPNType { get; set; }

        public string ProductId { get; set; }

        public string WorkOrderId { get; set; }

        public string ReadyBillId { get; set; }
    }

    public enum ReplaceMPNType
    {
        GlobalReplace,
        BOMReplace,
        WoReplace
    }
}
