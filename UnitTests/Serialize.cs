using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Serialize
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSerialize1()
        {
            JSON.Converter.Serialize(null);
        }

        [TestMethod]
        public void TestSerialize2()
        {
            var l = new List<object>() { "foo", "bar" };

            Assert.IsTrue(@"[""foo"", ""bar""]".Equals(JSON.Converter.Serialize(l)));
        }

        [TestMethod]
        public void TestSerialize3()
        {
            var d = new Dictionary<object, object>();
            d.Add("foo", "bar");

            Assert.IsTrue(@"{""foo"": ""bar""}".Equals(JSON.Converter.Serialize(d)));
        }

        [TestMethod]
        public void TestSerialize4()
        {
            var l = new List<object>() { 234, "bar" };

            Assert.IsTrue(@"[234, ""bar""]".Equals(JSON.Converter.Serialize(l)));
        }

        [TestMethod]
        public void TestSerialize5()
        {
            var d = new Dictionary<object, object>();
            d.Add("foo", 234);

            Assert.IsTrue(@"{""foo"": 234}".Equals(JSON.Converter.Serialize(d)));
        }

        [TestMethod]
        public void TestSerialize6()
        {
            var d = new Dictionary<object, object>();
            d.Add("foo", new List<object>() { "bar", 123 });

            Assert.IsTrue(@"{""foo"": [""bar"", 123]}".Equals(JSON.Converter.Serialize(d)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSerialize7()
        {
            JSON.Converter.Serialize(new object());
        }

        [TestMethod]
        public void TestSerialize8()
        {
            var d1 = new Dictionary<object, object>();
            d1.Add("foo", 234);
            var d2 = new Dictionary<object, object>();
            d2.Add("tee", 5);
            var d3 = new Dictionary<object, object>();
            d3.Add(68, 234);

            var l = new List<object>(){ d1, d2, d3 };

            Assert.IsTrue(@"[{""foo"": 234}, {""tee"": 5}, {68: 234}]".Equals(JSON.Converter.Serialize(l)));
        }

        [TestMethod]
        public void TestSerialize9()
        {
            var d1 = new Dictionary<object, object>();
            d1.Add("foo", 234);
            var d2 = new Dictionary<object, object>();
            d2.Add("tee", new List<object>() { "tee", 4, "free" });
            var d3 = new Dictionary<object, object>();
            d3.Add(68, 234);

            var l = new List<object>() { d1, d2, d3 };

            Assert.IsTrue(@"[{""foo"": 234}, {""tee"": [""tee"", 4, ""free""]}, {68: 234}]".Equals(JSON.Converter.Serialize(l)));
        }

        [TestMethod]
        public void TestSerialize10()
        {
            Assert.IsTrue(JSON.Converter.Serialize(new List<object>()).Equals("[]"));
        }

        [TestMethod]
        public void TestSerialize11()
        {
            var dict = new Dictionary<object, object>();
            Assert.IsTrue(JSON.Converter.Serialize(dict).Equals("{}"));
        }
    }
}
