using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.ProduceData.ReelMoveMethods.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveMethods
{
    public class ReelMoveMethodAppService : ServiceBase<ReelMoveMethod, ReelMoveMethodDto, string>, IReelMoveMethodAppService
    {
        readonly IWMSRepositories<RMMStorageMap, string> _repositoryRMMStorageMap;
        readonly IWMSRepositories<ReelMoveMethod, string> _repository;

        public ReelMoveMethodAppService(IWMSRepositories<ReelMoveMethod, string> repository, IWMSRepositories<RMMStorageMap, string> repositoryRMMStorageMap) : base(repository)
        {
            _repositoryRMMStorageMap = repositoryRMMStorageMap;
            _repository = repository;
        }
        [HttpPost]
        public async override Task<PagedResultDto<ReelMoveMethodDto>> GetAll(PagedResultRequestInput input)
        {
            CheckGetAllPermission();

            var query = _repository.DynamicQuery(input).Include(r=>r.OutStorages);

            var tasksCount = await query.CountAsync();

            var taskList = query.PageBy(input).ToList();

            return new PagedResultDto<ReelMoveMethodDto>(tasksCount, ObjectMapper.Map<List<ReelMoveMethodDto>>(taskList));
        }

        public override Task<ReelMoveMethodDto> Update(ReelMoveMethodDto input)
        {
            foreach (var item in _repositoryRMMStorageMap.GetAll().Where(r => r.ReelMoveMethodId == input.Id).ToList())
            {
                _repositoryRMMStorageMap.Delete(item.Id);
            }
            CurrentUnitOfWork.SaveChanges();
            return base.Update(input);
        }

        public async Task<ICollection<ReelMoveMethodDto>> GetReelMoveMethodByKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(c => c.Id.Contains(keyName) && c.AllocationTypesStr.ToLower().Contains("send")).Take(10).ToListAsync();  //  && c.AllocationType.ToString().ToLower().Contains("send")

            return ObjectMapper.Map<List<ReelMoveMethodDto>>(res);
        }
    }
}
