using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.Slots.Dto
{
    public class BatchSlotDto
    {
        /// <summary>
        /// 机种
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 线别 SMTA or SMTB or SMTC ........
        /// </summary>
        public string LineId { get; set; }
        /// <summary>
        /// 拼版数
        /// </summary>
        public int Pin { get; set; }
        /// <summary>
        /// 面别 T or B  其中 B为S面 T为C面
        /// </summary>
        public SideType BoardSide { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 站位详细列表
        /// </summary>
        public ICollection<BatchSlotListDto> Slots { get; set; }
    }


    public class BatchSlotListDto
    {
        /// <summary>
        /// 为料站表中序号,必须和文件顺序一致
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        public string PartNoId { get; set; }
        /// <summary>
        /// 用量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 线边别 L or R
        /// </summary>
        public SideType LineSide { get; set; }
        /// <summary>
        /// 机器类型 CM or NXT or NPM
        /// </summary>
        public string MachineType { get; set; }
        /// <summary>
        /// 机器
        /// </summary>
        public string Machine { get; set; }
        /// <summary>
        /// Table
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 站位
        /// </summary>
        public string SlotName { get; set; }
        /// <summary>
        /// 站位边别  L or R
        /// </summary>
        public SideType Side { get; set; }
        /// <summary>
        /// 点位 
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 飞达选型
        /// </summary>
        public string Feeder { get; set; }

    }
}
