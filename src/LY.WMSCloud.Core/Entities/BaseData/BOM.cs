using System.ComponentModel.DataAnnotations;

namespace LY.WMSCloud.Entities.BaseData
{
    public class BOM : EntitieTenantBase
    {
        /// <summary>
        /// 机种代码
        /// </summary>
        [StringLength(36)]
        public string ProductId { get; set; }

        /// <summary>
        /// 物料代码
        /// </summary>
        [StringLength(36)]
        public string PartNoId { get; set; }

        /// <summary>
        /// 物料代码
        /// </summary>
        [StringLength(36)]
        public string MainPartNoId { get; set; }

        /// <summary>
        /// 需求数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 允许超发
        /// </summary>
        public bool AllowableMoreSend { get; set; }

        /// <summary>
        /// 超发百分比
        /// </summary>
        public double MoreSendPercentage { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [StringLength(10)]
        public string Version { get; set; }
    }
}
