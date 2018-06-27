using Browseo.Browser.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Browseo.Browser.Framework.IO
{
    public class Path
    {
        public static string Combine(params string[] path)
        {
            string toreturn = "";
            foreach (var item in path)
            {
                toreturn += item + "\\";
            }
            toreturn = toreturn.Replace("\\\\", "\\");
            if (toreturn.EndsWith("\\")) toreturn = toreturn.Remove(toreturn.LastIndexOf("\\"));

            return toreturn;
        }
    }


    public class File : Delimon.Win32.IO.File
    {
        new public static void WriteAllText(string path, string contents)
        {
            if (File.Exists(path)) File.Delete(path);

            Delimon.Win32.IO.File.WriteAllText(path, contents);
        }

        new public static void WriteAllText(string path, string contents, Encoding enc)
        {
            if (File.Exists(path)) File.Delete(path);

            Delimon.Win32.IO.File.WriteAllText(path, contents, enc);
        }

        new public static void WriteAllLines(string path, string[] contents)
        {
            WriteAllLines(path, contents.ToList());
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            string content = "";
            foreach (var line in contents)
            {
                content += line + Environment.NewLine;
            }
            WriteAllText(path, content);
        }

        new public static bool Exists(string path)
        {
            return Delimon.Win32.IO.File.Exists(path);
        }
    }


    public class Directory : Delimon.Win32.IO.Directory
    {
        new public static void CreateDirectory(string path)
        {
            string thenewDirectorys = path;
            thenewDirectorys = thenewDirectorys.Replace(BaseDirectories.Instance.BaseDir, "");
            var dirs = thenewDirectorys.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            string appendedDirs = Path.Combine(BaseDirectories.Instance.BaseDir);
            foreach (var dir in dirs)
            {
                appendedDirs = appendedDirs + "\\" + dir;
                if (!Directory.Exists(appendedDirs)) Delimon.Win32.IO.Directory.CreateDirectory(appendedDirs);
            }
        }
    }

    public class DirectoryInfo : Delimon.Win32.IO.DirectoryInfo
    {
        public DirectoryInfo(string dir):base(dir)
        {

        }
    }
}
