namespace CSharpToolkit.Utilities {
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    /// <summary>
    /// A utility class used to decode certain data types.
    /// </summary>
    public static class Decode {

        /// <summary>
        /// Decodes string into Nullable&lt;int&gt;.
        /// </summary>
        /// <param name="intString">Input string to decode.</param>
        /// <returns>If int parse is successful, return int. If not, null.</returns>
        public static int? Int(string intString) {
            int n;
            if (int.TryParse(intString, out n))
                return n;
            return null;
        }

        /// <summary>
        /// Decodes System.Security.SecureString into string.
        /// </summary>
        /// <param name="value">System.Security.SecureString to decode.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult<string> SecureString(SecureString value) {
            IntPtr valuePtr = IntPtr.Zero;
            try {
                value = value ?? new SecureString(); 
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

        /// <summary>
        /// Decodes Excel column names.
        /// </summary>
        /// <param name="input">Column number to decode.</param>
        /// <returns>Column Name.</returns>
        public static string ExcelColumnName(int input) {
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