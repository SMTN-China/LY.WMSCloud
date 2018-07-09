using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelShortTemps.Dto
{
    public class ReelShortTempDto : IEntityDto<string>
    {
        public string ReReadyMBillId { get; set; }

        public string PartNoId { get; set; }

        /// <summary>
        /// 此料号总需求数量
        /// </summary>
        public int DemandQty { get; set; }
        public int DemandSendQty { get; set; }
        /// <summary>
        /// 此料号总挑料数量
        /// </summary>
        public int SelectQty { get; set; }
        /// <summary>
        /// 缺料数量
        /// </summary>
        public int ShortQty { get; set; }

        public bool IsActive { get; set; }
        public string Id { get; set; }
    }
}
