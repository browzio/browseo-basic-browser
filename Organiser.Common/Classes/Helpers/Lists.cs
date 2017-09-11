using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Organiser.Common.Classes
{
    public class Lists
    {
        //public static List<T> BubbleSort<T>(List<T> toSort)
        //{
        //    int temp;

        //    // foreach(int i in a)
        //    for (int i = 1; i <= toSort.Count; i++)
        //        for (int j = 0; j < toSort.Count - i; j++)
        //        {
        //            if (toSort[j].Keyword == toSort[j + 1].Keyword)
        //                if (Convert.ToInt32(toSort[j].Position) > Convert.ToInt32(toSort[j + 1].Position))
        //                {
        //                    temp = Convert.ToInt32(toSort[j].Position);
        //                    toSort[j].Position = toSort[j + 1].Position;
        //                    toSort[j + 1].Position = temp.ToString();
        //                }
        //        }

        //    return toSort;
        //}

        public static List<List<T>> BreakIntoChunks<T>(List<T> list, int chunkSize)
        {
            List<List<T>> retVal = new List<List<T>>();

            while (list.Count > 0)
            {
                int count = list.Count > chunkSize ? chunkSize : list.Count;
                retVal.Add(list.GetRange(0, count));
                list.RemoveRange(0, count);
            }

            return retVal;
        }

        public static ObservableCollection<T> Shuffle<T>(ObservableCollection<T> list)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Random rng = new Random();
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            });
            return list;
        }
    }
}
