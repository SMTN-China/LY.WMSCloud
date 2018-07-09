using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud
{
    [AbpAuthorize]
    public class ServiceBase<TEntity, TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput> : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, PagedResultRequestInput, TCreateInput, TUpdateInput>
        , IServiceBase<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput>
         where TEntity : class, IEntity<TPrimaryKey>, new()
         where TEntityDto : class, IEntityDto<TPrimaryKey>
         where TCreateInput : class
         where TUpdateInput : class, IEntityDto<TPrimaryKey>
    {
        readonly IWMSRepositories<TEntity, TPrimaryKey> _repository;
        public UserManager UserManager { get; set; }

        IQueryable<TEntity> GetAllQueryable { get; set; }

        public ServiceBase(
            IWMSRepositories<TEntity, TPrimaryKey> repository
            ) : base(repository)
        {
            _repository = repository;
            var entityName = typeof(TEntity).Name;

            CreatePermissionName = "Pages." + entityName + "s.Create";
            GetAllPermissionName = "Pages." + entityName + "s";
            GetPermissionName = "Pages." + entityName + "s";
            UpdatePermissionName = "Pages." + entityName + "s.Update";
            DeletePermissionName = "Pages." + entityName + "s.Delete";
            LocalizationSourceName = WMSCloudConsts.LocalizationSourceName;
        }

        [HttpPost]
        public async override Task<PagedResultDto<TEntityDto>> GetAll(PagedResultRequestInput input)
        {
            CheckGetAllPermission();

            var query = _repository.DynamicQuery(input);

            var tasksCount = await query.CountAsync();

            var taskList = query.PageBy(input).ToList();

            return new PagedResultDto<TEntityDto>(tasksCount, ObjectMapper.Map<List<TEntityDto>>(taskList));
        }

        public override async Task<TEntityDto> Get(EntityDto<TPrimaryKey> input)
        {
            CheckGetPermission();

            var entity = await base.Get(input);

            if (entity != null)
            {
                if (entity is BaseDto<TPrimaryKey>)
                {
                    var baseDto = entity as BaseDto<TPrimaryKey>;

                    var creatorUser = await UserManager.Users.FirstOrDefaultAsync(u => u.Id == baseDto.CreatorUserId);

                    if (creatorUser != null)
                    {
                        baseDto.CreatorUser = creatorUser.Name;
                    }

                    var lastModifierUser = await UserManager.Users.FirstOrDefaultAsync(u => u.Id == baseDto.LastModifierUserId);

                    if (lastModifierUser != null)
                    {
                        baseDto.LastModifierUser = lastModifierUser.Name;
                    }
                }
            }

            return entity;
        }

        public override Task<TEntityDto> Create(TCreateInput input)
        {
            CheckCreatePermission();
            return base.Create(input);
        }

        public override Task<TEntityDto> Update(TUpdateInput input)
        {
            CheckUpdatePermission();
            return base.Update(input);
        }

        public override Task Delete(EntityDto<TPrimaryKey> input)
        {
            CheckDeletePermission();
            return base.Delete(input);
        }
    }

    public class ServiceBase<TEntity, TEntityDto, TPrimaryKey, TEditInput> : ServiceBase<TEntity, TEntityDto, TPrimaryKey, TEditInput, TEditInput>, IServiceBase<TEntityDto, TPrimaryKey, TEditInput>

        where TEntity : class, IEntity<TPrimaryKey>, new()
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TEditInput : class, IEntityDto<TPrimaryKey>
    {
        public ServiceBase(
            IWMSRepositories<TEntity, TPrimaryKey> repository
            ) : base(repository)
        {

        }
    }

    public class ServiceBase<TEntity, TEntityDto, TPrimaryKey> : ServiceBase<TEntity, TEntityDto, TPrimaryKey, TEntityDto>, IServiceBase<TEntityDto, TPrimaryKey>

       where TEntity : class, IEntity<TPrimaryKey>, new()
       where TEntityDto : class, IEntityDto<TPrimaryKey>
    {
        public ServiceBase(
             IWMSRepositories<TEntity, TPrimaryKey> repository

             ) : base(repository)
        {

        }
    }


    public class DefaultServiceBase<TEntity, TEntityDto, TCreateInput, TUpdateInput> : ServiceBase<TEntity, TEntityDto, string, TCreateInput, TUpdateInput>
        , IDefaultServiceBase<TEntityDto, TCreateInput, TUpdateInput>
         where TEntity : class, IEntity<string>, new()
         where TEntityDto : class, IEntityDto<string>
         where TCreateInput : class
         where TUpdateInput : class, IEntityDto<string>
    {
        public DefaultServiceBase(
            IWMSRepositories<TEntity, string> repository
            ) : base(repository)
        {

        }
    }

    public class DefaultServiceBase<TEntity, TEntityDto, TEditInput> : ServiceBase<TEntity, TEntityDto, string, TEditInput, TEditInput>, IDefaultServiceBase<TEntityDto, TEditInput>
       where TEntity : class, IEntity<string>, new()
       where TEntityDto : class, IEntityDto<string>
       where TEditInput : class, IEntityDto<string>
    {
        public DefaultServiceBase(
            IWMSRepositories<TEntity, string> repository
            ) : base(repository)
        {

        }
    }

    public class DefaultServiceBase<TEntity, TEntityDto> : ServiceBase<TEntity, TEntityDto, string, TEntityDto>, IDefaultServiceBase<TEntityDto>
       where TEntity : class, IEntity<string>, new()
       where TEntityDto : class, IEntityDto<string>
    {
        public DefaultServiceBase(
            IWMSRepositories<TEntity, string> repository
            ) : base(repository)
        {

        }
    }
}
