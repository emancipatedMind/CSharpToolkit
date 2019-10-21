namespace CSharpToolkit.Utilities {
    using Extensions;
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text;
    using System.Net.Sockets;
    using System.Security;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A utility used to perform some operations.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class Perform {

        public static bool SecureStringCompare(SecureString first, SecureString second) {
            if (first == null || second == null) {
                return false;
            }

            if (first.Length != second.Length) {
                return false;
            }

            try {
                IntPtr ss_bstr1_ptr = IntPtr.Zero;
                IntPtr ss_bstr2_ptr = IntPtr.Zero;
                try {
                    ss_bstr1_ptr = Marshal.SecureStringToBSTR(first);
                    ss_bstr2_ptr = Marshal.SecureStringToBSTR(second);

                    string str1 = Marshal.PtrToStringBSTR(ss_bstr1_ptr);
                    string str2 = Marshal.PtrToStringBSTR(ss_bstr2_ptr);

                    return str1.Equals(str2);
                }
                finally {
                    if (ss_bstr1_ptr != IntPtr.Zero) {
                        Marshal.ZeroFreeBSTR(ss_bstr1_ptr);
                    }

                    if (ss_bstr2_ptr != IntPtr.Zero) {
                        Marshal.ZeroFreeBSTR(ss_bstr2_ptr);
                    }
                }

            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Performs a replace if the new, and old value are different. If new value is null, default value is consulted. If it is also null, this is what will be assigned.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value for variable.</param>
        /// <param name="defaultValue">Default value for use if newValue is null.</param>
        /// <returns>Operation result denoting whether value was updated.</returns>
        public static OperationResult ReplaceIfDifferent<T>(ref T oldValue, T newValue, T defaultValue = default(T)) {
            if (newValue == null)
                newValue = defaultValue;

            if ((oldValue == null && newValue == null) || (oldValue?.Equals(newValue) == true))
                return new OperationResult(false);

            oldValue = newValue;
            return new OperationResult(true);
        }

        /// <summary>
        /// Performs a replace of a property using the propertyName on the obj provided. If new value is null, default value is consulted. If it is also null, this is what will be assigned.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="obj">The object to use during replace.</param>
        /// <param name="propertyName">The property name to use for replace.</param>
        /// <param name="newValue">New value for variable.</param>
        /// <param name="defaultValue">Default value for use if newValue is null.</param>
        /// <returns>Operation result denoting whether value was updated.</returns>
        public static OperationResult ReplaceIfDifferent<T>(object obj, string propertyName, T newValue, T defaultValue = default(T)) {
            var propertyAcquisitionOperation = Get.Property(obj, propertyName);
            if (propertyAcquisitionOperation.HadErrors)
                return new OperationResult(propertyAcquisitionOperation.Exceptions);
            PropertyInfo prop = propertyAcquisitionOperation.Result.Item2;
            return ReplaceIfDifferent(obj, prop, newValue, defaultValue);
        }

        /// <summary>
        /// Performs a replace of a provided property on the obj provided. If new value is null, default value is consulted. If it is also null, this is what will be assigned.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="obj">The object to use during replace.</param>
        /// <param name="prop">The property name to use for replace.</param>
        /// <param name="newValue">New value for variable.</param>
        /// <param name="defaultValue">Default value for use if newValue is null.</param>
        /// <returns>Operation result denoting whether value was updated.</returns>
        public static OperationResult ReplaceIfDifferent<T>(object obj, PropertyInfo prop, T newValue, T defaultValue = default(T)) {
            if (prop.PropertyType != typeof(T)) {
                return new OperationResult(new[] { new ArgumentException($"The property {prop.Name} is of type {prop.PropertyType} which does not match {typeof(T)}.") });
            }

            var exceptions = new List<Exception>();
            if (prop.CanRead == false)
                exceptions.Add(new InvalidOperationException($"The property {prop.Name} cannot be read."));

            if (prop.CanWrite == false)
                exceptions.Add(new InvalidOperationException($"The property {prop.Name} cannot be written to."));

            if (exceptions.Any())
                return new OperationResult(exceptions);

            var oldValue = prop.GetValue(obj);
            if (newValue == null)
                newValue = defaultValue;

            if (oldValue?.Equals(newValue) == true)
                return new OperationResult(false);

            prop.SetValue(obj, newValue);
            return new OperationResult(true);
        }

        /// <summary>
        /// Slices array into pieces delimited by length.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="source">Source array.</param>
        /// <param name="maxResultElements">Max elements before slice.</param>
        /// <returns>Two dimensional sliced array.</returns>
        public static T[][] ArraySlice<T>(T[] source, int maxResultElements) {
            int numberOfArrays = source.Length / maxResultElements;
            if (maxResultElements * numberOfArrays < source.Length)
                numberOfArrays++;
            T[][] target = new T[numberOfArrays][];
            for (int index = 0; index < numberOfArrays; index++) {
                int elementsInThisArray = Math.Min(maxResultElements, source.Length - index * maxResultElements);
                target[index] = new T[elementsInThisArray];
                Array.Copy(source, index * maxResultElements, target[index], 0, elementsInThisArray);
            }
            return target;
        }

        /// <summary>
        /// Joins a collection of strings with a comma followed by a white space.
        /// </summary>
        /// <param name="input">Collection of strings to be joined.</param>
        /// <param name="addTrailingWhiteSpace">Whether to add white space after the comma or not. Defaults to true</param>
        /// <returns>Comma joined string.</returns>
        public static string JoinWithComma(IEnumerable<string> input, bool addTrailingWhiteSpace = true) =>
            string.Join($",{(addTrailingWhiteSpace ? " " : "") }", input);

        /// <summary>
        /// Uses to reflection to transfer the value of one property to another.
        /// </summary>
        /// <param name="source">Source object, and property name.</param>
        /// <param name="dest">Destination object, and property name.</param>
        /// <returns>Operation result.</returns>
        public static OperationResult PropertyAssignmentThroughReflection(KeyValuePair<object, string> source, KeyValuePair<object, string> dest) =>
            Get.OperationResult(() => {
                if (source.Key == null)
                    throw new ArgumentException("The source object for the transfer is null. Operation canceled.");
                if (dest.Key == null)
                    throw new ArgumentException("The destination object for the transfer is null. Operation canceled.");
                if (string.IsNullOrWhiteSpace(source.Value))
                    throw new ArgumentException("The property name for the source is blank. Operation canceled.");
                if (string.IsNullOrWhiteSpace(dest.Value))
                    throw new ArgumentException("The property name for the destination is blank. Operation canceled.");

                var sourcePropertyRetrievalOperation = Get.Property(source.Key, source.Value);
                var destPropertyRetrievalOperation = Get.Property(dest.Key, dest.Value);

                var exceptionList = new List<Exception>();
                exceptionList.AddRange(sourcePropertyRetrievalOperation.Exceptions);
                exceptionList.AddRange(destPropertyRetrievalOperation.Exceptions);
                if (exceptionList.Any())
                    return new OperationResult(exceptionList);

                Tuple<object, System.Reflection.PropertyInfo> getTuple = sourcePropertyRetrievalOperation.Result;
                Tuple<object, System.Reflection.PropertyInfo> setTuple = destPropertyRetrievalOperation.Result;

                object getObj = getTuple.Item1;
                object setObj = setTuple.Item1;
                System.Reflection.PropertyInfo getProperty = getTuple.Item2;
                System.Reflection.PropertyInfo setProperty = setTuple.Item2;

                if (setProperty.PropertyType != getProperty.PropertyType)
                    throw new ArgumentException("Types between properties are mismatched.");

                setProperty.SetValue(setObj, getProperty.GetValue(getObj, null));
                return new OperationResult();
            });

        /// <summary>
        /// Change base of number.
        /// </summary>
        /// <param name="number">Number for which base should be changed.</param>
        /// <param name="newBase">New base.</param>
        /// <param name="isBigEndian">Whether result should be bigendian for littleendian.</param>
        /// <returns>New number in list form.</returns>
        public static List<int> BaseChange(int number, int newBase, bool isBigEndian = false) =>
            Get.List<int>(digits => {

                Action<int> addDigit =
                    isBigEndian ?
                    new Action<int>(d => digits.Insert(0, d)) :
                    new Action<int>(d => digits.Add(d));

                while (number >= newBase) {
                    int digit = number % newBase;
                    addDigit(digit);
                    number = number / newBase;
                }
                addDigit(number);

            });

        /// <summary>
        /// Replaces strings that are null, and white space with an empty string.
        /// </summary>
        /// <param name="text">Strings to perform replace on.</param>
        /// <returns>Input with null, and whitespace strings replaced.</returns>
        public static string[] NullAndWhitespaceReplace(params string[] text) =>
            text?.Select(t => string.IsNullOrWhiteSpace(t) ? "" : t).ToArray() ?? new string[0];

        /// <summary>
        /// Clears collection, and then rehydrates with newItems using current SynchronizationContext.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <returns>Task to complete asynchronously.</returns>
        public static async Task CollectionClear<C>(ObservableCollection<C> collection) {
            if (collection == null)
                return;
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            await Task.Factory.StartNew(
                () => collection.ToArray().ForEach(item => collection.Remove(item)),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }

        /// <summary>
        /// Clears collection, and then rehydrates with newItems using current SynchronizationContext.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <typeparam name="I">The input type.</typeparam>
        /// <typeparam name="TKey">The order type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <param name="newItems">The new items for the collection.</param>
        /// <param name="selectFunction">The callback to transform each new item.</param>
        /// <param name="orderByFunction">The function used to order the collection</param>
        /// <param name="orderByDesc">If true, the collection will be ordered in a descending fashion.</param>
        /// <returns>Task to complete asynchronously.</returns>
        public static async Task CollectionRehydrationAsync<C, I, TKey>(ObservableCollection<C> collection, IEnumerable<I> newItems, Func<I, C> selectFunction, Func<C, TKey> orderByFunction, bool orderByDesc = false) {
            if (collection == null || newItems == null || selectFunction == null)
                return;

            await CollectionClear(collection);

            C[] newCollection = orderByDesc ?
                await Task.Run(() => newItems.Select(selectFunction).OrderByDescending(orderByFunction).ToArray()) :
                await Task.Run(() => newItems.Select(selectFunction).OrderBy(orderByFunction).ToArray());

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            await Task.Factory.StartNew(
                () => newCollection.ForEach(collection.Add),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }
        /// <summary>
        /// Clears collection, and then rehydrates with newItems using current SynchronizationContext.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <typeparam name="I">The input type.</typeparam>
        /// <typeparam name="TKey">The order type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <param name="newItems">The new items for the collection.</param>
        /// <param name="selectFunction">The callback to transform each new item.</param>
        /// <param name="orderByFunction">The function used to order the collection</param>
        /// <param name="orderByDesc">If true, the collection will be ordered in a descending fashion.</param>
        /// <returns>Task to complete asynchronously.</returns>
        public static void CollectionRehydration<C, I, TKey>(ObservableCollection<C> collection, IEnumerable<I> newItems, Func<I, C> selectFunction, Func<C, TKey> orderByFunction, bool orderByDesc = false) =>
            CollectionRehydrationAsync(collection, newItems, selectFunction, orderByFunction, orderByDesc).GetAwaiter().GetResult();


        /// <summary>
        /// Clears collection, and then rehydrates with newItems using current SynchronizationContext.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <param name="newItems">The new items for the collection.</param>
        /// <returns>Task to complete asynchronously.</returns>
        public static async Task CollectionRehydrationAsync<C>(ObservableCollection<C> collection, IEnumerable<C> newItems) {
            if (collection == null || newItems == null)
                return;
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            await CollectionClear(collection);
            await Task.Factory.StartNew(
                () => newItems.ForEach(collection.Add),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }

        /// <summary>
        /// Clears collection, and then rehydrates with newItems using current SynchronizationContext.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <typeparam name="I">The input type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <param name="newItems">The new items for the collection.</param>
        /// <param name="selectFunction">The callback to transform each new item.</param>
        /// <returns>Task to complete asynchronously.</returns>
        public static async Task CollectionRehydrationAsync<C, I>(ObservableCollection<C> collection, IEnumerable<I> newItems, Func<I, C> selectFunction) {
            if (collection == null || newItems == null || selectFunction == null)
                return;

            await CollectionClear(collection);

            C[] newCollection =
                await Task.Run(() => newItems.Select(selectFunction).ToArray());

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            await Task.Factory.StartNew(
                () => newCollection.ForEach(collection.Add),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }
        /// <summary>
        /// Clears collection, and then rehydrates with newItems.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <param name="newItems">The new items for the collection.</param>
        public static void CollectionRehydration<C>(ObservableCollection<C> collection, IEnumerable<C> newItems) =>
            CollectionRehydrationAsync(collection, newItems).GetAwaiter().GetResult();

        /// <summary>
        /// Clears collection, and then rehydrates with newItems.
        /// </summary>
        /// <typeparam name="C">The collection type.</typeparam>
        /// <typeparam name="I">The input type.</typeparam>
        /// <param name="collection">The collection for which the rehydration will be performed.</param>
        /// <param name="newItems">The new items for the collection.</param>
        /// <param name="selectFunction">The callback to transform each new item.</param>
        public static void CollectionRehydration<C, I>(ObservableCollection<C> collection, IEnumerable<I> newItems, Func<I, C> selectFunction) =>
            CollectionRehydrationAsync(collection, newItems, selectFunction).GetAwaiter().GetResult();

        /// <summary>
        /// Invokes an async method.
        /// </summary>
        /// <param name="task">The async method to call.</param>
        /// <param name="useCurrentSynchronization">Whether the current synchronization context should be used.</param>
        /// <returns>An operation result containing a task whose result is a task that represents the async call. The nested inner Task should only be awaited. The outer Task can have Wait called on it to be synchronous.</returns>
        /// <exception>If the current synchronization context is null, and has been elected to be used, </exception>
        public static OperationResult<Task<Task>> InvocationOfAsyncMethod(Func<Task> task, bool useCurrentSynchronization = true) =>
            Get.OperationResult(() => {
                TaskScheduler scheduler = TaskScheduler.Default;

                if (useCurrentSynchronization) {
                    if (SynchronizationContext.Current == null)
                        throw new NullSynchronizationException();
                    scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                }

                return
                    Task.Factory.StartNew(
                        async () => await task(),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        scheduler
                    );
            });

        /// <summary>
        /// Invokes an async method.
        /// </summary>
        /// <typeparam name="T">The return type of the Task</typeparam>
        /// <param name="task">The async method to call.</param>
        /// <param name="useCurrentSynchronization">Whether the current synchronization context should be used.</param>
        /// <returns>An operation result containing a task whose result is a task that represents the async call. The nested inner Task should never have Wait called on it, and should only be awaited. The outer Task can have Wait called on it to be synchronous.</returns>
        public static OperationResult<Task<Task<T>>> InvocationOfAsyncMethod<T>(Func<Task<T>> task, bool useCurrentSynchronization = true) =>
            Get.OperationResult(() => {
                TaskScheduler scheduler = TaskScheduler.Default;

                if (useCurrentSynchronization) {
                    if (SynchronizationContext.Current == null)
                        throw new NullSynchronizationException();
                    scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                }

                return
                    Task.Factory.StartNew(
                        async () => await task(),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        scheduler
                    );
            });

        /// <summary>
        /// Used to scrub text.
        /// </summary>
        /// <param name="text">Text to scrub.</param>
        /// <param name="scrubCharacters">Characters to be removed by scrub.</param>
        /// <returns>Scrubbed text.</returns>
        public static string[] TextScrub(IEnumerable<string> text, char[] scrubCharacters) {
            string[] textArray = NullAndWhitespaceReplace(text?.ToArray());
            return scrubCharacters == null ?
                textArray :
                textArray
                    .Select(
                        t =>
                            scrubCharacters
                                .Aggregate(t, (prev, next) => prev.Replace(next.ToString(), ""))
                    ).ToArray();
        }

        /// <summary>
        /// Used to add a <see cref="TimeSpan"/> to a <see cref="DateTime"/> .
        /// </summary>
        /// <param name="date">The starting <see cref="DateTime"/> .</param>
        /// <param name="time">The <see cref="TimeSpan"/> to add.</param>
        /// <returns></returns>
        public static DateTime? DateTimeAndTimeSpanAddition(DateTime? date, TimeSpan? time) =>
            date.HasValue && time.HasValue ?
                date.Value.Add(time.Value) :
                date;

        /// <summary>
        /// Checks to see if a directory exists. If it doesn't, it will attempt to create it. 
        /// </summary>
        /// <param name="directoryAddress">The directory to check.</param>
        /// <returns>An <see cref="OperationResult"/> detailing success of directory creation.</returns>
        public static OperationResult DirectoryCreation(string directoryAddress) =>
            Get.OperationResult(() => {
                var directory = new DirectoryInfo(directoryAddress);
                if (directory.Exists == false)
                    directory.Create();
            });

        public static Task<string> QueryStreamAsync(Stream stream, string query, int queryDelay = 100, string terminator = "\r\n", int bufferSize = 4096, int readDelay = 50) =>
            Task.Run(async () => {
                byte[] rawQuery = Encoding.ASCII.GetBytes(query + terminator);

                // Send stream request.
                stream.Write(rawQuery, 0, rawQuery.Length);

                // Introduce delay.
                await Task.Delay(queryDelay);

                return await ReadStreamAsync(stream, bufferSize, readDelay);
            });

        public static Task<string> ReadStreamAsync(Stream stream, int bufferSize = 4096, int readDelay = 50) =>
            Task.Run(async () => {
                // Get stream response.
                string response = "";

                byte[] readBuffer = new byte[bufferSize];
                do {
                    int responseLength = stream.Read(readBuffer, 0, readBuffer.Length);
                    response += Encoding.ASCII.GetString(readBuffer, 0, responseLength);
                    await Task.Delay(readDelay);
                }
                while ((stream as NetworkStream)?.DataAvailable ?? false);

                return response;
            });

        public static void ExceptionTypeAndMethodRecord(OperationResult result, Type declaringType, string methodName) =>
            ExceptionTypeAndMethodRecord(new[] { result }, declaringType, methodName);
        public static void ExceptionTypeAndMethodRecord(IEnumerable<OperationResult> results, Type declaringType, string methodName) {
            if ((results?.Any() ?? false) == false)
                return;
            results.ForEach(result => result.Exceptions.ForEach(ex => {
                if (declaringType != null)
                    ex.Data.Add("Faulted.Type", declaringType.AssemblyQualifiedName);
                if (methodName.IsMeaningful())
                    ex.Data.Add("Faulted.Method", methodName);
                ex.Data.Add("Faulted.Time", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }));
        }

        /// <summary>
        /// Checks to see if <typeparamref name="T"/> contains all property names contained in <paramref name="propertyNames"/>.
        /// </summary>
        /// <typeparam name="T">The type to check properties against.</typeparam>
        /// <param name="propertyNames">The collection of property names to check.</param>
        /// <returns><see cref="OperationResult"/>. If all property names are not contained in <typeref name="T"/>, then <see cref="OperationResult"/> contains <see cref="ArgumentException"/> denoting missing properties.</returns>
        public static OperationResult PropertyCheck<T>(IEnumerable<string> propertyNames) =>
            PropertyCheck(typeof(T), propertyNames);

        /// <summary>
        /// Checks to see if <paramref name="type"/> contains all property names contained in <paramref name="propertyNames"/>.
        /// </summary>
        /// <param name="type">The type to check properties against.</param>
        /// <param name="propertyNames">The collection of property names to check.</param>
        /// <returns><see cref="OperationResult"/>. If all property names are not contained in <paramref name="type"/>, then <see cref="OperationResult"/> contains <see cref="ArgumentException"/> denoting missing properties.</returns>
        public static OperationResult PropertyCheck(Type type, IEnumerable<string> propertyNames) =>
            Get.OperationResult(() => {
                if (type == null)
                    throw new ArgumentNullException($"{nameof(type)} cannot be null.");

                var typeFields = type.GetProperties().Select(p => p.Name).ToArray();
                var argumentList =
                new List<string>(propertyNames.Where(c => typeFields.Contains(c) == false));

                if (argumentList.Any()) {
                    var ex = new ArgumentException("Some requested properties are not contained by the type.");
                    ex.Data.Add("Checked Type", type.AssemblyQualifiedName);
                    argumentList.ForEach(a => ex.Data.Add("Missing Property Name", a));
                    throw ex;
                }
            });

    }
}