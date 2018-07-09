using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LY.WMSCloud
{
    /// <summary>
    /// 批量更新单一属性、值对象
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    /// <typeparam name="TProperty">属性</typeparam>
    public class LYParameter<TEntity, TProperty> where TEntity : class, new()
    {
        /// <summary>
        /// 属性表达式
        /// </summary>
        public Expression<Func<TEntity, TProperty>> Property { get; set; }
        /// <summary>
        /// 更新后的值
        /// </summary>
        public object Value;
    }
}
