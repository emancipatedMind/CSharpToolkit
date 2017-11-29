namespace CSharpToolkit.DataAccess {
    using System;
    using System.Data;
    using Abstractions.DataAccess;
    using Abstractions.Logging;
    public class LoggingDataAccessorDecorator : IDataReaderAccessor {

        ITimeStampLogger _logger;
        string _separationString = " : " ;
        IDataReaderAccessor _component;

        public LoggingDataAccessorDecorator(IDataReaderAccessor component, ITimeStampLogger logger) {
            _logger = logger;
            _component = component;
        }

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            DateTime beginTime = LogWithCurrentTime("Operation has begun.\r\n", false);
            _logger.Log(sql + "\r\n");
            _component.UseDataReader(sql, callback);
            DateTime finishTime = LogWithCurrentTime("Operation Done.\r\n", false);
            LogWithCurrentTime($"Operation Duration : {(finishTime - beginTime).TotalMilliseconds} ms.\r\n", false);
            _logger.Log("\r\n");
        }

        DateTime LogWithCurrentTime(string content, bool prependNewLine) =>
            _logger.LogWithCurrentTime(content, _separationString, prependNewLine);

    }
}