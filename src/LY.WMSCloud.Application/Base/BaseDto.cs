using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using LY.WMSCloud.Authorization.Users;

namespace LY.WMSCloud
{
    public class BaseDto<TPrimaryKey> : EntityDto<TPrimaryKey>, IAudited, IExtendableObject, IPassivable
    {
        public long? CreatorUserId { get; set; }
        public string CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public string LastModifierUser { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string ExtensionData { get; set; }
        public bool IsActive { get; set; }
        public int? TenantId { get; set; }
    }

    public class BaseDto : BaseDto<string>
    {

    }
}
