using LY.WMSCloud.WMS.BaseData.Lines.Dto;
using LY.WMSCloud.WMS.BaseData.Storages.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Lines
{
    public interface ILineAppService : IServiceBase<LineDto,string>
    {
       

        Task<ICollection<LineDto>> GetLineByKeyName(string keyName);
    }
}
