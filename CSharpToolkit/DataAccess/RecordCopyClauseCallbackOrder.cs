namespace CSharpToolkit.DataAccess {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class RecordCopyClauseCallbackOrder {

        public RecordCopyClauseCallbackOrder(IEnumerable<Tuple<string, object>> keys, IEnumerable<Tuple<string, string>> fieldParameterNameTupleCollection) {
            Keys = keys?.ToArray() ?? new Tuple<string, object>[0];
            FieldParameterNameTupleCollection = fieldParameterNameTupleCollection?.ToArray() ?? new Tuple<string, string>[0];
        }

        public Tuple<string, object>[] Keys { get; }
        public Tuple<string, string>[] FieldParameterNameTupleCollection { get; }

    }
}