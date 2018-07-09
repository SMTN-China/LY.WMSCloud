using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.Reels.Dto
{
    public class ReelMoveDto
    {
        public string BarCode { get; set; }

        public string IQCCheckId { get; set; }

        public string ShlefLab { get; set; }

        public int ExtendShelfLife { get; set; }

        public string ReelMoveMethodId { get; set; }

        public bool IsContinuity { get; set; }

        public bool IsReturnReel { get; set; }

        public int ReturnReelQty { get; set; }
    }
}
