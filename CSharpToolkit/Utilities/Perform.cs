namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class Perform {

        public static OperationResult ReplaceIfDifferent<T>(ref T oldValue, T newValue, T defaultValue = default(T)) {
            if (newValue == null)
                newValue = defaultValue;

            if (oldValue?.Equals(newValue) == true)
                return new OperationResult(false);

            oldValue = newValue;
            return new OperationResult(true);
        }

        public static T[][] ArraySlice<T>(T[] source, int maxResultElements) {
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

        public static OperationResult PropertyAssignmentThroughReflection(KeyValuePair<object, string> source, KeyValuePair<object, string> dest) {
            var sourcePropertyRetrievalOperation = Get.Property(source.Key, source.Value);
            var destPropertyRetrievalOperation = Get.Property(dest.Key, dest.Value);

            var exceptionList = new List<Exception>();
            exceptionList.AddRange(sourcePropertyRetrievalOperation.Exceptions);
            exceptionList.AddRange(destPropertyRetrievalOperation.Exceptions);
            if (exceptionList.Any())
                return new OperationResult(exceptionList);

            var (getObj, getProperty) = sourcePropertyRetrievalOperation.Result;
            var (setObj, setProperty) = destPropertyRetrievalOperation.Result;

            if (setProperty.PropertyType != getProperty.PropertyType)
                return new OperationResult(
                    new Exception[] { new ArgumentException("Types between properties are mismatched.") }
                );

            setProperty.SetValue(setObj, getProperty.GetValue(getObj, null));
            return new OperationResult(true);
        }


        public static List<int> BaseChange(int number, int newBase, bool isBigEndian = false) =>
            Get.General<List<int>>(digits => {

                while (number >= newBase) {
                    int digit = number % newBase;
                    AddDigit(digit);
                    number = number / newBase;
                }
                AddDigit(number);

                void AddDigit(int digit) {
                    if (isBigEndian) digits.Insert(0, digit);
                    else digits.Add(digit);
                }
            });
    }
}