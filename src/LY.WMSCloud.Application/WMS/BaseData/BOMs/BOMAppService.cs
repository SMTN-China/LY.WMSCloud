using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Linq.Extensions;
using AutoMapper;
using LY.WMSCloud.Authorization;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.WMS.BaseData.BOMs.Dto;
using LY.WMSCloud.WMS.BaseData.MPNs.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.BOMs
{
    public class BOMAppService : ServiceBase<BOM, ProductDto, string, BOMDto>, IBOMAppService
    {
        readonly IWMSRepositories<BOM, string> _repository;
        readonly IWMSRepositories<MPN, string> _repositoryMPN;
        MapperConfiguration config;
        public BOMAppService(IWMSRepositories<BOM, string> repository, IWMSRepositories<MPN, string> repositoryMPN) : base(repository)
        {
            _repository = repository;
            _repositoryMPN = repositoryMPN;

            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MPN, ProductDto>().ForMember(m => m.ItemCount, opt => opt.MapFrom(s => _repository.GetAllList(b => b.ProductId == s.Id).Count));

                cfg.CreateMap<BOM, BOMDto>();
            }
             );
        }

        [HttpPost]
        public PagedResultDto<BOMDto> GetItemsById(string Id, PagedResultRequestInput input)
        {
            // 查询
            // var query = _repository.GetAll().Where(m => m.ProductId == Id);


            var query = config.CreateMapper().Map<List<BOM>, List<BOMDto>>(
             _repository.GetAll().Where(m => m.ProductId == Id).ToList());

            var res = query.AsQueryable().DynamicQuery(input);

            var tasksCount = res.Count();

            //默认的分页方式
            //var taskList = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            //ABP提供了扩展方法PageBy分页方式
            var taskList = res.PageBy(input).ToList();

            return new PagedResultDto<BOMDto>(tasksCount, taskList);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_BOMs)]
        public async override Task<PagedResultDto<ProductDto>> GetAll(PagedResultRequestInput input)
        {
            // 查询 添加组件查询
            // var query = _repositoryMPN.GetAll().Where(m => m.MPNHierarchy == MPNHierarchy.组件);
            input.RequestWMSDtos.Add(new RequestWMSDto() { LinkOperation = LinkOperation.And, Operation = Operation.Equal, PropertyName = "MPNHierarchy", QueryValue = 0 });

            //获取总数
            var query = _repositoryMPN.DynamicQuery(input);

            var tasksCount = await query.CountAsync();

            //默认的分页方式
            //var taskList = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            //ABP提供了扩展方法PageBy分页方式
            var taskList = query.PageBy(input).ToList();


            return new PagedResultDto<ProductDto>(tasksCount, config.CreateMapper().Map<List<MPN>, List<ProductDto>>(taskList));
        }

        public async Task<ICollection<string>> GetMainPNById(string productId, string partNoId)
        {
            var pns = await _repository.GetAll().Where(r => r.ProductId == productId && r.PartNoId == r.MainPartNoId && r.PartNoId.Contains(partNoId)).Select(r => r.PartNoId).ToArrayAsync();

            return pns;
        }
    }
}
