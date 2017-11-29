namespace CSharpToolkit.DataAccess {
    using System;
    using System.Data;
    using Abstractions.DataAccess;
    using Abstractions.Logging;
    public class LoggingDataAccessorDecorator : IDataReaderAccessor {

        IDurationLogger _logger;
        string _separationString = " : " ;
        IDataReaderAccessor _component;

        public LoggingDataAccessorDecorator(IDataReaderAccessor component, IDurationLogger logger) {
            _logger = logger;
            _component = component;
        }

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            _logger.LogTimeMetricsOf(() => _component.UseDataReader(sql, callback));
        }

    }
}