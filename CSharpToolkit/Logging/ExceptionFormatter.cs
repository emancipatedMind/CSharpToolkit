namespace CSharpToolkit.Logging {
    using Abstractions;
    using Extensions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities;
    using System.Reflection;
    /// <summary>
    /// Formats Exception for logging.
    /// </summary>
    public class ExceptionFormatter : IExceptionFormatter {

        string[] _exceptionNames = typeof(Exception).GetProperties().Select(prop => prop.Name).ToArray();
        string _linkText = "\r\n";
        PropertyInfo _dictionaryValueProp = typeof(DictionaryEntry).GetProperty(nameof(DictionaryEntry.Value));

        /// <summary>
        /// Instantiates an ExceptionFormatter whose link text is \r\n.
        /// </summary>
        public ExceptionFormatter() { }

        /// <summary>
        /// Instantiates an ExceptionFormatter.
        /// </summary>
        /// <param name="linkText">Determines text used to link exceptions.</param>
        public ExceptionFormatter(string linkText) {
            _linkText = linkText ?? _linkText;
        }

        /// <summary>
        /// Format exception into string.
        /// </summary>
        /// <param name="exceptions">Exceptions to format.</param>
        /// <returns>Exception formatted as string.</returns>
        public string FormatException(params Exception[] exceptions) => string.Join(_linkText, exceptions.SelectMany(ex => Get.ObjectStringifiedAsLines(ex)));

    }
}