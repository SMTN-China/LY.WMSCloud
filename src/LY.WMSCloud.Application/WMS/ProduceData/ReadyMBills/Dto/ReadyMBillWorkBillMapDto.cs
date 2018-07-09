using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    [AutoMapFrom(typeof(ReadyMBillWorkBillMap))]
    public class ReadyMBillWorkBillMapDto : BaseDto
    {
        [Required]
        public string ReadyMBillId { get; set; }
        [Required]
        public string WorkBillId { get; set; }
        public string ProductId { get; set; }
        public string LineId { get; set; }
        /// <summary>
        /// 相关工单备料套数量
        /// </summary>
        public int Qty { get; set; }
    }
}
