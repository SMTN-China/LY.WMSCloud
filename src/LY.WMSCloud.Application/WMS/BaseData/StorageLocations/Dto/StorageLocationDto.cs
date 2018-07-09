using Abp.AutoMapper;
using LY.WMSCloud.Entities.StorageData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.StorageLocations.Dto
{
    [AutoMapFrom(typeof(StorageLocation))]
    public class StorageLocationDto : BaseDto
    {
        /// <summary>
        /// 料架号
        /// </summary>
        [StringLength(30)]
        public string Code { get; set; }
        /// <summary>
        /// 料架名称
        /// </summary>
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 料卷号
        /// </summary>
        [StringLength(100)]
        public string ReelId { get; set; }

        /// <summary>
        /// 主板Id
        /// </summary>
        public int MainBoardId { get; set; }
        /// <summary>
        /// 库位Id
        /// </summary>
        public int PositionId { get; set; }
        /// <summary>
        /// 货架类型
        /// </summary>
        [StringLength(36)]
        public string StorageLocationTypeId { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        [StringLength(36)]
        public string StorageAreaId { get; set; }
        /// <summary>
        /// 仓库Id
        /// </summary>
        [StringLength(36)]
        public string StorageId { get; set; }


        /// <summary>
        /// 灯状态
        /// </summary>
        public LightState LightState { get; set; }

        /// <summary>
        /// 灯颜色
        /// </summary>
        public LightColor LightColor { get; set; }
    }
}
