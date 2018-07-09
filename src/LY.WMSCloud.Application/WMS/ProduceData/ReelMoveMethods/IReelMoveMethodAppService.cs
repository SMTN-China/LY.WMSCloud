using LY.WMSCloud.WMS.ProduceData.ReelMoveMethods.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReelMoveMethods
{
    public interface IReelMoveMethodAppService : IServiceBase<ReelMoveMethodDto, string>
    {
        Task<ICollection<ReelMoveMethodDto>> GetReelMoveMethodByKeyName(string keyName);

    }
}
