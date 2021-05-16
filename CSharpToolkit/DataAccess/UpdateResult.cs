namespace CSharpToolkit.DataAccess {
    public class UpdateResult<TKey> {

        public UpdateResult(TKey key, int? returnCode = null, string returnMessage = null) {
            Key = key;
            ReturnCode = returnCode ?? 0;
            ReturnMessage = returnMessage;
        }

        public TKey Key { get; }
        public int ReturnCode { get; }
        public string ReturnMessage { get; }

    }
}
