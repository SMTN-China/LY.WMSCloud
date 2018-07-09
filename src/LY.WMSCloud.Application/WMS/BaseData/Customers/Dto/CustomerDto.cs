using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.Customers.Dto
{
    [AutoMapFrom(typeof(Customer))]
    public class CustomerDto : BaseDto
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
    }
}
