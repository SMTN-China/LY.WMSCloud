using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    public class ReadyMBillDetailedDto : BaseDto
    {
        public string ReadyMBillId { get; set; }
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
        /// 需求数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 发料数量
        /// </summary>
        public int SendQty { get; set; }
        public bool IsCut { get; set; }
        public string BOMId { get; set; }

        public string SlotId { get; set; }

        public string Suppliers { get; set; }

        /// <summary>
        /// 指定批次号,之间 “|”分割
        /// </summary>
        public string BatchCodes { get; set; }

        /// <summary>
        /// 指定替代料,之间 “|”分割
        /// </summary>
        public string ReplacePNs { get; set; }

        /// <summary>
        /// 优先替代料
        /// </summary>
        public bool PriorityReplacePN { get; set; }

        /// <summary>
        /// 退料数量
        /// </summary>
        public int ReturnQty { get; set; }
        /// <summary>
        /// 沿用数量
        /// </summary>
        public int FollowQty { get; set; }
    }
}
