using Abp.Domain.Entities;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class WorkBill   : EntitieTenantBase
    {

        /// <summary>
        /// 机种
        /// </summary>
        [StringLength(36)]
        public string ProductId { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [StringLength(36)]
        public string LineId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 备料单关联
        /// </summary>
        public virtual ICollection<ReadyMBillWorkBillMap> ReadyMBills { get; set; }

        /// <summary>
        /// 工单套数
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 备料套数
        /// </summary>
        public int ReadyMQty { get; set; }

        /// <summary>
        /// 生产套数
        /// </summary>
        public int ProductionQty { get; set; }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime PlanStartTime { get; set; }

        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime PlanEndTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }        

        /// <summary>
        /// 工单状态
        /// </summary>
        public WorkBillStatus WorkBillStatus { get; set; }       
    }

    public enum WorkBillStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        New,
        /// <summary>
        /// 分配完成
        /// </summary>
        Assigned,
        /// <summary>
        /// 备料
        /// </summary>
        ReadyM,
        /// <summary>
        /// 备料完成
        /// </summary>
        ReadyMCompletion,
        /// <summary>
        /// 开始生产
        /// </summary>
        ProductionStrat,
        /// <summary>
        /// 生产挂起
        /// </summary>
        ProductionSuspend,
        /// <summary>
        /// 生产完成
        /// </summary>
        ProductionCompletion,
        /// <summary>
        /// 强制完成
        /// </summary>
        ForceCompletion
    }
}
