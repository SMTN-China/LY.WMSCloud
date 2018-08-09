using Abp.Configuration;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using LY.WMSCloud.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.CommonService
{
    public class LightService
    {
        HttpHelp HttpHelp { get; set; }
        ISettingManager Setting { get; set; }
        IAbpSession AbpSession { get; set; }
        ILogger Logger;
        public LightService(HttpHelp httpHelp, ILogger logger, ISettingManager setting, IAbpSession abpSession)
        {
            HttpHelp = httpHelp;
            Logger = logger;
            Setting = setting;
            AbpSession = abpSession;
        }

        public void LightOrder(List<StorageLight> storageLight)
        {
            try
            {
                if (Setting.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.GetTenantId()) == 1)
                {
                    storageLight.ForEach(r =>
                    {
                        if (r.LightColor == Entities.StorageData.LightColor.Default)
                        {
                            r.LightColor = Entities.StorageData.LightColor.Green;
                        }
                    });
                }
                HttpHelp.Post<LightMsg>("/api/Light/LightOrder", storageLight);
            }
            catch (Exception wx)
            {
                Logger.Error("灯控制失败", wx);
            }

        }

        public void HouseOrder(List<HouseLight> houseLights)
        {
            try
            {
                if (Setting.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.GetTenantId()) == 1)
                {
                    houseLights.ForEach(r =>
                    {
                        if (r.LightColor == Entities.StorageData.LightColor.Default)
                        {
                            r.LightColor = Entities.StorageData.LightColor.Green;
                        }
                    });
                }
                HttpHelp.Post<LightMsg>("/api/Light/HouseOrder", houseLights);
            }
            catch (Exception wx)
            {
                Logger.Error("灯控制失败", wx);
            }

        }

        public void AllLightOrder(List<AllLight> allLightOrders)
        {
            try
            {
                if (Setting.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.GetTenantId()) == 1)
                {
                    allLightOrders.ForEach(r =>
                    {
                        if (r.LightColor == Entities.StorageData.LightColor.Default)
                        {
                            r.LightColor = Entities.StorageData.LightColor.Green;
                        }
                    });
                }
                HttpHelp.Post<LightMsg>("/api/Light/AllLightOrder", allLightOrders);
            }
            catch (Exception wx)
            {
                Logger.Error("灯控制失败", wx);
            }
        }
    }
}
