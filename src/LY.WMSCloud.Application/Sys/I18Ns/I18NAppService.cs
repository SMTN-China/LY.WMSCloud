using Abp.Localization;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LY.WMSCloud.Sys.I18Ns.Dto;
using Abp.Reflection.Extensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LY.WMSCloud.Sys.I18Ns
{
    public class I18NAppService : ServiceBase<ApplicationLanguageText, I18NDto, long>, II18NAppService
    {
        readonly IWMSRepositories<ApplicationLanguageText, long> _repository;
        public I18NAppService(IWMSRepositories<ApplicationLanguageText, long> repository) : base(repository)
        {
            _repository = repository;
        }

        public async override Task<I18NDto> Create(I18NDto input)
        {
            // CheckCreatePermission();
            input.TenantId = AbpSession.TenantId;
            input.Source = LocalizationSourceName;
            var lang = await SettingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, AbpSession.ToUserIdentifier());
            input.LanguageName = lang;
            return await base.Create(input);
        }

        public async Task<ICollection<I18NDto>> GetByDtoName(string dtoName)
        {
            var thisAssembly = typeof(WMSCloudApplicationModule).GetAssembly().ExportedTypes.Where(t => t.Name.ToLower() == dtoName.ToLower()).FirstOrDefault();

            var lang = await SettingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, AbpSession.ToUserIdentifier());

            var dbI18N = await _repository.GetAll().Where(i => i.Key.StartsWith(dtoName) && i.LanguageName == lang).ToListAsync();

            var p = thisAssembly.GetProperties();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<System.Reflection.PropertyInfo, ApplicationLanguageText>()
                    .ForMember(m => m.Key, opt => opt.MapFrom(s => dtoName + s.Name))
                    .ForMember(m => m.Value, opt => opt.MapFrom(s =>
                        dbI18N.FirstOrDefault(i => i.Key == dtoName + s.Name) == null ? s.Name : dbI18N.FirstOrDefault(i => i.Key == dtoName + s.Name).Value))
                    .ForMember(m => m.LanguageName, opt => opt.MapFrom(s => lang));
            }
             );

            var languageText = config.CreateMapper().Map<List<System.Reflection.PropertyInfo>, List<ApplicationLanguageText>>(p.ToList());

            var nDtos = ObjectMapper.Map<List<I18NDto>>(languageText);
            var more = dbI18N.Where(r => !(nDtos.Select(o => o.Key).Contains(r.Key)));
            languageText.AddRange(more);
            return ObjectMapper.Map<List<I18NDto>>(languageText);
        }
    }
}
