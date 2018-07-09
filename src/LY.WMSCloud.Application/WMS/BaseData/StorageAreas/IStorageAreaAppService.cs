using LY.WMSCloud.WMS.BaseData.StorageAreas.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.StorageAreas
{
    public interface IStorageAreaAppService : IServiceBase<StorageAreaDto, string>
    {
        Task<ICollection<string>> GetShelfByKeyName(string keyName);

    }
}
