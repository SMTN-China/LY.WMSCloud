using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class PrintReel : EntitieBase
    {
        /// <summary>
        /// 打印编号
        /// </summary>
        [StringLength(36)]
        public string PrintStr { get; set; }

        public int PrintIndex { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }

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
        [StringLength(500)]
        public string Info { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
    }
}
