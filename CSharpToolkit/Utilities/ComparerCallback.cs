namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;

    public class ComparerCallback<TParameter> : Comparer<TParameter> {
            Comparison<TParameter> _callback;
            public ComparerCallback(Comparison<TParameter> callback) {
                _callback = callback;
            }

            public override int Compare(TParameter x, TParameter y) => _callback(x, y);
        }

}
