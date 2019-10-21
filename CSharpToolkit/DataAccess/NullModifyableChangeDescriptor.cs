namespace CSharpToolkit.DataAccess {
    using Abstractions;
    using System;
    using System.Collections.Generic;
    public class NullModifyableChangeDescriptor : IModifyableChangeDescriptor {
        static NullModifyableChangeDescriptor _instance;
        public static NullModifyableChangeDescriptor Instance => _instance ?? (_instance = new NullModifyableChangeDescriptor());
        private NullModifyableChangeDescriptor() { }
        bool IModifyable.Modified => false;
        List<PropertyModification> IChangeDescriptor.GetChangedProperties() => new List<PropertyModification>();
        void IModifyable.Reset() { }
        bool IModifyable.Reset(string name) => false;
        void IModifyable.Save() { }
    }
}