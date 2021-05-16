namespace CSharpToolkit.DataAccess {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    public class MassUpdateModel<TRecord> : IEnumerable<TRecord> {

        IEnumerable<TRecord> _records;

        public MassUpdateModel(IEnumerable<TRecord> records, string userCode) : this(records, userCode, DateTime.Now) { } 
        public MassUpdateModel(IEnumerable<TRecord> records, string userCode, DateTime updateTime) {
            _records = records ?? Enumerable.Empty<TRecord>();
            UserCode = userCode;
            UpdateTime = updateTime;
        }

        public IEnumerable<TRecord> Records => RecordFilter != null ? _records.Where(RecordFilter) : _records;

        public string UserCode { get; }
        public DateTime UpdateTime { get; }
        public Func<TRecord, bool> RecordFilter { get; set; }

        public IEnumerator<TRecord> GetEnumerator() => Records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Records.GetEnumerator();
    }
}
