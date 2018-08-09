using Abp.Dependency;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using LY.WMSCloud.EntityFrameworkCore.Seed;
using System.Transactions;

namespace LY.WMSCloud.EntityFrameworkCore
{
    [DependsOn(
        typeof(WMSCloudCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class WMSCloudEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<WMSCloudDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        WMSCloudDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        WMSCloudDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.Register(typeof(IWMSRepositories<,>), typeof(Repositories.WMSRepositories<,>), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IWMSRepositories<>), typeof(Repositories.WMSRepositories<>), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(Core.Customized.Foxlink.IFoxLinkRepositories), typeof(Customized.Foxlink.FoxLinkRepositories), DependencyLifeStyle.Transient);


            IocManager.RegisterAssemblyByConvention(typeof(WMSCloudEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            // Configuration.UnitOfWork.IsolationLevel = IsolationLevel.ReadCommitted; Orcale 需求取消注释
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
