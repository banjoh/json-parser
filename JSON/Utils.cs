using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON
{
    // Utility functions
    public class Utils
    {
        public static bool IsValue(object o)
        {
            return !IsDictionary(o) && !IsList(o);
        }

        public static Dictionary<object, object> ToDictionary(object o)
        {
            return o as Dictionary<object, object>;
        }

        public static List<object> ToList(object o)
        {
            return o as List<object>;
        }

        public static bool IsDictionary(object o)
        {
            return ToDictionary(o) != null;
        }

        public static bool IsList(object o)
        {
            return ToList(o) != null;
        }

        public static bool AreDictionaries(object a, object b)
        {
            return IsDictionary(a) && IsDictionary(b);
        }

        public static bool AreLists(object a, object b)
        {
            return IsList(a) && IsList(b);
        }

        public static bool ValuesEqual(object a, object b)
        {
            return IsValue(a) && IsValue(b) && a.Equals(b);
        }
    }
}
