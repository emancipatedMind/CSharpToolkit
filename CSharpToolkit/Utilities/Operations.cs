namespace CSharpToolkit.Utilities {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
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

        public static OperationResult PerformReplaceIfDifferent<T>(ref T oldValue, T newValue, T defaultValue = default(T)) {
            if (oldValue == null)
                return new OperationResult(false);

            if (newValue == null) {
                if (defaultValue == null)
                    return new OperationResult(false);
                else
                    newValue = defaultValue;
            }

            if (oldValue.Equals(newValue))
                return new OperationResult(false);

            oldValue = newValue;
            return new OperationResult(true);
        }

        public static T[][] SliceArray<T>(T[] source, int maxResultElements) {
            int numberOfArrays = source.Length / maxResultElements;
            if (maxResultElements * numberOfArrays < source.Length)
                numberOfArrays++;
            T[][] target = new T[numberOfArrays][];
            for (int index = 0; index < numberOfArrays; index++) {
                int elementsInThisArray = Math.Min(maxResultElements, source.Length - index * maxResultElements);
                target[index] = new T[elementsInThisArray];
                Array.Copy(source, index * maxResultElements, target[index], 0, elementsInThisArray);
            }
            return target;
        }

        public static Action<string> GetWriteMethod(string fileName) =>
            GetWriteMethod(new FileInfo(fileName));

        public static Action<string> GetWriteMethod(FileInfo file) =>
            s => { using (var stream = file.AppendText()) stream.Write(s); };

    }
}