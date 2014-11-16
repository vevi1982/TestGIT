using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Vevisoft.Excel.Core
{
    /// <summary>
    /// 导出数据到Excel文件
    /// </summary>
    public class ExportExcelCore
    {
        #region 不带模板的导出

        /// <summary>
        /// 导出DataTable到Excel文件
        /// </summary>
        /// <param name="filepath">导出文件路径</param>
        /// <param name="sourceTable"></param>
        /// <param name="title"></param>
        /// <param name="columnsNameList">Datatable列名，数据列名</param>
        public void RenderDataTableToExcel(string filepath, DataTable sourceTable,string title,
                                           Dictionary<string, string> columnsNameList)
        {
            try
            {
                var originalTime = DateTime.Now;
                System.Console.WriteLine("Start time: " + originalTime);

                var workbook = new XSSFWorkbook();
                //日期格式
                ICellStyle dateStyle = workbook.CreateCellStyle();
                IDataFormat format = workbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                //
                ISheet sheet = workbook.CreateSheet();
                int rowIndex = 0;
                //标题
                #region 如果有标题，设置
                #region 表头及样式
                if (!string.IsNullOrEmpty(title))
                {
                    IRow titleRow = sheet.CreateRow(rowIndex);
                    titleRow.HeightInPoints = 25;
                    titleRow.CreateCell(0).SetCellValue(title);

                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    titleStyle.Alignment = HorizontalAlignment.Center;
                    IFont titleFont = workbook.CreateFont();
                    titleFont.FontHeightInPoints = 20;
                    titleFont.Boldweight = 700;
                    titleStyle.SetFont(titleFont);
                    titleStyle.BorderBottom = titleStyle.BorderLeft = titleStyle.BorderRight = titleStyle.BorderTop = BorderStyle.Thin;
                    //
                    titleRow.GetCell(0).CellStyle = titleStyle;
                    //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1)); 
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, sourceTable.Columns.Count - 1));
                    //
                    rowIndex++;
                }
                #endregion
                #endregion
                //列头
                IRow headerRow = sheet.CreateRow(rowIndex);
                //列头样式设置
                ICellStyle headStyle = workbook.CreateCellStyle();
                headStyle.Alignment = HorizontalAlignment.Center;
                IFont font = workbook.CreateFont();
                font.FontHeightInPoints = 10;
                font.Boldweight = 700;
                headStyle.BorderBottom = headStyle.BorderLeft = headStyle.BorderRight = headStyle.BorderTop = BorderStyle.Thin;
                headStyle.SetFont(font);
                //
                foreach (DataColumn column in sourceTable.Columns)
                {
                    var colName = column.ColumnName;
                    if (columnsNameList != null && columnsNameList.ContainsKey(column.ColumnName))
                        colName = columnsNameList[column.ColumnName];
                    headerRow.CreateCell(column.Ordinal).SetCellValue(colName);
                    headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                    //列宽设置
                    var colWidth = Encoding.GetEncoding(936).GetBytes(colName).Length;
                    colWidth++;
                    colWidth = Math.Max(12, colWidth);
                    if (colWidth  * 256 > 30000)
                        sheet.SetColumnWidth(column.Ordinal, 10000);
                    else
                        sheet.SetColumnWidth(column.Ordinal, colWidth  * 256);
                }
                rowIndex++;
                //rowIndex = 1;
                //普通Cell的样式
                ICellStyle style = sheet.Workbook.CreateCellStyle();
                style.BorderBottom = style.BorderLeft = style.BorderRight = style.BorderTop = BorderStyle.Thin;
                foreach (DataRow row in sourceTable.Rows)
                {
                    WriteDataRowToExcelSheet(sheet, row, rowIndex, style, dateStyle, 0);
                    ++rowIndex;
                    if (rowIndex%10000 == 0)
                    {
                        System.Console.WriteLine("[" + (DateTime.Now - originalTime) + "]" + " " + rowIndex +
                                                 " rows written");
                    }
                }
                //
                System.Console.WriteLine("[" + (DateTime.Now - originalTime) + "]" + " "+rowIndex+" Written over");
                //列宽自适应，只对英文和数字有效.此操作太耗时间了
                //for (int i = 0; i <= sourceTable.Columns.Count; ++i)
                //    sheet.AutoSizeColumn(i);
                System.Console.WriteLine("[" + (DateTime.Now - originalTime) + "]" + "FileStream WriteStart");
                using (var sw = File.Create(filepath))
                    workbook.Write(sw);
                //
                System.Console.WriteLine("[" + (DateTime.Now - originalTime) + "]" + "FileStream WriteEnd");

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            //打开Excel文件
            if (File.Exists(filepath))
                System.Diagnostics.Process.Start(filepath);
        }

        /// <summary>
        /// 将数据行写入到Excel sheet中
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellstyle"></param>
        /// <param name="dateStyle"></param>
        /// <param name="startColumn"></param>
        private void WriteDataRowToExcelSheet(ISheet sheet, DataRow row, int rowIndex,
            ICellStyle cellstyle, ICellStyle dateStyle,int startColumn)
        {
            //
            var dataRow =  sheet.CreateRow(rowIndex);
            foreach (DataColumn column in row.Table.Columns)
            {
                string drValue = row[column].ToString();
                var colNum = column.Ordinal;
                if (startColumn > 1)
                    colNum += startColumn - 1;
                ICell newCell = dataRow.CreateCell(colNum);
                newCell.CellStyle = cellstyle;
                switch (column.DataType.ToString())
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
                        //
                        //日期格式
                        if (dateStyle == null)
                        {
                            dateStyle = sheet.Workbook.CreateCellStyle();
                            IDataFormat format = sheet.Workbook.CreateDataFormat();
                            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                        }
                        //
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
        }

        
        #endregion

        #region 带模板的导出

        /// <summary>
        /// 如果模板文件是Excel的模板文件，那么导出来的全部是2003格式。
        /// 但是我们可以用xls或者xlsx文件作为模板，因为都是读了一个文件而已
        /// 这样就可以生成2007以上版本的Excel了
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="templatePath"></param>
        /// <param name="sourceTable"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        public void RenderDataTableToExcelHasTemplate(string filePath, string templatePath, DataTable sourceTable,
                                                      int startRow, int startColumn)
        {
            using (var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(file);
                var sheet = workbook.GetSheetAt(0);
                try
                {
                    //日期格式
                    var dateStyle = workbook.CreateCellStyle();
                    var format = workbook.CreateDataFormat();
                    dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                    //
                    var rowIndex = startRow - 1;
                    //普通Cell的样式
                    var style = sheet.Workbook.CreateCellStyle();
                    style.BorderBottom = BorderStyle.Thin;
                    style.BorderLeft = BorderStyle.Thin;
                    style.BorderRight = BorderStyle.Thin;
                    style.BorderTop = BorderStyle.Thin;
                    foreach (DataRow row in sourceTable.Rows)
                    {
                        WriteDataRowToExcelSheet(sheet, row, rowIndex, style, dateStyle, startColumn - 1);
                        ++rowIndex;
                    }
                    sheet.ForceFormulaRecalculation = true;
                    using (var filess = File.Create(filePath))
                    {
                        workbook.Write(filess);
                    }
                }
                catch (Exception e1)
                {
                    throw e1;
                }
            }


            //
            if (File.Exists(filePath))
                System.Diagnostics.Process.Start(filePath);
        }

        #endregion

        #region 格式设置

        // <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet">要合并单元格所在的sheet</param>
        /// <param name="rowstart">开始行的索引</param>
        /// <param name="rowend">结束行的索引</param>
        /// <param name="colstart">开始列的索引</param>
        /// <param name="colend">结束列的索引</param>
        public static void SetCellRangeAddress(ISheet sheet, int rowstart, int rowend, int colstart, int colend)
        {
            var cellRangeAddress = new CellRangeAddress(rowstart, rowend, colstart, colend);
            sheet.AddMergedRegion(cellRangeAddress);
        }

        /// <summary>
        /// 获取单元格样式
        /// </summary>
        /// <param name="hssfworkbook">Excel操作类</param>
        /// <param name="font">单元格字体</param>
        /// <param name="fillForegroundColor">图案的颜色</param>
        /// <param name="fillPattern">图案样式</param>
        /// <param name="fillBackgroundColor">单元格背景</param>
        /// <param name="ha">垂直对齐方式</param>
        /// <param name="va">垂直对齐方式</param>
        /// <returns></returns>
        public static ICellStyle GetCellStyle(HSSFWorkbook hssfworkbook, IFont font,
                                              HSSFColor fillForegroundColor, FillPattern fillPattern,
                                              HSSFColor fillBackgroundColor, HorizontalAlignment ha,
                                              VerticalAlignment va)
        {
            ICellStyle cellstyle = hssfworkbook.CreateCellStyle();
            cellstyle.FillPattern = fillPattern;
            cellstyle.Alignment = ha;
            cellstyle.VerticalAlignment = va;
            if (fillForegroundColor != null)
            {
                cellstyle.FillForegroundColor = fillForegroundColor.Indexed; //.GetIndex();
            }
            if (fillBackgroundColor != null)
            {
                cellstyle.FillBackgroundColor = fillBackgroundColor.Indexed; //.GetIndex();
            }
            if (font != null)
            {
                cellstyle.SetFont(font);
            }
            //有边框
            cellstyle.BorderBottom = BorderStyle.Thin; // CellBorderType.THIN;
            cellstyle.BorderLeft = BorderStyle.Thin;
            cellstyle.BorderRight = BorderStyle.Thin;
            cellstyle.BorderTop = BorderStyle.Thin;
            return cellstyle;
        }

        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="hssfworkbook">Excel操作类</param>
        /// <param name="fontname">字体名</param>
        /// <param name="fontcolor">字体颜色</param>
        /// <param name="fontsize">字体大小</param>
        /// <returns></returns>
        public static IFont GetFontStyle(HSSFWorkbook hssfworkbook, string fontfamily, HSSFColor fontcolor, int fontsize)
        {
            IFont font1 = hssfworkbook.CreateFont();
            if (string.IsNullOrEmpty(fontfamily))
            {
                font1.FontName = fontfamily;
            }
            if (fontcolor != null)
            {
                font1.Color = fontcolor.Indexed; //.GetIndex();
            }
            font1.IsItalic = true;
            font1.FontHeightInPoints = (short) fontsize;
            return font1;
        }

        #endregion


    }
}
