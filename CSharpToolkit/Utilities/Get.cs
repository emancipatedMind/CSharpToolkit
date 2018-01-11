namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    /// <summary>
    /// Utility class which is used to obtain some kind of object.
    /// </summary>
    public static class Get {

        /*
        public static OperationResult<T> EnumValue<T>(object v) {
            try {
                if (Enum.IsDefined(typeof(T), v)) {
                    if (v.GetType() == typeof(string))
                        return new OperationResult<T>((T)Enum.Parse(typeof(T), v.ToString(), true));

                    return new OperationResult<T>((T)v);
                }
                return new OperationResult<T>(false, default(T));
            }
            catch (Exception ex) {
                return new OperationResult<T>(new[] { ex });
            }
        }
        */

        /// <summary>
        /// Gets callback method for writing out to file.
        /// </summary>
        /// <param name="fileName">File name to write out to.</param>
        /// <returns>Callback method for writing out to file.</returns>
        public static Action<string> WriteMethod(string fileName) =>
            WriteMethod(new FileInfo(fileName));

        /// <summary>
        /// Gets callback method for writing out to file.
        /// </summary>
        /// <param name="file">File to write out to.</param>
        /// <returns>Callback method for writing out to file.</returns>
        public static Action<string> WriteMethod(FileInfo file) =>
            s => { using (var stream = file.AppendText()) stream.Write(s); };


        /// <summary>
        /// Gets list of type T.
        /// </summary>
        /// <typeparam name="T">List type.</typeparam>
        /// <param name="action">Operation to fill list before returning.</param>
        /// <returns>Returns new list.</returns>
        public static List<T> List<T>(Action<List<T>> action) =>
            General(action);

        /// <summary>
        /// Gets generic object of type T.
        /// </summary>
        /// <typeparam name="T">object type.</typeparam>
        /// <param name="action">Operation to perform on object before returning.</param>
        /// <param name="parameters">Parameters to be passed to object of type T for creating.</param>
        /// <returns>Returns new object.</returns>
        public static T General<T>(Action<T> action, object[] parameters = null) {
            var g = (T) Activator.CreateInstance(typeof(T), parameters);
            action(g);
            return g;
        }

        /// <summary>
        /// Gets operation result. operation must return bool, and this denotes whether operation was successful or not. Any exceptions thrown are captured, and returned as OperationResult.Exceptions.
        /// </summary>
        /// <param name="operation">Operation to perform.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult OperationResult(Func<bool> operation) {
            try {
                return new OperationResult(operation());
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets operation result. This is used when operation just needs to complete without error to be deemed a success.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult OperationResult(Action operation) {
            try {
                operation();
                return new OperationResult();
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets operation result. Uses return value of operation which becomes available through OperationResult.Result.
        /// </summary>
        /// <param name="operation">Operation to complete.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult<T> OperationResult<T>(Func<T> operation) {
            try {
                return new OperationResult<T>(operation());
            }
            catch (Exception ex) {
                return new OperationResult<T>(new Exception[] { ex });
            }
        }

        /// <summary>
        /// Gets PropertyInfo of optionally embedded property.
        /// </summary>
        /// <param name="target">Target of query.</param>
        /// <param name="compoundProperty">Optionally embedded property info to get. If you have an object with a property, just specify that property like so "Name". If property has property, syntax is "Name.Key". Syntax may be repeated until property is discovered.</param>
        /// <returns>Operation result containing propertyinfo, and target object propertyinfo was found on.</returns>
        public static OperationResult<(object, PropertyInfo)> Property(object target, string compoundProperty) =>
            OperationResult(() => {
                object originalTarget = target;
                string[] bits = compoundProperty.Split('.');
                for (int i = 0; i < bits.Length - 1; i++) {
                    PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
                    target = propertyToGet.GetValue(target, null);
                }
                var propertyInfoToReturn = target.GetType().GetProperty(bits.Last(), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
                if (propertyInfoToReturn == null)
                    throw new ArgumentException($"On {originalTarget}, {compoundProperty} could not be found.");
                return (target, propertyInfoToReturn);
            });

    }
}
