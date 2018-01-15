namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System;
    using System.Data;
    public class DisconnectedDataReaderAccessor : IDataReaderAccessor {

        IDataReaderAccessor _dataAccessor;

        public DisconnectedDataReaderAccessor(IDataReaderAccessor dataAccessor) {
            _dataAccessor = dataAccessor;
        } 

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            var table = new DataTable();
            _dataAccessor.UseDataReader(sql, table.Load);
            callback(table.CreateDataReader());
        }
    }
}