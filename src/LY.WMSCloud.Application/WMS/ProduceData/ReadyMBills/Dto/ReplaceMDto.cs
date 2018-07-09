using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    public class ReplaceMDto
    {
        public string MainPartNo { get; set; }

        public string ReplacePartNo { get; set; }

        public string Loaction { get; set; }
    }

    public class ErpReplaceMDto
    {
        public string part_no { get; set; }

        public string group_no { get; set; }

        public string location { get; set; }

        public int stat_default { get; set; }
    }
}
