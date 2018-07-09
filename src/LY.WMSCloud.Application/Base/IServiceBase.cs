using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud
{
    public interface IServiceBase<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput> : IAsyncCrudAppService<TEntityDto, TPrimaryKey, PagedResultRequestInput, TCreateInput, TUpdateInput>
         where TEntityDto : class, IEntityDto<TPrimaryKey>
         where TCreateInput : class
         where TUpdateInput : class, IEntityDto<TPrimaryKey>
    {
        
    }

    public interface IServiceBase<TEntityDto, TPrimaryKey, TEditInput> : IServiceBase<TEntityDto, TPrimaryKey, TEditInput, TEditInput>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TEditInput : class, IEntityDto<TPrimaryKey>
    {

    }

    public interface IServiceBase<TEntityDto, TPrimaryKey> : IServiceBase<TEntityDto, TPrimaryKey, TEntityDto>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
    {

    }


    public interface IDefaultServiceBase<TEntityDto, TCreateInput, TUpdateInput> : IServiceBase<TEntityDto, string, TCreateInput, TUpdateInput>
         where TEntityDto : class, IEntityDto<string>
         where TCreateInput : class
         where TUpdateInput : class, IEntityDto<string>
    {

    }

    public interface IDefaultServiceBase<TEntityDto, TEditInput> : IDefaultServiceBase<TEntityDto, TEditInput, TEditInput>
        where TEntityDto : class, IEntityDto<string>
        where TEditInput : class, IEntityDto<string>
    {

    }

    public interface IDefaultServiceBase<TEntityDto> : IDefaultServiceBase<TEntityDto, TEntityDto>
        where TEntityDto : class, IEntityDto<string>
    {

    }
}
