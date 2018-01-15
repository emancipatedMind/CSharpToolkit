namespace CSharpToolkit.Utilities {
    using System.Collections.Generic;
    using Abstractions;
    public class Alias : IAlias {

        public Alias(string name, KeyValuePair<string, string> alias) {
            Name = name;
            Data = alias;
        }

        public string Name { get; }
        public KeyValuePair<string, string> Data { get; }

    }
}