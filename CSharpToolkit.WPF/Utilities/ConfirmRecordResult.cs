namespace CSharpToolkit.Utilities {
    using Utilities;
    using System.Collections.Generic;
    using System.Linq;

    public class ConfirmRecordResult {

        public ConfirmRecordResult() { }
        public ConfirmRecordResult(int id) : this(id, null) { }
        public ConfirmRecordResult(ConfirmRecordType confirmRecordType) : this(0, new[] { confirmRecordType }) { }
        public ConfirmRecordResult(IEnumerable<ConfirmRecordType> types) : this(0, types) { }
        public ConfirmRecordResult(int id, IEnumerable<ConfirmRecordType> types) {
            Id = id;
            Types.AddRange(types ?? new ConfirmRecordType[0]);
        }

        public int Id { get; }
        public List<ConfirmRecordType> Types { get; } = new List<ConfirmRecordType>();

    }
}
