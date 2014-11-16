using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace Vevisoft.DBUtility
{
    public class DgvColInfoXmlHelper
    {
        private const string FileName = "DgvColInfoXml.vevi";
        private const string Attribute1 = "DBColName"; // element.SetAttribute("DBColName", dbColName);
        private const string Attribute2 = "ShowColName"; // element.SetAttribute("ShowColName", ShowColName);
        private const string Attribute3 = "ExcelColName"; // element.SetAttribute("ExcelColName", excelColName);"
        private const string Attribute4 = "IsPri"; //是否关键字段
        private const string RootName = "Datas";

        public static DataTable CreateTableStructure()
        {
            var dt = new DataTable();
            dt.Columns.Add(Attribute1, typeof (string));
            dt.Columns.Add(Attribute2, typeof(string));
            dt.Columns.Add(Attribute3, typeof(string));
            dt.Columns.Add(Attribute4, typeof(bool));
            return dt;
        }

        public static void CreateXmlFile()
        {
            if (System.IO.File.Exists(FileName))
                return;
            var doc = new XmlDocument();
            var dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            //创建一个根节点（一级）
            var root = doc.CreateElement(RootName);
            doc.AppendChild(root);
            doc.Save(FileName);
        }

        /// <summary>
        /// 获取DGV显示信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="showColList"></param>
        /// <param name="importColList"></param>
        public static void ReadInfos(string name, Dictionary<string, string> showColList,
                                    Dictionary<string, string> importColList)
        {
            CreateXmlFile();
            var doc = new XmlDocument();
            doc.Load(FileName);
            //获取信息
            var path = "/"+RootName + "/" + name;
            if (doc.DocumentElement != null)
            {
                var node = doc.DocumentElement.SelectSingleNode(path);
                if (node == null)
                    return;
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    var element = subNode as XmlElement;
                    if (element == null)
                        continue;
                    string str1 = element.Attributes[Attribute1].Value;
                    string str2 = element.Attributes[Attribute2].Value;
                    string str3 = element.Attributes[Attribute3].Value;
                    var isPri = bool.Parse(element.Attributes[Attribute4].Value);
                    if (showColList.ContainsKey(str1))
                        showColList[str1] = str2;
                    else showColList.Add(str1, str2);
                    if (importColList.ContainsKey(str1))
                        importColList[str1] = str3;
                    else importColList.Add(str1, str3);
                }
            }
        }
        public static DataTable ReadInfos(string name)
        {
            CreateXmlFile();
            var doc = new XmlDocument();
            doc.Load(FileName);
            //
            var dt = CreateTableStructure();
            //获取信息
            var path = "/" + RootName + "/" + name;
            if (doc.DocumentElement != null)
            {
                var node = doc.DocumentElement.SelectSingleNode(path);

                if (node != null)
                    foreach (XmlNode subNode in node.ChildNodes)
                    {
                        var element = subNode as XmlElement;
                        if (element == null)
                            continue;
                        string str1 = element.GetAttribute(Attribute1);
                        string str2 = element.GetAttribute(Attribute2);
                        string str3 =element.GetAttribute(Attribute3);
                        var isPri = bool.Parse(string.IsNullOrEmpty(element.GetAttribute(Attribute4)) ? "False" : element.GetAttribute(Attribute4));
                        var row = dt.NewRow();
                        row[0] = str1;
                        row[1] = str2;
                        row[2] = str3;
                        row[3] = isPri;
                        dt.Rows.Add(row);
                    }
            }
            return dt;
        }
        /// <summary>
        /// 将数据写入文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="showColList"></param>
        /// <param name="importColList"></param>
        public static void WriteInfos(string name, Dictionary<string, string> showColList,
                                     Dictionary<string, string> importColList)
        {
             CreateXmlFile();
            var doc = new XmlDocument();
            doc.Load(FileName);
            //获取信息
            var path = "/" + RootName + "/" + name;
            if (doc.DocumentElement != null)
            {
                var node = doc.DocumentElement.SelectSingleNode(path);
                if (node == null)
                {
                    //创建表的节点
                    node = doc.CreateElement(name);
                    doc.DocumentElement.AppendChild(node);
                }
                else node.RemoveAll();
                //添加子节点
                foreach (var key in showColList.Keys)
                {
                    if (importColList.ContainsKey(key))
                    {
                        var xe = WriteOneItem(node as XmlElement, key, showColList[key], importColList[key],false);
                        if (node != null) node.AppendChild(xe);
                    }
                }
                doc.Save(FileName);
            }
        }
        /// <summary>
        /// 将数据写入文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="showColList"></param>
        /// <param name="importColList"></param>
        public static void WriteInfos(string name, DataTable dtSource)
        {
            CreateXmlFile();
            var doc = new XmlDocument();
            doc.Load(FileName);
            //获取信息
            var path = "/" + RootName + "/" + name;
            if (doc.DocumentElement != null)
            {
                var node = doc.DocumentElement.SelectSingleNode(path);
                if (node == null)
                {
                    //创建表的节点
                    node = doc.CreateElement(name);
                    doc.DocumentElement.AppendChild(node);
                }
                else node.RemoveAll();
                //添加子节点
                foreach (DataRow row in dtSource.Rows)
                {
                    var str1 = "";
                    var str2 = ""; var str3 = ""; var isPri = false;
                    if (dtSource.Columns.Contains(Attribute1))
                     str1 = row[Attribute1].ToString();
                    if (dtSource.Columns.Contains(Attribute2))
                     str2 = row[Attribute2].ToString();
                    if(dtSource.Columns.Contains(Attribute3))
                     str3 = row[Attribute3].ToString();
                    if(dtSource.Columns.Contains(Attribute4))
                     isPri = Convert.ToBoolean(row[Attribute4].ToString());
                    var xe = WriteOneItem(node as XmlElement, str1, str2, str3, isPri);
                    node.AppendChild(xe);
                }
                doc.Save(FileName);
            }
        }
        private static XmlElement WriteOneItem(XmlElement xdoc, string dbColName, string ShowColName, string excelColName,bool isPri)
        {
            if (xdoc!=null&&xdoc.OwnerDocument != null)
            {
                var element = xdoc.OwnerDocument.CreateElement("Col");
                element.SetAttribute(Attribute1, dbColName);
                element.SetAttribute(Attribute2, ShowColName);
                element.SetAttribute(Attribute3, excelColName);
                element.SetAttribute(Attribute4, isPri ? "True" : "False");
                return element;
            }
            return null;
        }
    }
}
