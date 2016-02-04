using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Organiser.Common.Classes
{
    public static class ObjectCopier
    {
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }
            StringBuilder sbReplaced = sb.Replace("&", "&amp;");

            return sbReplaced.ToString();
        }

        public static List<string> XmlSerializeToStringChunks(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            List<string> chunks = new List<string>();
            string s = "";

            for (int i = 0; i < sb.Length; i++)
            {
                s += sb[i];
                if(s.Length >= 1000000)
                {
                    chunks.Add(s);
                    s = "";
                }
            }

            return chunks;
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append(objectData);
            //StringBuilder sbReplaced = sb.Replace("&", "&amp;");
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static string CleanInvalidXmlChars(this string StrInput)
        {
            //Returns same value if the value is empty.
            if (string.IsNullOrWhiteSpace(StrInput))
            {
                return StrInput;
            }
            // From xml spec valid chars:
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]    
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
            string RegularExp = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(StrInput, RegularExp, String.Empty);
        }
    }
}
