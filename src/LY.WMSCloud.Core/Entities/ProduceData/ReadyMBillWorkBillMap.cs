using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class ReadyMBillWorkBillMap : EntitieTenantBase
    {

        public string ReadyMBillId { get; set; }
        public virtual ReadyMBill ReadyMBill { get; set; }

        public string WorkBillId { get; set; }
        public virtual WorkBill WorkBill { get; set; }
        
        /// <summary>
        /// 相关工单备料套数量
        /// </summary>
        public int Qty { get; set; }
    }
}
