using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Collections;

namespace Vevisoft.Excel.Core
{
    public class ExcelHelper
    {
        private void InitializeWorkbook(HSSFWorkbook hssfworkbook, string headerText)
        {
            hssfworkbook = new HSSFWorkbook();

            //创建一个文档摘要信息实体。
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Weilog Team"; //公司名称
            hssfworkbook.DocumentSummaryInformation = dsi;

            //创建一个摘要信息实体。
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Weilog 系统生成";
            si.Author = "Weilog 系统";
            si.Title = headerText;
            si.Subject = headerText;
            si.CreateDateTime = DateTime.Now;
            hssfworkbook.SummaryInformation = si;

        }

        private static MemoryStream WriteToStream(HSSFWorkbook hssfworkbook)
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }

        //Export(DataTable table, string headerText, string sheetName, string[] columnName, string[] columnTitle)
        /// <summary>
        /// 向客户端输出文件。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <param name="headerText">头部文本。</param>
        /// <param name="sheetName"></param>
        /// <param name="columnName">数据列名称。</param>
        /// <param name="columnTitle">表标题。</param>
        /// <param name="fileName">文件名称。</param>
        public static void Write(DataTable table, string headerText, string sheetName, string[] columnName,
                                 string[] columnTitle, string fileName)
        {
            HttpContext context = HttpContext.Current;
            context.Response.ContentType = "application/vnd.ms-excel";
            context.Response.AddHeader("Content-Disposition",
                                       string.Format("attachment;filename={0}",
                                                     HttpUtility.UrlEncode(fileName, Encoding.UTF8)));
            context.Response.Clear();
            HSSFWorkbook hssfworkbook = GenerateData(table, headerText, sheetName, columnName, columnTitle);
            context.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
            context.Response.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="headerText"></param>
        /// <param name="sheetName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnTitle"></param>
        /// <returns></returns>
        public static HSSFWorkbook GenerateData(DataTable table, string headerText, string sheetName,
                                                string[] columnName, string[] columnTitle)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet(sheetName);

            #region 设置文件属性信息

            //创建一个文档摘要信息实体。
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Weilog Team"; //公司名称
            hssfworkbook.DocumentSummaryInformation = dsi;

            //创建一个摘要信息实体。
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "本文档由 Weilog 系统生成";
            si.Author = " Weilog 系统";
            si.Title = headerText;
            si.Subject = headerText;
            si.CreateDateTime = DateTime.Now;
            hssfworkbook.SummaryInformation = si;

            #endregion

            ICellStyle dateStyle = hssfworkbook.CreateCellStyle();
            IDataFormat format = hssfworkbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            #region 取得列宽

            int[] colWidth = new int[columnName.Length];
            for (int i = 0; i < columnName.Length; i++)
            {
                colWidth[i] = Encoding.GetEncoding(936).GetBytes(columnTitle[i]).Length;
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < columnName.Length; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(table.Rows[i][columnName[j]].ToString()).Length;
                    if (intTemp > colWidth[j])
                    {
                        colWidth[j] = intTemp;
                    }
                }
            }

            #endregion

            int rowIndex = 0;
            foreach (DataRow row in table.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = hssfworkbook.CreateSheet(sheetName + ((int) rowIndex/65535).ToString());
                    }

                    #region 表头及样式

                    //if (!string.IsNullOrEmpty(headerText))
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(headerText);

                        ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = hssfworkbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;
                        //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1)); 
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, table.Columns.Count - 1));
                    }

                    #endregion

                    #region 列头及样式

                    {
                        //HSSFRow headerRow = sheet.CreateRow(1); 
                        IRow headerRow;
                        //if (!string.IsNullOrEmpty(headerText))
                        //{
                        //    headerRow = sheet.CreateRow(0);
                        //}
                        //else
                        //{
                        headerRow = sheet.CreateRow(1);
                        //}
                        ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = hssfworkbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        for (int i = 0; i < columnName.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(columnTitle[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            //设置列宽 
                            if ((colWidth[i] + 1)*256 > 30000)
                            {
                                sheet.SetColumnWidth(i, 10000);
                            }
                            else
                            {
                                sheet.SetColumnWidth(i, (colWidth[i] + 1)*256);
                            }
                        }
                        /* 
                        foreach (DataColumn column in dtSource.Columns) 
                        { 
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName); 
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle; 
   
                            //设置列宽    
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256); 
                        } 
                         * */
                    }

                    #endregion

                    //if (!string.IsNullOrEmpty(headerText))
                    //{
                    //    rowIndex = 1;
                    //}
                    //else
                    //{
                    rowIndex = 2;
                    //}

                }

                #endregion

                #region 填充数据

                IRow dataRow = sheet.CreateRow(rowIndex);
                for (int i = 0; i < columnName.Length; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);

                    string drValue = row[columnName[i]].ToString();

                    switch (table.Columns[columnName[i]].DataType.ToString())
                    {
                        case "System.String": //字符串类型   
                            if (drValue.ToUpper() == "TRUE")
                                newCell.SetCellValue("是");
                            else if (drValue.ToUpper() == "FALSE")
                                newCell.SetCellValue("否");
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime": //日期类型    
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示    
                            break;
                        case "System.Boolean": //布尔型    
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            if (boolV)
                                newCell.SetCellValue("是");
                            else
                                newCell.SetCellValue("否");
                            break;
                        case "System.Int16": //整型    
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型    
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理    
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }

                #endregion

                rowIndex++;
            }

            return hssfworkbook;
        }
        #region 将指定的集合转换成数据表...

        /// <summary>
        /// 将指定的集合转换成DataTable。
        /// </summary>
        /// <param name="list">将指定的集合。</param>
        /// <returns>返回转换后的DataTable。</returns>
        //public static DataTable ListToDataTable(IList list)
        //{
        //    DataTable table = new DataTable();
        //    if (list.Count > 0)
        //    {
        //        PropertyInfo[] propertys = list[0].GetType().GetProperties();
        //        foreach (PropertyInfo pi in propertys)
        //        {
        //            Type pt = pi.PropertyType;
        //            if ((pt.IsGenericType) && (pt.GetGenericTypeDefinition() == typeof(Nullable<>)))
        //            {
        //                pt = pt.GetGenericArguments()[0];
        //            }
        //            table.Columns.Add(new DataColumn(pi.Name, pt));
        //        }

        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            ArrayList tempList = new ArrayList();
        //            foreach (PropertyInfo pi in propertys)
        //            {
        //                object obj = pi.GetValue(list[i], null);
        //                tempList.Add(obj);
        //            }
        //            object[] array = tempList.ToArray();
        //            table.LoadDataRow(array, true);
        //        }
        //    }
        //    return table;
        //}

        #endregion

        #region 导出数据...

        public static void ExportData()
        {
            var table = new DataTable();
            table.Columns.Add("Code");
            table.Columns.Add("Name");
            table.Columns.Add("DeptName");
            for (int i = 0; i < 1000; i++)
            {
                var row = table.NewRow();
                row[0] = i;
                row[1] = i + 1;
                row[2] = "asdffd" + i + 2;
                table.Rows.Add(row);
            }
            string[] strFields = { "Code", "Name", "DeptName"};
            string[] strFieldsName = { "编码", "名称", "所属部门" };
            Write(table, "产品表", "产品表", strFields, strFieldsName, "产品表.xls");
        }

        #endregion
    }
}
