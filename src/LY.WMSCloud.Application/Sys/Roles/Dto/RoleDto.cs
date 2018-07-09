using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using LY.WMSCloud.Authorization.Roles;

namespace LY.WMSCloud.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RoleDto : EntityDto<int>
    {
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }
        
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        public bool IsStatic { get; set; }

        public string OrgName { get; set; }

        public int? OrgId { get; set; }

        [StringLength(2000)]
        public string Remark { get; set; }

        public int Grade { get; set; }

        public bool IsActive { get; set; }

        public List<string> Permissions { get; set; }
    }
}
