namespace CSharpToolkit.DataAccess {
    using System;
    using System.Data;
    using Abstractions;
    using Logging.Abstractions;
    public class LoggingDataAccessor : IDataReaderAccessor {

        IDurationLogger _logger;
        string _separationString;
        IDataReaderAccessor _component;

        public LoggingDataAccessor(IDataReaderAccessor component, IDurationLogger logger) : this(component, logger, " : ") { }
        public LoggingDataAccessor(IDataReaderAccessor component, IDurationLogger logger, string separationString) {
            _separationString = separationString;
            _logger = logger;
            _component = component;
        }

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            _logger.LogTimeMetricsOf(() => {
                _logger.Log(sql);
                _component.UseDataReader(sql, callback);
            });
        }

    }
}