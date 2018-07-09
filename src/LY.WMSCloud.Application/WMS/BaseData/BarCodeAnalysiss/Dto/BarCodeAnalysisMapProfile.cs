using AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss.Dto
{
    public class BarCodeAnalysisMapProfile : Profile
    {
        public BarCodeAnalysisMapProfile()
        {
            CreateMap<BarCodeAnalysis, BarCodeAnalysisDto>();

            CreateMap<BarCodeAnalysisDto, BarCodeAnalysis>();
        }
    }
}
