using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.Sys.I18Ns.Dto
{
    [AutoMapTo(typeof(ApplicationLanguageText))]
    public class I18NDto : IEntityDto<long>
    {
        public long Id { get; set; }
        public virtual int? TenantId { get; set; }

        public virtual string LanguageName { get; set; }

        public virtual string Source { get; set; }
        //
        // 摘要:
        //     Localization key
        [Required]
        [StringLength(256)]
        public virtual string Key { get; set; }
        //
        // 摘要:
        //     Localized value
        [Required(AllowEmptyStrings = true)]
        [StringLength(67108864)]
        public virtual string Value { get; set; }
    }
}
