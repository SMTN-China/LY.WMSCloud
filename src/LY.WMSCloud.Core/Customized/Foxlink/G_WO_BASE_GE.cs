using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Customized.Foxlink
{
    public class G_WO_BASE_GE
    {
        public string WORK_ORDER { get; set; }
        public string WO_TYPE { get; set; }

        public string PART_NO { get; set; }

        public string ROUTE_NAME { get; set; }

        public int TARGET_QTY { get; set; }

        public DateTime WO_CREATE_DATE { get; set; }

        public DateTime WO_START_DATE { get; set; }

        public DateTime WO_CLOSE_DATE { get; set; }

        public DateTime WO_CLOSE_DATE_Q { get; set; }

        public int INPUT_QTY { get; set; }

        public int OUTPUT_QTY { get; set; }

        public string WORK_FLAG { get; set; }

        public string WO_STATUS { get; set; }

        public string REMARK { get; set; }

        public int UPDATE_USERID { get; set; }

        public DateTime UPDATE_TIME { get; set; }

        public DateTime RELEASE_DATE { get; set; }

        public string MODEL_NAME { get; set; }

        public int SCRAP_QTY { get; set; }
    }
}
