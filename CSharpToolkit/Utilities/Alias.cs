namespace CSharpToolkit.Utilities {
    using Abstractions;
    using System;
    using System.Collections.Generic;
    public class Alias : IAlias {

        public Alias(string name, Tuple<string, string> alias) {
            Name = name;
            Data = alias;
        }

        public Alias(string name, KeyValuePair<string, string> alias) {
            Name = name;
            Data = Tuple.Create(alias.Key, alias.Value);
        }

        public string Name { get; }
        public Tuple<string, string> Data { get; }

        public static Alias Create(string name, string dataItem1, string dataItem2) =>
            new Alias(name, new Tuple<string, string>(dataItem1, dataItem2));

    }
}