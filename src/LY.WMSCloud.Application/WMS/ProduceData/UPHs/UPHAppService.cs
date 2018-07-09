using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.UPHs.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.UPHs
{
    public class UPHAppService : ServiceBase<UPH, UPHDto, string>, IUPHAppService
    {
        public UPHAppService(IWMSRepositories<UPH, string> repository) : base(repository)
        {
        }
    }
}
