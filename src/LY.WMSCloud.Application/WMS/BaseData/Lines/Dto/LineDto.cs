using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System.ComponentModel.DataAnnotations;

namespace LY.WMSCloud.WMS.BaseData.Lines.Dto
{
    [AutoMapFrom(typeof(Line))]
    public class LineDto : BaseDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        public string Name { get; set; }

        public string ForCustomerMStorageId { get; set; }

        public string ForSelfMStorageId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
