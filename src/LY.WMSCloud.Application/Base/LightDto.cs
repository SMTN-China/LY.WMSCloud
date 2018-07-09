using LY.WMSCloud.Entities.StorageData;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Base
{
    /// <summary>
    /// 整个货架控制
    /// </summary>
    public class AllLight
    {
        /// <summary>
        /// 主板Id
        /// </summary>
        public int MainBoardId { get; set; }
        /// <summary>
        /// 灯颜色
        /// </summary>
        public LightColor LightColor { get; set; }
        /// <summary>
        /// 灯指令
        /// </summary>
        public int LightOrder { get; set; }
    }
    /// <summary>
    /// 灯塔控制
    /// </summary>
    public class HouseLight
    {
        /// <summary>
        /// 主板Id
        /// </summary>
        public int MainBoardId { get; set; }
        /// <summary>
        /// 灯颜色
        /// </summary>
        public LightColor LightColor { get; set; }
        /// <summary>
        /// 灯塔边别
        /// </summary>
        public int HouseLightSide { get; set; }
        /// <summary>
        /// 灯指令
        /// </summary>
        public int LightOrder { get; set; }
    }
    public class LightMsg
    {
        public bool IsOK { get; set; }
        public string Msg { get; set; }
    }

    /// <summary>
    /// 库位灯
    /// </summary>
    public class StorageLight
    {
        /// <summary>
        /// 主板Id
        /// </summary>
        public int MainBoardId { get; set; }
        /// <summary>
        /// 库位Id
        /// </summary>
        public int RackPositionId { get; set; }
        /// <summary>
        /// 灯指令
        /// </summary>
        public int LightOrder { get; set; }
        /// <summary>
        /// 灯颜色
        /// </summary>
        public LightColor LightColor { get; set; }
        /// <summary>
        /// 灯指令
        /// </summary>
        public int ContinuedTime { get; set; }
    }

    /// <summary>
    /// 亮灯指令
    /// </summary>
    public enum LightOrder
    {
        /// <summary>
        /// 亮
        /// </summary>
        LightUp,
        /// <summary>
        /// 灭
        /// </summary>
        LightOff,
        /// <summary>
        /// 闪
        /// </summary>
        LightFlicker
    }
}
