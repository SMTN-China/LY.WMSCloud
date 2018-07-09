using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.Storages.Dto
{
    [AutoMapFrom(typeof(Storage))]
    public class StorageDto : BaseDto
    {

        [StringLength(50)]
        public string Name { get; set; }

        public IncomingMethod IncomingMethod { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
    }
}
