using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltiumTask.Utils
{
    public static class MyExtensions
    {
        public static List<string> CustomSort(this List<string> chunk)
        {
            chunk.Sort((item1, item2) =>
            {
                var splitItem1 = SplitItem(item1);
                var splitItem2 = SplitItem(item2);
                int stringComparison = string.Compare(splitItem1.Item2, splitItem2.Item2, StringComparison.Ordinal);
                if (stringComparison != 0)
                    return stringComparison;

                int number1 = int.Parse(splitItem1.Item1);
                int number2 = int.Parse(splitItem2.Item1);
                return number1.CompareTo(number2);
            });
            return chunk;
        }

        private static Tuple<string, string> SplitItem(string line)
        {
            int dotIndex = line.IndexOf('.');
            string numberPart = line.Substring(0, dotIndex).Trim();
            string stringPart = line.Substring(dotIndex + 1).Trim();
            return Tuple.Create(numberPart, stringPart);
        }
    }
}
