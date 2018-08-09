using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using LY.WMSCloud.Core.Customized.Foxlink;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace LY.WMSCloud.Customized.Foxlink
{


    public class FoxlinkKanBanAppService : WMSCloudAppServiceBase, IFoxlinkKanBanAppService
    {
        IFoxLinkRepositories FoxLinkRepositories { get; }
        IWMSRepositories<Entities.ProduceData.UPH, string> UPHRepositories { get; }
        IWMSRepositories<FaileDesc, String> FDRepositories { get; }

        public FoxlinkKanBanAppService
            (
            IFoxLinkRepositories foxLinkRepositories,
            IWMSRepositories<Entities.ProduceData.UPH, string> uPHRepositories,
            IWMSRepositories<FaileDesc, String> fdRepositories
            )
        {
            FoxLinkRepositories = foxLinkRepositories;
            UPHRepositories = uPHRepositories;
            FDRepositories = fdRepositories;
        }

        [HttpPost]
        public async Task<FoxLinkKanBanDto> GetData(FoxLinkKanBanQueryDto queryDto)
        {
            // 查询工单主表
            var workOrderInfo = await FoxLinkRepositories.GetG_WO_BASE_GE(queryDto.WorkOrder);

            // 查询工单所有过站记录
            var g_SN_TRAVEL_GE = await FoxLinkRepositories.GetG_SN_TRAVEL_GEs(queryDto.WorkOrder, queryDto.ProcessName);

            // 第一条大于开始时间的数据
            var g_SN_TRAVEL_GEFirstTime = g_SN_TRAVEL_GE.Where(r => r.CURRENT_STATUS == "0" && r.IN_PDLINE_TIME >= queryDto.PlanStartTime).OrderBy(r => r.OUT_PROCESS_TIME).FirstOrDefault();

            // 第一条数据
            var g_SN_TRAVEL_GEFirst = g_SN_TRAVEL_GE.Where(r => r.CURRENT_STATUS == "0").OrderBy(r => r.OUT_PROCESS_TIME).FirstOrDefault();

            // 最后一条数据
            var g_SN_TRAVEL_GEEnd = g_SN_TRAVEL_GE.Where(r => r.CURRENT_STATUS == "0").OrderByDescending(r => r.OUT_PROCESS_TIME).FirstOrDefault();

            // UPH查询
            var uph = await UPHRepositories.FirstOrDefaultAsync(r => r.ProductId == workOrderInfo.PART_NO && r.LineId == g_SN_TRAVEL_GEFirst.PDLINE_NAME);

            if (uph == null)
            {
                throw new LYException(string.Format("未维护UPH信息，机种[{0}] 线别[{1}]", workOrderInfo.PART_NO, g_SN_TRAVEL_GEFirst.PDLINE_NAME));
            }

            // 获取上一个工单
            var previousWo = await FoxLinkRepositories.GetPreviousWorkLastData(g_SN_TRAVEL_GEFirst.IN_PDLINE_TIME, g_SN_TRAVEL_GEFirst.PDLINE_NAME);

            // 获取直通数量
            var fpq = g_SN_TRAVEL_GE.Where(r => r.IN_PDLINE_TIME >= queryDto.PlanStartTime).GroupBy(r => r.SERIAL_NUMBER).Select(r => r.OrderBy(s => s.IN_PDLINE_TIME).FirstOrDefault()).Select(r => r.CURRENT_STATUS == "0").Count();

            var todayCompleteQty = g_SN_TRAVEL_GE.Where(r => r.CURRENT_STATUS == "0" && r.IN_PDLINE_TIME >= queryDto.PlanStartTime).Select(r => r.SERIAL_NUMBER).Distinct().Count();

            var todayCount = g_SN_TRAVEL_GE.Where(r => r.IN_PDLINE_TIME >= queryDto.PlanStartTime).Select(r => r.SERIAL_NUMBER).Distinct().Count();

            var startTime = g_SN_TRAVEL_GEFirstTime.IN_PDLINE_TIME;

            var hours = ((g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME - startTime).Hours + ((g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME - startTime).Minutes * 1.0 / 60));

            var oee = (todayCount / ((g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME - startTime).Hours + ((g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME - startTime).Minutes * 1.0 / 60))) / (uph.Qty / uph.Meter);

            var completeQty = g_SN_TRAVEL_GE.Where(r => r.CURRENT_STATUS == "0").Select(r => r.SERIAL_NUMBER).Distinct().Count();

            FoxLinkKanBanDto kanBanDto = new FoxLinkKanBanDto()
            {
                WorkOrder = queryDto.WorkOrder,
                ProcessName = queryDto.ProcessName,
                FaileDesc = queryDto.FaileDesc,
                FaileQty = queryDto.FaileQty,
                MachineState = queryDto.MachineState,
                PlanStartTime = queryDto.PlanStartTime,
                Product = workOrderInfo.PART_NO,
                ProductName = g_SN_TRAVEL_GEFirst.MODEL_NAME,
                Line = g_SN_TRAVEL_GEFirst.PDLINE_NAME,
                CompleteQty = completeQty,
                StartTime = startTime,
                CycleTime = uph.Meter,
                UPH = uph.Qty / uph.Meter,
                WorkIndex = await FoxLinkRepositories.GetWorkIndex(queryDto.WorkOrder, queryDto.PlanStartTime),
                WorkQty = workOrderInfo.TARGET_QTY,
                TodayConsumeTime = g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME - g_SN_TRAVEL_GEFirstTime.IN_PDLINE_TIME,
                ConsumeTime = g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME - g_SN_TRAVEL_GEFirst.IN_PDLINE_TIME,
                LineChangingTime = g_SN_TRAVEL_GEFirstTime.IN_PDLINE_TIME - (previousWo == null ? g_SN_TRAVEL_GEFirstTime.IN_PDLINE_TIME : previousWo.OUT_PROCESS_TIME),
                SetupTime = g_SN_TRAVEL_GEFirstTime.IN_PDLINE_TIME - queryDto.PlanStartTime,
                TodayCompleteQty = todayCompleteQty,
                UnCompleteQty = workOrderInfo.TARGET_QTY - completeQty,
                UnConsumeTime = new TimeSpan((long)((workOrderInfo.TARGET_QTY - workOrderInfo.OUTPUT_QTY) * 1.0 / (uph.Qty / uph.Meter) * 60 * 60 * 1000)),
                ReachRate = Math.Round(completeQty * 100.0 / workOrderInfo.TARGET_QTY, 3),
                FPY = Math.Round(fpq * 100.0 / todayCount, 3),
                Yield = Math.Round((todayCompleteQty - queryDto.FaileQty) * 100.0 / todayCompleteQty, 3),
                Oee = Math.Round(oee * 100.0, 3),
                EndTime = g_SN_TRAVEL_GEEnd.OUT_PROCESS_TIME.AddMilliseconds((workOrderInfo.TARGET_QTY - workOrderInfo.OUTPUT_QTY) * 1.0 / (uph.Qty / uph.Meter)),
                ImportantProcess = (await FoxLinkRepositories.GetImportantProcesses(queryDto.ImportantProcess.Where(r => r.IsImportant), queryDto.WorkOrder, queryDto.PlanStartTime)).ToArray()
            };

            var faileDesc = await FDRepositories.FirstOrDefaultAsync(queryDto.WorkOrder);
            if (faileDesc == null)
            {
                await FDRepositories.InsertAsync(new FaileDesc() { Id = queryDto.WorkOrder, Text = queryDto.FaileDesc, TenantId = AbpSession.TenantId.Value, IsActive = true });
            }
            else
            {
                faileDesc.Text = queryDto.FaileDesc;
                await FDRepositories.UpdateAsync(faileDesc);
            }

            return kanBanDto;
        }
        [HttpPost]
        public async Task<IEnumerable<string>> GeWorkOrder(DateTime startTime)
        {
            return await FoxLinkRepositories.GeWorkOrder(startTime);
        }
        [HttpPost]
        public async Task<IEnumerable<ImportantProcess>> GePROCESS_NAME(string workOrder)
        {
            return (await FoxLinkRepositories.GePROCESS_NAME(workOrder)).Select(r => new ImportantProcess() { Name = r });
        }
        [HttpPost]
        public async Task<ICollection<string>> GetFaileDescByKeyName(string keyName)
        {
            if (keyName == null)
            {
                keyName = "";
            }
            var res = await FDRepositories.GetAll().Where(w => w.Text.Contains(keyName) && w.IsActive).Take(10).Select(r => r.Text).ToListAsync();

            return res;
        }
    }
}
