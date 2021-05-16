namespace CSharpToolkit.Views {
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public class FindViewBase : Control {

        public object SearchCriteria { get; set; }

        public object BelowGridArea {
            get { return GetValue(BelowGridAreaProperty); }
            set { SetValue(BelowGridAreaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BelowGridArea.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BelowGridAreaProperty =
            DependencyProperty.Register("BelowGridArea", typeof(object), typeof(FindViewBase), new PropertyMetadata(null));

        public object Buttons {
            get { return GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Buttons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register(nameof(Buttons), typeof(object), typeof(FindViewBase), new PropertyMetadata(null));

    }
}
