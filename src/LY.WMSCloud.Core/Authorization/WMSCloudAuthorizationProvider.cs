using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace LY.WMSCloud.Authorization
{
    public class WMSCloudAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var fieldInfos = typeof(PermissionNames).GetFields();

            foreach (var fi in fieldInfos)
            {
                if (fi.IsLiteral && !fi.IsInitOnly)
                {
                    var permissionName = fi.GetValue(null).ToString();
                    if (permissionName.Contains("Pages.Tenants"))
                    {
                        context.CreatePermission(permissionName, L(permissionName), multiTenancySides: MultiTenancySides.Host);
                    }
                    else
                    {
                        context.CreatePermission(permissionName, L(permissionName));
                    }
                }
            }


            //context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            //context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            //context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, WMSCloudConsts.LocalizationSourceName);
        }
    }
}
