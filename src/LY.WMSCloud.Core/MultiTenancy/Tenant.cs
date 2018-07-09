using Abp.MultiTenancy;
using LY.WMSCloud.Authorization.Users;

namespace LY.WMSCloud.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
