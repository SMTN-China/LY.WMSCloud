using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.MPNs.Dto
{
    [AutoMapFrom(typeof(MPN))]
    public class MPNDto : BaseDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        public string Info { get; set; }
        [StringLength(200)]
        public string Supplier { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public MPNHierarchy MPNHierarchy { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public MPNLevel MPNLevel { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MPNType MPNType { get; set; }

        /// <summary>
        /// 类型等级
        /// </summary>
        public MSDLevel MSDLevel { get; set; }
        /// <summary>
        /// 来料方式
        /// </summary>
        public IncomingMethod IncomingMethod { get; set; }

        /// <summary>
        /// 原包装数量
        /// </summary>
        [StringLength(50)]
        public string MPQs { get; set; }
        /// <summary>
        /// 保质期（天）
        /// </summary>
        public double ShelfLife { get; set; }

        /// <summary>
        /// 客户代码
        /// </summary>
        [StringLength(36)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 默认注册仓库代码
        /// </summary>
        [StringLength(36)]
        public string RegisterStorageId { get; set; }
    }
}
