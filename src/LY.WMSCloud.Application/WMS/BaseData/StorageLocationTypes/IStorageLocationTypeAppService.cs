using LY.WMSCloud.WMS.BaseData.StorageLocationTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.StorageLocationTypes
{
    public interface IStorageLocationTypeAppService : IServiceBase<StorageLocationTypeDto, string>
    {
        Task<ICollection<StorageLocationTypeDto>> GetStorageLocationTypeByKeyName(string keyName);

    }
}
