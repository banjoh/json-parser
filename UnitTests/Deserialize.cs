using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Deserialize
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDeserialize1()
        {
            JSON.Converter.Deserialize("");
        }

        [TestMethod]
        public void TestDeserialize2()
        {
            Dictionary<object, object> obj = JSON.Converter.Deserialize(@"{""foo"": ""b ar""}") as Dictionary<object, object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContainsKey("foo"));
            Assert.IsTrue(obj["foo"].Equals("b ar"));
        }

        [TestMethod]
        public void TestDeserialize3()
        {
            Dictionary<object, object> obj = JSON.Converter.Deserialize(@"{""foo"": [{""bar"": ""baz""}, {""bee"": 7}]}") as Dictionary<object, object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContainsKey("foo"));
            var obj1 = obj["foo"] as List<object>;
            Assert.IsNotNull(obj1);
            Assert.IsTrue(obj1.Count == 2);
            var obj2 = obj1[0] as Dictionary<object, object>;
            Assert.IsNotNull(obj2);
            Assert.IsTrue(obj2.ContainsKey("bar"));
            Assert.IsTrue(obj2["bar"].Equals("baz"));
        }

        [TestMethod]
        public void TestDeserialize4()
        {
            Dictionary<object, object> obj = JSON.Converter.Deserialize(@"{""foo"": ""bar""}") as Dictionary<object, object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count == 1);
            Assert.IsTrue(obj.ContainsKey("foo"));
            Assert.IsTrue(obj["foo"].Equals("bar"));
        }

        [TestMethod]
        public void TestDeserialize5()
        {
            List<object> obj = JSON.Converter.Deserialize(@"[""foo"", 12354]") as List<object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count == 2);
            Assert.IsTrue(obj[0].Equals("foo"));
            Assert.IsTrue(obj[1].Equals(12354));
        }

        [TestMethod]
        public void TestDeserialize6()
        {
            List<object> obj = JSON.Converter.Deserialize(@"[9, 1]") as List<object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count == 2);
            Assert.IsTrue(obj[0].Equals(9));
            Assert.IsTrue(obj[1].Equals(1));
        }

        [TestMethod]
        public void TestDeserialize7()
        {
            List<object> obj = JSON.Converter.Deserialize(@"[9, [9, 1]]") as List<object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count == 2);
            Assert.IsTrue(obj[0].Equals(9));
            List<object> obj1 = obj[1] as List<object>;
            Assert.IsNotNull(obj1);
            Assert.IsTrue(obj1.Count == 2);
            Assert.IsTrue(obj1[0].Equals(9));
            Assert.IsTrue(obj1[1].Equals(1));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeserialize8()
        {
            JSON.Converter.Deserialize(@"{""foo"": ""bar""");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeserialize9()
        {
            JSON.Converter.Deserialize(@"""foo"": ""bar""");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeserialize10()
        {
            JSON.Converter.Deserialize("foo");
        }

        [TestMethod]
        public void TestDeserialize11()
        {
            var obj = JSON.Converter.Deserialize(@"{""a"": {""b"": ""c"", ""d"": ""e""}, ""f"": {""g"": ""h""}}") as Dictionary<object, object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContainsKey("a"));
            var d = obj["a"] as Dictionary<object, object>;
            Assert.IsNotNull(d);
            Assert.IsTrue(d.ContainsKey("b"));
            Assert.IsTrue("c".Equals(d["b"]));
            Assert.IsTrue(d.ContainsKey("d"));
            Assert.IsTrue("e".Equals(d["d"]));
            var d1 = obj["f"] as Dictionary<object, object>;
            Assert.IsNotNull(d1);
            Assert.IsTrue(d1.ContainsKey("g"));
            Assert.IsTrue("h".Equals(d1["g"]));
        }

        [TestMethod]
        public void TestDeserialize12()
        {
            var d = JSON.Converter.Deserialize(@"{""b"": ""c"", ""d"": ""e""}") as Dictionary<object, object>;

            Assert.IsNotNull(d);
            Assert.IsTrue(d.ContainsKey("b"));
            Assert.IsTrue("c".Equals(d["b"]));
            Assert.IsTrue(d.ContainsKey("d"));
            Assert.IsTrue("e".Equals(d["d"]));
        }

        [TestMethod]
        public void TestDeserialize13()
        {
            List<object> obj = JSON.Converter.Deserialize(@"[9, [9, 1], 7, 5, [2, 1], [3]]") as List<object>;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count == 6);
            Assert.IsTrue(obj[0].Equals(9));
            List<object> obj1 = obj[1] as List<object>;
            Assert.IsNotNull(obj1);
            Assert.IsTrue(obj1.Count == 2);
            Assert.IsTrue(obj1[0].Equals(9));
            Assert.IsTrue(obj1[1].Equals(1));
            Assert.IsTrue(obj[2].Equals(7));
            Assert.IsTrue(obj[3].Equals(5));
            List<object> obj2 = obj[4] as List<object>;
            Assert.IsNotNull(obj2);
            Assert.IsTrue(obj2.Count == 2);
            Assert.IsTrue(obj2[0].Equals(2));
            Assert.IsTrue(obj2[1].Equals(1));
            List<object> obj3 = obj[5] as List<object>;
            Assert.IsNotNull(obj3);
            Assert.IsTrue(obj3.Count == 1);
            Assert.IsTrue(obj3[0].Equals(3));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeserialize14()
        {
            // Duplicate keys in the same associative array not allowed
            JSON.Converter.Deserialize(@"{""b"": ""c"", ""b"": ""e""}");
        }

        [TestMethod]
        public void TestDeserialize15()
        {
            string first = @"{""foo"": {""bar"": ""baz"", ""biz"": ""foo""}, ""fiz"": {""foo"": ""baz""}, ""bar"": ""baz"", ""baz"": [""foo"", ""bar""], ""miss"": 123}";
            // Duplicate keys in the same associative array not allowed
            var o = JSON.Converter.Deserialize(first);
            Assert.IsNotNull(o);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeserialize16()
        {
            JSON.Converter.Deserialize(@"[""foo"", 1, 2,]");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeserialize17()
        {
            JSON.Converter.Deserialize(@"[""foo"", 1, ,2]");
        }
    }
}
