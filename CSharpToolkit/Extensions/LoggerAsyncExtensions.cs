namespace CSharpToolkit.Extensions {
    using System;
    using System.Threading.Tasks;
    using Utilities;
    using Logging.Abstractions;
    public static class LoggerAsyncExtensions {
        public static async Task<OperationResult> LogTimeMetricsOfAsync(this ILogger logger, Action action) =>
            await Task.Run(() => logger.LogTimeMetricsOf(action));

        public static async Task<OperationResult> LogWithTimeAsync(this ILogger logger, object content, string prependageText = "", string dateTimeFormat = "MM/dd/yyyy HH:mm:ss.fff") =>
            await Task.Run(() => logger.LogWithTimeAsync(content, prependageText, dateTimeFormat));
    }
}