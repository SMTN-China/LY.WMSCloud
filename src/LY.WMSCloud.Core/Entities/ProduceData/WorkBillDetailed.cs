using Abp.Domain.Entities;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class WorkBillDetailed : EntitieTenantBase
    {
        /// <summary>
        /// 工单号
        /// </summary>
        [StringLength(36)]

        public string WorkBillId { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>

        [StringLength(36)]
        public string PartNoId { get; set; }


        /// <summary>
        /// 需求数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 生产扫描数量
        /// </summary>
        public int SendQty { get; set; }
        [StringLength(36)]

        public string BOMId { get; set; }

        /// <summary>
        /// 站位Id
        /// </summary>
        public string SlotId { get; set; }

        /// <summary>
        /// 退料数量
        /// </summary>
        public int ReturnQty { get; set; }

    }
}
