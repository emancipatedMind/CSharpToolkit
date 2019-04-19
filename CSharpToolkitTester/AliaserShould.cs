namespace CSharpToolkitTester {
    using CSharpToolkit.Utilities;
    using NUnit.Framework;
    using System;
    using System.Linq;
    [TestFixture]
    public class AliaserShould {

        [Test]
        public void NotThrowExceptionIfAllNamesAndAliasesAreUnique() {

            var aliaser = new Aliaser();
            var firstAliasSet = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            aliaser.AddAlias(firstAliasSet);

            var secondAliasSet = new Tuple<string, string>[] {
                Tuple.Create("Item4", "Alias4"),
                Tuple.Create("Item5", "Alias5"),
                Tuple.Create("Item6", "Alias6"),
            };
            Assert.DoesNotThrow(() => aliaser.AddAlias(secondAliasSet));
        }

        [Test]
        public void ThrowArgumentExceptionIfNameIsRepeated() {
            var hydratedAliaser = new Aliaser();
            var emptyAliaser = new Aliaser();
            var uniqueList = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            hydratedAliaser.AddAlias(uniqueList);
            var repeatList = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias4"),
            };

            Assert.Throws<ArgumentException>(() => hydratedAliaser.AddAlias(repeatList));
            Assert.Throws<ArgumentException>(() => emptyAliaser.AddAlias(uniqueList.Concat(repeatList)));
        }

        [Test]
        public void ThrowArgumentExceptionIfAliasIsRepeated() {
            var hydratedAliaser = new Aliaser();
            var emptyAliaser = new Aliaser();
            var uniqueList = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            hydratedAliaser.AddAlias(uniqueList);
            var repeatList = new Tuple<string, string>[] {
                Tuple.Create("Item4", "Alias1"),
            };

            Assert.Throws<ArgumentException>(() => hydratedAliaser.AddAlias(repeatList));
            Assert.Throws<ArgumentException>(() => emptyAliaser.AddAlias(uniqueList.Concat(repeatList)));
        }


        [Test]
        public void AllowRepeatsToBeAddedAfterClear() {
            var aliaser = new Aliaser();
            var aliases = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            Assert.DoesNotThrow(() => aliaser.AddAlias(aliases));
            Assert.Throws<ArgumentException>(() => aliaser.AddAlias(aliases));

            aliaser.Clear();
            Assert.DoesNotThrow(() => aliaser.AddAlias(aliases));
        }

        [Test]
        public void ReturnPairingAliasFromNameUsingLookUpAlias() {
            var aliaser = new Aliaser();
            var aliases = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            aliaser.AddAlias(aliases);

            foreach (var aliasSet in aliases) {
                string name = aliasSet.Item1;
                string alias = aliasSet.Item2;
                Assert.AreEqual(alias, aliaser.LookUpAlias(name));
            }

        }

        [Test]
        public void ReturnPairingNameFromAliasUsingLookUpName() {
            var aliaser = new Aliaser();
            var aliases = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            aliaser.AddAlias(aliases);

            foreach (var aliasSet in aliases) {
                string name = aliasSet.Item1;
                string alias = aliasSet.Item2;
                Assert.AreEqual(name, aliaser.LookUpName(alias));
            }
        }

        [Test]
        public void ReturnInputIfNameNorAliasIsFound() {
            var aliaser = new Aliaser();
            var aliases = new Tuple<string, string>[] {
                Tuple.Create("Item1", "Alias1"),
                Tuple.Create("Item2", "Alias2"),
                Tuple.Create("Item3", "Alias3"),
            };
            aliaser.AddAlias(aliases);

            string name = "Item4";
            string alias = "Alias4";
            Assert.AreEqual(alias, aliaser.LookUpName(alias));
            Assert.AreEqual(name, aliaser.LookUpAlias(name));
        }

        [Test]
        public void ReturnInputDenotingAliaserWasCleared() {
            var aliaser = new Aliaser();

            string name = "Item1";
            string alias = "Alias1";
            aliaser.AddAlias(name, alias);
            Assert.AreEqual(name, aliaser.LookUpName(alias));
            Assert.AreEqual(alias, aliaser.LookUpAlias(name));
            aliaser.Clear();

            Assert.AreEqual(alias, aliaser.LookUpName(alias));
            Assert.AreEqual(name, aliaser.LookUpAlias(name));
        }
    }
}