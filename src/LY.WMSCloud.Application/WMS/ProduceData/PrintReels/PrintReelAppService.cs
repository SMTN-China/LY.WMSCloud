using Abp.Configuration;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.PrintReels.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.PrintReels
{
    public class PrintReelAppService : ServiceBase<PrintReel, PrintReelDto, string>, IPrintReelAppService
    {
        readonly IWMSRepositories<PrintReel, string> _repository;
        readonly IWMSRepositories<Setting, long> _repositoryT;
        readonly IWMSRepositories<MPN, string> _repositoryMPN;

        public PrintReelAppService(
            IWMSRepositories<PrintReel, string> repository,
            IWMSRepositories<Setting, long> repositoryT,
            IWMSRepositories<MPN, string> repositoryMPN

            ) : base(repository)
        {
            _repository = repository;
            _repositoryT = repositoryT;
            _repositoryMPN = repositoryMPN;
        }
        [HttpPost]
        public async Task<PrintReelDto> GetNewPrintReel(PrintReelDto printReelDto)
        {

            var printStartStr = await _repositoryT.FirstOrDefaultAsync(r => r.TenantId == AbpSession.TenantId && r.Name == "printStartStr");

            var partStr = DateTime.Now.ToString("yyyyMMdd");

            if (printStartStr != null)
            {
                partStr = printStartStr.Value + partStr;
            }
            printReelDto.PrintStr = partStr;
            var printReelOld = _repository.GetAll().Where(r => r.PrintStr == partStr).OrderByDescending(r => r.PrintIndex).FirstOrDefault();
            if (printReelOld == null)
            {
                printReelDto.PrintIndex = 1;
            }
            else
            {
                printReelDto.PrintIndex = printReelOld.PrintIndex + 1;
            }
            printReelDto.Id = partStr + printReelDto.PrintIndex.ToString().PadLeft(5, '0');
            var res = await this.Create(printReelDto);
            return res;
        }
    }
}
