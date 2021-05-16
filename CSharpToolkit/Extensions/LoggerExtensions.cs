namespace CSharpToolkit.Extensions {
    using System;
    using System.Threading.Tasks;
    using Utilities;
    using Logging.Abstractions;
    public static class LoggerExtensions {
        public static OperationResult LogTimeMetricsOf(this ILogger logger, Action action) {
            string format = "MM/dd/yyyy HH:mm:ss.fffffff";
            DateTime beginTime = DateTime.Now;
            action();
            DateTime finishTime = DateTime.Now;
            var messages = new string[] {
                $"Operation begin time : {beginTime.ToString(format) }",
                $"Operation end time : {finishTime.ToString(format) }",
                $"Operation Duration : {(finishTime - beginTime).TotalMilliseconds} ms.\r\n",
            };
            return logger.Log(string.Join("\r\n", messages));
        }

        public static OperationResult LogWithTime(this ILogger logger, object content, string prependageText = "", string dateTimeFormat = "MM/dd/yyyy HH:mm:ss.fff") =>
            logger.Log($"{prependageText}{DateTime.Now.ToString(dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)} : {content}");

    }
}