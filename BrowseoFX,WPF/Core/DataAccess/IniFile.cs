using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace BrowseoFX_WPF.Core.DataAccess
{
    public class IniFile
    {
        public string path;
        public string[] fileLines = new string[0];

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <param name="INIPath"></param>
        public IniFile(string INIPath)
        {
            path = INIPath;
            if (File.Exists(this.path)) fileLines = File.ReadAllLines(this.path);
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <param name="Section"></param>
        /// Section name
        /// <param name="Key"></param>
        /// Key Name
        /// <param name="Value"></param>
        /// Value Name
        public void IniWriteValue<T>(string section, T Value)
        {
            IniWriteValue(section, Value.GetType().Name, Convert.ToString(Value));
        }
        public void IniWriteValue(string Section, string Key, string Value)
        {
            var vals = WritePrivateProfileString(Section, Key, Value, this.path);
            if (vals == 0)
            {
                if (!File.Exists(this.path))
                {
                    File.AppendAllText(path, "[" + Section + "]" + Environment.NewLine);
                }
                string fileText = File.ReadAllText(this.path);
                if (!fileText.Contains(Key))
                {
                    File.AppendAllText(path, Key + "=" + Value + Environment.NewLine);
                }
                else
                {
                    string[] lines = fileText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var user in lines)
                    {
                        string fline = user;
                        if (fline.Split(new string[] { "=" }, StringSplitOptions.None)[0].Trim() == Key.Trim())
                        {
                            fileText = fileText.Replace(fline, Key + "=" + Value);
                            File.WriteAllText(this.path, fileText);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            if (i == 0)
            {
                return "";
                //foreach (var line in fileLines)
                //{
                //    if (!line.Contains("=")) continue;
                //    string[] keyvale = line.Split('=');
                //    string sectionVal = keyvale[1], sectionkey = keyvale[0];
                //    if (sectionkey == Key)
                //    {
                //        return sectionVal;
                //    }
                //}
            }
            return temp.ToString();

        }
        public T IniReadValue<T>(T Key)
        {
            return IniReadValue<T>("Data", Key.GetType().Name);
        }
        public T IniReadValue<T>(string Section, string Key)
        {
            var val = IniReadValue(Section, Key);
            //var converter = TypeDescriptor.GetConverter(typeof(string)).ConvertTo(val, typeof(T));

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(string));
            object propValue = typeConverter.ConvertFromString(val);

            return (T)propValue;
        }

        // The Function called to obtain the SectionHeaders,
        // and returns them in an Dynamic Array.
        public string[] GetSectionNames()
        {
            //    Sets the maxsize buffer to 500, if the more
            //    is required then doubles the size each time.
            for (int maxsize = 500; true; maxsize *= 2)
            {
                //    Obtains the information in bytes and stores
                //    them in the maxsize buffer (Bytes array)
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxsize, path);

                // Check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size < maxsize - 2)
                {
                    // Converts the bytes value into an ASCII char. This is one long string.
                    string Selected = Encoding.ASCII.GetString(bytes, 0,
                                               size - (size > 0 ? 1 : 0));
                    // Splits the Long string into an array based on the "\0"
                    // or null (Newline) value and returns the value(s) in an array
                    return Selected.Split(new char[] { '\0' });
                }
            }
        }
    }
}
