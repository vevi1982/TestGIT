using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace QQMusicShareSongList
{
    public class QQBusiness
    {
        public DataTable GetQQList(string where)
        {
            return SqlLiteHelper.ExecuteDataset("select * from TbQQList", null).Tables[0];
        }

        public bool InsertQQList(string qq, string qqpass)
        {
            string sql = string.Format("INSERT INTO TbQQList ('QQNo','QQPass') VALUES ('{0}','{1}')", qq, qqpass);
            return Vevisoft.DataBase.SqlLiteHelper.ExecuteNonQuery(sql, null) > -1;
        }

        public bool ExistQQ(string qqno)
        {
            var sql = "select * from tbQQList where QQNo='" + qqno + "'";
            return Vevisoft.DataBase.SqlLiteHelper.ExecuteScalar(sql, null) != null;
        }
    }
}
