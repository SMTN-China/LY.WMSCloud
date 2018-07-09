using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.BaseData.Lines.Dto;
using LY.WMSCloud.WMS.BaseData.Storages.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Lines
{
    public class LineAppService : ServiceBase<Line, LineDto, string>, ILineAppService
    {
        readonly IWMSRepositories<Line, string> _repository;
        public LineAppService(IWMSRepositories<Line, string> repository) : base(repository)
        {
            _repository = repository;
        }



        public async Task<ICollection<LineDto>> GetLineByKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(l => l.Id.Contains(keyName)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<LineDto>>(res);
        }


    }
}
