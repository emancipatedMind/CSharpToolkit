namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class Perform {

        /// <summary>
        /// Performs a replace if the new, and old value are different. If new value is null, default value is consulted. If it is also null, this is what will be assigned.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value for variable.</param>
        /// <param name="defaultValue">Default value for use if newValue is null.</param>
        /// <returns>Operation result denoting whether value was updated.</returns>
        public static OperationResult ReplaceIfDifferent<T>(ref T oldValue, T newValue, T defaultValue = default(T)) {
            if (newValue == null)
                newValue = defaultValue;

            if (oldValue?.Equals(newValue) == true)
                return new OperationResult(false);

            oldValue = newValue;
            return new OperationResult(true);
        }

        /// <summary>
        /// Slices array into pieces delimited by length.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="source">Source array.</param>
        /// <param name="maxResultElements">Max elements before slice.</param>
        /// <returns>Two dimensional sliced array.</returns>
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

        /// <summary>
        /// Uses to reflection to transfer the value of one property to another.
        /// </summary>
        /// <param name="source">Source object, and property name.</param>
        /// <param name="dest">Destination object, and property name.</param>
        /// <returns>Operation result.</returns>
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

        /// <summary>
        /// Change base of number.
        /// </summary>
        /// <param name="number">Number for which base should be changed.</param>
        /// <param name="newBase">New base.</param>
        /// <param name="isBigEndian">Whether result should be bigendian for littleendian.</param>
        /// <returns>New number in list form.</returns>
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