using Abp.Domain.Entities;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.StorageData
{
    public class MPNStorageAreaMap : EntitieTenantBase
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(36)]
        public string MPNId { get; set; }
        public virtual MPN MPN { get; set; }
        /// <summary>
        /// 仓库Id
        /// </summary>

        [StringLength(36)]
        public string StorageAreaId { get; set; }
        public virtual StorageArea StorageArea { get; set; }
    }
}
