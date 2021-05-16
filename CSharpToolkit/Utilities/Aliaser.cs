namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    /// <summary>
    /// Class that provides aliasing features.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class Aliaser : IDisposable {

        bool _disposed;
        List<Tuple<string, string>> _combinedAliases = new List<Tuple<string, string>>();

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">A collection of aliases to add.</param>
        public void AddAlias(IEnumerable<Tuple<string, string>> aliases) =>
            _AddAlias(aliases);

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">A collection of aliases to add.</param>
        public void AddAlias(IEnumerable<KeyValuePair<string, string>> aliases) =>
            _AddAlias(aliases.Select(a => Tuple.Create(a.Key, a.Value)));

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to add.</param>
        public void AddAlias(KeyValuePair<string, string> alias) =>
            _AddAlias(new[] { Tuple.Create(alias.Key, alias.Value) });

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to use.</param>
        public void AddAlias(Tuple<string, string> alias) =>
            _AddAlias(new[] { alias });

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="column">Column name alias will reference.</param>
        /// <param name="alias">Alias to use.</param>
        public void AddAlias(string column, string alias) =>
            _AddAlias(new[] { Tuple.Create(column, alias) });

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="alias">Alias to add.</param>
        public void AddAlias(Abstractions.IAlias alias) =>
            _AddAlias(new[] { alias.Data });

        /// <summary>
        /// Add alias that can be used to access data.
        /// </summary>
        /// <param name="aliases">Alias collection to add.</param>
        public void AddAlias(IEnumerable<Abstractions.IAlias> aliases) =>
            _AddAlias(aliases.Select(a => a.Data));

        /// <summary>
        /// Provides a mechanism to look up alias using name. If name provided has no alias, the name is returned.
        /// </summary>
        /// <param name="name">The name to use to find alias.</param>
        /// <returns>Name found. If no alias found, returns name provided.</returns>
        public string LookUpAlias(string name) {
            string alias = _combinedAliases.FirstOrDefault(a => a.Item1 == name)?.Item2;
            return string.IsNullOrWhiteSpace(alias) ? name : alias; 
        }

        /// <summary>
        /// Provides a mechanism to look up name using alias. If alias provided has no name, the alias is returned.
        /// </summary>
        /// <param name="alias">The alias to use to find name.</param>
        /// <returns>Name found. If no name found, returns alias provided.</returns>
        public string LookUpName(string alias) {
            string name = _combinedAliases.FirstOrDefault(a => a.Item2 == alias)?.Item1;
            return string.IsNullOrWhiteSpace(name) ? alias : name; 
        }

        /// <summary>
        /// Clears all aliases.
        /// </summary>
        public void Clear() =>
            _combinedAliases.Clear();

        private void _AddAlias(IEnumerable<Tuple<string, string>> aliases) {
            var newCollection = _combinedAliases.Concat(aliases.Where(a => string.IsNullOrWhiteSpace(a.Item2) == false)).ToArray();

            int nameRepeatCount = newCollection.GroupBy(tuple => tuple.Item1).Count();
            int aliasRepeatCount = newCollection.GroupBy(tuple => tuple.Item2).Count();

            if (nameRepeatCount != newCollection.Length)
                throw new ArgumentException("Repeats of names are not allowed.");
            if (aliasRepeatCount != newCollection.Length)
                throw new ArgumentException("Repeats of aliases are not allowed.");

            _combinedAliases.AddRange(aliases);
        }

        protected void Dispose(bool disposing) {
            if (!_disposed) {
                _combinedAliases.Clear();
            }
            _disposed = true;
        } 

        public void Dispose() => Dispose(true);


    }
}