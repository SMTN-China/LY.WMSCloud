using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LY.WMSCloud.Roles.Dto;
using LY.WMSCloud.Sys.Orgs.Dto;

namespace LY.WMSCloud.Sys
{
    public interface IOrgAppService : IServiceBase<OrgDto, int>
    {
        Task<ICollection<OrgDto>> GetChild(int id = -1);

        Task<ICollection<RoleDto>> GetRole(int id);
    }
}
