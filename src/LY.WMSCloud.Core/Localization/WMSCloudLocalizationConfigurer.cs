using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace LY.WMSCloud.Localization
{
    public static class WMSCloudLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(WMSCloudConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(WMSCloudLocalizationConfigurer).GetAssembly(),
                        "LY.WMSCloud.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
