using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.WMS.BaseData.Customers.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Customers
{
    public class CustomerAppService : ServiceBase<Customer, CustomerDto, string>, ICustomerAppService
    {
        readonly IWMSRepositories<Customer, string> _repository;
        public CustomerAppService(IWMSRepositories<Customer, string> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<CustomerDto>> GetCustomerById(string Id)
        {
            if (Id == null)
            {
                Id = "";
            }
            var res = await _repository.GetAll().Where(c => c.Id.Contains(Id)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<CustomerDto>>(res);
        }

        public async Task<ICollection<CustomerDto>> GetCustomerByKeyName(string keyName)
        {
            if (keyName == null)
            {
                keyName = "";
            }
            var res = await _repository.GetAll().Where(c => c.Id.Contains(keyName)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<CustomerDto>>(res);
        }
    }
}
