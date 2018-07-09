using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.EntityFrameworkCore.Repositories
{
    public class WMSRepositories<TEntity, TPrimaryKey> : WMSCloudRepositoryBase<TEntity, TPrimaryKey>, IWMSRepositories<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, new()
    {
        public WMSRepositories(IDbContextProvider<WMSCloudDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public IQueryable<TEntity> DynamicQuery(PagedResultRequestInput input)
        {
            var sourceData = GetAll();
            return sourceData.DynamicQuery(input);
        }
    }

    public class WMSRepositories<TEntity> : WMSRepositories<TEntity, int>, IWMSRepositories<TEntity>
       where TEntity : class, IEntity<int>, new()
    {
        public WMSRepositories(IDbContextProvider<WMSCloudDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
