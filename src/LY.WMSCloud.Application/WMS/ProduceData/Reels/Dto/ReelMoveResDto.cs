using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.Reels.Dto
{
    public class ReelMoveResDto
    {
        public string NextShlefLab { get; set; }

        public ReelDto Reel { get; set; }

        public bool IsContinuity { get; set; }

        public string Msg { get; set; }
    }
}
