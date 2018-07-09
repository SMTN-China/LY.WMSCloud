using Abp.AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveMethods.Dto
{
    [AutoMapFrom(typeof(RMMStorageMap))]
    public class RMMStorageMapDto : BaseDto
    {
        [StringLength(36)]
        public string ReelMoveMethodId { get; set; }

        [StringLength(36)]
        public string StorageId { get; set; }
    }
}
