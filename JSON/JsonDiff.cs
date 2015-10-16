using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Diagnostics;

[assembly: InternalsVisibleTo("UnitTests")]
namespace JSON
{
    // Final classes used to "close" a data structure once the closing tag is reached when
    // unwinding the stack. Such an object is treated the same as any other i.e. appended to array etc
    internal class FinalDictionary
    {
        public Dictionary<object, object> Dictionary { get; private set; }

        public FinalDictionary(Dictionary<object, object> dict)
        {
            Dictionary = dict;
        }
    }

    internal class FinalList
    {
        public List<object> List { get; private set; }

        public FinalList(List<object> list)
        {
            List = list;
        }
    }

    // JSON class containing serializer and deserializer functions. A diff function for diff'ing two json string is available
    public static class JsonDiff
    {
        public static string DiffStrings(string a, string b)
        {
            if (String.IsNullOrWhiteSpace(a))
                throw new ArgumentException("a argument is not a valid string");
            if (String.IsNullOrWhiteSpace(b))
                throw new ArgumentException("b argument is not a valid string");

            if (a.Trim().Equals(b.Trim()))
            {
                return null;
            }

            var aObj = Converter.Deserialize(a);
            var bObj = Converter.Deserialize(b);

            var diff = DiffObjects(aObj, bObj);
            if (diff != null)
                return Converter.Serialize(diff);

            return null;
        }

        /// <summary>
        /// Recursive search for differences
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Null if both ojects are equal, otherwise an object with the differences</returns>
        public static object DiffObjects(object a, object b)
        {
            if (Utils.AreDictionaries(a, b))
            {
                return DiffDictionary(Utils.ToDictionary(a), Utils.ToDictionary(b));
            }
            else if (Utils.AreLists(a, b))
            {
                return DiffList(Utils.ToList(a), Utils.ToList(b));
            }
            else if (Utils.ValuesEqual(a, b))
            {
                return null;
            }

            return b;
        }

        internal static object DiffDictionary(Dictionary<object, object> a, Dictionary<object, object> b)
        {
            Dictionary<object, object> diff = new Dictionary<object, object>();

            var aList = a.ToList();
            var bList = b.ToList();
            var bListMatchedIdx = new List<int>();
            var notFoundKeys = new List<KeyValuePair<object, object>>();

            // Nested loop to search for a's key-value pair in b
            for (int i = 0; i < aList.Count; i++)
            {
                var aKv = aList[i];
                bool aFoundKeyMatch = false;

                // Iterate through b list to find a's key-value pair in it
                for (int j = 0; j < bList.Count; j++)
                {
                    var bKv = bList[j];

                    if (DiffObjects(aKv.Key, bKv.Key) == null)
                    {
                        aFoundKeyMatch = true;
                        if (DiffObjects(aKv.Value, bKv.Value) == null)
                        {
                            // A key-value pair was found.
                            // TODO: Duplicates ignored. Should we support duplicates?
                            bListMatchedIdx.Add(j);
                            break;
                        }
                    }
                }

                // a's key was not found in b list
                if (!aFoundKeyMatch)
                    notFoundKeys.Add(new KeyValuePair<object, object>(aKv.Key, "undefined"));
            }

            // Store all the unmatched key-value pairs in the diff list
            for (int i = 0; i < bList.Count; i++)
            {
                if (!bListMatchedIdx.Contains(i))
                {
                    var bKv = bList[i];
                    KeyValuePair<object, object> diffed = new KeyValuePair<object, object>(null, null);

                    // Do a further comparison at the same index in aList
                    // TODO: Improve algorithm for performance. We do not need to iterate a second time
                    if (aList.Count > i)
                    {
                        var aKv = aList[i];
                        if (DiffObjects(aKv.Key, bKv.Key) == null)
                        {
                            diffed = new KeyValuePair<object, object>(bKv.Key, DiffObjects(aKv.Value, bKv.Value));
                        }
                    }

                    if (diffed.Key == null || diffed.Value == null)
                    {
                        diff.Add(bKv.Key, bKv.Value);
                    }
                    else
                    {
                        diff.Add(diffed.Key, diffed.Value);
                    }
                }
            }

            // Append all the keys that were not found in 'a' to the diff
            foreach (var kv in notFoundKeys)
            {
                diff.Add(kv.Key, kv.Value);
            }

            return diff.Count == 0 ? null : diff;
        }
        
        internal static object DiffList(List<object> a, List<object> b)
        {
            var diff = new List<object>();
            var bListMatchedIdx = new List<int>();

            // Nested loop to compare list elements
            foreach (var o in a)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (DiffObjects(o, b[i]) == null)
                    {
                        // Elements found in b list. Duplicate ignored
                        bListMatchedIdx.Add(i);
                        break;
                    }
                }
            }

            // Store all the unmatched key-value pairs
            for (int i = 0; i < b.Count; i++)
            {
                if (!bListMatchedIdx.Contains(i))
                {
                    // Do a further comparison at the same index in 'a'
                    if (a.Count > i)
                    {
                        diff.Add(DiffObjects(a[i], b[i]));
                    }
                    else
                    {
                        diff.Add(b[i]);
                    }
                }
            }
            
            return diff.Count == 0 ? null : diff;
        }
    }
}
