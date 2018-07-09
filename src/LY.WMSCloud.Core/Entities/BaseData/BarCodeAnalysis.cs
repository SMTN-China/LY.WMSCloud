using System.ComponentModel.DataAnnotations;

namespace LY.WMSCloud.Entities.BaseData
{
    public class BarCodeAnalysis : EntitieTenantBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(30)]        
        public string Name { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        [StringLength(30)]
        public string ClassName { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        [StringLength(30)]
        public string PropertyName { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [StringLength(2000)]
        public string RegEX { get; set; }
        /// <summary>
        /// 是否替换
        /// </summary>
        public bool IsReplace { get; set; }
        /// <summary>
        /// 测试数据
        /// </summary>
        [StringLength(1000)]
        public string Test { get; set; }
        /// <summary>
        /// 测试结果
        /// </summary>
        [StringLength(500)]
        public string TestValue { get; set; }
    }
}
