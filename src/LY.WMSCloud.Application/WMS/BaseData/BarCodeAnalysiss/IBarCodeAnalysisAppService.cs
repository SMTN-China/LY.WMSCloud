using LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss
{
    public interface IBarCodeAnalysisAppService : IServiceBase<BarCodeAnalysisDto, string>
    {
        Task<List<string>> TestAnalysis(TestAnalysisDto testAnalysisDto);


        Task<AnalysisResDto> Analysis(AnalysisDto analysisDto);
    }
}
