using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.ReceivedReelBills.Dto
{
    public class ReceivedReelBillMapProfile : Profile
    {
        public ReceivedReelBillMapProfile()
        {
            CreateMap<ReceivedReelBill, ReceivedReelBillDto>();
            CreateMap<ReceivedReelBillDto, ReceivedReelBill>();
        }
    }
}
