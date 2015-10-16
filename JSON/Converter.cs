using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JSON
{
    // Serializer and deserializer functions
    public class Converter
    {
        public static object Deserialize(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                throw new ArgumentException("input argument is in a valid string");

            Debug.WriteLine(input);

            // Stack used to push objects in as the json structure increases in depth.
            // At the end of the input iteration the stach should contain 1 object
            // which is the root JSON object.
            Stack<object> stack = new Stack<object>();

            // Holds the value characters.
            List<char> currValue = new List<char>();
            bool inQuotes = false;
            Stack<object> commaFlags = new Stack<object>();

            try
            {
                // Start parsing input
                foreach (char c in input)
                {
                    Debug.WriteLine(c);

                    // Check for control characters
                    switch (c)
                    {
                        case '[':
                            stack.Push(new List<object>());
                            currValue.Clear();
                            continue;
                        case ']':
                            ValidateCommaFlag(commaFlags);
                            if (currValue.Count > 0)
                            {
                                // Push the key to the top of the stack
                                stack.Push(ParseValue(currValue));
                                currValue.Clear();
                            }

                            // Unwind stack until we find a list object
                            // Add all values to the list
                            var tempList = new List<object>();

                            // Count will check if the first object is a list
                            int count = 0;
                            while (stack.Count > 0)
                            {
                                object o = stack.Pop();
                                if (!Utils.IsList(o) || count == 0)
                                {
                                    tempList.Add(CastObject(o));
                                }
                                else
                                {
                                    tempList.Reverse();
                                    stack.Push(new FinalList(tempList));
                                    break;
                                }

                                ++count;
                            }
                            currValue.Clear();
                            continue;
                        case '{':
                            stack.Push(new Dictionary<object, object>());
                            currValue.Clear();
                            continue;
                        case '}':
                            ValidateCommaFlag(commaFlags);
                            if (currValue.Count > 0)
                            {
                                // Push the key to the top of the stack
                                stack.Push(ParseValue(currValue));
                                currValue.Clear();
                            }

                            List<Tuple<object, object>> l = new List<Tuple<object, object>>();
                            while (stack.Count > 0)
                            {
                                // Unwind the stack till we find a dictionary
                                object v = stack.Pop();
                                object k = stack.Pop();
                                l.Add(new Tuple<object, object>(k, v));
                                if (Utils.IsDictionary(stack.Peek()))
                                {
                                    var d = Utils.ToDictionary(stack.Pop());
                                    l.Reverse();
                                    foreach (var t in l)
                                    {
                                        d.Add(CastObject(t.Item1), CastObject(t.Item2));
                                    }
                                    stack.Push(new FinalDictionary(d));
                                    break;
                                }
                            }

                            currValue.Clear();
                            continue;
                        case '"':
                            if (inQuotes)
                            {
                                stack.Push(ParseValue(currValue));
                            }
                            currValue.Clear();
                            inQuotes = !inQuotes;
                            continue;
                        case ':':
                        case ',':
                            if (currValue.Count > 0)
                            {
                                // Push the key to the top of the stack
                                stack.Push(ParseValue(currValue));
                                currValue.Clear();
                            }
                            commaFlags.Push(new object());
                            continue;
                        default:
                            break;
                    }

                    // Discard unnecessary spaces when not parsing a string
                    if (inQuotes || (Char.IsNumber(c) || c.Equals('-') || c.Equals('+')))
                    {
                        currValue.Add(c);
                        if (commaFlags.Count > 0)
                            commaFlags.Pop();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            ValidateCommaFlag(commaFlags);
            if (stack.Count != 1)
                throw new Exception("The stack countains more/less objects than what is expected.");

            return stack.Count == 1 ? CastObject(stack.Pop()) : null;
        }

        private static void ValidateCommaFlag(Stack<object> commaFlags)
        {
            if (commaFlags.Count > 0)
                throw new Exception("A comma without a value after is in the schema");
        }
        
        private static object CastObject(object o)
        {
            if (o as FinalList != null)
                return (o as FinalList).List;
            else if (o as FinalDictionary != null)
                return (o as FinalDictionary).Dictionary;
            else
                return o;
        }

        private static object ParseValue(List<char> value)
        {
            string str = new string(value.ToArray());
            int val;
            if (Int32.TryParse(str.Trim(), out val))
                return val;

            return str;
        }

        public static string Serialize(object obj)
        {
            if (Utils.IsDictionary(obj))
                return ParseDict(Utils.ToDictionary(obj));
            else if (Utils.IsList(obj))
                return ParseList(Utils.ToList(obj));
            else
                throw new ArgumentNullException("obj argument is not a valid JSON object. Needs to be List of Dictionary");
        }

        private static string ParseList(List<object> list)
        {
            var l = new List<string>();

            foreach (object o in list)
            {
                l.Add(ParseObject(o));
            }

            return "[" + String.Join(", ", l) + "]";
        }

        private static string ParseDict(Dictionary<object, object> dict)
        {
            List<string> l = new List<string>();

            foreach (object o in dict.Keys)
            {
                string key = ParseObject(o);
                string value = ParseObject(dict[o]);

                l.Add(key + ": " + value);
            }

            return "{" + String.Join(", ", l) + "}";
        }

        private static string ParseObject(object o)
        {
            int i;
            if (Int32.TryParse(o.ToString(), out i))
            {
                return o.ToString();
            }
            else if (Utils.IsDictionary(o))
            {
                return ParseDict(Utils.ToDictionary(o));
            }
            else if (Utils.IsList(o))
            {
                return ParseList(Utils.ToList(o));
            }
            else if (o as string != null)
            {
                return String.Format(@"""{0}""", o as string);
            }
            else throw new Exception("Could not parse object");
        }
    }
}
