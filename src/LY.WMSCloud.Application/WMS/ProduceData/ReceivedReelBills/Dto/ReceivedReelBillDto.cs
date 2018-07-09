using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReceivedReelBills.Dto
{
    [AutoMapFrom(typeof(ReceivedReelBill))]
    public class ReceivedReelBillDto : BaseDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [StringLength(200)]
        public string PoId { get; set; }
        [StringLength(30)]

        public string IQCCheckId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }

        /// <summary>
        /// 允许收料数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 已收料数量
        /// </summary>
        public int ReceivedQty { get; set; }

        /// 备注
        /// </summary>
        [StringLength(500)]

        public string Remark { get; set; }
    }
}
