namespace CSharpToolkit.Logging {
    using System;
    using Abstractions.Logging;
    using CSharpToolkit.Utilities;
    public class ExceptionFormatter : IExceptionFormatter {
        public string FormatException(Exception ex) =>
            Use.StringBuilder(builder => {
                builder.AppendLine($"Name : {ex.GetType().Name}");
                builder.AppendLine($"Message : {ex.Message}");
                builder.AppendLine($"Target Site : {ex.TargetSite}");
                builder.AppendLine($"Source : {ex.Source}");
                builder.AppendLine($"Stack Trace : {ex.StackTrace}");
            });
    }
}
