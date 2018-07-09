using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LY.WMSCloud.Authorization.Roles;

namespace LY.WMSCloud.Entities
{
    [Table("SysOrg")]
    public class Org : EntitieCommonBase<int>
    {
        [StringLength(30)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int? ParentId { get; set; }

        public virtual Org Parent { get; set; }

        public virtual ICollection<Org> Children { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
