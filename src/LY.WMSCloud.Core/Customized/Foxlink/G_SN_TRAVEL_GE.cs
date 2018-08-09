using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Customized.Foxlink
{
    public class G_SN_TRAVEL_GE
    {
        public string WORK_ORDER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string PART_NO { get; set; }
        public string MODEL_NAME { get; set; }
        public string ROUTE_NAME { get; set; }
        public string PDLINE_NAME { get; set; }
        public string PROCESS_NAME { get; set; }
        public string CURRENT_STATUS { get; set; }
        public string WORK_FLAG { get; set; }
        public DateTime IN_PROCESS_TIME { get; set; }
        public DateTime OUT_PROCESS_TIME { get; set; }
        public DateTime IN_PDLINE_TIME { get; set; }
        public DateTime OUT_PDLINE_TIME { get; set; }
        public int ENC_CNT { get; set; }
        public string PALLET_NO { get; set; }
        public string CARTON_NO { get; set; }
        public string REWORK_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string CUSTOMER_SN { get; set; }
        public int WIP_QTY { get; set; }
        public string BOX_NO { get; set; }
    }
}
