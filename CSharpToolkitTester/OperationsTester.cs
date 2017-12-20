namespace CSharpToolkitTester {
    using NUnit.Framework;
    using System.Security;
    using System.Linq;
    using CSharpToolkit.Utilities;
    [TestFixture]
    public class OperationsTester {
        [Test]
        public void DecodeSecureStringShouldMatchStringPassedIn() {
            string expected = "123abczyx";
            SecureString secureString = new SecureString();

            expected
                .ToList()
                .ForEach(c => secureString.AppendChar(c));

            var result = Decode.SecureString(secureString);
            string actual = result.Result;

            Assert.IsTrue(result.WasSuccessful);
            Assert.IsFalse(result.HadErrors);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsTrueIfValuesAreDifferentForValueType() {
            int oldValue = 5;
            int newValue = 6;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.IsTrue(result.WasSuccessful);
        }
        [Test]
        public void PerformReplaceIfDifferentShouldReplaceIfValueTypesDifferent() {
            int oldValue = 5;
            int expected = 6;
            int newValue = expected;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.AreEqual(expected, oldValue);
        }
        [Test]
        public void PerformReplaceIfDifferentShouldReplaceIfReferenceTypesAreDifferentInstances() {
            object oldValue = new object();
            object expected = new object();
            object newValue = expected;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.AreSame(expected, oldValue);
        }
        [Test]
        public void PerformReplaceIfDifferentShouldLeaveVariableWithValueIfValueTypeAndSame() {
            int oldValue = 5;
            int expected = 5;
            int newValue = expected;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.AreEqual(expected, oldValue);
        }
        [Test]
        public void PerformReplaceIfDifferentShouldLeaveVariableWithObjectIfReferenceTypeAndSame() {
            object expected = new object();
            object oldValue = expected;
            object newValue = expected;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.AreSame(expected, oldValue);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsTrueIfReferenceTypesAreDifferentInstances() {
            object oldValue = new object();
            object newValue = new object();

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.IsTrue(result.WasSuccessful);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsFalseIfOldValueAndDefaultAreSameAndNewValueIsNull() {
            object oldValue = new object();
            object defaultValue = oldValue;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, null, defaultValue);

            Assert.IsFalse(result.WasSuccessful);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsTrueIfOldValueAndDefaultAreDifferentAndNewValueIsNull() {
            object oldValue = new object();
            object defaultValue = new object();

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, null, defaultValue);

            Assert.IsTrue(result.WasSuccessful);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsFalseIfValuesAreSameForValueType() {
            int oldValue = 5;
            int newValue = 5;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.IsFalse(result.WasSuccessful);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsFalseIfReferenceTypesAreSameInstance() {
            object oldValue = new object();
            object newValue = oldValue;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);

            Assert.IsFalse(result.WasSuccessful);
        }
        [Test]
        public void PerformReplaceIfDifferentReturnsFalseIfNewValueAndDefaultValueAreNull() {
            object oldValue = new object();
            object newValue = null;

            OperationResult result = Perform.ReplaceIfDifferent(ref oldValue, newValue);
            Assert.IsFalse(result.WasSuccessful);

            result = Perform.ReplaceIfDifferent(ref oldValue, newValue, null);
            Assert.IsFalse(result.WasSuccessful);
        }
    }
}