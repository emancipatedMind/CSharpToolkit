namespace CSharpToolkitTester {
    using System.Collections.Generic;
    using System;
    using NUnit.Framework;
    [TestFixture]
    public class SQLInjectionValidatorShould {
        static CSharpToolkit.Validation.SQLInjectionValidator _validator = new CSharpToolkit.Validation.SQLInjectionValidator();

        [Test]
        public void FailIfContainsCommentStarter() {
            var order = "--1st";
            TestFailure(new string[] { order }, false);
            TestFailure(new string[] { order }, true);
        }

        [Test]
        public void FailIfContainsSemiColon() {
            var order = ";1st";
            TestFailure(new string[] { order }, false);
            TestFailure(new string[] { order }, true);
        }

        [Test]
        public void FailIfContainsNonAlphaNumericCharacter() {
            var order = "'; Select * from sysobjects;--";
            TestFailure(new string[] { order }, false);
            TestFailure(new string[] { order }, true);
        }

        [Test]
        public void FailIfContainsSingleQuotesAndSingleQuotesDisallowed() {
            var order = "'1st'";
            TestFailure(new string[] { order }, false);
        }

        [Test]
        public void FailIfContainsOddNumberOfQuotesAndSingleQuotesDisallowed() {
            var order = "1st'''";
            TestFailure(new string[] { order }, false);
        }

        [Test]
        public void FailIfFailureAmongSuccesses() {
            var parameters = new string[] {
                "",
                "--1st",
                "1st",
                "Anchors",
                "123456789",
            };
            TestFailure(parameters, false);
            TestFailure(parameters, true);
        }

        [Test]
        public void PassIfEmptyParameterSet() {
            TestPass(new string[0], false);
            TestPass(new string[0], true);
        }

        [Test]
        public void PassIfContainsEmptyString() {
            TestPass(new string[] { "" }, false);
            TestPass(new string[] { "" }, true);
        }

        [Test]
        public void PassIfContainsEscapedQuotes() {
            TestPass(new string[] { "''1st''" }, false);
            TestPass(new string[] { "''1st''" }, true);
        }

        [Test]
        public void PassIfContainsOnlyNumericCharacters() {
            TestPass(new string[] { "1234567489" }, false);
            TestPass(new string[] { "1234567489" }, true);
        }

        [Test]
        public void PassIfContainsOnlyAlphaCharacters() {
            TestPass(new string[] { "Anchors" }, false);
            TestPass(new string[] { "Anchors" }, true);
        }

        [Test]
        public void PassIfContainsMixOfAlphaNumericCharacters() {
            TestPass(new string[] { "1st" }, false);
            TestPass(new string[] { "1st" }, true);
        }

        [Test]
        public void PassIfContainsSingleQuotesIfRequested() {
            TestPass(new string[] { "'1st'" }, true);
        }

        [Test]
        public void PassIfContainsMultiplePassingParametrs() {
            var parameters = new string[] {
                "",
                "''1st''",
                "1st",
                "Anchors",
                "123456789",
            };
            TestPass(parameters);
        }

        void TestPass(IEnumerable<string> parameters, bool allowSingleQuotes = false) =>
            RunTestOnValidator(parameters, Assert.IsTrue, Assert.IsFalse, allowSingleQuotes);

        void TestFailure(IEnumerable<string> parameters, bool allowSingleQuotes = false) =>
            RunTestOnValidator(parameters, Assert.IsFalse, Assert.IsTrue, allowSingleQuotes);

        void RunTestOnValidator(IEnumerable<string> parameters, Action<bool> WasSuccessfulWrapper, Action<bool> HadErrorsWrapper, bool allowSingleQuotes) {
            _validator.AllowSingleQuotes = allowSingleQuotes;
            var result = _validator.Validate(parameters);
            WasSuccessfulWrapper(result.WasSuccessful);
            HadErrorsWrapper(result.HadErrors);
        }

    }
}