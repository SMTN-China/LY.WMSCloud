using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class ReelShortTemp : Entity<string>, IAudited, IMustHaveTenant, IPassivable
    {
        /// <summary>
        /// 记账单号
        /// </summary>
        [StringLength(36)]   
        public string ReReadyMBillId { get; set; }
        public virtual ReadyMBill ReReadyMBill { get; set; }


        /// <summary>
        /// 备料详细Id
        /// </summary>
        [StringLength(36)]  
        public string ReadyMBillDetailedId { get; set; }
        public virtual ReadyMBillDetailed ReadyMBillDetailed { get; set; }


        /// <summary>
        /// BOM id
        /// </summary>
        [StringLength(36)]    
        public string BOMId { get; set; }
        public virtual BOM BOM { get; set; }


        /// <summary>
        /// 站位Id
        /// </summary>
        public string SlotId { get; set; }
        public virtual Slot Slot { get; set; }


        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }
        public virtual MPN PartNo{ get; set; }


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

        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int TenantId { get; set; }
    }
}
