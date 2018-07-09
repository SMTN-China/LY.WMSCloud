using Abp.Domain.Entities;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class UPH : EntitieTenantBase
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
        /// 节拍周期
        /// </summary>
        public int Meter { get; set; }
        /// <summary>
        /// 联版数
        /// </summary>
        public int Pin { get; set; }
        /// <summary>
        /// 单位产量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
