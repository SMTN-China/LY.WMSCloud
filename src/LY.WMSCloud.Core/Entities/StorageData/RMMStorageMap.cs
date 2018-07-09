using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.StorageData
{
    public class RMMStorageMap : EntitieTenantBase
    {

        [StringLength(36)]
        public string ReelMoveMethodId { get; set; }
        public virtual ReelMoveMethod ReelMoveMethod { get; set; }


        [StringLength(36)]
        public string StorageId { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
