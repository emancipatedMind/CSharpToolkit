namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Timers;
    public class SelectorBehaviors {

        // -------------------- Reset Index After Timeout Attached Behavior. --------------------

        public static object GetCommandOnTimeoutResetParameter(DependencyObject obj) => obj.GetValue(CommandOnTimeoutResetParameterProperty);
        public static void SetCommandOnTimeoutResetParameter(DependencyObject obj, object value) => obj.SetValue(CommandOnTimeoutResetParameterProperty, value);

        // Using a DependencyProperty as the backing store for CommandOnTimeoutResetParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandOnTimeoutResetParameterProperty =
            DependencyProperty.RegisterAttached("CommandOnTimeoutResetParameter", typeof(object), typeof(SelectorBehaviors), new PropertyMetadata(null));

        public static ICommand GetCommandOnTimeoutReset(DependencyObject obj) => (ICommand)obj.GetValue(CommandOnTimeoutResetProperty);
        public static void SetCommandOnTimeoutReset(DependencyObject obj, ICommand value) => obj.SetValue(CommandOnTimeoutResetProperty, value);
        public static readonly DependencyProperty CommandOnTimeoutResetProperty =
            DependencyProperty.RegisterAttached("CommandOnTimeoutReset", typeof(ICommand), typeof(SelectorBehaviors), new PropertyMetadata(null));

        public static double GetResetIndexTimeout(DependencyObject obj) => (double)obj.GetValue(ResetIndexTimeoutProperty);
        public static void SetResetIndexTimeout(DependencyObject obj, double value) => obj.SetValue(ResetIndexTimeoutProperty, value);
        public static readonly DependencyProperty ResetIndexTimeoutProperty =
            DependencyProperty.RegisterAttached("ResetIndexTimeout", typeof(double), typeof(SelectorBehaviors), new PropertyMetadata(1000.0));

        public static int? GetResetIndexAfterTimeout(DependencyObject obj) => (int?)obj.GetValue(ResetIndexAfterTimeoutProperty);
        public static void SetResetIndexAfterTimeout(DependencyObject obj, int? value) => obj.SetValue(ResetIndexAfterTimeoutProperty, value);
        public static readonly DependencyProperty ResetIndexAfterTimeoutProperty =
            DependencyProperty.RegisterAttached("ResetIndexAfterTimeout", typeof(int?), typeof(SelectorBehaviors), new PropertyMetadata(null, ResetIndexAfterTimeout_PropertyChanged));

        private static void ResetIndexAfterTimeout_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Selector == false)
                return;
            var element = (Selector)d;
            if (e.NewValue != null) {
                if (e.OldValue == null) {
                    var environment = GetResetIndexEnvironment(element);
                    if (environment != null) {
                        environment.Dispose();
                    }
                    SetResetIndexEnvironment(element, new ResetIndexEnvironment(element));
                }
            }
            else {
                var environment = GetResetIndexEnvironment(element);
                if (environment != null) {
                    environment.Dispose();
                }
                SetResetIndexEnvironment(element, null);
            }
        }

        static ResetIndexEnvironment GetResetIndexEnvironment(DependencyObject obj) => (ResetIndexEnvironment)obj.GetValue(ResetIndexEnvironmentProperty);
        static void SetResetIndexEnvironment(DependencyObject obj, ResetIndexEnvironment value) => obj.SetValue(ResetIndexEnvironmentProperty, value);
        static readonly DependencyProperty ResetIndexEnvironmentProperty = DependencyProperty.RegisterAttached("ResetIndexEnvironment", typeof(ResetIndexEnvironment), typeof(SelectorBehaviors), new PropertyMetadata(null));

        // -------------------- END - Reset Index After Timeout Attached Behavior. --------------------

        public static int? GetResetIndexAfterFocusLost(DependencyObject obj) => (int?)obj.GetValue(ResetIndexAfterFocusLostProperty);
        public static void SetResetIndexAfterFocusLost(DependencyObject obj, int? value) => obj.SetValue(ResetIndexAfterFocusLostProperty, value);
        public static readonly DependencyProperty ResetIndexAfterFocusLostProperty =
            DependencyProperty.RegisterAttached("ResetIndexAfterFocusLost", typeof(int?), typeof(SelectorBehaviors), new PropertyMetadata(null, ResetIndexAfterFocusLost_PropertyChanged));

        private static void ResetIndexAfterFocusLost_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Selector == false)
                return;

            var element = (Selector)d;

            if (e.NewValue != null) {
                if (e.OldValue == null)
                    element.LostFocus += ResetIndexAfterFocusLost_LostFocus;
            }
            else
                element.LostFocus -= ResetIndexAfterFocusLost_LostFocus;
        }

        private static void ResetIndexAfterFocusLost_LostFocus(object sender, RoutedEventArgs e) {
            if (sender is Selector == false)
                return;

            var element = (Selector)sender;

            int? index = GetResetIndexAfterFocusLost(element);
            if (index.HasValue == false)
                return;

            element.Dispatcher.BeginInvoke(new Action(() => element.SelectedIndex = index.Value < 0 ? 0 : index.Value));
        }

        public static int? GetNullSelectorIndex(Selector obj) =>
            (int?)obj.GetValue(NullSelectorIndexProperty);

        public static void SetNullSelectorIndex(Selector obj, int? value) =>
            obj.SetValue(NullSelectorIndexProperty, value);

        public static readonly DependencyProperty NullSelectorIndexProperty =
            DependencyProperty.RegisterAttached("NullSelectorIndex", typeof(int?), typeof(SelectorBehaviors), new PropertyMetadata(null, NullSelectorIndex_PropertyChanged));

        private static void NullSelectorIndex_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Selector == false)
                return;

            var element = (Selector)d;

            if (e.NewValue != null) {
                if (e.OldValue == null)
                    element.LostFocus += NullSelectorIndex_LostFocus;
            }
            else
                element.LostFocus -= NullSelectorIndex_LostFocus;
        }

        private static void NullSelectorIndex_LostFocus(object sender, RoutedEventArgs e) {
            if (sender is Selector == false)
                return;

            var element = (Selector)sender;

            if (element.SelectedItem != null)
                return;

            int? index = GetNullSelectorIndex(element);
            if (index.HasValue == false)
                return;

            element.SelectedIndex = index.Value < 0 ? 0 : index.Value;
        }

        public static IList GetSynchronizedSelectedItems(Selector obj) =>
            (IList)obj.GetValue(SynchronizedSelectedItemsProperty);

        public static void SetSynchronizedSelectedItems(Selector obj, IList value) =>
            obj.SetValue(SynchronizedSelectedItemsProperty, value);

        // Using a DependencyProperty as the backing store for SynchronizedSelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SynchronizedSelectedItemsProperty =
            DependencyProperty.RegisterAttached("SynchronizedSelectedItems", typeof(IList), typeof(SelectorBehaviors), new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

        private static void OnSynchronizedSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != null) {
                SynchronizationManager synchronizer = GetSynchronizationManager(d);
                synchronizer.StopSynchronizing();
                SetSynchronizationManager(d, null);
            }

            var list = e.NewValue as IList;
            var selector = d as Selector;

            if (list != null && selector != null) {
                SynchronizationManager synchronizer = GetSynchronizationManager(d);
                if (synchronizer == null) {
                    synchronizer = new SynchronizationManager(selector);
                    SetSynchronizationManager(selector, synchronizer);
                }
                synchronizer.StartSynchronizing();
            }
        }

        private static SynchronizationManager GetSynchronizationManager(DependencyObject obj) =>
            (SynchronizationManager)obj.GetValue(SynchronizationManagerProperty);

        private static void SetSynchronizationManager(DependencyObject obj, SynchronizationManager value) =>
            obj.SetValue(SynchronizationManagerProperty, value);

        // Using a DependencyProperty as the backing store for SynchronizationManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SynchronizationManagerProperty =
            DependencyProperty.RegisterAttached("SynchronizationManager", typeof(SynchronizationManager), typeof(SelectorBehaviors), new PropertyMetadata(null));


        private class SynchronizationManager {

            readonly Selector _selector;
            TwoListSynchronizer _synchronizer;

            public SynchronizationManager(Selector selector) {
                _selector = selector;
            }

            public void StopSynchronizing() =>
                _synchronizer.StopSynchronizing();

            public void StartSynchronizing() {
                IList list = GetSynchronizedSelectedItems(_selector);

                if (list == null)
                    return;

                _synchronizer = new TwoListSynchronizer(GetSelectedItemsCollection(_selector), list);
                _synchronizer.StartSynchronizing();
            }

            static IList GetSelectedItemsCollection(Selector selector) {
                if (selector is MultiSelector)
                    return ((MultiSelector)selector).SelectedItems;
                if (selector is ListBox)
                    return ((ListBox)selector).SelectedItems;
                throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
            }
        }

        class ResetIndexEnvironment : IDisposable {

            bool _disposed;
            Selector _selector;
            Timer _timer;

            public ResetIndexEnvironment(Selector selector) : this(selector, new Timer()) { }

            public ResetIndexEnvironment(Selector selector, Timer timer) {
                _timer = timer;
                _selector = selector;
                _timer.AutoReset = false;
                _timer.Elapsed += Timer_Elapsed;
                _selector.SelectionChanged += Selector_SelectionChanged;
            }

            public void Dispose() {
                if (_disposed)
                    return;
                _disposed = true;

                _timer.Elapsed -= Timer_Elapsed;
                _selector.SelectionChanged -= Selector_SelectionChanged;
                _timer = null;
                _selector = null;
            }

            private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
                _timer.Stop();
                int? index = GetResetIndexAfterTimeout(_selector);
                if (index.HasValue == false || index == _selector.SelectedIndex)
                    return;
                _timer.Interval = GetResetIndexTimeout(_selector);
                _timer.Start();
            }

            private void Timer_Elapsed(object sender, ElapsedEventArgs e) =>
                _selector.Dispatcher.BeginInvoke(new Action(() => {

                    int? index = GetResetIndexAfterTimeout(_selector);
                    if (index.HasValue == false || index == _selector.SelectedIndex)
                        return;

                    ICommand command = GetCommandOnTimeoutReset(_selector);
                    if (command != null)
                        command.Execute(GetCommandOnTimeoutResetParameter(_selector) ?? new object());

                    _selector.SelectedIndex = index.Value < 0 ? 0 : index.Value;
                }));

        }
    }
}