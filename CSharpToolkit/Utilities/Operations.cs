namespace CSharpToolkit.Utilities {
    using Common;
    using System;
    using System.Security;
    using System.Runtime.InteropServices;
    public static class Operations {
        public static OperationResult<string> DecodeSecureString(SecureString value) {
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
    }
}