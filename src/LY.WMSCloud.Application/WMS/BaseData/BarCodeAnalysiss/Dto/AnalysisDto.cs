using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss.Dto
{
    public class AnalysisDto
    {
        public string BarCode { get; set; }

        public string DtoName { get; set; }


    }

    public class AnalysisResDto
    {
        public bool Success { get; set; }

        public object Result { get; set; }

        public string Msg { get; set; }

    }
}
