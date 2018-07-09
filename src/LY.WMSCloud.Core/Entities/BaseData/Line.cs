using LY.WMSCloud.Entities.StorageData;
using System.ComponentModel.DataAnnotations;

namespace LY.WMSCloud.Entities.BaseData
{
    public class Line : EntitieTenantBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 客供料仓代码
        /// </summary>
        [StringLength(36)]
        public string ForCustomerMStorageId { get; set; }

        /// <summary>
        /// 自购料仓代码
        /// </summary>
        [StringLength(36)]
        public string ForSelfMStorageId { get; set; }

    }
}
