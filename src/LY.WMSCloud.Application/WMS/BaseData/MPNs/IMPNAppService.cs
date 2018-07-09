using LY.WMSCloud.WMS.BaseData.Customers.Dto;
using LY.WMSCloud.WMS.BaseData.MPNs.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.MPNs
{
    public interface IMPNAppService : IServiceBase<MPNDto, string>
    {


        Task<bool> BatchInsOrUpdate(ICollection<MPNDto> input);

        Task<ICollection<MPNDto>> GetProductByKeyName(string keyName);

        Task<ICollection<MPNDto>> GetPartNoByKeyName(string keyName);
    }
}
