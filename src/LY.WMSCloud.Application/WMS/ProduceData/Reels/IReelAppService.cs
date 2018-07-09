using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities;
using LY.WMSCloud.WMS.ProduceData.Reels.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.Reels
{
    public interface IReelAppService : IServiceBase<ReelDto, string>
    {
        Task<ReelMoveResDto> ReelMove(ReelMoveDto inputDto);

        Task<PagedResultDto<ReelOutLifeDto>> GetOutLifeReel(PagedResultRequestInput input);

        Task UpdateReelESL(UpdateReelESLDto updateReelESL);

        Task<PagedResultDto<GroupReelDto>> GetGroupReel(PagedResultRequestInput input);

        Task BrightByPartNoIds(LightOrderDto[] pns);

        Task BrightByReelIds(LightOrderDto[] reels);

        Task ExtinguishedByPartNoIds(LightOrderDto[] pns);

        Task ExtinguishedByReelIds(LightOrderDto[] reels);

        Task ReturnReel(ReturnReelDto returnReel);

        Task<GetReceivedsResult> GetReceiveds(ReelMoveDto inputDto);

        Task<ReelMoveResDto> GetIsReturnReel(ReelMoveDto inputDto);
    }
}
