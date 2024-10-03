using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltiumTask.Utils
{
    public class CustomPriority : IComparable<CustomPriority>
    {
        public int _number { get; set; }
        public string _text { get; set; }

        public CustomPriority(int number, string text)
        {
            _number = number;
            _text = text;
        }

        public int CompareTo(CustomPriority? itemToCompare)
        {
            int textComparison = String.Compare(_text, itemToCompare._text);
            if(textComparison != 0)
                return textComparison;

            return _number.CompareTo(itemToCompare._number);
        }
    }
}
