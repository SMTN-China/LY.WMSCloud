using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class ReelSendTemp : Entity<string>, IAudited, IMustHaveTenant, IPassivable
    {
        /// <summary>
        /// 记账单号
        /// </summary>
        [StringLength(36)]
        public string ReReadyMBillId { get; set; }
        public virtual ReadyMBill ReReadyMBill { get; set; }


        /// <summary>
        /// 备料单详细行Id
        /// </summary>
        [StringLength(36)]
        public string ReadyMBillDetailedId { get; set; }
        public virtual ReadyMBillDetailed ReadyMBillDetailed { get; set; }


        /// <summary>
        /// 库位Id
        /// </summary>
        [StringLength(36)]
        public string StorageLocationId { get; set; }
        public virtual StorageLocation StorageLocation { get; set; }


        /// <summary>
        /// BOM id
        /// </summary>
        [StringLength(36)]
        public string BOMId { get; set; }
        public virtual BOM BOM { get; set; }


        /// <summary>
        /// 料站表Id
        /// </summary>
        public string SlotId { get; set; }
        public virtual Slot Slot { get; set; }


        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }
        public virtual MPN PartNo { get; set; }

        /// <summary>
        /// 调拨方式
        /// </summary>
        [StringLength(36)]
        public string ReelMoveMethodId { get; set; }
        public virtual ReelMoveMethod ReelMoveMethod { get; set; }


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

        /// <summary>
        /// 需发数量
        /// </summary>
        public int DemandSendQty { get; set; }

        /// <summary>
        /// 发料数量
        /// </summary>
        public int SendQty { get; set; }

        /// <summary>
        /// 是否剪短
        /// </summary>
        public bool IsCut { get; set; }

        /// <summary>
        /// 是否发料
        /// </summary>
        public bool IsSend { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        [StringLength(36)]
        public string FisrtStorageLocationId { get; set; }
        public int TenantId { get; set; }
    }
}
