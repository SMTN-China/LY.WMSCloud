using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelSupplyTemps.Dto
{
    [AutoMapFrom(typeof(ReelSupplyTemp))]
    public class ReelSupplyTempDto : BaseDto
    {
        /// <summary>
        /// 记账单号
        /// </summary>
        [StringLength(36)]
        public string ReReadyMBillId { get; set; }

        /// <summary>
        /// 记账行号
        /// </summary>
        [StringLength(36)]
        public string ReadyMBillDetailedId { get; set; }

        /// <summary>
        /// 库位Id
        /// </summary>
        [StringLength(36)]
        public string StorageLocationId { get; set; }

        /// <summary>
        /// Bom ID
        /// </summary>
        [StringLength(36)]
        public string BOMId { get; set; }

        /// <summary>
        /// 站位Id
        /// </summary>
        public int? SlotId { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }

        /// <summary>
        /// 调拨方式Id
        /// </summary>
        [StringLength(36)]
        public string ReelMoveMethodId { get; set; }

        /// <summary>
        /// 此料号总需求数量
        /// </summary>
        public int DemandQty { get; set; }
        /// <summary>
        /// 此料号总挑料数量
        /// </summary>
        public int SelectQty { get; set; }
        /// <summary>
        /// 料盘数量
        /// </summary>
        public int Qty { get; set; }
        public int DemandSendQty { get; set; }
        /// <summary>
        /// 需发数量
        /// </summary>
        public int SendQty { get; set; }
        /// <summary>
        /// 是否剪断
        /// </summary>
        public bool IsCut { get; set; }
        public bool IsSend { get; set; }
    }
}
