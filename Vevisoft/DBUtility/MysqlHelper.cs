using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Vevisoft.DBUtility
{
    public class MysqlHelper
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        private string ConnString = "";
        /// <summary>
        /// 数据库连接
        /// </summary>
        private MySqlConnection Conn;
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = "";
        /// <summary>
        /// 超时（秒）
        /// </summary>
        public int TimeOut = 100000;
        /// <summary>
        /// 初始化数据库链接
        /// </summary>
        /// <param name="connString">数据库链接</param>
        public MysqlHelper(string connString)
        {
            ConnString = connString;
            ConnTo();
        }
        /// <summary>
        /// 执行sql返回DataTable
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <param name="parms">Sql参数</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
                return null;
            }
        }


        /// <summary>
        /// 返回第一行
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public DataRow ExecuteDataTableRow(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
            }
            return null;
        }
        /// <summary>
        /// 返回第一个值
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public string ExecuteFirst(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                MySqlDataReader ss = cmd.ExecuteReader();
                string xx = "";
                if (ss.Read())
                    xx = ss[0].ToString();
                ss.Close();
                return xx;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
            }
            return null;
        }
        /// <summary>
        /// 返回第一个值
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public long ExecuteInsertId(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
            }
            return 0;
        }

        /// <summary>
        /// 执行无返回SQL语句
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <param name="parms">Sql参数</param>
        ///<returns>是否执行成功</returns>
        public bool ExecuteNonQuery(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
                return false;
            }
        }

        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <param name="parms">SQL参数</param>
        /// <returns>是否存在</returns>
        public bool ExecuteExists(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
                return false;
            }
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        private void ConnTo()
        {
            Close();
            try
            {
                Conn = new MySqlConnection(ConnString);
                Conn.Open();
            }
            catch (Exception e)
            {
                ErrorString += "数据库连接错误：" + e.Message + "\r\n连接串：" + ConnString + "\r\n";
                if (!string.IsNullOrEmpty(ErrorString) && ErrorString.Length > 1000)
                    ErrorString = null;
            }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        private void AddError(string message, string sql)
        {
            ErrorString += "数据库连接错误：" + message + "\r\nSQL语句：" + sql + "\r\n";
            if (!string.IsNullOrEmpty(ErrorString) && ErrorString.Length > 1000)
                ErrorString = "";
        }

        /// <summary>
        /// 关闭数据库链接
        /// </summary>
        public void Close()
        {
            try
            {
                Conn.Close();
                Conn = null;
            }
            catch
            {
            }
        }

    }
}
