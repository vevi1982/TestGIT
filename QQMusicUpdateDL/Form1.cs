using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.DBUtility;

namespace QQMusicUpdateDL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string>upSqlList=new List<string>(); 


        private QQMusicHelper.DownLoadInfo GetUpdateSql(string qqno)
        {
            var sql = "";
            var info= QQMusicHelper.DownLoadInfoHelper.GetDownLoadInfo(qqno);
            //
            if (info.Level == 3)
            {
                if (info.Remain > 0)
                {
                    var count = 800 - info.Remain;
                    sql = string.Format("update account set week_counter={0},day_counter={1},is_enable=1 where qq_no='{2}'", info.Dl,
                                        count,
                                        qqno);
                }
                else
                {
                    sql = string.Format("update account set week_counter={0},day_counter={1},is_enable=0 where qq_no='{2}'", info.Dl,
                                        0,
                                        qqno);
                }
                upSqlList.Add(sql);
            }else if (info.Level == 4)
            {
                if (info.Remain > 0)
                {
                    var count = 800 - info.Remain;
                    var weekcount = 2400 - info.Remain;
                    sql = string.Format("update account set month_counter={0},week_counter={3},day_counter={1},is_enable=1 where qq_no='{2}'", info.Dl,
                                        count,
                                        qqno,weekcount);
                }
                else
                {
                    sql = string.Format("update account set month_counter={0},week_counter=2400,day_counter={1},is_enable=0 where qq_no='{2}'", info.Dl,
                                        0,
                                        qqno);
                }
                upSqlList.Add(sql);
            }
            else
            {
                //level=0 当天下载量
                if (info.Remain > 0)
                {
                    var count = 800 - info.Remain;
                    sql = string.Format("update account set day_counter={0},is_enable=1 where qq_no='{1}'", info.Dl,
                                        qqno);
                }
                else
                {
                    sql = string.Format("update account set day_counter={0},is_enable=1 where qq_no='{1}'", info.Dl,
                                      qqno);
                } upSqlList.Add(sql);
            }
            return info;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetAccount();
        }

        private void GetAccount()
        {
            var core = new MySqlDBCOre();
            var dt = core.GetAccountInfo();
            dataGridView1.DataSource = dt;
            //
            if (!string.IsNullOrEmpty(MysqlHelper.ErrorString))
                MessageBox.Show(MysqlHelper.ErrorString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            upSqlList=new List<string>();
            var dt = dataGridView1.DataSource as DataTable;
            if (dt == null)
            {
                MessageBox.Show("请现获取QQ账户信息!");
                return;
            }
            //获取信息
            var count = 1;
            foreach (DataRow row in dt.Rows)
            {
                var qqno = row["qq_no"].ToString();
                var info= GetUpdateSql(qqno);
                richTextBox1.AppendText(string.Format("{0}  {1}",count,info.ToString()));
                count++;
            }

            //richTextBox1.AppendText("======SQL List===========\r\n");
            //foreach (var VARIABLE in upSqlList)
            //{
            //    richTextBox1.AppendText(VARIABLE+"\r\n");
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var core = new MySqlDBCOre();
            if (upSqlList.Count > 0)
            {
                foreach (var star in upSqlList)
                {
                    bool ret = core.ExecuteSql(star);
                    richTextBox1.AppendText(star +"============   "+(ret?"True":"False")+"   "+ "\r\n");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new FrmAddQQ().Show();
        }



    }
}
