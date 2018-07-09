using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LY.WMSCloud.Configuration;

namespace LY.WMSCloud.Web.Host.Startup
{
    [DependsOn(
       typeof(WMSCloudWebCoreModule))]
    public class WMSCloudWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public WMSCloudWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WMSCloudWebHostModule).GetAssembly());
        }
    }
}
