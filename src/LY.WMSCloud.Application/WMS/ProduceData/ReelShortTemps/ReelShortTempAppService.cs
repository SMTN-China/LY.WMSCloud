using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.ReelShortTemps.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelShortTemps
{
    public class ReelShortTempAppService : ServiceBase<ReelShortTemp, ReelShortTempDto, string>, IReelShortTempAppService
    {
        public ReelShortTempAppService(IWMSRepositories<ReelShortTemp, string> repository) : base(repository)
        {
        }
    }
}
