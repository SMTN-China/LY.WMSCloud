using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.Customized.Foxlink
{
    public interface IFoxlinkKanBanAppService
    {
        Task<FoxLinkKanBanDto> GetData(FoxLinkKanBanQueryDto queryDto);

        /// <summary>
        /// 按开始时间获取工单
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GeWorkOrder(DateTime startTime);

        /// <summary>
        /// 获取工单工站
        /// </summary>
        /// <param name="workOrder"></param>
        /// <returns></returns>
        Task<IEnumerable<ImportantProcess>> GePROCESS_NAME(string workOrder);

        Task<ICollection<string>> GetFaileDescByKeyName(string keyName);
    }
}
