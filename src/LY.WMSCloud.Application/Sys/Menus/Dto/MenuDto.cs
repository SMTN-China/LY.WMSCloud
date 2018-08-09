using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using LY.WMSCloud.Entities;

namespace LY.WMSCloud.Sys.Menus.Dto
{
    [AutoMapTo(typeof(Menu))]

    public class MenuDto : BaseDto<int>
    {
        [StringLength(20)]
        public string Text { get; set; }

        [StringLength(100)]
        public string I18n { get; set; }

        public bool Group { get; set; }

        [StringLength(500)]
        public string Link { get; set; }

        [StringLength(500)]
        public string ExternalLink { get; set; }

        [StringLength(10)]
        public string Target { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }

        public int Index { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }
        public string Name { get; set; }
        public string Acl { get; set; }

        public bool Hide { get; set; }

        public ICollection<int> RoleIds { get; set; }
    }
}
