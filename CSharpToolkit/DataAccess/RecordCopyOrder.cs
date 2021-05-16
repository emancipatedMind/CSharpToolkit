namespace CSharpToolkit.DataAccess {
    using System.Collections.Generic;
    using System.Linq;
    public class RecordCopyOrder<TModelImplementation> {
        public RecordCopyOrder(IEnumerable<int> transferKeys, TModelImplementation defaultModel, IEnumerable<string> defaultFieldsToCopy, string parentIdName) {
            TransferKeys = transferKeys?.ToArray() ?? new int[0];
            DefaultFieldsToCopy = defaultFieldsToCopy?.ToArray() ?? new string[0];
            DefaultModel = defaultModel;
            ParentIdName = parentIdName;
        }

        public int[] TransferKeys { get; }
        public string[] DefaultFieldsToCopy { get; }
        public string ParentIdName { get; }
        public TModelImplementation DefaultModel { get; }
    }

}
