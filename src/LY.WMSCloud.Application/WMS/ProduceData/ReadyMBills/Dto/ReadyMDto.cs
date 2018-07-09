using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    public class ReadyMDto
    {
        /// <summary>
        ///  沿用单号
        /// </summary>
        public ReadyMBillDto FollowReadyMBill { get; set; }

        /// <summary>
        /// 备料单号
        /// </summary>
        public ICollection<ReadyMBillDto> ReadyMBills { get; set; }

        /// <summary>
        /// 记账备料单号
        /// </summary>
        public string ReReadyMBill { get; set; }

        public string ShelfCar { get; set; }

        /// <summary>
        /// 备料条件集合
        /// </summary>
        public ICollection<ReadyMCondition> ReadyMCondition { get; set; }
    }
}
