using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
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

        public static void ImportExcel()
        {
            using (var con = new OleDbConnection(ConfigurationManager.ConnectionStrings["ExcelCon"].ConnectionString))
            {
                con.Open();
                var com = new OleDbCommand("Select * from [EmployeeInfo$]", con);
                var dr = com.ExecuteReader();
                using (var sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["Sql"].ConnectionString))
                {
                    sqlcon.Open();
                    using (var bulkCopy = new SqlBulkCopy(sqlcon))
                    {
                        bulkCopy.ColumnMappings.Add("[Employee Name]", "EmpName");
                        bulkCopy.ColumnMappings.Add("Department", "Department");
                        bulkCopy.ColumnMappings.Add("Address", "Address");
                        bulkCopy.ColumnMappings.Add("Age", "Age");
                        bulkCopy.ColumnMappings.Add("Sex", "Sex");
                        bulkCopy.DestinationTableName = "Employees";
                        if (dr != null) bulkCopy.WriteToServer(dr);
                    }
                }
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            //Response.Write("Upload Successfull!");
        }

        #region Methodes 2 
        ///<summary>
        ///根据excel路径和sheet名称，返回excel的DataTable
        ///</summary>
        public static DataTable GetExcelDataTable(string path, string tname)
        {
            /*Office 2007*/
            string ace = "Microsoft.ACE.OLEDB.12.0";
            /*Office 97 - 2003*/
            string jet = "Microsoft.Jet.OLEDB.4.0";
            string xl2007 = "Excel 12.0 Xml";
            string xl2003 = "Excel 8.0";
            string imex = "IMEX=1";
            /* csv */
            string text = "text";
            string fmt = "FMT=Delimited";
            string hdr = "Yes";
            string conn = "Provider={0};Data Source={1};Extended Properties=\"{2};HDR={3};{4}\";";
            string select = string.Format("SELECT * FROM [{0}$]", tname);
            //string select = sql;
            string ext = Path.GetExtension(path);
            OleDbDataAdapter oda;
            DataTable dt = new DataTable("data");
            switch (ext.ToLower())
            {
                case ".xlsx":
                    conn = String.Format(conn, ace, Path.GetFullPath(path), xl2007, hdr, imex);
                    break;
                case ".xls":
                    conn = String.Format(conn, jet, Path.GetFullPath(path), xl2003, hdr, imex);
                    break;
                case ".csv":
                    conn = String.Format(conn, jet, Path.GetDirectoryName(path), text, hdr, fmt);
                    //sheet = Path.GetFileName(path);
                    break;
                default:
                    throw new Exception("File Not Supported!");
            }
            OleDbConnection con = new OleDbConnection(conn);
            con.Open();
            //select = string.Format(select, sql);
            oda = new OleDbDataAdapter(select, con);
            oda.Fill(dt);
            con.Close();
            return dt;
        }
        /// <summary>
        /// 批量把数据导入到数据库
        /// </summary>
        /// <param name="maplist"></param>
        /// <param name="TableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool SqlBulkCopyImport(IList<string> maplist, string TableName, DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(""))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = TableName;
                    foreach (string a in maplist)
                    {
                        bulkCopy.ColumnMappings.Add(a, a);
                    }
                    try
                    {
                        bulkCopy.WriteToServer(dt);
                        return true;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        #endregion
    }
}
