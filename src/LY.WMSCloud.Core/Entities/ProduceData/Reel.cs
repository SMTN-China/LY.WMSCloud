using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.ComponentModel.DataAnnotations;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class Reel : EntitieTenantBase
    {
        /// <summary>
        /// 料号
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }
        public virtual MPN PartNo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        [StringLength(30)]
        public string Supplier { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime MakeDate { get; set; }
        /// <summary>
        /// D/C
        /// </summary>
        [StringLength(15)]
        public string DateCode { get; set; }
        /// <summary>
        /// 大批次号
        /// </summary>
        [StringLength(50)]
        public string LotCode { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        [StringLength(30)]
        public string BatchCode { get; set; }
        /// <summary>
        /// 是否在用
        /// </summary>
        public bool IsUseed { get; set; }
        /// <summary>
        /// 延期时长
        /// </summary>
        public double ExtendShelfLife { get; set; }
        /// <summary>
        /// 备料单号
        /// </summary>
        [StringLength(30)]
        public string ReadyMBillId { get; set; }
        /// <summary>
        /// 备料单明细Id
        /// </summary>
        [StringLength(36)]
        public string ReadyMBillDetailedId { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        [StringLength(36)]
        public string WorkBillId { get; set; }
        /// <summary>
        /// 工单明细Id
        /// </summary>
        [StringLength(36)]
        public string WorkBillDetailedId { get; set; }

        /// <summary>
        /// 库位Id
        /// </summary>
        [StringLength(36)]
        public string StorageLocationId { get; set; }
        public virtual StorageLocation StorageLocation { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [StringLength(36)]
        public string StorageId { get; set; }

        /// <summary>
        /// 收料单
        /// </summary>
        [StringLength(36)]
        public string ReceivedReelBillId { get; set; }

        /// <summary>
        /// Po
        /// </summary>
        [StringLength(30)]
        public string PoId { get; set; }
        /// <summary>
        /// 检验单号
        /// </summary>
        [StringLength(30)]
        public string IQCCheckId { get; set; }
        /// <summary>
        /// 料站表行Id
        /// </summary>
        public string SlotId { get; set; }
    }
}
