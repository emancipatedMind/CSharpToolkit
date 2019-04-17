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
        public string FormatException(params Exception[] exceptions) => string.Join(_linkText, exceptions.SelectMany(ex => Format(ex, 0)));

        List<string> Format(Exception ex, int precedingSpaceCount) =>
            PrependSpaceCount(precedingSpaceCount, list => {
                list.Add($"Name : {ex.GetType().AssemblyQualifiedName}");
                list.AddRange(GetAllProperties(ex, prop => prop.Name != nameof(ex.InnerException) && prop.CanRead && prop.GetMethod.IsPublic, 2));
                if (ex.InnerException != null)
                    list.AddRange(Format(ex.InnerException, 2));
            });

        List<string> GetAllProperties(object obj, Func<PropertyInfo, bool> filter, int precedingSpaceCount) =>
            PrependSpaceCount(precedingSpaceCount, list => {
                foreach (PropertyInfo prop in obj.GetType().GetProperties().Where(filter)) {
                    object value = prop.GetValue(obj);
                    if (value == null)
                        return;
                    if (value is string || prop.PropertyType.IsSubclassOf(typeof(ValueType))) {
                        if (value.ToString().Length < 200) {
                            list.Add($"{prop.Name} : {System.Text.RegularExpressions.Regex.Replace(value.ToString(), @"(\s{2,})|([\r\n]{1,2})", " ")}");
                        }
                        else {
                            list.Add($"{prop.Name} :");
                            list.AddRange(value.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None).Select(str => "  " + str.Trim()));
                        }
                    }
                    else if (value is Exception)
                        FormatException((Exception)value);
                    else {
                        Type valueType = value.GetType();
                        list.Add($"{prop.Name} :");
                        if (value is IDictionary) {
                            var dictionary = (IDictionary)value;
                            foreach (DictionaryEntry entry in dictionary) {
                                if (entry.Value == null) {
                                    list.Add(new string(' ', 2) + $"{entry.Key} : NULL");
                                    continue;
                                } 
                                if (entry.Value is string || entry.Value.GetType().IsSubclassOf(typeof(ValueType))) {
                                    list.Add(new string(' ', 2) + $"{entry.Key} : {entry.Value}");
                                    continue;
                                }
                                list.Add(new string(' ', 2) + $"{entry.Key} :");
                                list.AddRange(GetAllProperties(entry.Value, 2));
                            }
                        }
                        else if (value is IEnumerable) {
                            var enumerator = ((IEnumerable)value).GetEnumerator();
                            while (enumerator.MoveNext()) {
                                list.Add(new string(' ', 2) + $"Name : {enumerator.Current.GetType().AssemblyQualifiedName}");
                                list.AddRange(GetAllProperties(enumerator.Current, 4));
                            }
                        }
                    };
                }
            });

        List<string> GetAllProperties(object obj, int precedingSpaceCount) =>
            GetAllProperties(obj, prop => prop.GetIndexParameters().Length == 0 && prop.CanRead && prop.GetMethod.IsPublic, precedingSpaceCount);

        List<string> PrependSpaceCount(int precedingSpaceCount, Action<List<string>> UseList) =>
            Get.List<string>(returnList =>
                Use.List<string>(list => {
                    UseList(list);
                    returnList.AddRange(list.Select(line => new string(' ', precedingSpaceCount) + line));
                }));
    }
}