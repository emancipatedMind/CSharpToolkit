namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    // When a binding allows null as a valid value, and the field is strongly typed, the validation in the ViewModel never gets called upon a conversion error. For example, the binding attempts
    // to convert 'a' into a int?. This results in a null, but since null is allowed, validation in the ViewModel is satisfied, but the form will report an error
    // saying that 'a' cannot be converted, and the ErrorTemplate would be activated. Essentially, this means a form with an error could be submitted resulting in possible undesired behavior.
    // This bit of code allows the consumer to explicitly record an error in the ViewModel if the conversion to the strongly typed field fails. This way, the ViewModel may proceed using normal
    // operation disallowals until error is fixed. The ViewModel must implement the CSharpToolkit.XAML.Abstractions.IExplicitErrorAdder, and the binding should have ValidatesOnDataErrors marked
    // as true.
    public class ExplicitErrorAddingBehavior {

        public static void SetExplicitBindingValidation(DependencyObject obj, DependencyProperty value) =>
            obj.SetValue(ExplicitBindingValidationProperty, value);

        // Using a DependencyProperty as the backing store for ExplicitBindingValidation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExplicitBindingValidationProperty =
            DependencyProperty.RegisterAttached("ExplicitBindingValidation", typeof(DependencyProperty), typeof(ExplicitErrorAddingBehavior), new PropertyMetadata(null, ExplicitBindingValidation_PropertyChanged));

        private static void ExplicitBindingValidation_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var element = d as FrameworkElement;
            var property = e.NewValue as DependencyProperty;
            if (element == null || property == null)
                return;

            if (element.IsLoaded)
                SetUp(element, property);
            else
                new Environment(element, property);
        }

        // Just a small environment to house state if element isn't completely laid out. 
        class Environment {
            FrameworkElement _element;
            DependencyProperty _property;
            public Environment(FrameworkElement element, DependencyProperty property) {
                _element = element;
                _property = property;
                _element.Loaded += Elemend_Loaded;
            }
            private void Elemend_Loaded(object sender, RoutedEventArgs e) {
                _element.Loaded -= Elemend_Loaded;
                SetUp(_element, _property);
                _element = null;
                _property = null;
            }
        }

        private static void SetUp(FrameworkElement element, DependencyProperty property) {
            BindingExpression bindingExpression = element.GetBindingExpression(property);
            Binding binding = bindingExpression.ParentBinding;
            binding.UpdateSourceExceptionFilter = DateValidationHandler;
            bindingExpression.UpdateSource();
        }

        private static object DateValidationHandler(object bindExpression, Exception exception) {
            var bindEx = (BindingExpression)bindExpression;
            var explicitErrorAdder = bindEx.ResolvedSource as Abstractions.IExplicitErrorAdder;
            var propertyName = bindEx.ResolvedSourcePropertyName;
            explicitErrorAdder?.AddError(propertyName, exception);
            return exception;
        }

    }
}