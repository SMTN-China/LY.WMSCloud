using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.WMS.BaseData.BOMs.Dto;
using LY.WMSCloud.WMS.BaseData.MPNs.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.BOMs
{
    public interface IBOMAppService : IServiceBase<ProductDto, string,BOMDto>
    {
        PagedResultDto<BOMDto> GetItemsById(string Id, PagedResultRequestInput input);

        
    }
}
