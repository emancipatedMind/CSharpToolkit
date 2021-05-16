namespace CSharpToolkit.Logging {
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using Utilities;

    public class NullExceptionLogger : IExceptionLogger {

        static NullExceptionLogger _instance;

        public static NullExceptionLogger Instance => _instance ?? (_instance = new NullExceptionLogger());

        private NullExceptionLogger() { }

        public OperationResult LogExceptions(params Exception[] exceptions) =>
            new OperationResult();

    }
}