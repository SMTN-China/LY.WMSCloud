using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Runtime.Session;
using LY.WMSCloud.Configuration.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LY.WMSCloud.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : WMSCloudAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }

        [HttpPost]
        public async Task<ICollection<ISettingValue>> GetAppConfig(string[] names)
        {
            List<ISettingValue> list = new List<ISettingValue>();
            foreach (var name in names)
            {
                var value = await SettingManager.GetSettingValueForTenantAsync(name, AbpSession.TenantId.Value);
                list.Add(new SettingValue() { Name = name, Value = value });
            }

            return list;
        }

        [HttpPost]
        public async Task SetAppConfig(SettingValue[] settings)
        {
            foreach (var setting in settings)
            {
                await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, setting.Name, setting.Value);
            }
        }
    }
}
