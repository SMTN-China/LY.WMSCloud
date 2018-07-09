using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.Slots.Dto
{
    [AutoMapFrom(typeof(Slot))]
    public class SlotDto : BaseDto
    {
        /// <summary>
        /// 机种代码
        /// </summary>
        [StringLength(36)]
        public string ProductId { get; set; }

        /// <summary>
        /// 物料代码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }

        /// <summary>
        /// 用量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 站位序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 线别代码
        /// </summary>
        [StringLength(30)]
        public string LineId { get; set; }

        /// <summary>
        /// 版面
        /// </summary>
        public SideType BoardSide { get; set; }
        /// <summary>
        /// 线边别
        /// </summary>
        public SideType LineSide { get; set; }
        /// <summary>
        /// 机器
        /// </summary>
        [StringLength(30)]

        public string Machine { get; set; }
        [StringLength(10)]
        public string Table { get; set; }
        /// <summary>
        /// 站位
        /// </summary>
        [StringLength(30)]
        public string SlotName { get; set; }
        /// <summary>
        /// 站位边别
        /// </summary>
        public SideType Side { get; set; }
        /// <summary>
        /// 机器类型
        /// </summary>
        [StringLength(30)]
        public string MachineType { get; set; }
        /// <summary>
        /// 点位
        /// </summary>
        [StringLength(1000)]

        public string Location { get; set; }
        /// <summary>
        /// 飞达选型
        /// </summary>
        [StringLength(50)]

        public string Feeder { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        [StringLength(10)]
        public string Version { get; set; }
    }
}
