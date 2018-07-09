using AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.Lines.Dto
{
    public class LineMapProfile : Profile
    {
        public LineMapProfile()
        {
            CreateMap<Line, LineDto>();

            CreateMap<LineDto, Line>();
        }
    }
}
