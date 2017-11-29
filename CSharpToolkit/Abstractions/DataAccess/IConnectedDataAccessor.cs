namespace CSharpToolkit.Abstractions.DataAccess {
    using System;
    using System.Data;
    public interface IConnectedDataAccessor {
        void UseConnection(Action<IDbConnection> callback);  
    }
}
