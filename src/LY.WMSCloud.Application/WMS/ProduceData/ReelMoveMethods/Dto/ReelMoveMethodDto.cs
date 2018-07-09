using Abp.AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveMethods.Dto
{
    [AutoMapFrom(typeof(ReelMoveMethod))]
    public class ReelMoveMethodDto : BaseDto
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

        public ICollection<RMMStorageMapDto> OutStorages { get; set; }

        public ICollection<AllocationType> AllocationTypes { get; set; }
        [StringLength(100)]
        public string AllocationTypesStr { get; set; }
        [StringLength(36)]
        public string OutStorageIds { get; set; }
        [StringLength(36)]
        public string InStorageId { get; set; }
    }
}
