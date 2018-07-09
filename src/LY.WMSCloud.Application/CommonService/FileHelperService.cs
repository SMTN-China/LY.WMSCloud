using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using Abp.Localization;

namespace LY.WMSCloud.CommonService
{
    public class FileHelperService
    {

        public async Task<List<List<T>>> ExcleToListEntities<T>(List<ApplicationLanguageText> i18ns, string dtoName, FileStream fileStream) where T : class, new()
        {
            List<List<T>> res = new List<List<T>>();
            // 查询 T 的 I18N 列名
            Type entitiesType = typeof(T);

            await Task.Factory.StartNew(() =>
            {
                using (ExcelPackage package = new ExcelPackage(fileStream))
                {
                    foreach (var sheet in package.Workbook.Worksheets)
                    {
                        try
                        {
                            var sheetT = new List<T>();
                            Dictionary<int, ApplicationLanguageText> columnInfos = new Dictionary<int, ApplicationLanguageText>();
                            // 读取表头
                            for (int j = sheet.Dimension.Start.Column, k = sheet.Dimension.End.Column; j <= k; j++)
                            {
                                var columnName = sheet.Cells[sheet.Dimension.Start.Row, j].Value.ToString();
                                // 查询是否包含此列
                                var columnInfo = i18ns.Where(i => i.Value.ToLower() == columnName.ToLower()).FirstOrDefault();
                                if (columnInfo != null)
                                {
                                    columnInfos.Add(j, columnInfo);
                                }

                            }
                            if (columnInfos.Count == 0)
                            {
                                continue;
                            }
                            // 读取数据
                            for (int m = sheet.Dimension.Start.Row + 1, n = sheet.Dimension.End.Row; m <= n; m++)
                            {
                                var rowT = new T();
                                for (int j = sheet.Dimension.Start.Column, k = sheet.Dimension.End.Column; j <= k; j++)
                                {
                                    var columnInfo = columnInfos.FirstOrDefault(c => c.Key == j);

                                    if (columnInfo.Value != null)
                                    {
                                        var cellValue = sheet.Cells[m, j].Value;
                                        Type objType = rowT.GetType();
                                        PropertyInfo info = null;
                                        info = objType.GetProperty(columnInfo.Value.Key.Replace(dtoName, ""));
                                        //判断对象是否有该属性
                                        if (info != null)
                                        {
                                            //为对象属性赋值
                                            //propertie.PropertyType
                                            //等式右边的值    并将右边的数据类型强转为左边的                                     
                                            var propertyType = info.PropertyType;
                                            if (propertyType.BaseType.Name == "Enum")
                                            {
                                                info.SetValue(rowT, Enum.Parse(propertyType, cellValue.ToString(), true), null);
                                            }
                                            else
                                            {
                                                info.SetValue(rowT, Convert.ChangeType(cellValue, propertyType), null);
                                            }
                                        }
                                    }
                                }
                                sheetT.Add(rowT);
                            }

                            res.Add(sheetT);
                        }
                        catch
                        {

                        }
                    }
                }
            });


            return res;
        }


        public DataTable OpenCSV(string filePath)//从csv读取数据返回table
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strToAry(strLine);

                while (dt.Columns.Count < aryLine.Length)
                {
                    dt.Columns.Add(new DataColumn("Column" + dt.Columns.Count));
                }
                dt.Rows.Add(dt.NewRow().ItemArray = aryLine);
            }
            sr.Close();
            fs.Close();
            return dt;
        }

        private string[] strToAry(string strLine)
        {
            string rep = "@" + Guid.NewGuid().ToString() + "@";

            // strLine = strLine.Replace("\"\"", rep);   //替换所有文本引号



            string strItem = "";
            int iFenHao = 0;
            System.Collections.ArrayList lstStr = new System.Collections.ArrayList();
            for (int i = 0; i < strLine.Length; i++)
            {
                string strA = strLine.Substring(i, 1);
                if (strA == "\"")
                {
                    iFenHao = iFenHao + 1;
                }
                if (iFenHao == 2)
                {
                    iFenHao = 0;
                }
                if (strA == "," && iFenHao == 0)
                {
                    lstStr.Add(strItem.Replace("\"", "").Replace(rep, "\""));
                    strItem = "";
                }
                else
                {
                    strItem = strItem + strA;
                }
            }
            if (strItem.Length > 0)
                lstStr.Add(strItem);
            return (String[])lstStr.ToArray(typeof(string));
        }
        System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}
