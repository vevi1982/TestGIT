using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace Vevisoft.Office
{
    public class ExcelHelper
    {
        /// <summary>
        /// 返回Excel数据源
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        static public DataSet ExcelToDataSet(string filename,string sheetName)
        {
            DataSet ds;
            string strCon = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1;\"", filename);
                           
            var myConn = new OleDbConnection(strCon);
            string strCom = "SELECT * FROM [" + sheetName + "$]";
            myConn.Open();
            var myCommand = new OleDbDataAdapter(strCom, myConn);
            ds = new DataSet();
            myCommand.Fill(ds,"data");
            myConn.Close();
            return ds;
        }
    }
}
