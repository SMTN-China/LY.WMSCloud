using LY.WMSCloud.WMS.BaseData.Customers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.Customers
{
    public interface ICustomerAppService : IServiceBase<CustomerDto, string>
    {

        Task<ICollection<CustomerDto>> GetCustomerByKeyName(string keyName);

        Task<ICollection<CustomerDto>> GetCustomerById(string id);
    }
}
