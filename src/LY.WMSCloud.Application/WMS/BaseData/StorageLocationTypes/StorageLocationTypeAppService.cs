using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.BaseData.StorageLocationTypes.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.StorageLocationTypes
{
    public class StorageLocationTypeAppService : ServiceBase<StorageLocationType, StorageLocationTypeDto, string>, IStorageLocationTypeAppService
    {
        readonly IWMSRepositories<StorageLocationType, string> _repository;
        public StorageLocationTypeAppService(IWMSRepositories<StorageLocationType, string> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<StorageLocationTypeDto>> GetStorageLocationTypeByKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(c => c.Id.Contains(keyName)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<StorageLocationTypeDto>>(res);
        }
    }
}
