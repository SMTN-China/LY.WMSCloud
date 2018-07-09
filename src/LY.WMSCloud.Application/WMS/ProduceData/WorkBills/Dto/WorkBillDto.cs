using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.WorkBills.Dto
{
    [AutoMapFrom(typeof(WorkBill))]
    public class WorkBillDto : BaseDto
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
        public ICollection<ReadyMBillWorkBillMap> ReadyMBills { get; set; }

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
}
