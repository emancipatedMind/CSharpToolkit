namespace CSharpToolkit.Abstractions.Logging {
    using System;
    public interface IExceptionFormatter {
        string FormatException(Exception ex); 
    }
}
