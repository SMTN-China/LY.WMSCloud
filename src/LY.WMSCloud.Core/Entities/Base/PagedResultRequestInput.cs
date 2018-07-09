using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LY.WMSCloud.Entities
{
    /// <summary>
    /// 对Linq进行扩展,使其一定程度内可进行动态查询
    /// </summary>
    public static class ExtentionLinq
    {
        /// <summary>
        /// 对Linq进行扩展,使其一定程度内可进行动态查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData">源数据</param>
        /// <param name="pagedResultRequestWMSDto">查询参数</param>
        /// <returns>返回查询</returns>
        public static IQueryable<T> DynamicQuery<T>(this IQueryable<T> sourceData, PagedResultRequestInput pagedResultRequestWMSDto)
        {
            Expression<Func<T, bool>> totalExpr = null;
            Type queryConditionType = typeof(T);
            var properties = queryConditionType.GetProperties();

            // 查询
            if (pagedResultRequestWMSDto.RequestWMSDtos != null)
            {
                totalExpr = GetWhereExpression<T>(pagedResultRequestWMSDto.RequestWMSDtos, properties);
                if (totalExpr != null)
                {
                    sourceData = sourceData.Where(totalExpr);
                }
            }

            // 排序
            if (pagedResultRequestWMSDto.SortName != null)
            {
                var orderBy = properties.Where(p => p.Name.ToLower() == pagedResultRequestWMSDto.SortName.ToLower()).FirstOrDefault();

                if (orderBy != null)
                {
                    if (pagedResultRequestWMSDto.Desc)
                    {
                        pagedResultRequestWMSDto.SortName += " desc";
                    }
                    sourceData = sourceData.OrderBy(pagedResultRequestWMSDto.SortName);
                }
            }

            // 获取 SQL 版本更新,不再用第三方类库支持,语句已注释
            // var sql = sourceData.ToSql();

            // 分页已移动到外面
            // sourceData = sourceData.Skip(pagedResultRequestMESDto.SkipCount).Take(pagedResultRequestMESDto.MaxResultCount);    

            return sourceData;
        }

        private static Expression<Func<T, bool>> GetWhereExpression<T>(List<RequestWMSDto> requestMESDtos, PropertyInfo[] properties)
        {

            Expression<Func<T, bool>> totalExpr = null;
            foreach (var requestMESDto in requestMESDtos)
            {
                Expression<Func<T, bool>> filter = null;
                if (requestMESDto.RequestWMSDtos == null || requestMESDto.RequestWMSDtos.Count == 0)
                {
                    if (requestMESDto.PropertyName == null)
                    {
                        continue;
                    }

                    var propertie = properties.Where(p => p.Name.ToLower() == requestMESDto.PropertyName.ToLower()).FirstOrDefault();
                    if (propertie == null)
                    {
                        continue;
                    }

                    if (requestMESDto.Operation != Operation.Null && requestMESDto.Operation != Operation.NotNull &&
                        requestMESDto.Operation != Operation.NotEmpty && requestMESDto.Operation != Operation.Empry)
                    {
                        if (requestMESDto.QueryValue == null || requestMESDto.QueryValue.ToString().Length == 0)
                        {
                            continue;
                        }
                    }

                    try
                    {
                        var queryValue = Newtonsoft.Json.JsonConvert.DeserializeObject(requestMESDto.QueryValue.ToString(), propertie.PropertyType);

                        //各种操作的具体处理                  
                        switch (requestMESDto.Operation)
                        {
                            case Operation.Contains:

                                filter = Lambda.Contains<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.NotContains:
                                filter = Lambda.NotContains<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.Equal:
                                filter = Lambda.Equal<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.NotEqual:
                                filter = Lambda.NotEqual<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.GreaterThan:
                                filter = Lambda.Greater<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.GreaterThanOrEqual:
                                filter = Lambda.GreaterEqual<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.LessThan:
                                filter = Lambda.Less<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.LessThanOrEqual:
                                filter = Lambda.LessEqual<T>(requestMESDto.PropertyName, queryValue);
                                break;
                            case Operation.Empry:
                                filter = Lambda.Equal<T>(requestMESDto.PropertyName, "");
                                break;
                            case Operation.Null:
                                filter = Lambda.Equal<T>(requestMESDto.PropertyName, null);
                                break;
                            case Operation.RegEx:
                                filter = Lambda.RegEx<T>(requestMESDto.PropertyName, requestMESDto.QueryValue.ToString());
                                break;
                            case Operation.StartsWith:
                                filter = Lambda.Starts<T>(requestMESDto.PropertyName, requestMESDto.QueryValue.ToString());
                                break;
                            case Operation.EndsWith:
                                filter = Lambda.Ends<T>(requestMESDto.PropertyName, requestMESDto.QueryValue.ToString());
                                break;
                            case Operation.NotEmpty:
                                filter = Lambda.NotEqual<T>(requestMESDto.PropertyName, "");
                                break;
                            case Operation.NotNull:
                                filter = Lambda.NotEqual<T>(requestMESDto.PropertyName, null);
                                break;
                        }
                        if (filter != null)
                        {
                            if (totalExpr == null)
                            {

                                totalExpr = filter;
                            }
                            else
                            {
                                switch (requestMESDto.LinkOperation)
                                {
                                    case LinkOperation.And:
                                        totalExpr = totalExpr.And(filter);
                                        break;
                                    case LinkOperation.Or:
                                        totalExpr = totalExpr.Or(filter);
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    var subExpression = GetWhereExpression<T>(requestMESDto.RequestWMSDtos, properties);
                    if (subExpression != null)
                    {
                        if (totalExpr == null)
                        {
                            totalExpr = subExpression;
                        }
                        else
                        {
                            switch (requestMESDto.LinkOperation)
                            {

                                case LinkOperation.And:
                                    totalExpr = totalExpr.And(GetWhereExpression<T>(requestMESDto.RequestWMSDtos, properties));
                                    break;
                                case LinkOperation.Or:
                                    totalExpr = totalExpr.Or(GetWhereExpression<T>(requestMESDto.RequestWMSDtos, properties));
                                    break;
                            }
                        }
                    }
                }
            }
            return totalExpr;
        }
    }

    /// <summary>
    /// 分页查询对象
    /// </summary>
    public class PagedResultRequestInput : IPagedResultRequest
    {
        /// <summary>
        /// 开始行
        /// </summary>
        public int SkipCount { get; set; }
        /// <summary>
        /// 获取数量
        /// </summary>
        public int MaxResultCount { get; set; }
        /// <summary>
        /// 筛选条件
        /// </summary>
        public List<RequestWMSDto> RequestWMSDtos { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortName { get; set; }
        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool Desc { get; set; }

    }

    /// <summary>
    /// 筛选条件
    /// </summary>
    public class RequestWMSDto
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 操作符
        /// </summary>
        public Operation Operation { get; set; }
        /// <summary>
        /// 查询参考值
        /// </summary>
        public object QueryValue { get; set; }
        /// <summary>
        /// 链接符
        /// </summary>
        public LinkOperation LinkOperation { get; set; }
        /// <summary>
        /// 子查询
        /// </summary>
        public List<RequestWMSDto> RequestWMSDtos { get; set; }
    }

    /// <summary>
    /// 链接符
    /// </summary>
    public enum LinkOperation
    {
        And = 0,
        Or
    }

    /// <summary>
    /// 操作符
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 0,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// 包含
        /// </summary>
        Contains,
        /// <summary>
        /// 以***开始
        /// </summary>
        StartsWith,
        /// <summary>
        /// 以***结尾
        /// </summary>
        EndsWith,
        /// <summary>
        /// 不包含
        /// </summary>
        NotContains,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 为空
        /// </summary>
        Empry,
        /// <summary>
        /// 不为空
        /// </summary>
        NotEmpty,
        /// <summary>
        /// 为NULL
        /// </summary>
        Null,
        /// <summary>
        /// 不为NULL
        /// </summary>
        NotNull,
        /// <summary>
        /// 正则匹配
        /// </summary>
        RegEx
    }

    /// <summary>
    /// 系统扩展 - Lambda表达式
    /// </summary>
    public static class Extensions
    {

        #region Property(属性表达式)

        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="propertyName">属性名,支持多级属性名，与句点分隔，范例：Customer.Name</param>
        public static Expression Property(this Expression expression, string propertyName)
        {
            if (propertyName.All(t => t != '.'))
                return Expression.Property(expression, propertyName);
            var propertyNameList = propertyName.Split('.');
            Expression result = null;
            for (int i = 0; i < propertyNameList.Length; i++)
            {
                if (i == 0)
                {
                    result = Expression.Property(expression, propertyNameList[0]);
                    continue;
                }
                result = result.Property(propertyNameList[i]);
            }
            return result;
        }

        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="member">属性</param>
        public static Expression Property(this Expression expression, MemberInfo member)
        {
            return Expression.MakeMemberAccess(expression, member);
        }

        #endregion

        #region And(与表达式)

        /// <summary>
        /// 与操作表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression And(this Expression left, Expression right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            return Expression.AndAlso(left, right);
        }

        /// <summary>
        /// 与操作表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            return left.Compose(right, Expression.AndAlso);
        }

        #endregion

        #region Or(或表达式)

        /// <summary>
        /// 或操作表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression Or(this Expression left, Expression right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            return Expression.OrElse(left, right);
        }

        /// <summary>
        /// 或操作表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            return left.Compose(right, Expression.OrElse);
        }

        #endregion

        #region Value(获取lambda表达式的值)

        /// <summary>
        /// 获取lambda表达式的值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public static object Value<T>(this Expression<Func<T, bool>> expression)
        {
            return Lambda.GetValue(expression);
        }

        #endregion

        #region Equal(等于表达式)

        /// <summary>
        /// 创建等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression Equal(this Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        /// <summary>
        /// 创建等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression Equal(this Expression left, object value)
        {
            return left.Equal(Lambda.Constant(left, value));
        }

        #endregion

        #region NotEqual(不等于表达式)

        /// <summary>
        /// 创建不等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression NotEqual(this Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        /// <summary>
        /// 创建不等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression NotEqual(this Expression left, object value)
        {
            return left.NotEqual(Lambda.Constant(left, value));
        }

        #endregion

        #region Greater(大于表达式)

        /// <summary>
        /// 创建大于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression Greater(this Expression left, Expression right)
        {
            return Expression.GreaterThan(left, right);
        }

        /// <summary>
        /// 创建大于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression Greater(this Expression left, object value)
        {
            return left.Greater(Lambda.Constant(left, value));
        }

        #endregion

        #region GreaterEqual(大于等于表达式)

        /// <summary>
        /// 创建大于等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression GreaterEqual(this Expression left, Expression right)
        {
            return Expression.GreaterThanOrEqual(left, right);
        }

        /// <summary>
        /// 创建大于等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression GreaterEqual(this Expression left, object value)
        {
            return left.GreaterEqual(Lambda.Constant(left, value));
        }

        #endregion

        #region Less(小于表达式)

        /// <summary>
        /// 创建小于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression Less(this Expression left, Expression right)
        {
            return Expression.LessThan(left, right);
        }

        /// <summary>
        /// 创建小于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression Less(this Expression left, object value)
        {
            return left.Less(Lambda.Constant(left, value));
        }

        #endregion

        #region LessEqual(小于等于表达式)

        /// <summary>
        /// 创建小于等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        public static Expression LessEqual(this Expression left, Expression right)
        {
            return Expression.LessThanOrEqual(left, right);
        }

        /// <summary>
        /// 创建小于等于运算表达式
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression LessEqual(this Expression left, object value)
        {
            return left.LessEqual(Lambda.Constant(left, value));
        }

        #endregion

        #region StartsWith(头匹配)

        /// <summary>
        /// 头匹配
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression StartsWith(this Expression left, object value)
        {
            return left.Call("StartsWith", new[] { typeof(string) }, value);
        }




        /// <summary>
        /// 正则匹配
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression RegEx(this Expression left, object value)
        {
            return left.CallRegEx(value);
        }

        #endregion

        #region EndsWith(尾匹配)

        /// <summary>
        /// 尾匹配
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression EndsWith(this Expression left, object value)
        {
            return left.Call("EndsWith", new[] { typeof(string) }, value);
        }

        #endregion

        #region Contains(模糊匹配)

        /// <summary>
        /// 模糊匹配
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression Contains(this Expression left, object value)
        {
            return left.Call("Contains", new[] { typeof(string) }, value);
        }

        #endregion

        #region NotContains(模糊匹配)

        /// <summary>
        /// 模糊匹配
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="value">值</param>
        public static Expression NotContains(this Expression left, object value)
        {
            return Expression.Not(left.Call("Contains", new[] { typeof(string) }, value));
        }

        #endregion

        #region Operation(操作)

        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="operator">运算符</param>
        /// <param name="value">值</param>
        public static Expression Operation(this Expression left, Operation @operator, object value)
        {
            switch (@operator)
            {
                case Entities.Operation.Equal:
                    return left.Equal(value);
                case Entities.Operation.NotEqual:
                    return left.NotEqual(value);
                case Entities.Operation.Contains:
                    return left.Contains(value);
                case Entities.Operation.StartsWith:
                    return left.StartsWith(value);
                case Entities.Operation.EndsWith:
                    return left.EndsWith(value);
                case Entities.Operation.NotContains:
                    break;
                case Entities.Operation.GreaterThan:
                    return left.Greater(value);
                case Entities.Operation.GreaterThanOrEqual:
                    return left.GreaterEqual(value);
                case Entities.Operation.LessThan:
                    return left.Less(value);
                case Entities.Operation.LessThanOrEqual:
                    return left.LessEqual(value);
                case Entities.Operation.Empry:
                    break;
                case Entities.Operation.NotEmpty:
                    break;
                case Entities.Operation.Null:
                    break;
                case Entities.Operation.NotNull:
                    break;
                case Entities.Operation.RegEx:
                    break;
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        #endregion

        #region Call(调用方法表达式)

        /// <summary>
        /// 创建调用方法表达式
        /// </summary>
        /// <param name="instance">调用的实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="values">参数值列表</param>
        public static Expression Call(this Expression instance, string methodName, params Expression[] values)
        {
            return Expression.Call(instance, instance.Type.GetTypeInfo().GetMethod(methodName), values);
        }

        /// <summary>
        /// 创建调用方法表达式
        /// </summary>
        /// <param name="instance">调用的实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="values">参数值列表</param>
        public static Expression Call(this Expression instance, string methodName, params object[] values)
        {
            if (values == null || values.Length == 0)
                return Expression.Call(instance, instance.Type.GetTypeInfo().GetMethod(methodName));
            return Expression.Call(instance, instance.Type.GetTypeInfo().GetMethod(methodName), values.Select(Expression.Constant));
        }

        /// <summary>
        /// 创建调用方法表达式
        /// </summary>
        /// <param name="instance">调用的实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="paramTypes">参数类型列表</param>
        /// <param name="values">参数值列表</param>
        public static Expression Call(this Expression instance, string methodName, Type[] paramTypes, params object[] values)
        {
            if (values == null || values.Length == 0)
                return Expression.Call(instance, instance.Type.GetTypeInfo().GetMethod(methodName, paramTypes));
            return Expression.Call(instance, instance.Type.GetTypeInfo().GetMethod(methodName, paramTypes), values.Select(Expression.Constant));
        }

        /// <summary>
        /// 创建调用方法表达式
        /// </summary>
        /// <param name="instance">调用的实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="paramTypes">参数类型列表</param>
        /// <param name="values">参数值列表</param>
        public static Expression CallRegEx(this Expression instance, params object[] values)
        {
            return Expression.Call(typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string) }), instance, values.Select(Expression.Constant).First());
        }

        /// <summary>
        /// 创建调用方法表达式
        /// </summary>
        /// <param name="instance">调用的实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="paramTypes">参数类型列表</param>
        /// <param name="values">参数值列表</param>
        public static Expression Not(this Expression instance, string methodName, Type[] paramTypes, params object[] values)
        {
            return Expression.Not(Call(instance, methodName, paramTypes, values));
        }

        #endregion



        #region Compose(组合表达式)

        /// <summary>
        /// 组合表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="first">左操作数</param>
        /// <param name="second">右操作数</param>
        /// <param name="merge">合并操作</param>
        internal static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        #endregion

        #region ToLambda(创建Lambda表达式)

        /// <summary>
        /// 创建Lambda表达式
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="body">表达式</param>
        /// <param name="parameters">参数列表</param>
        public static Expression<TDelegate> ToLambda<TDelegate>(this Expression body, params ParameterExpression[] parameters)
        {
            if (body == null)
                return null;
            return Expression.Lambda<TDelegate>(body, parameters);
        }

        #endregion
    }

    public static class Lambda
    {

        #region GetMember(获取成员)

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="expression">表达式,范例：t => t.Name</param>
        public static MemberInfo GetMember(Expression expression)
        {
            var memberExpression = GetMemberExpression(expression);
            return memberExpression?.Member;
        }

        /// <summary>
        /// 获取成员表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        public static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression == null)
                return null;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return GetMemberExpression(((LambdaExpression)expression).Body);
                case ExpressionType.Convert:
                    return GetMemberExpression(((UnaryExpression)expression).Operand);
                case ExpressionType.MemberAccess:
                    return (MemberExpression)expression;
            }
            return null;
        }

        #endregion

        #region GetName(获取成员名称)

        /// <summary>
        /// 获取成员名称，范例：t => t.Name,返回 Name
        /// </summary>
        /// <param name="expression">表达式,范例：t => t.Name</param>
        public static string GetName(Expression expression)
        {
            var memberExpression = GetMemberExpression(expression);
            return GetMemberName(memberExpression);
        }

        /// <summary>
        /// 获取成员名称
        /// </summary>
        public static string GetMemberName(MemberExpression memberExpression)
        {
            if (memberExpression == null)
                return string.Empty;
            string result = memberExpression.ToString();
            return result.Substring(result.IndexOf(".", StringComparison.Ordinal) + 1);
        }

        #endregion

        #region GetNames(获取名称列表)

        /// <summary>
        /// 获取名称列表
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expression">属性集合表达式,范例：t => new object[]{t.A,t.B}</param>
        public static List<string> GetNames<T>(Expression<Func<T, object[]>> expression)
        {
            var result = new List<string>();
            if (expression == null)
                return result;
            var arrayExpression = expression.Body as NewArrayExpression;
            if (arrayExpression == null)
                return result;
            foreach (var each in arrayExpression.Expressions)
                AddName(result, each);
            return result;
        }

        /// <summary>
        /// 添加名称
        /// </summary>
        private static void AddName(List<string> result, Expression expression)
        {
            var name = GetName(expression);
            if (string.IsNullOrWhiteSpace(name))
                return;
            result.Add(name);
        }

        #endregion

        #region GetValue(获取值)

        /// <summary>
        /// 获取值,范例：t => t.Name == "A",返回 A
        /// </summary>
        /// <param name="expression">表达式,范例：t => t.Name == "A"</param>
        public static object GetValue(Expression expression)
        {
            if (expression == null)
                return null;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return GetValue(((LambdaExpression)expression).Body);
                case ExpressionType.Convert:
                    return GetValue(((UnaryExpression)expression).Operand);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                    return GetValue(((BinaryExpression)expression).Right);
                case ExpressionType.Call:
                    return GetMethodCallExpressionValue(expression);
                case ExpressionType.MemberAccess:
                    return GetMemberValue((MemberExpression)expression);
                case ExpressionType.Constant:
                    return GetConstantExpressionValue(expression);
            }
            return null;
        }

        /// <summary>
        /// 获取方法调用表达式的值
        /// </summary>
        private static object GetMethodCallExpressionValue(Expression expression)
        {
            var methodCallExpression = (MethodCallExpression)expression;
            var value = GetValue(methodCallExpression.Arguments.FirstOrDefault());
            if (value != null)
                return value;
            return GetValue(methodCallExpression.Object);
        }

        /// <summary>
        /// 获取属性表达式的值
        /// </summary>
        private static object GetMemberValue(MemberExpression expression)
        {
            if (expression == null)
                return null;
            var field = expression.Member as FieldInfo;
            if (field != null)
            {
                var constValue = GetConstantExpressionValue(expression.Expression);
                return field.GetValue(constValue);
            }
            var property = expression.Member as PropertyInfo;
            if (property == null)
                return null;
            if (expression.Expression == null)
                return property.GetValue(null);
            var value = GetMemberValue(expression.Expression as MemberExpression);
            if (value == null)
                return null;
            return property.GetValue(value);
        }

        /// <summary>
        /// 获取常量表达式的值
        /// </summary>
        private static object GetConstantExpressionValue(Expression expression)
        {
            var constantExpression = (ConstantExpression)expression;
            return constantExpression.Value;
        }

        #endregion

        #region GetParameter(获取参数)

        /// <summary>
        /// 获取参数，范例：t.Name,返回 t
        /// </summary>
        /// <param name="expression">表达式，范例：t.Name</param>
        public static ParameterExpression GetParameter(Expression expression)
        {
            if (expression == null)
                return null;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return GetParameter(((LambdaExpression)expression).Body);
                case ExpressionType.Convert:
                    return GetParameter(((UnaryExpression)expression).Operand);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                    return GetParameter(((BinaryExpression)expression).Left);
                case ExpressionType.MemberAccess:
                    return GetParameter(((MemberExpression)expression).Expression);
                case ExpressionType.Call:
                    return GetParameter(((MethodCallExpression)expression).Object);
                case ExpressionType.Parameter:
                    return (ParameterExpression)expression;
            }
            return null;
        }

        #endregion

        #region GetConditionCount(获取查询条件个数)

        /// <summary>
        /// 获取查询条件个数
        /// </summary>
        /// <param name="expression">谓词表达式,范例1：t => t.Name == "A" ，结果1。
        /// 范例2：t => t.Name == "A" &amp;&amp; t.Age =1 ，结果2。</param>
        public static int GetConditionCount(LambdaExpression expression)
        {
            if (expression == null)
                return 0;
            var result = expression.ToString().Replace("AndAlso", "|").Replace("OrElse", "|");
            return result.Split('|').Count();
        }

        #endregion

        #region GetAttribute(获取特性)

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="expression">属性表达式</param>
        public static TAttribute GetAttribute<TAttribute>(Expression expression) where TAttribute : Attribute
        {
            var memberInfo = GetMember(expression);
            return memberInfo.GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="propertyExpression">属性表达式</param>
        public static TAttribute GetAttribute<TEntity, TProperty, TAttribute>(Expression<Func<TEntity, TProperty>> propertyExpression) where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(propertyExpression);
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="propertyExpression">属性表达式</param>
        public static TAttribute GetAttribute<TProperty, TAttribute>(Expression<Func<TProperty>> propertyExpression) where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(propertyExpression);
        }

        #endregion

        #region GetAttributes(获取特性列表)

        /// <summary>
        /// 获取特性列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="propertyExpression">属性表达式</param>
        public static IEnumerable<TAttribute> GetAttributes<TEntity, TProperty, TAttribute>(Expression<Func<TEntity, TProperty>> propertyExpression) where TAttribute : Attribute
        {
            var memberInfo = GetMember(propertyExpression);
            return memberInfo.GetCustomAttributes<TAttribute>();
        }

        #endregion

        #region Constant(获取常量)

        /// <summary>
        /// 获取常量表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="value">值</param>
        public static ConstantExpression Constant(Expression expression, object value)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null)
                return Expression.Constant(value);
            return Expression.Constant(value, memberExpression.Type);
        }

        #endregion

        #region Equal(等于表达式)

        /// <summary>
        /// 创建等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> Equal<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .Equal(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        private static ParameterExpression CreateParameter<T>()
        {
            return Expression.Parameter(typeof(T), "t");
        }

        #endregion

        #region NotEqual(不等于表达式)

        /// <summary>
        /// 创建不等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> NotEqual<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .NotEqual(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Greater(大于表达式)

        /// <summary>
        /// 创建大于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> Greater<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .Greater(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region GreaterEqual(大于等于表达式)

        /// <summary>
        /// 创建大于等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> GreaterEqual<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .GreaterEqual(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Less(小于表达式)

        /// <summary>
        /// 创建小于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> Less<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .Less(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region LessEqual(小于等于表达式)

        /// <summary>
        /// 创建小于等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> LessEqual<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .LessEqual(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Starts(调用StartsWith方法)

        /// <summary>
        /// 调用StartsWith方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> Starts<T>(string propertyName, string value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .StartsWith(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }


        /// <summary>
        /// 调用RegEx方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> RegEx<T>(string propertyName, string value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .RegEx(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Ends(调用EndsWith方法)

        /// <summary>
        /// 调用EndsWith方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> Ends<T>(string propertyName, string value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .EndsWith(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Contains(调用Contains方法)

        /// <summary>
        /// 调用Contains方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> Contains<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .Contains(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        /// <summary>
        /// 调用Contains方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public static Expression<Func<T, bool>> NotContains<T>(string propertyName, object value)
        {
            var parameter = CreateParameter<T>();
            return parameter.Property(propertyName)
                    .NotContains(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region ParsePredicate(解析为谓词表达式)

        /// <summary>
        /// 解析为谓词表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <param name="operator">运算符</param>
        public static Expression<Func<T, bool>> ParsePredicate<T>(string propertyName, object value, Operation @operator)
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            return parameter.Property(propertyName).Operation(@operator, value).ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static Expression<Func<T, TKey>> OrderExpression<T, TKey>(string propertyName)
        {
            var parameter = CreateParameter<T>();
            return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, propertyName), parameter);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        /// <summary>
        /// 参数字典
        /// </summary>
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        /// <summary>
        /// 初始化参数重绑定操作
        /// </summary>
        /// <param name="map">参数字典</param>
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 替换参数
        /// </summary>
        /// <param name="map">参数字典</param>
        /// <param name="exp">表达式</param>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// 访问参数
        /// </summary>
        /// <param name="parameterExpression">参数</param>
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (_map.TryGetValue(parameterExpression, out var replacement))
                parameterExpression = replacement;
            return base.VisitParameter(parameterExpression);
        }
    }
}
