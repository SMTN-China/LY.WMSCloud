using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Configuration;
using LY.WMSCloud.Configuration.Dto;

namespace LY.WMSCloud.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);

        Task<ICollection<ISettingValue>> GetAppConfig(string[] names);

        Task SetAppConfig(SettingValue[] settings);

        Task<ICollection<ISettingValue>> GetUserConfig(string[] names);

        Task SetUserConfig(SettingValue[] settings);
    }
}
