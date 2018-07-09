using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelSendTemps.Dto
{
    public class ReelSendTempDto : IEntityDto<string>
    {
        public string ReReadyMBillId { get; set; }

        public string StorageLocationId { get; set; }

        public string PartNoId { get; set; }

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
        /// 料盘需发数量
        /// </summary>
        public int SendQty { get; set; }
        /// <summary>
        /// 是否剪短
        /// </summary>
        public bool IsCut { get; set; }

        public bool IsSend { get; set; }

        public bool IsActive { get; set; }
        public string Id { get; set; }
    }
}
