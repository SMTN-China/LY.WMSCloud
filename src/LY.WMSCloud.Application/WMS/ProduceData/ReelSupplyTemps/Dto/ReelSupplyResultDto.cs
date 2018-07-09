using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelSupplyTemps.Dto
{
    public class ReelSupplyResultDto
    {
        /// <summary>
        /// 不在备料单中的物料,清单
        /// </summary>
        public List<string> ErrorMsgs { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
    }


    public class ReelSupplyInputDto
    {
        /// <summary>
        /// 补料备料单
        /// </summary>
        public string ReadyMBillId { get; set; }
        /// <summary>
        /// 补料料号
        /// </summary>
        public string PartNoId { get; set; }
        /// <summary>
        /// 补料数量
        /// </summary>
        public int Qtys { get; set; }
    }
}
