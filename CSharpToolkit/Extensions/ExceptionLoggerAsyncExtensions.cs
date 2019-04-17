namespace CSharpToolkit.Extensions {
    using System;
    using System.Threading.Tasks;
    using Logging.Abstractions;
    using Utilities;
    //await ErrorLogger.LogExceptionsAsync(nonValidationExceptions.ToArray());
    public static class ExceptionLoggerAsyncExtensions {
        public static async Task<OperationResult> LogExceptionsAsync(this IExceptionLogger logger, params Exception[] exceptions) =>
            await Task.Run(() => logger.LogExceptions(exceptions));
    }
}