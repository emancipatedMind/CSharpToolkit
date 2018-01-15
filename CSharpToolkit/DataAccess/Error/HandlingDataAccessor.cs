namespace CSharpToolkit.DataAccess.Error {
    using System;
    using System.Data;
    using Abstractions;
    using Logging.Abstractions;
    public class HandlingDataAccessor : IDataReaderAccessor, ILoggable {

        IDataReaderAccessor _component;
        IExceptionLogger _errorLogger;
        ILogger _standardLogger;

        public HandlingDataAccessor(IDataReaderAccessor component, IExceptionLogger errorLogger) : this(component, errorLogger, null) { }
        public HandlingDataAccessor(IDataReaderAccessor component, IExceptionLogger errorLogger, ILogger standardLogger) {
            _component = component;
            _errorLogger = errorLogger;
            _standardLogger = standardLogger;
        }

        public ILogger Logger { set { _standardLogger = value; } }

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            try {
                _component.UseDataReader(sql, callback);
            }
            catch(Exception ex) {
                string fileName = $"Exception{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                _standardLogger?.Log($"Exception thrown while performing operation. Check {fileName} for details.\r\n");
                _errorLogger.FileName = fileName;
                _errorLogger.LogException(ex);
                callback(new DataReader());
            }
        }

    }
}