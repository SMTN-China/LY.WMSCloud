using LY.WMSCloud.WMS.ProduceData.PrintReels.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.PrintReels
{
    public interface IPrintReelAppService : IServiceBase<PrintReelDto, string>
    {
        Task<PrintReelDto> GetNewPrintReel(PrintReelDto printReelDto);
    }
}
