using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    internal static class PathApi
    {
        /// <summary>
        /// create path if not exists
        /// deletes all content in path 
        /// </summary>
        /// <param name="path"></param>
        internal static void ClearCreateFolder(string path)
        {
            if (false == System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string[] files = System.IO.Directory.GetFiles(path);
            foreach (string  file in files)
                System.IO.File.Delete(file);

            string[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (string dir in dirs)
                System.IO.Directory.Delete(dir,true);
        }

        internal static void CreateFolder(string path)
        {
            if (false == System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }
    }
}
