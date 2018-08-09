using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.WMS.ProduceData.WorkBills.Dto;
using Microsoft.EntityFrameworkCore;

namespace LY.WMSCloud.WMS.ProduceData.WorkBills
{
    public class WorkBillAppService : ServiceBase<WorkBill, WorkBillDto, string>, IWorkBillAppService
    {
        readonly IWMSRepositories<WorkBill, string> _repository;
        public WorkBillAppService(IWMSRepositories<WorkBill, string> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<WorkBillDto>> GetWorkBillByKeyName(string keyName)
        {
            if (keyName == null)
            {
                keyName = "";
            }
            var res = await _repository.GetAll().Where(w => w.Id.Contains(keyName) && w.IsActive && w.Qty > w.ReadyMQty).Take(10).ToListAsync();

            return ObjectMapper.Map<List<WorkBillDto>>(res);
        }
    }
}
