namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    public static class Get {

        public static OperationResult<T> EnumValue<T>(object v) where T : struct =>
            typeof(T).IsEnum ?
                new OperationResult<T>(Enum.IsDefined(typeof(T), v), (T)v) :
                new OperationResult<T>(new Exception[] { new NotSupportedException("Type must be System.Enum.") });

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
