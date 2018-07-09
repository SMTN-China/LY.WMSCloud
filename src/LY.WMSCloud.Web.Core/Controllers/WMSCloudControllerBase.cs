using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace LY.WMSCloud.Controllers
{
    public abstract class WMSCloudControllerBase: AbpController
    {
        protected WMSCloudControllerBase()
        {
            LocalizationSourceName = WMSCloudConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
