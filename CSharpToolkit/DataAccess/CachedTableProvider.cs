namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System.Data;

    public class CachedTableProvider : ICachedTableProvider {

        public CachedTableProvider(string selectString, IDataReaderAccessor dataAccessor) {
            Table = new DataTable();
            dataAccessor.UseDataReader(selectString, Table.Load);
        }

        public DataTable Table { get; }
    }
}