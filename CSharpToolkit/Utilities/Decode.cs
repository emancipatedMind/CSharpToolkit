namespace CSharpToolkit.Utilities {
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    public static class Decode {

        public static int? Int(string intString) {
            int n;
            if (int.TryParse(intString, out n))
                return n;
            return null;
        }

        public static OperationResult<string> SecureString(SecureString value) {
            IntPtr valuePtr = IntPtr.Zero;
            try {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                string plainSecureString = Marshal.PtrToStringUni(valuePtr);
                return new OperationResult<string>(plainSecureString);
            }
            catch (Exception ex) {
                return new OperationResult<string>(new Exception[] { ex });
            }
            finally {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private static string ExcelColumnName(int input) {
            var stringBuilder = new System.Text.StringBuilder();
            while (input > 0) {
                int modulo = (input - 1) % 26;
                stringBuilder.Insert(0, Convert.ToChar(65 + modulo));
                input = (input - modulo) / 26;
            }
            return stringBuilder.ToString();
        }

    }
}