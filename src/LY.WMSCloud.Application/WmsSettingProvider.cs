using Abp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud
{
    public class WmsSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                    {
                    new SettingDefinition("reelMoveMethodId","",scopes: SettingScopes.Tenant),
                    new SettingDefinition("defaultForCustomerMStorageId","001",scopes: SettingScopes.Tenant),
                    new SettingDefinition("defaultForSelfMStorageId","001",scopes: SettingScopes.Tenant),
                    new SettingDefinition("asyncInterval","4",scopes: SettingScopes.Tenant),
                    new SettingDefinition("mustFifoDay","30",scopes: SettingScopes.Tenant),
                    new SettingDefinition("overdueDay","30",scopes: SettingScopes.Tenant),
                    new SettingDefinition("readyLossQty","200",scopes: SettingScopes.Tenant),
                    new SettingDefinition("readyFirstMinimumQty","2000",scopes: SettingScopes.Tenant),
                    new SettingDefinition("lightIsRGB","1",scopes: SettingScopes.Tenant)

                };
        }
    }
}
