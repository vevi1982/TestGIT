using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vevisoft.IO
{
    public class Directory
    {
        /// <summary>
        /// 删除空文件夹内的所有文件与文件夹
        /// </summary>
        /// <param name="path">要删除的文件夹目录</param>
        public static void DeleteDirectoryContent(string path)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                var childs = dir.GetDirectories();
                foreach (var child in childs)
                {
                    child.Delete(true);
                
                }
                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }
                //dir.Delete(true);
            }
        }
    }
}
