using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Customized.Foxlink
{
    public class FoxLinkKanBanDto
    {
        /// <summary>
        /// 工单顺序
        /// </summary>
        public int WorkIndex { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        public string WorkOrder { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// 站位
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 机种料号
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 机种名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 周期时间
        /// </summary>
        public int CycleTime { get; set; }
        /// <summary>
        /// 时段产能
        /// </summary>
        public int UPH { get; set; }
        /// <summary>
        /// 机器状态
        /// </summary>
        public string MachineState { get; set; }
        /// <summary>
        /// 工单数量
        /// </summary>
        public int WorkQty { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime PlanStartTime { get; set; }
        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// (预计)完成时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 准备耗时
        /// </summary>
        public TimeSpan SetupTime { get; set; }
        /// <summary>
        /// 已生产数量
        /// </summary>
        public int TodayCompleteQty { get; set; }
        /// <summary>
        /// 总已生产数量
        /// </summary>
        public int CompleteQty { get; set; }
        /// <summary>
        /// 待生产数量
        /// </summary>
        public int UnCompleteQty { get; set; }
        /// <summary>
        /// 已生产时间
        /// </summary>
        public TimeSpan ConsumeTime { get; set; }
        /// <summary>
        /// 今日已生产时间
        /// </summary>
        public TimeSpan TodayConsumeTime { get; set; }
        /// <summary>
        /// 待生产时间
        /// </summary>
        public TimeSpan UnConsumeTime { get; set; }
        /// <summary>
        /// 单位时间完成率
        /// </summary>
        public double Oee { get; set; }
        /// <summary>
        /// 达成率
        /// </summary>
        public double ReachRate { get; set; }
        /// <summary>
        /// 直通率
        /// </summary>
        public double FPY { get; set; }
        /// <summary>
        /// 不良数
        /// </summary>
        public int FaileQty { get; set; }
        /// <summary>
        /// 不良说明
        /// </summary>
        public string FaileDesc { get; set; }
        /// <summary>
        /// 良率
        /// </summary>
        public double Yield { get; set; }
        /// <summary>
        /// 换线时间
        /// </summary>
        public TimeSpan LineChangingTime { get; set; }
        /// <summary>
        /// 重点工站
        /// </summary>
        public ImportantProcess[] ImportantProcess { get; set; }
    }

    public class FoxLinkKanBanQueryDto
    {
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime PlanStartTime { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        public string WorkOrder { get; set; }
        /// <summary>
        /// 站位
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 不良数量
        /// </summary>
        public int FaileQty { get; set; }
        /// <summary>
        /// 不良说明
        /// </summary>
        public string FaileDesc { get; set; }
        /// <summary>
        /// 机器状态
        /// </summary>

        public string MachineState { get; set; }
        /// <summary>
        /// 重点工站
        /// </summary>
        public ImportantProcess[] ImportantProcess { get; set; }
    }

    
}
