namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        public static Action<string> WriteMethod(string fileName) =>
            WriteMethod(new FileInfo(fileName));

        public static Action<string> WriteMethod(FileInfo file) =>
            s => { using (var stream = file.AppendText()) stream.Write(s); };


        public static List<T> List<T>(Action<List<T>> action) =>
            General(action);

        public static T General<T>(Action<T> action, object[] parameters = null) {
            var g = (T) Activator.CreateInstance(typeof(T), parameters);
            action(g);
            return g;
        }

        public static OperationResult OperationResult(Func<bool> operation) {
            try {
                return new OperationResult(operation());
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        public static OperationResult OperationResult(Action operation) {
            try {
                operation();
                return new OperationResult();
            }
            catch (Exception ex) {
                return new OperationResult(new Exception[] { ex });
            }
        }

        public static OperationResult<T> OperationResult<T>(Func<T> operation) {
            try {
                return new OperationResult<T>(operation());
            }
            catch (Exception ex) {
                return new OperationResult<T>(new Exception[] { ex });
            }
        }

    }
}
