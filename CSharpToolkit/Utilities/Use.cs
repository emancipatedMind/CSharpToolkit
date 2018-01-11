namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    /// <summary>
    /// Utility classes which provides throwaway objects.
    /// </summary>
    public static class Use {

        /// <summary>
        /// Provides string builder for use to produce string.
        /// </summary>
        /// <param name="action">Operation which uses the string builder.</param>
        /// <returns>Built string.</returns>
        public static string StringBuilder(Action<StringBuilder> action) =>
            Get.General(action).ToString();

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

    }
}
