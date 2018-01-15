namespace CSharpToolkit.DataAccess.Abstractions {
    using System;
    using System.Data;
    public interface IDataReaderAccessor {
        void UseDataReader(string sql, Action<IDataReader> callback); 
    }
}
