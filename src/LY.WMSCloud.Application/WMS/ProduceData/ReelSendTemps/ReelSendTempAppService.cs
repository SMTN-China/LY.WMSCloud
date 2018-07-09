using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.ReelSendTemps.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReelSendTemps
{
    public class ReelSendTempAppService : ServiceBase<ReelSendTemp, ReelSendTempDto, string>, IReelSendTempAppService
    {
        public ReelSendTempAppService(IWMSRepositories<ReelSendTemp, string> repository) : base(repository)
        {
        }
    }
}
