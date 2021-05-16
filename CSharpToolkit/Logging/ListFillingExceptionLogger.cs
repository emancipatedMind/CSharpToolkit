namespace CSharpToolkit.Logging {
    using System;
    using Abstractions;
    using Utilities;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ListFillingExceptionLogger : IExceptionLogger {

        readonly List<Exception> _list;

        public ListFillingExceptionLogger() : this (new List<Exception>()) { }

        public ListFillingExceptionLogger(List<Exception> list) {
            _list = list;
            List = _list.AsReadOnly();
        }

        public OperationResult LogExceptions(params Exception[] exceptions) {
            _list.AddRange(exceptions);
            return new OperationResult();
        }

        public ReadOnlyCollection<Exception> List { get; }

    }

}
