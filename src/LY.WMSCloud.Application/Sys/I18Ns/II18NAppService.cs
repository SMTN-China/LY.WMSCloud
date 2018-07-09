using Abp.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LY.WMSCloud.Sys.I18Ns.Dto;

namespace LY.WMSCloud.Sys.I18Ns
{
    public interface II18NAppService : IServiceBase<I18NDto, long>
    {
        Task<ICollection<I18NDto>> GetByDtoName(string dtoName);
    }
}
