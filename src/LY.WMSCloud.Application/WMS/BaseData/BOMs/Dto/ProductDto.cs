using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.BOMs.Dto
{
    public class ProductDto : BaseDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        [StringLength(200)]
        public string Supplier { get; set; }

        /// <summary>
        /// 子件数量
        /// </summary>
        public int ItemCount { get; set; }
    }
}
