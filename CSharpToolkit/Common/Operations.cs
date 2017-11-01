namespace CSharpToolkit.Common {
    using System;
    using System.IO;
    public static class Operations {
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
