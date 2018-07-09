using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Roles.Dto;

namespace LY.WMSCloud.Sys.Orgs.Dto
{
    [AutoMapTo(typeof(Org))]
    public class OrgDto : BaseDto<int>
    {
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(2000)]
        public string Remark { get; set; }

        public string ParentName { get; set; }


        public int? ParentId { get; set; }

        public OrgDto Parent { get; set; }

        public ICollection<RoleDto> Roles { get; set; }
    }
}
