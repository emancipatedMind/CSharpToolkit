namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    using Logging;
    using Logging.Abstractions;

    /// <summary>
    /// Utility classes which provides throwaway objects.
    /// </summary>
    public static class Use {

        /// <summary>
        /// Provides string builder for use to produce string.
        /// </summary>
        /// <param name="action">Operation which uses the string builder.</param>
        /// <returns>Task containing built string.</returns>
        public static async Task<string> StringBuilderAsync(Func<StringBuilder, Task> action) {
            var builder = new System.Text.StringBuilder();
            await action(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Provides string builder for use to produce string.
        /// </summary>
        /// <param name="action">Operation which uses the string builder.</param>
        /// <returns>Task containing built string.</returns>
        public static Task<string> StringBuilderAsync(Action<StringBuilder> action) {
            var builder = new System.Text.StringBuilder();
            action(builder);
            return Task.FromResult(builder.ToString());
        }

        /// <summary>
        /// Provides string builder for use to produce string.
        /// </summary>
        /// <param name="action">Operation which uses the string builder.</param>
        /// <returns>Built string.</returns>
        public static string StringBuilder(Action<StringBuilder> action) {
            var builder = new System.Text.StringBuilder();
            action(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Provides throwaway list for use.
        /// </summary>
        /// <typeparam name="T">List type.</typeparam>
        /// <param name="action">Operation which needs throwaway list.</param>
        public static void List<T>(Action<List<T>> action) =>
            Get.General(action);

        /// <summary>
        /// Provides scope for Disposable object. Object is disposed once method is exited.
        /// </summary>
        /// <typeparam name="T">Object type. Must implement IDisposable.</typeparam>
        /// <param name="method">Operation for disposable object.</param>
        public static void DisposableObject<T>(Action<T> method) where T : IDisposable, new() {
            using (var context = Activator.CreateInstance<T>())
                method(context);
        }

        public static void GenericLogger(string fileName, Action<ILogger> callback) =>
            GenericLogger(fileName, true, callback);

        public static void GenericLogger(string fileName, bool openFile, Action<ILogger> callback) {
            var logger = new Logger(fileName);
            try {
                callback(logger);
            }
            catch (Exception ex) {

                var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERRORS");

                var directoryInfo = new DirectoryInfo(directory);

                bool faulted = false;
                if (directoryInfo.Exists == false) {
                    try {
                        directoryInfo.Create();
                    }
                    catch {
                        faulted = true;
                        // Any exception encountered here cannot be logged. All options have been exhausted.
                    }
                }
                if (faulted)
                    throw;

                string errorFilename = Path.Combine(directory, $"ErrorFile_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                new Logger(errorFilename).Log(new ExceptionFormatter().FormatException(ex));

                System.Diagnostics.Process.Start("notepad", errorFilename);

                throw;
            }

            if (System.IO.File.Exists(fileName) && openFile)
                System.Diagnostics.Process.Start("notepad", fileName);

        }

        public static Task GenericLoggerAsync(string fileName, Func<ILogger, Task> callback) =>
            GenericLoggerAsync(fileName, true, callback);


        public static async Task GenericLoggerAsync(string fileName, bool openFile, Func<ILogger, Task> callback) {
            var logger = new Logger(fileName);
            try {
                await callback(logger);
            }
            catch (Exception ex) {

                var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERRORS");

                var directoryInfo = new DirectoryInfo(directory);

                bool faulted = false;
                if (directoryInfo.Exists == false) {
                    try {
                        directoryInfo.Create();
                    }
                    catch {
                        faulted = true;
                        // Any exception encountered here cannot be logged. All options have been exhausted.
                    }
                }
                if (faulted)
                    throw;

                string errorFilename = Path.Combine(directory, $"ErrorFile_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                new Logger(errorFilename).Log(new ExceptionFormatter().FormatException(ex));

                System.Diagnostics.Process.Start("notepad", errorFilename);

                throw;
            }

            if (System.IO.File.Exists(fileName) && openFile)
                System.Diagnostics.Process.Start("notepad", fileName);

        }


    }
}
