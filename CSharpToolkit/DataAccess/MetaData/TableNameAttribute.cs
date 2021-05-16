namespace CSharpToolkit.DataAccess.MetaData {
    [System.AttributeUsage(System.AttributeTargets.Interface | System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TableNameAttribute : System.Attribute {

        public TableNameAttribute(string tableName) {
            TableName = tableName;
        }

        public string TableName { get; }

    }
}
