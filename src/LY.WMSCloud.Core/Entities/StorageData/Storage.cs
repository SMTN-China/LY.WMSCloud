using Abp.Domain.Entities;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Entities.StorageData
{
    public class Storage : EntitieTenantBase
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
        /// 地址
        /// </summary>
        [StringLength(200)]
        public string Address { get; set; }
        /// <summary>
        /// 相关人员
        /// </summary>
        public int? AboutUserId { get; set; }
        public virtual User AboutUser { get; set; }
        /// <summary>
        /// 来料方式
        /// </summary>
        public IncomingMethod IncomingMethod { get; set; }
    }

}
