using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud
{
    public interface IWMSRepositories<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, new()
    {
        /// <summary>
        /// 动态查询
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>返回查询</returns>
        IQueryable<TEntity> DynamicQuery(PagedResultRequestInput input);
    }

    public interface IWMSRepositories<TEntity> : IWMSRepositories<TEntity, int> where TEntity : class, IEntity<int>, new()
    {

    }
}
