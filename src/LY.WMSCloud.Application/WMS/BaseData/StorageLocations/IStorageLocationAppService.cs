using LY.WMSCloud.WMS.BaseData.StorageLocations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.StorageLocations
{
    public interface IStorageLocationAppService : IServiceBase<StorageLocationDto, string>
    {
        Task<bool> GetIsHave(string id);

        Task AddByLY(LYDto lYDto);

        Task AllBright();

        Task NonReelBright();

        Task AllExtinguished();
    }
}
