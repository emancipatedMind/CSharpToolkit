namespace CSharpToolkitTester {
    using CSharpToolkit.DataAccess;
    using NUnit.Framework;
    using System.Linq;
    [TestFixture]
    public class ClauseShould {

        [Test]
        public void ReturnEmptyStringIfNoClauseAdded() {
            System.Enum
                .GetValues(typeof(ClauseType))
                .Cast<ClauseType>()
                .ToList()
                .ForEach(clauseType => Assert.AreEqual(string.Empty, Clause.New(clauseType).Build().Query));
        }

        [Test]
        public void NotSurroundSingleORClauseWithparanthesesAndShouldHaveAPrecedingWhiteSpace() {
            var query = Clause.New(ClauseType.OR).AddClause("OrderId = 187").Build().Query;
            Assert.AreEqual(" OrderId = 187", query);
        }

        [Test]
        public void SurroundMultipleORClauseWithparanthesesShouldHaveAPrecedingWhiteSpaceAndCombinedWithOR() {
            var query = Clause.New(ClauseType.OR).AddClause("OrderId = 187").AddClause("CustomerId = 985").Build().Query;
            Assert.AreEqual(" (OrderId = 187 OR CustomerId = 985)", query);
        }

        [Test]
        public void MultipleANDClauseHasNoParanthesesShouldHaveAPrecedingWhiteSpaceAndCombinedWithAND() {
            var query = Clause.New(ClauseType.AND).AddClause("OrderId = 187").AddClause("CustomerId = 985").Build().Query;
            Assert.AreEqual(" OrderId = 187 AND CustomerId = 985", query);
        }
    }
}