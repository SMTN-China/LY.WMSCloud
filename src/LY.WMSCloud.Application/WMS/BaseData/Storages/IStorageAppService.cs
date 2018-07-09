using LY.WMSCloud.WMS.BaseData.Storages.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Storages
{
    public interface IStorageAppService : IServiceBase<StorageDto, string>
    {
        Task<ICollection<StorageDto>> GetCStorageByKeyName(string keyName);
        Task<ICollection<StorageDto>> GetSStorageByKeyName(string keyName);
        Task<ICollection<StorageDto>> GetStorageByKeyName(string keyName);
    }
}
