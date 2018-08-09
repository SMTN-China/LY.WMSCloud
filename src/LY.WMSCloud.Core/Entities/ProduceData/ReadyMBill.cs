using Abp.Domain.Entities;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    /// <summary>
    /// 备料单,工单备料的时候生成
    /// </summary>
    public class ReadyMBill : EntitieTenantBase
    {

        /// <summary>
        /// 所包含工单
        /// </summary>
        public virtual ICollection<ReadyMBillWorkBillMap> WorkBills { get; set; }

        public virtual ICollection<ReadyMBillDetailed> ReadyMBillDetailed { get; set; }

        /// <summary>
        /// 记账单号
        /// </summary>
        [StringLength(36)]
        public string ReReadyMBillId { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 生成方式
        /// </summary>
        public MakeDetailsType MakeDetailsType { get; set; }
        /// <summary>
        /// 备料方式
        /// </summary>
        public ReadyMType ReadyMType { get; set; }

        /// <summary>
        /// 发料状态
        /// </summary>
        public ReadyMStatus ReadyMStatus { get; set; }

        /// <summary>
        /// 调拨方式
        /// </summary>
        [StringLength(30)]
        public string ReelMoveMethodId { get; set; }

        /// <summary>
        /// 备料时长(小时),耗时
        /// </summary>
        public int ConsumingTime { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 交付时间
        /// </summary>
        public DateTime DeliverTime { get; set; }

        /// <summary>
        /// 交付对象
        /// </summary>
        public DateTime DeliverObject { get; set; }

        /// <summary>
        /// 机种信息
        /// </summary>
        [StringLength(100)]
        public string Productstr { get; set; }

        /// <summary>
        /// 工单数量信息
        /// </summary>
        [StringLength(100)]
        public string WorkBilQtys { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [StringLength(50)]
        public string Linestr { get; set; }
    }

    public enum MakeDetailsType
    {
        BOM = 0,
        Slot,
        Detailed
    }

    public enum ReadyMType
    {
        ALL = 0,
        JIT,
        Other
    }

    public enum ReadyMStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        Ready,
        /// <summary>
        /// 发料中
        /// </summary>
        InIssUe,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish
    }
}


