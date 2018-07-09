using LY.WMSCloud.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.CommonService
{
    public class LightService
    {
        HttpHelp HttpHelp { get; set; }

        public LightService(HttpHelp httpHelp)
        {
            HttpHelp = httpHelp;
        }

        public LightMsg LightOrder(List<StorageLight> storageLight)
        {
            return HttpHelp.Post<LightMsg>("/api/Light/LightOrder", storageLight);
        }

        public LightMsg HouseOrder(List<HouseLight> houseLights)
        {
            return HttpHelp.Post<LightMsg>("/api/Light/HouseOrder", houseLights);
        }

        public LightMsg AllLightOrder(List<AllLight> allLightOrders)
        {
            return HttpHelp.Post<LightMsg>("/api/Light/AllLightOrder", allLightOrders);
        }
    }
}
