using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LY.WMSCloud.Authorization;
using LY.WMSCloud.CommonService;

namespace LY.WMSCloud
{
    [DependsOn(
        typeof(WMSCloudCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class WMSCloudApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<WMSCloudAuthorizationProvider>();
            Configuration.Settings.Providers.Add<WmsSettingProvider>();

        }

        public override void Initialize()
        {
            //var thisAssembly = typeof(WMSCloudApplicationModule).GetAssembly();

            //IocManager.RegisterAssemblyByConvention(thisAssembly);

            //Configuration.Modules.AbpAutoMapper().Configurators.Add(
            //    // Scan the assembly for classes which inherit from AutoMapper.Profile
            //    cfg => cfg.AddProfiles(thisAssembly)
            //);

            var thisAssembly = typeof(WMSCloudApplicationModule).GetAssembly();

            IocManager.Register(typeof(IServiceBase<,,,>), typeof(ServiceBase<,,,,>), Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IServiceBase<,,>), typeof(ServiceBase<,,,>), Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IServiceBase<,>), typeof(ServiceBase<,,>), Abp.Dependency.DependencyLifeStyle.Transient);

            IocManager.Register(typeof(IDefaultServiceBase<,,>), typeof(DefaultServiceBase<,,,>), Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IDefaultServiceBase<,>), typeof(DefaultServiceBase<,,>), Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IDefaultServiceBase<>), typeof(DefaultServiceBase<,>), Abp.Dependency.DependencyLifeStyle.Transient);

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            IocManager.Register<HttpHelp>(Abp.Dependency.DependencyLifeStyle.Singleton);

            IocManager.Register<FileHelperService>(Abp.Dependency.DependencyLifeStyle.Singleton);

            IocManager.Register<LightService>(Abp.Dependency.DependencyLifeStyle.Singleton);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
