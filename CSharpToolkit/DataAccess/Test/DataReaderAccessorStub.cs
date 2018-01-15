namespace CSharpToolkit.DataAccess.Test {
    using System;
    using System.Data;
    public class DataReaderAccessorStub : Abstractions.IDataReaderAccessor {

        IDataReader _reader;

        public DataReaderAccessorStub(IDataReader reader) {
            _reader = reader;
        }

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            callback(_reader);
        }
    }
}