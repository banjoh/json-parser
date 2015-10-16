using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Diff
    {
        private readonly string first = @"{
                                            ""foo"": {
                                            ""bar"": ""baz"",
                                            ""biz"": ""foo""
                                            },
                                            ""fiz"": {
                                            ""foo"": ""baz""
                                            },
                                            ""bar"": ""baz"",
                                            ""baz"": [
                                            ""foo"",
                                            ""bar""
                                            ],
                                            ""miss"": 123
                                          }";

        private readonly string second = @"{
                                              ""foo"": {
                                                ""bar"": ""baz1"",
                                                ""biz"": ""foo""
                                              },
                                              ""fiz"": {
                                                ""foo"": ""baz""
                                              },
                                              ""bar"": ""baz"",
                                              ""baz"": [
                                                ""foo1""
                                              ],
                                              ""new_value"": 1
                                           }";
        private readonly string diff = @"{""foo"": {""bar"": ""baz1""}, ""baz"": [""foo1""], ""new_value"": 1, ""miss"": ""undefined""}";

        [TestMethod]
        public void TestDiff1()
        {
            string s = JSON.JsonDiff.DiffStrings(first, second);
            Assert.IsTrue(diff.Equals(s));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDiff2()
        {
            JSON.JsonDiff.DiffStrings("", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDiff3()
        {
            JSON.JsonDiff.DiffStrings(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDiff4()
        {
            JSON.JsonDiff.DiffStrings(first, "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDiff5()
        {
            JSON.JsonDiff.DiffStrings(null, second);
        }

        [TestMethod]
        public void TestDiff6()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("bar", 7);
            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            b.Add("bar", 7);
            Assert.IsNull(JSON.JsonDiff.DiffDictionary(a, b));
        }

        [TestMethod]
        public void TestDiff7()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("bar", 7);
            var b = new Dictionary<object, object>();
            b.Add("foo", 8);
            b.Add("bar", 7);
            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;

            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.ContainsKey("foo"));
            Assert.IsTrue(diff["foo"].Equals(8));
        }

        [TestMethod]
        public void TestDiff8()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("bar", "tests");
            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            b.Add("bar", 7);

            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.ContainsKey("bar"));
            Assert.IsTrue(diff["bar"].Equals(7));
        }

        [TestMethod]
        public void TestDiff9()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("bar", new List<object> { "gee", "eff" });
            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            b.Add("bar", new List<object> { "gee", "eff" });
            Assert.IsNull(JSON.JsonDiff.DiffDictionary(a, b));
        }

        [TestMethod]
        public void TestDiff10()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            var aa = new Dictionary<object, object>();
            aa.Add("gee", "eff");
            a.Add("bar", aa);
            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            var bb = new Dictionary<object, object>();
            bb.Add("gee", "eff");
            b.Add("bar", bb);
            Assert.IsNull(JSON.JsonDiff.DiffDictionary(a, b));
        }

        [TestMethod]
        public void TestDiff11()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("bar", new List<object> { "gee", "eff" });
            var b = new Dictionary<object, object>();
            b.Add("foo", 10);
            b.Add("bar", new List<object> { "gee", "eff" });

            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            Assert.IsTrue(diff.ContainsKey("foo"));
            Assert.IsTrue(diff["foo"].Equals(10));
        }

        [TestMethod]
        public void TestDiff12()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            var aa = new Dictionary<object, object>();
            aa.Add("gee", "eff");
            a.Add("bar", aa);
            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            var bb = new Dictionary<object, object>();
            bb.Add("gee", 12);
            b.Add("bar", bb);

            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            Assert.IsTrue(diff.ContainsKey("bar"));
            var d = diff["bar"] as Dictionary<object, object>;
            Assert.IsNotNull(d);
            Assert.IsTrue(d["gee"].Equals(12));
        }

        [TestMethod]
        public void TestDiff13()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            var aa = new Dictionary<object, object>();
            aa.Add("gee", "eff");
            a.Add("bar", aa);

            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            var bb = new Dictionary<object, object>();
            bb.Add("gee", "eff");
            bb.Add("frr", "eff");
            b.Add("bar", bb);

            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            Assert.IsTrue(diff.ContainsKey("bar"));
            var d = diff["bar"] as Dictionary<object, object>;
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Count == 1);
            Assert.IsTrue(d.ContainsKey("frr"));
            Assert.IsTrue(d["frr"].Equals("eff"));
        }

        [TestMethod]
        public void TestDiff14()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            var aa = new Dictionary<object, object>();
            aa.Add("gee", "eff");
            a.Add("bar", aa);
            a.Add("num", new List<object>() { 1, 2, 3 });

            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            var bb = new Dictionary<object, object>();
            var bbb = new Dictionary<object, object>();
            bbb.Add("key", "val");
            bb.Add("gee", bbb);
            b.Add("bar", bb);
            b.Add("num", new List<object>() { 1, 2, 3 });

            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            Assert.IsTrue(diff.ContainsKey("bar"));
            var d = diff["bar"] as Dictionary<object, object>;
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Count == 1);
            Assert.IsTrue(d.ContainsKey("gee"));
        }

        [TestMethod]
        public void TestDiff15()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("num", new List<object>() { 1, 2, 3 });

            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            b.Add("num", new List<object>() { 1, 2, 3 });

            var _a = new List<object>() { 1, 2, a };
            var _b = new List<object>() { 1, 2, b };
            Assert.IsNull(JSON.JsonDiff.DiffList(_a, _b) as List<object>);
        }

        [TestMethod]
        public void TestDiff16()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("num", new List<object>() { 1, 2 });

            var b = new Dictionary<object, object>();
            b.Add("foo", 7);
            b.Add("num", new List<object>() { 1, 2, 3 });

            var _a = new List<object>() { 1, 2, a };
            var _b = new List<object>() { 1, 2, b };

            var diff = JSON.JsonDiff.DiffList(_a, _b) as List<object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            var d = diff[0] as Dictionary<object, object>;
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Count == 1);
            Assert.IsTrue(d.ContainsKey("num"));

            var l = d["num"] as List<object>;
            Assert.IsNotNull(l);
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals(3));
        }

        [TestMethod]
        public void TestDiff17()
        {
            var a = new List<object>();
            a.Add(1);
            a.Add(2);
            a.Add(3);
            a.Add(new List<object>() { 1, 2, 3 });

            var b = new List<object>();
            b.Add(1);
            b.Add(2);
            b.Add(3);
            b.Add(new List<object>() { 1, 2, 3 });
            Assert.IsNull(JSON.JsonDiff.DiffList(a, b));
        }

        [TestMethod]
        public void TestDiff18()
        {
            var a = new List<object>();
            a.Add(1);
            a.Add(2);
            a.Add(3);
            a.Add(new List<object>() { 1, 2, 3 });

            var b = new List<object>();
            b.Add(1);
            b.Add(2);
            b.Add(3);
            b.Add(new List<object>() { 1, 2, 7 });

            var diff = JSON.JsonDiff.DiffList(a, b) as List<object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            var l = diff[0] as List<object>;
            Assert.IsNotNull(l);
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals(7));
        }

        [TestMethod]
        public void TestDiff19()
        {
            var a = new List<object>();
            a.Add(1);
            a.Add(2);
            a.Add(3);
            a.Add(new List<object>() { 1, 2, 3 });

            var b = new List<object>();
            b.Add(1);
            b.Add(2);
            b.Add(3);
            b.Add(new List<object>() { 1, 2, 7 });

            var diff = JSON.JsonDiff.DiffList(a, b) as List<object>;
            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.Count == 1);
            var l = diff[0] as List<object>;
            Assert.IsNotNull(l);
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals(7));
        }

        [TestMethod]
        public void TestDiff20()
        {
            var a = new Dictionary<object, object>();
            a.Add("foo", 7);
            a.Add("bar", 7);
            var b = new Dictionary<object, object>();
            b.Add("bar", 7);
            var diff = JSON.JsonDiff.DiffDictionary(a, b) as Dictionary<object, object>;

            Assert.IsNotNull(diff);
            Assert.IsTrue(diff.ContainsKey("foo"));
            Assert.IsTrue(diff["foo"].Equals("undefined"));
        }

        [TestMethod]
        public void TestDiff21()
        {
            string s = @"{""b"": ""c"", ""d"": ""e""}";
            Assert.IsNull(JSON.JsonDiff.DiffStrings(s, s));
        }

        [TestMethod]
        public void TestDiff22()
        {
            string a = @"  {""b"": ""c"", ""d"": ""e""}  ";
            string b = @"{""b"": ""c"", ""d"": ""e""}";
            Assert.IsNull(JSON.JsonDiff.DiffStrings(a, b));
        }

        [TestMethod]
        public void TestDiff23()
        {
            string a = @"{""b"":""c"",   ""d"": ""e""}  ";
            string b = @"{""b"": ""c"",""d"": ""e""}";
            Assert.IsNull(JSON.JsonDiff.DiffStrings(a, b));
        }
    }
}
