namespace CSharpToolkitTester {
    using CSharpToolkit.Utilities;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    [TestFixture]
    public class SelectQueryOrderShould {
        [Test]
        public void HaveConstructorThrowArgumentExceptionWhenTableNameIsEmpty() {
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder(null, "", new string[0]));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("", "", new string[0]));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder(" ", "", new string[0]));
        }

        [Test]
        public void NotHaveConstructorThrowExceptionWhenFilledInProperly() {
            Assert.DoesNotThrow(() => new SelectQueryOrder("Table", "", new string[0]));
            Assert.DoesNotThrow(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn", new KeyValuePair<string, string>("table", "column")));
        }

        [Test]
        public void HaveConstructorThrowArgumentExceptionWhenJoinColumnIsEmpty() {
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], null, new KeyValuePair<string, string>("table", "column")));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "", new KeyValuePair<string, string>("table", "column")));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], " ", new KeyValuePair<string, string>("table", "column")));
        }

        [Test]
        public void HaveConstructorThrowArgumentExceptionWhenForeignKeyKeyIsEmpty() {
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn" , new KeyValuePair<string, string>(null, "column")));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn", new KeyValuePair<string, string>("", "column")));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn", new KeyValuePair<string, string>(" ", "column")));
        }

        [Test]
        public void HaveSelectQueryConstructorThrowArgumentExceptionWhenForeignKeyValueIsEmpty() {
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn" , new KeyValuePair<string, string>("table" , null)));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn", new KeyValuePair<string, string>("table", "")));
            Assert.Throws<ArgumentException>(() => new SelectQueryOrder("Table", "", new string[0], "joinColumn", new KeyValuePair<string, string>("table", " ")));
        }

    }
}
