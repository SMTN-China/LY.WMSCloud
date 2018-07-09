using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.StorageData
{
    public class StorageArea : EntitieTenantBase
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 区域物料
        /// </summary>
        public virtual ICollection<MPNStorageAreaMap> MPNs { get; set; }
        /// <summary>
        /// 区域库位
        /// </summary>
        public virtual ICollection<StorageLocation> StorageLocations { get; set; }
    }
}
