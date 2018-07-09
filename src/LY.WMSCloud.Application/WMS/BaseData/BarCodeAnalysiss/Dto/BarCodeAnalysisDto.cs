using Abp.AutoMapper;
using LY.WMSCloud.Entities.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss.Dto
{
    [AutoMapFrom(typeof(BarCodeAnalysis))]
    public class BarCodeAnalysisDto : BaseDto
    {
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(30)]
        public string ClassName { get; set; }
        [StringLength(30)]
        public string PropertyName { get; set; }

        [StringLength(2000)]
        public string RegEX { get; set; }

        public bool IsReplace { get; set; }
        [StringLength(1000)]
        public string Test { get; set; }
        [StringLength(500)]
        public string TestValue { get; set; }
    }
}
