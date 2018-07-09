using Abp.Localization;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Sys.I18Ns.Dto
{
    public class I18NMapProfile: Profile
    {
        public I18NMapProfile()
        {
            CreateMap<ApplicationLanguageText, I18NDto>();

            CreateMap<I18NDto, ApplicationLanguageText>();
        }
    }
}
