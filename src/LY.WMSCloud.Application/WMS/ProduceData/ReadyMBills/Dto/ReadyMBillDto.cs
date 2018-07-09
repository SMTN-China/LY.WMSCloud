using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.ReelMoveMethods.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    [AutoMapFrom(typeof(ReadyMBill))]
    public class ReadyMBillDto : BaseDto
    {
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
        public ReadyMStatus ReadyMStatus { get; set; }


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


        public ICollection<ReadyMBillWorkBillMapDto> WorkBills { get; set; }

        public ICollection<ReadyMBillDetailedDto> ReadyMBillDetailed { get; set; }


    }
}
