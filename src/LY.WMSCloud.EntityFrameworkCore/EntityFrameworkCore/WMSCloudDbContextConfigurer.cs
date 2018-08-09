using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace LY.WMSCloud.EntityFrameworkCore
{
    public static class WMSCloudDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<WMSCloudDbContext> builder, string connectionString)
        {
            builder
               .UseLazyLoadingProxies()
               .UseMySql(connectionString);
            // .UseOracle(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<WMSCloudDbContext> builder, DbConnection connection)
        {
            builder
               .UseLazyLoadingProxies()
               .UseMySql(connection);
            // .UseOracle(connection);
        }
    }
}
