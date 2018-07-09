using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.BaseData;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto
{
    public class ReadyMBillMapProfile : Profile
    {
        public ReadyMBillMapProfile()
        {
            CreateMap<ReadyMBill, ReadyMBillDto>();

            CreateMap<ReadyMBillDto, ReadyMBill>();

            CreateMap<ReadyMBillWorkBillMap, ReadyMBillWorkBillMapDto>()
                .ForMember(m => m.ProductId, opt => opt.MapFrom(s => s.WorkBill.ProductId))
                .ForMember(m => m.LineId, opt => opt.MapFrom(s => s.WorkBill.LineId));

            CreateMap<ReadyMBillWorkBillMapDto, ReadyMBillWorkBillMap>();

            CreateMap<ReadyMBillDetailedDto, ReadyMBillDetailed>();

            CreateMap<ReadyMBillDetailed, ReadyMBillDetailedDto>();

            //CreateMap<Slot, ReadySlot>();
        }

    }
}
