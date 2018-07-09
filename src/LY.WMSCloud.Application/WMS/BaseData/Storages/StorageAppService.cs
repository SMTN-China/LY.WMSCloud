using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.BaseData.Storages.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Storages
{
    public class StorageAppService : ServiceBase<Storage, StorageDto, string>, IStorageAppService
    {
        readonly IWMSRepositories<Storage, string> _repository;
        public StorageAppService(IWMSRepositories<Storage, string> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<StorageDto>> GetStorageByKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(c => c.Id.Contains(keyName)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<StorageDto>>(res);
        }

        public async Task<ICollection<StorageDto>> GetSStorageByKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(c => (c.IncomingMethod == IncomingMethod.ForSelf || c.IncomingMethod == IncomingMethod.Other) && c.Id.Contains(keyName)).Take(10).ToListAsync();
            return ObjectMapper.Map<List<StorageDto>>(res);
        }
        public async Task<ICollection<StorageDto>> GetCStorageByKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(c => (c.IncomingMethod == IncomingMethod.ForCustomer || c.IncomingMethod == IncomingMethod.Other) && c.Id.Contains(keyName)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<StorageDto>>(res);
        }
    }
}
