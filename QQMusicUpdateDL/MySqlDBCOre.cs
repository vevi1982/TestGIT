using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Vevisoft.DBUtility;

namespace QQMusicUpdateDL
{
    public class MySqlDBCOre
    {

        string connstr = ConfigurationManager.AppSettings["MySqlConnectionStr"];
        private Vevisoft.DBUtility.MysqlHelper helper;

        public MySqlDBCOre()
        {
            helper=new MysqlHelper(connstr);
        }

        public DataTable GetAccountInfo()
        {
            var sql = "select * from account";
            return helper.ExecuteDataTable(sql, null);
        }

        public bool ExecuteSql(string sql)
        {
            return helper.ExecuteNonQuery(sql, null);
        }
    }
}
