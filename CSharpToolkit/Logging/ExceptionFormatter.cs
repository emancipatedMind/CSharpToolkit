namespace CSharpToolkit.Logging {
    using Abstractions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities;
    public class ExceptionFormatter : IExceptionFormatter {
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