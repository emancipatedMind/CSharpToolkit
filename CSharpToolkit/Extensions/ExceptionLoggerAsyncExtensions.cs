namespace CSharpToolkit.Extensions {
    using System;
    using System.Threading.Tasks;
    using Logging.Abstractions;
    using Utilities;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExceptionLoggerAsyncExtensions {

        public static async Task<OperationResult> LogExceptionsAsync(this IExceptionLogger logger, params Exception[] exceptions) =>
            await Task.Run(() => logger.LogExceptions(exceptions));

        public static Task<OperationResult<T>> ErrorLogIfOperationFaultedAsync<T>(this IExceptionLogger logger, Func<Type> declaringTypeCallback, string callingMethodName, Func<Task<OperationResult<T>>> method) =>
            ErrorLogIfOperationFaultedAsync(logger, declaringTypeCallback, callingMethodName, Enumerable.Empty<Tuple<string, object>>(), method);

        public static Task<OperationResult> ErrorLogIfOperationFaultedAsync(this IExceptionLogger logger, Func<Type> declaringTypeCallback, string callingMethodName, Func<Task<OperationResult>> method) =>
            ErrorLogIfOperationFaultedAsync(logger, declaringTypeCallback, callingMethodName, Enumerable.Empty<Tuple<string, object>>(), method);

        public static Task<OperationResult<T>> ErrorLogIfOperationFaultedAsync<T>(this IExceptionLogger logger, Func<Type> declaringTypeCallback, string callingMethodName, IEnumerable<Tuple<string, object>> infoToAttach, Func<Task<OperationResult<T>>> method) =>
            ErrorLogIfOperationFaultedAsync(logger, declaringTypeCallback, callingMethodName, () => Task.FromResult(infoToAttach), method);

        public static Task<OperationResult> ErrorLogIfOperationFaultedAsync(this IExceptionLogger logger, Func<Type> declaringTypeCallback, string callingMethodName, IEnumerable<Tuple<string, object>> infoToAttach, Func<Task<OperationResult>> method) =>
            ErrorLogIfOperationFaultedAsync(logger, declaringTypeCallback, callingMethodName, () => Task.FromResult(infoToAttach), method);

        public async static Task<OperationResult<T>> ErrorLogIfOperationFaultedAsync<T>(this IExceptionLogger logger, Func<Type> declaringTypeCallback, string callingMethodName, Func<Task<IEnumerable<Tuple<string, object>>>> infoToAttach, Func<Task<OperationResult<T>>> method) {
            OperationResult<T> result = await method();
            if (result.HadErrors) {
                var info = new List<Tuple<string, object>>();

                if (infoToAttach != null)
                    info.AddRange(await infoToAttach());

                Type declaringType = declaringTypeCallback();
                if (declaringType != null) {
                    info.AddRange(new[] {
                        Tuple.Create<string, object>("Calling.Class", declaringType.AssemblyQualifiedName),
                        Tuple.Create<string, object>("Calling.Method", callingMethodName),
                    });
                }

                if (info.Any())
                    ExceptionExtensions.AttachInfo(result.Exceptions, info);

                await logger.LogExceptionsAsync(result.Exceptions);
            }
            return result;
        }

        public async static Task<OperationResult> ErrorLogIfOperationFaultedAsync(this IExceptionLogger logger, Func<Type> declaringTypeCallback, string callingMethodName, Func<Task<IEnumerable<Tuple<string, object>>>> infoToAttach, Func<Task<OperationResult>> method) {
            OperationResult result = await method();
            if (result.HadErrors) {
                var info = new List<Tuple<string, object>>();

                if (infoToAttach != null)
                    info.AddRange(await infoToAttach());

                Type declaringType = declaringTypeCallback();
                if (declaringType != null) {
                    info.AddRange(new[] {
                        Tuple.Create<string, object>("Calling.Class", declaringType.AssemblyQualifiedName),
                        Tuple.Create<string, object>("Calling.Method", callingMethodName),
                    });
                }

                if (info.Any())
                    ExceptionExtensions.AttachInfo(result.Exceptions, info);

                await logger.LogExceptionsAsync(result.Exceptions);
            }
            return result;
        }

    }
}