using Abp.Reflection.Extensions;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss
{
    public class BarCodeAnalysisAppService : ServiceBase<BarCodeAnalysis, BarCodeAnalysisDto, string>, IBarCodeAnalysisAppService
    {
        readonly IWMSRepositories<BarCodeAnalysis, string> _repository;
        public BarCodeAnalysisAppService(IWMSRepositories<BarCodeAnalysis, string> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> TestAnalysis(TestAnalysisDto testAnalysisDto)
        {
            var res = "";
            try
            {
                if (testAnalysisDto.IsReplace)
                {
                    res = Regex.Replace(testAnalysisDto.Input, testAnalysisDto.Regex, "");
                }
                else
                {
                    res = Regex.Match(testAnalysisDto.Input, testAnalysisDto.Regex).Value;
                }
            }
            catch
            {

            }

            res = await Task.Factory.StartNew(() => res);

            return new List<string>() { res };
        }

        public Task<AnalysisResDto> Analysis(AnalysisDto analysisDto)
        {
            return Task.Factory.StartNew(() =>
            {
                AnalysisResDto res = new AnalysisResDto() { Success = true, Msg = "条码解析成功" };

                var thisAssembly = typeof(WMSCloudApplicationModule).GetAssembly().ExportedTypes.Where(t => t.Name.ToLower() == analysisDto.DtoName.ToLower()).FirstOrDefault();

                if (thisAssembly == null)
                {
                    throw new LYException("未找到要解析的数据传输对象");
                }

                // 查询该对象的解析规则
                var analysiss = _repository.GetAll().Where(a => a.ClassName == analysisDto.DtoName).ToList();
                if (analysiss == null || analysiss.Count < 1)
                {
                    throw new LYException("未设置条码解析规则");
                }

                // 条码解析
                try
                {
                    //获取指定名称的类型
                    object resObj = Activator.CreateInstance(thisAssembly, null);             //创建指定类型实例
                    PropertyInfo[] fields = resObj.GetType().GetProperties();       //获取指定对象的所有公共属性

                    foreach (var pInfo in fields)
                    {
                        // 判断是否存在解析规则

                        var analysis = analysiss.Where(a => a.PropertyName.ToLower() == pInfo.Name.ToLower()).FirstOrDefault();
                        if (analysis == null)
                        {
                            continue;
                        }
                        var oneStr = "";
                        // 进行正则解析
                        if (analysis.IsReplace)
                        {
                            oneStr = Regex.Replace(analysisDto.BarCode, analysis.RegEX, "");
                        }
                        else
                        {
                            oneStr = Regex.Match(analysisDto.BarCode, analysis.RegEX).Value;
                        }
                        if (pInfo.PropertyType.BaseType.Name == "Enum")
                        {
                            pInfo.SetValue(resObj, System.Enum.Parse(pInfo.PropertyType, oneStr, true), null);
                        }
                        else
                        {
                            if (pInfo.PropertyType == typeof(DateTime))
                            {
                                oneStr = GetDateCode(oneStr);
                            }

                            pInfo.SetValue(resObj, Convert.ChangeType(oneStr, pInfo.PropertyType), null);
                        }
                    }
                    res.Result = resObj;
                }
                catch (Exception ex)
                {
                    throw new LYException("条码解析失败" + ex.Message);
                }
                return res;
            });


        }
        string GetDateCode(string oldDC)
        {
            DateTime dt = DateTime.Now;
            try
            {
                switch (oldDC.Length)
                {
                    case 4:
                        string newYear = DateTime.Now.Year.ToString().Substring(0, 2) + oldDC.Substring(0, 2);
                        DayOfWeek dtWeek = new DateTime(int.Parse(newYear), 1, 1).DayOfWeek;
                        if (oldDC.Substring(2, 2) == "01")
                        {
                            dt = new DateTime(int.Parse(newYear), 1, 1);
                        }
                        else
                        {
                            dt = new DateTime(int.Parse(newYear), 1, 1).AddDays(7 * (int.Parse(oldDC.Substring(2, 2)) - 1)).AddDays(DayOfWeek.Sunday - dtWeek);
                        }
                        break;
                    case 6:
                        newYear = DateTime.Now.Year.ToString().Substring(0, 2) + oldDC.Substring(0, 2); ;
                        dt = new DateTime(int.Parse(newYear), int.Parse(oldDC.Substring(2, 2)), int.Parse(oldDC.Substring(4, 2)));
                        break;
                    case 8:
                        dt = new DateTime(int.Parse(oldDC.Substring(0, 4)), int.Parse(oldDC.Substring(4, 2)), int.Parse(oldDC.Substring(6, 2)));
                        break;
                    default:
                        dt = DateTime.Now;
                        break;
                }
            }
            catch
            {
                dt = DateTime.Now;
            }
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
