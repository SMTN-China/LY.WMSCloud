using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using LY.WMSCloud.Entities.StorageData;

namespace LY.WMSCloud.Entities.ProduceData
{
    public class ReelMoveMethod : EntitieTenantBase
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
        /// 移除仓集合
        /// </summary>
        public virtual ICollection<RMMStorageMap> OutStorages { get; set; }
        /// <summary>
        /// 移入仓
        /// </summary>
        [StringLength(36)]
        public string InStorageId { get; set; }
        public string Icon { get; set; }
        /// <summary>
        /// 操作合集
        /// </summary>
        [NotMapped]
        public ICollection<AllocationType> AllocationTypes { get; set; }
        /// <summary>
        /// 操作字符
        /// </summary>
        [StringLength(100)]
        public string AllocationTypesStr
        {
            get
            {
                if (AllocationTypes == null)
                {
                    return null;
                }
                return string.Join("|", AllocationTypes.Select(s => s.ToString()));
            }
            set
            {
                if (value == null)
                {
                    AllocationTypes = null;
                }
                else
                {
                    var res = new List<AllocationType>();
                    var strs = value.Split('|');
                    foreach (var item in strs)
                    {
                        res.Add(Enum.Parse<AllocationType>(item));
                    }
                    AllocationTypes = res;
                }
            }
        }
    }

    public enum AllocationType
    {

        /// <summary>
        /// 转仓
        /// </summary>
        Move = 0,
        /// <summary>
        /// 上架
        /// </summary>
        OnSL,
        /// <summary>
        /// 下架
        /// </summary>
        UpSl,
        /// <summary>
        /// 发料
        /// </summary>
        Send,
        /// <summary>
        /// 退料
        /// </summary>
        Return,
        /// <summary>
        /// 收料
        /// </summary>
        Received,
        /// <summary>
        /// 注册
        /// </summary>
        Register,
        /// <summary>
        /// 发首料
        /// </summary>
        SendFirstReel,
        /// <summary>
        /// 补料
        /// </summary>
        SupplyReel,
        /// <summary>
        /// 库位下架
        /// </summary>
        UpByShelf,
        /// <summary>
        /// 出库
        /// </summary>
        Out
    }
}
