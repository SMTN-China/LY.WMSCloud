using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.Customized.Foxlink
{
    public class ImportantProcess
    {
        public string Name { get; set; }

        public string ProsessState { get; set; }

        public bool IsImportant { get; set; }

        public int TodayCompleteQty { get; set; }
    }
}
