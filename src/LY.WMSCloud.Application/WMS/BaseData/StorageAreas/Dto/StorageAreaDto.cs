using Abp.AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.StorageAreas.Dto
{
    [AutoMapFrom(typeof(StorageArea))]
    public class StorageAreaDto : BaseDto
    {
        [StringLength(30)]
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        public ICollection<string> MPNIds { get; set; }

        public ICollection<string> ShelfNames { get; set; }

    }
}
