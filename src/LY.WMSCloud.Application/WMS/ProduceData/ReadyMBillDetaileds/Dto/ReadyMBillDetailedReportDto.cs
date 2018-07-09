using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBillDetaileds.Dto
{
    public class ReadyMBillDetailedReportDto
    {
        public string ReReadyMBillId { get; set; }
        public string ReadyMBillIds { get; set; }

        public string WorkBillIds { get; set; }

        public string Products { get; set; }

        public string Lines { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string PartNoId { get; set; }

        public string ReelMoveMethodId { get; set; }

        /// <summary>
        /// 合并后的需求数
        /// </summary>
        public int DemandQty { get; set; }

        /// <summary>
        /// 沿用数量
        /// </summary>
        public int FollowQty { get; set; }
        /// <summary>
        /// 发料数量
        /// </summary>
        public int SendQty { get; set; }
        /// <summary>
        /// 非沿用发料数量
        /// </summary>
        public int NoFollowQty { get; set; }

        /// <summary>
        /// 退料数量
        /// </summary>
        public int ReturnQty { get; set; }

        /// <summary>
        /// 实发数量
        /// </summary>
        public int RealSendQty { get; set; }

        /// <summary>
        /// 实发数量
        /// </summary>
        public int MoreSendQty { get; set; }
    }
}
