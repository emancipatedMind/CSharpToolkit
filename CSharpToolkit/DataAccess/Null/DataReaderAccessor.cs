namespace CSharpToolkit.DataAccess.Null {
    using System;
    using System.Data;
    using Abstractions;
    public class DataReaderAccessor : IDataReaderAccessor {
        public void UseDataReader(string sql, Action<IDataReader> callback) {
            callback(new DataAccess.Error.DataReader()); 
        }
    }
}