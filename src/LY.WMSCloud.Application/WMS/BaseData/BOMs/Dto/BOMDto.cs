using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.BOMs.Dto
{
    [AutoMapFrom(typeof(BOM))]
    public class BOMDto : BaseDto
    {

        /// <summary>
        /// 机种ID
        /// </summary>
        [StringLength(36)]
        public string ProductId { get; set; }


        /// <summary>
        /// 物料ID
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }
        /// <summary>
        /// 物料代码
        /// </summary>
        [StringLength(36)]
        public string MainPartNoId { get; set; }

        /// <summary>
        /// 需求数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 允许超发
        /// </summary>
        public bool AllowableMoreSend { get; set; }
        /// <summary>
        /// 超发百分比
        /// </summary>
        public double MoreSendPercentage { get; set; }
        [StringLength(10)]
        public string Version { get; set; }
    }
}
