namespace CSharpToolkit.DataAccess {
    using System;
    using System.Data;
    using Abstractions.DataAccess;
    public class ThreadLockDataAccessor : IDataReaderAccessor {

        IDataReaderAccessor _component;
        object lockToken = new object();

        public ThreadLockDataAccessor(IDataReaderAccessor component) {
            _component = component;
        } 

        public void UseDataReader(string sql, Action<IDataReader> callback) {
            lock (lockToken)
                _component.UseDataReader(sql, callback);
        }
    }
}