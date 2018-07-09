using AutoMapper;
using LY.WMSCloud.Entities.ProduceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.ProduceData.WorkBills.Dto
{
    public class WorkBillMapProfile : Profile
    {
        public WorkBillMapProfile()
        {
            CreateMap<WorkBill, WorkBillDto>();

            CreateMap<WorkBillDto, WorkBill>();
        }
    }
}
