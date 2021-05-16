namespace CSharpToolkit.DataAccess {
    using System;
    public class PropertyModification {

        public PropertyModification(string propertyName, Type type, object oldValue, object newValue) {
            PropertyName = propertyName;
            PropertyType = type;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public Type PropertyType { get; }
        public string PropertyName { get; }
        public object NewValue { get; }
        public object OldValue { get; }

    }
}