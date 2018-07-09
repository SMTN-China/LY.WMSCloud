using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveLogs.Dto
{
    [AutoMapFrom(typeof(ReelMoveLog))]
    public class ReelMoveLogDto : BaseDto
    {
        /// <summary>
        /// 工单代码
        /// </summary>
        [StringLength(36)]

        public string WorkBillId { get; set; }

        /// <summary>
        /// 备料单代码
        /// </summary>
        [StringLength(36)]

        public string ReadyMBillId { get; set; }

        /// <summary>
        /// 备料单明细Id
        /// </summary>
        [StringLength(36)]
        public string ReadyMBillDetailedId { get; set; }

        /// <summary>
        /// 调拨方式代码
        /// </summary>
        [StringLength(36)]
        public string ReelMoveMethodId { get; set; }

        /// <summary>
        /// 料卷编码
        /// </summary>
        [StringLength(100)]
        public string ReelId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }

        /// <summary>
        /// 库位编码
        /// </summary>
        [StringLength(36)]
        public string StorageLocationId { get; set; }

        /// <summary>
        /// 料站表Id
        /// </summary>
        public int? SlotId { get; set; }

        /// <summary>
        /// 收料单号
        /// </summary>
        [StringLength(36)]

        public string ReceivedReelBillId { get; set; }

        /// <summary>
        /// 料盘数量
        /// </summary>
        public int Qty { get; set; }
    }
}
