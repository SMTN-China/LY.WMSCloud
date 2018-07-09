using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using LY.WMSCloud.Configuration;
using LY.WMSCloud.Web;

namespace LY.WMSCloud.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class WMSCloudDbContextFactory : IDesignTimeDbContextFactory<WMSCloudDbContext>
    {
        public WMSCloudDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<WMSCloudDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            WMSCloudDbContextConfigurer.Configure(builder, configuration.GetConnectionString(WMSCloudConsts.ConnectionStringName));

            return new WMSCloudDbContext(builder.Options);
        }
    }
}
