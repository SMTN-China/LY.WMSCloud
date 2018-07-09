using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    /// <summary>
    /// 备料条件
    /// </summary>
    public class ReadyMCondition
    {
        /// <summary>
        /// 料号
        /// </summary>
        public string PartNoId { get; set; }

        /// <summary>
        /// 制定供应商,之间 “|”分割
        /// </summary>
        public string Suppliers { get; set; }

        /// <summary>
        /// 指定批次号,之间 “|”分割
        /// </summary>
        public string BatchCodes { get; set; }

        /// <summary>
        /// 指定替代料,之间 “|”分割
        /// </summary>
        public string ReplacePNs { get; set; }

        /// <summary>
        /// 优先替代料
        /// </summary>
        public bool PriorityReplacePN { get; set; }
    }
}
