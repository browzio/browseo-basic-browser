using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Organiser.Common.Classes
{
    public static class ExtensionMethods
    {
        public static int RemoveAllThese<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null) return "";

            FieldInfo field = type.GetField(name);
            if (field == null) return "";

            DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attr == null) return "";

            return attr.Description;
        }

        public static string RemoveAmps(this string str)
        {
            str = str.Replace("&amp;" ,"&");
            str = str.Replace("amp;" ,"");
            return str;
        }
    }
}
