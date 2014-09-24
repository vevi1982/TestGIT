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
           
                MySql.Data.MySqlClient.MySqlConnectionStringBuilder conn=new MySqlConnectionStringBuilder(
                    );
            
        }

        public DataTable GetAccountInfo()
        {
            var sql = "select * from account";
            return helper.ExecuteDataTable(sql, null);
        }
    }
}
