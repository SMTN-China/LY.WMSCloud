using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LY.WMSCloud.Entities
{
    [Table("SysMenu")]
    public class Menu : EntitieCommonBase<int>
    {
        [StringLength(30)]
        public string Text { get; set; }

        [StringLength(50)]
        public string I18n { get; set; }

        public bool Group { get; set; }

        [StringLength(500)]
        public string Link { get; set; }

        [StringLength(500)]
        public string ExternalLink { get; set; }

        [StringLength(10)]
        public string Target { get; set; }

        [StringLength(30)]
        public string Icon { get; set; }

        public int Index { get; set; }

        public int? ParentId { get; set; }

        public virtual Menu Parent { get; set; }
        [StringLength(30)]
        public string Acl { get; set; }

        public virtual ICollection<Menu> Children { get; set; }

    }
}
