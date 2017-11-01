namespace CSharpToolkit.Diagnostics {
    using System;
    using System.Linq;
    using System.Reflection;
    public class Time {
        public static (T instance, TimeSpan duration) Constructor<T>(object[] parameters, int average = 5) =>
            (
                (T)Activator.CreateInstance(typeof(T), parameters),
                new TimeSpan(
                    Convert.ToInt64(
                        Enumerable.Range(0, average)
                        .Select(x => {
                            var timer = new System.Diagnostics.Stopwatch();
                            timer.Start();
                            var instance = (T)Activator.CreateInstance(typeof(T), parameters);
                            timer.Stop();
                            return timer.Elapsed.Ticks;
                        })
                        .Average()
                    )
                )
            );

        public static TimeSpan Method(object instance, string methodName, object[] parameters, int average = 5) =>
            new TimeSpan(
                Convert.ToInt64(
                    Enumerable.Range(0, average)
                    .Select(x => {
                        var timer = new System.Diagnostics.Stopwatch();
                        MethodInfo mi = instance.GetType().GetMethod(methodName);
                        timer.Start();
                        mi.Invoke(instance, parameters);
                        timer.Stop();
                        return timer.Elapsed.Ticks;
                    })
                    .Average()
                )
            );
    }
}
