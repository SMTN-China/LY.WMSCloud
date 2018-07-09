using Abp.Authorization;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;

namespace LY.WMSCloud.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
