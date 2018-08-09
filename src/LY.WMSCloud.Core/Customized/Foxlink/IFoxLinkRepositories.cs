using LY.WMSCloud.Customized.Foxlink;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.Core.Customized.Foxlink
{
    /// <summary>
    /// 富港看板仓储接口
    /// </summary>
    public interface IFoxLinkRepositories
    {


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
        Task<IEnumerable<string>> GePROCESS_NAME(string workOrder);

        Task<WMSCloud.Customized.Foxlink.G_WO_BASE_GE> GetG_WO_BASE_GE(string workOrder);

        /// <summary>
        /// 获取工单指定工站过站明细
        /// </summary>
        /// <param name="workOrder"></param>
        /// <returns></returns>
        Task<IEnumerable<WMSCloud.Customized.Foxlink.G_SN_TRAVEL_GE>> GetG_SN_TRAVEL_GEs(string workOrder, string processName);
        /// <summary>
        /// 获取工单顺序
        /// </summary>
        /// <param name="workOrder"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        Task<int> GetWorkIndex(string workOrder, DateTime startTime);

        /// <summary>
        /// 获取上一个工单最好出站时间
        /// </summary>
        /// <param name="startWorkTime"></param>
        /// <param name="lineName"></param>
        /// <returns></returns>
        Task<WMSCloud.Customized.Foxlink.G_SN_TRAVEL_GE> GetPreviousWorkLastData(DateTime startWorkTime, string lineName);

        /// <summary>
        /// 获取重点工站数据
        /// </summary>
        /// <param name="processes"></param>
        /// <param name="workOrder"></param>
        /// <param name="startWorkTime"></param>
        /// <returns></returns>
        Task<IEnumerable<ImportantProcess>> GetImportantProcesses(IEnumerable<ImportantProcess> processes, string workOrder, DateTime startWorkTime);

    }
}
