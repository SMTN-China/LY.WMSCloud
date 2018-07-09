using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.MultiTenancy;

namespace LY.WMSCloud
{
    public class EntitieCommonBase<TPrimaryKey> : EntitieBase<TPrimaryKey>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
    }

    public class EntitieCommonBase : EntitieCommonBase<string>
    {

    }

    public class EntitieTenantBase<TPrimaryKey> : EntitieBase<TPrimaryKey>, IMustHaveTenant
    {
        public int TenantId { get; set; }
    }

    public class EntitieTenantBase : EntitieTenantBase<string>
    {

    }

    public class EntitieBase<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IExtendableObject, IPassivable
    {
        public EntitieBase()
        {
            IsActive = true;
        }
        public string ExtensionData { get; set; }
        public bool IsActive { get; set; }
    }

    public class EntitieBase : EntitieBase<string>
    {

    }
}
