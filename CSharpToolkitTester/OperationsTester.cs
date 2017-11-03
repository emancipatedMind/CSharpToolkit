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

            var result = Operations.DecodeSecureString(secureString);
            string actual = result.Result;

            Assert.IsTrue(result.WasSuccessful);
            Assert.IsFalse(result.HadErrors);
            Assert.AreEqual(expected, actual);
        }
    }
}
