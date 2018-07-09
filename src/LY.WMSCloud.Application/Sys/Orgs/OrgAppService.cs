using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Roles.Dto;
using LY.WMSCloud.Sys.Orgs.Dto;

namespace LY.WMSCloud.Sys
{
    public class OrgAppService : ServiceBase<Org, OrgDto, int>, IOrgAppService
    {
        public RoleManager RoleManager { get; set; }

        public IWMSRepositories<Role> RoleRepositories { get; set; }

        readonly IWMSRepositories<Org, int> _repository;

        public OrgAppService(IWMSRepositories<Org, int> repository) : base(repository)
        {
            _repository = repository;
        }

        public override Task<OrgDto> Create(OrgDto input)
        {
            input.TenantId = AbpSession.TenantId;
            return base.Create(input);
        }

        public override Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            // 删除组织下属角色
            RoleRepositories.Delete(r => r.OrgId == input.Id);

            return base.Delete(input);
        }

        public async Task<ICollection<OrgDto>> GetChild(int id = -1)
        {
            List<Org> res;
            if (id == -1)
            {
                res = await _repository.GetAllListAsync(o => o.ParentId == null);
            }
            else
            {
                res = await _repository.GetAllListAsync(o => o.ParentId == id);
            }


            return ObjectMapper.Map<List<OrgDto>>(res);
        }

        public async Task<ICollection<RoleDto>> GetRole(int id)
        {
            var org = await _repository.GetAllIncluding(o => o.Roles).FirstOrDefaultAsync(r => r.Id == id);

            return ObjectMapper.Map<List<RoleDto>>(org.Roles);
        }
    }
}
