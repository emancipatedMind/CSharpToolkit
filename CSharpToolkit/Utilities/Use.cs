﻿namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    public static class Use {

        public static string StringBuilder(Action<StringBuilder> action) =>
            Get.General(action).ToString();

        public static void List<T>(Action<List<T>> action) =>
            Get.General(action);

        public static void DisposableObject<T>(Action<T> method) where T : IDisposable, new() {
            using (var context = Activator.CreateInstance<T>())
                method(context);
        }

    }
}
