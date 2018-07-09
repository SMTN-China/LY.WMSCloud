using System;
using System.Collections.Generic;
using System.Text;

namespace LY.WMSCloud.WMS.BaseData.StorageLocations.Dto
{
    public class LYDto
    {
        public int[][] mainId { get; set; }

        public int Count { get; set; }

        public string[] ShelfCode { get; set; }

        public string[] ShelfId { get; set; }

        public string StorageId { get; set; }

        public string StorageLocationTypeId { get; set; }
    }
}
