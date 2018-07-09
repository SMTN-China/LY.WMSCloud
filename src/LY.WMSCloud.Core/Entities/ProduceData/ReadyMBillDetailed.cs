using Abp.Domain.Entities;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    /// <summary>
    /// 备料单详细
    /// </summary>
    public class ReadyMBillDetailed : EntitieTenantBase
    {
        /// <summary>
        /// 备料单号
        /// </summary>
        [StringLength(36)]
        public string ReadyMBillId { get; set; }
        public virtual ReadyMBill ReadyMBill { get; set; }


        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }
        public virtual MPN PartNo { get; set; }


        /// <summary>
        /// 调拨方式
        /// </summary>
        [StringLength(36)]
        public string ReelMoveMethodId { get; set; }
        public virtual ReelMoveMethod ReelMoveMethod { get; set; }


        /// <summary>
        /// 合并后的需求数,记账用
        /// </summary>
        public int DemandQty { get; set; }

        /// <summary>
        /// 单条需求数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 发料数量
        /// </summary>
        public int SendQty { get; set; }

        /// <summary>
        /// BOM编号,用BOM生成时会记录
        /// </summary>
        [StringLength(36)]

        public string BOMId { get; set; }
        public virtual BOM BOM { get; set; }
        /// <summary>
        /// BOM编号,用BOM生成时会记录
        /// </summary>
        [StringLength(36)]

        public string SlotId { get; set; }
        public virtual Slot Slot { get; set; }

        /// <summary>
        /// 指定供应商，支持正则
        /// </summary>
        [StringLength(50)]        
        public string Suppliers { get; set; }

        /// <summary>
        /// 指定批次号,支持正则
        /// </summary>
        [StringLength(50)]
        public string BatchCodes { get; set; }

        /// <summary>
        /// 指定替代料,支持正则
        /// </summary>
        [StringLength(50)]
        public string ReplacePNs { get; set; }

        /// <summary>
        /// 是否优先发替代料
        /// </summary>
        public bool PriorityReplacePN { get; set; }

        /// <summary>
        /// 是否未精确发料
        /// </summary>
        public bool IsCut { get; set; }

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
