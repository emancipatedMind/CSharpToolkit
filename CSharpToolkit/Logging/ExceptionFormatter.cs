namespace CSharpToolkit.Logging {
    using Logging.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities;
    /// <summary>
    /// Formats Exception for logging.
    /// </summary>
    public class ExceptionFormatter : IExceptionFormatter {
        /// <summary>
        /// Format exception into string.
        /// </summary>
        /// <param name="ex">Exception to format.</param>
        /// <returns>Exception formatted as string.</returns>
        public string FormatException(Exception ex) => string.Join("\r\n", Format(ex, 0));

        List<string> Format(Exception ex, int precedingSpaceCount) =>
            Get.List<string>(returnList =>
                Use.List<string>(list => {
                    list.Add($"Name : {ex.GetType().Name}");
                    list.Add($"Message : {ex.Message}");
                    list.Add($"Target Site : {ex.TargetSite}");
                    list.Add($"Source : {ex.Source}");
                    list.Add($"Stack Trace : {ex.StackTrace}");
                    if (ex.InnerException != null) {
                        list.Add("Inner Exception Found: ");
                        list.AddRange(Format(ex.InnerException, 2));
                    }
                    returnList.AddRange(list.Select(line => new string(' ', precedingSpaceCount) + line));
                }));

    }
}